using System;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnQuestNodeWrapper : BaseSceneViewModel<scnQuestNode>
{
    public scnQuestNodeWrapper(scnQuestNode scnSceneGraphNode) : base(scnSceneGraphNode)
    {
    }

    internal override void GenerateSockets()
    {
        for (ushort i = 0; i < _castedData.IsockMappings.Count; i++)
        {
            var name = _castedData.IsockMappings[i].GetResolvedText()!;

            Input.Add(new SceneInputConnectorViewModel(name, name, NodeId, i));
        }

        for (var i = 0; i < _castedData.OutputSockets.Count; i++)
        {
            var name = $"Out{i}";
            if (_castedData.OsockMappings.Count > i)
            {
                name = _castedData.OsockMappings[i].GetResolvedText()!;
            }

            Output.Add(new SceneOutputConnectorViewModel(name, name, NodeId, _castedData.OutputSockets[i]));
        }
    }
}