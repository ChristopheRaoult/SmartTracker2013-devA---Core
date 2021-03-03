using System;
using System.Collections;
using System.Threading;
using ErrorMessage;
using SDK_SC_RfidReader;

namespace SDK_SC_MedicalCabinet
{
    public delegate void NotifyHandlerBadgeReader(Object sender, string badgeId);

    abstract public class Cabinet : IDisposable
    {
        public event NotifyHandlerBadgeReader BadgeReader;

        protected virtual void OnBadgeReader(string badgeId)
        {
            NotifyHandlerBadgeReader handler = BadgeReader;
            if (handler != null)
                handler(this, badgeId);
        }

        public Rs232Module Rs232Module;
       

        protected Thread EventThread;
        protected bool StoppingThread = false;
        protected bool CancelOps = false;
        protected AutoResetEvent BadgeEvent = new AutoResetEvent(false);
        protected AutoResetEvent ThreadEvent = new AutoResetEvent(false);
        protected AutoResetEvent DoorEventNormal = new AutoResetEvent(false);
        protected AutoResetEvent DoorEventAlarm = new AutoResetEvent(false);

        protected string StrPortBadgeAndLCD;

        protected bool DisplayBadge;
        protected string StrBadgeRead = "";

        protected short CptDoor;
        protected bool WaitDoor;
        // protected bool DoorClosed; // TODO : Useless ? Ask Christophe's confirmation
        protected readonly RfidReader CurrenRfidReader = null;

        protected System.Timers.Timer Clock = new System.Timers.Timer();

        protected ArrayList BadgeList = new ArrayList();
        public string BadgeUser { get { return StrBadgeRead; } }

        public Door_Status DoorStatus { get { return CurrenRfidReader.Door_Status; } }


        protected abstract void myDevice_NotifyEvent(object sender, rfidReaderArgs args);


        protected Cabinet(RfidReader currenRfidReader, string strPortBadgeAndLCD)
        {
            Rs232Module = new Rs232Module(strPortBadgeAndLCD);
            MessageReceivedEventHandler msgReceivedEventHandler = UpdateMessage;
            Rs232Module.MessageEvent += msgReceivedEventHandler;

            CurrenRfidReader = currenRfidReader;

            if (CurrenRfidReader != null)
                CurrenRfidReader.NotifyEvent += myDevice_NotifyEvent;

            StrPortBadgeAndLCD = strPortBadgeAndLCD;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            CancelPendingOperations();
            StopThread();
            Rs232Module.ClosePort();
            Thread.Sleep(500);
        }


        private void UpdateMessage(object sender, MessageReceivedEvent e)
        {
            StrBadgeRead = (e.Message.Substring(0, 10));
            if (CurrenRfidReader != null)
            {
                if (CurrenRfidReader.TCPActionPending == false)
                {
                    CurrenRfidReader.UserActionPending = true;
                    DisplayBadge = true;
                    string str = string.Format("------Badge Received : {0} ------", StrBadgeRead);
                    LogToFile.LogMessageToFile(str);
                }
            }
        }


        public void LoadBadges(ArrayList badgeList)
        {
            BadgeList = badgeList;
        }


        public void ClearBadgeList()
        {
            BadgeList.Clear();
        }


        protected void StopThread()
        {
            StoppingThread = true;
        }


        protected void CancelPendingOperations()
        {
            CancelOps = true;
            if (BadgeEvent != null)
                BadgeEvent.Set();
        }


        protected bool ScanDevice()
        {
            if (CurrenRfidReader == null) return false;
            if (!CurrenRfidReader.IsConnected) return false;

            if (CurrenRfidReader.IsInScan)
            {
                CurrenRfidReader.RequestEndScan();
                Thread.Sleep(2000);
            }

            CurrenRfidReader.RequestScan3D(true, true);
            return true;
        }


        protected bool StopScan()
        {
            if (CurrenRfidReader == null) return false;
            if (CurrenRfidReader.IsConnected)
                CurrenRfidReader.RequestEndScan();
            return true;
        }


        protected bool CheckBadge(string strBadge)
        {
            return (BadgeList.Contains(strBadge));
        }
    }
}
