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

    protected internal override void UpdateDisplayValue(string? suffix = null)
    {
        var displayValue = "null";
        if (_data?.GetValue() != null)
        {
            displayValue = $"{{{PrettyValue(_data.GetValue()!.ToString())}}}";
        }

        var additionalInfo = Properties.FirstOrDefault(x => x.DisplayName == "debugName")?.DisplayValue;
        if (!string.IsNullOrEmpty(additionalInfo))
        {
            displayValue += $" {additionalInfo}";
        }

        if (!string.IsNullOrEmpty(suffix))
        {
            displayValue += $" {suffix}";
        }

        DisplayValue = displayValue;
    }
}