using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;

namespace SMProject
{
    public class HardwareService
    {
        private readonly SerialDataReceivedEventHandler dataReceived;

        public HardwareService(SerialDataReceivedEventHandler dataReceived)
        {
            this.dataReceived = dataReceived;
            CurrentPortName = AllowedPortNames.First(x => x == "COM6");
            InitSerialPortSender(CurrentPortName, this.dataReceived);
        }

        public string CurrentPortName { get; set; }

        public IEnumerable<string> AllowedPortNames => SerialPort.GetPortNames().Where(x => x.StartsWith("COM"));
        private SerialPort SerialPort { get; set; }

        public bool SetCurrentPortName(string portName)
        {
            var pName = AllowedPortNames.First(x => x == portName);
            CurrentPortName = pName;
            return InitSerialPortSender(pName, dataReceived);
        }


        private bool InitSerialPortSender(string portName, SerialDataReceivedEventHandler dr)
        {
            SerialPort = new SerialPort(portName, 9600, Parity.None)
            {
                Handshake = Handshake.None,
                Parity = Parity.None
            };
            SerialPort.DataReceived += dr;
            try
            {
                SerialPort.Open();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }


        public string Send(string data)
        {
            var returnData = "";
            try
            {
                if (SerialPort.IsOpen)
                    try
                    {
                        SerialPort.Write(data);
                        returnData = $"Sent '{data}' through {CurrentPortName}." + Environment.NewLine;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return "Unexpected serial port error.";
            }

            return returnData == "" ? "Program did not receive any return message." : returnData;
        }
    }
}