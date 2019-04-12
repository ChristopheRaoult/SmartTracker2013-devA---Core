using smartTracker.Properties;

namespace smartTracker
{
    partial class LiveDataForDeviceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LiveDataForDeviceForm));
            this.toolStripLiveData = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonScan = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStopScan = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.treeViewDevice = new System.Windows.Forms.TreeView();
            this.grouptimageList = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStripReader = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.calibrateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findThresholdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.conversionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doorLightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renewFingerprintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerStartup = new System.Windows.Forms.Timer(this.components);
            this.timerRestart = new System.Windows.Forms.Timer(this.components);
            this.openFileDialogImage = new System.Windows.Forms.OpenFileDialog();
            this.timerObjectList = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.pictureBoxAdded = new System.Windows.Forms.PictureBox();
            this.pictureBoxRemoved = new System.Windows.Forms.PictureBox();
            this.labelInventoryTagCount = new System.Windows.Forms.Label();
            this.labelRemoved = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.labelInventoryUser = new System.Windows.Forms.Label();
            this.labelAdded = new System.Windows.Forms.Label();
            this.labelInventoryDate = new System.Windows.Forms.Label();
            this.listBoxTag = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBoxPresent = new System.Windows.Forms.PictureBox();
            this.labelPresent = new System.Windows.Forms.Label();
            this.dataGridViewScan = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripLiveData.SuspendLayout();
            this.contextMenuStripReader.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdded)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRemoved)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPresent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewScan)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripLiveData
            // 
            this.toolStripLiveData.AutoSize = false;
            this.toolStripLiveData.BackColor = System.Drawing.Color.White;
            this.toolStripLiveData.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLiveData.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripLiveData.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripLiveData.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonScan,
            this.toolStripButtonStopScan,
            this.toolStripSeparator1,
            this.toolStripButtonExit});
            this.toolStripLiveData.Location = new System.Drawing.Point(0, 0);
            this.toolStripLiveData.Name = "toolStripLiveData";
            this.toolStripLiveData.Size = new System.Drawing.Size(1037, 35);
            this.toolStripLiveData.TabIndex = 0;
            this.toolStripLiveData.Text = "toolStrip1";
            // 
            // toolStripButtonScan
            // 
            this.toolStripButtonScan.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonScan.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonScan.Image = global::smartTracker.Properties.Resources.radio_single;
            this.toolStripButtonScan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonScan.Name = "toolStripButtonScan";
            this.toolStripButtonScan.Size = new System.Drawing.Size(71, 32);
            this.toolStripButtonScan.Text = "Scan";
            this.toolStripButtonScan.ToolTipText = "Scan - Press F2 to scan!";
            this.toolStripButtonScan.Click += new System.EventHandler(this.toolStripButtonScan_Click);
            // 
            // toolStripButtonStopScan
            // 
            this.toolStripButtonStopScan.Enabled = false;
            this.toolStripButtonStopScan.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonStopScan.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonStopScan.Image = global::smartTracker.Properties.Resources.StopIcon;
            this.toolStripButtonStopScan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStopScan.Name = "toolStripButtonStopScan";
            this.toolStripButtonStopScan.Size = new System.Drawing.Size(108, 32);
            this.toolStripButtonStopScan.Text = "Stop Scan";
            this.toolStripButtonStopScan.Click += new System.EventHandler(this.toolStripButtonStopScan_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // toolStripButtonExit
            // 
            this.toolStripButtonExit.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.treeViewDevice.Size = new System.Drawing.Size(296, 204);
            this.treeViewDevice.TabIndex = 0;
            this.treeViewDevice.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDevice_AfterCollapse);
            this.treeViewDevice.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDevice_AfterExpand);
            this.treeViewDevice.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDevice_AfterSelect);
            this.treeViewDevice.DoubleClick += new System.EventHandler(this.treeViewDevice_DoubleClick);
            this.treeViewDevice.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeViewDevice_KeyDown);
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
            this.grouptimageList.Images.SetKeyName(7, "ban_added");
            this.grouptimageList.Images.SetKeyName(8, "ban_present");
            this.grouptimageList.Images.SetKeyName(9, "ban_removed");
            // 
            // contextMenuStripReader
            // 
            this.contextMenuStripReader.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextMenuStripReader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.calibrateToolStripMenuItem,
            this.findThresholdToolStripMenuItem,
            this.tagSetToolStripMenuItem,
            this.conversionToolStripMenuItem,
            this.doorLightToolStripMenuItem,
            this.renewFingerprintToolStripMenuItem});
            this.contextMenuStripReader.Name = "contextMenuStripReader";
            this.contextMenuStripReader.Size = new System.Drawing.Size(195, 136);
            // 
            // calibrateToolStripMenuItem
            // 
            this.calibrateToolStripMenuItem.Name = "calibrateToolStripMenuItem";
            this.calibrateToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.calibrateToolStripMenuItem.Text = "&Calibrate";
            this.calibrateToolStripMenuItem.Click += new System.EventHandler(this.calibrateToolStripMenuItem_Click);
            // 
            // findThresholdToolStripMenuItem
            // 
            this.findThresholdToolStripMenuItem.Name = "findThresholdToolStripMenuItem";
            this.findThresholdToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.findThresholdToolStripMenuItem.Text = "&Find Threshold";
            this.findThresholdToolStripMenuItem.Click += new System.EventHandler(this.findThresholdToolStripMenuItem_Click);
            // 
            // tagSetToolStripMenuItem
            // 
            this.tagSetToolStripMenuItem.Name = "tagSetToolStripMenuItem";
            this.tagSetToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.tagSetToolStripMenuItem.Text = "&TagSet";
            this.tagSetToolStripMenuItem.Click += new System.EventHandler(this.tagSetToolStripMenuItem_Click);
            // 
            // conversionToolStripMenuItem
            // 
            this.conversionToolStripMenuItem.Name = "conversionToolStripMenuItem";
            this.conversionToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.conversionToolStripMenuItem.Text = "C&onversion";
            this.conversionToolStripMenuItem.Click += new System.EventHandler(this.conversionToolStripMenuItem_Click);
            // 
            // doorLightToolStripMenuItem
            // 
            this.doorLightToolStripMenuItem.Name = "doorLightToolStripMenuItem";
            this.doorLightToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.doorLightToolStripMenuItem.Text = "&Door && Light";
            this.doorLightToolStripMenuItem.Click += new System.EventHandler(this.doorLightToolStripMenuItem_Click);
            // 
            // renewFingerprintToolStripMenuItem
            // 
            this.renewFingerprintToolStripMenuItem.Name = "renewFingerprintToolStripMenuItem";
            this.renewFingerprintToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.renewFingerprintToolStripMenuItem.Text = "&Renew Fingerprint";
            this.renewFingerprintToolStripMenuItem.Click += new System.EventHandler(this.renewFingerprintToolStripMenuItem_Click_1);
            // 
            // timerStartup
            // 
            this.timerStartup.Tick += new System.EventHandler(this.timerStartup_Tick);
            // 
            // timerRestart
            // 
            this.timerRestart.Enabled = true;
            this.timerRestart.Interval = 5000;
            this.timerRestart.Tick += new System.EventHandler(this.timerRestart_Tick);
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
            this.splitContainer1.Panel2.Controls.Add(this.dataGridViewScan);
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(1037, 539);
            this.splitContainer1.SplitterDistance = 208;
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
            this.splitContainer2.Panel2.Controls.Add(this.pictureBoxAdded);
            this.splitContainer2.Panel2.Controls.Add(this.pictureBoxRemoved);
            this.splitContainer2.Panel2.Controls.Add(this.labelInventoryTagCount);
            this.splitContainer2.Panel2.Controls.Add(this.labelRemoved);
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox2);
            this.splitContainer2.Panel2.Controls.Add(this.labelInventoryUser);
            this.splitContainer2.Panel2.Controls.Add(this.labelAdded);
            this.splitContainer2.Panel2.Controls.Add(this.labelInventoryDate);
            this.splitContainer2.Panel2.Controls.Add(this.listBoxTag);
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer2.Panel2.Controls.Add(this.pictureBoxPresent);
            this.splitContainer2.Panel2.Controls.Add(this.labelPresent);
            this.splitContainer2.Size = new System.Drawing.Size(1037, 208);
            this.splitContainer2.SplitterDistance = 300;
            this.splitContainer2.SplitterIncrement = 5;
            this.splitContainer2.TabIndex = 0;
            // 
            // pictureBoxAdded
            // 
            this.pictureBoxAdded.BackgroundImage = global::smartTracker.Properties.Resources.banVert;
            this.pictureBoxAdded.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxAdded.Location = new System.Drawing.Point(329, 21);
            this.pictureBoxAdded.Name = "pictureBoxAdded";
            this.pictureBoxAdded.Size = new System.Drawing.Size(234, 50);
            this.pictureBoxAdded.TabIndex = 41;
            this.pictureBoxAdded.TabStop = false;
            this.pictureBoxAdded.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxAdded_Paint);
            // 
            // pictureBoxRemoved
            // 
            this.pictureBoxRemoved.BackgroundImage = global::smartTracker.Properties.Resources.banrouge;
            this.pictureBoxRemoved.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxRemoved.Location = new System.Drawing.Point(329, 135);
            this.pictureBoxRemoved.Name = "pictureBoxRemoved";
            this.pictureBoxRemoved.Size = new System.Drawing.Size(234, 50);
            this.pictureBoxRemoved.TabIndex = 42;
            this.pictureBoxRemoved.TabStop = false;
            this.pictureBoxRemoved.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxRemoved_Paint);
            // 
            // labelInventoryTagCount
            // 
            this.labelInventoryTagCount.AutoSize = true;
            this.labelInventoryTagCount.BackColor = System.Drawing.Color.Transparent;
            this.labelInventoryTagCount.Font = new System.Drawing.Font("Verdana", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInventoryTagCount.ForeColor = System.Drawing.Color.Black;
            this.labelInventoryTagCount.Location = new System.Drawing.Point(11, 136);
            this.labelInventoryTagCount.Name = "labelInventoryTagCount";
            this.labelInventoryTagCount.Size = new System.Drawing.Size(191, 45);
            this.labelInventoryTagCount.TabIndex = 29;
            this.labelInventoryTagCount.Text = "Tag(s): 0";
            // 
            // labelRemoved
            // 
            this.labelRemoved.AutoSize = true;
            this.labelRemoved.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRemoved.ForeColor = System.Drawing.Color.Red;
            this.labelRemoved.Location = new System.Drawing.Point(362, 138);
            this.labelRemoved.Name = "labelRemoved";
            this.labelRemoved.Size = new System.Drawing.Size(164, 25);
            this.labelRemoved.TabIndex = 35;
            this.labelRemoved.Text = "REMOVED : 0";
            // 
            // pictureBox2
            // 
            this.pictureBox2.ErrorImage = null;
            this.pictureBox2.Image = global::smartTracker.Properties.Resources.date_heure;
            this.pictureBox2.Location = new System.Drawing.Point(19, 23);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(45, 48);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 38;
            this.pictureBox2.TabStop = false;
            // 
            // labelInventoryUser
            // 
            this.labelInventoryUser.BackColor = System.Drawing.Color.Transparent;
            this.labelInventoryUser.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInventoryUser.ForeColor = System.Drawing.Color.Black;
            this.labelInventoryUser.Location = new System.Drawing.Point(70, 81);
            this.labelInventoryUser.Name = "labelInventoryUser";
            this.labelInventoryUser.Size = new System.Drawing.Size(253, 48);
            this.labelInventoryUser.TabIndex = 28;
            this.labelInventoryUser.Text = "Inventory User :";
            this.labelInventoryUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelAdded
            // 
            this.labelAdded.AutoSize = true;
            this.labelAdded.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAdded.ForeColor = System.Drawing.Color.Green;
            this.labelAdded.Location = new System.Drawing.Point(362, 35);
            this.labelAdded.Name = "labelAdded";
            this.labelAdded.Size = new System.Drawing.Size(130, 25);
            this.labelAdded.TabIndex = 34;
            this.labelAdded.Text = "ADDED : 0";
            // 
            // labelInventoryDate
            // 
            this.labelInventoryDate.AutoSize = true;
            this.labelInventoryDate.BackColor = System.Drawing.Color.Transparent;
            this.labelInventoryDate.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInventoryDate.ForeColor = System.Drawing.Color.Black;
            this.labelInventoryDate.Location = new System.Drawing.Point(70, 42);
            this.labelInventoryDate.Name = "labelInventoryDate";
            this.labelInventoryDate.Size = new System.Drawing.Size(153, 18);
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
            this.listBoxTag.Location = new System.Drawing.Point(569, 23);
            this.listBoxTag.MultiColumn = true;
            this.listBoxTag.Name = "listBoxTag";
            this.listBoxTag.Size = new System.Drawing.Size(148, 160);
            this.listBoxTag.TabIndex = 30;
            // 
            // pictureBox1
            // 
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Image = global::smartTracker.Properties.Resources.user_accounts;
            this.pictureBox1.Location = new System.Drawing.Point(19, 77);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(45, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 37;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBoxPresent
            // 
            this.pictureBoxPresent.BackgroundImage = global::smartTracker.Properties.Resources.ban_bleu;
            this.pictureBoxPresent.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxPresent.Location = new System.Drawing.Point(329, 78);
            this.pictureBoxPresent.Name = "pictureBoxPresent";
            this.pictureBoxPresent.Size = new System.Drawing.Size(234, 50);
            this.pictureBoxPresent.TabIndex = 40;
            this.pictureBoxPresent.TabStop = false;
            this.pictureBoxPresent.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxPresent_Paint);
            // 
            // labelPresent
            // 
            this.labelPresent.AutoSize = true;
            this.labelPresent.BackColor = System.Drawing.Color.Transparent;
            this.labelPresent.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPresent.ForeColor = System.Drawing.Color.Blue;
            this.labelPresent.Location = new System.Drawing.Point(362, 83);
            this.labelPresent.Name = "labelPresent";
            this.labelPresent.Size = new System.Drawing.Size(155, 25);
            this.labelPresent.TabIndex = 36;
            this.labelPresent.Text = "PRESENT : 0";
            this.labelPresent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.dataGridViewScan.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewScan.MultiSelect = false;
            this.dataGridViewScan.Name = "dataGridViewScan";
            this.dataGridViewScan.ReadOnly = true;
            this.dataGridViewScan.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewScan.Size = new System.Drawing.Size(1033, 301);
            this.dataGridViewScan.TabIndex = 1;
            this.dataGridViewScan.SelectionChanged += new System.EventHandler(this.dataGridViewScan_SelectionChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 301);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1033, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelInfo
            // 
            this.toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
            this.toolStripStatusLabelInfo.Size = new System.Drawing.Size(50, 17);
            this.toolStripStatusLabelInfo.Text = "Info : ";
            // 
            // LiveDataForDeviceForm
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
            this.Name = "LiveDataForDeviceForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Live Data For Device";
            this.Activated += new System.EventHandler(this.LiveDataForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LiveDataForm_FormClosing);
            this.Load += new System.EventHandler(this.LiveDataForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LiveDataForm_KeyDown);
            this.toolStripLiveData.ResumeLayout(false);
            this.toolStripLiveData.PerformLayout();
            this.contextMenuStripReader.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdded)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRemoved)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPresent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewScan)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripLiveData;
        private System.Windows.Forms.ToolStripButton toolStripButtonScan;
        private System.Windows.Forms.Timer timerStartup;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        private System.Windows.Forms.Timer timerRestart;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripReader;
        private System.Windows.Forms.ToolStripMenuItem calibrateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findThresholdToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem conversionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tagSetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem doorLightToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialogImage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Timer timerObjectList;
        private System.Windows.Forms.ImageList grouptimageList;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.DataGridView dataGridViewScan;
        private System.Windows.Forms.ToolStripMenuItem renewFingerprintToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelInfo;
        private System.Windows.Forms.ToolStripButton toolStripButtonStopScan;
        private System.Windows.Forms.TreeView treeViewDevice;
        private System.Windows.Forms.PictureBox pictureBoxAdded;
        private System.Windows.Forms.PictureBox pictureBoxRemoved;
        private System.Windows.Forms.Label labelInventoryTagCount;
        private System.Windows.Forms.Label labelRemoved;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label labelInventoryUser;
        private System.Windows.Forms.Label labelAdded;
        private System.Windows.Forms.Label labelInventoryDate;
        private System.Windows.Forms.ListBox listBoxTag;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBoxPresent;
        private System.Windows.Forms.Label labelPresent;
        

    }
}