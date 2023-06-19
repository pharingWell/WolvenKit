using System;
using System.Collections.Generic;
using System.IO;
using WolvenKit.Common.Model.Arguments;
using WolvenKit.Core.Interfaces;
using WolvenKit.Modkit.Interfaces;
using WolvenKit.Modkit.RedConverters;
using WolvenKit.RED4.Archive;
using WolvenKit.RED4.Archive.CR2W;
using WolvenKit.RED4.Archive.IO;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.Types;

namespace WolvenKit.Modkit.Managers;

public class ConvertManager : IConvertManager
{
    private readonly ILoggerService _loggerService;

    private readonly Dictionary<ERedExtension, Func<BaseExporter>> _exporterDictionary = new();

    public List<IGameArchive>? Archives { get; set; }
    public string? DepotPath { get; set; }

    public ConvertManager(ILoggerService loggerService)
    {
        _loggerService = loggerService;

        Initialize();
    }

    private void Initialize()
    {
        _exporterDictionary.Add(ERedExtension.mesh, () => new MeshExporter(this, _loggerService));
        _exporterDictionary.Add(ERedExtension.xbm, () => new XbmExporter(this, _loggerService));
    }

    private Func<BaseExporter>? GetExporter(CResource resource)
    {
        var fileTypes = FileTypeHelper.GetRedExtension(resource);
        foreach (var fileType in fileTypes)
        {
            if (_exporterDictionary.TryGetValue(fileType, out var exporterFactory))
            {
                return exporterFactory;
            }
        }
        return null;
    }

    public Stream? Extract(ResourcePath path, List<IGameArchive>? archives = null)
    {
        archives ??= Archives;
        if (archives == null)
        {
            return null;
        }

        foreach (var archive in archives)
        {
            foreach (var (hash, gameFile) in archive.Files)
            {
                if (hash == path)
                {
                    using var ms = new MemoryStream();
                    gameFile.Extract(ms);
                    ms.Position = 0;

                    return ms;
                }
            }
        }

        return null;
    }

    public Stream? Extract(ResourcePath path, CR2WFile file)
    {
        foreach (var embeddedFile in file.EmbeddedFiles)
        {
            if (embeddedFile.FileName == path)
            {
                using var ms = new MemoryStream();
                using var writer = new CR2WWriter(ms);

                writer.WriteFile(new CR2WFile { RootChunk = embeddedFile.Content });

                return ms;
            }
        }

        return null;
    }

    public void Export(IGameFile gameFile, GlobalExportArgs globalExportArgs)
    {
        if (!Enum.TryParse<ERedExtension>(gameFile.Extension[1..], true, out var redExtension))
        {
            return;
        }

        if (!_exporterDictionary.TryGetValue(redExtension, out var factory))
        {
            return;
        }

        factory().Export(gameFile, globalExportArgs);
    }

    public void ExportEmbedded(ICR2WEmbeddedFile embeddedFile, GlobalExportArgs globalExportArgs)
    {
        var factory = GetExporter(embeddedFile.Content);
        if (factory == null)
        {
            return;
        }

        factory().Export(embeddedFile, globalExportArgs);
    }

    public void ExtractAndExport(ResourcePath path, CR2WFile file)
    {

    }

    #region GameFileHelper

    internal ResourceWrapper<T>? GetGameFile<T>(CResourceReference<T> resourceReference) where T : CResource => GetGameFile<T>(resourceReference.DepotPath);

    internal ResourceWrapper<T>? GetGameFile<T>(CResourceAsyncReference<T> resourceReference) where T : CResource => GetGameFile<T>(resourceReference.DepotPath);

    internal ResourceWrapper<T>? GetGameFile<T>(ResourcePath path) where T : CResource
    {
        if (Archives == null || path == ResourcePath.Empty)
        {
            return null;
        }

        var pathHash = (ulong)path;
        foreach (var archive in Archives)
        {
            foreach (var (hash, gameFile) in archive.Files)
            {
                if (hash != pathHash)
                {
                    continue;
                }

                using var ms = new MemoryStream();
                gameFile.Extract(ms);
                ms.Position = 0;

                using var reader = new CR2WReader(ms);
                if (reader.ReadFile(out var cr2wFile) == EFileReadErrorCodes.NoError && cr2wFile!.RootChunk.GetType() == typeof(T))
                {
                    return new ResourceWrapper<T>(cr2wFile);
                }
            }
        }

        return null;
    }

    internal class ResourceWrapper<T> where T : CResource
    {
        public CR2WFile? File { get; }

        public T RootChunk { get; }

        public ResourceWrapper(CR2WFile file)
        {
            File = file;
            RootChunk = (T)File.RootChunk;
        }

        public ResourceWrapper(T resource) => RootChunk = resource;
    }

    #endregion GameFileHelper
}