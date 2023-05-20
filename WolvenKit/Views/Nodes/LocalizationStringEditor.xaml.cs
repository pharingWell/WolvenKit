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
using WolvenKit.RED4.Types;
using WolvenKit.Views.UserControls;

namespace WolvenKit.Views.Nodes;
/// <summary>
/// Interaktionslogik für LocalizationStringEditor.xaml
/// </summary>
public partial class LocalizationStringEditor : UserControl
{
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source), typeof(LocalizationString), typeof(LocalizationStringEditor), new PropertyMetadata(null));

    public LocalizationString Source
    {
        get => (LocalizationString)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public LocalizationStringEditor()
    {
        InitializeComponent();
    }
}
