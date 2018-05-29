using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabelPrint.Receipt;
using LabelPrint.Util;
using LabelPrint.Data;
using LabelPrint.PrintForms;
using LabelPrint.NetWork;

namespace LabelPrint
{
    public partial class LiuYanForm : Form
    {
		//流延工序
		private const int COMMUNICATION_TYPE_CAST_PROCESS_START = 0xB8;  //printing SW started cast process, server need to send dispatch info to printing SW
		private const int COMMUNICATION_TYPE_CAST_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xB9;  //printing SW send large roll info to server
		private const int COMMUNICATION_TYPE_CAST_PROCESS_END = 0xBA;
		private FilmSocket m_FilmSocket;
		FilmSocket.networkstatehandler m_networkstatehandler;
		FilmSocket.networkdatahandler m_networkdatahandler;
		private string m_dispatchCode;
		private int m_lastRsp;
		private bool m_connected;

        LiuYanUserinputData UserInput;
        BardCodeHooK BarCodeHook = new BardCodeHooK();

        SerialPort serialPort1;  //磅秤
        SerialPort serialPort2;  //扫描枪

        Byte[] serialDataBuf = new Byte[128];

        public LiuYanForm(FilmSocket filmsocket)
        {
            InitializeComponent();
			m_FilmSocket = filmsocket;
			m_connected = m_FilmSocket.get_status();
        }
		~LiuYanForm()
        {
            m_FilmSocket.network_state_event -= m_networkstatehandler;
			m_FilmSocket.network_data_event -= m_networkdatahandler;
		}

		public void network_status_change(bool status)
        {
        	Console.WriteLine("network changed to {0}", status);
			m_connected = status;
		}

        private void LiuYanForm_Load(object sender, EventArgs e)
        {
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;

            BarCodeHook.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            BarCodeHook.Start();
            
            //rb_NoonWork.Visible = false;
            UserInput = new LiuYanUserinputData();
            UserInput.CreateDataTable();
            object[] productCodes = UserInput.GetComboStrsForProductCode();
            cb_ProductCode.Items.AddRange(productCodes);
            tb_DateTime.Enabled = false;

            SetWorkClassType(WorkClassType.JIA);
            SetManufactureType(ManufactureType.M_SINGLE);
            SetWorkTimeType(WorkTimeType.DAYWORK);
            tb_ManHour.Text = "0";
            tb_LiuYanMachineNo.Text = SysSetting.CurSettingInfo.MachineNo;
            tb_LiuYanMachineNo.Enabled = false;
            tb_worker.Text = gVariable.userAccount;
            UserInput.WorkerNo = gVariable.userAccount;
            tb_worker.Enabled = false;
            cb_ProductState.Items.AddRange(LiuYanUserinputData.PrintProductStateStr);
            cb_ProductState.SelectedIndex = 0;
            InitProductQualityComboBox(cb_ProductQuality);
            cb_ProductQuality.SelectedIndex = 0;
            initSerialPort();

			m_networkstatehandler = new FilmSocket.networkstatehandler(network_status_change);
			m_FilmSocket.network_state_event += m_networkstatehandler;
			m_networkdatahandler = new FilmSocket.networkdatahandler(network_data_received);
			m_FilmSocket.network_data_event += m_networkdatahandler;
        }

        private void Liuyan_FormClosing(object sender, EventArgs e)
        {
            if (serialPort1 != null)
                serialPort1.Close();
            if (serialPort2 != null)
                serialPort2.Close();
        }
        
        void initSerialPort()
        {
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;

            try
            {
                serialPort1 = new SerialPort(SysSetting.CurSettingInfo.ScaleSerialPort, 9600, Parity.None, 8, StopBits.One);
                serialPort1.Open();
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialDataReceived1);

                serialPort2 = new SerialPort(SysSetting.CurSettingInfo.ScannerSerialPort, 9600, Parity.None, 8, StopBits.One);
                serialPort2.Open();
                serialPort2.DataReceived += new SerialDataReceivedEventHandler(serialDataReceived2);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open serial port failed!" + ex);
            }
        }

        void serialDataReceived1(object sender, SerialDataReceivedEventArgs e)
        {
            int num;
            string weightStr;

            try
            {
                num = 0;
                weightStr = "";

                Thread.Sleep(1000);

                while (weightStr.Length < 4)
                {
                    serialPort1.Read(serialDataBuf, 0, serialPort1.BytesToRead);
                    weightStr += getSerialPortOutput(serialDataBuf);
                    if (weightStr.Length == 0)
                        break;

                    num++;
                    if (num > 2)
                        break;
                }

                if (weightStr.Length > 1)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        tb_RollWeight.Text = weightStr;
                        printOneLabel();
                    }));
                }
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        string getSerialPortOutput(byte[] serialDataBuf)
        {
            int i;
            int start, end;
            int flag;
            string str; 

            flag = 0;
            start = 0;
            end = 0;

            str = System.Text.Encoding.ASCII.GetString(serialDataBuf);
            //Console.WriteLine(DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + ": " + str);

            for (i = 0; i < 20; i++)
            {
                Console.Write(serialDataBuf[i].ToString() + ", ");
                if (serialDataBuf[i] == 0x0d || serialDataBuf[i] == 0x0a || serialDataBuf[i] == 0)
                {
                    end = i;
                    break;
                }

                //we got the data header
                if(flag == 0 && serialDataBuf[i] == 0x20 && serialDataBuf[i + 1] == 0x20 && serialDataBuf[i + 2] == 0x20 && serialDataBuf[i + 3] == 0x20)
                {
                    flag = 1;
                }
                else if (flag == 1 && serialDataBuf[i] != 0x20)  //data start position
                {
                    start = i;
                    flag = 2;
                }
            }

            Console.WriteLine(" ");
            
            if (i > 20)
                return null;

            return ( System.Text.Encoding.ASCII.GetString(serialDataBuf, start, end - start));
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
                    label6.Text = System.Text.Encoding.ASCII.GetString(serialDataBuf1);
                }));

            }
            catch (TimeoutException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        
        private void SetManufactureType(ManufactureType type)
        {
            switch (type)
            {
                case ManufactureType.M_MULTIPLE:
                    //rb_SetWork.Checked = true;
                    break;
                case ManufactureType.M_SINGLE:
                    //rb_SingleWork.Checked = true;
                    break;
            }
        }

        private void SetWorkClassType(WorkClassType type)
        {
            switch (type)
            {
                case WorkClassType.JIA:
                    rb_Jia.Checked = true;
                    break;
                case WorkClassType.YI:
                    rb_Yi.Checked = true;
                    break;
                case WorkClassType.BING:
                    rb_Bing.Checked = true;
                    break;
                    //case WorkClassType.DING:
                    //    rb_Ding.Checked = true;
                    //    break;
            }
        }
        private void SetWorkClassTypeUnChangeAble(WorkClassType type)
        {
            rb_Jia.Enabled = false;
            rb_Yi.Enabled = false;
            rb_Bing.Enabled = false;
            //rb_Ding.Enabled = false;
            switch (type)
            {
                case WorkClassType.JIA:
                    rb_Jia.Enabled = true;
                    break;
                case WorkClassType.YI:
                    rb_Yi.Enabled = true;
                    break;
                case WorkClassType.BING:
                    rb_Bing.Enabled = true;
                    break;
                    //case WorkClassType.DING:
                    //    rb_Ding.Enabled = true;
                    //    break;
            }
        }

        private WorkClassType GetWorkClassType()
        {
            if (rb_Jia.Checked)
            {
                return WorkClassType.JIA;
            }
            else if (rb_Yi.Checked)
            {
                return WorkClassType.YI;
            }
            else if (rb_Bing.Checked)
            {
                return WorkClassType.BING;
            }
            //else if (rb_Ding.Checked)
            //{
            //    return WorkClassType.DING;
            //}
            return WorkClassType.JIA;
        }

        private void SetWorkTimeType(WorkTimeType type)
        {
            switch (type)
            {
                case WorkTimeType.DAYWORK:
                    rb_DayWork.Checked = true;
                    break;
                //case WorkTimeType.MIDDLEWORK:
                //    rb_NoonWork.Checked = true;
                //    break;
                case WorkTimeType.NIGHTWORK:
                    rb_NightWork.Checked = true;
                    break;
            }
        }
        private WorkTimeType GetWorkTimeType()
        {
            if (rb_DayWork.Checked)
            {
                return WorkTimeType.DAYWORK;
            }
            else if (rb_NightWork.Checked)
            {
                return WorkTimeType.NIGHTWORK;
            }
            //else if (rb_NoonWork.Checked)
            //{
            //    return WorkTimeType.MIDDLEWORK;
            //}
            return WorkTimeType.DAYWORK;
        }
        private void bt_Printing_Click(object sender, EventArgs e)
        {
            printOneLabel();
        }

        private void printOneLabel()
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
            //SendItemToServer();
            PostUpdateProductData();

        }

        private void UpdateUserInput()
        {

            UserInput.ProductCode = cb_ProductCode.Text;
            // UserInput.LittleRollCount = tb_LittleRollCount1.Text;
            //UserInput.LittleRollCount2 = tb_LittleRollCount2.Text;
            //UserInput.LittleRollCount3 = tb_LittleRollCount3.Text;

            UserInput.Width = tb_Width.Text;
            //UserInput.Width2 = tb_Width2.Text;
            //UserInput.Width3 = tb_Width3.Text;


            UserInput.CustomerName = tb_CustomerName.Text;
            //UserInput.CustomerName2 = tb_CustomerName2.Text;
            //UserInput.CustomerName3 = tb_CustomerName3.Text;

            UserInput.RecipeCode = tb_RecipeCode.Text;
            //UserInput.RawMaterialCode2 = tb_RawMaterialCode2.Text;
            //UserInput.RawMaterialCode3 = tb_RawMaterialCode3.Text;

            //UserInput.MaterialName = tb_MaterialName1.Text;
            //UserInput.MaterialName2 = tb_MaterialName2.Text;
            //UserInput.MaterialName3 = tb_MaterialName3.Text;

            UserInput.BatchNo = tb_BatchNo.Text;
            //UserInput.BatchNo2 = tb_BatchNo2.Text;
            //UserInput.BatchNo3 = tb_BatchNo3.Text;

            //  UserInput.LittleRollCount = tb_LittleRollCount.Text;

            UserInput.WorkNo = tb_WorkNo.Text;
            UserInput.WorkHour = tb_ManHour.Text;
            UserInput.WorkerNo = tb_worker.Text;
            UserInput.LiuYanMachineNo = tb_LiuYanMachineNo.Text;


            //Get the Selected Item from ProductState/ ProductQuality / RealWeight
            UserInput.Desc = GetDesc(tb_Desc);
            UserInput.BigRollNo = GetCurBigRollNo(tb_BigRollNo);
            UserInput.WorkClsType = GetWorkClassType();
            UserInput.WorkTType = GetWorkTimeType();
            UserInput.ProductState = cb_ProductState.Text;
            UserInput.Weight = tb_RollWeight.Text;
            UpdateUserInputQualityInfo();
        }
		private void UpdateView()
        {
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
                    label6.Text = barCode.BarCode;
            }
        }


        private String GetDesc(TextBox tb)
        {
            return tb.Text;
        }

        private String GetCurBigRollNo(TextBox tb)
        {
            return tb.Text;
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

            //LiuYanLabel label = new LiuYanLabel();
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
            cb_ProductState.SelectedIndex = 0;
            tb_BigRollNo.Text = CommonFormHelper.UpdateBigRollNo(tb_BigRollNo.Text);
            //tb_DateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void bt_scan_Click(object sender, EventArgs e)
        {
            //String barcode;
            //String orderNo;
            //String batchNo;
            //String DevNo;
            //String WorkNoSn;
            //String productName;
            //String fixture;

            //ScanForm f = new ScanForm();
            //f.ShowDialog();
            //if (f.DialogResult == DialogResult.OK)
            //{
            //    barcode = f.GetBarCodeValue();
            //    barcode = "S17110906L302S118012014310500100";
            //    if (!UserInput.ParseBarCode(barcode))
            //        return;

            //    tb_WorkNo.Text = UserInput.WorkNo;
            //    if (!UserInput.ParseWorkNo(tb_WorkNo.Text, out orderNo, out batchNo, out DevNo, out WorkNoSn))
            //        return;
            //    if (!UserInput.GetProductInfoBySaleOrder(orderNo, out UserInput.ProductCode, out productName, out UserInput.CustomerName))
            //        return;
            //    if (!UserInput.GetInfoByProductCode(UserInput.ProductCode, out UserInput.Width, out UserInput.RecipeCode, out fixture, out UserInput.CustomerName))
            //        return;

            //    tb_Width.Text = UserInput.Width;
            //    tb_RecipeCode.Text = UserInput.RecipeCode;
            //    tb_CustomerName.Text = UserInput.CustomerName;
            //    tb_BatchNo.Text = batchNo;
            //    tb_BigRollNo.Text = UserInput.BigRollNo;

            //    tb_BatchNo.Text = UserInput.BatchNo;
            //    SetManufactureType(UserInput.MType);
            //    cb_ProductCode.Text = UserInput.ProductCode;
            //}
        }

        private void bt_GongDanOk_Click(object sender, EventArgs e)
        {
            String orderNo;
            String batchNo;
            String DevNo;
            String WorkNoSn;
            String productName;
            String fixture;
            UserInput.ParseWorkNo(tb_WorkNo.Text, out batchNo, out DevNo, out WorkNoSn);
            if (gVariable.orderNo != null)
            {
                orderNo = gVariable.orderNo;
                UserInput.GetProductInfoBySaleOrder(orderNo, out UserInput.ProductCode, out productName, out UserInput.CustomerName);
                UserInput.GetInfoByProductCode(UserInput.ProductCode, out UserInput.Width, out UserInput.RecipeCode, out fixture, out UserInput.CustomerName);
            }

            tb_Width.Text = UserInput.Width;
            tb_RecipeCode.Text = UserInput.RecipeCode;
            tb_CustomerName.Text = UserInput.CustomerName;
            tb_BatchNo.Text = batchNo;
            cb_ProductCode.Text = UserInput.ProductCode;
            tb_ManHour.Text = "0";
            tb_Desc.Text = "";
        }

        private void bt_Record_Click(object sender, EventArgs e)
        {
            String JiaoJieRecord;

            JiaoJieBanForm f = new JiaoJieBanForm();
            f.ShowDialog();
            if (f.DialogResult == DialogResult.OK)
            {
                UpdateUserInput();
                JiaoJieRecord = f.GetJiaoBanRecord();
                if (JiaoJieRecord!=null &&JiaoJieRecord!="")
                {
                    UserInput.JiaoJiRecord = f.GetJiaoBanRecord();
                    UserInput.InsertJIaoJieRecord();
                    //write jiao JIe Record to DB

                }
                ToServer_endwork();
            }
#if true
#if false
            String barcode;
            UpdateScanForm f = new UpdateScanForm();
            f.SetUserInput(UserInput);
            f.ShowDialog();
#endif
#else
            String barcode;

            ScanForm f = new ScanForm();
            f.ShowDialog();
            if (f.DialogResult == DialogResult.OK)
            {
                barcode = f.GetBarCodeValue();

                barcode = UserInput.OutputBarcode;
                if (!UserInput.UpdateMStateInDB(barcode))
                    return;
            }
#endif
        }

        private void cb_ProductCode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            String fixture;
            UserInput.ProductCode = (String)cb_ProductCode.SelectedItem.ToString();
            //UserInput.GetInfoByProductCode(UserInput.ProductCode, out UserInput.Width, out UserInput.RecipeCode, out fixture, out UserInput.CustomerName);
            UserInput.GetInfoByProductCodeExt(UserInput.ProductCode, out UserInput.Width, out UserInput.RecipeCode, out fixture, out UserInput.CustomerName, out UserInput.RawMaterialCode, out UserInput.ProductLength, out UserInput.ProductName, out UserInput.ProductWeight);
            tb_Width.Text = UserInput.Width;
            tb_RecipeCode.Text = UserInput.RecipeCode;
            tb_CustomerName.Text = UserInput.CustomerName;

           // tb_ProductName.Text = UserInput.GetProductNameByProductCode(UserInput.ProductCode);
          //  cb_CustomerCodes.Text = UserInput.GetCustomerCodeByCustomerName(UserInput.CustomerName);
        }

        //start work
        private void button1_Click(object sender, EventArgs e)
        {
        	switch (ToServer_startwork())
        	{
        	case -1:
				System.Windows.Forms.MessageBox.Show("失败，请重新");
				break;
			case 0:
				System.Windows.Forms.MessageBox.Show("无工单");
				break;
			default:
				break;
        	}
        }

        private String GetProductState(ComboBox productState_cb)
        {
            return productState_cb.SelectedItem.ToString();
        }

        private String GetProductQuality(ComboBox productQuality_cb)
        {
            return productQuality_cb.SelectedItem.ToString();
        }
        void InitProductQualityComboBox(ComboBox productQulity)
        {
            productQulity.Items.AddRange(ProcessData.ProductQualityStrForComBoList);

        }
        void UpdateUserInputQualityInfo()
        {
            UserInput.ProductState = GetProductState(cb_ProductState);
            UserInput.ProductStateIndex = cb_ProductState.SelectedIndex;
            if (cb_ProductState.SelectedIndex != 0)
            {
                UserInput.ProductQuality = GetProductQuality(cb_ProductQuality);
                UserInput.ProductQualityIndex = cb_ProductQuality.SelectedIndex;
            }
            else
            {
                UserInput.ProductQuality = "";
                UserInput.ProductQualityIndex = 0;
            }
        }

        private void cb_ProductState_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cb_ProductState.SelectedIndex)
            {
                case 0:
                    //cb_ProductQuality.Enabled = false;
                    cb_ProductQuality.Visible = false;
                    lb_ProductQulity.Visible = false;

                    break;
                default:
                    //cb_ProductQuality.Enabled = true;
                    cb_ProductQuality.Visible = true;
                    lb_ProductQulity.Visible = true;
                    break;
            }
        }

        private void cb_ProductQuality_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

		//返回值：		0：	无工单
		//			1：	成功，工单信息会显示在界面上
		//			-1：通讯失败
		private int ToServer_startwork()
        {
        	byte[] send_buf = System.Text.Encoding.Default.GetBytes(UserInput.WorkerNo);
			byte[] data;
			string[] start_work;
			Thread toserver_thread;

			if (m_connected)
        		return m_FilmSocket.sendDataPacketToServer(send_buf, COMMUNICATION_TYPE_CAST_PROCESS_START, send_buf.Length);
			else
				return -1;

			/*data = m_FilmSocket.RecvData(10000);
			if (data != null) {
				if (data[0]==(byte)0xff)
					return -1;//重发
				if (data[0]==(byte)0)	
					return 0;//无工单

				start_work = System.Text.Encoding.Default.GetString(data).Split(';');

				//<工单编号>;<产品编号>
				tb_WorkNo.Text = start_work[0];
				cb_ProductCode.Text = start_work[1];
				cb_ProductCode_SelectionChangeCommitted(null,null);
				tb_BatchNo.Text = start_work[0].Substring(0,7);
				if (start_work[0].Substring(9,1) == "1")
					rb_DayWork.Checked = true;
				else
					rb_NightWork.Checked = true;
				return 1;//成功
			}
			return -1;//通讯错误*/
		}

		//返回值：		 0：	成功
		//			-1：通讯失败
		private int ToServer_product_barcode_upload()
		{
			//<大卷条码>;<重量>
			string str = UserInput.OutputBarcode + ";" + UserInput.Weight;
			byte[] send_buf = System.Text.Encoding.Default.GetBytes(str);

			if (m_connected)
				return m_FilmSocket.sendDataPacketToServer(send_buf, COMMUNICATION_TYPE_CAST_PROCESS_PRODUCT_BARCODE_UPLOAD, send_buf.Length);
			else
				return -1;

			//return m_FilmSocket.RecvResponse(1000);
		}


		//返回值：		 0：	成功
		//			-1：通讯失败
		private int ToServer_endwork()
		{
			//<工单编码>;<记录>
			string str = UserInput.WorkNo + ";" + UserInput.JiaoJiRecord;
			byte[] send_buf = System.Text.Encoding.Default.GetBytes(str);

			if (m_connected)
				return m_FilmSocket.sendDataPacketToServer(send_buf, COMMUNICATION_TYPE_CAST_PROCESS_END, send_buf.Length);
			else
				return -1;

			//return m_FilmSocket.RecvResponse(1000);
		}

		private void network_data_received(int communicationType, byte[] data_buf, int len)
		{
			string[] start_work;

			if (communicationType == COMMUNICATION_TYPE_CAST_PROCESS_START) {
				if (data_buf != null) {
					if (data_buf[0] == 0xff){
						m_lastRsp = -1;
						return;
					}
					if (data_buf[0] == (byte)0){
						m_lastRsp = 0;//无工单
						return;
					}

					start_work = System.Text.Encoding.Default.GetString(data_buf).Split(';');

					//<工单编号>;<产品编号>
					this.Invoke((EventHandler)(delegate
					{
						tb_WorkNo.Text = start_work[0];
						cb_ProductCode.Text = start_work[1];
						cb_ProductCode_SelectionChangeCommitted(null,null);
						tb_BatchNo.Text = start_work[0].Substring(0,7);
						if (start_work[0].Substring(9,1) == "1")
							rb_DayWork.Checked = true;
						else
							rb_NightWork.Checked = true;
					}));
					
					m_lastRsp = 1;//成功
				}
				else
					m_lastRsp = -1;//通讯错误
			}
			if (communicationType == COMMUNICATION_TYPE_CAST_PROCESS_PRODUCT_BARCODE_UPLOAD) {
				m_lastRsp = data_buf[0];
			}
			if (communicationType == COMMUNICATION_TYPE_CAST_PROCESS_END) {
				m_lastRsp = data_buf[0];
			}
		}
    }
}
