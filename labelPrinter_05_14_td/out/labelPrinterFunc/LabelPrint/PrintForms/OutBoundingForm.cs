﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
		string[,] m_materialCode = new string[7,7];
		string[,] m_materialRequired = new string[7,7];
		
        OutBoundingInputData UserInput;

        const int MAX_LIAOCANG_NUM = 8;

        ComboBox[] cb_RawMaterialCodes = new ComboBox[MAX_LIAOCANG_NUM];
        TextBox[] tb_RawMaterialBachNos = new TextBox[MAX_LIAOCANG_NUM];
        TextBox[] tb_XuQiuWeights = new TextBox[MAX_LIAOCANG_NUM];
        TextBox[] tb_YiChuKuWeights = new TextBox[MAX_LIAOCANG_NUM];
        TextBox[] tb_BenCiChuKuWeights = new TextBox[MAX_LIAOCANG_NUM];

        BardCodeHooK BarCodeHook = new BardCodeHooK();

        public OutBoundingForm(FilmSocket filmsocket)
        {
            InitializeComponent();
			m_FilmSocket = filmsocket;
        }
		~OutBoundingForm()
        {
	        m_FilmSocket.network_state_event -= m_networkstatehandler;
		}

		public void network_status_change(bool status)
        {
        	Console.WriteLine("network changed to {0}", status);
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
            cb_RawMaterialCodes[7] = cb_RawMaterialCode8;

            tb_RawMaterialBachNos[0] = tb_BachNo1;
            tb_RawMaterialBachNos[1] = tb_BachNo2;
            tb_RawMaterialBachNos[2] = tb_BachNo3;
            tb_RawMaterialBachNos[3] = tb_BachNo4;
            tb_RawMaterialBachNos[4] = tb_BachNo5;
            tb_RawMaterialBachNos[5] = tb_BachNo6;
            tb_RawMaterialBachNos[6] = tb_BachNo7;
            tb_RawMaterialBachNos[7] = tb_BachNo8;

            tb_XuQiuWeights[0] = tb_XuQiu1;
            tb_XuQiuWeights[1] = tb_XuQiu2;
            tb_XuQiuWeights[2] = tb_XuQiu3;
            tb_XuQiuWeights[3] = tb_XuQiu4;
            tb_XuQiuWeights[4] = tb_XuQiu5;
            tb_XuQiuWeights[5] = tb_XuQiu6;
            tb_XuQiuWeights[6] = tb_XuQiu7;
            tb_XuQiuWeights[7] = tb_XuQiu8;

            tb_YiChuKuWeights[0] = tb_YiChuKu1;
            tb_YiChuKuWeights[1] = tb_YiChuKu2;
            tb_YiChuKuWeights[2] = tb_YiChuKu3;
            tb_YiChuKuWeights[3] = tb_YiChuKu4;
            tb_YiChuKuWeights[4] = tb_YiChuKu5;
            tb_YiChuKuWeights[5] = tb_YiChuKu6;
            tb_YiChuKuWeights[6] = tb_YiChuKu7;
            tb_YiChuKuWeights[7] = tb_YiChuKu8;

            tb_BenCiChuKuWeights[0] = tb_BenCiChuKu1;
            tb_BenCiChuKuWeights[1] = tb_BenCiChuKu2;
            tb_BenCiChuKuWeights[2] = tb_BenCiChuKu3;
            tb_BenCiChuKuWeights[3] = tb_BenCiChuKu4;
            tb_BenCiChuKuWeights[4] = tb_BenCiChuKu5;
            tb_BenCiChuKuWeights[5] = tb_BenCiChuKu6;
            tb_BenCiChuKuWeights[6] = tb_BenCiChuKu7;
            tb_BenCiChuKuWeights[7] = tb_BenCiChuKu8;
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

            //switch (UserInput.LiaoCangNo)
            //{
            //    case "1":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode1.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo1.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu1.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu1.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu1.Text;
            //        break;
            //    case "2":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode2.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo2.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu2.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu2.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu2.Text;
            //        break;
            //    case "3":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode3.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo3.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu3.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu3.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu3.Text;
            //        break;
            //    case "4":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode4.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo4.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu4.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu4.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu4.Text;
            //        break;
            //    case "5":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode5.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo5.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu5.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu5.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu5.Text;
            //        break;
            //    case "6":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode6.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo6.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu6.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu6.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu6.Text;
            //        break;
            //    case "7":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode7.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo7.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu7.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu7.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu7.Text;
            //        break;
            //    case "8":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode8.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo8.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu8.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu8.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu8.Text;
            //        break;
            //    default:
            //        break;
            //}
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

            //UserInput.RawMaterialCode1 = cb_RawMaterialCode1.Text;
            //UserInput.RawMaterialCode2 = cb_RawMaterialCode2.Text;
            //UserInput.RawMaterialCode3 = cb_RawMaterialCode3.Text;
            //UserInput.RawMaterialCode4 = cb_RawMaterialCode4.Text;
            //UserInput.RawMaterialCode5 = cb_RawMaterialCode5.Text;
            //UserInput.RawMaterialCode6 = cb_RawMaterialCode6.Text;
            //UserInput.RawMaterialCode7 = cb_RawMaterialCode7.Text;
            //UserInput.RawMaterialCode8 = cb_RawMaterialCode8.Text;

            //UserInput.RawMaterialBatchNo1 = tb_BachNo1.Text;
            //UserInput.RawMaterialBatchNo2 = tb_BachNo2.Text;
            //UserInput.RawMaterialBatchNo3 = tb_BachNo3.Text;
            //UserInput.RawMaterialBatchNo4 = tb_BachNo4.Text;
            //UserInput.RawMaterialBatchNo5 = tb_BachNo5.Text;
            //UserInput.RawMaterialBatchNo6 = tb_BachNo6.Text;
            //UserInput.RawMaterialBatchNo7 = tb_BachNo7.Text;
            //UserInput.RawMaterialBatchNo8 = tb_BachNo8.Text;


            //UserInput.XuQiuWeight1 = tb_XuQiu1.Text;
            //UserInput.XuQiuWeight2 = tb_XuQiu2.Text;
            //UserInput.XuQiuWeight3 = tb_XuQiu3.Text;
            //UserInput.XuQiuWeight4 = tb_XuQiu4.Text;
            //UserInput.XuQiuWeight5 = tb_XuQiu5.Text;
            //UserInput.XuQiuWeight6 = tb_XuQiu6.Text;
            //UserInput.XuQiuWeight7 = tb_XuQiu7.Text;
            //UserInput.XuQiuWeight8 = tb_XuQiu8.Text;

            //UserInput.YiChuKuWeight1 = tb_YiChuKu1.Text;
            //UserInput.YiChuKuWeight2 = tb_YiChuKu2.Text;
            //UserInput.YiChuKuWeight3 = tb_YiChuKu3.Text;
            //UserInput.YiChuKuWeight4 = tb_YiChuKu4.Text;
            //UserInput.YiChuKuWeight5 = tb_YiChuKu5.Text;
            //UserInput.YiChuKuWeight6 = tb_YiChuKu6.Text;
            //UserInput.YiChuKuWeight7 = tb_YiChuKu7.Text;
            //UserInput.YiChuKuWeight8 = tb_YiChuKu8.Text;

            //UserInput.BenCiChuKuWeight1 = tb_BenCiChuKu1.Text;
            //UserInput.BenCiChuKuWeight2 = tb_BenCiChuKu2.Text;
            //UserInput.BenCiChuKuWeight3 = tb_BenCiChuKu3.Text;
            //UserInput.BenCiChuKuWeight4 = tb_BenCiChuKu4.Text;
            //UserInput.BenCiChuKuWeight5 = tb_BenCiChuKu5.Text;
            //UserInput.BenCiChuKuWeight6 = tb_BenCiChuKu6.Text;
            //UserInput.BenCiChuKuWeight7 = tb_BenCiChuKu7.Text;
            //UserInput.BenCiChuKuWeight8 = tb_BenCiChuKu8.Text;
            //public String Date_Time;
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
                
                //cb_RawMaterialCode2.Items.AddRange(combosStrs);
                //cb_RawMaterialCode3.Items.AddRange(combosStrs);
                //cb_RawMaterialCode4.Items.AddRange(combosStrs);
                //cb_RawMaterialCode5.Items.AddRange(combosStrs);
                //cb_RawMaterialCode6.Items.AddRange(combosStrs);
                //cb_RawMaterialCode7.Items.AddRange(combosStrs);
                //cb_RawMaterialCode8.Items.AddRange(combosStrs);
            }

            cb_TargetMachineNo.Items.AddRange(UserInput.targets);

			m_networkstatehandler = new FilmSocket.networkstatehandler(network_status_change);
			m_FilmSocket.network_state_event += m_networkstatehandler;
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
            else
            if (rb_button8.Checked)
            {
                UserInput.LiaoCangNo = "8";
                SetRadioComStateByLiaoCangNo(8);
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
        
        	m_FilmSocket.sendDataPacketToServer(send_buf, COMMUNICATION_TYPE_WAREHOUE_OUT_START, send_buf.Length);

			data = m_FilmSocket.RecvData(10000);
			if (data != null) {
				if (data[0]==(byte)0xff)
					return -1;//重发
				if (data[0]==(byte)0)	
					return 0;//无物料单

				start_work = System.Text.Encoding.Default.GetString(data).Split(';');

				//7台设备，每台7个料仓，一共49组数据，每组数据格式如下：物料代码;物料数量;
				for (int i=0;i<UserInput.targets.Length;i++) {
					for (int j=0;j<7;j++) {
						m_materialCode[i,j] = start_work[i*14+j];
						m_materialRequired[i,j] = start_work[i*14+j+1];
					}
				}
				
				cb_TargetMachineNo.Text  = UserInput.targets[0];
				cb_RawMaterialCode1.Text = start_work[0];
				tb_XuQiu1.Text			 = start_work[1];
				cb_RawMaterialCode2.Text = start_work[2];
				tb_XuQiu2.Text			 = start_work[3];
				cb_RawMaterialCode3.Text = start_work[4];
				tb_XuQiu3.Text			 = start_work[5];
				cb_RawMaterialCode4.Text = start_work[6];
				tb_XuQiu4.Text			 = start_work[7];
				cb_RawMaterialCode5.Text = start_work[8];
				tb_XuQiu5.Text			 = start_work[9];
				cb_RawMaterialCode6.Text = start_work[10];
				tb_XuQiu6.Text			 = start_work[11];
				cb_RawMaterialCode7.Text = start_work[12];
				tb_XuQiu7.Text			 = start_work[13];

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

			str += UserInput.RawMaterialCode + ";";
			str	+= UserInput.RawMaterialBatchNo + ";";
			for (index=0;index<UserInput.targets.Length;index++) {
				if (UserInput.TargetMachineNo == UserInput.targets[index]) {
					str += index + ";";
					break;
				}
			}
			str += UserInput.LiaoCangNo + ";";
			str += UserInput.BenCiChuKuWeight;
			m_FilmSocket.sendDataPacketToServer(send_buf, COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE, send_buf.Length);

			return m_FilmSocket.RecvResponse(1000);
		}
    }
}
