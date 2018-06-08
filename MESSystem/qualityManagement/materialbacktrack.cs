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

namespace MESSystem.quality
{
    public partial class materialBacktrack : Form
    {
        const int SALES_ORDER_NUM_MAX = 5000;

        const int MATERIAL_OUT_PROCESS = 0;
        const int MATERIAL_FEED_PROCESS = 1;
        const int CASTING_PROCESS = 2;
        const int PRINTING_PROCESS = 3;
        const int SLITTING_PROCESS = 4;
        const int INSPECTION_PROCESS = 5;
        const int PACKING_PROCESS = 6;

        public static materialBacktrack materialBacktrackClass = null; //it is used to reference this windows

        int screenRefresh;
        float screenRatioX, screenRatioY;

        string materialBatchNum;

        int salesOrderSelected;
        int salesTableLength;
        int[] salesIndexArray;
        string[,] salesTableArray;

        int dispatchSelected;
        gVariable.dispatchSheetStruct[] dispatchListArray;

        SolidBrush colorGrayBrush = new SolidBrush(Color.DarkGray);  //machine turned off   
        SolidBrush colorGreenBrush = new SolidBrush(Color.LightGreen);  //machine working

        float[,] commonFontSize = { 
                                        { 8.4F,  9.6F,  10.8F, 12F,   13.2F,  14.4F}, 
                                        { 7.2F,  7.8F,  9.6F,  10.2F, 10.8F,  12F},  
                                        { 6.6F,  7.2F,  7.8F,  8.4F,  9.0F,   9.6F},  
                                     };

        string[] inspectionError = { "厚度不均", "孔洞", "晶点", "虫洞", "印刷不良", "不合格" };

        //string feedBarcode;
        //string[,] tableArrayFeeding;

        //string slitBarcode;
        string[,] tableArraySlitting;

        //string printBarcode;
        string[,] tableArrayPrinting;

        //string castBarcode;
        string[,] tableArrayCasting;

        //string inspectionBarcode;
        string[,] tableArrayInspection;

        //string packBarcode;
        string[,] tableArrayPacking;

        System.Windows.Forms.Timer aTimer;

        public materialBacktrack(string materialBatchNum_)
        {
            materialBatchNum = materialBatchNum_;

            InitializeComponent();
            initializeVariables();
            resizeScreen();
        }

        void initializeVariables()
        {
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
            label1.Text = gVariable.enterpriseTitle + "物料追溯系统";
            this.Text = gVariable.enterpriseTitle + "物料追溯系统";

            groupBox5.Text = "物料 " + materialBatchNum + " 参与生产的订单";
            groupBox2.Text = "选中订单的工单列表";
            groupBox1.Text = "选中工单的生产流程列表";

            screenRefresh = 1;
        }

        void resizeScreen()
        {
            int i;
            int x, y, w, h;
            float fontSize;
            GroupBox[] groupBoxArray = { groupBox1, groupBox2, groupBox5 };
            ListView[] listViewArray = { listView1, listView2 };

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

            for (i = 0; i < listViewArray.Length; i++)
            {
                listViewArray[i].Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                w = (int)(listViewArray[i].Size.Width * screenRatioX);
                h = (int)(listViewArray[i].Size.Height * screenRatioY);
                listViewArray[i].Size = new System.Drawing.Size(w, h);
                x = (int)(listViewArray[i].Location.X * screenRatioX);
                y = (int)(listViewArray[i].Location.Y * screenRatioY);
                listViewArray[i].Location = new System.Drawing.Point(x, y);
            }
        }

        private void backtrack_Load(object sender, EventArgs e)
        {
            aTimer = new System.Windows.Forms.Timer();

            getSalesOrderByMaterial(materialBatchNum);

            //refresh screen every 2 seconds
            aTimer.Interval = 2000;
            aTimer.Enabled = true;

            aTimer.Tick += new EventHandler(timer_updateForm);
            loadScreen();
        }

        private void timer_updateForm(Object source, EventArgs e)
        {
            if (screenRefresh != 0)
                loadScreen();
            screenRefresh = 0;
        }

        private void loadScreen()
        {
            if (screenRefresh == 1)
                displaySalesOrderAndDispatch();

            displayProductionProcess();
        }

        int getSalesOrderByMaterial(string materialCode)
        {
            int i, j, k, l;
            int num;
            string commandText;
            string[,] tableArray;
            string[] productListArray;

            try
            {
                //get product spec, so we know what kind of product will use this material.  
                //tableArray is a product table which contains records for all the products
                commandText = "select * from `" + gVariable.productTableName + "`";
                tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (tableArray == null)
                    return -1;

                //product array that will contains product code(this product contains the input material)  
                productListArray = new string[tableArray.GetLength(0)];

                k = 0;
                l = 0;
                for(i = 0; i < tableArray.GetLength(0); i++)
                {
                    num = Convert.ToInt32(tableArray[i, 15]);
                    for(j = 0; j < num; j++)
                    {
                        if(tableArray[i, 16 + j] == materialCode)
                        {
                            //this product has the material Code, so save it in productListArray
                            productListArray[k++] = tableArray[i, 2];
                            break;
                        }
                    }
                }

                num = mySQLClass.getRecordNumInTable(gVariable.globalDatabaseName, gVariable.salesOrderTableName);
                if (num > SALES_ORDER_NUM_MAX)
                    num = SALES_ORDER_NUM_MAX;

                commandText = "select * from `" + gVariable.salesOrderTableName + "` order by id desc";
                salesTableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (salesTableArray == null)
                    num = 0;

                salesIndexArray = new int[salesTableArray.GetLength(0)];

                for (i = 0; i < num; i++ )
                {
                    for (j = 0; j < k; j++)
                    {
                        //whether this sales order has the same product code as stored in productListArray[] 
                        if (salesTableArray[i, 3] == productListArray[j])
                        {
                            //this product has the material Code, so save it in productListArray
                            salesIndexArray[l++] = i;
                            break;
                        }
                    }
                }

                salesTableLength = l;

                return 0;
            }
            catch (Exception ex)
            {
                Console.Write("displaySalesOrderAndDispatch failed!" + ex);
                return -1;
            }
        }

        void displaySalesOrderAndDispatch()
        {
            int i, j;
            int length;
            int status;
            int index1;
            int index2;
            int selectionChanged;
            string commandText;
            ListViewItem OptionItem;
            int[] salesOrderLenArray = { 1, 45, 100, 100, 145, 90, 90, 90, 120, 120, 120, 120, 75 };
            string[] salesOrderListHeader = 
            {
                " ", "序号", "订单号", "产品编码", "产品名称", "接单日期", "交货日期", "需求数量", "客户名", "排产时间", "计划开工", "计划完工", "订单状态"
            };
            int[] dispatchLenArray = { 1, 45, 105, 130, 105, 146, 80, 100, 70, 100, 120, 120, 100 };
            string[] dispatchListHeader = 
            {
                " ", "序号", "订单号", "工单号", "产品编码", "产品名称", "计划数量", "承接设备", "班组", "负责人", "计划开工时间", "计划完工时间", "工单状态"
            };
            int[,] listviewLineNum1 = { 
                                         {7, 7, 7, 7, 8, 8},
                                         {7, 7, 7, 7, 8, 8},
                                         {7, 7, 7, 7, 8, 8}
                                     };
            int[] listviewLineNum2 = { 7, 7, 7, 7, 8, 8 };

            try
            {
                selectionChanged = 0;
                salesOrderSelected = -1;
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

                listView1.Clear();

                this.listView1.BeginUpdate();

                listView1.GridLines = true;
                listView1.Dock = DockStyle.Fill;

                for (i = 0; i < salesOrderLenArray.Length; i++)
                    listView1.Columns.Add(salesOrderListHeader[i], (int)(salesOrderLenArray[i] * screenRatioX), HorizontalAlignment.Center);

                for (i = 0, j = 0; i < salesTableLength; i++, j++)
                {
                    OptionItem = new ListViewItem();

                    OptionItem.SubItems.Add((i + 1).ToString());
                    OptionItem.SubItems.Add(salesTableArray[salesIndexArray[i], mySQLClass.ORDER_CODE_IN_SALESORDER_DATABASE]);
                    OptionItem.SubItems.Add(salesTableArray[salesIndexArray[i], mySQLClass.PRODUCT_CODE_IN_SALESORDER_DATABASE]);
                    OptionItem.SubItems.Add(salesTableArray[salesIndexArray[i], mySQLClass.PRODUCT_NAME_IN_SALESORDER_DATABASE]);
                    OptionItem.SubItems.Add(salesTableArray[salesIndexArray[i], mySQLClass.ERP_TIME_IN_SALESORDER_DATABASE]);
                    OptionItem.SubItems.Add(salesTableArray[salesIndexArray[i], mySQLClass.DELIVERY_TIME_IN_SALESORDER_DATABASE]);
                    OptionItem.SubItems.Add(salesTableArray[salesIndexArray[i], mySQLClass.REQUIRED_NUM_IN_SALESORDER_DATABASE] + " " + salesTableArray[i, mySQLClass.UNIT_IN_SALESORDER_DATABASE]);
                    OptionItem.SubItems.Add(salesTableArray[salesIndexArray[i], mySQLClass.CUSTOMER_IN_SALESORDER_DATABASE]);
                    OptionItem.SubItems.Add(salesTableArray[salesIndexArray[i], mySQLClass.APS_TIME_IN_SALESORDER_DATABASE]);
                    OptionItem.SubItems.Add(salesTableArray[salesIndexArray[i], mySQLClass.PLANNED_START_TIME_IN_SALESORDER_DATABASE]);
                    OptionItem.SubItems.Add(salesTableArray[salesIndexArray[i], mySQLClass.PLANNED_COMPLETE_TIME_IN_SALESORDER_DATABASE]);

                    status = Convert.ToInt16(salesTableArray[salesIndexArray[i], mySQLClass.STATUS_IN_SALESORDER_DATABASE]);
                    OptionItem.SubItems.Add(gVariable.salesorderStatus[status]);

                    listView1.Items.Add(OptionItem);
                }
                if (salesOrderSelected >= 0)
                    listView1.Items[salesOrderSelected].Selected = true;

                if (listView1.TopItem != null && listView1.Items.Count > index1 + listviewLineNum1[gVariable.dpiValue, gVariable.resolutionLevel] - 1)
                    listView1.EnsureVisible(index1 + listviewLineNum1[gVariable.dpiValue, gVariable.resolutionLevel] - 1);

                listView1.EndUpdate();

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
                    OptionItem.SubItems.Add(gVariable.machineNameArrayDatabase[Convert.ToInt16(dispatchListArray[i].machineID) - 1]);
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
                Console.Write("displaySalesOrderAndDispatch failed!" + ex);
            }
        }

        void displayProductionProcess()
        {
            int i, j;
            int[] x = {10, 0,    0,   0,   0,   0,   0,   0,   0,   0};
            int[] w = {80, 120,  68,  80,  50,  90,  68,  255, 80,  250}; 
            int y0 = 1;
            int h0 = 22;
            int y1 = 5;
            int gap_x = 10;
            int gapy = 23;
            //int selectionChanged;
            Font font;
            float fontSize;
            string dispatchCode;
            string dispatchCodeT;
            string dispatchCodeL;
            string dispatchCodeY;
            string dispatchCodeF;
            string indexStr;
            string commandText;
            string databaseName;
            string[,] tableArray;
            string[,] processData = new string[7, 5]; 
            string[,] processList = 
            { 
                {"出库时间", "责任人", "设备", "",       ""},
                {"投料时间", "责任人", "设备", "料仓号", ""},
                {"流延时间", "责任人", "设备", "大卷号", ""},
                {"印刷时间", "责任人", "设备", "大卷号", "油墨配比"},
                {"分切时间", "责任人", "设备", "小卷号", "接头数量"},
                {"质检时间", "责任人", "设备", "小卷号", "质检结果"},
                {"打包时间", "责任人", "设备", "大标签", ""  },
            };
            Graphics e = panel1.CreateGraphics();

            try
            {
                //selectionChanged = 0;
                dispatchSelected = -1;

                //no dispatch selected, don't show anything
                if (listView2.SelectedItems.Count == 0)
                {
                    return;
                }

                //Console.WriteLine("dispatchSelected = " + dispatchSelected + "; index = " + listView2.SelectedItems[0].Index);
                if (dispatchSelected != listView2.SelectedItems[0].Index)
                {
                    salesOrderSelected = listView2.SelectedItems[0].Index;
                    //selectionChanged = 1;
                }

                dispatchCode = dispatchListArray[salesOrderSelected].dispatchCode;
                dispatchCode = dispatchCode.Remove(9, 1);

                indexStr = dispatchCode.Substring(10, 1);
                databaseName = gVariable.DBHeadString + indexStr.PadLeft(3, '0');

                dispatchCodeT = dispatchCode.Insert(9, "T");
                dispatchCodeL = dispatchCode.Insert(9, "L");
                dispatchCodeY = dispatchCode.Insert(9, "P");
                dispatchCodeF = dispatchCode.Insert(9, "S");

                //feed material
                //find a feed record with this cast dispatch code in material in/out record table
                commandText = "select * from `" + gVariable.materialDeliveryTableName + "` where dispatchCode = '" + dispatchCodeT + "'";
                tableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (tableArray != null)
                {
                    processData[1, 0] = tableArray[0, 10];  //feeding time
                    processData[1, 1] = toolClass.getNameByIDAndIDByName(null, tableArray[0, 11]);  //feeder
                    processData[1, 2] = gVariable.machineNameArrayDatabase[Convert.ToInt32(tableArray[0, 6])];
                    processData[1, 3] = tableArray[0, 7] + "号料仓";
                    processData[1, 4] = "";

                    //material output from warehouse
                    //find previous output record from warehouse with the same material code and machine name and feedbin index
                    commandText = "select * from `" + gVariable.materialDeliveryTableName + "` where id < '" + tableArray[0, 0] + "' and materialCode = '" + tableArray[0, 1] +
                                  "' and targetMachine = '" + tableArray[0, 6] + "' and feedbinindex = '" + tableArray[0, 7] + "' and direction = '1' order by id DESC";
                    tableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                    if (tableArray != null)
                    {
                        processData[0, 0] = tableArray[0, 4];
                        processData[0, 1] = toolClass.getNameByIDAndIDByName(null, tableArray[0, 8]);
                        processData[0, 2] = "出库铲车";
                        processData[0, 3] = "";
                        processData[0, 4] = "";
                    }
                }

                //cast
                commandText = "select * from `" + gVariable.productCastListTableName + "` where dispatchCode = '" + dispatchCodeL + "'";
                tableArrayCasting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (tableArrayCasting != null)
                {
                    processData[2, 0] = tableArrayCasting[0, 3];
                    processData[2, 1] = toolClass.getNameByIDAndIDByName(null, tableArrayCasting[0, 8]);
                    processData[2, 2] = gVariable.machineNameArrayDatabase[Convert.ToInt32(tableArrayCasting[0, 1])];
                    processData[2, 3] = tableArrayCasting[0, 2]; //cast barcode
                    processData[2, 4] = "";
                }

                //print
                commandText = "select * from `" + gVariable.productPrintListTableName + "` where dispatchCode = '" + dispatchCodeY + "'";
                tableArrayPrinting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (tableArrayPrinting != null)
                {
                    processData[3, 0] = tableArrayPrinting[0, 3];
                    processData[3, 1] = toolClass.getNameByIDAndIDByName(null, tableArrayPrinting[0, 8]);
                    processData[3, 2] = gVariable.machineNameArrayDatabase[Convert.ToInt32(tableArrayPrinting[0, 1])];
                    processData[3, 3] = tableArrayPrinting[0, 4];  //print barcode
                    commandText = "select * from `" + gVariable.productTableName + "` where productCode = '" + tableArrayCasting[0, 9] + "'";
                    tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                    processData[3, 4] = tableArray[0, 25]; //ink ratio
                }

                //slit
                commandText = "select * from `" + gVariable.productSlitListTableName + "` where dispatchCode = '" + dispatchCodeF + "'";
                tableArraySlitting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (tableArraySlitting != null)
                {
                    processData[4, 0] = tableArraySlitting[0, 3];
                    processData[4, 1] = toolClass.getNameByIDAndIDByName(null, tableArraySlitting[0, 14]);
                    processData[4, 2] = gVariable.machineNameArrayDatabase[Convert.ToInt32(tableArraySlitting[0, 1])];
                    processData[4, 3] = tableArraySlitting[0, 4];    //slit bar code
                    processData[4, 4] = tableArraySlitting[0, 13];  //num of joints
                }

                //inspection
                commandText = "select * from `" + gVariable.inspectionListTableName + "` where dispatchCode = '" + dispatchCodeL + "'";
                tableArrayInspection = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (tableArrayInspection != null)
                {
                    processData[5, 0] = tableArrayInspection[0, 3];  //inspection time
                    processData[5, 1] = toolClass.getNameByIDAndIDByName(null, tableArrayInspection[0, 9]);
                    processData[5, 2] = "质检仪器";
                    processData[5, 3] = tableArrayInspection[0, 4];  //inspection bar code
                    processData[5, 4] = tableArrayInspection[0, 10];  //result
                    if (tableArrayInspection[0, 10] == "0")
                    {
                        processData[5, 4] = "检验合格";  //inspection result
                    }
                    else
                    {
                        i = Convert.ToInt32(tableArrayInspection[0, 10]);
                        if (i > inspectionError.Length)
                            i = inspectionError.Length;
                        processData[5, 4] = inspectionError[i - 1];  //inspection error code
                    }
                }

                commandText = "select * from `" + gVariable.finalPackingTableName + "` where dispatchcode = '" + dispatchCodeL + "'";
                tableArrayPacking = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (tableArrayPacking != null) //this is a packing barcode, try to get slit/print/cast barcode
                {
                    //pack
                    processData[6, 0] = tableArrayPacking[0, 6];  //packing scan time
                    processData[6, 1] = toolClass.getNameByIDAndIDByName(null, tableArrayPacking[0, 10]);  //worker
                    processData[6, 2] = "打包机";
                    processData[6, 3] = tableArrayInspection[0, 5];  //packing bar code
                    processData[6, 4] = "";
                }

                fontSize = commonFontSize[gVariable.dpiValue, gVariable.resolutionLevel];
                font = new Font("Microsoft Sans Serif", fontSize, FontStyle.Regular);

                for (i = 1; i < x.Length; i++)
                {
                    x[i] = x[i - 1] + w[i - 1] + gap_x;
                }

                //process title
                for (i = 0; i < processList.GetLength(0); i++)
                {
                    for (j = 0; j < processList.GetLength(1) * 2; j++)
                    {
                        if (j % 2 == 0)
                        {
                            e.FillRectangle(colorGrayBrush, (int)(x[j] * screenRatioX), (int)((y0 + i * gapy) * screenRatioY), (int)(w[j] * screenRatioX), (int)(h0 * screenRatioY));
                            e.DrawString(processList[i, j / 2], font, Brushes.Black, (int)((x[j] + 9) * screenRatioX), (int)((y1 + i * gapy) * screenRatioY));
                        }
                        else
                        {
                            e.FillRectangle(colorGreenBrush, (int)(x[j] * screenRatioX), (int)((y0 + i * gapy) * screenRatioY), (int)(w[j] * screenRatioX), (int)(h0 * screenRatioY));
                            if (tableArrayCasting != null)
                            {
                                e.DrawString(processData[i, (j - 1) / 2], font, Brushes.Black, (int)((x[j] + 9) * screenRatioX), (int)((y1 + i * gapy) * screenRatioY));
                            }
                        }
                    }
                }
                e.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("displayProductionProcess failed" + ex);
            }
        }

        private void backtrack_FormClosing(object sender, EventArgs e)
        {
            try
            {
                aTimer.Enabled = false;

                serialNumbacktrack.serialNumbacktrackClass.Show();
            }
            catch (Exception ex)
            {
                Console.Write("close material backtrack class failed!" + ex);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            screenRefresh = 1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            displayProductionProcess();
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            screenRefresh = 2;
        }

    }
}
