using System;
using System.Collections.Generic;
using System.Linq;
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
using WolvenKit.App.ViewModels.Nodes;

namespace WolvenKit.Views.Nodes;
/// <summary>
/// Interaktionslogik für GraphView.xaml
/// </summary>
public partial class GraphView
{
    public GraphView()
    {
        InitializeComponent();
    }

    private void MenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is not GraphViewModel { } vm)
        {
            return;
        }

        vm.ArrangeNodes();
    }
}
