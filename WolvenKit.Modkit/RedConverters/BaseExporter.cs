using System;
using System.IO;
using WolvenKit.Common.Model.Arguments;
using WolvenKit.Core.Interfaces;
using WolvenKit.Modkit.Interfaces;
using WolvenKit.Modkit.Managers;
using WolvenKit.RED4.Archive;
using WolvenKit.RED4.Archive.CR2W;
using WolvenKit.RED4.Archive.IO;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.Types;

namespace WolvenKit.Modkit.RedConverters;

public abstract class BaseExporter
{
    protected readonly ConvertManager _convertManager;
    protected readonly ILoggerService _loggerService;

    protected IGameFile? _gameFile;
    protected ResourcePath? _filePath;
    protected CR2WFile? _file;
    protected CResource? _resource;
    protected GlobalExportArgs _globalExportArgs = null!;

    protected BaseExporter(ConvertManager convertManager, ILoggerService loggerService)
    {
        _convertManager = convertManager;
        _loggerService = loggerService;
    }

    public abstract ExportArgs GetExportArgs();

    protected abstract bool Export();

    public bool Export(IGameFile gameFile, GlobalExportArgs args)
    {
        _gameFile = gameFile;
        _filePath = gameFile.Key;
        _file = ReadCR2W(_gameFile);
        _resource = _file.RootChunk;
        _globalExportArgs = args;

        return Export();
    }

    public bool Export(CR2WFile file, GlobalExportArgs args)
    {
        _file = file;
        _resource = _file.RootChunk;
        _globalExportArgs = args;

        return Export();
    }

    public bool Export(ICR2WEmbeddedFile embeddedFile, GlobalExportArgs args)
    {
        _filePath = embeddedFile.FileName;
        _resource = embeddedFile.Content;
        _globalExportArgs = args;

        return Export();
    }

    private CR2WFile ReadCR2W(IGameFile gameFile)
    {
        using var ms = new MemoryStream();
        gameFile.Extract(ms);
        ms.Position = 0;

        using var reader = new CR2WReader(ms);
        if (reader.ReadFile(out var file) == EFileReadErrorCodes.NoError)
        {
            return file!;
        }

        throw new Exception();
    }
}