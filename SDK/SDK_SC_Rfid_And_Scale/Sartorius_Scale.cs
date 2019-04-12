
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Collections;


namespace SDK_SC_Rfid_And_Scale
{
    public delegate void NotifyHandlerWeightScaleDelegate(Object sender, string deviceSerial, string info);
    public class Sartorius_Scale
    {
        public event NotifyHandlerWeightScaleDelegate NotifyWeightEvent;
        const char CHAR_EndOfFrameHF = (char)0x0A;
        private SerialPort serialPort;
        private string inboundBuffer = string.Empty;
        private string dataRead = string.Empty;
        private string deviceSerial = null;
        private string lastSerialRead = null;
        private string lastModelRead = null;
        private string strCom = string.Empty;
        public string Com { get { return strCom; } }
        public Sartorius_Scale(string strCom)
        {
            if (!string.IsNullOrEmpty(strCom))
            {
                this.strCom = strCom;
                openPort(strCom);                
            }
        }
        ~Sartorius_Scale()
        {
            if (serialPort != null)
                closePort();
        }
        public void closePort()
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.DataReceived -= new SerialDataReceivedEventHandler(OnDataReceived);
                    serialPort.ErrorReceived -= new SerialErrorReceivedEventHandler(OnErrorReceived);
                    serialPort.PinChanged -= new SerialPinChangedEventHandler(OnSerialPinChanged);

                    GC.ReRegisterForFinalize(serialPort.BaseStream);
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    serialPort.Close();
                }
                serialPort.Dispose();
                serialPort = null;
            }
           

        }
        public void openPort(string strCom)
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

            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
                serialPort = null;
            }
            try
            {
                serialPort = new SerialPort(strCom, 1200, Parity.Odd, 7, StopBits.One);
                serialPort.DtrEnable = true;
                serialPort.Handshake = Handshake.None;
                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    GC.SuppressFinalize(serialPort.BaseStream);
                    serialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
                    serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(OnErrorReceived);
                    serialPort.PinChanged += new SerialPinChangedEventHandler(OnSerialPinChanged);
                }
            }
            catch (Exception exp)
            {
                //ErrorMessage.ExceptionMessageBox.Show(exp);
            }
        }
        private scaleResult lastScaledWeight = null;
        public scaleResult LastScaledWeight { get { return lastScaledWeight; } }
        Command lastCmd;

        public bool IsConnected { get { return serialPort.IsOpen; } }

        public enum Command
        {
            GetSerial = 0x00,
            GetModel = 0x01,
            GetWeight = 0x02,
            SetTare = 0x03,

        }

        protected virtual void sendMessage(byte[] message, int size)
        {
            if (serialPort == null) return;
            if (!serialPort.IsOpen) return;
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();            
            serialPort.Write(message, 0, size);
            System.Threading.Thread.Sleep(10);
        }

        public void getWeight()
        {
            lastCmd = Command.GetWeight;
            byte[] buffer;
            buffer = new byte[2];
            buffer[0] = 0x1B;
            buffer[1] = 0x50;
            lastScaledWeight = null;
            sendMessage(buffer, buffer.Length);
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
                if (!serialPort.IsOpen) return;
                inboundBuffer += serialPort.ReadExisting();
                int frameEnd = inboundBuffer.IndexOf(CHAR_EndOfFrameHF, 0);
                if (frameEnd < 0)
                    break;
                // A full message is available.
                string message = inboundBuffer.Substring(0, frameEnd - 1);
                inboundBuffer = inboundBuffer.Substring(frameEnd + 1);
                dataRead = message;

                if (lastCmd == Command.GetWeight)
                    lastScaledWeight = getScaleInfo(dataRead);
                else if (lastCmd == Command.GetSerial)
                    lastSerialRead = dataRead.TrimStart().Substring(0, 8);
                else if (lastCmd == Command.GetModel)
                    lastModelRead = dataRead;
                if (NotifyWeightEvent != null) NotifyWeightEvent(this, deviceSerial, dataRead);
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
                default:
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
                default:
                    break;
            }
        }

        scaleResult getScaleInfo(string data)
        {
            scaleResult sr = new scaleResult();
            sr.deviceSerial = deviceSerial;
            if (data.Length != 14) return null;
            string strWeight = data.Substring(0, 10);
            string strUnit = data.Substring(11);

            double value = 0.0;

            if (double.TryParse(strWeight.Replace(" ", ""), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out value))
            {
                sr.WeightValue = value;
                sr.WeightEvent = DateTime.Now;

                if (strUnit.Equals("ct "))
                {
                    sr.bValueStable = true;
                    sr.unit = scaleResult.Unit.Carats;
                }
                else if (strUnit.Equals("g  "))
                {
                    sr.bValueStable = true;
                    sr.unit = scaleResult.Unit.Grams;
                }
                else
                {
                    sr.bValueStable = false;
                    sr.unit = scaleResult.Unit.None;
                }

                return sr;
            }
            else
                return null;

        }     

        public class scaleResult
        {
            public enum Unit
            {
                None = 0x00,
                Carats = 0x01,
                Grams = 0x02,
            }
            public string deviceSerial;
            public bool bValueStable;
            public double WeightValue;
            public Unit unit;
            public DateTime WeightEvent;

        }

        public class ScaleInfo
        {
            public string Model;
            public string Serial;
            public string portCom;
        }
    }
}
