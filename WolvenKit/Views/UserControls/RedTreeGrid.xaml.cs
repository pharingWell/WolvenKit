#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.TreeGrid;
using WolvenKit.App.Helpers;
using WolvenKit.App.ViewModels.Shell.RedTypes;
using WolvenKit.Helpers;
using WolvenKit.RED4.Types;
using WolvenKit.Views.Documents;

namespace WolvenKit.Views.UserControls;
/// <summary>
/// Interaktionslogik für RedTreeGrid.xaml
/// </summary>
public partial class RedTreeGrid : UserControl
{
    public RedTreeGrid()
    {
        InitializeComponent();

        RedTreeView.SortComparers.Add(new SortComparer() { Comparer = new PropertyViewComparer(), PropertyName = "DisplayName" });

        RedTreeView.ItemsSourceChanged += RedTreeView_OnItemsSourceChanged;
        RedTreeView.NodeExpanded += RedTreeView_OnNodeExpanded;
        RedTreeView.SelectionChanged += RedTreeView_OnSelectionChanged;
        RedTreeView.TreeGridContextMenuOpening += RedTreeView_OnTreeGridContextMenuOpening;
    }

    private void RedTreeView_OnItemsSourceChanged(object? sender, TreeGridItemsSourceChangedEventArgs e)
    {
        if (RedTreeView.ItemsSource == null)
        {
            return;
        }

        RedTreeView.View.NodeCollectionChanged += RedTreeViewView_OnNodeCollectionChanged;
        RedTreeView.View.RecordPropertyChanged += RedTreeViewView_OnRecordPropertyChanged;
    }

    private void RedTreeViewView_OnRecordPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var rowIndex = RedTreeView.ResolveToRowIndex(sender);

        RedTreeView.CollapseNode(rowIndex);
        RedTreeView.ExpandNode(rowIndex);
    }

    private void RedTreeViewView_OnNodeCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
    }

    public event EventHandler<RedTreeGridContextMenuEventArgs>? TreeGridContextMenuOpening;

    #region DependencyProperties

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source), typeof(PropertyViewModel), typeof(RedTreeGrid), new PropertyMetadata(null, OnSourceChanged));

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not RedTreeGrid view)
        {
            return;
        }

        view.RefreshData();
    }

    public PropertyViewModel? Source
    {
        get => (PropertyViewModel?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(RedTreeGrid));

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }


    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.Register(nameof(SelectedItems), typeof(object), typeof(RedTreeGrid));

    public object? SelectedItems
    {
        get => GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    #endregion DependencyProperties

    #region Events

    public event EventHandler<GridSelectionChangedEventArgs>? SelectionChanged;

    #endregion Events

    public ObservableCollection<PropertyViewModel> Nodes { get; } = new();

    private void RefreshData()
    {
        Nodes.Clear();

        if (Source == null)
        {
            return;
        }
        
        foreach (var viewModel in Source.DisplayCollection)
        {
            Nodes.Add(viewModel);
        }
    }

    private void RedTreeView_OnNodeExpanded(object? sender, NodeExpandedEventArgs e)
    {
        if (e.Node.ChildNodes.Count == 1)
        {
            RedTreeView.ExpandNode(e.Node.ChildNodes[0]);
        }
    }

    private void RedTreeView_OnSelectionChanged(object? sender, GridSelectionChangedEventArgs e) => SelectionChanged?.Invoke(this, e);

    private void RedTreeView_OnTreeGridContextMenuOpening(object? sender, TreeGridContextMenuEventArgs e)
    {
        var node = RedTreeView.GetNodeAtRowIndex(e.RowColumnIndex.RowIndex);
        if (node.Item is not PropertyViewModel propertyViewModel)
        {
            e.Handled = true;
            return;
        }

        e.ContextMenu.Items.Clear();

        if (propertyViewModel.Parent is ArrayViewModel parentArray)
        {
            //e.ContextMenu.Items.Add(new MenuItem() { Header = "Delete array Item", Command = parentArray.DeleteArrayItemCommand, CommandParameter = propertyViewModel });
            //e.ContextMenu.Items.Add(new Separator());
        }

        if (propertyViewModel is ArrayViewModel arrayPropertyViewModel)
        {
            //e.ContextMenu.Items.Add(new MenuItem() { Header = "Add item", Command = arrayPropertyViewModel.AddArrayItemCommand });
            //e.ContextMenu.Items.Add(new MenuItem() { Header = "Clear", Command = arrayPropertyViewModel.ClearArrayCommand });
        }

        var args = new RedTreeGridContextMenuEventArgs(propertyViewModel, e.ContextMenu);
        TreeGridContextMenuOpening?.Invoke(sender, args);

        if (args.Handled || e.ContextMenu.Items.Count == 0)
        {
            e.Handled = true;
        }
    }

    public void SelectTreeItem(PropertyViewModel item)
    {
        var items = new List<PropertyViewModel>();

        while (true)
        {
            items.Add(item);
            if (item.Parent == null)
            {
                break;
            }
            item = item.Parent;
        }

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
}

public class RedTreeGridContextMenuEventArgs
{
    public RedTreeGridContextMenuEventArgs(PropertyViewModel propertyViewModel, ContextMenu contextMenu)
    {
        PropertyViewModel = propertyViewModel;
        ContextMenu = contextMenu;
    }

    public PropertyViewModel PropertyViewModel { get; set; }
    public ContextMenu ContextMenu { get; set; }
    public bool Handled { get; set; }
}
