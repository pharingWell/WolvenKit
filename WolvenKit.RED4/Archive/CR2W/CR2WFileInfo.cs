using WolvenKit.Core.Extensions;
using WolvenKit.RED4.Types;

namespace WolvenKit.RED4.Archive.CR2W;

public class CR2WFileInfo
{
    public CR2WFileHeader FileHeader { get; internal set; }

    public Dictionary<uint, CName> StringDict { get; internal set; } = new();

    public CR2WNameInfo[] NameInfo { get; internal set; } = Array.Empty<CR2WNameInfo>();
    public CR2WImportInfo[] ImportInfo { get; internal set; } = Array.Empty<CR2WImportInfo>();
    public CR2WPropertyInfo[] PropertyInfo { get; internal set; } = Array.Empty<CR2WPropertyInfo>();
    public CR2WExportInfo[] ExportInfo { get; internal set; } = Array.Empty<CR2WExportInfo>();
    public CR2WBufferInfo[] BufferInfo { get; internal set; } = Array.Empty<CR2WBufferInfo>();
    public CR2WEmbeddedInfo[] EmbeddedInfo { get; internal set; } = Array.Empty<CR2WEmbeddedInfo>();

    public List<CR2WImport> GetImports()
    {
        var result = new List<CR2WImport>();
        foreach (var info in ImportInfo)
        {
            result.Add(new CR2WImport
            {
                ClassName = StringDict[info.className]!,
                DepotPath = (ulong)StringDict[info.offset],
                Flags = (InternalEnums.EImportFlags)info.flags
            });
        }

        return result;
    }
}