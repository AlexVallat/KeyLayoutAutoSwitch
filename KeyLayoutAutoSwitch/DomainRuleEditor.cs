namespace KeyLayoutAutoSwitch
{
	internal class DomainRuleEditor :  RuleEditor
	{
		private System.Windows.Forms.Label mSuffixHelp;
		private System.Windows.Forms.TextBox mDomainSuffix;

		public DomainRuleEditor()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DomainRuleEditor));
			this.mDomainSuffix = new System.Windows.Forms.TextBox();
			this.mSuffixHelp = new System.Windows.Forms.Label();
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
			// mDomainSuffix
			// 
			resources.ApplyResources(this.mDomainSuffix, "mDomainSuffix");
			this.mDomainSuffix.Name = "mDomainSuffix";
			// 
			// mSuffixHelp
			// 
			resources.ApplyResources(this.mSuffixHelp, "mSuffixHelp");
			this.mSuffixHelp.AutoEllipsis = true;
			this.mSuffixHelp.Name = "mSuffixHelp";
			// 
			// DomainRuleEditor
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.mSuffixHelp);
			this.Controls.Add(this.mDomainSuffix);
			this.Name = "DomainRuleEditor";
			this.Controls.SetChildIndex(this.mDomainSuffix, 0);
			this.Controls.SetChildIndex(this.mSuffixHelp, 0);
			this.Controls.SetChildIndex(this.mDoNotChange, 0);
			this.Controls.SetChildIndex(this.mSetLayout, 0);
			this.Controls.SetChildIndex(this.mInputMethods, 0);
			this.Controls.SetChildIndex(this.mDescription, 0);
			((System.ComponentModel.ISupportInitialize)(this.mInputMethods)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private new DomainRule Rule => (DomainRule)base.Rule;

		protected override void PopulateControlsFromRule()
		{
			base.PopulateControlsFromRule();

			mDomainSuffix.Text = Rule.DomainSuffix;
		}

		protected override void SetRuleDataFromControls()
		{
			base.SetRuleDataFromControls();

			Rule.DomainSuffix = mDomainSuffix.Text;
		}
	}
}
