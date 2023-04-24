using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnStartNodeWrapper : BaseSceneViewModel<scnStartNode>
{
    public scnStartNodeWrapper(scnStartNode scnSceneGraphNode, string name) : base(scnSceneGraphNode)
    {
        Details.Add("Name", name);
    }
}