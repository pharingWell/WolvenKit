using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest;

public class questInputNodeDefinitionWrapper : BaseQuestViewModel<questInputNodeDefinition>
{
    public questInputNodeDefinitionWrapper(questInputNodeDefinition questInputNodeDefinition) : base(questInputNodeDefinition)
    {
        Title = $"{Title} [{questInputNodeDefinition.Id}]";
    }
}