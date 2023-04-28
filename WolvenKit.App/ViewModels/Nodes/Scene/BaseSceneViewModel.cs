using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public abstract class BaseSceneViewModel : NodeViewModel
{
    public scnSceneGraphNode Data { get; }

    protected BaseSceneViewModel(scnSceneGraphNode scnSceneGraphNode)
    {
        Data = scnSceneGraphNode;

        UniqueId = NodeId;
        Title = $"{Data.GetType().Name[3..^4]} [{NodeId}]";

        GenerateSockets();
    }

    public uint NodeId => Data.NodeId.Id;
}

public abstract class BaseSceneViewModel<T> : BaseSceneViewModel where T : scnSceneGraphNode
{
    protected T _castedData => (T)Data;

    public BaseSceneViewModel(scnSceneGraphNode scnSceneGraphNode) : base(scnSceneGraphNode)
    {
    }

    internal override void GenerateSockets()
    {
        Input.Add(new SceneInputConnectorViewModel("In", "In", NodeId, 0));

        for (var i = 0; i < Data.OutputSockets.Count; i++)
        {
            Output.Add(new SceneOutputConnectorViewModel($"Out{i}", $"Out{i}", NodeId, Data.OutputSockets[i]));
        }
    }
}