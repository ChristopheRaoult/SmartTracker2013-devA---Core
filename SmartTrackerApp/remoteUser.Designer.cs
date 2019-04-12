namespace smartTracker
{
    partial class RemoteUser
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
            this.listBoxUser = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxUser
            // 
            this.listBoxUser.FormattingEnabled = true;
            this.listBoxUser.Location = new System.Drawing.Point(12, 12);
            this.listBoxUser.Name = "listBoxUser";
            this.listBoxUser.Size = new System.Drawing.Size(273, 160);
            this.listBoxUser.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(101, 188);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 30);
            this.button1.TabIndex = 1;
            this.button1.Text = "Remove";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // remoteUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 250);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBoxUser);
            this.Name = "RemoteUser";
            this.Text = "remote User";
            this.Load += new System.EventHandler(this.remoteUser_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxUser;
        private System.Windows.Forms.Button button1;
    }
}