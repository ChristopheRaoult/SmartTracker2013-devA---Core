using DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace TcpIP_class
{
    public  class SynchronizedDevice
    {
        public bool bUseSynchonisation { get; set; }
        public string DeviceIpRight { get; set; }
        public int DevicePortRight { get; set; }
        public string DeviceIpLeft { get; set; }
        public int DevicePortLeft { get; set; }
        public int TimeoutInSec { get; set; }

        private DateTime expiredTime;

       
        public volatile bool bIsWaitingScan = false;
        public void CanStartScan()
        {
            bIsWaitingScan = true;
            if (bUseSynchonisation)
            {
                bool bFirstPass = true;
                int bQuitValue = 0;
                int bQuitCondition = 0;

                int waitRight = 0;
                int waitLeft = 0;
                int doorOpen = 0;
                if (!string.IsNullOrEmpty(DeviceIpRight)) bQuitCondition++;
                if (!string.IsNullOrEmpty(DeviceIpLeft)) bQuitCondition++;
                TcpIpClient tcp = new TcpIP_class.TcpIpClient();
                TcpIpClient.RetCode ret;
                expiredTime = DateTime.Now.AddSeconds(TimeoutInSec); //time to quit in any case

                Random rnd = new Random();
               
                do
                {
                    TimeSpan ts = expiredTime - DateTime.Now;

                    if (!bIsWaitingScan) return;  //quit if requested

                    if (ts.TotalSeconds < 0)
                    {
                        bIsWaitingScan = false;
                        return;
                    }                                   

                    bQuitValue = 0;
                   
                    if (!string.IsNullOrEmpty(DeviceIpLeft)) // J'ai un device à gauche
                    {
                       // if (tcpUtils.PingAddress(DeviceIpLeft, 1000))
                        //{
                        ret = tcp.pingServer(DeviceIpLeft, DevicePortLeft);
                        if (ret == TcpIpClient.RetCode.RC_Succeed)
                        {
                            DeviceStatus ds = DeviceStatus.DS_NotReady;
                            string status;
                            ret = tcp.getStatus(DeviceIpLeft, DevicePortLeft, null, out status);
                            if (ret == TcpIpClient.RetCode.RC_Succeed)
                            {
                                ds = (DeviceStatus)Enum.Parse(typeof(DeviceStatus), status);
                                if ((ds == DeviceStatus.DS_WaitForScan) || (ds == DeviceStatus.DS_WaitForLed))  waitLeft++;
                                else waitLeft = 0;

                                if (bFirstPass)
                                {                                   
                                    if (ds == DeviceStatus.DS_DoorOpen)
                                    {
                                        doorOpen++;
                                        if (doorOpen > 2) bQuitValue++;
                                    }
                                    else if ((ds == DeviceStatus.DS_InScan) || (ds == DeviceStatus.DS_LedOn) || (ds == DeviceStatus.DS_WaitForScan) || (ds == DeviceStatus.DS_WaitForLed))
                                    {
                                        // need to wait 
                                        if (waitLeft > 4) //gauche  en wait depuis long temps on lance
                                            bQuitValue++;
                                    }
                                    else
                                    {                                       
                                        bQuitValue++;
                                    }
                                }
                                else
                                {
                                    if (ds == DeviceStatus.DS_DoorOpen) 
                                    {
                                        doorOpen++;
                                        if (doorOpen > 2) bQuitValue++;
                                    }
                                    else if ((ds == DeviceStatus.DS_InScan) || (ds == DeviceStatus.DS_LedOn) || (ds == DeviceStatus.DS_WaitForScan) || (ds == DeviceStatus.DS_WaitForLed))
                                    {
                                        // need to wait 
                                        if (waitLeft > 4) //gauche  en wait depuis long temps on lance
                                            bQuitValue++;
                                    }
                                    else
                                    {                                       
                                        bQuitValue++;
                                    }

                                }                            
                            }
                            else
                                bQuitValue++;
                        }
                        else
                            bQuitValue++;
                      //  }
                       // else //pas de ping  
                       //     bQuitValue++;
                    }
                    if (!string.IsNullOrEmpty(DeviceIpRight)) // J'ai un device à droite
                    {
                        //if (tcpUtils.PingAddress(DeviceIpRight, 1000))
                        //{
                        ret = tcp.pingServer(DeviceIpRight, DevicePortRight);
                        if (ret == TcpIpClient.RetCode.RC_Succeed)
                        {
                            DeviceStatus ds = DeviceStatus.DS_NotReady;
                            string status;
                            ret = tcp.getStatus(DeviceIpRight, DevicePortRight, null, out status);
                            if (ret == TcpIpClient.RetCode.RC_Succeed)
                            {
                                ds = (DeviceStatus)Enum.Parse(typeof(DeviceStatus), status);
                                if ((ds == DeviceStatus.DS_WaitForScan) || (ds == DeviceStatus.DS_WaitForLed)) waitRight++;
                                else waitRight = 0;
                                if ((ds == DeviceStatus.DS_InScan) || (ds == DeviceStatus.DS_LedOn) || (ds == DeviceStatus.DS_WaitForScan) || (ds == DeviceStatus.DS_WaitForLed))
                                {
                                    // need to wait 
                                    if (waitRight > 2) //droite en wait depuis long temps on lance
                                        bQuitValue++;
                                }
                                else
                                {
                                  
                                    bQuitValue++;
                                }

                            }
                            else
                                bQuitValue++;
                        }
                        else
                            bQuitValue++;
                        //}
                        //else //pas de ping  
                        //    bQuitValue++;
                    }

                    if (bFirstPass)
                    {
                        bFirstPass = false;                       
                    }
                    else
                    {
                        tcpUtils.NonBlockingSleep(rnd.Next(1000, 3000));
                    }

                }
                while (bQuitValue != bQuitCondition);

            }
            bIsWaitingScan = false;

        }

        public volatile bool bIsWaitingLed = false;
        public void CanStartLed()
        {
            bIsWaitingLed = true;
            if (bUseSynchonisation)
            {
                bool bFirstPass = true;
                int bQuitValue = 0;
                int bQuitCondition = 0;

                if (!string.IsNullOrEmpty(DeviceIpRight)) bQuitCondition++;
                if (!string.IsNullOrEmpty(DeviceIpLeft)) bQuitCondition++;
                TcpIpClient tcp = new TcpIP_class.TcpIpClient();
                TcpIpClient.RetCode ret;
                expiredTime = DateTime.Now.AddSeconds(TimeoutInSec); //time to quit in any case

                Random rnd = new Random();
                bool bNeedTreatDoorOpenOnLeft = false;
                do
                {
                    TimeSpan ts = expiredTime - DateTime.Now;

                    if (ts.TotalSeconds < 0)
                    {
                        bIsWaitingLed = false;
                        return;
                    }
                       

                    bQuitValue = 0;

                    if (!string.IsNullOrEmpty(DeviceIpLeft)) // J'ai un device à gauche
                    {
                        // if (tcpUtils.PingAddress(DeviceIpLeft, 1000))
                        //{
                        ret = tcp.pingServer(DeviceIpLeft, DevicePortLeft);
                        if (ret == TcpIpClient.RetCode.RC_Succeed)
                        {
                            DeviceStatus ds = DeviceStatus.DS_NotReady;
                            string status;
                            ret = tcp.getStatus(DeviceIpLeft, DevicePortLeft, null, out status);
                            if (ret == TcpIpClient.RetCode.RC_Succeed)
                            {
                                ds = (DeviceStatus)Enum.Parse(typeof(DeviceStatus), status);
                                if (bFirstPass)
                                {
                                    if ((ds == DeviceStatus.DS_DoorOpen) || (ds == DeviceStatus.DS_WaitForLed))
                                    {
                                        if (!bNeedTreatDoorOpenOnLeft)
                                        {
                                            bNeedTreatDoorOpenOnLeft = true;
                                        }
                                        else
                                        {
                                            bNeedTreatDoorOpenOnLeft = false;
                                            bQuitValue++;
                                        }
                                    }
                                    else if ((ds == DeviceStatus.DS_InScan) || (ds == DeviceStatus.DS_LedOn))
                                    {
                                        // need to wait 
                                    }
                                    else
                                    {
                                        bNeedTreatDoorOpenOnLeft = false;
                                        bQuitValue++;
                                    }
                                }   
                                else
                                {
                                    if (ds == DeviceStatus.DS_DoorOpen)
                                    {
                                        if (!bNeedTreatDoorOpenOnLeft)
                                        {
                                            bNeedTreatDoorOpenOnLeft = true;
                                        }
                                        else
                                        {
                                            bNeedTreatDoorOpenOnLeft = false;
                                            bQuitValue++;
                                        }
                                    }
                                    else if ((ds == DeviceStatus.DS_InScan) || (ds == DeviceStatus.DS_LedOn))
                                    {
                                        //need to wait
                                    }
                                    else
                                    {
                                        bNeedTreatDoorOpenOnLeft = false;
                                        bQuitValue++;
                                    }
                                }
                            }
                            else
                                bQuitValue++;
                        }
                        else
                            bQuitValue++;
                        //  }
                        // else //pas de ping  
                        //     bQuitValue++;
                    }
                    if (!string.IsNullOrEmpty(DeviceIpRight)) // J'ai un device à droite
                    {
                        //if (tcpUtils.PingAddress(DeviceIpRight, 1000))
                        //{
                        ret = tcp.pingServer(DeviceIpRight, DevicePortRight);
                        if (ret == TcpIpClient.RetCode.RC_Succeed)
                        {
                            DeviceStatus ds = DeviceStatus.DS_NotReady;
                            string status;
                            ret = tcp.getStatus(DeviceIpRight, DevicePortRight, null, out status);
                            if (ret == TcpIpClient.RetCode.RC_Succeed)
                            {
                                ds = (DeviceStatus)Enum.Parse(typeof(DeviceStatus), status);
                                if (ds != DeviceStatus.DS_InScan)
                                    bQuitValue++;
                            }
                            else
                                bQuitValue++;
                        }
                        else
                            bQuitValue++;
                        //}
                        //else //pas de ping  
                        //    bQuitValue++;
                    }
                    if (bFirstPass)
                    {
                        bFirstPass = false;
                        if (bNeedTreatDoorOpenOnLeft)
                            tcpUtils.NonBlockingSleep(rnd.Next(1000, 3000));
                    }
                    else
                    {
                        tcpUtils.NonBlockingSleep(rnd.Next(1000, 3000));
                    }

                }
                while (bQuitValue != bQuitCondition);

            }
            bIsWaitingLed = false;
        }
    }
}
