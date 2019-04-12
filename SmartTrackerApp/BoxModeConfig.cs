using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using DBClass;
using BrightIdeasSoftware;
using System.Text.RegularExpressions;
using ErrorMessage;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class BoxModeConfig : Form
    {

        MainDBClass db = new MainDBClass();
        DataTable _dtGroup;
        ArrayList _groupTagList;

        Hashtable _columnInfo;
        DataTable _dtProductRef;
        BoxTagInfo _bti;
        LiveDataForm ldf;
        private BoxModeCreateUpdate _bmcu;        
        public BoxModeConfig( LiveDataForm ldf)
        {
            InitializeComponent();
            this.ldf = ldf;
        }

        private void BoxModeConfig_Load(object sender, EventArgs e)
        {
            db.OpenDB();           
            _columnInfo = db.GetColumnInfo();
            GetProduct();
            ConfigureList();
            UpdateGroup();
        }


        public void GetProduct()
        {          
            if (db != null)
            {
                _dtProductRef = db.RecoverAllProduct();
            }
        }

        private void addGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bti = null;
            if (dataGridViewGroup.SelectedRows.Count > 0)
            {
                _bti = new BoxTagInfo();
                _bti.TagBox = dataGridViewGroup.SelectedRows[0].Cells[0].Value.ToString();
                _bti.BoxRef = dataGridViewGroup.SelectedRows[0].Cells[1].Value.ToString();
                _bti.BoxDesc = dataGridViewGroup.SelectedRows[0].Cells[2].Value.ToString();
                _bti.Criteria = dataGridViewGroup.SelectedRows[0].Cells[3].Value.ToString();
                _bti.Criteria = _bti.Criteria.Replace("&quote", "'");
            }
            

            if (_bmcu != null)
            {
                _bmcu.Close();
                _bmcu = null;
            }
            _bmcu = new BoxModeCreateUpdate(_bti,ldf);
            _bmcu.ShowDialog();
            UpdateGroup();
        }

        private void BoxModeConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.CloseDB();
        }

        private void deleteGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewGroup.SelectedRows.Count > 0)
            {
                string tagBox = dataGridViewGroup.SelectedRows[0].Cells[0].Value.ToString();
                db.DeleteGroup(tagBox);
                UpdateGroup();
            }
        }
        private void UpdateGroup()
        {
            _dtGroup = new DataTable();

            _dtGroup = db.RecoverAllGroup();          
            _dtGroup.Columns[0].ColumnName = ResStrings.BoxModeConfig_UpdateGroup_TagUID;
            _dtGroup.Columns[1].ColumnName = ResStrings.str_Group_Reference;
            _dtGroup.Columns[2].ColumnName = ResStrings.str_Group_Description;
            _dtGroup.Columns[3].ColumnName = ResStrings.str_Criteria;
            _dtGroup.Columns.Add(ResStrings.BoxModeConfig_UpdateGroup_Nb_Item_s__in_Group, typeof(String));
            _dtGroup.Columns[4].ColumnName = ResStrings.BoxModeConfig_UpdateGroup_Nb_Item_s__in_Group;
            dataGridViewGroup.DataSource = _dtGroup;
            Font ft = new Font(dataGridViewGroup.Font, FontStyle.Bold);
            dataGridViewGroup.ColumnHeadersDefaultCellStyle.Font = ft;

            for (int i = 0; i < dataGridViewGroup.Rows.Count - 1; i++)
            {
                dataGridViewGroup.Rows[i].Cells[ResStrings.BoxModeConfig_UpdateGroup_Nb_Item_s__in_Group].Value = db.GetGroupLinkCount(dataGridViewGroup.Rows[i].Cells[ResStrings.BoxModeConfig_UpdateGroup_TagUID].Value.ToString());
                dataGridViewGroup.Rows[i].Cells[ResStrings.str_Criteria].Value = dataGridViewGroup.Rows[i].Cells[ResStrings.str_Criteria].Value.ToString().Replace("&quote", "'");
            }              
    

        }

        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dataGridViewGroup_SelectionChanged(object sender, EventArgs e)
        {

            
            if (dataGridViewGroup.SelectedRows.Count > 0)
            {                
                    _bti = new BoxTagInfo();
                    _bti.TagBox = dataGridViewGroup.SelectedRows[0].Cells[0].Value.ToString();
                    _bti.BoxRef = dataGridViewGroup.SelectedRows[0].Cells[1].Value.ToString();
                    _bti.BoxDesc = dataGridViewGroup.SelectedRows[0].Cells[2].Value.ToString();
                    _bti.Criteria = dataGridViewGroup.SelectedRows[0].Cells[3].Value.ToString();
                    _bti.Criteria = _bti.Criteria.Replace("&quote","'");
                
                if (!string.IsNullOrEmpty(_bti.TagBox))           
                {
                    _groupTagList = db.GetGroupLink(_bti.TagBox);                   
                }
             
               
            }
            if (_groupTagList == null) _groupTagList = new ArrayList();
            UpdateDataList();
         
        }

        private void UpdateDataList()
        {
            DataTable newDt = new DataTable();
            newDt.Columns.Add(ResStrings.BoxModeConfig_UpdateDataList_Box, typeof(string));
            for (int i = 0; i < _columnInfo.Count; i++)
            {
                newDt.Columns.Add(_columnInfo[i].ToString(), typeof(string));
            }

            foreach (string uid in _groupTagList)
            {
                string selectString = "["+ ResStrings.BoxModeConfig_UpdateGroup_TagUID +"]= '" + uid + "'";
                DataRow[] productInfo = _dtProductRef.Select(selectString);
                if (productInfo.Length > 0)
                {
                    DataRow rowToadd = newDt.NewRow();
                    rowToadd[0] = ResStrings.BoxModeConfig_UpdateDataList_Box;
                    for (int i = 0; i < _columnInfo.Count; i++)
                        rowToadd[i + 1] = productInfo[0].ItemArray[i];
                    newDt.Rows.Add(rowToadd);

                }
                else
                {
                    DataRow rowToadd = newDt.NewRow();
                    rowToadd[0] = ResStrings.BoxModeConfig_UpdateDataList_Box;
                    rowToadd[1] = uid;
                    rowToadd[2] = ResStrings.str_Unreferenced;                 
                    newDt.Rows.Add(rowToadd);
                }
            }

            dataListView.DataSource = null;
            dataListView.DataSource = newDt;
            dataListView.BuildList();


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
        public void ConfigureList()
        {
            // cretate datatable for column
            DataTable newDt = new DataTable();
            newDt.Columns.Add(ResStrings.BoxModeConfig_UpdateDataList_Box, typeof(string));
            for (int i = 0; i < _columnInfo.Count; i++)
            {
                newDt.Columns.Add(_columnInfo[i].ToString(), typeof(string));
            }
            dataListView.DataSource = null;
            dataListView.DataSource = newDt;
            dataListView.Columns[0].Name = ResStrings.BoxModeConfig_UpdateDataList_Box;
            dataListView.Columns[0].Text = ResStrings.BoxModeConfig_UpdateDataList_Box;

            for (int i = 0; i < _columnInfo.Count ; i++)
            {
                dataListView.Columns[i+1].Name = _columnInfo[i].ToString();
                dataListView.Columns[i+1].Text = _columnInfo[i].ToString();
            }

          
            //empty List

            dataListView.EmptyListMsg = "";
            TextOverlay textOverlay = dataListView.EmptyListMsgOverlay as TextOverlay;
            textOverlay.TextColor = Color.Firebrick;
            textOverlay.BackColor = Color.AntiqueWhite;
            textOverlay.BorderColor = Color.DarkRed;
            textOverlay.BorderWidth = 4.0f;
            textOverlay.Font = new Font("Chiller", 36);
            textOverlay.Rotation = -5;


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
        }

        private void dataListView_BeforeCreatingGroups(object sender, CreateGroupsEventArgs e)
        {
            if (_bti != null)

            dataListView.GetColumn(ResStrings.BoxModeConfig_UpdateDataList_Box).MakeGroupies(
               new object[] { ResStrings.BoxModeConfig_UpdateDataList_Box },
               new string[] { "Rien", _bti.BoxRef },
               new string[] { "not", ResStrings.BoxModeConfig_dataListView_BeforeCreatingGroups_Box_Present },
               new string[] { "Not", ResStrings.BoxModeConfig_dataListView_BeforeCreatingGroups_Tag_s__present_in_selected_box},
               new string[] { "Rien", "" }  );
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ProcessData();
        }

        private void txtDataToSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Return)
            {
                ProcessData();
            }
        }

        private static Regex _isNumber = new Regex(@"^\d+$");

        public static bool IsInteger(string theValue)
        {
            Match m = _isNumber.Match(theValue);
            return m.Success;
        } //IsInteger

        private void ProcessData()
        {
            bool bFind = false;
            string txtSearch = txtDataToSearch.Text;
            string saveTxt = txtSearch;
            if (string.IsNullOrEmpty(txtSearch)) return;


            if ((IsInteger(txtSearch)) && (db.IsProductExist(txtSearch)))
            {
                if (db.IsProductExist(txtSearch))
                {
                    string selectString = "[" + ResStrings.BoxModeConfig_UpdateGroup_TagUID + "] = '" + txtSearch + "'";
                    DataRow[] productInfo = _dtProductRef.Select(selectString);
                    if (productInfo.Length > 0) // c'est un uid
                    {
                        string groupBoxName = db.GetGroupName(txtSearch);
                        if (!string.IsNullOrEmpty(groupBoxName))
                        {
                            bFind = true;
                            txtSearch = groupBoxName;
                        }
                    }
                    else // c'est une ref
                    {
                        string uid = db.getProductTagID(txtSearch);
                        if (!string.IsNullOrEmpty(uid))
                        {
                            string groupBoxName = db.GetGroupName(txtSearch);
                            if (!string.IsNullOrEmpty(groupBoxName))
                            {
                                bFind = true;
                                txtSearch = groupBoxName;
                                saveTxt = uid;
                            }
                        }
                    }
                }
            }
            else // search for a ref
            {
                string uid = db.getProductTagID(txtSearch);
                if (!string.IsNullOrEmpty(uid))
                {
                    string groupBoxName = db.GetGroupName(uid);
                    if (!string.IsNullOrEmpty(groupBoxName))
                    {
                        bFind = true;
                        txtSearch = groupBoxName;
                        saveTxt = uid;
                    }
                }
            }


            // search for tag in group
           for (int  i = 0 ;  i < dataGridViewGroup.Rows.Count-1 ; i++)
            {
                if (dataGridViewGroup.Rows[i].Cells[0].Value.ToString().Equals(txtSearch) || dataGridViewGroup.Rows[i].Cells[1].Value.ToString().Equals(txtSearch))
                {
                    bFind = true;
                    dataGridViewGroup.ClearSelection();
                    dataGridViewGroup.Rows[i].Selected = true;
                    break;
                }
            }

           if (bFind)
           {
               UpdateDataList();
               if (!string.IsNullOrEmpty(saveTxt))
               {
                   dataListView.SelectedItems.Clear();
                   foreach (ListViewItem lvi in dataListView.Items)
                   {
                       if (lvi.SubItems[1].Text.Equals(saveTxt))
                       {                          
                           dataListView.Items[lvi.Index].Selected = true;
                           break;
                       }
                       
                   }
                   dataListView.Focus();
               }               
           }
           else
               MessageBox.Show(ResStrings.BoxModeConfig_ProcessData_Search_process_find_nothing, ResStrings.BoxModeConfig_ProcessData__Box_Mode_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if ((dataGridViewGroup.SelectedRows.Count > 0) && (_bti != null))
            {
                CheckCriteria();
            }
        }
        private void CheckCriteria()
        {
            try
            {
                DataTable tmpDt = (DataTable)dataListView.DataSource;
                string selectString = "NOT (" + _bti.Criteria + ")";
                DataView dv = new DataView(tmpDt);
                dv.RowFilter = selectString;

                if (dv.Count > 0)
                {
                    BadCriteriaForm bcf = new BadCriteriaForm(dv, _columnInfo,null);
                    bcf.Show();
                }
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }

        }

       
    }

    public class BoxTagInfo
    {
        public string TagBox;
        public string BoxRef;
        public string BoxDesc;
        public string Criteria;
    }
}
