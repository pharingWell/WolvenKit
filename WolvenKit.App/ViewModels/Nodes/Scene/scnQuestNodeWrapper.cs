using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnQuestNodeWrapper : BaseSceneViewModel<scnQuestNode>
{
    public scnQuestNodeWrapper(scnQuestNode scnSceneGraphNode) : base(scnSceneGraphNode)
    {
    }

    internal override void GenerateSockets()
    {
        foreach (var isockMapping in _castedData.IsockMappings)
        {
            var name = isockMapping.GetResolvedText()!;

            Input.Add(new SceneInputConnectorViewModel(name, name, NodeId, 0));
        }

        for (var i = 0; i < _castedData.OutputSockets.Count; i++)
        {
            var name = _castedData.OsockMappings[i].GetResolvedText()!;

            Output.Add(new SceneOutputConnectorViewModel(name, name, NodeId, _castedData.OutputSockets[i]));
        }
    }
}