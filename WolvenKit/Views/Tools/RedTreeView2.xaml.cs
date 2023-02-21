using System.Windows;
using System.Windows.Controls;

namespace WolvenKit.Views.Tools;
/// <summary>
/// Interaktionslogik für RedTreeView2.xaml
/// </summary>
public partial class RedTreeView2 : UserControl
{
    #region DependencyProperties

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(RedTreeView2));

    public object ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }


    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(RedTreeView2));

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }


    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.Register(nameof(SelectedItems), typeof(object), typeof(RedTreeView2));

    public object SelectedItems
    {
        get => GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    #endregion DependencyProperties

    public RedTreeView2()
    {
        InitializeComponent();
    }
}
