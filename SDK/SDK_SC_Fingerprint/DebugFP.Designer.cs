namespace SDK_SC_Fingerprint
{
    partial class DebugFP
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pictureBoxFP = new System.Windows.Forms.PictureBox();
            this.labelTouch = new System.Windows.Forms.Label();
            this.labelGone = new System.Windows.Forms.Label();
            this.labelElapsed = new System.Windows.Forms.Label();
            this.labelFirsname = new System.Windows.Forms.Label();
            this.labelLastName = new System.Windows.Forms.Label();
            this.labelFinger = new System.Windows.Forms.Label();
            this.labelFAR = new System.Windows.Forms.Label();
            this.labelQuality = new System.Windows.Forms.Label();
            this.labelReader = new System.Windows.Forms.Label();
            this.labelCapture = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFP)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pictureBoxFP
            // 
            this.pictureBoxFP.Location = new System.Drawing.Point(10, 12);
            this.pictureBoxFP.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBoxFP.Name = "pictureBoxFP";
            this.pictureBoxFP.Size = new System.Drawing.Size(125, 150);
            this.pictureBoxFP.TabIndex = 0;
            this.pictureBoxFP.TabStop = false;
            // 
            // labelTouch
            // 
            this.labelTouch.AutoSize = true;
            this.labelTouch.Location = new System.Drawing.Point(139, 22);
            this.labelTouch.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTouch.Name = "labelTouch";
            this.labelTouch.Size = new System.Drawing.Size(30, 16);
            this.labelTouch.TabIndex = 1;
            this.labelTouch.Text = "label";
            // 
            // labelGone
            // 
            this.labelGone.AutoSize = true;
            this.labelGone.Location = new System.Drawing.Point(139, 38);
            this.labelGone.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelGone.Name = "labelGone";
            this.labelGone.Size = new System.Drawing.Size(36, 16);
            this.labelGone.TabIndex = 2;
            this.labelGone.Text = "label2";
            // 
            // labelElapsed
            // 
            this.labelElapsed.AutoSize = true;
            this.labelElapsed.Location = new System.Drawing.Point(139, 54);
            this.labelElapsed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelElapsed.Name = "labelElapsed";
            this.labelElapsed.Size = new System.Drawing.Size(36, 16);
            this.labelElapsed.TabIndex = 3;
            this.labelElapsed.Text = "label3";
            // 
            // labelFirsname
            // 
            this.labelFirsname.AutoSize = true;
            this.labelFirsname.Location = new System.Drawing.Point(139, 70);
            this.labelFirsname.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelFirsname.Name = "labelFirsname";
            this.labelFirsname.Size = new System.Drawing.Size(36, 16);
            this.labelFirsname.TabIndex = 4;
            this.labelFirsname.Text = "label3";
            // 
            // labelLastName
            // 
            this.labelLastName.AutoSize = true;
            this.labelLastName.Location = new System.Drawing.Point(139, 86);
            this.labelLastName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelLastName.Name = "labelLastName";
            this.labelLastName.Size = new System.Drawing.Size(36, 16);
            this.labelLastName.TabIndex = 5;
            this.labelLastName.Text = "label3";
            // 
            // labelFinger
            // 
            this.labelFinger.AutoSize = true;
            this.labelFinger.Location = new System.Drawing.Point(139, 102);
            this.labelFinger.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelFinger.Name = "labelFinger";
            this.labelFinger.Size = new System.Drawing.Size(36, 16);
            this.labelFinger.TabIndex = 6;
            this.labelFinger.Text = "label3";
            // 
            // labelFAR
            // 
            this.labelFAR.AutoSize = true;
            this.labelFAR.Location = new System.Drawing.Point(139, 118);
            this.labelFAR.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelFAR.Name = "labelFAR";
            this.labelFAR.Size = new System.Drawing.Size(36, 16);
            this.labelFAR.TabIndex = 7;
            this.labelFAR.Text = "label3";
            // 
            // labelQuality
            // 
            this.labelQuality.AutoSize = true;
            this.labelQuality.Location = new System.Drawing.Point(139, 134);
            this.labelQuality.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelQuality.Name = "labelQuality";
            this.labelQuality.Size = new System.Drawing.Size(36, 16);
            this.labelQuality.TabIndex = 8;
            this.labelQuality.Text = "label3";
            // 
            // labelReader
            // 
            this.labelReader.AutoSize = true;
            this.labelReader.Location = new System.Drawing.Point(139, 6);
            this.labelReader.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelReader.Name = "labelReader";
            this.labelReader.Size = new System.Drawing.Size(30, 16);
            this.labelReader.TabIndex = 9;
            this.labelReader.Text = "label";
            // 
            // labelCapture
            // 
            this.labelCapture.AutoSize = true;
            this.labelCapture.Location = new System.Drawing.Point(139, 150);
            this.labelCapture.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCapture.Name = "labelCapture";
            this.labelCapture.Size = new System.Drawing.Size(36, 16);
            this.labelCapture.TabIndex = 10;
            this.labelCapture.Text = "label3";
            // 
            // DebugFP
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(319, 175);
            this.ControlBox = false;
            this.Controls.Add(this.labelCapture);
            this.Controls.Add(this.labelReader);
            this.Controls.Add(this.labelQuality);
            this.Controls.Add(this.labelFAR);
            this.Controls.Add(this.labelFinger);
            this.Controls.Add(this.labelLastName);
            this.Controls.Add(this.labelFirsname);
            this.Controls.Add(this.labelElapsed);
            this.Controls.Add(this.labelGone);
            this.Controls.Add(this.labelTouch);
            this.Controls.Add(this.pictureBoxFP);
            this.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.DarkBlue;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DebugFP";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFP)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox pictureBoxFP;
        private System.Windows.Forms.Label labelTouch;
        private System.Windows.Forms.Label labelGone;
        private System.Windows.Forms.Label labelElapsed;
        private System.Windows.Forms.Label labelFirsname;
        private System.Windows.Forms.Label labelLastName;
        private System.Windows.Forms.Label labelFinger;
        private System.Windows.Forms.Label labelFAR;
        private System.Windows.Forms.Label labelQuality;
        private System.Windows.Forms.Label labelReader;
        private System.Windows.Forms.Label labelCapture;
    }
}