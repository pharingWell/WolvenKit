using System.Collections.ObjectModel;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public interface IDynamicInputNode
{
    public ObservableCollection<InputConnectorViewModel> Input { get; }

    public BaseConnectorViewModel AddInput();
    public void RemoveInput();
}