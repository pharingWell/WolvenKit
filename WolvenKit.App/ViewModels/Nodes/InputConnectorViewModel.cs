namespace WolvenKit.App.ViewModels.Nodes;

public class InputConnectorViewModel : BaseConnectorViewModel
{
    public InputConnectorViewModel(string title, uint ownerId) : this(title, title, ownerId)
    {
    }

    public InputConnectorViewModel(string name, string title, uint ownerId) : base(name, title, ownerId)
    {
    }
}