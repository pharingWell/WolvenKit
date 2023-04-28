using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Quest;

public abstract class BaseQuestViewModel : NodeViewModel
{
    protected graphGraphNodeDefinition _data;

    protected BaseQuestViewModel(graphGraphNodeDefinition graphGraphNodeDefinition)
    {
        _data = graphGraphNodeDefinition;

        UniqueId = (uint)graphGraphNodeDefinition.GetHashCode();
        Title = $"{_data.GetType().Name[5..^14]}";

        GenerateSockets();
    }
}

public class BaseQuestViewModel<T> : BaseQuestViewModel where T : graphGraphNodeDefinition
{
    protected T _castedData => (T)_data;

    public BaseQuestViewModel(graphGraphNodeDefinition graphGraphNodeDefinition) : base(graphGraphNodeDefinition)
    {
    }

    internal override void GenerateSockets()
    {
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
                    Output.Add(new QuestOutputConnectorViewModel(name, name, UniqueId, socketDefinition));
                }
            }
        }
    }
}