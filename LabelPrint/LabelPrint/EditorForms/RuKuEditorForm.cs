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
        int selId = 1;
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
            SysSetting.DynPrintData.setWorkProcess("RuKu");

            InitialDataForForm();
        }


        void InitRuKuComboBox()
        {
            object[] combosStrs = UserInput.GetComboStrsForMaterialCode();
            if (combosStrs != null)
            {
                cb_RawMaterialCode.Items.AddRange(combosStrs);
            }

            cb_TargetMachineNo.Items.AddRange(RuKuInputData.targets);
            cb_LiaoCangNo.Items.AddRange(RuKuInputData.LiaoCangNoStrs);

            String indexStr = UserInput.LiaoCangNo;
            int liaocanghao = 1;
            if (indexStr == null || indexStr == "")
                liaocanghao = 1;

            indexStr = indexStr.Substring(0, 1);
            if (int.TryParse(indexStr, out liaocanghao)==false)
            {
                liaocanghao = 1;
            }



            cb_LiaoCangNo.SelectedIndex = liaocanghao - 1;

            //cb_TargetMachineNo.SelectedIndex = 0;
        }
        private void InitialDataForForm()
        {
            InitRuKuComboBox();
            cb_TargetMachineNo.Text = UserInput.TargetMachineNo;
            cb_RawMaterialCode.Text = UserInput.RawMaterialCode;
            tb_RawMaterialBachNo.Text = UserInput.RawMaterialBatchNo;
            tb_XuQiuWeight.Text = UserInput.XuQiuWeight;
            tb_YiChuKuWeight.Text = UserInput.YiChuKuWeight;
            tb_BenCiChuKuWeight.Text = UserInput.BenCiChuKuWeight;
            tb_Desc.Text = UserInput.Desc;
            tb_WorkerNo.Text = UserInput.WorkerNo;

            tb_Date.Text = UserInput.WorkDate;
            tb_Time.Text = UserInput.WorkTime;
            tb_Date.Enabled = false;
            tb_Time.Enabled = false;
        }


        private void UpdateUserInput()
        {
            UserInput.WorkerNo = tb_WorkerNo.Text;
            UserInput.Desc = tb_Desc.Text;

            UserInput.TargetMachineNo = cb_TargetMachineNo.Text;

            UserInput.RawMaterialCode = cb_RawMaterialCode.Text;
            UserInput.RawMaterialBatchNo = tb_RawMaterialBachNo.Text;
            UserInput.XuQiuWeight = tb_XuQiuWeight.Text;
            UserInput.YiChuKuWeight = tb_YiChuKuWeight.Text;
            UserInput.BenCiChuKuWeight = tb_BenCiChuKuWeight.Text;

            int index;
            index = cb_LiaoCangNo.SelectedIndex + 1;
            UserInput.LiaoCangNo = index.ToString();
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
