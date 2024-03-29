﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using KeyLayoutAutoSwitch.Properties;

namespace KeyLayoutAutoSwitch
{
	internal abstract class Rule
	{
		private const string InputLanguageAttributeName = "inputLanguage";
		private InputLanguage mLanguage;

		public InputLanguage Language
		{
			get => mLanguage;
			set
			{
				if (mLanguage != value)
				{
					mLanguage = value;
					DisplayLanguage = mLanguage == null ? Resources.LanguageDoNotChange : String.Format(Resources.LanguageDisplayFormat, Language.Culture.DisplayName, Language.GetLayoutName());
				}
			}
		}

		// Do no expensive calculation here as it can be used on paint
		public virtual string DisplayLanguage { get; private set; } = Resources.LanguageDoNotChange;

		// Do no expensive calculation here as it can be used on paint
		public abstract string DisplayName { get; }

		public abstract string RuleEditorDescription { get; }

		public virtual DialogResult ShowEditorDialog(IWin32Window owner = null)
		{
			return new RuleEditor().ShowDialog(this, owner);
		}

		public virtual XElement Serialize()
		{
			var element = new XElement(ElementName);
			if (Language != null)
			{
				element.Add(new XAttribute(InputLanguageAttributeName, Language.Handle.ToInt64()));
			}

			return element;
		}

		public virtual void Deserialize(XElement element, string version)
		{
			var inputLanguageHandle = (long?)element.Attribute(InputLanguageAttributeName);
			if (inputLanguageHandle.HasValue)
			{
				Language = InputLanguage.InstalledInputLanguages.Cast<InputLanguage>().FirstOrDefault(inputLanguage => inputLanguage.Handle.ToInt64() == inputLanguageHandle);
			}
		}

		public virtual string ElementName => GetType().Name;
	}

	internal class LocationBarRule : Rule
	{
		public override string DisplayName => Resources.LocationBarRule;
		public override string RuleEditorDescription => Resources.LocationBarRuleDescription;
	}

	internal class SearchBarRule : Rule
	{
		public override string DisplayName => Resources.SearchBarRule;
		public override string RuleEditorDescription => Resources.SearchBarRuleDescription;
	}

	internal class FindInPageRule : Rule
	{
		private const string MatchPageAttributeName = "matchPage";
		public override string DisplayName => Resources.FindInPageRule;
		public override string RuleEditorDescription => Resources.FindInPageRuleDescription;

		public bool MatchPageDomainRule { get; set; }

		public override string DisplayLanguage
		{
			get
			{
				if (MatchPageDomainRule)
				{
					return Resources.FindInPageMatchWebPage;
				}

				return base.DisplayLanguage;
			}
		}

		public override DialogResult ShowEditorDialog(IWin32Window owner = null)
		{
			return new FindInPageRuleEditor().ShowDialog(this, owner);
		}

		public override XElement Serialize()
		{
			var element = base.Serialize();
			if (MatchPageDomainRule)
			{
				element.Add(new XAttribute(MatchPageAttributeName, true));
			}

			return element;
		}

		public override void Deserialize(XElement element, string version)
		{
			base.Deserialize(element, version);
			MatchPageDomainRule = ((bool?)element.Attribute(MatchPageAttributeName)) ?? false;
		}
	}

	internal class PreviouslyVisitedPageRule : Rule
	{
		private const string RestorePreviousLayoutAttributeName = "restorePreviousLayout";
		private const string ApplyToSiteAttributeName = "applyToSite";

		public override string DisplayName => Resources.PreviouslyVisitedPageRule;
		public override string RuleEditorDescription => Resources.PrevioulsyVisitedPageRuleDescription;

		public bool RestorePreviousLayout { get; set; }

		public bool ApplyToSite { get; set; }

		public string GetApplicableUrl(string urlString)
		{
			if (ApplyToSite)
			{
				return Rules.GetDomain(urlString) ?? urlString;
			}
			return urlString;
		}

		public string GetRuleNameForUrl(string url)
		{
			return ApplyToSite ? String.Format(Resources.PreviouslyVisitedPageRuleNameSite, GetApplicableUrl(url)) : Resources.PreviouslyVisitedPageRuleNamePage;
		}

		public override string DisplayLanguage
		{
			get
			{
				if (RestorePreviousLayout)
				{
					return ApplyToSite ? Resources.PreviouslyVisitedPageRestoreLayoutForSite : Resources.PreviouslyVisitedPageRestoreLayout;
				}
				return base.DisplayLanguage;
			}
		}

		public override DialogResult ShowEditorDialog(IWin32Window owner = null)
		{
			return new PreviouslyVisitedPageRuleEditor().ShowDialog(this, owner);
		}

		public override XElement Serialize()
		{
			var element = base.Serialize();
			if (RestorePreviousLayout)
			{
				element.Add(new XAttribute(RestorePreviousLayoutAttributeName, true));
			}
			if (ApplyToSite)
			{
				element.Add(new XAttribute(ApplyToSiteAttributeName, true));
			}

			return element;
		}

		public override void Deserialize(XElement element, string version)
		{
			base.Deserialize(element, version);
			RestorePreviousLayout = ((bool?)element.Attribute(RestorePreviousLayoutAttributeName)) ?? false;
			ApplyToSite = ((bool?)element.Attribute(ApplyToSiteAttributeName)) ?? false;
		}
	}

	internal class DefaultPageRule : Rule
	{
		public override string DisplayName => Resources.DefaultPageRule;
		public override string RuleEditorDescription => Resources.DefaultPageRuleDescription;
	}

	internal class DefaultUIElementRule : Rule
	{
		public override string DisplayName => Resources.DefaultUIElementRule;
		public override string RuleEditorDescription => Resources.DefaultUIElementRuleDescription;
	}

	internal class DomainRule : Rule
	{
		private const string DomainAttributeName = "domain";

		public string DomainSuffix { get; set; }

		public override string DisplayName => String.Format(Resources.DomainRule, DomainSuffix);
		public override string RuleEditorDescription => Resources.DomainRuleDescription;

		public override DialogResult ShowEditorDialog(IWin32Window owner = null)
		{
			return new DomainRuleEditor().ShowDialog(this, owner);
		}

		public override XElement Serialize()
		{
			var element = base.Serialize();
			element.Add(new XAttribute(DomainAttributeName, DomainSuffix));

			return element;
		}

		public override void Deserialize(XElement element, string version)
		{
			base.Deserialize(element, version);
			DomainSuffix = (string)element.Attribute(DomainAttributeName);
		}
	}

	internal class BrowserProcessNameRule : Rule
	{
		private static readonly string[] DefaultBrowserProcesses = new[]
		{
			"brave",
			"chrome",
			"msedge",
			"opera",
			"firefox",
			"gecko"
		};

		#region Do not show in the UI
		public override string DisplayName => null;
		public override string RuleEditorDescription => null;
		public override DialogResult ShowEditorDialog(IWin32Window owner = null) => DialogResult.Cancel;
		#endregion

		private readonly HashSet<string> mAllowedProcesses = new HashSet<string>(DefaultBrowserProcesses);

		public bool IsProcessEnabled(string processName) => mAllowedProcesses.Contains(processName.ToLowerInvariant());

		public override XElement Serialize()
		{
			var element = base.Serialize();

			foreach (var processName in mAllowedProcesses)
			{
				element.Add(new XElement("Process", processName));
			}

			return element;
		}

		public override void Deserialize(XElement element, string version)
		{
			base.Deserialize(element, version);

			mAllowedProcesses.Clear();
			foreach (var processElement in element.Elements("Process"))
			{
				mAllowedProcesses.Add(processElement.Value.ToLowerInvariant());
			}

			// Upgrade from old rules versions
			switch (version)
			{
				case "2.1.0.0":
					mAllowedProcesses.Add("msedge");
					break;
				default:
					break;
			}
		}
	}
}
