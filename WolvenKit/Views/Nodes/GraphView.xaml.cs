using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Nodify;
using ReactiveUI;
using WolvenKit.App.ViewModels.Nodes;
using WolvenKit.App.ViewModels.Nodes.Quest;
using WolvenKit.App.ViewModels.Nodes.Scene;
using WolvenKit.App.ViewModels.Shell.RedTypes;
using WolvenKit.RED4.Types;
using WolvenKit.Views.Others;
using Point = System.Windows.Point;

namespace WolvenKit.Views.Nodes;
/// <summary>
/// Interaktionslogik für GraphView.xaml
/// </summary>
public partial class GraphView : INotifyPropertyChanged
{
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source), typeof(RedGraph), typeof(GraphView), new PropertyMetadata(null, OnSourceChanged));

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not GraphView { Source: not null } view)
        {
            return;
        }

        view.Dispatcher.BeginInvoke(new Action(() => UpdateView(view)), DispatcherPriority.ContextIdle);
    }

    private static void UpdateView(GraphView view)
    {
        view.Source.ArrangeNodes();
        view.Editor.FitToScreen();
    }

    public RedGraph Source
    {
        get => (RedGraph)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    private NodeViewModel _selectedNode;
    private PropertyViewModel _selectedPropertyViewModel;

    public NodeViewModel SelectedNode
    {
        get => _selectedNode;
        set
        {
            _selectedPropertyViewModel?.Dispose();

            SetField(ref _selectedNode, value);

            if (_selectedNode?.Data != null)
            {
                _selectedPropertyViewModel = PropertyViewModel.Create(_selectedNode.Data);
            }

            OnPropertyChanged(nameof(SelectedProperty));
        }
    }

    public PropertyViewModel SelectedProperty => _selectedPropertyViewModel;

    public Point ViewportLocation { get; set; }

    public GraphView()
    {
        InitializeComponent();
    }

    private void MenuItem_OnClick(object sender, RoutedEventArgs e) => ArrangeNodes();

    private void ArrangeNodes()
    {
        if (Source == null)
        {
            return;
        }

        UpdateLayout();
        Source.ArrangeNodes();
    }

    private void Editor_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        if (sender is not NodifyEditor nodifyEditor || Source == null)
        {
            return;
        }

        nodifyEditor.ContextMenu ??= new ContextMenu();
        nodifyEditor.ContextMenu.Items.Clear();

        if (Source.GraphType == RedGraphType.Scene)
        {
            var addMenu = new MenuItem { Header = "Add..."};

            var nodeTypes = Source.GetSceneNodeTypes();
            foreach (var nodeType in nodeTypes)
            {
                addMenu.Items.Add(CreateMenuItem(nodeType.Name[3..^4], () => Source.CreateSceneNode(nodeType, ViewportLocation)));
            }

            nodifyEditor.ContextMenu.Items.Add(addMenu);
        }

        if (Source.GraphType == RedGraphType.Quest)
        {
            var addMenu = new MenuItem { Header = "Add..." };

            var nodeTypes = Source.GetQuestNodeTypes();
            foreach (var nodeType in nodeTypes)
            {
                addMenu.Items.Add(CreateMenuItem(nodeType.Name[5..^14], () => Source.CreateQuestNode(nodeType, ViewportLocation)));
            }

            nodifyEditor.ContextMenu.Items.Add(addMenu);
        }

        nodifyEditor.ContextMenu.Items.Add(CreateMenuItem("Arrange Items", ArrangeNodes));

        nodifyEditor.ContextMenu.SetCurrentValue(ContextMenu.IsOpenProperty, true);

        e.Handled = true;
    }

    private void Node_ContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        if (sender is not Node { DataContext: NodeViewModel nvm } node || Source == null)
        {
            return;
        }

        node.ContextMenu ??= new ContextMenu();
        
        node.ContextMenu.Items.Clear();

        if (node.DataContext is IDynamicInputNode dynamicInputNode)
        {
            node.ContextMenu.Items.Add(CreateMenuItem("Add Input", () => dynamicInputNode.AddInput()));
            node.ContextMenu.Items.Add(new Separator());
        }

        if (node.DataContext is IDynamicOutputNode dynamicOutputNode)
        {
            node.ContextMenu.Items.Add(CreateMenuItem("Add Output", () => dynamicOutputNode.AddOutput()));
            node.ContextMenu.Items.Add(new Separator());
        }

        if (node.DataContext is scnChoiceNodeWrapper choice)
        {
            node.ContextMenu.Items.Add(CreateMenuItem("Add Choice", () => choice.AddChoice()));
            node.ContextMenu.Items.Add(new Separator());
        }

        if (node.DataContext is questPhaseNodeDefinitionWrapper questPhase)
        {
            node.ContextMenu.Items.Add(CreateMenuItem("Recalculate sockets", () => questPhase.RecalculateSockets()));
            node.ContextMenu.Items.Add(new Separator());
        }

        node.ContextMenu.Items.Add(CreateMenuItem("Remove Node", () => Source.RemoveNode(nvm)));

        node.ContextMenu.SetCurrentValue(ContextMenu.IsOpenProperty, true);

        e.Handled = true;
    }

    private MenuItem CreateMenuItem(string header, Action click)
    {
        var item = new MenuItem { Header = header };
        item.Click += (_, _) => click();
        return item;
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion
}
