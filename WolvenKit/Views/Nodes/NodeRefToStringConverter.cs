using System;
using System.Globalization;
using System.Windows.Data;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Nodes;

public class NodeRefToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not NodeRef nodeRef)
        {
            throw new NotSupportedException();
        }

        return nodeRef.GetResolvedText()!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string str)
        {
            throw new NotSupportedException();
        }

        return (NodeRef)str;
    }
}