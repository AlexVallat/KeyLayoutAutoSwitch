using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace KeyLayoutAutoSwitch
{
	internal static class ExtensionMethods
	{
		private static readonly MethodInfo GetLocalizedKeyboardLayoutNameMethod = typeof(InputLanguage).GetMethod("GetLocalizedKeyboardLayoutName", BindingFlags.NonPublic | BindingFlags.Static);

		private static readonly Dictionary<uint, string> _layoutKeyNamesByLayoutId = LoadLayoutKeyNames();
		private static readonly Dictionary<IntPtr, string> _layoutNameCache = new Dictionary<IntPtr, string>();

		private static Dictionary<uint, string> LoadLayoutKeyNames()
		{
			var layoutKeyNames = new Dictionary<uint, string>();

			using (var keyboardLayouts = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layouts"))
			{
				foreach (var keyboardLayout in keyboardLayouts.GetSubKeyNames())
				{
					using (var keyboardLayoutKey = keyboardLayouts.OpenSubKey(keyboardLayout))
					{
						var layoutId = keyboardLayoutKey.GetValue("Layout Id") as string;
						if (layoutId != null)
						{
							layoutKeyNames[Convert.ToUInt32(layoutId, 16)] = keyboardLayout;
						}
					}
				}
			}

			return layoutKeyNames;
		}

		public static string GetLayoutName(this InputLanguage inputLanguage)
		{
			if (!_layoutNameCache.TryGetValue(inputLanguage.Handle, out var layoutName))
			{
				// InputLanguage.LayoutName is bugged (https://github.com/dotnet/winforms/issues/4345), try to do better
				try
				{
					var targetLayoutIdOrLanguage = (uint)inputLanguage.Handle >> 16;

					var keyName = (targetLayoutIdOrLanguage & 0xf000) == 0xf000 ?
							// This is a layout ID, so get the key name from the lookup
							_layoutKeyNamesByLayoutId[targetLayoutIdOrLanguage & 0x0fff]
						:
							// This is a language ID, the layout name is just that.
							targetLayoutIdOrLanguage.ToString("X8");

					using (var keyboardLayoutKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layouts\" + keyName))
					{
						// Attempt to extract the localized keyboard layout name, default back to legacy name from registry, final default just use InputLanguage code path
						layoutName = GetLocalizedKeyboardLayoutName(keyboardLayoutKey.GetValue("Layout Display Name") as string)
										?? keyboardLayoutKey.GetValue("Layout Text") as string
										?? inputLanguage.LayoutName;
					}
				}
				catch
				{
					// Fallback on the buggy WinForms implemenation
					layoutName = inputLanguage.LayoutName;
				}
				_layoutNameCache.Add(inputLanguage.Handle, layoutName);
			}

			return layoutName;
		}

		private static string GetLocalizedKeyboardLayoutName(string layoutDisplayName)
		{
			return GetLocalizedKeyboardLayoutNameMethod?.Invoke(null, new[] { layoutDisplayName }) as string;
		}
	}
}
