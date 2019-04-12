using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace smartTracker
{
    class PrinterClass
    {
        public enum MAPI_FEED_RETURN_TYPE
        {
            MAPI_FEED_RETURN_ERROR_WRITE_PROCESS = -3,
            MAPI_FEED_RETURN_ERROR_SOCKET = -2,
            MAPI_FEED_RETURN_NO_PRINTER_FOUND = -1,
            MAPI_FEED_RETURN_SUCCEED = 0

        };
        public MAPI_FEED_RETURN_TYPE FeedPrinter(string IP, long Port)
        {
            return (MAPI_FEED_RETURN_TYPE)Feed(IP, Port);
        }
        private MAPI_FEED_RETURN_TYPE Feed(string IP, long Port)
        {

            int nbSend = 0;

            Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), (int)Port);

            try
            {
                _socket.Connect(iep);

                if (_socket.Connected)
                {

                    byte[] buffer = new byte[3];
                    byte[] rdBuf = new byte[512];
                    buffer[0] = 0x1b;
                    buffer[1] = (byte)'f';
                    buffer[2] = (byte)'\n';

                    nbSend = _socket.Send(buffer, buffer.Length, SocketFlags.None);
                    if (nbSend == 3)
                    {
                        _socket.Close();

                        return MAPI_FEED_RETURN_TYPE.MAPI_FEED_RETURN_SUCCEED;
                    }
                    else
                    {
                        _socket.Close();
                        return MAPI_FEED_RETURN_TYPE.MAPI_FEED_RETURN_ERROR_WRITE_PROCESS;
                    }
                }
                else
                {
                    return MAPI_FEED_RETURN_TYPE.MAPI_FEED_RETURN_NO_PRINTER_FOUND;
                }
            }
            catch
            {
                return MAPI_FEED_RETURN_TYPE.MAPI_FEED_RETURN_ERROR_SOCKET;
            }
        }
        public enum MAPI_CANCEL_PRINT_RETURN_TYPE
        {
            MAPI_CANCEL_PRINT_RETURN_ERROR_WRITE_PROCESS = -3,
            MAPI_CANCEL_PRINT_RETURN_ERROR_SOCKET = -2,
            MAPI_CANCEL_PRINT_RETURN_NO_PRINTER_FOUND = -1,
            MAPI_CANCEL_PRINT_RETURN_SUCCEED = 0

        };
        public MAPI_CANCEL_PRINT_RETURN_TYPE CancelActivePrint(string IP, long Port)
        {
            return (MAPI_CANCEL_PRINT_RETURN_TYPE)CancelPrint(IP, Port);
        }
        public MAPI_CANCEL_PRINT_RETURN_TYPE CancelPrint(string IP, long Port)
        {
            int nbSend = 0;

            Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), (int)Port);

            _socket.Connect(iep);

            if (_socket.Connected)
            {

                byte[] buffer = new byte[3];
                byte[] rdBuf = new byte[512];
                buffer[0] = 0x1b;
                buffer[1] = (byte)'c';
                buffer[2] = (byte)'\n';

                nbSend = _socket.Send(buffer, buffer.Length, SocketFlags.None);
                if (nbSend == 3)
                {
                    _socket.Close();
                    return MAPI_CANCEL_PRINT_RETURN_TYPE.MAPI_CANCEL_PRINT_RETURN_SUCCEED;
                }
                else
                {
                    _socket.Close();
                    return MAPI_CANCEL_PRINT_RETURN_TYPE.MAPI_CANCEL_PRINT_RETURN_ERROR_WRITE_PROCESS;
                }
            }
            else
            {
                return MAPI_CANCEL_PRINT_RETURN_TYPE.MAPI_CANCEL_PRINT_RETURN_NO_PRINTER_FOUND;
            }
        }
        public enum MAPI_GETPRINTERSTATUS_TYPE
        {
            MAPI_GETPRINTERSTATUS_RETURN_ERROR_READ_PROCESS = -4,
            MAPI_GETPRINTERSTATUS_RETURN_ERROR_WRITE_PROCESS = -3,
            MAPI_GETPRINTERSTATUS_RETURN_ERROR_SOCKET = -2,
            MAPI_GETPRINTERSTATUS_RETURN_NO_PRINTER_FOUND = -1,
            MAPI_GETPRINTERSTATUS_RETURN_PRINTER_READY = 0,
            MAPI_GETPRINTERSTATUS_RETURN_PRINTER_BUSY = 2,
            MAPI_GETPRINTERSTATUS_RETURN_OFFLINE = 3,
            MAPI_GETPRINTERSTATUS_RETURN_PRINTHEAD_OPEN = 4,
            MAPI_GETPRINTERSTATUS_RETURN_CALIBRATION_ERROR = 5,
            MAPI_GETPRINTERSTATUS_RETURN_OUT_OF_RIBBON = 6,
            MAPI_GETPRINTERSTATUS_RETURN_OUT_OF_PAPER = 7,
            MAPI_GETPRINTERSTATUS_RETURN_UNKNOWN_ERROR = 8
        };
        public MAPI_GETPRINTERSTATUS_TYPE GetStatusPrinter(string IP, long Port)
        {
            return (MAPI_GETPRINTERSTATUS_TYPE)GetPrinterStatus(IP, Port);
        }
        public MAPI_GETPRINTERSTATUS_TYPE GetPrinterStatus(string IP, long Port)
        {
            int nbSend = 0;
            int nbRead = 0;
            Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), (int)Port);

            try
            {
                _socket.Connect(iep);

                if (_socket.Connected)
                {

                    byte[] buffer = new byte[3];
                    byte[] rdBuf = new byte[512];
                    buffer[0] = 0x1b;
                    buffer[1] = (byte)'s';
                    buffer[2] = (byte)'\n';

                    nbSend = _socket.Send(buffer, buffer.Length, SocketFlags.None);
                    if (nbSend == 3)
                    {
                        nbRead = _socket.Receive(rdBuf, 0, rdBuf.Length, SocketFlags.None);
                        switch (nbRead)
                        {
                            case 0:
                                _socket.Close();
                                return MAPI_GETPRINTERSTATUS_TYPE.MAPI_GETPRINTERSTATUS_RETURN_ERROR_READ_PROCESS;


                            case -1:
                                _socket.Close();
                                return MAPI_GETPRINTERSTATUS_TYPE.MAPI_GETPRINTERSTATUS_RETURN_ERROR_SOCKET;

                            default:

                                _socket.Close();

                                if (rdBuf[8] == 'Y') return MAPI_GETPRINTERSTATUS_TYPE.MAPI_GETPRINTERSTATUS_RETURN_PRINTER_BUSY;
                                if (rdBuf[1] == '-') return MAPI_GETPRINTERSTATUS_TYPE.MAPI_GETPRINTERSTATUS_RETURN_PRINTER_READY;
                                if (rdBuf[1] == 'D') return MAPI_GETPRINTERSTATUS_TYPE.MAPI_GETPRINTERSTATUS_RETURN_PRINTHEAD_OPEN;
                                if (rdBuf[1] == 'E') return MAPI_GETPRINTERSTATUS_TYPE.MAPI_GETPRINTERSTATUS_RETURN_CALIBRATION_ERROR;
                                if (rdBuf[1] == 'F') return MAPI_GETPRINTERSTATUS_TYPE.MAPI_GETPRINTERSTATUS_RETURN_OUT_OF_RIBBON;
                                if (rdBuf[1] == 'P') return MAPI_GETPRINTERSTATUS_TYPE.MAPI_GETPRINTERSTATUS_RETURN_OUT_OF_PAPER;
                                if (rdBuf[0] == 'N') return MAPI_GETPRINTERSTATUS_TYPE.MAPI_GETPRINTERSTATUS_RETURN_OFFLINE;

                                break;
                        }
                    }
                    else
                    {
                        _socket.Close();
                        return MAPI_GETPRINTERSTATUS_TYPE.MAPI_GETPRINTERSTATUS_RETURN_ERROR_WRITE_PROCESS;
                    }
                }
                else
                {
                    return MAPI_GETPRINTERSTATUS_TYPE.MAPI_GETPRINTERSTATUS_RETURN_NO_PRINTER_FOUND;
                }
                return MAPI_GETPRINTERSTATUS_TYPE.MAPI_GETPRINTERSTATUS_RETURN_ERROR_SOCKET;
            }
            catch
            {
                return MAPI_GETPRINTERSTATUS_TYPE.MAPI_GETPRINTERSTATUS_RETURN_ERROR_SOCKET;
            }
        }
        public enum MAPI_PRINTTAGUID_RETURN_TYPE
        {
            MAPI_PRINTTAGUID_RETURN_ERROR_WRITE_PROCESS = -3,
            MAPI_PRINTTAGUID_RETURN_ERROR_SOCKET = -2,
            MAPI_PRINTTAGUID_RETURN_NO_PRINTER_FOUND = -1,
            MAPI_PRINTTAGUID_RETURN_SUCCEED = 0,
            MAPI_PRINTTAGUID_RETURN_ERROR_ARGUMENT = 1

        };
        public MAPI_PRINTTAGUID_RETURN_TYPE PrintTag(string IP, int Port, string LabelName, int NbArg, string Arg)
        {
            return (MAPI_PRINTTAGUID_RETURN_TYPE)PrintTagUID(IP, Port, LabelName, NbArg, Arg);

        }
        public MAPI_PRINTTAGUID_RETURN_TYPE PrintTagUID(string IP, int Port, string LabelName, int NbArg, string Arg)
        {

            int nbSend = 0;

            if (NbArg > 99) return MAPI_PRINTTAGUID_RETURN_TYPE.MAPI_PRINTTAGUID_RETURN_ERROR_ARGUMENT;

            string[] strWord = Arg.Split(';');
            string tmp = string.Empty;
            string cmd = string.Format("M l LBL;/iffs/{0}\r\n", LabelName);

            for (int i = 0; i < NbArg; i++)
            {
                tmp = string.Format("R ARG{0};{1}\r\n", i + 1, strWord[i]);
                cmd += tmp;
            }

            tmp = string.Format("A 1\r\n");
            cmd += tmp;

            Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), (int)Port);
            try
            {
                _socket.Connect(iep);

                if (_socket.Connected)
                {

                    byte[] buffer = System.Text.Encoding.ASCII.GetBytes(cmd);


                    nbSend = _socket.Send(buffer, buffer.Length, SocketFlags.None);
                    if (nbSend == buffer.Length)
                    {
                        _socket.Close();
                        return MAPI_PRINTTAGUID_RETURN_TYPE.MAPI_PRINTTAGUID_RETURN_SUCCEED;
                    }
                    else
                    {
                        _socket.Close();
                        return MAPI_PRINTTAGUID_RETURN_TYPE.MAPI_PRINTTAGUID_RETURN_ERROR_WRITE_PROCESS;
                    }
                }
                else
                {
                    return MAPI_PRINTTAGUID_RETURN_TYPE.MAPI_PRINTTAGUID_RETURN_NO_PRINTER_FOUND;
                }
            }
            catch (Exception exp)
            {
                return MAPI_PRINTTAGUID_RETURN_TYPE.MAPI_PRINTTAGUID_RETURN_ERROR_SOCKET;
            }
        }
    }

    public struct PrinterData
    {
        public string PrinterIP;
        public string TemplateName;
        public int port;
        public bool bReady;
    }
}
