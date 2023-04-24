using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest;

public class questNodeDefinitionWrapper : BaseQuestViewModel<questNodeDefinition>
{
    public questNodeDefinitionWrapper(questNodeDefinition questNodeDefinition) : base(questNodeDefinition)
    {
        Title = $"{Title} [{questNodeDefinition.Id}]";
    }
}