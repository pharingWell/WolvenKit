using System;
using System.Linq;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell;

public class ClassPropertyViewModel : PropertyViewModel<RedBaseClass>
{
    public ClassPropertyViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, RedBaseClass? data) : base(parent, redPropertyInfo, data)
    {
    }

    protected override void FetchProperties()
    {
        if (_data == null)
        {
            return;
        }

        ExtendedTypeInfo typeInfo;
        if (RedPropertyInfo.ExtendedPropertyInfo != null)
        {
            typeInfo = RedReflection.GetTypeInfo(RedPropertyInfo.ExtendedPropertyInfo.Type);
        }
        else if (_data != null)
        {
            typeInfo = RedReflection.GetTypeInfo(_data);
        }
        else
        {
            throw new ArgumentNullException(nameof(typeInfo));
        }

        Properties.Clear();

        foreach (var propertyInfo in typeInfo.PropertyInfos)
        {
            ArgumentNullException.ThrowIfNull(propertyInfo.RedName);

            Properties.Add(Create(this, new RedPropertyInfo(propertyInfo), _data.GetProperty(propertyInfo.RedName)));
        }

        foreach (var propertyInfo in typeInfo.DynamicPropertyInfos)
        {
            ArgumentNullException.ThrowIfNull(propertyInfo.RedName);

            Properties.Add(Create(this, new RedPropertyInfo(propertyInfo), _data.GetProperty(propertyInfo.RedName)));
        }
    }

    protected override string? GetDisplayProperty() => Properties.FirstOrDefault(x => x.DisplayName == "debugName")?.DisplayValue;
}