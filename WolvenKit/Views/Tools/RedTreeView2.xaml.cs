using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DynamicData;
using SharpDX;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.UI.Xaml.TreeGrid.Filtering;
using Syncfusion.Windows.Tools.Controls;
using WolvenKit.App.Models;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.Helpers;
using WolvenKit.Views.UserControls;

namespace WolvenKit.Views.Tools;
/// <summary>
/// Interaktionslogik für RedTreeView2.xaml
/// </summary>
public partial class RedTreeView2 : UserControl
{
    #region DependencyProperties

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(RedTreeView2), new PropertyMetadata(OnItemsSourceChanged));

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

    public static readonly DependencyProperty SearchResultsProperty =
        DependencyProperty.Register(nameof(SearchResults), typeof(ObservableCollection<SearchResult>), typeof(RedTreeView2));

    public ObservableCollection<SearchResult> SearchResults
    {
        get => (ObservableCollection<SearchResult>)GetValue(SearchResultsProperty);
        set => SetValue(SearchResultsProperty, value);
    }

    public static readonly DependencyProperty NavigationItemsSourceProperty =
        DependencyProperty.Register(nameof(NavigationItemsSource), typeof(List<PropertyViewModel>), typeof(RedTreeView2));

    public List<PropertyViewModel> NavigationItemsSource
    {
        get => (List<PropertyViewModel>)GetValue(NavigationItemsSourceProperty);
        set => SetValue(NavigationItemsSourceProperty, value);
    }

    #endregion DependencyProperties

    public RedTreeView2()
    {
        InitializeComponent();

        //Navigator.HierarchyNavigatorSelectedItemChanged += Navigator_OnHierarchyNavigatorSelectedItemChanged;
        Navigator.SelectedItemChanged += Navigator_OnSelectedItemChanged;

        RedTreeView.SortComparers.Add(new SortComparer() { Comparer = new PropertyViewComparer(), PropertyName = "DisplayName" });

        RedTreeView.SelectionChanged += RedTreeView_OnSelectionChanged;
        RedTreeView.TreeGridContextMenuOpening += RedTreeView_OnTreeGridContextMenuOpening;
    }

    private void Navigator_OnSelectedItemChanged(object sender, EventArgs e)
    {
        if (Navigator.SelectedItem is not { } propertyViewModel)
        {
            return;
        }

        SelectTreeItem(propertyViewModel);
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not RedTreeView2 view)
        {
            return;
        }

        view.Navigator.SetCurrentValue(Breadcrumb.ItemsSourceProperty, new List<PropertyViewModel>((List<PropertyViewModel>)view.ItemsSource));
    }

    private bool _selectionChanging;

    private void RedTreeView_OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 1 && e.AddedItems[0] is TreeGridRowInfo info)
        {
            var parts = new List<PropertyViewModel>();

            var property = (PropertyViewModel)info.RowData;
            do
            {
                parts.Add(property);
                property = property.Parent;
            } while (property != null);

            parts.Reverse();

            Navigator.SetCurrentValue(Breadcrumb.ItemsSourceProperty, parts);
        }
    }

    private void SelectTreeItem()
    {
        /*var items = Navigator.ItemsHost.Items;

        if (items.Count < 2)
        {
            return;
        }

        for (var i = 0; i < items.Count - 1; i++)
        {
            var rowIndex = RedTreeView.ResolveToRowIndex(items[i]);
            RedTreeView.ExpandNode(rowIndex);
        }

        var lastItem = items[^1];
        RedTreeView.SetCurrentValue(SfGridBase.SelectedItemProperty, lastItem);

        var rowIndex2 = RedTreeView.ResolveToRowIndex(lastItem);
        var columnIndex = RedTreeView.ResolveToStartColumnIndex();
        RedTreeView.ScrollInView(new RowColumnIndex(rowIndex2, columnIndex));*/
    }

    private void SelectTreeItem(PropertyViewModel item)
    {
        var items = new List<PropertyViewModel>();
        do
        {
            items.Add(item);
            item = item.Parent;
        } while (item != null);

        items.Reverse();

        if (items.Count > 0)
        {
            int rowIndex;
            for (var i = 0; i < items.Count - 1; i++)
            {
                rowIndex = RedTreeView.ResolveToRowIndex(items[i]);
                RedTreeView.ExpandNode(rowIndex);
            }

            var lastItem = items[^1];
            RedTreeView.SetCurrentValue(SfGridBase.SelectedItemProperty, lastItem);

            rowIndex = RedTreeView.ResolveToRowIndex(lastItem);
            var columnIndex = RedTreeView.ResolveToStartColumnIndex();

            RedTreeView.ScrollInView(new RowColumnIndex(rowIndex, columnIndex));
        }
    }

    private void Navigator_OnHierarchyNavigatorSelectedItemChanged(object sender, HierarchyNavigatorSelectedItemChangedEventArgs e)
    {
        if (_selectionChanging)
        {
            return;
        }

        _selectionChanging = true;
        SelectTreeItem();
        _selectionChanging = false;
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

    private IEnumerable<PropertyViewModel> FindItems(PropertyViewModel property, string text)
    {
        if (property.DisplayValue.Contains(text))
        {
            yield return property;
        }

        if (property.Properties.Count > 0)
        {
            foreach (var child in property.Properties)
            {
                foreach (var childProperty in FindItems(child, text))
                {
                    yield return childProperty;
                }
                
            }
        }
    }

    private string BuildPath(PropertyViewModel property)
    {
        var parts = new List<string>();

        do
        {
            parts.Add(property.DisplayName);
            property = property.Parent;
        } while (property != null);

        parts.Reverse();

        return string.Join('\\', parts);
    }

    private void SearchTextBox_OnKeyUp(object sender, KeyEventArgs e)
    {
        if (ItemsSource is not List<PropertyViewModel> list || list.Count == 0)
        {
            return;
        }

        var root = list[0];

        if (e.Key == Key.Enter)
        {
            if (SearchResults == null)
            {
                SetCurrentValue(SearchResultsProperty, new ObservableCollection<SearchResult>());
            }

            SearchResults!.Clear();
            foreach (var propertyViewModel in FindItems(root, SearchTextBox.Text))
            {
                SearchResults.Add(new SearchResult($"{{{BuildPath(propertyViewModel)}}} {propertyViewModel.DisplayValue}", propertyViewModel));
            }
        }
    }

    private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ListBoxItem item || item.Content is not SearchResult result)
        {
            return;
        }

        SelectTreeItem(result.Data);
    }

    public record SearchResult(string Name, PropertyViewModel Data);
}