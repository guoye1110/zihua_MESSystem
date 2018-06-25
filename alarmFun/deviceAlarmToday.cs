using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.common;
using MESSystem.curves;
using MESSystem.mainUI;

namespace MESSystem.alarmFun
{
    public partial class alarmListView : Form
    {
        const int ALARM_NUM_IN_ONE_SCREEN = 32;

        //a flag to indicate where we want to exit
        //0 means exit to higher layer -> workshop function; 
        //1 means we want to move to other screen of the same layer
        int closeReason;

        System.Windows.Forms.Timer aTimer;

        int currentPageIndex;
        int numOfPagesInList;
        int numOfAlarmInThisPage;

        int[] alarmNumInOnePage = {18, 20, 20, 22, 25, 31};

        private int totalAlarmNum;
        public static alarmListView alarmListViewClass = null;
        int machineSelected;
        int alarmTypeSelected;
        int alarmStatusSelected;

        string dName;
        string tName;

        string[] alarmFailureCodeArray = new string[ALARM_NUM_IN_ONE_SCREEN];
        string[] alarmMachineNameArray = new string[ALARM_NUM_IN_ONE_SCREEN];

        gVariable.alarmTableStruct alarmTableStructImpl;

        int[] alarmIDArray = new int[ALARM_NUM_IN_ONE_SCREEN];  //suppose there won't be more than 5000 alarms in one day, otherwise, I will die
        int[] alarmStatusArray = new int[ALARM_NUM_IN_ONE_SCREEN];  //suppose there won't be more than 5000 alarms in one day, otherwise, I will die

        public alarmListView(string databaseName_, int machineName_, int alarmType_, int alarmStatus_, int closeReason_)
        {
            InitializeComponent();

            resizeForScreen();

            initVariables(databaseName_, machineName_, alarmType_, alarmStatus_, closeReason_);
        }

        private void initVariables(string databaseName_, int machineName_, int alarmType_, int alarmStatus_, int closeReason_)
        {
            int i;

            dName = databaseName_;

            if (dName == gVariable.globalDatabaseName)  //this is a all alarm list
            {
                tName = gVariable.globalAlarmListTableName;
            }
            else  //this is an alarm list for one board/machine
            {
                tName = gVariable.alarmListTableName;
            }

            machineSelected = machineName_;
            alarmTypeSelected = alarmType_;
            alarmStatusSelected = alarmStatus_;
            closeReason = closeReason_;

            for (i = 0; i < ALARM_NUM_IN_ONE_SCREEN; i++)
                alarmIDArray[i] = 0;

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            dateTimePicker1.Value = new DateTime(2017, 1, 1);
            dateTimePicker2.Value = DateTime.Now;

            totalAlarmNum = getRecordNumbyConditions(dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss"), dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            currentPageIndex = 0;
        }

        void resizeForScreen()
        {
            int i;
            int x, y, w, h;
            float screenRatioX, screenRatioY;
            float fontSize;
            GroupBox[] groupBoxArray = { groupBox1, groupBox2, groupBox3 };
            Label[] labelArray = { label2, label4, label3, label6, label5, label7, label8, label9,  };
            TextBox[] textBoxArray = { textBox1 };
            Panel[] panelArray = { panel1 };
            Button[] buttonArray = { button1, button42, button44, button46, button47 };
            ComboBox[] comboBoxArray = { comboBox1, comboBox2, comboBox3 };
            //CheckBox[] checkBoxArray = { checkBox1, checkBox2, checkBox3 };
            DateTimePicker[] timePickerArray = { dateTimePicker1, dateTimePicker2 };
            ListView[] listViewArray = { listView1 }; 
            float[,] commonFontSize = { 
                                        { 7F,  8F,  9F,  10F, 11F,  12F}, 
                                        { 6F,  6.5F,  8F, 8.5F, 9F,  10F},  
                                        { 5.5F, 6F, 6.5F, 7F, 7.5F, 8F},  
                                     };

            //float[] commonFontSize = { 6F, 6.5F, 7F, 7.5F, 8F, 8.5F };
            float[] titleFontSize = { 20F, 22F, 23F, 24F, 25F, 28F };
            Rectangle rect = new Rectangle();

            screenRatioX = gVariable.screenRatioX;
            screenRatioY = gVariable.screenRatioY;

            fontSize = commonFontSize[gVariable.dpiValue, gVariable.resolutionLevel];

            rect = Screen.GetWorkingArea(this);
            x = (rect.Width - label1.Size.Width) / 2;
            y = (int)(label1.Location.Y * screenRatioY);
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", titleFontSize[gVariable.resolutionLevel], System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label1.Location = new System.Drawing.Point(x, y);

            for (i = 0; i < groupBoxArray.Length; i++)
            {
                groupBoxArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(groupBoxArray[i].Size.Width * screenRatioX);
                h = (int)(groupBoxArray[i].Size.Height * screenRatioY);
                groupBoxArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(groupBoxArray[i].Location.X * screenRatioX);
                y = (int)(groupBoxArray[i].Location.Y * screenRatioY);
                groupBoxArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < labelArray.Length; i++)
            {
                w = (int)(labelArray[i].Size.Width * screenRatioX);
                h = (int)(labelArray[i].Size.Height * screenRatioY);
                labelArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(labelArray[i].Location.X * screenRatioX);
                y = (int)(labelArray[i].Location.Y * screenRatioY);
                labelArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < textBoxArray.Length; i++)
            {
                w = (int)(textBoxArray[i].Size.Width * screenRatioX);
                h = (int)(textBoxArray[i].Size.Height * screenRatioY);
                textBoxArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(textBoxArray[i].Location.X * screenRatioX);
                y = (int)(textBoxArray[i].Location.Y * screenRatioY);
                textBoxArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < buttonArray.Length; i++)
            {
                buttonArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(buttonArray[i].Size.Width * screenRatioX);
                h = (int)(buttonArray[i].Size.Height * screenRatioY);
                buttonArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(buttonArray[i].Location.X * screenRatioX);
                y = (int)(buttonArray[i].Location.Y * screenRatioY);
                buttonArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < listViewArray.Length; i++)
            {
                w = (int)(listViewArray[i].Size.Width * screenRatioX);
                h = (int)(listViewArray[i].Size.Height * screenRatioY);
                listViewArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(listViewArray[i].Location.X * screenRatioX);
                y = (int)(listViewArray[i].Location.Y * screenRatioY);
                listViewArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < panelArray.Length; i++)
            {
                w = (int)(panelArray[i].Size.Width * screenRatioX);
                h = (int)(panelArray[i].Size.Height * screenRatioY);
                panelArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(panelArray[i].Location.X * screenRatioX);
                y = (int)(panelArray[i].Location.Y * screenRatioY);
                panelArray[i].Location = new System.Drawing.Point(x, y);
            }

            /*for (i = 0; i < checkBoxArray.Length; i++)
            {
                w = (int)(checkBoxArray[i].Size.Width * screenRatioX);
                h = (int)(checkBoxArray[i].Size.Height * screenRatioY);
                checkBoxArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(checkBoxArray[i].Location.X * screenRatioX);
                y = (int)(checkBoxArray[i].Location.Y * screenRatioY);
                checkBoxArray[i].Location = new System.Drawing.Point(x, y);
            }*/

            for (i = 0; i < comboBoxArray.Length; i++)
            {
                w = (int)(comboBoxArray[i].Size.Width * screenRatioX);
                h = (int)(comboBoxArray[i].Size.Height * screenRatioY);
                comboBoxArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(comboBoxArray[i].Location.X * screenRatioX);
                y = (int)(comboBoxArray[i].Location.Y * screenRatioY);
                comboBoxArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < timePickerArray.Length; i++)
            {
                w = (int)(timePickerArray[i].Size.Width * screenRatioX);
                h = (int)(timePickerArray[i].Size.Height * screenRatioY);
                timePickerArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(timePickerArray[i].Location.X * screenRatioX);
                y = (int)(timePickerArray[i].Location.Y * screenRatioY);
                timePickerArray[i].Location = new System.Drawing.Point(x, y);
            }

            listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }


        private void deviceAlarm_Load(object sender, EventArgs e)
        {
            int i;

            for (i = 0; i < gVariable.machineNameArrayTouchScreen.Length; i++)
            {
                comboBox1.Items.Add(gVariable.machineNameArrayTouchScreen[i]);
            }
            comboBox1.Items.Add("所有设备");
            comboBox1.SelectedIndex = machineSelected;

            for (i = 0; i < gVariable.strAlarmTypeForSelection.Length; i++)
            {
                comboBox2.Items.Add(gVariable.strAlarmTypeForSelection[i]);
            }
            comboBox2.SelectedIndex = alarmTypeSelected;

            for (i = 0; i < gVariable.strAlarmStatusForSelection.Length; i++)
            {
                comboBox3.Items.Add(gVariable.strAlarmStatusForSelection[i]);
            }
            comboBox3.SelectedIndex = alarmStatusSelected;

            displayAlarmListView();

            aTimer = new System.Windows.Forms.Timer();

            //refresh screen every 100 ms
            aTimer.Interval = 300;
            aTimer.Enabled = true;

            aTimer.Tick += new EventHandler(timer_listview);
        }


        private void displayAlarmListView()
        {
            int i, j;
            //int ret;
            //int two_char_len = 48;
            //int three_char_len = 60;
            //int five_char_len = 85;
            //int eight_char_len = 103;
            //int ten_char_len = 113;
            //int twelve_char_len = 192;
            int startIndex, endIndex;
            string statusStr;
            List<int> idList;
            int[] dispatchLenArray = { 48, 113, 85, 79, 97, 192, 80, 125, 60, 60, 125, 60, 125 };

            try
            {
                totalAlarmNum = getRecordNumbyConditions(dateTimePicker1.Value.ToString("yyyy-MM-dd 00:00:00"), dateTimePicker2.Value.ToString("yyyy-MM-dd 23:59:59"));
                numOfPagesInList = totalAlarmNum / alarmNumInOnePage[gVariable.resolutionLevel] + 1;

                if (currentPageIndex >= numOfPagesInList)
                    currentPageIndex = 0;

                if (currentPageIndex < numOfPagesInList - 1)
                    numOfAlarmInThisPage = alarmNumInOnePage[gVariable.resolutionLevel];
                else
                    numOfAlarmInThisPage = totalAlarmNum % alarmNumInOnePage[gVariable.resolutionLevel];

                label8.Text = "页 (共" + numOfPagesInList.ToString() + "页)";
                label9.Text = (currentPageIndex + 1).ToString();

                this.listView1.Clear();
                this.listView1.BeginUpdate();
                listView1.GridLines = true;
                listView1.Dock = DockStyle.Fill;

                //used to increase the height of one line of the listview
                this.listView1.SmallImageList = imageList1;
            
                i = 0;
                j = 0;

                this.Text = gVariable.programTitle + "安灯报警列表";
                label1.Text = gVariable.enterpriseTitle + "安灯报警列表";

                listView1.Columns.Add(" ", 0, HorizontalAlignment.Center);
                listView1.Columns.Add(gVariable.deviceAlarmListTitle0, (int)(dispatchLenArray[i++] * gVariable.screenRatioX), HorizontalAlignment.Center);  //id
                listView1.Columns.Add(gVariable.deviceAlarmListTitle1, (int)(dispatchLenArray[i++] * gVariable.screenRatioX), HorizontalAlignment.Center); //device failureNo
                listView1.Columns.Add(gVariable.deviceAlarmListTitle4, (int)(dispatchLenArray[i++] * gVariable.screenRatioX), HorizontalAlignment.Center); //alarm type
                listView1.Columns.Add(gVariable.deviceAlarmListTitle2, (int)(dispatchLenArray[i++] * gVariable.screenRatioX), HorizontalAlignment.Center); //device name
                listView1.Columns.Add(gVariable.deviceAlarmListTitle3, (int)(dispatchLenArray[i++] * gVariable.screenRatioX), HorizontalAlignment.Center); //bin ID
                listView1.Columns.Add(gVariable.deviceAlarmListTitle5, (int)(dispatchLenArray[i++] * gVariable.screenRatioX), HorizontalAlignment.Center);  //error desc
                listView1.Columns.Add(gVariable.deviceAlarmListTitle6, (int)(dispatchLenArray[i++] * gVariable.screenRatioX), HorizontalAlignment.Center);  //operator
                listView1.Columns.Add(gVariable.deviceAlarmListTitle7, (int)(dispatchLenArray[i++] * gVariable.screenRatioX), HorizontalAlignment.Center);  //time
                listView1.Columns.Add(gVariable.deviceAlarmListTitle8, (int)(dispatchLenArray[i++] * gVariable.screenRatioX), HorizontalAlignment.Center);  //status
                listView1.Columns.Add(gVariable.deviceAlarmListTitle9, (int)(dispatchLenArray[i++] * gVariable.screenRatioX), HorizontalAlignment.Center);  //signer
                listView1.Columns.Add(gVariable.deviceAlarmListTitle11, (int)(dispatchLenArray[i++] * gVariable.screenRatioX), HorizontalAlignment.Center);  //sign time
                listView1.Columns.Add(gVariable.deviceAlarmListTitle10, (int)(dispatchLenArray[i++] * gVariable.screenRatioX), HorizontalAlignment.Center); //completer
                listView1.Columns.Add(gVariable.deviceAlarmListTitle14, (int)(dispatchLenArray[i++] * gVariable.screenRatioX), HorizontalAlignment.Center);  //complete time

                startIndex = currentPageIndex * alarmNumInOnePage[gVariable.resolutionLevel];
                endIndex = startIndex + alarmNumInOnePage[gVariable.resolutionLevel];

                idList = getRecordIndexArraybyConditions(startIndex, endIndex, dateTimePicker1.Value.ToString("yyyy-MM-dd 00:00:00"), dateTimePicker2.Value.ToString("yyyy-MM-dd 23:59:59"));
                for (i = 0; i < idList.Count; i++)
                {
                    ListViewItem OptionItem = new ListViewItem();

                    alarmTableStructImpl = mySQLClass.getAlarmTableContent(dName, tName, idList[i]); //, ref errorDesc, ref dispatchCode, ref alarmFailureCode, ref machineCode, ref machineName,
                    if (alarmTableStructImpl.dispatchCode == null)
                        break;

                    if (alarmTableStructImpl.status < gVariable.strAlarmStatus.Length)
                        statusStr = gVariable.strAlarmStatus[alarmTableStructImpl.status];
                    else
                        statusStr = gVariable.strAlarmStatus[0];

                    OptionItem.SubItems.Add((startIndex + i + 1).ToString());
                    OptionItem.SubItems.Add(alarmTableStructImpl.alarmFailureCode);

                    if (alarmTableStructImpl.type >= gVariable.ALARM_TYPE_TOTAL_NUM - 1)  //type is illegal
                        alarmTableStructImpl.type = gVariable.ALARM_TYPE_DEVICE;
                    OptionItem.SubItems.Add(gVariable.strAlarmTypeInDetail[alarmTableStructImpl.type]);

                    OptionItem.SubItems.Add(alarmTableStructImpl.machineName);
                    OptionItem.SubItems.Add(alarmTableStructImpl.feedBinID + "号料仓");
                    OptionItem.SubItems.Add(alarmTableStructImpl.errorDesc);
                    OptionItem.SubItems.Add(alarmTableStructImpl.operatorName);
                    OptionItem.SubItems.Add(alarmTableStructImpl.time);
                    OptionItem.SubItems.Add(statusStr);
                    OptionItem.SubItems.Add(alarmTableStructImpl.signer);
                    OptionItem.SubItems.Add(alarmTableStructImpl.time1);
                    OptionItem.SubItems.Add(alarmTableStructImpl.completer);
                    OptionItem.SubItems.Add(alarmTableStructImpl.time2);

                    listView1.Items.Add(OptionItem);

                    alarmIDArray[j] = idList[i];  //put alarm id in alarm table in array
                    alarmStatusArray[j] = alarmTableStructImpl.status;  //put alarm status in alarm table in array

                    alarmFailureCodeArray[j] = alarmTableStructImpl.alarmFailureCode;  //record alarm failure number in buffer, so we can get this alarm 
                    alarmMachineNameArray[j] = alarmTableStructImpl.machineName;
                    j++;

                    if (j >= alarmNumInOnePage[gVariable.resolutionLevel])
                        break;
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
            if(aTimer != null)
                aTimer.Enabled = false;

            if (closeReason == gVariable.MAIN_FUNCTION_ANDON)
            {
                firstScreen.firstScreenClass.Show();
            }
            else //if(gVariable.FUNCTION_DISPATCH_LIST_UI)
            {
                dispatchUI.dispatchUIClass.Show();
            }
        }


        private void timer_listview(Object source, EventArgs e)
        {
            displayAlarmListView(); 
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index;
            string str;
            string machineName;
            string databaseName;

            try
            {
                if (this.listView1.SelectedItems.Count > 0)
                {
                    index = this.listView1.SelectedItems[0].Index;

                    machineName = alarmMachineNameArray[index];
                    databaseName = toolClass.getDatabaseNameByMachineName(machineName);
                    if (databaseName == null)
                        return;

                    str = mySQLClass.getAnothercolumnFromDatabaseByOneColumn(databaseName, gVariable.alarmListTableName, "alarmFailureCode", alarmFailureCodeArray[index], 0);
                    if (toolClass.isDigitalNum(str) == 1)
                    {
                        toolClass.processNewAlarm(databaseName, Convert.ToInt16(str));
                    }
                    else
                    {
                        MessageBox.Show("抱歉，没有发现 " + databaseName + "-> 0_alarm -> " + alarmFailureCodeArray[index] + "！", "信息提示", MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("show detailed alarm info failed. " + ex);
            }
        }

        //we use this function before that we can review an alarm but cannot complete or cancel an alarm, we don't need this any more
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            machineSelected = comboBox1.SelectedIndex;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            alarmTypeSelected = comboBox2.SelectedIndex;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            alarmStatusSelected = comboBox3.SelectedIndex;
        }

        //first page
        private void button1_Click_1(object sender, EventArgs e)
        {
            currentPageIndex = 0;
        }

        //previous page
        private void button42_Click(object sender, EventArgs e)
        {
            if (currentPageIndex > 0)
                currentPageIndex--;
        }

        //next page
        private void button44_Click(object sender, EventArgs e)
        {
            if (currentPageIndex < numOfPagesInList - 1)
                currentPageIndex++;
        }


        //last page
        private void button46_Click(object sender, EventArgs e)
        {
            currentPageIndex = numOfPagesInList - 1;
        }

        //confirm page index input
        private void button47_Click(object sender, EventArgs e)
        {
            if (toolClass.isDigitalNum(textBox1.Text) == 1)
                currentPageIndex = Convert.ToInt16(textBox1.Text) - 1;
            else
            {
                MessageBox.Show("请输入纯数字，不要夹杂其他字符，谢谢！", "信息提示", MessageBoxButtons.OK);
            }
        }

        private int getRecordNumbyConditions(string time1, string time2)
        {
            string commandText;

            commandText = generateCommandText(time1, time2);

            return mySQLClass.queryRecordNumAction(gVariable.globalDatabaseName, commandText);
        }

        private List<int> getRecordIndexArraybyConditions(int id1, int id2, string time1, string time2)
        {
            string commandText;

            commandText = generateCommandText(time1, time2);

            return mySQLClass.queryRecordIDToArray(gVariable.globalDatabaseName, commandText, alarmNumInOnePage[gVariable.resolutionLevel], id1, id2);
        }


        private string generateCommandText(string time1, string time2)
        {
            int andFlag;
            string tmp;
            string condition1, condition2, condition3; 
            string commandText;

            andFlag = 0;
            condition1 = null;
            condition2 = null;
            condition3 = null;

            if (machineSelected < gVariable.MACHINE_NAME_ALL_FOR_SELECTION)
            {
                condition1 = "machineName = '" + gVariable.machineNameArrayTouchScreen[machineSelected] + "'";
                andFlag = 1;
            }

            if (alarmTypeSelected < gVariable.ALARM_TYPE_ALL_FOR_SELECTION)
            {
                if (andFlag == 1)
                    tmp = " and ";
                else
                    tmp = null;

                if (alarmTypeSelected == gVariable.ALARM_TYPE_DATA)  //data alarm includes quality data/craft data/
                {
                    condition2 = tmp + "type > 1 and type < 5";
                }
                else
                    condition2 = tmp + "type = " + alarmTypeSelected;


                andFlag = 1;
            }

            if (alarmStatusSelected < gVariable.ALARM_STATUS_ALL_FOR_SELECTION)
            {
                if (andFlag == 1)
                    condition3 = " and status = " + alarmStatusSelected;
                else
                    condition3 = "status = " + alarmStatusSelected;

                andFlag = 1;
            }

            if(andFlag == 1)
                commandText = "select * from `" + gVariable.globalAlarmListTableName + "` where " + condition1 + condition2 + condition3 + " and time >= '" + time1 + "' and time <= '" + time2 + "' order by id desc";
            else
                commandText = "select * from `" + gVariable.globalAlarmListTableName + "` where " + condition1 + condition2 + condition3 + "time >= '" + time1 + "' and time <= '" + time2 + "' order by id desc";

            return commandText;
        }
    }
}
