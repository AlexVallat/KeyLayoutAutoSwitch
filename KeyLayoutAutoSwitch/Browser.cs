using Accessibility;

namespace KeyLayoutAutoSwitch
{
	internal enum FocusType
	{
		Location,
		Search,
		FindInPage,
		Page,
		Other
	}
	internal abstract class Browser
	{
		public abstract FocusType GetFocusType(IAccessible accessibleObject, out string url);
	}
}
