using WolvenKit.App.ViewModels.Nodes.Quest.Internal;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes;

public class questPauseConditionNodeDefinitionWrapper : BaseQuestSignalStoppingNodeDefinitionWrapper<questPauseConditionNodeDefinition>
{
    public questPauseConditionNodeDefinitionWrapper(questPauseConditionNodeDefinition graphGraphNodeDefinition) : base(graphGraphNodeDefinition)
    {
        if (graphGraphNodeDefinition.Condition.Chunk != null)
        {
            Details.Add("Type", graphGraphNodeDefinition.Condition.Chunk.GetType().Name[5..^9]);
        }
    }
}