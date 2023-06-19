using System;
using System.Collections.Generic;
using System.IO;
using WolvenKit.Common.Model.Arguments;
using WolvenKit.Core.Interfaces;
using WolvenKit.Modkit.Managers;
using WolvenKit.Modkit.RED4.Tools;
using WolvenKit.RED4.Archive.CR2W;
using WolvenKit.RED4.Types;

namespace WolvenKit.Modkit.RedConverters;

public class MeshExporter : BaseExporter
{
    private MeshExportArgs _meshExportArgs => _globalExportArgs.Get<MeshExportArgs>();

    public MeshExporter(ConvertManager convertManager, ILoggerService loggerService) : base(convertManager, loggerService)
    {
    }

    public override ExportArgs GetExportArgs() => new MeshExportArgs();

    protected override bool Export()
    {
        switch (_meshExportArgs.MeshExporter)
        {
            case MeshExporterType.Default:
                return DefaultExport();
            case MeshExporterType.Experimental:
                break;
            case MeshExporterType.REDmod:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
    }

    private bool DefaultExport()
    {
        switch (_meshExportArgs.meshExportType)
        {
            case MeshExportType.MeshOnly:
                return DefaultExportMeshOnly();
            case MeshExportType.WithRig:
                break;
            case MeshExportType.Multimesh:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
    }

    private bool DefaultExportMeshOnly()
    {
        if (_resource is not CMesh { RenderResourceBlob.Chunk: rendRenderMeshBlob rendblob } mesh)
        {
            return false;
        }

        var meshesInfo = MeshTools.GetMeshesinfo(rendblob, mesh);

        if (_meshExportArgs.withMaterials)
        {
            if (_meshExportArgs.MaterialRepo is null)
            {
                _loggerService.Error("Depot path is not set: Choose a Depot location within Settings for generating materials.");
                return false;
            }
            GetMaterials();
            // ParseMaterials(cr2w, meshStream, outfile, archives, args.MaterialRepo, meshesinfo, eUncookExtension);
        }

        return true;
    }

    private void GetMaterials()
    {
        if (_resource is not CMesh mesh)
        {
            throw new ArgumentException(nameof(_resource));
        }
        
        var exported = new HashSet<ulong>();

        var externalMaterials = new List<IMaterial>();
        var localMaterials = new List<IMaterial>();

        foreach (var entry in mesh.ExternalMaterials)
        {
            var result = _convertManager.GetGameFile(entry);
            if (result == null)
            {
                break;
            }

            externalMaterials.Add(result.RootChunk);

            if (result.File != null)
            {
                foreach (var import in result.File.Info.GetImports())
                {
                    if (exported.Add(import.DepotPath))
                    {
                        ExtractAsset(import);
                    }
                }
            }
        }

        foreach (var entry in mesh.PreloadExternalMaterials)
        {
            var result = _convertManager.GetGameFile(entry);
            if (result == null)
            {
                break;
            }

            externalMaterials.Add(result.RootChunk);

            if (result.File != null)
            {
                foreach (var import in result.File.Info.GetImports())
                {
                    if (exported.Add(import.DepotPath))
                    {
                        ExtractAsset(import);
                    }
                }
            }
        }

        if (mesh.LocalMaterialBuffer.Materials != null)
        {
            foreach (var material in mesh.LocalMaterialBuffer.Materials)
            {
                localMaterials.Add(material);
            }

            if (_file != null)
            {
                foreach (var import in _file.Info.GetImports())
                {
                    if (exported.Add(import.DepotPath))
                    {
                        ExtractAsset(import);
                    }
                }
            }
        }
        else
        {
            foreach (var handle in mesh.PreloadLocalMaterialInstances)
            {
                if (handle.Chunk is not { } material)
                {
                    continue;
                }

                localMaterials.Add(material);
            }

            if (_file != null)
            {
                foreach (var import in _file.Info.GetImports())
                {
                    if (exported.Add(import.DepotPath))
                    {
                        ExtractAsset(import);
                    }
                }
            }
        }
    }

    private IGameFile? GetGameFile(ResourcePath path, IGameArchive archive)
    {
        foreach (var (hash, gameFile) in archive.Files)
        {
            if (hash == path)
            {
                return gameFile;
            }
        }

        return null;
    }

    private void ExtractAsset(CR2WImport import)
    {
        IGameFile? gameFile = null;
        if (import.Flags.HasFlag(InternalEnums.EImportFlags.Embedded))
        {
            if (_file == null)
            {
                _loggerService.Error("");
                return;
            }

            foreach (var embeddedFile in _file.EmbeddedFiles)
            {
                if (embeddedFile.FileName == import.DepotPath)
                {
                    _convertManager.ExportEmbedded(embeddedFile, _globalExportArgs);
                    return;
                }
            }
        }

        if (import.Flags.HasFlag(InternalEnums.EImportFlags.Soft)) // Load from any archive?
        {
            foreach (var archive in _convertManager.Archives!)
            {
                gameFile = GetGameFile(import.DepotPath, archive);
                if (gameFile != null)
                {
                    _convertManager.Export(gameFile, _globalExportArgs);
                    return;
                }
            }
        }

        if (import.Flags.HasFlag(InternalEnums.EImportFlags.Default)) // Load from same archive?
        {
            if (_gameFile != null)
            {
                gameFile = GetGameFile(import.DepotPath, _gameFile.GetArchive());
                if (gameFile != null)
                {
                    _convertManager.Export(gameFile, _globalExportArgs);
                    return;
                }
            }

            if (gameFile == null) // Fallback
            {
                foreach (var archive in _convertManager.Archives!)
                {
                    gameFile = GetGameFile(import.DepotPath, archive);
                    if (gameFile != null)
                    {
                        _convertManager.Export(gameFile, _globalExportArgs);
                        return;
                    }
                }
            }
        }
    }
}