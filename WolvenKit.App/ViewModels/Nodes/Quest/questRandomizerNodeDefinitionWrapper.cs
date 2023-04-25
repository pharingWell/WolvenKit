using System.Collections.Generic;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest;

public class questRandomizerNodeDefinitionWrapper : BaseQuestViewModel<questRandomizerNodeDefinition>
{
    public questRandomizerNodeDefinitionWrapper(questRandomizerNodeDefinition questRandomizerNodeDefinition) : base(questRandomizerNodeDefinition)
    {
        Title = $"{Title} [{questRandomizerNodeDefinition.Id}]";
        Details.Add("Type", questRandomizerNodeDefinition.Mode.ToEnumString());
    }

    protected override void GenerateOutputSockets()
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
                if (socketDefinition.Type == Enums.questSocketType.Output ||
                    socketDefinition.Type == Enums.questSocketType.CutSource)
                {
                    var name = socketDefinition.Name.GetResolvedText()!;
                    var chance = (float)_castedData.OutputWeights[index++] / total * 100;

                    Output.Add(new OutputConnectorViewModel(name, $"[{chance:0.00}%] {name}", UniqueId, new List<(uint, ushort)>()));
                }
            }
        }
    }
}