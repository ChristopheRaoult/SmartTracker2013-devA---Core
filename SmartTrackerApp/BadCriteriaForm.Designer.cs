using smartTracker.Properties;

namespace smartTracker
{
    partial class BadCriteriaForm
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
            this.components = new System.ComponentModel.Container();
            this.toolStripStat = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonFindByLed = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonQuit = new System.Windows.Forms.ToolStripButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.saveXlsFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.dataListView = new BrightIdeasSoftware.DataListView();
            this.toolStripStat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListView)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripStat
            // 
            this.toolStripStat.AutoSize = false;
            this.toolStripStat.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStat.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripStat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonFindByLed,
            this.toolStripButtonExport,
            this.toolStripButtonQuit});
            this.toolStripStat.Location = new System.Drawing.Point(0, 0);
            this.toolStripStat.Name = "toolStripStat";
            this.toolStripStat.Size = new System.Drawing.Size(1103, 35);
            this.toolStripStat.TabIndex = 9;
            this.toolStripStat.Text = "toolStrip1";
            // 
            // toolStripButtonFindByLed
            // 
            this.toolStripButtonFindByLed.Image = global::smartTracker.Properties.Resources.Search;
            this.toolStripButtonFindByLed.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFindByLed.Name = "toolStripButtonFindByLed";
            this.toolStripButtonFindByLed.Size = new System.Drawing.Size(172, 32);
            this.toolStripButtonFindByLed.Text = "Find Tag(s) By Led";
            this.toolStripButtonFindByLed.Click += new System.EventHandler(this.toolStripButtonFindByLed_Click);
            // 
            // toolStripButtonExport
            // 
            this.toolStripButtonExport.Image = global::smartTracker.Properties.Resources.exportXLS;
            this.toolStripButtonExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExport.Name = "toolStripButtonExport";
            this.toolStripButtonExport.Size = new System.Drawing.Size(84, 32);
            this.toolStripButtonExport.Text = global::smartTracker.Properties.ResStrings.str_Export;
            this.toolStripButtonExport.Click += new System.EventHandler(this.toolStripButtonExport_Click);
            // 
            // toolStripButtonQuit
            // 
            this.toolStripButtonQuit.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonQuit.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonQuit.Image = global::smartTracker.Properties.Resources.exit;
            this.toolStripButtonQuit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonQuit.Name = "toolStripButtonQuit";
            this.toolStripButtonQuit.Size = new System.Drawing.Size(66, 32);
            this.toolStripButtonQuit.Text = "Quit";
            this.toolStripButtonQuit.Click += new System.EventHandler(this.toolStripButtonQuit_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // saveXlsFileDialog
            // 
            this.saveXlsFileDialog.Filter = "Excel  2007 - 2013 File (*.xlsx)|*.xlsx";
            // 
            // dataListView
            // 
            this.dataListView.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dataListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.dataListView.DataSource = null;
            this.dataListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListView.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataListView.FullRowSelect = true;
            this.dataListView.GridLines = true;
            this.dataListView.Location = new System.Drawing.Point(0, 35);
            this.dataListView.Name = "dataListView";
            this.dataListView.ShowItemCountOnGroups = true;
            this.dataListView.Size = new System.Drawing.Size(1103, 287);
            this.dataListView.SpaceBetweenGroups = 20;
            this.dataListView.TabIndex = 10;
            this.dataListView.UseCompatibleStateImageBehavior = false;
            this.dataListView.UseCustomSelectionColors = true;
            this.dataListView.UseExplorerTheme = true;
            this.dataListView.UseFiltering = true;
            this.dataListView.UseHotItem = true;
            this.dataListView.UseTranslucentHotItem = true;
            this.dataListView.View = System.Windows.Forms.View.Details;
            // 
            // BadCriteriaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1103, 322);
            this.Controls.Add(this.dataListView);
            this.Controls.Add(this.toolStripStat);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "BadCriteriaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Not fit with defined criteria";
            this.Load += new System.EventHandler(this.BadCriteriaForm_Load);
            this.toolStripStat.ResumeLayout(false);
            this.toolStripStat.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripStat;
        private System.Windows.Forms.ToolStripButton toolStripButtonQuit;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripButton toolStripButtonExport;
        private System.Windows.Forms.SaveFileDialog saveXlsFileDialog;
        private BrightIdeasSoftware.DataListView dataListView;
        private System.Windows.Forms.ToolStripButton toolStripButtonFindByLed;
    }
}