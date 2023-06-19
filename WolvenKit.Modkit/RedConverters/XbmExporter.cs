using System.IO;
using WolvenKit.Common.Model.Arguments;
using WolvenKit.Core.Interfaces;
using WolvenKit.Modkit.Managers;
using WolvenKit.RED4.CR2W;
using WolvenKit.RED4.Types;

namespace WolvenKit.Modkit.RedConverters;

public class XbmExporter : BaseExporter
{
    public XbmExporter(ConvertManager convertManager, ILoggerService loggerService) : base(convertManager, loggerService)
    {
    }

    public override ExportArgs GetExportArgs() => new XbmExportArgs();

    protected override bool Export()
    {
        if (_filePath is not {} filePath)
        {
            return false;
        }

        if (_resource is not CBitmapTexture bitmapTexture)
        {
            return false;
        }

        var image = RedImage.FromXBM(bitmapTexture);

        var outFilePath = Path.Combine(_convertManager.DepotPath!, _file != null ? "main" : "embedded");
        var outFileName = ((ulong)filePath).ToString();
        if (filePath.IsResolvable)
        {
            var resolved = filePath.GetResolvedText()!;

            outFilePath = Path.Combine(outFilePath, Path.GetDirectoryName(resolved) ?? "");
            outFileName = Path.GetFileNameWithoutExtension(resolved);
        }

        Directory.CreateDirectory(outFilePath);
        image.SaveToPNG(Path.Combine(outFilePath, $"{outFileName}.png"));

        return true;
    }
}