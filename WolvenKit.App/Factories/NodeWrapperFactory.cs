using WolvenKit.App.ViewModels.Nodes.Quest;
using WolvenKit.Common;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.Factories;

public class NodeWrapperFactory : INodeWrapperFactory
{
    private readonly IArchiveManager _archiveManager;

    public NodeWrapperFactory(IArchiveManager archiveManager)
    {
        _archiveManager = archiveManager;
    }

    public questPhaseNodeDefinitionWrapper QuestPhaseNodeDefinitionWrapper(questPhaseNodeDefinition nodeDefinition) =>
        new(nodeDefinition, this, _archiveManager);

    public questSceneNodeDefinitionWrapper QuestSceneNodeDefinitionWrapper(questSceneNodeDefinition nodeDefinition) =>
        new(nodeDefinition, this, _archiveManager);
}