using System.Collections.Generic;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnRandomizerNodeWrapper : BaseSceneViewModel<scnRandomizerNode>
{
    public scnRandomizerNodeWrapper(scnRandomizerNode scnRandomizerNode) : base(scnRandomizerNode)
    {
    }

    protected override void GenerateOutputSockets()
    {
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
            var targets = new List<(uint, ushort)>();
            foreach (var destination in _castedData.OutputSockets[i].Destinations)
            {
                targets.Add((destination.NodeId.Id, destination.IsockStamp.Ordinal));
            }

            Output.Add(new OutputConnectorViewModel(names[i], NodeId, targets));
        }
    }
}