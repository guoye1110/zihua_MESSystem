using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.mainUI;
using MESSystem.common;
using MESSystem.APSDLL;

namespace MESSystem.APS_UI
{
    public partial class APSUI : Form
    {
        //if there are more than this number of sales order, we will only dela with recent this number
        const int SALES_ORDER_NUM_MAX = 5000;

        string[] machineName1 = { "不指定", "1号流延机", "2号流延机", "3号流延机", "4号流延机", "5号流延机" };
        string[] machineName2 = { "不指定", "2号印刷机", "3号印刷机", "4号印刷机" };
        string[] machineName3 = { "不指定", "1号分切机", "3号分切机", "5号分切机", "6号分切机", "7号分切机" };

        string[] salesOrderPriorityRule = { "不指定", "交货日期优先", "重要客户优先", "产量大者优先" };
        string[] machinePriorityRule = { "不指定", "设备产量优先", "良品率优先" };

        const int PROIRITY_ORDER_NOT_SELECTED = 0;
        const int PROIRITY_ORDER_DELIVERY_TIME = 1;
        const int PROIRITY_ORDER_CUSTOMER_IMPORTANCE = 2;
        const int PROIRITY_ORDER_OUTPUT_QUANTITY = 3;

        int machineSelected1;
        int machineSelected2;
        int machineSelected3;
        int salesOrderPrioritySelected;
        int machinePrioritySelected;

        int salesOrderSelected;

        float screenRatioX, screenRatioY;

        public static APSUI APSUIClass = null; //it is used to reference this windows
        //public static int [] salesOrderIndexArray = new int[SALES_ORDER_NUM_MAX];
        string[,] salesTableArray;
        gVariable.dispatchSheetStruct[] dispatchListArray;

        System.Windows.Forms.Timer aTimer;

        public APSUI()
        {
            InitializeComponent();
            initVariables();
            resizeScreen();
        }

        void initVariables()
        {
            int i;

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            gVariable.APSScreenRefresh = 1;

            label1.Text = gVariable.enterpriseTitle + "生产计划排程系统";
            this.Text = gVariable.enterpriseTitle + "生产计划排程系统";

            machineSelected1 = 0;
            machineSelected2 = 0;
            machineSelected3 = 0;

            salesOrderSelected = -1;

            for (i = 0; i < machineName1.Length; i++)
            {
                comboBox1.Items.Add(machineName1[i]);
            }
            comboBox1.SelectedIndex = machineSelected1;

            for (i = 0; i < machineName2.Length; i++)
            {
                comboBox2.Items.Add(machineName2[i]);
            }
            comboBox2.SelectedIndex = machineSelected2;

            for (i = 0; i < machineName3.Length; i++)
            {
                comboBox5.Items.Add(machineName3[i]);
            }
            comboBox5.SelectedIndex = machineSelected3;

            for (i = 0; i < salesOrderPriorityRule.Length; i++)
            {
                comboBox3.Items.Add(salesOrderPriorityRule[i]);
            }
            comboBox3.SelectedIndex = salesOrderPrioritySelected;

            for (i = 0; i < machinePriorityRule.Length; i++)
            {
                comboBox6.Items.Add(machinePriorityRule[i]);
            }
            comboBox6.SelectedIndex = machinePrioritySelected;
        }

        void resizeScreen()
        {
            int i;
            int x, y, w, h;
            float fontSize;
            GroupBox[] groupBoxArray = { groupBox1, groupBox2, groupBox3, groupBox4 };
            Label[] labelArray = { label2, label4, label3, label5, label6, label7, label8, label9, label10, label11, label12, label13 };
            TextBox[] textBoxArray = { textBox1, textBox2 };
            Button[] buttonArray = { button1, button2, button3, button4, button5, button6, button7 };
            ComboBox[] comboBoxArray = { comboBox1, comboBox2, comboBox3, comboBox4, comboBox5, comboBox6 };
            CheckBox[] checkBoxArray = { checkBox1, checkBox2, checkBox3 };
            DateTimePicker[] timePickerArray = { dateTimePicker1, dateTimePicker2, dateTimePicker3, dateTimePicker4 };
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

            for (i = 0; i < timePickerArray.Length; i++)
            {
                w = (int)(timePickerArray[i].Size.Width * screenRatioX);
                h = (int)(timePickerArray[i].Size.Height * screenRatioY);
                timePickerArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(timePickerArray[i].Location.X * screenRatioX);
                y = (int)(timePickerArray[i].Location.Y * screenRatioY);
                timePickerArray[i].Location = new System.Drawing.Point(x, y);
            }

            //listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }

        private void APSUI_Load(object sender, EventArgs e)
        {
            aTimer = new System.Windows.Forms.Timer();

            //refresh screen every 2 seconds
            aTimer.Interval = 2000;
            aTimer.Enabled = true;

            aTimer.Tick += new EventHandler(timer_updateForm);
            loadScreen();
        }

        private void timer_updateForm(Object source, EventArgs e)
        {
            if (gVariable.APSScreenRefresh != 0)
                loadScreen();
            gVariable.APSScreenRefresh = 0;
        }

        //cast machine assigned
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            machineSelected1 = comboBox1.SelectedIndex;
        }

        //print machine assigned
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            machineSelected2 = comboBox2.SelectedIndex;
        }

        //slit machine asigned
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            machineSelected3 = comboBox5.SelectedIndex;
        }

        //sales order priority selection
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            salesOrderPrioritySelected = comboBox3.SelectedIndex;
        }

        //machine priority selection
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            machinePrioritySelected = comboBox6.SelectedIndex;
        }

        private void loadScreen()
        {
            int i, j;
            //int ret;
            int length;
            int num;
            //int min;
            int status;
            int index1;
            int index2;
            int selectionChanged;
            string commandText;
            ListViewItem OptionItem;
            //string[] salesOrderCodeArray = new string[SALES_ORDER_NUM_MAX];
            //gVariable.salesOrderStruct salesOrderImpl = new gVariable.salesOrderStruct();
            int[] salesOrderLenArray = { 18, 45, 100, 100, 145, 90, 90, 90, 120, 120, 120, 120, 75 };
            string[] salesOrderListHeader = 
            {
                "全", "序号", "订单号", "产品编码", "产品名称", "接单日期", "交货日期", "需求数量", "客户名", "排产时间", "计划开工", "计划完工", "订单状态"
            };
            int[] dispatchLenArray = { 1, 45, 105, 130, 105, 146, 80, 100, 70, 100, 120, 120, 100 };
            string[] dispatchListHeader = 
            {
                " ", "序号", "订单号", "工单号", "产品编码", "产品名称", "计划数量", "承接设备", "班组", "负责人", "计划开工时间", "计划完工时间", "工单状态"
            };
            int[,] listviewLineNum1 = { 
                                         {10, 11, 11, 12, 12, 13},
                                         { 9, 11, 11, 11, 11, 13},
                                         { 9, 10, 10, 11, 11, 13}
                                     };
            int[] listviewLineNum2 = { 8, 10, 10, 10, 10, 11 };

            try
            {
                selectionChanged = 0;
                if (listView1.SelectedItems.Count != 0)
                {
                    //Console.WriteLine("salesOrderSelected = " + salesOrderSelected + "; index = " + listView1.SelectedItems[0].Index);

                    if (salesOrderSelected != listView1.SelectedItems[0].Index)
                    {
                        salesOrderSelected = listView1.SelectedItems[0].Index;
                        selectionChanged = 1;
                    }
                }

                index1 = 0;
                index2 = 0;

                if (listView1.TopItem != null)
                    index1 = listView1.TopItem.Index;

                if (listView2.TopItem != null)
                    index2 = listView2.TopItem.Index;

                num = mySQLClass.getRecordNumInTable(gVariable.globalDatabaseName, gVariable.salesOrderTableName);
                if (num > SALES_ORDER_NUM_MAX)
                    num = SALES_ORDER_NUM_MAX;

                commandText = "select * from `" + gVariable.salesOrderTableName + "` order by id desc";
                salesTableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (salesTableArray == null)
                    num = 0;

                //for (i = 0, j = 0; i < num; i++, j++)
                {
                    //salesOrderIndexArray[i] = Convert.ToInt32(salesTableArray[i, mySQLClass.ID_VALUE_IN_SALESORDER_DATABASE]) - 1;
                    //salesOrderCodeArray[i] = salesTableArray[i, mySQLClass.ORDER_CODE_IN_SALESORDER_DATABASE];
                }

                if (gVariable.APSScreenRefresh == 1)
                {
                    listView1.Clear();

                    this.listView1.BeginUpdate();

                    listView1.GridLines = true;
                    listView1.Dock = DockStyle.Fill;

                    for (i = 0; i < salesOrderLenArray.Length; i++)
                        listView1.Columns.Add(salesOrderListHeader[i], (int)(salesOrderLenArray[i] * screenRatioX), HorizontalAlignment.Center);

                    for (i = 0, j = 0; i < num; i++, j++)
                    {
                        OptionItem = new ListViewItem();

                        OptionItem.SubItems.Add((i + 1).ToString());
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.ORDER_CODE_IN_SALESORDER_DATABASE]);
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.PRODUCT_CODE_IN_SALESORDER_DATABASE]);
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.PRODUCT_NAME_IN_SALESORDER_DATABASE]);
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.ERP_TIME_IN_SALESORDER_DATABASE]);
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.DELIVERY_TIME_IN_SALESORDER_DATABASE]);
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.REQUIRED_NUM_IN_SALESORDER_DATABASE] + " " + salesTableArray[i, mySQLClass.UNIT_IN_SALESORDER_DATABASE]);
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.CUSTOMER_IN_SALESORDER_DATABASE]);
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.APS_TIME_IN_SALESORDER_DATABASE]);
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.PLANNED_START_TIME_IN_SALESORDER_DATABASE]);
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.PLANNED_COMPLETE_TIME_IN_SALESORDER_DATABASE]);

                        status = Convert.ToInt16(salesTableArray[i, mySQLClass.STATUS_IN_SALESORDER_DATABASE]);
                        switch (status)
                        {
                            case gVariable.SALES_ORDER_STATUS_ERP_PUBLISHED:
                                OptionItem.SubItems.Add("已导入");
                                break;
                            case gVariable.SALES_ORDER_STATUS_APS_OK:
                                OptionItem.SubItems.Add("已排程");
                                break;
                            case gVariable.SALES_ORDER_STATUS_CONFIRMED:
                                OptionItem.SubItems.Add("已确认");
                                break;
                            case gVariable.SALES_ORDER_STATUS_APPLIED:
                                OptionItem.SubItems.Add("已下发");
                                break;
                            case gVariable.SALES_ORDER_STATUS_STARTED:
                                OptionItem.SubItems.Add("已开工");
                                break;
                            case gVariable.SALES_ORDER_STATUS_COMPLETED:
                                OptionItem.SubItems.Add("已完工");
                                break;
                            case gVariable.SALES_ORDER_STATUS_CANCELLED:
                                OptionItem.SubItems.Add("已取消");
                                break;
                            default:
                                break;
                        }

                        listView1.Items.Add(OptionItem);
                    }

                    if (salesOrderSelected >= 0)
                        this.listView1.Items[salesOrderSelected].Selected = true;

                    if (listView1.TopItem != null && listView1.Items.Count > index1 + listviewLineNum1[gVariable.dpiValue, gVariable.resolutionLevel] - 1)
                        listView1.EnsureVisible(index1 + listviewLineNum1[gVariable.dpiValue, gVariable.resolutionLevel] - 1);

                    this.listView1.EndUpdate();
                }

                listView2.Clear();
                listView2.BeginUpdate();

                dispatchListArray = null;
                if (salesOrderSelected >= 0)
                {
                    commandText = " where salesOrderCode = " + "'" + salesTableArray[salesOrderSelected, mySQLClass.ORDER_CODE_IN_SALESORDER_DATABASE] + "'";
                    //find unpublished dispatch, which includes generated and confirmed
                    dispatchListArray = mySQLClass.getDispatchListByCommand(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, commandText);
                }

                if (dispatchListArray == null)
                    length = 0;
                else
                    length = dispatchListArray.Length;

                listView2.GridLines = true;
                listView2.Dock = DockStyle.Fill;

                for (i = 0; i < dispatchLenArray.Length; i++)
                    listView2.Columns.Add(dispatchListHeader[i], (int)(dispatchLenArray[i] * screenRatioX), HorizontalAlignment.Center);

                for (i = 0; i < length; i++)
                {
                    OptionItem = new ListViewItem();
                    OptionItem.SubItems.Add((i + 1).ToString());
                    OptionItem.SubItems.Add(dispatchListArray[i].salesOrderCode);
                    OptionItem.SubItems.Add(dispatchListArray[i].dispatchCode);
                    OptionItem.SubItems.Add(dispatchListArray[i].productCode);
                    OptionItem.SubItems.Add(dispatchListArray[i].productName);
                    OptionItem.SubItems.Add(dispatchListArray[i].plannedNumber.ToString());
                    OptionItem.SubItems.Add(gVariable.machineNameArray[Convert.ToInt16(dispatchListArray[i].machineID) - 1]);
                    OptionItem.SubItems.Add(dispatchListArray[i].workshift);
                    OptionItem.SubItems.Add(dispatchListArray[i].operatorName);
                    OptionItem.SubItems.Add(dispatchListArray[i].planTime1);
                    OptionItem.SubItems.Add(dispatchListArray[i].planTime2);

                    switch (dispatchListArray[i].status)
                    {
                        case gVariable.MACHINE_STATUS_DISPATCH_GENERATED:
                            OptionItem.SubItems.Add("预排程完毕");
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_CONFIRMED:
                            OptionItem.SubItems.Add("排程已确认");
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_UNPUBLISHED:
                            OptionItem.SubItems.Add("已排程");
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED:
                            OptionItem.SubItems.Add("车间已发布");
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_APPLIED:
                            OptionItem.SubItems.Add("工单已申请");
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_STARTED:
                            OptionItem.SubItems.Add("工单已开工");
                            break;
                        case gVariable.MACHINE_STATUS_DISPATCH_COMPLETED:
                            OptionItem.SubItems.Add("工单已完工");
                            break;
                        default:
                            break;
                    }
                    listView2.Items.Add(OptionItem);
                }

                if (listView2.TopItem != null && listView2.Items.Count > index2 + listviewLineNum2[gVariable.resolutionLevel] - 2 && selectionChanged == 0)
                    listView2.EnsureVisible(index2 + listviewLineNum2[gVariable.resolutionLevel] - 2);
                else
                {
                }

                listView2.EndUpdate();
            }
            catch (Exception ex)
            {
                Console.Write("screen fresh for APS UI failed! " + ex);
            }
        }

        private void APSUI_FormClosing(object sender, EventArgs e)
        {
            try
            {
                aTimer.Enabled = false;

                firstScreen.firstScreenClass.Show();
            }
            catch (Exception ex)
            {
                Console.Write("close apsui class" + ex);
            }
        }


        //adjust machine calendar
        private void button1_Click(object sender, EventArgs e)
        {

        }

        //add manual order
        private void button2_Click(object sender, EventArgs e)
        {
            int index;

            //if we selected a sales order then add a new one, new sales order will have the content of the selected sales order, then the user can modify the content
            if (this.listView1.SelectedItems.Count != 0)
                index = Convert.ToInt32(salesTableArray[listView1.SelectedItems[0].Index, mySQLClass.ID_VALUE_IN_SALESORDER_DATABASE]);
            else
                index = -1;

            manualSalesOrder.manualSalesOrderImpl = new manualSalesOrder(index);
            manualSalesOrder.manualSalesOrderImpl.Show();

            toolClass.nonBlockingDelay(2000);
        }


        //formal APS
        private void button3_Click(object sender, EventArgs e)
        {
            int i;
            int index;
            int startTimeStamp;
            int endTimeStamp;
            //int[] dispatchIndexList = new int[SALES_ORDER_NUM_MAX];
            int[] assignedMachineByUserArray = new int[3];
            int[] indexArray;
            string commandText;
            string companyName;
            string[] strArray;
            string[,] tableArray;
            APSProcess APSProcessImpl = new APSProcess();

            try
            {
                if (listView1.CheckedItems.Count == 0)
                {
                    index = -1;
                    //if (listView1.SelectedItems.Count != 0)
                    //    index = listView1.SelectedItems[0].Index;
                    MessageBox.Show("请先在工单列表左侧的复选框中选中需要排程的订单，然后再点选'勾选订单排程'按钮开始排程。", "提示信息", MessageBoxButtons.OK);
                    //this function still has problem, after pop up info box, the original reversely displayed lines will become normal display, need more study
                    if (listView1.SelectedItems.Count != 0)
                    {
                        //listView1.Items[index].Selected = false;
                        //toolClass.systemDelay(2000);
                        //listView1.Items[index].Selected = true;
                        //listView1.Items[index].EnsureVisible();
                    }
                    return;
                }

                for (i = 0; i < listView1.CheckedItems.Count; i++)
                {
                    index = listView1.CheckedItems[i].Index;
                    if (salesTableArray[index, mySQLClass.STATUS_IN_SALESORDER_DATABASE] != gVariable.SALES_ORDER_STATUS_ERP_PUBLISHED.ToString())
                    {
                        MessageBox.Show("请确认所选择的订单处于已导入状态，因为已排程的订单不能再次排程。若确实需要重新排程，请先取消该订单的排程结果后再尝试。", "提示信息", MessageBoxButtons.OK);
                        //  listView1.SelectedItems = index;
                        return;
                    }
                }

                indexArray = new int[listView1.CheckedItems.Count];
                strArray = new string[listView1.CheckedItems.Count];

                //get sales order list for APS
                switch (salesOrderPrioritySelected)
                {
                    case PROIRITY_ORDER_NOT_SELECTED: //do APS by sales order receive order
                        for (i = 0; i < listView1.CheckedItems.Count; i++)
                        {
                            indexArray[i] = listView1.CheckedItems[i].Index;
                        }
                        break;
                    case PROIRITY_ORDER_CUSTOMER_IMPORTANCE:  //do APS by the order of the importance of the customer company
                        //put sales order info in array and then do sorting by the importance of customer company
                        for (i = 0; i < listView1.CheckedItems.Count; i++)
                        {
                            index = listView1.CheckedItems[i].Index;
                            companyName = salesTableArray[index, mySQLClass.CUSTOMER_IN_SALESORDER_DATABASE];

                            commandText = "select * from `" + gVariable.customerListTableName + "` where customerName = '" + companyName + "'";
                            tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                            if (tableArray == null)
                            {
                                MessageBox.Show("基础数据中无法找到客户名称：" + companyName + "，请确认订单信息", "提示信息", MessageBoxButtons.OK);
                                return;
                            }
                            strArray[i] = tableArray[0, 3];
                            indexArray[i] = Convert.ToInt32(salesTableArray[index, mySQLClass.ID_VALUE_IN_SALESORDER_DATABASE]);
                        }
                        toolClass.stringSortingRecordIndex(strArray, indexArray);
                        break;
                    case PROIRITY_ORDER_OUTPUT_QUANTITY:  //do APS by the order of the output quantity, function not support at this time
                    case PROIRITY_ORDER_DELIVERY_TIME: //do APS by delivery time order
                    default:
                        //put sales order info in array and then do sorting by delivery time
                        for (i = 0; i < listView1.CheckedItems.Count; i++)
                        {
                            index = listView1.CheckedItems[i].Index;
                            strArray[i] = salesTableArray[index, mySQLClass.DELIVERY_TIME_IN_SALESORDER_DATABASE];
                            indexArray[i] = Convert.ToInt32(salesTableArray[index, mySQLClass.ID_VALUE_IN_SALESORDER_DATABASE]);
                        }
                        toolClass.stringSortingRecordIndex(strArray, indexArray);
                        break;
                }

                if (listView1.CheckedItems.Count != 0)
                {
                    assignedMachineByUserArray[0] = machineSelected1 - 1;
                    assignedMachineByUserArray[1] = machineSelected2 - 1;
                    assignedMachineByUserArray[2] = machineSelected3 - 1;

                    //start time assigned
                    if (checkBox2.Checked == true)
                    {
                        startTimeStamp = toolClass.ConvertDateTimeInt(dateTimePicker1.Value);
                    }
                    else
                    {
                        //if we use reversed APS, start time should not be considered, the user will make a judgment whether this APS result is acceptable
                        startTimeStamp = -1;
                    }

                    //complete time assigned
                    if (checkBox1.Checked == true)
                    {
                        //we need to use reversed APS by this end time
                        endTimeStamp = toolClass.ConvertDateTimeInt(dateTimePicker2.Value);
                    }
                    else
                    {
                        //if we use forwarding APS, end time should not be considered, the user will make a judgment whether this APS result is acceptable
                        endTimeStamp = -1;
                    }

                    //Console.WriteLine("start +" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    for (i = 0; i < indexArray.Length; i++)
                    {
                        APSProcessImpl.runAPSProcess(Convert.ToInt32(salesTableArray[indexArray[i], mySQLClass.ID_VALUE_IN_SALESORDER_DATABASE]), assignedMachineByUserArray, startTimeStamp, endTimeStamp);
                    }
                    //Console.WriteLine("end -" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                    toolClass.systemDelay(2000);
                    gVariable.APSScreenRefresh = 1;
                }
                else
                {
                    //no row selected, return directly
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("formal APS failed!" + ex);
            }
        }

        //APS result confirmation
        private void button4_Click(object sender, EventArgs e)
        {
            string commandText;

            commandText = "update `" + gVariable.globalDispatchTableName + "` set status = '-2' where status = '-3'";

            mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

            gVariable.APSScreenRefresh = 1;
        }

        //review APS result
        private void button5_Click(object sender, EventArgs e)
        {
            APSExhibit.APSExhibitClass = new APSExhibit();
            APSExhibit.APSExhibitClass.Show();
        }

        //search for sales order
        private void button6_Click(object sender, EventArgs e)
        {
        }

        //cancel the scheduled dispatches
        private void button7_Click(object sender, EventArgs e)
        {
            int i;
            int index;
            int ID;
            string updateStr;
            APSProcess APSProcessImpl = new APSProcess();

            try
            {
                if (listView1.CheckedItems.Count != 0)
                {
                    for (i = 0; i < listView1.CheckedItems.Count; i++)
                    {
                        index = listView1.CheckedItems[i].Index;
                        if (salesTableArray[index, mySQLClass.STATUS_IN_SALESORDER_DATABASE] == gVariable.SALES_ORDER_STATUS_ERP_PUBLISHED.ToString())
                        {
                            MessageBox.Show("请确认所选择的订单处于已排程状态，只有该状态的订单才可以取消排程。", "提示信息", MessageBoxButtons.OK);
                            return;
                        }
                    }

                    for (i = 0; i < listView1.CheckedItems.Count; i++)
                    {
                        index = listView1.CheckedItems[i].Index;
                        APSProcessImpl.cancelAPSProcess(Convert.ToInt32(salesTableArray[index, mySQLClass.ID_VALUE_IN_SALESORDER_DATABASE]));

                        ID = Convert.ToInt32(salesTableArray[index, mySQLClass.ID_VALUE_IN_SALESORDER_DATABASE]);
                        updateStr = "update `" + gVariable.salesOrderTableName + "` set orderStatus = '" + gVariable.SALES_ORDER_STATUS_ERP_PUBLISHED + "', planTime1 = null, planTime2 = null, APSTime = null where id = '" + ID + "'";

                        mySQLClass.updateTableItems(gVariable.globalDatabaseName, updateStr);
                    }

                    toolClass.systemDelay(2000);
                    gVariable.APSScreenRefresh = 1;
                }
                else
                {
                    //no row selected, return directly
                    MessageBox.Show("请先在工单列表左侧的复选框中选中需要取消排程的订单，然后再点选'排程结果取消'按钮。", "提示信息", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("formal APS failed!" + ex);
            }

        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {

        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker5_ValueChanged(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            gVariable.APSScreenRefresh = 2;
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //gVariable.APSScreenRefresh = 1;
        }
    }
}
