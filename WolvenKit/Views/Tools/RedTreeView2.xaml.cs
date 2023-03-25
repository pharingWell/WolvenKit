using System.Windows;
using System.Windows.Controls;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.TreeGrid;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.Helpers;

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

        RedTreeView.SortComparers.Add(new SortComparer() { Comparer = new PropertyViewComparer(), PropertyName = "DisplayName" });

        RedTreeView.TreeGridContextMenuOpening += RedTreeView_OnTreeGridContextMenuOpening;
    }

    private void RedTreeView_OnTreeGridContextMenuOpening(object sender, TreeGridContextMenuEventArgs e)
    {
        var node = RedTreeView.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex);
        if (node.Item is not PropertyViewModel propertyViewModel)
        {
            e.Handled = true;
            return;
        }

        e.ContextMenu.Items.Clear();

        if (propertyViewModel.Parent is ArrayPropertyViewModel parentArray)
        {
            e.ContextMenu.Items.Add(new MenuItem() { Header = "Delete array Item", Command = parentArray.DeleteArrayItemCommand, CommandParameter = propertyViewModel });
            e.ContextMenu.Items.Add(new Separator());
        }

        if (propertyViewModel is ArrayPropertyViewModel arrayPropertyViewModel)
        {
            e.ContextMenu.Items.Add(new MenuItem() { Header = "Add item", Command = arrayPropertyViewModel.AddArrayItemCommand });
            e.ContextMenu.Items.Add(new MenuItem() { Header = "Clear", Command = arrayPropertyViewModel.ClearArrayCommand });
        }

        if (e.ContextMenu.Items.Count == 0)
        {
            e.Handled = true;
        }
    }
}