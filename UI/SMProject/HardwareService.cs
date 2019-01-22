using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;

namespace SMProject
{
    public class HardwareService
    {
        public HardwareService()
        {
            CurrentPortName = AllowedPortNames.First();
            InitSerialPortSender(CurrentPortName);
        }

        public string CurrentPortName { get; private set; }
        public void SetCurrentPortName(string portName)
        {
            var pName =  AllowedPortNames.First(x => x == portName);
            CurrentPortName = pName;
            InitSerialPortSender(pName);
        }

        public IEnumerable<string> AllowedPortNames => SerialPort.GetPortNames().Where(x=> x.StartsWith("COM")); 

        private SerialPort SerialPort { get; set; }

        private void InitSerialPortSender(string portName)
        {
            SerialPort = new SerialPort(portName, 9600, Parity.None)
            {
                Handshake = Handshake.None,
                Parity = Parity.None
            };
            SerialPort.Open();
        }


        public string Send(string data)
        {
            string returnData = "";
            try
            {
                if (SerialPort.IsOpen)
                {
                    SerialPort.Write(data);
                    returnData  = SerialPort.ReadLine();
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