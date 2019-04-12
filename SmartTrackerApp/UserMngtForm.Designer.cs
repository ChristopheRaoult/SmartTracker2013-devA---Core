using smartTracker.Properties;

namespace smartTracker
{
    partial class UserMngtForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserMngtForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.listBoxUser = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.toolStripUserMngt = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonCreateNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonReset = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEnrollFingerprint = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonApply = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridViewGrant = new System.Windows.Forms.DataGridView();
            this.DoorGranted = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.groupBoxUserCtrl = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButtonHF = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxCom = new System.Windows.Forms.TextBox();
            this.radioButtonLF = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonBadge = new System.Windows.Forms.Button();
            this.comboBoxRemoteReader = new System.Windows.Forms.ComboBox();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxFinger = new System.Windows.Forms.ComboBox();
            this.textBoxRemoveValue = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxRemoveItem = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxReaderCard = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridViewFinger = new System.Windows.Forms.DataGridView();
            this.Finger = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Enrolled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.textBoxLastName = new System.Windows.Forms.TextBox();
            this.textBoxFirstName = new System.Windows.Forms.TextBox();
            this.gpBx = new System.Windows.Forms.GroupBox();
            this.cbxFinger = new System.Windows.Forms.ComboBox();
            this.pBarEnroll = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btEnroll = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.cbArmReader = new System.Windows.Forms.ComboBox();
            this.backgroundWorkerEnroll = new System.ComponentModel.BackgroundWorker();
            this.toolStripUserMngt.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGrant)).BeginInit();
            this.groupBoxUserCtrl.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFinger)).BeginInit();
            this.gpBx.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxUser
            // 
            this.listBoxUser.FormattingEnabled = true;
            this.listBoxUser.Location = new System.Drawing.Point(12, 17);
            this.listBoxUser.Name = "listBoxUser";
            this.listBoxUser.Size = new System.Drawing.Size(294, 134);
            this.listBoxUser.TabIndex = 0;
            this.listBoxUser.SelectedIndexChanged += new System.EventHandler(this.listBoxUser_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "First Name :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Last Name :";
            // 
            // toolStripUserMngt
            // 
            this.toolStripUserMngt.AutoSize = false;
            this.toolStripUserMngt.BackColor = System.Drawing.Color.White;
            this.toolStripUserMngt.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripUserMngt.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripUserMngt.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonCreateNew,
            this.toolStripButtonReset,
            this.toolStripButtonEnrollFingerprint,
            this.toolStripButtonDelete,
            this.toolStripButtonApply,
            this.toolStripButtonExit});
            this.toolStripUserMngt.Location = new System.Drawing.Point(0, 0);
            this.toolStripUserMngt.Name = "toolStripUserMngt";
            this.toolStripUserMngt.Size = new System.Drawing.Size(1003, 35);
            this.toolStripUserMngt.TabIndex = 3;
            this.toolStripUserMngt.Text = "toolStripUserMngt";
            // 
            // toolStripButtonCreateNew
            // 
            this.toolStripButtonCreateNew.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonCreateNew.Image = global::smartTracker.Properties.Resources.add_user;
            this.toolStripButtonCreateNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCreateNew.Name = "toolStripButtonCreateNew";
            this.toolStripButtonCreateNew.Size = new System.Drawing.Size(120, 32);
            this.toolStripButtonCreateNew.Text = "Create New";
            this.toolStripButtonCreateNew.Click += new System.EventHandler(this.toolStripButtonCreateNew_Click);
            // 
            // toolStripButtonReset
            // 
            this.toolStripButtonReset.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonReset.Image = global::smartTracker.Properties.Resources.edit_clear;
            this.toolStripButtonReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReset.Name = "toolStripButtonReset";
            this.toolStripButtonReset.Size = new System.Drawing.Size(77, 32);
            this.toolStripButtonReset.Text = "Reset";
            this.toolStripButtonReset.Click += new System.EventHandler(this.toolStripButtonReset_Click);
            // 
            // toolStripButtonEnrollFingerprint
            // 
            this.toolStripButtonEnrollFingerprint.Enabled = false;
            this.toolStripButtonEnrollFingerprint.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonEnrollFingerprint.Image = global::smartTracker.Properties.Resources.enroll;
            this.toolStripButtonEnrollFingerprint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEnrollFingerprint.Name = "toolStripButtonEnrollFingerprint";
            this.toolStripButtonEnrollFingerprint.Size = new System.Drawing.Size(161, 32);
            this.toolStripButtonEnrollFingerprint.Text = "Enroll Fingerprint";
            this.toolStripButtonEnrollFingerprint.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButtonDelete
            // 
            this.toolStripButtonDelete.Enabled = false;
            this.toolStripButtonDelete.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonDelete.Image = global::smartTracker.Properties.Resources.delete_user;
            this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Size = new System.Drawing.Size(83, 32);
            this.toolStripButtonDelete.Text = "Delete";
            this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            // 
            // toolStripButtonApply
            // 
            this.toolStripButtonApply.Enabled = false;
            this.toolStripButtonApply.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonApply.Image = global::smartTracker.Properties.Resources.apply;
            this.toolStripButtonApply.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonApply.Name = "toolStripButtonApply";
            this.toolStripButtonApply.Size = new System.Drawing.Size(77, 32);
            this.toolStripButtonApply.Text = "Apply";
            this.toolStripButtonApply.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripButtonExit
            // 
            this.toolStripButtonExit.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonExit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExit.Image")));
            this.toolStripButtonExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExit.Name = "toolStripButtonExit";
            this.toolStripButtonExit.Size = new System.Drawing.Size(64, 32);
            this.toolStripButtonExit.Text = "Exit";
            this.toolStripButtonExit.Click += new System.EventHandler(this.toolStripButtonExit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dataGridViewGrant);
            this.groupBox1.Controls.Add(this.listBoxUser);
            this.groupBox1.Location = new System.Drawing.Point(13, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(976, 165);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Existing Users";
            // 
            // dataGridViewGrant
            // 
            this.dataGridViewGrant.AllowUserToAddRows = false;
            this.dataGridViewGrant.AllowUserToDeleteRows = false;
            this.dataGridViewGrant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewGrant.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewGrant.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewGrant.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DoorGranted});
            this.dataGridViewGrant.Location = new System.Drawing.Point(328, 17);
            this.dataGridViewGrant.Name = "dataGridViewGrant";
            this.dataGridViewGrant.Size = new System.Drawing.Size(628, 134);
            this.dataGridViewGrant.TabIndex = 1;
            // 
            // DoorGranted
            // 
            this.DoorGranted.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.DoorGranted.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.DoorGranted.FillWeight = 180F;
            this.DoorGranted.HeaderText = "Door Granted";
            this.DoorGranted.Name = "DoorGranted";
            this.DoorGranted.Width = 180;
            // 
            // groupBoxUserCtrl
            // 
            this.groupBoxUserCtrl.Controls.Add(this.groupBox4);
            this.groupBoxUserCtrl.Controls.Add(this.groupBox3);
            this.groupBoxUserCtrl.Controls.Add(this.groupBox2);
            this.groupBoxUserCtrl.Controls.Add(this.comboBoxFinger);
            this.groupBoxUserCtrl.Controls.Add(this.textBoxRemoveValue);
            this.groupBoxUserCtrl.Controls.Add(this.label6);
            this.groupBoxUserCtrl.Controls.Add(this.textBoxRemoveItem);
            this.groupBoxUserCtrl.Controls.Add(this.label5);
            this.groupBoxUserCtrl.Controls.Add(this.label4);
            this.groupBoxUserCtrl.Controls.Add(this.textBoxReaderCard);
            this.groupBoxUserCtrl.Controls.Add(this.label3);
            this.groupBoxUserCtrl.Controls.Add(this.dataGridViewFinger);
            this.groupBoxUserCtrl.Controls.Add(this.textBoxLastName);
            this.groupBoxUserCtrl.Controls.Add(this.textBoxFirstName);
            this.groupBoxUserCtrl.Controls.Add(this.label2);
            this.groupBoxUserCtrl.Controls.Add(this.label1);
            this.groupBoxUserCtrl.Enabled = false;
            this.groupBoxUserCtrl.Location = new System.Drawing.Point(12, 218);
            this.groupBoxUserCtrl.Name = "groupBoxUserCtrl";
            this.groupBoxUserCtrl.Size = new System.Drawing.Size(532, 356);
            this.groupBoxUserCtrl.TabIndex = 5;
            this.groupBoxUserCtrl.TabStop = false;
            this.groupBoxUserCtrl.Text = "User Information";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox4.Controls.Add(this.radioButtonHF);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.textBoxCom);
            this.groupBox4.Controls.Add(this.radioButtonLF);
            this.groupBox4.Location = new System.Drawing.Point(10, 267);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(280, 76);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Local Badge Reader";
            // 
            // radioButtonHF
            // 
            this.radioButtonHF.AutoSize = true;
            this.radioButtonHF.Checked = true;
            this.radioButtonHF.Location = new System.Drawing.Point(140, 20);
            this.radioButtonHF.Name = "radioButtonHF";
            this.radioButtonHF.Size = new System.Drawing.Size(79, 17);
            this.radioButtonHF.TabIndex = 18;
            this.radioButtonHF.TabStop = true;
            this.radioButtonHF.Text = "13,56Mhz";
            this.radioButtonHF.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(128, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Com Badge Reader :";
            // 
            // textBoxCom
            // 
            this.textBoxCom.AcceptsReturn = true;
            this.textBoxCom.Location = new System.Drawing.Point(149, 43);
            this.textBoxCom.Name = "textBoxCom";
            this.textBoxCom.Size = new System.Drawing.Size(59, 21);
            this.textBoxCom.TabIndex = 16;
            this.textBoxCom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxCom_KeyUp);
            // 
            // radioButtonLF
            // 
            this.radioButtonLF.AutoSize = true;
            this.radioButtonLF.Location = new System.Drawing.Point(26, 20);
            this.radioButtonLF.Name = "radioButtonLF";
            this.radioButtonLF.Size = new System.Drawing.Size(67, 17);
            this.radioButtonLF.TabIndex = 17;
            this.radioButtonLF.Text = "125Khz";
            this.radioButtonLF.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.buttonBadge);
            this.groupBox3.Controls.Add(this.comboBoxRemoteReader);
            this.groupBox3.Controls.Add(this.radioButton4);
            this.groupBox3.Controls.Add(this.radioButton3);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.textBox2);
            this.groupBox3.Location = new System.Drawing.Point(10, 197);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(280, 64);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Remote Badge Reader";
            // 
            // buttonBadge
            // 
            this.buttonBadge.Enabled = false;
            this.buttonBadge.Location = new System.Drawing.Point(198, 22);
            this.buttonBadge.Name = "buttonBadge";
            this.buttonBadge.Size = new System.Drawing.Size(76, 27);
            this.buttonBadge.TabIndex = 20;
            this.buttonBadge.Text = "Get Badge";
            this.buttonBadge.UseVisualStyleBackColor = true;
            this.buttonBadge.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBoxRemoteReader
            // 
            this.comboBoxRemoteReader.FormattingEnabled = true;
            this.comboBoxRemoteReader.Location = new System.Drawing.Point(11, 26);
            this.comboBoxRemoteReader.Name = "comboBoxRemoteReader";
            this.comboBoxRemoteReader.Size = new System.Drawing.Size(170, 21);
            this.comboBoxRemoteReader.TabIndex = 19;
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Checked = true;
            this.radioButton4.Location = new System.Drawing.Point(117, 109);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(79, 17);
            this.radioButton4.TabIndex = 18;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "13,56Mhz";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(3, 109);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(67, 17);
            this.radioButton3.TabIndex = 17;
            this.radioButton3.Text = "125Khz";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(-8, 135);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(128, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Com Badge Reader :";
            // 
            // textBox2
            // 
            this.textBox2.AcceptsReturn = true;
            this.textBox2.Location = new System.Drawing.Point(126, 132);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(59, 21);
            this.textBox2.TabIndex = 16;
            this.textBox2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxCom_KeyUp);
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(15, 197);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(276, 64);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Local Badge Reader";
            // 
            // comboBoxFinger
            // 
            this.comboBoxFinger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFinger.FormattingEnabled = true;
            this.comboBoxFinger.Items.AddRange(new object[] {
            global::smartTracker.Properties.ResStrings.str_None,
            global::smartTracker.Properties.ResStrings.strr_Right_Thumb,
            global::smartTracker.Properties.ResStrings.str_Right_Index,
            global::smartTracker.Properties.ResStrings.str_Right_Middle,
            global::smartTracker.Properties.ResStrings.str_Right_Ring,
            global::smartTracker.Properties.ResStrings.str_Right_Little,
            global::smartTracker.Properties.ResStrings.str_Left_Thumb,
            global::smartTracker.Properties.ResStrings.str_Left_Index,
            global::smartTracker.Properties.ResStrings.str_Left_Middle,
            global::smartTracker.Properties.ResStrings.str_Left_Ring,
            global::smartTracker.Properties.ResStrings.str_Left_Little});
            this.comboBoxFinger.Location = new System.Drawing.Point(131, 108);
            this.comboBoxFinger.MaxDropDownItems = 11;
            this.comboBoxFinger.Name = "comboBoxFinger";
            this.comboBoxFinger.Size = new System.Drawing.Size(159, 21);
            this.comboBoxFinger.TabIndex = 14;
            this.comboBoxFinger.SelectedIndexChanged += new System.EventHandler(this.comboBoxFinger_SelectedIndexChanged);
            // 
            // textBoxRemoveValue
            // 
            this.textBoxRemoveValue.Location = new System.Drawing.Point(131, 164);
            this.textBoxRemoveValue.Name = "textBoxRemoveValue";
            this.textBoxRemoveValue.Size = new System.Drawing.Size(160, 21);
            this.textBoxRemoveValue.TabIndex = 12;
            this.textBoxRemoveValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxRemoveItem_KeyDown);
            this.textBoxRemoveValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxRemoveValue_KeyPress);
            this.textBoxRemoveValue.Leave += new System.EventHandler(this.textBoxRemoveValue_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(-3, 167);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Max Remove Values:";
            // 
            // textBoxRemoveItem
            // 
            this.textBoxRemoveItem.Location = new System.Drawing.Point(131, 136);
            this.textBoxRemoveItem.Name = "textBoxRemoveItem";
            this.textBoxRemoveItem.Size = new System.Drawing.Size(160, 21);
            this.textBoxRemoveItem.TabIndex = 10;
            this.textBoxRemoveItem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxRemoveItem_KeyDown);
            this.textBoxRemoveItem.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxRemoveItem_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Max Remove Items:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Finger Alert :";
            // 
            // textBoxReaderCard
            // 
            this.textBoxReaderCard.Location = new System.Drawing.Point(131, 80);
            this.textBoxReaderCard.Name = "textBoxReaderCard";
            this.textBoxReaderCard.Size = new System.Drawing.Size(160, 21);
            this.textBoxReaderCard.TabIndex = 6;
            this.textBoxReaderCard.TextChanged += new System.EventHandler(this.textBoxReaderCard_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "ReaderCard ID :";
            // 
            // dataGridViewFinger
            // 
            this.dataGridViewFinger.AllowUserToAddRows = false;
            this.dataGridViewFinger.AllowUserToDeleteRows = false;
            this.dataGridViewFinger.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewFinger.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewFinger.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewFinger.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewFinger.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFinger.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Finger,
            this.Enrolled});
            this.dataGridViewFinger.Location = new System.Drawing.Point(308, 24);
            this.dataGridViewFinger.Name = "dataGridViewFinger";
            this.dataGridViewFinger.Size = new System.Drawing.Size(218, 326);
            this.dataGridViewFinger.TabIndex = 5;
            // 
            // Finger
            // 
            this.Finger.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Finger.HeaderText = "Finger";
            this.Finger.MinimumWidth = 100;
            this.Finger.Name = "Finger";
            this.Finger.ReadOnly = true;
            this.Finger.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Enrolled
            // 
            this.Enrolled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.NullValue = false;
            this.Enrolled.DefaultCellStyle = dataGridViewCellStyle2;
            this.Enrolled.HeaderText = "Is Enrolled";
            this.Enrolled.Name = "Enrolled";
            this.Enrolled.ReadOnly = true;
            this.Enrolled.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Enrolled.Width = 74;
            // 
            // textBoxLastName
            // 
            this.textBoxLastName.Location = new System.Drawing.Point(131, 52);
            this.textBoxLastName.Name = "textBoxLastName";
            this.textBoxLastName.Size = new System.Drawing.Size(160, 21);
            this.textBoxLastName.TabIndex = 1;
            this.textBoxLastName.TextChanged += new System.EventHandler(this.textBoxLastName_TextChanged);
            // 
            // textBoxFirstName
            // 
            this.textBoxFirstName.Location = new System.Drawing.Point(131, 24);
            this.textBoxFirstName.Name = "textBoxFirstName";
            this.textBoxFirstName.Size = new System.Drawing.Size(160, 21);
            this.textBoxFirstName.TabIndex = 0;
            this.textBoxFirstName.TextChanged += new System.EventHandler(this.textBoxFirstName_TextChanged);
            // 
            // gpBx
            // 
            this.gpBx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpBx.Controls.Add(this.cbxFinger);
            this.gpBx.Controls.Add(this.pBarEnroll);
            this.gpBx.Controls.Add(this.lblStatus);
            this.gpBx.Controls.Add(this.btEnroll);
            this.gpBx.Controls.Add(this.label8);
            this.gpBx.Controls.Add(this.cbArmReader);
            this.gpBx.Enabled = false;
            this.gpBx.Location = new System.Drawing.Point(558, 218);
            this.gpBx.Name = "gpBx";
            this.gpBx.Size = new System.Drawing.Size(430, 356);
            this.gpBx.TabIndex = 6;
            this.gpBx.TabStop = false;
            this.gpBx.Text = "Remote Enrolment";
            // 
            // cbxFinger
            // 
            this.cbxFinger.FormattingEnabled = true;
            this.cbxFinger.Location = new System.Drawing.Point(201, 77);
            this.cbxFinger.Name = "cbxFinger";
            this.cbxFinger.Size = new System.Drawing.Size(209, 21);
            this.cbxFinger.TabIndex = 5;
            // 
            // pBarEnroll
            // 
            this.pBarEnroll.Location = new System.Drawing.Point(30, 122);
            this.pBarEnroll.Maximum = 4;
            this.pBarEnroll.Name = "pBarEnroll";
            this.pBarEnroll.Size = new System.Drawing.Size(381, 23);
            this.pBarEnroll.TabIndex = 4;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(27, 162);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(56, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Status : ";
            // 
            // btEnroll
            // 
            this.btEnroll.Location = new System.Drawing.Point(30, 68);
            this.btEnroll.Name = "btEnroll";
            this.btEnroll.Size = new System.Drawing.Size(150, 42);
            this.btEnroll.TabIndex = 2;
            this.btEnroll.Text = "Enroll";
            this.btEnroll.UseVisualStyleBackColor = true;
            this.btEnroll.Click += new System.EventHandler(this.btEnroll_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(115, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Available Device : ";
            // 
            // cbArmReader
            // 
            this.cbArmReader.FormattingEnabled = true;
            this.cbArmReader.Location = new System.Drawing.Point(141, 33);
            this.cbArmReader.Name = "cbArmReader";
            this.cbArmReader.Size = new System.Drawing.Size(270, 21);
            this.cbArmReader.TabIndex = 0;
            // 
            // backgroundWorkerEnroll
            // 
            this.backgroundWorkerEnroll.WorkerReportsProgress = true;
            this.backgroundWorkerEnroll.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerEnroll_DoWork);
            this.backgroundWorkerEnroll.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerEnroll_ProgressChanged);
            this.backgroundWorkerEnroll.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerEnroll_RunWorkerCompleted);
            // 
            // UserMngtForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1003, 586);
            this.Controls.Add(this.gpBx);
            this.Controls.Add(this.groupBoxUserCtrl);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStripUserMngt);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(738, 624);
            this.Name = "UserMngtForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "User Management";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UserMngtForm_FormClosing);
            this.Load += new System.EventHandler(this.UserMngtForm_Load);
            this.toolStripUserMngt.ResumeLayout(false);
            this.toolStripUserMngt.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGrant)).EndInit();
            this.groupBoxUserCtrl.ResumeLayout(false);
            this.groupBoxUserCtrl.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFinger)).EndInit();
            this.gpBx.ResumeLayout(false);
            this.gpBx.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxUser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStrip toolStripUserMngt;
        private System.Windows.Forms.ToolStripButton toolStripButtonReset;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
        private System.Windows.Forms.ToolStripButton toolStripButtonApply;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBoxUserCtrl;
        private System.Windows.Forms.DataGridView dataGridViewFinger;
        private System.Windows.Forms.TextBox textBoxLastName;
        private System.Windows.Forms.TextBox textBoxFirstName;
        private System.Windows.Forms.ToolStripButton toolStripButtonCreateNew;
        private System.Windows.Forms.ToolStripButton toolStripButtonEnrollFingerprint;
        private System.Windows.Forms.TextBox textBoxReaderCard;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dataGridViewGrant;
        private System.Windows.Forms.TextBox textBoxRemoveValue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxRemoveItem;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxFinger;
        private System.Windows.Forms.TextBox textBoxCom;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton radioButtonHF;
        private System.Windows.Forms.RadioButton radioButtonLF;
        private System.Windows.Forms.DataGridViewComboBoxColumn DoorGranted;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonBadge;
        private System.Windows.Forms.ComboBox comboBoxRemoteReader;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Finger;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Enrolled;
        private System.Windows.Forms.GroupBox gpBx;
        private System.Windows.Forms.ProgressBar pBarEnroll;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btEnroll;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbArmReader;
        private System.Windows.Forms.ComboBox cbxFinger;
        private System.ComponentModel.BackgroundWorker backgroundWorkerEnroll;
    }
}