using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest.Internal;

public class QuestOutputConnectorViewModel : OutputConnectorViewModel
{
    public graphGraphSocketDefinition Data { get; }

    public QuestOutputConnectorViewModel(string name, string title, uint ownerId, graphGraphSocketDefinition data) : base(name, title, ownerId)
    {
        Data = data;
    }
}