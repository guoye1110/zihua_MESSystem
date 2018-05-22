using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using MESSystem.common;
using MESSystem.alarmFun;
using MESSystem.quality;
using MESSystem.APS_UI;
using MESSystem.commonControl;
using MESSystem.OEEManagement;
using MESSystem.communication;
using MESSystem.dispatchManagement;
using MESSystem.materialManagement;
//using exceptionTest;

namespace MESSystem.mainUI
{
    public partial class firstScreen : Form
    {
        const int LABEL_NUM = 1;

        const int USER_ENTER_SYSTEM = 0;
        const int USER_EXIT_SYSTEM = 1;
        const int USER_PRIVILEGE_SETTING = 2;

        public static firstScreen firstScreenClass = null; //it is used to reference this windows

        System.Windows.Forms.Timer aTimer;

        const int numOfworkshop = 4;
        int previousMinuteValue;

        Label[] labelArray = new Label[LABEL_NUM];

        int errListSize = 18;
        int emailPollingCycleTime;

        public const int POLLING_CYCLE_FOR_HEART_BEAT = 10;
        public const int WORKER_INFO_FILE_INDEX = 0;
        public const int PRODUCT_INFO_FILE_INDEX = 1;

        private OEEFactory factory;
        
        public firstScreen()
        {
            InitializeComponent();
            InitializeComponent2();

            //use double video buffer to output display contents to screen, one for processing display data, and one for output, to avoid flickering
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            resizeForScreen();
            InitializeVariables();

            //getMachineNameAndPos();
            //buttonSetting();
        }

        private void InitializeComponent2()
        {
//            string logo;

            mySQLClass mySQL = new mySQLClass();

            label1.Text = gVariable.programTitle;

            switch (gVariable.CompanyIndex)
            {
                case gVariable.ZIHUA_ENTERPRIZE:
                    gVariable.DBHeadString = "h";
                    break;
                default:
                    break;
            }
            BackgroundImage = Image.FromFile(gVariable.backgroundArray[gVariable.CompanyIndex]);
            this.Icon = new Icon(gVariable.logoInTitleArray[gVariable.CompanyIndex]);
        }

        private void resizeForScreen()
        {
            int tmpDPI;
            float floatFont;
            float[,] fontArray = 
            {
                {26F, 29F, 32F, 34F, 36F, 38F},
                {21F, 24F, 27F, 28F, 30F, 32F},
                {18F, 20F, 22F, 24F, 25F, 27F},
            };
            float[] menuFontSize = { 12F, 10F, 8F };
            int[] menuSizeY = { 33, 27, 23 };

            //try
            {
                Rectangle rect = new Rectangle();

                rect = Screen.GetWorkingArea(this);
                gVariable.screenRatioX = (float)rect.Width / gVariable.SMALLEST_SCREEN_X;
                gVariable.screenRatioY = (float)rect.Height / gVariable.SMALLEST_SCREEN_Y;

                tmpDPI = toolClass.getScalingFactorForScreen();

                switch (tmpDPI)
                {
                    case gVariable.SMALLER_DPI:
                        gVariable.dpiValue = 0;
                        gVariable.dpiRatioX = (float)rect.Width / gVariable.SMALLEST_SCREEN_X;
                        gVariable.dpiRatioY = (float)rect.Height / gVariable.SMALLEST_SCREEN_Y;
                        Console.WriteLine("screen font scale is small DPI, effective screen resolution is " + rect.Width + " * " + rect.Height);
                        break;
                    case gVariable.MEDIUM_DPI:
                        gVariable.dpiValue = 1;
                        gVariable.dpiRatioX = (float)rect.Width / gVariable.SMALLEST_SCREEN_X / 1.25F;
                        gVariable.dpiRatioY = (float)rect.Height / gVariable.SMALLEST_SCREEN_Y / 1.25F;
                        Console.WriteLine("screen font scale is medium DPI, effective screen resolution is " + rect.Width + " * " + rect.Height);
                        break;
                    case gVariable.LARGER_DPI:
                        gVariable.dpiValue = 2;
                        gVariable.dpiRatioX = (float)rect.Width / gVariable.SMALLEST_SCREEN_X / 1.5F;
                        gVariable.dpiRatioY = (float)rect.Height / gVariable.SMALLEST_SCREEN_Y / 1.5F;
                        Console.WriteLine("screen font scale is large DPI, effective screen resolution is " + rect.Width + " * " + rect.Height);
                        break;
                }

                myMenu1.Font = new System.Drawing.Font("Segoe UI", menuFontSize[gVariable.dpiValue]);
                myMenu1.Size = new System.Drawing.Size(1426, menuSizeY[gVariable.dpiValue]);

                gVariable.onePointstandForHowManyMinutes = 2;
                if (rect.Width < gVariable.resolutionLevelValue1) // < 1024 * 768 not really supported
                {
                    gVariable.resolutionLevel = gVariable.resolution_1024;
                }
                else if (rect.Width < gVariable.resolutionLevelValue2)  //1280 * 800
                {
                    gVariable.resolutionLevel = gVariable.resolution_1280;
                }
                else if (rect.Width < gVariable.resolutionLevelValue3)  //1366 * 768
                {
                    gVariable.resolutionLevel = gVariable.resolution_1366;
                }
                else if (rect.Width < gVariable.resolutionLevelValue4)  //1440 * 900
                {
                    gVariable.resolutionLevel = gVariable.resolution_1440;
                }
                else if (rect.Width < gVariable.resolutionLevelValue5)  //1600 * 900
                {
                    gVariable.resolutionLevel = gVariable.resolution_1600;
                }
                else //if (rect.Width >= gVariable.resolutionLevelValue5)  //1920 * 1080
                {
                    gVariable.resolutionLevel = gVariable.resolution_1920;
                    gVariable.onePointstandForHowManyMinutes = 1;
                }

                floatFont = fontArray[gVariable.dpiValue, gVariable.resolutionLevel];
                this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", floatFont, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            }
            //catch (Exception ex)
            {
                //Console.WriteLine("system init failed! " + ex);
            }

        }

        private void InitializeVariables()
        {
            int i, j;
            int ret;
            string today;

            previousMinuteValue = -1;

            gVariable.userAccount = "admin";
            gVariable.wifiErrorNum = 0;
            gVariable.refreshMultiCurve = 0;
            gVariable.willClose = 0;  //indicating whether winform will be closed
            gVariable.SPCChartIndex = 0;
            gVariable.whereComesTheSettingData = gVariable.SETTING_DATA_FROM_TOUCHPAD;
            gVariable.whatSettingDataModified = gVariable.NO_SETTING_DATA_TO_BOARD;

            gVariable.mainFunctionIndex = gVariable.MAIN_FUNCTION_PRODUCTION;

            //we are now working witrh the newest dispatch
            gVariable.contemporarydispatchUI = 1;

//            gVariable.startingSerialNumber = 0;
//            gVariable.nextSerialNumber = 0;

            gVariable.gpioStatus = 0xffff;
            gVariable.emailForwarderSocket = null;
            emailPollingCycleTime = 0;
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
                gVariable.machineCommunicationType[i] = gVariable.typeAllFromBoard;  //default to board controls dispatch apply/start/complete, android app only review data
                gVariable.machineCurrentStatus[i] = gVariable.MACHINE_STATUS_SHUTDOWN;
                gVariable.internalMachineName[i] = gVariable.DBHeadString + (i + 1).ToString().PadLeft(3, '0');

                gVariable.newQualityDataArrivedFlag[i] = 0;

                for (j = 0; j < 1440; j++)
                    gVariable.dispatchAlarmIDForOneDay[i, j] = -1;

            }

            for(i = 0; i < gVariable.ALARM_TYPE_TOTAL_NUM; i++)
            {
                for (j = 0; j < (gVariable.maxMachineNum + 1); j++)
                    gVariable.typeAlarmAlreadyAlive[i, j] = 0;
            }

            if (gVariable.thisIsHostPC == true)  //only a server program can initilize our database, client can only read it.
            {
                //it will clear old database and generate new one
                //need to be disabled when database already exist and don't want to be cleared
                ret = mySQLClass.databaseExistsOrNot(gVariable.DBHeadString + "001");
                if (gVariable.buildNewDatabase == 1 || ret == 0)
                {
                    //(gVariable.buildNewDatabase == 1) means no matter database exist or not, we need to clear and regenerate database
                    //(ret == 0), we didnot detect database, so generate it
                    mySQLClass.buildBasicDatabase();
                }
                else if (gVariable.buildNewDatabase == 2)  //build missing database and alarm table
                {
                    //                    if (checkEmployeeAndProductInfo() == 0)
                    //                        mySQLClass.buildMissingDatabase();
                }
            }

            toolClass.getMachineInfoFromDatabase();

            //gVariable.maxMachineNum = gVariable.machineNameArrayDatabase.Length;

            gVariable.worldStartTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 1, 1, 1));

            initDispatchQualityCraftData();

            initSettingData();

            //gVariable.semaphoreForAndonPush = new Semaphore();

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

            today = DateTime.Now.Date.ToString("yyyy-MM-dd");

            for (i = 0; i < gVariable.maxMachineNum; i++)
            {
                gVariable.today[i] = today;
                gVariable.today_old[i] = today;
            }

            //for alarm list view
            gVariable.MACHINE_NAME_ALL_FOR_SELECTION = gVariable.machineNameArrayAPS.Length;
            gVariable.ALARM_TYPE_ALL_FOR_SELECTION = gVariable.strAlarmTypeForSelection.Length - 1;
            gVariable.ALARM_STATUS_ALL_FOR_SELECTION = gVariable.strAlarmStatusForSelection.Length - 1;

            //get employee name and workerID info into global arrays
            toolClass.getWorkerNameAndIDArray();

            factory = new OEEFactory();

//            internalTestFunction();
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

                for (i = 0; i< gVariable.MAX_NUM_UART; i++)
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

        void TestForBeatFunc()
        {
            int i;

            for (i = 1; i <= gVariable.maxMachineNum; i++)
            {
                //                mySQLClass.readDatabaseToFile(gVariable.internalMachineName[i]);
            }
        }

        private void internalTestFunction()
        {
            //fanucClass.connectToFanucDevice("192.168.1.100", "8900", 1000);

            //test code are put here
            //gVariable.activeAlarmInstanceArray[0] = new SetAlarmClass("1121", "z30", "error desc", "err112333", "machine name", "operator", "2016-0807", 0, "workshop", "server PC");
            //gVariable.activeAlarmInstanceArray[0].Show();

            //tell this alarm class its index in alarm ring buffer
            //gVariable.activeAlarmInstanceArray[0].setIndexInRingBuffer(0);

            //test for email function
            //string[] MailList = { "tandidi@foxmail.com", "leeluojm@126.com" };
            //oolClass.sendEmail(MailList, "department 1");
            //mySQLClass.writeAlarmTable("z31", "1121_alarm", "aaaa", "1121", "ad-0011", "Z31", "bbb", "cccc", "2016-08-05 12:34:05", 0, "dddd", " ", " ");
        }

        private void firstScreen_Load(object sender, EventArgs e)
        {
            //int ret;
            Thread thread1 = null;

            this.label1.BackColor = Color.Transparent;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Text = gVariable.programTitle;

            //login function before the user enter the main screen
            login.loginClass = new login(USER_ENTER_SYSTEM);
            login.loginClass.Show();
            this.Enabled = false;
            if(login.loginClass.enterLoginScreen() < 0)
                System.Environment.Exit(0);

            this.Enabled = true;

            gVariable.basicmailListAlarm = toolClass.getAlarmMailList();
            //we use mailListAlarm as mail list, basicmailListAlarm is the initial mail list, every client can modify it to mailListAlarm
            gVariable.mailListAlarm = gVariable.basicmailListAlarm;

            //if this is a server PC, we start a new thread for communication with data collect board and a server thread to communication with client PC
            if (gVariable.thisIsHostPC == true)
            {
                gVariable.programTitle += "(服务器版)";

                thread1 = new Thread(new ThreadStart(communicate.comProccess));
                thread1.Start();
            }
            else
            {
                gVariable.programTitle += "(客户端版)";

                //if this is a client PC, we only start a alarm communication process
                thread1 = new Thread(new ThreadStart(communicate.clientPCFuncion));
                thread1.Start();
            }
            this.Text = gVariable.programTitle;

            if (gVariable.workshopReport == gVariable.WORKSHOP_REPORT_FUNCTION)
            {
                workshopReport.workshopReportClass = new workshopReport();
                workshopReport.workshopReportClass.Show();
            }

            aTimer = new System.Windows.Forms.Timer();

            //refresh screen every 100 ms
            aTimer.Interval = 1000;
            aTimer.Enabled = true;

            aTimer.Tick += new EventHandler(firstScreenTimePolling);
        }

        //this is a main cycle loop function that will will always working(other form functions may stop if the form is closed), it has the following jobs:
        //1. Record current value of all machines for every minute in database
        //2. Record current machine status for all machines for every minute, and if a dispatch is started or a alarm is triggered, record its ID also in database
        //3. Check for new alarm, if an alarm comes, display it on screen
        private void firstScreenTimePolling(Object source, EventArgs e)
        {
            int i;
            int minute;
            int stamp;
            int id;
            int value;
            float f;
            //string tableName;
            //string fileName;

            try
            {
                emailPollingCycleTime++;

                if (emailPollingCycleTime > POLLING_CYCLE_FOR_HEART_BEAT)
                    emailPollingCycleTime = 0;
                id = 0;
                value = 0;
                minute = Convert.ToInt16(DateTime.Now.ToString("mm"));

                //a minute passed, we need to write current(amphere) value into database
                if (minute != previousMinuteValue)  
                {
                    previousMinuteValue = minute;

//                    Console.Write(DateTime.Now + "  --  ");
                    for (i = 0; i < gVariable.maxMachineNum + 1; i++)
                    {
                        if (gVariable.connectionStatus[i] == 0)
                        {
                            f = 0;
                            id = 0;
                            value = 0;
                        }
                        else
                        {
                            //this is recent current value recorded by vol/cur module 
                            f = gVariable.currentValueNow[i];

                            switch(gVariable.machineCurrentStatus[i])
                            {
                                case gVariable.MACHINE_STATUS_SHUTDOWN:   //machine shut down
                                    value = gVariable.MACHINE_STATUS_DOWN;
                                    id = 0;
                                    break;
                                case gVariable.MACHINE_STATUS_DISPATCH_DUMMY:   //machine in idle mode, no dispatch 
                                case gVariable.MACHINE_STATUS_DISPATCH_COMPLETED:  //completed means we are in dummy mode, need apply for a new dispatch
                                    value = gVariable.MACHINE_STATUS_IDLE;
                                    id = 1;
                                    break;
                                case gVariable.MACHINE_STATUS_DISPATCH_APPLIED:   //machine in idle mode, dispatch applied
                                    value = gVariable.MACHINE_STATUS_IDLE;
                                    id = mySQLClass.getRecordNumInTable(gVariable.internalMachineName[i], gVariable.dispatchListTableName); //get id for the newest dispatch (applied, but not started)  
                                    break;
                                case gVariable.MACHINE_STATUS_DISPATCH_STARTED:
                                case gVariable.MACHINE_STATUS_DISPATCH_FIRST_PREPARED:
                                    value = gVariable.MACHINE_STATUS_STARTED;
                                    id = mySQLClass.getRecordNumInTable(gVariable.internalMachineName[i], gVariable.dispatchListTableName); //get id for the newest dispatch (already started)  
                                    break;
                            }

                            if (gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_MATERIAL, i] != 0)
                            {
                                value = gVariable.MACHINE_STATUS_MATERIAL_ALARM;
                                id = mySQLClass.getRecordNumInTable(gVariable.internalMachineName[i], gVariable.alarmListTableName);  //get alarm id
                            }
                            else if (gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DEVICE, i] != 0)
                            {
                                value = gVariable.MACHINE_STATUS_DEVICE_ALARM;
                                id = mySQLClass.getRecordNumInTable(gVariable.internalMachineName[i], gVariable.alarmListTableName);
                            }
                            else if (gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DATA, i] != 0)
                            {
                                value = gVariable.MACHINE_STATUS_DATA_ALARM;
                                id = mySQLClass.getRecordNumInTable(gVariable.internalMachineName[i], gVariable.alarmListTableName);
                            }
                        }

                        stamp = toolClass.ConvertDateTimeInt(DateTime.Now);

                        mySQLClass.writeOneFloatToTable(gVariable.internalMachineName[i], gVariable.currentValueTableName, stamp, f);
                        mySQLClass.writeTwoIntToTable(gVariable.internalMachineName[i], gVariable.machineStatusRecordTableName, stamp, value, id);

                        gVariable.currentValueNow[i] = 0;
                    }
                }

                //check for new alarm or alarm status changes
                if (gVariable.activeAlarmInfoUpdatedLocally == 1)
                {
                    //we build up current alarm info, display it on screen then send it to all client PC, so all client PCs will pop up this new alarm
                    //sometimes alarm infos come almost at the same time, so we need to process these info one by one 
                    for (i = 0; i < gVariable.activeAlarmTotalNumber; i++)
                    {
                        if (gVariable.activeAlarmNewStatus[i] != gVariable.ACTIVE_ALARM_OLD_ALARM)  //new alarm or content modified alarm
                        {
                            try
                            {
                                SetAlarmClass.activeAlarmInstanceArray[i].Show();
                            }
                            catch (Exception ex)
                            {
                                Console.Write("firstScreenTimePolling failed in display the new alarm" + ex);
                            }
                        }
                    }
                }

                //check to see if email forwarder is still alive
                if (emailPollingCycleTime == POLLING_CYCLE_FOR_HEART_BEAT - 1)
                {
                    if (gVariable.emailForwarderHeartBeatNum == gVariable.emailForwarderHeartBeatOld)  //email server disconnected
                    {
                        gVariable.emailForwarderHeartBeatNum = 0;
                    }
                    else
                    {
                        gVariable.emailForwarderHeartBeatOld = gVariable.emailForwarderHeartBeatNum;
                    }
                }

                gVariable.activeAlarmInfoUpdatedLocally = 0;
            }
            catch (Exception ex)
            {
                Console.Write("firstScreenTimePolling failed !! " + ex);
            }
        }


        private void form1_MouseDown(object sender, EventArgs e)
        {
            switch (gVariable.CompanyIndex)
            {
                case gVariable.ZIHUA_ENTERPRIZE:
                    firstScreenClass = this;
                    this.Hide();

                    gVariable.mainFunctionIndex = gVariable.PRODUCT_CURRENT_STATUS;
                    workshopZihua.workshopZihuaClass = new workshopZihua();
                    workshopZihua.workshopZihuaClass.Show();
                    return;
                default:
                    break;
            }
        }


        private void firstScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            int i;

            login.loginClass = new login(USER_EXIT_SYSTEM);
            login.loginClass.Show();

            if (login.loginClass.enterLoginScreen() >= 0)
            {
                //this is not a server, it is a client, so we need to inform our server so client can be closed gracefully
                if (gVariable.thisIsHostPC == false && gVariable.clientSocket != null)
                {
                    gVariable.clientSocket.Shutdown(SocketShutdown.Both);
                    gVariable.clientSocket.Close();
                }

                gVariable.willClose = 1;

                i = 0;

                if (aTimer != null)
                    aTimer.Enabled = false;

                toolClass.writeFileFooter();

                while (gVariable.willClose == 1)
                {
                    toolClass.nonBlockingDelay(200);
                    i++;
                    if (i > 5)
                        break;
                }
                System.Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
                return;
            }
        }

        //current time working status
        private void aaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.PRODUCT_CURRENT_STATUS;

            workshopZihua.workshopZihuaClass = new workshopZihua();
            workshopZihua.workshopZihuaClass.Show();

            this.Hide();
        }

        //time division working status
        private void abToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.PRODUCT_TIME_DIVISION_STATUS;

            machineProgress.machineProgressClass = new machineProgress();
            machineProgress.machineProgressClass.Show();

            this.Hide();
        }

        //dispatch publish
        private void acToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.PRODUCT_DISPATCH_PUBLISH;
            dispatchPublish.dispatchPublishClass = new dispatchPublish();
            dispatchPublish.dispatchPublishClass.Show();

            this.Hide();
        }

        //dispatch review
        /*private void adToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.PRODUCT_DISPATCH_REVIEW;

            showDispatchList.dispatchListClass = new showDispatchList(gVariable.globalDatabaseName, gVariable.MACHINE_STATUS_DISPATCH_ALL, gVariable.TIME_CHECK_TYPE_PLANNED_START, gVariable.PRODUCT_DISPATCH_REVIEW);
            showDispatchList.dispatchListClass.Show();

            //this is a pop up window, so we don't hide current screen
            //this.Hide();
        }*/

        //product release
        private void aeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.PRODUCT_DELIVERY_TO_GODOWN;

            //this.Hide();
        }

        //storage left
        private void baToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.MATERIAL_STORAGE_CHECKING;

            //this.Hide();
        }

        //
        private void bbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.MATERIAL_OUTGOING_CHECKING;

            materialInOutput.materialInOutputClass = new materialInOutput();
            materialInOutput.materialInOutputClass.Show();
            this.Hide();
        }

        //status of stack and feed bin
        private void bcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.MATERIAL_FEEDBIN_CHECKING;

            StackAndFeedbin.StackAndFeedbinClass = new StackAndFeedbin();
            StackAndFeedbin.StackAndFeedbinClass.Show();
            this.Hide();
        }

        //rebuild material by unqualified product
        private void bdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.MATERIAL_WASTE_REBUILD;

            materialRebuild.materialRebuildClass = new materialRebuild();
            materialRebuild.materialRebuildClass.Show();
            this.Hide();
        }

        //waste material need to be discarded
        private void beToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.MATERIAL_WASTE_DISCARD;

            materialWaste.materialWasteClass = new materialWaste();
            materialWaste.materialWasteClass.Show();
            this.Hide();
        }

        //machine status
        private void caToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.MACHINE_MANAGEMENT_STATUS;

            machineProgress.machineProgressClass = new machineProgress();
            machineProgress.machineProgressClass.Show();

            this.Hide();
        }

        //machine list
        private void cbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.MACHINE_MANAGEMENT_LEDGER;

            commonListview.commonListviewClass = new commonListview(gVariable.basicInfoDatabaseName, gVariable.machineTableName, gVariable.machineFileName, "生产设备台帐管理", 0);
            commonListview.commonListviewClass.Show();

            this.Hide();
        }

        //maintenance
        private void cdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.MACHINE_MANAGEMENT_MAINTENANCE;

            workshopZihua.workshopZihuaClass = new workshopZihua();
            workshopZihua.workshopZihuaClass.Show();

            this.Hide();
        }

        //repairing
        private void ceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.MACHINE_MANAGEMENT_REPAIRING;

            workshopZihua.workshopZihuaClass = new workshopZihua();
            workshopZihua.workshopZihuaClass.Show();

            this.Hide();
        }

        //routine
        private void ceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.MACHINE_MANAGEMENT_ITEM_CHECKING;

            workshopZihua.workshopZihuaClass = new workshopZihua();
            workshopZihua.workshopZihuaClass.Show();

            this.Hide();
        }

        //working calendar
        private void cfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.MACHINE_MANAGEMENT_WORKING_CALENDAR;

            MaintainaceCalendar maintainCalendar = new MaintainaceCalendar()
            {
                WindowState = FormWindowState.Maximized
            };
            maintainCalendar.Show();

            this.Hide();
        }

        //andon management
        private void dToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.MAIN_FUNCTION_ANDON;

            alarmListView.alarmListViewClass = new alarmListView(gVariable.globalDatabaseName, gVariable.MACHINE_NAME_ALL_FOR_SELECTION, gVariable.ALARM_TYPE_ALL_FOR_SELECTION,
                                                                 gVariable.ALARM_TYPE_DEVICE, gVariable.MAIN_FUNCTION_ANDON);
            alarmListView.alarmListViewClass.Show();
            
            this.Hide();
        }

        //SPC management
        private void eaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.QUALITY_MANAGEMENT_SPC_CONTROL;

            //first display current machine working status, then choose one of the machine for SPC data 
            workshopZihua.workshopZihuaClass = new workshopZihua();
            workshopZihua.workshopZihuaClass.Show();

            this.Hide();
        }

        //quality backtrack
        private void ebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.QUALITY_MANAGEMENT_DEFECT_BACKTRACK;

            serialNumbacktrack.serialNumbacktrackClass = new serialNumbacktrack();
            serialNumbacktrack.serialNumbacktrackClass.Show();

            this.Hide();
        }

        //quality inspection management
        private void ecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.QUALITY_MANAGEMENT_INSPECTION_CONTROL;

            //this.Hide();
        }

        //unqualified product management
        private void edToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.QUALITY_MANAGEMENT_DEFECTS_CONTROL;

            //this.Hide();
        }

        //machine working hour
        private void faToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.EFFICIENCY_MACHINE_WORK_TIME;

            OEEMachineManager oeeMachineMgn = new OEEMachineManager(factory)
            {
                WindowState = FormWindowState.Maximized
            };
            oeeMachineMgn.Show();
        }

        //employee working hour
        private void fbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.EFFICIENCY_EMPLOYEE_WORK_TIME;

            OEEStaffManager oeeStaffMgn = new OEEStaffManager(factory)
            {
                WindowState = FormWindowState.Maximized
            };
            oeeStaffMgn.Show();
        }

        //machine power consumed
        private void fcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.EFFICIENCY_MACHINE_POWER_CONSUMPTION;

            OEEEnergyManager oeeEnergyMgn = new OEEEnergyManager(factory)
            {
                WindowState = FormWindowState.Maximized
            };
            oeeEnergyMgn.Show();
        }

        //machine utilization status
        private void fdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.EFFICIENCY_MACHINE_OEE;

            OEESummaryManager oeeSummaryMgn = new OEESummaryManager(factory)
            {
                WindowState = FormWindowState.Maximized
            };
            oeeSummaryMgn.Show();
        }

        //cost analysis
        private void feToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.EFFICIENCY_COST_ANALYSIS;

            OEECostManager oeeCostMgn = new OEECostManager(factory)
            {
                WindowState = FormWindowState.Maximized
            };
            oeeCostMgn.Show();
        }

        //performance analysis
        private void ffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.EFFICIENCY_PERFORMENCE_REVIEW;
        }

        private void fgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;

        }

        //sales order list
        private void gaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.MAIN_FUNCTION_APS;

            salesOrderList salesOrderListImpl = new salesOrderList();
            salesOrderListImpl.Show();

            this.Hide();
        }

        //APS
        private void gbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.MAIN_FUNCTION_APS;

            APSUI.APSUIClass = new APSUI();
            APSUI.APSUIClass.Show();

            this.Hide();
        }

        //APS
        private void gToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }


        //product info
        private void haToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.BASICINFO_PRODUCT_INFO;

            commonListview.commonListviewClass = new commonListview(gVariable.basicInfoDatabaseName, gVariable.productTableName, gVariable.productFileName, "产品信息", 14);
            commonListview.commonListviewClass.Show();

            this.Hide();
        }

        //customer info
        private void hbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.BASICINFO_CUSTOMER_INFO;

            commonListview.commonListviewClass = new commonListview(gVariable.basicInfoDatabaseName, gVariable.customerListTableName, gVariable.customerListFileName, "客户信息", 0);
            commonListview.commonListviewClass.Show();

            this.Hide();
        }

        //material vendor info
        private void hcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.BASICINFO_VENDER_INFO;

            commonListview.commonListviewClass = new commonListview(gVariable.basicInfoDatabaseName, gVariable.vendorListTableName, gVariable.vendorListFileName, "供应商信息", 0);
            commonListview.commonListviewClass.Show();

            this.Hide();
        }

        //material info
        private void hdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.BASICINFO_MATERIAL_INFO;

            commonListview.commonListviewClass = new commonListview(gVariable.basicInfoDatabaseName, gVariable.materialTableName, gVariable.materialFileName, "物料信息", 0);
            commonListview.commonListviewClass.Show();

            this.Hide();
        }

        //BOM info
        private void heToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.BASICINFO_BOM_INFO;

            commonListview.commonListviewClass = new commonListview(gVariable.basicInfoDatabaseName, gVariable.bomTableName, gVariable.bomFileName, "产品配方信息", 17);
            commonListview.commonListviewClass.Show();

            this.Hide();
        }

        //warehouse info
        private void hfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;

            commonListview.commonListviewClass = new commonListview(gVariable.basicInfoDatabaseName, gVariable.warehouseListTableName, gVariable.warehouseListFileName, "仓库信息", 0);
            commonListview.commonListviewClass.Show();

            this.Hide();
        }

        //emplyoee info
        private void hgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;

            commonListview.commonListviewClass = new commonListview(gVariable.basicInfoDatabaseName, gVariable.employeeTableName, gVariable.employeeFileName, "员工信息", 12);
            commonListview.commonListviewClass.Show();

            this.Hide();
        }

        //machines info
        private void hhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;

            commonListview.commonListviewClass = new commonListview(gVariable.basicInfoDatabaseName, gVariable.machineTableName, gVariable.machineFileName, "设备信息", 0);
            commonListview.commonListviewClass.Show();

            this.Hide();
        }

        //product specification
        private void hiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;

            commonListview.commonListviewClass = new commonListview(gVariable.basicInfoDatabaseName, gVariable.machineTableName, gVariable.machineFileName, "产品规格书", 0);
            commonListview.commonListviewClass.Show();

            this.Hide();
        }

        //show display boards 
        private void iToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //user privilege management
        private void jaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gVariable.mainFunctionIndex = gVariable.SYSTEM_USER_PRIVILEGE;

            login.loginClass = new login(USER_PRIVILEGE_SETTING);
            login.loginClass.Show();
        }

        //system setting
        private void jbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firstScreenClass = this;
            gVariable.mainFunctionIndex = gVariable.SYSTEM_SETTINGS;

            //this.Hide();
        }

        //version info
        private void jcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string title;

            gVariable.mainFunctionIndex = gVariable.SYSTEM_VERSION_INFO;

            //excelClass c = new excelClass();
            //c.slitReportFunc();

            title = gVariable.enterpriseTitle + "智能生产管理系统 1.01\r\n\r\n  版权(c) 2017 - 2018 敏仪电子科技(上海)有限公司";
            MessageBox.Show(title, "版权信息", MessageBoxButtons.OK);
        }

    }
}
