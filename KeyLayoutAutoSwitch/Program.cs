using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
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

		internal static void ShowAddDomainRule(string url)
		{
			var newRule = new DomainRule
			{
				DomainSuffix = Rules.GetDomain(url),
				Language = InputLanguage.CurrentInputLanguage
			};

			if (newRule.ShowEditorDialog(sCurrentConfigDialog) == DialogResult.OK)
			{
				Rules.Instance.AddDomainRule(newRule);
				if (sCurrentConfigDialog != null)
				{
					sCurrentConfigDialog.SaveAndUpdateList();
				}
				else
				{
					Rules.Instance.Save();
				}
			}
		}
	}

	internal class BackgroundApplicationContext : ApplicationContext
	{
		private readonly NotifyIcon mNotifyIcon;
		private readonly IDisposable mFocusEventHook;
		private readonly ChromeAccessibilityWinEventHook mChromeAccessibilityWinEventHook;

		public BackgroundApplicationContext(bool showInitialConfigDialog)
		{
			mFocusEventHook = NativeMethods.AddFocusEventHook(OnFocusChanged);
			mChromeAccessibilityWinEventHook = new ChromeAccessibilityWinEventHook();

			Application.ApplicationExit += OnApplicationExit;

			mNotifyIcon = new NotifyIcon
			{
				Icon = Resources.LanguageBar,
				Text = Resources.ApplicationName,
				ContextMenuStrip = new ContextMenuStrip(),
			};

			ToolStripMenuItem resetPreviouslyVisitedMenuItem;
			mNotifyIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[]
			{
				new ToolStripMenuItem(Resources.IconMenuConfigure, null, OnConfigure),
				new ToolStripMenuItem(Resources.IconMenuAddRule, null, OnAddRule),
				resetPreviouslyVisitedMenuItem = new ToolStripMenuItem(Resources.IconMenuResetPreviouslyVisited, null, OnResetPreviouslyVisited),
				new ToolStripSeparator(),
				new ToolStripMenuItem(Resources.IconMenuExit, null, OnExit),
			});

			// Only show the reset previously visible if actually using that rule
			mNotifyIcon.ContextMenuStrip.Opening += delegate
			{
				resetPreviouslyVisitedMenuItem.Visible = Rules.Instance.PreviouslyVisitedPageRule.RestorePreviousLayout;
			};

			mNotifyIcon.MouseDoubleClick += OnDoubleClick;

			mNotifyIcon.Visible = true;

			if (showInitialConfigDialog)
			{
				SynchronizationContext.Current.Post(_ => Program.ShowConfigDialog(), null);
			}
		}

		private void OnDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Program.ShowConfigDialog();
			}
		}

		private void OnExit(object sender, EventArgs e)
		{
			ExitThread();
		}
		private void OnResetPreviouslyVisited(object sender, EventArgs e)
		{
			mPreviousUrlLayouts.Clear();
		}

		private void OnAddRule(object sender, EventArgs e)
		{
			Program.ShowAddDomainRule(mLastFocusedUrl);
		}

		private void OnConfigure(object sender, EventArgs e)
		{
			Program.ShowConfigDialog();
		}

		private string mLastFocusedUrl;
		private readonly Dictionary<int, IntPtr> mPreviousUrlLayouts = new Dictionary<int, IntPtr>();

		private bool TryPopPreviousUrlLayout(string url, out IntPtr layout)
		{
			var key = url.GetHashCode();
			var result = mPreviousUrlLayouts.TryGetValue(key, out layout);
			if (result)
			{
				Debug.WriteLine($"Restoring previous layout for {url}: {layout.GetKeyboardLayoutName()}");
				mPreviousUrlLayouts.Remove(key);
			}

			return result;
		}

		private void SetPreviousUrlLayout(string url, IntPtr layout)
		{
			mPreviousUrlLayouts[url.GetHashCode()] = layout;
			Debug.WriteLine($"Stored previous layout for {url}: {layout.GetKeyboardLayoutName()}");
		}

		private void OnFocusChanged(IntPtr hwnd, uint idObject, uint idChild)
		{
			var className = NativeMethods.GetWindowClassName(hwnd);

			//Debug.WriteLine($"Focus changed to window: {hwnd} ({className}), object {idObject}, child {idChild}, layout {NativeMethods.GetKeyboardLayout(hwnd)}");

			Browser browser = null;
			switch (className)
			{
				case "MozillaWindowClass":
					browser = new Firefox();
					break;
				case "Chrome_RenderWidgetHostHWND":
					browser = new Chrome();
					break;
				default:
					if (className.StartsWith("Chrome_WidgetWin_"))
					{
						browser = new ChromeWidgets();
					}
					break;
			}
			if (browser != null)
			{
				try
				{
					var accessibleObject = NativeMethods.GetAccessibleFromEvent(hwnd, idObject, idChild);
					if (accessibleObject != null)
					{
						var focusType = browser.GetFocusType(accessibleObject, out var fullUrl);
						var url = fullUrl == null ? null : new Uri(fullUrl).GetLeftPart(UriPartial.Path); // Ignore query and anchor parts of the URL

						Debug.WriteLine($"Focus on {focusType} with url {url}");
						//Debug.WriteLine($"Focus on accessible object: {accessibleObject.accName[0]} ({AccessibleObjectHelper.GetRole(accessibleObject)})");

						// If the URL hasn't changed (and it's a URL-based focus) then don't re-apply the keyboard layout
						if (url == null || url != mLastFocusedUrl)
						{
							if (mLastFocusedUrl != null && Rules.Instance.PreviouslyVisitedPageRule.RestorePreviousLayout)
							{
								// Leaving this URL, so store the current layout that it uses so it can be restored
								var urlForPreviouslyVisited = Rules.Instance.PreviouslyVisitedPageRule.GetApplicableUrl(mLastFocusedUrl);

								// If the layout is the same as the one specified by rule, then don't bother recording it
								var ruleLayoutForLastUrl = Rules.Instance.GetApplicableRule(FocusType.Page, mLastFocusedUrl).Language?.Handle ?? IntPtr.Zero;

								var currentLayout = NativeMethods.GetKeyboardLayout(hwnd);
								Debug.Assert(currentLayout != IntPtr.Zero, "Unable to get current keyboard layout");
								if (currentLayout != IntPtr.Zero && currentLayout != ruleLayoutForLastUrl)
								{
									SetPreviousUrlLayout(urlForPreviouslyVisited, currentLayout);
								}
							}

							// Attempt to look up the previous layout for this url
							if (url != null &&  Rules.Instance.PreviouslyVisitedPageRule.RestorePreviousLayout &&
								TryPopPreviousUrlLayout(Rules.Instance.PreviouslyVisitedPageRule.GetApplicableUrl(url), out var previousLayout))
							{
								SwitchToLayout(hwnd, previousLayout, Rules.Instance.PreviouslyVisitedPageRule.GetRuleNameForUrl(url));
							}
							else
							{
								// No previous layout, so apply rule-based layout
								var rule = Rules.Instance.GetApplicableRule(focusType, url);

								if (rule.Language != null)
								{
									Debug.WriteLine($"Sending switch to language {rule.DisplayLanguage}");

									SwitchToLayout(hwnd, rule.Language.Handle, rule.DisplayName);
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

		private void SwitchToLayout(IntPtr hwnd, IntPtr layout, string rule)
		{
			NativeMethods.SwitchKeyboardLayout(hwnd, layout);
			try
			{
				mNotifyIcon.SetText(String.Format(Resources.SwitchTooltip, layout.GetKeyboardLayoutName(), rule));
			}
			catch
			{
				// Ignore setting the text, it's entirely optional
			}
		}

		private void OnApplicationExit(object sender, EventArgs eventArgs)
		{
			mFocusEventHook.Dispose();
			mNotifyIcon.Dispose();
			mChromeAccessibilityWinEventHook.Dispose();
		}
	}
}
