using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.common;
using MESSystem.mainUI;
using MESSystem.communication;

namespace MESSystem.dispatchManagement
{
    public partial class dispatchPublish : Form
    {
        int customerSelected;
        int machineSelected;
        int dispatchStatusSelected;
        int lineOwnerSelected;
        int assistant1Selected;
        int assistant2Selected;

        string[] customerList;
        string[] machineList;
        int[] dispatchStatusIDList = { 5, -3, -2, 0, 2, 3, 1 };
        string[] dispatchStatusList = { "全部状态", "预排程完毕", "排程已确认", "工单已发布", "工单已申请", "工单已开工", "工单已完工" };
       
        string[] workerIDList;
        string[] workerNameList;

        string startDateSearch, endDateSearch;
        string startDatePlanned, endDatePlanned;

        string[,] batchTableArray;

        public static int publishScreenRefresh;
        public static string cancelReason;

        System.Windows.Forms.Timer aTimer;

        public static dispatchPublish dispatchPublishClass = null; //用来引用主窗口

        gVariable.dispatchSheetStruct[] dispatchListArray; //all the dispatches in listview following the search condition
        adjustDispatch adjustDispatchImpl;  //assign worker/asistant worker to this dispatch, not used any more

        public dispatchPublish()
        {
            InitializeComponent();
            initializeVariables();
            resizeScreen();
        }


        void initializeVariables()
        {
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            label1.Text = gVariable.enterpriseTitle + "任务单下发系统";

            customerSelected = 0;
            machineSelected = 0;
            publishScreenRefresh = 0;

            customerList = toolClass.getCustomerList(1);  //1 means add item of "all customer"
            machineList = toolClass.getMachineList(1); //1 means add item of "all machines"
            workerIDList = toolClass.getWorkerInfoList(1, 1);  //1 means add item of "all employees, 1 means worker ID
            workerNameList = toolClass.getWorkerInfoList(1, 0);  //1 means add item of "all employees, 0 means worker name
        }

        void resizeScreen()
        {
            int i;
            int x, y, w, h;
            float fontSize;
            float screenRatioX;
            float screenRatioY;
            GroupBox[] groupBoxArray = { groupBox1, groupBox4 };
            Label[] labelArray = { label2, label6, label8, label9, label11, label12 };
            TextBox[] textBoxArray = { textBox1,  };
            Button[] buttonArray = { button1, button2, button3, button5, button6 };
            ComboBox[] comboBoxArray = { comboBox1, comboBox2, comboBox4 };
            //CheckBox[] checkBoxArray = { checkBox1, checkBox3 };
            DateTimePicker[] timePickerArray = { dateTimePicker3, dateTimePicker4 };
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

            /*
            for (i = 0; i < checkBoxArray.Length; i++)
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

            //listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", commonFontSize[gVariable.resolutionLevel], System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }

        private void listView_Load(object sender, EventArgs e)
        {
            int i;

            for (i = 0; i < machineList.Length; i++)
            {
                comboBox1.Items.Add(machineList[i]);
            }
            comboBox1.SelectedIndex = machineSelected;

            for (i = 0; i < dispatchStatusList.Length; i++)
            {
                comboBox2.Items.Add(dispatchStatusList[i]);
            }
            comboBox2.SelectedIndex = dispatchStatusSelected;

            for (i = 0; i < customerList.Length; i++)
            {
                comboBox4.Items.Add(customerList[i]);
            }
            comboBox4.SelectedIndex = customerSelected;

            displayDispatchList();

            aTimer = new System.Windows.Forms.Timer();

            //refresh screen every 2 seconds
            aTimer.Interval = 2000;
            aTimer.Enabled = true;

            aTimer.Tick += new EventHandler(timer_updateForm);
        }

        private void timer_updateForm(Object source, EventArgs e)
        {
            if (publishScreenRefresh != 0)
                displayDispatchList();
            publishScreenRefresh = 0;
        }

        void displayDispatchList()
        {
            int i;
            int status;
            int nameIndex;
            int[] dispatchLenArray = { 22, 45, 80, 90, 80, 100, 90, 70, 90, 110, 110, 65, 65, 110, 120 };
            string[] dispatchColumnArray = 
            {
                "", "序号", "生产批次号", "订单编号", "产品编码", "产品名称", "交货日期", "生产状态", "客户", "预计开工", "预计完工", "计划数", "取消人", "取消时间", "取消原因"
            };

            try
            {
                getBatchListByCondition();

                //cancel publish class just confirmed cancelation of the selected batch orders
                if (publishScreenRefresh == 2)
                {
                    cancelSelectedDispatches();
                }

                listView1.Clear();

                this.listView1.BeginUpdate();
                listView1.GridLines = true;
                listView1.Dock = DockStyle.Fill;

                if (batchTableArray != null)
                {
                    for (i = 0; i < dispatchLenArray.Length; i++)
                    {
                        if (i == 0)
                            listView1.Columns.Add(dispatchColumnArray[i], (int)(dispatchLenArray[i]), HorizontalAlignment.Center);
                        else
                            listView1.Columns.Add(dispatchColumnArray[i], (int)(dispatchLenArray[i] * gVariable.screenRatioX), HorizontalAlignment.Center);
                    }

                    for (i = 0; i < batchTableArray.GetLength(0); i++)
                    {
                        ListViewItem OptionItem = new ListViewItem();

                        nameIndex = 0;
                        if (nameIndex > gVariable.machineNameArrayAPS.Length)
                            nameIndex = 0;
                        OptionItem.SubItems.Add((i + 1).ToString());  //ID
                        //OptionItem.SubItems.Add(""); //gVariable.machineNameArrayAPS[nameIndex]);
                        if (batchTableArray[i, 19] != "")
                            OptionItem.SubItems.Add(batchTableArray[i, 19]);  //batchNum
                        else
                            OptionItem.SubItems.Add("");  //batchNum
                        OptionItem.SubItems.Add(batchTableArray[i, 1]);  //batch order 
                        OptionItem.SubItems.Add(batchTableArray[i, 4]);  //product code
                        OptionItem.SubItems.Add(batchTableArray[i, 5]);  //product name
                        OptionItem.SubItems.Add(batchTableArray[i, 3]);  //delivery date
                         
                        status = Convert.ToInt32(batchTableArray[i, 17]);  //status 
 
                        if(status > gVariable.SALES_ORDER_STATUS_CANCELLED)
                        {
                            OptionItem.SubItems.Add("批次单状态有误");
                        }
                        else
                        {
                            if (status == gVariable.SALES_ORDER_STATUS_CANCEL_APPLIED)
                                OptionItem.BackColor = Color.Red;
                            else if (status == gVariable.SALES_ORDER_STATUS_CANCELLED)
                                OptionItem.BackColor = Color.Gray;
                            else if (status > gVariable.SALES_ORDER_STATUS_CONFIRMED)
                                OptionItem.BackColor = Color.LightGreen;

                            if (status <= gVariable.SALES_ORDER_STATUS_CONFIRMED)
                                OptionItem.SubItems.Add("未下发");
                            else
                                OptionItem.SubItems.Add(gVariable.salesorderStatus[status]);
                        }

                        OptionItem.SubItems.Add(batchTableArray[i, 8]); //customer
                        OptionItem.SubItems.Add(batchTableArray[i, 13]);  //planned start
                        OptionItem.SubItems.Add(batchTableArray[i, 14]);  //planned finish
                        OptionItem.SubItems.Add(batchTableArray[i, 6] + "kg");  //planned quantity 
                        OptionItem.SubItems.Add(batchTableArray[i, 20]);  //canceller 
                        OptionItem.SubItems.Add(batchTableArray[i, 22]);  //cancel time 
                        OptionItem.SubItems.Add(batchTableArray[i, 21]);  //cancel reason 

                        listView1.Items.Add(OptionItem);
                    }
                }
                this.listView1.EndUpdate();

                this.Text = gVariable.programTitle + "工单列表查询";
            }
            catch (Exception ex)
            {
                Console.WriteLine("dispatchPublish listView_load failed! " + ex);
            }
        }

        private void dispatchPublish_FormClosing(object sender, EventArgs e)
        {
            firstScreen.firstScreenClass.Show();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.listView1.SelectedItems.Count > 0)
                {
                    //adjustDispatchImpl = new adjustDispatch(this.listView1.SelectedItems[0].SubItems[4].Text);
                    //adjustDispatchImpl.Show();
                    //refreshFlag = 1;
                }
            }
            catch (Exception ex)
            {
                Console.Write("listView1_SelectedIndexChanged failed. " + ex);
                return;
            }
        }

        //publish dispatches
        private void button3_Click(object sender, EventArgs e)
        {
            int i, j;
            //int delta;
            int ret;
            int index;
            int machineID;
            //string str;
            string columnsDispatch;
            string columnsMaterial;
            string databaseName;
            string commandText;
            string[] strArray;
            string[,] tableArray;

            try
            {
                if (listView1.CheckedItems.Count == 0)
                {
                    MessageBox.Show("请先在工单列表左侧的复选框中选中需要发布的工单，然后再点选工单发布按钮。", "提示信息", MessageBoxButtons.OK);
                    return;
                }

                for (i = 0; i < listView1.CheckedItems.Count; i++)
                {
                    index = listView1.CheckedItems[i].Index;
                    if (Convert.ToInt32(batchTableArray[index, 17]) != gVariable.SALES_ORDER_STATUS_CONFIRMED)
                    {
                        MessageBox.Show("请确认所有选中的工单都处于排程已确认状态（该状态的工单都会以白色背景显示），其它状态的工单无法发布。", "提示信息", MessageBoxButtons.OK);
                        return;
                    }
                }

                strArray = null;
                columnsDispatch = null;
                //get all the column names for dispatch table, we get these names from excel file(the original dispatch sample),  in case these columns are changed, 
                ret = mySQLClass.getListTitleFromExcel(gVariable.dispatchListFileName, ref strArray, gVariable.EXCEL_FIRSTLINE_TITLE);
                if (ret < 0)
                {
                    MessageBox.Show("文件 ..\\..\\data\\machine\\dispatchList.xlsx 有误，请联系开发人员。", "提示信息", MessageBoxButtons.OK);
                    return;
                }
                else
                {
                    for (i = 0; i < strArray.Length; i++)
                    {
                        if (i == 0)
                            columnsDispatch += strArray[i];
                        else
                            columnsDispatch += "," + strArray[i];
                    }
                }

                columnsMaterial = null;
                ret = mySQLClass.getListTitleFromExcel(gVariable.materialListFileName, ref strArray, gVariable.EXCEL_FIRSTLINE_TITLE);
                if (ret < 0)
                {
                    MessageBox.Show("文件 ..\\..\\data\\machine\\materialList.xlsx 有误，请联系开发人员。", "提示信息", MessageBoxButtons.OK);
                    return;
                }
                else
                {
                    for (i = 0; i < strArray.Length; i++)
                    {
                        if (i == 0)
                            columnsMaterial += strArray[i];
                        else
                            columnsMaterial += "," + strArray[i];
                    }
                }

                //deal with all machines one by one using machine ID
                for (i = 0; i < listView1.CheckedItems.Count; i++)
                {
                    //get index in dispatch list
                    index = listView1.CheckedItems[i].Index;

                    //if (batchTableArray[index, 17] != gVariable.SALES_ORDER_STATUS_CONFIRMED)
                    //    continue;

                    commandText = "update `" + gVariable.productBatchTableName + "` set orderStatus = '" + gVariable.SALES_ORDER_STATUS_PUBLISHED + "' where id = '" + batchTableArray[index, 0] + "'";
                    mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

                    //if only some of the batch order inside a sales order changed its status, we should not modify status for the whole sales order, so remove this function below
                    //commandText = "update `" + gVariable.salesOrderTableName + "` set orderStatus = '" + gVariable.SALES_ORDER_STATUS_PUBLISHED + "' where salesOrdercode = '" + batchTableArray[index, 2] + "'";
                    //mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

                    //first update this dispatch to the status published
                    commandText = "update `" + gVariable.globalDispatchTableName + "` set status = '" + gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED + "' where salesOrderBatchCode = '" + batchTableArray[index, 1] + "'";
                    mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

                    commandText = "select * from `" + gVariable.globalDispatchTableName + "` where salesOrderBatchCode = '" + batchTableArray[index, 1] + "'";
                    tableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                    if (tableArray == null)
                    {
                        Console.WriteLine("filed to find dispatch with this code！");
                        return;
                    }

                    for (j = 0; j < tableArray.GetLength(0); j++)
                    {
                        machineID = Convert.ToInt32(tableArray[j, 1]); // Convert.ToInt32(dispatchListArray[index].machineID);
                        databaseName = gVariable.DBHeadString + machineID.ToString().PadLeft(3, '0');
                        //then copy this dispatch to database for the specific machine
                        commandText = "insert into " + databaseName + "." + gVariable.dispatchListTableName + "(" + columnsDispatch + ") select " + columnsDispatch + " from " + gVariable.globalDatabaseName + "." +
                                       gVariable.globalDispatchTableName + " where dispatchCode = '" + tableArray[j, 2] + "'";
                        mySQLClass.pureDatabaseNonQueryAction(databaseName, commandText);

                        //for casting process, we need to put material list from global database to machine database together with dispatch
                        if (machineID <= gVariable.castingProcess.Length)
                        {
                            //cast machine ID 
                            //delta = gVariable.castingProcess[0] - gVariable.feedingProcess[0];
                            databaseName = gVariable.DBHeadString + machineID.ToString().PadLeft(3, '0');

                            //save in cast machine database
                            commandText = "insert into " + databaseName + "." + gVariable.materialListTableName + "(" + columnsMaterial + ") select " + columnsMaterial + " from " + gVariable.globalDatabaseName + "." +
                                           gVariable.globalMaterialTableName + " where dispatchCode = '" + tableArray[j, 2] + "'";
                            mySQLClass.pureDatabaseNonQueryAction(databaseName, commandText);
                            //change dispatch code name to cast machine dispatch code
                            //str = dispatchListArray[index].dispatchCode.Replace("T", "L");
                            //commandText = "update `" + gVariable.materialListTableName + "` set dispatchCode = '" + str + "' where dispatchCode = '" + dispatchListArray[index].dispatchCode + "'";
                            //mySQLClass.pureDatabaseNonQueryAction(databaseName, commandText);
                        }
                    }
                }

                toolClass.systemDelay(1000);

                //displayDispatchList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("publish dispatch failed!" + ex);
            }
        }

        //search for then display all dispatches according to conditions
        private void button1_Click(object sender, EventArgs e)
        {
            displayDispatchList();
        }



        void getBatchListByCondition()
        {
            int num;
            int sthUpdated;
            string commandText;
            string tableName;
            string databaseName;
            string str1, str2, str3, str4, str5;

            databaseName = gVariable.globalDatabaseName;
            tableName = gVariable.productBatchTableName;

            sthUpdated = 0;
            str1 = null;

            //this is a legal string for dispatch code
            if (textBox1.Text != null && textBox1.Text.Length > 5)
            {
                str1 = " dispatchCode = '" + textBox1.Text + "'";
                sthUpdated = 1;
            }

            str2 = null;
            if(customerSelected != 0)
            {
                if (sthUpdated == 0)
                    str2 = " customer = '" + customerList[customerSelected] + "'";
                else
                    str2 = " and customer = '" + customerList[customerSelected] + "'";

                sthUpdated = 1;
            }

            str3 = null;
            /*
            if(machineSelected != 0)
            {
                if (sthUpdated == 0)
                    str3 = " machineID = '" + machineSelected + "'";
                else
                    str3 = " and machineID = '" + machineSelected + "'";

                sthUpdated = 1;
            }
            */
            str4 = null;
            if(dispatchStatusSelected != 0)
            {
                if (sthUpdated == 0)
                    str4 = " status = '" + dispatchStatusIDList[dispatchStatusSelected] + "'";
                else
                    str4 = " and status = '" + dispatchStatusIDList[dispatchStatusSelected] + "'";
            }
            else
            {
                if (sthUpdated == 0)
                    str4 = " orderStatus > '" + gVariable.SALES_ORDER_STATUS_APS_OK + "'";
                else
                    str4 = " and orderStatus > '" + gVariable.SALES_ORDER_STATUS_APS_OK + "'";
            }
            sthUpdated = 1;

            str5 = null;
            if(startDateSearch != null)
            {
                if (sthUpdated == 0)
                {
                    str5 = " time1 >= '" + startDateSearch + "' and time2 <= '" + endDateSearch + "' 23:59";
                }
                else
                {
                    str5 = " and time1 >= '" + startDateSearch + "' and time2 <= '" + endDateSearch + "' 23:59";
                }

                sthUpdated = 1;
            }

            if (sthUpdated == 0)
                commandText = "select * from `" + tableName + "`";
            else
                commandText = "select * from `" + tableName + "`" + " where" + str1 + str2 + str3 + str4 + str5;

            batchTableArray = mySQLClass.databaseCommonReading(databaseName, commandText);
            if (batchTableArray == null)
            {
                Console.WriteLine("getBatchListByCondition filed！");
                return;
            }

            /*
            if (sthUpdated == 0)
                commandText = "select count(*) from `" + tableName + "`";
            else
                commandText = "select count(*) from `" + tableName + "`" + " where" + str1 + str2 + str3 + str4 + str5;

            num = mySQLClass.getNumOfRecordByCondition(databaseName, commandText);
            
            dispatchListArray = mySQLClass.getDispatchListInternal(databaseName, tableName, commandText, num);
             */
        }

        //output to excel file
        private void button6_Click(object sender, EventArgs e)
        {

        }

        //cancel publish, restore the the selected dispatches to confirmed mode(unpublished)
        public void cancelSelectedDispatches()
        {
            int i, j;
            int index;
            int machineID;
            string databaseName;
            string commandText;
            string[,] tableArray;

            try
            {
                //deal with all machines one by one using machine ID
                for (i = 0; i < listView1.CheckedItems.Count; i++)
                {
                    index = listView1.CheckedItems[i].Index;

                    commandText = "update `" + gVariable.productBatchTableName + "` set orderStatus = '" + gVariable.SALES_ORDER_STATUS_CANCEL_APPLIED + 
                                  "', canceller = '" + gVariable.userAccount + "', cancelReason = '" + dispatchPublish.cancelReason + "', cancelTime = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") +
                                  "' where id = '" + batchTableArray[index, 0] + "'";
                    mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

                    //commandText = "update `" + gVariable.salesOrderTableName + "` set orderStatus = '" + gVariable.SALES_ORDER_STATUS_CONFIRMED + "' where orderStatus = '" + gVariable.SALES_ORDER_STATUS_PUBLISHED + "'";
                    //mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

                    //first update this dispatch to the status published
                    commandText = "update `" + gVariable.globalDispatchTableName + "` set status = '0' where salesOrderBatchCode = '" + batchTableArray[index, 1] + "'";
                    mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

                    commandText = "update `" + gVariable.globalDispatchTableName + "` set status = '" + gVariable.MACHINE_STATUS_DISPATCH_CONFIRMED + "' where salesOrderBatchCode = '" + batchTableArray[index, 1] + "'";
                    mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

                    commandText = "select * from `" + gVariable.globalDispatchTableName + "` where salesOrderBatchCode = '" + batchTableArray[index, 1] + "'";
                    tableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                    if (tableArray == null)
                    {
                        Console.WriteLine("filed to find dispatch with this code 2！");
                        return;
                    }

                    for (j = 0; j < tableArray.GetLength(0); j++)
                    {
                        machineID = Convert.ToInt32(tableArray[j, 1]); // Convert.ToInt32(dispatchListArray[index].machineID);
                        databaseName = gVariable.DBHeadString + machineID.ToString().PadLeft(3, '0');

                        commandText = "delete from `" + gVariable.dispatchListTableName + "` where dispatchCode = '" + tableArray[j, 2] + "'";
                        mySQLClass.pureDatabaseNonQueryAction(databaseName, commandText);
                        mySQLClass.redoIDIncreamentAfterRecordDeleted(databaseName, gVariable.dispatchListTableName);

                        //for casting process, we need to put material list from global database to machine database together with dispatch
                        if (machineID <= gVariable.castingProcess.Length)
                        {
                            commandText = "delete from `" + gVariable.materialListTableName + "` where dispatchCode = '" + tableArray[j, 2] + "'";
                            mySQLClass.pureDatabaseNonQueryAction(databaseName, commandText);
                            mySQLClass.redoIDIncreamentAfterRecordDeleted(databaseName, gVariable.dispatchListTableName);
                        }
                    }
                }
                getBatchListByCondition();

                publishScreenRefresh = 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("publish dispatch failed!" + ex);
            }
        }

        //cancel publish, restore the the selected dispatches to confirmed mode(unpublished)
        private void button2_Click(object sender, EventArgs e)
        {
            int i;
            //int ret;
            int index;
            string databaseName;
            string commandText;
            //string[] strArray;

            try
            {
                if (listView1.CheckedItems.Count == 0)
                {
                    MessageBox.Show("请先在工单列表左侧的复选框中选中需要取消发布的工单，然后再点选取消发布按钮。", "提示信息", MessageBoxButtons.OK);
                    return;
                }

                for (i = 0; i < listView1.CheckedItems.Count; i++)
                {
                    index = listView1.CheckedItems[i].Index;
                    if (Convert.ToInt32(batchTableArray[index, 17]) != gVariable.SALES_ORDER_STATUS_PUBLISHED)
                    {
                        MessageBox.Show("请确认所有选中的工单都处于已发布状态，其它状态的工单无法取消发布。", "提示信息", MessageBoxButtons.OK);
                        return;
                    }
                }

                index = listView1.CheckedItems[0].Index;
                cancelPublish cancelPublishImpl = new cancelPublish(batchTableArray, index);
                cancelPublishImpl.Show();
                //displayDispatchList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("publish dispatch failed!" + ex);
            }
        }


        //time condition for dispatch searching
        /*
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                startDateSearch = dateTimePicker3.Value.ToString("yyyy-MM-dd");
                endDateSearch = dateTimePicker4.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                startDateSearch = null;
                endDateSearch = null;
            }
        }
        

        //time condition to publish/unpublish selected dispatches
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                startDatePlanned = dateTimePicker3.Value.ToString("yyyy-MM-dd");
                endDatePlanned = dateTimePicker4.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                startDatePlanned = null;
                endDatePlanned = null;
            }
        }
        */

        //customer selection changed
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            customerSelected = comboBox4.SelectedIndex;
        }

        //machine selection changed
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            machineSelected = comboBox1.SelectedIndex;
        }

        //dsipatch status selection changed
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            dispatchStatusSelected = comboBox2.SelectedIndex;
        }

        //line owner selection changed
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
        //    lineOwnerSelected = comboBox5.SelectedIndex;
        }

        //assistant 1 selected
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
        //    assistant1Selected = comboBox6.SelectedIndex;
        }

        //assistant 2 selected
        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
        //    assistant2Selected = comboBox7.SelectedIndex;
        }

        //confirm the modification of the selected dispatch info
        private void button4_Click(object sender, EventArgs e)
        {

        }

        //cancel approve
        private void button5_Click(object sender, EventArgs e)
        {
            int i;
            int index;
            string commandText;
            DialogResult dr;

            dr = MessageBox.Show("确认核准取消该任务单吗？", "需要确认", MessageBoxButtons.YesNo);
            if (dr == DialogResult.No)
            {
                return;
            }

            for (i = 0; i < listView1.CheckedItems.Count; i++)
            {
                index = listView1.CheckedItems[i].Index;
                if (Convert.ToInt32(batchTableArray[index, 17]) != gVariable.SALES_ORDER_STATUS_CANCEL_APPLIED)
                {
                    MessageBox.Show("请确认所有选中的任务单都处于申请取消状态，其他状态无法核准。", "提示信息", MessageBoxButtons.OK);
                    return;
                }
            }

            for (i = 0; i < listView1.CheckedItems.Count; i++)
            {
                index = listView1.CheckedItems[i].Index;
                commandText = "update `" + gVariable.productBatchTableName + "` set orderStatus = '" + gVariable.SALES_ORDER_STATUS_CANCELLED + "' where id = '" + batchTableArray[index, 0] + "'";
                mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);
            }

            return;
        }

        //cancel reason
        private void button4_Click_1(object sender, EventArgs e)
        {

        }
    }
}
