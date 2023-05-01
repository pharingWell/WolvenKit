using WolvenKit.App.ViewModels.Nodes.Quest.Internal;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest;

public class questInputNodeDefinitionWrapper : BaseQuestIONodeDefinitionWrapper<questInputNodeDefinition>
{
    public questInputNodeDefinitionWrapper(questInputNodeDefinition node) : base(node)
    {
    }

    internal override void GenerateSockets()
    {
        base.GenerateSockets();

        var name = _castedData.SocketName.GetResolvedText()!;
        Input.Add(new QuestInputConnectorViewModel(name, name, UniqueId, new questSocketDefinition()));
    }
}