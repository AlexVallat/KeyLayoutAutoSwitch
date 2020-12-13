using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Accessibility;
using CommandLine;

namespace KeyLayoutAutoSwitch
{
	internal class Chrome : Browser
	{
		public override FocusType GetFocusType(IAccessible accessibleObject, out string url)
		{
			url = null;

			// Walk up tree finding parent
			var parent = accessibleObject;
			while (parent != null)
			{
				var role = AccessibleObjectHelper.GetRole(parent);
				if (role == AccessibleRole.Document && AccessibleObjectHelper.HasState(accessibleObject, AccessibleStates.Focusable))
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
