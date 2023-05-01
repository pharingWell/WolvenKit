using System;
using WolvenKit.App.ViewModels.Nodes.Scene.Internal;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnQuestNodeWrapper : BaseSceneViewModel<scnQuestNode>
{
    public scnQuestNodeWrapper(scnQuestNode scnSceneGraphNode) : base(scnSceneGraphNode)
    {
    }

    internal override void GenerateSockets()
    {
        var inSockets = new[] { "CutDestination", "In" };
        for (ushort i = 0; i < inSockets.Length; i++)
        {
            var name = inSockets[i];
            if (_castedData.IsockMappings.Count > i)
            {
                name = _castedData.IsockMappings[i].GetResolvedText()!;
            }

            Input.Add(new SceneInputConnectorViewModel(name, name, UniqueId, i));
        }

        for (var i = 0; i < _castedData.OutputSockets.Count; i++)
        {
            var name = $"Out{i}";
            if (_castedData.OsockMappings.Count > i)
            {
                name = _castedData.OsockMappings[i].GetResolvedText()!;
            }

            Output.Add(new SceneOutputConnectorViewModel(name, name, UniqueId, _castedData.OutputSockets[i]));
        }
    }
}