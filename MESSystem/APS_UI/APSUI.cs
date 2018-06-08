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
        //if there are more than this number of product batch order, we will only process the recent this number
        const int SALES_ORDER_NUM_MAX = 10000;

        string[] productBatchPriorityRule = { "不指定", "交货日期优先", "重要客户优先", "产量大者优先" };
        string[] machinePriorityRule = { "不指定", "设备产量优先", "良品率优先" };

        const int PROIRITY_ORDER_NOT_SELECTED = 0;
        const int PROIRITY_ORDER_DELIVERY_TIME = 1;
        const int PROIRITY_ORDER_CUSTOMER_IMPORTANCE = 2;
        const int PROIRITY_ORDER_OUTPUT_QUANTITY = 3;

        int productBatchPrioritySelected;
        int machinePrioritySelected;

        int productBatchSelected;
        int MAX_NUM_SELECTED_SALES_ORDER_APS;

        float screenRatioX, screenRatioY;

        public static APSUI APSUIClass = null; //it is used to reference this windows
        //public static int [] productBatchIndexArray = new int[SALES_ORDER_NUM_MAX];
        string[,] batchTableArray; //record all the batch orders in list view
        //gVariable.dispatchSheetStruct[] dispatchListArray;

        System.Windows.Forms.Timer aTimer;

        public static APSRules.APSRulesDef [] APSRulesArray = new APSRules.APSRulesDef[gVariable.MAX_NUM_SELECTED_SALES_ORDER_APS];

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
            MAX_NUM_SELECTED_SALES_ORDER_APS = gVariable.MAX_NUM_SELECTED_SALES_ORDER_APS;

            label1.Text = gVariable.enterpriseTitle + "生产计划排程系统";
            this.Text = gVariable.enterpriseTitle + "生产计划排程系统";

            productBatchSelected = -1;
            gVariable.numOfBatchDefinedAPSRule = 0;
            gVariable.indexOfBatchDefinedAPSRule = 0;

            for (i = 0; i < productBatchPriorityRule.Length; i++)
            {
                comboBox3.Items.Add(productBatchPriorityRule[i]);
            }
            comboBox3.SelectedIndex = productBatchPrioritySelected;
            
            for (i = 0; i < machinePriorityRule.Length; i++)
            {
                comboBox6.Items.Add(machinePriorityRule[i]);
            }
            comboBox6.SelectedIndex = machinePrioritySelected;

            //we can set APS rule for 50 batch orders for onw time
            for(i = 0; i < gVariable.MAX_NUM_SELECTED_SALES_ORDER_APS; i++)
            {
                APSRulesArray[i] = new APSRules.APSRulesDef();
                //APSRulesArray[i].ruleAlreadyDefined = 0;
                APSRulesArray[i].listviewIndex = -1;
                APSRulesArray[i].assignedStartTime = -1;
                APSRulesArray[i].assignedEndTime = -1;
                APSRulesArray[i].assignedMachineID1 = 0;
                APSRulesArray[i].assignedMachineID2 = 0;
                APSRulesArray[i].assignedMachineID3 = 0;
                APSRulesArray[i].BOMName = null;
                APSRulesArray[i].materialCode = new string[gVariable.maxMaterialTypeNum];
                APSRulesArray[i].materialSelected = new int[gVariable.maxMaterialTypeNum];
            }
        }

        void resizeScreen()
        {
            int i;
            int x, y, w, h;
            float fontSize;
            GroupBox[] groupBoxArray = { groupBox1, groupBox3, groupBox4 };
            Label[] labelArray = { label6, label7, label8, label9, label10, label11, label12};
            TextBox[] textBoxArray = { textBox1, textBox2 };
            Button[] buttonArray = { button1, button2, button3, button4, button5, button6, button7, button8, button9, button10 };
            ComboBox[] comboBoxArray = { comboBox3, comboBox4, comboBox6 };
            //CheckBox[] checkBoxArray = { checkBox3 };
            DateTimePicker[] timePickerArray = { dateTimePicker3, dateTimePicker4 };
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

            /*
            for (i = 0; i < checkBoxArray.Length; i++)
            {
                w = (int)(checkBoxArray[i].Size.Width * screenRatioX);
                h = (int)(checkBoxArray[i].Size.Height * screenRatioY);
                checkBoxArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(checkBoxArray[i].Location.X * screenRatioX);
                y = (int)(checkBoxArray[i].Location.Y * screenRatioY);
                checkBoxArray[i].Location = new System.Drawing.Point(x, y);
            }
             * */

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

        //product batch order priority selection
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            productBatchPrioritySelected = comboBox3.SelectedIndex;
        }

        //machine priority selection
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            machinePrioritySelected = comboBox6.SelectedIndex;
        }

        private void loadScreen()
        {
            int i, j, k;
            //int ret;
            //int length;
            int num;
            //int min;
            int status;
            int index1;
            //int index2;
            //int selectionChanged;
            string commandText;
            ListViewItem OptionItem;
            //string[] salesOrderBatchCodeArray = new string[SALES_ORDER_NUM_MAX];
            //gVariable.productBatchStruct productBatchImpl = new gVariable.productBatchStruct();
            int[] productBatchLenArray = { 40, 45, 85, 100, 100, 90, 80, 110, 120, 120, 120, 75 };
            string[] productBatchListHeader = 
            {
                " ", "序号", "生产批次号", "订单批次号", "产品编码", "交货日期", "需求数量", "客户名", "排产时间", "计划开工", "计划完工", "订单状态"
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
                //selectionChanged = 0;
                if (listView1.SelectedItems.Count != 0)
                {
                    //Console.WriteLine("productBatchSelected = " + productBatchSelected + "; index = " + listView1.SelectedItems[0].Index);

                    if (productBatchSelected != listView1.SelectedItems[0].Index)
                    {
                        productBatchSelected = listView1.SelectedItems[0].Index;
                        //selectionChanged = 1;
                    }
                }

                index1 = 0;
                //index2 = 0;

                if (listView1.TopItem != null)
                    index1 = listView1.TopItem.Index;

                //if (listView2.TopItem != null)
                //    index2 = listView2.TopItem.Index;

                num = mySQLClass.getRecordNumInTable(gVariable.globalDatabaseName, gVariable.productBatchTableName);
                if (num > SALES_ORDER_NUM_MAX)
                    num = SALES_ORDER_NUM_MAX;

                commandText = "select * from `" + gVariable.productBatchTableName + "` order by id desc";
                batchTableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (batchTableArray == null)
                    num = 0;

                //for (i = 0, j = 0; i < num; i++, j++)
                {
                    //productBatchIndexArray[i] = Convert.ToInt32(batchTableArray[i, mySQLClass.ID_VALUE_IN_BATCHNUM_DATABASE]) - 1;
                    //salesOrderBatchCodeArray[i] = batchTableArray[i, mySQLClass.ORDER_CODE_IN_BATCHNUM_DATABASE];
                }

                if (gVariable.APSScreenRefresh == 1)
                {
                    listView1.Clear();

                    listView1.BeginUpdate();

                    listView1.GridLines = true;
                    listView1.Dock = DockStyle.Fill;

                    for (i = 0; i < productBatchLenArray.Length; i++)
                        listView1.Columns.Add(productBatchListHeader[i], (int)(productBatchLenArray[i] * screenRatioX), HorizontalAlignment.Center);

                    for (i = 0, j = 0; i < num; i++, j++)
                    {
                        OptionItem = new ListViewItem();

                        OptionItem.SubItems.Add((i + 1).ToString());
                        OptionItem.SubItems.Add(batchTableArray[i, mySQLClass.BATCH_NUM_IN_BATCHNUM_DATABASE]);
                        OptionItem.SubItems.Add(batchTableArray[i, mySQLClass.BATCH_CODE_IN_BATCHNUM_DATABASE]);
                        OptionItem.SubItems.Add(batchTableArray[i, mySQLClass.PRODUCT_CODE_IN_BATCHNUM_DATABASE]);
                        //OptionItem.SubItems.Add(batchTableArray[i, mySQLClass.PRODUCT_NAME_IN_BATCHNUM_DATABASE]);
                        //OptionItem.SubItems.Add(batchTableArray[i, mySQLClass.ERP_TIME_IN_BATCHNUM_DATABASE]);
                        OptionItem.SubItems.Add(batchTableArray[i, mySQLClass.DELIVERY_TIME_IN_BATCHNUM_DATABASE]);
                        OptionItem.SubItems.Add(batchTableArray[i, mySQLClass.REQUIRED_NUM_IN_BATCHNUM_DATABASE] + " kg");
                        OptionItem.SubItems.Add(batchTableArray[i, mySQLClass.CUSTOMER_IN_BATCHNUM_DATABASE]);
                        OptionItem.SubItems.Add(batchTableArray[i, mySQLClass.APS_TIME_IN_BATCHNUM_DATABASE]);
                        OptionItem.SubItems.Add(batchTableArray[i, mySQLClass.PLANNED_START_TIME_IN_BATCHNUM_DATABASE]);
                        OptionItem.SubItems.Add(batchTableArray[i, mySQLClass.PLANNED_COMPLETE_TIME_IN_BATCHNUM_DATABASE]);

                        status = Convert.ToInt16(batchTableArray[i, mySQLClass.STATUS_IN_BATCHNUM_DATABASE]);

                        if (status >= gVariable.SALES_ORDER_STATUS_CONFIRMED)
                            OptionItem.SubItems.Add("已发布");
                        else
                            OptionItem.SubItems.Add(gVariable.salesorderStatus[status]);
                        listView1.Items.Add(OptionItem);
                    }

                    if (productBatchSelected >= 0)
                        this.listView1.Items[productBatchSelected].Selected = true;

                    if (listView1.TopItem != null && listView1.Items.Count > index1 + listviewLineNum1[gVariable.dpiValue, gVariable.resolutionLevel] - 1)
                        listView1.EnsureVisible(index1 + listviewLineNum1[gVariable.dpiValue, gVariable.resolutionLevel] - 1);

                    listView1.EndUpdate();
                }

                listView1.BeginUpdate();
                for (i = 0; i < num; i++)
                {
                    if (gVariable.numOfBatchDefinedAPSRule != 0)
                    {
                        for (k = 0; k < gVariable.numOfBatchDefinedAPSRule; k++)
                        {
                            if (APSUI.APSRulesArray[k].listviewIndex == i)
                            {
                                listView1.Items[i].BackColor = Color.Yellow;
                                break;
                            }
                        }
                    }

                    status = Convert.ToInt16(batchTableArray[i, mySQLClass.STATUS_IN_BATCHNUM_DATABASE]);
                    if (status >= gVariable.SALES_ORDER_STATUS_APS_OK)
                    {
                        listView1.Items[i].BackColor = Color.LightGreen;
                    }
                }
                listView1.EndUpdate();

                /*
                listView2.Clear();
                listView2.BeginUpdate();

                dispatchListArray = null;
                if (productBatchSelected >= 0)
                {
                    commandText = " where batchNum = " + "'" + batchTableArray[productBatchSelected, mySQLClass.ORDER_CODE_IN_BATCHNUM_DATABASE] + "'";
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
                    OptionItem.SubItems.Add(dispatchListArray[i].batchNum);
                    OptionItem.SubItems.Add(dispatchListArray[i].dispatchCode);
                    OptionItem.SubItems.Add(dispatchListArray[i].productCode);
                    //OptionItem.SubItems.Add(dispatchListArray[i].productName);
                    OptionItem.SubItems.Add(dispatchListArray[i].plannedNumber.ToString());
                    OptionItem.SubItems.Add(gVariable.machineNameArrayAPS[Convert.ToInt16(dispatchListArray[i].machineID) - 1]);
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
                {
                    listView2.EnsureVisible(index2 + listviewLineNum2[gVariable.resolutionLevel] - 2);
                }
                else
                {
                }

                listView2.EndUpdate();
                 */

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

            //if we selected a product batch order then add a new one, new batch order will have the content of the selected batch order, then the user can modify the content
            if (this.listView1.SelectedItems.Count != 0)
                index = Convert.ToInt32(batchTableArray[listView1.SelectedItems[0].Index, mySQLClass.ID_VALUE_IN_SALESORDER_DATABASE]);
            else
                index = -1;

            manualSalesOrder.manualSalesOrderImpl = new manualSalesOrder(index);
            manualSalesOrder.manualSalesOrderImpl.Show();

            toolClass.nonBlockingDelay(2000);
        }


        //formal APS
        private void button3_Click(object sender, EventArgs e)
        {
            int i, j;
            int index;
            int startTimeStamp;
            int endTimeStamp;
            //int[] dispatchIndexList = new int[SALES_ORDER_NUM_MAX];
            int[] assignedMachineByUserArray = new int[3];
            string commandText;
            string companyName;

            int[] indexArray;  //all the selected batch order index in batchTableArray[], all the batch orders in listview
            int[] batchOrderIDArray;  //all the selected batch order iD in batch order database table
            string[] conditionArray;  //APS priority conditions, could be delivery time prioritized or customer importance prioritized, we use this condition to sort for a reasonable indexArray
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

                if (listView1.CheckedItems.Count > MAX_NUM_SELECTED_SALES_ORDER_APS)
                {
                    MessageBox.Show("抱歉，本系统最多支持一次选择 " + MAX_NUM_SELECTED_SALES_ORDER_APS + " 个订单进行排产。", "提示信息", MessageBoxButtons.OK);
                    return;
                }

                for (i = 0; i < listView1.CheckedItems.Count; i++)
                {
                    index = listView1.CheckedItems[i].Index;
                    if (batchTableArray[index, mySQLClass.STATUS_IN_BATCHNUM_DATABASE] != gVariable.SALES_ORDER_STATUS_SEPARATE_OK.ToString())
                    {
                        MessageBox.Show("请确认所选择的订单处于已导入状态，因为已排程的订单不能再次排程。若确实需要重新排程，请先取消该订单的排程结果后再尝试。", "提示信息", MessageBoxButtons.OK);
                        //  listView1.SelectedItems = index;
                        return;
                    }
                }

                indexArray = new int[listView1.CheckedItems.Count];
                conditionArray = new string[listView1.CheckedItems.Count];
                batchOrderIDArray = new int[listView1.CheckedItems.Count];

                //get product batch order list for APS
                switch (productBatchPrioritySelected)
                {
                    case PROIRITY_ORDER_NOT_SELECTED: //do APS by receive order
                        for (i = 0; i < listView1.CheckedItems.Count; i++)
                        {
                            indexArray[i] = listView1.CheckedItems[i].Index;
                        }
                        break;
                    case PROIRITY_ORDER_CUSTOMER_IMPORTANCE:  //do APS by the order of the importance of the customer company
                        //put batch order info in array and then do sorting by the importance of customer company
                        for (i = 0; i < listView1.CheckedItems.Count; i++)
                        {
                            index = listView1.CheckedItems[i].Index;
                            companyName = batchTableArray[index, mySQLClass.CUSTOMER_IN_BATCHNUM_DATABASE];

                            commandText = "select * from `" + gVariable.customerListTableName + "` where customerName = '" + companyName + "'";
                            tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                            if (tableArray == null)
                            {
                                MessageBox.Show("基础数据中无法找到客户名称：" + companyName + "，请确认订单信息", "提示信息", MessageBoxButtons.OK);
                                return;
                            }
                            conditionArray[i] = tableArray[0, 3];
                            indexArray[i] = Convert.ToInt32(batchTableArray[index, mySQLClass.ID_VALUE_IN_BATCHNUM_DATABASE]);
                        }
                        toolClass.stringSortingRecordIndex(conditionArray, indexArray);
                        break;
                    case PROIRITY_ORDER_OUTPUT_QUANTITY:  //do APS by the order of the output quantity, function not support at this time
                    case PROIRITY_ORDER_DELIVERY_TIME: //do APS by delivery time order
                    default:
                        //put batch order info in array and then do sorting by delivery time
                        for (i = 0; i < listView1.CheckedItems.Count; i++)
                        {
                            index = listView1.CheckedItems[i].Index;
                            conditionArray[i] = batchTableArray[index, mySQLClass.DELIVERY_TIME_IN_BATCHNUM_DATABASE];
                            indexArray[i] = Convert.ToInt32(batchTableArray[index, mySQLClass.ID_VALUE_IN_BATCHNUM_DATABASE]);
                        }
                        toolClass.stringSortingRecordIndex(conditionArray, indexArray);
                        break;
                }

                if (listView1.CheckedItems.Count != 0)
                {
                    //Console.WriteLine("start +" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    for (i = 0; i < indexArray.Length; i++)
                    {
                        for(j = 0; j < gVariable.numOfBatchDefinedAPSRule; j++)
                        {
                            if(indexArray[i] == APSUI.APSRulesArray[j].listviewIndex)
                                break;
                        }

                        //get batch order index in rules table
                        if(j != 0 && j >= gVariable.numOfBatchDefinedAPSRule)
                        {
                            Console.WriteLine("Formal APS. failed to get correct batch order index");
                            return;
                        }

                        if (APSRulesArray[j].assignedStartTime == -1 && APSRulesArray[j].assignedEndTime == -1)
                        {
                            startTimeStamp = toolClass.ConvertDateTimeInt(DateTime.Now);
                            endTimeStamp = -1;
                        }
                        else //if (APSRulesArray.assignedStartTime == -1)
                        {
                            startTimeStamp = APSRulesArray[j].assignedStartTime;
                            endTimeStamp = APSRulesArray[j].assignedEndTime;
                        }

                        //APSRulesArray[j].assignedMachineID1 is 0 means not assigned, so the assignedMachineByUserArray[] value should be -1 
                        if (APSRulesArray[j].assignedMachineID1 == 0)
                            assignedMachineByUserArray[0] = -1;
                        else
                        {
                            assignedMachineByUserArray[0] = APSRulesArray[j].assignedMachineID1 - 1 + gVariable.castingProcess[0];
                        }

                        if (APSRulesArray[j].assignedMachineID2 == 0)
                            assignedMachineByUserArray[1] = -1;
                        else
                        {
                            assignedMachineByUserArray[1] = APSRulesArray[j].assignedMachineID2 - 1 + gVariable.printingProcess[0];
                        }

                        if (APSRulesArray[j].assignedMachineID3 == 0)
                            assignedMachineByUserArray[2] = -1;
                        else
                        {
                            assignedMachineByUserArray[2] = APSRulesArray[j].assignedMachineID3 - 1 + gVariable.slittingProcess[0];
                        }

                        APSProcessImpl.runAPSProcess(Convert.ToInt32(batchTableArray[indexArray[i], mySQLClass.ID_VALUE_IN_BATCHNUM_DATABASE]), assignedMachineByUserArray, startTimeStamp, endTimeStamp);
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
            DialogResult result;

            result = MessageBox.Show("确认把当前已排程的任务单下发到生产管理部门吗？", "提示信息", MessageBoxButtons.OKCancel);
            if (result != DialogResult.OK)
            {
                return;
            }

            commandText = "update `" + gVariable.globalDispatchTableName + "` set status = '-2' where status = '-3'";
            mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

            commandText = "update `" + gVariable.productBatchTableName + "` set orderStatus = '" + gVariable.SALES_ORDER_STATUS_CONFIRMED + "' where orderStatus = '" + gVariable.SALES_ORDER_STATUS_APS_OK + "'";
            mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

            commandText = "update `" + gVariable.salesOrderTableName + "` set orderStatus = '" + gVariable.SALES_ORDER_STATUS_CONFIRMED + "' where orderStatus = '" + gVariable.SALES_ORDER_STATUS_APS_OK + "'";
            mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

            gVariable.APSScreenRefresh = 1;
        }

        //review APS result
        private void button5_Click(object sender, EventArgs e)
        {
            APSExhibit.APSExhibitClass = new APSExhibit();
            APSExhibit.APSExhibitClass.Show();
        }

        //search for batch order
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
            DialogResult result;
            APSProcess APSProcessImpl = new APSProcess();

            try
            {
                if (listView1.CheckedItems.Count != 0)
                {
                    for (i = 0; i < listView1.CheckedItems.Count; i++)
                    {
                        index = listView1.CheckedItems[i].Index;
                        if (batchTableArray[index, mySQLClass.STATUS_IN_BATCHNUM_DATABASE] == gVariable.SALES_ORDER_STATUS_ERP_PUBLISHED.ToString())
                        {
                            MessageBox.Show("请确认所选择的订单处于已排程状态，只有该状态的订单才可以取消排程。", "提示信息", MessageBoxButtons.OK);
                            return;
                        }
                    }

                    result = MessageBox.Show("确认要取消所选订单的排程结果吗？", "提示信息", MessageBoxButtons.OKCancel);
                    if (result != DialogResult.OK)
                    {
                        return;
                    }


                    for (i = 0; i < listView1.CheckedItems.Count; i++)
                    {
                        index = listView1.CheckedItems[i].Index;
                        APSProcessImpl.cancelAPSProcess(Convert.ToInt32(batchTableArray[index, mySQLClass.ID_VALUE_IN_BATCHNUM_DATABASE]));

                        ID = Convert.ToInt32(batchTableArray[index, mySQLClass.ID_VALUE_IN_BATCHNUM_DATABASE]);
                        updateStr = "update `" + gVariable.productBatchTableName + "` set orderStatus = '" + gVariable.SALES_ORDER_STATUS_SEPARATE_OK + "', planTime1 = null, planTime2 = null, APSTime = null where id = '" + ID + "'";

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
            /*
            int index;
            int status;
            string batchNum;

            gVariable.APSScreenRefresh = 2;

            try
            {
                if (this.listView1.SelectedItems.Count == 1)
                {
                    index = Convert.ToInt32(this.listView1.SelectedItems[0].SubItems[1].Text) - 1;
                    status = Convert.ToInt16(batchTableArray[index, mySQLClass.STATUS_IN_BATCHNUM_DATABASE]);
                    if (status >= gVariable.SALES_ORDER_STATUS_APS_OK)
                    {
                        batchNum = this.listView1.SelectedItems[0].SubItems[2].Text;
                        APSBatchResult APSBatchResultImpl = new APSBatchResult(batchNum);
                        APSBatchResultImpl.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("listView1_SelectedIndexChanged for APS UI failed. ");
                Console.WriteLine(ex.ToString());
                return;
            }
             */
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //gVariable.APSScreenRefresh = 1;
        }

        //insert an order 
        private void button9_Click(object sender, EventArgs e)
        {

        }


        //review machine loading
        private void button8_Click(object sender, EventArgs e)
        {
            adjustLoading adjustLoadingImpl = new adjustLoading();
            adjustLoadingImpl.Show();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        //APS rule define for a batch order
        private void button10_Click(object sender, EventArgs e)
        {
            int i;
            int selectedBatchOrderID;

            if (this.listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先反白选中一个订单批次号，然后再点选'排产规则设定'按钮开始针对该订单的排程规则设定。", "提示信息", MessageBoxButtons.OK);
                return;
            }

            for (i = 0; i < gVariable.numOfBatchDefinedAPSRule; i++)
            {
                if (listView1.SelectedItems[0].Index == APSUI.APSRulesArray[i].listviewIndex)
                {
                    gVariable.indexOfBatchDefinedAPSRule = i;
                    break;
                }
            }

            if (i >= gVariable.numOfBatchDefinedAPSRule)
                gVariable.indexOfBatchDefinedAPSRule = gVariable.numOfBatchDefinedAPSRule;

            selectedBatchOrderID = Convert.ToInt32(batchTableArray[listView1.SelectedItems[0].Index, 0]);
            APSRules APSRulesImpl = new APSRules(selectedBatchOrderID, listView1.SelectedItems[0].Index);
            APSRulesImpl.Show();
            gVariable.APSScreenRefresh = 1;
        }

        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index;
            int status;
            string batchNum;

            gVariable.APSScreenRefresh = 2;

            try
            {
                if (this.listView1.SelectedItems.Count == 1)
                {
                    index = Convert.ToInt32(this.listView1.SelectedItems[0].SubItems[1].Text) - 1;
                    status = Convert.ToInt16(batchTableArray[index, mySQLClass.STATUS_IN_BATCHNUM_DATABASE]);
                    if (status >= gVariable.SALES_ORDER_STATUS_APS_OK)
                    {
                        batchNum = this.listView1.SelectedItems[0].SubItems[2].Text;
                        APSBatchResult APSBatchResultImpl = new APSBatchResult(batchNum);
                        APSBatchResultImpl.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("listView1_SelectedIndexChanged for APS UI failed. ");
                Console.WriteLine(ex.ToString());
                return;
            }
        }

        //multi-product setting
        private void button9_Click_1(object sender, EventArgs e)
        {
            int status;
            string productCode;
            string productBatchCode;

            if (this.listView1.SelectedItems.Count == 1)
            {
                status = Convert.ToInt32(batchTableArray[listView1.SelectedItems[0].Index, mySQLClass.STATUS_IN_BATCHNUM_DATABASE]);
                if (status >= gVariable.SALES_ORDER_STATUS_APS_OK)
                {
                    MessageBox.Show("排产结束后就不能再设定套作信息了，请先取消该订单批次的排产结果。", "提示信息", MessageBoxButtons.OK);
                    return;
                }

                productCode = batchTableArray[listView1.SelectedItems[0].Index, mySQLClass.PRODUCT_CODE_IN_BATCHNUM_DATABASE];
                productBatchCode = batchTableArray[listView1.SelectedItems[0].Index, mySQLClass.BATCH_CODE_IN_BATCHNUM_DATABASE];
                multiProducts multiProductsImpl = new multiProducts(productCode, productBatchCode);
                multiProductsImpl.Show();
            }
        }
    }
}
