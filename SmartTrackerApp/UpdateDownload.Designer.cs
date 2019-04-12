namespace smartTracker
{
    partial class UpdateDownload
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.labelDownload = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 64);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(351, 23);
            this.progressBar.Step = 0;
            this.progressBar.TabIndex = 0;
            // 
            // labelDownload
            // 
            this.labelDownload.AutoSize = true;
            this.labelDownload.Location = new System.Drawing.Point(13, 30);
            this.labelDownload.Name = "labelDownload";
            this.labelDownload.Size = new System.Drawing.Size(61, 13);
            this.labelDownload.TabIndex = 1;
            this.labelDownload.Text = "Download :";
            // 
            // UpdateDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 128);
            this.Controls.Add(this.labelDownload);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "UpdateDownload";
            this.Text = "Update Download";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label labelDownload;
    }
}