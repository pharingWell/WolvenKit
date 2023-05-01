using WolvenKit.App.ViewModels.Nodes.Quest.Internal;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest;

public class questNodeDefinitionWrapper : BaseQuestNodeDefinitionWrapper<questNodeDefinition>
{
    public questNodeDefinitionWrapper(questNodeDefinition questNodeDefinition) : base(questNodeDefinition)
    {
    }
}