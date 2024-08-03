using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using EasyModbus;
using Microsoft.Win32;
namespace CameraCalibration
{
    public class ModbusFunction
    {
        public bool isConnect = false;

        private ModbusClient client;
        public void Connect(string ip, string port) 
        {
            client = new ModbusClient(ip, Convert.ToInt32(port));
            client.Connect(); // 连接到Modbus服务器
            isConnect = true;
        }
        public void WriteCoils(int ReadNums, bool[] bools)
        {
            client.WriteMultipleCoils(ReadNums, bools);
        }
        public void WriteRegisters(int[] ints)
        {
            client.WriteMultipleRegisters(0, ints);
        }
        public void Disconnect()
        {
            client.Disconnect(); // 断开连接
            isConnect= false;
        }
    }
}