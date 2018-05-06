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

namespace MESSystem.APS_UI
{
    public partial class salesOrderList : Form
    {
        const int SALES_ORDER_NUM_MAX = 10000;

        const int SALES_ORDER_NUM_MAX_SELECTED = 50;

        const int SEPARTE_MODE_AVERAGE = 0;
        const int SEPARTE_MODE_EARLY_FULL = 1;

        int salesOrderSelected;
        int customerSelected;
        int statusSelected;

        int[] salesOrderIndexSelected = new int[SALES_ORDER_NUM_MAX];
        string[,] salesTableArray;

        System.Windows.Forms.Timer aTimer;

        float screenRatioX, screenRatioY;

        string[] salesorderStatus = { "已导入", "已排程", "已确认", "已发布", "已申请", "已开工", "已完工", "已取消" };


        struct separateCondition  
        {
            int separateMode;
            string endDate;
        };

        public salesOrderList()
        {
            InitializeComponent();
            initVariables();
            resizeScreen();
        }

        void initVariables()
        {
            int i;
            string commandText;
            string[,] tableArray;

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            gVariable.APSScreenRefresh = 1;

            label1.Text = gVariable.enterpriseTitle + "订单列表";
            this.Text = gVariable.enterpriseTitle + "订单列表";

            salesOrderSelected = -1;

            commandText = "select * from `" + gVariable.customerListTableName + "`";
            tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
            if (tableArray == null)
            {
                MessageBox.Show("基础数据中无法找到客户列表", "提示信息", MessageBoxButtons.OK);
                return;
            }
            customerSelected = 0;
            statusSelected = 0;

            comboBox4.Items.Add("全部客户");
            for (i = 0; i < tableArray.GetLength(0); i++)
            {
                comboBox4.Items.Add(tableArray[i, 2]);
            }
            comboBox4.SelectedIndex = customerSelected;

            comboBox1.Items.Add("所有状态");
            for (i = 0; i < salesorderStatus.Length; i++)
            {
                comboBox1.Items.Add(salesorderStatus[i]);
            }
            comboBox1.SelectedIndex = statusSelected;
        }

        void resizeScreen()
        {
            int i;
            int x, y, w, h;
            float fontSize;
            GroupBox[] groupBoxArray = { groupBox1, groupBox4 };
            Label[] labelArray = { label2, label3, label4, label6, label8, label9, label11, label12};
            TextBox[] textBoxArray = { textBox1, textBox2 };
            Button[] buttonArray = { button1, button2, button3, button5, button6, button7};
            ComboBox[] comboBoxArray = { comboBox1, comboBox4 };
            CheckBox[] checkBoxArray = { checkBox1, checkBox3 };
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

        private void salesOrderList_Load(object sender, EventArgs e)
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

        //sales order status
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            customerSelected = comboBox1.SelectedIndex;
        }

        //customer name
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            statusSelected = comboBox4.SelectedIndex;
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
            int[] salesOrderLenArray = { 45, 45, 100, 100, 90, 90, 120, 120, 110, 140, 75 };
            string[] salesOrderListHeader = 
            {
                "全选", "序号", "订单号", "产品编码", "产品名称", "交货日期", "需求数量", "客户名", "分拆时间", "分拆结果", "订单状态"
            };
            int[] dispatchLenArray = { 1, 45, 105, 130, 105, 80, 100, 70, 100, 120, 120, 100 };
            string[] dispatchListHeader = 
            {
                " ", "序号", "订单号", "工单号", "产品编码", "计划数量", "承接设备", "班组", "负责人", "计划开工时间", "计划完工时间", "工单状态"
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
                        //OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.ERP_TIME_IN_SALESORDER_DATABASE]);
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.DELIVERY_TIME_IN_SALESORDER_DATABASE]);
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.REQUIRED_NUM_IN_SALESORDER_DATABASE] + " " + salesTableArray[i, mySQLClass.UNIT_IN_SALESORDER_DATABASE]);
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.CUSTOMER_IN_SALESORDER_DATABASE]);
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.APS_TIME_IN_SALESORDER_DATABASE]);
                        //OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.PLANNED_START_TIME_IN_SALESORDER_DATABASE]);
                        //OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.PLANNED_COMPLETE_TIME_IN_SALESORDER_DATABASE]);
                        OptionItem.SubItems.Add(salesTableArray[i, mySQLClass.RESULT_IN_SALESORDER_DATABASE]);

                        status = Convert.ToInt16(salesTableArray[i, mySQLClass.STATUS_IN_SALESORDER_DATABASE]);
                        switch (status)
                        {
                            case gVariable.SALES_ORDER_STATUS_ERP_PUBLISHED:
                                OptionItem.SubItems.Add("已导入");
                                break;
                            case gVariable.SALES_ORDER_STATUS_SEPARATE_OK:
                                OptionItem.SubItems.Add("已拆分");
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

            }
            catch (Exception ex)
            {
                Console.Write("screen fresh for sales order list failed! " + ex);
            }
        }

        private void salesOrderList_FormClosing(object sender, EventArgs e)
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


        //search for sales order
        private void button6_Click(object sender, EventArgs e)
        {
        }

        //cancel the scheduled dispatches
        private void button7_Click(object sender, EventArgs e)
        {
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

        //complete the selected sales order
        private void button3_Click_1(object sender, EventArgs e)
        {
        
        }


        //manually generate a new sales order
        private void button1_Click(object sender, EventArgs e)
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


        //sales order progress display
        private void button4_Click(object sender, EventArgs e)
        {
        
        }

        //sales order breakdown
        private void button5_Click(object sender, EventArgs e)
        {
            int index;

            if (listView1.SelectedItems.Count > 0)
            {
                if (listView1.SelectedItems.Count > SALES_ORDER_NUM_MAX_SELECTED)
                {
                    MessageBox.Show("很抱歉，您勾选的订单数量太多，做多支持50个订单同时拆分。", "版权信息", MessageBoxButtons.OK);
                }
                
                index = Convert.ToInt32(listView1.SelectedItems[0].SubItems[0]);

            }

        }

    }
}
