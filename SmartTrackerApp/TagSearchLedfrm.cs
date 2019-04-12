using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using DataClass;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class TagSearchLedfrm : Form
    {
        private DataTable _dtProductToFind;
        private SDK_SC_RFID_Devices.RFID_Device _currentDevice;
        List<string> selectedTags = new List<string>();
        private bool _bStop = false;
        public TagSearchLedfrm(DataTable dtProductToFind , SDK_SC_RFID_Devices.RFID_Device currentDevice)
        {
            InitializeComponent();
            this._dtProductToFind = dtProductToFind;
            _dtProductToFind.CaseSensitive = true;
            this._currentDevice = currentDevice;
        }
        

        public void refreshDataGrid()
        {
            

            dataGridViewTagToFind.DataSource = null;
            dataGridViewTagToFind.DataSource = _dtProductToFind;
        }

        private void TagSearchLedfrm_Load(object sender, EventArgs e)
        {

            if ((_currentDevice != null) && (_currentDevice.DeviceStatus == DeviceStatus.DS_Ready))
            {
                labelStatus.Text = "Info : Device Ready";
                if (_dtProductToFind.Rows.Count > 0)
                {
                    buttonProcess.Enabled = _dtProductToFind.Rows.Count > 0;
                }

            }
            else
            {
                labelStatus.Text = "Info : Device not Ready";
                buttonProcess.Enabled = false;
            }
            Font ft = new Font(dataGridViewTagToFind.Font, FontStyle.Bold);
            dataGridViewTagToFind.ColumnHeadersDefaultCellStyle.Font = ft;

            refreshDataGrid();
        }

        private void TagSearchLedfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _dtProductToFind.Rows.Clear();
            refreshDataGrid();
        }

        private void buttonProcess_Click(object sender, EventArgs e)
        {
            int cpt = -1;
            buttonProcess.Enabled = false;
            buttonStop.Enabled = true;
            selectedTags.Clear();
            foreach (DataGridViewRow oRow in dataGridViewTagToFind.Rows)
            {
                string tagId = oRow.Cells[0].Value.ToString();
                selectedTags.Add(tagId);
            }
            UpdateGridColor();


            if (selectedTags.Count == 0)
            {
                MessageBox.Show("No Tag to light - Process aborted");
                buttonProcess.Enabled = true;
                buttonStop.Enabled = false;
                return;
            }

            _bStop = false;
            int nbTagToLight = selectedTags.Count;
            if (nbTagToLight == 0) return;

            DialogResult ret = DialogResult.No;
            

            if ((_currentDevice.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                (_currentDevice.DeviceStatus == DeviceStatus.DS_Ready))
            {
              
                while ((ret == DialogResult.No) && (selectedTags.Count > 0) && (!_bStop))
                {
                    cpt++;
                    if (cpt == 0) labelStatus.Text = "Info : Process Running";
                    else if (cpt == 1) labelStatus.Text = "Info : Process Running.";
                    else if (cpt == 2) labelStatus.Text = "Info : Process Running..";
                    else if (cpt > 2)
                    {
                        labelStatus.Text = "Info : Process Running...";
                        cpt = -1;
                    }
                    
                    
                    Application.DoEvents();
                    nbTagToLight = selectedTags.Count;
                    _currentDevice.TestLightingOneAxis(selectedTags, 1);
                    if (nbTagToLight != selectedTags.Count) // des tags on ete trouvé
                    {
                        string message = string.Empty;

                        if ((nbTagToLight == 1) && ((nbTagToLight - selectedTags.Count) == 1))
                            message = String.Format(ResStrings.str_LedTagFound, nbTagToLight, nbTagToLight - selectedTags.Count);
                        else if ((nbTagToLight - selectedTags.Count) == 1)
                            message = String.Format(ResStrings.str_LedFound2, nbTagToLight, nbTagToLight - selectedTags.Count);
                        else
                            message = String.Format(ResStrings.str_LedFound3, nbTagToLight, nbTagToLight - selectedTags.Count);

                        if (selectedTags.Count > 0)
                        {
                            UpdateGridColor();
                            message += "\r\n Would you like to stop to search?";
                            ret = MessageBox.Show(message, ResStrings.str_LED_Information, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        }
                        else
                        {
                            UpdateGridColor();
                            message += "\r\n All tags Found - Process will stop?";
                            ret = MessageBox.Show(message, ResStrings.str_LED_Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                    }
                    _currentDevice.StopLightingLeds();
                }
            }
            buttonProcess.Enabled = true;
            buttonStop.Enabled = false;
            labelStatus.Text = "Info : Device Ready";

        }

        private void UpdateGridColor()
        {
            foreach (DataGridViewRow oRow in dataGridViewTagToFind.Rows)
            {
                string tagId = oRow.Cells[0].Value.ToString();
                oRow.DefaultCellStyle.ForeColor = selectedTags.Contains(tagId) ? Color.Red : Color.Green;
                oRow.Selected = false;
            }
            dataGridViewTagToFind.ClearSelection();
        }

        private void labelStatus_Click(object sender, EventArgs e)
        {

        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            _bStop = true;
            buttonProcess.Enabled = true;
            buttonStop.Enabled = false;
        }

        private void TagSearchLedfrm_Shown(object sender, EventArgs e)
        {
            if ((_currentDevice != null) && (_currentDevice.DeviceStatus == DeviceStatus.DS_Ready))
            {
                labelStatus.Text = "Info : Device Ready";
                if (_dtProductToFind.Rows.Count > 0)
                {
                    buttonProcess.Enabled = _dtProductToFind.Rows.Count > 0;
                }

            }
            else
            {
                labelStatus.Text = "Info : Device not Ready";
             
            }
        }

        private void TagSearchLedfrm_VisibleChanged(object sender, EventArgs e)
        {
            if ((_currentDevice != null) && (_currentDevice.DeviceStatus == DeviceStatus.DS_Ready))
            {
                labelStatus.Text = "Info : Device Ready";
                if (_dtProductToFind.Rows.Count > 0)
                {
                    buttonProcess.Enabled = _dtProductToFind.Rows.Count > 0;
                }

            }
            else
            {
                labelStatus.Text = "Info : Device not Ready";
            }
        }

       
    }
}
