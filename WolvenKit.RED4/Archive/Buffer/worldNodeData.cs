using WolvenKit.RED4.Types;

namespace WolvenKit.RED4.Archive.Buffer;

// this might just be worldNodeEditorData
public class worldNodeData : RedBaseClass
{
    [REDProperty(IsIgnored = true)]
    public CUInt64 Id
    {
        get => GetPropertyValue<CUInt64>();
        set => SetPropertyValue<CUInt64>(value);
    }

    [REDProperty(IsIgnored = true)]
    public CUInt16 NodeIndex
    {
        get => GetPropertyValue<CUInt16>();
        set => SetPropertyValue<CUInt16>(value);
    }

    [REDProperty(IsIgnored = true)]
    public Vector4 Position
    {
        get => GetPropertyValue<Vector4>();
        set => SetPropertyValue<Vector4>(value);
    }

    [REDProperty(IsIgnored = true)]
    public Quaternion Orientation
    {
        get => GetPropertyValue<Quaternion>();
        set => SetPropertyValue<Quaternion>(value);
    }

    [REDProperty(IsIgnored = true)]
    public Vector3 Scale
    {
        get => GetPropertyValue<Vector3>();
        set => SetPropertyValue<Vector3>(value);
    }

    [REDProperty(IsIgnored = true)]
    public Vector3 Pivot
    {
        get => GetPropertyValue<Vector3>();
        set => SetPropertyValue<Vector3>(value);
    }

    [REDProperty(IsIgnored = true)]
    public Box Bounds
    {
        get => GetPropertyValue<Box>();
        set => SetPropertyValue<Box>(value);
    }

    [REDProperty(IsIgnored = true)]
    public NodeRef QuestPrefabRefHash
    {
        get => GetPropertyValue<NodeRef>();
        set => SetPropertyValue<NodeRef>(value);
    }

    [REDProperty(IsIgnored = true)]
    public NodeRef UkHash1
    {
        get => GetPropertyValue<NodeRef>();
        set => SetPropertyValue<NodeRef>(value);
    }

    [REDProperty(IsIgnored = true)]
    public CResourceReference<worldCookedPrefabData> CookedPrefabData
    {
        get => GetPropertyValue<CResourceReference<worldCookedPrefabData>>();
        set => SetPropertyValue<CResourceReference<worldCookedPrefabData>>(value);
    }

    [REDProperty(IsIgnored = true)]
    public CFloat MaxStreamingDistance
    {
        get => GetPropertyValue<CFloat>();
        set => SetPropertyValue<CFloat>(value);
    }

    [REDProperty(IsIgnored = true)]
    public CFloat UkFloat1
    {
        get => GetPropertyValue<CFloat>();
        set => SetPropertyValue<CFloat>(value);
    }

    // likely a bitfield

    [REDProperty(IsIgnored = true)]
    public CUInt16 Uk10
    {
        get => GetPropertyValue<CUInt16>();
        set => SetPropertyValue<CUInt16>(value);
    }

    [REDProperty(IsIgnored = true)]
    public CUInt16 Uk11
    {
        get => GetPropertyValue<CUInt16>();
        set => SetPropertyValue<CUInt16>(value);
    }

    [REDProperty(IsIgnored = true)]
    public CUInt16 Uk12
    {
        get => GetPropertyValue<CUInt16>();
        set => SetPropertyValue<CUInt16>(value);
    }
}