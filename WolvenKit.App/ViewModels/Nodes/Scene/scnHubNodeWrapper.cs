using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnHubNodeWrapper : BaseSceneViewModel<scnHubNode>, IDynamicInputNode
{
    public scnHubNodeWrapper(scnHubNode scnSceneGraphNode) : base(scnSceneGraphNode)
    {
    }

    internal override void GenerateSockets()
    {
        for (var i = 0; i < Data.OutputSockets.Count; i++)
        {
            Output.Add(new SceneOutputConnectorViewModel($"Out{i}", $"Out{i}", NodeId, Data.OutputSockets[i]));
        }
    }

    public void AddInput()
    {
        var index = (ushort)Input.Count;

        Input.Add(new SceneInputConnectorViewModel($"In{index}", $"In{index}", NodeId, index));
    }

    public void RemoveInput() => Input.Remove(Input[^1]);
}