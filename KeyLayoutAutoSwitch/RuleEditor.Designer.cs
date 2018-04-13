using BrightIdeasSoftware;

namespace KeyLayoutAutoSwitch
{
	partial class RuleEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RuleEditor));
			this.mInputMethods = new BrightIdeasSoftware.ObjectListView();
			this.mColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.mDoNotChange = new System.Windows.Forms.RadioButton();
			this.mSetLayout = new System.Windows.Forms.RadioButton();
			this.mCancel = new System.Windows.Forms.Button();
			this.mOK = new System.Windows.Forms.Button();
			this.mDescription = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.mInputMethods)).BeginInit();
			this.SuspendLayout();
			// 
			// mInputMethods
			// 
			resources.ApplyResources(this.mInputMethods, "mInputMethods");
			this.mInputMethods.AllColumns.Add(this.mColumn);
			this.mInputMethods.CellEditUseWholeCell = false;
			this.mInputMethods.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.mColumn});
			this.mInputMethods.CopySelectionOnControlC = false;
			this.mInputMethods.FullRowSelect = true;
			this.mInputMethods.HasCollapsibleGroups = false;
			this.mInputMethods.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.mInputMethods.HideSelection = false;
			this.mInputMethods.MultiSelect = false;
			this.mInputMethods.Name = "mInputMethods";
			this.mInputMethods.OverlayText.Text = resources.GetString("resource.Text");
			this.mInputMethods.SelectAllOnControlA = false;
			this.mInputMethods.SelectColumnsOnRightClick = false;
			this.mInputMethods.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
			this.mInputMethods.ShowFilterMenuOnRightClick = false;
			this.mInputMethods.UseCompatibleStateImageBehavior = false;
			this.mInputMethods.UseHotControls = false;
			this.mInputMethods.View = System.Windows.Forms.View.Details;
			this.mInputMethods.GotFocus += new System.EventHandler(this.mInputMethods_GotFocus);
			// 
			// mColumn
			// 
			this.mColumn.FillsFreeSpace = true;
			resources.ApplyResources(this.mColumn, "mColumn");
			// 
			// mDoNotChange
			// 
			resources.ApplyResources(this.mDoNotChange, "mDoNotChange");
			this.mDoNotChange.Checked = true;
			this.mDoNotChange.Name = "mDoNotChange";
			this.mDoNotChange.TabStop = true;
			this.mDoNotChange.UseVisualStyleBackColor = true;
			// 
			// mSetLayout
			// 
			resources.ApplyResources(this.mSetLayout, "mSetLayout");
			this.mSetLayout.Name = "mSetLayout";
			this.mSetLayout.UseVisualStyleBackColor = true;
			this.mSetLayout.CheckedChanged += new System.EventHandler(this.mSetLayout_CheckedChanged);
			// 
			// mCancel
			// 
			resources.ApplyResources(this.mCancel, "mCancel");
			this.mCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.mCancel.Name = "mCancel";
			// 
			// mOK
			// 
			resources.ApplyResources(this.mOK, "mOK");
			this.mOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.mOK.Name = "mOK";
			// 
			// mDescription
			// 
			resources.ApplyResources(this.mDescription, "mDescription");
			this.mDescription.AutoEllipsis = true;
			this.mDescription.Name = "mDescription";
			// 
			// RuleEditor
			// 
			this.AcceptButton = this.mOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.mCancel;
			this.Controls.Add(this.mDescription);
			this.Controls.Add(this.mCancel);
			this.Controls.Add(this.mOK);
			this.Controls.Add(this.mInputMethods);
			this.Controls.Add(this.mSetLayout);
			this.Controls.Add(this.mDoNotChange);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RuleEditor";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.mInputMethods)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OLVColumn mColumn;
		private System.Windows.Forms.Button mCancel;
		private System.Windows.Forms.Button mOK;
		protected ObjectListView mInputMethods;
		protected System.Windows.Forms.RadioButton mDoNotChange;
		protected System.Windows.Forms.RadioButton mSetLayout;
		protected System.Windows.Forms.Label mDescription;
	}
}