using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DynamicData;
using ICSharpCode.AvalonEdit.Document;
using ReactiveUI;
using WolvenKit.App.ViewModels.Documents;
using WolvenKit.App.ViewModels.Nodes;
using WolvenKit.App.ViewModels.Nodes.Quest;
using WolvenKit.Views.Nodes;

namespace WolvenKit.Views.Documents;
/// <summary>
/// Interaktionslogik für RDTGraphView2.xaml
/// </summary>
public partial class RDTGraphView2 : ReactiveUserControl<RDTGraphViewModel2>
{
    private readonly List<RedGraph> _history = new();

    public RDTGraphView2()
    {
        InitializeComponent();

        KeyDown += OnKeyDown;

        this.WhenActivated(disposables =>
        {
            _history.Add(ViewModel!.MainGraph);
            BuildBreadcrumb();
        });
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab && Editor.SelectedNode != null)
        {
            if (Editor.SelectedNode is IGraphProvider provider)
            {
                _history.Add(provider.Graph);
                Editor.SetCurrentValue(GraphView.SourceProperty, provider.Graph);

                BuildBreadcrumb();
            }

            if (Editor.SelectedNode is questInputNodeDefinitionWrapper input)
            {
                if (_history.Count > 1)
                {
                    _history.Remove(_history[^1]);
                    Editor.SetCurrentValue(GraphView.SourceProperty, _history[^1]);

                    BuildBreadcrumb();
                }
            }
        }
    }

    private void BuildBreadcrumb()
    {
        Breadcrumb.Children.Clear();

        for (var i = 0; i < _history.Count; i++)
        {
            AddNewElement(_history[i].Title, _history[i]);

            if (i < _history.Count - 1)
            {
                AddNewElement(">", null);
            }
        }

        void AddNewElement(string text, RedGraph graph)
        {
            if (Breadcrumb.Children.Count > 0)
            {
                text = " " + text;
            }

            var tmp = new TextBlock { Text = text, Tag = graph };
            tmp.PreviewMouseDown += BreadcrumbElement_OnPreviewMouseDown;

            Breadcrumb.Children.Add(tmp);
        }
    }

    private void BreadcrumbElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not TextBlock { Tag: RedGraph graph } block)
        {
            return;
        }

        if (_history.Count == 1)
        {
            return;
        }

        if (block.Text.Trim() == ">")
        {
            return;
        }

        Editor.SetCurrentValue(GraphView.SourceProperty, graph);

        for (var i = _history.Count - 1; i >= 0; i--)
        {
            if (ReferenceEquals(_history[i], graph))
            {
                break;
            }
            _history.RemoveAt(i);
        }

        BuildBreadcrumb();
    }
}
