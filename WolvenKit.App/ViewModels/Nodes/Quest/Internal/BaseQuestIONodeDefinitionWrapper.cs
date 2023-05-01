using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest.Internal;

public abstract class BaseQuestIONodeDefinitionWrapper<T> : BaseQuestDisableableNodeDefinitionWrapper<T> where T : questIONodeDefinition
{
    public BaseQuestIONodeDefinitionWrapper(graphGraphNodeDefinition graphGraphNodeDefinition) : base(graphGraphNodeDefinition)
    {
    }
}