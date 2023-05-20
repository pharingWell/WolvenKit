using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell.RedTypes;

public class DefaultPropertyViewModel : PropertyViewModel<IRedType>
{
    public DefaultPropertyViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, IRedType? data) : base(parent, redPropertyInfo, data)
    {
    }

    protected internal override void FetchProperties()
    {

    }
}