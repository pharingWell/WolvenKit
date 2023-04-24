using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnEndNodeWrapper : BaseSceneViewModel<scnEndNode>
{
    public scnEndNodeWrapper(scnEndNode scnEndNode, string name) : base(scnEndNode)
    {
        Details.Add("Name", name);
        Details.Add("Type", scnEndNode.Type.ToEnumString());
    }
}