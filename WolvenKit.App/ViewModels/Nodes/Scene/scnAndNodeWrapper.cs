using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnAndNodeWrapper : BaseSceneViewModel<scnAndNode>, IDynamicInputNode
{
    public scnAndNodeWrapper(scnAndNode scnSceneGraphNode) : base(scnSceneGraphNode)
    {
    }

    internal override void GenerateSockets()
    {
        for (ushort i = 0; i < _castedData.NumInSockets; i++)
        {
            Input.Add(new SceneInputConnectorViewModel($"In{i}", $"In{i}", NodeId, i));
        }

        for (var i = 0; i < Data.OutputSockets.Count; i++)
        {
            Output.Add(new SceneOutputConnectorViewModel($"Out{i}", $"Out{i}", NodeId, Data.OutputSockets[i]));
        }
    }

    public BaseConnectorViewModel AddInput()
    {
        var index = (ushort)Input.Count;
        var input = new SceneInputConnectorViewModel($"In{index}", $"In{index}", NodeId, index);

        Input.Add(input);
        _castedData.NumInSockets++;

        return input;
    }

    public void RemoveInput()
    {
        Input.Remove(Input[^1]);
        _castedData.NumInSockets--;
    }
}