using System.Collections.Generic;
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

        GenerateInputSockets();
        GenerateOutputSockets();
    }
}

public class BaseQuestViewModel<T> : BaseQuestViewModel where T : graphGraphNodeDefinition
{
    protected T _castedData => (T)_data;

    public BaseQuestViewModel(graphGraphNodeDefinition graphGraphNodeDefinition) : base(graphGraphNodeDefinition)
    {
    }

    protected override void GenerateInputSockets()
    {
        foreach (var socketHandle in _data.Sockets)
        {
            if (socketHandle.Chunk is questSocketDefinition socketDefinition)
            {
                if (socketDefinition.Type == Enums.questSocketType.Input ||
                    socketDefinition.Type == Enums.questSocketType.CutDestination)
                {
                    Input.Add(new InputConnectorViewModel(socketDefinition.Name.GetResolvedText()!, UniqueId));
                }
            }
            else
            {
                
            }
        }
    }

    protected override void GenerateOutputSockets()
    {
        foreach (var socketHandle in _data.Sockets)
        {
            if (socketHandle.Chunk is questSocketDefinition socketDefinition)
            {
                if (socketDefinition.Type == Enums.questSocketType.Output ||
                    socketDefinition.Type == Enums.questSocketType.CutSource)
                {
                    Output.Add(new OutputConnectorViewModel(socketDefinition.Name.GetResolvedText()!, UniqueId, new List<(uint, ushort)>()));
                }
            }
            else
            {

            }
        }
    }
}