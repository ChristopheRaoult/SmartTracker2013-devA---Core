namespace smartTracker
{
    partial class AutoFillBox
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
            this.textBoxLotID = new System.Windows.Forms.TextBox();
            this.textBoxTagID = new System.Windows.Forms.TextBox();
            this.labelLotID = new System.Windows.Forms.Label();
            this.labelTagID = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.cbxAutoWrite = new System.Windows.Forms.CheckBox();
            this.buttonDispose = new System.Windows.Forms.Button();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.comboBoxDevice = new System.Windows.Forms.ComboBox();
            this.checkBoxUseRFid = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonNext = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxLotID
            // 
            this.textBoxLotID.Location = new System.Drawing.Point(139, 30);
            this.textBoxLotID.Name = "textBoxLotID";
            this.textBoxLotID.Size = new System.Drawing.Size(132, 23);
            this.textBoxLotID.TabIndex = 0;
            this.textBoxLotID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxLotID_KeyUp);
            // 
            // textBoxTagID
            // 
            this.textBoxTagID.Location = new System.Drawing.Point(139, 71);
            this.textBoxTagID.Name = "textBoxTagID";
            this.textBoxTagID.Size = new System.Drawing.Size(132, 23);
            this.textBoxTagID.TabIndex = 1;
            this.textBoxTagID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxTagID_KeyDown);
            // 
            // labelLotID
            // 
            this.labelLotID.ForeColor = System.Drawing.Color.Black;
            this.labelLotID.Location = new System.Drawing.Point(17, 33);
            this.labelLotID.Name = "labelLotID";
            this.labelLotID.Size = new System.Drawing.Size(116, 20);
            this.labelLotID.TabIndex = 2;
            this.labelLotID.Text = "Enter :";
            this.labelLotID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTagID
            // 
            this.labelTagID.ForeColor = System.Drawing.Color.Black;
            this.labelTagID.Location = new System.Drawing.Point(17, 74);
            this.labelTagID.Name = "labelTagID";
            this.labelTagID.Size = new System.Drawing.Size(116, 20);
            this.labelTagID.TabIndex = 3;
            this.labelTagID.Text = "Enter :";
            this.labelTagID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonConnect);
            this.groupBox1.Controls.Add(this.cbxAutoWrite);
            this.groupBox1.Controls.Add(this.buttonDispose);
            this.groupBox1.Controls.Add(this.buttonCreate);
            this.groupBox1.Controls.Add(this.buttonRefresh);
            this.groupBox1.Controls.Add(this.comboBoxDevice);
            this.groupBox1.Controls.Add(this.checkBoxUseRFid);
            this.groupBox1.Location = new System.Drawing.Point(326, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(292, 112);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rfid";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(19, 79);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(92, 27);
            this.buttonConnect.TabIndex = 13;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // cbxAutoWrite
            // 
            this.cbxAutoWrite.AutoSize = true;
            this.cbxAutoWrite.Location = new System.Drawing.Point(173, 21);
            this.cbxAutoWrite.Name = "cbxAutoWrite";
            this.cbxAutoWrite.Size = new System.Drawing.Size(109, 20);
            this.cbxAutoWrite.TabIndex = 12;
            this.cbxAutoWrite.Text = "Write SPCE2";
            this.cbxAutoWrite.UseVisualStyleBackColor = true;
            this.cbxAutoWrite.Visible = false;
            // 
            // buttonDispose
            // 
            this.buttonDispose.Enabled = false;
            this.buttonDispose.Location = new System.Drawing.Point(190, 79);
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
            this.buttonCreate.Location = new System.Drawing.Point(19, 79);
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
            this.buttonRefresh.Location = new System.Drawing.Point(255, 42);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(27, 31);
            this.buttonRefresh.TabIndex = 9;
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // comboBoxDevice
            // 
            this.comboBoxDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDevice.FormattingEnabled = true;
            this.comboBoxDevice.Location = new System.Drawing.Point(19, 49);
            this.comboBoxDevice.Name = "comboBoxDevice";
            this.comboBoxDevice.Size = new System.Drawing.Size(230, 24);
            this.comboBoxDevice.TabIndex = 8;
            this.comboBoxDevice.SelectedIndexChanged += new System.EventHandler(this.comboBoxDevice_SelectedIndexChanged);
            // 
            // checkBoxUseRFid
            // 
            this.checkBoxUseRFid.AutoSize = true;
            this.checkBoxUseRFid.Location = new System.Drawing.Point(19, 23);
            this.checkBoxUseRFid.Name = "checkBoxUseRFid";
            this.checkBoxUseRFid.Size = new System.Drawing.Size(86, 20);
            this.checkBoxUseRFid.TabIndex = 0;
            this.checkBoxUseRFid.Text = "Use RFID";
            this.checkBoxUseRFid.UseVisualStyleBackColor = true;
            this.checkBoxUseRFid.Click += new System.EventHandler(this.checkBoxUseRFid_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 134);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(626, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelInfo
            // 
            this.toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
            this.toolStripStatusLabelInfo.Size = new System.Drawing.Size(34, 17);
            this.toolStripStatusLabelInfo.Text = "Info :";
            // 
            // buttonNext
            // 
            this.buttonNext.BackgroundImage = global::smartTracker.Properties.Resources.Next;
            this.buttonNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonNext.Location = new System.Drawing.Point(277, 25);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(32, 33);
            this.buttonNext.TabIndex = 7;
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // AutoFillBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 156);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelTagID);
            this.Controls.Add(this.labelLotID);
            this.Controls.Add(this.textBoxTagID);
            this.Controls.Add(this.textBoxLotID);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoFillBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AutoFillBox_FormClosing);
            this.Load += new System.EventHandler(this.AutoFillBox_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxLotID;
        private System.Windows.Forms.TextBox textBoxTagID;
        private System.Windows.Forms.Label labelLotID;
        private System.Windows.Forms.Label labelTagID;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonDispose;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.ComboBox comboBoxDevice;
        private System.Windows.Forms.CheckBox checkBoxUseRFid;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelInfo;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.CheckBox cbxAutoWrite;
        private System.Windows.Forms.Button buttonConnect;
    }
}