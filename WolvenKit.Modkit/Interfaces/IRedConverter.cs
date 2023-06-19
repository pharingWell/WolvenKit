using WolvenKit.Common.Model.Arguments;

namespace WolvenKit.Modkit.Interfaces;

public interface IRedExporter
{
    public ExportArgs GetExportArgs();

    public bool Export();
}