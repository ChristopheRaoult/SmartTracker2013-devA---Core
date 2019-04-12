namespace smartTracker
{
    partial class DbColumnForm
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.changeInUINT64ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeInDoubleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeInStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.resetAlertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAlertValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAlertDLCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAlertStockLimitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAlertBadBloodForPatientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defineWeightColumnToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonSubmit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.dataGridViewDB = new System.Windows.Forms.DataGridView();
            this.openFileDialogXLS = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDB)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSplitButton1,
            this.toolStripDropDownButton1,
            this.toolStripButtonSubmit,
            this.toolStripButtonExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(945, 35);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButton1.Image = global::smartTracker.Properties.Resources.exportXLS;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(202, 32);
            this.toolStripButton1.Text = "Import Column Header";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeInUINT64ToolStripMenuItem,
            this.changeInDoubleToolStripMenuItem,
            this.changeInStringToolStripMenuItem});
            this.toolStripSplitButton1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold);
            this.toolStripSplitButton1.Image = global::smartTracker.Properties.Resources._23135_bubka_Xcode;
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(201, 32);
            this.toolStripSplitButton1.Text = "Change Column Type";
            // 
            // changeInUINT64ToolStripMenuItem
            // 
            this.changeInUINT64ToolStripMenuItem.Name = "changeInUINT64ToolStripMenuItem";
            this.changeInUINT64ToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.changeInUINT64ToolStripMenuItem.Text = "Change in UINT64";
            this.changeInUINT64ToolStripMenuItem.Click += new System.EventHandler(this.changeInUINT64ToolStripMenuItem_Click);
            // 
            // changeInDoubleToolStripMenuItem
            // 
            this.changeInDoubleToolStripMenuItem.Name = "changeInDoubleToolStripMenuItem";
            this.changeInDoubleToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.changeInDoubleToolStripMenuItem.Text = "Change in DOUBLE";
            this.changeInDoubleToolStripMenuItem.Click += new System.EventHandler(this.changeInDoubleToolStripMenuItem_Click);
            // 
            // changeInStringToolStripMenuItem
            // 
            this.changeInStringToolStripMenuItem.Name = "changeInStringToolStripMenuItem";
            this.changeInStringToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.changeInStringToolStripMenuItem.Text = "Change in STRING";
            this.changeInStringToolStripMenuItem.Click += new System.EventHandler(this.changeInStringToolStripMenuItem_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetAlertToolStripMenuItem,
            this.addAlertValueToolStripMenuItem,
            this.addAlertDLCToolStripMenuItem,
            this.addAlertStockLimitToolStripMenuItem,
            this.addAlertBadBloodForPatientToolStripMenuItem,
            this.defineWeightColumnToolStripMenuItem1});
            this.toolStripDropDownButton1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripDropDownButton1.Image = global::smartTracker.Properties.Resources._23177_bubka_TimeMachine;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(220, 32);
            this.toolStripDropDownButton1.Text = "Alert && Specific Column!";
            // 
            // resetAlertToolStripMenuItem
            // 
            this.resetAlertToolStripMenuItem.Image = global::smartTracker.Properties.Resources._23178_bubka_TrashEmpty;
            this.resetAlertToolStripMenuItem.Name = "resetAlertToolStripMenuItem";
            this.resetAlertToolStripMenuItem.Size = new System.Drawing.Size(308, 30);
            this.resetAlertToolStripMenuItem.Text = "Reset Alert";
            this.resetAlertToolStripMenuItem.Click += new System.EventHandler(this.resetAlertToolStripMenuItem_Click);
            // 
            // addAlertValueToolStripMenuItem
            // 
            this.addAlertValueToolStripMenuItem.Image = global::smartTracker.Properties.Resources._23144_bubka_Console;
            this.addAlertValueToolStripMenuItem.Name = "addAlertValueToolStripMenuItem";
            this.addAlertValueToolStripMenuItem.Size = new System.Drawing.Size(308, 30);
            this.addAlertValueToolStripMenuItem.Text = "Add Alert Value";
            this.addAlertValueToolStripMenuItem.Click += new System.EventHandler(this.addAlertValueToolStripMenuItem_Click);
            // 
            // addAlertDLCToolStripMenuItem
            // 
            this.addAlertDLCToolStripMenuItem.Image = global::smartTracker.Properties.Resources._23145_bubka_Dashboard;
            this.addAlertDLCToolStripMenuItem.Name = "addAlertDLCToolStripMenuItem";
            this.addAlertDLCToolStripMenuItem.Size = new System.Drawing.Size(308, 30);
            this.addAlertDLCToolStripMenuItem.Text = "Add Alert DLC";
            this.addAlertDLCToolStripMenuItem.Click += new System.EventHandler(this.addAlertDLCToolStripMenuItem_Click);
            // 
            // addAlertStockLimitToolStripMenuItem
            // 
            this.addAlertStockLimitToolStripMenuItem.Image = global::smartTracker.Properties.Resources._23135_bubka_Xcode;
            this.addAlertStockLimitToolStripMenuItem.Name = "addAlertStockLimitToolStripMenuItem";
            this.addAlertStockLimitToolStripMenuItem.Size = new System.Drawing.Size(308, 30);
            this.addAlertStockLimitToolStripMenuItem.Text = "Add Alert Stock Limit";
            this.addAlertStockLimitToolStripMenuItem.Click += new System.EventHandler(this.addAlertStockLimitToolStripMenuItem_Click);
            // 
            // addAlertBadBloodForPatientToolStripMenuItem
            // 
            this.addAlertBadBloodForPatientToolStripMenuItem.Image = global::smartTracker.Properties.Resources._23148_bubka_Finder;
            this.addAlertBadBloodForPatientToolStripMenuItem.Name = "addAlertBadBloodForPatientToolStripMenuItem";
            this.addAlertBadBloodForPatientToolStripMenuItem.Size = new System.Drawing.Size(308, 30);
            this.addAlertBadBloodForPatientToolStripMenuItem.Text = "Add Alert Bad Blood for Patient";
            this.addAlertBadBloodForPatientToolStripMenuItem.Click += new System.EventHandler(this.addAlertBadBloodForPatientToolStripMenuItem_Click);
            // 
            // defineWeightColumnToolStripMenuItem1
            // 
            this.defineWeightColumnToolStripMenuItem1.Image = global::smartTracker.Properties.Resources._23154_bubka_HardDrive;
            this.defineWeightColumnToolStripMenuItem1.Name = "defineWeightColumnToolStripMenuItem1";
            this.defineWeightColumnToolStripMenuItem1.Size = new System.Drawing.Size(308, 30);
            this.defineWeightColumnToolStripMenuItem1.Text = "Define Weight Column";
            this.defineWeightColumnToolStripMenuItem1.Click += new System.EventHandler(this.defineWeightColumnToolStripMenuItem1_Click);
            // 
            // toolStripButtonSubmit
            // 
            this.toolStripButtonSubmit.Enabled = false;
            this.toolStripButtonSubmit.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonSubmit.Image = global::smartTracker.Properties.Resources.apply;
            this.toolStripButtonSubmit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSubmit.Name = "toolStripButtonSubmit";
            this.toolStripButtonSubmit.Size = new System.Drawing.Size(152, 32);
            this.toolStripButtonSubmit.Text = "Submit Change!";
            this.toolStripButtonSubmit.Click += new System.EventHandler(this.toolStripButtonSubmit_Click);
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
            // dataGridViewDB
            // 
            this.dataGridViewDB.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewDB.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDB.Location = new System.Drawing.Point(0, 35);
            this.dataGridViewDB.MultiSelect = false;
            this.dataGridViewDB.Name = "dataGridViewDB";
            this.dataGridViewDB.Size = new System.Drawing.Size(945, 305);
            this.dataGridViewDB.TabIndex = 1;
            this.dataGridViewDB.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridViewDB_CellBeginEdit);
            this.dataGridViewDB.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridViewDB_EditingControlShowing);
            // 
            // openFileDialogXLS
            // 
            this.openFileDialogXLS.Filter = "Excel 97 - 2003 File (*.xls)|*.xls|Excel 2010 File (*.xlsx)|*.xlsx|CSV File (*.cs" +
    "v)|*.csv";
            this.openFileDialogXLS.FilterIndex = 2;
            this.openFileDialogXLS.Title = "Import XLS File";
            // 
            // DbColumnForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(945, 340);
            this.Controls.Add(this.dataGridViewDB);
            this.Controls.Add(this.toolStrip1);
            this.Name = "DbColumnForm";
            this.ShowIcon = false;
            this.Text = "Configure Column";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DbColumnForm_FormClosing);
            this.Load += new System.EventHandler(this.DbColumnForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.DataGridView dataGridViewDB;
        private System.Windows.Forms.ToolStripButton toolStripButtonSubmit;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.OpenFileDialog openFileDialogXLS;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem addAlertValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetAlertToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem changeInUINT64ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeInDoubleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeInStringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addAlertDLCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addAlertStockLimitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addAlertBadBloodForPatientToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defineWeightColumnToolStripMenuItem1;
    }
}