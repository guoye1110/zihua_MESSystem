using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabelPrint.Data;
using LabelPrint.Util;
using LabelPrint.Receipt;
using LabelPrint.PrintForms;
using LabelPrint.NetWork;

namespace LabelPrint
{
    public partial class packForm : Form
    {
		//打包工序		
		private const int COMMUNICATION_TYPE_PACKING_PROCESS_PACKAGE_BARCODE_UPLOAD = 0xC7;
		private FilmSocket m_FilmSocket;
		FilmSocket.networkstatehandler m_networkstatehandler;

        PackUserinputData UserInput;
        BardCodeHooK BarCodeHook = new BardCodeHooK();

        public packForm(FilmSocket filmsocket)
        {
            InitializeComponent();
			m_FilmSocket = filmsocket;
        }
		~packForm()
        {
        	m_FilmSocket.network_state_event -= m_networkstatehandler;
		}

		public void network_status_change(bool status)
        {
        	Console.WriteLine("network changed to {0}", status);
		}

        private void PackForm_Load(object sender, EventArgs e)
        {
            //rb_NoonWork.Visible = false;
            UserInput = new PackUserinputData();
            UserInput.CreateDataTable();
            object[] productCodes = UserInput.GetComboStrsForProductCode();
            cb_ProductCode.Items.AddRange(productCodes);
            tb_DateTime.Enabled = false;

            BarCodeHook.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            BarCodeHook.Start();

            SetWorkClassType(WorkClassType.JIA);
            SetManufactureType(ManufactureType.M_SINGLE);
            SetWorkTimeType(WorkTimeType.DAYWORK);
            tb_ManHour.Text = "0";
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            tb_PackMachineNo.Text = SysSetting.CurSettingInfo.MachineNo;
            tb_PackMachineNo.Enabled = false;
            tb_worker.Text = gVariable.userAccount;
            tb_worker.Enabled = false;
            tb_LittleRollNo.Text = "0";
			
			m_networkstatehandler = new FilmSocket.networkstatehandler(network_status_change);
			m_FilmSocket.network_state_event += m_networkstatehandler;           
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
        private void UpdateProductData()
        {
            UpdateUserInput();
        }

        private void PrintLabel()
        {
           // SystemSetting SysSetting;
           // SysSetting = GlobalConfig.Setting;

           // SysSetting.DynPrintData = new DynamicPrintLabelData();
           // DynamicPrintLabelData DynPrintData = SysSetting.DynPrintData;
           //// UserInput.UpdatePrintPrintData(DynPrintData);
           // //PackLabel label = new PackLabel();
           // //label.Printlabel();
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


        private void UpdateUserInput()
        {
            UserInput.ProductCode = cb_ProductCode.Text;
            UserInput.LittleRollCount = tb_LittleRollCount.Text;
            UserInput.Width = tb_Width.Text;
            UserInput.CustomerName = tb_CustomerName.Text;
            UserInput.RecipeCode = tb_RecipeCode.Text;
            UserInput.MaterialName = tb_MaterialName.Text;
            
            

            UserInput.BatchNo = tb_BatchNo.Text;
            UserInput.WorkNo = tb_WorkNo.Text;
            UserInput.WorkHour = tb_ManHour.Text;
            UserInput.WorkerNo = tb_worker.Text;

            UserInput.PackMachineNo = tb_PackMachineNo.Text;
            //Get the Selected Item from ProductState/ ProductQuality / RealWeight
            UserInput.Desc = GetDesc(tb_Desc);
            UserInput.BigRollNo = GetCurBigRollNo(tb_BigRollNo);
            UserInput.LittleRollNo = tb_LittleRollNo.Text;

            UserInput.WorkClsType = GetWorkClassType();
            UserInput.WorkTType = GetWorkTimeType();

            UserInput.Roll_Weight = tb_Roll_Weight.Text;
            UserInput.RawMaterialCode = tb_RawMaterialCode.Text;

            UserInput.PlateNo = tb_PlateNo.Text;
            UserInput.ProductLength = tb_ProductLength.Text;
            UserInput.ProductWeight = tb_ProductWeight.Text;
            UserInput.RawMaterialCode = tb_RawMaterialCode.Text;
            UserInput.OrderNo = tb_OrderNo.Text;
        }

        private String GetDesc(TextBox tb)
        {
            return tb.Text;
        }

        private String GetCurBigRollNo(TextBox tb)
        {
            return tb.Text;
        }

        private void bt_Scan_Click(object sender, EventArgs e)
        {
            String barcode;
            String orderNo;
            String batchNo;
            String DevNo;
            String WorkNoSn;
            String productName;
            String fixture;

            ScanForm f = new ScanForm();
            f.ShowDialog();
            if (f.DialogResult == DialogResult.OK)
            {
                barcode = f.GetBarCodeValue();
                barcode = "S17110906L302S118012014310500100";
                if (!UserInput.ParseBarCode(barcode))
                    return;

                tb_WorkNo.Text = UserInput.WorkNo;
                if (!UserInput.ParseWorkNo(tb_WorkNo.Text, out batchNo, out DevNo, out WorkNoSn))
                    return;
                if (gVariable.orderNo!=null)
                {
                    orderNo = gVariable.orderNo;
                if (!UserInput.GetProductInfoBySaleOrder(orderNo, out UserInput.ProductCode, out productName, out UserInput.CustomerName))
                    return;
                if (!UserInput.GetInfoByProductCode(UserInput.ProductCode, out UserInput.Width, out UserInput.RecipeCode, out fixture, out UserInput.CustomerName))
                    return;
                }

                UserInput.InputBarcode = barcode;
                lb_InputBarcode.Text = barcode;
                tb_Width.Text = UserInput.Width;
                tb_RecipeCode.Text = UserInput.RecipeCode;
                tb_CustomerName.Text = UserInput.CustomerName;
                //tb_BatchNo.Text = batchNo;
                tb_BigRollNo.Text = UserInput.BigRollNo;
                tb_BatchNo.Text = UserInput.BatchNo;
                tb_ManHour.Text = "0";
                tb_Desc.Text = "";

                tb_LittleRollNo.Text = UserInput.LittleRollNo;
                
                SetManufactureType(UserInput.MType);
                cb_ProductCode.Text = UserInput.ProductCode;

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


			//<打包条码>;<工号>
			string str = UserInput.OutputBarcode + ";" + UserInput.WorkerNo;
			byte[] send_buf = System.Text.Encoding.Default.GetBytes(str);

			m_FilmSocket.sendDataPacketToServer(send_buf, COMMUNICATION_TYPE_PACKING_PROCESS_PACKAGE_BARCODE_UPLOAD, send_buf.Length);

			int rsp = m_FilmSocket.RecvResponse(1000);
			if (rsp == 0)	System.Windows.Forms.MessageBox.Show("发送成功！");

			
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

        private void cb_ProductCode_SelectedValueChanged(object sender, EventArgs e)
        {
            String fixture;
            String Width;
            String RecipeCode;
            String Fixture;
            String CustomerName;
            int comBoxIndex = 0;
            String ProductCode;
            ProductCode = (String)cb_ProductCode.SelectedItem.ToString();

            if (ProductCode == null || ProductCode == "")
                return;
            String RawMaterialCode = null;
            String productLength = null;
            String productName = null;
            String productWeight = null;
            try
            {
                //  ProductCode = cb_ProductCode1.Text;
                UserInput.GetInfoByProductCodeExt(ProductCode, out Width, out RecipeCode, out Fixture, out CustomerName, out RawMaterialCode, out productLength, out productName, out productWeight);

            }
            catch (Exception ex)
            {
              //  Log.e("CutForm", ex.Message);
                return;
            }

         //   UserInput.GetInfoByProductCode(UserInput.ProductCode, out UserInput.Width, out UserInput.RecipeCode, out fixture, out UserInput.CustomerName);

            tb_Width.Text = Width;
            tb_RecipeCode.Text = RecipeCode;
            tb_CustomerName.Text = CustomerName;
            tb_ProductLength.Text = productLength;
            tb_ProductWeight.Text = productWeight;
            tb_RawMaterialCode.Text = RawMaterialCode;

            UserInput.ProductCode = ProductCode;
            UserInput.Width = Width;
            UserInput.RecipeCode = RecipeCode;
            UserInput.CustomerName = CustomerName;
            UserInput.ProductLength = productLength;
            UserInput.ProductWeight = productWeight;
//            tb_MaterialName.Text = 

        }
    }
}
