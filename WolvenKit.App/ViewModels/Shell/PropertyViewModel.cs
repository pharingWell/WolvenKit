using System.Collections.ObjectModel;
using System.ComponentModel;
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

    public bool IsDefault => RedPropertyInfo.ExtendedPropertyInfo?.IsDefault(DataObject) ?? true;


    public ObservableCollection<PropertyViewModel> Properties { get; } = new();

    
    public PropertyViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, object? data)
    {
        Parent = parent;
        RedPropertyInfo = redPropertyInfo;
        _dataObject = data;
        
        FetchProperties();
        UpdateInfos();

        PropertyChanged += delegate(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(DataObject))
            {
                OnPropertyChanged(nameof(IsDefault));
            }
        };
    }

    protected abstract void FetchProperties();

    protected virtual void UpdateInfos()
    {
        DisplayName = "";
        DisplayValue = $"{PrettyValue(DataObject?.ToString())} {GetDisplayProperty()}";
        DisplayType = RedPropertyInfo.RedTypeName;

        if (RedPropertyInfo.Index != -1)
        {
            DisplayName = $"[{RedPropertyInfo.Index}] ";
        }

        if (RedPropertyInfo.ExtendedPropertyInfo != null)
        {
            if (!string.IsNullOrEmpty(RedPropertyInfo.ExtendedPropertyInfo.RedName))
            {
                DisplayName += RedPropertyInfo.ExtendedPropertyInfo.RedName;
            }
        }

        if (string.IsNullOrEmpty(DisplayName))
        {
            DisplayName = "ROOT";
        }
    }

    protected virtual string? GetDisplayProperty() => null;

    protected string PrettyValue(string? str)
    {
        if (str != null)
        {
            return str.Replace("WolvenKit.RED4.Types.", "");
        }

        return "null";
    }

    public static PropertyViewModel Create(PropertyViewModel? parent, RedPropertyInfo propertyInfo, IRedType? data)
    {
        if (data is RedBaseClass redBaseClass)
        {
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