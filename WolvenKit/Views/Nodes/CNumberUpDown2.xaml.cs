using System;
using System.Collections.Generic;
using System.Globalization;
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
using Syncfusion.Licensing.math;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Nodes;
/// <summary>
/// Interaktionslogik für CNumberUpDown2.xaml
/// </summary>
public partial class CNumberUpDown2 : UserControl
{
    public IRedInteger RedInteger
    {
        get => (IRedInteger)GetValue(RedIntegerProperty);
        set => SetValue(RedIntegerProperty, value);
    }

    public static readonly DependencyProperty RedIntegerProperty =
        DependencyProperty.Register(nameof(RedInteger), typeof(IRedInteger),
            typeof(CNumberUpDown2), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRedIntegerChanged));

    private static void OnRedIntegerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CNumberUpDown2 view)
        {
            return;
        }

        view.UpdateView();
    }

    private decimal _minValue = 0;
    private decimal _maxValue = 0;
    private decimal _value = 0;

    public decimal Value
    {
        get => _value;
        set
        {
            _value = value;
            TextBoxNum.SetValue(TextBox.TextProperty, _value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public CNumberUpDown2()
    {
        InitializeComponent();

        TextBoxNum.TextChanged += TextBoxNum_OnTextChanged;
        ButtonUp.Click += ButtonUp_OnClick;
        ButtonDown.Click += ButtonDown_OnClick;
    }

    private void TextBoxNum_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (decimal.TryParse(TextBoxNum.Text, out var dec))
        {
            if (dec > _maxValue)
            {
                dec = _maxValue;
            }

            if (dec < _minValue)
            {
                dec = _minValue;
            }

            _value = dec;

            if (RedInteger is CInt64)
            {
                SetCurrentValue(RedIntegerProperty, (CInt64)Value);
            }

            if (RedInteger is CUInt64)
            {
                SetCurrentValue(RedIntegerProperty, (CUInt64)Value);
            }

            if (RedInteger is CRUID)
            {
                SetCurrentValue(RedIntegerProperty, (CRUID)Value);
            }
        }

        TextBoxNum.SetCurrentValue(TextBox.TextProperty, _value.ToString(CultureInfo.InvariantCulture));
    }

    private void ButtonUp_OnClick(object sender, RoutedEventArgs e)
    {
        if (Value + 1 <= _maxValue)
        {
            Value++;
        }
    }

    private void ButtonDown_OnClick(object sender, RoutedEventArgs e)
    {
        if (Value - 1 >= _minValue)
        {
            Value--;
        }
    }

    private void UpdateView()
    {
        if (RedInteger is CInt64 i64)
        {
            _minValue = long.MinValue;
            _maxValue = long.MaxValue;
            Value = i64;
        }

        if (RedInteger is CUInt64 u64)
        {
            _minValue = ulong.MinValue;
            _maxValue = ulong.MaxValue;
            Value = u64;
        }

        if (RedInteger is CRUID cruid)
        {
            _minValue = ulong.MinValue;
            _maxValue = ulong.MaxValue;
            Value = cruid;
        }
    }
}
