using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ReactiveUI;
using WolvenKit.App.ViewModels.Nodes;
using WolvenKit.Views.Others;

namespace WolvenKit.Views.Nodes;
/// <summary>
/// Interaktionslogik für GraphView.xaml
/// </summary>
public partial class GraphView
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

    public NodeViewModel SelectedNode { get; set; }

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
}
