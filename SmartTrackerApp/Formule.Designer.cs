namespace smartTracker
{
    partial class Formule
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
            this.checkBoxEnable = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxFormuleTitle = new System.Windows.Forms.TextBox();
            this.textBoxFormule = new System.Windows.Forms.TextBox();
            this.listBoxParam = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.toolStripStat = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonQuit = new System.Windows.Forms.ToolStripButton();
            this.toolStripStat.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxEnable
            // 
            this.checkBoxEnable.AutoSize = true;
            this.checkBoxEnable.Location = new System.Drawing.Point(147, 45);
            this.checkBoxEnable.Name = "checkBoxEnable";
            this.checkBoxEnable.Size = new System.Drawing.Size(126, 20);
            this.checkBoxEnable.TabIndex = 0;
            this.checkBoxEnable.Text = "Enable Formula";
            this.checkBoxEnable.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "title displayed : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Formule : ";
            // 
            // textBoxFormuleTitle
            // 
            this.textBoxFormuleTitle.Location = new System.Drawing.Point(147, 82);
            this.textBoxFormuleTitle.Name = "textBoxFormuleTitle";
            this.textBoxFormuleTitle.Size = new System.Drawing.Size(195, 23);
            this.textBoxFormuleTitle.TabIndex = 3;
            // 
            // textBoxFormule
            // 
            this.textBoxFormule.Location = new System.Drawing.Point(147, 133);
            this.textBoxFormule.Name = "textBoxFormule";
            this.textBoxFormule.Size = new System.Drawing.Size(195, 23);
            this.textBoxFormule.TabIndex = 4;
            this.textBoxFormule.TextChanged += new System.EventHandler(this.textBoxFormule_TextChanged);
            // 
            // listBoxParam
            // 
            this.listBoxParam.FormattingEnabled = true;
            this.listBoxParam.ItemHeight = 16;
            this.listBoxParam.Location = new System.Drawing.Point(392, 68);
            this.listBoxParam.Name = "listBoxParam";
            this.listBoxParam.Size = new System.Drawing.Size(206, 100);
            this.listBoxParam.TabIndex = 5;
            this.listBoxParam.Click += new System.EventHandler(this.listBoxParam_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(389, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(145, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Available Parameters";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(258, 165);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Save!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // toolStripStat
            // 
            this.toolStripStat.AutoSize = false;
            this.toolStripStat.BackColor = System.Drawing.Color.White;
            this.toolStripStat.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStat.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripStat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonQuit});
            this.toolStripStat.Location = new System.Drawing.Point(0, 0);
            this.toolStripStat.Name = "toolStripStat";
            this.toolStripStat.Size = new System.Drawing.Size(629, 35);
            this.toolStripStat.TabIndex = 8;
            this.toolStripStat.Text = "toolStrip1";
            // 
            // toolStripButtonQuit
            // 
            this.toolStripButtonQuit.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonQuit.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonQuit.Image = global::smartTracker.Properties.Resources.exit;
            this.toolStripButtonQuit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonQuit.Name = "toolStripButtonQuit";
            this.toolStripButtonQuit.Size = new System.Drawing.Size(66, 32);
            this.toolStripButtonQuit.Text = "Quit";
            this.toolStripButtonQuit.Click += new System.EventHandler(this.toolStripButtonQuit_Click);
            // 
            // Formule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(629, 200);
            this.Controls.Add(this.toolStripStat);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listBoxParam);
            this.Controls.Add(this.textBoxFormule);
            this.Controls.Add(this.textBoxFormuleTitle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxEnable);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Formule";
            this.Text = "Formule Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Formule_FormClosing);
            this.Load += new System.EventHandler(this.Formule_Load);
            this.toolStripStat.ResumeLayout(false);
            this.toolStripStat.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxEnable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxFormuleTitle;
        private System.Windows.Forms.TextBox textBoxFormule;
        private System.Windows.Forms.ListBox listBoxParam;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStrip toolStripStat;
        private System.Windows.Forms.ToolStripButton toolStripButtonQuit;
    }
}