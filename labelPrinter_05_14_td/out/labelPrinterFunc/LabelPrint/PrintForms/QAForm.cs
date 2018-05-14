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
namespace LabelPrint
{
    public partial class QAForm : Form
    {

        QAUserinputData UserInput;
        BardCodeHooK BarCodeHook = new BardCodeHooK();

        public QAForm()
        {
            InitializeComponent();
        }

        private void QAForm_Load(object sender, EventArgs e)
        {
            //rb_NoonWork.Visible = false;
            UserInput = new QAUserinputData();
            UserInput.CreateDataTable();
            object[] productCodes = UserInput.GetComboStrsForProductCode();
            cb_ProductCode.Items.AddRange(productCodes);
            tb_DateTime.Enabled = false;


            BarCodeHook.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            BarCodeHook.Start();

            SetWorkClassType(WorkClassType.JIA);
            SetManufactureType(ManufactureType.M_SINGLE);
            SetWorkTimeType(WorkTimeType.DAYWORK);
            InitProductStateComboBox(cb_ProductState);
            InitProductQualityComboBox(cb_ProductQuality);
            cb_ProductState.SelectedIndex = 0;
            cb_ProductQuality.SelectedIndex = 0;
            tb_ManHour.Text = "0";
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            //tb_QAMachineNo.Text = SysSetting.CurSettingInfo.MachineNo;
            //  tb_QAMachineNo.Enabled = false;
            tb_worker.Text = gVariable.userAccount;
            tb_worker.Enabled = false;


            lb_InputBarcode.Text = "";
            lb_OutputBarcode.Text = "";
        }


        void InitProductStateComboBox(ComboBox productStates)
        {
            productStates.Items.AddRange(ProcessData.ProductStateStr);

        }

        void InitProductQualityComboBox(ComboBox productQulity)
        {
            productQulity.Items.AddRange(ProcessData.ProductQualityStrForComBoList);

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
                    //rb_Jia.Checked = true;
                    break;
                case WorkClassType.YI:
                    //rb_Yi.Checked = true;
                    break;
                case WorkClassType.BING:
                    //rb_Bing.Checked = true;
                    break;
                    //case WorkClassType.DING:
                    //    rb_Ding.Checked = true;
                    //    break;
            }
        }
        private void SetWorkClassTypeUnChangeAble(WorkClassType type)
        {
            //rb_Jia.Enabled = false;
            //rb_Yi.Enabled = false;
            //rb_Bing.Enabled = false;
            //rb_Ding.Enabled = false;
            switch (type)
            {
                case WorkClassType.JIA:
                    //rb_Jia.Enabled = true;
                    break;
                case WorkClassType.YI:
                    //rb_Yi.Enabled = true;
                    break;
                case WorkClassType.BING:
                    //rb_Bing.Enabled = true;
                    break;
                    //case WorkClassType.DING:
                    //    rb_Ding.Enabled = true;
                    //    break;
            }
        }

        private WorkClassType GetWorkClassType()
        {
            /*
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
             */
            return WorkClassType.JIA;
        }

        private void SetWorkTimeType(WorkTimeType type)
        {
            switch (type)
            {
                case WorkTimeType.DAYWORK:
                    //rb_DayWork.Checked = true;
                    break;
                //case WorkTimeType.MIDDLEWORK:
                //    rb_NoonWork.Checked = true;
                //    break;
                case WorkTimeType.NIGHTWORK:
                    //rb_NightWork.Checked = true;
                    break;
            }
        }

        private WorkTimeType GetWorkTimeType()
        {
            /*
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
             */ 
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
            //    UserInput.ProductCode1 = cb_ProductCode1.Text;
            //   UserInput.ProductCode1 = cb_ProductCode1.Text;
            //UserInput.LittleRollCount = tb_LittleRollCount1.Text;
            //    UserInput.LittleRollCount2 = tb_LittleRollCount2.Text;
            //     UserInput.LittleRollCount3 = tb_LittleRollCount3.Text;

            // UserInput.Width = tb_Width.Text;
            //    UserInput.Width2 = tb_Width2.Text;
            //    UserInput.Width3 = tb_Width3.Text;
            //UserInput.LittleRollCount2 = tb_LittleRollCount2.Text;
            //UserInput.LittleRollCount3 = tb_LittleRollCount3.Text;


            UserInput.CustomerName = tb_CustomerName.Text;
            //     UserInput.CustomerName2 = tb_CustomerName2.Text;
            //     UserInput.CustomerName3 = tb_CustomerName3.Text;

            UserInput.RecipeCode = tb_RecipeCode.Text;
            //   UserInput.RawMaterialCode2 = tb_RawMaterialCode2.Text;
            //   UserInput.RawMaterialCode3 = tb_RawMaterialCode3.Text;

            //UserInput.MaterialName = tb_MaterialName1.Text;
            //   UserInput.MaterialName2 = tb_MaterialName2.Text;
            //   UserInput.MaterialName3 = tb_MaterialName3.Text;

            UserInput.BatchNo = tb_BatchNo.Text;
            //  UserInput.BatchNo2 = tb_BatchNo2.Text;
            //  UserInput.BatchNo3 = tb_BatchNo3.Text;



            UserInput.WorkNo = tb_WorkNo.Text;
            UserInput.WorkHour = tb_ManHour.Text;
            UserInput.WorkerNo = tb_worker.Text;
            //  UserInput.QAMachineNo = tb_QAMachineNo.Text;
            //UserInput.JonitCount = tb_JointCount.Text;


            // UserInput.ProductState = GetProductState(cb_ProductState);
            //  UserInput.ProductQuality = GetProductQuality(cb_ProductQuality);
            //UserInput.ShowRealWeight = GetShowRealWeight(cb_ShowRealWight);

            //Get the Selected Item from ProductState/ ProductQuality / RealWeight
            UserInput.Desc = GetDesc(tb_Desc);
            UserInput.BigRollNo = GetCurBigRollNo(tb_BigRollNo);
            UserInput.LittleRollNo = GetCurLittleRollNo(tb_LittleRollNo);
            // UserInput.Weight = tb_LittleRollWeight.Text;
            UserInput.WorkClsType = GetWorkClassType();
            UserInput.WorkTType = GetWorkTimeType();
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

            lb_OutputBarcode.Text = UserInput.OutputBarcode;

            //UserInput.UpdateDateTime();
        }

        #region Get the data from the UI
        private String GetProductState(ComboBox productState_cb)
        {
            return productState_cb.SelectedItem.ToString();
        }

        private String GetProductQuality(ComboBox productQuality_cb)
        {
            return productQuality_cb.SelectedItem.ToString();
        }

        private String GetShowRealWeight(ComboBox showRealWight_cb)
        {
            return showRealWight_cb.SelectedItem.ToString();
        }

        private String GetDesc(TextBox tb)
        {
            return tb.Text;
        }

        private String GetCurBigRollNo(TextBox tb)
        {
            return tb.Text;
        }

        private String GetCurLittleRollNo(TextBox tb)
        {
            return tb.Text;
        }
        #endregion

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
                if (gVariable.orderNo != null)
                {
                    orderNo = gVariable.orderNo;
                    if (!UserInput.GetProductInfoBySaleOrder(orderNo, out UserInput.ProductCode, out productName, out UserInput.CustomerName))
                        return;
                    if (!UserInput.GetInfoByProductCode(UserInput.ProductCode, out UserInput.Width, out UserInput.RecipeCode, out fixture, out UserInput.CustomerName))
                        return;
                }

                UserInput.InputBarcode = barcode;
                lb_InputBarcode.Text = barcode;
                //tb_Width.Text = UserInput.Width;
                tb_RecipeCode.Text = UserInput.RecipeCode;
                tb_CustomerName.Text = UserInput.CustomerName;
                tb_BatchNo.Text = batchNo;
                tb_BatchNo.Text = UserInput.BatchNo;
                tb_BigRollNo.Text = UserInput.BigRollNo;
                tb_LittleRollNo.Text = UserInput.LittleRollNo;
                SetManufactureType(UserInput.MType);
                cb_ProductCode.Text = UserInput.ProductCode;


                tb_ManHour.Text = "0";
                tb_Desc.Text = "";


            }
        }

        private void cb_ProductState_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cb_ProductState.SelectedIndex == 0)//ok
            {
                cb_ProductQuality.Visible = false;
                lb_ProductQulity.Visible = false;
            }
            else//not ok
            {
                cb_ProductQuality.Visible = true;
                lb_ProductQulity.Visible = true;
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
            }
#endif
        }

        private void cb_ProductCode_SelectedIndexChanged(object sender, EventArgs e)
        {

            String ProductCode;
            String Width;
            String RecipeCode;
            String Fixture;
            String CustomerName;
            int comBoxIndex = 0;

            ProductCode = cb_ProductCode.Text;
            if (ProductCode == null || ProductCode == "")
                return;

            String RawMaterialCode = null;
            String productLength = null;
            String productName = null;
            String productWeight = null;

            try
            {
                //  ProductCode = cb_ProductCode1.Text;
                //UserInput.GetInfoByProductCode(ProductCode, out Width, out RecipeCode, out Fixture, out CustomerName);
                UserInput.GetInfoByProductCodeExt(ProductCode, out Width, out RecipeCode, out Fixture, out CustomerName, out RawMaterialCode, out productLength, out productName, out productWeight);
            }
            catch (Exception ex)
            {
                //    Log.e("QAForm", ex.Message);
                return;
            }
            tb_CustomerName.Text = CustomerName;
            tb_RecipeCode.Text = RecipeCode;
            //tb_Width.Text = Width;

        }

    }
}
