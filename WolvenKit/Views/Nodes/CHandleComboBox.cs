using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Nodes;

public class CHandleComboBox : ComboBox
{
    private Dictionary<string, Type> _types;

    public IRedHandle RedHandle
    {
        get => (IRedHandle)GetValue(RedHandleProperty);
        set => SetValue(RedHandleProperty, value);
    }

    public static readonly DependencyProperty RedHandleProperty =
        DependencyProperty.Register(nameof(RedHandle), typeof(IRedHandle),
            typeof(CHandleComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRedHandleChanged));

    private static void OnRedHandleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CHandleComboBox view)
        {
            return;
        }

        view.UpdateTypes();
    }

    public CHandleComboBox()
    {
        SelectionChanged += CHandleComboBox_SelectionChanged;

        Dispatcher.BeginInvoke(() =>
        {
            if (Template.FindName("toggleButton", this) is ToggleButton toggleButton)
            {
                if (toggleButton.Template.FindName("templateRoot", toggleButton) is Border border)
                {
                    border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF121212")!);
                }
            }
        }, DispatcherPriority.ContextIdle);
    }

    private void CHandleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedItem is not string str)
        {
            return;
        }

        if (!_types.TryGetValue(str, out var targetType))
        {
            return;
        }

        if (RedHandle.GetValue()?.GetType() != targetType)
        {
            RedHandle.SetValue(targetType != null ? RedTypeManager.Create(targetType) : null);
        }

        e.Handled = true;
    }

    private void UpdateTypes()
    {
        if (RedHandle == null)
        {
            return;
        }

        _types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => RedHandle.InnerType.IsAssignableFrom(x) && !x.IsAbstract)
            .ToDictionary(x => x.Name, x => x);

        _types.Add("null", null);

        SetCurrentValue(ItemsSourceProperty, _types.Keys);

        var current = RedHandle.GetValue();
        SetCurrentValue(SelectedItemProperty, current != null ? current.GetType().Name : "null");
    }
}