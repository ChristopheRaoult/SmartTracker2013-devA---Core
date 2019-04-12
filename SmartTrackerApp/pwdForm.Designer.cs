namespace smartTracker
{
    partial class PwdForm
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
            this.textBoxPwd = new System.Windows.Forms.TextBox();
            this.buttonProcess = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxPwd
            // 
            this.textBoxPwd.Location = new System.Drawing.Point(15, 24);
            this.textBoxPwd.Name = "textBoxPwd";
            this.textBoxPwd.Size = new System.Drawing.Size(163, 21);
            this.textBoxPwd.TabIndex = 0;
            this.textBoxPwd.UseSystemPasswordChar = true;
            this.textBoxPwd.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxPwd_KeyUp);
            // 
            // buttonProcess
            // 
            this.buttonProcess.Location = new System.Drawing.Point(227, 20);
            this.buttonProcess.Name = "buttonProcess";
            this.buttonProcess.Size = new System.Drawing.Size(71, 24);
            this.buttonProcess.TabIndex = 1;
            this.buttonProcess.Text = "OK!";
            this.buttonProcess.UseVisualStyleBackColor = true;
            this.buttonProcess.Click += new System.EventHandler(this.buttonProcess_Click);
            // 
            // pwdForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 67);
            this.Controls.Add(this.buttonProcess);
            this.Controls.Add(this.textBoxPwd);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PwdForm";
            this.ShowIcon = false;
            this.Text = "Enter Administrator Password!";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxPwd;
        private System.Windows.Forms.Button buttonProcess;
    }
}