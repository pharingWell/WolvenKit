using WolvenKit.Common.Model.Arguments;
using WolvenKit.RED4.Archive;
using WolvenKit.RED4.Archive.CR2W;

namespace WolvenKit.Modkit.RedConverters;

public class ExportItem
{
    public ExportItem(CR2WFile file) => File = file;

    public CR2WFile File { get; }

    public ICyberGameArchive? ParentArchive { get; set; }
    public ExportArgs? Args { get; set; }
}