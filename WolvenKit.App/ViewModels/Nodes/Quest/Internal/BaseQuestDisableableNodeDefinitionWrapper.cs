using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest.Internal;

public class BaseQuestDisableableNodeDefinitionWrapper<T> : BaseQuestNodeDefinitionWrapper<T> where T : questDisableableNodeDefinition
{
    public BaseQuestDisableableNodeDefinitionWrapper(graphGraphNodeDefinition graphGraphNodeDefinition) : base(graphGraphNodeDefinition)
    {
    }
}