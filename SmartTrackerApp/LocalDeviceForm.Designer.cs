namespace smartTracker
{
    partial class LocalDeviceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocalDeviceForm));
            this.toolStripLocalDevice = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonCreateNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonReset = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonApply = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.groupBoxDevice = new System.Windows.Forms.GroupBox();
            this.listBoxLocalDevice = new System.Windows.Forms.ListBox();
            this.groupBoxLocalUserCtrl = new System.Windows.Forms.GroupBox();
            this.comboBoxFridgeType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.btUpdateBadgeCom = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioHF = new System.Windows.Forms.RadioButton();
            this.radioLF = new System.Windows.Forms.RadioButton();
            this.textBoxSlaveBadgeReader = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pB1 = new System.Windows.Forms.PictureBox();
            this.listBoxReader = new System.Windows.Forms.ListBox();
            this.textBoxTempReader = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBoxSerialFPSlave = new System.Windows.Forms.ComboBox();
            this.comboBoxSerialFPMaster = new System.Windows.Forms.ComboBox();
            this.comboBoxSerialRFID = new System.Windows.Forms.ComboBox();
            this.textBoxMasterBadgeReader = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.checkBoxEnabledDevice = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDeviceName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.imageListReader = new System.Windows.Forms.ImageList(this.components);
            this.toolStripLocalDevice.SuspendLayout();
            this.groupBoxDevice.SuspendLayout();
            this.groupBoxLocalUserCtrl.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pB1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripLocalDevice
            // 
            this.toolStripLocalDevice.AutoSize = false;
            this.toolStripLocalDevice.BackColor = System.Drawing.Color.White;
            this.toolStripLocalDevice.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLocalDevice.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripLocalDevice.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonCreateNew,
            this.toolStripButtonReset,
            this.toolStripButtonDelete,
            this.toolStripButtonApply,
            this.toolStripButtonExit});
            this.toolStripLocalDevice.Location = new System.Drawing.Point(0, 0);
            this.toolStripLocalDevice.Name = "toolStripLocalDevice";
            this.toolStripLocalDevice.Size = new System.Drawing.Size(978, 35);
            this.toolStripLocalDevice.TabIndex = 0;
            this.toolStripLocalDevice.Text = "toolStrip1";
            // 
            // toolStripButtonCreateNew
            // 
            this.toolStripButtonCreateNew.Image = global::smartTracker.Properties.Resources.addhardware;
            this.toolStripButtonCreateNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCreateNew.Name = "toolStripButtonCreateNew";
            this.toolStripButtonCreateNew.Size = new System.Drawing.Size(120, 32);
            this.toolStripButtonCreateNew.Text = "Create New";
            this.toolStripButtonCreateNew.Click += new System.EventHandler(this.toolStripButtonCreateNew_Click);
            // 
            // toolStripButtonReset
            // 
            this.toolStripButtonReset.Image = global::smartTracker.Properties.Resources.edit_clear;
            this.toolStripButtonReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReset.Name = "toolStripButtonReset";
            this.toolStripButtonReset.Size = new System.Drawing.Size(77, 32);
            this.toolStripButtonReset.Text = "Reset";
            this.toolStripButtonReset.Click += new System.EventHandler(this.toolStripButtonReset_Click);
            // 
            // toolStripButtonDelete
            // 
            this.toolStripButtonDelete.Enabled = false;
            this.toolStripButtonDelete.Image = global::smartTracker.Properties.Resources.removehardware;
            this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Size = new System.Drawing.Size(83, 32);
            this.toolStripButtonDelete.Text = "Delete";
            this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            // 
            // toolStripButtonApply
            // 
            this.toolStripButtonApply.Enabled = false;
            this.toolStripButtonApply.Image = global::smartTracker.Properties.Resources.apply;
            this.toolStripButtonApply.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonApply.Name = "toolStripButtonApply";
            this.toolStripButtonApply.Size = new System.Drawing.Size(77, 32);
            this.toolStripButtonApply.Text = "Apply";
            this.toolStripButtonApply.Click += new System.EventHandler(this.toolStripButtonApply_Click);
            // 
            // toolStripButtonExit
            // 
            this.toolStripButtonExit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExit.Image")));
            this.toolStripButtonExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExit.Name = "toolStripButtonExit";
            this.toolStripButtonExit.Size = new System.Drawing.Size(64, 32);
            this.toolStripButtonExit.Text = "Exit";
            this.toolStripButtonExit.Click += new System.EventHandler(this.toolStripButtonExit_Click);
            // 
            // groupBoxDevice
            // 
            this.groupBoxDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxDevice.Controls.Add(this.listBoxLocalDevice);
            this.groupBoxDevice.Location = new System.Drawing.Point(13, 41);
            this.groupBoxDevice.Name = "groupBoxDevice";
            this.groupBoxDevice.Size = new System.Drawing.Size(951, 144);
            this.groupBoxDevice.TabIndex = 1;
            this.groupBoxDevice.TabStop = false;
            this.groupBoxDevice.Text = "Existing Devices";
            // 
            // listBoxLocalDevice
            // 
            this.listBoxLocalDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxLocalDevice.FormattingEnabled = true;
            this.listBoxLocalDevice.Location = new System.Drawing.Point(17, 17);
            this.listBoxLocalDevice.Name = "listBoxLocalDevice";
            this.listBoxLocalDevice.Size = new System.Drawing.Size(918, 108);
            this.listBoxLocalDevice.TabIndex = 0;
            this.listBoxLocalDevice.SelectedIndexChanged += new System.EventHandler(this.listBoxLocalDevice_SelectedIndexChanged);
            // 
            // groupBoxLocalUserCtrl
            // 
            this.groupBoxLocalUserCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxLocalUserCtrl.Controls.Add(this.comboBoxFridgeType);
            this.groupBoxLocalUserCtrl.Controls.Add(this.label6);
            this.groupBoxLocalUserCtrl.Controls.Add(this.button1);
            this.groupBoxLocalUserCtrl.Controls.Add(this.btUpdateBadgeCom);
            this.groupBoxLocalUserCtrl.Controls.Add(this.groupBox1);
            this.groupBoxLocalUserCtrl.Controls.Add(this.textBoxSlaveBadgeReader);
            this.groupBoxLocalUserCtrl.Controls.Add(this.label5);
            this.groupBoxLocalUserCtrl.Controls.Add(this.pB1);
            this.groupBoxLocalUserCtrl.Controls.Add(this.listBoxReader);
            this.groupBoxLocalUserCtrl.Controls.Add(this.textBoxTempReader);
            this.groupBoxLocalUserCtrl.Controls.Add(this.label12);
            this.groupBoxLocalUserCtrl.Controls.Add(this.comboBoxSerialFPSlave);
            this.groupBoxLocalUserCtrl.Controls.Add(this.comboBoxSerialFPMaster);
            this.groupBoxLocalUserCtrl.Controls.Add(this.comboBoxSerialRFID);
            this.groupBoxLocalUserCtrl.Controls.Add(this.textBoxMasterBadgeReader);
            this.groupBoxLocalUserCtrl.Controls.Add(this.label10);
            this.groupBoxLocalUserCtrl.Controls.Add(this.checkBoxEnabledDevice);
            this.groupBoxLocalUserCtrl.Controls.Add(this.label2);
            this.groupBoxLocalUserCtrl.Controls.Add(this.label4);
            this.groupBoxLocalUserCtrl.Controls.Add(this.label3);
            this.groupBoxLocalUserCtrl.Controls.Add(this.textBoxDeviceName);
            this.groupBoxLocalUserCtrl.Controls.Add(this.label1);
            this.groupBoxLocalUserCtrl.Enabled = false;
            this.groupBoxLocalUserCtrl.Location = new System.Drawing.Point(12, 198);
            this.groupBoxLocalUserCtrl.Name = "groupBoxLocalUserCtrl";
            this.groupBoxLocalUserCtrl.Size = new System.Drawing.Size(952, 299);
            this.groupBoxLocalUserCtrl.TabIndex = 2;
            this.groupBoxLocalUserCtrl.TabStop = false;
            this.groupBoxLocalUserCtrl.Text = "Device Information";
            // 
            // comboBoxFridgeType
            // 
            this.comboBoxFridgeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFridgeType.FormattingEnabled = true;
            this.comboBoxFridgeType.Items.AddRange(new object[] {
            "Unknown",
            "Sonde Liebherr (Com)",
            "Sonde Evermed (Com)",
            "Sonde PT100 (Usb)",
            "sonde Fanem (Com)"});
            this.comboBoxFridgeType.Location = new System.Drawing.Point(626, 240);
            this.comboBoxFridgeType.Name = "comboBoxFridgeType";
            this.comboBoxFridgeType.Size = new System.Drawing.Size(162, 21);
            this.comboBoxFridgeType.TabIndex = 40;
            this.comboBoxFridgeType.SelectedIndexChanged += new System.EventHandler(this.comboBoxFridgeType_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(538, 243);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 39;
            this.label6.Text = "Fridge Type :";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(354, 30);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 29);
            this.button1.TabIndex = 38;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btUpdateBadgeCom
            // 
            this.btUpdateBadgeCom.Location = new System.Drawing.Point(803, 164);
            this.btUpdateBadgeCom.Name = "btUpdateBadgeCom";
            this.btUpdateBadgeCom.Size = new System.Drawing.Size(91, 25);
            this.btUpdateBadgeCom.TabIndex = 37;
            this.btUpdateBadgeCom.Text = "Update Com";
            this.btUpdateBadgeCom.UseVisualStyleBackColor = true;
            this.btUpdateBadgeCom.Visible = false;
            this.btUpdateBadgeCom.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioHF);
            this.groupBox1.Controls.Add(this.radioLF);
            this.groupBox1.Location = new System.Drawing.Point(690, 154);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(98, 80);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Badge  Type";
            // 
            // radioHF
            // 
            this.radioHF.AutoSize = true;
            this.radioHF.Checked = true;
            this.radioHF.Location = new System.Drawing.Point(30, 47);
            this.radioHF.Name = "radioHF";
            this.radioHF.Size = new System.Drawing.Size(39, 17);
            this.radioHF.TabIndex = 35;
            this.radioHF.TabStop = true;
            this.radioHF.Text = "HF";
            this.radioHF.UseVisualStyleBackColor = true;
            this.radioHF.Click += new System.EventHandler(this.radioHF_Click);
            // 
            // radioLF
            // 
            this.radioLF.AutoSize = true;
            this.radioLF.Location = new System.Drawing.Point(30, 24);
            this.radioLF.Name = "radioLF";
            this.radioLF.Size = new System.Drawing.Size(37, 17);
            this.radioLF.TabIndex = 34;
            this.radioLF.Text = "LF";
            this.radioLF.UseVisualStyleBackColor = true;
            this.radioLF.Click += new System.EventHandler(this.radioLF_Click);
            // 
            // textBoxSlaveBadgeReader
            // 
            this.textBoxSlaveBadgeReader.Location = new System.Drawing.Point(626, 186);
            this.textBoxSlaveBadgeReader.Name = "textBoxSlaveBadgeReader";
            this.textBoxSlaveBadgeReader.Size = new System.Drawing.Size(58, 21);
            this.textBoxSlaveBadgeReader.TabIndex = 5;
            this.textBoxSlaveBadgeReader.Text = "COM";
            this.textBoxSlaveBadgeReader.TextChanged += new System.EventHandler(this.textBoxComSlave_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(459, 189);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(161, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Slave Badge reader COM :";
            // 
            // pB1
            // 
            this.pB1.BackColor = System.Drawing.Color.White;
            this.pB1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pB1.Location = new System.Drawing.Point(279, 100);
            this.pB1.Name = "pB1";
            this.pB1.Size = new System.Drawing.Size(160, 173);
            this.pB1.TabIndex = 32;
            this.pB1.TabStop = false;
            // 
            // listBoxReader
            // 
            this.listBoxReader.FormattingEnabled = true;
            this.listBoxReader.Items.AddRange(new object[] {
            "Diamond Smart Box\t(DT_DSB)",
            "Jewelry Smart Cabinet\t(DT_JSC)",
            "Smart SAS\t\t(DT_SAS)",
            "Smart Drawer\t\t(DT_MSR)",
            "Smart Board\t\t(DT_SBR)",
            "Medical Cabinet\t\t(DT_SMC)",
            "Fridge\t\t\t(DT_SFR)",
            "Smart Station / SmartPad\t(DT_STR)",
            "Blood Fridge\t\t(DT_SBF)"});
            this.listBoxReader.Location = new System.Drawing.Point(20, 100);
            this.listBoxReader.Name = "listBoxReader";
            this.listBoxReader.Size = new System.Drawing.Size(250, 173);
            this.listBoxReader.TabIndex = 31;
            this.listBoxReader.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listBoxReader_MouseClick);
            this.listBoxReader.SelectedIndexChanged += new System.EventHandler(this.listBoxReader_SelectedIndexChanged);
            this.listBoxReader.MouseLeave += new System.EventHandler(this.listBoxReader_MouseLeave);
            this.listBoxReader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBoxReader_MouseMove);
            // 
            // textBoxTempReader
            // 
            this.textBoxTempReader.Location = new System.Drawing.Point(626, 213);
            this.textBoxTempReader.Name = "textBoxTempReader";
            this.textBoxTempReader.Size = new System.Drawing.Size(58, 21);
            this.textBoxTempReader.TabIndex = 6;
            this.textBoxTempReader.Text = "COM";
            this.textBoxTempReader.TextChanged += new System.EventHandler(this.textBoxTempReader_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(515, 216);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(105, 13);
            this.label12.TabIndex = 29;
            this.label12.Text = "T° Reader COM :";
            // 
            // comboBoxSerialFPSlave
            // 
            this.comboBoxSerialFPSlave.FormattingEnabled = true;
            this.comboBoxSerialFPSlave.Location = new System.Drawing.Point(550, 127);
            this.comboBoxSerialFPSlave.Name = "comboBoxSerialFPSlave";
            this.comboBoxSerialFPSlave.Size = new System.Drawing.Size(363, 21);
            this.comboBoxSerialFPSlave.TabIndex = 3;
            this.comboBoxSerialFPSlave.TextUpdate += new System.EventHandler(this.textBoxSerialFPSlave_TextChanged);
            this.comboBoxSerialFPSlave.SelectedValueChanged += new System.EventHandler(this.textBoxSerialFPSlave_TextChanged);
            this.comboBoxSerialFPSlave.TextChanged += new System.EventHandler(this.textBoxSerialFPSlave_TextChanged);
            // 
            // comboBoxSerialFPMaster
            // 
            this.comboBoxSerialFPMaster.FormattingEnabled = true;
            this.comboBoxSerialFPMaster.Location = new System.Drawing.Point(550, 101);
            this.comboBoxSerialFPMaster.Name = "comboBoxSerialFPMaster";
            this.comboBoxSerialFPMaster.Size = new System.Drawing.Size(363, 21);
            this.comboBoxSerialFPMaster.TabIndex = 2;
            this.comboBoxSerialFPMaster.SelectedIndexChanged += new System.EventHandler(this.textBoxSerialFPMaster_TextChanged);
            this.comboBoxSerialFPMaster.TextUpdate += new System.EventHandler(this.textBoxSerialFPMaster_TextChanged);
            this.comboBoxSerialFPMaster.TextChanged += new System.EventHandler(this.textBoxSerialFPMaster_TextChanged);
            // 
            // comboBoxSerialRFID
            // 
            this.comboBoxSerialRFID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSerialRFID.FormattingEnabled = true;
            this.comboBoxSerialRFID.Location = new System.Drawing.Point(139, 33);
            this.comboBoxSerialRFID.Name = "comboBoxSerialRFID";
            this.comboBoxSerialRFID.Size = new System.Drawing.Size(209, 21);
            this.comboBoxSerialRFID.TabIndex = 0;
            this.comboBoxSerialRFID.SelectedIndexChanged += new System.EventHandler(this.textBoxSerialRFID_TextChanged);
            this.comboBoxSerialRFID.TextUpdate += new System.EventHandler(this.textBoxSerialRFID_TextChanged);
            this.comboBoxSerialRFID.TextChanged += new System.EventHandler(this.textBoxSerialRFID_TextChanged);
            // 
            // textBoxMasterBadgeReader
            // 
            this.textBoxMasterBadgeReader.Location = new System.Drawing.Point(626, 159);
            this.textBoxMasterBadgeReader.Name = "textBoxMasterBadgeReader";
            this.textBoxMasterBadgeReader.Size = new System.Drawing.Size(58, 21);
            this.textBoxMasterBadgeReader.TabIndex = 4;
            this.textBoxMasterBadgeReader.Text = "COM";
            this.textBoxMasterBadgeReader.TextChanged += new System.EventHandler(this.textBoxComMaster_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(453, 162);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(167, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Master Badge reader COM :";
            // 
            // checkBoxEnabledDevice
            // 
            this.checkBoxEnabledDevice.AutoSize = true;
            this.checkBoxEnabledDevice.Location = new System.Drawing.Point(450, 37);
            this.checkBoxEnabledDevice.Name = "checkBoxEnabledDevice";
            this.checkBoxEnabledDevice.Size = new System.Drawing.Size(114, 17);
            this.checkBoxEnabledDevice.TabIndex = 14;
            this.checkBoxEnabledDevice.Text = "Enabled Device";
            this.checkBoxEnabledDevice.UseVisualStyleBackColor = true;
            this.checkBoxEnabledDevice.CheckedChanged += new System.EventHandler(this.checkBoxEnabledDevice_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "RFID S/N:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(453, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "FP Slave S/N :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(447, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "FP Master S/N :";
            // 
            // textBoxDeviceName
            // 
            this.textBoxDeviceName.Location = new System.Drawing.Point(139, 59);
            this.textBoxDeviceName.Name = "textBoxDeviceName";
            this.textBoxDeviceName.Size = new System.Drawing.Size(209, 21);
            this.textBoxDeviceName.TabIndex = 1;
            this.textBoxDeviceName.TextChanged += new System.EventHandler(this.textBoxDeviceName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Device nickName :";
            // 
            // imageListReader
            // 
            this.imageListReader.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListReader.ImageStream")));
            this.imageListReader.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListReader.Images.SetKeyName(0, "DT_DSB");
            this.imageListReader.Images.SetKeyName(1, "DT_JSC");
            this.imageListReader.Images.SetKeyName(2, "DT_MSR");
            this.imageListReader.Images.SetKeyName(3, "DT_SAS");
            this.imageListReader.Images.SetKeyName(4, "DT_SBR");
            this.imageListReader.Images.SetKeyName(5, "DT_SFR");
            this.imageListReader.Images.SetKeyName(6, "DT_SMC");
            this.imageListReader.Images.SetKeyName(7, "DT_STR");
            // 
            // LocalDeviceForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(978, 509);
            this.Controls.Add(this.groupBoxLocalUserCtrl);
            this.Controls.Add(this.groupBoxDevice);
            this.Controls.Add(this.toolStripLocalDevice);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(664, 483);
            this.Name = "LocalDeviceForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Local Device";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LocalDeviceForm_FormClosing);
            this.Load += new System.EventHandler(this.LocalDeviceForm_Load);
            this.toolStripLocalDevice.ResumeLayout(false);
            this.toolStripLocalDevice.PerformLayout();
            this.groupBoxDevice.ResumeLayout(false);
            this.groupBoxLocalUserCtrl.ResumeLayout(false);
            this.groupBoxLocalUserCtrl.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pB1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripLocalDevice;
        private System.Windows.Forms.GroupBox groupBoxDevice;
        private System.Windows.Forms.ListBox listBoxLocalDevice;
        private System.Windows.Forms.GroupBox groupBoxLocalUserCtrl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDeviceName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripButton toolStripButtonCreateNew;
        private System.Windows.Forms.ToolStripButton toolStripButtonReset;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
        private System.Windows.Forms.ToolStripButton toolStripButtonApply;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxEnabledDevice;
        private System.Windows.Forms.TextBox textBoxMasterBadgeReader;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboBoxSerialRFID;
        private System.Windows.Forms.ComboBox comboBoxSerialFPMaster;
        private System.Windows.Forms.ComboBox comboBoxSerialFPSlave;
        private System.Windows.Forms.TextBox textBoxTempReader;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ListBox listBoxReader;
        private System.Windows.Forms.PictureBox pB1;
        private System.Windows.Forms.ImageList imageListReader;
        private System.Windows.Forms.TextBox textBoxSlaveBadgeReader;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioHF;
        private System.Windows.Forms.RadioButton radioLF;
        private System.Windows.Forms.Button btUpdateBadgeCom;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxFridgeType;
    }
}