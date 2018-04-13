using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace KeyLayoutAutoSwitch
{
	public partial class Configuration : Form
	{
		public Configuration()
		{
			InitializeComponent();
			mRuleColumn.AspectGetter = o => ((Rule)o).DisplayName;
			mLanguageColumn.AspectGetter = o => ((Rule)o).DisplayLanguage;

			mRules.SetObjects(Rules.Instance.GetAllRules());

			mRules.AutoResizeColumn(mLanguageColumn.Index, ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		private void mClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void mAdd_Click(object sender, EventArgs e)
		{
			var newRule = new DomainRule();
			if (newRule.ShowEditorDialog(this) == DialogResult.OK)
			{
				Rules.Instance.AddDomainRule(newRule);
				SaveAndUpdateList();
				mRules.SelectedObject = newRule;
			}
		}

		private void mRemove_Click(object sender, EventArgs e)
		{
			if (mRules.SelectedObject is DomainRule selectedRule)
			{
				Rules.Instance.RemoveDomainRule(selectedRule);
				SaveAndUpdateList();
			}
		}

		private void mEdit_Click(object sender, EventArgs e)
		{
			EditSelectedRule();
		}

		private void mRules_ItemActivate(object sender, EventArgs e)
		{
			EditSelectedRule();
		}

		private void EditSelectedRule()
		{
			if (mRules.SelectedObject is Rule selectedRule)
			{
				if (selectedRule.ShowEditorDialog(this) == DialogResult.OK)
				{
					SaveAndUpdateList();
				}
			}
		}

		private void mRules_SelectionChanged(object sender, EventArgs e)
		{
			mEdit.Enabled = mRules.SelectedObject is Rule;
			mRemove.Enabled = mRules.SelectedObject is DomainRule;
		}

		private void SaveAndUpdateList()
		{
			Rules.Instance.Save();
			mRules.BuildList();
			mRules.AutoResizeColumn(mLanguageColumn.Index, ColumnHeaderAutoResizeStyle.ColumnContent);
			mRules_SelectionChanged(null, EventArgs.Empty);
		}

		private void mDonations_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://keylayoutautoswitch.byalexv.co.uk/donate");
		}

		private void mHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://keylayoutautoswitch.byalexv.co.uk/help");
		}
	}
}
