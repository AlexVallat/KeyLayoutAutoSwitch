using System;
using System.Drawing;
using System.Windows.Forms;

namespace KeyLayoutAutoSwitch
{
	internal partial class RuleEditor : Form
	{
		protected Rule mRule;
		public RuleEditor()
		{
			InitializeComponent();

			mColumn.AspectGetter = o => ((InputLanguage)o).LayoutName;
			mColumn.GroupKeyGetter = o => ((InputLanguage)o).Culture.DisplayName;
			mColumn.CellPadding = new Rectangle(20,0,0,0);

			foreach (InputLanguage inputLanguage in InputLanguage.InstalledInputLanguages)
			{
				mInputMethods.AddObject(inputLanguage);
			}
		}

		protected Rule Rule => mRule;

		internal DialogResult ShowDialog(Rule rule, IWin32Window owner = null)
		{
			mRule = rule;

			mDescription.Text = Rule.RuleEditorDescription;
			Text = String.Format(Text, Rule.DisplayName);

			PopulateControlsFromRule();

			var dialogResult = ShowDialog(owner);
			if (dialogResult == DialogResult.OK)
			{
				SetRuleDataFromControls();
			}

			return dialogResult;
		}

		protected virtual void PopulateControlsFromRule()
		{
			if (Rule.Language == null)
			{
				mDoNotChange.Checked = true;
			}
			else
			{
				mInputMethods.SelectedObject = Rule.Language;
				mSetLayout.Checked = true;
				mInputMethods.TopItemIndex = mInputMethods.SelectedIndex;
			}
		}

		protected virtual void SetRuleDataFromControls()
		{
			if (mDoNotChange.Checked)
			{
				Rule.Language = null;
			}
			else
			{
				Rule.Language = mInputMethods.SelectedObject as InputLanguage;
			}
		}

		private void mInputMethods_GotFocus(object sender, EventArgs e)
		{
			mSetLayout.Checked = true;
		}

		private void mSetLayout_CheckedChanged(object sender, EventArgs e)
		{
			if (mSetLayout.Checked)
			{
				if (mInputMethods.SelectedIndex == -1)
				{
					mInputMethods.SelectedIndex = 0;
				}
				mInputMethods.Focus();
			}
			else
			{
				mInputMethods.SelectedObject = null;
			}
		}
	}
}
