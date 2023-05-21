using System;
using System.Collections.Generic;
using System.IO;
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

namespace WolvenKit.Views.Nodes;
/// <summary>
/// Interaktionslogik für ResourceReferenceEditor.xaml
/// </summary>
public partial class ResourceReferenceEditor : UserControl
{
    public IRedRef RedResourceReference
    {
        get => (IRedRef)GetValue(RedResourceReferenceProperty);
        set => SetValue(RedResourceReferenceProperty, value);
    }

    public static readonly DependencyProperty RedResourceReferenceProperty =
        DependencyProperty.Register(nameof(RedResourceReference), typeof(IRedRef),
            typeof(ResourceReferenceEditor), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRedResourceReferenceChanged));

    private static void OnRedResourceReferenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ResourceReferenceEditor view)
        {
            return;
        }

        view.UpdateView();
    }

    public ResourceReferenceEditor()
    {
        InitializeComponent();

        TextBoxDepotPath.TextChanged += TextBoxDepotPath_OnTextChanged;
    }

    private void TextBoxDepotPath_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var str = TextBoxDepotPath.Text;

        ResourcePath path;
        if (str == "")
        {
            path = ResourcePath.Empty;
        }
        else if (ulong.TryParse(str, out var val))
        {
            path = val;
        }
        else
        {
            path = str;
        }

        SetCurrentValue(RedResourceReferenceProperty, (IRedRef)System.Activator.CreateInstance(RedResourceReference.GetType(), path, RedResourceReference.Flags));
    }

    private void UpdateView()
    {
        if (RedResourceReference == null)
        {
            return;
        }

        if (RedResourceReference.DepotPath.IsResolvable)
        {
            TextBoxDepotPath.SetCurrentValue(TextBox.TextProperty, RedResourceReference.DepotPath.GetResolvedText()!);
        }
        else if (RedResourceReference.DepotPath != 0)
        {
            TextBoxDepotPath.SetCurrentValue(TextBox.TextProperty, ((ulong)RedResourceReference.DepotPath).ToString());
        }
        else
        {
            TextBoxDepotPath.SetCurrentValue(TextBox.TextProperty, "");
        }
    }
}
