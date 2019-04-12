using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections;
using SDK_SC_RfidReader;


namespace SDK_SC_MedicalCabinet
{
    // New Class for Medical cabinet 1 D - 3D and Cathrack.
    public class MedicalCabinet : Cabinet
    {
        //private Hashtable _listTagWithChannel;
        //public Hashtable ListTagWithChannel { get { return _listTagWithChannel; } set { _listTagWithChannel = value; } }
        private byte _channelInScan;
        private readonly bool _isSchroff;
        public Rs232Module Rs232Display;

        public bool bTempReady = false;
        string errmsg = "";
        YTemperature myPT100Probe = null;

        public double TimeZoneOffset = 1.0;

        private bool bDoorJustClosed = false;
        Thread LedThread = null;

        double ambiantTemp = double.MinValue;
        public double AmbiantTemp
        {
            get { return ambiantTemp; }
        }

        public MedicalCabinet(RfidReader currenRfidReader, string strPortBadgeAndLCD, bool isSchroff = false)
            : base(currenRfidReader, strPortBadgeAndLCD)
        {
            EventThread = new Thread(EventThreadProc) { Name = "SmartCabinet event", IsBackground = true };
            EventThread.Start();

            if (currenRfidReader.HardwareVersion.StartsWith("13"))
            {
                this._isSchroff = true;
                Rs232Display = new Rs232Module("COM2");
                InitLCD();
                HideCursor();

                if (YAPI.RegisterHub("usb", ref errmsg) == YAPI.SUCCESS)
                    bTempReady = true;
                else
                {
                    ErrorMessage.ExceptionMessageBox.Show("error init YAPI",null);
                }

            }

            Clock.Interval = 1000;
            Clock.Elapsed += Timer_Tick;
            Clock.Start();
        }
        protected override void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (LedThread != null)
            {
                LedThread.Abort();
                LedThread.Join(1000);
                LedThread = null;
            }

            CancelPendingOperations();
            StopThread();
            Rs232Module.ClosePort();
            Thread.Sleep(500);
            if( _isSchroff)
                Rs232Display.ClosePort();
        }

        public bool ScanDevice(uint dcu, byte axis)
        {
            if ((axis > 16)) return false;
            if ((dcu > 168)) return false;

            if ((!CurrenRfidReader.IsConnected) ||
                ((CurrenRfidReader.IsInScan) && (CurrenRfidReader.Door_Status != Door_Status.Door_Close)))
                return false;

            CurrenRfidReader.DeviceBoard.sendSwitchCommand(1, axis);
            Thread.Sleep(10);
            CurrenRfidReader.RequestScan3D(true, true);
            return true;
        }

        private void Timer_Tick(object sender, EventArgs eArgs)
        {
            ambiantTemp = double.MinValue;
            if (sender != Clock) return;
            if (!DisplayBadge)
            {
                if ((!CurrenRfidReader.IsConnected) || (bDoorJustClosed) ||
                    ((CurrenRfidReader.IsInScan) || (CurrenRfidReader.Door_Status == Door_Status.Door_Open)))
                    return;

                if (bTempReady)
                {
                    if (myPT100Probe == null)
                    {
                        myPT100Probe = YTemperature.FirstTemperature();
                    }
                    if ((myPT100Probe != null) && (myPT100Probe.isOnline()))
                    {
                       ambiantTemp = myPT100Probe.get_currentValue();
                      
                    }
                }

                if (ambiantTemp != double.MinValue)
                {
                    WriteLCDLine(1, " - WAIT FOR USER -");
                    WriteLCDLine(2, " " + GetTime() + " - " + ambiantTemp.ToString("00.0") + "C");
                 
                }
                else
                {
                    WriteLCDLine(1, " - WAIT FOR USER -");
                   WriteLCDLine(2, "      "  + GetTime());
                }
              
                return;
            }

            Clock.Stop();
            DisplayBadge = false;
            WriteLCDLine(1, "       BADGE :");
            WriteLCDLine(2, "     " + StrBadgeRead);
            Thread.Sleep(100);
            if (!CurrenRfidReader.TCPActionPending)
            {

                if (CheckBadge(StrBadgeRead))
                {
                    OnBadgeReader(StrBadgeRead);
                    if (BadgeEvent != null)
                        BadgeEvent.Set();
                    /*
                    if (LedThread == null)
                    {
                        MyThreadHandleForLed threadHandle = new MyThreadHandleForLed(CurrenRfidReader, 500);
                        LedThread = new Thread(new ThreadStart(threadHandle.ThreadLoop));
                        LedThread.Start();
                    }*/
                }

                else
                {
                    WriteLCDLine(1, "       BADGE :");
                    WriteLCDLine(2, " BADGE NOT GRANTED!");
                    Clock.Start();
                    //Enable TCP Scan
                    CurrenRfidReader.UserActionPending = false;

                }
            }
            else
            {
                WriteLCDLine(1, "       INFO  :");
                WriteLCDLine(2, " NETWORK QUERY");
                Clock.Start();
                //Enable TCP Scan
                CurrenRfidReader.UserActionPending = false;
            }
        }

        public void OpenDoor()
        {
            CurrenRfidReader.OpenDoorMaster();
        }


        public void CloseDoor()
        {
            CurrenRfidReader.CloseDoorMaster();
        }


        private void EventThreadProc()
        {
            while (!StoppingThread)
            {
                BadgeEvent.WaitOne();
                if (CancelOps) return;
                Clock.Stop();

                WaitDoor = true;
                OpenDoor();
                Clock.Start();
                CptDoor = 10;
                DoorEventNormal.Reset();

                if (DoorEventNormal.WaitOne(20000, false)) continue; // next loop iteration as the process will continue to classic door event
                if (LedThread != null)
                {
                    LedThread.Abort();
                    LedThread.Join(1000);
                    LedThread = null;
                }
                Thread.Sleep(500);

                WaitDoor = false;
                DisplayBadge = false;
                //if ((CurrenRfidReader != null) && (CurrenRfidReader.IsConnected)) CurrenRfidReader.SetLightPower(300);
                CloseDoor();
                if (CurrenRfidReader != null) CurrenRfidReader.NotifyRelock();
                Clock.Start();
            }
            if (ThreadEvent != null)
                ThreadEvent.Set();
        }

        protected override void myDevice_NotifyEvent(object sender, rfidReaderArgs args)
        {
            switch (args.RN_Value)
            {
                case rfidReaderArgs.ReaderNotify.RN_ScanStarted:
                    bDoorJustClosed = false;
                    _channelInScan = 1;
                   // _listTagWithChannel = new Hashtable();
                    Clock.Stop();
                    WriteLCDLine(1, " - IDENTIFICATION - ");
                    WriteLCDLine(2, " ");
                    break;

                case rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan:
                case rfidReaderArgs.ReaderNotify.RN_ReaderScanTimeout:
                     WriteLCDLine(1, " - IDENTIFICATION - ");
                     WriteLCDLine(2, " ID failure ");
                    break;

                case rfidReaderArgs.ReaderNotify.RN_ScanCompleted:
                case rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan:
                    string tmp = string.Format(" {0:D3} TAG(S) SCANNED", CurrenRfidReader.ReaderData.nbTagScan);
                    WriteLCDLine(1, " - IDENTIFICATION - ");
                    WriteLCDLine(2, tmp);

                    Thread.Sleep(2000);
                    Clock.Start();
                    ClearLCD();
                    break;

                case rfidReaderArgs.ReaderNotify.RN_Door_Closed:
                    if (DoorEventNormal != null)
                     DoorEventNormal.Set();
                    break;

                case rfidReaderArgs.ReaderNotify.RN_Door_Opened:

                    if (LedThread != null)
                    {
                        LedThread.Abort();
                        LedThread.Join(1000);
                        LedThread = null;
                    }
                    Thread.Sleep(3000);
                    //if ((CurrenRfidReader != null) && (CurrenRfidReader.IsConnected)) CurrenRfidReader.SetLightPower(300);
                    CloseDoor();
                      WriteLCDLine(1, "  - INFORMATION - ");
                      WriteLCDLine(2, " WAIT DOOR CLOSING");
                      bDoorJustClosed = true;
                    break;

                case rfidReaderArgs.ReaderNotify.RN_ActiveChannnelChange:
                    byte.TryParse(args.Message, out _channelInScan);
                    break;

                case rfidReaderArgs.ReaderNotify.RN_TagAdded:
                   // if (!_listTagWithChannel.ContainsKey(args.Message))
                    //    _listTagWithChannel.Add(args.Message, _channelInScan);
                    break;
            }
        }
        //Led Flash
        public class MyThreadHandleForLed
        {
            RfidReader device;
            int timeout;
            public MyThreadHandleForLed(RfidReader device, int timeout)
            {
                this.device = device;
                this.timeout = timeout;
            }
            public void ThreadLoop()
            {
                while (Thread.CurrentThread.IsAlive)
                {
                    if (device.IsConnected)
                    {
                        device.DeviceBoard.SetLightDuty(0);
                        Thread.Sleep(timeout);
                        device.DeviceBoard.SetLightDuty(300);
                        Thread.Sleep(200);
                    }
                }
            }
        }

        #region LCD
        private void InitLCD()
        {
            if (!_isSchroff) return;
            byte[] command = new byte[1];
            command[0] = 0xA0;
            Rs232Display.SendMessage(command, command.Length);
        }

        private void HideCursor()
        {
            if (!_isSchroff) return;
            byte[] command = new byte[2];
            command[0] = 0xA3;
            command[1] = 0x0C;
            Rs232Display.SendMessage(command, command.Length);
        }

        private void ClearLCD()
        {
            if (!_isSchroff) return;
            byte[] command = new byte[2];
            command[0] = 0xA3;
            command[1] = 0x01;
            Rs232Display.SendMessage(command, command.Length);
        }

        private void MoveLCDCursor(byte row, byte line)
        {
            if (!_isSchroff) return;
            byte[] command = new byte[3];
            command[0] = 0xA1;
            command[2] = row;
            command[1] = line;
            Rs232Display.SendMessage(command, command.Length);
        }

        private void WriteLCD(string message)
        {
            if (!_isSchroff) return;
            byte[] data = StrToByteArray(message);
            byte[] command = new byte[data.Length + 2];
            data.CopyTo(command, 1);
            command[0] = 0xA2;
            command[data.Length + 1] = 0x00;
            Rs232Display.SendMessage(command, command.Length);
        }

        public void WriteLCDLine(byte line, string str)
        {
            if (!_isSchroff) return;
            string strtmp = str.PadRight(20, ' ');
            MoveLCDCursor(line, 0);
            Thread.Sleep(25);
            WriteLCD(strtmp);
        }

        private static byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        #endregion

        private string GetTime()
        {
            DateTime dt = DateTime.Now;

            if (isSummerTime(dt))
            {
                return dt.AddHours(TimeZoneOffset + 1.0).ToString("HH:mm:ss");
            }
            return dt.AddHours(TimeZoneOffset).ToString("HH:mm:ss");
        }

        private bool isSummerTime(DateTime dt)
        {
            if ((dt > (new DateTime(2014, 3, 30)) && (dt < (new DateTime(2014, 10, 26)))) ||
                (dt > (new DateTime(2015, 3, 29)) && (dt < (new DateTime(2015, 10, 25)))) ||
                (dt > (new DateTime(2016, 3, 27)) && (dt < (new DateTime(2016, 10, 30)))) ||
                (dt > (new DateTime(2017, 3, 26)) && (dt < (new DateTime(2017, 10, 29)))) ||
                (dt > (new DateTime(2018, 3, 25)) && (dt < (new DateTime(2018, 10, 28)))) ||
                (dt > (new DateTime(2019, 3, 31)) && (dt < (new DateTime(2019, 10, 27)))) ||
                (dt > (new DateTime(2020, 3, 29)) && (dt < (new DateTime(2020, 10, 25)))) ||
                (dt > (new DateTime(2021, 3, 28)) && (dt < (new DateTime(2021, 10, 31)))) ||
                (dt > (new DateTime(2022, 3, 27)) && (dt < (new DateTime(2022, 10, 30)))))
                return true;
            return false;
        }


    }


        //Old Class Medical cabinet change as Schroff disappear.
        // New medical cabinet and catrack have not LCD and use classic WAy to detect door event
       /* [DllImport("Kernel32.dll")]
        static extern bool Beep(
            uint dwFreq, uint dwDuration
        );

        private readonly bool _isFridge;
        public double TimeZone { get; set; }
		
        public MedicalCabinet(RfidReader currenRfidReader, string strPortBadgeAndLCD, bool isFridge = false)
            : base(currenRfidReader, strPortBadgeAndLCD)
        {
            _isFridge = isFridge;

            EventThread = new Thread(EventThreadProc) {Name = "SmartCabinet event", IsBackground = true};

            InitLCD();
            HideCursor();

            EventThread.Start();

            Clock.Interval = 1000;
            Clock.Elapsed += Timer_Tick;
            Clock.Start();
        }

        #region LCD
        private void InitLCD()
        {
            if (_isFridge) return;
            byte[] command = new byte[1];
            command[0] = 0xA0;
            Rs232Module.SendMessage(command, command.Length);
        }

        private void HideCursor()
        {
            if (_isFridge) return;
            byte[] command = new byte[2];
            command[0] = 0xA3;
            command[1] = 0x0C;
            Rs232Module.SendMessage(command, command.Length);
        }

        private void ClearLCD()
        {
            if (_isFridge) return;
            byte[] command = new byte[2];
            command[0] = 0xA3;
            command[1] = 0x01;
            Rs232Module.SendMessage(command, command.Length);
        }

        private void MoveLCDCursor(byte row, byte line)
        {
            if (_isFridge) return;
            byte[] command = new byte[3];
            command[0] = 0xA1;
            command[2] = row;
            command[1] = line;
            Rs232Module.SendMessage(command, command.Length);
        }

        private void WriteLCD(string message)
        {
            if (_isFridge) return;
            byte[] data = StrToByteArray(message);
            byte[] command = new byte[data.Length + 2];
            data.CopyTo(command, 1);
            command[0] = 0xA2;
            command[data.Length + 1] = 0x00;
            Rs232Module.SendMessage(command, command.Length);
        }

        public void WriteLCDLine(byte line, string str)
        {
            if (_isFridge) return;
            string strtmp = str.PadRight(20, ' ');
            MoveLCDCursor(line, 0);
            Thread.Sleep(25);
            WriteLCD(strtmp);
        }

        private static byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        #endregion

        private string GetTime()
        {
            DateTime dt = DateTime.Now.AddHours(TimeZone);
            return dt.ToString("HH:mm:ss");
        }


        private void Timer_Tick(object sender, EventArgs eArgs)
        {
            if (sender == Clock)
            {
                if (DisplayBadge)
                {
                    Clock.Stop();
                    DisplayBadge = false;
                    WriteLCDLine(1, "       BADGE :");
                    WriteLCDLine(2, "     " + StrBadgeRead);
                    Thread.Sleep(100);

					OnBadgeReader(StrBadgeRead);
						
                    if (CheckBadge(StrBadgeRead))
                    { 
                        BadgeEvent.Set();
                    }

                    else
                    {
                        WriteLCDLine(1, "       BADGE :");
                        WriteLCDLine(2, " BADGE NOT GRANTED!");
                        Clock.Start();
                    }
                }

                else if (WaitDoor)
                {
                    if (CptDoor > 0)
                    {
                        WriteLCDLine(1, "     DOOR OPEN");
                        string str = string.Format("      WAIT {0:D2}s", CptDoor);
                        WriteLCDLine(2, str);
                        CptDoor--;

                        if (!_isFridge) 
                            if (Rs232Module.DsrValue == false) 
                                DoorEventNormal.Set();
                    }

                    else
                    {
                        Clock.Stop();
                        WaitDoor = false;
                        if (!_isFridge) DoorEventNormal.Set();
                    }
                }

                else
                {
                    WriteLCDLine(1, " - WAIT FOR USER -");
                    WriteLCDLine(2, "      " + GetTime());
                }
            }
        }


        public void OpenDoor()
        {
            Rs232Module.SetDtrValue(true);
            CurrenRfidReader.OpenDoorMaster();
        }

        public void CloseDoor()
        {
            Rs232Module.SetDtrValue(false);
            CurrenRfidReader.OpenDoorSlave();
        }

        private void EventThreadProc()
        {
            while (!StoppingThread)
            {
                BadgeEvent.WaitOne();
                if (CancelOps)
                    return;
                Clock.Stop();

                // DoorClosed = false; // commented until we don't know where it is used (where it is tested)
                WaitDoor = true;
                OpenDoor();
                Thread.Sleep(100);
                CloseDoor();
                Thread.Sleep(500);
                Clock.Start();
                CptDoor = 10;
                DoorEventNormal.Reset();
                DoorEventNormal.WaitOne();
                WaitDoor = false;
                Clock.Stop();

                if (!_isFridge)
                {
                    if (CptDoor <= 0)
                    {
                        while (Rs232Module.DsrValue)
                        {
                            // DoorClosed = true; // commented until we don't know where it is used (where it is tested)
                            WriteLCDLine(1, "------ ALARME ------");
                            WriteLCDLine(2, "    CLOSE DOOR");
                            Beep(1500, 300);
                            Thread.Sleep(100);
                            Beep(2000, 300);
                        }

                    }
                }

                WaitDoor = false;
                DisplayBadge = false;
                
                if (!_isFridge) ScanDevice();
            }

            ThreadEvent.Set();
        }

        protected override void myDevice_NotifyEvent(object sender, rfidReaderArgs args)
        {
            switch (args.RN_Value)
            {
                case rfidReaderArgs.ReaderNotify.RN_ScanStarted:

                    Clock.Stop();
                    WriteLCDLine(1, " - IDENTIFICATION - ");
                    WriteLCDLine(2, " ");
                    break;
                case rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan:
                case rfidReaderArgs.ReaderNotify.RN_ReaderScanTimeout:

                    WriteLCDLine(1, " - IDENTIFICATION - ");
                    WriteLCDLine(2, " ID failure ");

                    break;
                case rfidReaderArgs.ReaderNotify.RN_ScanCompleted:
                case rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan:

                    string tmp = string.Format(" {0:D3} TAG(S) SCANNED", CurrenRfidReader.ReaderData.nbTagScan);
                    WriteLCDLine(1, " - IDENTIFICATION - ");
                    WriteLCDLine(2, tmp);

                    Thread.Sleep(2000);
                    Clock.Start();
                    ClearLCD();

                    break;

                case rfidReaderArgs.ReaderNotify.RN_Door_Closed:
                    DoorEventNormal.Set();
                    break;
            }
        }
    }*/
}
