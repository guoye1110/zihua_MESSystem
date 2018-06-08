using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LabelPrint.Util;
using LabelPrint.PrintForms;
using LabelPrint.NetWork;

using LabelPrint.Data;
namespace LabelPrint.PrintForms
{
    public partial class RuKuForm : Form
    {

        RuKuInputData UserInput;
        //出入库工序
        private const int COMMUNICATION_TYPE_WAREHOUE_OUT_START = 0xB5;  //printing SW started and ask for material info, server send material info for all feeding machine to printing SW 
        private const int COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE = 0xB6;  //printing machine send barcode info to server whever a stack of material is moved out of the warehouse
        private const int COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE = 0xB7;  //printing machine send barcode info to server whever a stack of material is moved into the warehouse
        private FilmSocket m_FilmSocket;
        FilmSocket.networkstatehandler m_networkstatehandler;


        SerialPort serialPort2;  //扫描枪
        public RuKuForm(FilmSocket filmsocket)
        {
            InitializeComponent();
            m_FilmSocket = filmsocket;
        }


        ~RuKuForm()
        {
            m_FilmSocket.network_state_event -= m_networkstatehandler;
        }

        public void network_status_change(bool status)
        {
            Console.WriteLine("network changed to {0}", status);
        }

        void initSerialPort()
        {
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;

            try
            {
                serialPort2 = new SerialPort(SysSetting.CurSettingInfo.ScannerSerialPort, 9600, Parity.None, 8, StopBits.One);
                serialPort2.Open();
                serialPort2.DataReceived += new SerialDataReceivedEventHandler(serialDataReceived2);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open serial port failed!" + ex);
            }
        }

        void serialDataReceived2(object sender, SerialDataReceivedEventArgs e)
        {
            int len;
            string str;
            Byte[] serialDataBuf1 = new Byte[128];
			string[] barcodeSplitted;

            try
            {
                Thread.Sleep(1000);

                //remove last byte of \n
                len = serialPort2.Read(serialDataBuf1, 0, serialPort2.BytesToRead) - 1;
                str = System.Text.Encoding.ASCII.GetString(serialDataBuf1, 0, len);
                this.Invoke((EventHandler)(delegate
                {
                	barcodeSplitted = str.Split(';');

					if (barcodeSplitted.Length == 5) {
                        //first time ruku scan, put scanned content on screen
                    	label1.Text = str;
						HandleBarcode(label1.Text);
					} else {
                        //second time scan, need to upload scanned dats to server
                        ToServer_material_in_upload(str);
					}
                }));
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void bt_Record_Click(object sender, EventArgs e)
        {

        }

        //返回值：		 0：	成功
        //			-1：通讯失败
        private int ToServer_material_in_upload(string str)
        {
            //<原料代码>;<原料批次号>;<目标设备号>;<料仓号>;<重量>
            //string str = cb_RawMaterialCode.Text + ";" + tb_RawMaterialBachNo.Text + ";";
			//str += cb_TargetMachineNo.Text + ";" + tb_BenCiChuKuWeight.Text;
            byte[] send_buf = System.Text.Encoding.Default.GetBytes(str);

            m_FilmSocket.sendDataPacketToServer(send_buf, COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE, send_buf.Length);

            return 0;
            //return m_FilmSocket.RecvResponse(1000);
        }

        private void RuKuForm_Load_1(object sender, EventArgs e)
        {
            UserInput = new RuKuInputData();
            UserInput.CreateDataTable();

            //BarCodeHook.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            //BarCodeHook.Start();

            //InitComponentsArray();

            tb_DateTime.Enabled = false;
            tb_WorkerNo.Text = gVariable.userAccount;
            UserInput.WorkerNo = gVariable.userAccount;
            tb_WorkerNo.Enabled = false;

            //rb_button1.Checked = true;
            //SetRadioComStateByLiaoCangNo(1);
            //rb_button1.Checked = true;
            UserInput.LiaoCangNo = "1";
            //object[] combosStrs = UserInput.GetComboStrsForMaterialName();
            //if (combosStrs != null)
            //    cb_RawMaterialName.Items.AddRange(combosStrs);

            object[] combosStrs = UserInput.GetComboStrsForMaterialCode();
            if (combosStrs != null)
            {
                cb_RawMaterialCode.Items.AddRange(combosStrs);
            }

            cb_TargetMachineNo.Items.AddRange(RuKuInputData.targets);
            cb_TargetMachineNo.SelectedIndex = 0;
            cb_LiaoCangNo.Items.AddRange(RuKuInputData.LiaoCangNoStrs);
            cb_LiaoCangNo.SelectedIndex = 0;
            //m_networkstatehandler = new FilmSocket.networkstatehandler(network_status_change);
            //m_FilmSocket.network_state_event += m_networkstatehandler;
            //m_networkdatahandler = new FilmSocket.networkdatahandler(network_data_received);
            //m_FilmSocket.network_data_event += m_networkdatahandler;


            initSerialPort();
        }

        private void bt_printLabel_Click(object sender, EventArgs e)
        {
            //update the data from UI or Scale.
            UpdateProductData();


            //  if (!UserInput.CheckUserInput())
            //      return;

            UserInput.UpdateDateTime();
            tb_DateTime.Text = UserInput.GetDateTime();

            //Printing
            PrintLabel();
            //save the data to local database
            CreateLocalDataBaseItem();
            //Save the data to server
            SendItemToServer();
            PostUpdateProductData();
        }


        private void UpdateUserInput()
        {

            UserInput.WorkerNo = tb_WorkerNo.Text;
            UserInput.Desc = tb_Desc.Text;

            UserInput.TargetMachineNo = cb_TargetMachineNo.Text;


            UserInput.RawMaterialCode = cb_RawMaterialCode.Text;
            UserInput.RawMaterialBatchNo = tb_RawMaterialBachNo.Text;
            UserInput.XuQiuWeight = tb_XuQiuWeight.Text;
            UserInput.YiChuKuWeight = tb_YiChuKuWeight.Text;
            UserInput.BenCiChuKuWeight = tb_BenCiChuKuWeight.Text;
            int index;
            index = cb_LiaoCangNo.SelectedIndex + 1;
            UserInput.LiaoCangNo = index.ToString();
        }



        private void UpdateProductData()
        {
            UpdateUserInput();
        }

        private void PrintLabel()
        {
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;

            SysSetting.DynPrintData = new DynamicPrintLabelData();
            DynamicPrintLabelData DynPrintData = SysSetting.DynPrintData;
            UserInput.UpdatePrintPrintData(DynPrintData);
            UserInput.PrintLabel();
        }

        private void CreateLocalDataBaseItem()
        {
            UserInput.insertOneRowMSateZero();
        }
        private void SendItemToServer()
        {

        }
        private void PostUpdateProductData()
        {

        }


        private void cb_RawMaterialCode_SelectionChangeCommitted(object sender, EventArgs e)
        {

        }

        private void RuKuForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort2 != null)
            {
                serialPort2.Close();
                serialPort2 = null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort2 != null)
            {
                serialPort2.Close();
                serialPort2 = null;
            }

            OutBoundingForm f = new OutBoundingForm(m_FilmSocket);
            f.ShowDialog();
            this.Close();
        }

        private void HandleBarcode(String barcode)
        {
            //String WorkNo;
            //String BatchNo;
            //String BigRollNo;
			string[] barcodeSplited; 

            if (barcode == null || barcode == "")
                return;

			barcodeSplited = barcode.Split(';');
			cb_RawMaterialCode.Text = barcodeSplited[0];
			tb_RawMaterialBachNo.Text = barcodeSplited[1];
            cb_TargetMachineNo.Text = RuKuInputData.targets[Convert.ToUInt16(barcodeSplited[2]) - 1];
            cb_LiaoCangNo.Text = RuKuInputData.LiaoCangNoStrs[Convert.ToUInt16(barcodeSplited[3]) - 1];
			//tb_XuQiuWeight.Text = barcodeSplited[4];
			
            //if (!UserInput.ParseOutBoundingBarcode(barcode, out WorkNo, out BatchNo, out BigRollNo))
              //  return;



            //tb_WorkNo.Text = UserInput.WorkNo = WorkNo;
            //tb_BatchNo1.Text = UserInput.BatchNo = BatchNo;
            //tb_BigRollNo.Text = UserInput.BigRollNo = BigRollNo;
        }
    }
}
