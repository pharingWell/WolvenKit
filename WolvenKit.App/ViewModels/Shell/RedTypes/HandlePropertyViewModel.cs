using System;
using System.Linq;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell;

public class HandlePropertyViewModel : PropertyViewModel<IRedBaseHandle>
{
    public HandlePropertyViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, IRedBaseHandle? data) : base(parent, redPropertyInfo, data)
    {
    }

    protected override void FetchProperties()
    {
        if (_data == null)
        {
            return;
        }

        ExtendedTypeInfo typeInfo;
        if (_data != null && _data.GetValue() != null)
        {
            typeInfo = RedReflection.GetTypeInfo(_data.GetValue());
        }
        else
        {
            throw new ArgumentNullException(nameof(typeInfo));
        }

        Properties.Clear();

        foreach (var propertyInfo in typeInfo.PropertyInfos)
        {
            ArgumentNullException.ThrowIfNull(propertyInfo.RedName);

            Properties.Add(Create(this, new RedPropertyInfo(propertyInfo), _data.GetValue().GetProperty(propertyInfo.RedName)));
        }

        foreach (var propertyInfo in typeInfo.DynamicPropertyInfos)
        {
            ArgumentNullException.ThrowIfNull(propertyInfo.RedName);

            Properties.Add(Create(this, new RedPropertyInfo(propertyInfo), _data.GetValue().GetProperty(propertyInfo.RedName)));
        }
    }

    protected override void UpdateInfos()
    {
        base.UpdateInfos();

        DisplayValue = _data?.GetValue() != null ? $"{{{PrettyValue(_data.GetValue().ToString())}}} {GetDisplayProperty()}" : "null";
    }

    protected override string? GetDisplayProperty() => Properties.FirstOrDefault(x => x.DisplayName == "debugName")?.DisplayValue;
}