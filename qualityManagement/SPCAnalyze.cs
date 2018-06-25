using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MESSystem.common;
using MESSystem.quality;
using MESSystem.dispatchManagement;

namespace MESSystem.quality
{
    public partial class SPCAnalyze : Form
    {
        int tipFlag = 0;

        public int SPCResult;

        string databaseName;
        string tableName;
        int id;   //id of the alarm in alarm table for this machine
        int boardIndex;

        int machineSelected;    //which machine is selected to display quality data
        int dispatchSelected;   //which dispatch is selected 
        int qualityDataIndexSelected;   //which data item in quality data need to be displayed

        int SPCFromAlarmOrQualityManagement;
        int totalDispatchNum;
        int numOfPointNeedForChart;

        string dispatchCode;
        int type;
        int category;
        int indexInTable;
        int startID;

        gVariable.alarmTableStruct alarmTableStructImpl;

        string[] dispatchListArray;
        string[] qualityDataItemArray = new string[gVariable.maxQualityDataNum];

        //spec limits for this data item in quality data
        public float[] specLowerLimitArray = new float[gVariable.maxQualityDataNum];
        public float[] specUpperLimitArray = new float[gVariable.maxQualityDataNum];

        string errorItem;
        int analyzeChartType; //what kind of chart we need to draw, could be XBar-S chart, C chart or NO SPC(original) chart  

        const int TAB_CONTROL_SPC_CURVE_DATA = 0;
        const int TAB_CONTROL_CPK_DATA = 1;
        const int TAB_CONTROL_ORIGINAL_DATA = 2;
        const int TAB_CONTROL_AVERAGE_DATA = 3;
        const int TAB_CONTROL_ALL_DATA = 4;

        const int TAB_CONTROL_XBAR_S_DATA = 5;
        const int TAB_CONTROL_C_CHART_DATA = 6;

        //this parameter decides which tab (also means which chart) will be displayed in focus mode, other tab in hide mode 
        int tabControlIndex;

        //we get below arrays by readSmallPartOfDataToArray()
        //SPC control limits/center value for XBar and S chart
        public float[,] controlCenterValueArray = new float[gVariable.maxQualityDataNum, 6]; //high1, center1, low1, high2, center2, low2
        public float[,] dataInPoint = new float[gVariable.maxQualityDataNum, gVariable.totalPointNumForSChart];
        public int[,] timeInPoint = new int[gVariable.maxQualityDataNum, gVariable.totalPointNumForSChart];
        public int[,] statusInPoint = new int[gVariable.maxCurveNum, gVariable.totalPointNumForSChart];

        const int CHART_CRAFT_DATA = 0;   //no group, only seperate data
        const int CHART_QUALITY_VALUE_DATA = 1;  //values like thickness 
        const int CHART_QUALITY_NUMBER_DATA = 2;   //values like number of black dots/holes 

        int SPCDataNotEnough;
        float[] thisGroupOfData = new float[gVariable.totalPointNumForSChart];
//        public static SPCAnalyze SPCAnalyzeClass;

        SPCFunctions SPCFuncClass;

        //whereFrom: from alarm or quality
        //id: if this function is triggered by alarm, id means index in alarm table
        //type: this is a quaity data or craft data triggered alarm 
        public SPCAnalyze(string databaseName_, int whereFrom, int id_, int type_)
        {
            InitializeComponent();

            databaseName = databaseName_;

            //if the function is triggered by alarm, id means alarm index in alarm table
            id = id_;
            //if this function is triggered by quality management, id means data item index in quality data table
            qualityDataIndexSelected = id_;

            type = type_;

            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);

            if (initVariables(whereFrom) < 0)
            {
                SPCDataNotEnough = 1;
                this.Close();
                //gVariable.SPCDataNotEnough = 1;
            }
        }

        public int getSPCFuncClassStatus()
        {
            return SPCDataNotEnough;
        }
        private int initVariables(int whereFrom)
        {
            SPCResult = 0;
            indexInTable = 0;
            startID = 0;
            category = 0;
            SPCDataNotEnough = 0;
            boardIndex = Convert.ToInt32(databaseName.Remove(0, 1)) - 1;
            SPCFuncClass = new SPCFunctions();

            if(setComboBoxes(whereFrom) < 0)
                return -1;

            setPageControl();

            return 0;
        }

        public int checkResult()
        {
            return SPCResult;
        }

        private void setPageControl()
        {
            //there are altogether 5 pages, 2:SPC chart; 3:CPK; 4:original data curve; 1:average data curve; 5:all-in-one data curve 
            switch (type)
            {
                case gVariable.ALARM_TYPE_QUALITY_DATA:
                    tabControlIndex = TAB_CONTROL_SPC_CURVE_DATA;
                    break;
                case gVariable.ALARM_TYPE_CRAFT_DATA:
                    this.tabPage2.Text = " ------ ";
                    this.tabPage1.Text = " ------ ";
                    tabControl1.SelectedIndex = 2;  //2 means start with original data curve
                    tabControlIndex = TAB_CONTROL_ORIGINAL_DATA;
                    break;
                case gVariable.ALARM_TYPE_DEVICE:
                case gVariable.ALARM_TYPE_MATERIAL:
//                case gVariable.ALARM_TYPE_CYCTLE_TIME:
                default:
                    //we should not come here, but if we come here, pretend to be in data overflow mode
                    this.tabPage2.Text = " ------ ";
                    tabControl1.SelectedIndex = 2;  //2 means original data
                    tabControlIndex = TAB_CONTROL_ORIGINAL_DATA;
                    break;
            }
        }

        private int setComboBoxes(int whereFrom)
        {
            int i;
            int dataNum;
            string commandText;
            DataTable dTable;

            try
            {
                if (whereFrom == gVariable.FROM_ALARM_DISPLAY_FUNC)  //comes from Andon
                {
                    label30.Visible = false;
                    label31.Visible = false;
                    label32.Visible = false;

                    label33.Text = "安灯报警数据分析";

                    comboBox1.Visible = false;
                    comboBox2.Visible = false;
                    comboBox3.Visible = false;

                    SPCFromAlarmOrQualityManagement = gVariable.FROM_ALARM_DISPLAY_FUNC;
                }
                else  //from quality management
                {
                    label30.Visible = true;
                    label31.Visible = true;
                    label32.Visible = true;

                    label33.Text = "质量管理数据分析";

                    comboBox1.Visible = true;
                    comboBox2.Visible = true;
                    comboBox3.Visible = true;

                    totalDispatchNum = mySQLClass.getRecordNumInTable(databaseName, gVariable.dispatchListTableName);
                    if (totalDispatchNum <= 1)  //means no dispatch exist, 1 means only a dummy dispatch
                    {
                        MessageBox.Show("抱歉, 该设备没有工单记录, 因此无法提供质量数据管理功能！", "信息提示", MessageBoxButtons.OK);
                        return -1;
                    }

                    for (i = 0; i < gVariable.machineNameArrayAPS.Length; i++)
                    {
                        comboBox1.Items.Add(gVariable.machineNameArrayAPS[i]);
                    }
                    comboBox1.SelectedIndex = machineSelected;

                    totalDispatchNum--;
                    dispatchListArray = new string[totalDispatchNum];

                    //dTable = toolClass.getDispatchListFromDatabase(databaseName, gVariable.dispatchListTableName);
                    commandText = "select * from `" + gVariable.dispatchListTableName + "`";
                    dTable = mySQLClass.queryDataTableAction(databaseName, commandText, null);
                    i = 0;
                    foreach (DataRow dr in dTable.Rows)
                    {
                        if (i == 0)
                        {
                            i++;
                            continue;
                        }

                        if (totalDispatchNum < i)
                            break;  //should not come here

                        dispatchListArray[totalDispatchNum - i] = dr[mySQLClass.DISPATCH_CODE_IN_DISPATCHLIST_DATABASE].ToString();
                        i++;
                    }

                    for (i = 0; i < totalDispatchNum; i++)
                    {
                        comboBox2.Items.Add(dispatchListArray[i]);
                    }
                    comboBox2.SelectedIndex = dispatchSelected;

                    dataNum = dispatchTools.getQualityDataItemByDispatch(databaseName, gVariable.qualityListTableName, dispatchListArray[dispatchSelected], qualityDataItemArray);
                    for (i = 0; i < dataNum; i++)
                    {
                        comboBox3.Items.Add(qualityDataItemArray[i]);
                    }
                    comboBox3.SelectedIndex = qualityDataIndexSelected;

                    SPCFromAlarmOrQualityManagement = gVariable.FROM_QUALITY_MANAGEMENT_FUNC;
                }
            }
            catch (Exception ex)
            {
                Console.Write("SPCAnalyze setComboBoxes failed! " + ex);
            }
            return 0;
        }

        private void SPCAnalyze_Load(object sender, EventArgs e)
        {
            if (initGetAllDataNeeded() < 0)
            {
                this.Close();
                return;
            }

            prepareAllCharts();

            drawCurrentChart();

            listView_Load();
        }

        private void SPCAnalyze_Closing(object sender, FormClosingEventArgs e)
        {

        }

        private void drawCurrentChart()
        {
            drawSPCCharts(tabControlIndex);
        }


        //we've got all data we needed in getAllDataNeeded(), now try to get control limits/center data then put all data to chart to draw various charts
        private void drawSPCCharts(int tabIndex)
        {
            switch (tabIndex)
            {
                case TAB_CONTROL_SPC_CURVE_DATA:
                    SPCChartSettings(chart5);  //including c chart and XBar-S chart
                    break;
                case TAB_CONTROL_CPK_DATA:
                    CPKChartSettings(chart2);  //only responsible for a column chart drawing
                    break;
                case TAB_CONTROL_ORIGINAL_DATA:
                    OriChartSettings(chart4);  //
                    break;
                case TAB_CONTROL_AVERAGE_DATA:
                    AverageChartSettings(chart3);
                    break;
                case TAB_CONTROL_ALL_DATA:
                    OverallChartSettings(chart1, 100, 100);
                    break;
                default:
                    break;
            }
        }


        private int getSPCAlarmInfo()
        {
            try
            {
                alarmTableStructImpl = mySQLClass.getAlarmTableContent(databaseName, gVariable.alarmListTableName, id);

                if (type == gVariable.ALARM_TYPE_QUALITY_DATA)
                {
                    tableName = gVariable.qualityListTableName;

                    //get chart type for this quality dat item
                    //could be CHART_TYPE_SPC_C or CHART_TYPE_SPC_XBAR_S, that is 1 or 2
                    analyzeChartType = toolClass.getQualityDataChartType(databaseName, tableName, alarmTableStructImpl.dispatchCode, alarmTableStructImpl.indexInTable);
                }
                else //if (type == gVariable.ALARM_TYPE_CRAFT_DATA)
                {
                    tableName = gVariable.craftListTableName;
                    analyzeChartType = gVariable.CHART_TYPE_NO_SPC;  //value is 0
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getSPCAlarmInfo() failed" + ex);
                return -1;
            }
        }

        //get chart related informations, it includes 5 parts below:
        //1. get SPC information: get chart type/number of points needed/start ID, etc
        //If this SPC detailed requirement comes from alarm, we get alarm info from alarm table, if comes from quality management, we gget it from quality table
        //2. get value: get time/value for every data point in chart
        //3. SPC calculation: including CP/CPK/PP/PPK/sigma/average, etc
        //4. get column chart info:get data for column chart
        //5. get screen title 
        private int initGetAllDataNeeded()
        {
            int i;
            int startDataIDInReading;
            int dataItemNum;
            int totalDataNum;
            string [] strArray;

            try
            {
                //1. get SPC information
                if (SPCFromAlarmOrQualityManagement == gVariable.FROM_ALARM_DISPLAY_FUNC)
                {
                    alarmTableStructImpl = mySQLClass.getAlarmTableContent(databaseName, gVariable.alarmListTableName, id);

                    if (type == gVariable.ALARM_TYPE_QUALITY_DATA)  //it is an craft alarm, so we should not come to this place
                    {
                        tableName = gVariable.qualityListTableName;

                        //get chart type for this quality dat item
                        //could be CHART_TYPE_SPC_C or CHART_TYPE_SPC_XBAR_S, that is 1 or 2
                        analyzeChartType = toolClass.getQualityDataChartType(databaseName, tableName, alarmTableStructImpl.dispatchCode, alarmTableStructImpl.indexInTable);
                    }
                    else //if (type == gVariable.ALARM_TYPE_CRAFT_DATA)
                    {
                        tableName = gVariable.craftListTableName;
                        analyzeChartType = gVariable.CHART_TYPE_NO_SPC;  //value is 0
                    }

                    dispatchCode = alarmTableStructImpl.dispatchCode;
                    startID = alarmTableStructImpl.startID;

                    totalDataNum = mySQLClass.getRecordNumInTable(databaseName, dispatchCode + gVariable.craftTableNameAppendex);
                    if (analyzeChartType == gVariable.CHART_TYPE_SPC_XBAR_S)
                    {
                        numOfPointNeedForChart = gVariable.totalPointNumForSChart;   //for XBar-S, we need at least 270 data 
//                        numOfPointNeedForXBARS_Chart
                    }
                    else if (analyzeChartType == gVariable.CHART_TYPE_SPC_C)
                    {   //for C chart and craft data, 
                        numOfPointNeedForChart = gVariable.totalPointNumForCChart;
//                        numOfPointNeedForC_Chart
                    }
                    else // if (analyzeChartType == gVariable.CHART_TYPE_NO_SPC)
                    {
                        if (startID < 20)
                        {
                            startID = 0;
                            if (totalDataNum >= gVariable.minDataNumToTriggerAlarm)
                                numOfPointNeedForChart = gVariable.totalPointNumForNoSPCChart;
                            else
                                numOfPointNeedForChart = totalDataNum; //we should not come to this place, just in case
                        }
                        else if (startID >= totalDataNum - 20)
                        {
                            startID = totalDataNum - gVariable.totalPointNumForNoSPCChart;
                            numOfPointNeedForChart = gVariable.totalPointNumForNoSPCChart;
                        }
                        else
                        {
                            startID -= 20;
                            numOfPointNeedForChart = gVariable.totalPointNumForNoSPCChart;
                        }
                    }
                }
                else //if (SPCFromAlarmOrQualityManagement == gVariable.FROM_QUALITY_MANAGEMENT_FUNC)   
                {
                    dispatchCode = gVariable.productTaskSheet[boardIndex].dispatchCode;

                    if (dispatchCode == "dummy" || gVariable.machineCurrentStatus[boardIndex] < gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                    {
                        MessageBox.Show("抱歉, 当前没有在产工单, 因此无法提供质量数据管理功能！", "信息提示", MessageBoxButtons.OK);
                        SPCResult = -1;
                        return -1;
                    }

                    analyzeChartType = toolClass.getQualityDataChartType(databaseName, tableName, dispatchCode, indexInTable);

                    if (analyzeChartType == gVariable.CHART_TYPE_SPC_XBAR_S)
                        numOfPointNeedForChart = gVariable.totalPointNumForSChart;   //for XBar-S, we need at least 270 data 
                    else if (analyzeChartType == gVariable.CHART_TYPE_SPC_C)
                    {   //for C chart
                        numOfPointNeedForChart = gVariable.totalPointNumForCChart;
                    }
                    else // if (analyzeChartType == gVariable.CHART_TYPE_NO_SPC)
                    {
                        //shoud not come to this place, this is an alarmtriggered craft data, should not come from quality management
                        numOfPointNeedForChart = gVariable.totalPointNumForNoSPCChart;
                    }

                    totalDataNum = mySQLClass.getRecordNumInTable(databaseName, gVariable.productTaskSheet[boardIndex].dispatchCode + gVariable.qualityTableNameAppendex);
                    startID = totalDataNum - numOfPointNeedForChart;
                    type = gVariable.ALARM_TYPE_QUALITY_DATA;  //just pretend this is a quality data alarm
                    indexInTable = qualityDataIndexSelected;  //which data item in quality data
                    tableName = gVariable.qualityListTableName;
                }

                //get the number of items this quality data support, maybe only 2, length and weight, maybe 10 ...
                dataItemNum = toolClass.getCraftQualityData(databaseName, tableName, dispatchCode, specUpperLimitArray, specLowerLimitArray);
                for (i = 0; i < dataItemNum; i++)
                {
                    //we put spec limitation here, for example, spec is 5 - 20, real value is 10,11,15,17,23,21,19,18, if we don't use spec limitation as column limitation,
                    //column step value go from 10 to 23, should be 5 to 23
                    gVariable.upperLimitValueForPie[i] = specUpperLimitArray[indexInTable];
                    gVariable.lowerLimitValueForPie[i] = specLowerLimitArray[indexInTable];
                }

                //2. get value 
                if (type == gVariable.ALARM_TYPE_QUALITY_DATA)
                {
                    tableName = dispatchCode + gVariable.qualityTableNameAppendex;
                    //read data for all these items into dataInPoint/timeInPoint/statusInPoint starting from ID startID
                    startDataIDInReading = mySQLClass.readSmallPartOfDataForSPC(databaseName, tableName, dataItemNum, numOfPointNeedForChart, dataInPoint, timeInPoint, statusInPoint, startID);
                    if (startDataIDInReading == -1) //not enough data
                    {
                        MessageBox.Show("抱歉质量数据个数太少，请等待有足够的数据后再查看 SPC 功能！", "信息提示", MessageBoxButtons.OK);
                        SPCResult = -1;
                        return -1;
                    }

                }
                else
                {
                    tableName = dispatchCode + gVariable.craftTableNameAppendex;
                    //read data for all these items into dataInPoint/timeInPoint starting from ID startID
                    startDataIDInReading = mySQLClass.readSmallPartOfDataForSPC(databaseName, tableName, dataItemNum, numOfPointNeedForChart, dataInPoint, timeInPoint, null, startID);
                    if (startDataIDInReading == -1) //not enough data
                    {
                        MessageBox.Show("抱歉质量数据个数太少，请等待有足够的数据后再查看 SPC 功能！", "信息提示", MessageBoxButtons.OK);
                        SPCResult = -1;
                        return -1;
                    }
                }

                //3. SPC calculation：for CP/PP/CPK/PPK/sigma/average calculation
                //analyzeChartType means what kind of chart we need to draw, could be XBar-S chart, C chart or NO SPC(original) chart  
                SPCFuncClass.CalculateSPC(specUpperLimitArray[indexInTable], specLowerLimitArray[indexInTable], indexInTable, dataInPoint, analyzeChartType);

                //4. get column chart info: for column/pie chart only
                toolClass.dataInCategory(indexInTable, numOfPointNeedForChart, dataInPoint);

                //5: get screen title
                if (SPCFromAlarmOrQualityManagement == gVariable.FROM_ALARM_DISPLAY_FUNC)
                {
                    //data alarm
                    if (category == gVariable.ALARM_CATEGORY_QUALITY_DATA_OVERFLOW || category == gVariable.ALARM_CATEGORY_CRAFT_DATA_OVERFLOW)
                    {
                        strArray = alarmTableStructImpl.errorDesc.Split(':');
                        errorItem = strArray[0];
                        groupBox1.Text = "工单" + dispatchCode + " 生产过程中" + errorItem; // +"超出警戒范围";
                    }
                    else if (category >= gVariable.ALARM_CATEGORY_SPC_DATA_START && category <= gVariable.ALARM_CATEGORY_SPC_DATA_LOCATE_APART)
                    {
                        strArray = alarmTableStructImpl.errorDesc.Split(':');
                        errorItem = strArray[0];
                        groupBox1.Text = "工单" + dispatchCode + " " + errorItem; // +" " + gVariable.errSPCDescList[category - gVariable.ALARM_CATEGORY_SPC_DATA_START];
                    }
                    else
                    {
                        strArray = alarmTableStructImpl.errorDesc.Split(':');
                        errorItem = strArray[0];
                        groupBox1.Text = "工单" + dispatchCode + " " + errorItem;
                    }
                }
                else //if (SPCFromAlarmOrQualityManagement == gVariable.FROM_QUALITY_MANAGEMENT_FUNC)
                {
                    i = Convert.ToInt32(databaseName.Remove(0, 1)) - 1;
                    groupBox1.Text = gVariable.machineNameArrayAPS[i] + " 订单号:";
                    groupBox1.Text += dispatchListArray[dispatchSelected] + " 质量数据: " + qualityDataItemArray[indexInTable];
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getAllDataNeeded() failed" + ex);
                return -1;
            }
        }

        //chart is a Microsoft package of curve drawing, we need to feed this packet with initial data, then it will draw some curve accordingly
        //this function feed the package wih all kinds of data needed 
        private void prepareAllCharts()
        {
            if (analyzeChartType == gVariable.CHART_TYPE_SPC_C)
            {
                //second parameter of null means there is no chart drawing function needed, we only need data
                SPCFuncClass.get_C_Chart(indexInTable, null, gVariable.SPC_DATA_ONLY, dataInPoint, timeInPoint, controlCenterValueArray);
                C_chart_Init(chart5);
            }
            else if (analyzeChartType == gVariable.CHART_TYPE_SPC_XBAR_S)
            {
                //second parameter of null means there is no chart drawing function needed, we only need data
                SPCFuncClass.get_XBar_S_Chart(indexInTable, null, gVariable.SPC_DATA_ONLY, dataInPoint, timeInPoint, controlCenterValueArray);
                XBar_S_chart_Init(chart5);  //generate 2 charts in this tabpage  
            }

            Ori_chart_Init(chart4);  //generate 1 charts in this tabpage  
            CPK_chart_Init(chart2);  //generate 1 charts in this tabpage  
            Average_chart_Init(chart3);  //generate 1 charts in this tabpage  
            Overall_chart_Init(chart1);  //generate 1 charts in this tabpage  
        }


        //For SPCAnalyze screen, the upper part is a curve, the lower part is a table of data, 
        //this function list all data in this table
        private void listView_Load()
        {
            const int len1 = 115;
            const int len2 = 65;
            const int len3 = 45;
            const int len4 = 64;
            const int NumOfDataOneLine = 5;
            int i, j;
            int flag;
            float v, sum, average, delta, max, min, sigma;
            string strTime;
            int[] illegalOrNot = new int[NumOfDataOneLine];

            try
            {
                this.listView1.BeginUpdate();
                listView1.GridLines = true;
                listView1.Dock = DockStyle.Fill;

                if (analyzeChartType == gVariable.CHART_TYPE_NO_SPC)
                {
                    listView1.Columns.Add(" ", 1, HorizontalAlignment.Left);
                    listView1.Columns.Add("序号", len4, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据产生时间", len1, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据值", len2, HorizontalAlignment.Left);
                    listView1.Columns.Add("序号", len4, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据产生时间", len1, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据值", len2, HorizontalAlignment.Left);
                    listView1.Columns.Add("序号", len4, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据产生时间", len1, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据值", len2, HorizontalAlignment.Left);
                    listView1.Columns.Add("序号", len4, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据产生时间", len1, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据值", len2, HorizontalAlignment.Left);
                    listView1.Columns.Add("序号", len4, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据产生时间", len1, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据值", len2, HorizontalAlignment.Left);

                    j = 0;
                    min = specLowerLimitArray[indexInTable];
                    max = specUpperLimitArray[indexInTable];
                    for (i = 0; i < numOfPointNeedForChart / NumOfDataOneLine; i++)
                    {
                        ListViewItem OptionItem = new ListViewItem();

                        for (j = 0; j < NumOfDataOneLine; j++)
                        {
                            strTime = toolClass.GetTime((timeInPoint[indexInTable, i * NumOfDataOneLine + j] - 3600 * 7).ToString()).ToString().Remove(0, 2);
                            OptionItem.SubItems.Add((i * NumOfDataOneLine + j + startID).ToString());
                            OptionItem.SubItems.Add(strTime);
                            v = dataInPoint[indexInTable, i * NumOfDataOneLine + j];

                            //mark illegal data for red color display
                            if (v < min || v > max)
                                illegalOrNot[j] = 1;
                            else
                                illegalOrNot[j] = 0;

                            OptionItem.SubItems.Add(v.ToString());
                        }

                        listView1.Items.Add(OptionItem);

                        listView1.Items[i].UseItemStyleForSubItems = false;

                        for (j = 0; j < NumOfDataOneLine; j++)
                        {
                            if (illegalOrNot[j] == 1)
                                listView1.Items[i].SubItems[j * 3 + 3].BackColor = Color.Red;
                            else
                                listView1.Items[i].SubItems[j * 3 + 3].BackColor = Color.LightGreen;
                        }
                    }
                }
                else if (analyzeChartType == gVariable.CHART_TYPE_SPC_C)
                {
//                    SPCFuncClass.get_C_Chart(indexInTable, null, gVariable.SPC_DATA_ONLY, dataInPoint, timeInPoint, controlCenterValueArray);

                    listView1.Columns.Add(" ", 1, HorizontalAlignment.Left);
                    listView1.Columns.Add("序号", len3, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据产生时间", len1, HorizontalAlignment.Left);
                    listView1.Columns.Add("不良数", len2, HorizontalAlignment.Left);
                    listView1.Columns.Add("序号", len3, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据产生时间", len1, HorizontalAlignment.Left);
                    listView1.Columns.Add("不良数", len2, HorizontalAlignment.Left);
                    listView1.Columns.Add("序号", len3, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据产生时间", len1, HorizontalAlignment.Left);
                    listView1.Columns.Add("不良数", len2, HorizontalAlignment.Left);
                    listView1.Columns.Add("序号", len3, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据产生时间", len1, HorizontalAlignment.Left);
                    listView1.Columns.Add("不良数", len2, HorizontalAlignment.Left);
                    listView1.Columns.Add("序号", len3, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据产生时间", len1, HorizontalAlignment.Left);
                    listView1.Columns.Add("不良数", len2, HorizontalAlignment.Left);

                    j = 0;
                    for (i = 0; i < gVariable.numOfGroupsInCChart; i++)
                    {
                        ListViewItem OptionItem = new ListViewItem();

                        strTime = toolClass.GetTime((timeInPoint[indexInTable, j] - 3600 * 7).ToString()).ToString().Remove(0, 2);
                        OptionItem.SubItems.Add((j + 1).ToString());
                        OptionItem.SubItems.Add(strTime);
                        v = dataInPoint[indexInTable, j];
                        OptionItem.SubItems.Add(v.ToString());
                        j++;

                        strTime = toolClass.GetTime((timeInPoint[indexInTable, j] - 3600 * 7).ToString()).ToString().Remove(0, 2);
                        OptionItem.SubItems.Add((j + 1).ToString());
                        OptionItem.SubItems.Add(strTime);
                        v = dataInPoint[indexInTable, j];
                        OptionItem.SubItems.Add(v.ToString());
                        j++;

                        strTime = toolClass.GetTime((timeInPoint[indexInTable, j] - 3600 * 7).ToString()).ToString().Remove(0, 2);
                        OptionItem.SubItems.Add((j + 1).ToString());
                        OptionItem.SubItems.Add(strTime);
                        v = dataInPoint[indexInTable, j];
                        OptionItem.SubItems.Add(v.ToString());
                        j++;

                        strTime = toolClass.GetTime((timeInPoint[indexInTable, j] - 3600 * 7).ToString()).ToString().Remove(0, 2);
                        OptionItem.SubItems.Add((j + 1).ToString());
                        OptionItem.SubItems.Add(strTime);
                        v = dataInPoint[indexInTable, j];
                        OptionItem.SubItems.Add(v.ToString());
                        j++;

                        strTime = toolClass.GetTime((timeInPoint[indexInTable, j] - 3600 * 7).ToString()).ToString().Remove(0, 2);
                        OptionItem.SubItems.Add((j + 1).ToString());
                        OptionItem.SubItems.Add(strTime);
                        v = dataInPoint[indexInTable, j];
                        OptionItem.SubItems.Add(v.ToString());
                        j++;

                        listView1.Items.Add(OptionItem);
                    }
                }
                else //if (analyzeChartType == gVariable.CHART_TYPE_SPC_XBAR_S)
                {
//                    SPCFuncClass.get_XBar_S_Chart(indexInTable, null, gVariable.SPC_DATA_ONLY, dataInPoint, timeInPoint, controlCenterValueArray);

                    listView1.Columns.Add(" ", 1, HorizontalAlignment.Left);
                    listView1.Columns.Add("序号", len3, HorizontalAlignment.Left);
                    listView1.Columns.Add("数据产生时间", len1, HorizontalAlignment.Left);

                    for (i = 0; i < gVariable.pointNumInSChartGroup; i++)
                        listView1.Columns.Add("样本" + (i + 1), len2, HorizontalAlignment.Left);

                    //            listView1.Columns.Add("状态", len3, HorizontalAlignment.Left);
                    listView1.Columns.Add("均值", len2, HorizontalAlignment.Left);
                    listView1.Columns.Add("极差", len2, HorizontalAlignment.Left);
                    listView1.Columns.Add("标准差", len2, HorizontalAlignment.Left);
                    listView1.Columns.Add("最大值", len2, HorizontalAlignment.Left);
                    listView1.Columns.Add("最小值", len2, HorizontalAlignment.Left);

                    for (i = 0; i < gVariable.numOfGroupsInSChart; i++)
                    {
                        ListViewItem OptionItem = new ListViewItem();

                        strTime = toolClass.GetTime((timeInPoint[indexInTable, i * gVariable.pointNumInSChartGroup] - 3600 * 7).ToString()).ToString().Remove(0, 2);
                        OptionItem.SubItems.Add((i + startID).ToString());
                        OptionItem.SubItems.Add(strTime);

                        sum = 0;
                        delta = 0;
                        sigma = 0;
                        max = 0;
                        min = 0;

                        for (j = 0; j < gVariable.pointNumInSChartGroup; j++)
                        {
                            v = dataInPoint[indexInTable, i * gVariable.pointNumInSChartGroup + j];
                            OptionItem.SubItems.Add(v.ToString());

                            sum += v;
                            if (j == 0)
                            {
                                max = v;
                                min = v;
                            }
                            if (v > max)
                                max = v;
                            else if (v < min)
                                min = v;
                        }

                        average = sum / gVariable.pointNumInSChartGroup;

                        for (j = 0; j < gVariable.pointNumInSChartGroup; j++)
                        {
                            v = dataInPoint[indexInTable, i * gVariable.pointNumInSChartGroup + j];

                            sigma += (v - average) * (v - average);
                        }

                        delta = max - min;
                        sigma = (float)System.Math.Sqrt(sigma / (gVariable.pointNumInSChartGroup - 1));

                        //                OptionItem.SubItems.Add("合格");
                        OptionItem.SubItems.Add(average.ToString());
                        OptionItem.SubItems.Add(delta.ToString());
                        OptionItem.SubItems.Add(sigma.ToString());
                        OptionItem.SubItems.Add(max.ToString());
                        OptionItem.SubItems.Add(min.ToString());
                        listView1.Items.Add(OptionItem);

                        listView1.Items[i].UseItemStyleForSubItems = false;

                        flag = 0;
                        for (j = 0; j < gVariable.pointNumInSChartGroup; j++)
                        {
                            if (statusInPoint[indexInTable, i * gVariable.pointNumInSChartGroup + j] != 0)
                            {
                                listView1.Items[i].SubItems[j + 3].BackColor = Color.Red;

                                if (flag == 0)
                                {
//                                    if (type == gVariable.ALARM_TYPE_QUALITY_DATA)
                                        listView1.Items[i].SubItems[3 + gVariable.pointNumInSChartGroup + j].BackColor = Color.Red;  //the third column of average data item should be in red color
//                                    else //if(type == gVariable.ALARM_TYPE_QUALITY_DATA2)
//                                        listView1.Items[i].SubItems[3 + gVariable.pointNumInSChartGroup + 2].BackColor = Color.Red;  //the fifth column of variance data item should be in red color

                                    flag = 1;
                                }
                            }
                            else
                            {
                                listView1.Items[i].SubItems[j + 3].BackColor = Color.LightGreen;
                            }
                        }
                    }
                }
                this.listView1.EndUpdate();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SPC analyze load listview failed" + ex);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.tabControl1.SelectedIndex)
            {
                case TAB_CONTROL_SPC_CURVE_DATA:
                    if (category == gVariable.ALARM_CATEGORY_QUALITY_DATA_OVERFLOW || category == gVariable.ALARM_CATEGORY_CRAFT_DATA_OVERFLOW)
                    {
                        tabControl1.SelectedIndex = tabControlIndex;
                        return;
                    }
                    tabControlIndex = TAB_CONTROL_SPC_CURVE_DATA;
                    break;
                case TAB_CONTROL_CPK_DATA:
                    tabControlIndex = TAB_CONTROL_CPK_DATA;
                    break;
                case TAB_CONTROL_ORIGINAL_DATA:
                    tabControlIndex = TAB_CONTROL_ORIGINAL_DATA;
                    break;
                case TAB_CONTROL_AVERAGE_DATA:
                    if (category == gVariable.ALARM_CATEGORY_QUALITY_DATA_OVERFLOW || category == gVariable.ALARM_CATEGORY_CRAFT_DATA_OVERFLOW)
                    {
                        tabControl1.SelectedIndex = tabControlIndex;
                        return;
                    }
                    tabControlIndex = TAB_CONTROL_AVERAGE_DATA;
                    break;
                case TAB_CONTROL_ALL_DATA:
                    tabControlIndex = TAB_CONTROL_ALL_DATA;
                    break;
            }

            drawCurrentChart();
        }

        private void chart4_MouseMove(object sender, MouseEventArgs e)
        {
            int i;
            string str;

            HitTestResult myTestResult = chart4.HitTest(e.X, e.Y); 
            if (tipFlag == 0 && myTestResult.ChartElementType == ChartElementType.DataPoint)  
            {  
//               this.Cursor = Cursors.Cross;
                i = myTestResult.PointIndex; 
                str = "序号: " + (startID + i) + "\r\n数据值: ";
                str += myTestResult.Series.Points[i].YValues[0].ToString("F3");

                toolTip1.SetToolTip(chart4, str);
                tipFlag = 1;
            }
            else
                tipFlag = 0;
        }

        private void chart5_MouseMove(object sender, MouseEventArgs e)
        {
            int i;
            string str;

            HitTestResult myTestResult = chart5.HitTest(e.X, e.Y);
            if (tipFlag == 0 && myTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                //               this.Cursor = Cursors.Cross;
                if (analyzeChartType == gVariable.CHART_TYPE_NO_SPC)
                {

                }
                else if (analyzeChartType == gVariable.CHART_TYPE_SPC_C)
                {
                }
                else //if (analyzeChartType == gVariable.CHART_TYPE_SPC_XBAR_S)
                {
                }
                i = myTestResult.PointIndex;
                str = "序号: " + (startID + i) + "\r\n数据值: ";
                str += myTestResult.Series.Points[i].YValues[0].ToString("F3");

                toolTip1.SetToolTip(chart5, str);
                tipFlag = 1;
            }
            else
                tipFlag = 0;
        }

        private void chart4_MouseLeave(object sender, MouseEventArgs e)
        {
//            toolTipController.HideHint();
        }  

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int i;
            int line, column;
            float center1, center2;
            string s, str, title, unit;
            float f, min, max, min1, max1, min2, max2;

            try
            {
                ListViewHitTestInfo info = listView1.HitTest(e.X, e.Y);
                if (info.Item == null) //no item in listview selected
                    return;

                line = info.Item.Index + 1;
                column = info.Item.SubItems.IndexOf(info.SubItem);

                if (analyzeChartType == gVariable.CHART_TYPE_NO_SPC)
                {
                    //for no spc chart, this means a chart with craft data is clicked 
                    if (column % 3 == 0)
                    {
                        f = (float)Convert.ToDouble(info.SubItem.Text);
                        min = specLowerLimitArray[indexInTable];
                        max = specUpperLimitArray[indexInTable];
                        title = gVariable.craftList[boardIndex].paramName[indexInTable];
                        unit = gVariable.craftList[boardIndex].paramUnit[indexInTable];
                        if (f < min || f > max)
                        {
                            str = "数据含义：" + title + "\r\n数据状态：异常，数据超出规格限\r\n数据值：" + f + unit + "\r\n规格上限：" + max + "\r\n规格下限：" + min + "\r\n处理状态：" + gVariable.strAlarmStatus[alarmTableStructImpl.status];
                            MessageBox.Show(str, "异常数据点详情", MessageBoxButtons.OK);
                        }
                        else
                        {
                            str = "数据含义：" + title + "\r\n数据状态：数据正常\r\n数据值：" + f + unit + "\r\n规格上限：" + max + "\r\n规格下限：" + min;
                            MessageBox.Show(str, "数据点详情", MessageBoxButtons.OK);
                        }
                    }
                    else  //data time or index is clicked, so just return
                        return;
                }
                else if (analyzeChartType == gVariable.CHART_TYPE_SPC_C)
                {
                    //for no spc chart, this means a chart with craft data is clicked 
                    if (column % 3 == 0)
                    {
                        f = (float)Convert.ToDouble(info.SubItem.Text);
                        min = gVariable.qualityList[boardIndex].specLowerLimit[indexInTable];
                        max = gVariable.qualityList[boardIndex].specUpperLimit[indexInTable];
                        min1 = gVariable.qualityList[boardIndex].controlLowerLimit1[indexInTable];
                        center1 = gVariable.qualityList[boardIndex].controlLowerLimit1[indexInTable];
                        max1 = gVariable.qualityList[boardIndex].controlUpperLimit1[indexInTable];
                        title = gVariable.craftList[boardIndex].paramName[indexInTable];
                        unit = gVariable.craftList[boardIndex].paramUnit[indexInTable];

                        i = statusInPoint[indexInTable, (line - 1) * gVariable.pointNumInCChartGroup + column - 3];
                        if (i != gVariable.ALARM_CATEGORY_NORMAL)
                        {
                            i = i - gVariable.ALARM_CATEGORY_SPC_DATA_START;
                            if (i < 0 || i >= gVariable.NUM_OF_ALARM_CATEGORY_SPC)  //category error, don't didplay anything
                                return;
                            s = gVariable.errSPCDescList[i];

                            str = "数据含义：" + title + "\r\n数据状态：异常，" + s + "\r\n数据值：" + f + unit + "\r\n规格上限：" + max + "\r\n规格下限：" + min +
                                 "\r\nC图控制上限：" + max1 + "\r\nC图控制中心：" + center1 + "\r\nC图控制下限：" + min1 + "\r\n安灯处理状态：" + gVariable.strAlarmStatus[alarmTableStructImpl.status];
                            MessageBox.Show(str, "异常数据点详情", MessageBoxButtons.OK);
                        }
                        else
                        {
                            str = "数据含义：" + title + "\r\n数据状态：正常\r\n数据值：" + f + unit + "\r\n规格上限：" + max + "\r\n规格下限：" + min +
                                 "\r\nC图规格上限：" + max1 + "\r\nC图控制中心：" + center1 + "\r\nC图规格下限：" + min1 + "\r\n安灯处理状态：" + gVariable.strAlarmStatus[alarmTableStructImpl.status];
                            MessageBox.Show(str, "数据点详情", MessageBoxButtons.OK);
                        }
                    }
                    else  //data time or index is clicked, so just return
                        return;
                }
                else //if (analyzeChartType == gVariable.CHART_TYPE_SPC_XBAR_S)
                {
                    //for no spc chart, this means a chart with craft data is clicked 
                    if (column >= 3 && column < 3 + gVariable.pointNumInSChartGroup)
                    {
                        f = (float)Convert.ToDouble(info.SubItem.Text);
                        min = gVariable.qualityList[boardIndex].specLowerLimit[indexInTable];
                        max = gVariable.qualityList[boardIndex].specUpperLimit[indexInTable];
                        min1 = gVariable.qualityList[boardIndex].controlCenterValue1[indexInTable];
                        center1 = gVariable.qualityList[boardIndex].controlLowerLimit1[indexInTable];
                        max1 = gVariable.qualityList[boardIndex].controlUpperLimit1[indexInTable];
                        min2 = gVariable.qualityList[boardIndex].controlLowerLimit2[indexInTable];
                        center2 = gVariable.qualityList[boardIndex].controlCenterValue2[indexInTable];
                        max2 = gVariable.qualityList[boardIndex].controlUpperLimit2[indexInTable];
                        title = gVariable.qualityList[boardIndex].checkItem[indexInTable];
                        unit = gVariable.qualityList[boardIndex].unit[indexInTable];

                        i = statusInPoint[indexInTable, (line - 1) * gVariable.pointNumInSChartGroup + column - 3];
                        if (i != gVariable.ALARM_CATEGORY_NORMAL)
                        {
                            i = i - gVariable.ALARM_CATEGORY_SPC_DATA_START;
                            if (i < 0 || i >= gVariable.NUM_OF_ALARM_CATEGORY_SPC)  //category error, don't didplay anything
                                return;

                            s = gVariable.errSPCDescList[i];

                            str = "数据含义：" + title + "\r\n数据状态：异常，" + s + "\r\n数据值：" + f + unit + "\r\n规格上限：" + max + "\r\n规格下限：" + min +
                                 "\r\nXBar图控制上限：" + max1 + "\r\nXBar图控制中心：" + center1 + "\r\nXBar图控制下限：" + min1 + "\r\nS图控制上限：" + max2 + "\r\nS图控制中心：" +
                                 center2 + "\r\nS图控制下限：" + min2 + "\r\n安灯处理状态：" + gVariable.strAlarmStatus[alarmTableStructImpl.status];
                            MessageBox.Show(str, "异常数据点详情", MessageBoxButtons.OK);
                        }
                        else
                        {
                            str = "数据含义：" + title + "\r\n数据状态：正常\r\n数据值：" + f + unit + "\r\n规格上限：" + max + "\r\n规格下限：" + min +
                                 "\r\nXBar图规格上限：" + max1 + "\r\nXBar图控制中心：" + center1 + "\r\nXBar图规格下限：" + min1 + "\r\nS图规格上限：" + max2 +
                                 "\r\nS图控制中心：" + center2 + "\r\nS图规格下限：" + min2 + "\r\n安灯处理状态：" + gVariable.strAlarmStatus[alarmTableStructImpl.status];
                            MessageBox.Show(str, "数据点详情", MessageBoxButtons.OK);
                        }
                    }
                    else  //data time or index is clicked, so just return
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("listView1_MouseDoubleClick():" + ex.ToString());
            }
        }


        //when header column is clicked, we will enter this function, we can change the sorting order of the listview
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
//            int index, column;
//            ColumnHeader header;

            if (this.listView1.SelectedItems.Count > 0)
            {
                //            index = this.listView1.SelectedItems[0].Index;
                //            column = listView1.Columns[e.Column].Index;
            }
        }

        // We set ownerDraw to true, so we need to draw entire ListView items by ourselves.
        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            e.DrawText();
        }

        // We set ownerDraw to true, so we need to draw entire ListView subitems by ourselves.
        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            TextFormatFlags flags = TextFormatFlags.Left;

            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                flags = TextFormatFlags.HorizontalCenter;

                // Draw the text and background
                e.DrawBackground();
                e.DrawText(flags);
            }
        }

        // We set ownerDraw to true, so we need to draw column header by ourselves.
        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;

            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;

                // Draw the standard header background.
                e.DrawBackground();
                e.DrawText(flags);

                // Draw the header text.
//                using (Font headerFont = new Font("Helvetica", 9, FontStyle.Bold))
                {
//                    e.Graphics.DrawString(e.Header.Text, headerFont, Brushes.Black, e.Bounds, sf);
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
//            int index;
            if (this.listView1.SelectedItems.Count > 0)
            {
                //            index = this.listView1.SelectedItems[0].Index;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            machineSelected = comboBox1.SelectedIndex;
            databaseName = gVariable.DBHeadString + (machineSelected + 1).ToString().PadLeft(3, '0');
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            dispatchSelected = comboBox2.SelectedIndex;
            tableName = dispatchListArray[dispatchSelected] + "_quality";
            dispatchCode = dispatchListArray[dispatchSelected];
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            qualityDataIndexSelected = comboBox3.SelectedIndex;
            indexInTable = qualityDataIndexSelected;
            analyzeChartType = toolClass.getQualityDataChartType(databaseName, gVariable.qualityListTableName, dispatchCode, indexInTable);
        }
    }
}
