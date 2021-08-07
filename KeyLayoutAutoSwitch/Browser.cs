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
		Other,

		/// <summary>
		/// Ignore focus on elements of this type, behave as if no focus had occurred
		/// </summary>
		Ignore
	}
	internal abstract class Browser
	{
		public abstract FocusType GetFocusType(IAccessible accessibleObject, out string url);
	}
}
