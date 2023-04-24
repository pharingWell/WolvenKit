using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WolvenKit.App.ViewModels.Nodes;

public abstract partial class BaseConnectorViewModel : ObservableObject
{
    [ObservableProperty]
    private Point _anchor;

    [ObservableProperty]
    private bool _isConnected;

    public string Title { get; }
    public uint OwnerId { get; }

    public BaseConnectorViewModel(string title, uint ownerId)
    {
        Title = title;
        OwnerId = ownerId;
    }
}