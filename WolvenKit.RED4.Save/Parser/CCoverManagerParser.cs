using WolvenKit.RED4.Save.IO;
using WolvenKit.RED4.Types;

namespace WolvenKit.RED4.Save
{
    public class CCoverManager : INodeData
    {
        public List<CCoverManagerEntry> Entries { get; set; }

        public CCoverManager()
        {
            Entries = new List<CCoverManagerEntry>();
        }
    }
    public class CCoverManagerEntry
    {
        public NodeRef Unk_NodeRef { get; set; }
        public ulong Unk_EntityID { get; set; }

        // probably bool, if this is true, the hashes are also used somewhere els in the save
        public byte Unknown3 { get; set; }
    }


    public class CCoverManagerParser : INodeParser
    {
        public static string NodeName => Constants.NodeNames.C_COVER_MANAGER;

        public void Read(BinaryReader reader, NodeEntry node)
        {
            var data = new CCoverManager();
            var entryCount = reader.ReadUInt32();
            for (int i = 0; i < entryCount; i++)
            {
                var entry = new CCoverManagerEntry();
                entry.Unk_NodeRef = reader.ReadUInt64();
                entry.Unk_EntityID = reader.ReadUInt64();
                entry.Unknown3 = reader.ReadByte();
                data.Entries.Add(entry);
            }

            node.Value = data;
        }

        public void Write(NodeWriter writer, NodeEntry node)
        {
            var data = (CCoverManager)node.Value;

            writer.Write(data.Entries.Count);
            foreach (var entry in data.Entries)
            {
                writer.Write((ulong)entry.Unk_NodeRef);
                writer.Write(entry.Unk_EntityID);
                writer.Write(entry.Unknown3);
            }
        }
    }

}
