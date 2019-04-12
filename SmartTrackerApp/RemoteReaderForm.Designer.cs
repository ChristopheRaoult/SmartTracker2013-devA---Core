namespace smartTracker
{
    partial class RemoteReaderForm
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
            this.toolStripRemoteReader = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonACreateNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonQuit = new System.Windows.Forms.ToolStripButton();
            this.dataGridViewRemote = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxIPAddress = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.listBoxReader = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.listBoxPC = new System.Windows.Forms.ListBox();
            this.textBoxportsearch = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.toolStripRemoteReader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRemote)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripRemoteReader
            // 
            this.toolStripRemoteReader.AutoSize = false;
            this.toolStripRemoteReader.BackColor = System.Drawing.Color.White;
            this.toolStripRemoteReader.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripRemoteReader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonACreateNew,
            this.toolStripButtonDelete,
            this.toolStripButtonQuit});
            this.toolStripRemoteReader.Location = new System.Drawing.Point(0, 0);
            this.toolStripRemoteReader.Name = "toolStripRemoteReader";
            this.toolStripRemoteReader.Size = new System.Drawing.Size(780, 35);
            this.toolStripRemoteReader.TabIndex = 0;
            this.toolStripRemoteReader.Text = "toolStrip1";
            // 
            // toolStripButtonACreateNew
            // 
            this.toolStripButtonACreateNew.Enabled = false;
            this.toolStripButtonACreateNew.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonACreateNew.Image = global::smartTracker.Properties.Resources.addhardware;
            this.toolStripButtonACreateNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonACreateNew.Name = "toolStripButtonACreateNew";
            this.toolStripButtonACreateNew.Size = new System.Drawing.Size(235, 32);
            this.toolStripButtonACreateNew.Text = "Create New Remote Access";
            this.toolStripButtonACreateNew.Click += new System.EventHandler(this.toolStripButtonACreateNew_Click);
            // 
            // toolStripButtonDelete
            // 
            this.toolStripButtonDelete.Enabled = false;
            this.toolStripButtonDelete.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonDelete.Image = global::smartTracker.Properties.Resources.removehardware;
            this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Size = new System.Drawing.Size(198, 32);
            this.toolStripButtonDelete.Text = "Delete Remote Access";
            this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            // 
            // toolStripButtonQuit
            // 
            this.toolStripButtonQuit.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonQuit.Image = global::smartTracker.Properties.Resources.exit;
            this.toolStripButtonQuit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonQuit.Name = "toolStripButtonQuit";
            this.toolStripButtonQuit.Size = new System.Drawing.Size(66, 32);
            this.toolStripButtonQuit.Text = "Quit";
            this.toolStripButtonQuit.Click += new System.EventHandler(this.toolStripButtonQuit_Click);
            // 
            // dataGridViewRemote
            // 
            this.dataGridViewRemote.AllowUserToAddRows = false;
            this.dataGridViewRemote.AllowUserToDeleteRows = false;
            this.dataGridViewRemote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewRemote.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewRemote.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRemote.Location = new System.Drawing.Point(12, 41);
            this.dataGridViewRemote.MultiSelect = false;
            this.dataGridViewRemote.Name = "dataGridViewRemote";
            this.dataGridViewRemote.ReadOnly = true;
            this.dataGridViewRemote.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRemote.Size = new System.Drawing.Size(756, 142);
            this.dataGridViewRemote.TabIndex = 1;
            this.dataGridViewRemote.SelectionChanged += new System.EventHandler(this.dataGridViewRemote_SelectionChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 205);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP Adress :";
            // 
            // textBoxIPAddress
            // 
            this.textBoxIPAddress.Location = new System.Drawing.Point(93, 202);
            this.textBoxIPAddress.Name = "textBoxIPAddress";
            this.textBoxIPAddress.Size = new System.Drawing.Size(102, 21);
            this.textBoxIPAddress.TabIndex = 3;
            this.textBoxIPAddress.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxIPAddress_KeyUp);
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(93, 229);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(102, 21);
            this.textBoxPort.TabIndex = 5;
            this.textBoxPort.Text = "6901";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 232);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Port :";
            // 
            // buttonSearch
            // 
            this.buttonSearch.Location = new System.Drawing.Point(213, 209);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(100, 35);
            this.buttonSearch.TabIndex = 6;
            this.buttonSearch.Text = "Get Reader!";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusInfo,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 409);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(780, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusInfo
            // 
            this.toolStripStatusInfo.Name = "toolStripStatusInfo";
            this.toolStripStatusInfo.Size = new System.Drawing.Size(45, 17);
            this.toolStripStatusInfo.Text = "Status :";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.MarqueeAnimationSpeed = 0;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // listBoxReader
            // 
            this.listBoxReader.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxReader.FormattingEnabled = true;
            this.listBoxReader.Location = new System.Drawing.Point(12, 264);
            this.listBoxReader.Name = "listBoxReader";
            this.listBoxReader.Size = new System.Drawing.Size(756, 134);
            this.listBoxReader.TabIndex = 10;
            this.listBoxReader.SelectedIndexChanged += new System.EventHandler(this.listBoxReader_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(336, 209);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 35);
            this.button1.TabIndex = 11;
            this.button1.Text = "Get Device";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBoxPC
            // 
            this.listBoxPC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxPC.FormattingEnabled = true;
            this.listBoxPC.Location = new System.Drawing.Point(491, 205);
            this.listBoxPC.Name = "listBoxPC";
            this.listBoxPC.Size = new System.Drawing.Size(277, 43);
            this.listBoxPC.TabIndex = 12;
            this.listBoxPC.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxPC_MouseDoubleClick);
            // 
            // textBoxportsearch
            // 
            this.textBoxportsearch.Location = new System.Drawing.Point(442, 223);
            this.textBoxportsearch.Name = "textBoxportsearch";
            this.textBoxportsearch.Size = new System.Drawing.Size(43, 21);
            this.textBoxportsearch.TabIndex = 13;
            this.textBoxportsearch.Text = "6901";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(442, 205);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Port :";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // RemoteReaderForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(780, 431);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxportsearch);
            this.Controls.Add(this.listBoxPC);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBoxReader);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxIPAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewRemote);
            this.Controls.Add(this.toolStripRemoteReader);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "RemoteReaderForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Remote Reader";
            this.Load += new System.EventHandler(this.RemoteReaderForm_Load);
            this.toolStripRemoteReader.ResumeLayout(false);
            this.toolStripRemoteReader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRemote)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripRemoteReader;
        private System.Windows.Forms.ToolStripButton toolStripButtonACreateNew;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
        private System.Windows.Forms.ToolStripButton toolStripButtonQuit;
        private System.Windows.Forms.DataGridView dataGridViewRemote;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxIPAddress;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusInfo;
        private System.Windows.Forms.ListBox listBoxReader;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBoxPC;
        private System.Windows.Forms.TextBox textBoxportsearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}