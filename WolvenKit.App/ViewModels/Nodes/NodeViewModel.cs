using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WolvenKit.App.ViewModels.Nodes.Scene;
using Point = System.Windows.Point;

namespace WolvenKit.App.ViewModels.Nodes;

public abstract partial class NodeViewModel : ObservableObject
{
    public uint UniqueId { get; protected set; } = uint.MaxValue;

    [ObservableProperty]
    private Point _location;

    public Size Size { get; set; }

    public string Title { get; protected set; } = null!;
    public Dictionary<string, string> Details { get; } = new();

    public ObservableCollection<InputConnectorViewModel> Input { get; } = new();
    public ObservableCollection<OutputConnectorViewModel> Output { get; } = new();

    public bool IsDynamic => GetType().IsAssignableTo(typeof(IDynamicInputNode));

    internal abstract void GenerateSockets();
}