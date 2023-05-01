using System.Globalization;
using System.Windows.Data;
using System;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Nodes;

public class CInt32ToInt32Converter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not CInt32 cInt32)
        {
            throw new NotSupportedException();
        }

        return (int)cInt32;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not double i)
        {
            throw new NotSupportedException();
        }

        return (CInt32)i;
    }
}