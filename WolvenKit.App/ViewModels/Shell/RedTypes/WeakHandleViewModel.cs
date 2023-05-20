using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell.RedTypes;

public class WeakHandleViewModel : PropertyViewModel<IRedWeakHandle>
{
    public RedBaseClass? Chunk => _data?.GetValue();

    public WeakHandleViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, IRedWeakHandle? data) : base(parent, redPropertyInfo, data)
    {
    }

    protected internal override void FetchProperties()
    {
    }
}