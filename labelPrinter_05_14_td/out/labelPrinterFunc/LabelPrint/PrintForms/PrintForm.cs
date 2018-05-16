﻿using System;
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
using LabelPrint.Data;
using LabelPrint.Util;
using LabelPrint.Receipt;
using LabelPrint.PrintForms;
namespace LabelPrint
{
    public partial class PrintForm : Form
    {
        FilmPrintUserinputData UserInput;
        BardCodeHooK BarCodeHook = new BardCodeHooK();
        object[] productCodes;

        SerialPort serialPort1;
        Byte[] serialDataBuf = new Byte[128];

        public PrintForm()
        {
            InitializeComponent();
        }

        private void PrintForm_Load(object sender, EventArgs e)
        {
            //rb_NoonWork.Visible = false;
            UserInput = new FilmPrintUserinputData();

            UserInput.CreateDataTable();

            BarCodeHook.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            BarCodeHook.Start();

            productCodes = UserInput.GetComboStrsForProductCode();
            cb_ProductCode.Items.AddRange(productCodes);
            tb_DateTime.Enabled = false;

            SetWorkClassType(WorkClassType.JIA);
            SetManufactureType(ManufactureType.M_SINGLE);
            SetWorkTimeType(WorkTimeType.DAYWORK);
            tb_ManHour.Text = "0";
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            tb_PrintMachineNo.Text = SysSetting.CurSettingInfo.MachineNo;
            tb_PrintMachineNo.Enabled = false;
            tb_worker.Text = gVariable.userAccount;
            tb_worker.Enabled = false;
            lb_InputBarcode.Text = "";
            lb_InputBarCode1.Text = "";
            lb_OutputBarCode.Text = "";

            cb_ProductState.Items.AddRange(FilmPrintUserinputData.PrintProductStateStr);
            cb_ProductState.SelectedIndex = 0;


            initSerialPort();
        }

        private void ProductCutForm_FormClosing(object sender, EventArgs e)
        {
            serialPort1.Close();
        }
        
        void initSerialPort()
        {
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;

            try
            {
                serialPort1 = new SerialPort(SysSetting.CurSettingInfo.ScaleSerialPort, 9600, Parity.None, 8, StopBits.One);
                serialPort1.Open();
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialDataReceived);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open serial port failed!" + ex);
            }
        }

        void serialDataReceived(object sender, SerialDataReceivedEventArgs e)
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


        private void SetManufactureType(ManufactureType type)
        {
            switch (type)
            {
                case ManufactureType.M_MULTIPLE:
                    //rb_SetWork.Checked = true;
                    break;
                case ManufactureType.M_SINGLE:
                   // rb_SingleWork.Checked = true;
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
            SendItemToServer();
            PostUpdateProductData();
        }

        private void UpdateUserInput()
        {

            UserInput.ProductCode = cb_ProductCode.Text;
           // UserInput.LittleRollCount = tb_LittleRollCount1.Text;
            UserInput.Width = tb_Width.Text;

            UserInput.CustomerName = tb_CustomerName.Text;
            UserInput.RecipeCode = tb_RecipeCode.Text;

           // UserInput.MaterialName = tb_MaterialName1.Text;

            UserInput.BatchNo = tb_BatchNo.Text;
            UserInput.WorkNo = tb_WorkNo.Text;
            UserInput.WorkHour = tb_ManHour.Text;
            UserInput.WorkerNo = tb_worker.Text;
            UserInput.PrintMachineNo = tb_PrintMachineNo.Text;
           // UserInput.LittleRollCount = tb_LittleRollCount.Text;

            //Get the Selected Item from ProductState/ ProductQuality / RealWeight
            UserInput.Desc = GetDesc(tb_Desc);
            UserInput.BigRollNo = GetCurBigRollNo(tb_BigRollNo);

            UserInput.WorkClsType = GetWorkClassType();
            UserInput.WorkTType = GetWorkTimeType();
            UserInput.Product_State = cb_ProductState.Text;
            UserInput.Roll_Weight = tb_RollWeight.Text;
            //   UserInput.LittleRollNo = tb_LittleRollNo.Text;
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
            //SystemSetting SysSetting;
            //SysSetting = GlobalConfig.Setting;

            //SysSetting.DynPrintData = new DynamicPrintLabelData();
            //DynamicPrintLabelData DynPrintData = SysSetting.DynPrintData;
            //UserInput.UpdatePrintPrintData(DynPrintData);
            ////FilmPrintLabel label = new FilmPrintLabel();
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
                    lb_InputBarCode1.Text = barCode.BarCode;
            }
        }

        
        private void PostUpdateProductData()
        {
            lb_OutputBarCode.Text = UserInput.OutputBarcode;

            cb_ProductState.SelectedIndex = 0;
            tb_BigRollNo.Text = CommonFormHelper.UpdateBigRollNo(tb_BigRollNo.Text);
            //UserInput.UpdateDateTime();
            //  tb_DateTime.Text = UserInput.GetDateTime();
            //            tb_DateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
        }

        private void cb_ProductCode1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //String fixture;
            //// UpdateProductData();
            //UserInput.ProductCode = (String)productCodes[cb_ProductCode.SelectedIndex];
            //UserInput.GetInfoByProductCode(UserInput.ProductCode, out UserInput.Width, out UserInput.RecipeCode, out fixture, out UserInput.CustomerName);

            //tb_Width.Text = UserInput.Width;
            //tb_RecipeCode.Text = UserInput.RecipeCode;
            //tb_CustomerName.Text = UserInput.CustomerName;



            // String fixture;
            String Width;
            String RecipeCode;
            String Fixture;
            String CustomerName;
          //  int comBoxIndex = 0;
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
            //tb_ProductLength.Text = productLength;
            //tb_ProductWeight.Text = productWeight;
           // tb_RawMaterialCode.Text = RawMaterialCode;

            UserInput.ProductCode = ProductCode;
            UserInput.Width = Width;
            UserInput.RecipeCode = RecipeCode;
            UserInput.CustomerName = CustomerName;
            UserInput.ProductLength = productLength;
            UserInput.ProductWeight = productWeight;
            UserInput.RawMaterialCode = RawMaterialCode;
            UserInput.ProductName = productName;
            //switch (cb_ProductState.SelectedIndex)  productCodes
        }

        private void tb_WorkNoOK_Click(object sender, EventArgs e)
        {
            String orderNo;
            String batchNo;
            String DevNo;
            String WorkNoSn;
            String productName;
            String fixture;
            UserInput.ParseWorkNo(tb_WorkNo.Text,  out batchNo, out DevNo, out WorkNoSn);
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
                if (!UserInput.ParseWorkNo(tb_WorkNo.Text,  out batchNo, out DevNo, out WorkNoSn))
                    return;
                if (gVariable.orderNo != null)
                {
                    orderNo = gVariable.orderNo;
                    if (!UserInput.GetProductInfoBySaleOrder(orderNo, out UserInput.ProductCode, out productName, out UserInput.CustomerName))
                        return;
                    if (!UserInput.GetInfoByProductCode(UserInput.ProductCode, out UserInput.Width, out UserInput.RecipeCode, out fixture, out UserInput.CustomerName))
                        return;
                }
                lb_InputBarcode.Text = barcode;
                tb_Width.Text = UserInput.Width;
                tb_RecipeCode.Text = UserInput.RecipeCode;
                tb_CustomerName.Text = UserInput.CustomerName;
                tb_BatchNo.Text = batchNo;
                tb_BigRollNo.Text = UserInput.BigRollNo;

                tb_BatchNo.Text = UserInput.BatchNo;
                SetManufactureType(UserInput.MType);
                cb_ProductCode.Text = UserInput.ProductCode;

                tb_ManHour.Text = "0";
                tb_Desc.Text = "";


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

//#if true
//#if false
//            String barcode;
//            UpdateScanForm f = new UpdateScanForm();
//            f.SetUserInput(UserInput);
//            f.ShowDialog();
//#endif
//#else
//            String barcode;

//            ScanForm f = new ScanForm();
//            f.ShowDialog();
//            if (f.DialogResult == DialogResult.OK)
//            {
//                barcode = f.GetBarCodeValue();

//                barcode = UserInput.OutputBarcode;
//                if (!UserInput.UpdateMStateInDB(barcode))
//                    return;
//            }
//#endif
        }
    }
}