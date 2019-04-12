using System;
using System.Globalization;
using System.IO.Ports;
using System.Threading;
using DataClass;
using ErrorMessage;

namespace SDK_SC_MedicalCabinet
{
    public class TempFridgeThreadCarel
    {
        public enum Command
        {
            Discover = 0x01,
            Force = 0x02,
            Read = 0x03,
            Acq = 0x04,
        }

        private const char CharStartOfFrame = (char) 0x02;
        private const char CharEndOfFrame = (char) 0x03;
        private const byte Adr = 0x31;

        private readonly string _strCom;
        public bool IsRunning = false;
        public Command LastCommand;
        public double Tempfridge = 0.0;
        private volatile bool _defrostActive;
        private Thread _eventThread;
        private string _inboundBuffer = "";
        private volatile bool _isDefrostStatusReceive;
        private volatile bool _isTempReceive;
        private SerialPort _serialPort;
        private bool _stop;

        public TempFridgeThreadCarel(string strCom)
        {
            _strCom = strCom;
        }

        public bool DefrostActive
        {
            get { return _defrostActive; }
        }

        public tempInfo TempInfoClass { get; private set; }
        public tempInfo PreviousTempInfoClass { get; set; }

        ~TempFridgeThreadCarel()
        {
            if (_eventThread != null)
            {
                if (_eventThread.IsAlive)
                    _eventThread.Abort();
            }
            ClosePort();
        }

        #region thread

        private readonly object _lockObj = new object();

        public void StartThread()
        {
            _stop = false;
            _eventThread = new Thread(EventThreadProc) {Name = "fridge Thread", IsBackground = true};
            _eventThread.Start();
        }

        private void EventThreadProc()
        {
            while (!_stop)
            {
                IsRunning = true;

                // attente 30eme seconds
                DateTime dt = DateTime.Now;
                if (dt.Second < 15)
                    Thread.Sleep((15 - dt.Second)*1000);
                else
                    Thread.Sleep(((59 - dt.Second) + 15)*1000);

                dt = DateTime.Now;

                if (dt.Minute == 00)
                    TempInfoClass = new tempInfo();


                _isTempReceive = false;
                _isDefrostStatusReceive = false;
                int cpt = 20;

                OpenPort(_strCom);
                if (_serialPort.IsOpen)
                {
                    SendMessage(Command.Force);
                    Thread.Sleep(500);
                    do
                    {
                        _serialPort.DiscardInBuffer();
                        _serialPort.DiscardOutBuffer();
                        SendMessage(Command.Read);
                        Thread.Sleep(500);
                    } //while ((!IsTempReceive) && (IsDefrostStatusReceive) && (cpt-- > 0));
                    while (cpt-- > 0);


                    if (_isTempReceive)
                    {
                        if (TempInfoClass == null) TempInfoClass = new tempInfo();
                        AddTemp(dt, Tempfridge);
                    }
                    if (_isDefrostStatusReceive)
                    {
                        if (TempInfoClass == null) TempInfoClass = new tempInfo();
                        TempInfoClass.DefrostActive = DefrostActive;
                        if (DefrostActive) TempInfoClass.WasInDefrost = true;
                    }

                    ClosePort();
                    _serialPort.Dispose();
                }

                dt = DateTime.Now;

                if (dt.Minute == 59)
                {
                    StoreTemp();
                }

                if (dt.Second < 59)
                    Thread.Sleep(((59 - dt.Second) + 5)*1000);
                else
                    Thread.Sleep(5*1000);
            }
            IsRunning = false;
        }

        public void StopThread()
        {
            _stop = true;
            if (_eventThread != null)
            {
                if (_eventThread.IsAlive)
                    _eventThread.Abort();
            }
            ClosePort();
        }


        public void AddTemp(DateTime dt, double value)
        {
            lock (_lockObj)
            {
                TempInfoClass.lastTempValue = value;
                TempInfoClass.lastTempAcq = dt;
                TempInfoClass.tempArray.Add(dt.Minute, value);
                TempInfoClass.nbValueTemp++;
                TempInfoClass.sumTemp += value;
                TempInfoClass.mean = TempInfoClass.sumTemp/TempInfoClass.nbValueTemp;
                if (TempInfoClass.min > value)
                    TempInfoClass.min = value;

                if (TempInfoClass.max < value)
                    TempInfoClass.max = value;
            }
        }

        public void StoreTemp()
        {
            lock (_lockObj)
            {
                if ((TempInfoClass != null) && (TempInfoClass.nbValueTemp > 0))
                    PreviousTempInfoClass = TempInfoClass;
            }
        }

        #endregion

        #region RS232

        public void ClosePort()
        {
            if (_serialPort == null) return;
            if (_serialPort.IsOpen) _serialPort.Close();
        }

        public void OpenPort(string strCom)
        {
            _serialPort = new SerialPort(strCom, 19200, Parity.None, 8, StopBits.Two);
            try
            {
                _serialPort.Open();
                _serialPort.RtsEnable = false;

                _serialPort.DataReceived += OnDataReceived;
                //serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(OnErrorReceived);
                //serialPort.PinChanged += new SerialPinChangedEventHandler(OnSerialPinChanged);

                var buffer = new byte[1];

                buffer[0] = 0x55;
                if (_serialPort.IsOpen)
                    _serialPort.Write(buffer, 0, buffer.Length);

                Thread.Sleep(100);
                buffer[0] = 0xAA;
                if (_serialPort.IsOpen)
                    _serialPort.Write(buffer, 0, buffer.Length);
                Thread.Sleep(500);
            }
            catch (Exception e)
            {
                ExceptionMessageBox.Show(e);
            }
        }


        protected void SendMessage(Command command)
        {
            byte[] buffer;
            LastCommand = command;
            switch (command)
            {
                case Command.Discover:
                    buffer = new byte[6];
                    buffer[0] = 0x02;
                    buffer[1] = Adr;
                    buffer[2] = 0x3F;
                    buffer[3] = 0x03;
                    buffer[4] = 0x37;
                    buffer[5] = 0x35;


                    if (_serialPort.IsOpen)
                        _serialPort.Write(buffer, 0, buffer.Length);
                    break;
                case Command.Force:
                    buffer = new byte[7];

                    buffer[0] = 0x02;
                    buffer[1] = Adr;
                    buffer[2] = 0x46;
                    buffer[3] = 0x31;
                    buffer[4] = 0x03;
                    buffer[5] = 0x3A;
                    buffer[6] = 0x3D;


                    if (_serialPort.IsOpen)
                        _serialPort.Write(buffer, 0, buffer.Length);
                    if (_serialPort.IsOpen)
                        _serialPort.Write(buffer, 0, buffer.Length);
                    break;
                case Command.Read:
                    buffer = new byte[2];
                    buffer[0] = 0x05;
                    buffer[1] = Adr;
                    if (_serialPort.IsOpen)
                        _serialPort.Write(buffer, 0, buffer.Length);
                    break;
                case Command.Acq:
                    buffer = new byte[1];
                    buffer[0] = 0x06;
                    if (_serialPort.IsOpen)
                        _serialPort.Write(buffer, 0, buffer.Length);
                    break;
            }
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(100);
            /*inboundBuffer += serialPort.ReadExisting();
            string message = inboundBuffer;
            inboundBuffer = "";
            OnMessageReceived(message);*/

            while (true)
            {
                _inboundBuffer += _serialPort.ReadExisting();
                int frameStart = _inboundBuffer.IndexOf(CharStartOfFrame);
                if (frameStart != 0)
                {
                    if (frameStart < 0)
                    {
                        _inboundBuffer = "";
                        break;
                    }
                    _inboundBuffer = _inboundBuffer.Substring(frameStart);
                }

                int frameEnd = _inboundBuffer.IndexOf(CharEndOfFrame, 0);
                if (frameEnd < 0)
                    break;

                // A full message is available.
                string message = _inboundBuffer.Substring(0, frameEnd + 2);
                _inboundBuffer = _inboundBuffer.Substring(frameEnd + 2);
                OnMessageReceived(message);
            }
        }

        public void OnMessageReceived(string message)
        {
            switch (LastCommand)
            {
                case Command.Read:

                    char type = message[2];
                    int startaddress;
                    int numVar;
                    switch (type)
                    {
                        case 'S':
                            startaddress = message[3] - 0x30;
                            numVar = message[4] - 0x30;
                            int nIndex = 5;
                            int nAdress = startaddress;
                            for (int loop = 0; loop < numVar; loop++)
                            {
                                string valueHex = "";
                                if (message.Length < nIndex + 4) break;
                                valueHex += message[nIndex++];
                                valueHex += message[nIndex++];
                                valueHex += message[nIndex++];
                                valueHex += message[nIndex++];
                                int valueDec = Int32.Parse(valueHex, NumberStyles.HexNumber);
                                if (valueDec > 32767) //il est negatif
                                {
                                    valueDec = valueDec - 65535 - 1;
                                }
                                string value = valueDec.ToString(CultureInfo.InvariantCulture);

                                if (value.StartsWith("-"))
                                {
                                    string strtrimmed = "-" + value.Substring(1).PadLeft(5, '0');
                                    value = strtrimmed.Insert(5, ".");
                                }
                                else
                                {
                                    value = value.PadLeft(5, '0');
                                    value = value.Insert(4, ".");
                                }
                                value = value.Replace('.', ',');
                                if (nAdress == 3)
                                {
                                    Tempfridge = double.Parse(value);
                                    _isTempReceive = true;
                                    break;
                                }
                                nAdress++;
                            }
                            break;
                        case 'B':
                        {
                            startaddress = message[3] - 0x30;
                            numVar = message[4] - 0x30;
                            if (startaddress > 31) break;
                            if (numVar + startaddress > 31) // defrost info here
                            {
                                int rgChar = 5;
                                int indexChar = startaddress;
                                while ((indexChar + 8) < 31)
                                {
                                    indexChar += 8;
                                    rgChar += 2;
                                }
                                string valueHex = "";
                                valueHex += message[rgChar];
                                valueHex += message[rgChar + 1];
                                int valueDec = Int32.Parse(valueHex, NumberStyles.HexNumber);
                                valueDec = valueDec >> (31 - indexChar);
                                _defrostActive = ((valueDec & 0x01) == 1);

                                _isDefrostStatusReceive = true;
                            }
                        }
                            break;
                    }
                    SendMessage(Command.Acq);
                    break;
            }
        }

        /*
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
         * */

        #endregion
    }
}