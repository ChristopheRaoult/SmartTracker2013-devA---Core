using System;
using System.Collections;
using System.Globalization;
using System.IO.Ports;
using System.Net.Sockets;
using DataClass;
using System.Threading;
using ErrorMessage;
using System.Text;
namespace SDK_SC_MedicalCabinet
{
    internal class TempFridgeFanem
    {
       
        private volatile bool _isTempReceive;
        private Thread _eventThread;
        public bool IsRunning = false;
        private tempInfo _previousTempInfoClass;
        private bool _stop;
        public double TempBottle = 0.0;
        public double TempChamber = 0.0;

        private tempInfo _tempInfoClass;
        private readonly FridgeFanem _tpf = null;

        private readonly object _lockObj = new object();

        public tempInfo TempInfoClass
        {
            get { return _tempInfoClass; }
        }

        public tempInfo PreviousTempInfoClass
        {
            get { return _previousTempInfoClass; }
            set { _previousTempInfoClass = value; }
        }

        public DataFanemInfo FanemInfo
        {
            get
            {
                return _tpf != null ? _tpf.GetFridgeInfo : null;
            }
        }

        public TempFridgeFanem(string strCom)
        {
            _tpf = new FridgeFanem(strCom);
              _tpf.StartThread();

         
        }
        ~TempFridgeFanem()
        {

            if (_eventThread != null)
            {
                if (_eventThread.IsAlive)
                    _eventThread.Abort();
            }

            if( _tpf != null)
                _tpf.StopThread();
         
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
                _tempInfoClass.mean = _tempInfoClass.sumTemp / _tempInfoClass.nbValueTemp;
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

        public void StartThread()
        {
            _stop = false;
            _eventThread = new Thread(EventThreadProc) { Name = "fridge Thread", IsBackground = true };
            _eventThread.Start();
        }
        private void EventThreadProc()
        {
            while (!_stop)
            {
                IsRunning = true;
                try
                {
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


                    if ((_tpf != null) && (_tpf.GetFridgeInfo != null))
                    {
                        TempChamber = TempBottle = _tpf.GetFridgeInfo.GetT0;
                        _isTempReceive = true;
                    }

                    if (_isTempReceive)
                    {
                        if (_tempInfoClass == null) _tempInfoClass = new tempInfo();
                        AddTemp(dt, TempBottle);
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
                catch (ThreadAbortException)
                {
                    
                }
                catch (Exception e)
                {
                    ExceptionMessageBox.Show(e);
                }
            }
            IsRunning = false;
        }
        public void StopThread()
        {
            _tpf.StopThread();
            Thread.Sleep(1000);
            _stop = true;
            if (_eventThread != null)
            {
                if (_eventThread.IsAlive)
                    _eventThread.Abort();
            }           
        }
    
    }

    public class FridgeFanem
    {
        private TcpFanemServer _tfs;
        private const char CharStartOfFrame = '!';
        private const char CharEndOfFrame = '#';

        private readonly string _strCom;
        public bool IsRunning = false;
        private SerialPort _serialPort;
        private Thread _eventThread;
        private bool _stop;
        private string _inboundBuffer = "";

        private readonly DataFanemInfo _dfi;
        public DataFanemInfo GetFridgeInfo {get { return _dfi; }}

        public string ServerString {get { return BuildServerString(_dfi); }}

        public FridgeFanem(string strCom)
        {
            _strCom = strCom;
            _dfi = new DataFanemInfo();
        }

         ~FridgeFanem()
         {
             _stop = true;
               if (_tfs!=null)
                _tfs.StopSocket();
            if (_eventThread != null)
            {
                if (_eventThread.IsAlive)
                    _eventThread.Abort();
            }
            ClosePort();
        }

        #region thread
       
        public void StartThread()
        {
            _stop = false;
            _eventThread = new Thread(EventThreadProc) { Name = "fridge fanem Thread", IsBackground = true };
            _eventThread.Start();

            _tfs = new TcpFanemServer(2101);
            _tfs.StartServer(this);
        }

        private void EventThreadProc()
        {
            try
            {
                while (!_stop)
                {
                    IsRunning = true;
                    if (_serialPort == null) OpenPort(_strCom);
                    if (_serialPort != null && !_serialPort.IsOpen)
                    {
                        ClosePort();
                        OpenPort(_strCom);
                    }
                    RequestData("!I#");
                    int cpt = 100;
                    while (cpt-- > 0)
                    {
                        OnDataReceived(this, null);
                        Thread.Sleep(100);
                    }

                    ClosePort();
                    Thread.Sleep(100);

                }
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception e)
            {
               ExceptionMessageBox.Show(e);
            }
           
        }
        public void StopThread()
        {

            _stop = true;
             if (_tfs!=null)
                _tfs.StopSocket();

            if (_eventThread != null)
            {
                if (_eventThread.IsAlive)
                    _eventThread.Abort();
            }
            ClosePort();
        }

        #endregion

        #region RS232
        public void ClosePort()
        {
            if (_serialPort == null) return;
            if (_serialPort.IsOpen) _serialPort.Close();
            _serialPort.Dispose();
            
        }

        public void OpenPort(string strCom)
        {
            _serialPort = new SerialPort(strCom, 57600, Parity.None, 8, StopBits.One);
            try
            {
                _serialPort.Open();
                //_serialPort.DataReceived += OnDataReceived;
            }
            catch (Exception e)
            {
                ExceptionMessageBox.Show(e);
            }
        }

        public bool RequestData(string cmd)
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    //string cmd = "!SRTp0#";
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    Byte[] bytes = encoding.GetBytes(cmd);
                    _serialPort.Write(bytes, 0, bytes.Length);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                ExceptionMessageBox.Show(e);
            }
            return false;
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (true)
                {
                    _inboundBuffer += _serialPort.ReadExisting();
                    while (_inboundBuffer.Contains("#"))
                    {
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
                        string message = _inboundBuffer.Substring(0, frameEnd + 1);
                        _inboundBuffer = _inboundBuffer.Substring(frameEnd + 1);
                        OnMessageReceived(message);
                    }
                }
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp, "inboundbuffer = " + _inboundBuffer);
            }
        }


        public void OnMessageReceived(string message)
        {
            try
            {

                if (message.StartsWith("!GR"))
                {
                    if (message.Equals("!GRTp#"))
                        RequestData("!SRTp0#");
                    return;
                }
                if (message.StartsWith("!A#")) return;
                if (message.StartsWith("!R#")) return;
         
                if  (message.StartsWith("!SRT0"))
                {
                    GetTemp(message,out _dfi.T0);
                    return;
                }
                if (message.StartsWith("!SRT1"))
                {
                    GetTemp(message, out _dfi.T1);
                    return;
                }
                if (message.StartsWith("!SRT2"))
                {
                    GetTemp(message, out _dfi.T2);
                    return;
                }
                if (message.StartsWith("!SRTT"))
                {
                    GetTemp(message, out _dfi.Tconsigne);
                    return;
                }
                if (message.StartsWith("!SRTH"))
                {
                    GetTemp(message, out _dfi.AlarmHigh);
                    return;
                }
                if (message.StartsWith("!SRTL"))
                {
                    GetTemp(message, out _dfi.AlarmLow);
                    return;
                }
                if (message.StartsWith("!SRTM"))
                {
                    GetTemp(message, out _dfi.MaxT);
                    return;
                }
                if (message.StartsWith("!SRTI"))
                {
                    GetTemp(message, out _dfi.MinT);
                    return;
                }
                if (message.StartsWith("!SRTP"))
                {
                    int val;
                    GetInt(message, 1, out val);
                    _dfi.Door = val == 0 ? DataFanemInfo.doorStatus.Closed : DataFanemInfo.doorStatus.Opened;
                
                    return;
                }
                if (message.StartsWith("!SRTE"))
                {
                    int val;
                    GetInt(message, 1, out val);
                    _dfi.AcPower = val == 0 ? DataFanemInfo.AcPowerStatus.AC_OK : DataFanemInfo.AcPowerStatus.AC_Fault;
                    return;
                }
                if (message.StartsWith("!SRTG"))
                {
                    int val;
                    GetInt(message, 1, out val);
                    _dfi.Defrost = val == 0 ? DataFanemInfo.OnOffStatus.Off : DataFanemInfo.OnOffStatus.On;
                    return;
                }
                if (message.StartsWith("!SRTF"))
                {
                    int val;
                    GetInt(message, 1, out val);
                    _dfi.FlashDrive = val == 0 ? DataFanemInfo.OnOffStatus.Off : DataFanemInfo.OnOffStatus.On;
                    return;
                }
                if (message.StartsWith("!SRTR"))
                {
                    int val;
                    GetInt(message, 1, out val);
                    _dfi.Refrigeration = val == 0 ? DataFanemInfo.OnOffStatus.Off : DataFanemInfo.OnOffStatus.On;
                    return;
                }
                if (message.StartsWith("!SRTh"))
                {
                    GetInt(message,2, out _dfi.Hour);
                    return;
                }
                if (message.StartsWith("!SRTm"))
                {
                    GetInt(message,2, out _dfi.Minute);
                    return;
                }
                if (message.StartsWith("!SRTs"))
                {
                    GetInt(message,2, out _dfi.Second);
                    return;
                }
                if (message.StartsWith("!SRTd"))
                {
                    GetInt(message,2, out _dfi.Day);
                    return;
                }
                if (message.StartsWith("!SRTe"))
                {
                    GetInt(message,2, out _dfi.Month);
                    return;
                }
                if (message.StartsWith("!SRTa"))
                {
                    GetInt(message,2, out _dfi.Year);
                    return;
                }
                if (message.StartsWith("!SRTl"))
                {
                    _dfi.Model = Getmodel(message);
                    return;
                }
                if (message.StartsWith("!SRTi"))
                {
                   _dfi.Serial = message.Substring(5, 9);
                }
          
            }
            catch (Exception e)
            {
               ExceptionMessageBox.Show(e,"message = " + message);
            }
           
        }
        #endregion
        #region utils

        private static bool GetTemp(string message, out double temperature)
        {
            try
            {
                if (message.Length > 10)
                {
                    string strTemp = message.Substring(5, 5);
                    strTemp = strTemp.Insert(4, ".");
                    return double.TryParse(strTemp, NumberStyles.Any, CultureInfo.InvariantCulture, out temperature);
                }
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show("Error GetTemp : " + message + " :" + exp.InnerException.Message, "Error Get Temp");

            }
            temperature = 0.0;
            return false;

        }

        private static bool GetInt(string message, int len , out int val)
        {

            try
            {
                if (message.Length > (5+len))
                {
                    string strVal = message.Substring(5, len);
                    return int.TryParse(strVal, NumberStyles.Number, CultureInfo.InvariantCulture, out val);
                }
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show("Error GetInt : " + message + " :" + exp.InnerException.Message, "Error Get int");

            }
            
            val = 0;
            return false;
        }

        private static string BuildServerString(DataFanemInfo dfi)
        {
            if (dfi == null) return null;
            StringBuilder sb = new StringBuilder();
            // SOF
            sb.Append("2");
            // Model
            sb.Append(dfi.Model);

            //Sign et T0
            sb.Append(dfi.GetT0 >= 0 ? "+" : "-");
            sb.Append((dfi.GetT0 * 10).ToString(CultureInfo.InvariantCulture).PadLeft(4, '0'));

            //Sign et High alarm
            sb.Append(dfi.GetAlarmHigh >= 0 ? "+" : "-");
            sb.Append((dfi.GetAlarmHigh * 10).ToString(CultureInfo.InvariantCulture).PadLeft(4,'0'));

            //Sign et low alarm
            sb.Append(dfi.GetAlarmLow >= 0 ? "+" : "-");
            sb.Append((dfi.GetAlarmLow * 10).ToString(CultureInfo.InvariantCulture).PadLeft(4, '0'));

            //Sign et max Temp
            sb.Append(dfi.GetMaxT >= 0 ? "+" : "-");
            sb.Append((dfi.GetMaxT * 10).ToString(CultureInfo.InvariantCulture).PadLeft(4, '0'));

            //Sign et min Temp
            sb.Append(dfi.GetMinT >= 0 ? "+" : "-");
            sb.Append((dfi.GetMinT * 10).ToString(CultureInfo.InvariantCulture).PadLeft(4, '0'));

            //serial
            sb.Append(dfi.GetSerial);

            // Spare 0 
            sb.Append("0000000");

            //Byte
            byte st = 0;

            if (dfi.Door == DataFanemInfo.doorStatus.Closed)
                st += 1;

            if (dfi.GetAcPower == DataFanemInfo.AcPowerStatus.AC_OK)
                st += 2;

            st += 4; // value soft chamber = 1

            if (dfi.GetT0 < dfi.GetAlarmHigh)
                st += 8;

            if (dfi.GetRefrigeration == DataFanemInfo.OnOffStatus.Off)
                st += 16;

            if (dfi.GetRefrigeration == DataFanemInfo.OnOffStatus.On)
                st += 32;

            st += 64; // value for sound always 1

            if (dfi.GetT0 > dfi.GetAlarmLow)
                st += 128;

            sb.Append(Convert.ToChar(st));

            //EOF
            sb.Append("3");
      
            return sb.ToString();

        }

        private static string Getmodel(string message)
        {
            // Todo ask for other vacina version ?
            if (message.Contains("33470")) return "347a";
            if (message.Contains("33471")) return "347b";
            if (message.Contains("33472")) return "347c";
            if (message.Contains("33473")) return "347d";
            if (message.Contains("33474")) return "347e";
            if (message.Contains("33475")) return "347f";
            if (message.Contains("33476")) return "347g";
            if (message.Contains("33477")) return "347h";
            return null;
        }

        #endregion
    }

    public class TcpFanemServer
    {
        public struct Client
        {
            public Socket Socket;
            public Thread Thread;
            public string ClientName;
            public string Command;
        }

        private ArrayList ListeClients;
        private Thread threadClient;
        private Thread vigile = null;

        private TcpListener myListener = null;
        private Socket socketServeur = null;

        private bool stop = false;

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        private int port;
        private FridgeFanem tff;

        public TcpFanemServer(int port)
        {

            this.port = port;
            ListeClients = new ArrayList();
        }

        public void StartServer(FridgeFanem tff)
        {
            this.tff = tff;
            vigile = new Thread(new ThreadStart(ConnexionClient));
            vigile.IsBackground = true;
            vigile.Start();
        }

        private void ConnexionClient()
        {

            //myListener = new TcpListener(IPAddress.Any, Port);
            myListener = new TcpListener(Port);
            try
            {
                myListener.Start();
            }
            catch (SocketException se)
            {
                // Socket already opened on this port
                myListener.Stop();
                return;
            }

            //TextBoxRefresh("Server Started on port " + Port.ToString(), false, ">");
            while (!stop)
            {
                try
                {
                    socketServeur = myListener.AcceptSocket();
                    threadClient = new Thread(new ThreadStart(EcouteClient));
                    threadClient.Start();
                }
                catch
                {

                }
            }

            myListener.Stop();
        }

        private void EcouteClient()
        {
            string strReceive = null;
            Client myClient = new Client();
            myClient.Thread = threadClient;
            Socket mySocket = socketServeur;
            myClient.Socket = mySocket;
            ListeClients.Add(myClient);
            try
            {
                while (myClient.Socket.Connected)
                {
                    byte[] data = System.Text.Encoding.Default.GetBytes(tff.ServerString);
                    int total = 0;
                    int size = data.Length;
                    int dataleft = size;
                    int sent;
                    while (total < size)
                    {
                        sent = myClient.Socket.Send(data, total, dataleft, System.Net.Sockets.SocketFlags.None);
                        total += sent;
                        dataleft -= sent;

                    }
                    Thread.Sleep(5000);

                }
                for (int i = 0; i < ListeClients.Count; i++)
                {
                    Client c = (Client) ListeClients[i];
                    if (c.Socket == mySocket)
                    {
                        mySocket.Close();
                        ListeClients.RemoveAt(i);

                    }
                }
            }
            catch
            {


            }
        }

        public int ClientNumber()
        {
            return ListeClients.Count;
        }

        public void StopSocket()
        {
            stop = true;
            if (ListeClients.Count != 0)
            {
                for (int n = 0; n < ListeClients.Count; ++n)
                {
                    Client c = (Client) ListeClients[n];
                    c.Socket.Close();
                }
            }
            if (socketServeur != null)
            {
                socketServeur.Close();
                socketServeur = null;
            }

        }
    }

}
