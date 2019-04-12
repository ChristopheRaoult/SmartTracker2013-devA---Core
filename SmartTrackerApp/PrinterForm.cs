using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataClass;
using SDK_SC_RFID_Devices;

namespace smartTracker
{
    public partial class PrinterForm : Form
    {
        public RFID_Device device = null;
        private PrinterClass printer = null;
        public PrinterData printInfo;
        private string rfidPrinterSerial;
        rfidPluggedInfo[] arrayOfPluggedDevice = null;
        public string TagUidPrinted;

        private volatile int nbprint;

        public string lotID;

        public PrinterForm(RFID_Device device , string lotID , PrinterData printInfo)
        {
            
            this.device = device;
            this.lotID = lotID;
            this.printInfo = printInfo;
            InitializeComponent();
        }

        private void PrinterForm_Load(object sender, EventArgs e)
        {
            txtItem.Text = lotID;
        }
        private void ReadConfiguration()
        {
            printInfo = new PrinterData();
            rfidPrinterSerial = ConfigurationManager.AppSettings["Rfid_Printer_Serial"];
            printInfo.PrinterIP = ConfigurationManager.AppSettings["PrinterIP"];
            printInfo.TemplateName = ConfigurationManager.AppSettings["Template"];

            txtPrinterIp.Text = printInfo.PrinterIP;
            txtPrinterSerial.Text = rfidPrinterSerial;
            txtTemplate.Text = printInfo.TemplateName;

        }
        private void FindDeviceAndConnect(string serial)
        {
            arrayOfPluggedDevice = null;
            RFID_Device tmp = new RFID_Device();
            arrayOfPluggedDevice = tmp.getRFIDpluggedDevice(false);
            tmp.ReleaseDevice();

            if (arrayOfPluggedDevice != null)
            {
                foreach (rfidPluggedInfo dev in arrayOfPluggedDevice)
                {
                    if (dev.SerialRFID.Equals(serial))
                    {
                        if (device != null)
                        {
                            if (device.ConnectionStatus != ConnectionStatus.CS_Connected)
                                device.ReleaseDevice();
                        }
                        //Create a new object 
                        device = new RFID_Device();
                        updatelabel("Info : In Connection");
                        //subscribe the event 
                        device.NotifyRFIDEvent += new NotifyHandlerRFIDDelegate(rfidDev_NotifyRFIDEvent);
                        device.Create_NoFP_Device(dev.SerialRFID, dev.portCom);

                        break;
                    }
                }
            }
            else
            {
                updatelabel("Info : No Device Detected!");
            }
        }
        // Function to get rfid event
        private void rfidDev_NotifyRFIDEvent(object sender, SDK_SC_RfidReader.rfidReaderArgs args)
        {
            switch (args.RN_Value)
            {
                // Event when failed to connect          
                case SDK_SC_RfidReader.rfidReaderArgs.ReaderNotify.RN_FailedToConnect:
                    updatelabel("Info : Failed to Connect");
                    break;
                // Event when release the object
                case SDK_SC_RfidReader.rfidReaderArgs.ReaderNotify.RN_Disconnected:
                    updatelabel("Info : Device Disconnected");
                    break;

                //Event when device is connected
                case SDK_SC_RfidReader.rfidReaderArgs.ReaderNotify.RN_Connected:
                    updatelabel("Info : Device Connected");
                    break;

                // Event when scan started
                case SDK_SC_RfidReader.rfidReaderArgs.ReaderNotify.RN_ScanStarted:
                    updatelabel("Info : Scan Started");
                    break;

                //event when fail to start scan
                case SDK_SC_RfidReader.rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan:
                    updatelabel("Info : Failed to start scan");
                    break;

                //event when a new tag is identify
                case SDK_SC_RfidReader.rfidReaderArgs.ReaderNotify.RN_TagAdded:

                    break;

                // Event when scan completed
                case SDK_SC_RfidReader.rfidReaderArgs.ReaderNotify.RN_ScanCompleted:

                    if (device.currentInventory.nbTagAll == 0)
                    {
                        updatelabel("Info : No Tag Found");
                        // feed the printer

                        PrinterClass printer = new PrinterClass();
                        PrinterClass.MAPI_FEED_RETURN_TYPE ret = printer.FeedPrinter(printInfo.PrinterIP, 9100);
                        if (ret == PrinterClass.MAPI_FEED_RETURN_TYPE.MAPI_FEED_RETURN_SUCCEED)
                        {
                            updatelabel("Feed Succeed");
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                    else if (device.currentInventory.nbTagAll > 1)
                    {
                        updatelabel("Info : More than one tag scanned - Please check the printer");
                    }
                    else
                    {
                        updatelabel("Info : Scan Completed - Tag Scanned : " + device.currentInventory.listTagAll[0].ToString());
                        TagUidPrinted = device.currentInventory.listTagAll[0].ToString();
                        print(TagUidPrinted);
                    }

                    break;

                //error when error during scan
                case SDK_SC_RfidReader.rfidReaderArgs.ReaderNotify.RN_ReaderScanTimeout:
                case SDK_SC_RfidReader.rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan:
                    updatelabel("Info : Scan has error");
                    break;
                case SDK_SC_RfidReader.rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost:
                    updatelabel("Info : Scan cancel by host");
                    break;
            }
            Application.DoEvents();
        }
        private void updatelabel(string text)
        {
            if (labelInfo.InvokeRequired)
            {
                labelInfo.Invoke((MethodInvoker)delegate { labelInfo.Text = text; });
            }
            else
            {
                labelInfo.Text = text;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            ReadConfiguration();
            if (device != null)
            {
                if (this.device.ConnectionStatus == ConnectionStatus.CS_Connected)
                {
                    updatelabel("Info : Device Connected");
                }
                else
                {
                    updatelabel("Info : Device Not Connected");

                }
            }
            else
            {
                
                FindDeviceAndConnect(rfidPrinterSerial);
              
            }
        }
        private bool checkPrinter(string Ip)
        {
            printInfo.bReady = false;
            if (TcpIP_class.tcpUtils.PingAddress(Ip, 2000))
            {
                try
                {
                    printer = new PrinterClass();
                    PrinterClass.MAPI_GETPRINTERSTATUS_TYPE resp = printer.GetStatusPrinter(Ip, 9100);
                    if (resp == PrinterClass.MAPI_GETPRINTERSTATUS_TYPE.MAPI_GETPRINTERSTATUS_RETURN_PRINTER_READY)
                    {
                        printInfo.bReady = true;
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Printer not ready : " + resp.ToString());
                        return false;
                    }
                }
                catch
                {
                    MessageBox.Show("Printer not ready");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("No printer detect on the IP " + Ip);
                return false;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((device != null) && (device.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                (checkPrinter(printInfo.PrinterIP)))
            {
                //nbprint = 0;
                ScanDevice();
            }
            else
            {
                MessageBox.Show("Printer Not Ready");
            }

        }

        private void ScanDevice()
        {
            if (device == null) return;
            if ((device.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                (device.DeviceStatus == DeviceStatus.DS_Ready))
            {
                //Request a scan
                //Scan status will be notified by event
                device.ScanDevice();
            }
            else
                MessageBox.Show("Device not ready or not connected");
        }

        public void print(string TagID)
        {
            //Ie KB for double print error.

            //if (nbprint != 0) return;
            //nbprint++;
            PrinterClass.MAPI_PRINTTAGUID_RETURN_TYPE respprint;
            printer = new PrinterClass();

            string arg = string.Empty;
            arg += TagID + ";";

            respprint = printer.PrintTag(printInfo.PrinterIP, 9100, printInfo.TemplateName, 1, arg);
            if (respprint == PrinterClass.MAPI_PRINTTAGUID_RETURN_TYPE.MAPI_PRINTTAGUID_RETURN_SUCCEED)
            {
                updatelabel("Print Succeed");
                System.Threading.Thread.Sleep(1000);
                BeginInvoke((MethodInvoker) delegate
                {
                    
                    //Close();
                    Hide();
                });
            }
            else
            {
                updatelabel("Print Error :" + respprint.ToString());
                System.Threading.Thread.Sleep(1000);
            }
        }

    }
}
