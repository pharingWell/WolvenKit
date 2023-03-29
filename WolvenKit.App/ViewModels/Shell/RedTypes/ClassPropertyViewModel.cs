using System;
using System.Linq;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell;

public class ClassPropertyViewModel<T> : PropertyViewModel<T> where T : RedBaseClass
{
    public ClassPropertyViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, T? data) : base(parent, redPropertyInfo, data)
    {
    }

    protected override void SetValue(PropertyViewModel propertyViewModel)
    {
        _data!.SetProperty(propertyViewModel.DisplayName, (IRedType?)propertyViewModel.DataObject);
        OnPropertyChanged(nameof(DataObject));
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

    protected internal override void UpdateDisplayValue(string? suffix = null)
    {
        DisplayValue = $"{PrettyValue(DataObject?.ToString())} {Properties.FirstOrDefault(x => x.DisplayName == "debugName")?.DisplayValue}";
        if (!string.IsNullOrEmpty(suffix))
        {
            DisplayValue += $" {suffix}";
        }
    }
}

public class ClassPropertyViewModel : ClassPropertyViewModel<RedBaseClass>
{
    public ClassPropertyViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, RedBaseClass? data) : base(parent, redPropertyInfo, data)
    {
    }
}