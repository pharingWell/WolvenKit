using System;
using System.ComponentModel;
using System.Linq;
using WolvenKit.App.Helpers;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell.RedTypes;

public class HandleViewModel : PropertyViewModel<IRedHandle>
{
    private IRedHandle? _castedData => (IRedHandle?)_dataObject;

    private ClassViewModel _chunkViewModel;

    public RedBaseClass? Chunk => _data?.GetValue();

    public HandleViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, IRedHandle? data) : base(parent, redPropertyInfo, data)
    {
        _dataObject ??= RedTypeManager.CreateRedType(redPropertyInfo.ExtendedPropertyInfo!.Type);
        _chunkViewModel = (ClassViewModel)Create(this, new RedPropertyInfo(redPropertyInfo.InnerType!), _castedData!.GetValue(), false);

        _castedData.PropertyChanging += (sender, args) =>
        {
            Properties.Clear();
            _chunkViewModel.Dispose();
        };
        _castedData.PropertyChanged += (sender, args) =>
        {
            _chunkViewModel = (ClassViewModel)Create(this, new RedPropertyInfo(redPropertyInfo.InnerType!), _castedData!.GetValue());
            FetchProperties();
            UpdateInfos();
        };
    }

    protected override void SetValue(PropertyViewModel propertyViewModel) => _castedData!.SetValue((RedBaseClass?)_chunkViewModel.DataObject);

    protected override void UpdateInfos()
    {
        base.UpdateInfos();
        _chunkViewModel.DisplayName = DisplayName;
    }

    protected internal override void FetchProperties()
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

            Properties.Add(Create(_chunkViewModel, new RedPropertyInfo(propertyInfo), val.GetProperty(propertyInfo.RedName)));
        }

        foreach (var propertyInfo in typeInfo.DynamicPropertyInfos)
        {
            ArgumentNullException.ThrowIfNull(propertyInfo.RedName);

            Properties.Add(Create(_chunkViewModel, new RedPropertyInfo(propertyInfo), val.GetProperty(propertyInfo.RedName)));
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