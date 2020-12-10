using Microsoft.Win32;
using System.Reflection;
using System.Windows.Forms;

namespace KeyLayoutAutoSwitch
{
	internal static class ExtensionMethods
	{
		private static readonly MethodInfo GetLocalizedKeyboardLayoutNameMethod = typeof(InputLanguage).GetMethod("GetLocalizedKeyboardLayoutName", BindingFlags.NonPublic | BindingFlags.Static);
		public static string GetLayoutName(this InputLanguage inputLanguage)
		{
			var handle = (uint)inputLanguage.Handle;
			if ((handle & 0xf0000000) == 0xf0000000) // InputLanguage.LayoutName is bugged when layout ID is provided, so try and do better
			{
				var targetLanguage = (handle & 0xffff).ToString("X4");
				var targetLayoutId = ((handle >> 16) & 0x0fff).ToString("X4");

				var keyboardLayouts = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layouts");
				foreach (var keyboardLayout in keyboardLayouts.GetSubKeyNames())
				{
					if (keyboardLayout.Length == 8)
					{
						var layoutLanguage = keyboardLayout.Substring(4);
						if (layoutLanguage == targetLanguage)
						{
							// Does this match the layout too?
							using (var keyboardLayoutKey = keyboardLayouts.OpenSubKey(keyboardLayout))
							{
								var layoutId = keyboardLayoutKey.GetValue("Layout Id") as string;
								if (layoutId == targetLayoutId)
								{
									// Match by language and layout ID,


									// Attempt to extract the localized keyboard layout name, default back to legacy name from registry, final default just use InputLanguage code path
									return GetLocalizedKeyboardLayoutName(keyboardLayoutKey.GetValue("Layout Display Name") as string)
										?? keyboardLayoutKey.GetValue("Layout Text") as string
										?? inputLanguage.LayoutName;
								}
							}
						}
					}
				}
			}

			return inputLanguage.LayoutName;
		}

		private static string GetLocalizedKeyboardLayoutName(string layoutDisplayName)
		{
			return GetLocalizedKeyboardLayoutNameMethod?.Invoke(null, new[] { layoutDisplayName }) as string;
		}
	}
}
