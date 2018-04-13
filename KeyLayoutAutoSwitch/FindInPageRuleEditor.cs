using System.Windows.Forms;

namespace KeyLayoutAutoSwitch
{
	internal class FindInPageRuleEditor :  RuleEditor
	{
		private RadioButton mMatchPageLayout;

		public FindInPageRuleEditor()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindInPageRuleEditor));
			this.mMatchPageLayout = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.mInputMethods)).BeginInit();
			this.SuspendLayout();
			// 
			// mInputMethods
			// 
			resources.ApplyResources(this.mInputMethods, "mInputMethods");
			this.mInputMethods.OverlayText.Text = resources.GetString("resource.Text");
			// 
			// mDoNotChange
			// 
			resources.ApplyResources(this.mDoNotChange, "mDoNotChange");
			// 
			// mSetLayout
			// 
			resources.ApplyResources(this.mSetLayout, "mSetLayout");
			// 
			// mDescription
			// 
			resources.ApplyResources(this.mDescription, "mDescription");
			// 
			// mMatchPageLayout
			// 
			resources.ApplyResources(this.mMatchPageLayout, "mMatchPageLayout");
			this.mMatchPageLayout.Name = "mMatchPageLayout";
			this.mMatchPageLayout.TabStop = true;
			this.mMatchPageLayout.UseVisualStyleBackColor = true;
			// 
			// FindInPageRuleEditor
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.mMatchPageLayout);
			this.Name = "FindInPageRuleEditor";
			this.Controls.SetChildIndex(this.mDoNotChange, 0);
			this.Controls.SetChildIndex(this.mSetLayout, 0);
			this.Controls.SetChildIndex(this.mInputMethods, 0);
			this.Controls.SetChildIndex(this.mDescription, 0);
			this.Controls.SetChildIndex(this.mMatchPageLayout, 0);
			((System.ComponentModel.ISupportInitialize)(this.mInputMethods)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private new FindInPageRule Rule => (FindInPageRule)base.Rule;

		protected override void PopulateControlsFromRule()
		{
			base.PopulateControlsFromRule();

			if (Rule.MatchPageDomainRule)
			{
				mMatchPageLayout.Checked = true;
			}
		}

		protected override void SetRuleDataFromControls()
		{
			base.SetRuleDataFromControls();

			if (mMatchPageLayout.Checked)
			{
				Rule.Language = null;
				Rule.MatchPageDomainRule = true;
			}
			else
			{
				Rule.MatchPageDomainRule = false;
			}
		}
	}
}
