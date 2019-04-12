using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Drawing.Drawing2D;
using DBClass;
using DataClass;
using ErrorMessage;
using OfficeOpenXml.Style;
using SDK_SC_RFID_Devices;
using OfficeOpenXml;
using BrightIdeasSoftware;
using smartTracker.LIB;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class LiveDataViewerForm : Form
    {
        delegate void MethodInvoker();
        #region Variables

        readonly MainDBClass _db = new MainDBClass();


        DataTable _dtGroup;
        readonly BoxTagInfo _bti = null;

        CompareInventoryForm _cptInvForm = null;
        volatile deviceClass[] _localDeviceArray = null;       
        UserClassTemplate[] _localUserArray = null;  

       
        private int _selectedReader = -1;
        volatile private bool _bClosing = false; 

        private int _timeTcpToRefresh = 10000;

        private double _timeZoneOffset = 0.0;
        private int _nbRecordToKeep = 1000;
        private int _selIndex = -1;
        private bool _bStoreTagEvent = false;
        public DataTable DtProductRef;
        public bool BRestart = false;
        private bool _bCompareOnLotId = true;
        private bool _bUseAlarm = false;

        public bool BViewerMode = false;
        public bool BShowImage = true;
        public bool BShowAlert = true;
        private readonly bool[] _treeviewExpandArray = new bool[100];

        Hashtable _columnInfo = null;
            
        ImageList _treeImageList = null;    
       
        private readonly object _locker = new object();

        FormuleData _formule = null;
        bool _runMacro = false;
        private int _rowDlcDate = -1;
        readonly string[] _formats = { "dd/MM/yyyy", "dd/MM/yy", "dd-MM-yyyy", "dd-MM-yy", "dd MM yyyy", "dd MM yy" };
        readonly string _columnExpiredDate = string.Empty;
      

        #endregion
        #region group
        private void UpdateGroup()
        {
            _dtGroup = new DataTable();

            _dtGroup = _db.RecoverAllGroup();
            _dtGroup.Columns[0].ColumnName = ResStrings.str_TagUID;
            _dtGroup.Columns[1].ColumnName = ResStrings.str_Group_Reference;
            _dtGroup.Columns[2].ColumnName = ResStrings.str_Group_Description;
            _dtGroup.Columns[3].ColumnName = ResStrings.str_Criteria;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_bti != null)
            {
                _bti.Criteria = txtCriteria.Text;
                CheckCriteria();
            }
        }
        #endregion
        #region datagridScanHistory
        DataTable _tbReaderScan;
        InventoryData[] _inventoryArray;

        private void InitReaderScanTable()
        {
            _tbReaderScan = new DataTable();
            _tbReaderScan.Columns.Add(ResStrings.str_EventDate, typeof(string));
            _tbReaderScan.Columns.Add(ResStrings.str_Serial_RFID, typeof(string));
            _tbReaderScan.Columns.Add(ResStrings.str_Reader_Name, typeof(string));            
            _tbReaderScan.Columns.Add(ResStrings.str_First_Name, typeof(string));
            _tbReaderScan.Columns.Add(ResStrings.str_Last_Name, typeof(string));
            _tbReaderScan.Columns.Add(ResStrings.str_All, typeof(int));
            _tbReaderScan.Columns.Add(ResStrings.str_Present, typeof(int));
            _tbReaderScan.Columns.Add(ResStrings.str_Added, typeof(int));
            _tbReaderScan.Columns.Add(ResStrings.str_Removed, typeof(int));
            dataGridViewScan.DataSource = null;
            dataGridViewScan.DataSource = _tbReaderScan.DefaultView;

        }
        private void ProcessData(DeviceInfo di)
        {
            InitReaderScanTable();

            InventoryData[] invData = _db.GetInventory(di, null , 10);
            if (invData == null) return;
            _inventoryArray = new InventoryData[invData.Length];
            invData.CopyTo(_inventoryArray, 0);
            foreach (InventoryData dt in _inventoryArray)
            {
                try
                {
                    DeviceInfo tmpdi = _db.RecoverDevice(dt.serialNumberDevice);
                    _tbReaderScan.Rows.Add(dt.eventDate.ToString("G"), dt.serialNumberDevice, tmpdi.DeviceName,
                        dt.userFirstName, dt.userLastName,dt.nbTagAll, dt.nbTagPresent, 
                        dt.nbTagAdded, dt.nbTagRemoved);
                }
                catch
                {

                }
            }
            dataGridViewScan.DataSource = null;
            dataGridViewScan.DataSource = _tbReaderScan.DefaultView;          
        }

        private void UpdateScanHistory(DeviceInfo di)
        {
            if (di == null)
            {
                if (_selectedReader == -1) return;

                if (_selectedReader >= 0)  //reader local
                {
                    if (_localDeviceArray == null) return;
                    di = _localDeviceArray[_selectedReader].infoDev;
                }
            }
            ProcessData(di);
        }

        private void dataGridViewScan_SelectionChanged(object sender, EventArgs e)
        {
            if (tabControlInfo.SelectedIndex == 1)
            {
                if (dataGridViewScan.SelectedRows.Count == 1)
                {
                    int selectedData = dataGridViewScan.SelectedRows[0].Index;

                    if (_selectedReader == -1) return;

                    if (_selectedReader >= 0)  //reader local
                    {
                        if (_localDeviceArray == null) return;
                        _localDeviceArray[_selectedReader].currentInventory = _inventoryArray[selectedData];

                    }
                    labelInventoryDate.Invoke((MethodInvoker)delegate { labelInventoryDate.Text = _inventoryArray[selectedData].eventDate.ToLocalTime().ToString("G"); });
                    labelInventoryUser.Invoke((MethodInvoker)delegate { labelInventoryUser.Text = string.Format("{0} {1}", _inventoryArray[selectedData].userFirstName, _inventoryArray[selectedData].userLastName); });
                    labelInventoryTagCount.Invoke((MethodInvoker)delegate { labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags, _inventoryArray[selectedData].dtTagAll.Rows.Count); });

                    timerObjectList.Start();               
                }
            }
        }

        #endregion
        #region Formule
        public void LoadFormule()
        {
            _formule = _db.getFormuleInfo();            
        }
        #endregion
        #region LiveDataWindows
        public LiveDataViewerForm()
        {
            InitializeComponent();  

        }
        private void LiveDataForm_Load(object sender, EventArgs e)
        {
            try
            {
                timerObjectList.Stop();                
                tagListDataGrid.Visible = false;
                _db.OpenDB();
                _columnInfo = _db.GetColumnInfo();
                LoadFormule();
                UpdateGroup();               

                int.TryParse(ConfigurationManager.AppSettings["NbRecordToKeep"], out _nbRecordToKeep);
                int.TryParse(ConfigurationManager.AppSettings["timeTcpToRefresh"], out _timeTcpToRefresh);
                bool.TryParse(ConfigurationManager.AppSettings["bStoreEventTag"], out _bStoreTagEvent);
                bool.TryParse(ConfigurationManager.AppSettings["bCompareOnLotID"], out _bCompareOnLotId);
                bool.TryParse(ConfigurationManager.AppSettings["bShowImage"], out BShowImage);
                bool.TryParse(ConfigurationManager.AppSettings["bShowAlert"], out BShowAlert);
                bool.TryParse(ConfigurationManager.AppSettings["bUseAlarm"], out _bUseAlarm);
                bool.TryParse(ConfigurationManager.AppSettings["bViewerMode"], out BViewerMode);
                
                BViewerMode = true;              

                ConfigureList();


                string valTime = ConfigurationManager.AppSettings["TimeZoneOffset"];

                CultureInfo culture = valTime.Contains(".") ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;

                // Conversion de la chaîne en double, en utilisant la culture correspondante.            
                double.TryParse(valTime, NumberStyles.Number, culture, out _timeZoneOffset);             



                _treeImageList = new ImageList();
                _treeImageList.Images.Add(new Bitmap(Resources.button_yellow_off));
                _treeImageList.Images.Add(new Bitmap(Resources.button_green_on));
                _treeImageList.Images.Add(new Bitmap(Resources.button_red_on));
                _treeImageList.Images.Add(new Bitmap(Resources.refresh));

                treeViewDevice.ImageList = _treeImageList;

                for (int i = 0; i < 100; i++) _treeviewExpandArray[i] = false;

                copyLotIDToClipBoardToolStripMenuItem.Text = string.Format(ResStrings.str_Copy_to_ClipBoard, _columnInfo[1]);
                GetProduct();
                CreateDevice();                             
                UpdateTreeView();
                UpdateScanHistory(null);

                //dateFridgeALarmBlocked = DateTime.Now;               

                if (treeViewDevice.Nodes.Count > 0)
                    _selectedReader = 0;                

                UserClassTemplate[] users = _db.RecoverUser();
                if (users != null)
                {
                    _localUserArray = new UserClassTemplate[users.Length];
                    users.CopyTo(_localUserArray, 0);
                }

                timerStartup.Enabled = true;
                timerStartup.Start();
                _bClosing = false;

            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }

        }
        private void LiveDataForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
              
                _bClosing = true;
                timerStartup.Enabled = false;               
                _db.CloseDB();      
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ExceptionMessageBox.Show(exp);
            }
        }
     
        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            _bClosing = true;
            BeginInvoke((MethodInvoker)delegate { Close(); });
        }
        private void toolStripButExportXLS_Click(object sender, EventArgs e)
        {
            _runMacro = false;
            if (_selectedReader >= 0)  //reader local
            {
                if (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus != DeviceStatus.DS_InScan)
                {
                    try
                    {
                        ExportToExcel();
                    }
                    catch (Exception exp)
                    {
                        ExceptionMessageBox.Show(exp);
                    }
                }
                else
                    MessageBox.Show(ResStrings.str_Unable_to_Export_Device_In_Scan, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    ExportToExcel();
                }
                catch (Exception exp)
                {
                    ExceptionMessageBox.Show(exp);
                }
            }


        }
        private void exportAndRunExcelMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _runMacro = true;
            if (_selectedReader >= 0)  //reader local
            {
                if (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus != DeviceStatus.DS_InScan)
                {
                    try
                    {
                        ExportToExcel();
                    }
                    catch (Exception exp)
                    {
                        ExceptionMessageBox.Show(exp);
                    }
                }
                else
                    MessageBox.Show(ResStrings.str_Unable_to_Export_Device_In_Scan, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    ExportToExcel();
                }
                catch (Exception exp)
                {
                    ExceptionMessageBox.Show(exp);
                }
            }
        }
        private void toolStripCompare_Click(object sender, EventArgs e)
        {
            if (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus != DeviceStatus.DS_InScan)
            {
                _cptInvForm = new CompareInventoryForm(_localDeviceArray[_selectedReader].currentInventory, _bCompareOnLotId);
                _cptInvForm.Show();
            }
            else
                MessageBox.Show(ResStrings.str_Unable_to_Compare_Device_in_Scan, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void toolStripButtonRemoveLink_Click(object sender, EventArgs e)
        {
            int nbToDelete = listBoxTag.Items.Count;
            int nbDelete = 0;
            DialogResult res = MessageBox.Show(this, string.Format(ResStrings.strk_Are_You_sure_you_want_to_remove_the_association, nbToDelete), ResStrings.str_Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (res == DialogResult.Yes)
            {

                for (int loop = 0; loop < listBoxTag.Items.Count; loop++)
                {
                    if (_db.DeleteProduct(listBoxTag.Items[loop].ToString())) nbDelete++;
                }
                MessageBox.Show(this, string.Format(ResStrings.str_product_deleted, nbDelete, nbToDelete), ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                GetProduct();
            }


        }            
        private void LiveDataForm_Activated(object sender, EventArgs e)
        {
            UpdateGroup();
        }
        #endregion
        #region treeview
        private void UpdateTreeView()
        {
            try
            {
                if (_bClosing) return;

                if (treeViewDevice.InvokeRequired)
                {
                    treeViewDevice.Invoke(new MethodInvoker(UpdateTreeView));
                }
                else
                {
                    if (treeViewDevice.SelectedNode != null)
                        _selIndex = treeViewDevice.SelectedNode.Index;
                    treeViewDevice.Nodes.Clear();
                    if (_localDeviceArray != null)
                    {
                        foreach (deviceClass dc in _localDeviceArray)
                        {
                            TreeNode serialnode = new TreeNode(string.Format(ResStrings.str_SerialSN, dc.infoDev.SerialRFID));
                            serialnode.StateImageIndex = 0;
                            TreeNode typeNode = new TreeNode(string.Format(ResStrings.str_Type, dc.infoDev.deviceType));                          

                            TreeNode[] ndArray = null;
                            ndArray = new[] { serialnode, typeNode };
                            
                            TreeNode fullNode = new TreeNode(dc.infoDev.DeviceName + "      ", ndArray);
                            fullNode.NodeFont = new Font(treeViewDevice.Font, FontStyle.Bold);
                            if (BViewerMode)
                                fullNode.ImageIndex = fullNode.SelectedImageIndex = 3;
                            else if (dc.rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected)
                                fullNode.ImageIndex = fullNode.SelectedImageIndex = 1;
                            else
                                fullNode.ImageIndex = fullNode.SelectedImageIndex = 2;
                            int rg = treeViewDevice.Nodes.Add(fullNode);

                            if (_treeviewExpandArray[rg]) treeViewDevice.Nodes[rg].Expand();
                            else treeViewDevice.Nodes[rg].Collapse();
                            
                        }
                        if (_bClosing)
                        {
                            _localDeviceArray = null;                       
                            Close();
                       }
                    }
                   
                    if (_selIndex >= 0)
                        treeViewDevice.SelectedNode = treeViewDevice.Nodes[_selIndex];
                    treeViewDevice.Refresh();
                }

            }
            catch
            {

            }

        }
        private void treeViewDevice_AfterSelect(object sender, TreeViewEventArgs e)
        {

            if (treeViewDevice.SelectedNode != null)
            {
                if (treeViewDevice.SelectedNode.Parent != null)
                {
                    TreeNode parNode = treeViewDevice.SelectedNode.Parent;
                    treeViewDevice.SelectedNode = parNode;
                    return;
                }
                _selectedReader = treeViewDevice.SelectedNode.Index;
            }
            else
            {
                if (treeViewDevice.Nodes.Count > 0)
                {
                    TreeNode parNode = treeViewDevice.Nodes[0];
                    treeViewDevice.SelectedNode = parNode;
                    _selectedReader = 0;
                }
            }
            treeViewDevice.ContextMenuStrip = null;
            dataListView.Items.Clear();
            RefreshInventory();
            UpdateScanHistory(null);

        }
        private void treeViewDevice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D && e.Modifiers == Keys.Control)
            {
                if (treeViewDevice.ContextMenuStrip == null)
                    treeViewDevice.ContextMenuStrip = contextMenuStripReader;
                else
                    treeViewDevice.ContextMenuStrip = null;
            }
        }
        private void treeViewDevice_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            _treeviewExpandArray[e.Node.Index] = false;
        }
        private void treeViewDevice_AfterExpand(object sender, TreeViewEventArgs e)
        {
            _treeviewExpandArray[e.Node.Index] = true;
        }
        private void treeViewDevice_DoubleClick(object sender, EventArgs e)
        {
            UpdateTreeView();          
        }
       
       
        #endregion
        #region Alert
       
        private double ProcessValueAlert(DataTable dt, string colName)
        {
            double sumValue = 0.0;

            foreach (DataRow oRow in dt.Rows)
            {
                string strValue = oRow[colName].ToString();
                strValue = strValue.Replace(" ", "");
                strValue = strValue.Replace("'", "");

                if (strValue.Contains("."))
                {
                    double tmpVal;
                    if (double.TryParse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture, out tmpVal))
                        sumValue += tmpVal;
                }
                else
                {
                    double tmpVal;
                    if (double.TryParse(strValue, NumberStyles.Number, CultureInfo.CurrentCulture, out tmpVal))
                        sumValue += tmpVal;
                }
            }
            return sumValue;
        }
        #endregion
        #region dataList
        public void ConfigureList()
        {
            // cretate datatable for column
            DataTable newDt = new DataTable();
            newDt.Columns.Add(ResStrings.str_Event, typeof(string));

            for (int i = 0; i < _columnInfo.Count; i++)
            {
                newDt.Columns.Add(_columnInfo[i].ToString(), typeof(string));
            }            
            newDt.Columns.Add(ResStrings.str_Tag_Location, typeof(string));
            newDt.Columns.Add(ResStrings.str_Expiration, typeof(string));
            dataListView.DataSource = null;
            dataListView.DataSource = newDt;

            for (int i = 0; i < _columnInfo.Count; i++)
            {
                dataListView.Columns[i + 1].Name = _columnInfo[i].ToString();
                dataListView.Columns[i + 1].Text = _columnInfo[i].ToString();
            }
            

            //empty List

            dataListView.EmptyListMsg = "";   
            TextOverlay textOverlay = dataListView.EmptyListMsgOverlay as TextOverlay;
            if (textOverlay != null)
            {
                textOverlay.TextColor = Color.Firebrick;
                textOverlay.BackColor = Color.AntiqueWhite;
                textOverlay.BorderColor = Color.DarkRed;
                textOverlay.BorderWidth = 4.0f;
                textOverlay.Font = new Font("Chiller", 36);
                textOverlay.Rotation = -5;
            }


            for (int i = 0; i < dataListView.Columns.Count; i++)
            {
                OLVColumn ol = dataListView.GetColumn(i);
                //ol.FillsFreeSpace = true;
                ol.HeaderFont = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
                ol.HeaderForeColor = Color.AliceBlue;
                ol.IsTileViewColumn = true;
                ol.UseInitialLetterForGroup = false;
                ol.MinimumWidth = 20 + ol.Text.Length * 10;

            }

            if (BShowImage)
            {
                if (dataListView.UseTranslucentHotItem)
                {
                    if (_columnInfo.Count < 20) // ie : block card if number of colum too big for display info
                    {
                        dataListView.HotItemStyle.Overlay = new BusinessCardOverlay(_columnInfo.Count);
                        dataListView.HotItemStyle = dataListView.HotItemStyle;
                    }
                    else
                    {
                        RowBorderDecoration rbd = new RowBorderDecoration();
                        rbd.BorderPen = new Pen(Color.SeaGreen, 2);
                        rbd.FillBrush = null;
                        rbd.CornerRounding = 4.0f;
                        HotItemStyle hotItemStyle2 = new HotItemStyle();
                        hotItemStyle2.Decoration = rbd;
                        dataListView.HotItemStyle = hotItemStyle2;
                    }
                }
            }
            else
            {

                RowBorderDecoration rbd = new RowBorderDecoration();
                rbd.BorderPen = new Pen(Color.SeaGreen, 2);
                rbd.FillBrush = null;
                rbd.CornerRounding = 4.0f;
                HotItemStyle hotItemStyle2 = new HotItemStyle();
                hotItemStyle2.Decoration = rbd;
                dataListView.HotItemStyle = hotItemStyle2;
            }           
       

            // Install a custom renderer that draws the Tile view in a special way
            dataListView.ItemRenderer = new BusinessCardRenderer();
            dataListView.Invalidate();

        }
        internal class BusinessCardOverlay : AbstractOverlay
        {
            readonly int _nbItem = 5;
            public BusinessCardOverlay(int nbItem)
            {
                businessCardRenderer.HeaderBackBrush = Brushes.DarkBlue;
                businessCardRenderer.BorderPen = new Pen(Color.DarkBlue, 2);
                Transparency = 255;
                _nbItem = nbItem;
            }
            #region IOverlay Members

            public override void Draw(ObjectListView olv, Graphics g, Rectangle r)
            {
                if (olv.HotRowIndex < 0)
                    return;

                if (olv.View == View.Tile)
                    return;

                OLVListItem item = olv.GetItem(olv.HotRowIndex);
                if (item == null)
                    return;

                Size cardSize = new Size(320, 50 + (_nbItem * 12));
                Rectangle cardBounds = new Rectangle(
                    r.Right - cardSize.Width - 8, r.Bottom - cardSize.Height - 8, cardSize.Width, cardSize.Height);
                businessCardRenderer.DrawBusinessCard(g, cardBounds, item.RowObject, olv, item);
                
            }

            #endregion

            private BusinessCardRenderer businessCardRenderer = new BusinessCardRenderer();
        }
        public static Image GetImage(string lotId)
        {

            Image imgFromDb = Resources.No_Image_Available;
            MainDBClass dbTemp = new MainDBClass();

            dbTemp.OpenDB();
            if (dbTemp.IsImageLinkExist(lotId))
            {
                string imgName = dbTemp.getImageNameLink(lotId);
                if (imgName != null)
                {
                    imgFromDb = ImageUtils.byteArrayToImage(dbTemp.getImage(imgName));
                }
            }
            dbTemp.CloseDB();
            return imgFromDb;
        }
        /// <summary>
        /// Hackish renderer that draw a fancy version of a person for a Tile view.
        /// </summary>
        /// <remarks>This is not the way to write a professional level renderer.
        /// It is hideously inefficient (we should at least cache the images),
        /// but it is obvious</remarks>
        internal class BusinessCardRenderer : AbstractRenderer
        {
            public override bool RenderItem(DrawListViewItemEventArgs e, Graphics g, Rectangle itemBounds, object rowObject)
            {
                // If we're in any other view than Tile, return false to say that we haven't done
                // the rendereing and the default process should do it's stuff
                ObjectListView olv = e.Item.ListView as ObjectListView;
                if (olv == null || olv.View != View.Tile)
                    return false;

                // Use buffered graphics to kill flickers
                BufferedGraphics buffered = BufferedGraphicsManager.Current.Allocate(g, itemBounds);
                g = buffered.Graphics;
                g.Clear(olv.BackColor);
                g.SmoothingMode = ObjectListView.SmoothingMode;
                g.TextRenderingHint = ObjectListView.TextRenderingHint;

                if (e.Item.Selected)
                {
                    BorderPen = Pens.Blue;
                    HeaderBackBrush = new SolidBrush(olv.HighlightBackgroundColorOrDefault);
                }
                else
                {
                    BorderPen = new Pen(Color.FromArgb(0x33, 0x33, 0x33));
                    HeaderBackBrush = new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33));
                }
                DrawBusinessCard(g, itemBounds, rowObject, olv, (OLVListItem)e.Item);

                // Finally render the buffered graphics
                buffered.Render();
                buffered.Dispose();

                // Return true to say that we've handled the drawing
                return true;
            }

            internal Pen BorderPen = new Pen(Color.FromArgb(0x33, 0x33, 0x33));
            internal Brush TextBrush = new SolidBrush(Color.FromArgb(0x22, 0x22, 0x22));
            internal Brush HeaderTextBrush = Brushes.AliceBlue;
            internal Brush HeaderBackBrush = new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33));
            internal Brush BackBrush = Brushes.LemonChiffon;

            public void DrawBusinessCard(Graphics g, Rectangle itemBounds, object rowObject, ObjectListView olv, OLVListItem item)
            {
                const int spacing = 8;

                // Allow a border around the card
                itemBounds.Inflate(-2, -2);

                // Draw card background
                const int rounding = 20;
                GraphicsPath path = GetRoundedRect(itemBounds, rounding);
                g.FillPath(BackBrush, path);
                g.DrawPath(BorderPen, path);
                
                g.Clip = new Region(itemBounds);
               
                // Draw the photo
                Rectangle photoRect = itemBounds;
                photoRect.Inflate(-spacing, -spacing);

                photoRect.Width = 80;
                OLVColumn columnLotId = olv.GetColumn(2);

                Image photo = GetImage(columnLotId.GetStringValue(rowObject));
                if (photo.Width > photoRect.Width)
                    photoRect.Height = (int)(photo.Height * ((float)photoRect.Width / photo.Width));
                else
                    photoRect.Height = photo.Height;
                g.DrawImage(photo, photoRect);




                // Now draw the text portion
                RectangleF textBoxRect = photoRect;
                
                textBoxRect.X += (photoRect.Width + spacing);
                textBoxRect.Width = itemBounds.Right - textBoxRect.X - spacing;

                StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap);
                fmt.Trimming = StringTrimming.EllipsisCharacter;
                fmt.Alignment = StringAlignment.Center;
                fmt.LineAlignment = StringAlignment.Near;
                //String txt = item.Text;
                String txt = columnLotId.GetStringValue(rowObject);

                using (Font font = new Font("Tahoma", 11))
                {
                    // Measure the height of the title
                    SizeF size = g.MeasureString(txt, font, (int)textBoxRect.Width, fmt);
                    // Draw the title
                    RectangleF r3 = textBoxRect;
                    r3.Height = size.Height;
                    path = GetRoundedRect(r3, 15);
                    g.FillPath(HeaderBackBrush, path);
                    g.DrawString(txt, font, HeaderTextBrush, textBoxRect, fmt);                    
                    textBoxRect.Y += size.Height + spacing;
                }

                // Draw the other bits of information
                using (Font font = new Font("Tahoma", 8))
                {
                    SizeF size = g.MeasureString("Wj", font, itemBounds.Width, fmt);
                    textBoxRect.Height = size.Height;
                    fmt.Alignment = StringAlignment.Near;
                    for (int i = 3; i < olv.Columns.Count; i++)
                    {
                        OLVColumn column = olv.GetColumn(i);
                        if (!column.IsTileViewColumn) continue;
                        txt = column.AspectName + " : " + column.GetStringValue(rowObject);
                        g.DrawString(txt, font, TextBrush, textBoxRect, fmt);
                        textBoxRect.Y += size.Height;
                    }
                }
            }


            private GraphicsPath GetRoundedRect(RectangleF rect, float diameter)
            {
                GraphicsPath path = new GraphicsPath();

                RectangleF arc = new RectangleF(rect.X, rect.Y, diameter, diameter);
                path.AddArc(arc, 180, 90);
                arc.X = rect.Right - diameter;
                path.AddArc(arc, 270, 90);
                arc.Y = rect.Bottom - diameter;
                path.AddArc(arc, 0, 90);
                arc.X = rect.Left;
                path.AddArc(arc, 90, 90);
                path.CloseFigure();

                return path;
            }
        }
        private void timerObjectList_Tick(object sender, EventArgs e)
        {
            timerObjectList.Stop();
            try
            {
                if (!string.IsNullOrEmpty(_columnExpiredDate))
                {
                    for (int i = 0; i < dataListView.Columns.Count; i++)
                    {
                        OLVColumn ol = dataListView.GetColumn(i);
                        if (ol.Name == _columnExpiredDate)
                        {
                            _rowDlcDate = i;
                        }
                    }
                }

                if (_selectedReader >= 0)  //reader local
                {
                    if (_localDeviceArray == null) return;
                    lock (_locker)
                    {
                        if (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus == DeviceStatus.DS_InScan)
                        {
                            dataListView.Items.Clear();
                            return;
                        }

                        DataTable newDt = new DataTable();
                        newDt.Columns.Add(ResStrings.str_Event, typeof(string));

                        for (int i = 0; i < _columnInfo.Count; i++)
                        {
                            newDt.Columns.Add(_columnInfo[i].ToString(), typeof(string));
                        }
                        
                        newDt.Columns.Add(ResStrings.str_Tag_Location, typeof(string));
                        newDt.Columns.Add(ResStrings.str_Expiration, typeof(string));                    
               
                        foreach (DataRow dr in _localDeviceArray[_selectedReader].currentInventory.dtTagAdded.Rows)
                        {
                            DataRow rowToadd = newDt.NewRow();

                            rowToadd[0] = ResStrings.str_Added;
                            for (int i = 0; i < dr.ItemArray.Length; i++)
                                rowToadd[i + 1] = dr.ItemArray[i];

                            if ((_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SFR) ||
                                (_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SBF))
                                if (_localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel.Contains(dr.ItemArray[0]))
                                {
                                    string tmpInfo = string.Format(ResStrings.str_Shelf, _localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel[dr.ItemArray[0]]);
                                    rowToadd[dr.ItemArray.Length + 1] = tmpInfo;
                                }

                            if (!string.IsNullOrEmpty(_columnExpiredDate))
                            {
                                string date = (string)dr.ItemArray[_rowDlcDate - 1];
                                DateTime dt;
                                if (DateTime.TryParseExact(date, _formats, CultureInfo.CurrentUICulture, DateTimeStyles.None, out dt))
                                {
                                    DateTime now = DateTime.Now;
                                    TimeSpan elapsed = dt.Subtract(now);
                                    double daysAgo = elapsed.TotalDays;

                                    if (daysAgo < 0)
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Expired;
                                    else if ((daysAgo >= 0) & (daysAgo < 30))
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_30_Days_Left;
                                    else if ((daysAgo >= 31) & (daysAgo < 90))
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_90_Days_Left;
                                    else if (daysAgo >= 91)
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_More_than_90_Days_Left;

                                }
                                else
                                {
                                    rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_No_Date;
                                }
                            }

                            newDt.Rows.Add(rowToadd);
                        }
                       
                        foreach (DataRow dr in _localDeviceArray[_selectedReader].currentInventory.dtTagRemove.Rows)
                        {
                            
                            DataRow rowToadd = newDt.NewRow();

                            rowToadd[0] = ResStrings.str_Removed;
                            for (int i = 0; i < dr.ItemArray.Length; i++)
                                rowToadd[i + 1] = dr.ItemArray[i];
                            if ((_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SFR) ||
                                (_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SBF))
                                if (_localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel.Contains(dr.ItemArray[0]))
                                {
                                    string tmpInfo = string.Format(ResStrings.str_Shelf, _localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel[dr.ItemArray[0]]);
                                    rowToadd[dr.ItemArray.Length + 1] = tmpInfo;
                                }
                            if (!string.IsNullOrEmpty(_columnExpiredDate))
                            {
                                string date = (string)dr.ItemArray[_rowDlcDate - 1];
                                DateTime dt;
                                if (DateTime.TryParseExact(date, _formats, CultureInfo.CurrentUICulture, DateTimeStyles.None, out dt))
                                {
                                    DateTime now = DateTime.Now;
                                    TimeSpan elapsed = dt.Subtract(now);
                                    double daysAgo = elapsed.TotalDays;

                                    if (daysAgo < 0)
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Expired;
                                    else if ((daysAgo >= 0) & (daysAgo < 30))
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_30_Days_Left;
                                    else if ((daysAgo >= 31) & (daysAgo < 90))
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_90_Days_Left;
                                    else if (daysAgo >= 91)
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_More_than_90_Days_Left;

                                }
                                else
                                {
                                    rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_No_Date;
                                }
                            }
                            
                            newDt.Rows.Add(rowToadd);
                              
                                                     

                        }
                        foreach (DataRow dr in _localDeviceArray[_selectedReader].currentInventory.dtTagPresent.Rows)
                        {
                            DataRow rowToadd = newDt.NewRow();
                            rowToadd[0] = ResStrings.str_Present;
                            for (int i = 0; i < dr.ItemArray.Length; i++)
                                rowToadd[i + 1] = dr.ItemArray[i];
                            if ((_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SFR) ||
                               (_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SBF))
                                if (_localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel.Contains(dr.ItemArray[0]))
                                {
                                    string tmpInfo = string.Format(ResStrings.str_Shelf, _localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel[dr.ItemArray[0]]);
                                    rowToadd[dr.ItemArray.Length + 1] = tmpInfo;
                                }

                            if (!string.IsNullOrEmpty(_columnExpiredDate))
                            {
                                string date = (string)dr.ItemArray[_rowDlcDate - 1];
                                DateTime dt;
                                if (DateTime.TryParseExact(date, _formats, CultureInfo.CurrentUICulture, DateTimeStyles.None, out dt))
                                {
                                    DateTime now = DateTime.Now;
                                    TimeSpan elapsed = dt.Subtract(now);
                                    double daysAgo = elapsed.TotalDays;

                                    if (daysAgo < 0)
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Expired;
                                    else if ((daysAgo >= 0) & (daysAgo < 30))
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_30_Days_Left;
                                    else if ((daysAgo >= 31) & (daysAgo < 90))
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_90_Days_Left;
                                    else if (daysAgo >= 91)
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_More_than_90_Days_Left;

                                }
                                else
                                {
                                    rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_No_Date;
                                }
                            }
                            newDt.Rows.Add(rowToadd);
                        }
                        dataListView.DataSource = null;
                        dataListView.DataSource = newDt;

                    }

                    bool needUpdate = false;
                    bool previousState;

                    OLVColumn aColumn = dataListView.AllColumns[dataListView.AllColumns.Count - 2];
                    previousState = aColumn.IsVisible;
                    aColumn.IsVisible = false;
                    if ((_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SFR) ||
                               (_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SBF))
                        aColumn.IsVisible = true;

                    if (aColumn.IsVisible != previousState) needUpdate = true;

                    OLVColumn bColumn = dataListView.AllColumns[dataListView.AllColumns.Count - 1];
                    previousState = bColumn.IsVisible;
                    if (!string.IsNullOrEmpty(_columnExpiredDate))
                        bColumn.IsVisible = true;
                    else
                        bColumn.IsVisible = false;
                    if (bColumn.IsVisible != previousState) needUpdate = true;
                    if (needUpdate) dataListView.RebuildColumns();
                  
                    Invoke((MethodInvoker)delegate { labelPresent.Text = string.Format(ResStrings.str_PRESENT_format, _localDeviceArray[_selectedReader].currentInventory.dtTagPresent.Rows.Count); });
                    Invoke((MethodInvoker)delegate { labelAdded.Text = string.Format(ResStrings.str_ADDED_format, _localDeviceArray[_selectedReader].currentInventory.dtTagAdded.Rows.Count); });
                    Invoke((MethodInvoker)delegate { labelRemoved.Text = string.Format(ResStrings.str_REMOVED_format, _localDeviceArray[_selectedReader].currentInventory.dtTagRemove.Rows.Count); });
                 
                    for (int i = 0; i < dataListView.Columns.Count; i++)
                    {
                        OLVColumn ol = dataListView.GetColumn(i);
                        ol.Width = 25;
                        ol.FillsFreeSpace = false;
                        ol.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                        if (ol.Index == dataListView.Columns.Count - 1)
                            ol.FillsFreeSpace = true;

                    }
                }               


                if ((_bti != null) && (!string.IsNullOrEmpty(_bti.Criteria)))
                    CheckCriteria();
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }

        }
        private void dataListView_AfterCreatingGroups(object sender, CreateGroupsEventArgs e)
        {
            if ((dataListView == null) || (dataListView.OLVGroups == null)) return;
            //OLVGroup grp = dataListView.OLVGroups[0];
            //string headerValue = grp.Header;
            //if ((grp.Header.Contains("Present") || (grp.Header.Contains("Added")) || (grp.Header.Contains("Removed"))))
            if (e.Parameters.GroupByColumn.AspectName.Equals(ResStrings.str_Event))
            {
                foreach (ListViewItem lvi in dataListView.Items)
                {
                    string strCase = lvi.SubItems[0].Text;
                    if (strCase.Equals(ResStrings.str_Present)) lvi.ForeColor = Color.Blue;
                    else if (strCase.Equals(ResStrings.str_Added)) lvi.ForeColor = Color.Green;
                    else if (strCase.Equals(ResStrings.str_Removed)) lvi.ForeColor = Color.Red;
                    else lvi.ForeColor = Color.Black;
                }
            }
            else if (e.Parameters.GroupByColumn.AspectName.Equals(ResStrings.str_Expiration))
            {
                foreach (ListViewItem lvi in dataListView.Items)
                {
                    string strCase = lvi.SubItems[lvi.SubItems.Count - 1].Text;

                    if (strCase.Equals(ResStrings.str_Less_than_30_Days_Left)) lvi.ForeColor = Color.OrangeRed;
                    else if (strCase.Equals(ResStrings.str_Less_than_90_Days_Left)) lvi.ForeColor = Color.Blue;
                    else if (strCase.Equals(ResStrings.str_More_than_90_Days_Left)) lvi.ForeColor = Color.Green;
                    else if (strCase.Equals(ResStrings.str_Expired)) lvi.ForeColor = Color.Red;
                    else lvi.ForeColor = Color.Black;
                }
            }


            foreach (OLVGroup grp2 in dataListView.OLVGroups)
                grp2.Collapsed = true;
            dataListView.Refresh();
            
        }
        private void dataListView_BeforeCreatingGroups(object sender, CreateGroupsEventArgs e)
        {
            dataListView.GetColumn(ResStrings.str_Event).MakeGroupies(
                new object[] { ResStrings.str_Added, ResStrings.str_Present, ResStrings.str_Removed },
                new[] { "Rien", ResStrings.str_Added, ResStrings.str_Present, ResStrings.str_Removed },
                new object[] { "not", "add", "present", "remove" },
                new[] { "", GetSumValue(2), GetSumValue(1), GetSumValue(3) },
                new[] { "Rien", ResStrings.str_Tag_s__added_at_last_scan, ResStrings.Livstr_Tag_s__already_present_in_previous_scan, ResStrings.str_Tag_s__removed_from_previous_scan }
            );  

            if (!string.IsNullOrEmpty(_columnExpiredDate))
            {
                dataListView.GetColumn(ResStrings.str_Expiration).MakeGroupies(
                    new[] { ResStrings.str_Expired,ResStrings.str_Less_than_30_Days_Left, ResStrings.str_Less_than_90_Days_Left, ResStrings.str_More_than_90_Days_Left, ResStrings.str_No_Date },
                    new[] { "Rien",ResStrings.str_Expired,ResStrings.str_Less_than_30_Days_Left, ResStrings.str_Between_30_to_90_Days_Left, ResStrings.str_More_than_90_Days_Left,ResStrings.str_No_Date },
                    new object[] { "not","expired","attention-1mois","attention-3mois","attention","not"}                   
                    );
             }
            
        }

        private void dataListView_HotItemChanged(object sender, HotItemChangedEventArgs e)
        {
            if ((dataListView == null) || (dataListView.OLVGroups == null)) return;
            OLVGroup grp = dataListView.OLVGroups[0];

            if ((grp.Header.Contains(ResStrings.str_Present) || (grp.Header.Contains(ResStrings.str_Added)) || (grp.Header.Contains(ResStrings.str_Removed))))
            {
                foreach (ListViewItem lvi in dataListView.Items)
                {
                    string strCase = lvi.SubItems[0].Text;
                    if (strCase.Equals(ResStrings.str_Present)) lvi.ForeColor = Color.Blue;
                    else if (strCase.Equals(ResStrings.str_Added)) lvi.ForeColor = Color.Green;
                    else if (strCase.Equals(ResStrings.str_Removed)) lvi.ForeColor = Color.Red;
                    else lvi.ForeColor = Color.Black;
                }
            }
            else
            {
                foreach (ListViewItem lvi in dataListView.Items)
                {
                    string strCase = lvi.SubItems[lvi.SubItems.Count - 1].Text;

                    if (strCase.Equals(ResStrings.str_Less_than_30_Days_Left)) lvi.ForeColor = Color.OrangeRed;
                    else if (strCase.Equals(ResStrings.str_Less_than_90_Days_Left)) lvi.ForeColor = Color.Blue;
                    else if (strCase.Equals(ResStrings.str_More_than_90_Days_Left)) lvi.ForeColor = Color.Green;
                    else if (strCase.Equals(ResStrings.str_Expired)) lvi.ForeColor = Color.Red;
                    else lvi.ForeColor = Color.Black;
                }
            }   
        }
        private void dataListView_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            if (dataListView.SelectedItems.Count > 0)
            {
                ListViewItem item = dataListView.SelectedItems[0];
                string tagClicked = item.SubItems[1].Text;

                if (!string.IsNullOrEmpty(tagClicked))
                {
                    ItemHistoryForm ihf = new ItemHistoryForm(tagClicked, null);
                    ihf.MdiParent = ParentForm;
                    ihf.WindowState = FormWindowState.Maximized;
                    ihf.Show();
                }
            }
        }
        private void toolStripDropDownButtonView_Click(object sender, EventArgs e)
        {
            dataListView.TileSize = new Size(250, 50 + (_columnInfo.Count * 12));
            if (toolStripDropDownButtonView.Text == ResStrings.str_Display_in_Tile)
            {
                toolStripDropDownButtonView.Text = ResStrings.str_Display_in_Details;
                dataListView.View = View.Tile;
            }
            else
            {
                toolStripDropDownButtonView.Text = ResStrings.str_Display_in_Tile;
                dataListView.View = View.Details;
            }
        }
        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataListView.OLVGroups.Count; i++)
            {
                OLVGroup grp = dataListView.OLVGroups[i];
                grp.Collapsed = true;

            }
        }
        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            dataListView.ShowGroups = !dataListView.ShowGroups;
            toolStripSplitButton1.Text = (dataListView.ShowGroups) ? ResStrings.str_Group_Mode___OFF : ResStrings.str_Group_Mode__ON;
            timerObjectList.Start();
        }      
        #endregion
        #region Device   

        private void CreateDevice()
        {
            try
            {
                if (_bClosing) return;
                if (_db.getUsedDBType == MainDBClass.dbUsedType.db_SqlLite)
                {
                    DeviceInfo[] tmpDeviceArray = _db.RecoverAllDevice();                    
                    if (tmpDeviceArray != null)
                    {
                        _localDeviceArray = new deviceClass[tmpDeviceArray.Length];
                        for (int nIndex = 0; nIndex < tmpDeviceArray.Length; nIndex++)
                        {
                            
                            _localDeviceArray[nIndex] = new deviceClass(_columnInfo);
                            _localDeviceArray[nIndex].infoDev = new DeviceInfo();
                            _localDeviceArray[nIndex].infoDev = tmpDeviceArray[nIndex];
                            _localDeviceArray[nIndex].rfidDev = new RFID_Device();

                            InventoryData tmpInv = _db.GetLastScan(_localDeviceArray[nIndex].infoDev.SerialRFID);
                            if (tmpInv != null)
                            {
                                _localDeviceArray[nIndex].currentInventory = tmpInv;
                                _localDeviceArray[nIndex].lastProcessInventoryGmtDate = TruncateMs(tmpInv.eventDate.ToUniversalTime());
                            }
                        }
                    }
                }
                else if (_db.getUsedDBType == MainDBClass.dbUsedType.db_SqlServer)
                {
                    ArrayList listSerial = _db.RecoverDistinctSerialRFID();
                    if ((listSerial != null) && (listSerial.Count > 0))
                    {
                        _localDeviceArray = new deviceClass[listSerial.Count];

                        for (int nIndex = 0; nIndex < listSerial.Count; nIndex++)
                        {
                            _localDeviceArray[nIndex] = new deviceClass(_columnInfo);
                            _localDeviceArray[nIndex].infoDev = new DeviceInfo();
                            _localDeviceArray[nIndex].infoDev = _db.RecoverDevice(listSerial[nIndex].ToString());
                            _localDeviceArray[nIndex].rfidDev = new RFID_Device();

                            InventoryData tmpInv = _db.GetLastScan(_localDeviceArray[nIndex].infoDev.SerialRFID);
                            if (tmpInv != null)
                            {
                                _localDeviceArray[nIndex].currentInventory = tmpInv;
                                _localDeviceArray[nIndex].lastProcessInventoryGmtDate = TruncateMs(tmpInv.eventDate.ToUniversalTime());
                            }
                        }
                    }
                }

               
               
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ExceptionMessageBox.Show(exp);
            }
        }

        private void timerStartup_Tick(object sender, EventArgs e)
        {

          
                timerStartup.Interval = 5000;
                timerStartup.Enabled = true;
                timerStartup.Stop();

                bool bNeedRefresh = false;

                if (_localDeviceArray != null)
                foreach (deviceClass di in _localDeviceArray)
                {
                    InventoryData inv = _db.GetLastScan(di.infoDev.SerialRFID);
                    if (inv == null) continue;
                    if (inv.eventDate > di.currentInventory.eventDate)
                    {
                        di.currentInventory = inv;
                        di.lastProcessInventoryGmtDate = TruncateMs(inv.eventDate.ToUniversalTime());
                        bNeedRefresh = true;
                    }                    
                }
               

                if (bNeedRefresh) RefreshInventory();
                timerStartup.Start();          
            

        }
       
        #endregion
        #region inventory
        public void GetProduct()
        {
            if (_bClosing) return;
            if (_db != null)
            {
                DtProductRef = _db.RecoverAllProduct();
            }
        }   
        public string GetSumValue(int selectedDt)
        {
            try
            {
                lock (_locker)
                {
                    MathParser mp = new MathParser();
                    int nIndex = 0;
                    dtColumnInfo[] listCol = _db.GetdtColumnInfo();
                    if (listCol == null) return " ";
                    string upStatus = string.Empty;
                   
                    foreach (dtColumnInfo col in listCol)
                    {
                        if (col.colDoSum == 1)
                        {
                            if (!string.IsNullOrEmpty(col.colName))
                            {
                                double sumValue = 0.0;
                                if (selectedDt == 0)
                                    sumValue = ProcessValueAlert(_localDeviceArray[_selectedReader].currentInventory.dtTagAll, col.colName);
                                else if (selectedDt == 1)
                                    sumValue = ProcessValueAlert(_localDeviceArray[_selectedReader].currentInventory.dtTagPresent, col.colName);
                                else if (selectedDt == 2)
                                    sumValue = ProcessValueAlert(_localDeviceArray[_selectedReader].currentInventory.dtTagAdded, col.colName);
                                else if (selectedDt == 3)
                                    sumValue = ProcessValueAlert(_localDeviceArray[_selectedReader].currentInventory.dtTagRemove, col.colName);

                                upStatus += string.Format(ResStrings.str_sum, col.colName, sumValue.ToString("0.00"));
                                mp.Parameters.Add((Parameters)nIndex++, (decimal)sumValue);
                            }
                        }
                    }
                    

                    if (_formule != null)
                    {
                        if (_formule.Enable == 1)
                        {                            
                            decimal result = mp.Calculate(_formule.Formule);
                            upStatus += _formule.Title + result.ToString("0.00");
                        }
                    }

                    return upStatus;
                }
            }

            catch 
            {               
                return " ";
            }
        }

        private void RefreshInventory()
        {
            Invoke((MethodInvoker)delegate
            {
                if (treeViewDevice.SelectedNode != null)
                {
                    if (treeViewDevice.SelectedNode.Parent != null)
                    {
                        TreeNode parNode = treeViewDevice.SelectedNode.Parent;
                        treeViewDevice.SelectedNode = parNode;
                        return;
                    }
                    _selectedReader = treeViewDevice.SelectedNode.Index;
                }
                else
                {
                    if (treeViewDevice.Nodes.Count > 0)
                    {
                        TreeNode parNode = treeViewDevice.Nodes[0];
                        treeViewDevice.SelectedNode = parNode;
                        _selectedReader = 0;
                    }
                }
            });

            if (_localDeviceArray != null)
            {
                if (_selectedReader >= 0)  //reader local
                { 
                    if (_localDeviceArray[_selectedReader].currentInventory != null)
                    {
                        lock (_locker)
                        {
                            labelInventoryDate.Invoke((MethodInvoker)delegate { labelInventoryDate.Text = _localDeviceArray[_selectedReader].currentInventory.eventDate.ToLocalTime().ToString("G"); });
                            labelInventoryUser.Invoke((MethodInvoker)delegate { labelInventoryUser.Text = string.Format("{0} {1}", _localDeviceArray[_selectedReader].currentInventory.userFirstName, _localDeviceArray[_selectedReader].currentInventory.userLastName); });


                            listBoxTag.Invoke((MethodInvoker)delegate { listBoxTag.Items.Clear(); });
                            foreach (string strTag in _localDeviceArray[_selectedReader].currentInventory.listTagAll)
                            {
                                string tag = strTag;
                                listBoxTag.Invoke((MethodInvoker)delegate { listBoxTag.Items.Add(tag); });
                            }

                            labelInventoryTagCount.Invoke((MethodInvoker)delegate { labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags, listBoxTag.Items.Count); });
                        }

                    }
                    else
                    {
                        labelInventoryDate.Invoke((MethodInvoker)delegate { labelInventoryDate.Text = ""; });
                        labelInventoryUser.Invoke((MethodInvoker)delegate { labelInventoryUser.Text = ""; });
                        listBoxTag.Invoke((MethodInvoker)delegate { listBoxTag.Items.Clear(); });
                        labelInventoryTagCount.Invoke((MethodInvoker)delegate { labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags, 0); });
                    }
                }
            } 
            timerObjectList.Start();
        }

        #endregion
        #region contextmenudebug
        private void calibrateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int treeNodeSelected = treeViewDevice.SelectedNode.Index;

            if ((_localDeviceArray[treeNodeSelected].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[treeNodeSelected].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
            {

                _localDeviceArray[treeNodeSelected].rfidDev.get_RFID_Device.CalibrateDialog();

            }
        }
        private void findThresholdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int treeNodeSelected = treeViewDevice.SelectedNode.Index;

            if ((_localDeviceArray[treeNodeSelected].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[treeNodeSelected].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
            {
                _localDeviceArray[treeNodeSelected].rfidDev.get_RFID_Device.FindThresholdDialog();
            }
        }
        private void conversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int treeNodeSelected = treeViewDevice.SelectedNode.Index;

            if ((_localDeviceArray[treeNodeSelected].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[treeNodeSelected].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
            {
                _localDeviceArray[treeNodeSelected].rfidDev.get_RFID_Device.ConversionDialog();
            }
        }
        private void tagSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int treeNodeSelected = treeViewDevice.SelectedNode.Index;

            if ((_localDeviceArray[treeNodeSelected].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[treeNodeSelected].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
            {
                _localDeviceArray[treeNodeSelected].rfidDev.get_RFID_Device.TagSetDialog();
            }
        }
        private void doorLightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int treeNodeSelected = treeViewDevice.SelectedNode.Index;

            if ((_localDeviceArray[treeNodeSelected].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[treeNodeSelected].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
            {
                _localDeviceArray[treeNodeSelected].rfidDev.get_RFID_Device.DoorAndLightDiag();
            }
        }
        #endregion
        #region contextmenuList
        private void copyLotIDToClipBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataListView.SelectedItems.Count < 0)
            {
                StringBuilder buffer = new StringBuilder();
                // Loop over all the selected items
                foreach (ListViewItem currentItem in dataListView.Items)
                {
                    // Don't need to look at currentItem, because it is in subitem[0]
                    // So just loop over all the subitems of this selected item
                    //foreach (ListViewItem.ListViewSubItem sub in currentItem.SubItems)
                    //{
                    // Append the text and tab
                    OLVColumn olc = dataListView.GetColumn(_columnInfo[1].ToString());
                    buffer.Append(currentItem.SubItems[olc.Index].Text);
                    buffer.Append("\t");
                    //}
                    // Annoyance: there is a trailing tab in the buffer, get rid of it
                    buffer.Remove(buffer.Length - 1, 1);
                    // If you only use \n, not all programs (notepad!!!) will recognize the newline
                    buffer.Append("\r\n");
                }
                // Set output to clipboard.
                Clipboard.SetDataObject(buffer.ToString());
            }
            else
            {
                StringBuilder buffer = new StringBuilder();
                // Loop over all the selected items
                foreach (ListViewItem currentItem in dataListView.SelectedItems)
                {
                    // Don't need to look at currentItem, because it is in subitem[0]
                    // So just loop over all the subitems of this selected item
                    // foreach(ListViewItem.ListViewSubItem sub in currentItem.SubItems)
                    //{
                    // Append the text and tab
                    OLVColumn olc = dataListView.GetColumn(_columnInfo[1].ToString());
                    buffer.Append(currentItem.SubItems[olc.Index].Text);
                    buffer.Append("\t");
                    //}
                    // Annoyance: there is a trailing tab in the buffer, get rid of it
                    buffer.Remove(buffer.Length - 1, 1);
                    // If you only use \n, not all programs (notepad!!!) will recognize the newline
                    buffer.Append("\r\n");
                }
                // Set output to clipboard.
                Clipboard.SetDataObject(buffer.ToString());

            }
        }
        private void copyToClipBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder buffer = new StringBuilder();
            // Loop over all the selected items
            foreach (ListViewItem currentItem in dataListView.Items)
            {
                foreach (ListViewItem.ListViewSubItem sub in currentItem.SubItems)
                {
                    buffer.Append(sub.Text);
                    buffer.Append("\t");
                }

                buffer.Remove(buffer.Length - 1, 1);
                // If you only use \n, not all programs (notepad!!!) will recognize the newline
                buffer.Append("\r\n");
            }
            // Set output to clipboard.
            Clipboard.SetDataObject(buffer.ToString());
        }
        private void unselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataListView.SelectedItems.Clear();
        }
        private void itemListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArrayList uid = new ArrayList();
            foreach (ListViewItem currentItem in dataListView.SelectedItems)
            {
                OLVColumn olc = dataListView.GetColumn(_columnInfo[0].ToString());
                uid.Insert(0, currentItem.SubItems[olc.Index].Text);
            }

            ItemListForm ilf = new ItemListForm(null, uid);
            ilf.Show();

        }
        #endregion
        #region exportexcel
        private DataTable ConvertForExport(DataTable dt)
        {
            try
            {
                MainDBClass dbClass = new MainDBClass();
                dbClass.OpenDB();
                ArrayList columnToExport = dbClass.GetColumnToExport();
                dtColumnInfo[] colList = dbClass.GetdtColumnInfo();
                dbClass.CloseDB();
                if (columnToExport != null)
                {
                    DataTable dtToexport = dt.Copy();
                    DataColumnCollection dcc = dt.Columns;
                    for (int loop = 0; loop < dcc.Count; loop++)
                    {
                        if (!columnToExport.Contains(dcc[loop].ColumnName))
                            dtToexport.Columns.Remove(dcc[loop].ColumnName);
                    }
                    return ConvertColumnType(dtToexport, colList);
                }
                return ConvertColumnType(dt, colList);
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
                return null;
            }

        }
        private DataTable ConvertColumnType(DataTable dt, dtColumnInfo[] colList)
        {

            DataTable dtNewColType = new DataTable();

            foreach (dtColumnInfo col in colList)
            {
                dtNewColType.Columns.Add(col.colName, col.colType);
            }
            try
            {
                // if double value empty, is exception so return 
                dtNewColType.Load(dt.CreateDataReader(), LoadOption.OverwriteChanges);
                return dtNewColType;
            }
            catch
            {
                return dt;
            }

        }
       
        private void ExportToExcel()
        {
            string fileSaveName = null;
            string macroName = null;
            string pathMacro = null;   

            if (!_runMacro)
            {
                saveXlsFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                DialogResult ret = saveXlsFileDialog.ShowDialog();

                if (ret == DialogResult.OK)
                {
                    fileSaveName = Path.GetFullPath(saveXlsFileDialog.FileName);
                }
            }
            else
            {
                pathMacro = ConfigurationManager.AppSettings["pathExcelSheet"];
                macroName = ConfigurationManager.AppSettings["MacroName"];
                fileSaveName = Application.StartupPath + Path.DirectorySeparatorChar + "XLMacro" + Path.DirectorySeparatorChar + "RFID_export.xlsx";
               
                if (string.IsNullOrEmpty(pathMacro) | string.IsNullOrEmpty(macroName) | !File.Exists(pathMacro))
                {
                    MessageBox.Show(ResStrings.str_ErrorMAcroExcel, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }


            if (fileSaveName == null) return;

                if (File.Exists(fileSaveName))
                    File.Delete(fileSaveName);

                dtColumnInfo[] listCol = _db.GetdtColumnInfo();

            InventoryData dataToExport = _localDeviceArray[_selectedReader].currentInventory;
               
                if (Path.GetExtension(fileSaveName).Contains("xlsx")) // 2010
                {

                    FileInfo newFile = new FileInfo(fileSaveName);
                    ExcelPackage pck = new ExcelPackage(newFile);
                    {
                        //Create the worksheet
                        ExcelWorksheet ws1 = pck.Workbook.Worksheets.Add(ResStrings.str_All);
                        //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                        ws1.Cells.Style.Font.Size = 10;
                        ws1.Cells.Style.Font.Name = "Verdana";

                        ws1.Cells["A1"].LoadFromDataTable(ConvertForExport(dataToExport.dtTagAll), true);

                        ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                        ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);


                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                            ws1.Column(loop).AutoFit(25);


                        if (listCol != null)
                        {
                            foreach (dtColumnInfo col1 in listCol)
                            {
                                if (col1.colDoSum == 1)
                                {
                                    if (!string.IsNullOrEmpty(col1.colName))
                                    {
                                        double sumValue = 0.0;
                                        sumValue = ProcessValueAlert(dataToExport.dtTagAll, col1.colName);

                                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                                        {
                                            if (ws1.Cells[1, loop].Value.ToString().Equals(col1.colName))
                                            {
                                                ws1.Cells[dataToExport.dtTagAll.Rows.Count + 2, loop].Value = sumValue;
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        ExcelWorksheet ws2 = pck.Workbook.Worksheets.Add("Prior");

                        ws2.Cells.Style.Font.Size = 10;
                        ws2.Cells.Style.Font.Name = "Verdana";

                        //Load the datatable into the sheet, starting from cell A1. Print the column names on 
                        ws2.Cells["A1"].LoadFromDataTable(ConvertForExport(dataToExport.dtTagPresent), true);
                        ws2.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                        ws2.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws2.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                            ws2.Column(loop).AutoFit(25);

                        if (listCol != null)
                        {
                            foreach (dtColumnInfo col1 in listCol)
                            {
                                if (col1.colDoSum == 1)
                                {
                                    if (!string.IsNullOrEmpty(col1.colName))
                                    {
                                        double sumValue = 0.0;
                                        sumValue = ProcessValueAlert(dataToExport.dtTagPresent, col1.colName);

                                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                                        {
                                            if (ws1.Cells[1, loop].Value.ToString().Equals(col1.colName))
                                            {
                                                ws1.Cells[dataToExport.dtTagPresent.Rows.Count + 2, loop].Value = sumValue;

                                            }
                                        }
                                    }
                                }
                            }
                        }



                        ExcelWorksheet ws3 = pck.Workbook.Worksheets.Add(ResStrings.str_Added);
                        ws3.Cells.Style.Font.Size = 10;
                        ws3.Cells.Style.Font.Name = "Verdana";

                        //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1                        
                        ws3.Cells["A1"].LoadFromDataTable(ConvertForExport(dataToExport.dtTagAdded), true);

                        ws3.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                        ws3.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws3.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                            ws3.Column(loop).AutoFit(25);
                        if (listCol != null)
                        {
                            foreach (dtColumnInfo col1 in listCol)
                            {
                                if (col1.colDoSum == 1)
                                {
                                    if (!string.IsNullOrEmpty(col1.colName))
                                    {
                                        double sumValue = 0.0;
                                        sumValue = ProcessValueAlert(dataToExport.dtTagAdded, col1.colName);

                                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                                        {
                                            if (ws1.Cells[1, loop].Value.ToString().Equals(col1.colName))
                                            {
                                                ws1.Cells[dataToExport.dtTagAdded.Rows.Count + 2, loop].Value = sumValue;

                                            }
                                        }
                                    }
                                }
                            }
                        }


                        ExcelWorksheet ws4 = pck.Workbook.Worksheets.Add(ResStrings.str_Removed);

                        ws4.Cells.Style.Font.Size = 10;
                        ws4.Cells.Style.Font.Name = "Verdana";
                        //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1                        
                        ws4.Cells["A1"].LoadFromDataTable(ConvertForExport(dataToExport.dtTagRemove), true);

                        ws4.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                        ws4.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws4.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                            ws4.Column(loop).AutoFit(25);

                        if (listCol != null)
                        {
                            foreach (dtColumnInfo col1 in listCol)
                            {
                                if (col1.colDoSum == 1)
                                {
                                    if (!string.IsNullOrEmpty(col1.colName))
                                    {
                                        double sumValue = 0.0;
                                        sumValue = ProcessValueAlert(dataToExport.dtTagRemove, col1.colName);

                                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                                        {
                                            if (ws1.Cells[1, loop].Value.ToString().Equals(col1.colName))
                                            {
                                                ws1.Cells[dataToExport.dtTagRemove.Rows.Count + 2, loop].Value = sumValue;

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        pck.Save();
                       

                        
                    }
                }
                else
                {
                    new ErrorExportDLG().Show();
                }


                if (_runMacro)
                {
                    string processMacro = Application.StartupPath + Path.DirectorySeparatorChar + "XLMacro" + Path.DirectorySeparatorChar + "RunMacro.vbs";
                    string args = "\"" +  pathMacro + "\" " + macroName;

                    Process p = new Process();
                    p.StartInfo.FileName = processMacro;
                    p.StartInfo.Arguments = args;
                    p.StartInfo.UseShellExecute = true;
                    p.Start();
                  
                }
                else
                {
                    if (File.Exists(fileSaveName))
                    Process.Start(fileSaveName);
                }
           
          
        }
        #endregion

        private void CheckCriteria()
        {
            try
            {
                DataTable tmpDt = (DataTable)dataListView.DataSource;
                string selectString = "NOT ("  + _bti.Criteria + ")" ;
                DataView dv = new DataView(tmpDt);
                dv.RowFilter = selectString;

                if (dv.Count > 0)
                {
                    string mes = string.Format(ResStrings.str_CheckCriteria,dv.Count,_bti.Criteria);
                    DialogResult res = MessageBox.Show(this,mes, ResStrings.str_Criteria_info, MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                    if (res == DialogResult.Yes)
                    {
                        BadCriteriaForm bcf = new BadCriteriaForm(dv, _columnInfo,null);
                        bcf.Show();
                    }
                }
            }
            catch(Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }

        }     
        
        public static bool CheckIpAddr(string ipAddress)
        {
            return Regex.IsMatch(ipAddress,
            @"^(25[0-5]|2[0-4]\d|[0-1]?\d?\d)(\.(25[0-5]|2[0-4]\d|[0-1]?\d?\d)){3}$");
        }


        private static DateTime TruncateMs(DateTime dt)
        {
            return dt.AddMilliseconds(-dt.Millisecond);
        }
    }    
}


