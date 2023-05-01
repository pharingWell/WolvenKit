using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest.Internal;

public class BaseQuestNodeDefinitionWrapper<T> : BaseQuestViewModel<T> where T : questNodeDefinition
{
    public BaseQuestNodeDefinitionWrapper(graphGraphNodeDefinition graphGraphNodeDefinition) : base(graphGraphNodeDefinition)
    {
        Title = $"{Title} [{_castedData.Id}]";
    }
}