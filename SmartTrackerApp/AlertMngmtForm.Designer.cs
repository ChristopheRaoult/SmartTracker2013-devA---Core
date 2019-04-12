namespace smartTracker
{
    partial class AlertMngmtForm
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
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxSMTPActive = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBoxSSL = new System.Windows.Forms.CheckBox();
            this.textBoxMailTO = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonTest = new System.Windows.Forms.Button();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxPwd = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxLogin = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSender = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSmtpServer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkedListBox = new System.Windows.Forms.CheckedListBox();
            this.textBoxAlert = new System.Windows.Forms.TextBox();
            this.labelAlert = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.buttonSaveAlert = new System.Windows.Forms.Button();
            this.buttonTestAlert = new System.Windows.Forms.Button();
            this.checkBoxAlertActive = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.textBoxMailSubject = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxBCCList = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxCCList = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxMailToList = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxAlertType = new System.Windows.Forms.ComboBox();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(901, 35);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonExit
            // 
            this.toolStripButtonExit.Image = global::smartTracker.Properties.Resources.exit;
            this.toolStripButtonExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExit.Name = "toolStripButtonExit";
            this.toolStripButtonExit.Size = new System.Drawing.Size(53, 32);
            this.toolStripButtonExit.Text = "Exit";
            this.toolStripButtonExit.Click += new System.EventHandler(this.toolStripButtonExit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkBoxSMTPActive);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.checkBoxSSL);
            this.groupBox1.Controls.Add(this.textBoxMailTO);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.buttonTest);
            this.groupBox1.Controls.Add(this.textBoxPort);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBoxPwd);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBoxLogin);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxSender);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxSmtpServer);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(16, 34);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(869, 209);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SMTP";
            // 
            // checkBoxSMTPActive
            // 
            this.checkBoxSMTPActive.AutoSize = true;
            this.checkBoxSMTPActive.Location = new System.Drawing.Point(132, 128);
            this.checkBoxSMTPActive.Name = "checkBoxSMTPActive";
            this.checkBoxSMTPActive.Size = new System.Drawing.Size(337, 20);
            this.checkBoxSMTPActive.TabIndex = 17;
            this.checkBoxSMTPActive.Text = "Enable SMTP (If Disable , no Alert will be sent)";
            this.checkBoxSMTPActive.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(665, 128);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(90, 36);
            this.button3.TabIndex = 16;
            this.button3.Text = "Save";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(665, 74);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 36);
            this.button2.TabIndex = 15;
            this.button2.Text = "Delete";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(665, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 36);
            this.button1.TabIndex = 14;
            this.button1.Text = "Reset";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBoxSSL
            // 
            this.checkBoxSSL.AutoSize = true;
            this.checkBoxSSL.Location = new System.Drawing.Point(445, 58);
            this.checkBoxSSL.Name = "checkBoxSSL";
            this.checkBoxSSL.Size = new System.Drawing.Size(81, 20);
            this.checkBoxSSL.TabIndex = 13;
            this.checkBoxSSL.Text = "Use SSL";
            this.checkBoxSSL.UseVisualStyleBackColor = true;
            // 
            // textBoxMailTO
            // 
            this.textBoxMailTO.Location = new System.Drawing.Point(300, 167);
            this.textBoxMailTO.Name = "textBoxMailTO";
            this.textBoxMailTO.Size = new System.Drawing.Size(318, 23);
            this.textBoxMailTO.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(176, 170);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 16);
            this.label6.TabIndex = 11;
            this.label6.Text = "MailTo for test :";
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(64, 160);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(90, 36);
            this.buttonTest.TabIndex = 6;
            this.buttonTest.Text = "Test !";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(528, 27);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(90, 23);
            this.textBoxPort.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(437, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Smtp Port :";
            // 
            // textBoxPwd
            // 
            this.textBoxPwd.Location = new System.Drawing.Point(494, 85);
            this.textBoxPwd.Name = "textBoxPwd";
            this.textBoxPwd.Size = new System.Drawing.Size(124, 23);
            this.textBoxPwd.TabIndex = 4;
            this.textBoxPwd.UseSystemPasswordChar = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(442, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Pwd :";
            // 
            // textBoxLogin
            // 
            this.textBoxLogin.Location = new System.Drawing.Point(132, 85);
            this.textBoxLogin.Name = "textBoxLogin";
            this.textBoxLogin.Size = new System.Drawing.Size(286, 23);
            this.textBoxLogin.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(73, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Login :";
            // 
            // textBoxSender
            // 
            this.textBoxSender.Location = new System.Drawing.Point(132, 56);
            this.textBoxSender.Name = "textBoxSender";
            this.textBoxSender.Size = new System.Drawing.Size(286, 23);
            this.textBoxSender.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Sender :";
            // 
            // textBoxSmtpServer
            // 
            this.textBoxSmtpServer.Location = new System.Drawing.Point(132, 27);
            this.textBoxSmtpServer.Name = "textBoxSmtpServer";
            this.textBoxSmtpServer.Size = new System.Drawing.Size(286, 23);
            this.textBoxSmtpServer.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Smtp Server :";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.checkedListBox);
            this.groupBox2.Controls.Add(this.textBoxAlert);
            this.groupBox2.Controls.Add(this.labelAlert);
            this.groupBox2.Controls.Add(this.button7);
            this.groupBox2.Controls.Add(this.button6);
            this.groupBox2.Controls.Add(this.button5);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.buttonSaveAlert);
            this.groupBox2.Controls.Add(this.buttonTestAlert);
            this.groupBox2.Controls.Add(this.checkBoxAlertActive);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.richTextBox);
            this.groupBox2.Controls.Add(this.textBoxMailSubject);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.textBoxBCCList);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.textBoxCCList);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.textBoxMailToList);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.comboBoxAlertType);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(16, 257);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(868, 329);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ALERTS";
            // 
            // checkedListBox
            // 
            this.checkedListBox.FormattingEnabled = true;
            this.checkedListBox.HorizontalScrollbar = true;
            this.checkedListBox.Location = new System.Drawing.Point(634, 71);
            this.checkedListBox.Name = "checkedListBox";
            this.checkedListBox.Size = new System.Drawing.Size(225, 94);
            this.checkedListBox.TabIndex = 25;
            this.checkedListBox.Visible = false;
            // 
            // textBoxAlert
            // 
            this.textBoxAlert.Location = new System.Drawing.Point(811, 42);
            this.textBoxAlert.Name = "textBoxAlert";
            this.textBoxAlert.Size = new System.Drawing.Size(48, 23);
            this.textBoxAlert.TabIndex = 23;
            this.textBoxAlert.Visible = false;
            // 
            // labelAlert
            // 
            this.labelAlert.Location = new System.Drawing.Point(643, 44);
            this.labelAlert.Name = "labelAlert";
            this.labelAlert.Size = new System.Drawing.Size(155, 18);
            this.labelAlert.TabIndex = 22;
            this.labelAlert.Text = "Alert label :";
            this.labelAlert.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelAlert.Visible = false;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(649, 298);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(186, 23);
            this.button7.TabIndex = 21;
            this.button7.Text = "[date]";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(649, 269);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(186, 23);
            this.button6.TabIndex = 20;
            this.button6.Text = "[user name]";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(649, 240);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(186, 23);
            this.button5.TabIndex = 19;
            this.button5.Text = "[reader serial]";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(649, 211);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(186, 23);
            this.button4.TabIndex = 18;
            this.button4.Text = "[reader name]";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // buttonSaveAlert
            // 
            this.buttonSaveAlert.Location = new System.Drawing.Point(745, 169);
            this.buttonSaveAlert.Name = "buttonSaveAlert";
            this.buttonSaveAlert.Size = new System.Drawing.Size(90, 36);
            this.buttonSaveAlert.TabIndex = 17;
            this.buttonSaveAlert.Text = "Save";
            this.buttonSaveAlert.UseVisualStyleBackColor = true;
            this.buttonSaveAlert.Click += new System.EventHandler(this.buttonSaveAlert_Click);
            // 
            // buttonTestAlert
            // 
            this.buttonTestAlert.Location = new System.Drawing.Point(649, 169);
            this.buttonTestAlert.Name = "buttonTestAlert";
            this.buttonTestAlert.Size = new System.Drawing.Size(90, 36);
            this.buttonTestAlert.TabIndex = 16;
            this.buttonTestAlert.Text = "Test ";
            this.buttonTestAlert.UseVisualStyleBackColor = true;
            this.buttonTestAlert.Click += new System.EventHandler(this.buttonTestAlert_Click);
            // 
            // checkBoxAlertActive
            // 
            this.checkBoxAlertActive.AutoSize = true;
            this.checkBoxAlertActive.Location = new System.Drawing.Point(408, 37);
            this.checkBoxAlertActive.Name = "checkBoxAlertActive";
            this.checkBoxAlertActive.Size = new System.Drawing.Size(74, 20);
            this.checkBoxAlertActive.TabIndex = 15;
            this.checkBoxAlertActive.Text = "Active ";
            this.checkBoxAlertActive.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(51, 201);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 16);
            this.label12.TabIndex = 14;
            this.label12.Text = "Body  :";
            // 
            // richTextBox
            // 
            this.richTextBox.Location = new System.Drawing.Point(117, 189);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(501, 134);
            this.richTextBox.TabIndex = 13;
            this.richTextBox.Text = "";
            this.richTextBox.Enter += new System.EventHandler(this.textBoxMailSubject_Enter);
            // 
            // textBoxMailSubject
            // 
            this.textBoxMailSubject.Location = new System.Drawing.Point(117, 159);
            this.textBoxMailSubject.Name = "textBoxMailSubject";
            this.textBoxMailSubject.Size = new System.Drawing.Size(501, 23);
            this.textBoxMailSubject.TabIndex = 11;
            this.textBoxMailSubject.Enter += new System.EventHandler(this.textBoxMailSubject_Enter);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 162);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(100, 16);
            this.label11.TabIndex = 12;
            this.label11.Text = "Mail Subject :";
            // 
            // textBoxBCCList
            // 
            this.textBoxBCCList.Location = new System.Drawing.Point(117, 130);
            this.textBoxBCCList.Name = "textBoxBCCList";
            this.textBoxBCCList.Size = new System.Drawing.Size(501, 23);
            this.textBoxBCCList.TabIndex = 9;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(64, 133);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 16);
            this.label10.TabIndex = 10;
            this.label10.Text = "Bcc :";
            // 
            // textBoxCCList
            // 
            this.textBoxCCList.Location = new System.Drawing.Point(117, 100);
            this.textBoxCCList.Name = "textBoxCCList";
            this.textBoxCCList.Size = new System.Drawing.Size(501, 23);
            this.textBoxCCList.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(71, 103);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 16);
            this.label9.TabIndex = 8;
            this.label9.Text = "Cc :";
            // 
            // textBoxMailToList
            // 
            this.textBoxMailToList.Location = new System.Drawing.Point(117, 71);
            this.textBoxMailToList.Name = "textBoxMailToList";
            this.textBoxMailToList.Size = new System.Drawing.Size(501, 23);
            this.textBoxMailToList.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(71, 74);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 16);
            this.label8.TabIndex = 6;
            this.label8.Text = "To :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(23, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 16);
            this.label7.TabIndex = 1;
            this.label7.Text = "Alert Type :";
            // 
            // comboBoxAlertType
            // 
            this.comboBoxAlertType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAlertType.FormattingEnabled = true;
            this.comboBoxAlertType.Items.AddRange(new object[] {
            "Power Cut",
            "Usb Unplug ",
            "Door Open Too Long ",
            "Finger Alert ",
            "Remove Too Many Items ",
            "Limit Value Exceed ",
            "Move Sensor",
            "Max Fridge Temp",
            "Remove Tag Time",
            "Date Expired",
            "Stock Limit",
            "Bad Blood Patient"});
            this.comboBoxAlertType.Location = new System.Drawing.Point(117, 34);
            this.comboBoxAlertType.Name = "comboBoxAlertType";
            this.comboBoxAlertType.Size = new System.Drawing.Size(285, 24);
            this.comboBoxAlertType.TabIndex = 0;
            this.comboBoxAlertType.SelectedIndexChanged += new System.EventHandler(this.comboBoxAlertType_SelectedIndexChanged);
            // 
            // AlertMngmtForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 596);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AlertMngmtForm";
            this.Text = "AlertMngmtForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AlertMngmtForm_FormClosing);
            this.Load += new System.EventHandler(this.AlertMngmtForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxPwd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxLogin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSender;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSmtpServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMailTO;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBoxSSL;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxAlertType;
        private System.Windows.Forms.CheckBox checkBoxAlertActive;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.TextBox textBoxMailSubject;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxBCCList;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxCCList;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxMailToList;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonSaveAlert;
        private System.Windows.Forms.Button buttonTestAlert;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.CheckBox checkBoxSMTPActive;
        private System.Windows.Forms.TextBox textBoxAlert;
        private System.Windows.Forms.Label labelAlert;
        private System.Windows.Forms.CheckedListBox checkedListBox;
    }
}