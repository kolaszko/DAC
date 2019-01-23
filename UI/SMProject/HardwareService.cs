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

        public string CurrentPortName { get; set; }
        public bool SetCurrentPortName(string portName)
        {
            var pName =  AllowedPortNames.First(x => x == portName);
            CurrentPortName = pName;
            return InitSerialPortSender(pName);
        }

        public IEnumerable<string> AllowedPortNames => SerialPort.GetPortNames().Where(x=> x.StartsWith("COM")); 

        private SerialPort SerialPort { get; set; }

        private bool InitSerialPortSender(string portName)
        {
            SerialPort = new SerialPort(portName, 9600, Parity.None)
            {
                Handshake = Handshake.None,
                Parity = Parity.None
            };

            try
            {
                SerialPort.Open();
                return true;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }


        public string Send(string data)
        {
            string returnData = "";
            try
            {
                if (SerialPort.IsOpen)
                {
                    try
                    {
                        SerialPort.Write(data);
                        //returnData = SerialPort.ReadLine();
                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine(e);
                    }
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