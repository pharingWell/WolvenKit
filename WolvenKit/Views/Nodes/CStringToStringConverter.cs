using System.Globalization;
using System.Windows.Data;
using System;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Nodes;

public class CStringToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not CString cStr)
        {
            throw new NotSupportedException();
        }

        return (string)cStr;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string str)
        {
            throw new NotSupportedException();
        }

        return (CString)str;
    }
}