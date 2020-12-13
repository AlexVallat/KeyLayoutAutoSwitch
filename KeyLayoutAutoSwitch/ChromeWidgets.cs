using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Accessibility;
using CommandLine;

namespace KeyLayoutAutoSwitch
{
	internal class ChromeWidgets : Browser
	{
		public override FocusType GetFocusType(IAccessible accessibleObject, out string url)
		{
			url = null;

			// Walk up tree finding parent
			var parent = accessibleObject;
			while (parent != null)
			{
				var role = AccessibleObjectHelper.GetRole(parent);
				if (role == AccessibleRole.Text && AccessibleObjectHelper.HasState(accessibleObject, AccessibleStates.Focusable))
				{
					// Could be the location bar, if the parent is a grouping
					if (accessibleObject.accParent is IAccessible immediateParent)
					{
						if (AccessibleObjectHelper.GetRole(immediateParent) == AccessibleRole.Grouping && (AccessibleStates)immediateParent.accState[0] == AccessibleStates.None)
						{
							return FocusType.Location;
						}
						else
						{
							if (AccessibleObjectHelper.FindAncestor(immediateParent, AccessibleRole.Dialog) != null)
							{
								return FocusType.FindInPage;
							}
						}
					}
				}
				parent = parent.accParent as IAccessible;
			}

			return FocusType.Other;
		}
	}
}
