namespace smartTracker
{
    partial class TagUidWritingForm
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
            this.textBoxPreviousUID = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBoxNewUID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataListViewHistory = new BrightIdeasSoftware.DataListView();
            this.comboBoxSelWrite = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Previous TagUID:";
            // 
            // textBoxPreviousUID
            // 
            this.textBoxPreviousUID.BackColor = System.Drawing.Color.White;
            this.textBoxPreviousUID.Location = new System.Drawing.Point(178, 36);
            this.textBoxPreviousUID.MaxLength = 10;
            this.textBoxPreviousUID.Name = "textBoxPreviousUID";
            this.textBoxPreviousUID.ReadOnly = true;
            this.textBoxPreviousUID.Size = new System.Drawing.Size(188, 23);
            this.textBoxPreviousUID.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(253, 121);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(113, 52);
            this.button1.TabIndex = 2;
            this.button1.Text = "Write";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBoxNewUID
            // 
            this.textBoxNewUID.Location = new System.Drawing.Point(178, 76);
            this.textBoxNewUID.MaxLength = 17;
            this.textBoxNewUID.Name = "textBoxNewUID";
            this.textBoxNewUID.Size = new System.Drawing.Size(188, 23);
            this.textBoxNewUID.TabIndex = 1;
            this.textBoxNewUID.TextChanged += new System.EventHandler(this.textBoxNewUID_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(76, 79);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "New TagUID:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataListViewHistory);
            this.groupBox1.Location = new System.Drawing.Point(5, 191);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(432, 303);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Uid History";
            // 
            // dataListViewHistory
            // 
            this.dataListViewHistory.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dataListViewHistory.Cursor = System.Windows.Forms.Cursors.Default;
            this.dataListViewHistory.DataSource = null;
            this.dataListViewHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListViewHistory.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataListViewHistory.FullRowSelect = true;
            this.dataListViewHistory.GridLines = true;
            this.dataListViewHistory.Location = new System.Drawing.Point(3, 19);
            this.dataListViewHistory.Name = "dataListViewHistory";
            this.dataListViewHistory.ShowGroups = false;
            this.dataListViewHistory.Size = new System.Drawing.Size(426, 281);
            this.dataListViewHistory.SpaceBetweenGroups = 20;
            this.dataListViewHistory.TabIndex = 7;
            this.dataListViewHistory.UseCompatibleStateImageBehavior = false;
            this.dataListViewHistory.UseCustomSelectionColors = true;
            this.dataListViewHistory.UseExplorerTheme = true;
            this.dataListViewHistory.UseFiltering = true;
            this.dataListViewHistory.UseHotItem = true;
            this.dataListViewHistory.UseTranslucentHotItem = true;
            this.dataListViewHistory.View = System.Windows.Forms.View.Details;
            // 
            // comboBoxSelWrite
            // 
            this.comboBoxSelWrite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelWrite.FormattingEnabled = true;
            this.comboBoxSelWrite.Items.AddRange(new object[] {
            "Write SPCE2 With Family",
            "Write Decimal"});
            this.comboBoxSelWrite.Location = new System.Drawing.Point(26, 136);
            this.comboBoxSelWrite.Name = "comboBoxSelWrite";
            this.comboBoxSelWrite.Size = new System.Drawing.Size(200, 24);
            this.comboBoxSelWrite.TabIndex = 5;
            // 
            // TagUidWritingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(442, 499);
            this.Controls.Add(this.comboBoxSelWrite);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxNewUID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxPreviousUID);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TagUidWritingForm";
            this.Text = "TagUID Writing Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TagUIDWritingForm_FormClosing);
            this.Load += new System.EventHandler(this.TagUIDWritingForm_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataListViewHistory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPreviousUID;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBoxNewUID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private BrightIdeasSoftware.DataListView dataListViewHistory;
        private System.Windows.Forms.ComboBox comboBoxSelWrite;
    }
}