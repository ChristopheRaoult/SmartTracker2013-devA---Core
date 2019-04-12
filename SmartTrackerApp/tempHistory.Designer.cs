using smartTracker.Properties;

namespace smartTracker
{
    partial class TempHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TempHistory));
            this.toolStripTmpHistory = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBoxReader = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBoxTime = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonGet = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPrint = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.saveExportFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.panelChart = new System.Windows.Forms.Panel();
            this.myPrintDocument = new System.Drawing.Printing.PrintDocument();
            this.toolStripTmpHistory.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripTmpHistory
            // 
            this.toolStripTmpHistory.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripTmpHistory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripComboBoxReader,
            this.toolStripSeparator1,
            this.toolStripLabel2,
            this.toolStripLabel3,
            this.toolStripComboBoxTime,
            this.toolStripSeparator2,
            this.toolStripButtonGet,
            this.toolStripSeparator3,
            this.toolStripButtonExport,
            this.toolStripButtonPrint,
            this.toolStripSeparator4,
            this.toolStripButtonExit});
            this.toolStripTmpHistory.Location = new System.Drawing.Point(0, 0);
            this.toolStripTmpHistory.Name = "toolStripTmpHistory";
            this.toolStripTmpHistory.Size = new System.Drawing.Size(1122, 25);
            this.toolStripTmpHistory.TabIndex = 1;
            this.toolStripTmpHistory.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel1.ForeColor = System.Drawing.Color.Black;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(64, 22);
            this.toolStripLabel1.Text = ResStrings.UserStatForm_InitForm_Reader;
            // 
            // toolStripComboBoxReader
            // 
            this.toolStripComboBoxReader.DropDownHeight = 150;
            this.toolStripComboBoxReader.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripComboBoxReader.ForeColor = System.Drawing.Color.Black;
            this.toolStripComboBoxReader.IntegralHeight = false;
            this.toolStripComboBoxReader.Name = "toolStripComboBoxReader";
            this.toolStripComboBoxReader.Size = new System.Drawing.Size(200, 25);
            this.toolStripComboBoxReader.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxReader_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(69, 22);
            this.toolStripLabel2.Text = "T° from: ";
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(40, 22);
            this.toolStripLabel3.Text = "For :";
            // 
            // toolStripComboBoxTime
            // 
            this.toolStripComboBoxTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxTime.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripComboBoxTime.Items.AddRange(new object[] {
            "1 Day",
            "1 Week"});
            this.toolStripComboBoxTime.Name = "toolStripComboBoxTime";
            this.toolStripComboBoxTime.Size = new System.Drawing.Size(121, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonGet
            // 
            this.toolStripButtonGet.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonGet.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonGet.Image = global::smartTracker.Properties.Resources.refresh;
            this.toolStripButtonGet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGet.Name = "toolStripButtonGet";
            this.toolStripButtonGet.Size = new System.Drawing.Size(72, 22);
            this.toolStripButtonGet.Text = "Get T°";
            this.toolStripButtonGet.Click += new System.EventHandler(this.toolStripButtonGet_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonExport
            // 
            this.toolStripButtonExport.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonExport.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonExport.Image = global::smartTracker.Properties.Resources.open;
            this.toolStripButtonExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExport.Name = "toolStripButtonExport";
            this.toolStripButtonExport.Size = new System.Drawing.Size(76, 22);
            this.toolStripButtonExport.Text = ResStrings.str_Export;
            this.toolStripButtonExport.Click += new System.EventHandler(this.toolStripButtonExport_Click);
            // 
            // toolStripButtonPrint
            // 
            this.toolStripButtonPrint.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonPrint.Image = global::smartTracker.Properties.Resources.print;
            this.toolStripButtonPrint.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolStripButtonPrint.Name = "toolStripButtonPrint";
            this.toolStripButtonPrint.Size = new System.Drawing.Size(62, 22);
            this.toolStripButtonPrint.Text = "Print";
            this.toolStripButtonPrint.Click += new System.EventHandler(this.toolStripButtonPrint_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonExit
            // 
            this.toolStripButtonExit.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonExit.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonExit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExit.Image")));
            this.toolStripButtonExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExit.Name = "toolStripButtonExit";
            this.toolStripButtonExit.Size = new System.Drawing.Size(56, 22);
            this.toolStripButtonExit.Text = "Exit";
            this.toolStripButtonExit.Click += new System.EventHandler(this.toolStripButtonExit_Click);
            // 
            // timerRefresh
            // 
            this.timerRefresh.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker.Location = new System.Drawing.Point(922, 2);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(200, 23);
            this.dateTimePicker.TabIndex = 42;
            // 
            // saveExportFileDialog
            // 
            this.saveExportFileDialog.Filter = "Excel 97 - 2003 File (*.xls)|*.xls|Excel 2010 File (*.xlsx)|*.xlsx";
            this.saveExportFileDialog.FilterIndex = 2;
            this.saveExportFileDialog.RestoreDirectory = true;
            this.saveExportFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveExportFileDialog_FileOk);
            // 
            // panelChart
            // 
            this.panelChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelChart.Location = new System.Drawing.Point(0, 25);
            this.panelChart.Name = "panelChart";
            this.panelChart.Size = new System.Drawing.Size(1122, 427);
            this.panelChart.TabIndex = 43;
            // 
            // myPrintDocument
            // 
            this.myPrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.myPrintDocument_PrintPage);
            // 
            // tempHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1122, 452);
            this.Controls.Add(this.panelChart);
            this.Controls.Add(this.dateTimePicker);
            this.Controls.Add(this.toolStripTmpHistory);
            this.Name = "TempHistory";
            this.Text = "Fridge Temperature History";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.tempHistory_FormClosing);
            this.Load += new System.EventHandler(this.tempHistory_Load);
            this.toolStripTmpHistory.ResumeLayout(false);
            this.toolStripTmpHistory.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripTmpHistory;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxReader;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonGet;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxTime;
        //private System.Windows.Forms.DataVisualization.Charting.Chart chartTemp;
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.DateTimePicker dateTimePicker;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripButton toolStripButtonExport;
        private System.Windows.Forms.SaveFileDialog saveExportFileDialog;
        private System.Windows.Forms.Panel panelChart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrint;
        private System.Drawing.Printing.PrintDocument myPrintDocument;
    }
}