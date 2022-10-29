using WolvenKit.Core.Extensions;
using WolvenKit.RED4.Save.IO;
using WolvenKit.RED4.Types;

namespace WolvenKit.RED4.Save
{
    public class CommunitySystem : INodeData
    {
        public List<NodeRef> Unk1_NodeRefList { get; set; } = new();
        public List<CommunitySystem_Sub1> Unk2_UnkList { get; set; } = new();
        public List<NodeRef> Unk3_NodeRefList { get; set; } = new();
        public List<CommunitySystem_Sub3> Unk4_UnkList { get; set; } = new();
        public List<CommunitySystem_Sub4> Unk5_UnkList { get; set; } = new();
        public byte[] TrailingBytes { get; set; } // always 12?
    }

    public class CommunitySystem_Sub1
    {
        public List<CName> Unk1_NameList { get; set; } = new();
        public List<CommunitySystem_Sub2> Unk2_UnkList { get; set; } = new();
    }

    public class CommunitySystem_Sub2
    {
        public List<ulong> Unk1_EntityIdList { get; set; } = new();
        public byte[] Unk2_Bytes { get; set; }
        public List<ulong> Unk3_EntityIdList { get; set; } = new();
        public string Unk4_String { get; set; }
    }

    public class CommunitySystem_Sub3
    {
        public List<ulong> Unk1_EntityIdList { get; set; } = new();
        public byte[] Unk2_Bytes { get; set; }
        public List<ulong> Unk3_EntityIdList { get; set; } = new();
    }

    public class CommunitySystem_Sub4
    {
        public NodeRef Unk1_NodeRef { get; set; }
        public List<float> Unk2_FloatList { get; set; } = new();
        public List<Vector3> Unk3_VectorList { get; set; } = new();
    }


    public class CommunitySystemParser : INodeParser
    {
        public static string NodeName => Constants.NodeNames.COMMUNITY_SYSTEM;

        public void Read(BinaryReader reader, NodeEntry node)
        {
            var data = new CommunitySystem();

            var entryCount = reader.ReadUInt32();
            for (int i = 0; i < entryCount; i++)
            {
                data.Unk1_NodeRefList.Add(reader.ReadUInt64());
            }

            for (int i = 0; i < entryCount; i++)
            {
                var entry = new CommunitySystem_Sub1();

                var cnt = reader.ReadUInt32();
                for (var j = 0; j < cnt; j++)
                {
                    entry.Unk1_NameList.Add(reader.ReadUInt64());
                }

                for (int j = 0; j < cnt; j++)
                {
                    var entry2 = new CommunitySystem_Sub2();

                    var cnt2 = reader.ReadUInt32();
                    for (int k = 0; k < cnt2; k++)
                    {
                        entry2.Unk1_EntityIdList.Add(reader.ReadUInt64());
                    }

                    entry2.Unk2_Bytes = reader.ReadBytes(11);

                    var cnt3 = reader.ReadUInt32();
                    for (int k = 0; k < cnt3; k++)
                    {
                        entry2.Unk3_EntityIdList.Add(reader.ReadUInt64());
                    }
                    entry2.Unk4_String = reader.ReadLengthPrefixedString();

                    entry.Unk2_UnkList.Add(entry2);
                }

                data.Unk2_UnkList.Add(entry);
            }

            var tCnt = reader.ReadUInt32();
            for (int i = 0; i < tCnt; i++)
            {
                data.Unk3_NodeRefList.Add(reader.ReadUInt64());
            }

            for (int i = 0; i < tCnt; i++)
            {
                var entry = new CommunitySystem_Sub3();

                var cnt2 = reader.ReadUInt32();
                for (int k = 0; k < cnt2; k++)
                {
                    entry.Unk1_EntityIdList.Add(reader.ReadUInt64());
                }

                entry.Unk2_Bytes = reader.ReadBytes(11);

                var cnt3 = reader.ReadUInt32();
                for (int k = 0; k < cnt3; k++)
                {
                    entry.Unk3_EntityIdList.Add(reader.ReadUInt64());
                }

                data.Unk4_UnkList.Add(entry);
            }

            tCnt = reader.ReadUInt32();
            for (int i = 0; i < tCnt; i++)
            {
                var entry = new CommunitySystem_Sub4();

                entry.Unk1_NodeRef = reader.ReadUInt64();

                for (int j = 0; j < 17; j++)
                {
                    entry.Unk2_FloatList.Add(reader.ReadSingle());
                }

                var unk = reader.ReadUInt32();
                for (int j = 0; j < unk; j++)
                {
                    entry.Unk3_VectorList.Add(new Vector3 { X = reader.ReadSingle(), Y = reader.ReadSingle(), Z = reader.ReadSingle() });
                }

                data.Unk5_UnkList.Add(entry);
            }

            data.TrailingBytes = reader.ReadBytes(12);

            int trailingSize = node.Size - ((int)reader.BaseStream.Position - node.Offset);
            if (trailingSize != 0)
            {
                throw new Exception("CommunitySystemParser size mismatch!");
            }

            node.Value = data;
        }

        public void Write(NodeWriter writer, NodeEntry node)
        {
            var startPos = writer.BaseStream.Position;

            var data = (CommunitySystem)node.Value;

            writer.Write(data.Unk1_NodeRefList.Count);
            foreach (var entry in data.Unk1_NodeRefList)
            {
                writer.Write((ulong)entry);
            }

            writer.Write(data.Unk2_UnkList.Count);
            foreach (var entry in data.Unk2_UnkList)
            {
                writer.Write(entry.Unk1_NameList.Count);
                foreach (var cName in entry.Unk1_NameList)
                {
                    writer.Write((ulong)cName);
                }

                foreach (var entry2 in entry.Unk2_UnkList)
                {
                    writer.Write(entry2.Unk1_EntityIdList.Count);
                    foreach (var entityId in entry2.Unk1_EntityIdList)
                    {
                        writer.Write(entityId);
                    }

                    writer.Write(entry2.Unk2_Bytes);

                    writer.Write(entry2.Unk3_EntityIdList.Count);
                    foreach (var entityId in entry2.Unk3_EntityIdList)
                    {
                        writer.Write(entityId);
                    }

                    writer.WriteLengthPrefixedString(entry2.Unk4_String);
                }
            }

            writer.Write(data.Unk3_NodeRefList.Count);
            foreach (var entry in data.Unk3_NodeRefList)
            {
                writer.Write((ulong)entry);
            }

            foreach (var entry in data.Unk4_UnkList)
            {
                writer.Write(entry.Unk1_EntityIdList.Count);
                foreach (var entityId in entry.Unk1_EntityIdList)
                {
                    writer.Write(entityId);
                }

                writer.Write(entry.Unk2_Bytes);

                writer.Write(entry.Unk3_EntityIdList.Count);
                foreach (var entityId in entry.Unk3_EntityIdList)
                {
                    writer.Write(entityId);
                }
            }

            writer.Write(data.Unk5_UnkList.Count);
            foreach (var entry in data.Unk5_UnkList)
            {
                writer.Write((ulong)entry.Unk1_NodeRef);

                foreach (var f in entry.Unk2_FloatList)
                {
                    writer.Write(f);
                }

                writer.Write(entry.Unk3_VectorList.Count);
                foreach (var vector3 in entry.Unk3_VectorList)
                {
                    writer.Write(vector3.X);
                    writer.Write(vector3.Y);
                    writer.Write(vector3.Z);
                }
            }

            writer.Write(data.TrailingBytes);

            var size = writer.BaseStream.Position - startPos;
        }
    }

}
