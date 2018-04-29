using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.common;

namespace MESSystem.alarmFun
{
    public partial class historyDetailReview : Form
    {
        string lineFeed = "\r\n";

        string databaseName;
        string tableName;
        int id;

        gVariable.alarmTableStruct alarmTableStructImpl;

        public static historyDetailReview historyDetailReviewClass = null;

        public historyDetailReview(string databaseName_, int id_)
        {
            databaseName = databaseName_;

            if (databaseName == gVariable.globalDatabaseName)   //for all alarm list
                tableName = gVariable.globalAlarmListTableName;
            else   //for alarm of certain machine 
                tableName = gVariable.alarmListTableName;
            id = id_;
            InitializeComponent();
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            this.TopMost = true;
        }

        private void detailsForm_Load(object sender, EventArgs e)
        {
            string statusStr;
            string typeStr;

            try
            {
                alarmTableStructImpl = mySQLClass.getAlarmTableContent(databaseName, tableName, id);

                textBox1.Text = "【报警单号】：" + alarmTableStructImpl.alarmFailureCode + lineFeed;
                textBox1.Text += "【报警工单】：" + alarmTableStructImpl.dispatchCode + lineFeed;
                textBox1.Text += "【报警设备】：" + alarmTableStructImpl.machineCode + "(" + alarmTableStructImpl.machineName + ")" + lineFeed;

                if (alarmTableStructImpl.status < gVariable.strAlarmStatus.Length)
                    statusStr = gVariable.strAlarmStatus[alarmTableStructImpl.status];
                else
                    statusStr = gVariable.strAlarmStatus[0];

                if (alarmTableStructImpl.type == gVariable.ALARM_TYPE_DEVICE)
                {
                    typeStr = gVariable.strAlarmTypeInDetail[gVariable.ALARM_TYPE_DEVICE];
                    textBox1.Text += "【报警原因】：" + alarmTableStructImpl.errorDesc + lineFeed;
                }
                else if (alarmTableStructImpl.type == gVariable.ALARM_TYPE_MATERIAL)
                {
                    typeStr = gVariable.strAlarmTypeInDetail[gVariable.ALARM_TYPE_MATERIAL];
                    textBox1.Text += "【物料批次】：" + alarmTableStructImpl.errorDesc + lineFeed;
                }
                else if (alarmTableStructImpl.type == gVariable.ALARM_TYPE_QUALITY_DATA)
                {
                    typeStr = gVariable.strAlarmTypeInDetail[gVariable.ALARM_TYPE_QUALITY_DATA];
                    textBox1.Text += "【报警原因】：" + alarmTableStructImpl.errorDesc + lineFeed;
                }
                else if (alarmTableStructImpl.type == gVariable.ALARM_TYPE_CRAFT_DATA)
                {
                    typeStr = gVariable.strAlarmTypeInDetail[gVariable.ALARM_TYPE_CRAFT_DATA];
                    textBox1.Text += "【报警原因】：" + alarmTableStructImpl.errorDesc + lineFeed;
                }
                else if (alarmTableStructImpl.type == gVariable.ALARM_TYPE_CURRENT_VALUE)
                {
                    typeStr = gVariable.strAlarmTypeInDetail[gVariable.ALARM_TYPE_CURRENT_VALUE];
                    textBox1.Text += "【报警原因】：" + alarmTableStructImpl.errorDesc + lineFeed;
                }
                else
                {
                    typeStr = "报警类型未知";
                }
                textBox1.Text += "【报警员工】：" + alarmTableStructImpl.operatorName + lineFeed;
                textBox1.Text += "【报警时间】：" + alarmTableStructImpl.time + lineFeed;
                textBox1.Text += "【报警类型】：" + typeStr + lineFeed;
                textBox1.Text += "【报警状态】：" + statusStr;

                textBox5.Text = alarmTableStructImpl.operatorName + " 于 " + alarmTableStructImpl.time + " " + "开启安灯报警" + lineFeed;

                if (alarmTableStructImpl.signer.Length > 2)
                    textBox5.Text += alarmTableStructImpl.signer + " 于 " + alarmTableStructImpl.time1 + "报警签到" + lineFeed;

                if (alarmTableStructImpl.status == gVariable.ALARM_STATUS_COMPLETED)
                {
                    textBox5.Text += alarmTableStructImpl.completer + " 于 " + alarmTableStructImpl.time2 + " 确认报警处理完成" + lineFeed;
                }
                else if (alarmTableStructImpl.status == gVariable.ALARM_STATUS_CANCELLED)
                {
                    textBox5.Text += alarmTableStructImpl.completer + " 于 " + alarmTableStructImpl.time2 + " 确认报警取消" + lineFeed;
                }

                textBox2.Text = alarmTableStructImpl.discuss;
                textBox4.Text = alarmTableStructImpl.solution;
            }
            catch (Exception ex)
            {
                Console.WriteLine("deviceAlarm_Load exception occurred!" + ex);
            }
        }

        private void detailsForm_FormClosing(object sender, EventArgs e)
        {
        }

        private void button6_Click(object sender, EventArgs e)
        {
            historyDetailReviewClass = null;
            this.Close();
        }
    }
}
