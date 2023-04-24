using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest;

public class questPhaseNodeDefinitionWrapper : BaseQuestViewModel<questPhaseNodeDefinition>
{
    public GraphViewModel MainGraph { get; set; }

    public questPhaseNodeDefinitionWrapper(questPhaseNodeDefinition questPhaseNodeDefinition, GraphViewModel mainGraph) : base(questPhaseNodeDefinition)
    {
        MainGraph = mainGraph;

        Title = $"{Title} [{questPhaseNodeDefinition.Id}]";
    }
}