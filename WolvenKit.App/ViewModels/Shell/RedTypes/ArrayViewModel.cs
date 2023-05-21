using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell.RedTypes;

public class ArrayViewModel : PropertyViewModel<IRedBaseArray>
{
    public ArrayViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, IRedBaseArray? data) : base(parent, redPropertyInfo, data)
    {
        if (data != null)
        {
            data.CollectionChanged += DataOnCollectionChanged;
        }
    }

    private void DataOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateProperties();
        UpdateDisplayValue();
    }

    protected override void SetValue(PropertyViewModel propertyViewModel) => _data![propertyViewModel.RedPropertyInfo.Index] = propertyViewModel.DataObject;

    protected internal override void FetchProperties()
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

    private void UpdateProperties()
    {
        if (_data == null)
        {
            return;
        }

        var copy = new List<PropertyViewModel>(Properties);
        for (var i = 0; i < _data.Count; i++)
        {
            var entry = (IRedType?)_data[i];

            if (entry == null)
            {
                Properties.Insert(i, Create(this, new RedPropertyInfo(_data.InnerType) { Index = i }, entry));
            }
            else
            {
                var existing = Properties.FirstOrDefault(x => ReferenceEquals(x.DataObject, entry));
                if (existing != null)
                {
                    copy.Remove(existing);
                }
                else
                {
                    Properties.Insert(i, Create(this, new RedPropertyInfo(entry) { Index = i }, entry));
                }
            }
        }

        foreach (var viewModel in copy)
        {
            Properties.Remove(viewModel);
        }
    }

    protected internal override void UpdateDisplayValue(string? suffix = null)
    {
        DisplayValue = _data != null ? $"Count = {_data.Count}" : "null";
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (_data != null)
                {
                    _data.CollectionChanged -= DataOnCollectionChanged;
                }
            }

            base.Dispose(disposing);
        }
    }
}