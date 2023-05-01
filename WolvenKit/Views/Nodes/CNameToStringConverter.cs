using System;
using System.Globalization;
using System.Windows.Data;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Nodes;

public class CNameToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not CName cName)
        {
            throw new NotSupportedException();
        }

        return cName.GetResolvedText()!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string str)
        {
            throw new NotSupportedException();
        }

        return (CName)str;
    }
}