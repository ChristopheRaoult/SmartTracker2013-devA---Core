using smartTracker.Properties;

namespace smartTracker
{
    partial class ItemListForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemListForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStripItemListForm = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonGet = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonImport = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonFillGrid = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonApply = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.dataGridViewImport = new System.Windows.Forms.DataGridView();
            this.ImageImport = new System.Windows.Forms.DataGridViewImageColumn();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.overwriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeProductInImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addImageFromLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadImageFromLotIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addProductToListToFindByLedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printAssociateTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labeTag = new System.Windows.Forms.Label();
            this.textBoxTagUID = new System.Windows.Forms.TextBox();
            this.openFileDialogXLS = new System.Windows.Forms.OpenFileDialog();
            this.textBoxLotID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbAutoFill = new System.Windows.Forms.CheckBox();
            this.txtConstant = new System.Windows.Forms.TextBox();
            this.txtVar = new System.Windows.Forms.TextBox();
            this.groupBoxAutoFill = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.openFileDialogImage = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.folderBrowserDialogImage = new System.Windows.Forms.FolderBrowserDialog();
            this.saveXlsFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolStripItemListForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewImport)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.groupBoxAutoFill.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripItemListForm
            // 
            this.toolStripItemListForm.AutoSize = false;
            this.toolStripItemListForm.BackColor = System.Drawing.Color.White;
            this.toolStripItemListForm.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripItemListForm.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripItemListForm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonGet,
            this.toolStripButtonImport,
            this.toolStripButtonExport,
            this.toolStripSeparator1,
            this.toolStripButtonFillGrid,
            this.toolStripButtonApply,
            this.toolStripButtonExit});
            this.toolStripItemListForm.Location = new System.Drawing.Point(0, 0);
            this.toolStripItemListForm.Name = "toolStripItemListForm";
            this.toolStripItemListForm.Size = new System.Drawing.Size(666, 35);
            this.toolStripItemListForm.TabIndex = 0;
            this.toolStripItemListForm.Text = "toolStrip1";
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
            // toolStripButtonImport
            // 
            this.toolStripButtonImport.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonImport.Image = global::smartTracker.Properties.Resources.exportXLS;
            this.toolStripButtonImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonImport.Name = "toolStripButtonImport";
            this.toolStripButtonImport.Size = new System.Drawing.Size(86, 32);
            this.toolStripButtonImport.Text = "Import";
            this.toolStripButtonImport.Click += new System.EventHandler(this.toolStripButtonImport_Click);
            // 
            // toolStripButtonExport
            // 
            this.toolStripButtonExport.Image = global::smartTracker.Properties.Resources.exportXLS;
            this.toolStripButtonExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExport.Name = "toolStripButtonExport";
            this.toolStripButtonExport.Size = new System.Drawing.Size(84, 32);
            this.toolStripButtonExport.Text = global::smartTracker.Properties.ResStrings.str_Export;
            this.toolStripButtonExport.Click += new System.EventHandler(this.toolStripButtonExport_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // toolStripButtonFillGrid
            // 
            this.toolStripButtonFillGrid.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonFillGrid.Image = global::smartTracker.Properties.Resources._23138_bubka_Application;
            this.toolStripButtonFillGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFillGrid.Name = "toolStripButtonFillGrid";
            this.toolStripButtonFillGrid.Size = new System.Drawing.Size(90, 32);
            this.toolStripButtonFillGrid.Text = "Fill Grid";
            this.toolStripButtonFillGrid.Click += new System.EventHandler(this.toolStripButtonFillGrid_Click);
            // 
            // toolStripButtonApply
            // 
            this.toolStripButtonApply.ForeColor = System.Drawing.Color.Black;
            this.toolStripButtonApply.Image = global::smartTracker.Properties.Resources.apply;
            this.toolStripButtonApply.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonApply.Name = "toolStripButtonApply";
            this.toolStripButtonApply.Size = new System.Drawing.Size(77, 32);
            this.toolStripButtonApply.Text = "Apply";
            this.toolStripButtonApply.Click += new System.EventHandler(this.toolStripButtonApply_Click);
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
            // dataGridViewImport
            // 
            this.dataGridViewImport.AllowUserToAddRows = false;
            this.dataGridViewImport.AllowUserToDeleteRows = false;
            this.dataGridViewImport.AllowUserToResizeColumns = false;
            this.dataGridViewImport.AllowUserToResizeRows = false;
            this.dataGridViewImport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewImport.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewImport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewImport.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImageImport});
            this.dataGridViewImport.ContextMenuStrip = this.contextMenuStrip;
            this.dataGridViewImport.Location = new System.Drawing.Point(9, 138);
            this.dataGridViewImport.Name = "dataGridViewImport";
            this.dataGridViewImport.RowTemplate.Height = 75;
            this.dataGridViewImport.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewImport.Size = new System.Drawing.Size(646, 350);
            this.dataGridViewImport.TabIndex = 1;
            this.dataGridViewImport.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewImport_CellEndEdit);
            this.dataGridViewImport.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridViewImport_CellFormatting);
            this.dataGridViewImport.SelectionChanged += new System.EventHandler(this.dataGridViewImport_SelectionChanged);
            this.dataGridViewImport.Sorted += new System.EventHandler(this.dataGridViewImport_Sorted);
            // 
            // ImageImport
            // 
            this.ImageImport.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle1.NullValue")));
            this.ImageImport.DefaultCellStyle = dataGridViewCellStyle1;
            this.ImageImport.FillWeight = 200F;
            this.ImageImport.HeaderText = "Image";
            this.ImageImport.MinimumWidth = 200;
            this.ImageImport.Name = "ImageImport";
            this.ImageImport.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ImageImport.Width = 200;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.overwriteToolStripMenuItem,
            this.removeProductInImportToolStripMenuItem,
            this.addImageToolStripMenuItem,
            this.addImageFromLibraryToolStripMenuItem,
            this.loadImageFromLotIDToolStripMenuItem,
            this.removeImageToolStripMenuItem,
            this.imageListToolStripMenuItem,
            this.addProductToListToFindByLedToolStripMenuItem,
            this.printAssociateTagToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(304, 224);
            // 
            // overwriteToolStripMenuItem
            // 
            this.overwriteToolStripMenuItem.Image = global::smartTracker.Properties.Resources.icon_overwrite;
            this.overwriteToolStripMenuItem.Name = "overwriteToolStripMenuItem";
            this.overwriteToolStripMenuItem.Size = new System.Drawing.Size(303, 22);
            this.overwriteToolStripMenuItem.Text = "Overwrite";
            this.overwriteToolStripMenuItem.Click += new System.EventHandler(this.overwriteToolStripMenuItem_Click);
            // 
            // removeProductInImportToolStripMenuItem
            // 
            this.removeProductInImportToolStripMenuItem.Image = global::smartTracker.Properties.Resources.edit_clear;
            this.removeProductInImportToolStripMenuItem.Name = "removeProductInImportToolStripMenuItem";
            this.removeProductInImportToolStripMenuItem.Size = new System.Drawing.Size(303, 22);
            this.removeProductInImportToolStripMenuItem.Text = "Remove Product in Import";
            this.removeProductInImportToolStripMenuItem.Click += new System.EventHandler(this.removeProductInImportToolStripMenuItem_Click);
            // 
            // addImageToolStripMenuItem
            // 
            this.addImageToolStripMenuItem.Image = global::smartTracker.Properties.Resources._23158_bubka_ImageCapture;
            this.addImageToolStripMenuItem.Name = "addImageToolStripMenuItem";
            this.addImageToolStripMenuItem.Size = new System.Drawing.Size(303, 22);
            this.addImageToolStripMenuItem.Text = "Add Image from File";
            this.addImageToolStripMenuItem.Click += new System.EventHandler(this.addImageToolStripMenuItem_Click);
            // 
            // addImageFromLibraryToolStripMenuItem
            // 
            this.addImageFromLibraryToolStripMenuItem.Image = global::smartTracker.Properties.Resources._23158_bubka_ImageCapture;
            this.addImageFromLibraryToolStripMenuItem.Name = "addImageFromLibraryToolStripMenuItem";
            this.addImageFromLibraryToolStripMenuItem.Size = new System.Drawing.Size(303, 22);
            this.addImageFromLibraryToolStripMenuItem.Text = "Add Image from Library";
            this.addImageFromLibraryToolStripMenuItem.Click += new System.EventHandler(this.addImageFromLibraryToolStripMenuItem_Click);
            // 
            // loadImageFromLotIDToolStripMenuItem
            // 
            this.loadImageFromLotIDToolStripMenuItem.Image = global::smartTracker.Properties.Resources._23158_bubka_ImageCapture;
            this.loadImageFromLotIDToolStripMenuItem.Name = "loadImageFromLotIDToolStripMenuItem";
            this.loadImageFromLotIDToolStripMenuItem.Size = new System.Drawing.Size(303, 22);
            this.loadImageFromLotIDToolStripMenuItem.Text = "Load Image from LotID";
            this.loadImageFromLotIDToolStripMenuItem.Click += new System.EventHandler(this.loadImageFromLotIDToolStripMenuItem_Click);
            // 
            // removeImageToolStripMenuItem
            // 
            this.removeImageToolStripMenuItem.Image = global::smartTracker.Properties.Resources._23178_bubka_TrashEmpty;
            this.removeImageToolStripMenuItem.Name = "removeImageToolStripMenuItem";
            this.removeImageToolStripMenuItem.Size = new System.Drawing.Size(303, 22);
            this.removeImageToolStripMenuItem.Text = "Remove Image";
            this.removeImageToolStripMenuItem.Click += new System.EventHandler(this.removeImageToolStripMenuItem_Click);
            // 
            // imageListToolStripMenuItem
            // 
            this.imageListToolStripMenuItem.Image = global::smartTracker.Properties.Resources._23150_bubka_FolderPhoto;
            this.imageListToolStripMenuItem.Name = "imageListToolStripMenuItem";
            this.imageListToolStripMenuItem.Size = new System.Drawing.Size(303, 22);
            this.imageListToolStripMenuItem.Text = "Image List";
            this.imageListToolStripMenuItem.Click += new System.EventHandler(this.imageListToolStripMenuItem_Click);
            // 
            // addProductToListToFindByLedToolStripMenuItem
            // 
            this.addProductToListToFindByLedToolStripMenuItem.Image = global::smartTracker.Properties.Resources.button_red_on;
            this.addProductToListToFindByLedToolStripMenuItem.Name = "addProductToListToFindByLedToolStripMenuItem";
            this.addProductToListToFindByLedToolStripMenuItem.Size = new System.Drawing.Size(303, 22);
            this.addProductToListToFindByLedToolStripMenuItem.Text = "Add Product to List To find By Led";
            this.addProductToListToFindByLedToolStripMenuItem.Visible = false;
            this.addProductToListToFindByLedToolStripMenuItem.Click += new System.EventHandler(this.addProductToListToFindByLedToolStripMenuItem_Click);
            // 
            // printAssociateTagToolStripMenuItem
            // 
            this.printAssociateTagToolStripMenuItem.Image = global::smartTracker.Properties.Resources.print;
            this.printAssociateTagToolStripMenuItem.Name = "printAssociateTagToolStripMenuItem";
            this.printAssociateTagToolStripMenuItem.Size = new System.Drawing.Size(303, 22);
            this.printAssociateTagToolStripMenuItem.Text = "Print && Associate Tag";
            this.printAssociateTagToolStripMenuItem.Click += new System.EventHandler(this.printAssociateTagToolStripMenuItem_Click);
            // 
            // labeTag
            // 
            this.labeTag.AutoSize = true;
            this.labeTag.ForeColor = System.Drawing.Color.Black;
            this.labeTag.Location = new System.Drawing.Point(23, 55);
            this.labeTag.Name = "labeTag";
            this.labeTag.Size = new System.Drawing.Size(62, 13);
            this.labeTag.TabIndex = 2;
            this.labeTag.Text = "Tag UID :";
            // 
            // textBoxTagUID
            // 
            this.textBoxTagUID.Location = new System.Drawing.Point(85, 52);
            this.textBoxTagUID.Name = "textBoxTagUID";
            this.textBoxTagUID.Size = new System.Drawing.Size(143, 21);
            this.textBoxTagUID.TabIndex = 3;
            this.textBoxTagUID.Click += new System.EventHandler(this.textBoxTagUID_Click);
            this.textBoxTagUID.TextChanged += new System.EventHandler(this.textBoxTagUID_TextChanged);
            this.textBoxTagUID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxTagUID_KeyUp);
            this.textBoxTagUID.Leave += new System.EventHandler(this.textBoxTagUID_Leave);
            // 
            // openFileDialogXLS
            // 
            this.openFileDialogXLS.Filter = "Excel 97 - 2003 File (*.xls)|*.xls|Excel 2010 File (*.xlsx)|*.xlsx|CSV File (*.cs" +
    "v)|*.csv";
            this.openFileDialogXLS.FilterIndex = 2;
            this.openFileDialogXLS.Title = "Import XLS File";
            // 
            // textBoxLotID
            // 
            this.textBoxLotID.Location = new System.Drawing.Point(85, 79);
            this.textBoxLotID.Name = "textBoxLotID";
            this.textBoxLotID.Size = new System.Drawing.Size(143, 21);
            this.textBoxLotID.TabIndex = 5;
            this.textBoxLotID.Click += new System.EventHandler(this.textBoxLotID_Click);
            this.textBoxLotID.TextChanged += new System.EventHandler(this.textBoxLotID_TextChanged);
            this.textBoxLotID.Leave += new System.EventHandler(this.textBoxLotID_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(35, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Lot ID :";
            // 
            // cbAutoFill
            // 
            this.cbAutoFill.AutoSize = true;
            this.cbAutoFill.Location = new System.Drawing.Point(131, 16);
            this.cbAutoFill.Name = "cbAutoFill";
            this.cbAutoFill.Size = new System.Drawing.Size(67, 17);
            this.cbAutoFill.TabIndex = 6;
            this.cbAutoFill.Text = "AutoFill";
            this.cbAutoFill.UseVisualStyleBackColor = true;
            // 
            // txtConstant
            // 
            this.txtConstant.Location = new System.Drawing.Point(101, 36);
            this.txtConstant.Name = "txtConstant";
            this.txtConstant.Size = new System.Drawing.Size(132, 21);
            this.txtConstant.TabIndex = 7;
            this.txtConstant.Text = "Tag";
            // 
            // txtVar
            // 
            this.txtVar.Location = new System.Drawing.Point(101, 61);
            this.txtVar.Name = "txtVar";
            this.txtVar.Size = new System.Drawing.Size(132, 21);
            this.txtVar.TabIndex = 8;
            this.txtVar.Text = "1501";
            // 
            // groupBoxAutoFill
            // 
            this.groupBoxAutoFill.Controls.Add(this.label3);
            this.groupBoxAutoFill.Controls.Add(this.label2);
            this.groupBoxAutoFill.Controls.Add(this.txtVar);
            this.groupBoxAutoFill.Controls.Add(this.txtConstant);
            this.groupBoxAutoFill.Controls.Add(this.cbAutoFill);
            this.groupBoxAutoFill.Location = new System.Drawing.Point(410, 38);
            this.groupBoxAutoFill.Name = "groupBoxAutoFill";
            this.groupBoxAutoFill.Size = new System.Drawing.Size(244, 94);
            this.groupBoxAutoFill.TabIndex = 9;
            this.groupBoxAutoFill.TabStop = false;
            this.groupBoxAutoFill.Text = "Auto Fill Feature";
            this.groupBoxAutoFill.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Variable :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Constant :";
            // 
            // openFileDialogImage
            // 
            this.openFileDialogImage.FileName = "openFileDialogImage";
            this.openFileDialogImage.Filter = "\"Image Files|*.jpg;*.gif;*.bmp;*.png;*.jpeg";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 491);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(666, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelInfo
            // 
            this.toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
            this.toolStripStatusLabelInfo.Size = new System.Drawing.Size(37, 17);
            this.toolStripStatusLabelInfo.Text = "Info : ";
            this.toolStripStatusLabelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // folderBrowserDialogImage
            // 
            this.folderBrowserDialogImage.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialogImage.ShowNewFolderButton = false;
            // 
            // saveXlsFileDialog
            // 
            this.saveXlsFileDialog.Filter = "Excel 2007 -  2013 File (*.xlsx)|*.xlsx";
            this.saveXlsFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveXlsFileDialog_FileOk);
            // 
            // ItemListForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(666, 513);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.textBoxLotID);
            this.Controls.Add(this.groupBoxAutoFill);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxTagUID);
            this.Controls.Add(this.labeTag);
            this.Controls.Add(this.dataGridViewImport);
            this.Controls.Add(this.toolStripItemListForm);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ItemListForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Item List";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ItemListForm_FormClosing);
            this.Load += new System.EventHandler(this.ItemListForm_Load);
            this.toolStripItemListForm.ResumeLayout(false);
            this.toolStripItemListForm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewImport)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.groupBoxAutoFill.ResumeLayout(false);
            this.groupBoxAutoFill.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripItemListForm;
        private System.Windows.Forms.ToolStripButton toolStripButtonGet;
        private System.Windows.Forms.ToolStripButton toolStripButtonImport;
        private System.Windows.Forms.ToolStripButton toolStripButtonApply;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        public  System.Windows.Forms.DataGridView dataGridViewImport;
        private System.Windows.Forms.Label labeTag;
        private System.Windows.Forms.TextBox textBoxTagUID;
        private System.Windows.Forms.OpenFileDialog openFileDialogXLS;
        private System.Windows.Forms.TextBox textBoxLotID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem overwriteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeProductInImportToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbAutoFill;
        private System.Windows.Forms.TextBox txtConstant;
        private System.Windows.Forms.TextBox txtVar;
        private System.Windows.Forms.GroupBox groupBoxAutoFill;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewImageColumn ImageImport;
        private System.Windows.Forms.ToolStripMenuItem addImageToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialogImage;
        private System.Windows.Forms.ToolStripMenuItem removeImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonFillGrid;
        private System.Windows.Forms.ToolStripMenuItem imageListToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelInfo;
        private System.Windows.Forms.ToolStripMenuItem addImageFromLibraryToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem loadImageFromLotIDToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogImage;
        private System.Windows.Forms.ToolStripButton toolStripButtonExport;
        private System.Windows.Forms.SaveFileDialog saveXlsFileDialog;
        private System.Windows.Forms.ToolStripMenuItem addProductToListToFindByLedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printAssociateTagToolStripMenuItem;
    }
}