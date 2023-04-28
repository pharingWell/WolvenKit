using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest;

public class questRandomizerNodeDefinitionWrapper : BaseQuestViewModel<questRandomizerNodeDefinition>
{
    public questRandomizerNodeDefinitionWrapper(questRandomizerNodeDefinition questRandomizerNodeDefinition) : base(questRandomizerNodeDefinition)
    {
        Title = $"{Title} [{questRandomizerNodeDefinition.Id}]";
        Details.Add("Type", questRandomizerNodeDefinition.Mode.ToEnumString());
    }

    internal override void GenerateSockets()
    {
        var total = 0;
        foreach (var weight in _castedData.OutputWeights)
        {
            total += weight;
        }

        var index = 0;
        foreach (var socketHandle in _data.Sockets)
        {
            if (socketHandle.Chunk is questSocketDefinition socketDefinition)
            {
                var name = socketDefinition.Name.GetResolvedText()!;

                if (socketDefinition.Type == Enums.questSocketType.Input ||
                    socketDefinition.Type == Enums.questSocketType.CutDestination)
                {
                    Input.Add(new QuestInputConnectorViewModel(name, name, UniqueId, socketDefinition));
                }

                if (socketDefinition.Type == Enums.questSocketType.Output ||
                    socketDefinition.Type == Enums.questSocketType.CutSource)
                {
                    var chance = (float)_castedData.OutputWeights[index++] / total * 100;

                    Output.Add(new QuestOutputConnectorViewModel(name, $"[{chance:0.00}%] {name}", UniqueId, socketDefinition));
                }
            }
        }
    }
}