using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabelPrint.Util;
using LabelPrint.Data;
using LabelPrint.Receipt;
using LabelPrint.PrintForms;
using LabelPrint.NetWork;

namespace LabelPrint
{


    public partial class RecoveryForm : Form
    {
		//再造料工序
		private const int COMMUNICATION_TYPE_REUSE_PROCESS_BARCODE_UPLOAD = 0xC6;
		private FilmSocket m_FilmSocket;
		FilmSocket.networkstatehandler m_networkstatehandler;

        RcvInputData UserInput;
        BardCodeHooK BarCodeHook = new BardCodeHooK();

        public RecoveryForm(FilmSocket filmsocket)
        {
            InitializeComponent();
			m_FilmSocket = filmsocket;
        }
		~RecoveryForm()
        {
            m_FilmSocket.network_state_event -= m_networkstatehandler;
		}

		public void network_status_change(bool status)
        {
        	Console.WriteLine("network changed to {0}", status);
		}
		
		//返回值：		 0：	成功
		//			-1：通讯失败
		private int ToServer_recovery_barcode_upload()
		{
			//<再造料条码>;<总重量>;<条码…>;<配方单>;<员工工号>
			//To Do
            string str = "";
			byte[] send_buf = System.Text.Encoding.Default.GetBytes(str);

			m_FilmSocket.sendDataPacketToServer(send_buf, COMMUNICATION_TYPE_REUSE_PROCESS_BARCODE_UPLOAD, send_buf.Length);

			return m_FilmSocket.RecvResponse(1000);
		}

        private void tb_PlatelPerLayx_KeyPress(object sender, KeyPressEventArgs e)
        {
            ControlHelper.LimitToDigitOnly(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //update the data from UI or Scale.
            UpdateProductData();
           // if (UserInput.WorkNo==null)
           // {
           //     MessageBox.Show("打印标签之前必须先扫描！");
            //    return;
            //}
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

            UserInput.WorkProcess = //tb_WorkProcess.Text;
            UserInput.Recipe = tb_RecipeNo.Text;
            UserInput.Color = tb_Color.Text;
            //UserInput.Vendor = tb_Vendor.Text;
            UserInput.WeightPerBag = tb_WeightPerBag.Text;
            //UserInput.StackWeight = tb_StackWeight.Text;
            //UserInput.Bags_x = tb_PlatelPerLayx.Text;
            //UserInput.Bags_y = tb_PlatelPerLayy.Text;
            //UserInput.Bags_xy = tb_PlateRollNum.Text;

            UserInput.WorkerNo = tb_WorkerNo.Text;

            UserInput.Desc = tb_Desc.Text;
            UserInput.OldCode1 = tb_OldCode1.Text;
            UserInput.OldCode2 = tb_OldCode2.Text;
            UserInput.OldCode3 = tb_OldCode3.Text;
            UserInput.OldCode4 = tb_OldCode4.Text;
            UserInput.OldCode5 = tb_OldCode5.Text;
            UserInput.OldCode6 = tb_OldCode6.Text;
            UserInput.OldCode7 = tb_OldCode7.Text;
            UserInput.OldCode8 = tb_OldCode8.Text;
            UserInput.OldCode9 = tb_OldCode9.Text;
            UserInput.OldCode10 = tb_OldCode10.Text;

            UserInput.RecoveryMachineNo = tb_RecoveryMachineNo.Text;

        }
        private void UpdateProductData()
        {
             UpdateUserInput();
        }

        private void PrintLabel()
        {
            //SystemSetting SysSetting;
            //SysSetting = GlobalConfig.Setting;

            //SysSetting.DynPrintData = new DynamicPrintLabelData();
            //DynamicPrintLabelData DynPrintData = SysSetting.DynPrintData;
            //UserInput.UpdatePrintPrintData(DynPrintData);
            ////RecoveryLabel label = new RecoveryLabel();
            ////label.Printlabel();
            UserInput.PrintLabel();
        }
        private void CreateLocalDataBaseItem()
        {
            UserInput.insertOneRow();
        }
        private void SendItemToServer()
        {

        }
        private void PostUpdateProductData()
        {
            lb_OutputBarcode.Text = UserInput.OutputBarcode;
            //tb_DateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
        }

        private void RecoveryForm_Load(object sender, EventArgs e)
        {
            UserInput = new RcvInputData();
            UserInput.CreateDataTable();
            //tb_WorkProcess.Text = "再造料工序";
            //tb_WorkProcess.Enabled = false;
            tb_DateTime.Enabled = false;
            tb_WorkerNo.Text = gVariable.userAccount;
            tb_WorkerNo.Enabled = false;
            lb_InputBarcode.Text = "";
            lb_OutputBarcode.Text = "";

            BarCodeHook.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            BarCodeHook.Start();

            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;

            tb_RecoveryMachineNo.Text = SysSetting.CurSettingInfo.MachineNo;
            tb_RecoveryMachineNo.Enabled = false;

			m_networkstatehandler = new FilmSocket.networkstatehandler(network_status_change);
			m_FilmSocket.network_state_event += m_networkstatehandler;			
        }
        static int v = 0;
        private void bt_Scan_Click(object sender, EventArgs e)
        {
            String barcode;
            String orderNo;
            String batchNo;
            String DevNo;
            String WorkNoSn;
            String productName;

            ScanForm f = new ScanForm();
            f.ShowDialog();
            if (f.DialogResult == DialogResult.OK)
            {
                barcode = f.GetBarCodeValue();
            
                if (v==0 )
                {
                    v = 1;
                    barcode = "S17110906L302S118012014310500100";
                }
                else
                {
                    v = 0;
                    barcode = "S17110906L302P21801201431050";
                }
                if (!UserInput.ParseBarCode(barcode))
                    return;

                //tb_WorkNo.Text = UserInput.WorkNo;
                if (!UserInput.ParseWorkNo(UserInput.WorkNo,  out batchNo, out DevNo, out WorkNoSn))
                    return;
                if (gVariable.orderNo != null)
                {
                    orderNo = gVariable.orderNo;
                    if (!UserInput.GetProductInfoBySaleOrder(orderNo, out UserInput.ProductCode, out productName, out UserInput.CustomerName))
                        return;
                    if (!UserInput.GetRecoveryInfoByProductCode(UserInput.ProductCode, out UserInput.WeightPerBag, out UserInput.RecipeCode, out UserInput.Color))
                        return;
                }
                UserInput.InputBarcode = barcode;
                lb_InputBarcode.Text = barcode;
                tb_Color.Text = UserInput.Color;
                tb_WeightPerBag.Text = UserInput.WeightPerBag;
                tb_RecipeNo.Text = UserInput.RecipeCode;
                
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
                    lb_InputBarcode.Text = barCode.BarCode;
            }
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
                if (JiaoJieRecord != null && JiaoJieRecord != "")
                {
                    UserInput.JiaoJiRecord = f.GetJiaoBanRecord();
                    UserInput.InsertJIaoJieRecord();
                    //write jiao JIe Record to DB

                }
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
#endif          
        }
    }
    }
