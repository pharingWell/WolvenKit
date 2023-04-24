using System.Collections.Generic;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnQuestNodeWrapper : BaseSceneViewModel<scnQuestNode>
{
    public scnQuestNodeWrapper(scnQuestNode scnSceneGraphNode) : base(scnSceneGraphNode)
    {
    }

    protected override void GenerateInputSockets()
    {
        foreach (var isockMapping in _castedData.IsockMappings)
        {
            Input.Add(new InputConnectorViewModel(isockMapping.GetResolvedText()!, NodeId));
        }
    }

    protected override void GenerateOutputSockets()
    {
        for (var i = 0; i < _data.OutputSockets.Count; i++)
        {
            var targets = new List<(uint, ushort)>();
            foreach (var destination in _data.OutputSockets[i].Destinations)
            {
                targets.Add((destination.NodeId.Id, destination.IsockStamp.Ordinal));
            }

            Output.Add(new OutputConnectorViewModel(_castedData.OsockMappings[i].GetResolvedText()!, NodeId, targets));
        }
    }
}