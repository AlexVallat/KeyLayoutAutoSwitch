using System.Windows.Forms;
using Accessibility;

namespace KeyLayoutAutoSwitch
{
	internal class Firefox : Browser
	{
		public override FocusType GetFocusType(IAccessible accessibleObject, out string url)
		{
			url = null;

			// Walk up tree finding parent
			var parent = accessibleObject;
			while (parent != null)
			{
				var role = AccessibleObjectHelper.GetRole(parent);
				if (role == AccessibleRole.ToolBar)
				{
					// This is on the toolbar, so either location box or search box
					var immediateParent = accessibleObject.accParent as IAccessible;
					if (immediateParent != null)
					{
						if (AccessibleObjectHelper.GetRole(immediateParent) == AccessibleRole.ComboBox)
						{
							if (immediateParent.accChildCount > 2)
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
							var propertyPage = parent.accParent as IAccessible;
							if (propertyPage != null && AccessibleObjectHelper.GetRole(propertyPage) == AccessibleRole.PropertyPage)
							{
								// Get URL for page
								var document = AccessibleObjectHelper.FindChild(AccessibleObjectHelper.FindChild(propertyPage,
													customRole: "browser"),
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
