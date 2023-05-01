using WolvenKit.App.ViewModels.Nodes.Quest.Internal;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest;

public class questOutputNodeDefinitionWrapper : BaseQuestIONodeDefinitionWrapper<questOutputNodeDefinition>
{
    public questOutputNodeDefinitionWrapper(graphGraphNodeDefinition graphGraphNodeDefinition) : base(graphGraphNodeDefinition)
    {
    }

    internal override void GenerateSockets()
    {
        base.GenerateSockets();

        var name = _castedData.SocketName.GetResolvedText()!;
        Output.Add(new QuestOutputConnectorViewModel(name, name, UniqueId, new questSocketDefinition()));
    }
}