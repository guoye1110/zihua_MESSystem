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
namespace LabelPrint.EditorForms
{
    public partial class LiuYanEditorForm : Form
    {
        DataTable dtEditor;
        int selId;
        LiuYanUserinputData UserInput;
        public void SetSelItem(DataTable dt, int id)
        {
            this.selId = id;
            dtEditor = dt;
        }

        public LiuYanEditorForm()
        {
            InitializeComponent();
        }

        private void LiuYanEditorForm_Load(object sender, EventArgs e)
        {
            UserInput = new LiuYanUserinputData();
            UserInput.GetSelItemFromDB(selId);
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            SysSetting.DynPrintData = new DynamicPrintLabelData();
            SysSetting.DynPrintData.setWorkProcess("Cut");

            InitialDataForForm();
        }

        private void InitialDataForForm()
        {
            tb_Date.Enabled = false;
            tb_Time.Enabled = false;
            cb_WorkTime.Enabled = false;
            cb_WorkClass.Enabled = false;

            cb_WorkTime.Items.AddRange(ProcessData.WorkTimeTypes);
            cb_WorkClass.Items.AddRange(ProcessData.WorkClassTypes);

            cb_WorkTime.Text = UserInput.GetWorkTimeType();
            cb_WorkClass.Text = UserInput.GetWorkClassType();

            tb_Date.Text = UserInput.WorkDate;
            tb_Time.Text = UserInput.WorkTime;
            object[] productCodes = UserInput.GetComboStrsForProductCode();
            cb_ProductCode.Items.AddRange(productCodes);
            cb_ProductCode.Text = UserInput.ProductCode;
            tb_ProductName.Text = UserInput.GetProductNameByProductCode(UserInput.ProductCode);

            object[] customerCodes = UserInput.GetComboStrsForCustomerCode();
            //cb_CustomerCodes.Items.AddRange(customerCodes);
           // cb_CustomerCodes.Text = UserInput.GetCustomerCodeByCustomerName(UserInput.CustomerName);
            tb_Width.Text = UserInput.Width;
            //tb_LittleRollCount.Text = UserInput.LittleRollCount;
            tb_RecipeCode.Text = UserInput.RecipeCode;
            //tb_MaterialName1.Text = UserInput.MaterialName;
            tb_CustomerName.Text = UserInput.CustomerName;

            tb_BatchNo.Text = UserInput.BatchNo;
            tb_ManHour.Text = UserInput.WorkHour;
            tb_WorkNo.Text = UserInput.WorkNo;
            tb_worker.Text = UserInput.WorkerNo;

            tb_LiuYanMachineNo.Text = UserInput.LiuYanMachineNo;
            tb_BigRollNo.Text = UserInput.BigRollNo;
            tb_Desc.Text = UserInput.Desc;
            cb_ProductState.Items.AddRange(LiuYanUserinputData.PrintProductStateStr);
            cb_ProductState.Text = UserInput.ProductState;
            InitProductQualityComboBox(cb_ProductQuality);
            cb_ProductQuality.Text = UserInput.ProductQuality;
            tb_RollWeight.Text = UserInput.Roll_Weight;
        }
        void InitProductQualityComboBox(ComboBox productQulity)
        {
            productQulity.Items.AddRange(ProcessData.ProductQualityStrForComBoList);

        }
        private void UpdateUserInput()
        {
            UserInput.ProductCode = cb_ProductCode.Text;
            UserInput.Width = tb_Width.Text;
           // UserInput.LittleRollCount = tb_LittleRollCount.Text;
            UserInput.RecipeCode = tb_RecipeCode.Text;
           // UserInput.MaterialName = tb_MaterialName1.Text;
            UserInput.CustomerName = tb_CustomerName.Text;

            UserInput.BatchNo = tb_BatchNo.Text;
            UserInput.WorkHour = tb_ManHour.Text;
            UserInput.WorkNo = tb_WorkNo.Text;
            UserInput.WorkerNo = tb_worker.Text;

            UserInput.LiuYanMachineNo = tb_LiuYanMachineNo.Text;
            UserInput.BigRollNo = tb_BigRollNo.Text;
            UserInput.Desc = tb_Desc.Text;
            UserInput.ProductState = cb_ProductState.Text;
            UserInput.Roll_Weight = tb_RollWeight.Text;
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


           // if (cb_ShowRealWight.SelectedItem != null)
             //   label26.Text = cb_ShowRealWight.SelectedItem.ToString();
            //cb_ShowRealWight.DropDownStyle = ComboBoxStyle.Simple;

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

        private void cb_ProductCode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            String fixture;
            UserInput.ProductCode = (String)cb_ProductCode.SelectedItem.ToString();
            UserInput.GetInfoByProductCode(UserInput.ProductCode, out UserInput.Width, out UserInput.RecipeCode, out fixture, out UserInput.CustomerName);

            tb_Width.Text = UserInput.Width;
            tb_RecipeCode.Text = UserInput.RecipeCode;
            tb_CustomerName.Text = UserInput.CustomerName;

            tb_ProductName.Text = UserInput.GetProductNameByProductCode(UserInput.ProductCode);
            //cb_CustomerCodes.Text = UserInput.GetCustomerCodeByCustomerName(UserInput.CustomerName);
        }

        private void cb_CustomerCodes_SelectionChangeCommitted(object sender, EventArgs e)
        {

            //int index = cb_CustomerCodes.SelectedIndex;
          //  String CustomerCode = cb_CustomerCodes.SelectedItem.ToString();
          //  tb_CustomerName.Text = UserInput.GetCustomerNameByCustomerCode(CustomerCode);
        }

        private void cb_WorkClass_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
