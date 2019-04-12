namespace smartTracker
{
    partial class ItemHistoryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemHistoryForm));
            this.toolStripItemList = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxTagUID = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxLotID = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonGet = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.dataGridViewItemHistory = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelHistory = new System.Windows.Forms.Label();
            this.timerStart = new System.Windows.Forms.Timer(this.components);
            this.toolStripItemList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewItemHistory)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripItemList
            // 
            this.toolStripItemList.AutoSize = false;
            this.toolStripItemList.BackColor = System.Drawing.Color.White;
            this.toolStripItemList.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold);
            this.toolStripItemList.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripItemList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripTextBoxTagUID,
            this.toolStripSeparator1,
            this.toolStripLabel2,
            this.toolStripTextBoxLotID,
            this.toolStripSeparator2,
            this.toolStripButtonGet,
            this.toolStripButtonExit});
            this.toolStripItemList.Location = new System.Drawing.Point(0, 0);
            this.toolStripItemList.Name = "toolStripItemList";
            this.toolStripItemList.Size = new System.Drawing.Size(763, 35);
            this.toolStripItemList.TabIndex = 0;
            this.toolStripItemList.Text = "toolStripItemList";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel1.ForeColor = System.Drawing.Color.Black;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(73, 32);
            this.toolStripLabel1.Text = "Tag UID :";
            // 
            // toolStripTextBoxTagUID
            // 
            this.toolStripTextBoxTagUID.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripTextBoxTagUID.ForeColor = System.Drawing.Color.Black;
            this.toolStripTextBoxTagUID.Name = "toolStripTextBoxTagUID";
            this.toolStripTextBoxTagUID.Size = new System.Drawing.Size(100, 35);
            this.toolStripTextBoxTagUID.Click += new System.EventHandler(this.toolStripTextBoxTagUID_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel2.ForeColor = System.Drawing.Color.Black;
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(56, 32);
            this.toolStripLabel2.Text = "LotID :";
            // 
            // toolStripTextBoxLotID
            // 
            this.toolStripTextBoxLotID.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripTextBoxLotID.ForeColor = System.Drawing.Color.Black;
            this.toolStripTextBoxLotID.Name = "toolStripTextBoxLotID";
            this.toolStripTextBoxLotID.Size = new System.Drawing.Size(100, 35);
            this.toolStripTextBoxLotID.Click += new System.EventHandler(this.toolStripTextBoxLotID_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 35);
            // 
            // toolStripButtonGet
            // 
            this.toolStripButtonGet.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonGet.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonGet.Image = global::smartTracker.Properties.Resources.refresh;
            this.toolStripButtonGet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGet.Name = "toolStripButtonGet";
            this.toolStripButtonGet.Size = new System.Drawing.Size(93, 32);
            this.toolStripButtonGet.Text = "Process";
            this.toolStripButtonGet.Click += new System.EventHandler(this.toolStripButtonGet_Click);
            // 
            // toolStripButtonExit
            // 
            this.toolStripButtonExit.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonExit.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonExit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExit.Image")));
            this.toolStripButtonExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExit.Name = "toolStripButtonExit";
            this.toolStripButtonExit.Size = new System.Drawing.Size(64, 32);
            this.toolStripButtonExit.Text = "Exit";
            this.toolStripButtonExit.Click += new System.EventHandler(this.toolStripButtonExit_Click);
            // 
            // dataGridViewItemHistory
            // 
            this.dataGridViewItemHistory.AllowUserToAddRows = false;
            this.dataGridViewItemHistory.AllowUserToDeleteRows = false;
            this.dataGridViewItemHistory.AllowUserToResizeColumns = false;
            this.dataGridViewItemHistory.AllowUserToResizeRows = false;
            this.dataGridViewItemHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewItemHistory.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewItemHistory.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewItemHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewItemHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewItemHistory.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewItemHistory.MultiSelect = false;
            this.dataGridViewItemHistory.Name = "dataGridViewItemHistory";
            this.dataGridViewItemHistory.ReadOnly = true;
            this.dataGridViewItemHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewItemHistory.Size = new System.Drawing.Size(763, 376);
            this.dataGridViewItemHistory.TabIndex = 1;
            this.dataGridViewItemHistory.SelectionChanged += new System.EventHandler(this.dataGridViewItemHistory_SelectionChanged);
            this.dataGridViewItemHistory.Sorted += new System.EventHandler(this.dataGridViewItemHistory_Sorted);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 35);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel1.Controls.Add(this.labelHistory);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridViewItemHistory);
            this.splitContainer1.Size = new System.Drawing.Size(763, 430);
            this.splitContainer1.TabIndex = 2;
            // 
            // labelHistory
            // 
            this.labelHistory.AutoSize = true;
            this.labelHistory.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHistory.ForeColor = System.Drawing.Color.Black;
            this.labelHistory.Location = new System.Drawing.Point(12, 14);
            this.labelHistory.Name = "labelHistory";
            this.labelHistory.Size = new System.Drawing.Size(130, 18);
            this.labelHistory.TabIndex = 0;
            this.labelHistory.Text = "Item History : ";
            // 
            // timerStart
            // 
            this.timerStart.Enabled = true;
            this.timerStart.Tick += new System.EventHandler(this.timerStart_Tick);
            // 
            // ItemHistoryForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(763, 465);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStripItemList);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ItemHistoryForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Item History";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ItemHistoryForm_FormClosing);
            this.Load += new System.EventHandler(this.ItemHistoryForm_Load);
            this.toolStripItemList.ResumeLayout(false);
            this.toolStripItemList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewItemHistory)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripItemList;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButtonGet;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxTagUID;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.DataGridView dataGridViewItemHistory;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label labelHistory;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxLotID;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Timer timerStart;
    }
}