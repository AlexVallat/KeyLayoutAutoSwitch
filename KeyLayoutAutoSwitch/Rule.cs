using System;
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

		public virtual void Deserialize(XElement element)
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

		public override void Deserialize(XElement element)
		{
			base.Deserialize(element);
			MatchPageDomainRule = ((bool?)element.Attribute(MatchPageAttributeName)).GetValueOrDefault(false);
		}
	}

	internal class PreviouslyVisitedPageRule : Rule
	{
		private const string RestorePreviousLayoutAttributeName = "restorePreviousLayout";

		public override string DisplayName => Resources.PreviouslyVisitedPageRule;
		public override string RuleEditorDescription => Resources.PrevioulsyVisitedPageRuleDescription;

		public bool RestorePreviousLayout { get; set; }

		public override string DisplayLanguage
		{
			get
			{
				if (RestorePreviousLayout)
				{
					return Resources.PreviouslyVisitedPageRestoreLayout;
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

			return element;
		}

		public override void Deserialize(XElement element)
		{
			base.Deserialize(element);
			RestorePreviousLayout = ((bool?)element.Attribute(RestorePreviousLayoutAttributeName)).GetValueOrDefault(false);
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

		public override void Deserialize(XElement element)
		{
			base.Deserialize(element);
			DomainSuffix = (string)element.Attribute(DomainAttributeName);
		}
	}
}
