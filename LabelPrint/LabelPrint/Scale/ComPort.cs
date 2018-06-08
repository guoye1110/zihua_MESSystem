using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using Microsoft.Win32;
namespace LabelPrinting.Scale
{
    class ComPort
    {
        private SerialPort Sp = new SerialPort();
        public delegate void HandleInterfaceUpdataDelegate(string text); //委托，此为重点 
        private HandleInterfaceUpdataDelegate interfaceUpdataHandle;

        /// <summary>
        /// 打开端口
        /// </summary>
        /// <returns></returns>
        public Boolean Open()
        {
            try
            {
                if (!Sp.IsOpen)
                {
                    Sp.Open();
                }
                return Sp.IsOpen;
            }
            catch (Exception)
            {
                Sp.Close();
                return false;
            }

        }

        /// <summary>
        /// 关闭端口
        /// </summary>
        /// <returns></returns>
        public Boolean Close()
        {
            if (Sp.IsOpen)
            {
                Sp.Close();
            }
            return !Sp.IsOpen;
        }

        /// <summary>
        /// 端口
        /// </summary>
        public System.IO.Ports.SerialPort SPort
        {
            get
            {
                return Sp;
            }
        }


        public void Sp_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string strTemp = "";
            double iSecond = 0.5;

            DateTime dtOld = System.DateTime.Now;
            DateTime dtNow = System.DateTime.Now;
            TimeSpan dtInter;
            dtInter = dtNow - dtOld;

            int i = Sp.BytesToRead;
            if (i > 0)
            {
                try
                {
                    strTemp = Sp.ReadExisting();
                }
                catch
                { }
                if (strTemp.ToLower().IndexOf("\r") < 0)
                {
                    i = 0;
                }
                else
                {
                 //   this.Invoke(interfaceUpdataHandle, strTemp);
                }
            }
            while (dtInter.TotalSeconds < iSecond && i <= 0)
            {
                dtNow = System.DateTime.Now;
                dtInter = dtNow - dtOld;
                i = Sp.BytesToRead;
                if (i > 0)
                {
                    try
                    {
                        strTemp += Sp.ReadExisting();
                    }
                    catch
                    { }
                    if (strTemp.ToLower().IndexOf("\r") < 0)
                    {
                        i = 0;
                    }
                    else
                    {
                   //     this.Invoke(interfaceUpdataHandle, strTemp);
                    }
                }
            }
            // do null  

        }

        /// <summary> 
        /// 从注册表获取系统串口列表 
        /// </summary> 
        private void GetComList()
        {
            RegistryKey keyCom = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");
            if (keyCom != null)
            {
                string[] sSubKeys = keyCom.GetValueNames();
                //this.comboBox1.Items.Clear();
                foreach (string sName in sSubKeys)
                {
                    string sValue = (string)keyCom.GetValue(sName);
                   // this.comboBox2.Items.Add(sValue);
                }
            }
        }

        private Boolean OpenComPort(String portName, int BaudRate, int Speed)
        {
            //interfaceUpdataHandle = new HandleInterfaceUpdataDelegate(UpdateTextBox);//实例化委托对象 
            Sp.PortName = portName;
            Sp.BaudRate = BaudRate;
            Sp.Parity = Parity.None;
            Sp.StopBits = StopBits.One;
            Sp.DataReceived += new SerialDataReceivedEventHandler(Sp_DataReceived);
            Sp.ReceivedBytesThreshold = 1;
            try
            {
                Sp.Open();

               // ATCommand3("AT+CLIP=1\r", "OK");


               // btPause.Enabled = true;
               // btENT.Enabled = false;
            }
            catch
            {
                return false;
               // MessageBox.Show("端口" + this.comboBox2.Text.Trim() + "打开失败！");
            }
            return true;
        }
    }
}
