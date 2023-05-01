using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Nodes;

public class CEnumComboBox : ComboBox
{
    public IRedEnum RedEnum
    {
        get => (IRedEnum)GetValue(RedEnumProperty);
        set => SetValue(RedEnumProperty, value);
    }

    public static readonly DependencyProperty RedEnumProperty =
        DependencyProperty.Register(nameof(RedEnum), typeof(IRedEnum),
            typeof(CEnumComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRedEnumChanged));

    private static void OnRedEnumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CEnumComboBox view)
        {
            return;
        }

        if (e.NewValue == null)
        {
            view.SetCurrentValue(ItemsSourceProperty, null);
            view.SetCurrentValue(SelectedItemProperty, null);

            return;
        }

        if (e.NewValue is not IRedEnum newData)
        {
            throw new NotSupportedException();
        }

        if (e.OldValue == null || e.OldValue.GetType() != e.NewValue.GetType())
        {
            var values = new List<string>();
            foreach (var s in Enum.GetNames(newData.GetInnerType()))
            {
                values.Add(s);
            }
            view.SetCurrentValue(ItemsSourceProperty, values);
        }

        view.SetCurrentValue(SelectedItemProperty, newData.ToEnumString());
    }

    public CEnumComboBox()
    {
        SelectionChanged += CEnumComboBox_SelectionChanged;

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

    private void CEnumComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RedEnum == null)
        {
            return;
        }

        SetCurrentValue(RedEnumProperty, CEnum.Parse(RedEnum.GetInnerType(), (string)SelectedItem));
    }
}