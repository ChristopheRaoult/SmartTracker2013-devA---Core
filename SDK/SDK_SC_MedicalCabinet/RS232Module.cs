using System;
using System.IO.Ports;

namespace SDK_SC_MedicalCabinet
{
    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEvent e);
    public class MessageReceivedEvent : EventArgs { public string Message; }

    public class Rs232Module
    {
        private const char CharEndOfFrame = (char)0x0D;
        private SerialPort _serialPort;
        private string _inboundBuffer = "";

        public bool DsrValue { get { return _serialPort.DsrHolding; } }
        public bool IsConnected { get; private set; }

        public event MessageReceivedEventHandler MessageEvent;
        public string LastBadgeRead = "";

        public Rs232Module(string comName)
        {
            IsConnected = false;
            try
            {
                OpenPort(comName);
                IsConnected = true;

            }
            catch (Exception e)
            {
                ErrorMessage.ExceptionMessageBox.Show(e);
            }
        }


        private void OpenPort(string comName)
        {

            string[] ports = SerialPort.GetPortNames();

            bool portExists = false;
            foreach (string port in ports)
                if (port.Equals(comName)) portExists = true;

            if (!portExists) return;

            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
                _serialPort = null;
            }

            _serialPort = new SerialPort(comName, 9600, Parity.None, 8, StopBits.One);
            _serialPort.Open();

            if (_serialPort.IsOpen)
            {
                GC.SuppressFinalize(_serialPort.BaseStream);
                _serialPort.DataReceived += OnDataReceived;
                _serialPort.ErrorReceived += OnErrorReceived;
                _serialPort.PinChanged += OnSerialPinChanged;
            }
        }


        public void ClosePort()
        {
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.DataReceived -= OnDataReceived;
                    _serialPort.ErrorReceived -= OnErrorReceived;
                    _serialPort.PinChanged -= OnSerialPinChanged;

                    GC.ReRegisterForFinalize(_serialPort.BaseStream);
                    _serialPort.Close();
                }
            }

            _serialPort = null;
            System.Threading.Thread.Sleep(2000);
            IsConnected = false;
        }


        public void SendMessage(byte[] message, int size)
        {
            if (_serialPort == null) return;
            if (_serialPort.IsOpen)
                _serialPort.Write(message, 0, size);
            System.Threading.Thread.Sleep(10);
        }


        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (true)
            {
                _inboundBuffer += _serialPort.ReadExisting();
                int frameEnd = _inboundBuffer.IndexOf(CharEndOfFrame, 0);
                if (frameEnd < 0)
                    break;
                // A full message is available.
                string message = _inboundBuffer.Substring(0, frameEnd - 2);
                _inboundBuffer = _inboundBuffer.Substring(frameEnd + 1);
                OnMessageEvent(message);
                //OnMessageReceived(message);
            }
        }


        private void OnMessageEvent(string message)
        {
            LastBadgeRead = (message.Substring(0, 10));
            MessageReceivedEvent msgEvent = new MessageReceivedEvent {Message = message};

            if (MessageEvent != null)
            {
                MessageEvent(this, msgEvent);
            }

            //MessageEvent = null;
        }
        
        
        private void OnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
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


        private void OnSerialPinChanged(object sender, SerialPinChangedEventArgs e)
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


        public void SetDtrValue(bool value)
        {
            _serialPort.DtrEnable = value;
        }
    }
}
