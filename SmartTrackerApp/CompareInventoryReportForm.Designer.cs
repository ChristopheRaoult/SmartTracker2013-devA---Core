using smartTracker.Properties;

namespace smartTracker
{
    partial class CompareInventoryReportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompareInventoryReportForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStripReaderHistory = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBoxReader = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonGet = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.timeTo = new System.Windows.Forms.DateTimePicker();
            this.timeFrom = new System.Windows.Forms.DateTimePicker();
            this.dateTo = new System.Windows.Forms.DateTimePicker();
            this.dateFrom = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControlInventory = new System.Windows.Forms.TabControl();
            this.tabPageAll = new System.Windows.Forms.TabPage();
            this.dataGridViewAll = new System.Windows.Forms.DataGridView();
            this.ImageAll = new System.Windows.Forms.DataGridViewImageColumn();
            this.tabPagePresent = new System.Windows.Forms.TabPage();
            this.dataGridViewPresent = new System.Windows.Forms.DataGridView();
            this.ImagePresent = new System.Windows.Forms.DataGridViewImageColumn();
            this.tabPageAdded = new System.Windows.Forms.TabPage();
            this.dataGridViewAdded = new System.Windows.Forms.DataGridView();
            this.ImageAdd = new System.Windows.Forms.DataGridViewImageColumn();
            this.tabPageRemoved = new System.Windows.Forms.TabPage();
            this.dataGridViewRemoved = new System.Windows.Forms.DataGridView();
            this.ImageRemove = new System.Windows.Forms.DataGridViewImageColumn();
            this.saveExportFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolStripReaderHistory.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControlInventory.SuspendLayout();
            this.tabPageAll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAll)).BeginInit();
            this.tabPagePresent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPresent)).BeginInit();
            this.tabPageAdded.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAdded)).BeginInit();
            this.tabPageRemoved.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRemoved)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripReaderHistory
            // 
            this.toolStripReaderHistory.AutoSize = false;
            this.toolStripReaderHistory.BackColor = System.Drawing.Color.White;
            this.toolStripReaderHistory.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripReaderHistory.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripReaderHistory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripComboBoxReader,
            this.toolStripSeparator1,
            this.toolStripButtonGet,
            this.toolStripButtonExport,
            this.toolStripButtonExit});
            this.toolStripReaderHistory.Location = new System.Drawing.Point(0, 0);
            this.toolStripReaderHistory.Name = "toolStripReaderHistory";
            this.toolStripReaderHistory.Size = new System.Drawing.Size(683, 35);
            this.toolStripReaderHistory.TabIndex = 0;
            this.toolStripReaderHistory.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.ForeColor = System.Drawing.Color.Black;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(68, 32);
            this.toolStripLabel1.Text = ResStrings.UserStatForm_InitForm_Reader;
            // 
            // toolStripComboBoxReader
            // 
            this.toolStripComboBoxReader.DropDownHeight = 150;
            this.toolStripComboBoxReader.DropDownWidth = 200;
            this.toolStripComboBoxReader.ForeColor = System.Drawing.Color.Black;
            this.toolStripComboBoxReader.IntegralHeight = false;
            this.toolStripComboBoxReader.Name = "toolStripComboBoxReader";
            this.toolStripComboBoxReader.Size = new System.Drawing.Size(200, 35);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // toolStripButtonGet
            // 
            this.toolStripButtonGet.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonGet.Image = global::smartTracker.Properties.Resources.refresh;
            this.toolStripButtonGet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGet.Name = "toolStripButtonGet";
            this.toolStripButtonGet.Size = new System.Drawing.Size(93, 32);
            this.toolStripButtonGet.Text = "Process";
            this.toolStripButtonGet.Click += new System.EventHandler(this.toolStripButtonGet_Click);
            // 
            // toolStripButtonExport
            // 
            this.toolStripButtonExport.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonExport.Image = global::smartTracker.Properties.Resources.exportXLS;
            this.toolStripButtonExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExport.Name = "toolStripButtonExport";
            this.toolStripButtonExport.Size = new System.Drawing.Size(84, 32);
            this.toolStripButtonExport.Text = ResStrings.str_Export;
            this.toolStripButtonExport.Click += new System.EventHandler(this.toolStripButtonExport_Click);
            // 
            // toolStripButtonExit
            // 
            this.toolStripButtonExit.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonExit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExit.Image")));
            this.toolStripButtonExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExit.Name = "toolStripButtonExit";
            this.toolStripButtonExit.Size = new System.Drawing.Size(64, 32);
            this.toolStripButtonExit.Text = "Exit";
            this.toolStripButtonExit.Click += new System.EventHandler(this.toolStripButtonExit_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 35);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel1.Controls.Add(this.timeTo);
            this.splitContainer1.Panel1.Controls.Add(this.timeFrom);
            this.splitContainer1.Panel1.Controls.Add(this.dateTo);
            this.splitContainer1.Panel1.Controls.Add(this.dateFrom);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControlInventory);
            this.splitContainer1.Size = new System.Drawing.Size(683, 498);
            this.splitContainer1.SplitterDistance = 100;
            this.splitContainer1.TabIndex = 1;
            // 
            // timeTo
            // 
            this.timeTo.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timeTo.Location = new System.Drawing.Point(291, 46);
            this.timeTo.Name = "timeTo";
            this.timeTo.ShowUpDown = true;
            this.timeTo.Size = new System.Drawing.Size(85, 21);
            this.timeTo.TabIndex = 5;
            // 
            // timeFrom
            // 
            this.timeFrom.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timeFrom.Location = new System.Drawing.Point(291, 21);
            this.timeFrom.Name = "timeFrom";
            this.timeFrom.ShowUpDown = true;
            this.timeFrom.Size = new System.Drawing.Size(85, 21);
            this.timeFrom.TabIndex = 4;
            // 
            // dateTo
            // 
            this.dateTo.Location = new System.Drawing.Point(85, 49);
            this.dateTo.Name = "dateTo";
            this.dateTo.Size = new System.Drawing.Size(200, 21);
            this.dateTo.TabIndex = 3;
            // 
            // dateFrom
            // 
            this.dateFrom.Location = new System.Drawing.Point(85, 21);
            this.dateFrom.Name = "dateFrom";
            this.dateFrom.Size = new System.Drawing.Size(200, 21);
            this.dateFrom.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(38, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "To : ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(23, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "From : ";
            // 
            // tabControlInventory
            // 
            this.tabControlInventory.Controls.Add(this.tabPageAll);
            this.tabControlInventory.Controls.Add(this.tabPagePresent);
            this.tabControlInventory.Controls.Add(this.tabPageAdded);
            this.tabControlInventory.Controls.Add(this.tabPageRemoved);
            this.tabControlInventory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlInventory.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlInventory.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlInventory.ItemSize = new System.Drawing.Size(160, 30);
            this.tabControlInventory.Location = new System.Drawing.Point(0, 0);
            this.tabControlInventory.Name = "tabControlInventory";
            this.tabControlInventory.SelectedIndex = 0;
            this.tabControlInventory.Size = new System.Drawing.Size(683, 394);
            this.tabControlInventory.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlInventory.TabIndex = 1;
            this.tabControlInventory.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControlInventory_DrawItem_1);
            // 
            // tabPageAll
            // 
            this.tabPageAll.Controls.Add(this.dataGridViewAll);
            this.tabPageAll.Location = new System.Drawing.Point(4, 34);
            this.tabPageAll.Name = "tabPageAll";
            this.tabPageAll.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAll.Size = new System.Drawing.Size(675, 356);
            this.tabPageAll.TabIndex = 3;
            this.tabPageAll.Text = ResStrings.str_All;
            this.tabPageAll.UseVisualStyleBackColor = true;
            // 
            // dataGridViewAll
            // 
            this.dataGridViewAll.AllowUserToAddRows = false;
            this.dataGridViewAll.AllowUserToDeleteRows = false;
            this.dataGridViewAll.AllowUserToResizeColumns = false;
            this.dataGridViewAll.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewAll.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewAll.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewAll.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewAll.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dataGridViewAll.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAll.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImageAll});
            this.dataGridViewAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAll.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewAll.Name = "dataGridViewAll";
            this.dataGridViewAll.ReadOnly = true;
            this.dataGridViewAll.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewAll.RowTemplate.Height = 75;
            this.dataGridViewAll.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewAll.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewAll.Size = new System.Drawing.Size(669, 350);
            this.dataGridViewAll.TabIndex = 0;
            this.dataGridViewAll.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridViewAll_CellFormatting);
            // 
            // ImageAll
            // 
            this.ImageAll.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.NullValue = null;
            this.ImageAll.DefaultCellStyle = dataGridViewCellStyle2;
            this.ImageAll.FillWeight = 200F;
            this.ImageAll.HeaderText = "Image";
            this.ImageAll.MinimumWidth = 200;
            this.ImageAll.Name = "ImageAll";
            this.ImageAll.ReadOnly = true;
            this.ImageAll.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ImageAll.Width = 200;
            // 
            // tabPagePresent
            // 
            this.tabPagePresent.Controls.Add(this.dataGridViewPresent);
            this.tabPagePresent.Location = new System.Drawing.Point(4, 34);
            this.tabPagePresent.Name = "tabPagePresent";
            this.tabPagePresent.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePresent.Size = new System.Drawing.Size(675, 366);
            this.tabPagePresent.TabIndex = 0;
            this.tabPagePresent.Text = "Previous";
            this.tabPagePresent.UseVisualStyleBackColor = true;
            // 
            // dataGridViewPresent
            // 
            this.dataGridViewPresent.AllowUserToAddRows = false;
            this.dataGridViewPresent.AllowUserToDeleteRows = false;
            this.dataGridViewPresent.AllowUserToResizeColumns = false;
            this.dataGridViewPresent.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewPresent.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewPresent.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewPresent.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewPresent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPresent.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImagePresent});
            this.dataGridViewPresent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPresent.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewPresent.Name = "dataGridViewPresent";
            this.dataGridViewPresent.ReadOnly = true;
            this.dataGridViewPresent.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewPresent.RowTemplate.Height = 75;
            this.dataGridViewPresent.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewPresent.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPresent.Size = new System.Drawing.Size(669, 360);
            this.dataGridViewPresent.TabIndex = 0;
            this.dataGridViewPresent.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridViewPresent_CellFormatting);
            // 
            // ImagePresent
            // 
            this.ImagePresent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ImagePresent.FillWeight = 200F;
            this.ImagePresent.HeaderText = "Image";
            this.ImagePresent.MinimumWidth = 200;
            this.ImagePresent.Name = "ImagePresent";
            this.ImagePresent.ReadOnly = true;
            this.ImagePresent.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ImagePresent.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ImagePresent.Width = 200;
            // 
            // tabPageAdded
            // 
            this.tabPageAdded.Controls.Add(this.dataGridViewAdded);
            this.tabPageAdded.Location = new System.Drawing.Point(4, 34);
            this.tabPageAdded.Name = "tabPageAdded";
            this.tabPageAdded.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAdded.Size = new System.Drawing.Size(675, 366);
            this.tabPageAdded.TabIndex = 1;
            this.tabPageAdded.Text = ResStrings.str_Added;
            this.tabPageAdded.UseVisualStyleBackColor = true;
            // 
            // dataGridViewAdded
            // 
            this.dataGridViewAdded.AllowUserToAddRows = false;
            this.dataGridViewAdded.AllowUserToDeleteRows = false;
            this.dataGridViewAdded.AllowUserToResizeColumns = false;
            this.dataGridViewAdded.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewAdded.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewAdded.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewAdded.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewAdded.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAdded.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImageAdd});
            this.dataGridViewAdded.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAdded.GridColor = System.Drawing.SystemColors.ActiveBorder;
            this.dataGridViewAdded.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewAdded.Name = "dataGridViewAdded";
            this.dataGridViewAdded.ReadOnly = true;
            this.dataGridViewAdded.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewAdded.RowTemplate.Height = 75;
            this.dataGridViewAdded.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewAdded.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewAdded.Size = new System.Drawing.Size(669, 360);
            this.dataGridViewAdded.TabIndex = 0;
            this.dataGridViewAdded.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridViewAdded_CellFormatting);
            // 
            // ImageAdd
            // 
            this.ImageAdd.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ImageAdd.FillWeight = 200F;
            this.ImageAdd.HeaderText = "Image";
            this.ImageAdd.MinimumWidth = 200;
            this.ImageAdd.Name = "ImageAdd";
            this.ImageAdd.ReadOnly = true;
            this.ImageAdd.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ImageAdd.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ImageAdd.Width = 200;
            // 
            // tabPageRemoved
            // 
            this.tabPageRemoved.Controls.Add(this.dataGridViewRemoved);
            this.tabPageRemoved.Location = new System.Drawing.Point(4, 34);
            this.tabPageRemoved.Name = "tabPageRemoved";
            this.tabPageRemoved.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRemoved.Size = new System.Drawing.Size(675, 366);
            this.tabPageRemoved.TabIndex = 2;
            this.tabPageRemoved.Text = "Removed";
            this.tabPageRemoved.UseVisualStyleBackColor = true;
            // 
            // dataGridViewRemoved
            // 
            this.dataGridViewRemoved.AllowUserToAddRows = false;
            this.dataGridViewRemoved.AllowUserToDeleteRows = false;
            this.dataGridViewRemoved.AllowUserToResizeColumns = false;
            this.dataGridViewRemoved.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewRemoved.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewRemoved.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewRemoved.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewRemoved.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRemoved.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImageRemove});
            this.dataGridViewRemoved.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRemoved.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewRemoved.Name = "dataGridViewRemoved";
            this.dataGridViewRemoved.ReadOnly = true;
            this.dataGridViewRemoved.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewRemoved.RowTemplate.Height = 75;
            this.dataGridViewRemoved.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewRemoved.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRemoved.Size = new System.Drawing.Size(669, 360);
            this.dataGridViewRemoved.TabIndex = 0;
            this.dataGridViewRemoved.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridViewRemoved_CellFormatting);
            // 
            // ImageRemove
            // 
            this.ImageRemove.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ImageRemove.FillWeight = 200F;
            this.ImageRemove.HeaderText = "Image";
            this.ImageRemove.MinimumWidth = 200;
            this.ImageRemove.Name = "ImageRemove";
            this.ImageRemove.ReadOnly = true;
            this.ImageRemove.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ImageRemove.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ImageRemove.Width = 200;
            // 
            // saveExportFileDialog
            // 
            this.saveExportFileDialog.Filter = "Excel 97 - 2003 File (*.xls)|*.xls|Excel 2010 File (*.xlsx)|*.xlsx";
            this.saveExportFileDialog.RestoreDirectory = true;
            // 
            // CompareInventoryReportForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(683, 533);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStripReaderHistory);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CompareInventoryReportForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Reader History";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ReaderHistoryForm_FormClosing);
            this.Load += new System.EventHandler(this.ReaderHistoryForm_Load);
            this.toolStripReaderHistory.ResumeLayout(false);
            this.toolStripReaderHistory.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControlInventory.ResumeLayout(false);
            this.tabPageAll.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAll)).EndInit();
            this.tabPagePresent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPresent)).EndInit();
            this.tabPageAdded.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAdded)).EndInit();
            this.tabPageRemoved.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRemoved)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripReaderHistory;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxReader;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonGet;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripButton toolStripButtonExport;
        private System.Windows.Forms.SaveFileDialog saveExportFileDialog;
        private System.Windows.Forms.DateTimePicker timeTo;
        private System.Windows.Forms.DateTimePicker timeFrom;
        private System.Windows.Forms.DateTimePicker dateTo;
        private System.Windows.Forms.DateTimePicker dateFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControlInventory;
        private System.Windows.Forms.TabPage tabPageAll;
        private System.Windows.Forms.DataGridView dataGridViewAll;
        private System.Windows.Forms.DataGridViewImageColumn ImageAll;
        private System.Windows.Forms.TabPage tabPagePresent;
        private System.Windows.Forms.DataGridView dataGridViewPresent;
        private System.Windows.Forms.DataGridViewImageColumn ImagePresent;
        private System.Windows.Forms.TabPage tabPageAdded;
        private System.Windows.Forms.DataGridView dataGridViewAdded;
        private System.Windows.Forms.DataGridViewImageColumn ImageAdd;
        private System.Windows.Forms.TabPage tabPageRemoved;
        private System.Windows.Forms.DataGridView dataGridViewRemoved;
        private System.Windows.Forms.DataGridViewImageColumn ImageRemove;
    }
}