using System;
using System.Globalization;
using System.Windows.Data;
using WolvenKit.RED4.Types;

namespace WolvenKit.Converters;

public class RedTypeToSymbolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Type type)
        {
            return "SymbolClass";
        }

        return type.IsAssignableTo(typeof(IRedInteger))
            ? "SymbolNumeric"
            : type.IsAssignableTo(typeof(IRedString))
            ? "SymbolString"
            : type.IsAssignableTo(typeof(IRedBaseArray))
            ? "SymbolArray"
            : type.IsAssignableTo(typeof(IRedEnum))
            ? "SymbolEnum"
            : type.IsAssignableTo(typeof(IRedRef))
            ? "FileSymlinkFile"
            : type.IsAssignableTo(typeof(IRedBitField))
            ? "SymbolEnum"
            : type.IsAssignableTo(typeof(CBool))
            ? "SymbolBoolean"
            : type.IsAssignableTo(typeof(IRedBaseHandle))
            ? "References"
            : type.IsAssignableTo(typeof(DataBuffer)) || type.IsAssignableTo(typeof(SerializationDeferredDataBuffer))
            ? "GroupByRefType"
            : type.IsAssignableTo(typeof(CResourceAsyncReference<>)) || type.IsAssignableTo(typeof(CResourceReference<>))
            ? "RepoPull"
            : type.IsAssignableTo(typeof(TweakDBID))
            ? "DebugBreakpointConditionalUnverified"
            : type.IsAssignableTo(typeof(IRedPrimitive))
            ? "DebugBreakpointDataUnverified"
            : type.IsAssignableTo(typeof(WorldTransform))
            ? "Compass"
            : type.IsAssignableTo(typeof(WorldPosition))
            ? "Move"
            : type.IsAssignableTo(typeof(Quaternion))
            ? "IssueReopened"
            : type.IsAssignableTo(typeof(CColor))
            ? "SymbolColor"
            : "SymbolClass";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}