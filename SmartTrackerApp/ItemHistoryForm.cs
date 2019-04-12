using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;


using DBClass;
using DataClass;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class ItemHistoryForm : Form
    {
        private string _tagUid = null;
        private string _lotId = null;
        private MainDBClass _db = null;       
        private DataTable _tbItemHistory = null;
        private TagEventClass[] _tagEventArray = null;

        Hashtable _columnInfo = null;

        public ItemHistoryForm(string tagUid,string lotId)
        {
            InitializeComponent();
            Font ft = new Font(dataGridViewItemHistory.Font, FontStyle.Bold);
            dataGridViewItemHistory.ColumnHeadersDefaultCellStyle.Font = ft;
                 

            _tagUid = tagUid;
            _lotId = lotId;
            toolStripTextBoxTagUID.Text = tagUid;
            toolStripTextBoxLotID.Text = lotId;
        }

        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker) delegate { Close(); });
        }
       

        private void ItemHistoryForm_Load(object sender, EventArgs e)
        {
            
            _db = new MainDBClass();        
            _db.OpenDB();
            _columnInfo = _db.GetColumnInfo();
            InitTable();
         
        }
        private void timerStart_Tick(object sender, EventArgs e)
        {
            timerStart.Enabled = false;
            if (_tagUid != null)
            {
                ProcessTag(_tagUid);
                UpdateDg(-1);
            }
        }

        private void InitTable()
        {
            _tbItemHistory = new DataTable();
            _tbItemHistory.Columns.Add(ResStrings.str_EventDate, typeof(string));
            _tbItemHistory.Columns.Add(_columnInfo[0].ToString(), typeof(string));
            _tbItemHistory.Columns.Add(_columnInfo[1].ToString(), typeof(string));
            _tbItemHistory.Columns.Add(ResStrings.str_Reader_Name, typeof(string));
            _tbItemHistory.Columns.Add(ResStrings.str_First_Name, typeof(string));
            _tbItemHistory.Columns.Add(ResStrings.str_Last_Name, typeof(string));
            _tbItemHistory.Columns.Add(ResStrings.str_Event_Type, typeof(string));
            //
            for (int i = 2; i < _columnInfo.Count; i++)
            {
                _tbItemHistory.Columns.Add(_columnInfo[i].ToString(), typeof(string));
            }

            dataGridViewItemHistory.DataSource = null;
            dataGridViewItemHistory.DataSource = _tbItemHistory;
           
        }

        private void ProcessTag(string tagUid)
        {

            string[] pct = _db.RecoverProductInfo(tagUid);
            if (pct != null)
            {
                //labelHistory.Text = " Tag UID : " + tagUID + "          Reference : " + pct[1];
                labelHistory.Text = string.Empty;
                for (int i = 0; i < 2; i++)
                    labelHistory.Text += string.Format("{0}:{1}   ", _columnInfo[i], pct[i]);
            }
            else   
            {
                labelHistory.Text = string.Format("{0} : {1}   {2} : {3}", _columnInfo[0], tagUid, _columnInfo[1],ResStrings.str_Unreferenced);
            }

            InitTable();

            ArrayList prodHistory = _db.RecoverProductHistory(tagUid);
            TagEventClass[] tagData = _db.GetTagEvent(tagUid);
            if (tagData != null)
            {
                _tagEventArray = new TagEventClass[tagData.Length];
                tagData.CopyTo(_tagEventArray, 0);

               
                foreach (TagEventClass tec in _tagEventArray)
                {
                    
                    string[] prd = null;
                    string lotId = ResStrings.str_Unreferenced;
                    if (prodHistory != null)
                    {
                        for ( int loop = 0 ; loop < prodHistory.Count ; loop++)
                        {
                            prd = (string[])prodHistory[loop];
                            int compRes = (tec.eventSortedDate.CompareTo(prd[1]));
                            if (compRes > 0)
                            {
                                lotId = prd[3];
                                break;
                            }                           
                        }
                    }
                    DataRow rowToadd = _tbItemHistory.NewRow();

                    rowToadd[0] = tec.eventDate;
                    rowToadd[1] = tec.tagUID;
                    rowToadd[2] = lotId;
                    rowToadd[3] = tec.DeviceName + " ( " + tec.serialRFID + ")";
                    rowToadd[4] = tec.FirstName;
                    rowToadd[5] = tec.LastName;
                    rowToadd[6] = tec.tagEventType;
                    if (prd != null)
                    {
                        for (int i = 2; i < _columnInfo.Count; i++)
                            rowToadd[i + 5] = prd[i+2];
                    }
                    else
                    {
                        for (int i = 2; i < _columnInfo.Count; i++)
                            rowToadd[i + 5] = " ";
                    }

                   _tbItemHistory.Rows.Add(rowToadd);
                 
                    //tbItemHistory.Rows.Add(tec.eventDate, tec.tagUID,LotID,tec.DeviceName + " ( " + tec.serialRFID + ")",tec.FirstName, tec.LastName,tec.tagEventType);
                }
            }
            dataGridViewItemHistory.DataSource = null;
            dataGridViewItemHistory.DataSource = _tbItemHistory;

            Application.DoEvents();
        }

        private void UpdateDg(int rowSelect)
        {            
            if (rowSelect < 0)
            {
                foreach (DataGridViewRow dgvRow in dataGridViewItemHistory.Rows)
                {
                    Object cellValue = dgvRow.Cells[ResStrings.str_Event_Type].Value;
                    if (cellValue != null)
                    {
                        string strCase = cellValue.ToString();
                        if (strCase.Equals(ResStrings.str_Added)) dgvRow.DefaultCellStyle.ForeColor = Color.Green;  
                        else if (strCase.Equals( ResStrings.str_Present)) dgvRow.DefaultCellStyle.ForeColor = Color.Blue; 
                        else dgvRow.DefaultCellStyle.ForeColor = Color.Red;       
                    }
                }              
            }
            else
            {
                Object cellValue = dataGridViewItemHistory.Rows[rowSelect].Cells[ResStrings.str_Event_Type].Value;
                if (cellValue != null)
                {
                    string strCase = cellValue.ToString();
                    if (strCase.Equals(ResStrings.str_Added)) dataGridViewItemHistory.Rows[rowSelect].DefaultCellStyle.ForeColor = Color.Green;  
                    else if (strCase.Equals( ResStrings.str_Present)) dataGridViewItemHistory.Rows[rowSelect].DefaultCellStyle.ForeColor = Color.Blue; 
                    else dataGridViewItemHistory.Rows[rowSelect].DefaultCellStyle.ForeColor = Color.Red;  
                }
            }
            dataGridViewItemHistory.ClearSelection();
            Application.DoEvents();
        }

        private void ItemHistoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _db.CloseDB();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
        }

        private void toolStripButtonGet_Click(object sender, EventArgs e)
        {
            _tagUid = toolStripTextBoxTagUID.Text;
            _lotId = toolStripTextBoxLotID.Text;

            if (!string.IsNullOrEmpty(_tagUid))
            {
                ProcessTag(_tagUid);
                UpdateDg(-1);
            }
            else if (!string.IsNullOrEmpty(_lotId))
            {
                _tagUid = _db.getProductTagID(_lotId);
                if (_tagUid != null)
                {
                    ProcessTag(_tagUid);
                    UpdateDg(-1);
                }
            }
        }

        private void toolStripTextBoxTagUID_Click(object sender, EventArgs e)
        {
            toolStripTextBoxLotID.Text = null;
        }

        private void toolStripTextBoxLotID_Click(object sender, EventArgs e)
        {
            toolStripTextBoxTagUID.Text = null;
        }

        private void dataGridViewItemHistory_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewItemHistory.SelectedRows.Count > 0)
            {
                labelHistory.Text = string.Empty;
                for (int i = 0; i < 2; i++)
                    labelHistory.Text += string.Format("{0} : {1}   ", _columnInfo[i], dataGridViewItemHistory.SelectedRows[0].Cells[1 + i].Value);
            }         
        }

        private void dataGridViewItemHistory_Sorted(object sender, EventArgs e)
        {
            UpdateDg(-1);
        }
    }
}
