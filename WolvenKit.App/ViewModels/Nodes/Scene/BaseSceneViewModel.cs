using System.Collections.Generic;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public abstract class BaseSceneViewModel : NodeViewModel
{
    protected scnSceneGraphNode _data;

    protected BaseSceneViewModel(scnSceneGraphNode scnSceneGraphNode)
    {
        _data = scnSceneGraphNode;

        UniqueId = NodeId;
        Title = $"{_data.GetType().Name[3..^4]} [{NodeId}]";

        GenerateInputSockets();
        GenerateOutputSockets();
    }

    public uint NodeId => _data.NodeId.Id;
}

public abstract class BaseSceneViewModel<T> : BaseSceneViewModel where T : scnSceneGraphNode
{
    protected T _castedData => (T)_data;

    public BaseSceneViewModel(scnSceneGraphNode scnSceneGraphNode) : base(scnSceneGraphNode)
    {
    }

    protected override void GenerateInputSockets() => Input.Add(new InputConnectorViewModel("In", NodeId));
    protected override void GenerateOutputSockets()
    {
        for (var i = 0; i < _data.OutputSockets.Count; i++)
        {
            var targets = new List<(uint, ushort)>();
            foreach (var destination in _data.OutputSockets[i].Destinations)
            {
                targets.Add((destination.NodeId.Id, destination.IsockStamp.Ordinal));
            }

            Output.Add(new OutputConnectorViewModel($"Out{i}", NodeId, targets));
        }
    }
}