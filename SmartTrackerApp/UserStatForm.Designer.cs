using smartTracker.Properties;

namespace smartTracker
{
    partial class UserStatForm
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
            //System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.toolStripStat = new System.Windows.Forms.ToolStrip();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.readerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonReset = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonQuit = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.comboBoxIntervalType = new System.Windows.Forms.ComboBox();
            this.ckx_versusTime = new System.Windows.Forms.CheckBox();
            this.buttonGet = new System.Windows.Forms.Button();
            this.groupBoxToDate = new System.Windows.Forms.GroupBox();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.groupBoxFromDate = new System.Windows.Forms.GroupBox();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.groupBoxPrevious = new System.Windows.Forms.GroupBox();
            this.comboBoxDatePrevious = new System.Windows.Forms.ComboBox();
            this.numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.comboBoxDateChoice = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.labelType = new System.Windows.Forms.Label();
            //this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.toolStripStat.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBoxToDate.SuspendLayout();
            this.groupBoxFromDate.SuspendLayout();
            this.groupBoxPrevious.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
            //((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripStat
            // 
            this.toolStripStat.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton1,
            this.toolStripButtonReset,
            this.toolStripButtonExport,
            this.toolStripButtonQuit});
            this.toolStripStat.Location = new System.Drawing.Point(0, 0);
            this.toolStripStat.Name = "toolStripStat";
            this.toolStripStat.Size = new System.Drawing.Size(865, 25);
            this.toolStripStat.TabIndex = 0;
            this.toolStripStat.Text = "toolStrip1";
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readerToolStripMenuItem,
            this.userToolStripMenuItem});
            this.toolStripSplitButton1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripSplitButton1.ForeColor = System.Drawing.Color.Black;
            this.toolStripSplitButton1.Image = global::smartTracker.Properties.Resources._23135_bubka_Xcode;
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(134, 22);
            this.toolStripSplitButton1.Text = "Operation on";
            // 
            // readerToolStripMenuItem
            // 
            this.readerToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.readerToolStripMenuItem.Image = global::smartTracker.Properties.Resources._23154_bubka_HardDrive;
            this.readerToolStripMenuItem.Name = "readerToolStripMenuItem";
            this.readerToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.readerToolStripMenuItem.Text = "Reader";
            this.readerToolStripMenuItem.Click += new System.EventHandler(this.readerToolStripMenuItem_Click);
            // 
            // userToolStripMenuItem
            // 
            this.userToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.userToolStripMenuItem.Image = global::smartTracker.Properties.Resources._23148_bubka_Finder;
            this.userToolStripMenuItem.Name = "userToolStripMenuItem";
            this.userToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.userToolStripMenuItem.Text = "User";
            this.userToolStripMenuItem.Click += new System.EventHandler(this.userToolStripMenuItem_Click);
            // 
            // toolStripButtonReset
            // 
            this.toolStripButtonReset.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonReset.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonReset.Image = global::smartTracker.Properties.Resources.edit_clear;
            this.toolStripButtonReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReset.Name = "toolStripButtonReset";
            this.toolStripButtonReset.Size = new System.Drawing.Size(69, 22);
            this.toolStripButtonReset.Text = "Reset";
            this.toolStripButtonReset.Click += new System.EventHandler(this.toolStripButtonReset_Click);
            // 
            // toolStripButtonExport
            // 
            this.toolStripButtonExport.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonExport.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonExport.Image = global::smartTracker.Properties.Resources._23177_bubka_TimeMachine;
            this.toolStripButtonExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExport.Name = "toolStripButtonExport";
            this.toolStripButtonExport.Size = new System.Drawing.Size(76, 22);
            this.toolStripButtonExport.Text = ResStrings.str_Export;
            this.toolStripButtonExport.Click += new System.EventHandler(this.toolStripButtonExport_Click);
            // 
            // toolStripButtonQuit
            // 
            this.toolStripButtonQuit.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonQuit.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonQuit.Image = global::smartTracker.Properties.Resources.exit;
            this.toolStripButtonQuit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonQuit.Name = "toolStripButtonQuit";
            this.toolStripButtonQuit.Size = new System.Drawing.Size(58, 22);
            this.toolStripButtonQuit.Text = "Quit";
            this.toolStripButtonQuit.Click += new System.EventHandler(this.toolStripButtonQuit_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxIntervalType);
            this.splitContainer1.Panel1.Controls.Add(this.ckx_versusTime);
            this.splitContainer1.Panel1.Controls.Add(this.buttonGet);
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxToDate);
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxFromDate);
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxPrevious);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxDateChoice);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxType);
            this.splitContainer1.Panel1.Controls.Add(this.labelType);
            // 
            // splitContainer1.Panel2
            // 
           // this.splitContainer1.Panel2.Controls.Add(this.chart);
            this.splitContainer1.Size = new System.Drawing.Size(865, 720);
            this.splitContainer1.SplitterDistance = 150;
            this.splitContainer1.TabIndex = 1;
            // 
            // comboBoxIntervalType
            // 
            this.comboBoxIntervalType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIntervalType.ForeColor = System.Drawing.Color.Blue;
            this.comboBoxIntervalType.FormattingEnabled = true;
            this.comboBoxIntervalType.Items.AddRange(new object[] {
            "Hour(s)",
            "Day(s)",
            "Week(s)",
            "Month(s)",
            "Year(s)"});
            this.comboBoxIntervalType.Location = new System.Drawing.Point(569, 24);
            this.comboBoxIntervalType.Name = "comboBoxIntervalType";
            this.comboBoxIntervalType.Size = new System.Drawing.Size(99, 24);
            this.comboBoxIntervalType.TabIndex = 11;
            this.comboBoxIntervalType.SelectedIndexChanged += new System.EventHandler(this.comboBoxIntervalType_SelectedIndexChanged);
            // 
            // ckx_versusTime
            // 
            this.ckx_versusTime.AutoSize = true;
            this.ckx_versusTime.ForeColor = System.Drawing.Color.Black;
            this.ckx_versusTime.Location = new System.Drawing.Point(457, 24);
            this.ckx_versusTime.Name = "ckx_versusTime";
            this.ckx_versusTime.Size = new System.Drawing.Size(106, 20);
            this.ckx_versusTime.TabIndex = 10;
            this.ckx_versusTime.Text = "versus Time";
            this.ckx_versusTime.UseVisualStyleBackColor = true;
            this.ckx_versusTime.CheckedChanged += new System.EventHandler(this.ckx_versusTime_CheckedChanged);
            // 
            // buttonGet
            // 
            this.buttonGet.BackgroundImage = global::smartTracker.Properties.Resources.button_process;
            this.buttonGet.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonGet.Location = new System.Drawing.Point(40, 7);
            this.buttonGet.Name = "buttonGet";
            this.buttonGet.Size = new System.Drawing.Size(68, 74);
            this.buttonGet.TabIndex = 9;
            this.buttonGet.UseVisualStyleBackColor = true;
            this.buttonGet.Click += new System.EventHandler(this.buttonGet_Click);
            // 
            // groupBoxToDate
            // 
            this.groupBoxToDate.Controls.Add(this.dateTimePickerTo);
            this.groupBoxToDate.Location = new System.Drawing.Point(546, 87);
            this.groupBoxToDate.Name = "groupBoxToDate";
            this.groupBoxToDate.Size = new System.Drawing.Size(244, 53);
            this.groupBoxToDate.TabIndex = 8;
            this.groupBoxToDate.TabStop = false;
            this.groupBoxToDate.Text = "To Date";
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.CalendarForeColor = System.Drawing.Color.Blue;
            this.dateTimePickerTo.CalendarTitleForeColor = System.Drawing.Color.Blue;
            this.dateTimePickerTo.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dateTimePickerTo.Location = new System.Drawing.Point(13, 22);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.Size = new System.Drawing.Size(225, 23);
            this.dateTimePickerTo.TabIndex = 0;
            this.dateTimePickerTo.ValueChanged += new System.EventHandler(this.dateTimePickerTo_ValueChanged);
            // 
            // groupBoxFromDate
            // 
            this.groupBoxFromDate.Controls.Add(this.dateTimePickerFrom);
            this.groupBoxFromDate.Location = new System.Drawing.Point(279, 88);
            this.groupBoxFromDate.Name = "groupBoxFromDate";
            this.groupBoxFromDate.Size = new System.Drawing.Size(244, 53);
            this.groupBoxFromDate.TabIndex = 7;
            this.groupBoxFromDate.TabStop = false;
            this.groupBoxFromDate.Text = "From Date";
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.CalendarForeColor = System.Drawing.Color.Blue;
            this.dateTimePickerFrom.CalendarTitleForeColor = System.Drawing.Color.Blue;
            this.dateTimePickerFrom.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dateTimePickerFrom.Location = new System.Drawing.Point(13, 22);
            this.dateTimePickerFrom.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            this.dateTimePickerFrom.MinDate = new System.DateTime(2010, 1, 1, 0, 0, 0, 0);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(225, 23);
            this.dateTimePickerFrom.TabIndex = 0;
            this.dateTimePickerFrom.ValueChanged += new System.EventHandler(this.dateTimePickerFrom_ValueChanged);
            // 
            // groupBoxPrevious
            // 
            this.groupBoxPrevious.Controls.Add(this.comboBoxDatePrevious);
            this.groupBoxPrevious.Controls.Add(this.numericUpDown);
            this.groupBoxPrevious.Location = new System.Drawing.Point(40, 88);
            this.groupBoxPrevious.Name = "groupBoxPrevious";
            this.groupBoxPrevious.Size = new System.Drawing.Size(213, 53);
            this.groupBoxPrevious.TabIndex = 6;
            this.groupBoxPrevious.TabStop = false;
            this.groupBoxPrevious.Text = "Previous";
            // 
            // comboBoxDatePrevious
            // 
            this.comboBoxDatePrevious.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDatePrevious.ForeColor = System.Drawing.Color.Blue;
            this.comboBoxDatePrevious.FormattingEnabled = true;
            this.comboBoxDatePrevious.Items.AddRange(new object[] {
            "Day(s)",
            "Week(s)",
            "Month(s)",
            "Year(s)"});
            this.comboBoxDatePrevious.Location = new System.Drawing.Point(74, 19);
            this.comboBoxDatePrevious.Name = "comboBoxDatePrevious";
            this.comboBoxDatePrevious.Size = new System.Drawing.Size(99, 24);
            this.comboBoxDatePrevious.TabIndex = 5;
            this.comboBoxDatePrevious.SelectedIndexChanged += new System.EventHandler(this.comboBoxDatePrevious_SelectedIndexChanged);
            // 
            // numericUpDown
            // 
            this.numericUpDown.ForeColor = System.Drawing.Color.Black;
            this.numericUpDown.Location = new System.Drawing.Point(21, 20);
            this.numericUpDown.Name = "numericUpDown";
            this.numericUpDown.Size = new System.Drawing.Size(47, 23);
            this.numericUpDown.TabIndex = 4;
            this.numericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // comboBoxDateChoice
            // 
            this.comboBoxDateChoice.ForeColor = System.Drawing.Color.Blue;
            this.comboBoxDateChoice.FormattingEnabled = true;
            this.comboBoxDateChoice.Items.AddRange(new object[] {
            "For Date",
            "Between Times",
            "Previous"});
            this.comboBoxDateChoice.Location = new System.Drawing.Point(234, 57);
            this.comboBoxDateChoice.Name = "comboBoxDateChoice";
            this.comboBoxDateChoice.Size = new System.Drawing.Size(163, 24);
            this.comboBoxDateChoice.TabIndex = 3;
            this.comboBoxDateChoice.SelectedIndexChanged += new System.EventHandler(this.comboBoxDateChoice_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(165, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Date :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxType
            // 
            this.comboBoxType.ForeColor = System.Drawing.Color.Blue;
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Location = new System.Drawing.Point(235, 17);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(162, 24);
            this.comboBoxType.TabIndex = 1;
            // 
            // labelType
            // 
            this.labelType.ForeColor = System.Drawing.Color.Black;
            this.labelType.Location = new System.Drawing.Point(124, 19);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(104, 20);
            this.labelType.TabIndex = 0;
            this.labelType.Text = "To change:";
            this.labelType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chart
            // 
            /*this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            legend1.ForeColor = System.Drawing.Color.Blue;
            legend1.IsTextAutoFit = false;
            legend1.Name = "Legend1";
            this.chart.Legends.Add(legend1);
            this.chart.Location = new System.Drawing.Point(0, 0);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(865, 566);
            this.chart.TabIndex = 0;
            this.chart.Text = "chart1";*/
            // 
            // UserStatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(865, 745);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStripStat);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Blue;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "UserStatForm";
            this.Text = "User Operation Graph";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UserStatForm_FormClosing);
            this.Load += new System.EventHandler(this.UserStatForm_Load);
            this.toolStripStat.ResumeLayout(false);
            this.toolStripStat.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBoxToDate.ResumeLayout(false);
            this.groupBoxFromDate.ResumeLayout(false);
            this.groupBoxPrevious.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).EndInit();
           // ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripStat;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem readerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonReset;
        private System.Windows.Forms.ToolStripButton toolStripButtonExport;
        private System.Windows.Forms.ToolStripButton toolStripButtonQuit;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonGet;
        private System.Windows.Forms.GroupBox groupBoxToDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerTo;
        private System.Windows.Forms.GroupBox groupBoxFromDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerFrom;
        private System.Windows.Forms.GroupBox groupBoxPrevious;
        private System.Windows.Forms.ComboBox comboBoxDatePrevious;
        private System.Windows.Forms.NumericUpDown numericUpDown;
        private System.Windows.Forms.ComboBox comboBoxDateChoice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Label labelType;
       // private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.CheckBox ckx_versusTime;
        private System.Windows.Forms.ComboBox comboBoxIntervalType;
    }
}