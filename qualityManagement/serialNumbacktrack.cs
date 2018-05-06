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
    public partial class serialNumbacktrack : Form
    {
        const int MATERIAL_OUT_PROCESS = 0;
        const int MATERIAL_FEED_PROCESS = 1;
        const int CASTING_PROCESS = 2;
        const int PRINTING_PROCESS = 3;
        const int SLITTING_PROCESS = 4;
        const int INSPECTION_PROCESS = 5;
        const int PACKING_PROCESS = 6;

        public static serialNumbacktrack serialNumbacktrackClass = null; //it is used to reference this windows

        int barcodeProcessType;
        float screenRatioX, screenRatioY;

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

        string slitBarcode;
        string[,] tableArraySlitting;

        string printBarcode;
        string[,] tableArrayPrinting;

        string castBarcode;
        string[,] tableArrayCasting;

        string inspectionBarcode;
        string[,] tableArrayInspection;

        string packBarcode;
        string[,] tableArrayPacking;

        public serialNumbacktrack()
        {
            InitializeComponent();
            initializeVariables();
            resizeScreen();
        }

        void initializeVariables()
        {
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
            label1.Text = gVariable.enterpriseTitle + "质量追溯系统";
            this.Text = gVariable.enterpriseTitle + "质量追溯系统";
            textBox1.Text = "A02323huawang01";
        }

        void resizeScreen()
        {
            int i;
            int x, y, w, h;
            float fontSize;
            GroupBox[] groupBoxArray = { groupBox1, groupBox2, groupBox5 };
            Label[] labelArray = { label2, };
            TextBox[] textBoxArray = { textBox1};
            Button[] buttonArray = { button1 };
            ListView[] listViewArray = { listView1 };

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

            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

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
            //variable of 0 means there is no data, only titles need to be displayed
            displayDispatchInfo(0);
            displayMaterialInfo(0);
            displayProductionProcess(0);
        }

        int getAllBarcodeByPack(string tmpBarcode)
        {
            string commandText;

            commandText = "select * from `" + gVariable.finalPackingTableName + "` where largebarcode = '" + tmpBarcode + "'";
            tableArrayPacking = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
            if (tableArrayPacking != null) //this is a packing barcode, try to get slit/print/cast barcode
            {
                barcodeProcessType = PACKING_PROCESS;

                slitBarcode = tableArrayPacking[0, 1]; //the first slit roll barcode in this pack

                getAllBarcodeBySlit(slitBarcode);
                return 0;
            }
            else
                return -1;
        }


        //we've already had slit barcode, try get print/cast/inspection, and also pack infor if pack is null 
        int getAllBarcodeBySlit(string tmpBarcode)
        {
            string largeBarcode;
            string commandText;

            try
            {
                commandText = "select * from `" + gVariable.productSlitListTableName + "` where barcode2 = '" + tmpBarcode + "'";
                tableArraySlitting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (tableArraySlitting != null)  //got slit barcode, continue with print and cast
                {
                    barcodeProcessType = SLITTING_PROCESS;

                    slitBarcode = tmpBarcode;
                    largeBarcode = tableArraySlitting[0, 2]; //get large roll barcode of this small roll, it could be a print barcode or cast barcode
                    commandText = "select * from `" + gVariable.productPrintListTableName + "` where barcode2 = '" + largeBarcode + "'";
                    tableArrayPrinting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                    if (tableArrayPrinting != null)  //got print barcode, try to get cast
                    {
                        printBarcode = largeBarcode;
                        castBarcode = tableArrayPrinting[0, 2]; //get cast barcode from print table
                        commandText = "select * from `" + gVariable.productCastListTableName + "` where barcode = '" + castBarcode + "'";
                        tableArrayCasting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                        if (tableArrayCasting == null)
                        {
                            //should not come to this place
                            return -1;
                        }
                    }
                    else  //this slit roll does not have a print process, try to get cast directly
                    {
                        commandText = "select * from `" + gVariable.productCastListTableName + "` where barcode = '" + largeBarcode + "'";
                        tableArrayCasting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                        if (tableArrayCasting != null)
                            castBarcode = largeBarcode;
                        else //should not come to this place
                            return -1;
                    }

                    if (inspectionBarcode == null)
                    {
                        commandText = "select * from `" + gVariable.inspectionListTableName + "` where barcode1 = '" + slitBarcode + "'";
                        tableArrayInspection = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                        if (tableArrayInspection != null)
                            inspectionBarcode = tableArrayInspection[0, 4];
                    }

                    //packing barcode is still null, get it
                    if (tableArrayPacking == null)
                    {
                        commandText = "select * from `" + gVariable.finalPackingTableName + "` where startsmallbarcode <= '" + slitBarcode + "' and endsmallbarcode >= '" + slitBarcode + "'";
                        tableArrayPacking = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                        if (tableArrayPacking != null) //this is a packing barcode, try to get slit/print/cast barcode
                        {
                            packBarcode = tableArrayPacking[0, 5]; //we got packing code
                        }
                    }
                }

                if (slitBarcode == null)
                    return -1;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getAllBarcodeBySlit failed" + ex);
                return -1;
            }
        }

        int getAllBarcodeByInspection(string tmpBarcode)
        {
            string commandText;

            try
            {
                commandText = "select * from `" + gVariable.inspectionListTableName + "` where barcode2 = '" + tmpBarcode + "'";
                tableArrayCasting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (tableArrayPacking != null) //this is a packing barcode, try to get slit/print/cast barcode
                {
                    barcodeProcessType = INSPECTION_PROCESS;

                    slitBarcode = tableArrayPacking[0, 2]; //get slit roll barcode from inspection barcode
                    getAllBarcodeBySlit(slitBarcode);
                    return 0;
                }
                else
                    return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getAllBarcodeByInspection failed" + ex);
                return -1;
            }
        }


        int getAllBarcodeByPrint(string tmpBarcode)
        {
            string commandText;

            try
            {
                //try to see if this is a printing barcode
                commandText = "select * from `" + gVariable.productPrintListTableName + "` where barcode2 = '" + tmpBarcode + "'";
                tableArrayPrinting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (tableArrayPrinting != null)  //this is a printing barcode, try to get other info
                {
                    barcodeProcessType = PRINTING_PROCESS;

                    printBarcode = tableArrayPrinting[0, 4];
                    //if the print roll is slit in slit machine, we should be able to find the record in slit table
                    commandText = "select * from `" + gVariable.productSlitListTableName + "` where barcode1 = '" + printBarcode + "'";
                    tableArraySlitting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                    if (tableArraySlitting != null)  //we got the slitting barcode
                    {
                        slitBarcode = tableArrayPrinting[0, 4];
                        getAllBarcodeBySlit(slitBarcode);
                        return 0;
                    }
                    else //this product has no slit process, so it must be sold by large roll
                    {
                        castBarcode = tableArrayPrinting[0, 2];
                        //if the print roll is slit in slit machine, we should be able to find the record in slit table
                        commandText = "select * from `" + gVariable.productCastListTableName + "` where barcode = '" + castBarcode + "'";
                        tableArrayCasting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                        if (tableArrayCasting != null)  //we got the slitting barcode
                        {
                            return 0;
                        }
                        else
                        {
                            //we should not come to this place
                            return -1;
                        }
                    }
                }
                else
                    return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getAllBarcodeByPrint failed" + ex);
                return -1;
            }
        }

        int getAllBarcodeByCast(string tmpBarcode)
        {
            string commandText;

            try
            {
                commandText = "select * from `" + gVariable.productCastListTableName + "` where barcode = '" + tmpBarcode + "'";
                tableArrayCasting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                if (tableArrayCasting != null) //this is a cast barcode, try to get slit/print
                {
                    barcodeProcessType = CASTING_PROCESS;

                    commandText = "select * from `" + gVariable.productPrintListTableName + "` where barcode1 = '" + castBarcode + "'";
                    tableArrayPrinting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                    if (tableArrayPrinting != null)  //got printing barcode, try to get other info
                    {
                        printBarcode = tableArrayPrinting[0, 4];
                        //if the print roll is slit in slit machine, we should be able to find the record in slit table
                        commandText = "select * from `" + gVariable.productSlitListTableName + "` where barcode1 = '" + printBarcode + "'";
                        tableArraySlitting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                        if (tableArraySlitting != null)  //we got the slitting barcode
                        {
                            slitBarcode = tableArrayPrinting[0, 4];
                            getAllBarcodeBySlit(slitBarcode);
                        }
                        else //this product has no slit process, so it must be sold by large roll
                        {
                            return 0;
                        }
                    }
                    else  //no printing process, should be a non-prinintg product, deal with slitting directly
                    {
                        commandText = "select * from `" + gVariable.productSlitListTableName + "` where barcode1 = '" + castBarcode + "'";
                        tableArraySlitting = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                        if (tableArraySlitting != null)  //got slitting barcode, try to get other info
                        {
                            slitBarcode = tableArraySlitting[0, 4];
                            getAllBarcodeBySlit(slitBarcode);
                        }
                        else  //this slit process is not finished yet, so there is only casting info
                            return 0;
                    }
                    return 0;
                }
                else
                    return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getAllBarcodeByCast failed" + ex);
                return -1;
            }
        }


        int trackingForBarcodeAndDispatch()
        {
            string barCodeInOrigin;

            try
            {
                barCodeInOrigin = textBox1.Text;

                slitBarcode = null;
                tableArraySlitting = null;

                inspectionBarcode = null;
                tableArrayInspection = null;

                printBarcode = null;
                tableArrayPrinting = null;

                castBarcode = null;
                tableArrayCasting = null;

                //feedBarcode = null;
                //tableArrayFeeding = null;

                packBarcode = null;
                tableArrayPacking = null;

                //to see what kind of bar code this is, and at the same time, if found the barcode in database, put all related information into array accoringly
                if (getAllBarcodeByPack(barCodeInOrigin) >= 0)
                    return 0;

                if (getAllBarcodeBySlit(barCodeInOrigin) >= 0)
                    return 0;

                if (getAllBarcodeByInspection(barCodeInOrigin) >= 0)
                    return 0;
                
                if (getAllBarcodeByPrint(barCodeInOrigin) >= 0)
                    return 0;

                if (getAllBarcodeByCast(barCodeInOrigin) >= 0)
                    return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("trackingForBarcodeAndDispatch failed" + ex);
            }

            return -1;
        }


        void displayDispatchInfo(int displayData)
        {
            int i, j;
            int[] x = { 10, 0, 0, 0, 0, 0 };
            int[] w = { 80, 90, 80, 120, 80, 140 };
            int y0 = 7;
            int h0 = 30;
            int y1 = 14;
            int gap_x = 10;
            int gapy = 39;
            Font font;
            float fontSize;
            string barCodeCur;
            string dispatchCodeCur;
            string commandText;
            string databaseName;
            string[,] tableArray;
            string[,] dispatchData = new string[5, 3];

            string[] allMachineProcessForZihua = { "出库工序", "投料工序", "流延工序", "印刷工序", "分切工序", "质检工序", "打包工序" };

            string[,] dispatchInfo = 
            { 
                {"订单编号", "批次号",   "工单编号"},
                {"产品编码", "产品名称", "作业员工"},
                {"客户名称", "计划开始", "计划完成"},
                {"计划产量", "合格数量", "不合格数"},
                {"标签工序", "开工时间", "完工时间"},
            };
            Graphics e = panel2.CreateGraphics();

            try
            {
                if (tableArrayCasting != null)
                {
                    if (barcodeProcessType == PRINTING_PROCESS)
                    {
                        databaseName = gVariable.DBHeadString + (tableArrayPrinting[0, 1]).PadLeft(3, '0');
                        barCodeCur = tableArrayPrinting[0, 4];
                        dispatchCodeCur = tableArrayPrinting[0, 6];
                    }
                    else if (barcodeProcessType == CASTING_PROCESS)
                    {
                        databaseName = gVariable.DBHeadString + (tableArrayCasting[0, 1]).PadLeft(3, '0');
                        barCodeCur = tableArrayCasting[0, 2];
                        dispatchCodeCur = tableArrayCasting[0, 4];
                    }
                    else// if (barcodeProcessType == SLITTING_PROCESS/INSPECTION_PROCESS/PACKING_PROCESS)
                    {
                        databaseName = gVariable.DBHeadString + (tableArraySlitting[0, 1]).PadLeft(3, '0');
                        barCodeCur = tableArraySlitting[0, 4];
                        dispatchCodeCur = tableArraySlitting[0, 6];
                    }

                    commandText = "select * from `" + gVariable.dispatchListTableName + "` where dispatchCode = '" + tableArrayCasting[0, 4] + "'";
                    tableArray = mySQLClass.databaseCommonReading(databaseName, commandText);

                    dispatchData[0, 0] = dispatchCodeCur.Substring(0, gVariable.salesOrderLength);
                    dispatchData[0, 1] = barCodeCur.Substring(gVariable.dispatchCodeLength + 2, 4) + barCodeCur.Substring(gVariable.salesOrderLength + 3, 1) + barCodeCur.Substring(gVariable.salesOrderLength, 2);
                    dispatchData[0, 2] = dispatchCodeCur;
                    dispatchData[1, 0] = tableArray[0, mySQLClass.PRODUCT_CODE_IN_DISPATCHLIST_DATABASE];
                    dispatchData[1, 1] = tableArray[0, mySQLClass.PRODUCT_NAME_IN_DISPATCHLIST_DATABASE];
                    dispatchData[1, 2] = tableArray[0, mySQLClass.OPERATOR_NAME_IN_DISPATCHLIST_DATABASE];
                    dispatchData[2, 0] = tableArray[0, mySQLClass.CUSTOMER_IN_DISPATCHLIST_DATABASE];
                    dispatchData[2, 1] = tableArray[0, mySQLClass.PLANNED_STARTTIME_IN_DISPATCHLIST_DATABASE];
                    dispatchData[2, 2] = tableArray[0, mySQLClass.PLANNED_COMPLETETIME_IN_DISPATCHLIST_DATABASE];
                    dispatchData[4, 0] = allMachineProcessForZihua[barcodeProcessType];

                    if (barcodeProcessType == INSPECTION_PROCESS)
                    {
                        dispatchData[3, 0] = "";
                        dispatchData[3, 1] = "";
                        dispatchData[3, 2] = "";

                        dispatchData[4, 1] = tableArrayInspection[0, 1];
                        dispatchData[4, 2] = tableArrayInspection[0, 3];
                    }
                    else if (barcodeProcessType == PACKING_PROCESS)
                    {
                        dispatchData[3, 0] = "";
                        dispatchData[3, 1] = "";
                        dispatchData[3, 2] = "";

                        dispatchData[4, 1] = tableArrayPacking[0, 4];
                        dispatchData[4, 2] = tableArrayPacking[0, 6];
                    }
                    else
                    {
                        dispatchData[3, 0] = tableArray[0, mySQLClass.FORCAST_NUM_IN_DISPATCHLIST_DATABASE];
                        dispatchData[3, 1] = tableArray[0, mySQLClass.QUALIFIED_NUM_IN_DISPATCHLIST_DATABASE];
                        dispatchData[3, 2] = tableArray[0, mySQLClass.UNQUALIFIED_NUM_IN_DISPATCHLIST_DATABASE];

                        dispatchData[4, 1] = tableArray[0, mySQLClass.START_TIME_IN_DISPATCHLIST_DATABASE];
                        dispatchData[4, 2] = tableArray[0, mySQLClass.COMPLETE_TIME_IN_DISPATCHLIST_DATABASE];
                    }
                }

                fontSize = commonFontSize[gVariable.dpiValue, gVariable.resolutionLevel];
                font = new Font("Microsoft Sans Serif", fontSize, FontStyle.Regular);

                for (i = 1; i < x.Length; i++)
                {
                    x[i] = x[i - 1] + w[i - 1] + gap_x;
                }

                //dispatchInfo title
                for (i = 0; i < dispatchInfo.GetLength(0); i++)
                {
                    for (j = 0; j < dispatchInfo.GetLength(1) * 2; j++)
                    {
                        if (j % 2 == 0)
                        {
                            e.FillRectangle(colorGrayBrush, (int)(x[j] * screenRatioX), (int)((y0 + i * gapy) * screenRatioY), (int)(w[j] * screenRatioX), (int)(h0 * screenRatioY));
                            e.DrawString(dispatchInfo[i, j / 2], font, Brushes.Black, (int)((x[j] + 9) * screenRatioX), (int)((y1 + i * gapy) * screenRatioY));
                        }
                        else
                        {
                            e.FillRectangle(colorGreenBrush, (int)(x[j] * screenRatioX), (int)((y0 + i * gapy) * screenRatioY), (int)(w[j] * screenRatioX), (int)(h0 * screenRatioY));
                            if (tableArrayCasting != null)
                            {
                                e.DrawString(dispatchData[i, (j - 1) / 2], font, Brushes.Black, (int)((x[j] + 9) * screenRatioX), (int)((y1 + i * gapy) * screenRatioY));
                            }
                        }
                    }
                }
                e.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("displaydispatchInfo failed" + ex);
            }
        }

        void displayProductionProcess(int displayData)
        {
            int i, j;
            int[] x = {10, 0,    0,   0,   0,   0,   0,   0,   0,   0};
            int[] w = {80, 120,  68,  80,  50,  90,  68,  255, 80,  250}; 
            int y0 = 7;
            int h0 = 30;
            int y1 = 14;
            int gap_x = 10;
            int gapy = 39;
            Font font;
            float fontSize;
            string commandText;
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
                if (tableArrayCasting != null)
                {
                    //feed material
                    //find a feed record with this cast dispatch code in material in/out record table
                    commandText = "select * from `" + gVariable.materialDeliveryTableName + "` where dispatchCode = '" + tableArrayCasting[0, 4] + "'";
                    tableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
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
                    processData[0, 0] = tableArray[0, 4];
                    processData[0, 1] = toolClass.getNameByIDAndIDByName(null, tableArray[0, 8]);
                    processData[0, 2] = "出库铲车";
                    processData[0, 3] = "";
                    processData[0, 4] = "";

                    //cast
                    processData[2, 0] = tableArrayCasting[0, 3];
                    processData[2, 1] = toolClass.getNameByIDAndIDByName(null, tableArrayCasting[0, 8]);
                    processData[2, 2] = gVariable.machineNameArrayDatabase[Convert.ToInt32(tableArrayCasting[0, 1])];
                    processData[2, 3] = tableArrayCasting[0, 2]; //cast barcode
                    processData[2, 4] = "";

                    //print
                    if (tableArrayPrinting != null)
                    {
                        processData[3, 0] = tableArrayPrinting[0, 3];
                        processData[3, 1] = toolClass.getNameByIDAndIDByName(null, tableArrayPrinting[0, 10]);
                        processData[3, 2] = gVariable.machineNameArrayDatabase[Convert.ToInt32(tableArrayPrinting[0, 1])];
                        processData[3, 3] = tableArrayPrinting[0, 4];  //print barcode
                        commandText = "select * from `" + gVariable.productTableName + "` where productCode = '" + tableArrayCasting[0, 9] + "'";
                        tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                        processData[3, 4] = tableArray[0, 25]; //ink ratio
                    }

                    //slit
                    if (tableArraySlitting != null)
                    {
                        processData[4, 0] = tableArraySlitting[0, 3];
                        processData[4, 1] = toolClass.getNameByIDAndIDByName(null, tableArraySlitting[0, 14]);
                        processData[4, 2] = gVariable.machineNameArrayDatabase[Convert.ToInt32(tableArraySlitting[0, 1])];
                        processData[4, 3] = tableArraySlitting[0, 4];    //slit bar code
                        processData[4, 4] = tableArraySlitting[0, 13];  //num of joints
                    }

                    //inspection
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

        void displayMaterialInfo(int displayData)
        {
            int i; //, j;
            int num;
            string commandText;
            string databaseName;
            //string timeS1, timeS2;
            string[,] tableArray;
            string[,] tableArray2;
            string[,] tableArray3;
            int[] widthArray = { 50, 120, 100, 90, 100, 70 };

            try
            {
                listView1.Clear();

                listView1.BeginUpdate();

                listView1.GridLines = true;
                listView1.Dock = DockStyle.Fill;
                listView1.Columns.Add(" ", 1, HorizontalAlignment.Center);
                listView1.Columns.Add("序号", (int)(widthArray[0] * screenRatioX), HorizontalAlignment.Center);
                listView1.Columns.Add("物料批次", (int)(widthArray[1] * screenRatioX), HorizontalAlignment.Center);
                listView1.Columns.Add("物料名称", (int)(widthArray[2] * screenRatioX), HorizontalAlignment.Center);
                listView1.Columns.Add("供应商", (int)(widthArray[3] * screenRatioX), HorizontalAlignment.Center);
                listView1.Columns.Add("购买日期", (int)(widthArray[4] * screenRatioX), HorizontalAlignment.Center);
                listView1.Columns.Add("数量", (int)(widthArray[5] * screenRatioX), HorizontalAlignment.Center);

                //we need to diaplay material information and we've already have a dispatch, so we can get material info form that dispatch 
                if (displayData == 1 && tableArrayCasting != null)
                {
                    databaseName = gVariable.DBHeadString + (tableArrayCasting[0, 1]).PadLeft(3, '0');
                    commandText = "select * from `" + gVariable.materialListTableName + "` where dispatchCode = '" + tableArrayCasting[0, 4] + "'";
                    tableArray = mySQLClass.databaseCommonReading(databaseName, commandText);

                    commandText = "select * from `" + gVariable.binInventoryTableName + "` where dispatchCode = '" + tableArrayCasting[0, 4] + "'";
                    tableArray3 = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);

                    //get total kinds of material for this product
                    num = Convert.ToInt32(tableArray[0, 6]);
                    for (i = 0; i < num; i++)
                    {
                        //no legal material code
                        if (tableArray[0, 8 + i * 4].Length < 4)
                            break;

                        ListViewItem OptionItem = new ListViewItem();

                        OptionItem.SubItems.Add((i + 1).ToString());
                        OptionItem.SubItems.Add(tableArray[0, 8 + i * 4]);   //material code
                        OptionItem.SubItems.Add(tableArray[0, 7 + i * 4]);    //material name

                        commandText = "select * from `" + gVariable.materialPurchaseTableName + "` where materialCode = '" + tableArray[0, 8 + i * 4] + "'";
                        tableArray2 = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);

                        OptionItem.SubItems.Add(tableArray2[0, 6]);    //material vendor name
                        OptionItem.SubItems.Add(tableArray2[0, 7]);    //material purchase date name
                        OptionItem.SubItems.Add(tableArray3[1, 5 + i * 2]);    //material quantity used by this dispatch, the first one is dispatch start, the second one is dispatch completed, 

                        OptionItem.BackColor = Color.LightGreen;
                        listView1.Items.Add(OptionItem);
                    }
                }
                this.listView1.EndUpdate();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
            }
            catch (Exception ex)
            {
                Console.WriteLine("displayMaterialInfo failed" + ex);
            }
        }

        private void backtrack_FormClosing(object sender, EventArgs e)
        {
            try
            {
                firstScreen.firstScreenClass.Show();
            }
            catch (Exception ex)
            {
                Console.Write("close room class" + ex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int ret;

            ret = trackingForBarcodeAndDispatch();
            if(ret == -1)
            {
                MessageBox.Show("抱歉，在历史记录中没有查到这张标签。", "提示信息", MessageBoxButtons.OK);
                return;
            }

            displayDispatchInfo(1);

            displayMaterialInfo(1);

            displayProductionProcess(1);
        }

        //one of the material is selected, track this material
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int trackingMaterialSelected;
            string databaseName;
            string commandText;
            string materialBatchNum;
            string[,] tableArray;

            trackingMaterialSelected = listView1.SelectedItems[0].Index;

            databaseName = gVariable.DBHeadString + (tableArrayCasting[0, 1]).PadLeft(3, '0');
            commandText = "select * from `" + gVariable.materialListTableName + "` where dispatchCode = '" + tableArrayCasting[0, 4] + "'";
            tableArray = mySQLClass.databaseCommonReading(databaseName, commandText);

            materialBatchNum = tableArray[0, 8 + trackingMaterialSelected * 4];

            trackMaterialProcess(materialBatchNum);
        }

        private void trackMaterialProcess(string materialBatchNum)
        {
            materialBacktrack.materialBacktrackClass = new materialBacktrack(materialBatchNum);
            materialBacktrack.materialBacktrackClass.Show();

            this.Hide();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            displayDispatchInfo(1);

            //displayMaterialInfo(1);

            displayProductionProcess(1);
        }
    }
}
