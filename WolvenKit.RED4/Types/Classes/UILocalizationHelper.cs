
namespace WolvenKit.RED4.Types
{
	public abstract partial class UILocalizationHelper : IScriptable
	{
		public UILocalizationHelper()
		{
			PostConstruct();
		}

		partial void PostConstruct();
	}
}
