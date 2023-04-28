using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Nodes.Scene;

public class SceneOutputConnectorViewModel : OutputConnectorViewModel
{
    public scnOutputSocket Data { get; }

    public SceneOutputConnectorViewModel(string name, string title, uint ownerId, scnOutputSocket data) : base(name, title, ownerId)
    {
        Data = data;
    }
}