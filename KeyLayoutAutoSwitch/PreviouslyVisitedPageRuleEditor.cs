namespace KeyLayoutAutoSwitch
{
	internal class PreviouslyVisitedPageRuleEditor :  RuleEditor
	{
		private System.Windows.Forms.RadioButton mRestorePreviousLayoutForSite;
		private System.Windows.Forms.RadioButton mRestorePreviousLayout;

		public PreviouslyVisitedPageRuleEditor()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviouslyVisitedPageRuleEditor));
			this.mRestorePreviousLayout = new System.Windows.Forms.RadioButton();
			this.mRestorePreviousLayoutForSite = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.mInputMethods)).BeginInit();
			this.SuspendLayout();
			// 
			// mInputMethods
			// 
			resources.ApplyResources(this.mInputMethods, "mInputMethods");
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
			// mRestorePreviousLayout
			// 
			resources.ApplyResources(this.mRestorePreviousLayout, "mRestorePreviousLayout");
			this.mRestorePreviousLayout.Name = "mRestorePreviousLayout";
			this.mRestorePreviousLayout.UseVisualStyleBackColor = true;
			// 
			// mRestorePreviousLayoutForSite
			// 
			resources.ApplyResources(this.mRestorePreviousLayoutForSite, "mRestorePreviousLayoutForSite");
			this.mRestorePreviousLayoutForSite.Name = "mRestorePreviousLayoutForSite";
			this.mRestorePreviousLayoutForSite.UseVisualStyleBackColor = true;
			// 
			// PreviouslyVisitedPageRuleEditor
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.mRestorePreviousLayoutForSite);
			this.Controls.Add(this.mRestorePreviousLayout);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "PreviouslyVisitedPageRuleEditor";
			this.Controls.SetChildIndex(this.mSetLayout, 0);
			this.Controls.SetChildIndex(this.mInputMethods, 0);
			this.Controls.SetChildIndex(this.mDescription, 0);
			this.Controls.SetChildIndex(this.mRestorePreviousLayout, 0);
			this.Controls.SetChildIndex(this.mRestorePreviousLayoutForSite, 0);
			this.Controls.SetChildIndex(this.mDoNotChange, 0);
			((System.ComponentModel.ISupportInitialize)(this.mInputMethods)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private new PreviouslyVisitedPageRule Rule => (PreviouslyVisitedPageRule)base.Rule;

		protected override void PopulateControlsFromRule()
		{
			base.PopulateControlsFromRule();

			if (Rule.RestorePreviousLayout)
			{
				if (Rule.ApplyToSite)
				{
					mRestorePreviousLayoutForSite.Checked = true;
				}
				else
				{
					mRestorePreviousLayout.Checked = Rule.RestorePreviousLayout;
				}
			}
			else
			{
				mDoNotChange.Checked = true;
			}
		}

		protected override void SetRuleDataFromControls()
		{
			base.SetRuleDataFromControls();

			Rule.RestorePreviousLayout = mRestorePreviousLayout.Checked || mRestorePreviousLayoutForSite.Checked;
			Rule.ApplyToSite = mRestorePreviousLayoutForSite.Checked;
		}
	}
}
