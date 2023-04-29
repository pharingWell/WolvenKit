using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class scnStartNodeWrapper : BaseSceneViewModel<scnStartNode>
{
    private readonly scnEntryPoint _scnEntryPoint;

    public string Name
    {
        get => _scnEntryPoint.Name.GetResolvedText()!;
        set => _scnEntryPoint.Name = value;
    }

    public scnStartNodeWrapper(scnStartNode scnSceneGraphNode, scnEntryPoint entryPoint) : base(scnSceneGraphNode) => _scnEntryPoint = entryPoint;

    internal override void GenerateSockets()
    {
        for (var i = 0; i < Data.OutputSockets.Count; i++)
        {
            Output.Add(new SceneOutputConnectorViewModel($"Out{i}", $"Out{i}", NodeId, Data.OutputSockets[i]));
        }
    }
}