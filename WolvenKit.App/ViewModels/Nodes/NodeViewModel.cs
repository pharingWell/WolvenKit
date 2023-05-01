using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WolvenKit.RED4.Types;
using Point = System.Windows.Point;

namespace WolvenKit.App.ViewModels.Nodes;

public abstract partial class NodeViewModel : ObservableObject
{
    public abstract uint UniqueId { get; }

    [ObservableProperty]
    private Point _location;

    public Size Size { get; set; }

    public string Title { get; protected set; } = null!;
    public Dictionary<string, string> Details { get; } = new();

    public ObservableCollection<InputConnectorViewModel> Input { get; } = new();
    public ObservableCollection<OutputConnectorViewModel> Output { get; } = new();

    public RedBaseClass Data { get; }
    public bool IsDynamic => GetType().IsAssignableTo(typeof(IDynamicInputNode));

    protected NodeViewModel(RedBaseClass data) => Data = data;

    internal abstract void GenerateSockets();
}