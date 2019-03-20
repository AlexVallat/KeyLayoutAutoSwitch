using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using CommandLine;
using CommandLine.Text;
using KeyLayoutAutoSwitch.Properties;
using Microsoft.VisualBasic.ApplicationServices;

namespace KeyLayoutAutoSwitch
{
	static class Program
	{
		private static CultureInfo UICulture;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			new SingleInstanceController().Run(args);
		}

		private class SingleInstanceController : WindowsFormsApplicationBase
		{
			public SingleInstanceController() : base(AuthenticationMode.ApplicationDefined)
			{
				IsSingleInstance = true;

				StartupNextInstance += OnStartupNextInstance;

			}

			private void OnStartupNextInstance(object sender, StartupNextInstanceEventArgs e)
			{
				// For some reason OnStartupNextInstance gets called with the wrong context, so have to re-apply locale
				if (Program.UICulture != null)
				{
					CultureInfo.CurrentUICulture = Program.UICulture;
				}
				ShowConfigDialog();
			}

			protected override bool OnInitialize(ReadOnlyCollection<string> commandLineArgs)
			{
				FirstInstance(commandLineArgs);
				return false; // Do not do additional WindowsFormsApplicationBase processing
			}
		}

		private class CommandLineArgs
		{
			[Option(HelpText = "Override the default UI language")]
			public string Locale { get; set; }

			[Option(HelpText = "Skip displaying the configuration screen when run")]
			public bool Minimized { get; set; }
		}

		/// <summary>
		/// The main entry point for the first instance of the application.
		/// <remarks> Not executed for subsequent instances</remarks>
		/// </summary>
		private static void FirstInstance(IEnumerable<string> args)
		{
			var result = Parser.Default.ParseArguments<CommandLineArgs>(args);
			if (result.Tag == ParserResultType.NotParsed)
			{ 
				var helpText = HelpText.AutoBuild(result, 200);
				MessageBox.Show(helpText, Resources.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			var commandLineArgs = ((Parsed<CommandLineArgs>)result).Value;

			if (commandLineArgs.Locale != null)
			{
				try
				{
					UICulture = CultureInfo.GetCultureInfo(commandLineArgs.Locale);
					CultureInfo.CurrentUICulture = UICulture;
				}
				catch (Exception ex)
				{
					// ReSharper disable once LocalizableElement This is the error that occurs if the language can't be set properly, so don't translate it
					MessageBox.Show($"Could not set language: {commandLineArgs.Locale}\n\n{ex.Message}", Resources.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			Rules.Instance.Load();

			Application.Run(new BackgroundApplicationContext(!commandLineArgs.Minimized));
		}

		private static Configuration sCurrentConfigDialog;

		internal static void ShowConfigDialog()
		{
			// NOTE: Not thread-safe. Assuming always called on main thread.
			if (sCurrentConfigDialog != null)
			{
				sCurrentConfigDialog.Activate();
			}
			else
			{
				try
				{
					sCurrentConfigDialog = new Configuration();
					sCurrentConfigDialog.ShowDialog();
				}
				finally
				{
					sCurrentConfigDialog = null;
				}
			}
		}
	}

	internal class BackgroundApplicationContext : ApplicationContext
	{
		private readonly NotifyIcon mNotifyIcon;
		private readonly IDisposable mFocusEventHook;

		public BackgroundApplicationContext(bool showInitialConfigDialog)
		{
			mFocusEventHook = NativeMethods.AddFocusEventHook(OnFocusChanged);

			Application.ApplicationExit += OnApplicationExit;

			mNotifyIcon = new NotifyIcon
			{
				Icon = Resources.LanguageBar,
				Text = Resources.ApplicationName,
				ContextMenuStrip = new ContextMenuStrip(),
			};
			
			mNotifyIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[]
			{
				new ToolStripMenuItem(Resources.IconMenuConfigure, null, OnConfigure),
				new ToolStripSeparator(), 
				new ToolStripMenuItem(Resources.IconMenuExit, null, OnExit),
			});

			mNotifyIcon.DoubleClick += OnConfigure;

			mNotifyIcon.Visible = true;

			if (showInitialConfigDialog)
			{
				Program.ShowConfigDialog();
			}
		}

		private void OnExit(object sender, EventArgs e)
		{
			ExitThread();
		}

		private void OnConfigure(object sender, EventArgs e)
		{
			Program.ShowConfigDialog();
		}

		private string mLastFocusedUrl;
		private readonly Dictionary<int, IntPtr> mPreviousUrlLayouts = new Dictionary<int, IntPtr>();
		
		private void OnFocusChanged(IntPtr hwnd, uint idObject, uint idChild)
		{
			var className = NativeMethods.GetWindowClassName(hwnd);

			//Debug.WriteLine($"Focus changed to window: {hwnd} ({className}), object {idObject}, child {idChild}, layout {NativeMethods.GetKeyboardLayout(hwnd)}");

			Browser browser = null;
			if (className == "MozillaWindowClass")
			{
				browser = new Firefox();
			}
			if (browser != null)
			{
				try
				{
					var accessibleObject = NativeMethods.GetAccessibleFromEvent(hwnd, idObject, idChild);
					if (accessibleObject != null)
					{
						var focusType = browser.GetFocusType(accessibleObject, out var url);
						Debug.WriteLine($"Focus on {focusType} with url {url}");
						//Debug.WriteLine($"Focus on accessible object: {accessibleObject.accName[0]} ({AccessibleObjectHelper.GetRole(accessibleObject)})");
						
						// If the URL hasn't changed (and it's a URL-based focus) then don't re-apply the keyboard layout
						if (url == null || url != mLastFocusedUrl)
						{
							if (mLastFocusedUrl != null && Rules.Instance.RestorePreviouslyVisitedPageLayouts)
							{
								// Leaving this URL, so store the current layout that it uses so it can be restored

								// If the layout is the same as the one specified by rule, then don't bother recording it
								var ruleLayoutForLastUrl = Rules.Instance.GetApplicableRule(FocusType.Page, mLastFocusedUrl).Language?.Handle ?? IntPtr.Zero;

								var currentLayout = NativeMethods.GetKeyboardLayout(hwnd);
								Debug.Assert(currentLayout != IntPtr.Zero, "Unable to get current keyboard layout");
								if (currentLayout != IntPtr.Zero && currentLayout != ruleLayoutForLastUrl)
								{
									Debug.WriteLine($"Stored previous layout for {mLastFocusedUrl}: {currentLayout}");
									mPreviousUrlLayouts[mLastFocusedUrl.GetHashCode()] = currentLayout;
								}
							}
							
							// Attempt to look up the previous layout for this url
							if (url != null &&  Rules.Instance.RestorePreviouslyVisitedPageLayouts &&
								mPreviousUrlLayouts.TryGetValue(url.GetHashCode(), out var previousLayout))
							{
								mPreviousUrlLayouts.Remove(url.GetHashCode());
								Debug.WriteLine($"Restoring previous layout for {url}: {previousLayout}");
								NativeMethods.SwitchKeyboardLayout(hwnd, previousLayout);
							}
							else
							{
								// No previous layout, so apply rule-based layout
								var rule = Rules.Instance.GetApplicableRule(focusType, url);

								if (rule.Language != null)
								{
									Debug.WriteLine($"Sending switch to language {rule.DisplayLanguage}");
									NativeMethods.SwitchKeyboardLayout(hwnd, rule.Language.Handle);
								}
							}
							mLastFocusedUrl = url;
						}
					}
				}
				catch (Exception exception)
				{
					Debug.WriteLine("Failed to get accessible object: " + exception.Message);
				}
			}
			else
			{
				//Debug.WriteLine($"Focus changed to window: {hwnd} ({className}), object {idObject}, child {idChild}");
			}
		}

		private void OnApplicationExit(object sender, EventArgs eventArgs)
		{
			mFocusEventHook.Dispose();
			mNotifyIcon.Dispose();
		}
	}
}
