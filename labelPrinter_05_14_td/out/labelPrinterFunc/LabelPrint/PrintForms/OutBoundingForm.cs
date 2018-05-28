using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabelPrint.Data;
using LabelPrint.Receipt;
using LabelPrint.Util;
using LabelPrint.PrintForms;
using LabelPrint.NetWork;

namespace LabelPrint
{
    public partial class OutBoundingForm : Form
    {
		//出入库工序
		private const int COMMUNICATION_TYPE_WAREHOUE_OUT_START = 0xB5;  //printing SW started and ask for material info, server send material info for all feeding machine to printing SW 
		private const int COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE = 0xB6;	//printing machine send barcode info to server whever a stack of material is moved out of the warehouse
		private const int COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE = 0xB7;  //printing machine send barcode info to server whever a stack of material is moved into the warehouse
		private FilmSocket m_FilmSocket;
		FilmSocket.networkstatehandler m_networkstatehandler;
		FilmSocket.networkdatahandler m_networkdatahandler;
		private int m_lastRsp;
		private bool m_connected;

        const int MAX_LIAOCANG_NUM = 7;

        string[,] m_materialCode = new string[7, MAX_LIAOCANG_NUM];
		string[,] m_materialRequired = new string[7, MAX_LIAOCANG_NUM];
		
        OutBoundingInputData UserInput;



        Boolean bStart = false;

        ComboBox[] cb_RawMaterialCodes = new ComboBox[MAX_LIAOCANG_NUM];
        TextBox[] tb_RawMaterialBachNos = new TextBox[MAX_LIAOCANG_NUM];
        TextBox[] tb_XuQiuWeights = new TextBox[MAX_LIAOCANG_NUM];
        TextBox[] tb_YiChuKuWeights = new TextBox[MAX_LIAOCANG_NUM];
        TextBox[] tb_BenCiChuKuWeights = new TextBox[MAX_LIAOCANG_NUM];

  SerialPort serialPort2;  //扫描枪
        BardCodeHooK BarCodeHook = new BardCodeHooK();

        public OutBoundingForm(FilmSocket filmsocket)
        {
            InitializeComponent();
			m_FilmSocket = filmsocket;
        }
		~OutBoundingForm()
        {
	        m_FilmSocket.network_state_event -= m_networkstatehandler;
			m_FilmSocket.network_data_event -= m_networkdatahandler;
		}

		public void network_status_change(bool status)
        {
        	Console.WriteLine("network changed to {0}", status);
			m_connected = status;
		}

        void InitComponentsArray()
        {
            cb_RawMaterialCodes[0] = cb_RawMaterialCode1;
            cb_RawMaterialCodes[1] = cb_RawMaterialCode2;
            cb_RawMaterialCodes[2] = cb_RawMaterialCode3;
            cb_RawMaterialCodes[3] = cb_RawMaterialCode4;
            cb_RawMaterialCodes[4] = cb_RawMaterialCode5;
            cb_RawMaterialCodes[5] = cb_RawMaterialCode6;
            cb_RawMaterialCodes[6] = cb_RawMaterialCode7;


            tb_RawMaterialBachNos[0] = tb_BachNo1;
            tb_RawMaterialBachNos[1] = tb_BachNo2;
            tb_RawMaterialBachNos[2] = tb_BachNo3;
            tb_RawMaterialBachNos[3] = tb_BachNo4;
            tb_RawMaterialBachNos[4] = tb_BachNo5;
            tb_RawMaterialBachNos[5] = tb_BachNo6;
            tb_RawMaterialBachNos[6] = tb_BachNo7;


            tb_XuQiuWeights[0] = tb_XuQiu1;
            tb_XuQiuWeights[1] = tb_XuQiu2;
            tb_XuQiuWeights[2] = tb_XuQiu3;
            tb_XuQiuWeights[3] = tb_XuQiu4;
            tb_XuQiuWeights[4] = tb_XuQiu5;
            tb_XuQiuWeights[5] = tb_XuQiu6;
            tb_XuQiuWeights[6] = tb_XuQiu7;


            tb_YiChuKuWeights[0] = tb_YiChuKu1;
            tb_YiChuKuWeights[1] = tb_YiChuKu2;
            tb_YiChuKuWeights[2] = tb_YiChuKu3;
            tb_YiChuKuWeights[3] = tb_YiChuKu4;
            tb_YiChuKuWeights[4] = tb_YiChuKu5;
            tb_YiChuKuWeights[5] = tb_YiChuKu6;
            tb_YiChuKuWeights[6] = tb_YiChuKu7;


            tb_BenCiChuKuWeights[0] = tb_BenCiChuKu1;
            tb_BenCiChuKuWeights[1] = tb_BenCiChuKu2;
            tb_BenCiChuKuWeights[2] = tb_BenCiChuKu3;
            tb_BenCiChuKuWeights[3] = tb_BenCiChuKu4;
            tb_BenCiChuKuWeights[4] = tb_BenCiChuKu5;
            tb_BenCiChuKuWeights[5] = tb_BenCiChuKu6;
            tb_BenCiChuKuWeights[6] = tb_BenCiChuKu7;

        }
        void SetRadioComStateByLiaoCangNo(int LiaoCangNo)
        {
            for (int i = 0; i < MAX_LIAOCANG_NUM; i++)
            {
                if (LiaoCangNo == (i + 1))
                {

                    cb_RawMaterialCodes[i].Enabled = true;
                    tb_RawMaterialBachNos[i].Enabled = true;
                    tb_XuQiuWeights[i].Enabled = true;
                    tb_YiChuKuWeights[i].Enabled = true;
                    tb_BenCiChuKuWeights[i].Enabled = true;


                }
                else
                {
                    cb_RawMaterialCodes[i].Enabled = false;
                    tb_RawMaterialBachNos[i].Enabled = false;
                    tb_XuQiuWeights[i].Enabled = false;
                    tb_YiChuKuWeights[i].Enabled = false;
                    tb_BenCiChuKuWeights[i].Enabled = false;
                    cb_RawMaterialCodes[i].Text = null;
                    tb_RawMaterialBachNos[i].Text = null;
                    tb_XuQiuWeights[i].Text = null;
                    tb_YiChuKuWeights[i].Text = null;
                    tb_BenCiChuKuWeights[i].Text = null;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //update the data from UI or Scale.
            UpdateProductData();


            if (!UserInput.CheckUserInput())
                return;

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


        void UpdatUserInputForRadiOBoxByLiaoCangHao()
        {
            int LiaoCangNo;
            Boolean val = int.TryParse(UserInput.LiaoCangNo, out LiaoCangNo);
            if (val)
            {
                UserInput.RawMaterialCode = cb_RawMaterialCodes[LiaoCangNo-1].Text;
                UserInput.RawMaterialBatchNo = tb_RawMaterialBachNos[LiaoCangNo-1].Text;
                UserInput.XuQiuWeight = tb_XuQiuWeights[LiaoCangNo-1].Text;
                UserInput.YiChuKuWeight = tb_YiChuKuWeights[LiaoCangNo-1].Text;
                UserInput.BenCiChuKuWeight = tb_BenCiChuKuWeights[LiaoCangNo-1].Text;
            }


        }

        private void UpdateUserInput()
        {
        //    UserInput.MaterialName = cb_RawMaterialName.Text;
        //    UserInput.RawMaterialGrade = tb_RawMaterialGrade.Text;
       //     UserInput.Vendor = tb_Vendor.Text;
      //      UserInput.WeightPerBag = tb_WeightPerBag.Text;
            // UserInput.StackWeight = tb_StackWeight.Text;
            //UserInput.Bags_x = tb_Bags_x.Text;
            //UserInput.Bags_y = tb_Bags_y.Text;
            //UserInput.Bags_xy = tb_Bags_xy.Text;
            UserInput.WorkerNo = tb_WorkerNo.Text;
            UserInput.Desc = tb_Desc.Text;
            //UserInput.NeedBags = tb_NeedBags.Text;
            //UserInput.OutBags = tb_OutBags.Text;
            UserInput.TargetMachineNo = cb_TargetMachineNo.Text;

            UpdatUserInputForRadiOBoxByLiaoCangHao();


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
            //OutBoundingLabel label = new OutBoundingLabel();
            //label.Printlabel();
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
            //tb_DateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

        }

        private void OutBoundingForm_Load(object sender, EventArgs e)
        {
            UserInput = new OutBoundingInputData();
            UserInput.CreateDataTable();

            BarCodeHook.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            BarCodeHook.Start();

            InitComponentsArray();

            tb_DateTime.Enabled = false;
            tb_WorkerNo.Text = gVariable.userAccount;
            UserInput.WorkerNo = gVariable.userAccount;
            tb_WorkerNo.Enabled = false;

            rb_button1.Checked = true;
            SetRadioComStateByLiaoCangNo(1);
            rb_button1.Checked=true;
            UserInput.LiaoCangNo = "1";
            //object[] combosStrs = UserInput.GetComboStrsForMaterialName();
            //if (combosStrs != null)
            //    cb_RawMaterialName.Items.AddRange(combosStrs);

            object[] combosStrs = UserInput.GetComboStrsForMaterialCode();
            if (combosStrs != null)
            {
                for (int i= 0; i< MAX_LIAOCANG_NUM; i++)
                {
                    cb_RawMaterialCodes[i].Items.AddRange(combosStrs);
                }
                

            }

            cb_TargetMachineNo.Items.AddRange(UserInput.targets);
			m_networkstatehandler = new FilmSocket.networkstatehandler(network_status_change);
			m_FilmSocket.network_state_event += m_networkstatehandler;
			m_networkdatahandler = new FilmSocket.networkdatahandler(network_data_received);
			m_FilmSocket.network_data_event += m_networkdatahandler;

            initSerialPort();
        }

        private void OutBoundingForm_FormClosing(object sender, EventArgs e)
        {
            if (serialPort2 != null)
                serialPort2.Close();
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
            Byte[] serialDataBuf1 = new Byte[128];

            try
            {
                Thread.Sleep(1000);

                serialPort2.Read(serialDataBuf1, 0, serialPort2.BytesToRead);

                this.Invoke((EventHandler)(delegate
                {
                    label1.Text = System.Text.Encoding.ASCII.GetString(serialDataBuf1);
                }));

                ToServer_material_out_upload();
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void tb_Bags_x_KeyPress(object sender, KeyPressEventArgs e)
        {
            ControlHelper.LimitToDigitOnly(e);
        }

        private void cb_RawMaterialName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    UserInput.MaterialName = cb_RawMaterialName.SelectedItem.ToString();
            //    UserInput.GetInfoByMaterialName(UserInput.MaterialName, out UserInput.RawMaterialCode, out UserInput.Vendor, out UserInput.WeightPerBag, out UserInput.NeedBags);
            //}
            //catch (Exception arg)
            //{

            //}
            //tb_Vendor.Text = UserInput.Vendor;
            //tb_RawMaterialCode.Text = UserInput.RawMaterialCode;
            //tb_RawMaterialGrade.Text = "0";
            //tb_NeedBags.Text = UserInput.NeedBags;
            //tb_WeightPerBag.Text = UserInput.WeightPerBag;

            ////                UserInput.ProductCode = (String)productCodes[cb_ProductCode.SelectedIndex];
            ////UserInput.GetInfoByProductCode(UserInput.ProductCode, out UserInput.Width, out UserInput.RecipeCode, out fixture, out UserInput.CustomerName);

            ////tb_Width.Text = UserInput.Width;
            ////tb_RecipeCode.Text = UserInput.RecipeCode;
            ////tb_CustomerName.Text = UserInput.CustomerName;


        }

        private void rb_LiaoCangNoClick(object sender, EventArgs e)
        {
            if (rb_button1.Checked)
            {
                UserInput.LiaoCangNo = "1";
                SetRadioComStateByLiaoCangNo(1);
            }
            else
            if (rb_button2.Checked)
            {
                UserInput.LiaoCangNo = "2";
                SetRadioComStateByLiaoCangNo(2);
            }
            else
            if (rb_button3.Checked)
            {
                UserInput.LiaoCangNo = "3";
                SetRadioComStateByLiaoCangNo(3);
            }
            else
            if (rb_button4.Checked)
            {
                UserInput.LiaoCangNo = "4";
                SetRadioComStateByLiaoCangNo(4);
            }
            else
            if (rb_button5.Checked)
            {
                UserInput.LiaoCangNo = "5";
                SetRadioComStateByLiaoCangNo(5);
            }
            else
            if (rb_button6.Checked)
            {
                UserInput.LiaoCangNo = "6";
                SetRadioComStateByLiaoCangNo(6);
            }
            else
            if (rb_button7.Checked)
            {
                UserInput.LiaoCangNo = "7";
                SetRadioComStateByLiaoCangNo(7);
            }


        }

        delegate void AsynBarCode_BarCodeEvent(BardCodeHooK.BarCodes barCode);
        void BarCode_BarCodeEvent(BardCodeHooK.BarCodes barCode)
        {
            if (this.InvokeRequired)
            {
                Log.d("HH", "bar code Final Barcode=" + barCode.BarCode);
                this.BeginInvoke(new AsynBarCode_BarCodeEvent(BarCode_BarCodeEvent), new object[] { barCode });
            }
            else
            {
                Log.d("HH", "final keyboard =" + barCode.BarCode);
                if (barCode.BarCode != null && barCode.BarCode.Length > 5 && barCode.BarCode.Remove(1) == "`")
                    label1.Text = barCode.BarCode;
            }
        }


        //get material requirement list from server
        private void bt_Record_Click(object sender, EventArgs e)
        {
            ToServer_startwork();
        }

        private void cb_RawMaterialCode1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //UserInput.MaterialName = cb_RawMaterialCode1.SelectedItem.ToString();

            //try
            //{
            //    GetInfoByMaterialCode
            //    //UserInput.GetInfoByMaterialName(UserInput.MaterialName, out UserInput.RawMaterialCode, out UserInput.Vendor, out UserInput.WeightPerBag, out UserInput.NeedBags);
            //}
            //catch (Exception arg)
            //{

            //}
        }

        //material in
        private void button3_Click(object sender, EventArgs e)
        {
            RuKuForm f = new RuKuForm(m_FilmSocket);
            f.ShowDialog();
        }

        //material out 
        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void cb_RawMaterialCode1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

		//返回值：		0：	无物料单
		//			1：	成功，台设备，每台7个料仓，一共49组数据，保存在m_materialCode和m_materialRequired
		//				中，界面中只显示第一台设备的物料情况。
		//			-1：通讯失败
		private int ToServer_startwork()
        {
        	byte[] send_buf = System.Text.Encoding.Default.GetBytes(UserInput.WorkerNo);
			byte[] data;
			string[] start_work;

			if (m_connected)
	        	return m_FilmSocket.sendDataPacketToServer(send_buf, COMMUNICATION_TYPE_WAREHOUE_OUT_START, send_buf.Length);
			else
				return -1;

			data = m_FilmSocket.RecvData(10000);
			if (data != null) {
				if (data[0]==(byte)0xff)
					return -1;//重发
				if (data[0]==(byte)0)	
					return 0;//无物料单

				start_work = System.Text.Encoding.Default.GetString(data).Split(';');

				//7台设备，每台7个料仓，一共49组数据，每组数据格式如下：物料代码;物料数量;
				for (int i=0;i<UserInput.targets.Length;i++) {
					for (int j=0;j< MAX_LIAOCANG_NUM; j++) {
						m_materialCode[i,j] = start_work[i*14+j];
						m_materialRequired[i,j] = start_work[i*14+j+1];
					}
				}

                bStart = true;

                cb_TargetMachineNo.Text  = UserInput.targets[0];


				return 1;//成功
			}
			return -1;//通讯错误
		}

		//返回值：		 0：	成功
		//			-1：通讯失败
		private int ToServer_material_out_upload()
		{
			//<原料代码>;<原料批次号>;<目标设备号>;<料仓号>;<重量>
			string str="";
			int index;
			byte[] send_buf = System.Text.Encoding.Default.GetBytes(str);

			if (m_connected) {
				str += UserInput.RawMaterialCode + ";";
				str	+= UserInput.RawMaterialBatchNo + ";";
				for (index=0;index<UserInput.targets.Length;index++) {
					if (UserInput.TargetMachineNo == UserInput.targets[index]) {
						str += (index+1) + ";";
						break;
					}
				}
				str += UserInput.LiaoCangNo.Remove(1) + ";";
				str += UserInput.BenCiChuKuWeight;
				return m_FilmSocket.sendDataPacketToServer(send_buf, COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE, send_buf.Length);
			}
			else
				return -1;

			//return m_FilmSocket.RecvResponse(1000);
		}
		
		private void network_data_received(int communicationType, byte[] data_buf, int len)
		{
			string[] start_work;

			if (communicationType == COMMUNICATION_TYPE_WAREHOUE_OUT_START) {
				if (data_buf != null) {
					if (data_buf[0]==(byte)0xff){
						m_lastRsp = -1;//重发
						return;
					}
					if (data_buf[0]==(byte)0){
						m_lastRsp = 0;//无物料单
						return;
					}
				
					start_work = System.Text.Encoding.Default.GetString(data_buf).Split(';');
				
					//7台设备，每台7个料仓，一共49组数据，每组数据格式如下：物料代码;物料数量;
					for (int i=0;i<UserInput.targets.Length;i++) {
						for (int j=0;j< MAX_LIAOCANG_NUM; j++) {
							m_materialCode[i,j] = start_work[i*14+j];
							m_materialRequired[i,j] = start_work[i*14+j+1];
						}
					}
				
					bStart = true;
				
					cb_TargetMachineNo.Text  = UserInput.targets[0];
				
					m_lastRsp = 1;//成功
				}
				else
					m_lastRsp = -1;
			}
			if (communicationType == COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE) {
				m_lastRsp = data_buf[0];
			}
		}

        private void cb_TargetMachineNo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (bStart) { 
                for (int i = 0; i< MAX_LIAOCANG_NUM; i++) {
                    cb_RawMaterialCodes[i].Text = m_materialCode[cb_TargetMachineNo.SelectedIndex, i];
                    tb_XuQiuWeights[i].Text = m_materialRequired[cb_TargetMachineNo.SelectedIndex, i];
                }
            }

        }
    }
}
