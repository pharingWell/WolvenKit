﻿using DynamicData;
using WolvenKit.App.ViewModels.Nodes.Scene.Internal;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnDeletionMarkerNodeWrapper : BaseSceneViewModel<scnDeletionMarkerNode>, IDynamicInputNode
{
    public scnDeletionMarkerNodeWrapper(scnDeletionMarkerNode scnSceneGraphNode) : base(scnSceneGraphNode)
    {
    }

    internal override void GenerateSockets()
    {
        for (var i = 0; i < _castedData.OutputSockets.Count; i++)
        {
            Output.Add(new SceneOutputConnectorViewModel($"Out{i}", $"Out{i}", UniqueId, _castedData.OutputSockets[i]));
        }
    }

    public BaseConnectorViewModel AddInput()
    {
        var index = (ushort)Input.Count;
        var input = new SceneInputConnectorViewModel($"In{index}", $"In{index}", UniqueId, index);

        Input.Add(input);

        return input;
    }

    public void RemoveInput() => Input.Remove(Input[^1]);
}