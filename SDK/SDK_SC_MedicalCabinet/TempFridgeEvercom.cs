using System;
using System.IO.Ports;
using System.Threading;
using DataClass;

namespace SDK_SC_MedicalCabinet
{
    internal class TempFridgeThreadEvercom
    {
        private readonly Modbus _mb = new Modbus();
        private readonly string _strCom;
        private volatile bool _isDefrostStatusReceive;
        private volatile bool _isTempReceive;
        private bool _defrostActive;
        private Thread _eventThread;
        public bool IsRunning = false;
        private tempInfo _previousTempInfoClass;
        private bool _stop;
        public double TempBottle = 0.0;
        public double TempChamber = 0.0;

        private tempInfo _tempInfoClass;

        public TempFridgeThreadEvercom(string strCom)
        {
            _strCom = strCom;
        }

        public bool DefrostActive
        {
            get { return _defrostActive; }
        }

        public tempInfo TempInfoClass
        {
            get { return _tempInfoClass; }
        }

        public tempInfo PreviousTempInfoClass
        {
            get { return _previousTempInfoClass; }
            set { _previousTempInfoClass = value; }
        }

        ~TempFridgeThreadEvercom()
        {
            if (_eventThread != null)
            {
                if (_eventThread.IsAlive)
                    _eventThread.Abort();
            }
            if (_mb != null)
                _mb.Close();
        }

        public void AddTemp(DateTime dt, double value)
        {
            lock (_lockObj)
            {
                _tempInfoClass.lastTempValue = value;
                _tempInfoClass.lastTempAcq = dt;
                _tempInfoClass.tempArray.Add(dt.Minute, value);
                _tempInfoClass.tempChamber.Add(dt.Minute, value);
                _tempInfoClass.tempBottle.Add(dt.Minute, value);
                _tempInfoClass.nbValueTemp++;
                _tempInfoClass.sumTemp += value;
                _tempInfoClass.mean = _tempInfoClass.sumTemp/_tempInfoClass.nbValueTemp;
                if (_tempInfoClass.min > value)
                    _tempInfoClass.min = value;

                if (_tempInfoClass.max < value)
                    _tempInfoClass.max = value;
            }
        }

        public void StoreTemp()
        {
            lock (_lockObj)
            {
                if ((_tempInfoClass != null) && (_tempInfoClass.nbValueTemp > 0))
                    _previousTempInfoClass = _tempInfoClass;
            }
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
                    _tempInfoClass = new tempInfo();


                _isTempReceive = false;
                _isDefrostStatusReceive = false;

                if (_mb.Open(_strCom, 9600, 8, Parity.Even, StopBits.One))
                {
                    _isTempReceive = _mb.GetFridgeTemp(ref TempChamber, ref TempBottle);
                    _isDefrostStatusReceive = _mb.GetFridgeDefrostStatus(ref _defrostActive);
                    _mb.Close();
                }

                if (_isTempReceive)
                {
                    if (_tempInfoClass == null) _tempInfoClass = new tempInfo();
                    AddTemp(dt, TempBottle);
                }
                if (_isDefrostStatusReceive)
                {
                    if (_tempInfoClass == null) _tempInfoClass = new tempInfo();
                    _tempInfoClass.DefrostActive = DefrostActive;
                    if (DefrostActive) _tempInfoClass.WasInDefrost = true;
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
            if (_mb != null)
                _mb.Close();
        }

        #endregion
    }

    internal class Modbus
    {
        private readonly SerialPort _sp = new SerialPort();
        public string ModbusStatus;

        #region Constructor / Deconstructor

        #endregion

        #region Open / Close Procedures

        public bool Open(string portName, int baudRate, int databits, Parity parity, StopBits stopBits)
        {
            //Ensure port isn't already opened:
            if (!_sp.IsOpen)
            {
                //Assign desired settings to the serial port:
                _sp.PortName = portName;
                _sp.BaudRate = baudRate;
                _sp.DataBits = databits;
                _sp.Parity = parity;
                _sp.StopBits = stopBits;
                //These timeouts are default and cannot be editted through the class at this point:
                _sp.ReadTimeout = 1000;
                _sp.WriteTimeout = 1000;

                try
                {
                    _sp.Open();
                }
                catch (Exception err)
                {
                    ModbusStatus = "Error opening " + portName + ": " + err.Message;
                    return false;
                }
                ModbusStatus = portName + " opened successfully";
                return true;
            }
            ModbusStatus = portName + " already opened";
            return false;
        }

        public bool Close()
        {
            //Ensure port is opened before attempting to close:
            if (_sp.IsOpen)
            {
                try
                {
                    _sp.Close();
                }
                catch (Exception err)
                {
                    ModbusStatus = "Error closing " + _sp.PortName + ": " + err.Message;
                    return false;
                }
                ModbusStatus = _sp.PortName + " closed successfully";
                return true;
            }
            ModbusStatus = _sp.PortName + " is not open";
            return false;
        }

        #endregion

        #region CRC Computation

        private static void GetCRC(byte[] message, ref byte[] crc)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort crcFull = 0xFFFF;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                crcFull = (ushort) (crcFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    char crclsb = (char) (crcFull & 0x0001);
                    crcFull = (ushort) ((crcFull >> 1) & 0x7FFF);

                    if (crclsb == 1)
                        crcFull = (ushort) (crcFull ^ 0xA001);
                }
            }
            crc[1] = (byte) ((crcFull >> 8) & 0xFF);
            crc[0] = (byte) (crcFull & 0xFF);
        }

        #endregion

        #region Build Message

        private void BuildMessage(byte address, byte type, ushort start, ushort registers, ref byte[] message)
        {
            //Array to receive CRC bytes:
            var crc = new byte[2];

            message[0] = address;
            message[1] = type;
            message[2] = (byte) (start >> 8);
            message[3] = (byte) start;
            message[4] = (byte) (registers >> 8);
            message[5] = (byte) registers;

            GetCRC(message, ref crc);
            message[message.Length - 2] = crc[0];
            message[message.Length - 1] = crc[1];
        }

        #endregion

        #region Check Response

        private bool CheckResponse(byte[] response)
        {
            //Perform a basic CRC check:
            var crc = new byte[2];
            GetCRC(response, ref crc);
            return crc[0] == response[response.Length - 2] && crc[1] == response[response.Length - 1];
        }

        #endregion

        #region Get Response

        private void GetResponse(ref byte[] response)
        {
            //There is a bug in .Net 2.0 DataReceived Event that prevents people from using this
            //event as an interrupt to handle data (it doesn't fire all of the time).  Therefore
            //we have to use the ReadByte command for a fixed length as it's been shown to be reliable.
            for (int i = 0; i < response.Length; i++)
            {
                response[i] = (byte) (_sp.ReadByte());
            }
        }

        #endregion

        #region Function 16 - Write Multiple Registers

        public bool SendFc16(byte address, ushort start, ushort registers, short[] values)
        {
            //Ensure port is open:
            if (_sp.IsOpen)
            {
                //Clear in/out buffers:
                _sp.DiscardOutBuffer();
                _sp.DiscardInBuffer();
                //Message is 1 addr + 1 fcn + 2 start + 2 reg + 1 count + 2 * reg vals + 2 CRC
                var message = new byte[9 + 2*registers];
                //Function 16 response is fixed at 8 bytes
                var response = new byte[8];

                //Add bytecount to message:
                message[6] = (byte) (registers*2);
                //Put write values into message prior to sending:
                for (int i = 0; i < registers; i++)
                {
                    message[7 + 2*i] = (byte) (values[i] >> 8);
                    message[8 + 2*i] = (byte) (values[i]);
                }
                //Build outgoing message:
                BuildMessage(address, 16, start, registers, ref message);

                //Send Modbus message to Serial Port:
                try
                {
                    _sp.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    ModbusStatus = "Error in write event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    ModbusStatus = "Write successful";
                    return true;
                }
                ModbusStatus = "CRC error";
                return false;
            }
            ModbusStatus = "Serial port not open";
            return false;
        }

        #endregion

        #region Function 3 - Read Registers

        public bool SendFc3(byte address, ushort start, ushort registers, ref short[] values)
        {
            //Ensure port is open:
            if (_sp.IsOpen)
            {
                //Clear in/out buffers:
                _sp.DiscardOutBuffer();
                _sp.DiscardInBuffer();
                //Function 3 request is always 8 bytes:
                var message = new byte[8];
                //Function 3 response buffer:
                var response = new byte[5 + 2*registers];
                //Build outgoing modbus message:
                BuildMessage(address, 3, start, registers, ref message);
                //Send modbus message to Serial Port:
                try
                {
                    _sp.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    ModbusStatus = "Error in read event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    //Return requested register values:
                    for (int i = 0; i < (response.Length - 5)/2; i++)
                    {
                        values[i] = response[2*i + 3];
                        values[i] <<= 8;
                        values[i] += response[2*i + 4];
                    }
                    ModbusStatus = "Read successful";
                    return true;
                }
                ModbusStatus = "CRC error";
                return false;
            }
            ModbusStatus = "Serial port not open";
            return false;
        }

        #endregion

        #region GetFridgeData

        public bool GetFridgeTemp(ref double tempChamber, ref double tempBottle)
        {
            tempChamber = 0.0;
            tempBottle = 0.0;
            bool ret;
            var values = new short[4];

            const ushort pollStart = 513; // 513 Chamber //517 Botlle
            const ushort pollLength = 4;
            try
            {
                ret = SendFc3(1, pollStart, pollLength, ref values);
            }
            catch
            {
                return false;
            }

            if (ret)
            {
                // 0 tempChamber ,  3 bottle
                tempChamber = values[0]/2.0;
                tempBottle = values[3]/2.0;
                return true;
            }
            return false;
        }

        public bool GetFridgeDefrostStatus(ref bool defrostStatus)
        {
            defrostStatus = false;
            bool ret;
            var values = new short[1];

            const ushort pollStart = 385;
            const ushort pollLength = 1;
            try
            {
                ret = SendFc3(1, pollStart, pollLength, ref values);
            }
            catch
            {
                return false;
            }

            if (ret)
            {
                defrostStatus = (values[0] & 0x10) == 1;
                return true;
            }
            return false;
        }

        #endregion
    }
}