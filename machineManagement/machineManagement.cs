using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using MESSystem.common;
using MESSystem.mainUI;

namespace MESSystem.machine
{
    public partial class machineManagement : Form
    {
        public const int Task_Type_Routine = 0;  //routine check task
        public const int Task_Type_Maintenance = 1;  //maintenance task
        public const int Task_Type_Repairing = 2;   //repairing task

        int machineSelected = 0;
        int checkTypeSelected = 0;
        int taskType = 0;
        int taskSelected = 0;
        string machineTableName;

        System.Windows.Forms.Timer aTimer;

        int databaseType; //type value defined in database, 0 is dispatch list, 1 is material list... 16 is daily check, 17 is add oil, 18 is washup, 19 is maintenance 

        public static machineManagement machineManagementClass = null; //it is used to reference this windows
        public machineManagement(int machineIndex, int machineCheckType)
        {
            gVariable.machineListFresh = 0;
            machineSelected = machineIndex;
            checkTypeSelected = machineCheckType;

            InitializeComponent();
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
        }

        private void machineManagement_Load(object sender, EventArgs e)
        {
            int i;

            for (i = 0; i < gVariable.machineNameArrayAPS.Length; i++)
            {
                comboBox1.Items.Add(gVariable.machineNameArrayAPS[i]);
            }
            comboBox1.SelectedIndex = machineSelected;

            switch (checkTypeSelected)
            {
                case gVariable.MACHINE_MANAGEMENT_STATUS:
                    label1.Text = gVariable.enterpriseTitle + "设备管理系统 - 设备状态";
                    break;
                case gVariable.MACHINE_MANAGEMENT_LEDGER:
                    label1.Text = gVariable.enterpriseTitle + "设备管理系统 - 台帐管理";
                    break;
                case gVariable.MACHINE_MANAGEMENT_ITEM_CHECKING:
                    label1.Text = gVariable.enterpriseTitle + "设备管理系统 - 点巡检管理";

                    for (i = 0; i < gVariable.taskListCheckDispatch.Length; i++)
                    {
                        comboBox2.Items.Add(gVariable.taskListCheckDispatch[i]);
                    }

                    button2.Text = "启动点巡检工单";
                    taskType = Task_Type_Routine;
                    comboBox2.SelectedIndex = taskSelected;
                    break;
                case gVariable.MACHINE_MANAGEMENT_MAINTENANCE:
                    label1.Text = gVariable.enterpriseTitle + "设备管理系统 - 保养管理";
                    for (i = 0; i < gVariable.taskListMaintainDispatch.Length; i++)
                    {
                        comboBox2.Items.Add(gVariable.taskListMaintainDispatch[i]);
                    }

                    button2.Text = "启动保养工单";
                    taskType = Task_Type_Maintenance;
                    comboBox2.SelectedIndex = taskSelected;
                    break;
                case gVariable.MACHINE_MANAGEMENT_REPAIRING:
                    label1.Text = gVariable.enterpriseTitle + "设备管理系统 - 维修管理";
                    for (i = 0; i < gVariable.taskListRepairDispatch.Length; i++)
                    {
                        comboBox2.Items.Add(gVariable.taskListRepairDispatch[i]);
                    }

                    button2.Text = "启动维修工单";
                    taskType = Task_Type_Repairing;
                    comboBox2.SelectedIndex = taskSelected;
                    break;
                case gVariable.EFFICIENCY_MACHINE_OEE:
                    label1.Text = gVariable.enterpriseTitle + "设备管理系统 - OEE管理";
                    break;
            }

            databaseType = getDatabaseType(taskType, taskSelected);

            displayMachineManagementListview();

            aTimer = new System.Windows.Forms.Timer();

            //refresh screen every 100 ms
            aTimer.Interval = 4000;
            aTimer.Enabled = true;

            aTimer.Tick += new EventHandler(timer_machine);
        }

        private void timer_machine(Object source, EventArgs e)
        {
            if (gVariable.machineListFresh == 1)
            {
                toolClass.nonBlockingDelay(1000);
                displayMachineManagementListview();
            }

            gVariable.machineListFresh = 0;
        }

        //get database type index
        public static int getDatabaseType(int taskType1, int taskSelected1)
        {
            int databaseType1;

            //get file index in database fle table
            switch (taskType1)
            {
                case Task_Type_Routine:
                    databaseType1 = mySQLClass.DATA_TYPE_DAILYCHECK + taskSelected1;  //get index in mySQLClass.insertDataTableString
                    break;
                case Task_Type_Maintenance:
                    databaseType1 = mySQLClass.DATA_TYPE_MAINTAIN + taskSelected1;  //get index in mySQLClass.insertDataTableString
                    break;
                case Task_Type_Repairing:
                    databaseType1 = mySQLClass.DATA_TYPE_REPAIR + taskSelected1;  //get index in mySQLClass.insertDataTableString
                    break;
                default:
                    Console.Write("taskType is invalid, it is " + taskType1);
                    databaseType1 = -1;
                    break;
            }

            return databaseType1;
        }

        private DataTable getMachineTaskFromDatabase()
        {
            string databaseName;
            string commandText;

            try
            {
                databaseName = gVariable.DBHeadString + (machineSelected + 1).ToString().PadLeft(3, '0');
                commandText = "select * from `" + gVariable.machineManagementTableName[taskType, taskSelected] + "`"; // where machineName = " + "\'" + gVariable.machineNameArray[machineSelected] + "\'";

                return mySQLClass.queryDataTableAction(databaseName, commandText, null);
            }
            catch(Exception ex)
            {
                Console.WriteLine("getMachineTaskFromDatabase failed ", ex);
                return null;
            }
        }

        private void displayMachineManagementListview()
        {
            int i;
            int len;
            DataTable excelTable;
            DataTable dTable;
            int[] widthArray = { 65, 120, 70, 90, 120, 50, 110, 560, 80, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120 };

            try
            {
                dTable = getMachineTaskFromDatabase();

                listView1.Clear();
                listView1.BeginUpdate();
                listView1.GridLines = true;
                listView1.Dock = DockStyle.Fill;

                i = 0;
                len = 0;
                listView1.Columns.Add(" ", 1, HorizontalAlignment.Center);
                listView1.Columns.Add("序号", 40, HorizontalAlignment.Center);

                machineTableName = gVariable.machineTableFilename[taskType, taskSelected];  //like "..\\..\\data\\routin-check1.xlsx"
                //get title from excel, not from database, because we only save data into database, titles are not
                excelTable = mySQLClass.readExcelToDataTable(machineTableName, gVariable.EXCEL_FIRSTLINE_DATA);

                foreach (DataRow dr in excelTable.Rows)
                {
                    len = dr.ItemArray.Length;

                    for (i = 0; i < len; i++)
                        listView1.Columns.Add(dr[i].ToString().Trim(), widthArray[i + 1], HorizontalAlignment.Center);

                    break;
                }

                foreach (DataRow dr in dTable.Rows)
                {
                    ListViewItem OptionItem = new ListViewItem();

                    for (i = 0; i <= len; i++)
                        OptionItem.SubItems.Add(dr[i].ToString().Trim());

                    listView1.Items.Add(OptionItem);
                }

                this.listView1.EndUpdate();
            }
            catch (Exception ex)
            {
                Console.Write("displayMachineManagementListview() failed with exception: " + ex);
            }
        }

        //list required dispatch
        private void button1_Click(object sender, EventArgs e)
        {
            switch (checkTypeSelected)
            {
                case gVariable.MACHINE_MANAGEMENT_STATUS:
                    label1.Text = gVariable.enterpriseTitle + "设备管理系统 - 设备状态";
                    break;
                case gVariable.MACHINE_MANAGEMENT_LEDGER:
                    label1.Text = gVariable.enterpriseTitle + "设备管理系统 - 台帐管理";
                    break;
                case gVariable.MACHINE_MANAGEMENT_ITEM_CHECKING:
                    label1.Text = gVariable.enterpriseTitle + "设备管理系统 - 点巡检管理";
                    break;
                case gVariable.MACHINE_MANAGEMENT_MAINTENANCE:
                    label1.Text = gVariable.enterpriseTitle + "设备管理系统 - 保养管理";
                    break;
                case gVariable.MACHINE_MANAGEMENT_REPAIRING:
                    label1.Text = gVariable.enterpriseTitle + "设备管理系统 - 维修管理";
                    break;
                case gVariable.EFFICIENCY_MACHINE_OEE:
                    label1.Text = gVariable.enterpriseTitle + "设备管理系统 - OEE管理";
                    break;
            }
        }

        //generate new dispatch
        private void button2_Click(object sender, EventArgs e)
        {
            DataTable dt;

            try
            {
                switch (gVariable.secondFunctionIndex)
                {
                    case gVariable.MACHINE_MANAGEMENT_STATUS:
                        break;
                    case gVariable.MACHINE_MANAGEMENT_LEDGER:
                        break;
                    case gVariable.MACHINE_MANAGEMENT_ITEM_CHECKING:
                        machineTableName = gVariable.machineTableFilename[Task_Type_Routine, taskSelected];  //like "..\\..\\data\\routin-check1.xlsx"
                        if (machineTableName != null)
                        {
                            dt = mySQLClass.readExcelToDataTable(machineTableName, gVariable.EXCEL_FIRSTLINE_DATA);
                            mDispatchList.mDispatchListClass = new mDispatchList(machineSelected, Task_Type_Routine, taskSelected, dt);
                            mDispatchList.mDispatchListClass.Show();
                            this.Hide();
                        }
                        break;
                    case gVariable.MACHINE_MANAGEMENT_MAINTENANCE:
                        machineTableName = gVariable.machineTableFilename[Task_Type_Maintenance, taskSelected];  //like "..\\..\\data\\maintenance1.xlsx"
                        if (machineTableName != null)
                        {
                            dt = mySQLClass.readExcelToDataTable(machineTableName, gVariable.EXCEL_FIRSTLINE_DATA);
                            mDispatchList.mDispatchListClass = new mDispatchList(machineSelected, Task_Type_Maintenance, taskSelected, dt);
                            mDispatchList.mDispatchListClass.Show();
                            this.Hide();
                        }
                        break;
                    case gVariable.MACHINE_MANAGEMENT_REPAIRING:
                        machineTableName = gVariable.machineTableFilename[Task_Type_Repairing, taskSelected];  //like "..\\..\\data\\repairing1.xlsx"
                        if (machineTableName != null)
                        {
                            dt = mySQLClass.readExcelToDataTable(machineTableName, gVariable.EXCEL_FIRSTLINE_DATA);
                            mDispatchList.mDispatchListClass = new mDispatchList(machineSelected, Task_Type_Repairing, taskSelected, dt);
                            mDispatchList.mDispatchListClass.Show();
                            this.Hide();
                        }
                        break;
                    case gVariable.EFFICIENCY_MACHINE_OEE:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Write("machine management generate new dispatch failed!" + ex);
            }
            
        }

        private void machineManagement_FormClosing(object sender, EventArgs e)
        {
            workshopZihua.workshopZihuaClass.Show();
        }

        //exit
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //selected a new machine
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            machineSelected = comboBox1.SelectedIndex;
        }

        //selected a new task
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            taskSelected = comboBox2.SelectedIndex;
        }

        //start date changed
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        //end date changed
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

    }
}
