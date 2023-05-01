using System;
using System.Globalization;
using System.Windows.Data;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Nodes;

public class CBoolToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not CBool cBool)
        {
            throw new NotSupportedException();
        }

        return (bool)cBool;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool b)
        {
            throw new NotSupportedException();
        }

        return (CBool)b;
    }
}