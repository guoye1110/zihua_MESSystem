using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MESSystem.alarmFun;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MESSystem.common;
using MESSystem.commonControl;
using MESSystem.communication;

namespace MESSystem.mainUI
{
    public partial class workshopReport : Form
    {
        System.Windows.Forms.Timer aTimer;
        public static workshopReport workshopReportClass = null; //用来引用主窗口

        const int NUM_WORKSHOP = 3;
        const int NUM_MACHINE_ONE_WORKSHOP = 5;

        //        const int MAX_OUTPUT_ONE_DISPATCH = 200;

        public const int POLLING_CYCLE_FOR_HEART_BEAT = 10;
        public const int WORKER_INFO_FILE_INDEX = 0;
        public const int PRODUCT_INFO_FILE_INDEX = 1;

        int x1, x2, x3, x4, x6, x7, x8, x9;

        string timeFrame;

        int[] numForDispatch = new int[NUM_MACHINE_ONE_WORKSHOP];
        int[] numOutput = new int[NUM_MACHINE_ONE_WORKSHOP];
        int[] qualifiedNumber = new int[NUM_MACHINE_ONE_WORKSHOP];
        int[] unqualifiedNumber = new int[NUM_MACHINE_ONE_WORKSHOP];

        ListViewNF[] listViewArray = new ListViewNF[NUM_WORKSHOP];
        Chart[] pieChartArray = new Chart[NUM_WORKSHOP];
        Chart[] curveChartArray = new Chart[NUM_WORKSHOP];
        Chart[] columnChartArray = new Chart[NUM_WORKSHOP];
        GroupBox[] workshopGroupBoxArray = new GroupBox[NUM_WORKSHOP];
        GroupBox[] listGroupBoxArray = new GroupBox[NUM_WORKSHOP];
        Label[,] labelNumArray = new Label[NUM_WORKSHOP, NUM_MACHINE_ONE_WORKSHOP];
        Label[,] labelIndexArray = new Label[NUM_WORKSHOP, NUM_MACHINE_ONE_WORKSHOP];

        int[,] machineIndexInLine = 
        {
            { 0, 1, 2, 3, 4},
            { 5, 6, 7, 8, 9},
            { 10, 11, 12, 13, 14},
        };

        const string listGroupBoxTitle = "生产状况总表";

        public static string[] machineNameArray23 =
        {
            "S数控剃齿机",
            "高精度数控车床", "卧轴圆台平面磨床", "数控内圆端面磨床", "数控齿轮倒棱机", "滚齿机", "数控内圆端面磨床", "哈挺精密数控车床", "高精度数控车床", "高精度数控车床", "数控内圆端面磨床", 
            "高速精度数控车床", "立式加工中心", "磨齿机", "高精度数控车床", "数控内圆磨床", "内圆磨床", "高速精密度数控车床", "高速精密数控车床", "数控内圆端面磨床", "高精度数控车床", 
            "磨齿机", "磨齿机", "磨齿机", "哈挺精密数控机床", "卧轴圆台平面磨床", "磨齿机", "车磨中心", "内圆磨床", "磨齿机", "磨齿机", 
            "磨齿机", "数控内圆磨床", "磨齿机", "磨齿机", "普通内圆磨床", "磨齿机", "磨齿机", "磨齿机", "磨齿机", "磨齿机", 
            "卧轴圆台平面磨床", "高速精度数控车床", "卧轴圆台平面磨床", "半自动外圆磨床", "数控内圆端面磨床", "内圆磨床", "磨齿机", "万能外圆磨床", "磨齿机", "卧轴圆台平面磨床", 
            "数控钻铣床", "磨齿机", "高精度数控车床", "磨齿机", "内圆磨床", "数控内圆端面磨床", "磨齿机", "磨齿机", "高精度数控车床", "内圆磨床", 
            "设备未定", "滚齿机", "数控内圆端面磨床", "数控内圆端面磨床", "数控内圆端面磨床", "设备未定", "设备未定", "设备未定", "设备未定", "磨齿机", 
            "磨齿机", "设备未定", "设备未定", "S数控剃齿机", "设备未定", "设备未定", "设备未定", "数控高效滚齿机", "磨齿机", "数控倒棱机", 
            "设备未定", "数控滚齿机", "设备未定", "哈挺精密数控车床", "设备未定", "数控内圆端面磨床", "磨齿机", "立式加工中心", "卧轴圆台端面磨床", "数控卧式车床", 
            "数控滚齿机", "数控自动滚齿机", "立式加工中心", "设备未定", "数控高效滚齿机", "立式加工中心", "数控高效滚齿机", "车磨中心", "滚齿机", "卧轴圆台端面磨床", 
            "数控内圆端面磨床", "数控内圆端面磨床", "内圆磨床", "数控滚齿机", "数控齿轮倒角机", "数控滚齿机", "磨齿机", "数控径向剃齿机", "数控高效滚齿机", "数控高效滚齿机", 
            "哈挺精密数控车床", "数控自动滚齿机", "数控剃齿机", "数控齿轮倒棱机", "内圆磨床", "数控内圆端面磨床", "卧轴圆台端面磨床", "S数控剃齿机", "磨齿机", "数控椎度齿插齿机", 
            "卧式内拉床", "设备未定", "卧轴圆台端面磨床", "卧式拉床", "数控滚齿机", "立式加工中心", "数控滚齿机", "数控滚齿机", "数控插齿机", "齿轮倒角机", 
            "数控高效滚齿机", "卧轴圆台端面磨床", "设备未定", "设备未定", "设备未定", "设备未定", "设备未定", "设备未定", "设备未定", "设备未定"
        };

        string[] machineStatusArray = { "关机", "待机", "生产", "报警", "维修" };
        int[] machineStatusNumArray = { 5, 0, 0, 0, 0 };

        string[] outputArray = { "计划", "完工", "合格", "不合格" };
        int[] outputNumArray = { 0, 0, 0, 0 };

        int oneCurveScreenSize;
        int verticalLineEveryNumOfPoint;

        Color backGroundColor;

        //        Rectangle percentageRectBar = new Rectangle();

        SolidBrush colorGreenBrush = new SolidBrush(Color.Lime);
        SolidBrush colorGrayBrush = new SolidBrush(Color.DarkGray);  //not working

        public workshopReport()
        {
            InitializeComponent();
            resizeForScreen();

            if (gVariable.workshopReport == gVariable.WORKSHOP_REPORT_BULLETIN)
                initializeVariables1();
            initializeVariables2();
        }

        void initializeVariables1()
        {
            int i, j;
            int errListSize = 18;

            try
            {
                gVariable.userAccount = "admin";
                gVariable.wifiErrorNum = 0;
                gVariable.refreshMultiCurve = 0;
                gVariable.willClose = 0;  //indicating whether winform will be closed
                gVariable.SPCChartIndex = 0;
                gVariable.whereComesTheSettingData = gVariable.SETTING_DATA_FROM_TOUCHPAD;
                gVariable.whatSettingDataModified = gVariable.NO_SETTING_DATA_TO_BOARD;

                gVariable.mainFunctionIndex = gVariable.MAIN_FUNCTION_PRODUCTION;

                gVariable.DBHeadString = "h";

                mySQLClass mySQL = new mySQLClass();

                //we are now working witrh the newest dispatch
                gVariable.contemporarydispatchUI = 1;

                //            gVariable.startingSerialNumber = 0;
                //            gVariable.nextSerialNumber = 0;

                gVariable.gpioStatus = 0xffff;
                gVariable.emailForwarderSocket = null;
                gVariable.emailForwarderHeartBeatNum = 0;
                gVariable.emailForwarderHeartBeatOld = 0;
                //gVariable.SPCDataNotEnough = 0;
                gVariable.APSScreenRefresh = 0;
                gVariable.beatDataForCurveIndex = 0;

                gVariable.clientPCConnectionNumber = 0;
                gVariable.activeAlarmTotalNumber = 0;
                gVariable.activeAlarmInfoUpdatedLocally = 0;
                gVariable.activeAlarmInfoUpdatedCounterpart = 0;

                for (i = 0; i < gVariable.maxActiveAlarmNum; i++)
                {
                    gVariable.activeAlarmIDArray[i] = 0;
                    gVariable.activeAlarmNewStatus[i] = gVariable.ACTIVE_ALARM_OLD_ALARM;
                }

                gVariable.clientSocket = null;

                for (i = 0; i < gVariable.totalFileTypeNum; i++)
                    gVariable.currentDataIndex[i] = 0;

                for (i = 0; i < gVariable.maxMachineNum + 1; i++)
                {
                    gVariable.IDForLastAlarmByMachine[i] = -1;
                    gVariable.IDForLastAlarmByFactory[i] = -1;
                    gVariable.SPCCheckingThreadRunning[i] = 0;
                    gVariable.SPCTriggeredAlarmArray[i] = -1;
                    gVariable.dataForThisBoardIsUnderSPCControl[i] = 0;
                    gVariable.currentValueNow[i] = 0;
                    gVariable.machineCommunicationType[i] = gVariable.typeDataOnlyFromBoard;  //default to app controls dispatch apply/start/complete
                    gVariable.machineCurrentStatus[i] = gVariable.MACHINE_STATUS_SHUTDOWN;
                    gVariable.internalMachineName[i] = gVariable.DBHeadString + (i + 1).ToString().PadLeft(3, '0');

                    gVariable.newQualityDataArrivedFlag[i] = 0;

                    for (j = 0; j < 1440; j++)
                        gVariable.dispatchAlarmIDForOneDay[i, j] = -1;

                }

                for (i = 0; i < gVariable.ALARM_TYPE_TOTAL_NUM; i++)
                {
                    for (j = 0; j < (gVariable.maxMachineNum + 1); j++)
                        gVariable.typeAlarmAlreadyAlive[i, j] = 0;
                }

                for (i = 0; i < gVariable.numOfWorkshop; i++)
                {
                    for (j = 0; j < gVariable.totalButtonNum; j++)
                        gVariable.buttonBoardIndexTable[i, j] = -1;
                }

                gVariable.worldStartTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 1, 1, 1));

                initDispatchQualityCraftData();

                initSettingData();

                //            for (i = 0; i < gVariable.maxActiveAlarmNum; i++)
                //gVariable.semaphoreForAlarmPush[i] = new Semaphore(0, 1);

                gVariable.andonAlarmIndex = 0;

                gVariable.errorCodeList.errorCodeDesc = new string[errListSize];
                gVariable.errorCodeList.errorCode = new string[errListSize];

                gVariable.errorCodeList.errorCodeListSize = errListSize;
                for (i = 0; i < errListSize; i++)
                {
                    gVariable.errorCodeList.errorCodeDesc[i] = gConstText.deviceErrDescList[i];
                    gVariable.errorCodeList.errorCode[i] = gConstText.deviceErrNoList[i];
                }

                //            if (gVariable.debugMode != 0)
                {
                    toolClass.writeFileHeader();
                }

                //for alarm list view
                gVariable.MACHINE_NAME_ALL_FOR_SELECTION = gVariable.machineNameArrayTouchScreen.Length;
                gVariable.ALARM_TYPE_ALL_FOR_SELECTION = gVariable.strAlarmTypeForSelection.Length - 1;
                gVariable.ALARM_STATUS_ALL_FOR_SELECTION = gVariable.strAlarmStatusForSelection.Length - 1;

                if (gVariable.workshopReport == gVariable.WORKSHOP_REPORT_BULLETIN)
                {
                    this.Icon = new Icon("..\\..\\resource\\my-icon.ico");
                }
                else
                {
                    this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("initializeVariables1 failed! !" + ex);
            }
        }


        void initializeVariables2()
        {
            int i, j;
            string today;
            int y1, y2;
            int delta1, delta2;
            Label[,] labelNumArrayInternal = 
            {
                {label6,  label7,  label8,  label9,  label10},
                {label11, label12, label13, label14, label15},
                {label16, label17, label18, label19, label20},
            };

            Label[,] labelIndexArrayInternal = 
            {
                {label21, label22, label23, label24, label25},
                {label26, label27, label28, label29, label30},
                {label31, label32, label33, label34, label35},
            };

            try
            {
                today = DateTime.Now.Date.ToString("yyyy-MM-dd");

                for (i = 0; i < gVariable.maxMachineNum; i++)
                {
                    gVariable.today[i] = today;
                    gVariable.today_old[i] = today;
                }

                listViewArray[0] = listView1;
                listViewArray[1] = listViewNF1;
                listViewArray[2] = listViewNF2;

                pieChartArray[0] = chart3;
                pieChartArray[1] = chart6;
                pieChartArray[2] = chart9;

                curveChartArray[0] = chart2;
                curveChartArray[1] = chart5;
                curveChartArray[2] = chart8;

                columnChartArray[0] = chart1;
                columnChartArray[1] = chart4;
                columnChartArray[2] = chart7;

                workshopGroupBoxArray[0] = groupBox1;
                workshopGroupBoxArray[1] = groupBox3;
                workshopGroupBoxArray[2] = groupBox5;

                listGroupBoxArray[0] = groupBox2;
                listGroupBoxArray[1] = groupBox4;
                listGroupBoxArray[2] = groupBox6;

                for (i = 0; i < NUM_WORKSHOP; i++)
                {
                    for (j = 0; j < NUM_MACHINE_ONE_WORKSHOP; j++)
                    {
                        labelNumArray[i, j] = labelNumArrayInternal[i, j];
                        labelIndexArray[i, j] = labelIndexArrayInternal[i, j];
                    }
                }

                verticalLineEveryNumOfPoint = 6;

                y1 = 125;
                y2 = 136;
                delta1 = 2;
                delta2 = 12;
                x1 = 36;
                x2 = 108;
                x3 = 60;
                x4 = 90;
                x6 = 60;
                x7 = 60;
                x8 = 48;
                x9 = 48;

                this.BackColor = ColorTranslator.FromHtml("#193B4B");
                this.backGroundColor = Color.Transparent;

                for (i = 0; i < NUM_WORKSHOP; i++)
                {
                    listViewArray[i].Size = new System.Drawing.Size(x1 + x2 + x3 + x4 + x6 + x7 + x8 + x9 + delta1, y1);
                }


                for (i = 0; i < NUM_WORKSHOP; i++)
                {
                    workshopGroupBoxArray[i].BackColor = backGroundColor;
                    workshopGroupBoxArray[i].ForeColor = Color.White;

                    listGroupBoxArray[i].ForeColor = Color.White;
                    listGroupBoxArray[i].Text = listGroupBoxTitle;
                    listGroupBoxArray[i].Size = new System.Drawing.Size(x1 + x2 + x3 + x4 + x6 + x7 + x8 + x9 + delta2, y2);

                    curveChartArray[i].BackColor = backGroundColor;
                    initColumnChart(columnChartArray[i]);
                    initCurveChart(curveChartArray[i]);
                    initPieChart(pieChartArray[i]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("initializeVariables2 failed! !" + ex);
            }
        }

        private void resizeForScreen()
        {
        }


        void initSettingData()
        {
            int i, index;

            for (index = 0; index < gVariable.maxMachineNum; index++)
            {
                gVariable.ADCChannelInfo[index].channelEnabled = new int[gVariable.MAX_NUM_ADC];
                gVariable.ADCChannelInfo[index].channelTitle = new string[gVariable.MAX_NUM_ADC];
                gVariable.ADCChannelInfo[index].channelUnit = new string[gVariable.MAX_NUM_ADC];
                gVariable.ADCChannelInfo[index].lowerRange = new float[gVariable.MAX_NUM_ADC];
                gVariable.ADCChannelInfo[index].upperRange = new float[gVariable.MAX_NUM_ADC];
                gVariable.ADCChannelInfo[index].lowerLimit = new float[gVariable.MAX_NUM_ADC];
                gVariable.ADCChannelInfo[index].upperLimit = new float[gVariable.MAX_NUM_ADC];

                gVariable.ADCChannelInfo[index].workingVoltage = 0;  // 5V

                gVariable.GPIOSettingInfo[index].GPIOTriggerVoltage = new int[gVariable.numOfGPIOs];
                gVariable.GPIOSettingInfo[index].GPIOName = new string[gVariable.numOfGPIOs];

                for (i = 0; i < gVariable.numOfGPIOs; i++)
                {
                    gVariable.GPIOSettingInfo[index].GPIOTriggerVoltage[i] = 0;
                    gVariable.GPIOSettingInfo[index].GPIOName[i] = "备用";
                }

                gVariable.uartSettingInfo[index].uartBaudrate = new int[gVariable.MAX_NUM_UART];
                gVariable.uartSettingInfo[index].uartInputData = new string[gVariable.MAX_NUM_UART];
                gVariable.uartSettingInfo[index].uartOutputData = new string[gVariable.MAX_NUM_UART];

                for (i = 0; i < gVariable.MAX_NUM_UART; i++)
                {
                    gVariable.uartSettingInfo[index].uartBaudrate[i] = 4;  //115200
                }

                gVariable.beatPeriodInfo[index].deviceSelection = 0;

                gVariable.beatPeriodInfo[index].idleCurrentHigh = 0;
                gVariable.beatPeriodInfo[index].idleCurrentLow = 0;
                gVariable.beatPeriodInfo[index].workCurrentHigh = 0;
                gVariable.beatPeriodInfo[index].workCurrentLow = 0;
            }
        }

        void initDispatchQualityCraftData()
        {
            int i, index;

            for (index = 0; index < gVariable.maxMachineNum; index++)
            {
                gVariable.craftList[index].id = new int[gVariable.maxCraftParamNum];
                gVariable.craftList[index].paramName = new string[gVariable.maxCraftParamNum];
                gVariable.craftList[index].paramLowerLimit = new float[gVariable.maxCraftParamNum];
                gVariable.craftList[index].paramUpperLimit = new float[gVariable.maxCraftParamNum];
                gVariable.craftList[index].paramDefaultValue = new float[gVariable.maxCraftParamNum];
                gVariable.craftList[index].paramUnit = new string[gVariable.maxCraftParamNum];
                gVariable.craftList[index].paramValue = new float[gVariable.maxCraftParamNum];
                gVariable.craftList[index].rangeLowerLimit = new float[gVariable.maxCraftParamNum];
                gVariable.craftList[index].rangeUpperLimit = new float[gVariable.maxCraftParamNum];

                gVariable.qualityList[index].id = new int[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].checkItem = new string[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].checkRequirement = new string[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].controlCenterValue1 = new float[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].controlCenterValue2 = new float[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].specUpperLimit = new float[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].specLowerLimit = new float[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].controlUpperLimit1 = new float[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].controlUpperLimit2 = new float[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].controlLowerLimit1 = new float[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].controlLowerLimit2 = new float[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].sampleRatio = new int[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].checkResultData = new string[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].checkResult = new string[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].unit = new string[gVariable.maxQualityDataNum];
                gVariable.qualityList[index].chartType = new int[gVariable.maxQualityDataNum];

                //gVariable.materialList[index].materialName = new string[gVariable.maxMaterialTypeNum];
                gVariable.materialList[index].materialCode = new string[gVariable.maxMaterialTypeNum];
                gVariable.materialList[index].materialRequired = new int[gVariable.maxMaterialTypeNum];
                //gVariable.materialList[index].previousLeft = new int[gVariable.maxMaterialTypeNum];
                //gVariable.materialList[index].currentLeft = new int[gVariable.maxMaterialTypeNum];
                //gVariable.materialList[index].currentUsed = new int[gVariable.maxMaterialTypeNum];
                //gVariable.materialList[index].fullPackNum = new int[gVariable.maxMaterialTypeNum];

                gVariable.ADCChannelInfo[index].channelEnabled = new int[gVariable.MAX_NUM_ADC];
                gVariable.ADCChannelInfo[index].channelTitle = new string[gVariable.MAX_NUM_ADC];
                gVariable.ADCChannelInfo[index].channelUnit = new string[gVariable.MAX_NUM_ADC];
                gVariable.ADCChannelInfo[index].lowerRange = new float[gVariable.MAX_NUM_ADC];
                gVariable.ADCChannelInfo[index].upperRange = new float[gVariable.MAX_NUM_ADC];
                gVariable.ADCChannelInfo[index].lowerLimit = new float[gVariable.MAX_NUM_ADC];
                gVariable.ADCChannelInfo[index].upperLimit = new float[gVariable.MAX_NUM_ADC];
                gVariable.ADCChannelInfo[index].workingVoltage = 0;

                //initially all ADc are not enabled, we need to go to setting screen and enable one of the ADC channels
                for (i = 0; i < gVariable.MAX_NUM_ADC; i++)
                    gVariable.ADCChannelInfo[index].channelEnabled[i] = 0;
            }
        }


        private void report_Load(object sender, EventArgs e)
        {
            int ret;
            Thread thread1 = null;

            try
            {

                this.label1.BackColor = Color.Transparent;
                //this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                this.Text = gVariable.programTitle;

                ret = mySQLClass.databaseExistsOrNot(gVariable.DBHeadString + "001");
                if (gVariable.buildNewDatabase == 1 || ret == 0)
                {
                    mySQLClass.buildBasicDatabase();
                }

                this.Enabled = true;

                gVariable.basicmailListAlarm = toolClass.getAlarmMailList();
                //we use mailListAlarm as mail list, basicmailListAlarm is the initial mail list, every client can modify it to mailListAlarm
                gVariable.mailListAlarm = gVariable.basicmailListAlarm;

                gVariable.programTitle += "敏仪电子看板系统 --- 车间生产状况展示";

                thread1 = new Thread(new ThreadStart(communicate.comProccess));
                thread1.Start();

                this.Text = gVariable.programTitle;

                aTimer = new System.Windows.Forms.Timer();

                //refresh screen every 100 ms
                aTimer.Interval = 1000;
                aTimer.Enabled = true;

                aTimer.Tick += new EventHandler(timer_listview);

                this.Text = gVariable.programTitle;
                label5.Text = gVariable.programTitle;
            }
            catch (Exception ex)
            {
                Console.WriteLine("column_Load() in column function !" + ex);
            }
        }

        private void timer_listview(Object source, EventArgs e)
        {
            int i, j;
            int status;
            int machineIndex;
            string tableName;
            string databaseName;

            try
            {
                aTimer.Stop();

                label36.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                for (j = 0; j < NUM_WORKSHOP; j++)
                {
                    listViewArray[j].Clear();
                    listViewArray[j].BeginUpdate();
                    listViewArray[j].GridLines = true;
                    listViewArray[j].Dock = DockStyle.Fill;

                    listViewArray[j].Columns.Add(" ", 1, HorizontalAlignment.Center);
                    listViewArray[j].Columns.Add("序号", x1, HorizontalAlignment.Center);
                    listViewArray[j].Columns.Add("设备名称", x2, HorizontalAlignment.Left);
                    listViewArray[j].Columns.Add("设备状态", x3, HorizontalAlignment.Center);
                    listViewArray[j].Columns.Add("工单编号", x4, HorizontalAlignment.Center);
                    listViewArray[j].Columns.Add("目标产量", x6, HorizontalAlignment.Center);
                    listViewArray[j].Columns.Add("当前产量", x7, HorizontalAlignment.Center);
                    listViewArray[j].Columns.Add("合格品", x8, HorizontalAlignment.Center);
                    listViewArray[j].Columns.Add("不合格", x9, HorizontalAlignment.Center);

                    i = 0;

                    for (i = 0; i < NUM_MACHINE_ONE_WORKSHOP; i++)
                    {
                        ListViewItem OptionItem = new ListViewItem();

                        machineIndex = machineIndexInLine[j, i];

                        OptionItem.SubItems.Add((machineIndex + 1).ToString());
                        OptionItem.SubItems.Add(machineNameArray23[machineIndex]);
                        if (gVariable.machineCurrentStatus[machineIndex] < 0)
                            status = 0;
                        else if (gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DEVICE, machineIndex] != 0 || gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_MATERIAL, machineIndex] != 0)
                        {
                            status = 3;
                        }
                        else if (gVariable.machineCurrentStatus[machineIndex] <= gVariable.MACHINE_STATUS_DISPATCH_COMPLETED)
                        {
                            status = 1;
                        }
                        else //if (gVariable.machineCurrentStatus[machineIndex] > gVariable.MACHINE_STATUS_DISPATCH_COMPLETED)
                        {
                            status = 2;
                        }

                        OptionItem.SubItems.Add(machineStatusArray[status]);

                        timeFrame = DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "～" + DateTime.Now.AddHours(4).ToString("HH:mm");

                        OptionItem.SubItems.Add(gVariable.dispatchSheet[machineIndex].dispatchCode);
                        OptionItem.SubItems.Add(gVariable.dispatchSheet[machineIndex].plannedNumber.ToString());
                        OptionItem.SubItems.Add(gVariable.dispatchSheet[machineIndex].outputNumber.ToString());
                        OptionItem.SubItems.Add(gVariable.dispatchSheet[machineIndex].qualifiedNumber.ToString());
                        OptionItem.SubItems.Add(gVariable.dispatchSheet[machineIndex].unqualifiedNumber.ToString());

                        listViewArray[j].Items.Add(OptionItem);
                    }

                    listViewArray[j].EndUpdate();

                    for (i = 0; i < NUM_WORKSHOP; i++)
                    {
                        //last machine in a workshop will decide the beat and final output data
                        databaseName = gVariable.DBHeadString + (machineIndexInLine[i, NUM_MACHINE_ONE_WORKSHOP - 1] + 1).ToString().PadLeft(3, '0');
                        if (gVariable.dispatchSheet[(i + 1) * NUM_MACHINE_ONE_WORKSHOP - 1].dispatchCode == null)
                            tableName = null;
                        else
                            tableName = gVariable.dispatchSheet[(i + 1) * NUM_MACHINE_ONE_WORKSHOP - 1].dispatchCode + gVariable.beatTableNameAppendex;

                        pieChartForOutput(pieChartArray[i], i);
                        curveChartForDispatch(databaseName, tableName, curveChartArray[i], i);
                        columnChartForMachine(columnChartArray[i], i);
                    }
                }

                aTimer.Start();

            }
            catch (Exception ex)
            {
                Console.Write("workshop reports timer_listview() failed :" + ex);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int i, j;
            int index;
            int x, y;
            int delta;
            int width, height;
            int greenWidth;
            int beatNum;
            string databaseName;
            string tableName;

            try
            {
                delta = 18;

                x = 558;
                y = 72;
                width = 80;
                height = 11;

                for (j = 0; j < NUM_WORKSHOP; j++)
                {
                    for (i = 0; i < NUM_MACHINE_ONE_WORKSHOP; i++)
                    {
                        index = machineIndexInLine[j, i];

                        databaseName = gVariable.DBHeadString + (index + 1).ToString().PadLeft(3, '0');
                        if (gVariable.dispatchSheet[index].dispatchCode == null)
                            tableName = null;
                        else
                            tableName = gVariable.dispatchSheet[index].dispatchCode + gVariable.beatTableNameAppendex;

                        beatNum = gVariable.dispatchSheet[index].qualifiedNumber;
                        if (beatNum > gVariable.dispatchSheet[index].plannedNumber)
                            beatNum = gVariable.dispatchSheet[index].plannedNumber;

                        Graphics gGraphics = workshopGroupBoxArray[j].CreateGraphics();

                        //this version uses beat value as output number
                        if (gVariable.dispatchSheet[index].plannedNumber != 0)
                            greenWidth = beatNum * width / gVariable.dispatchSheet[index].plannedNumber;
                        else
                            greenWidth = 0;

                        if (greenWidth != 0)
                        {
                            gGraphics.FillRectangle(colorGreenBrush, x, y + i * delta, greenWidth, height);
                        }
                        else
                        {
                            greenWidth = 0;
                        }

                        if (greenWidth < width)
                        {
                            gGraphics.FillRectangle(colorGrayBrush, x + greenWidth, y + i * delta, width - greenWidth, height);
                        }

                        labelNumArray[j, i].Location = new System.Drawing.Point(x + width + 5, y + i * delta);
                        labelNumArray[j, i].Text = beatNum.ToString() + "/" + gVariable.dispatchSheet[index].plannedNumber.ToString();

                        labelIndexArray[j, i].Location = new System.Drawing.Point(x - 21, y + i * delta);
                        labelIndexArray[j, i].Text = (index + 1).ToString() + ":";
                    }
                }

                base.OnPaint(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine("dispatchUI OnPaint failed!" + ex);
            }
        }


        //draw a column chart
        private void initColumnChart(Chart chart)
        {
            try
            {
                //标题
                chart.Titles.Add("产量实时报表");
                chart.Titles[0].ForeColor = Color.White;
                chart.Titles[0].Font = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                chart.Titles[0].Alignment = ContentAlignment.TopCenter;
                //chart.Titles[1].ForeColor = Color.White;
                //chart.Titles[1].Font = new Font("微软雅黑", 8f, FontStyle.Regular);
                //chart.Titles[1].Alignment = ContentAlignment.TopRight;

                //控件背景
                //chart.BackColor = Color.Transparent;
                chart.BackColor = backGroundColor;
                //图表区背景
                chart.ChartAreas[0].BackColor = Color.Transparent;
                chart.ChartAreas[0].BorderColor = Color.Transparent;
                //X轴标签间距
                chart.ChartAreas[0].AxisX.Interval = 1;
                chart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = true;
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                chart.ChartAreas[0].AxisX.TitleFont = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                chart.ChartAreas[0].AxisX.TitleForeColor = Color.White;

                //X坐标轴颜色
                chart.ChartAreas[0].AxisX.LineColor = ColorTranslator.FromHtml("#38587a"); ;
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
                chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("微软雅黑", 8.25f, FontStyle.Regular);

                //X坐标轴标题
                //chart.ChartAreas[0].AxisX.Title = "数量(宗)";
                //chart.ChartAreas[0].AxisX.TitleFont = new Font("微软雅黑", 10f, FontStyle.Regular);
                //chart.ChartAreas[0].AxisX.TitleForeColor = Color.White;
                //chart.ChartAreas[0].AxisX.TextOrientation = TextOrientation.Horizontal;
                //chart.ChartAreas[0].AxisX.ToolTip = "数量(宗)";
                //X轴网络线条
                chart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");

                //Y坐标轴颜色
                chart.ChartAreas[0].AxisY.LineColor = ColorTranslator.FromHtml("#38587a");
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
                chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                //Y坐标轴标题
                chart.ChartAreas[0].AxisY.Title = "数量";
                chart.ChartAreas[0].AxisY.TitleFont = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                chart.ChartAreas[0].AxisY.TitleForeColor = Color.White;
                chart.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Rotated270;
                chart.ChartAreas[0].AxisY.ToolTip = "数量";
                //Y轴网格线条
                chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");

                chart.ChartAreas[0].AxisY2.LineColor = Color.Transparent;

                chart.ChartAreas[0].Area3DStyle.Enable3D = true;
                chart.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
                Legend legend = new Legend("legend");
                legend.Title = "legendTitle";

                chart.Series[0].XValueType = ChartValueType.String;  //设置X轴上的值类型
                chart.Series[0].Label = "#VAL";                //设置显示X Y的值    
                chart.Series[0].LabelForeColor = Color.White;
                chart.Series[0].ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
                chart.Series[0].ChartType = SeriesChartType.Column;    //图类型(折线)

                chart.Series[0].XValueType = ChartValueType.String;
                chart.Series[0].YValueType = ChartValueType.Int32;

                chart.Series[0].Color = Color.Lime;
                chart.Series[0].LegendText = legend.Name;
                chart.Series[0].IsValueShownAsLabel = true;
                chart.Series[0].LabelForeColor = Color.White;
                chart.Series[0].CustomProperties = "DrawingStyle = Cylinder";
                chart.Legends.Add(legend);
                chart.Legends[0].Position.Auto = false;
            }
            catch (Exception e)
            {
                Console.Write("init column charts failed :" + e);
            }
        }


        private void columnChartForMachine(Chart chart, int workshopIndex)
        {
            int machineIndex;

            //get the machine index for the last machine in a workshop
            machineIndex = machineIndexInLine[workshopIndex, NUM_MACHINE_ONE_WORKSHOP - 1];
            try
            {
                outputNumArray[0] = gVariable.dispatchSheet[machineIndex].plannedNumber;
                outputNumArray[1] = gVariable.dispatchSheet[machineIndex].outputNumber;
                outputNumArray[2] = gVariable.dispatchSheet[machineIndex].qualifiedNumber;
                outputNumArray[3] = gVariable.dispatchSheet[machineIndex].unqualifiedNumber;

                //绑定数据
                chart.Series[0].Points.DataBindXY(outputArray, outputNumArray);
                chart.Series[0].Points[0].Color = Color.White;
                chart.Series[0].Palette = ChartColorPalette.Bright;
            }
            catch (Exception e)
            {
                Console.Write("draw column charts failed :" + e);
            }
        }

        private void initCurveChart(Chart chart)
        {
            this.Text = "工单生产节拍";
            Color[] colorArray = { Color.Green, Color.Red, Color.Black, Color.Blue };

            try
            {
                chart.Series[0].ChartArea = "ChartArea1";
                chart.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                chart.Series[0].Color = System.Drawing.Color.MediumBlue;
                chart.Series[0].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
                chart.Series[0].LabelBackColor = System.Drawing.Color.Transparent;
                chart.Series[0].LabelBorderColor = System.Drawing.Color.Transparent;
                chart.Series[0].Legend = "Legend3";
                chart.Series[0].MarkerColor = System.Drawing.Color.Blue;
                chart.Series[0].MarkerSize = 8;
                chart.Series[0].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
                chart.Series[0].Name = "test";
                chart.Series[0].ShadowOffset = 2;
                chart.Series[0].ChartType = SeriesChartType.Spline; //Line is streight line, SpLine is curve
                chart.Series[0].MarkerStyle = MarkerStyle.Diamond; //shape of the data point
                chart.Series[0].Color = colorArray[0];
                chart.Series[0].BorderWidth = 2;
                chart.Series[0].ShadowOffset = 1;
                chart.Series[0].IsVisibleInLegend = false; // true; //whether we need a description of the data in curve
                chart.Series[0].IsValueShownAsLabel = false;
                chart.Series[0].MarkerSize = 4; // size of the data point
                chart.Series[0].XValueType = ChartValueType.String;
                chart.Series[0].YValueType = ChartValueType.Int32;

                chart.Visible = true;
                chart.Titles.Add("生产节拍(秒)");
                chart.Titles[0].ForeColor = Color.White;
                chart.Titles[0].Font = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                chart.BackColor = backGroundColor;

                //chart.ChartAreas[0].AxisY.LabelStyle.Format = "N1";
                chart.ChartAreas[0].AxisY.IsStartedFromZero = true;  //whether we need to start at 0 for Y axis
                chart.ChartAreas[0].AxisY.IntervalOffset = 1;
                chart.ChartAreas[0].AxisY.Maximum = 1000;
                chart.ChartAreas[0].AxisY.Minimum = 0;
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
                chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                //Y坐标轴标题
                chart.ChartAreas[0].AxisY.TitleFont = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                chart.ChartAreas[0].AxisY.TitleForeColor = Color.White;
                //chart1.ChartAreas[0].AxisX.IsStartedFromZero = true;  //whether we need to start at 0 for X axis

                chart.ChartAreas[0].AlignmentOrientation = ((AreaAlignmentOrientations)((AreaAlignmentOrientations.Vertical | AreaAlignmentOrientations.Horizontal)));
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
                chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                chart.ChartAreas[0].AxisX.Interval = oneCurveScreenSize / verticalLineEveryNumOfPoint;
                chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                chart.ChartAreas[0].AxisX.LabelAutoFitStyle = ((LabelAutoFitStyles)((LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap)));
                chart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = false;
                chart.ChartAreas[0].AxisX.LabelStyle.Interval = oneCurveScreenSize / verticalLineEveryNumOfPoint;
                chart.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 0;
                chart.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[0].AxisX.LineWidth = 2;
                chart.ChartAreas[0].AxisX.TitleFont = new Font("Sans Serif", 10, FontStyle.Bold);
                //chart.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Blue;
                chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chart.ChartAreas[0].AxisX.MajorTickMark.LineDashStyle = ChartDashStyle.NotSet;
                chart.ChartAreas[0].AxisX.Minimum = 1;
                chart.ChartAreas[0].AxisX.ScaleView.MinSizeType = DateTimeIntervalType.Number;
                chart.ChartAreas[0].AxisX.ScaleView.Size = oneCurveScreenSize;
                chart.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            }
            catch (Exception ex)
            {
                Console.WriteLine("initCurveChart() failed" + ex);
            }

        }


        private void curveChartForDispatch(string databaseName, string tableName, Chart chart, int workshopIndex)
        {
            int i, index;
            float f;
            float delta, tmpMax, tmpMin;
            DateTime dateTime;
            string xString;
            int totalDataNumInBuffer;  //total number of data need to be dislayed, only part of totalDataNumWanted because of the limitation of oneCurveScreenSize 
            int initialDatPointNum;  //initial point number for a curve

            tmpMax = 0;
            tmpMin = 0;
            initialDatPointNum = 25;

            totalDataNumInBuffer = 0;
            index = machineIndexInLine[workshopIndex, NUM_MACHINE_ONE_WORKSHOP - 1];
            if (gVariable.machineCurrentStatus[index] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED)
                totalDataNumInBuffer = mySQLClass.readOneTableForMoreData(databaseName, tableName, 25); //MAX_OUTPUT_ONE_DISPATCH);

            if (totalDataNumInBuffer < initialDatPointNum)
                oneCurveScreenSize = initialDatPointNum;
            else
                oneCurveScreenSize = totalDataNumInBuffer;

            chart.ChartAreas[0].AxisX.ScaleView.Size = oneCurveScreenSize;

            try
            {
                foreach (var series in chart.Series)
                {
                    series.Points.Clear();
                }

                if (totalDataNumInBuffer == 0)
                {
                    xString = DateTime.Now.ToString("MM-dd HH:mm");

                    chart.Series[0].Points.AddXY(xString, 0);
                }
                else
                {
                    for (i = 0; i < totalDataNumInBuffer; i++)
                    {
                        dateTime = toolClass.GetTime((gVariable.timeInPoint[0, i] - 3600 * 7).ToString());
                        xString = dateTime.ToString("MM-dd HH:mm:ss");
                        f = gVariable.dataInPoint[0, i];
                        chart.Series[0].Points.AddXY(xString, f);

                        if (i == 0)
                        {
                            tmpMax = f;
                            tmpMin = f;
                        }

                        if (f > tmpMax)
                            tmpMax = f;

                        if (f < tmpMin)
                            tmpMin = f;
                    }
                }

                if (tmpMax < 10)
                    tmpMax = 10;

                delta = tmpMax - tmpMin;
                tmpMax += delta / 7;
                tmpMin = 0;

                if (tmpMax == tmpMin)
                {
                    tmpMax += 1;
                    tmpMin -= 1;
                }

                chart.ChartAreas[0].AxisY.Maximum = tmpMax;
                chart.ChartAreas[0].AxisY.Minimum = tmpMin;
            }
            catch (Exception ex)
            {
                Console.Write("curveChartForDispatch failed! " + ex);
            }
        }

        private void initPieChart(Chart chart)
        {
            try
            {
                //int i;
                //int pointIndex;
                //DataPoint dataPoint;
                //string str;

                //标题
                //chart.Titles.Add("产量");
                //chart.Titles[0].ForeColor = Color.White;
                //chart.Titles[0].Font = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                //chart.Titles[0].Alignment = ContentAlignment.TopCenter;
                //chart.Titles[1].ForeColor = Color.White;
                //chart.Titles[1].Font = new Font("微软雅黑", 8f, FontStyle.Regular);
                //chart.Titles[1].Alignment = ContentAlignment.TopRight;

                //控件背景
                //chart.BackColor = Color.Transparent;
                chart.BackColor = backGroundColor;
                //图表区背景
                chart.ChartAreas[0].BackColor = Color.Transparent;
                chart.ChartAreas[0].BorderColor = Color.Transparent;
                //X轴标签间距
                chart.ChartAreas[0].AxisX.Interval = 1;
                chart.ChartAreas[0].AxisX.LabelStyle.IsStaggered = true;
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                chart.ChartAreas[0].AxisX.TitleFont = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                chart.ChartAreas[0].AxisX.TitleForeColor = Color.White;

                //X坐标轴颜色
                chart.ChartAreas[0].AxisX.LineColor = ColorTranslator.FromHtml("#38587a"); ;
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
                chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                //X坐标轴标题
                chart.ChartAreas[0].AxisX.Title = "数量";
                chart.ChartAreas[0].AxisX.TitleFont = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                chart.ChartAreas[0].AxisX.TitleForeColor = Color.White;
                chart.ChartAreas[0].AxisX.TextOrientation = TextOrientation.Horizontal;
                chart.ChartAreas[0].AxisX.ToolTip = "数量";
                //X轴网络线条
                chart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");

                //Y坐标轴颜色
                chart.ChartAreas[0].AxisY.LineColor = ColorTranslator.FromHtml("#38587a");
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
                chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                //Y坐标轴标题
                chart.ChartAreas[0].AxisY.Title = "数量";
                chart.ChartAreas[0].AxisY.TitleFont = new Font("微软雅黑", 8.25f, FontStyle.Regular);
                chart.ChartAreas[0].AxisY.TitleForeColor = Color.White;
                chart.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Rotated270;
                chart.ChartAreas[0].AxisY.ToolTip = "数量";
                //Y轴网格线条
                chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");

                chart.ChartAreas[0].AxisY2.LineColor = Color.Transparent;

                //背景渐变
                chart.ChartAreas[0].Area3DStyle.Enable3D = true;
                chart.ChartAreas[0].BackGradientStyle = GradientStyle.None;

                //图例样式
                Legend legend2 = new Legend();
                legend2.Title = "设备状态";
                legend2.TitleBackColor = backGroundColor;
                legend2.BackColor = backGroundColor;
                legend2.TitleForeColor = Color.White;
                legend2.TitleFont = new Font("微软雅黑", 7.8f, FontStyle.Regular);
                legend2.Font = new Font("微软雅黑", 7.8f, FontStyle.Regular);
                legend2.ForeColor = Color.White;

                chart.Series[0].XValueType = ChartValueType.String;  //设置X轴上的值类型
                //chart.Series[0].Label = "#VAL";                //设置显示X Y的值    
                chart.Series[0].LabelForeColor = Color.White;
                chart.Series[0].ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
                chart.Series[0].ChartType = SeriesChartType.Pie;    //图类型(折线)

                chart.Series[0].Color = Color.Lime;
                chart.Series[0].LegendText = legend2.Name;
                chart.Series[0].IsValueShownAsLabel = true;
                chart.Series[0].LabelForeColor = Color.Transparent;
                chart.Series[0].CustomProperties = "DrawingStyle = Cylinder";
                chart.Series[0].CustomProperties = "PieLabelStyle = Disabled";

                legend2.Name = "Legend1";
                legend2.Position.Auto = false;
                legend2.Position.Height = 85F;
                legend2.Position.Width = 33F;
                legend2.Position.X = 76F;
                legend2.Position.Y = 3F;
                chart.Legends.Add(legend2);

                //chart.Legends.Add(legend2);
                //chart.Legends[0].Position.Auto = true;
                chart.Series[0].IsValueShownAsLabel = false;
                //是否显示图例
                chart.Series[0].IsVisibleInLegend = true;
                chart.Series[0].ShadowOffset = 0;

                //饼图折线
                chart.Series[0]["PieLineColor"] = "White";
            }
            catch (Exception e)
            {
                Console.Write("pie init error :" + e);
            }
        }


        private void pieChartForOutput(Chart chart, int workshopIndex)
        {
            int i;
            int machineIndex;

            try
            {
                for (i = 0; i < machineStatusArray.Length; i++)
                {
                    machineStatusNumArray[i] = 0;
                }

                for (i = 0; i < NUM_MACHINE_ONE_WORKSHOP; i++)
                {
                    machineIndex = machineIndexInLine[workshopIndex, i];

                    if (gVariable.machineCurrentStatus[machineIndex] < 0)
                        machineStatusNumArray[0]++;
                    else if (gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DEVICE, machineIndex] != 0 || gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_MATERIAL, machineIndex] != 0)
                    {
                        machineStatusNumArray[3]++;
                    }
                    else if (gVariable.machineCurrentStatus[machineIndex] <= gVariable.MACHINE_STATUS_DISPATCH_COMPLETED)
                    {
                        machineStatusNumArray[1]++;
                    }
                    else //if (gVariable.machineCurrentStatus[machineIndex] > gVariable.MACHINE_STATUS_DISPATCH_COMPLETED)
                    {
                        machineStatusNumArray[2]++;
                    }
                }

                //绑定数据
                chart.Series[0].Points.DataBindXY(machineStatusArray, machineStatusNumArray);

                chart.Series[0].Points[0].Color = System.Drawing.Color.Silver;
                chart.Series[0].Points[1].Color = System.Drawing.Color.Yellow;
                chart.Series[0].Points[2].Color = System.Drawing.Color.Lime;
                chart.Series[0].Points[3].Color = System.Drawing.Color.Red;
                chart.Series[0].Points[4].Color = System.Drawing.Color.Black;

                //chart.Series[0].Points[0].Color = Color.White;
                //绑定颜色
                //chart.Series[0].Palette = ChartColorPalette.BrightPastel;
            }
            catch (Exception e)
            {
                Console.Write("draw pie error :" + e);
            }
        }


        private void report_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (aTimer != null)
                aTimer.Enabled = false;

            System.Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mySQLClass.buildBasicDatabase();
            System.Environment.Exit(0);
        }
    }
}
