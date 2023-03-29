using CommunityToolkit.Mvvm.Input;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell;

public partial class ArrayPropertyViewModel : PropertyViewModel<IRedArray>
{
    public ArrayPropertyViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, IRedArray? data) : base(parent, redPropertyInfo, data)
    {
    }

    protected override void SetValue(PropertyViewModel propertyViewModel)
    {
        _data![propertyViewModel.RedPropertyInfo.Index] = propertyViewModel.DataObject;
    }

    protected override void FetchProperties()
    {
        if (_data == null)
        {
            return;
        }

        Properties.Clear();

        for (var i = 0; i < _data.Count; i++)
        {
            var entry = (IRedType?)_data[i];

            if (entry != null)
            {
                Properties.Add(Create(this, new RedPropertyInfo(entry) { Index = i }, entry));
            }
            else
            {
                Properties.Add(Create(this, new RedPropertyInfo(_data.InnerType) { Index = i }, entry));
            }
        }
    }

    protected internal override void UpdateDisplayValue(string? suffix = null)
    {
        DisplayValue = _data != null ? $"Count = {_data.Count}" : "null";
    }

    [RelayCommand]
    public void AddArrayItem()
    {

    }

    [RelayCommand]
    public void ClearArray()
    {
        if (_data == null || _data.Count == 0)
        {
            return;
        }

        _data.Clear();

        FetchProperties();
        UpdateInfos();
    }

    [RelayCommand]
    public void DeleteArrayItem(PropertyViewModel child)
    {
        _data!.Remove(child.DataObject);

        FetchProperties();
        UpdateInfos();
    }
}