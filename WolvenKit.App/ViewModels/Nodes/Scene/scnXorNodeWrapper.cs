using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnXorNodeWrapper : BaseSceneViewModel<scnXorNode>
{
    public scnXorNodeWrapper(scnXorNode scnXorNode) : base(scnXorNode)
    {
    }

    protected override void GenerateInputSockets()
    {
        Input.Add(new InputConnectorViewModel("In0", NodeId));
        Input.Add(new InputConnectorViewModel("In1", NodeId));
    }
}