using System;
using System.Collections.Generic;
using System.Text;
using DataClass;

using System.Threading;
namespace SDK_SC_MedicalCabinet
{
    internal class TempFridgeThreadPT100
    {
        string errmsg = "";
        YTemperature myPT100Probe = null;
        private volatile bool _isTempReceive;
        private Thread _eventThread;
        public bool IsRunning = false;
        private tempInfo _previousTempInfoClass;
        private bool _stop;
        public double TempBottle = 0.0;
        public double TempChamber = 0.0;

        private tempInfo _tempInfoClass;

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

        public TempFridgeThreadPT100()
        {  
            if (YAPI.RegisterHub("usb", ref errmsg) != YAPI.SUCCESS)
            {
                ErrorMessage.ExceptionMessageBox.Show("Unable to register PT100 Module :" + errmsg, "Information");
            }          
        }
        ~TempFridgeThreadPT100()
        {
            if (_eventThread != null)
            {
                if (_eventThread.IsAlive)
                    _eventThread.Abort();
            }
           // YAPI.UnregisterHub("usb");
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

                // attente 30eme seconds
                DateTime dt = DateTime.Now;
                if (dt.Second < 15)
                    Thread.Sleep((15 - dt.Second) * 1000);
                else
                    Thread.Sleep(((59 - dt.Second) + 15) * 1000);

                dt = DateTime.Now;

                if (dt.Minute == 00)
                    _tempInfoClass = new tempInfo();


                _isTempReceive = false;

                if (myPT100Probe == null)
                {                   
                    myPT100Probe = YTemperature.FirstTemperature();
                }
                if ((myPT100Probe != null) && (myPT100Probe.isOnline()))
                {
                    TempChamber = TempBottle = myPT100Probe.get_currentValue();
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
                    Thread.Sleep(((59 - dt.Second) + 5) * 1000);
                else
                    Thread.Sleep(5 * 1000);
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
        }
    }
}
