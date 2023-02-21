using System;
using System.Reflection;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell;

public class ArrayPropertyViewModel : PropertyViewModel<IRedArray>
{
    public ArrayPropertyViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, IRedArray? data) : base(parent, redPropertyInfo, data)
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

    protected override void UpdateInfos()
    {
        base.UpdateInfos();

        DisplayValue = _data != null ? $"Count = {_data.Count}" : "null";
    }

    public int GetIndex(object? data) => _data!.IndexOf(data);
}