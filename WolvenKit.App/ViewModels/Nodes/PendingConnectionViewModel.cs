namespace WolvenKit.App.ViewModels.Nodes;

public class PendingConnectionViewModel
{
    public BaseConnectorViewModel Source { get; set; } = default!;
    public BaseConnectorViewModel? Target { get; set; }
}