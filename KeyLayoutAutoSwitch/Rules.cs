using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows.Forms;
using System.Xml.Linq;
using KeyLayoutAutoSwitch.Properties;

namespace KeyLayoutAutoSwitch
{
	internal sealed class Rules
	{
		private const string AppDataFolder = "KeyLayoutAutoSwitch";
		private const string RulesFileName = "Rules.xml";

		private static readonly string DirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataFolder);
		private static readonly string FilePath = Path.Combine(DirectoryPath, RulesFileName);

		private readonly PreviouslyVisitedPageRule mPreviouslyVisitedPageRule = new PreviouslyVisitedPageRule();
		private readonly List<DomainRule> mDomainRules = new List<DomainRule>();
		private readonly DefaultPageRule mDefaultPageRule = new DefaultPageRule();
		private readonly FindInPageRule mFindInPageRule = new FindInPageRule();
		private readonly LocationBarRule mLocationBarRule = new LocationBarRule();
		private readonly SearchBarRule mSearchBarRule = new SearchBarRule();
		private readonly DefaultUIElementRule mDefaultUIElementRule = new DefaultUIElementRule();

		private Rules() {}
		private static readonly Lazy<Rules> sRules = new Lazy<Rules>(() => new Rules());
		public static Rules Instance => sRules.Value;

		public IEnumerable<Rule> GetAllRules()
		{
			yield return mPreviouslyVisitedPageRule;

			foreach (var domainRule in mDomainRules.OrderByDescending(r => r.DomainSuffix.Length))
			{
				yield return domainRule;
			}

			yield return mDefaultPageRule;
			yield return mFindInPageRule;
			yield return mLocationBarRule;
			yield return mSearchBarRule;
			yield return mDefaultUIElementRule;
		}

		public void AddDomainRule(DomainRule rule)
		{
			mDomainRules.Add(rule);
		}

		public void RemoveDomainRule(DomainRule rule)
		{
			var result = mDomainRules.Remove(rule);
			Debug.Assert(result, "Domain rule was not found");
		}

		public Rule GetApplicableRule(FocusType focusType, string url = null)
		{
			switch (focusType)
			{
				case FocusType.Location:
					return mLocationBarRule;
				case FocusType.Search:
					return mSearchBarRule;
				case FocusType.FindInPage:
					return mFindInPageRule.MatchPageDomainRule ? GetDomainRule(url) : mFindInPageRule;
				case FocusType.Page:
					return GetDomainRule(url);
				case FocusType.Other:
					return mDefaultUIElementRule;
				default:
					Debug.Fail("Unexpected FocusType: " + focusType);
					return mDefaultUIElementRule;
			}
		}

#pragma warning disable RCS1085 // Use auto-implemented property: Consistency with other rules
		public PreviouslyVisitedPageRule PreviouslyVisitedPageRule => mPreviouslyVisitedPageRule;
#pragma warning restore RCS1085

		private Rule GetDomainRule(string urlString)
		{
			if (Uri.TryCreate(urlString, UriKind.Absolute, out var uri))
			{
				var domain = uri.Host;
				return mDomainRules.OrderByDescending(r => r.DomainSuffix.Length).FirstOrDefault(r => domain.EndsWith(r.DomainSuffix, StringComparison.InvariantCultureIgnoreCase)) ?? (Rule)mDefaultPageRule;
			}

			return mDefaultPageRule;
		}

		public void Save()
		{
			try
			{
				var rules = new XElement("Rules");
				foreach (var rule in GetAllRules())
				{
					rules.Add(rule.Serialize());
				}

				Directory.CreateDirectory(DirectoryPath); // Ensure directory exists first
				rules.Save(FilePath);
			}
			catch (Exception ex)
			{
				MessageBox.Show(String.Format(Resources.SaveRulesErrorMessage, FilePath, ex.Message), Resources.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void Load()
		{
			try
			{
				if (!File.Exists(FilePath))
				{
					return;
				}

				var rules = XDocument.Load(FilePath).Root;
				mDomainRules.Clear();

				foreach (var rule in GetAllRules())
				{
					Debug.Assert(!(rule is DomainRule), "No domain rules should be present at this point, as mDomainRules should be emptied.");
					DeserializeRule(rule, rules);
				}

				foreach (var element in rules.Elements(new DomainRule().ElementName))
				{
					var domainRule = new DomainRule();
					domainRule.Deserialize(element);
					mDomainRules.Add(domainRule);
				}
			}
			catch (FileNotFoundException)
			{
			}
			catch (SecurityException)
			{
			}
		}

		private void DeserializeRule(Rule rule, XElement rules)
		{
			var element = rules.Element(rule.ElementName);
			if (element != null)
			{
				rule.Deserialize(element);
			}
		}
	}
}
