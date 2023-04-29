using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnXorNodeWrapper : BaseSceneViewModel<scnXorNode>, IDynamicInputNode
{
    public scnXorNodeWrapper(scnXorNode scnXorNode) : base(scnXorNode)
    {
    }

    internal override void GenerateSockets()
    {
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

        return input;
    }

    public void RemoveInput() => Input.Remove(Input[^1]);
}