using WolvenKit.App.ViewModels.Nodes.Quest.Internal;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest;

public class questSwitchNodeDefinitionWrapper : BaseQuestDisableableNodeDefinitionWrapper<questSwitchNodeDefinition>
{
    public questSwitchNodeDefinitionWrapper(graphGraphNodeDefinition graphGraphNodeDefinition) : base(graphGraphNodeDefinition)
    {
    }
}