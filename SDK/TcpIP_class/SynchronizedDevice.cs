using DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public void CanStartScan()
        {           
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
                        return;                  

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
                                    if ((ds == DeviceStatus.DS_DoorOpen) || (ds == DeviceStatus.DS_WaitForScan))
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
                                    else if (ds != DeviceStatus.DS_InScan)
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
                                    else if (ds != DeviceStatus.DS_InScan)
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
                            System.Threading.Thread.Sleep(rnd.Next(1000, 3000));
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(rnd.Next(1000, 3000));
                    }

                }
                while (bQuitValue != bQuitCondition);

            }
        }

        public void CanStartLed()
        {
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
                        return;

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
                            System.Threading.Thread.Sleep(rnd.Next(1000, 3000));
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(rnd.Next(1000, 3000));
                    }

                }
                while (bQuitValue != bQuitCondition);

            }
        }

    }
}
