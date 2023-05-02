using System;
using System.Windows;
using Syncfusion.Windows.Shared;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Nodes;

public class CNumberUpDown : UpDown
{
    public IRedInteger RedInteger
    {
        get => (IRedInteger)GetValue(RedIntegerProperty);
        set => SetValue(RedIntegerProperty, value);
    }

    public static readonly DependencyProperty RedIntegerProperty =
        DependencyProperty.Register(nameof(RedInteger), typeof(IRedInteger),
            typeof(CNumberUpDown), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRedIntegerChanged));

    private static void OnRedIntegerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CNumberUpDown view)
        {
            return;
        }

        view.UpdateView();
    }

    public CNumberUpDown()
    {
        ValueChanged += OnValueChanged;
    }

    private void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not double dv)
        {
            return;
        }

        if (RedInteger is CDouble)
        {
            SetCurrentValue(RedIntegerProperty, (CDouble)dv);
            return;
        }

        if (RedInteger is CFloat)
        {
            SetCurrentValue(RedIntegerProperty, (CFloat)(float)dv);
            return;
        }

        if (RedInteger is CInt8)
        {
            SetCurrentValue(RedIntegerProperty, (CInt8)(sbyte)dv);
            return;
        }

        if (RedInteger is CUInt8)
        {
            SetCurrentValue(RedIntegerProperty, (CUInt8)(byte)dv);
            return;
        }

        if (RedInteger is CInt16)
        {
            SetCurrentValue(RedIntegerProperty, (CInt16)(short)dv);
            return;
        }

        if (RedInteger is CUInt16)
        {
            SetCurrentValue(RedIntegerProperty, (CUInt16)(ushort)dv);
            return;
        }

        if (RedInteger is CInt32)
        {
            SetCurrentValue(RedIntegerProperty, (CInt32)(int)dv);
            return;
        }

        if (RedInteger is CUInt32)
        {
            SetCurrentValue(RedIntegerProperty, (CUInt32)(uint)dv);
            return;
        }

        if (RedInteger is CInt64)
        {
            SetCurrentValue(RedIntegerProperty, (CInt64)(long)dv);
            return;
        }

        if (RedInteger is CUInt64)
        {
            SetCurrentValue(RedIntegerProperty, (CUInt64)(ulong)dv);
            return;
        }
    }

    private void UpdateView()
    {
        if (RedInteger == null)
        {
            return;
        }

        if (RedInteger is CDouble d)
        {
            SetCurrentValue(MaxValueProperty, double.MaxValue);
            SetCurrentValue(MinValueProperty, double.MinValue);
            SetCurrentValue(NumberDecimalDigitsProperty, 17);
            SetCurrentValue(ValueProperty, (double)d);
            return;
        }

        if (RedInteger is CFloat f)
        {
            SetCurrentValue(MaxValueProperty, (double)float.MaxValue);
            SetCurrentValue(MinValueProperty, (double)float.MinValue);
            SetCurrentValue(NumberDecimalDigitsProperty, 9);
            SetCurrentValue(ValueProperty, (double)(float)f);
            return;
        }

        SetCurrentValue(NumberDecimalDigitsProperty, 0);

        if (RedInteger is CInt8 i8)
        {
            SetCurrentValue(MaxValueProperty, (double)sbyte.MaxValue);
            SetCurrentValue(MinValueProperty, (double)sbyte.MinValue);
            SetCurrentValue(ValueProperty, (double)(sbyte)i8);
            return;
        }

        if (RedInteger is CUInt8 u8)
        {
            SetCurrentValue(MaxValueProperty, (double)byte.MaxValue);
            SetCurrentValue(MinValueProperty, (double)byte.MinValue);
            SetCurrentValue(ValueProperty, (double)(byte)u8);
            return;
        }

        if (RedInteger is CInt16 i16)
        {
            SetCurrentValue(MaxValueProperty, (double)short.MaxValue);
            SetCurrentValue(MinValueProperty, (double)short.MinValue);
            SetCurrentValue(ValueProperty, (double)(short)i16);
            return;
        }

        if (RedInteger is CUInt16 u16)
        {
            SetCurrentValue(MaxValueProperty, (double)ushort.MaxValue);
            SetCurrentValue(MinValueProperty, (double)ushort.MinValue);
            SetCurrentValue(ValueProperty, (double)(ushort)u16);
            return;
        }

        if (RedInteger is CInt32 i32)
        {
            SetCurrentValue(MaxValueProperty, (double)int.MaxValue);
            SetCurrentValue(MinValueProperty, (double)int.MinValue);
            SetCurrentValue(ValueProperty, (double)(int)i32);
            return;
        }

        if (RedInteger is CUInt32 u32)
        {
            SetCurrentValue(MaxValueProperty, (double)uint.MaxValue);
            SetCurrentValue(MinValueProperty, (double)uint.MinValue);
            SetCurrentValue(ValueProperty, (double)(uint)u32);
            return;
        }

        if (RedInteger is CInt64 i64)
        {
            SetCurrentValue(MaxValueProperty, (double)long.MaxValue);
            SetCurrentValue(MinValueProperty, (double)long.MinValue);
            SetCurrentValue(ValueProperty, (double)(long)i64);
            return;
        }

        if (RedInteger is CUInt64 u64)
        {
            SetCurrentValue(MaxValueProperty, (double)ulong.MaxValue);
            SetCurrentValue(MinValueProperty, (double)ulong.MinValue);
            SetCurrentValue(ValueProperty, (double)(ulong)u64);
            return;
        }
    }
}