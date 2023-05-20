using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.TreeGrid;
using WolvenKit.App.Helpers;
using WolvenKit.App.Models;
using WolvenKit.App.ViewModels.Shell.RedTypes;
using WolvenKit.RED4.Types;
using WolvenKit.Views.UserControls;

namespace WolvenKit.Views.Tools;
/// <summary>
/// Interaktionslogik für RedTreeView2.xaml
/// </summary>
public partial class RedTreeView2 : UserControl
{
    #region DependencyProperties

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source), typeof(IRedType), typeof(RedTreeView2), new PropertyMetadata(null, OnSourceChanged));

    public IRedType Source
    {
        get => (IRedType)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
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
        Navigator.TextPathChanged += Navigator_OnTextPathChanged;

        RedTreeGrid.SelectionChanged += RedTreeGrid_OnSelectionChanged;
        RedTreeGrid.TreeGridContextMenuOpening += RedTreeGrid_OnTreeGridContextMenuOpening;
    }

    private void RedTreeGrid_OnTreeGridContextMenuOpening(object sender, RedTreeGridContextMenuEventArgs args)
    {
        if (args.PropertyViewModel is WeakHandleViewModel weakHandleViewModel)
        {
            var item = new MenuItem { Header = "Find references" };
            item.Click += (_, _) =>
            {
                if (SearchResults == null)
                {
                    SetCurrentValue(SearchResultsProperty, new ObservableCollection<SearchResult>());
                }

                SearchResults!.Clear();
                foreach (var propertyViewModel in FindReferences(null, weakHandleViewModel.Chunk))
                {
                    SearchResults.Add(new SearchResult($"{{{BuildPath(propertyViewModel)}}} {propertyViewModel.DisplayValue}", propertyViewModel));
                }
            };
            args.ContextMenu.Items.Add(item);
        }
    }

    private IEnumerable<PropertyViewModel> FindReferences(PropertyViewModel source, IRedType data)
    {
        source ??= _sourceViewModel;

        if (source is HandleViewModel hvm && ReferenceEquals(hvm.Chunk, data))
        {
            yield return source;
        }

        if (source is WeakHandleViewModel whvm && ReferenceEquals(whvm.Chunk, data))
        {
            yield return source;
        }

        foreach (var child in source.Properties)
        {
            foreach (var childProperty in FindReferences(child, data))
            {
                yield return childProperty;
            }
        }
    }

    private void Navigator_OnSelectedItemChanged(object sender, EventArgs e)
    {
        if (Navigator.SelectedItem is not { } propertyViewModel)
        {
            return;
        }

        RedTreeGrid.SelectTreeItem(propertyViewModel);
    }

    private void Navigator_OnTextPathChanged(object sender, Breadcrumb.TextPathEventArgs e)
    {
        var parts = e.Path.Split('\\');

        PropertyViewModel item = null;
        IList<PropertyViewModel> items = new List<PropertyViewModel> { PropertyViewModel.Create(Source) };
        foreach (var part in parts)
        {
            var prop = items.FirstOrDefault(x => x.DisplayName == part);
            if (prop != null)
            {
                item = prop;
                items = prop.Properties;
            }
            else
            {
                return;
            }
        }

        if (item != null)
        {
            RedTreeGrid.SelectTreeItem(item);
            e.Handled = true;
        }
    }

    private PropertyViewModel _sourceViewModel;

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not RedTreeView2 view)
        {
            return;
        }

        view._sourceViewModel = PropertyViewModel.Create(view.Source);

        view.Navigator.SetCurrentValue(Breadcrumb.ItemsSourceProperty, new List<PropertyViewModel> { view._sourceViewModel });
        view.RedTreeGrid.SetCurrentValue(RedTreeGrid.SourceProperty, view._sourceViewModel);
    }

    private void RedTreeGrid_OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
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
        if (_sourceViewModel is not { } root)
        {
            return;
        }

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

        RedTreeGrid.SelectTreeItem(result.Data);
    }

    public record SearchResult(string Name, PropertyViewModel Data);
}