using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell.RedTypes;

public abstract class PropertyViewModel : INotifyPropertyChanging, INotifyPropertyChanged, IDisposable
{
    protected IRedType? _dataObject;
    private string _displayName = "NONE";
    private string? _displayValue = "";
    private string? _displayType = "";
    private bool _isDefault;

    public PropertyViewModel? Parent { get; }

    public RedPropertyInfo RedPropertyInfo { get; }

    public IRedType? DataObject
    {
        get => _dataObject;
        set => SetField(ref _dataObject, value);
    }

    public string DisplayName
    {
        get => _displayName;
        set => SetField(ref _displayName, value);
    }

    public string? DisplayValue
    {
        get => _displayValue;
        set => SetField(ref _displayValue, value);
    }

    public string? DisplayType
    {
        get => _displayType;
        set => SetField(ref _displayType, value);
    }

    public bool IsDefault
    {
        get => _isDefault;
        protected set => SetField(ref _isDefault, value);
    }

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

    public PropertyViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, IRedType? data)
    {
        Parent = parent;
        RedPropertyInfo = redPropertyInfo;
        _dataObject = data;
    }

    protected internal abstract void FetchProperties();

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
    }

    protected virtual void SetValue(PropertyViewModel propertyViewModel) => throw new NotImplementedException(nameof(SetValue));

    protected string PrettyValue(string? str)
    {
        if (str != null)
        {
            return str.Replace("WolvenKit.RED4.Types.", "");
        }

        return "null";
    }

    #region INotifyProperty

    public event PropertyChangingEventHandler? PropertyChanging;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanging([CallerMemberName] string? propertyName = null) =>
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (propertyName == nameof(DataObject))
        {
            UpdateInfos();
            SetIsDefault();

            Parent?.SetValue(this);
        }
        
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        OnPropertyChanging(propertyName);
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion INotifyProperty

    #region IDisposable

    protected bool _disposedValue;

    ~PropertyViewModel() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                foreach (var propertyViewModel in Properties)
                {
                    propertyViewModel.Dispose();
                }
            }

            _disposedValue = true;
        }
    }

    #endregion IDisposable

    private void AddToCache()
    {
        if (_dataObject == null)
        {
            return;
        }

        if (!s_cache.ContainsKey(_dataObject))
        {
            s_cache.TryAdd(_dataObject, this);
        }
    }

    private void RemoveFromCache()
    {
        if (_dataObject == null)
        {
            return;
        }

        if (s_cache.ContainsKey(_dataObject))
        {
            s_cache.TryRemove(_dataObject, out _);
        }
    }

    #region Static

    private static readonly ConcurrentDictionary<IRedType, PropertyViewModel> s_cache = new();

    public static PropertyViewModel Create(IRedType data) => Create(null, new RedPropertyInfo(data), data);

    public static PropertyViewModel Create(PropertyViewModel? parent, RedPropertyInfo propertyInfo, IRedType? data, bool fetchData = true)
    {
        PropertyViewModel? result = null;
        if (typeof(RedBaseClass).IsAssignableFrom(propertyInfo.BaseType))
        {
            result = new ClassViewModel(parent, propertyInfo, (RedBaseClass?)data);
        }
        else if (typeof(IRedBaseArray).IsAssignableFrom(propertyInfo.BaseType))
        {
            result = new ArrayViewModel(parent, propertyInfo, (IRedBaseArray?)data);
        }
        else if (typeof(IRedHandle).IsAssignableFrom(propertyInfo.BaseType))
        {
            result = new HandleViewModel(parent, propertyInfo, (IRedHandle?)data);
        }
        else if (typeof(IRedWeakHandle).IsAssignableFrom(propertyInfo.BaseType))
        {
            result = new WeakHandleViewModel(parent, propertyInfo, (IRedWeakHandle?)data);
        }

        result ??= new DefaultPropertyViewModel(parent, propertyInfo, data);

        if (fetchData)
        {
            result.FetchProperties();
            result.UpdateInfos();
            result.SetIsDefault();
        }

        return result;
    }

    #endregion Static
}

public abstract class PropertyViewModel<TRedType> : PropertyViewModel where TRedType : IRedType?
{
    protected TRedType? _data => (TRedType?)DataObject;

    protected PropertyViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, TRedType? data) : base(parent, redPropertyInfo, data)
    {
    }
}