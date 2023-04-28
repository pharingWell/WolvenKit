using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnXorNodeWrapper : BaseSceneViewModel<scnXorNode>
{
    public scnXorNodeWrapper(scnXorNode scnXorNode) : base(scnXorNode)
    {
    }

    internal override void GenerateSockets()
    {
        Input.Add(new SceneInputConnectorViewModel("In0", "In0", NodeId, 0));
        Input.Add(new SceneInputConnectorViewModel("In1", "In1", NodeId, 1));

        for (var i = 0; i < Data.OutputSockets.Count; i++)
        {
            Output.Add(new SceneOutputConnectorViewModel($"Out{i}", $"Out{i}", NodeId, Data.OutputSockets[i]));
        }
    }
}