using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WolvenKit.App.ViewModels.Shell.RedTypes;

namespace WolvenKit.Views.UserControls;
/// <summary>
/// Interaktionslogik für Breadcrumb.xaml
/// </summary>
public partial class Breadcrumb : UserControl
{
    public event EventHandler<EventArgs> SelectedItemChanged;
    public event EventHandler<TextPathEventArgs> TextPathChanged;

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(List<PropertyViewModel>), typeof(Breadcrumb), new PropertyMetadata(OnItemsSourceChanged));

    public List<PropertyViewModel> ItemsSource
    {
        get => (List<PropertyViewModel>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), typeof(PropertyViewModel), typeof(Breadcrumb), new PropertyMetadata(OnSelectedItemChanged));

    public PropertyViewModel SelectedItem
    {
        get => (PropertyViewModel)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public Breadcrumb()
    {
        InitializeComponent();
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Breadcrumb view)
        {
            return;
        }

        view.BuildPanel();
    }

    private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Breadcrumb view)
        {
            return;
        }

        view.SelectedItemChanged?.Invoke(view, EventArgs.Empty);
    }

    private void BuildPanel()
    {
        HideEditor();
        SetCurrentValue(SelectedItemProperty, null);

        StackPanel.Children.Clear();
        foreach (var propertyViewModel in ItemsSource)
        {
            if (propertyViewModel is HandleViewModel)
            {
                continue;
            }

            AddNewElement(propertyViewModel.DisplayName, propertyViewModel);
            if (propertyViewModel.Properties.Count > 0)
            {
                AddNewElement(">", propertyViewModel);
            }
        }

        void AddNewElement(string text, PropertyViewModel propertyViewModel)
        {
            if (StackPanel.Children.Count > 0)
            {
                text = " " + text;
            }

            var tmp = new TextBlock { Text = text, Tag = propertyViewModel };
            tmp.PreviewMouseDown += Element_OnPreviewMouseDown;

            StackPanel.Children.Add(tmp);
        }
    }

    private Popup _popup;

    private void Element_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not TextBlock block || block.Tag is not PropertyViewModel propertyViewModel)
        {
            return;
        }

        if (_popup != null)
        {
            _popup.SetCurrentValue(Popup.IsOpenProperty, false);
        }

        if (block.Text.Trim() != ">")
        {
            SetCurrentValue(SelectedItemProperty, propertyViewModel);
        }
        else
        {
            var panel = new ListBox();
            foreach (var viewModel in propertyViewModel.Properties)
            {
                var child = new TextBlock { Text = viewModel.DisplayName, Tag = viewModel };
                child.PreviewMouseDown += Child_OnPreviewMouseDown;

                panel.Items.Add(child);
            }

            _popup = new Popup
            {
                MaxHeight = 200,
                Child = panel,
                PlacementTarget = block,
                IsOpen = true,
                StaysOpen = false
            };
        }
    }

    private void Child_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not TextBlock block || block.Tag is not PropertyViewModel propertyViewModel)
        {
            return;
        }

        _popup.SetCurrentValue(Popup.IsOpenProperty, false);

        SetCurrentValue(SelectedItemProperty, propertyViewModel);
    }

    private void Browse_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (StackPanel.Visibility == Visibility.Collapsed)
        {
            HideEditor();
        }
        else
        {
            ShowEditor();
        }
    }

    private void ShowEditor()
    {
        Editor.SetCurrentValue(VisibilityProperty, Visibility.Visible);
        StackPanel.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);

        var parts = new List<string>();
        foreach (var viewModel in ItemsSource)
        {
            parts.Add(viewModel.DisplayName);
        }
        Editor.SetCurrentValue(TextBox.TextProperty, string.Join('\\', parts));
    }

    private void HideEditor()
    {
        Editor.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
        StackPanel.SetCurrentValue(VisibilityProperty, Visibility.Visible);
    }

    private void Editor_OnTextChanged(object sender, TextChangedEventArgs e)
    {
    }

    private void Editor_OnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var args = new TextPathEventArgs(Editor.Text);
            TextPathChanged?.Invoke(this, args);
            if (!args.Handled)
            {

            }
        }
    }

    public class TextPathEventArgs : EventArgs
    {
        public string Path { get; }
        public bool Handled { get; set; }

        public TextPathEventArgs(string path)
        {
            Path = path;
        }
    }
}