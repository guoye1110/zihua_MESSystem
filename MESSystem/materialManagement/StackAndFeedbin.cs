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

namespace MESSystem.materialManagement
{
    public partial class StackAndFeedbin : Form
    {
        System.Windows.Forms.Timer aTimer;

        int currentPageIndex;
        int numOfPagesInList;
        int numOfMaterialInThisPage;

        public static StackAndFeedbin StackAndFeedbinClass = null; //it is used to reference this windows

        int machineSelected;
        int feedbinIDSelected;
        int materialCodeIDSelected;

        const int numOfRecordInOneScreen = 40;

        string[] machineList;
        string[] workerIDList;
        string[] feedbinIDList = { "未选择", "1号料仓", "2号料仓", "3号料仓", "4号料仓", "5号料仓", "6号料仓", "7号料仓" };
        string[,] materialInventoryList; 

        string[,] materialListArray;

        public StackAndFeedbin()
        {
            InitializeComponent();
            initializeVariables();
            resizeScreen();
        }

        private void initializeVariables()
        {
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            label1.Text = gVariable.enterpriseTitle + "码垛及投料查询";

            machineSelected = 0;
            //workerIDSelected = 0;
            materialCodeIDSelected = 0;

            materialInventoryList = getMaterialInventoryList();  //1 means add item of "all customer"
            machineList = toolClass.getMachineList(1); //1 means add item of "all machines"
            workerIDList = toolClass.getWorkerInfoList(1, 1);  //1 means add item of "all employees, 1 means worker ID
            //workerNameList = toolClass.getWorkerInfoList(1, 0);  //1 means add item of "all employees, 0 means worker name
        }

        private void materialManagement_Load(object sender, EventArgs e)
        {
            int i;

            for (i = 0; i < 7; i++)
            {
                comboBox1.Items.Add(machineList[i]);
            }
            comboBox1.SelectedIndex = machineSelected;

            for (i = 0; i < feedbinIDList.Length; i++)
            {
                comboBox2.Items.Add(feedbinIDList[i]);
        
            }
            comboBox2.SelectedIndex = feedbinIDSelected;

            comboBox3.Items.Add("未选择");
            for (i = 0; i < materialInventoryList.GetLength(0); i++)
            {
                comboBox3.Items.Add(materialInventoryList[i, 1]);
            }
            comboBox3.SelectedIndex = materialCodeIDSelected;

            displayMaterialList();

            aTimer = new System.Windows.Forms.Timer();

            //refresh screen every 100 ms
            aTimer.Interval = 20000;
            aTimer.Enabled = true;

            aTimer.Tick += new EventHandler(timer_listview);
        }

        void resizeScreen()
        {
            int i;
            int x, y, w, h;
            float fontSize;
            float screenRatioX;
            float screenRatioY;
            GroupBox[] groupBoxArray = { groupBox1, groupBox2, groupBox3, groupBox4 };
            Label[] labelArray = { label2, label3, label4, label5, label6, label7, label8, label9 };
            TextBox[] textBoxArray = { textBox1 };
            Button[] buttonArray = { button1, button42, button44, button46, button47 };
            ComboBox[] comboBoxArray = { comboBox1, comboBox2, comboBox3 };
            CheckBox[] checkBoxArray = { checkBox1 };
            DateTimePicker[] timePickerArray = { dateTimePicker1, dateTimePicker2 };
            float[,] commonFontSize = { 
                                        { 7F,  8F,  9F,  10F, 11F,  12F}, 
                                        { 6F,  7F,  8F, 8.5F, 9F,  10F},  
                                        { 5.5F, 6F, 6.5F, 7F, 7.5F, 8F},  
                                     };
            //float[] commonFontSize = { 6F, 6.5F, 7F, 7.5F, 8F, 8.5F };
            float[] titleFontSize = { 20F, 22F, 23F, 24F, 25F, 28F };
            Rectangle rect = new Rectangle();

            screenRatioX = gVariable.screenRatioX;
            screenRatioY = gVariable.screenRatioY;

            fontSize = commonFontSize[gVariable.dpiValue, gVariable.resolutionLevel];

            rect = Screen.GetWorkingArea(this);
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", titleFontSize[gVariable.resolutionLevel], System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            x = (rect.Width - label1.Size.Width) / 2;
            y = (int)(label1.Location.Y * screenRatioY);
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

            for (i = 0; i < timePickerArray.Length; i++)
            {
                timePickerArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(timePickerArray[i].Size.Width * screenRatioX);
                h = (int)(timePickerArray[i].Size.Height * screenRatioY);
                timePickerArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(timePickerArray[i].Location.X * screenRatioX);
                y = (int)(timePickerArray[i].Location.Y * screenRatioY);
                timePickerArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < labelArray.Length; i++)
            {
                labelArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
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

            for (i = 0; i < checkBoxArray.Length; i++)
            {
                w = (int)(checkBoxArray[i].Size.Width * screenRatioX);
                h = (int)(checkBoxArray[i].Size.Height * screenRatioY);
                checkBoxArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(checkBoxArray[i].Location.X * screenRatioX);
                y = (int)(checkBoxArray[i].Location.Y * screenRatioY);
                checkBoxArray[i].Location = new System.Drawing.Point(x, y);
            }

            for (i = 0; i < comboBoxArray.Length; i++)
            {
                w = (int)(comboBoxArray[i].Size.Width * screenRatioX);
                h = (int)(comboBoxArray[i].Size.Height * screenRatioY);
                comboBoxArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(comboBoxArray[i].Location.X * screenRatioX);
                y = (int)(comboBoxArray[i].Location.Y * screenRatioY);
                comboBoxArray[i].Location = new System.Drawing.Point(x, y);
            }
        }

        private void displayMaterialList()
        {
            int i;
            int totalMaterialNum;
            //int startIndex, endIndex;
            //int nameIndex;
            string name;
            int[] materialLenArray = { 1, 45, 120, 100, 60, 60, 130, 90, 80, 100, 110, 80};
            string[] materialColumnArray = 
            {
                "", "序号", "物料编码", "目标设备", "料仓编号", "码垛余料", "投料时间", "投料人", "投料数量", "生产批次号", "工单编码", "料仓余量",
            };

            try
            {
                getMaterialListByCondition();

                totalMaterialNum = materialListArray.GetLength(0);
                numOfPagesInList = totalMaterialNum / numOfRecordInOneScreen + 1;

                if (currentPageIndex >= numOfPagesInList)
                    currentPageIndex = 0;

                if (currentPageIndex < numOfPagesInList - 1)
                    numOfMaterialInThisPage = numOfRecordInOneScreen;
                else
                    numOfMaterialInThisPage = totalMaterialNum % numOfRecordInOneScreen;

                label8.Text = "页 (共" + numOfPagesInList.ToString() + "页)";
                label9.Text = (currentPageIndex + 1).ToString();

                this.listView1.Clear();
                this.listView1.BeginUpdate();
                listView1.GridLines = true;
                listView1.Dock = DockStyle.Fill;

                if (materialListArray != null)
                {
                    for (i = 0; i < materialLenArray.Length; i++)
                    {
                        if (i == 0)
                            listView1.Columns.Add(materialColumnArray[i], (int)(materialLenArray[i]), HorizontalAlignment.Center);
                        else
                            listView1.Columns.Add(materialColumnArray[i], (int)(materialLenArray[i] * gVariable.screenRatioX), HorizontalAlignment.Center);
                    }

                    for (i = 0; i < materialListArray.GetLength(0); i++)
                    {
                        ListViewItem OptionItem = new ListViewItem();

                        OptionItem.SubItems.Add(materialListArray[i, 0]);
                        OptionItem.SubItems.Add(materialListArray[i, 1]);
                        OptionItem.SubItems.Add(gVariable.machineNameArrayDatabase[Convert.ToInt32(materialListArray[i, 2])]);
                        OptionItem.SubItems.Add(materialListArray[i, 3] + "号料仓");
                        OptionItem.SubItems.Add(materialListArray[i, 4] + "kg");
                        OptionItem.SubItems.Add(materialListArray[i, 5]);
                        if (toolClass.isDigitalNum(materialListArray[i, 6]) == 1)
                        {
                            name = toolClass.getNameByIDAndIDByName(null, materialListArray[i, 6]);
                        }
                        else
                        {
                            name = "未知";
                        }
                        OptionItem.SubItems.Add(name);
                        OptionItem.SubItems.Add(materialListArray[i, 7] + "kg");
                        OptionItem.SubItems.Add(materialListArray[i, 8]);
                        OptionItem.SubItems.Add(materialListArray[i, 9]);
                        OptionItem.SubItems.Add(materialListArray[i, 10] + "kg");

                        listView1.Items.Add(OptionItem);
                    }
                }
                this.listView1.EndUpdate();
            }
            catch (Exception ex)
            {
                Console.WriteLine("displayMaterialList in stack exception occurred!" + ex);
            }
        }

        string [,] getMaterialInventoryList()
        {
            string commandText;
            string tableName;
            string databaseName;

            databaseName = gVariable.basicInfoDatabaseName;
            tableName = gVariable.materialTableName;

            commandText = "select * from `" + tableName + "`";
            
            return mySQLClass.databaseCommonReading(databaseName, commandText);
        }

        void getMaterialListByCondition()
        {
            //int num;
            int sthUpdated;
            string commandText;
            string tableName;
            string databaseName;
            string str1, str2, str3;

            databaseName = gVariable.globalDatabaseName;
            tableName = gVariable.materialFeedingTableName;

            sthUpdated = 0;
            str1 = null;
            if (machineSelected != 0)
            {
                str1 = " machineID = '" + machineList[machineSelected] + "'";

                sthUpdated = 1;
            }

            str2 = null;
            if (feedbinIDSelected != 0)
            {
                if (sthUpdated == 0)
                    str2 = " responsor = '" + feedbinIDList[feedbinIDSelected] + "'";
                else
                    str2 = " and customer = '" + feedbinIDList[feedbinIDSelected] + "'";

                sthUpdated = 1;
            }

            str3 = null;
            if (materialCodeIDSelected != 0)
            {
                if (sthUpdated == 0)
                    str3 = " materialCode = '" + materialInventoryList[materialCodeIDSelected - 1, 1] + "'";
                else
                    str3 = " and materialCode = '" + materialInventoryList[materialCodeIDSelected - 1, 1] + "'";

                sthUpdated = 1;
            }

            if (sthUpdated == 0)
                commandText = "select * from `" + tableName + "`";
            else
                commandText = "select * from `" + tableName + "`" + " where" + str1 + str2 + str3;

            materialListArray = mySQLClass.databaseCommonReading(databaseName, commandText);
        }

        private void materialManagement_FormClosing(object sender, EventArgs e)
        {
            if(aTimer != null)
                aTimer.Enabled = false;

            firstScreen.firstScreenClass.Show();
        }


        private void timer_listview(Object source, EventArgs e)
        {
            displayMaterialList(); 
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            machineSelected = comboBox1.SelectedIndex;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            feedbinIDSelected = comboBox2.SelectedIndex;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            materialCodeIDSelected = comboBox3.SelectedIndex;
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
    }
}
