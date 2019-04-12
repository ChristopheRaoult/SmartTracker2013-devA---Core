namespace smartTracker
{
    partial class WeightScalefrm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkbContinu = new System.Windows.Forms.CheckBox();
            this.buttonScan = new System.Windows.Forms.Button();
            this.buttonDispose = new System.Windows.Forms.Button();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.comboBoxDevice = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonFindScale = new System.Windows.Forms.Button();
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.labelWeight = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelOldWeight = new System.Windows.Forms.Label();
            this.labelTagID = new System.Windows.Forms.Label();
            this.labelLotID = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.panelChart = new System.Windows.Forms.Panel();
            this.dataListViewHistory = new BrightIdeasSoftware.DataListView();
            this.timerscan = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkbContinu);
            this.groupBox1.Controls.Add(this.buttonScan);
            this.groupBox1.Controls.Add(this.buttonDispose);
            this.groupBox1.Controls.Add(this.buttonCreate);
            this.groupBox1.Controls.Add(this.buttonRefresh);
            this.groupBox1.Controls.Add(this.comboBoxDevice);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(229, 127);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rfid";
            // 
            // chkbContinu
            // 
            this.chkbContinu.AutoSize = true;
            this.chkbContinu.Location = new System.Drawing.Point(117, 91);
            this.chkbContinu.Name = "chkbContinu";
            this.chkbContinu.Size = new System.Drawing.Size(73, 17);
            this.chkbContinu.TabIndex = 16;
            this.chkbContinu.Text = "Automatic";
            this.chkbContinu.UseVisualStyleBackColor = true;
            this.chkbContinu.Click += new System.EventHandler(this.chkbContinu_Click);
            // 
            // buttonScan
            // 
            this.buttonScan.Enabled = false;
            this.buttonScan.Location = new System.Drawing.Point(19, 85);
            this.buttonScan.Name = "buttonScan";
            this.buttonScan.Size = new System.Drawing.Size(92, 27);
            this.buttonScan.TabIndex = 12;
            this.buttonScan.Text = "Scan !";
            this.buttonScan.UseVisualStyleBackColor = true;
            this.buttonScan.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonDispose
            // 
            this.buttonDispose.Enabled = false;
            this.buttonDispose.Location = new System.Drawing.Point(117, 52);
            this.buttonDispose.Name = "buttonDispose";
            this.buttonDispose.Size = new System.Drawing.Size(92, 27);
            this.buttonDispose.TabIndex = 11;
            this.buttonDispose.Text = "Dispose";
            this.buttonDispose.UseVisualStyleBackColor = true;
            this.buttonDispose.Click += new System.EventHandler(this.buttonDispose_Click);
            // 
            // buttonCreate
            // 
            this.buttonCreate.Enabled = false;
            this.buttonCreate.Location = new System.Drawing.Point(19, 52);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(92, 27);
            this.buttonCreate.TabIndex = 10;
            this.buttonCreate.Text = "Create";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.BackgroundImage = global::smartTracker.Properties.Resources.refresh;
            this.buttonRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonRefresh.Location = new System.Drawing.Point(196, 16);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(27, 31);
            this.buttonRefresh.TabIndex = 9;
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // comboBoxDevice
            // 
            this.comboBoxDevice.FormattingEnabled = true;
            this.comboBoxDevice.Location = new System.Drawing.Point(19, 22);
            this.comboBoxDevice.Name = "comboBoxDevice";
            this.comboBoxDevice.Size = new System.Drawing.Size(171, 21);
            this.comboBoxDevice.TabIndex = 8;
            this.comboBoxDevice.SelectedIndexChanged += new System.EventHandler(this.comboBoxDevice_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.buttonFindScale);
            this.groupBox2.Controls.Add(this.buttonUpdate);
            this.groupBox2.Controls.Add(this.labelWeight);
            this.groupBox2.Location = new System.Drawing.Point(256, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(527, 127);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Weight Scale";
            // 
            // buttonFindScale
            // 
            this.buttonFindScale.Enabled = false;
            this.buttonFindScale.Location = new System.Drawing.Point(104, 85);
            this.buttonFindScale.Name = "buttonFindScale";
            this.buttonFindScale.Size = new System.Drawing.Size(92, 27);
            this.buttonFindScale.TabIndex = 16;
            this.buttonFindScale.Text = "Find Scale";
            this.buttonFindScale.UseVisualStyleBackColor = true;
            this.buttonFindScale.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.Enabled = false;
            this.buttonUpdate.Location = new System.Drawing.Point(6, 85);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(92, 27);
            this.buttonUpdate.TabIndex = 15;
            this.buttonUpdate.Text = "Update";
            this.buttonUpdate.UseVisualStyleBackColor = true;
            this.buttonUpdate.Click += new System.EventHandler(this.button2_Click);
            // 
            // labelWeight
            // 
            this.labelWeight.Font = new System.Drawing.Font("Verdana", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWeight.Location = new System.Drawing.Point(6, 39);
            this.labelWeight.Name = "labelWeight";
            this.labelWeight.Size = new System.Drawing.Size(515, 43);
            this.labelWeight.TabIndex = 14;
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 488);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(795, 75);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelInfo
            // 
            this.toolStripStatusLabelInfo.Font = new System.Drawing.Font("Verdana", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
            this.toolStripStatusLabelInfo.Size = new System.Drawing.Size(97, 70);
            this.toolStripStatusLabelInfo.Text = "Info :";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.labelOldWeight);
            this.groupBox3.Controls.Add(this.labelTagID);
            this.groupBox3.Controls.Add(this.labelLotID);
            this.groupBox3.Location = new System.Drawing.Point(645, 180);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(138, 126);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Info ";
            // 
            // labelOldWeight
            // 
            this.labelOldWeight.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOldWeight.ForeColor = System.Drawing.Color.Black;
            this.labelOldWeight.Location = new System.Drawing.Point(16, 117);
            this.labelOldWeight.Name = "labelOldWeight";
            this.labelOldWeight.Size = new System.Drawing.Size(533, 20);
            this.labelOldWeight.TabIndex = 6;
            this.labelOldWeight.Text = "Enter :";
            this.labelOldWeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelTagID
            // 
            this.labelTagID.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTagID.ForeColor = System.Drawing.Color.Black;
            this.labelTagID.Location = new System.Drawing.Point(16, 73);
            this.labelTagID.Name = "labelTagID";
            this.labelTagID.Size = new System.Drawing.Size(533, 20);
            this.labelTagID.TabIndex = 5;
            this.labelTagID.Text = "Enter :";
            this.labelTagID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelLotID
            // 
            this.labelLotID.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLotID.ForeColor = System.Drawing.Color.Black;
            this.labelLotID.Location = new System.Drawing.Point(16, 32);
            this.labelLotID.Name = "labelLotID";
            this.labelLotID.Size = new System.Drawing.Size(533, 20);
            this.labelLotID.TabIndex = 4;
            this.labelLotID.Text = "Enter :";
            this.labelLotID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.panelChart);
            this.groupBox4.Controls.Add(this.dataListViewHistory);
            this.groupBox4.Location = new System.Drawing.Point(12, 145);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(771, 340);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "History";
            // 
            // panelChart
            // 
            this.panelChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelChart.Location = new System.Drawing.Point(417, 19);
            this.panelChart.Name = "panelChart";
            this.panelChart.Size = new System.Drawing.Size(347, 314);
            this.panelChart.TabIndex = 7;
            // 
            // dataListViewHistory
            // 
            this.dataListViewHistory.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dataListViewHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataListViewHistory.Cursor = System.Windows.Forms.Cursors.Default;
            this.dataListViewHistory.DataSource = null;
            this.dataListViewHistory.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataListViewHistory.FullRowSelect = true;
            this.dataListViewHistory.GridLines = true;
            this.dataListViewHistory.Location = new System.Drawing.Point(6, 19);
            this.dataListViewHistory.Name = "dataListViewHistory";
            this.dataListViewHistory.ShowGroups = false;
            this.dataListViewHistory.Size = new System.Drawing.Size(405, 315);
            this.dataListViewHistory.SpaceBetweenGroups = 20;
            this.dataListViewHistory.TabIndex = 6;
            this.dataListViewHistory.UseCompatibleStateImageBehavior = false;
            this.dataListViewHistory.UseCustomSelectionColors = true;
            this.dataListViewHistory.UseExplorerTheme = true;
            this.dataListViewHistory.UseFiltering = true;
            this.dataListViewHistory.UseHotItem = true;
            this.dataListViewHistory.UseTranslucentHotItem = true;
            this.dataListViewHistory.View = System.Windows.Forms.View.Details;
            // 
            // timerscan
            // 
            this.timerscan.Tick += new System.EventHandler(this.timerscan_Tick);
            // 
            // WeightScalefrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(795, 563);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(650, 400);
            this.Name = "WeightScalefrm";
            this.Text = "WeightScalefrm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WeightScalefrm_FormClosing);
            this.Load += new System.EventHandler(this.WeightScalefrm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewHistory)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonScan;
        private System.Windows.Forms.Button buttonDispose;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.ComboBox comboBoxDevice;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelWeight;
        private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelInfo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label labelTagID;
        private System.Windows.Forms.Label labelLotID;
        private System.Windows.Forms.Label labelOldWeight;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chkbContinu;
        private System.Windows.Forms.Timer timerscan;
        private System.Windows.Forms.Button buttonFindScale;
        private BrightIdeasSoftware.DataListView dataListViewHistory;
        private System.Windows.Forms.Panel panelChart;      
   
    }
}