using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using WolvenKit.Core.Extensions;
using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Documents;

public abstract class TweakEntryViewModel : ObservableObject
{
    protected TweakEntryViewModel(string name) => Name = name;

    public string Name { get; set; }

    public abstract string DisplayString { get; }
    public abstract string DisplayType { get; }

    public bool IsSelected { get; set; }
}

public sealed class GroupViewModel : TweakEntryViewModel
{
    private readonly gamedataTweakDBRecord _value;

    public GroupViewModel(string name, gamedataTweakDBRecord value) : base(name)
    {
        _value = value;

        Members = new ObservableCollection<FlatViewModel>(_value.GetPropertyNames()
            .Select(f => new FlatViewModel(f, _value.GetProperty(f).NotNull())
            {
                GroupName = Name
            }));

    }

    public ObservableCollection<FlatViewModel> Members { get; set; }

    public override string DisplayString => _value.ToString().NotNull();
    public override string DisplayType => _value.GetType().Name;

    public gamedataTweakDBRecord GetValue() => _value;
}

public sealed class FlatViewModel : TweakEntryViewModel
{
    private readonly IRedType _value;

    public FlatViewModel(string name, IRedType value) : base(name)
    {
        _value = value;

        if (value is IRedBaseArray array)
        {
            Members = new ObservableCollection<FlatViewModel>(array
                .OfType<IRedType>()
                .Select(f => new FlatViewModel(RedReflection.GetRedTypeFromCSType(f.GetType()), f)
                {
                    ArrayName = Name
                }));
        }

    }

    public ObservableCollection<FlatViewModel> Members { get; set; } = new();


    public override string DisplayString => _value.ToString().NotNull();
    public override string DisplayType => RedReflection.GetRedTypeFromCSType(_value.GetType());

    public bool IsArray => _value is IRedBaseArray array;

    public IRedType GetValue() => _value;

    public string? GroupName { get; set; }
    public string? ArrayName { get; set; }
}
