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
    public partial class OutBoundingEditorForm : Form
    {
        DataTable dtEditor;
        int selId;
        OutBoundingInputData UserInput;

        const int MAX_LIAOCANG_NUM = 8;

        ComboBox[] cb_RawMaterialCodes = new ComboBox[MAX_LIAOCANG_NUM];
        TextBox[] tb_RawMaterialBachNos = new TextBox[MAX_LIAOCANG_NUM];
        TextBox[] tb_XuQiuWeights = new TextBox[MAX_LIAOCANG_NUM];
        TextBox[] tb_YiChuKuWeights = new TextBox[MAX_LIAOCANG_NUM];
        TextBox[] tb_BenCiChuKuWeights = new TextBox[MAX_LIAOCANG_NUM];

        public void SetSelItem(DataTable dt, int id)
        {
            this.selId = id;
            dtEditor = dt;
        }

        public OutBoundingEditorForm()
        {
            InitializeComponent();
        }


        void InitComponentsArray()
        {
            cb_RawMaterialCodes[0] = cb_RawMaterialCode1;
            cb_RawMaterialCodes[1] = cb_RawMaterialCode2;
            cb_RawMaterialCodes[2] = cb_RawMaterialCode3;
            cb_RawMaterialCodes[3] = cb_RawMaterialCode4;
            cb_RawMaterialCodes[4] = cb_RawMaterialCode5;
            cb_RawMaterialCodes[5] = cb_RawMaterialCode6;
            cb_RawMaterialCodes[6] = cb_RawMaterialCode7;
            cb_RawMaterialCodes[7] = cb_RawMaterialCode8;

            tb_RawMaterialBachNos[0] = tb_BachNo1;
            tb_RawMaterialBachNos[1] = tb_BachNo2;
            tb_RawMaterialBachNos[2] = tb_BachNo3;
            tb_RawMaterialBachNos[3] = tb_BachNo4;
            tb_RawMaterialBachNos[4] = tb_BachNo5;
            tb_RawMaterialBachNos[5] = tb_BachNo6;
            tb_RawMaterialBachNos[6] = tb_BachNo7;
            tb_RawMaterialBachNos[7] = tb_BachNo8;

            tb_XuQiuWeights[0] = tb_XuQiu1;
            tb_XuQiuWeights[1] = tb_XuQiu2;
            tb_XuQiuWeights[2] = tb_XuQiu3;
            tb_XuQiuWeights[3] = tb_XuQiu4;
            tb_XuQiuWeights[4] = tb_XuQiu5;
            tb_XuQiuWeights[5] = tb_XuQiu6;
            tb_XuQiuWeights[6] = tb_XuQiu7;
            tb_XuQiuWeights[7] = tb_XuQiu8;

            tb_YiChuKuWeights[0] = tb_YiChuKu1;
            tb_YiChuKuWeights[1] = tb_YiChuKu2;
            tb_YiChuKuWeights[2] = tb_YiChuKu3;
            tb_YiChuKuWeights[3] = tb_YiChuKu4;
            tb_YiChuKuWeights[4] = tb_YiChuKu5;
            tb_YiChuKuWeights[5] = tb_YiChuKu6;
            tb_YiChuKuWeights[6] = tb_YiChuKu7;
            tb_YiChuKuWeights[7] = tb_YiChuKu8;

            tb_BenCiChuKuWeights[0] = tb_BenCiChuKu1;
            tb_BenCiChuKuWeights[1] = tb_BenCiChuKu2;
            tb_BenCiChuKuWeights[2] = tb_BenCiChuKu3;
            tb_BenCiChuKuWeights[3] = tb_BenCiChuKu4;
            tb_BenCiChuKuWeights[4] = tb_BenCiChuKu5;
            tb_BenCiChuKuWeights[5] = tb_BenCiChuKu6;
            tb_BenCiChuKuWeights[6] = tb_BenCiChuKu7;
            tb_BenCiChuKuWeights[7] = tb_BenCiChuKu8;
        }


        private void OutBoundingEditorForm_Load(object sender, EventArgs e)
        {
            UserInput = new OutBoundingInputData();
            UserInput.GetSelItemFromDB(selId);
            SystemSetting SysSetting;
            SysSetting = GlobalConfig.Setting;
            SysSetting.DynPrintData = new DynamicPrintLabelData();
            SysSetting.DynPrintData.setWorkProcess("Cut");

            InitialDataForForm();
        }

        void setLiaoCangNo()
        {
            rb_button1.Checked = false;
            rb_button2.Checked = false;
            rb_button3.Checked = false;
            rb_button4.Checked = false;
            rb_button5.Checked = false;
            rb_button6.Checked = false;
            rb_button7.Checked = false;
            rb_button8.Checked = false;
            if (UserInput.LiaoCangNo != null && UserInput.LiaoCangNo != "")
            {
                if (String.Compare(UserInput.LiaoCangNo, "1") == 0)
                {
                    rb_button1.Checked = true;
                }
                if (String.Compare(UserInput.LiaoCangNo, "2") == 0)
                {
                    rb_button2.Checked = true;
                }
                if (String.Compare(UserInput.LiaoCangNo, "3") == 0)
                {
                    rb_button3.Checked = true;
                }
                if (String.Compare(UserInput.LiaoCangNo, "4") == 0)
                {
                    rb_button4.Checked = true;
                }
                if (String.Compare(UserInput.LiaoCangNo, "5") == 0)
                {
                    rb_button5.Checked = true;
                }
                if (String.Compare(UserInput.LiaoCangNo, "6") == 0)
                {
                    rb_button6.Checked = true;
                }
                if (String.Compare(UserInput.LiaoCangNo, "7") == 0)
                {
                    rb_button7.Checked = true;
                }
                if (String.Compare(UserInput.LiaoCangNo, "8") == 0)
                {
                    rb_button8.Checked = true;
                }
            }

        }


        void SetRadioComStateByLiaoCangNo(int LiaoCangNo)
        {
            for (int i = 0; i < MAX_LIAOCANG_NUM; i++)
            {
                if (LiaoCangNo == (i + 1))
                {

                    cb_RawMaterialCodes[i].Enabled = true;
                    tb_RawMaterialBachNos[i].Enabled = true;
                    tb_XuQiuWeights[i].Enabled = true;
                    tb_YiChuKuWeights[i].Enabled = true;
                    tb_BenCiChuKuWeights[i].Enabled = true;


                }
                else
                {
                    cb_RawMaterialCodes[i].Enabled = false;
                    tb_RawMaterialBachNos[i].Enabled = false;
                    tb_XuQiuWeights[i].Enabled = false;
                    tb_YiChuKuWeights[i].Enabled = false;
                    tb_BenCiChuKuWeights[i].Enabled = false;
                    cb_RawMaterialCodes[i].Text = null;
                    tb_RawMaterialBachNos[i].Text = null;
                    tb_XuQiuWeights[i].Text = null;
                    tb_YiChuKuWeights[i].Text = null;
                    tb_BenCiChuKuWeights[i].Text = null;
                }
            }
        }


        void setDataAccordingToLiaoCangNo(int LiaoCangNo)
        {
            SetRadioComStateByLiaoCangNo(LiaoCangNo);
            //Why could not update the Index
            cb_RawMaterialCodes[LiaoCangNo - 1].Text = UserInput.RawMaterialCode;

            String str = cb_RawMaterialCodes[LiaoCangNo - 1].Text;
            tb_RawMaterialBachNos[LiaoCangNo - 1].Text = UserInput.RawMaterialBatchNo;
            tb_XuQiuWeights[LiaoCangNo - 1].Text = UserInput.XuQiuWeight;
            tb_YiChuKuWeights[LiaoCangNo - 1].Text = UserInput.YiChuKuWeight;
            tb_BenCiChuKuWeights[LiaoCangNo - 1].Text = UserInput.BenCiChuKuWeight;

        }

        void setDataAccordingToLiaoCangNo()
        {
            int LiaoCangNo;
            Boolean val = int.TryParse(UserInput.LiaoCangNo, out LiaoCangNo);
            if (val)
            {
                setDataAccordingToLiaoCangNo(LiaoCangNo);
            }
            //switch (UserInput.LiaoCangNo)
            //{
            //    case "1":
            //        cb_RawMaterialCode1.Text = UserInput.RawMaterialCode;
            //        tb_BachNo1.Text = UserInput.RawMaterialBatchNo;
            //        tb_XuQiu1.Text = UserInput.XuQiuWeight;
            //        tb_YiChuKu1.Text = UserInput.YiChuKuWeight;
            //        tb_BenCiChuKu1.Text = UserInput.BenCiChuKuWeight;
            //        break;

            //    case "2":
            //        cb_RawMaterialCode2.Text = UserInput.RawMaterialCode;
            //        tb_BachNo2.Text = UserInput.RawMaterialBatchNo;
            //        tb_XuQiu2.Text = UserInput.XuQiuWeight;
            //        tb_YiChuKu2.Text = UserInput.YiChuKuWeight;
            //        tb_BenCiChuKu2.Text = UserInput.BenCiChuKuWeight;
            //        break;

            //    case "3":
            //        cb_RawMaterialCode3.Text = UserInput.RawMaterialCode;
            //        tb_BachNo3.Text = UserInput.RawMaterialBatchNo;
            //        tb_XuQiu3.Text = UserInput.XuQiuWeight;
            //        tb_YiChuKu3.Text = UserInput.YiChuKuWeight;
            //        tb_BenCiChuKu3.Text = UserInput.BenCiChuKuWeight;
            //        break;

            //    case "4":
            //        cb_RawMaterialCode4.Text = UserInput.RawMaterialCode;
            //        tb_BachNo4.Text = UserInput.RawMaterialBatchNo;
            //        tb_XuQiu4.Text = UserInput.XuQiuWeight;
            //        tb_YiChuKu4.Text = UserInput.YiChuKuWeight;
            //        tb_BenCiChuKu4.Text = UserInput.BenCiChuKuWeight;
            //        break;

            //    case "5":
            //        cb_RawMaterialCode5.Text = UserInput.RawMaterialCode;
            //        tb_BachNo5.Text = UserInput.RawMaterialBatchNo;
            //        tb_XuQiu5.Text = UserInput.XuQiuWeight;
            //        tb_YiChuKu5.Text = UserInput.YiChuKuWeight;
            //        tb_BenCiChuKu5.Text = UserInput.BenCiChuKuWeight;
            //        break;

            //    case "6":
            //        cb_RawMaterialCode6.Text = UserInput.RawMaterialCode;
            //        tb_BachNo6.Text = UserInput.RawMaterialBatchNo;
            //        tb_XuQiu6.Text = UserInput.XuQiuWeight;
            //        tb_YiChuKu6.Text = UserInput.YiChuKuWeight;
            //        tb_BenCiChuKu6.Text = UserInput.BenCiChuKuWeight;
            //        break;

            //    case "7":
            //        cb_RawMaterialCode7.Text = UserInput.RawMaterialCode;
            //        tb_BachNo7.Text = UserInput.RawMaterialBatchNo;
            //        tb_XuQiu7.Text = UserInput.XuQiuWeight;
            //        tb_YiChuKu7.Text = UserInput.YiChuKuWeight;
            //        tb_BenCiChuKu7.Text = UserInput.BenCiChuKuWeight;
            //        break;

            //    case "8":
            //        cb_RawMaterialCode8.Text = UserInput.RawMaterialCode;
            //        tb_BachNo8.Text = UserInput.RawMaterialBatchNo;
            //        tb_XuQiu8.Text = UserInput.XuQiuWeight;
            //        tb_YiChuKu8.Text = UserInput.YiChuKuWeight;
            //        tb_BenCiChuKu8.Text = UserInput.BenCiChuKuWeight;
            //        break;
            //}



        }
        private void InitialDataForForm()
        {
            InitComponentsArray();

            setLiaoCangNo();

            object[] combosStrs = UserInput.GetComboStrsForProductCode();
            if (combosStrs != null)
            {
                for (int i = 0; i < MAX_LIAOCANG_NUM; i++)
                {
                    cb_RawMaterialCodes[i].Items.AddRange(combosStrs);
                }
            }
            setDataAccordingToLiaoCangNo();

            //tb_RawMaterialBatchNo.Text = UserInput.RawMaterialBatchNo;
            //tb_RawMaterialCode.Text = UserInput.RawMaterialCode;
            //tb_RawMaterialGrade.Text = UserInput.RawMaterialGrade;
            //cb_RawMaterialName.Text = UserInput.MaterialName;
            //tb_Vendor.Text = UserInput.Vendor;

            //// tb_Recipe.Text = UserInput.Recipe;
            //tb_WeightPerBag.Text = UserInput.WeightPerBag;
            ////tb_StackWeight.Text = UserInput.StackWeight;
            //tb_Bags_x.Text = UserInput.Bags_x;
            //tb_Bags_y.Text = UserInput.Bags_y;

            //tb_Bags_xy.Text = UserInput.Bags_xy;
            //tb_OutBags.Text = UserInput.OutBags;
            //tb_NeedBags.Text = UserInput.NeedBags;
            tb_WorkerNo.Text = UserInput.WorkerNo;
            tb_Desc.Text = UserInput.Desc;

          //  tb_DateTime.Text = UserInput.GetDateTime();


           // tb_DateTime.Enabled = false;
            tb_WorkerNo.Text = gVariable.userAccount;
            tb_WorkerNo.Enabled = false;
            //object[] combosStrs = UserInput.GetComboStrsForMaterialName();
            //if (combosStrs != null)
            //    cb_RawMaterialName.Items.AddRange(combosStrs);

            tb_Date.Text = UserInput.WorkDate;
            tb_Time.Text = UserInput.WorkTime;
            tb_Date.Enabled = false;
            tb_Time.Enabled = false;

            cb_TargetMachineNo.Items.AddRange(OutBoundingInputData.targets);
            cb_TargetMachineNo.Text = UserInput.TargetMachineNo;
        }

        void UpdatUserInputForRadiOBoxByLiaoCangHao()
        {

            int LiaoCangNo;
            Boolean val = int.TryParse(UserInput.LiaoCangNo, out LiaoCangNo);
            if (val)
            {
                UserInput.RawMaterialCode = cb_RawMaterialCodes[LiaoCangNo - 1].Text;
                UserInput.RawMaterialBatchNo = tb_RawMaterialBachNos[LiaoCangNo - 1].Text;
                UserInput.XuQiuWeight = tb_XuQiuWeights[LiaoCangNo - 1].Text;
                UserInput.YiChuKuWeight = tb_YiChuKuWeights[LiaoCangNo - 1].Text;
                UserInput.BenCiChuKuWeight = tb_BenCiChuKuWeights[LiaoCangNo - 1].Text;
            }

            //switch (UserInput.LiaoCangNo)
            //{
            //    case "1":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode1.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo1.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu1.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu1.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu1.Text;
            //        break;
            //    case "2":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode2.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo2.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu2.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu2.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu2.Text;
            //        break;
            //    case "3":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode3.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo3.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu3.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu3.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu3.Text;
            //        break;
            //    case "4":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode4.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo4.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu4.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu4.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu4.Text;
            //        break;
            //    case "5":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode5.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo5.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu5.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu5.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu5.Text;
            //        break;
            //    case "6":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode6.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo6.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu6.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu6.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu6.Text;
            //        break;
            //    case "7":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode7.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo7.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu7.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu7.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu7.Text;
            //        break;
            //    case "8":
            //        UserInput.RawMaterialCode = cb_RawMaterialCode8.Text;
            //        UserInput.RawMaterialBatchNo = tb_BachNo8.Text;
            //        UserInput.XuQiuWeight = tb_XuQiu8.Text;
            //        UserInput.YiChuKuWeight = tb_YiChuKu8.Text;
            //        UserInput.BenCiChuKuWeight = tb_BenCiChuKu8.Text;
            //        break;
            //    default:
            //        break;
            //}
        }


        private void UpdateUserInput()
        {
            UpdatUserInputForRadiOBoxByLiaoCangHao();
            //UserInput.RawMaterialBatchNo = tb_RawMaterialBatchNo.Text;
            //UserInput.MaterialName = cb_RawMaterialName.Text;
            ////UserInput.RawMaterialCode = tb_RawMaterialCode.Text;
            //UserInput.RawMaterialGrade = tb_RawMaterialGrade.Text;
            //UserInput.Vendor = tb_Vendor.Text;

            //// UserInput.Recipe = tb_Recipe.Text;
            //UserInput.WeightPerBag = tb_WeightPerBag.Text;
            ////UserInput.StackWeight = tb_StackWeight.Text;
            //UserInput.Bags_x = tb_Bags_x.Text;
            //UserInput.Bags_y = tb_Bags_y.Text;

            //UserInput.Bags_xy = tb_Bags_xy.Text;
            //UserInput.OutBags = tb_OutBags.Text;
            //UserInput.NeedBags = tb_NeedBags.Text;
            //UserInput.WorkerNo = tb_WorkerNo.Text;
            UserInput.Desc = tb_Desc.Text;
            UserInput.TargetMachineNo = cb_TargetMachineNo.Text;
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



        private void rb_LiaoCangNoClick(object sender, EventArgs e)
        {
            if (rb_button1.Checked)
            {
                UserInput.LiaoCangNo = "1";
                SetRadioComStateByLiaoCangNo(1);
            }
            else if (rb_button2.Checked)
            {
                UserInput.LiaoCangNo = "2";
                SetRadioComStateByLiaoCangNo(2);
            }
            else if (rb_button3.Checked)
            {
                UserInput.LiaoCangNo = "3";
                SetRadioComStateByLiaoCangNo(3);
            }
            else if (rb_button4.Checked)
            {
                UserInput.LiaoCangNo = "4";
                SetRadioComStateByLiaoCangNo(4);
            }
            else if (rb_button5.Checked)
            {
                UserInput.LiaoCangNo = "5";
                SetRadioComStateByLiaoCangNo(5);
            }
            else if (rb_button6.Checked)
            {
                UserInput.LiaoCangNo = "6";
                SetRadioComStateByLiaoCangNo(6);
            }
            else if (rb_button7.Checked)
            {
                UserInput.LiaoCangNo = "7";
                SetRadioComStateByLiaoCangNo(7);
            }
            else if (rb_button8.Checked)
            {
                UserInput.LiaoCangNo = "8";
                SetRadioComStateByLiaoCangNo(8);
            }
        }

        private void cb_RawMaterialName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    UserInput.MaterialName = cb_RawMaterialName.SelectedItem.ToString();
            //    UserInput.GetInfoByMaterialName(UserInput.MaterialName, out UserInput.RawMaterialCode, out UserInput.Vendor, out UserInput.WeightPerBag, out UserInput.NeedBags);
            //}
            //catch (Exception arg)
            //{

            //}
            //tb_Vendor.Text = UserInput.Vendor;
            //tb_RawMaterialCode.Text = UserInput.RawMaterialCode;
            //tb_RawMaterialGrade.Text = "0";
            //tb_NeedBags.Text = UserInput.NeedBags;
            //tb_WeightPerBag.Text = UserInput.WeightPerBag;
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
