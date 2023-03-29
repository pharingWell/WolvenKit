using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell;

public abstract partial class PropertyViewModel : ObservableObject
{
    public PropertyViewModel? Parent { get; }

    public RedPropertyInfo RedPropertyInfo { get; }
    [ObservableProperty] protected object? _dataObject;

    [ObservableProperty] private string _displayName = "NONE";
    [ObservableProperty] private string _displayValue = "";
    [ObservableProperty] private string _displayType = "";

    public bool IsDefault { get; protected set; }

    public ObservableCollection<PropertyViewModel> DisplayCollection
    {
        get
        {
            if (Properties.Count > 0)
            {
                return Properties;
            }

            return new ObservableCollection<PropertyViewModel> { this };
        }
    }


    public ObservableCollection<PropertyViewModel> Properties { get; } = new();

    
    public PropertyViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, object? data)
    {
        Parent = parent;
        RedPropertyInfo = redPropertyInfo;
        DataObject = data;

        PropertyChanged += OnPropertyChanged;

        FetchProperties();
        UpdateInfos();
        SetIsDefault();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(DataObject))
        {
            UpdateInfos();
            SetIsDefault();

            Parent?.SetValue(this);
        }
    }

    protected virtual void SetValue(PropertyViewModel propertyViewModel) => throw new NotImplementedException(nameof(SetValue));

    private void SetIsDefault()
    {
        if (RedPropertyInfo.ExtendedPropertyInfo != null)
        {
            IsDefault = RedPropertyInfo.ExtendedPropertyInfo.IsDefault(DataObject);
        }
        else if (DataObject != null && DataObject is not CKeyValuePair)
        {
            try
            {
                IsDefault = Equals(DataObject, RedTypeManager.CreateRedType(DataObject.GetType()));
            }
            catch (Exception)
            {
                // ignore
            }
        }
        else
        {
            IsDefault = false;
        }

        OnPropertyChanged(nameof(IsDefault));
    }

    protected abstract void FetchProperties();

    protected internal virtual void UpdateDisplayValue(string? suffix = null)
    {
        DisplayValue = $"{PrettyValue(DataObject?.ToString())}";
        if (!string.IsNullOrEmpty(suffix))
        {
            DisplayValue += $" {suffix}";
        }
    }

    protected virtual void UpdateInfos()
    {
        DisplayName = "";
        UpdateDisplayValue();
        DisplayType = RedPropertyInfo.RedTypeName;

        if (RedPropertyInfo.Index != -1)
        {
            DisplayName = $"[{RedPropertyInfo.Index}]";
        }

        if (RedPropertyInfo.ExtendedPropertyInfo != null)
        {
            if (!string.IsNullOrEmpty(RedPropertyInfo.ExtendedPropertyInfo.RedName))
            {
                if (!string.IsNullOrEmpty(DisplayName))
                {
                    DisplayName += " ";
                }

                DisplayName += RedPropertyInfo.ExtendedPropertyInfo.RedName;
            }
        }

        if (string.IsNullOrEmpty(DisplayName))
        {
            DisplayName = "ROOT";
        }
    }
    
    protected string PrettyValue(string? str)
    {
        if (str != null)
        {
            return str.Replace("WolvenKit.RED4.Types.", "");
        }

        return "null";
    }

    public PropertyViewModel GetRootModel()
    {
        var result = this;
        while (result.Parent != null)
        {
            result = result.Parent;
        }
        return result;
    }

    public PropertyViewModel? GetModelFromPath(string path)
    {
        var parts = path.Split('.');

        var result = this;
        foreach (var part in parts)
        {
            var newResult = result.Properties.FirstOrDefault(x => x.DisplayName == part);
            if (newResult == null)
            {
                return null;
            }

            result = newResult;
        }

        return result;
    }

    public static PropertyViewModel Create(PropertyViewModel? parent, RedPropertyInfo propertyInfo, IRedType? data)
    {
        if (data is RedBaseClass redBaseClass)
        {
            if (redBaseClass is CMesh mesh)
            {
                return new CMeshViewModel(parent, propertyInfo, mesh);
            }

            return new ClassPropertyViewModel(parent, propertyInfo, redBaseClass);
        }

        if (data is IRedArray redArray)
        {
            return new ArrayPropertyViewModel(parent, propertyInfo, redArray);
        }

        if (data is IRedBaseHandle handle)
        {
            return new HandlePropertyViewModel(parent, propertyInfo, handle);
        }

        return new DefaultPropertyViewModel(parent, propertyInfo, data);
    }
}

public abstract class PropertyViewModel<TRedType> : PropertyViewModel where TRedType : IRedType?
{
    protected TRedType? _data => (TRedType?)DataObject;

    protected PropertyViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, TRedType? data) : base(parent, redPropertyInfo, data)
    {
    }
}

public class PropertyViewModelChangedEventArgs : EventArgs
{
    public PropertyViewModelChangedEventArgs(ChangeType changeType, PropertyViewModel? oldPropertyViewModel, PropertyViewModel? newPropertyViewModel)
    {
        ChangeType = changeType;
        OldPropertyViewModel = oldPropertyViewModel;
        NewPropertyViewModel = newPropertyViewModel;
    }

    public ChangeType ChangeType { get; }
    public PropertyViewModel? OldPropertyViewModel { get; }
    public PropertyViewModel? NewPropertyViewModel { get; }
}

public enum ChangeType
{
    DataChanged
}