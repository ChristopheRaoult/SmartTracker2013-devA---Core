using System;
using System.IO.Ports;


namespace smartTracker
{
    public delegate void NotifyHandlerWeightScaleDelegate(Object sender, string deviceSerial, string info);
    public class SartoriusScale
    {
        public event NotifyHandlerWeightScaleDelegate NotifyWeightEvent;
        const char CharEndOfFrameHf = (char)0x0A;
        private SerialPort _serialPort;
        private string _inboundBuffer = string.Empty;
        private string _dataRead = string.Empty;
        private readonly string _deviceSerial = null;
        private string _lastSerialRead = null;
        private string _lastModelRead = null;
        private readonly string _strCom = string.Empty;
        public string Com { get { return _strCom; } }
        public SartoriusScale(string strCom)
        {
            if (!string.IsNullOrEmpty(strCom))
            {
                _strCom = strCom;
                OpenPort(strCom);                
            }
        }
        ~SartoriusScale()
        {
            if (_serialPort != null)
            {
                ClosePort();
                _serialPort.Dispose();
            }
        }
        public void ClosePort()
        {
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.DataReceived -= new SerialDataReceivedEventHandler(OnDataReceived);
                    _serialPort.ErrorReceived -= new SerialErrorReceivedEventHandler(OnErrorReceived);
                    _serialPort.PinChanged -= new SerialPinChangedEventHandler(OnSerialPinChanged);

                    GC.ReRegisterForFinalize(_serialPort.BaseStream);
                    _serialPort.DiscardInBuffer();
                    _serialPort.DiscardOutBuffer();
                    _serialPort.Close();
                }
                _serialPort.Dispose();
                _serialPort = null;
            }
           

        }
        public void OpenPort(string strCom)
        {

            string[] ports = SerialPort.GetPortNames();

            bool bExist = false;
            foreach (string pt in ports)
                if (pt.Equals(strCom))
                {
                    bExist = true;
                    break;
                }

            if (!bExist) return;

            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
                _serialPort = null;
            }
            try
            {
                _serialPort = new SerialPort(strCom, 1200, Parity.Odd, 7, StopBits.One);
                _serialPort.DtrEnable = true;
                _serialPort.Handshake = Handshake.None;
                _serialPort.Open();
                if (_serialPort.IsOpen)
                {
                    GC.SuppressFinalize(_serialPort.BaseStream);
                    _serialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
                    _serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(OnErrorReceived);
                    _serialPort.PinChanged += new SerialPinChangedEventHandler(OnSerialPinChanged);
                }
            }
            catch (Exception)
            {
                //ErrorMessage.ExceptionMessageBox.Show(exp);
            }
        }
        private ScaleResult _lastScaledWeight = null;
        public ScaleResult LastScaledWeight { get { return _lastScaledWeight; } }
        Command _lastCmd;

        public bool IsConnected { get { return _serialPort.IsOpen; } }

        public enum Command
        {
            GetSerial = 0x00,
            GetModel = 0x01,
            GetWeight = 0x02,
            SetTare = 0x03,

        }

        protected virtual void SendMessage(byte[] message, int size)
        {
            if (_serialPort == null) return;
            if (!_serialPort.IsOpen) return;
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();            
            _serialPort.Write(message, 0, size);
            System.Threading.Thread.Sleep(10);
        }

        public void GetWeight()
        {
            _lastCmd = Command.GetWeight;
            byte[] buffer;
            buffer = new byte[2];
            buffer[0] = 0x1B;
            buffer[1] = 0x50;
            _lastScaledWeight = null;
            SendMessage(buffer, buffer.Length);
        }

        /*public string getSerial()
        {
            lastCmd = Command.GetSerial;
            lastSerialRead = null;
            byte[] buffer;
            buffer = new byte[4];
            buffer[0] = 0x1B;
            buffer[1] = 0x78;
            buffer[2] = 0x32;
            buffer[3] = 0x5F;
            sendMessage(buffer, buffer.Length);
            System.Threading.Thread.Sleep(250);
            return lastSerialRead;
        }
        public string getModel()
        {
            lastCmd = Command.GetModel;
            lastModelRead = null;
            byte[] buffer;
            buffer = new byte[4];
            buffer[0] = 0x1B;
            buffer[1] = 0x78;
            buffer[2] = 0x31;
            buffer[3] = 0x5F;
            sendMessage(buffer, buffer.Length);
            System.Threading.Thread.Sleep(250);
            return lastModelRead;
        }

        public void setTare()
        {
            lastCmd = Command.SetTare;
            byte[] buffer;
            buffer = new byte[2];
            buffer[0] = 0x1B;
            buffer[1] = 0x54;
            sendMessage(buffer, buffer.Length);
        }*/

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (true)
            {
                if (!_serialPort.IsOpen) return;
                _inboundBuffer += _serialPort.ReadExisting();
                int frameEnd = _inboundBuffer.IndexOf(CharEndOfFrameHf, 0);
                if (frameEnd < 0)
                    break;
                // A full message is available.
                string message = _inboundBuffer.Substring(0, frameEnd - 1);
                _inboundBuffer = _inboundBuffer.Substring(frameEnd + 1);
                _dataRead = message;

                if (_lastCmd == Command.GetWeight)
                    _lastScaledWeight = GetScaleInfo(_dataRead);
                else if (_lastCmd == Command.GetSerial)
                    _lastSerialRead = _dataRead.TrimStart().Substring(0, 8);
                else if (_lastCmd == Command.GetModel)
                    _lastModelRead = _dataRead;
                if (NotifyWeightEvent != null) NotifyWeightEvent(this, _deviceSerial, _dataRead);
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

        ScaleResult GetScaleInfo(string data)
        {
            ScaleResult sr = new ScaleResult();
            sr.DeviceSerial = _deviceSerial;
            if (data.Length != 14) return null;
            string strWeight = data.Substring(0, 10);
            string strUnit = data.Substring(11);

            double value = 0.0;

            if (double.TryParse(strWeight.Replace(" ", "").Replace(",", "."), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out value))
            {
                sr.WeightValue = value;
                sr.WeightEvent = DateTime.Now;

                if (strUnit.Equals("ct "))
                {
                    sr.BValueStable = true;
                    sr.StrUnit = ScaleResult.Unit.Carats;
                }
                else if (strUnit.Equals("g  "))
                {
                    sr.BValueStable = true;
                    sr.StrUnit = ScaleResult.Unit.Grams;
                }
                else
                {
                    sr.BValueStable = false;
                    sr.StrUnit = ScaleResult.Unit.None;
                }

                return sr;
            }
            return null;
        }

        /*public ScaleInfo[] getScaleplugged()
        {
            try
            {
                ArrayList listdev = new ArrayList();
                List<string> ports = GetDevicePortCom();
                foreach (string s in ports)
                {
                    openPort(s);
                    if (serialPort.IsOpen)
                    {
                        deviceSerial = getSerial();
                        if (deviceSerial != null)
                        {
                            ScaleInfo tmpdev = new ScaleInfo();
                            tmpdev.portCom = s;
                            tmpdev.Serial = deviceSerial;
                            listdev.Add(tmpdev);
                        }

                        closePort();
                    }
                }

                if (listdev.Count > 0)
                {
                    int nIndex = 0;
                    ScaleInfo[] arrayDev = new ScaleInfo[listdev.Count];
                    foreach (ScaleInfo dev in listdev)
                        arrayDev[nIndex++] = dev;
                    return arrayDev;
                }
                else
                    return null;
            }
            catch (Exception exp)
            {
                return null;
            }
        }*/

/*
        private List<string> GetDevicePortCom()
        {
            List<string> comPortList = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
            try
            {
                const string VID = "0403";
                const string PID = "6010";
                //ArrayList comPortList = new ArrayList(System.IO.Ports.SerialPort.GetPortNames());

                string pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
                Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
                List<string> comports = new List<string>();
                RegistryKey rk1 = Registry.LocalMachine;
                RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
                foreach (String s3 in rk2.GetSubKeyNames())
                {
                    RegistryKey rk3 = rk2.OpenSubKey(s3);
                    //KB filter on FTDIBUS only
                    if (!rk3.Name.Equals(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum\FTDIBUS"))
                        continue;
                    foreach (String s in rk3.GetSubKeyNames())
                    {
                        if (_rx.Match(s).Success)
                        {
                            RegistryKey rk4 = rk3.OpenSubKey(s);
                            foreach (String s2 in rk4.GetSubKeyNames())
                            {
                                RegistryKey rk5 = rk4.OpenSubKey(s2);
                                RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");

                                string prt = (string)rk6.GetValue("PortName");
                                if (!string.IsNullOrEmpty(prt))
                                {
                                    if (comPortList.Contains(prt))
                                        comports.Add(prt);
                                }
                            }
                        }
                    }
                }

                if ((comports != null) && (comports.Count > 0))
                    return comports;
                else
                    return comPortList;

            }
            catch
            {
                return comPortList;
            }
        }
*/

        public class ScaleResult
        {
            public enum Unit
            {
                None = 0x00,
                Carats = 0x01,
                Grams = 0x02,
            }
            public string DeviceSerial;
            public bool BValueStable;
            public double WeightValue;
            public Unit StrUnit;
            public DateTime WeightEvent;

        }

        public class ScaleInfo
        {
            public string Model;
            public string Serial;
            public string PortCom;
        }
    }
}
