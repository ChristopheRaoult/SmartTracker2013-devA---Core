using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Collections;


using DataClass;

namespace SDK_SC_AccessControl
{
    public delegate void NotifyHandlerBadgeReaderDelegate(Object sender, string badgeID, string deviceSerial);
    public delegate void NotifyHandlerBrDelegate(Object sender, BadgeReaderArgs args);

    public class clBadgeReader
    {
        public event NotifyHandlerBadgeReaderDelegate NotifyBadgeReaderEvent;
        public event NotifyHandlerBrDelegate NotifyEvent;

        const char CHAR_StartOfFrameLF = (char)0x02;
        const char CHAR_EndOfFrameLF = (char)0x03;
        const char CHAR_EndOfFrameHF = (char)0x0D;

        private SerialPort serialPort;
        private string inboundBuffer = "";
        public bool DsrValue { get { return serialPort.DsrHolding; } }
        public bool IsConnected = false;
        private bool bNotifyAll = false;
        string deviceSerial = null;
        private string strBadgeRead = "";
        public string BadgeUser { get { return strBadgeRead; } }
        public ArrayList listeBadge = new ArrayList();
        public AccessBagerReaderType rdType;

        private UserClassTemplate[] _userTemplates;


        public void loadBadge(ArrayList listeBadge)
        {
            this.listeBadge = listeBadge;
        }


        public void closePort()
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.DataReceived -= (OnDataReceived);
                    serialPort.ErrorReceived -= (OnErrorReceived);
                    serialPort.PinChanged -= (OnSerialPinChanged);

                    GC.ReRegisterForFinalize(serialPort.BaseStream);
                    serialPort.Close();
                }
            }
            serialPort = null;
            System.Threading.Thread.Sleep(500);
            IsConnected = false;
        }


        ~clBadgeReader()
        {
            if (serialPort != null) closePort();
        }


        public clBadgeReader(AccessBagerReaderType rdType, string strCom, string deviceSerial, bool bNotifyAll)
        {
            try
            {
                this.rdType = rdType;
                this.deviceSerial = deviceSerial;
                this.bNotifyAll = bNotifyAll;
                listeBadge.Clear(); // assert no badge at start
                openPort(strCom);
                IsConnected = true;

            }
            catch (Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);

            }
        }


        public void openPort(string strCom)
        {

            string[] ports = SerialPort.GetPortNames();

            bool bExist = false;
            foreach (string pt in ports)
                if (pt.Equals(strCom)) bExist = true;

            if (!bExist) return;

            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
                serialPort = null;
            }
            serialPort = new SerialPort(strCom, 9600, Parity.None, 8, StopBits.One);
            serialPort.Open();
            if (serialPort.IsOpen)
            {
                GC.SuppressFinalize(serialPort.BaseStream);
                serialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
                serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(OnErrorReceived);
                serialPort.PinChanged += new SerialPinChangedEventHandler(OnSerialPinChanged);
            }
        }



        protected virtual void sendMessage(byte[] message, int size)
        {
            if (serialPort == null) return;
            if (serialPort.IsOpen)
                serialPort.Write(message, 0, size);
            System.Threading.Thread.Sleep(10);
        }


        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            switch (rdType)
            {
                case AccessBagerReaderType.RT_HF:

                    while (true)
                    {
                        inboundBuffer += serialPort.ReadExisting();
                        int frameEnd = inboundBuffer.IndexOf(CHAR_EndOfFrameHF, 0);
                        if (frameEnd < 0)
                            break;
                        // A full message is available.
                        string message = inboundBuffer.Substring(0, frameEnd - 2);
                        inboundBuffer = inboundBuffer.Substring(frameEnd + 1);
                        strBadgeRead = message.Substring(0, 10);
                        if ((listeBadge.Contains(strBadgeRead)) || bNotifyAll)
                        {
                            if (NotifyBadgeReaderEvent != null) NotifyBadgeReaderEvent(this, strBadgeRead, deviceSerial);

                            // new event type, for badge readers integrated to class "RFID_Device".
                            if (_userTemplates != null) // users have been loaded
                            {
                                UserClassTemplate currentUser = GetUserTemplateByBadgeId(strBadgeRead);

                                if (currentUser != null) // corresponding user has been found
                                {
                                    BadgeReaderArgs brEventArgs = new BadgeReaderArgs(deviceSerial, currentUser,
                                        AccessBagerReaderType.RT_HF);
                                    NotifyEvent(this, brEventArgs);
                                }
                            }
                        }
                    }

                    break;

                case AccessBagerReaderType.RT_LF:
                    while (true)
                    {
                        inboundBuffer += serialPort.ReadExisting();
                        int length = inboundBuffer.Length;
                        int frameStart = inboundBuffer.IndexOf(CHAR_StartOfFrameLF);
                        if (frameStart != 0)
                        {
                            if (frameStart < 0)
                            {
                                inboundBuffer = "";
                                break;
                            }
                            inboundBuffer = inboundBuffer.Substring(frameStart);
                            frameStart = 0;
                        }

                        int frameEnd = inboundBuffer.IndexOf(CHAR_EndOfFrameLF, 0);
                        if (frameEnd < 0)
                            break;

                        // A full message is available.
                        string message = inboundBuffer.Substring(1, frameEnd - 1);
                        inboundBuffer = inboundBuffer.Substring(frameEnd + 1);

                        strBadgeRead = message.Substring(0, 12);
                        if ((listeBadge.Contains(strBadgeRead)) || bNotifyAll)
                        {
                            if (NotifyBadgeReaderEvent != null) NotifyBadgeReaderEvent(this, strBadgeRead, deviceSerial);

                            // new event type, for badge readers integrated to class "RFID_Device".
                            if (_userTemplates != null) // users have been loaded
                            {
                                UserClassTemplate currentUser = GetUserTemplateByBadgeId(strBadgeRead);

                                if (currentUser != null) // corresponding user has been found
                                {
                                    BadgeReaderArgs brEventArgs = new BadgeReaderArgs(deviceSerial, currentUser,
                                        AccessBagerReaderType.RT_LF);
                                    NotifyEvent(this, brEventArgs);
                                }
                            }
                        }
                    }
                    break;
            }

        }


        void OnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            switch (e.EventType)
            {
                case SerialError.Frame:
                    break;
                case SerialError.Overrun:
                    break;
                case SerialError.RXOver:
                    break;
                case SerialError.RXParity:
                    break;
                case SerialError.TXFull:
                    break;
            }
        }


        void OnSerialPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            switch (e.EventType)
            {
                case SerialPinChange.Break:
                    break;
                case SerialPinChange.CDChanged:
                    break;
                case SerialPinChange.CtsChanged:
                    break;
                case SerialPinChange.DsrChanged:
                    break;
                case SerialPinChange.Ring:
                    break;
            }
        }


        protected virtual void SetDTRValue(bool value)
        {
            serialPort.DtrEnable = value;
        }


        public void LoaderUserTemplates(UserClassTemplate[] userTemplates)
        {
            _userTemplates = userTemplates;
        }


        private UserClassTemplate GetUserTemplateByBadgeId(string badgeId)
        {
            foreach(UserClassTemplate userTemplate in _userTemplates)
            {
                if (userTemplate.BadgeReaderID.Equals(badgeId))
                    return userTemplate;
            }

            return null;
        }
    }



    public class BadgeReaderArgs : EventArgs
    {
        public string DeviceSerial { get; private set; }
        public bool IsMaster { get; set; }
        public UserClassTemplate UserTemplate { get; private set; }
        public AccessBagerReaderType AccessType { get; private set; }

        public BadgeReaderArgs(string deviceSerial, UserClassTemplate userTemplate, AccessBagerReaderType accessType)
        {
            DeviceSerial = deviceSerial;
            UserTemplate = userTemplate;
            AccessType = accessType;
            IsMaster = true;
        }
    }
}

