namespace smartTracker
{
    partial class BoxModeCreateUpdate
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBoxTagUid = new System.Windows.Forms.TextBox();
            this.txtBoxRef = new System.Windows.Forms.TextBox();
            this.txtBoxDesc = new System.Windows.Forms.TextBox();
            this.txtCriteria = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.butScan = new System.Windows.Forms.Button();
            this.comboBoxReader = new System.Windows.Forms.ComboBox();
            this.listBoxParam = new System.Windows.Forms.ListBox();
            this.button3 = new System.Windows.Forms.Button();
            this.toolStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.listBoxOperator = new System.Windows.Forms.ListBox();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 43);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tag UID Box :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 82);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Box Reference :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 121);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "Box Description :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(58, 160);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 14);
            this.label4.TabIndex = 3;
            this.label4.Text = "Criteria : ";
            // 
            // txtBoxTagUid
            // 
            this.txtBoxTagUid.Location = new System.Drawing.Point(131, 40);
            this.txtBoxTagUid.Name = "txtBoxTagUid";
            this.txtBoxTagUid.Size = new System.Drawing.Size(253, 22);
            this.txtBoxTagUid.TabIndex = 4;
            // 
            // txtBoxRef
            // 
            this.txtBoxRef.Location = new System.Drawing.Point(131, 79);
            this.txtBoxRef.Name = "txtBoxRef";
            this.txtBoxRef.Size = new System.Drawing.Size(253, 22);
            this.txtBoxRef.TabIndex = 5;
            // 
            // txtBoxDesc
            // 
            this.txtBoxDesc.Location = new System.Drawing.Point(131, 118);
            this.txtBoxDesc.Name = "txtBoxDesc";
            this.txtBoxDesc.Size = new System.Drawing.Size(253, 22);
            this.txtBoxDesc.TabIndex = 6;
            // 
            // txtCriteria
            // 
            this.txtCriteria.Location = new System.Drawing.Point(131, 157);
            this.txtCriteria.Name = "txtCriteria";
            this.txtCriteria.Size = new System.Drawing.Size(253, 22);
            this.txtCriteria.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(211, 195);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Save !";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // butScan
            // 
            this.butScan.Enabled = false;
            this.butScan.Location = new System.Drawing.Point(412, 31);
            this.butScan.Name = "butScan";
            this.butScan.Size = new System.Drawing.Size(68, 31);
            this.butScan.TabIndex = 9;
            this.butScan.Text = "Scan";
            this.butScan.UseVisualStyleBackColor = true;
            this.butScan.Click += new System.EventHandler(this.button2_Click);
            // 
            // comboBoxReader
            // 
            this.comboBoxReader.FormattingEnabled = true;
            this.comboBoxReader.Items.AddRange(new object[] {
            "No Reader"});
            this.comboBoxReader.Location = new System.Drawing.Point(486, 36);
            this.comboBoxReader.Name = "comboBoxReader";
            this.comboBoxReader.Size = new System.Drawing.Size(147, 22);
            this.comboBoxReader.TabIndex = 10;
            this.comboBoxReader.SelectedIndexChanged += new System.EventHandler(this.comboBoxReader_SelectedIndexChanged);
            // 
            // listBoxParam
            // 
            this.listBoxParam.FormattingEnabled = true;
            this.listBoxParam.ItemHeight = 14;
            this.listBoxParam.Location = new System.Drawing.Point(412, 88);
            this.listBoxParam.Name = "listBoxParam";
            this.listBoxParam.Size = new System.Drawing.Size(133, 130);
            this.listBoxParam.TabIndex = 11;
            this.listBoxParam.Click += new System.EventHandler(this.listBoxParam_Click);
            // 
            // button3
            // 
            this.button3.BackgroundImage = global::smartTracker.Properties.Resources.refresh;
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button3.Location = new System.Drawing.Point(639, 31);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(36, 31);
            this.button3.TabIndex = 12;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelInfo});
            this.toolStrip.Location = new System.Drawing.Point(0, 222);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(687, 22);
            this.toolStrip.TabIndex = 13;
            this.toolStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabelInfo
            // 
            this.toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
            this.toolStripStatusLabelInfo.Size = new System.Drawing.Size(672, 17);
            this.toolStripStatusLabelInfo.Spring = true;
            this.toolStripStatusLabelInfo.Text = "Info :";
            this.toolStripStatusLabelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listBoxOperator
            // 
            this.listBoxOperator.FormattingEnabled = true;
            this.listBoxOperator.ItemHeight = 14;
            this.listBoxOperator.Location = new System.Drawing.Point(571, 88);
            this.listBoxOperator.Name = "listBoxOperator";
            this.listBoxOperator.Size = new System.Drawing.Size(104, 130);
            this.listBoxOperator.TabIndex = 14;
            this.listBoxOperator.Click += new System.EventHandler(this.listBoxOperator_Click);
            // 
            // BoxModeCreateUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 244);
            this.Controls.Add(this.listBoxOperator);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.listBoxParam);
            this.Controls.Add(this.comboBoxReader);
            this.Controls.Add(this.butScan);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtCriteria);
            this.Controls.Add(this.txtBoxDesc);
            this.Controls.Add(this.txtBoxRef);
            this.Controls.Add(this.txtBoxTagUid);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "BoxModeCreateUpdate";
            this.Text = "Create Update Box";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BoxModeCreateUpdate_FormClosing);
            this.Load += new System.EventHandler(this.BoxModeCreateUpdate_Load);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBoxTagUid;
        private System.Windows.Forms.TextBox txtBoxRef;
        private System.Windows.Forms.TextBox txtBoxDesc;
        private System.Windows.Forms.TextBox txtCriteria;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button butScan;
        private System.Windows.Forms.ComboBox comboBoxReader;
        private System.Windows.Forms.ListBox listBoxParam;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.StatusStrip toolStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelInfo;
        private System.Windows.Forms.ListBox listBoxOperator;
    }
}