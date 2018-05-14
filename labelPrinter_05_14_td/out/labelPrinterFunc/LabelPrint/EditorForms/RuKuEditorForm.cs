using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LabelPrint.Data;

namespace LabelPrint.EditorForms
{
    public partial class RuKuEditorForm : Form
    {
        DataTable dtEditor;
        int selId;
        RuKuInputData UserInput;
        public void SetSelItem(DataTable dt, int id)
        {
            this.selId = id;
            dtEditor = dt;
        }
        public RuKuEditorForm()
        {
            InitializeComponent();
        }

        private void RuKuEditorForm_Load(object sender, EventArgs e)
        {
            UserInput = new RuKuInputData();
            UserInput.GetSelItemFromDB(selId);
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            SysSetting.DynPrintData = new DynamicPrintLabelData();
            SysSetting.DynPrintData.setWorkProcess("Cut");

            InitialDataForForm();
        }

        private void InitialDataForForm()
        {
            tb_RawMaterialBatchNo.Text = UserInput.RawMaterialBatchNo;
            tb_RawMaterialCode.Text = UserInput.RawMaterialCode;
            tb_RawMaterialGrade.Text = UserInput.RawMaterialGrade;
            cb_RawMaterialName.Text = UserInput.MaterialName;
            tb_Vendor.Text = UserInput.Vendor;

            // tb_Recipe.Text = UserInput.Recipe;
            tb_WeightPerBag.Text = UserInput.WeightPerBag;
            //tb_StackWeight.Text = UserInput.StackWeight;
            tb_Bags_x.Text = UserInput.Bags_x;
            tb_Bags_y.Text = UserInput.Bags_y;

            tb_Bags_xy.Text = UserInput.Bags_xy;
            tb_OutBags.Text = UserInput.OutBags;
            tb_NeedBags.Text = UserInput.NeedBags;
            tb_WorkerNo.Text = UserInput.WorkerNo;
            tb_Desc.Text = UserInput.Desc;

        }
        private void UpdateUserInput()
        {
            UserInput.RawMaterialBatchNo = tb_RawMaterialBatchNo.Text;
            UserInput.MaterialName = cb_RawMaterialName.Text;
            UserInput.RawMaterialCode = tb_RawMaterialCode.Text;
            UserInput.RawMaterialGrade = tb_RawMaterialGrade.Text;
            UserInput.Vendor = tb_Vendor.Text;

            // UserInput.Recipe = tb_Recipe.Text;
            UserInput.WeightPerBag = tb_WeightPerBag.Text;
            //UserInput.StackWeight = tb_StackWeight.Text;
            UserInput.Bags_x = tb_Bags_x.Text;
            UserInput.Bags_y = tb_Bags_y.Text;

            UserInput.Bags_xy = tb_Bags_xy.Text;
            UserInput.OutBags = tb_OutBags.Text;
            UserInput.NeedBags = tb_NeedBags.Text;
            UserInput.WorkerNo = tb_WorkerNo.Text;
            UserInput.Desc = tb_Desc.Text;
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
    }
}
