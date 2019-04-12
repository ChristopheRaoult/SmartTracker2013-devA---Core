namespace smartTracker
{
    partial class FridgeTempFrm
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
            this.panelChart = new System.Windows.Forms.Panel();
            this.timerTemp = new System.Windows.Forms.Timer(this.components);
            this.labelCurrentTemp = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblInfo = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelChart
            // 
            this.panelChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelChart.Location = new System.Drawing.Point(12, 55);
            this.panelChart.Name = "panelChart";
            this.panelChart.Size = new System.Drawing.Size(500, 250);
            this.panelChart.TabIndex = 0;
            // 
            // timerTemp
            // 
            this.timerTemp.Enabled = true;
            this.timerTemp.Interval = 5000;
            this.timerTemp.Tick += new System.EventHandler(this.timerTemp_Tick_1);
            // 
            // labelCurrentTemp
            // 
            this.labelCurrentTemp.AutoSize = true;
            this.labelCurrentTemp.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrentTemp.ForeColor = System.Drawing.Color.Blue;
            this.labelCurrentTemp.Location = new System.Drawing.Point(12, 9);
            this.labelCurrentTemp.Name = "labelCurrentTemp";
            this.labelCurrentTemp.Size = new System.Drawing.Size(84, 25);
            this.labelCurrentTemp.TabIndex = 1;
            this.labelCurrentTemp.Text = "Info  : ";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblInfo);
            this.panel1.Location = new System.Drawing.Point(518, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(197, 254);
            this.panel1.TabIndex = 2;
            // 
            // lblInfo
            // 
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInfo.FormattingEnabled = true;
            this.lblInfo.Location = new System.Drawing.Point(0, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(197, 254);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Visible = false;
            // 
            // FridgeTempFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(724, 321);
            this.Controls.Add(this.panelChart);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelCurrentTemp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FridgeTempFrm";
            this.Text = "Fridge Temperature";
            this.Load += new System.EventHandler(this.FridgeTempFrm_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelChart;
        private System.Windows.Forms.Timer timerTemp;
        private System.Windows.Forms.Label labelCurrentTemp;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox lblInfo;
     
    }
}