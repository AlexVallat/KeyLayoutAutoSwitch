using Accessibility;

namespace KeyLayoutAutoSwitch
{
	internal enum FocusType
	{
		/// <summary>The location bar</summary>
		Location,
		/// <summary>The search bar</summary>
		Search,
		/// <summary>The Find In Page search box</summary>
		FindInPage,
		/// <summary>A web page</summary>
		Page,
		Other
	}
	internal abstract class Browser
	{
		public abstract FocusType GetFocusType(IAccessible accessibleObject, out string url);
	}
}
