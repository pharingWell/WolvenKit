using WolvenKit.Common.Model;

namespace WolvenKit.Common
{
    /// <summary>
    /// Abstract implementation for cyberpunk 2077 archive managers
    /// e.g. archive
    /// </summary>
    public abstract class RED4ArchiveManager : WolvenKitArchiveManager
    {
        protected readonly string[] VanillaDlClist = new string[]
        {
            "DLC1", "DLC2", "DLC3", "DLC4", "DLC5", "DLC6", "DLC7", "DLC8", "DLC9", "DLC10", "DLC11", "DLC12",
            "DLC13", "DLC14", "DLC15", "DLC16", "bob", "ep1"
        };
    }
}
