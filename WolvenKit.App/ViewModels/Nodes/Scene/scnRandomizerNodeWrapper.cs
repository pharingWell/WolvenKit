using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnRandomizerNodeWrapper : BaseSceneViewModel<scnRandomizerNode>
{
    public scnRandomizerNodeWrapper(scnRandomizerNode scnRandomizerNode) : base(scnRandomizerNode)
    {
    }

    internal override void GenerateSockets()
    {
        Input.Add(new SceneInputConnectorViewModel("In", "In", NodeId, 0));

        var names = new string[_castedData.NumOutSockets];

        var total = 0;
        for (var i = 0; i < _castedData.NumOutSockets; i++)
        {
            total += _castedData.Weights[i];
        }

        for (var i = 0; i < _castedData.NumOutSockets; i++)
        {
            var chance = (float)_castedData.Weights[i] / total * 100;
            names[i] = $"[{chance:0.00}%] Out{i}";
        }

        for (var i = 0; i < _castedData.OutputSockets.Count; i++)
        {
            Output.Add(new SceneOutputConnectorViewModel(names[i], names[i], NodeId, _castedData.OutputSockets[i]));
        }
    }
}