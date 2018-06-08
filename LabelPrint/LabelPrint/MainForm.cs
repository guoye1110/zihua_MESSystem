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
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using LabelPrint.QueryForms;

namespace LabelPrint
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();


        }
        SettingInfo info;
        private void button1_Click(object sender, EventArgs e)
        {

            CutSystemForm f = new CutSystemForm();
            f.ShowDialog();

            //switch ((ManufacturePhaseType)GlobalConfig.Setting.CurSettingInfo.ManufacturePhase)
            //{
            //    case ManufacturePhaseType.CUT:
            //        {

            //            CutSystemForm f = new CutSystemForm();
            //            f.ShowDialog();
            //        }
            //        break;
            //    case ManufacturePhaseType.PRINT:
            //        {
            //            PrintSysForm f = new PrintSysForm();
            //            f.ShowDialog();
            //        }
            //        break;
            //    case ManufacturePhaseType.LIUYAN:
            //        {
            //            LiuYanSysForm f = new LiuYanSysForm();
            //            f.ShowDialog();
            //        }
            //        break;
            //    case ManufacturePhaseType.OUTBOUNING:
            //        {
            //            OutBoundingSysForm f = new OutBoundingSysForm();
            //            f.ShowDialog();
            //        }
            //        break;
            //    case ManufacturePhaseType.QUALITYCHECK:
            //        {
            //            QASysForm f = new QASysForm();
            //            f.ShowDialog();
            //        }
            //        break;
            //    case ManufacturePhaseType.RECOVERY:
            //        {
            //            RecoverySysForm f = new RecoverySysForm();
            //            f.ShowDialog();
            //        }
            //        break;
            //    case ManufacturePhaseType.PACK:
            //        {
            //            PackSysForm f = new PackSysForm();
            //            f.ShowDialog();
            //        }
            //        break;
            //}
        }

        void DisableAllSitesButton()
        {
            bt_OutBoundingSys.Enabled = false;
            bt_LiuYanSys.Enabled = false;
            bt_PrintSys.Enabled = false;
            Bt_CutSys.Enabled = false;
            bt_PackSys.Enabled = false;
            bt_QASys.Enabled = false;
            bt_Recovery.Enabled = false;
        }

        void SetSiteButton(int type)
        {
            //Bt_CutSys.Text = gVariable.SiteSystems[(int)type];
            DisableAllSitesButton();
            switch ((ManufacturePhaseType)GlobalConfig.Setting.CurSettingInfo.ManufacturePhase)
            {
                case ManufacturePhaseType.CUT:
                    {
                        Bt_CutSys.Enabled = true;
                    }
                    break;
                case ManufacturePhaseType.PRINT:
                    {
                        bt_PrintSys.Enabled = true;
                    }
                    break;
                case ManufacturePhaseType.LIUYAN:
                    {
                        bt_LiuYanSys.Enabled = true;
                    }
                    break;
                case ManufacturePhaseType.OUTBOUNING:
                    {
                        bt_OutBoundingSys.Enabled = true;
                    }
                    break;
                case ManufacturePhaseType.QUALITYCHECK:
                    {
                        bt_QASys.Enabled = true;
                    }
                    break;
                case ManufacturePhaseType.RECOVERY:
                    {
                        bt_Recovery.Enabled = true;
                    }
                    break;
                case ManufacturePhaseType.PACK:
                    {
                        bt_PackSys.Enabled = true;
                    }
                    break;
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {

            GlobalConfig.Setting = new SystemSetting();
            mySQLClass mysql = new mySQLClass();

            mySQLClass.createDatabase();

            this.Icon = new Icon("..\\..\\resources\\zihualogo_48X48.ico");
            label3.Text = gVariable.userAccount;

            info = GlobalConfig.Setting.RestoreSystemSettingsFromJsonFile();
            if (info==null)
                info = GlobalConfig.Setting.GetDefaultSetting();
            GlobalConfig.Setting.CurSettingInfo = info;
            // GlobalConfig.Setting.CurWorker = "101";

            SetSiteButton(info.ManufacturePhase);

        }

        private void Bt_SysSetting_Click(object sender, EventArgs e)
        {
            SystemSettingForm f = new SystemSettingForm();
            f.ShowDialog();
            SetSiteButton(info.ManufacturePhase);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LabelDesignForm f = new LabelDesignForm();
            f.ShowDialog();

        }

        private void bt_BasicData_Click(object sender, EventArgs e)
        {
            BasicDataForm f = new BasicDataForm();
            f.ShowDialog();
        }

        private void bt_userManagement_Click(object sender, EventArgs e)
        {
            //UserManagementForm f = new UserManagementForm();
            //f.ShowDialog();

        }

        private void bt_CutListReport_Click(object sender, EventArgs e)
        {
            //if(GlobalConfig.Setting.CurSettingInfo.ManufacturePhase==(int)ManufacturePhaseType.PRINT)
            //{ 
            //    CutSystemForm f = new CutSystemForm();
            //    f.ShowDialog();
            //}
            QASysForm f = new QASysForm();
            f.ShowDialog();

        }

        private void bt_OutBoundingSys_Click(object sender, EventArgs e)
        {
            OutBoundingSysForm f = new OutBoundingSysForm();
            f.ShowDialog();
        }

        private void bt_LiuYanSys_Click(object sender, EventArgs e)
        {
            LiuYanSysForm f = new LiuYanSysForm();
            f.ShowDialog();
        }

        private void bt_PrintSys_Click(object sender, EventArgs e)
        {
            PrintSysForm f = new PrintSysForm();
            f.ShowDialog();
        }

        private void bt_PackSys_Click(object sender, EventArgs e)
        {
            PackSysForm f = new PackSysForm();
            f.ShowDialog();
        }

        private void bt_RecoverySys_Click(object sender, EventArgs e)
        {
            RecoverySysForm f = new RecoverySysForm();
            f.ShowDialog();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
