namespace smartTracker
{
    partial class LiveDataViewerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LiveDataViewerForm));
            this.toolStripLiveData = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.exportToExcelToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAndRunExcelMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareInventoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAssociationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.treeViewDevice = new System.Windows.Forms.TreeView();
            this.contextMenuStripDebug = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyLotIDToClipBoardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToClipBoardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unselectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grouptimageList = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStripReader = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.calibrateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findThresholdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.conversionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doorLightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerStartup = new System.Windows.Forms.Timer(this.components);
            this.saveXlsFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialogImage = new System.Windows.Forms.OpenFileDialog();
            this.timerObjectList = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabControlInfo = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.labelPresent = new System.Windows.Forms.Label();
            this.labelRemoved = new System.Windows.Forms.Label();
            this.labelAdded = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelInventoryTagCount = new System.Windows.Forms.Label();
            this.labelInventoryUser = new System.Windows.Forms.Label();
            this.labelInventoryDate = new System.Windows.Forms.Label();
            this.listBoxTag = new System.Windows.Forms.ListBox();
            this.tagListDataGrid = new System.Windows.Forms.DataGridView();
            this.TagID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SequentialPresent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SequentialMissing = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PresentTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MissingTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridViewScan = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.txtCriteria = new System.Windows.Forms.TextBox();
            this.txtBoxDesc = new System.Windows.Forms.TextBox();
            this.txtBoxRef = new System.Windows.Forms.TextBox();
            this.txtBoxTagUid = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dataListView = new BrightIdeasSoftware.DataListView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripDropDownButtonView = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripLiveData.SuspendLayout();
            this.contextMenuStripDebug.SuspendLayout();
            this.contextMenuStripReader.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControlInfo.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tagListDataGrid)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewScan)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListView)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripLiveData
            // 
            this.toolStripLiveData.AutoSize = false;
            this.toolStripLiveData.BackColor = System.Drawing.Color.White;
            this.toolStripLiveData.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLiveData.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripLiveData.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton,
            this.toolStripSeparator3,
            this.toolStripButtonExit});
            this.toolStripLiveData.Location = new System.Drawing.Point(0, 0);
            this.toolStripLiveData.Name = "toolStripLiveData";
            this.toolStripLiveData.Size = new System.Drawing.Size(1037, 35);
            this.toolStripLiveData.TabIndex = 0;
            this.toolStripLiveData.Text = "toolStrip1";
            // 
            // toolStripDropDownButton
            // 
            this.toolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToExcelToolStripMenuItem1,
            this.exportAndRunExcelMToolStripMenuItem,
            this.compareInventoryToolStripMenuItem,
            this.deleteAssociationToolStripMenuItem});
            this.toolStripDropDownButton.ForeColor = System.Drawing.Color.Black;
            this.toolStripDropDownButton.Image = global::smartTracker.Properties.Resources.action;
            this.toolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton.Name = "toolStripDropDownButton";
            this.toolStripDropDownButton.Size = new System.Drawing.Size(91, 32);
            this.toolStripDropDownButton.Text = "Action";
            // 
            // exportToExcelToolStripMenuItem1
            // 
            this.exportToExcelToolStripMenuItem1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.exportToExcelToolStripMenuItem1.Image = global::smartTracker.Properties.Resources.exportXLS;
            this.exportToExcelToolStripMenuItem1.Name = "exportToExcelToolStripMenuItem1";
            this.exportToExcelToolStripMenuItem1.Size = new System.Drawing.Size(277, 22);
            this.exportToExcelToolStripMenuItem1.Text = "Export to Excel";
            this.exportToExcelToolStripMenuItem1.Click += new System.EventHandler(this.toolStripButExportXLS_Click);
            // 
            // exportAndRunExcelMToolStripMenuItem
            // 
            this.exportAndRunExcelMToolStripMenuItem.Image = global::smartTracker.Properties.Resources.exportXLS;
            this.exportAndRunExcelMToolStripMenuItem.Name = "exportAndRunExcelMToolStripMenuItem";
            this.exportAndRunExcelMToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.exportAndRunExcelMToolStripMenuItem.Text = "Export and Run Excel Macro";
            this.exportAndRunExcelMToolStripMenuItem.Click += new System.EventHandler(this.exportAndRunExcelMToolStripMenuItem_Click);
            // 
            // compareInventoryToolStripMenuItem
            // 
            this.compareInventoryToolStripMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.compareInventoryToolStripMenuItem.Image = global::smartTracker.Properties.Resources.compareXLS;
            this.compareInventoryToolStripMenuItem.Name = "compareInventoryToolStripMenuItem";
            this.compareInventoryToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.compareInventoryToolStripMenuItem.Text = "Compare Inventory";
            this.compareInventoryToolStripMenuItem.Click += new System.EventHandler(this.toolStripCompare_Click);
            // 
            // deleteAssociationToolStripMenuItem
            // 
            this.deleteAssociationToolStripMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.deleteAssociationToolStripMenuItem.Image = global::smartTracker.Properties.Resources.remove_link;
            this.deleteAssociationToolStripMenuItem.Name = "deleteAssociationToolStripMenuItem";
            this.deleteAssociationToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.deleteAssociationToolStripMenuItem.Text = "Delete Association";
            this.deleteAssociationToolStripMenuItem.Click += new System.EventHandler(this.toolStripButtonRemoveLink_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 35);
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
            // treeViewDevice
            // 
            this.treeViewDevice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewDevice.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewDevice.FullRowSelect = true;
            this.treeViewDevice.HideSelection = false;
            this.treeViewDevice.ItemHeight = 20;
            this.treeViewDevice.Location = new System.Drawing.Point(0, 0);
            this.treeViewDevice.Name = "treeViewDevice";
            this.treeViewDevice.Size = new System.Drawing.Size(296, 176);
            this.treeViewDevice.TabIndex = 0;
            this.treeViewDevice.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDevice_AfterCollapse);
            this.treeViewDevice.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDevice_AfterExpand);
            this.treeViewDevice.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDevice_AfterSelect);
            this.treeViewDevice.DoubleClick += new System.EventHandler(this.treeViewDevice_DoubleClick);
            this.treeViewDevice.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeViewDevice_KeyDown);
            // 
            // contextMenuStripDebug
            // 
            this.contextMenuStripDebug.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextMenuStripDebug.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyLotIDToClipBoardToolStripMenuItem,
            this.copyToClipBoardToolStripMenuItem,
            this.unselectAllToolStripMenuItem,
            this.itemListToolStripMenuItem});
            this.contextMenuStripDebug.Name = "contextMenuStrip";
            this.contextMenuStripDebug.Size = new System.Drawing.Size(297, 92);
            // 
            // copyLotIDToClipBoardToolStripMenuItem
            // 
            this.copyLotIDToClipBoardToolStripMenuItem.Image = global::smartTracker.Properties.Resources.icon_overwrite;
            this.copyLotIDToClipBoardToolStripMenuItem.Name = "copyLotIDToClipBoardToolStripMenuItem";
            this.copyLotIDToClipBoardToolStripMenuItem.Size = new System.Drawing.Size(296, 22);
            this.copyLotIDToClipBoardToolStripMenuItem.Text = "Copy Selected LotID to ClipBoard";
            this.copyLotIDToClipBoardToolStripMenuItem.Click += new System.EventHandler(this.copyLotIDToClipBoardToolStripMenuItem_Click);
            // 
            // copyToClipBoardToolStripMenuItem
            // 
            this.copyToClipBoardToolStripMenuItem.Image = global::smartTracker.Properties.Resources.icon_overwrite;
            this.copyToClipBoardToolStripMenuItem.Name = "copyToClipBoardToolStripMenuItem";
            this.copyToClipBoardToolStripMenuItem.Size = new System.Drawing.Size(296, 22);
            this.copyToClipBoardToolStripMenuItem.Text = "Copy Full List to ClipBoard";
            this.copyToClipBoardToolStripMenuItem.Click += new System.EventHandler(this.copyToClipBoardToolStripMenuItem_Click);
            // 
            // unselectAllToolStripMenuItem
            // 
            this.unselectAllToolStripMenuItem.Image = global::smartTracker.Properties.Resources.edit_clear;
            this.unselectAllToolStripMenuItem.Name = "unselectAllToolStripMenuItem";
            this.unselectAllToolStripMenuItem.Size = new System.Drawing.Size(296, 22);
            this.unselectAllToolStripMenuItem.Text = "Unselect all";
            this.unselectAllToolStripMenuItem.Click += new System.EventHandler(this.unselectAllToolStripMenuItem_Click);
            // 
            // itemListToolStripMenuItem
            // 
            this.itemListToolStripMenuItem.Image = global::smartTracker.Properties.Resources.shoplist;
            this.itemListToolStripMenuItem.Name = "itemListToolStripMenuItem";
            this.itemListToolStripMenuItem.Size = new System.Drawing.Size(296, 22);
            this.itemListToolStripMenuItem.Text = "&Item List";
            this.itemListToolStripMenuItem.Click += new System.EventHandler(this.itemListToolStripMenuItem_Click);
            // 
            // grouptimageList
            // 
            this.grouptimageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("grouptimageList.ImageStream")));
            this.grouptimageList.TransparentColor = System.Drawing.Color.Transparent;
            this.grouptimageList.Images.SetKeyName(0, "present");
            this.grouptimageList.Images.SetKeyName(1, "remove");
            this.grouptimageList.Images.SetKeyName(2, "add");
            this.grouptimageList.Images.SetKeyName(3, "attention");
            this.grouptimageList.Images.SetKeyName(4, "attention-1mois");
            this.grouptimageList.Images.SetKeyName(5, "attention-3mois");
            this.grouptimageList.Images.SetKeyName(6, "expired");
            // 
            // contextMenuStripReader
            // 
            this.contextMenuStripReader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.calibrateToolStripMenuItem,
            this.findThresholdToolStripMenuItem,
            this.tagSetToolStripMenuItem,
            this.conversionToolStripMenuItem,
            this.doorLightToolStripMenuItem});
            this.contextMenuStripReader.Name = "contextMenuStripReader";
            this.contextMenuStripReader.Size = new System.Drawing.Size(154, 114);
            // 
            // calibrateToolStripMenuItem
            // 
            this.calibrateToolStripMenuItem.Name = "calibrateToolStripMenuItem";
            this.calibrateToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.calibrateToolStripMenuItem.Text = "&Calibrate";
            this.calibrateToolStripMenuItem.Click += new System.EventHandler(this.calibrateToolStripMenuItem_Click);
            // 
            // findThresholdToolStripMenuItem
            // 
            this.findThresholdToolStripMenuItem.Name = "findThresholdToolStripMenuItem";
            this.findThresholdToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.findThresholdToolStripMenuItem.Text = "&Find Threshold";
            this.findThresholdToolStripMenuItem.Click += new System.EventHandler(this.findThresholdToolStripMenuItem_Click);
            // 
            // tagSetToolStripMenuItem
            // 
            this.tagSetToolStripMenuItem.Name = "tagSetToolStripMenuItem";
            this.tagSetToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.tagSetToolStripMenuItem.Text = "&TagSet";
            this.tagSetToolStripMenuItem.Click += new System.EventHandler(this.tagSetToolStripMenuItem_Click);
            // 
            // conversionToolStripMenuItem
            // 
            this.conversionToolStripMenuItem.Name = "conversionToolStripMenuItem";
            this.conversionToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.conversionToolStripMenuItem.Text = "C&onversion";
            this.conversionToolStripMenuItem.Click += new System.EventHandler(this.conversionToolStripMenuItem_Click);
            // 
            // doorLightToolStripMenuItem
            // 
            this.doorLightToolStripMenuItem.Name = "doorLightToolStripMenuItem";
            this.doorLightToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.doorLightToolStripMenuItem.Text = "&Door && Light";
            this.doorLightToolStripMenuItem.Click += new System.EventHandler(this.doorLightToolStripMenuItem_Click);
            // 
            // timerStartup
            // 
            this.timerStartup.Tick += new System.EventHandler(this.timerStartup_Tick);
            // 
            // saveXlsFileDialog
            // 
            this.saveXlsFileDialog.Filter = "Excel 2007 - 2013 File (*.xlsx)|*.xlsx";
            // 
            // openFileDialogImage
            // 
            this.openFileDialogImage.FileName = "openFileDialogImage";
            this.openFileDialogImage.Filter = "\"Image Files|*.jpg;*.gif;*.bmp;*.png;*.jpeg";
            // 
            // timerObjectList
            // 
            this.timerObjectList.Enabled = true;
            this.timerObjectList.Tick += new System.EventHandler(this.timerObjectList_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 35);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1MinSize = 180;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataListView);
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(1037, 539);
            this.splitContainer1.SplitterDistance = 180;
            this.splitContainer1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeViewDevice);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Panel2.Controls.Add(this.tabControlInfo);
            this.splitContainer2.Size = new System.Drawing.Size(1037, 180);
            this.splitContainer2.SplitterDistance = 300;
            this.splitContainer2.SplitterIncrement = 5;
            this.splitContainer2.TabIndex = 0;
            // 
            // tabControlInfo
            // 
            this.tabControlInfo.Controls.Add(this.tabPage1);
            this.tabControlInfo.Controls.Add(this.tabPage2);
            this.tabControlInfo.Controls.Add(this.tabPage3);
            this.tabControlInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlInfo.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlInfo.Location = new System.Drawing.Point(0, 0);
            this.tabControlInfo.Name = "tabControlInfo";
            this.tabControlInfo.SelectedIndex = 0;
            this.tabControlInfo.Size = new System.Drawing.Size(729, 176);
            this.tabControlInfo.TabIndex = 27;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.Controls.Add(this.labelPresent);
            this.tabPage1.Controls.Add(this.labelRemoved);
            this.tabPage1.Controls.Add(this.labelAdded);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.labelInventoryTagCount);
            this.tabPage1.Controls.Add(this.labelInventoryUser);
            this.tabPage1.Controls.Add(this.labelInventoryDate);
            this.tabPage1.Controls.Add(this.listBoxTag);
            this.tabPage1.Controls.Add(this.tagListDataGrid);
            this.tabPage1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(721, 147);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Inventory Info";
            // 
            // labelPresent
            // 
            this.labelPresent.AutoSize = true;
            this.labelPresent.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPresent.ForeColor = System.Drawing.Color.Blue;
            this.labelPresent.Location = new System.Drawing.Point(347, 17);
            this.labelPresent.Name = "labelPresent";
            this.labelPresent.Size = new System.Drawing.Size(139, 25);
            this.labelPresent.TabIndex = 36;
            this.labelPresent.Text = "Present : 0";
            // 
            // labelRemoved
            // 
            this.labelRemoved.AutoSize = true;
            this.labelRemoved.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRemoved.ForeColor = System.Drawing.Color.Red;
            this.labelRemoved.Location = new System.Drawing.Point(329, 101);
            this.labelRemoved.Name = "labelRemoved";
            this.labelRemoved.Size = new System.Drawing.Size(158, 25);
            this.labelRemoved.TabIndex = 35;
            this.labelRemoved.Text = "Removed : 0";
            // 
            // labelAdded
            // 
            this.labelAdded.AutoSize = true;
            this.labelAdded.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAdded.ForeColor = System.Drawing.Color.Green;
            this.labelAdded.Location = new System.Drawing.Point(362, 59);
            this.labelAdded.Name = "labelAdded";
            this.labelAdded.Size = new System.Drawing.Size(124, 25);
            this.labelAdded.TabIndex = 34;
            this.labelAdded.Text = "Added : 0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(11, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 16);
            this.label1.TabIndex = 33;
            this.label1.Text = "Inventory date :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(13, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 16);
            this.label2.TabIndex = 31;
            this.label2.Text = "Inventory user :";
            // 
            // labelInventoryTagCount
            // 
            this.labelInventoryTagCount.AutoSize = true;
            this.labelInventoryTagCount.BackColor = System.Drawing.Color.Transparent;
            this.labelInventoryTagCount.Font = new System.Drawing.Font("Verdana", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInventoryTagCount.ForeColor = System.Drawing.Color.Blue;
            this.labelInventoryTagCount.Location = new System.Drawing.Point(20, 83);
            this.labelInventoryTagCount.Name = "labelInventoryTagCount";
            this.labelInventoryTagCount.Size = new System.Drawing.Size(195, 45);
            this.labelInventoryTagCount.TabIndex = 29;
            this.labelInventoryTagCount.Text = "Tag(s): 0";
            // 
            // labelInventoryUser
            // 
            this.labelInventoryUser.AutoSize = true;
            this.labelInventoryUser.BackColor = System.Drawing.Color.Transparent;
            this.labelInventoryUser.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInventoryUser.ForeColor = System.Drawing.Color.Black;
            this.labelInventoryUser.Location = new System.Drawing.Point(136, 44);
            this.labelInventoryUser.Name = "labelInventoryUser";
            this.labelInventoryUser.Size = new System.Drawing.Size(126, 16);
            this.labelInventoryUser.TabIndex = 28;
            this.labelInventoryUser.Text = "Inventory User :";
            // 
            // labelInventoryDate
            // 
            this.labelInventoryDate.AutoSize = true;
            this.labelInventoryDate.BackColor = System.Drawing.Color.Transparent;
            this.labelInventoryDate.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInventoryDate.ForeColor = System.Drawing.Color.Black;
            this.labelInventoryDate.Location = new System.Drawing.Point(136, 17);
            this.labelInventoryDate.Name = "labelInventoryDate";
            this.labelInventoryDate.Size = new System.Drawing.Size(127, 16);
            this.labelInventoryDate.TabIndex = 27;
            this.labelInventoryDate.Text = "Inventory Date :";
            // 
            // listBoxTag
            // 
            this.listBoxTag.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxTag.ColumnWidth = 100;
            this.listBoxTag.ForeColor = System.Drawing.Color.Black;
            this.listBoxTag.FormattingEnabled = true;
            this.listBoxTag.Location = new System.Drawing.Point(500, 10);
            this.listBoxTag.MultiColumn = true;
            this.listBoxTag.Name = "listBoxTag";
            this.listBoxTag.Size = new System.Drawing.Size(185, 121);
            this.listBoxTag.TabIndex = 30;
            this.listBoxTag.Visible = false;
            // 
            // tagListDataGrid
            // 
            this.tagListDataGrid.AllowUserToAddRows = false;
            this.tagListDataGrid.AllowUserToDeleteRows = false;
            this.tagListDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tagListDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tagListDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TagID,
            this.SequentialPresent,
            this.SequentialMissing,
            this.PresentTotal,
            this.MissingTotal});
            this.tagListDataGrid.Location = new System.Drawing.Point(490, 6);
            this.tagListDataGrid.Name = "tagListDataGrid";
            this.tagListDataGrid.ReadOnly = true;
            this.tagListDataGrid.RowHeadersVisible = false;
            this.tagListDataGrid.Size = new System.Drawing.Size(185, 130);
            this.tagListDataGrid.TabIndex = 32;
            this.tagListDataGrid.Visible = false;
            // 
            // TagID
            // 
            this.TagID.HeaderText = "Tag ID";
            this.TagID.Name = "TagID";
            this.TagID.ReadOnly = true;
            this.TagID.Width = 200;
            // 
            // SequentialPresent
            // 
            this.SequentialPresent.HeaderText = "Present Count";
            this.SequentialPresent.Name = "SequentialPresent";
            this.SequentialPresent.ReadOnly = true;
            // 
            // SequentialMissing
            // 
            this.SequentialMissing.HeaderText = "Missing Count";
            this.SequentialMissing.Name = "SequentialMissing";
            this.SequentialMissing.ReadOnly = true;
            // 
            // PresentTotal
            // 
            this.PresentTotal.HeaderText = "Present Total";
            this.PresentTotal.Name = "PresentTotal";
            this.PresentTotal.ReadOnly = true;
            // 
            // MissingTotal
            // 
            this.MissingTotal.HeaderText = "Missing Total";
            this.MissingTotal.Name = "MissingTotal";
            this.MissingTotal.ReadOnly = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridViewScan);
            this.tabPage2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(721, 147);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Reader History";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridViewScan
            // 
            this.dataGridViewScan.AllowUserToAddRows = false;
            this.dataGridViewScan.AllowUserToDeleteRows = false;
            this.dataGridViewScan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewScan.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewScan.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewScan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewScan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewScan.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewScan.MultiSelect = false;
            this.dataGridViewScan.Name = "dataGridViewScan";
            this.dataGridViewScan.ReadOnly = true;
            this.dataGridViewScan.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewScan.Size = new System.Drawing.Size(715, 141);
            this.dataGridViewScan.TabIndex = 1;
            this.dataGridViewScan.SelectionChanged += new System.EventHandler(this.dataGridViewScan_SelectionChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button1);
            this.tabPage3.Controls.Add(this.txtCriteria);
            this.tabPage3.Controls.Add(this.txtBoxDesc);
            this.tabPage3.Controls.Add(this.txtBoxRef);
            this.tabPage3.Controls.Add(this.txtBoxTagUid);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(721, 147);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Box Mode";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(520, 117);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Check";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtCriteria
            // 
            this.txtCriteria.Location = new System.Drawing.Point(145, 117);
            this.txtCriteria.Name = "txtCriteria";
            this.txtCriteria.Size = new System.Drawing.Size(360, 23);
            this.txtCriteria.TabIndex = 15;
            // 
            // txtBoxDesc
            // 
            this.txtBoxDesc.Location = new System.Drawing.Point(145, 83);
            this.txtBoxDesc.Name = "txtBoxDesc";
            this.txtBoxDesc.Size = new System.Drawing.Size(360, 23);
            this.txtBoxDesc.TabIndex = 14;
            // 
            // txtBoxRef
            // 
            this.txtBoxRef.Location = new System.Drawing.Point(145, 44);
            this.txtBoxRef.Name = "txtBoxRef";
            this.txtBoxRef.Size = new System.Drawing.Size(360, 23);
            this.txtBoxRef.TabIndex = 13;
            // 
            // txtBoxTagUid
            // 
            this.txtBoxTagUid.Location = new System.Drawing.Point(145, 7);
            this.txtBoxTagUid.Name = "txtBoxTagUid";
            this.txtBoxTagUid.Size = new System.Drawing.Size(360, 23);
            this.txtBoxTagUid.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(63, 120);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "Criteria : ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 86);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "Box Description :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 47);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 16);
            this.label5.TabIndex = 9;
            this.label5.Text = "Box Reference :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(33, 10);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(105, 16);
            this.label6.TabIndex = 8;
            this.label6.Text = "Tag UID Box :";
            // 
            // dataListView
            // 
            this.dataListView.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dataListView.ContextMenuStrip = this.contextMenuStripDebug;
            this.dataListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.dataListView.DataSource = null;
            this.dataListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataListView.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataListView.FullRowSelect = true;
            this.dataListView.GridLines = true;
            this.dataListView.GroupImageList = this.grouptimageList;
            this.dataListView.Location = new System.Drawing.Point(0, 0);
            this.dataListView.Name = "dataListView";
            this.dataListView.ShowItemCountOnGroups = true;
            this.dataListView.Size = new System.Drawing.Size(1033, 329);
            this.dataListView.SpaceBetweenGroups = 20;
            this.dataListView.TabIndex = 4;
            this.dataListView.UseCompatibleStateImageBehavior = false;
            this.dataListView.UseCustomSelectionColors = true;
            this.dataListView.UseExplorerTheme = true;
            this.dataListView.UseFiltering = true;
            this.dataListView.UseHotItem = true;
            this.dataListView.UseTranslucentHotItem = true;
            this.dataListView.View = System.Windows.Forms.View.Details;
            this.dataListView.AfterCreatingGroups += new System.EventHandler<BrightIdeasSoftware.CreateGroupsEventArgs>(this.dataListView_AfterCreatingGroups);
            this.dataListView.BeforeCreatingGroups += new System.EventHandler<BrightIdeasSoftware.CreateGroupsEventArgs>(this.dataListView_BeforeCreatingGroups);
            this.dataListView.HotItemChanged += new System.EventHandler<BrightIdeasSoftware.HotItemChangedEventArgs>(this.dataListView_HotItemChanged);
            this.dataListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dataListView_MouseDoubleClick_1);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton1,
            this.toolStripDropDownButtonView,
            this.toolStripDropDownButton1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 329);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1033, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.Image = global::smartTracker.Properties.Resources.refresh;
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(136, 20);
            this.toolStripSplitButton1.Text = "Group Mode : OFF";
            this.toolStripSplitButton1.ButtonClick += new System.EventHandler(this.toolStripSplitButton1_ButtonClick);
            // 
            // toolStripDropDownButtonView
            // 
            this.toolStripDropDownButtonView.Image = global::smartTracker.Properties.Resources._23164_bubka_Laptop;
            this.toolStripDropDownButtonView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonView.Name = "toolStripDropDownButtonView";
            this.toolStripDropDownButtonView.Size = new System.Drawing.Size(109, 20);
            this.toolStripDropDownButtonView.Text = "Display in Tile";
            this.toolStripDropDownButtonView.Click += new System.EventHandler(this.toolStripDropDownButtonView_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.Image = global::smartTracker.Properties.Resources.icon_overwrite;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(117, 20);
            this.toolStripDropDownButton1.Text = "Collapse Group";
            this.toolStripDropDownButton1.Click += new System.EventHandler(this.toolStripDropDownButton1_Click);
            // 
            // LiveDataViewerForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1037, 574);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStripLiveData);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(700, 550);
            this.Name = "LiveDataViewerForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Viewer Data Mode";
            this.Activated += new System.EventHandler(this.LiveDataForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LiveDataForm_FormClosing);
            this.Load += new System.EventHandler(this.LiveDataForm_Load);
            this.toolStripLiveData.ResumeLayout(false);
            this.toolStripLiveData.PerformLayout();
            this.contextMenuStripDebug.ResumeLayout(false);
            this.contextMenuStripReader.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.tabControlInfo.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tagListDataGrid)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewScan)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataListView)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripLiveData;
        private System.Windows.Forms.TreeView treeViewDevice;
        private System.Windows.Forms.Timer timerStartup;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripDebug;
        private System.Windows.Forms.ToolStripMenuItem itemListToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveXlsFileDialog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripReader;
        private System.Windows.Forms.ToolStripMenuItem calibrateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findThresholdToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem conversionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tagSetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem doorLightToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem compareInventoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAssociationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToExcelToolStripMenuItem1;
        private System.Windows.Forms.OpenFileDialog openFileDialogImage;
        private System.Windows.Forms.ToolStripMenuItem copyLotIDToClipBoardToolStripMenuItem;
        private System.Windows.Forms.Timer timerObjectList;
        private System.Windows.Forms.ImageList grouptimageList;
        private System.Windows.Forms.ToolStripMenuItem copyToClipBoardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unselectAllToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private BrightIdeasSoftware.DataListView dataListView;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonView;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem exportAndRunExcelMToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControlInfo;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label labelPresent;
        private System.Windows.Forms.Label labelRemoved;
        private System.Windows.Forms.Label labelAdded;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelInventoryTagCount;
        private System.Windows.Forms.Label labelInventoryUser;
        private System.Windows.Forms.Label labelInventoryDate;
        private System.Windows.Forms.ListBox listBoxTag;
        private System.Windows.Forms.DataGridView tagListDataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn TagID;
        private System.Windows.Forms.DataGridViewTextBoxColumn SequentialPresent;
        private System.Windows.Forms.DataGridViewTextBoxColumn SequentialMissing;
        private System.Windows.Forms.DataGridViewTextBoxColumn PresentTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn MissingTotal;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridViewScan;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox txtCriteria;
        private System.Windows.Forms.TextBox txtBoxDesc;
        private System.Windows.Forms.TextBox txtBoxRef;
        private System.Windows.Forms.TextBox txtBoxTagUid;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        

    }
}