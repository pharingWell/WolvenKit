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
        var val = _data?.GetValue();
        if (val == null)
        {
            return;
        }

        var typeInfo = RedReflection.GetTypeInfo(val);

        Properties.Clear();

        foreach (var propertyInfo in typeInfo.PropertyInfos)
        {
            ArgumentNullException.ThrowIfNull(propertyInfo.RedName);

            Properties.Add(Create(this, new RedPropertyInfo(propertyInfo), val.GetProperty(propertyInfo.RedName)));
        }

        foreach (var propertyInfo in typeInfo.DynamicPropertyInfos)
        {
            ArgumentNullException.ThrowIfNull(propertyInfo.RedName);

            Properties.Add(Create(this, new RedPropertyInfo(propertyInfo), val.GetProperty(propertyInfo.RedName)));
        }
    }

    protected override void UpdateInfos()
    {
        base.UpdateInfos();

        DisplayValue = _data?.GetValue() != null ? $"{{{PrettyValue(_data.GetValue()!.ToString())}}} {GetDisplayProperty()}" : "null";
    }

    protected override string? GetDisplayProperty() => Properties.FirstOrDefault(x => x.DisplayName == "debugName")?.DisplayValue;
}