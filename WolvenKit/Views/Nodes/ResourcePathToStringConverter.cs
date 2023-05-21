using System.Globalization;
using System;
using System.Windows.Data;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Nodes;

public class ResourcePathToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not ResourcePath resourcePath)
        {
            throw new NotSupportedException();
        }

        if (resourcePath.IsResolvable)
        {
            return resourcePath.GetResolvedText()!;
        }

        if (resourcePath != 0)
        {
            return ((ulong)resourcePath).ToString();
        }

        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string str)
        {
            throw new NotSupportedException();
        }

        if (str == "")
        {
            return ResourcePath.Empty;
        }

        if (ulong.TryParse(str, out var hash))
        {
            return (ResourcePath)hash;
        }

        return (ResourcePath)ResourcePath.SanitizePath(str);
    }
}