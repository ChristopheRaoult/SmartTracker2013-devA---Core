using System;
using System.Collections;
using System.Threading;

using SDK_SC_RfidReader;
using DataClass;
using ErrorMessage;

namespace SDK_SC_MedicalCabinet
{
    public class FridgeCabinet : Cabinet
    {
        public string StrPortTempReader { get; set; }
        private readonly FridgeType _fridgeType = FridgeType.FT_UNKNOWN;

       // private Hashtable _listTagWithChannel;
        //public Hashtable ListTagWithChannel { get { return _listTagWithChannel; } set { _listTagWithChannel = value; } }
        private byte _channelInScan;

        private TempFridgeThreadCarel _tempFridgeReaderCarel;
        private TempFridgeThreadEvercom _tempFridgeReaderEvercom;
        private TempFridgeThreadPT100 _tempFridgeReaderPT100;
        private TempFridgeFanem _tempFridgeFanem;

        public DataFanemInfo GetFridgeFanemInfo
        {
            get { 
                    if ((_tempFridgeFanem != null) && (_tempFridgeFanem.FanemInfo != null))
                    return _tempFridgeFanem.FanemInfo;
                    return null;
                }
        }

        public tempInfo GetTempInfo
        {
            get
            {
                if (_fridgeType == FridgeType.FT_CAREL)
                {
                    if (_tempFridgeReaderCarel == null) return null;
                    return _tempFridgeReaderCarel.TempInfoClass;
                }

                if (_fridgeType == FridgeType.FT_EVERCOM)
                {
                    if (_tempFridgeReaderEvercom == null) return null;
                    return _tempFridgeReaderEvercom.TempInfoClass;
                }
                if (_fridgeType == FridgeType.FT_PT100)
                {
                    if (_tempFridgeReaderPT100 == null) return null;
                    return _tempFridgeReaderPT100.TempInfoClass;
                }
                if (_fridgeType == FridgeType.FT_FANEM)
                {
                    if (_tempFridgeFanem == null) return null;
                    return _tempFridgeFanem.TempInfoClass;
                }

                return null;
            }
        }

        public tempInfo GetPreviousTempInfo
        {
            get
            {
                if (_fridgeType == FridgeType.FT_CAREL)
                {
                    if (_tempFridgeReaderCarel == null) return null;
                    return _tempFridgeReaderCarel.PreviousTempInfoClass;
                }

                if (_fridgeType == FridgeType.FT_EVERCOM)
                {
                    if (_tempFridgeReaderEvercom == null) return null;
                    return _tempFridgeReaderEvercom.PreviousTempInfoClass;
                }
                if (_fridgeType == FridgeType.FT_PT100)
                {
                    if (_tempFridgeReaderPT100 == null) return null;
                    return _tempFridgeReaderPT100.PreviousTempInfoClass;
                }
                if (_fridgeType == FridgeType.FT_FANEM)
                {
                    if (_tempFridgeFanem == null) return null;
                    return _tempFridgeFanem.PreviousTempInfoClass;
                }

                return null;
            }

            set
            {
                if (_fridgeType == FridgeType.FT_CAREL)
                    _tempFridgeReaderCarel.PreviousTempInfoClass = value;

                if (_fridgeType == FridgeType.FT_EVERCOM)
                    _tempFridgeReaderEvercom.PreviousTempInfoClass = value;

                if (_fridgeType == FridgeType.FT_PT100)
                    _tempFridgeReaderPT100.PreviousTempInfoClass = value;
                if (_fridgeType == FridgeType.FT_FANEM)
                    _tempFridgeFanem.PreviousTempInfoClass = value;
            }

        }


        protected override void Dispose(bool disposing)
        {
            if (!disposing) return;

            CancelPendingOperations();
            StopThread();
            Rs232Module.ClosePort();
            Thread.Sleep(500);

            if (_fridgeType == FridgeType.FT_CAREL)
            {
                if (_tempFridgeReaderCarel != null)
                {
                    _tempFridgeReaderCarel.StopThread();
                    Thread.Sleep(500);
                    _tempFridgeReaderCarel = null;
                }
            }

            if (_fridgeType == FridgeType.FT_EVERCOM)
            {
                if (_tempFridgeReaderEvercom == null) return;
                _tempFridgeReaderEvercom.StopThread();
                Thread.Sleep(500);
                _tempFridgeReaderEvercom = null;
            }
            if (_fridgeType == FridgeType.FT_PT100)
            {
                if (_tempFridgeReaderPT100 == null) return;
                _tempFridgeReaderPT100.StopThread();
                Thread.Sleep(500);
                _tempFridgeReaderPT100 = null;
            }
            if (_fridgeType == FridgeType.FT_FANEM)
            {
                if (_tempFridgeFanem == null) return;
                _tempFridgeFanem.StopThread();
                Thread.Sleep(500);
                _tempFridgeFanem = null;
            }
        }


        public FridgeCabinet(RfidReader currenRfidReader, string strPortBadgeAndLCD, string strPortTempReader, FridgeType fridgeType)
            : base(currenRfidReader, strPortBadgeAndLCD)
        {
            StrPortTempReader = strPortTempReader;
            _fridgeType = fridgeType;

            EventThread = new Thread(EventThreadProc) {Name = "SmartCabinet event", IsBackground = true};
            EventThread.Start();

            Clock.Interval = 1000;
            Clock.Elapsed += Timer_Tick;
            Clock.Start();

            if (string.IsNullOrEmpty(strPortTempReader)) return;

            if (fridgeType == FridgeType.FT_CAREL)
            {
                _tempFridgeReaderCarel = new TempFridgeThreadCarel(strPortTempReader);
                _tempFridgeReaderCarel.StartThread();
            }

            if (fridgeType == FridgeType.FT_EVERCOM)
            {
                _tempFridgeReaderEvercom = new TempFridgeThreadEvercom(strPortTempReader);
                _tempFridgeReaderEvercom.StartThread();
            }

            if (fridgeType == FridgeType.FT_PT100)
            {
                _tempFridgeReaderPT100 = new TempFridgeThreadPT100();
                _tempFridgeReaderPT100.StartThread();
            }
            if (fridgeType == FridgeType.FT_FANEM)
            {
                _tempFridgeFanem = new TempFridgeFanem(strPortTempReader);
                _tempFridgeFanem.StartThread();
            }
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
            if (sender != Clock) return;
            if (!DisplayBadge) return;

            Clock.Stop();
            DisplayBadge = false;
            Thread.Sleep(100);

            if (!CurrenRfidReader.TCPActionPending)
            {
                if (CheckBadge(StrBadgeRead))
                {
                   
                    OnBadgeReader(StrBadgeRead);
                    if (BadgeEvent != null)
                        BadgeEvent.Set();
                }

                else
                {
                    //Enable TCP Scan
                    CurrenRfidReader.UserActionPending = false;
                    Clock.Start();
                }
            }
            else
            {
                //Enable TCP Scan
                CurrenRfidReader.UserActionPending = false;
                Clock.Start();
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

                if (DoorEventNormal.WaitOne(10000, false)) continue; // next loop iteration

                WaitDoor = false;
                WaitDoor = false;
                DisplayBadge = false;
                CloseDoor();
				CurrenRfidReader.NotifyRelock();  
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
                    //_listTagWithChannel = new Hashtable();
                    _channelInScan = 1;
                    Clock.Stop();
                    break;

                case rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan:
                case rfidReaderArgs.ReaderNotify.RN_ReaderScanTimeout:
                    break;

                case rfidReaderArgs.ReaderNotify.RN_ScanCompleted:
                case rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan:
                    Thread.Sleep(2000);
                    Clock.Start();
                    break;

                case rfidReaderArgs.ReaderNotify.RN_Door_Closed:
                    if (DoorEventNormal != null)
                     DoorEventNormal.Set();
                    break;

                case rfidReaderArgs.ReaderNotify.RN_Door_Opened:
                    Thread.Sleep(500);
                    CloseDoor();
                    break;

                case rfidReaderArgs.ReaderNotify.RN_ActiveChannnelChange:
                    byte.TryParse(args.Message, out _channelInScan);
                    break;

                case rfidReaderArgs.ReaderNotify.RN_TagAdded:
                    //if (!_listTagWithChannel.ContainsKey(args.Message))
                     //   _listTagWithChannel.Add(args.Message, _channelInScan);
                    break;
            }
        }
    }
}
