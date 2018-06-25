using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.common;
using MESSystem.quality;

namespace MESSystem.alarmFun
{
    public partial class historyListReview : Form
    {
        const int ALARM_NUM_IN_ONE_SCREEN = 20;

        public static historyListReview historyListReviewClass = null;

        int type;
        int alarmCategory;
        int indexInTable;
        int checkResult;
        int num;

        string dName;
        string tName;

        public int[] alarmIDArray = new int[gVariable.MAX_ALARM_NUM_ONE_CATEGORY_IN_HISTORY];
        public string[] errorDescArray = new string[gVariable.MAX_ALARM_NUM_ONE_CATEGORY_IN_HISTORY];
        public string[] alarmFailureCodeArray = new string[gVariable.MAX_ALARM_NUM_ONE_CATEGORY_IN_HISTORY];
        public string[] discussArray = new string[gVariable.MAX_ALARM_NUM_ONE_CATEGORY_IN_HISTORY];
        public string[] solutionArray = new string[gVariable.MAX_ALARM_NUM_ONE_CATEGORY_IN_HISTORY];

        public delegate void myDelegateInformParent();
        public event myDelegateInformParent informParentEvent;

        public historyListReview(string databaseName_, int type_, int alarmCategory_, int indexInTable_)
        {
            InitializeComponent();
            initVariables(databaseName_, type_, alarmCategory_, indexInTable_);
        }

        private void initVariables(string databaseName_, int type_, int alarmCategory_, int indexInTable_)
        {
            checkResult = 0;
            dName = databaseName_;
            tName = gVariable.alarmListTableName;
            type = type_;
            alarmCategory = alarmCategory_;
            indexInTable = indexInTable_;

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            //get alarm info for alarms that in history table including alarm ID/failureNo/desc/discussion/solution
            num = mySQLClass.getAlarmContentInHistoryTable(dName, tName, indexInTable, alarmCategory, ref alarmIDArray, ref alarmFailureCodeArray, ref errorDescArray, ref discussArray, ref solutionArray);
            if (num <= 0)
            {
                checkResult = -1;
                MessageBox.Show("该类型报警暂无历史经验记录，请直接输入分析及处理措施，谢谢！", "信息提示", MessageBoxButtons.OK);
                return;
            }
        }

        public int getCheckResult()
        {
            return checkResult;
        }

        private void deviceAlarm_Load(object sender, EventArgs e)
        {
            displayAlarmListView();
        }


        public void displayAlarmListView()
        {
            int i;
            int three_char_len = 55;
            int ten_char_len = 113;

            listView1.GridLines = true;
            listView1.Dock = DockStyle.Fill;

            i = 0;
            try
            {
                this.Text = gVariable.programTitle + gVariable.alarmListHistoryTitle[type];
                this.label1.Text = gVariable.alarmListTitle[type];

                this.listView1.BeginUpdate();

                listView1.Columns.Add(" ", 0, HorizontalAlignment.Center);
                listView1.Columns.Add(gVariable.deviceAlarmListTitle0, three_char_len, HorizontalAlignment.Center);  //id
                listView1.Columns.Add(gVariable.deviceAlarmListTitle1, ten_char_len, HorizontalAlignment.Center); //device failureNo
                listView1.Columns.Add(gVariable.deviceAlarmListTitle5, ten_char_len, HorizontalAlignment.Center); //failure description
                listView1.Columns.Add(gVariable.deviceAlarmListTitle12, 470, HorizontalAlignment.Center); //discuss
                listView1.Columns.Add(gVariable.deviceAlarmListTitle13, 450, HorizontalAlignment.Center); //solution

                for (i = 0; i < num; i++)
                {
                    ListViewItem OptionItem = new ListViewItem();

                    OptionItem.SubItems.Add((i + 1).ToString());
                    OptionItem.SubItems.Add(alarmFailureCodeArray[i]);
                    OptionItem.SubItems.Add(errorDescArray[i]);
                    OptionItem.SubItems.Add(discussArray[i]);
                    OptionItem.SubItems.Add(solutionArray[i]);

                    listView1.Items.Add(OptionItem);
                }

                this.listView1.EndUpdate();
            }
            catch (Exception ex)
            {
                Console.WriteLine("deviceAlarm_Load exception occurred!" + ex);
            }
        }

        private void deviceAlarm_FormClosing(object sender, EventArgs e)
        {
        }

        //history alarm selected, we record discuss and solution strings
        private void button1_Click(object sender, EventArgs e)
        {
            int index;

            try
            {
                if (this.listView1.SelectedItems.Count > 0)
                {
                    index = this.listView1.SelectedItems[0].Index;

                    gVariable.alarmHistoryDiscuss = discussArray[index];
                    gVariable.alarmHistorySolution = solutionArray[index];

                    if (informParentEvent != null)
                        informParentEvent();

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Write("listView1_SelectedIndexChanged failed. ");
                Console.WriteLine(ex.ToString());
                return;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            gotoAlarmDetails();
        }

        public void gotoAlarmDetails()
        {
            int index;

            try
            {
                if (this.listView1.SelectedItems.Count > 0)
                {
                    index = this.listView1.SelectedItems[0].Index;

                    historyDetailReview.historyDetailReviewClass = new historyDetailReview(dName, alarmIDArray[index]);
                    historyDetailReview.historyDetailReviewClass.Show();
                }
            }
            catch (Exception ex)
            {
                Console.Write("show detailed alarm info failed. " + ex);
            }
        }
        //exit 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //display original data and curve
        private void button4_Click(object sender, EventArgs e)
        {
            int index;

            try
            {
                if (this.listView1.SelectedItems.Count > 0)
                {
                    index = this.listView1.SelectedItems[0].Index;

                    //four paramters:
                    //1: database name
                    //2: where comes this requirement, from alarm or quality
                    //3: alarmIDArray[index] means alarm index in alarm table
                    //4: this is a quaity data or craft data triggered alarm 
                    SPCAnalyze SPCAnalyzeClass = new SPCAnalyze(dName, gVariable.FROM_ALARM_DISPLAY_FUNC, alarmIDArray[index], type);
                    SPCAnalyzeClass.Show();
                }
            }
            catch (Exception ex)
            {
                Console.Write("show detailed alarm info failed. " + ex);
            }
        }

        //display alarm dealing process
        private void button3_Click(object sender, EventArgs e)
        {
            gotoAlarmDetails();
        }
    }
}
