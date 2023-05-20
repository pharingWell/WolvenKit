using System;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell.RedTypes;

public class RedPropertyInfo
{
    public ExtendedPropertyInfo? ExtendedPropertyInfo { get; }

    public int Index { get; set; } = -1;
    public Type BaseType { get; }
    public Type? InnerType { get; }
    public string RedTypeName { get; }

    public RedPropertyInfo(ExtendedPropertyInfo extendedPropertyInfo)
    {
        ExtendedPropertyInfo = extendedPropertyInfo;

        BaseType = ExtendedPropertyInfo.Type;
        if (BaseType.IsGenericType)
        {
            InnerType = BaseType.GetGenericArguments()[0];
            BaseType = BaseType.GetGenericTypeDefinition();
        }
        RedTypeName = RedReflection.GetRedTypeFromCSType(extendedPropertyInfo.Type, extendedPropertyInfo.Flags);
    }

    public RedPropertyInfo(Type type)
    {
        BaseType = type;
        if (BaseType.IsGenericType)
        {
            InnerType = BaseType.GetGenericArguments()[0];
            BaseType = BaseType.GetGenericTypeDefinition();
        }
        RedTypeName = RedReflection.GetRedTypeFromCSType(type);
    }

    public RedPropertyInfo(IRedType data)
    {
        BaseType = data.GetType();
        if (BaseType.IsGenericType)
        {
            InnerType = BaseType.GetGenericArguments()[0];
            BaseType = BaseType.GetGenericTypeDefinition();
        }
        RedTypeName = data.RedType;
    }
}