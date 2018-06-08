﻿using System;
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
namespace LabelPrint
{
    public partial class CutEditorForm : Form
    {
        public CutEditorForm()
        {
            InitializeComponent();
        }

        DataTable dtEditor;
        int selId;
        CutUserinputData UserInput;
        public void SetSelItem(DataTable dt, int id)
        {
            this.selId = id;
            dtEditor = dt;
        }
        //String[] dataBase = { "id", "Date", "Time", "BatchNo", "CustomerName", "ProductCode", "ProductName", "Recipe", "BigRollNo", "LittleRollNo", "LittleWeight", "Quality", "State" };
        private void CutEditorForm_Load(object sender, EventArgs e)
        {
            UserInput = new CutUserinputData();
            UserInput.GetSelItemFromDB(selId);
//#pragma warning disable CS0168 // The variable 'SysData' is declared but never used
//            CutSampleData SysData;
//#pragma warning restore CS0168 // The variable 'SysData' is declared but never used
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            SysSetting.DynPrintData = new DynamicPrintLabelData();
            SysSetting.DynPrintData.setWorkProcess("Cut");
            //mySQLClass a = new mySQLClass();
            //dtEditor = mySQLClass.queryDataTableAction("sampledatabase", "Select * from Cut where id="+this.selId, null);
            //int id = (int)dtEditor.Rows[0][0];
            InitialDataForForm();


        }

        private void InitialDataForForm()
        {
            tb_Date.Enabled = false;
            tb_Time.Enabled = false;
            tb_Date.Text = UserInput.WorkDate;
            tb_Time.Text = UserInput.WorkTime;
            tb_BatchNo.Text = UserInput.BatchNo;
            tb_CustomerName.Text = UserInput.CustomerName;

            cb_WorkTime.Enabled = false;
            cb_WorkClass.Enabled = false;

            cb_WorkTime.Items.AddRange(ProcessData.WorkTimeTypes);
            cb_WorkClass.Items.AddRange(ProcessData.WorkClassTypes);

            cb_WorkTime.Text = UserInput.GetWorkTimeType();
            cb_WorkClass.Text = UserInput.GetWorkClassType();

            object[] productCodes = UserInput.GetComboStrsForProductCode();
            cb_ProductCode.Items.AddRange(productCodes);
            cb_ProductCode.Text = UserInput.ProductCode;
            tb_ProductName.Text = UserInput.GetProductNameByProductCode(UserInput.ProductCode);

            object[] customerCodes = UserInput.GetComboStrsForCustomerCode();
            cb_CustomerCodes.Items.AddRange(customerCodes);
            cb_CustomerCodes.Text = UserInput.GetCustomerCodeByCustomerName(UserInput.CustomerName);
            //tb_ProductCode1.Text = UserInput.ProductCode;
            //tb_ProductName.Text = (String)dtEditor.Rows[0][6];
            //tb_Recipe.Text = (String)dtEditor.Rows[0][7];
            tb_BigRollNo.Text = UserInput.BigRollNo;
            tb_LittleRollNo.Text = UserInput.LittleRollNo;

            //tb_LittleRollCount1.Text = UserInput.LittleRollCount;
            tb_LittleRollWeight.Text = UserInput.Weight;
            tb_CutMachineNo.Text = UserInput.CutMachineNo;
            tb_JointCount.Text = UserInput.JointCount;
            tb_ManHour.Text = UserInput.WorkHour;
            tb_WorkNo.Text = UserInput.WorkNo;
            tb_RecipeCode.Text = UserInput.RecipeCode;
            //tb_PlateLayer1.Text = UserInput.
            //tb_PlateRollPerLay1
            //tb_PlateRollNum1
            tb_Width.Text = UserInput.Width;
            //tb_RawMaterialCode.Text = UserInput.RawMaterialCode;
          //  tb_MaterialName.Text = UserInput.MaterialName;

            tb_Desc.Text = UserInput.Desc;

            InitProductStateComboBox(cb_ProductState);
            InitProductQualityComboBox(cb_ProductQuality);
            cb_ProductState.Text = UserInput.ProductState;
            cb_ProductQuality.Text = UserInput.ProductQuality;

            tb_PlateRollNum1.Text = UserInput.PlateRollNum;
            tb_PlateRollPerLay1.Text = UserInput.PlateRollPerLay;
            tb_PlateLayer1.Text = UserInput.PlateLayer;
            tb_PlateLayer1.Enabled = false;
            tb_PlateRollNum1.Enabled = false;
            tb_PlateRollPerLay1.Enabled = false;

            tb_PlateNo1.Text = UserInput.CurPlatInfo.PLateNo.ToString();

            tb_WorkerNo.Text = gVariable.userAccount;
            tb_WorkerNo.Enabled = false;

            tb_ProductLength.Text = UserInput.ProductLength;
        }
        void InitProductStateComboBox(ComboBox productStates)
        {
            productStates.Items.AddRange(ProcessData.ProductStateStr);

        }

        void InitProductQualityComboBox(ComboBox productQulity)
        {
            productQulity.Items.AddRange(ProcessData.ProductQualityStrForComBoList);

        }
        private void UpdateUserInput()
        {
            UserInput.ProductCode = cb_ProductCode.Text;
            UserInput.ProductCode1 = cb_ProductCode.Text;
            UserInput.ProductCode1 = cb_ProductCode.Text;
            //UserInput.LittleRollCount1 = tb_LittleRollCount1.Text;

            UserInput.CutMachineNo = tb_CutMachineNo.Text;
            UserInput.LittleRollNo = tb_LittleRollNo.Text;
            UserInput.Width1 = tb_Width.Text;


            UserInput.CustomerName1 = tb_CustomerName.Text;


           // UserInput.RawMaterialCode1 = tb_RawMaterialCode.Text;


         //   UserInput.MaterialName1 = tb_MaterialName.Text;
        //    UserInput.MaterialName2 = tb_MaterialName2.Text;
        //    UserInput.MaterialName3 = tb_MaterialName3.Text;

            UserInput.BatchNo1 = tb_BatchNo.Text;


            UserInput.RecipeCode = tb_RecipeCode.Text;
            UserInput.WorkNo = tb_WorkNo.Text;
            UserInput.WorkHour = tb_ManHour.Text;
            UserInput.WorkerNo = tb_WorkerNo.Text;
            UserInput.CutMachineNo = tb_CutMachineNo.Text;
            UserInput.JointCount = tb_JointCount.Text;
            UserInput.ProductName = tb_ProductName.Text;


            UserInput.Desc = tb_Desc.Text;


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

            // UserInput.ProductState = GetProductState(cb_ProductState);
            //  UserInput.ProductQuality = GetProductQuality(cb_ProductQuality);
            //  UserInput.ShowRealWeight = GetShowRealWeight(cb_ShowRealWight);

            //Get the Selected Item from ProductState/ ProductQuality / RealWeight
            UserInput.Desc = GetDesc(tb_Desc);
            UserInput.BigRollNo = GetCurBigRollNo(tb_BigRollNo);
            UserInput.LittleRollNo = GetCurLittleRollNo(tb_LittleRollNo);
            UserInput.Weight = tb_LittleRollWeight.Text;

            UserInput.PlateRollNum = tb_PlateRollNum1.Text;

            UserInput.PlateRollPerLay = tb_PlateRollPerLay1.Text;
            UserInput.PlateLayer = tb_PlateLayer1.Text;

            int.TryParse(tb_PlateNo1.Text, out UserInput.CurPlatInfo.PLateNo);
            int.TryParse(tb_PlateRollPerLay1.Text, out UserInput.CurPlatInfo.LittleRollPerlayer);
            int.TryParse(tb_PlateLayer1.Text, out UserInput.CurPlatInfo.Layer);


             UserInput.ProductLength = tb_ProductLength.Text;
        }

        private void UpdateProductData()
        {
            CutSampleData SysData;
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            //CutProductItem ProdItem;
            SysData = CutSampleData.Instance;
            //CutProductItem Item = SysData.GetCurProductItem();

            //.SelectedItem
            //label26.Text = ((DataRowView)cb_ShowRealWight.SelectedItem).Row["id"].ToString();

            UpdateUserInput();


            if (cb_ShowRealWight.SelectedItem != null)
                label26.Text = cb_ShowRealWight.SelectedItem.ToString();
            cb_ShowRealWight.DropDownStyle = ComboBoxStyle.Simple;

            DynamicPrintLabelData DynPrintData = SysSetting.DynPrintData;
            UserInput.UpdatePrintPrintData(DynPrintData);


            ////Get the Selected Item from ProductState/ProductQuality/RealWeight
            //CurProductState = GetProductState(cb_ProductState);
            //CurProductQuality = GetProductQuality(cb_ProductQuality);
            //CurShowRealWeight = GetShowRealWeight(cb_ShowRealWight);

            ////Get the DescriptionData
            //CurDesc = GetDesc(tb_Desc);
            //CurBigRollNo = GetCurBigRollNo(tb_BigRollNo);
            //CurLittleRollNo = GetCurLittleRollNo(tb_LittleRollNo);

            ////tb_worker
            //CurWorkNo = tb_OrderNo.Text;

        }

        private void bt_Save_Click(object sender, EventArgs e)
        {
            UpdateProductData();
            //updatedata
             UserInput.UpdateOneRow(selId);
            Close();
        }

        private void bt_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bt_Printing_Click(object sender, EventArgs e)
        {
            UpdateProductData();
            UserInput.UpdateDateTime();
            UserInput.PrintLabel();
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

        private void cb_ProductCode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            String fixture;
            UserInput.ProductCode = (String)cb_ProductCode.SelectedItem.ToString();
            UserInput.GetInfoByProductCode(UserInput.ProductCode, out UserInput.Width, out UserInput.RecipeCode, out fixture, out UserInput.CustomerName);

            UserInput.ParsePlateInfo(fixture);


            tb_PlateRollNum1.Text = UserInput.PlateRollNum;
            tb_PlateRollPerLay1.Text = UserInput.PlateRollPerLay;
            tb_PlateLayer1.Text = UserInput.PlateLayer;




            tb_Width.Text = UserInput.Width;
            tb_RecipeCode.Text = UserInput.RecipeCode;
            tb_CustomerName.Text = UserInput.CustomerName;

            tb_ProductName.Text = UserInput.GetProductNameByProductCode(UserInput.ProductCode);
            cb_CustomerCodes.Text = UserInput.GetCustomerCodeByCustomerName(UserInput.CustomerName);
        }

        private void cb_CustomerCodes_SelectionChangeCommitted(object sender, EventArgs e)
        {

            //int index = cb_CustomerCodes.SelectedIndex;
            String CustomerCode = cb_CustomerCodes.SelectedItem.ToString();
            tb_CustomerName.Text = UserInput.GetCustomerNameByCustomerCode(CustomerCode);
        }
    }
}