using BrightIdeasSoftware;

namespace KeyLayoutAutoSwitch
{
	partial class Configuration
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configuration));
			this.mControls = new System.Windows.Forms.Panel();
			this.mClose = new System.Windows.Forms.Button();
			this.mHelp = new System.Windows.Forms.LinkLabel();
			this.mDonations = new System.Windows.Forms.LinkLabel();
			this.mEdit = new System.Windows.Forms.Button();
			this.mRemove = new System.Windows.Forms.Button();
			this.mAdd = new System.Windows.Forms.Button();
			this.mRules = new BrightIdeasSoftware.ObjectListView();
			this.mRuleColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.mLanguageColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.mControls.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mRules)).BeginInit();
			this.SuspendLayout();
			// 
			// mControls
			// 
			resources.ApplyResources(this.mControls, "mControls");
			this.mControls.Controls.Add(this.mClose);
			this.mControls.Controls.Add(this.mHelp);
			this.mControls.Controls.Add(this.mDonations);
			this.mControls.Controls.Add(this.mEdit);
			this.mControls.Controls.Add(this.mRemove);
			this.mControls.Controls.Add(this.mAdd);
			this.mControls.Name = "mControls";
			// 
			// mClose
			// 
			resources.ApplyResources(this.mClose, "mClose");
			this.mClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.mClose.Name = "mClose";
			this.mClose.Click += new System.EventHandler(this.mClose_Click);
			// 
			// mHelp
			// 
			resources.ApplyResources(this.mHelp, "mHelp");
			this.mHelp.Name = "mHelp";
			this.mHelp.TabStop = true;
			this.mHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.mHelp_LinkClicked);
			// 
			// mDonations
			// 
			resources.ApplyResources(this.mDonations, "mDonations");
			this.mDonations.Name = "mDonations";
			this.mDonations.TabStop = true;
			this.mDonations.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.mDonations_LinkClicked);
			// 
			// mEdit
			// 
			resources.ApplyResources(this.mEdit, "mEdit");
			this.mEdit.Name = "mEdit";
			this.mEdit.Click += new System.EventHandler(this.mEdit_Click);
			// 
			// mRemove
			// 
			resources.ApplyResources(this.mRemove, "mRemove");
			this.mRemove.Name = "mRemove";
			this.mRemove.Click += new System.EventHandler(this.mRemove_Click);
			// 
			// mAdd
			// 
			resources.ApplyResources(this.mAdd, "mAdd");
			this.mAdd.Name = "mAdd";
			this.mAdd.Click += new System.EventHandler(this.mAdd_Click);
			// 
			// mRules
			// 
			this.mRules.AllColumns.Add(this.mRuleColumn);
			this.mRules.AllColumns.Add(this.mLanguageColumn);
			this.mRules.CellEditUseWholeCell = false;
			this.mRules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.mRuleColumn,
            this.mLanguageColumn});
			this.mRules.CopySelectionOnControlC = false;
			this.mRules.Cursor = System.Windows.Forms.Cursors.Default;
			resources.ApplyResources(this.mRules, "mRules");
			this.mRules.FullRowSelect = true;
			this.mRules.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.mRules.HideSelection = false;
			this.mRules.IsSearchOnSortColumn = false;
			this.mRules.MultiSelect = false;
			this.mRules.Name = "mRules";
			this.mRules.SelectColumnsOnRightClick = false;
			this.mRules.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
			this.mRules.ShowFilterMenuOnRightClick = false;
			this.mRules.ShowGroups = false;
			this.mRules.UpdateSpaceFillingColumnsWhenDraggingColumnDivider = false;
			this.mRules.UseCompatibleStateImageBehavior = false;
			this.mRules.UseHotControls = false;
			this.mRules.View = System.Windows.Forms.View.Details;
			this.mRules.SelectionChanged += new System.EventHandler(this.mRules_SelectionChanged);
			this.mRules.ItemActivate += new System.EventHandler(this.mRules_ItemActivate);
			// 
			// mRuleColumn
			// 
			this.mRuleColumn.FillsFreeSpace = true;
			this.mRuleColumn.MinimumWidth = 50;
			resources.ApplyResources(this.mRuleColumn, "mRuleColumn");
			// 
			// mLanguageColumn
			// 
			this.mLanguageColumn.MinimumWidth = 50;
			resources.ApplyResources(this.mLanguageColumn, "mLanguageColumn");
			// 
			// Configuration
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mRules);
			this.Controls.Add(this.mControls);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Configuration";
			this.mControls.ResumeLayout(false);
			this.mControls.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.mRules)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Panel mControls;
		private System.Windows.Forms.Button mEdit;
		private System.Windows.Forms.Button mAdd;
		private System.Windows.Forms.Button mClose;
		private System.Windows.Forms.Button mRemove;
		private System.Windows.Forms.LinkLabel mHelp;
		private System.Windows.Forms.LinkLabel mDonations;
		private ObjectListView mRules;
		private OLVColumn mRuleColumn;
		private OLVColumn mLanguageColumn;
	}
}

