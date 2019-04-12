namespace smartTracker
{
    partial class TagSearchLedfrm
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
            this.dataGridViewTagToFind = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonProcess = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTagToFind)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewTagToFind
            // 
            this.dataGridViewTagToFind.AllowUserToAddRows = false;
            this.dataGridViewTagToFind.AllowUserToDeleteRows = false;
            this.dataGridViewTagToFind.AllowUserToResizeColumns = false;
            this.dataGridViewTagToFind.AllowUserToResizeRows = false;
            this.dataGridViewTagToFind.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewTagToFind.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewTagToFind.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTagToFind.Location = new System.Drawing.Point(12, 119);
            this.dataGridViewTagToFind.Name = "dataGridViewTagToFind";
            this.dataGridViewTagToFind.RowTemplate.Height = 75;
            this.dataGridViewTagToFind.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTagToFind.Size = new System.Drawing.Size(646, 446);
            this.dataGridViewTagToFind.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(30, 32);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 41);
            this.button1.TabIndex = 3;
            this.button1.Text = "Clear List!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonProcess
            // 
            this.buttonProcess.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonProcess.Location = new System.Drawing.Point(141, 32);
            this.buttonProcess.Name = "buttonProcess";
            this.buttonProcess.Size = new System.Drawing.Size(90, 41);
            this.buttonProcess.TabIndex = 4;
            this.buttonProcess.Text = "Find Tag(s)!";
            this.buttonProcess.UseVisualStyleBackColor = true;
            this.buttonProcess.Click += new System.EventHandler(this.buttonProcess_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.Location = new System.Drawing.Point(362, 44);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(45, 16);
            this.labelStatus.TabIndex = 5;
            this.labelStatus.Text = "Info :";
            this.labelStatus.Click += new System.EventHandler(this.labelStatus_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStop.Location = new System.Drawing.Point(252, 32);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(90, 41);
            this.buttonStop.TabIndex = 6;
            this.buttonStop.Text = "Stop!";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonStop);
            this.groupBox1.Controls.Add(this.labelStatus);
            this.groupBox1.Controls.Add(this.buttonProcess);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(646, 100);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Action";
            // 
            // TagSearchLedfrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 577);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dataGridViewTagToFind);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TagSearchLedfrm";
            this.Text = "Tag Search!";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TagSearchLedfrm_FormClosing);
            this.Load += new System.EventHandler(this.TagSearchLedfrm_Load);
            this.Shown += new System.EventHandler(this.TagSearchLedfrm_Shown);
            this.VisibleChanged += new System.EventHandler(this.TagSearchLedfrm_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTagToFind)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView dataGridViewTagToFind;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonProcess;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}