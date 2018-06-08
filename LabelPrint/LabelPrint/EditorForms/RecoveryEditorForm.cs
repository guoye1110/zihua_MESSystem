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
    public partial class RecoveryEditorForm : Form
    {
        DataTable dtEditor;
        int selId;
        RcvInputData UserInput;
        public void SetSelItem(DataTable dt, int id)
        {
            this.selId = id;
            dtEditor = dt;
        }
        public RecoveryEditorForm()
        {
            InitializeComponent();
        }

        private void InitialDataForForm()
        {
            tb_Date.Text = UserInput.WorkDate;
            tb_Time.Text = UserInput.WorkTime;
            tb_Date.Enabled = false;
            tb_Time.Enabled = false;
            //tb_WorkProcess.Text = UserInput.WorkProcess;
            tb_RecipeNo.Text = UserInput.RecipeCode;
            tb_Color.Text = UserInput.Color;
            // tb_Vendor.Text = UserInput.Vendor;
            tb_TMaterialWeight.Text = UserInput.TMaterialWeight;
            tb_RecoveryWeight.Text = UserInput.RecoveryWeight;
           // tb_Bagx.Text = UserInput.Bags_x;
           // tb_Bagy.Text = UserInput.Bags_y;
           // tb_Bagxy.Text = UserInput.Bags_xy;
            tb_WorkerNo.Text = UserInput.WorkerNo;
            tb_Desc.Text = UserInput.Desc;

            tb_RecoveryMachineNo.Text = UserInput.RecoveryMachineNo;
            tb_OldCode1.Text = UserInput.OldCode1;
            tb_OldCode2.Text = UserInput.OldCode2;
            tb_OldCode3.Text = UserInput.OldCode3;
            tb_OldCode4.Text = UserInput.OldCode4;
            tb_OldCode5.Text = UserInput.OldCode5;
            tb_OldCode6.Text = UserInput.OldCode6;
            tb_OldCode7.Text = UserInput.OldCode7;
            tb_OldCode8.Text = UserInput.OldCode8;
            tb_OldCode9.Text = UserInput.OldCode9;
            tb_OldCode10.Text = UserInput.OldCode10;


        }
        private void UpdateUserInput()
        {
          //  UserInput.WorkProcess = tb_WorkProcess.Text;
            UserInput.RecipeCode = tb_RecipeNo.Text;
            UserInput.Color = tb_Color.Text;
            //UserInput.Vendor = tb_Vendor.Text;
            UserInput.TMaterialWeight = tb_TMaterialWeight.Text;
            UserInput.RecoveryWeight = tb_RecoveryWeight.Text;
           // UserInput.Bags_x = tb_Bagx.Text;
           // UserInput.Bags_y = tb_Bagy.Text;
            //UserInput.Bags_xy = tb_Bagxy.Text;
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
            CutSampleData SysData;
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            //CutProductItem ProdItem;
            SysData = CutSampleData.Instance;

            UpdateUserInput();

            DynamicPrintLabelData DynPrintData = SysSetting.DynPrintData;
            UserInput.UpdatePrintPrintData(DynPrintData);
        }

        private void RecoveryEditorForm_Load(object sender, EventArgs e)
        {
            UserInput = new RcvInputData();
            UserInput.GetSelItemFromDB(selId);
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            SysSetting.DynPrintData = new DynamicPrintLabelData();
            SysSetting.DynPrintData.setWorkProcess("Cut");
          
            InitialDataForForm();
            tb_RecoveryMachineNo.Enabled = false;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

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
