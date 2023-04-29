using System.Windows;
using System.Windows.Controls;
using WolvenKit.App.ViewModels.Nodes.Scene;

namespace WolvenKit.Views.Nodes;

internal class NodeTemplateSelector : DataTemplateSelector
{
    public DataTemplate Default { get; set; }
    public DataTemplate SceneStartNode { get; set; }
    public DataTemplate SceneEndNode { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is scnStartNodeWrapper)
        {
            return SceneStartNode;
        }

        if (item is scnEndNodeWrapper)
        {
            return SceneEndNode;
        }

        return Default;
    }
}