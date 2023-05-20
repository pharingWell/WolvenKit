using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest.Internal;

public class BaseQuestSignalStoppingNodeDefinitionWrapper<T> : BaseQuestDisableableNodeDefinitionWrapper<T> where T : questSignalStoppingNodeDefinition
{
    public BaseQuestSignalStoppingNodeDefinitionWrapper(graphGraphNodeDefinition graphGraphNodeDefinition) : base(graphGraphNodeDefinition)
    {
    }
}