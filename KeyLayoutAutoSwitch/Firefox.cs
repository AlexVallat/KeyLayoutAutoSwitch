using System.Windows.Forms;
using Accessibility;

namespace KeyLayoutAutoSwitch
{
	internal class Firefox : Browser
	{
		public override FocusType GetFocusType(IAccessible accessibleObject, out string url)
		{
			url = null;

			// When switching to Firefox it can raise focus for both the focused element and the application, so ignore the application bit
			if (AccessibleObjectHelper.GetRole(accessibleObject) == AccessibleRole.Application)
			{
				return FocusType.Ignore;
			}

			// Walk up tree finding parent
			var parent = accessibleObject;
			while (parent != null)
			{
				var role = AccessibleObjectHelper.GetRole(parent);
				if (role == AccessibleRole.ToolBar)
				{
					// This is on the toolbar, so either location box or search box
					if (accessibleObject.accParent is IAccessible immediateParent)
					{
						if (AccessibleObjectHelper.GetRole(immediateParent) == AccessibleRole.ComboBox)
						{
							if (AccessibleObjectHelper.HasState(accessibleObject, AccessibleStates.HasPopup))
							{
								return FocusType.Location;
							}
							else
							{
								return FocusType.Search;
							}
						}
						else
						{
							if (parent.accParent is IAccessible propertyPage &&
								AccessibleObjectHelper.GetRole(propertyPage) == AccessibleRole.PropertyPage)
							{
								// Get URL for page
								var document = AccessibleObjectHelper.FindChild(AccessibleObjectHelper.FindChild(propertyPage),
														role: AccessibleRole.Document);
								if (document != null)
								{
									url = AccessibleObjectHelper.GetValue(document);
								}
							}

							return FocusType.FindInPage;
						}
					}
				}
				else if (role == AccessibleRole.Document && AccessibleObjectHelper.HasState(accessibleObject, AccessibleStates.Focusable))
				{
					// This is a web page
					url = AccessibleObjectHelper.GetValue(parent);
					return FocusType.Page;
				}
				parent = parent.accParent as IAccessible;
			}

			return FocusType.Other;
		}
	}
}
