namespace smartTracker
{
    partial class CompareInventoryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompareInventoryForm));
            this.openFileXLSDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStripCompare = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.saveXlsFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.dataListViewImport = new BrightIdeasSoftware.DataListView();
            this.toolStripCompare.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewImport)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileXLSDialog
            // 
            this.openFileXLSDialog.Filter = "Excel 97 - 2003 File (*.xls)|*.xls|Excel 2010 File (*.xlsx)|*.xlsx|CSV File (*.cs" +
    "v)|*.csv";
            this.openFileXLSDialog.FilterIndex = 2;
            this.openFileXLSDialog.Title = "Compare XLS Inventory";
            // 
            // toolStripCompare
            // 
            this.toolStripCompare.AutoSize = false;
            this.toolStripCompare.BackColor = System.Drawing.Color.White;
            this.toolStripCompare.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripCompare.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonOpen,
            this.toolStripButtonExport,
            this.toolStripButtonExit});
            this.toolStripCompare.Location = new System.Drawing.Point(0, 0);
            this.toolStripCompare.Name = "toolStripCompare";
            this.toolStripCompare.Size = new System.Drawing.Size(812, 35);
            this.toolStripCompare.TabIndex = 0;
            this.toolStripCompare.Text = "toolStrip1";
            // 
            // toolStripButtonOpen
            // 
            this.toolStripButtonOpen.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonOpen.Image = global::smartTracker.Properties.Resources.open;
            this.toolStripButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpen.Name = "toolStripButtonOpen";
            this.toolStripButtonOpen.Size = new System.Drawing.Size(147, 32);
            this.toolStripButtonOpen.Text = "Open Excel File";
            this.toolStripButtonOpen.Click += new System.EventHandler(this.toolStripButtonOpen_Click);
            // 
            // toolStripButtonExport
            // 
            this.toolStripButtonExport.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonExport.Image = global::smartTracker.Properties.Resources.exportXLS;
            this.toolStripButtonExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExport.Name = "toolStripButtonExport";
            this.toolStripButtonExport.Size = new System.Drawing.Size(146, 32);
            this.toolStripButtonExport.Text = "Export to Excel";
            this.toolStripButtonExport.Click += new System.EventHandler(this.toolStripButtonExport_Click);
            // 
            // toolStripButtonExit
            // 
            this.toolStripButtonExit.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonExit.Image = global::smartTracker.Properties.Resources.exit;
            this.toolStripButtonExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExit.Name = "toolStripButtonExit";
            this.toolStripButtonExit.Size = new System.Drawing.Size(64, 32);
            this.toolStripButtonExit.Text = "Exit";
            this.toolStripButtonExit.Click += new System.EventHandler(this.toolStripButtonExit_Click);
            // 
            // saveXlsFileDialog
            // 
            this.saveXlsFileDialog.Filter = "Excel 2007 -  2013 File (*.xlsx)|*.xlsx";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 457);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(812, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(34, 17);
            this.toolStripStatusLabel.Text = "Info :";
            // 
            // dataListViewImport
            // 
            this.dataListViewImport.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dataListViewImport.Cursor = System.Windows.Forms.Cursors.Default;
            this.dataListViewImport.DataSource = null;
            this.dataListViewImport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListViewImport.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataListViewImport.FullRowSelect = true;
            this.dataListViewImport.GridLines = true;
            this.dataListViewImport.Location = new System.Drawing.Point(0, 35);
            this.dataListViewImport.Name = "dataListViewImport";
            this.dataListViewImport.ShowItemCountOnGroups = true;
            this.dataListViewImport.Size = new System.Drawing.Size(812, 422);
            this.dataListViewImport.SpaceBetweenGroups = 20;
            this.dataListViewImport.TabIndex = 5;
            this.dataListViewImport.UseCompatibleStateImageBehavior = false;
            this.dataListViewImport.UseCustomSelectionColors = true;
            this.dataListViewImport.UseExplorerTheme = true;
            this.dataListViewImport.UseFiltering = true;
            this.dataListViewImport.UseHotItem = true;
            this.dataListViewImport.UseTranslucentHotItem = true;
            this.dataListViewImport.View = System.Windows.Forms.View.Details;
            this.dataListViewImport.HotItemChanged += new System.EventHandler<BrightIdeasSoftware.HotItemChangedEventArgs>(this.dataListViewImport_HotItemChanged);
            // 
            // CompareInventoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 479);
            this.Controls.Add(this.dataListViewImport);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStripCompare);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CompareInventoryForm";
            this.ShowIcon = false;
            this.Text = "Compare Inventory";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CompareInventoryForm_FormClosing);
            this.Load += new System.EventHandler(this.CompareInventoryForm_Load);
            this.toolStripCompare.ResumeLayout(false);
            this.toolStripCompare.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewImport)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileXLSDialog;
        private System.Windows.Forms.ToolStrip toolStripCompare;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpen;
        private System.Windows.Forms.ToolStripButton toolStripButtonExport;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        private System.Windows.Forms.SaveFileDialog saveXlsFileDialog;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private BrightIdeasSoftware.DataListView dataListViewImport;
    }
}