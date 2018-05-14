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
    public partial class PackEditorForm : Form
    {
        DataTable dtEditor;
        int selId;
        PackUserinputData UserInput;
        public void SetSelItem(DataTable dt, int id)
        {
            this.selId = id;
            dtEditor = dt;
        }
        public PackEditorForm()
        {
            InitializeComponent();
        }

        private void bt_Save_Click(object sender, EventArgs e)
        {
            UpdateProductData();
            //updatedata
            UserInput.UpdateOneRow(selId);
            Close();
        }
        private void InitialDataForForm()
        {
          //  tb_Date.Enabled = false;
           // tb_Time.Enabled = false;
            cb_WorkTime.Items.AddRange(ProcessData.WorkTimeTypes);
            cb_WorkClass.Items.AddRange(ProcessData.WorkClassTypes);

            cb_WorkTime.Text = UserInput.GetWorkTimeType();
            cb_WorkClass.Text = UserInput.GetWorkClassType();


            cb_WorkTime.Enabled = false;
            cb_WorkClass.Enabled = false;
            cb_WorkTime.Text = UserInput.GetWorkTimeType();
            cb_WorkClass.Text = UserInput.GetWorkClassType();

           // tb_Date.Text = UserInput.WorkDate;
           // tb_Time.Text = UserInput.WorkTime;
            object[] productCodes = UserInput.GetComboStrsForProductCode();
            cb_ProductCode.Items.AddRange(productCodes);
            cb_ProductCode.Text = UserInput.ProductCode;
           // tb_ProductName.Text = UserInput.GetProductNameByProductCode(UserInput.ProductCode);

            object[] customerCodes = UserInput.GetComboStrsForCustomerCode();
          //  cb_CustomerCodes.Items.AddRange(customerCodes);
          //  cb_CustomerCodes.Text = UserInput.GetCustomerCodeByCustomerName(UserInput.CustomerName);
            tb_Width.Text = UserInput.Width;
            tb_LittleRollCount.Text = UserInput.LittleRollCount;
            tb_RecipeCode.Text = UserInput.RecipeCode;
            tb_MaterialName.Text = UserInput.MaterialName;
            tb_CustomerName.Text = UserInput.CustomerName;

            tb_BatchNo.Text = UserInput.BatchNo;
            tb_worker.Text = UserInput.WorkerNo;
            tb_ManHour.Text = UserInput.WorkHour;
            tb_WorkNo.Text = UserInput.WorkNo;

            tb_PackMachineNo.Text = UserInput.PackMachineNo;
            tb_BigRollNo.Text = UserInput.BigRollNo;
            tb_Desc.Text = UserInput.Desc;
            tb_LittleRollNo.Text = UserInput.LittleRollNo;
            
            tb_Roll_Weight.Text = UserInput.Roll_Weight;
            tb_ProductLength.Text = UserInput.ProductLength;
            tb_ProductWeight.Text = UserInput.ProductWeight;
            tb_PlateNo.Text = UserInput.PlateNo;
            tb_RawMaterialCode.Text = UserInput.RawMaterialCode;
            tb_OrderNo.Text = UserInput.OrderNo;
        }

        private void UpdateUserInput()
        {
            UserInput.ProductCode = cb_ProductCode.Text;
            UserInput.Width = tb_Width.Text;
            UserInput.LittleRollCount = tb_LittleRollCount.Text;
            UserInput.RecipeCode = tb_RecipeCode.Text;
            UserInput.MaterialName = tb_MaterialName.Text;
            UserInput.CustomerName = tb_CustomerName.Text;

            UserInput.BatchNo = tb_BatchNo.Text;
            UserInput.WorkHour = tb_ManHour.Text;
            UserInput.WorkNo = tb_WorkNo.Text;
            UserInput.WorkerNo = tb_worker.Text;

            UserInput.PackMachineNo = tb_PackMachineNo.Text;
            UserInput.BigRollNo = tb_BigRollNo.Text;
            UserInput.Desc = tb_Desc.Text;
            UserInput.LittleRollNo = tb_LittleRollNo.Text;

            UserInput.Roll_Weight = tb_Roll_Weight.Text;

            UserInput.ProductLength = tb_ProductLength.Text;
            UserInput.ProductWeight = tb_ProductWeight.Text;

            UserInput.PlateNo = tb_PlateNo.Text;
            UserInput.RawMaterialCode = tb_RawMaterialCode.Text;
            UserInput.OrderNo = tb_OrderNo.Text;
        }

        private void UpdateProductData()
        {
            CutSampleData SysData;
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            //CutProductItem ProdItem;
            SysData = CutSampleData.Instance;

            UpdateUserInput();

            DynamicPrintLabelData DynPrintData = SysSetting.DynPrintData;
            UserInput.UpdatePrintPrintData(DynPrintData);

        }

        private void PackEditorForm_Load(object sender, EventArgs e)
        {
            UserInput = new PackUserinputData();
            UserInput.GetSelItemFromDB(selId);
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            SysSetting.DynPrintData = new DynamicPrintLabelData();
            SysSetting.DynPrintData.setWorkProcess("Cut");

            InitialDataForForm();
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

        #endregion

        private void bt_Printing_Click(object sender, EventArgs e)
        {
            UpdateProductData();
            UserInput.PrintLabel();
        }

        private void bt_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cb_ProductCode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //String fixture;
            //UserInput.ProductCode = (String)cb_ProductCode.SelectedItem.ToString();
            //UserInput.GetInfoByProductCode(UserInput.ProductCode, out UserInput.Width, out UserInput.RecipeCode, out fixture, out UserInput.CustomerName);

            //tb_Width.Text = UserInput.Width;
            //tb_RecipeCode.Text = UserInput.RecipeCode;
            //tb_CustomerName.Text = UserInput.CustomerName;

            // tb_ProductName.Text = UserInput.GetProductNameByProductCode(UserInput.ProductCode);
            // cb_CustomerCodes.Text = UserInput.GetCustomerCodeByCustomerName(UserInput.CustomerName);


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
        }

        private void cb_CustomerCodes_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //int index = cb_CustomerCodes.SelectedIndex;
        //    String CustomerCode = cb_CustomerCodes.SelectedItem.ToString();
          //  tb_CustomerName.Text = UserInput.GetCustomerNameByCustomerCode(CustomerCode);
        }
    }
}
