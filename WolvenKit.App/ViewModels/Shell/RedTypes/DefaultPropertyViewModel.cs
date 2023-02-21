using WolvenKit.RED4.Types;

namespace WolvenKit.App.ViewModels.Shell;

public class DefaultPropertyViewModel : PropertyViewModel<IRedType>
{
    public DefaultPropertyViewModel(PropertyViewModel? parent, RedPropertyInfo redPropertyInfo, IRedType? data) : base(parent, redPropertyInfo, data)
    {
    }

    protected override void FetchProperties()
    {

    }
}