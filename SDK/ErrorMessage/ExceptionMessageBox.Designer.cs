namespace ErrorMessage
{
    partial class ExceptionMessageBox
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonDetails = new System.Windows.Forms.Button();
            this.labelText = new System.Windows.Forms.Label();
            this.textBoxDetails = new System.Windows.Forms.TextBox();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.buttonMail = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonOK.Location = new System.Drawing.Point(332, 82);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonDetails
            // 
            this.buttonDetails.Location = new System.Drawing.Point(170, 82);
            this.buttonDetails.Name = "buttonDetails";
            this.buttonDetails.Size = new System.Drawing.Size(75, 23);
            this.buttonDetails.TabIndex = 1;
            this.buttonDetails.Text = "Détails...";
            this.buttonDetails.UseVisualStyleBackColor = true;
            this.buttonDetails.Click += new System.EventHandler(this.buttonDetails_Click);
            // 
            // labelText
            // 
            this.labelText.Location = new System.Drawing.Point(132, 12);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(278, 67);
            this.labelText.TabIndex = 2;
            // 
            // textBoxDetails
            // 
            this.textBoxDetails.Location = new System.Drawing.Point(12, 111);
            this.textBoxDetails.Multiline = true;
            this.textBoxDetails.Name = "textBoxDetails";
            this.textBoxDetails.ReadOnly = true;
            this.textBoxDetails.Size = new System.Drawing.Size(398, 168);
            this.textBoxDetails.TabIndex = 3;
            // 
            // pictureBox
            // 
            this.pictureBox.Image = global::ErrorMessage.Properties.Resources.error;
            this.pictureBox.Location = new System.Drawing.Point(12, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(77, 67);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 4;
            this.pictureBox.TabStop = false;
            // 
            // buttonMail
            // 
            this.buttonMail.Location = new System.Drawing.Point(251, 82);
            this.buttonMail.Name = "buttonMail";
            this.buttonMail.Size = new System.Drawing.Size(75, 23);
            this.buttonMail.TabIndex = 5;
            this.buttonMail.Text = "Send !";
            this.buttonMail.UseVisualStyleBackColor = true;
            this.buttonMail.Click += new System.EventHandler(this.buttonMail_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ExceptionMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 291);
            this.Controls.Add(this.buttonMail);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.textBoxDetails);
            this.Controls.Add(this.labelText);
            this.Controls.Add(this.buttonDetails);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExceptionMessageBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Erreur";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonDetails;
        private System.Windows.Forms.Label labelText;
        private System.Windows.Forms.TextBox textBoxDetails;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button buttonMail;
        private System.Windows.Forms.Timer timer1;
    }
}