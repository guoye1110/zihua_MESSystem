using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net.Sockets;

namespace MESSystem.common
{
    public class gVariable
    {
        //0: we will communicate with MES
        //1: we won't communicate with MES, only use some fake data
        //this parameter is defined in file init/mes.txt
        public static int faultData;

        //0:don't touch database
        //1:clean database then generate new default database 
        public static int buildNewDatabase = 0;

        //whether we enabled SPC checking and data limitation checking
        public static int checkDataCorrectness = 0;

        //If we set debugMode to value other than 0, performance will be affected, because we need to write all data to files
        //debugMode = 0: no debug info
        //debugMode = 1: write communication log into log file of "..\\..\\log\\debuginfo.log"
        //debugMode = 2: write communication data to "..\\..\\log\\dataRecord", error data to "..\\..\\log\\errorRecord", we can compare the recorded data with data sent from data collect board via UART6
        //debugMode = 3: We will output data to console
        public static int debugMode = 0;

        public const int DONGFENG_23 = 0;
        public const int ZIHUA_ENTERPRIZE = 1;
        public const int DONGFENG_20 = 2;

        //public static int CompanyIndex = ZIHUA_ENTERPRIZE;
        public static int CompanyIndex; // defined in file init/company.txt
        public const int maxClientPC = 100;

        //no bullenton board function at all
        public const int WORKSHOP_REPORT_NONE = 0;
        //bullentin board is the only function for this project
        public const int WORKSHOP_REPORT_BULLETIN = 1;
        //bullentin is one of the functions for this project
        public const int WORKSHOP_REPORT_FUNCTION = 2;

        //how do we want workshop report function to be performed
        public static int workshopReport = 0;

        public const int RESULT_OK = 0;
        public const int RESULT_ERR = 1;

        public const int MACHINE_POS_STATUS_FIXED = 0;
        public const int MACHINE_POS_STATUS_DESIGNED = 1;

        public const int SMALL_BARCODE_LEN = 23;
        public const int LARGE_BARCODE_LEN = 20;

        //board ID larger than that is an ID of an App that want a redo of handshake, but should not initialize parameters 
        //board ID less than that is an ID of normal data collect board, which has the function of dispatch apply/start/complete, and also craft/quality related functions
        public const int ID_APPS_NOT_INIT_DATA = 0x100000;

        //board ID larger than that is an ID of an App that want to do a handshake, which means a start of communication
        public const int ID_OTHERTHAN_TOUCHPAD = 0x1000000;

        //public static string[] processName;

        //definition for Zihua worikng process *****************************
        public const int LINE_NUM_ZIHUA = 7;  //the major product line is cast line, there are 7 cast lines
        public static int MACHINE_NAME_ALL_FOR_SELECTION;

        public const int NUM_OF_FEEDING_MACHINE = 7; //7条流水线，每条一个投料设备
        public const int STACK_NUM_ONE_MACHINE = 7; //每个偷料设备有8个料仓，每个料仓不同的原料
        //public static int[] allMachineIDForZihua = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

        //this machine array includes only cast/print/slit machines, normally used in APS process
        public static string[] machineNameArrayAPS = 
                { 
                   "1号流延机", "2号流延机", "3号流延机", "4号流延机", "5号流延机", "6号中试机", "7号吹膜机", 
                   "1号印刷机", "2号印刷机", "3号印刷机", "4号印刷机", "5号柔印机", 
                   "1号分切机", "3号分切机", "5号分切机", "6号分切机", "7号分切机" 
                };

        //these machines will connected with touchscreen or data collect board, so will have device alarm or material alarm, which includes feed/cast/print/slit
        public static string[] machineNameArrayTouchScreen = new string[24];
        //these machines will have its own database, will communicate with touchscreen/label printer
        public static string[] machineNameArrayDatabase;
        /*   { 
             "1号流延机", "2号流延机", "3号流延机", "4号流延机", "5号流延机", "6号中试机", "7号吹膜机", 
             "1号印刷机", "2号印刷机", "3号印刷机", "4号印刷机", "5号柔印机", 
             "1号分切机", "3号分切机", "5号分切机", "6号分切机", "7号分切机",
             "1号打包机", "2号打包机", "3号打包机", "4号打包机",
             "1号结粒机", "2号压片机", "3号压片机", "4号压片机", "5号压片机", 
           }; */

        //public static string[] machineNameArrayForSelection; // = new string[machineNameArray.Length + packingMachineNameArray.Length + 1];
        public static string[] machineWorkshopDatabase; // = { "一车间", "二车间", "二车间", "二车间", "二车间", "一车间", "一车间", "一车间", "一车间", "一车间", "一车间", "一车间", "二车间", "二车间", "二车间", "一车间", "一车间", "一车间" };
        public static string[] machineCodeArrayDatabase;

        //for these machines below, we will generate serial number for every piece of product
        //public static int[] feedingProcess = { 1, 2, 3, 4, 5 };
        public static int[] castingProcess = { 1, 2, 3, 4, 5, 6, 7 };
        public static int[] printingProcess = { 8, 9, 10, 11, 12 };
        public static int[] slittingProcess = { 13, 14, 15, 16, 17 };
        public static int[] packingProcess = { 18, 19, 20, 21 };
        public static int[] rebuildProcess = { 22, 23, 24, 25, 26 };

        public static int[] currentLargeRollIndex = new int[packingProcess[0]]; //current large roll index 
        public static byte[] currentRollStatus = new byte[packingProcess[0]]; //current large roll product status, 0:qunalified;1:unqualified;2:needTest;3:wasted

        // salesorder: "S171109"
        public const int salesOrderLength = 7;

        // batchNum: "1801306" - 2018-01, production line 3, batch 06 
        public const int batchNumLength = 7;

        // dispatchCode:"1801306121L2" => 1801306 + 12 (2018-01-12) + 1(day shift) + L2 cast machine #2
        public const int dispatchCodeLength = 12;

        // dispatchCode:"1801306121L2" + time:1824(18:24) + large roll ID:"005" + quality inspection result 0(quality OK)  
        //"1801306121L218240050";
        public const int castBarcodeLength = 20;

        // dispatchCode:"1801306121Y2" + time:1824(18:24) + large roll ID:"005" + quality inspection result 0(quality OK)  
        //"1801306121Y218240050";
        public const int printBarcodeLength = 20;

        // dispatchCode:"1801306121F218240050300" + time:1824(18:24) + large roll ID 005 + small roll ID 03 + multi-production ID 0 + quality inspection result 0(quality OK)  
        //"1801306121Y218240050";
        public const int slitBarcodeLength = 23;

        // dispatchCode:"1801306121J218240050300" + time:1824(18:24) + large roll ID 005 + small roll ID 03 + multi-production ID 0 + quality inspection result 0(quality OK)  
        //"1801306121J218240050";
        public const int inspectionBarcodeLength = 23;

        // dispatchCode:"1801306121R218240050300" + time:1824(18:24) + large roll ID 005 + small roll ID 03 + multi-production ID 0 + quality inspection result 0(quality OK)  
        //"1801306121R218240050";
        public const int reuseBarcodeLength = 23;

        //end of the definition for Zihua woring process *****************************

        public const int numOfWorkshop = 4;

        public static string[] backgroundArray = { "..\\..\\resource\\dongfeng23.jpg", "..\\..\\resource\\zihuaCover.jpg", "..\\..\\resource\\dongfeng20.jpg" };
        public static string[] logoInTitleArray = { "..\\..\\resource\\df-icon.ico", "..\\..\\resource\\zihualogo_48X48.ico", "..\\..\\resource\\20-logo.ico" };

        public static string userAccount;
        public static int privilegeLevel;

        public const int CURRENT_READING = 0;
        public const int HISTORY_READING = 1;
        //whether we display the current dispatch and newest data in dispatchUI,, if it is 0, it means we are reviewing an old dispatch so wedon't need to refresh the screen 
        public static int contemporarydispatchUI;
        public static string dispatchUnderReview;

        public static string programTitle = "紫华企业智能生产信息管理系统";
        public static string enterpriseTitle = "紫华企业";

        //misellaneous defintion for screen, data type, device type, etc *******************************************
        public static float screenRatioX;  //current screen size X / SMALLEST_SCREEN_X(1280), for a 1920*1080 smaller font screen this is 1.5 
        public static float screenRatioY;  //current screen size Y / SMALLEST_SCREEN_Y(680), for a 1920*1080 smaller font screen this is 1.5
        //currently we set all screen UI to none - atuoScaleMode, so DPI setting is useless, we always use finest dpi mode
        public static float dpiRatioX;  //current screen size X / SMALLEST_SCREEN_X(1280) then considering dpi, for a 1920*1080 smaller font screen this is 1.25  
        public static float dpiRatioY;  //current screen size Y / SMALLEST_SCREEN_Y(680) then considering dpi, for a 1920*1080 smaller font screen this is 1.25  
        public static int dpiValue; //0 means 96 dpi, 1 means 120 dpi, 2 means 144 dpi
        public const int SMALLER_DPI = 96;  //definition for screen DPI setting which will affect the position and size of label/button/bmp on screen 
        public const int MEDIUM_DPI = 120;
        public const int LARGER_DPI = 144;

        public const int EFFECTIVE_SCREEN_X = 1920;
        public const int EFFECTIVE_SCREEN_Y = 1040;

        public const int SMALLEST_SCREEN_X = 1280;
        public const int SMALLEST_SCREEN_Y = 680;

        public static int resolutionLevel;
        public static int screenSizeX;
        public static int screenSizeY;

        //we use these value to judge rect.with/rect.height value
        public const int resolutionLevelValue1 = 1100;
        public const int resolutionLevelValue2 = 1300;
        public const int resolutionLevelValue3 = 1400;
        public const int resolutionLevelValue4 = 1600;
        public const int resolutionLevelValue5 = 1800;

        //these values are for resolution type
        public const int resolution_total = 6;
        public const int resolution_1024 = 0;  //1024 * 768 not really supported
        public const int resolution_1280 = 1;  //supported 1280 * 800
        public const int resolution_1366 = 2;  //supported 1366 * 768
        public const int resolution_1440 = 3;  //supported 1440 * 900
        public const int resolution_1600 = 4;  //supported 1600 * 900
        public const int resolution_1920 = 5;  //supported 1920 * 1080

        public const int BAUDRATE_1200 = 0;
        public const int BAUDRATE_2400 = 1;
        public const int BAUDRATE_4800 = 2;
        public const int BAUDRATE_9600 = 3;
        public const int BAUDRATE_19200 = 4;
        public const int BAUDRATE_38400 = 5;
        public const int BAUDRATE_57600 = 6;
        public const int BAUDRATE_115200 = 7;
        public const int BAUDRATE_NONE = 8;

        public static int mainFunctionIndex; //range from 0 - 9 below
        public const int MAIN_FUNCTION_PRODUCTION = 0;
        public const int MAIN_FUNCTION_MATERIAL = 1;
        public const int MAIN_FUNCTION_MACHINE = 2;
        public const int MAIN_FUNCTION_ANDON = 3;
        public const int MAIN_FUNCTION_QUALITY = 4;
        public const int MAIN_FUNCTION_EFFICIENCY = 5;
        public const int MAIN_FUNCTION_APS = 6;
        public const int MAIN_FUNCTION_FUNDAMENTALS = 7;
        public const int MAIN_FUNCTION_SYSTEM_MANAGEMENT = 8;

        public const int PRODUCT_CURRENT_STATUS = 10;
        public const int PRODUCT_TIME_DIVISION_STATUS = 11;
        public const int PRODUCT_DISPATCH_PUBLISH = 12;
        public const int PRODUCT_DELIVERY_TO_GODOWN = 13;

        public const int MATERIAL_STORAGE_CHECKING = 21;
        public const int MATERIAL_OUTGOING_CHECKING = 22;
        public const int MATERIAL_FEEDBIN_CHECKING = 23;
        public const int MATERIAL_PRINTING_CHECKING = 24;
        public const int MATERIAL_WASTE_REBUILD = 25;
        public const int MATERIAL_WASTE_DISCARD = 26;

        public const int MACHINE_MANAGEMENT_STATUS = 30;
        public const int MACHINE_MANAGEMENT_LEDGER = 31;
        public const int MACHINE_MANAGEMENT_MAINTENANCE = 32;
        public const int MACHINE_MANAGEMENT_REPAIRING = 33;
        public const int MACHINE_MANAGEMENT_ITEM_CHECKING = 34;
        public const int MACHINE_MANAGEMENT_WORKING_CALENDAR = 35;

        public const int QUALITY_MANAGEMENT_SPC_CONTROL = 50;
        public const int QUALITY_MANAGEMENT_DEFECT_BACKTRACK = 51;
        public const int QUALITY_MANAGEMENT_INSPECTION_CONTROL = 52;
        public const int QUALITY_MANAGEMENT_DEFECTS_CONTROL = 53;

        public const int EFFICIENCY_MACHINE_WORK_TIME = 60;
        public const int EFFICIENCY_EMPLOYEE_WORK_TIME = 61;
        public const int EFFICIENCY_MACHINE_POWER_CONSUMPTION = 62;
        public const int EFFICIENCY_MACHINE_OEE = 63;
        public const int EFFICIENCY_COST_ANALYSIS = 64;
        public const int EFFICIENCY_PERFORMENCE_REVIEW = 65;

        public const int BASICINFO_PRODUCT_INFO = 80;
        public const int BASICINFO_CUSTOMER_INFO = 81;
        public const int BASICINFO_VENDER_INFO = 82;
        public const int BASICINFO_MATERIAL_INFO = 83;
        public const int BASICINFO_BOM_INFO = 84;

        public const int SYSTEM_USER_PRIVILEGE = 90;
        public const int SYSTEM_SETTINGS = 91;
        public const int SYSTEM_VERSION_INFO = 92;

        public const int FUNCTION_DISPATCH_LIST_UI = 1001;
        public const int FUNCTION_WORKSHOP_UI = 1002;

        public static int secondFunctionIndex; //range from 0 - 85 below

        public const int EXCEL_FIRSTLINE_DATA = 0;  //read every line to data table
        public const int EXCEL_FIRSTLINE_TITLE = 1; //so we will not read first line into table

        public const int FROM_ALARM_DISPLAY_FUNC = 0;   //this is an alarm triggered SPC data display
        public const int FROM_QUALITY_MANAGEMENT_FUNC = 1;  //this is a quality management triggered SPC data didplay

        //s1711165012  --> 2017-11, 16->sales order index, 5 -> production line 5, 01 -> dispatch index(an order could be divided into multiple dispatches), 2 -> process index(casting machine)  
        public const int LENGTH_DISPATCH_CODE = 10;

        //        public int status; //0：ERP published; 1：APS OK; 2：production started(the first dispatch started); 3：sales order completed; 4：sales order cancelled  

        //production  management

        public const int PHYSICAL_WORKING_HOUR_ONE_SHIFT = 12; //one shift is 12 hours
        public const int EFFECTIVE_WORKING_HOUR_ONE_SHIFT = 10; //one shift is 12 hours, but we need to do something like "清膜口", so real effective working time is about 10 hours

        //max nuber of output for a product batch, 20 tons
        public const int PRODUCT_BATCH_MAX_OUTPUT_NUM = 20000;
        public const int MAX_NUM_SELECTED_SALES_ORDER_APS = 20;

        //public const int MATERIAL_SHEET_STATUS_GENERATED = 0;
        //public const int MATERIAL_SHEET_STATUS_APPLIED = 2;
        //public const int MATERIAL_SHEET_STATUS_CONFIRMED = 3;
        //public const int MATERIAL_SHEET_STATUS_ERROR = 4;
        //public const int MATERIAL_SHEET_STATUS_SHORTAGE = 5;
        //end of production  management

        //material  management
        //end of material management

        //machine management
        public const int MACHINE_CHECKITEM_ROUTINECHECK = 0;
        public const int MACHINE_CHECKITEM_ADDOIL = 1;
        public const int MACHINE_CHECKITEM_WASHUP = 2;

        public const int MACHINE_MAINTENANCE_MAINTENANCE1 = 0;

        public const int MACHINE_REPAIRING_REPAIRING1 = 0;

        public const int machineTableRoutine1 = 0;
        public const int machineTableRoutine2 = 1;
        public const int machineTableRoutine3 = 2;
        public const int machineTableMaintenance1 = 3;
        public const int machineTableRepairing1 = 4;
        public static string[,] machineManagementTableName = 
        {
            {dailyCheckListTableName, addOilListTableName, washupListTableName}, 
            {maintainListTableName, null, null},
            {repairListTableName, null, null} 
        };

        public static string[] taskListCheckDispatch = { "点巡检表", "加油表", "设备清洁表" };
        public static string[] taskListMaintainDispatch = { "设备维护保养表" };
        public static string[] taskListRepairDispatch = { "设备维修表" };

        public static int machineListFresh;
        //public static int SPCDataNotEnough;
        public static int APSScreenRefresh;
        public static int numOfBatchDefinedAPSRule;  //total number of batch orders that have been defined for their APS rules
        public static int indexOfBatchDefinedAPSRule;  //current batch order that is being defined for their APS rules

        public static string[,] machineTableFilename = 
        {
            {"..\\..\\data\\routin-check1.xlsx", "..\\..\\data\\oil1.xlsx", "..\\..\\data\\washup1.xlsx"}, 
            {"..\\..\\data\\maintenance1.xlsx", null, null}, 
            {"..\\..\\data\\repairing1.xlsx", null, null}
        };
        //end of machine management

        //efficiency  management
        //end of efficiency  management

        //andon management
        //end of andon management

        //quality management
        //end of quality management

        //APS management
        //end of APS management

        //basic data management
        //end of basic data management

        //settings
        //end of settings

        //end of the misellaneous defintion for screen, data type, etc *******************************************

        public const int maxMachineNum = 26;

        public const int maxCurveNum = 36;  //we now have altogether 36 kind of data that need to be draw by curve

        public const int fileDataLength = 1024 * 1024 * 8;

        public const int maxErrorCodeNum = 40;  //an error code list may have a lot of error status
        public const int maxCraftParamNum = 8;  //there could be 10 kinds of craft parameter at most, like voltage, cuttent, spin speed
        public const int maxQualityDataNum = 16;  //there could be 10 kinds of quality data at most, like length, temparature, height
        public const int maxMaterialTypeNum = 7; //there could be 7 kinds of material for a dispatch 
        public const int maxDeviceAlarmNum = 4;  //defined by touch pad
        public const int maxNumDiscussClient = 20;  //20 PC can attend one alarm discussion

        //database related definitions ********************************************************************

        //this is a global database including global alarm table/sales order table throughout the factory
        public const string basicInfoDatabaseName = "basicInfo";
        public const string globalDatabaseName = "globalDatabase";

        //database for every board / machine
        public const string alarmListTableName = "0_alarmList";  //device alarm
        public const string alarmListFileName = "..\\..\\data\\machine\\alarmList.xlsx";
        public const string dispatchListTableName = "0_dispatchlist";
        public const string dispatchListFileName = "..\\..\\data\\machine\\dispatchList.xlsx";
        public const string qualityListTableName = "0_qualitylist";
        public const string qualityListFileName = "..\\..\\data\\machine\\qualityList.xlsx";
        public const string craftListTableName = "0_craftlist";
        public const string craftListFileName = "..\\..\\data\\machine\\craftList.xlsx";
        public const string machineStatusListTableName = "0_machineStatusList";
        public const string machineStatusListFileName = "..\\..\\data\\machine\\machineStatusList.xlsx";
        public const string materialRequirementTableName = "0_materialRequirement";
        public const string materialRequirementFileName = "..\\..\\data\\machine\\materialRequirement.xlsx";
        public const string materialListTableName = "0_materialList";
        public const string materialListFileName = "..\\..\\data\\machine\\materialList.xlsx";
        public const string materialTaskListTableName = "0_materialTaskList";
        //public const string materialListFileName = "..\\..\\data\\machine\\materialList.xlsx";
        public const string machineWorkingPlanTableName = "0_machineWorkingPlan";
        public const string machineWorkingPlanFileName = "..\\..\\data\\machine\\machineWorkingPlan.xlsx";
        public const string machineStatusRecordTableName = "0_machineStatusRecord";
        public const string machineStatusRecordFileName = "..\\..\\data\\machine\\machineStatusRecord.xlsx";
        public const string repairListTableName = "0_repairList";
        public const string repairListFileName = "..\\..\\data\\machine\\repairList.xlsx";
        public const string dailyCheckListTableName = "0_dailyCheckList";
        public const string dailyCheckListFileName = "..\\..\\data\\machine\\dailyCheckList.xlsx";
        public const string maintainListTableName = "0_maintainList";
        public const string maintainListFileName = "..\\..\\data\\machine\\maintainList.xlsx";
        public const string addOilListTableName = "0_oilAddList";
        public const string addOilListFileName = "..\\..\\data\\machine\\addOilList.xlsx";
        public const string washupListTableName = "0_washupList";
        public const string washupListFileName = "..\\..\\data\\machine\\washupList.xlsx";
        public const string currentValueTableName = "0_currentValue";
        public const string currentValueFileName = "..\\..\\data\\machine\\currentValue.xlsx";
        public const string stackInventoryListTableName = "0_stackInventory";
        public const string stackInventoryListFileName = "..\\..\\data\\machine\\stackInventory.xlsx";
        public const string feedBinInventoryListTableName = "0_feedBinInventory";
        public const string feedBinInventoryListFileName = "..\\..\\data\\machine\\feedBinInventory.xlsx";
        public const string productBeatTableName = "0_productBeatTable";
        public const string productBeatFileName = "..\\..\\data\\machine\\productBeat.xlsx";
        public const string productTaskListTableName = "0_productTaskListTable";
        public const string productTaskListFileName = "..\\..\\data\\machine\\dispatchList.xlsx";

        public const string dummyDispatchTableName = "dummy";
        public const string craftTableNameAppendex = "_craft";
        public const string qualityTableNameAppendex = "_quality";
        public const string volcurTableNameAppendex = "_volcur";
        public const string beatTableNameAppendex = "_beat";

        //basicdata database 
        public const string employeeTableName = "employee";
        public const string employeeFileName = "..\\..\\data\\basicData\\employee.xlsx";
        public const string materialTableName = "material";
        public const string materialFileName = "..\\..\\data\\basicData\\material.xlsx";
        public const string productTableName = "productSpec";
        public const string productFileName = "..\\..\\data\\basicData\\productspec.xlsx";
        public const string machineTableName = "machineList";
        public const string machineFileName = "..\\..\\data\\basicData\\machineList.xlsx";
        public const string machineCapabilityTableName = "machineCapability";
        public const string machineCapabilityFileName = "..\\..\\data\\basicData\\machineCapability.xlsx";
        public const string workProcedureTableName = "procedureList";
        public const string workProcedureFileName = "..\\..\\data\\basicData\\procedureList.xlsx";
        public const string bomTableName = "bomList";
        public const string bomFileName = "..\\..\\data\\basicData\\materialBom.xlsx";
        public const string inkBomTableName = "inkbomList";
        public const string inkBomFileName = "..\\..\\data\\basicData\\oilBom.xlsx";
        public const string packingTableName = "packingList";
        public const string packingFileName = "..\\..\\data\\basicData\\packingSpec.xlsx";
        public const string customerListTableName = "customerList";
        public const string customerListFileName = "..\\..\\data\\basicData\\customerList.xlsx";
        public const string vendorListTableName = "vendorList";
        public const string vendorListFileName = "..\\..\\data\\basicData\\vendorList.xlsx";
        public const string warehouseListTableName = "warehouseList";
        public const string warehouseListFileName = "..\\..\\data\\basicData\\warehouseList.xlsx";
        public const string castSpecTableName = "castSpec";
        public const string castSpecFileName = "..\\..\\data\\basicData\\castSpec.xlsx";
        public const string printSpecTableName = "printSpec";
        public const string printSpecFileName = "..\\..\\data\\basicData\\printSpec.xlsx";
        public const string slitSpecTableName = "slitSpec";
        public const string slitSpecFileName = "..\\..\\data\\basicData\\slitSpec.xlsx";
        public const string castCraftTableName = "castCraft";
        public const string castCraftFileName = "..\\..\\data\\basicData\\castCraft.xlsx";
        public const string printCraftTableName = "printCraft";
        public const string printCraftFileName = "..\\..\\data\\basicData\\printCraft.xlsx";
        public const string slitCraftTableName = "slitCraft";
        public const string slitCraftFileName = "..\\..\\data\\basicData\\slitCraft.xlsx";
        public const string castQualityTableName = "castQuality";
        public const string castQualityFileName = "..\\..\\data\\basicData\\castQuality.xlsx";
        public const string printQualityTableName = "printQuality";
        public const string printQualityFileName = "..\\..\\data\\basicData\\printQuality.xlsx";
        public const string slitQualityTableName = "slitQuality";
        public const string slitQualityFileName = "..\\..\\data\\basicData\\slitQuality.xlsx";
        public const string substitutesTableName = "substitutes";
        public const string substitutesFileName = "..\\..\\data\\basicData\\substitutes.xlsx";

        //global database
        public const string globalAlarmListTableName = "AlarmList";
        public const string globalAlarmListFileName = "..\\..\\data\\machine\\alarmList.xlsx";
        public const string salesOrderTableName = "salesOrderList";
        public const string salesOrderFileName = "..\\..\\data\\globalTables\\salesOrderList.xlsx";
        public const string productBatchTableName = "productBatchList";   //product batch -- 产品批次单，相当于把一个订单切成小于 20 吨的订单批次
        public const string productBatchFileName = "..\\..\\data\\globalTables\\productBatchList.xlsx";
        public const string globalProductTaskTableName = "productTaskListTable";   //product task -- 任务单，把批次单分解到不同工序 
        public const string globalProductTaskFileName = "..\\..\\data\\machine\\dispatchList.xlsx";
        public const string globalDispatchTableName = "dispatchList";
        public const string globalDispatchFileName = "..\\..\\data\\machine\\dispatchList.xlsx";  //make sure dispatch in different places have the same format
        public const string globalMaterialTableName = "materialList";
        public const string globalMaterialFileName = "..\\..\\data\\machine\\materialList.xlsx";
        public const string globalMaterialTaskTableName = "materialTaskList";
        //public const string globalMaterialFileName = "..\\..\\data\\machine\\materialList.xlsx";
        public const string materialFeedingTableName = "materialFeedingList";
        public const string materialFeedingFileName = "..\\..\\data\\globalTables\\materialFeedingList.xlsx";
        public const string materialDeliveryTableName = "materialDeliveryList";
        public const string materialDeliveryFileName = "..\\..\\data\\globalTables\\materialDeliveryList.xlsx";
        public const string reuseMaterialTableName = "reuseMaterialList";
        public const string reuseMaterialFileName = "..\\..\\data\\globalTables\\reuseMaterial.xlsx";
        public const string wasteMaterialTableName = "wasteMaterial";
        public const string wasteMaterialFileName = "..\\..\\data\\globalTables\\wasteMaterial.xlsx";
        public const string changePartRecordTableName = "changePartRecord";
        public const string changePartRecordFileName = "..\\..\\data\\globalTables\\changePartRecord.xlsx";
        public const string partsInventoryTableName = "partsInventory";
        public const string partsInventoryFileName = "..\\..\\data\\globalTables\\partsInventory.xlsx";
        public const string inspectionListTableName = "productInspectionList";
        public const string inspectionListFileName = "..\\..\\data\\globalTables\\inspectionList.xlsx";
        public const string finalPackingTableName = "productPackingList";
        public const string finalPackingFileName = "..\\..\\data\\globalTables\\finalPacking.xlsx";
        public const string productCastListTableName = "productCastingList";
        public const string productCastListFileName = "..\\..\\data\\globalTables\\productCastList.xlsx";
        public const string productPrintListTableName = "productPrintingList";
        public const string productPrintListFileName = "..\\..\\data\\globalTables\\productPrintList.xlsx";
        public const string productSlitListTableName = "productSlittingList";
        public const string productSlitListFileName = "..\\..\\data\\globalTables\\productSlitList.xlsx";
        public const string binInventoryTableName = "binInventory";
        public const string binInventoryFileName = "..\\..\\data\\globalTables\\binInventory.xlsx";
        public const string materialPurchaseTableName = "materialPurchaseList";
        public const string materialPurchaseFileName = "..\\..\\data\\globalTables\\materialPurchaseList.xlsx";
        public const string productBatchCurrentIndexTableName = "productBatchCurrentIndex";
        public const string productBatchCurrentIndexFileName = "..\\..\\data\\globalTables\\dispatchCurrentIndex.xlsx";
        public const string transferSheetTableName = "transferSheet";
        public const string transferSheetFileName = "..\\..\\data\\globalTables\\transferSheet.xlsx";
        public const string inkUsedTableName = "inkUsed";
        public const string inkUsedFileName = "..\\..\\data\\globalTables\\inkUsed.xlsx";
        public const string supplementryUsedTableName = "supplementryUsed";
        public const string supplementryUsedFileName = "..\\..\\data\\globalTables\\supplementryUsed.xlsx";

        public const int notAppendRecord = 0;  //this is not an append action
        public const int appendRecord = 1;  //this is an append action, so we need to consider the index in database for the last appended record
        //end of database related definitions ********************************************************************

        public static ServiceReference1.deviceFailureInfo[] errorDescList = null;
        public static ServiceReference6.UserInfo[] employeeInfo = null;

        public static int totalFileTypeNum = 20;  //we now have altogether 21 kind of data that need to be recorded, 15 curves plus GPIO/RFID/Scanner/RF/Printer
        public static string dataDirPath = "..\\..\\data\\";

        public const int MAX_DISPATCH_ONE_MACHINE = 10000;

        public const int MAX_NUM_ADC = 8;
        public const int MAX_NUM_UART = 3;

        public const int UARTNotSelected = 100;

        public static int onePointstandForHowManyMinutes = 2;  //every point in device status screeen stand for 1 minute

        public const int DISPATCH_TYPE_PRODUCTION = 0;
        public const int DISPATCH_TYPE_ROUTINE_CHECK = 1;
        public const int DISPATCH_TYPE_ADD_OIL = 2;
        public const int DISPATCH_TYPE_WASHUP = 3;
        public const int DISPATCH_TYPE_MAINTENANCE = 20;
        public const int DISPATCH_TYPE_REPAIR = 40;

        public const int SALES_ORDER_STATUS_ERP_PUBLISHED = 0;
        public const int SALES_ORDER_STATUS_SEPARATE_OK = 1;
        public const int SALES_ORDER_STATUS_APS_OK = 2;
        public const int SALES_ORDER_STATUS_CONFIRMED = 3;
        public const int SALES_ORDER_STATUS_PUBLISHED = 4;
        public const int SALES_ORDER_STATUS_APPLIED = 5;
        public const int SALES_ORDER_STATUS_STARTED = 6;
        public const int SALES_ORDER_STATUS_COMPLETED = 7;
        public const int SALES_ORDER_STATUS_CANCEL_APPLIED = 8;
        public const int SALES_ORDER_STATUS_CANCELLED = 9;

        public static string[] salesorderStatus = { "已导入", "已分拆", "已排程", "已确认", "已发布", "已申请", "已开工", "已完工", "申请取消", "核准取消" };

        //the order of the definitions below should not be changed, will have big problem if the rule is broken
        //it is for status of the dispatch
        public const int MACHINE_STATUS_SHUTDOWN = -10;   //machine shut down
        public const int MACHINE_STATUS_DISPATCH_GENERATED = -3;   //dispatch generated by APS planner, but not confirmed yet 
        public const int MACHINE_STATUS_DISPATCH_CONFIRMED = -2;   //dispatch confirmed by APS planner 
        public const int MACHINE_STATUS_DISPATCH_UNPUBLISHED = -1;   //this is not a real status, it refer to both -2 and -3
        public const int MACHINE_STATUS_DISPATCH_PUBLISHED = 0;   //dispatch published by production line manager 
        public const int MACHINE_STATUS_DISPATCH_DUMMY = 0;   //machine in idle mode, no dispatch 
        public const int MACHINE_STATUS_DISPATCH_COMPLETED = 1;  //completed means we are in dummy mode, need apply for a new dispatch
        public const int MACHINE_STATUS_DISPATCH_APPLIED = 2;
        public const int MACHINE_STATUS_DISPATCH_STARTED = 3;
        public const int MACHINE_STATUS_DISPATCH_FIRST_PREPARED = 4;
        public const int MACHINE_STATUS_DISPATCH_ALL = 5;  //all kinds of status need to be considered
        public const int MACHINE_STATUS_DISPATCH_DISABLED = 6;  //all dispatch in global database will be in disabled status after they are published to machine

        //it is machine working status, for status display in workshop machine progress function
        public const int MACHINE_STATUS_DOWN = 0;   //machine shut down
        public const int MACHINE_STATUS_IDLE = 1;   //machine in idle mode, no dispatch 
        public const int MACHINE_STATUS_STARTED = 2;  //working mode
        public const int MACHINE_STATUS_DEVICE_ALARM = 3;
        public const int MACHINE_STATUS_MATERIAL_ALARM = 4;
        public const int MACHINE_STATUS_DATA_ALARM = 5;

        //alarm/Andon management **************************************************
        public const int ANDON_MANAGEMENT_ALL_LIST = 0;
        public const int ANDON_MANAGEMENT_COMPLETED_LIST = 1;
        public const int ANDON_MANAGEMENT_UNCOMPLETED_LIST = 2;
        public const int ANDON_MANAGEMENT_ANDON_SETTING = 3;

        public const int ALARM_STATUS_UNCHANGED = -1;
        public const int ALARM_STATUS_NOT_OCCURRED = 0;
        public const int ALARM_STATUS_APPLIED = 1;
        public const int ALARM_STATUS_SIGNED = 2;
        public const int ALARM_STATUS_COMPLETED = 3;
        public const int ALARM_STATUS_CANCELLED = 3;
        public const int ALARM_STATUS_ALL = 4;
        public static string[] strAlarmStatus = { "已申请", "已签到", "已处理", "已取消" };

        public static int ALARM_STATUS_ALL_FOR_SELECTION;
        public static string[] strAlarmStatusForSelection = { "已申请", "已签到", "已处理", "已取消", "所有状态" };

        public const int ALARM_TYPE_UNDEFINED = -1;  //undefined alarm type means all kinds of alarm need to be considered
        public const int ALARM_TYPE_DEVICE = 0;  //no chart
        public const int ALARM_TYPE_MATERIAL = 1;  //no chart
        //we need to discern the differece between this 2 kinds of alarm, because in SPCanalyze.cs, we need to know whether it is XBar chart or S chart caused this alarm so display XBar/S data in red color accordingly
        public const int ALARM_TYPE_QUALITY_DATA = 2;  //chart type defined in quality table, for Xbar chart/C chart
        public const int ALARM_TYPE_CRAFT_DATA = 3;  //chart_type_no_spc
        public const int ALARM_TYPE_CURRENT_VALUE = 4;  //dispatch should be completed as planned, but fails to complete 
        public const int ALARM_TYPE_ALL_IN_DETAIL = 5;  // all kinds in 
        public const int ALARM_TYPE_TOTAL_NUM = 5;  //total number of alatm types  
        public static string[] strAlarmTypeInDetail = { "设备安灯报警", "物料安灯报警", "质量数据报警", "工艺参数报警", "工单未完成报警" };

        public const int ALARM_TYPE_DATA = 2;  //including quality/craft data alarm
        public static int ALARM_TYPE_ALL_FOR_SELECTION;
        public static string[] strAlarmTypeForSelection = { "设备安灯报警", "物料安灯报警", "各类数据报警", "所有类型安灯报警" };

        public const string SPCMonitoringSystem = "SPC监测系统";

        //whether there is a device alarm alive for this board/machine, 
        //if there is already an alarm of this kind displayed on server screen, we can not trigger another one for the same kind, unless we dismiss it from the current screen
        public static int[,] typeAlarmAlreadyAlive = new int[ALARM_TYPE_TOTAL_NUM, maxMachineNum + 1];

        //category data are used for alarm process method histroy review
        public const int ALARM_CATEGORY_NORMAL = 0;   //data have no problem
        public const int ALARM_CATEGORY_MATERIAL = 1;   //materail alarm

        public const int ALARM_CATEGORY_QUALITY_DATA_OVERFLOW = 2;   //quality data out of spec limit usl/lsl
        public const int ALARM_CATEGORY_SPC_DATA_START = 3;   //SPC data error start index
        public const int ALARM_CATEGORY_SPC_DATA_OVERFLOW = 3;   //SPC data out of control limit ucl/lcl
        public const int ALARM_CATEGORY_SPC_DATA_SAME_SIDE = 4;   //9 points at same side of the chart
        public const int ALARM_CATEGORY_SPC_DATA_ONE_TREND = 5;   //7 points lies in the same trend
        public const int ALARM_CATEGORY_SPC_DATA_SMALL_CHANGE = 6;   //15 points lies within 1/3 of LCL and UCL 
        public const int ALARM_CATEGORY_SPC_DATA_LOCATE_APART = 7;   //4 out of 5 ponits lies at 2/3 of LCL and UCL 

        public const int NUM_OF_ALARM_CATEGORY_SPC = 5;

        public const int ALARM_CATEGORY_DEVICE_START = 200;   // 100 and above means device alarm

        public const int ALARM_CATEGORY_CRAFT_DATA_START = 1000;
        public const int ALARM_CATEGORY_CRAFT_DATA_OVERFLOW = 1000;   //craft data out of spec

        public const int MAX_ALARM_NUM_ONE_CATEGORY_IN_HISTORY = 1000;

        public const int DEVICE_ALARM_INDEX_LIMIT = 40;  //currently we have only 17 errors for device andon, we limit this value to 40, if the value exceed 40, we believe this is not an device andon alarm, maybe material andon

        //definition for alarm columns, they are used when we put an alarm contents into a string, and then when someone get this string, will need to 
        //restore the string into the origial alarm contents
        public const int ALARM_COLUMN_INDEX_IN_RING = 1;
        public const int ALARM_COLUMN_DATABASE_NAME = 2;
        public const int ALARM_COLUMN_TABLE_NAME = 3;
        public const int ALARM_COLUMN_ID_IN_TABLE = 4;
        public const int ALARM_COLUMN_STATUS = 5;
        public const int ALARM_COLUMN_MAILLIST = 6;
        public const int ALARM_COLUMN_DISCUSSINFO = 7;

        public static int andonAlarmIndex;

        public const int alarmInfoArraySize = 1000;
        public const int maxAlarmDiscussbufferSize = 5000;
        public const int maxAlarmInfoSize = 10000;
        public const int alarmColumnNum = 11;

        public static string alarmHistoryDiscuss;
        public static string alarmHistorySolution;

        public const int ALARM_INHISTORY_UNCHANGED = -1;
        public const int ALARM_INHISTORY_FALSE = 0;
        public const int ALARM_INHISTORY_TRUE = 1;

        public static Mutex AlarmStatusChangeMutex = new Mutex();

        //control alarms dispalyed on screen
        //alarm that is now displayed on screen, also called active alarm, the status of these alarms could be applied/signed/canceled/completed, if some one
        //canceled an alarm, it won't disappear on screen, it will change it status to canceled only
        public const int maxActiveAlarmNum = 1000;
        public const int ACTIVE_ALARM_NUM_INCREASE = 1;
        public const int ACTIVE_ALARM_NUM_DECREASE = 0;
        public static int activeAlarmTotalNumber;
        public static int activeAlarmInfoUpdatedLocally;
        public static int activeAlarmInfoUpdatedCounterpart;
        public static string[] activeAlarmDatabaseNameArray = new string[maxActiveAlarmNum];  //store the database name of the active alarm
        public static int[] activeAlarmIDArray = new int[maxActiveAlarmNum]; //store the ID in database table of the active alarm

        public const int ACTIVE_ALARM_OLD_ALARM = 0;
        public const int ACTIVE_ALARM_NEW_ARRIVED = 1;
        public const int ACTIVE_ALARM_STATUS_CHANGED = 2;
        public static int[] activeAlarmNewStatus = new int[maxActiveAlarmNum];  //store the attribute which indicates whether this alarm is an old one(0), new arrived one(1) or status changed(2) 
        //end of the control of alarms dispalyed on screen

        //control the content changed alarm
        //for example, some one change alarm solution on server or client PC, MESSystem need to tell server and all the clients about the change and popup alarm screen with the changed content
        public static string[] contentChangedAlarmDatabaseNameArray = new string[maxActiveAlarmNum];
        public static int[] contentchangedAlarmIDArray = new int[maxActiveAlarmNum];
        //end control the content changed alarm

        //this machine has an alarm triggered by SPC monitor, so don't do SPC checking any more, we will start new SPC checking after this alarm is cancelled or completed 
        public static int[] SPCTriggeredAlarmArray = new int[maxActiveAlarmNum];

        public static int[] IDForLastAlarmByMachine = new int[maxMachineNum + 1];  //record ID for the last material alarm in 0_alarm table, we use this ID to close an material alarm when dispatch starts
        public static int[] IDForLastAlarmByFactory = new int[maxMachineNum + 1];  //record ID for the last material alarm in allalarm table
        //end of alarm management ******************************************


        //spc related definition ********************************************************
        public const int SPC_CHECKING_SPAN = 6000;  //we will check for SPC status(whether it is still in stable status, otherwise we will trigger an alarm) every 2 minutes

        public const int CHART_TYPE_NO_SPC = 0;  //definition for normal curve with no spc
        public const int CHART_TYPE_SPC_C = 1;
        public const int CHART_TYPE_SPC_XBAR_S = 2;
        public const int CHART_TYPE_SPC_XBAR_R = 3;
        public const int CHART_TYPE_SPC_XMED_R = 4;
        public const int CHART_TYPE_SPC_X_RM = 5;
        public const int CHART_TYPE_SPC_P = 6;
        public const int CHART_TYPE_SPC_PN = 7;
        public const int CHART_TYPE_SPC_R = 8;

        //        public const int SPC_DATA_SPAN = 2000;  //we will get last SPC_DATA_SPAN pieces of data and calculate SPC
        public const int numOfRecordsForPie = 10000;  //get latest number of points to draw PIE and column 
        public const int numOfRecordsInChart = 26;  //number of points for a chart in multiCurve and SPCCurve

        public const int pointNumInSChartGroup = 9;  //
        public const int numOfGroupsInSChart = 30;
        //number of points need to be read for a XBar-S chart, altogether 270
        public const int totalPointNumForSChart = 700; // max number of large roll in a dispatch is 50, max num of small roll is 14, the result is 48 * 14 = 700; pointNumInSChartGroup* numOfGroupsInSChart;

        public const int pointNumInCChartGroup = 5;  //
        public const int numOfGroupsInCChart = 25;
        //number of points need to be read for a C chart, altogether 100
        public const int totalPointNumForCChart = pointNumInCChartGroup * numOfGroupsInCChart;

        //according to http://www.6sq.net/question/60125, craft data can also be used to calculate CPK/PPK, but how to make a group, this is a probblem that need more consideration
        public const int pointNumInNoSPCChartGroup = 5;  //
        public const int numOfGroupsInNoSPCChart = 25;
        //number of points need to be read for a no SPC chart, altogether 100 
        public const int totalPointNumForNoSPCChart = pointNumInNoSPCChartGroup * numOfGroupsInNoSPCChart;

        //        public const int pointNumForCChart = 80; //for quality data, if we wanto to draw a C-chart, we need to ensure there are more than this number of quality data in table
        public const int minDataNumToTriggerAlarm = totalPointNumForNoSPCChart; //for craft data, we won't trigger any alarm until there are more than minDataNumToTriggerAlarm of data in craft table
        public const int minDataForOneScreen = 60;   //minimum number of points do we want to display in one screen in one-curve-mode
        public const int maxDataForOneScreen = 2000;  //maximum number of points do we want to display in one screen in one-curve-mode
        //        public static int oneCurveStartPointPos = 0;  //start point index for one curve display
        //total number of points for current display, this value depends on scrollbar1, if moved to right, value decrease, if left end, equals to total number of data in database
        public static int oneCurvePointNumNeeded = 0;

        //used to record the index for beat data in curve table(normally the first several data in index table are ADC, then VOL/CUR, then quality, then beat)
        public static int beatDataForCurveIndex;
        public const int PERCENTAGE_VALUE_FOR_ONE_CURVE = 1000; //one curve chart is a 1000/1000 chart, we move the scroll bar for one step, the chart will move for 1/1000 left or right

        public static int[] numOfPointsForPie = new int[maxCurveNum];
        public static float[] upperLimitValueForPie = new float[maxCurveNum];  //used for multiCurve data curve display as upper limit
        public static float[] lowerLimitValueForPie = new float[maxCurveNum];  //used for multiCurve data curve display as lowerer limit

        public const int SPC_DATA_GET_UCL_SCL = 0; //we only want to get UCL and LCL
        public const int SPC_DATA_ONLY = 1; //we only want SPC data, no chart is needed
        public const int SPC_DATA_AND_CHART = 2; //we need data to also need data to be put in chart

        public const float SPC_CPK_CONTROLLABLE_LIMIT = 0.9f; //1.33f;

        public const int SPC_DATA_CONTROLLABLE = 0;
        public const int SPC_DATA_UNCONTROLLABLE = 1;

        public static float[,] SPCControlValue = new float[maxCurveNum, 6];  //used for 2 SPCCurve SPC curves(like Xbar and R) as upper/middle/lower control limit
        public static int SPCChartIndex;

        //to indicate whether the SPC checking thread is runnning for this board 
        public static int[] SPCCheckingThreadRunning = new int[maxMachineNum + 1];

        //if data from pone board is under control mode, otherwise continue with observing
        public static int[] dataForThisBoardIsUnderSPCControl = new int[maxMachineNum + 1];

        //if this flag is 1, that means new quality data arrived, so we can continue with SPC checking, otherwise, stop SPC checking for this quality data
        public static int[] newQualityDataArrivedFlag = new int[maxMachineNum + 1];
        //end of spc related definition *********************************************


        public const int CURVE_SPLIT_POSITION = 240;  //curve display screen is split into 2 panels, left panel is current data value, right panel is curve panel
        public const int CURVE_CHART_WIDTH = 554;
        public const int CURVE_CHART_HEIGHT = 260;

        //setting related definition  ******************************************
        //setting function, -1 means from touchpad, otherwise from PC/App
        public const int SETTING_DATA_FROM_TOUCHPAD = -1;
        //setting already sent to board, now we need to make it work in PC
        //        public const int SETTING_DATA_FROM_PC = 1000;
        public const int SETTING_DATA_FROM_APP = 2000;

        public const int NO_SETTING_DATA_TO_BOARD = 0;
        public const int ADC_SETTING_DATA_TO_BOARD = 1;
        public const int UART_SETTING_DATA_TO_BOARD = 2;
        public const int GPIO_SETTING_DATA_TO_BOARD = 3;
        public const int BEAT_SETTING_DATA_TO_BOARD = 4;
        //end of setting related definition  ******************************************

        //email sending function, if server doesnot connected to internet, we cannot send out an email directly, we need an email forwarder(internet accessible) in local network, first send
        //email sending request to this forwarder via LAN, then this email forwarder send email request to 163 email server, 163 email server will send out enmail directly  
        public static int emailForwarderHeartBeatNum;
        public static int emailForwarderHeartBeatOld;
        public static Socket emailForwarderSocket;
        //end of email sending function

        //host server PC and client PC
        public const int PC_CLIENT_CONNECTED = 1;
        public const int PC_CLIENT_DISCONNECTED = 0;
        public static Mutex clientPCConnectionInfoUpdatedMutex = new Mutex();  //we use this mutex to ensure connection/disconnection of new PC client(new account) won't have problem for the 3 variables below  

        //all the manipulation of the 3 variables below should be under the control of clientPCConnectionInfoUpdatedMutex
        public static int clientPCConnectionNumber;
        public static string[] clientPCConnectionStatusArray = new string[maxClientPC];
        public static string[] clientPCAccountListArray = new string[maxClientPC];
        //end of host server PC and client PC

        //public static Semaphore semaphoreForAndonPush;
        //        public static Socket[] socketServer = new Socket[maxNumDiscussClient];
        //we need to remember which room we came from, and need to go back to this room after exit
        //        public static int fromWhichRoom;
        //we need to remember which form we came from, and need to go back to this form after exit
        //        public static int fromWhichForm;

        //tell multiCurve.cs, dispatch status has changed, we need to redo curve display
        public static int refreshMultiCurve;

        //this is used to set the max current value for a machine, exceeding this value will lead to a andon alarm
        public static float[] maxMachineCurrentValue = new float[maxMachineNum + 1];

        //this dispath code is only used for curve display, may not be the same as gVariable.dispatchSheet[myBoardIndex].dispatchCode
        //        public static string[] currentDispatchCode = new int[maxMachineNum + 1];

        //this flag indicates what is the current status for a machine, shutdown/idle/dummy/dispatch applied/started/completed, if no dispatch started, quality/craft/andon data upload to PC are legal
        public static int[] machineCurrentStatus = new int[maxMachineNum + 1];
        public static string[] scannedData = new string[maxMachineNum + 1];

        //for data table that will be dislayed in curve, if we don't consider PC client
        //        public static string[] craftDataTableName = new string[maxMachineNum + 1];
        //        public static string[] volcurDataTableName = new string[maxMachineNum + 1];
        //        public static string[] qualityDataTableName = new string[maxMachineNum + 1];
        //        public static string[] beatDataTableName = new string[maxMachineNum + 1];

        public static int[] totalCurveNum = new int[maxMachineNum + 1];  //need to know how many curves we need to draw by check the port info

        public static int[] machineStartTimeStamp = new int[maxMachineNum + 1];
        public static string[] machineStartTime = new string[maxMachineNum + 1];

        //current(ampere) value for all boards at this time point, we will record current value for all boards into database 
        public static float[] currentValueNow = new float[maxMachineNum + 1];

        //all current(ampere) values for this board for today(every minute) recorded from currentValueNow
        public static int[] currentStatusForOneDay = new int[1500];
        //record status for every minute for all devices
        public static int[,] dispatchAlarmIDForOneDay = new int[maxMachineNum + 1, 1440];

        public static string[] today = new string[maxMachineNum + 1];
        public static string[] today_old = new string[maxMachineNum + 1];

        //
        public static int[,] workingMachineForCurrentDispatch = new int[maxMachineNum + 1, 3];

        //both data collect board and app can apply new dispatch, the following definitions define who should have the privilege to do so  
        //every machine can have one of the following comunication type below
        public const int typeAllFromBoard = 0;  //data collect board will provide machine data and dispatch control, App can only review data
        public const int typeDataOnlyFromBoard = 1;  //data collect board will only provide machine data, App can review data and control dispatch process
        public const int typeNothingFromBoard = 2;   //no data collect board, so no machine data will be sent to server, App will control dispatch process
        public static int[] machineCommunicationType = new int[maxMachineNum + 1];

        public static int[] connectionStatus = new int[maxMachineNum + 1];
        public static int[] connectionCount = new int[maxMachineNum + 1];
        public static int[] connectionCount_old = new int[maxMachineNum + 1];
        public static Socket[] socketArray = new Socket[maxMachineNum + 1];

        public static string hostString;
        //        public static string communicationHostIP;

        public static int boardIndexSelected;

        public const int needInformOthers = 1;
        public const int noNeedInformOthers = 0;

        //what kind of time condition we want to check when we search for relative dispatches
        public const int TIME_CHECK_TYPE_PLANNED_START = 0;
        public const int TIME_CHECK_TYPE_PLANNED_FINISH = 1;
        public const int TIME_CHECK_TYPE_REAL_START = 2;
        public const int TIME_CHECK_TYPE_REAL_FINISH = 3;
        public const int TIME_CHECK_TYPE_REAL_START_FINISH = 4;
        public const int TIME_CHECK_TYPE_REAL_ONE_POINT = 5;


        //the following 3 items are used by client server, to record its current alarm status/mail list and discuss info
        //for server side, these 3 items will be recorded in database, but for client PC, database is read only, so we need to
        //record these 3 items in global then send it to server PC
        public static int clientalarmStatus;
        public static string clientMailList;
        public static string clientDiscussInfo;
        public static string clientUserName;
        //

        //every alarm has 3 paramters to idicate which alarm this is,
        //1st: database name
        //2nd: table name
        //3rd: id in this table
        public const int ALARM_CONTENT_DATABASE_NAME = 0;
        public const int ALARM_CONTENT_TABLE_NAME = 1;
        public const int ALARM_CONTENT_ID_IN_TABLE = 2;
        //        public const int ALARM_CONTENT_ALARM_TYPE = 3;

        //clientSocket is used in client mode only, so only one socket exists, when the program is performed in client mode, we use this socket to close connection when client PC close program
        public static Socket clientSocket;

        public static string machineCodeCurrent;
        public static string currentDatabaseName;

        public static string databaseNameAlarm;
        public static string dispatchCodeAlarm;
        public static string machineCodeAlarm;
        public static string errorDescAlarm;
        public static string alarmFailureCodeAlarm;
        public static string machineNameAlarm;
        public static string operatorNameAlarm;
        public static string timeOfWarningAlarm;
        public static string timeOfAlarmSigned;
        public static string timeOfAlarmEnded;
        public static string typeOfWarningAlarm;
        public static string mailListAlarm;  //real mail list that a client can add or remove
        public static int statusOfAlarm;
        public static int typeOfAlarm;
        public static int categoryOfAlarm;
        public static string workshopAlarm;  //real mail list that a client can add or remove

        public static int maxNumMailList = 100;
        public static string basicmailListAlarm;  //original mail list in database, every one change change this

        public const int intervalForX = 5;  //interval of points for date/time display

        public static int numOfGPIOs = 16;

        public static bool thisIsHostPC;

        //setting from touch pad or PC host, and maybe in the future from APP
        //-1: from touchpad, 
        //0 - 138: from PC to board, 
        //2000: setting data from app
        public static int whereComesTheSettingData;

        //PC set new setting data to board
        //0, no setting, 1/2/3/4 means what kind of setting that will be send to board
        public static int whatSettingDataModified;

        public static string previousDispatch;  //this is used in dispatchUI, to check whether dispatch content has change, if yes, we nned ti refresh screen

        public const int ALARM_NO_STARTID = 0;
        public const int ALARM_NO_INDEX_IN_TABLE = -1;

        public static string currentCurveDatabaseName;  //current database(or machine code) that is now been displayed as curve

        //info writer to record all board online/offline process 
        public static StreamWriter infoWriter;
        //stream writer to record all data received by server
        public static StreamWriter[] dataLogWriter = new StreamWriter[maxMachineNum];
        public static StreamWriter[] errorLogWriter = new StreamWriter[maxMachineNum];

        //data index for all kinds of data in database that need to be displayed as curves
        public const int CRAFT_DATA_IN_DATABASE = 0;
        public const int VOLCUR_DATA_IN_DATABASE = 1;
        public const int BEAT_DATA_IN_DATABASE = 2;
        public const int QUALITY_DATA_IN_DATABASE = 3;
        public const int maxCurveTypes = 4;

        public static int wifiErrorNum;  //total fail number, used in final report 
        public static int willClose;

        public static string textRF, textScanner, textPrinter, textRFID;

        public static string text1, text2, text3, text4, text5, text6, text7, text8, text9, text10, text11, text12, text13, text14, text15;
        public static string text16, text17, text18, text19, text20, text21, text22, text23, text24, text25, text26, text27, text28, text29, text30;
        public static string text31, text32, text33, text34, text35, text36;

        public static string[] curveTextArray = { text1,  text2,  text3,  text4,  text5,  text6,  text7,  text8,  text9,  text10, text11, text12, 
                                                  text13, text14, text15, text16, text17, text18, text19, text20, text21, text22, text23, text24,
                                                  text25, text26, text27, text28, text29, text30, text31, text32, text33, text34, text35, text36, 
                                                };
        //bit0 GPIO1, bit1 GPIO2, bit2 GPIO3 ...
        public static int gpioStatus = 0;  //assume every GPIO is low at beginning, maximum 20 gpios

        public static System.DateTime worldStartTime;

        //private int[] startLineNum = new int[12]; //曲线中起始记录的索引 

        //Now we get these values from product info excel file, not prefixed any more
        //public static float[] curveSTDValue = { 3.5F, 3.5F, 0.3F, 0.7F, 15.0F, 100.0F, 1, 1, 222, 0.038F, 3.8F, 50, 222, 0.038F, 3.8F, 50 };    //input value
        //public static float[] curveDeltaValue = { 0.1F, 0.1F, 0.1F, 0.3F, 8.0F, 0.1F, 0.1F, 0.1F, 0.4F, 0.02F, 0.2F, 5, 0.4F, 0.02F, 0.2F, 5 };  //tolerance

        //for debug purpose only, record number of data received for this port, can be output to file in debug mode 
        public static int[] currentDataIndex = new int[maxCurveNum];

        public static int[] curveOrNot = new int[maxCurveNum];  //indicating whether this port data will be displaed in curve
        //public static string[] dispatchBasedPortTableName = new string[maxCurveNum];  //table name of the port data in database, even if this port is not used as curve
        public static string[] dispatchBasedCurveTableName = new string[maxCurveNum];  //table name of the port data in database, mainly used for curve display

        //for curve port only
        //        public static int[] curveIndexInItsOwnTable = new int[maxCurveNum];  //for a craft data, it means index in craft list, for quality data, it means index in quality list
        public static int[] numOfCurveForOneType = new int[maxCurveTypes];  //number of curves for each of the 4 kinds of data (craft/volcur/quality/beat)
        public static string[] curveTitle = new string[maxCurveNum];  //title for curves
        public static int[] dataNumForCurve = new int[maxCurveNum];  //total record number for curves
        public static float[] curveUpperLimit = new float[maxCurveNum];
        public static float[] curveLowerLimit = new float[maxCurveNum];
        //        public static string[] curveUnit = new string[maxCurveNum];

        public static int fileDataInWriting;  //indicating whether file writing is undergoing, if yes, avoid this action to be performed by other thread 
        public static byte[] fileDataBuf = new byte[fileDataLength];

        public static float[,] dataInPoint = new float[maxCurveNum, totalPointNumForSChart];   //data buffer for 36 curves
        public static int[,] timeInPoint = new int[maxCurveNum, totalPointNumForSChart];   //time buffer for 36 curves
        //public static int[,] dataStatus = new int[maxCurveNum, totalPointNumForSChart];   //data status, qualified or unqualified data

        public static float[] oneCurveDataInPoint = new float[4100];   //data buffer for one curve
        public static int[] oneCurveTimeInPoint = new int[4100];   //time buffer for one curve
        public static int[] oneCurveIndexInPoint = new int[4100];   //index buffer for one curve

        public static float[] minDataValue = new float[maxCurveNum];
        public static float[] maxDataValue = new float[maxCurveNum];

        public const int numOfColumns = 10;
        public static float[,] columnLimits = new float[maxCurveNum, numOfColumns + 1];
        public static int[,] columnData = new int[maxCurveNum, numOfColumns];

        public static string DBHeadString;

        //this internal name is used to generate database, one machine uses one database name
        public static string[] internalMachineName = new string[maxMachineNum + 1];

        public const int button_Index_Not_Valid = 1000;
        public const int totalButtonLineNum = 10;
        public const int totalButtonColumnNum = 16;
        public const int totalButtonNum = totalButtonColumnNum * totalButtonLineNum;

        //list between button index and board index
        public static int[,] buttonBoardIndexTable = new int[numOfWorkshop, totalButtonNum];

        public static errorCodeListStruct errorCodeList;	//错误编码表
        //public static salesOrderStruct salesOrderImpl = new salesOrderStruct();
        public static BOMListStruct[] BOMList = new BOMListStruct[maxMachineNum];       //原料配比表

        public static dispatchSheetStruct[] productTaskSheet = new dispatchSheetStruct[maxMachineNum];  //工单表
        public static dispatchSheetStruct[] dispatchSheet = new dispatchSheetStruct[maxMachineNum];  //工单表
        public static machineStatusStruct[] machineStatus = new machineStatusStruct[maxMachineNum];  //设备状态表
        public static craftListStruct[] craftList = new craftListStruct[maxMachineNum];	      //工艺参数表
        public static qualityListStruct[] qualityList = new qualityListStruct[maxMachineNum];	    //质量检测表
        public static materialListStruct[] materialList = new materialListStruct[maxMachineNum];       //物料配送单列表

        public static beatPeriodStruct[] beatPeriodInfo = new beatPeriodStruct[maxMachineNum]; //beat setting value for currents
        public static ADCChannelStruct[] ADCChannelInfo = new ADCChannelStruct[maxMachineNum]; // ADC settings
        public static uartSettingStruct[] uartSettingInfo = new uartSettingStruct[maxMachineNum];
        public static GPIOSettingStruct[] GPIOSettingInfo = new GPIOSettingStruct[maxMachineNum];

        //故障编码表结构
        public struct errorCodeListStruct
        {
            public int errorCodeListSize;  //错误编码数量
            public string[] errorCode;
            public string[] errorCodeDesc;
        };

        //订单表结构
        public struct salesOrderStruct
        {
            public string ID;  //ID in sales order table
            public string salesOrderCode;
            public string deliveryTime;  //交货期
            public string productCode;	 //产品编码
            public string productName;  //产品名称
            public string requiredNum;	//计划生产数量
            public string unit;  //数量的单位
            public string customer; //客户名
            public string publisher; //下发者
            public string ERPTime; //ERP 下发时间
            public string APSTime; //APS 排程确认时间(包括手工调整)
            public string planTime1;	//预计开工时间
            public string planTime2;  //预计完工时间
            public string realStartTime;	//预计开工时间
            public string realFinishTime;  //预计完工时间
            public string source; //0: 手工输入; 1: ERP 导入
            public string status; //0：ERP published; 1：APS OK; 2：production started(the first dispatch started); 3：sales order completed; 4：sales order   
            public string cutTime; //分拆时间
            public string cutResult; //分拆结果
        };

        //批次单表结构
        public struct productBatchStruct
        {
            public string ID;  //ID in sales order table
            public string salesOrderBatchCode;  //批次订单号，把订单划成批次单后，原订单号 + 下标
            public string originalSalesOrderCode;  //原来的订单号
            public string deliveryTime;  //交货期
            public string productCode;	 //产品编码
            public string productName;  //产品名称
            public string requiredNum;	//计划生产数量
            public string unit;  //数量的单位
            public string customer; //客户名
            public string publisher; //下发者
            public string source; //订单来源 -- 0: 手工输入; 1: ERP 导入
            public string ERPTime; //ERP 下发时间
            public string APSTime; //APS 排程确认时间(包括手工调整)
            public string planTime1;	//预计开工时间
            public string planTime2;  //预计完工时间
            public string realStartTime;	//预计开工时间
            public string realFinishTime;  //预计完工时间
            public string status; //0：ERP published; 1：APS OK; 2：production started(the first dispatch started); 3：sales order completed; 4：sales order   
            public string cutTime; //分拆时间
            public string batchNum;  //生产批次号
            public string canceller; //批次单取消人
            public string cancelReason;  //取消原因
            public string cancelTime;   //取消时间
            public string approver;   //核准人
            public string productCode2;
            public string customer2;
            public string productCode3;
            public string customer3;
            public string productCode4;
            public string customer4;
			
        };

        //产品表结构
        public struct productStruct
        {
            public string customer;
            public string productCode;	 //产品编码
            public string productName;  //产品名称
            public string BOMCode;    //配方
            public string routeCode; //工艺路线
            public string productWeight;
            public string productDiameter;
            public string productWidth;
            public string productfixtureNum;
            public string productLength;
            public string baseColor;  //基色
            public string patternType;  //压纹类型
            public string RawMaterialCode;  //客户原材料代码
            public string inkRatio;   //油墨配比
            public string numOfMaterialType;  //原料钟数
            public string materialcode1;
            public string materialcode2;
            public string materialcode3;
            public string materialcode4;
            public string materialcode5;
            public string materialcode6;
            public string materialcode7;

            public string rollNumLayer;                 //底层卷数	                   
            public string layerNum;                     //堆高层数	              
            public string rollNumStack;                 //每托盘卷数	            
            public string totalWeightStack;             //每铲板产品重量(kg)	    
            public string stackType;                    //铲板类型	              
            public string stackLength;                  //铲板长(mm)	            
            public string stackwidth;                   //铲板宽(mm)	            
            public string recycle;                      //是否回收                
            public string stackLossRate;                //铲板损耗率1:            
            public string stackNumOneTon;               //吨产品铲板用量(个)	    
            public string paperCoreDiameter;            //纸芯内径(mm)	          
            public string paperCoreThickness;           //纸芯壁厚(mm)	          
            public string paperCoreLength;              //纸芯长度(mm)	          
            public string paperCoreOneStack;            //铲板纸芯用量(m)	        
            public string paperCorreLossRate;           //纸芯损耗率1:	          
            public string paperCoreOneTon;              //吨产品纸芯用量(m)	      
            public string paperBoardOneStack;           //铲板纸板用量(块)	      
            public string paperBoardLossRate;           //纸板损耗率1:	          
            public string paperBoardOneton;             //产品纸板用量(块) 	      
            public string craftPaperOneStack;           //铲板牛皮纸用量(m2/托盘)	
            public string craftPaperLossRate;           //牛皮纸损耗率1：         
            public string craftPaperOneTon;             //吨产品牛皮纸用量(m2)    
            public string corrugatedPaperOneStack;      //铲板瓦楞纸用量(m2/托盘) 
            public string corrugatedPaperLossRate;      //瓦楞纸损耗率1：         
            public string corrugatedPaperPerTon;        //吨产品瓦楞纸用量(m2)	  
            public string wrappingFilmOneStack;         //铲板缠绕膜用量(kg/托盘)	
            public string wrappingFilmOneTon;           //吨产品缠绕膜用量(kg)	  
            public string others;                       //其他	                  
            public string packingFilmOneStack;          //铲板包装用膜量(kg/托盘)	
            public string packingFilmOneTon;            //吨产品包装用膜用量(kg)" 
            public string craftSheetCode1;              //工艺单编号1
            public string craftSheetCode2;              //工艺单编号2
            public string craftSheetCode3;              //工艺单编号3
            public string criteria;                     //检验标准
            public string multiIngredient;              //是否多种配方
            public string slitOnline;                   //是否在线分切
        };

        //设备空闲时间表 -- for APS
        public struct machinePlanStruct
        {
            public string maintenanceID;
            public string saleOrderCode;
            public string dispatchCode;
            public string planTime1;
            public string timeStamp1;
            public string duration;
            public string planTime2;
            public string timeStamp2;
            public string productCode;
            public string productName;
            public string forcastNum;
            public string customer;
        };

        //设备状态持续时间表表 -- 用于记录设备运行/待机/停止的持续时间，用于生产状态的图形显示介面
        public struct machineStatusRecordStruct
        {
            public string recordTime;
            public string machineStatus;
            public string statusStart;
            public string keepMinutes;
        };

        //生产任务单表结构，任务单指一个批次的生产，可能两个小时就换产，可能持续 3 天，员工号栏位无意义，工时较长
        //工单表结构，工单可能两个小时就换产，最多 12 个小时，一个班次，指定到人，工时最多 12 个小时
        //都使用同一结构，
        public struct dispatchSheetStruct
        {
            public string machineID;   //设备序号 
            public string dispatchCode;  //dispatch code
            public string planTime1;	//预计开始时间
            public string planTime2;  //预计结束时间
            public string productCode;	 //产品编码
            public string productName;  //产品名称
            public string operatorName; //操作员
            public float plannedNumber;	//计划生产数量
            public float outputNumber;  //接收数量
            public float qualifiedNumber;  //合格数量
            public float unqualifiedNumber;  //不合格数量
            public string processName; //工序（工艺路线中包含多个工序）
            public string realStartTime;	  //实际开始时间
            public string realFinishTime;	  //实际完工时间
            public string prepareTimePoint;   //调整时间（试产时间），开工后先经过调整时间，然后再正式生产
            public int status;	  //0：dummy/prepared 工单准备完成但未发布，1:工单完工，新工单未发布，2：工单发布至设备 3：工单已启动 4：试产（调整时间） OK
            public int waitForCheck;  //待检品
            public int wastedOutput;  //废料
            public string workTeam;  //班别(甲乙丙)
            public string batchNum2; //套作时,第三个产品的批次号
            public string batchNum3; //套作时,第四个产品的批次号
            public string workshop;	 //车间名称，或者是流水线的名字
            public string workshift;	 //班次（早中晚班）
            public string salesOrderCode; //订单号
            public string BOMCode; // 
            public string customer;
            public string multiProduct; //套做标记
            public string productCode2; //套做中的第二种产品
            public string productCode3; //套做中的第三种产品
            public string operatorName2; //操作员2
            public string operatorName3; //操作员3
            public string batchNum; //批次号
            public string batchNum1; //套作时,第二个产品的批次号
            public string rawMateialcode;  //原材料代码（客户的产品编码，对于客户而言紫华的产品就是客户的原料）
            public string productLength; //卷长 
            public string productDiameter;  //卷径
            public string productWeight;  //卷重
            public string slitWidth;  //分切宽度 
            public string salesOrderBatchCode;  //批次订单号
            public string productCode4; //套做中的第四三种产品
            public string operatorName4; //操作员4
            public string notes;   //交接班记录
            public string comments;  //领导批示
        };

        //设备状态表结构
        public struct machineStatusStruct
        {
            public string recordTime;  //记录时间
            public string machineID;  //设备序号
            public string machineCode;  //设备编码
            public string machineName;  //设备名
            public string dispatchCode;  //当前工单号
            public string startTime;	  //开机时间
            public string offTime;	  //关机时间
            public int totalWorkingTime;  //作业总时间
            public float collectedNumber;  //设备采集件数
            public int productBeat;  //生产节拍
            public int workingTime;  //生产时间
            public int prepareTime;  //调整时间待机时间
            public int standbyTime;  //待机时间
            public int power; //功率
            public int powerConsumed;  //能耗
            public int pressure;  //压力
            public int revolution;  //转数
            public int maintenancePeriod;   //保养周期
            public int lastMaintenance;   //最近一次保养
            public int waitForCheck;  //待检品
            public int wastedOutput;  //废料
        };


        //工艺参数表结构
        public struct craftListStruct
        {
            public int paramNumber;  //工艺参数个数
            public int workingVoltage;
            public int[] id;
            public string[] paramName;
            public float[] paramLowerLimit;
            public float[] paramUpperLimit;
            public float[] paramDefaultValue;
            public string[] paramUnit;
            public float[] paramValue;
            public float[] rangeLowerLimit;
            public float[] rangeUpperLimit;
        };

        //质量检测表结构
        public struct qualityListStruct
        {
            public int checkItemNumber;  //质量检测参数个数
            public int[] id;
            public string[] checkItem;
            public float[] controlCenterValue1;
            public float[] controlCenterValue2;
            public string[] checkRequirement;
            public float[] specLowerLimit;
            public float[] controlLowerLimit1;
            public float[] controlLowerLimit2;
            public float[] specUpperLimit;
            public float[] controlUpperLimit1;
            public float[] controlUpperLimit2;
            public int[] sampleRatio;
            public string[] checkResultData;
            public string[] checkResult;
            public string[] unit;
            public int[] chartType;
        };

        //设备安灯报警表
        public struct alarmTableStruct
        {
            public string errorDesc;  //error description
            public string dispatchCode;
            public string alarmFailureCode;  //alarm sheet code
            public string feedBinID;
            public string machineName;
            public string operatorName;
            public string time;
            public string signer;
            public string time1;
            public string completer;
            public string time2;
            public int type;
            public int category;
            public int status;
            public int inHistory;
            public int startID;
            public int indexInTable;
            public string workshop;
            public string mailList;
            public string discuss;
            public string solution;
            public int deviceFailureIndex;  //index in error description table
        };

        //原料表 -- 场内原料列表
        public struct materialTableStruct
        {
            public string materialCode;
            public string materialName;
            public string materialStage;
            public string materialType;
            public string minPacket;
            public string unit;
            public string packet;
            public string vendor;
            public string fullstackNum;
            public string price;
        };

        //生产物料表 -- 附属于某一个工单
        public struct materialListStruct
        {
            public string salesOrderCode;
            public string dispatchCode;
            public string machineID;
            public string machineCode;
            public string machineName;
            //public string status;  //like 
            public int numberOfTypes;  //number of kinds of material
            //public string[] materialName;  //物料名称（包括计量单位）
            public string[] materialCode;  //物料编码
            public int[] materialRequired;  //物料需求数量
            public int[] previousLeft;  //上一班次物料余量
            public int[] currentlyUsed;  //本班物料消耗
            public int[] currentLeft;  //料仓中的余料
            public string salesOrderBatchCode;
            public string batchNum;
            public string materialStatus;  //"0000000": 不缺料; "1000000":1号料仓缺料; "1111111": 7个料仓缺料 
        };

        //物料需求表, 物料搬运工每天按表从仓库领料
        public struct materialRequirementStruct
        {
            public string materialRequiresheetCode;
            public string salesOrderCode;
            public string salesOrderBatchCode;
            public string batchNum;
            public string status;  //0:generated; 1:started; 2:completed 
            public string machineID;
            public int numberOfTypes;  //number of kinds of material
            public string[] materialCode;  //物料编码
            public int[] materialRequired;  //物料需求数量
        };

        //原料配比表 -- 基础数据的一部分，与工单及设备无关
        public struct BOMListStruct
        {
            public string BOMCode;
            public int numberOfTypes;  //number of kinds of material
            public string[] materialCode;  //物料编码
            public string[] materialName;  //物料名称（包括计量单位）
            public double[] materialQuantity;  //物料数量
        };

        //员工列表  -- 基础数据的一部分
        public struct employeeListStruct
        {
            public string workerID;
            public string workername;
            public string sex;
            public string age;
            public string department;
            public string position;
            public string date;
            public string password;
            public string rank;
            public string salary;
            public string privilege1;
            public string privilege2;
            public string privilege3;
            public string privilege4;
            public string mailAddress;
        };

        public struct machineListStruct
        {
            public string internalCode;
            public string machineCode;
            public string machineName;
            public string machineType;
            public string machineVendor;
            public string purchaseDate;
            public string machinePrepareTime;
            public string machineWorkingTime;
            public string preferValue;
            public string maintenancePeriod;
            public string maintenanceHour;
        };
        public struct castCraftStruct
        {
            public string castCraftCode;
            public string castCraft_C1;
            public string castCraft_C2;
            public string castCraft_C3;
            public string castCraft_C4;
            public string castCraft_C5;
            public string castCraft_C6;
            public string castCraft_C7;
            public string castCraft_C8;
            public string castCraft_C9;
        };

        public struct castQualityStruct
        {
            public string castQualityCode;
            public string castQualityReelWeight;
            public string castQualityReelThickness;
            public string castQualityReelCorona;
            public string castQualityReelWidth;
            public string castQualityReelDiameter;
            public string castQualityReelLength;
        };

        public struct printCraftStruct
        {
            //index in print craft
            public string printerCraftCode;
            public string printerCraftLineSpeed;
            public string printerCraftOvenTemperature;
            public string printerCraftKnifePressure;
            public string printerCraftKnifeAngle;
            public string printerCraftInkVicosity;
            public string printerCraftPrintRollPressure;
            public string printerCraftReleaseStrain;
            public string printerCraftInletStrain;
            public string printerCraftOutletStrain;
        };

        public struct printQualityStruct
        {
            public string printerQualityCode;
            public string printerQualityReelWidth;
            public string printerQualityReelDiameter;
            public string printerQualityCoronaSide;
            public string printerQualityCoronaDegree;
            public string printerQualityPatternDirection;
            public string printerQualityPatternPosition;
            public string printerQualityPatternCompleteness;
            public string printerQualityDefects;
        };

        public struct slitCraftStruct
        {
            public string slitCraftCode;
            public string slitCraftLineSpeed;
            public string slitCraftReleaseStrain;
            public string slitCraftTuckInitStrain1;
            public string slitCraftTuckTaper1;
            public string slitCraftTuckNumber1;
            public string slitCraftTuckInitStrain2;
            public string slitCraftTuckTaper2;
            public string slitCraftTuckNumber2;
        };

        public struct slitQualityStruct
        {
            //index in slit quality
            public string slitQualityCode;
            public string slitQualityReelWidth;
            public string slitQualityReelDiameter;
            public string slitQualityReelLength;
            public string slitQualityReelWeight;
            public string slitQualityReelThickness;
            public string slitQualityReelCorona;
            public string slitQualityPatternDirection;
            public string slitQualityPatternCompleteness;
            public string slitQualityDefects;
        };

        public struct beatPeriodStruct
        {
            public int deviceSelection;
            public float idleCurrentHigh;
            public float idleCurrentLow;
            public float workCurrentHigh;
            public float workCurrentLow;
            public int gapValue;
            public int peakValue;
        };

        public struct ADCChannelStruct
        {
            public int[] channelEnabled;
            public string[] channelTitle;
            public string[] channelUnit;
            public float[] lowerRange;
            public float[] upperRange;
            public float[] lowerLimit;
            public float[] upperLimit;
            public int workingVoltage;
        };

        public struct uartSettingStruct
        {
            public int selectedUart;
            public int[] uartBaudrate;
            public string[] uartInputData;
            public string[] uartOutputData;
        };

        public struct GPIOSettingStruct
        {
            //            public int[] GPIOPINEnabled;
            public int[] GPIOTriggerVoltage;
            public string[] GPIOName;
        };

        
        public const string exitStr = "退出";
        public const string dispatchUIListTitle0 = "序号";
        public const string dispatchUIListTitle1 = "角色";
        public const string dispatchUIListTitle2 = "检验项目";
        public const string dispatchUIListTitle3 = "检验要求";
        public const string dispatchUIListTitle4 = "公差下限";
        public const string dispatchUIListTitle5 = "控制下限";
        public const string dispatchUIListTitle6 = "公差上限";
        public const string dispatchUIListTitle7 = "控制上限";
        public const string dispatchUIListTitle8 = "抽样件数";
        public const string dispatchUIListTitle9 = "检验结果";
        public const string dispatchUIListTitle10 = "结果判断";

        public const string deviceAlarmScreenTitle = "设备安灯列表";
        public const string deviceAlarmListTitle0 = "序号";
        public const string deviceAlarmListTitle1 = "安灯编号";
        public const string deviceAlarmListTitle2 = "设备名称";
        public const string deviceAlarmListTitle3 = "料仓编号";
        public const string deviceAlarmListTitle4 = "安灯类型";
        public const string deviceAlarmListTitle5 = "安灯描述";
        public const string deviceAlarmListTitle6 = "呼叫人";
        public const string deviceAlarmListTitle7 = "呼叫时间";
        public const string deviceAlarmListTitle8 = "安灯状态";
        public const string deviceAlarmListTitle9 = "签到者";
        public const string deviceAlarmListTitle10 = "操作者";
        public const string deviceAlarmListTitle11 = "签到时间";
        public const string deviceAlarmListTitle12 = "错误分析";
        public const string deviceAlarmListTitle13 = "处理措施";
        public const string deviceAlarmListTitle14 = "处理/取消时间";

        public const string materialAlarmScreenTitle = "物料安灯列表";
        public const string materialAlarmListTitle0 = "勾选";
        public const string materialAlarmListTitle1 = "序号";
        public const string materialAlarmListTitle2 = "派工单号";
        public const string materialAlarmListTitle3 = "物料批次号";
        public const string materialAlarmListTitle4 = "设备名称(接收点)";

        public const string deviceAlarmListItem1 = "签到";
        public const string deviceAlarmListItem2 = "结果确认";
        public const string deviceAlarmListItem3 = "取消安灯";

        public static string[] SPCAnalyzeDataTitle = { "基本数据", "CPK数据", "控制图数据" };

        public static string[,] SPCAnalyzeDataStr = new string[3, 8]
        {
            {"样本总数", "规格上限", "规格中心", "规格下限", "最大值", "最小值", "平均值", ""},
            {"Cp", "Cpk", "Cpl", "Cpu", "Pp", "Ppk", "Ppl", "Ppu"},
            {"XBar控制上限", "XBar中心线", "XBar控制下限", "S图控制上限", "S图中心线", "S图控制下限", "", ""},
        };

        public static string[] alarmListTitle = { "设备安灯报警记录列表", "物料安灯报警记录列表", "质量数据报警记录列表", "质量数据报警记录列表", "工艺参数报警记录列表", "工单未完成报警记录列表", "所有安灯报警记录列表" };
        public static string[] alarmListHistoryTitle = { "设备安灯报警经验列表", "物料安灯报警经验列表", "质量数据报警经验列表", "工艺参数报警经验列表", "工单未完成报警经验列表", "所有安灯报警经验列表" };

        public const string strOfUnkown = "未知";
        public const string serverPCUserName = "服务器使用者";

        public const string strExitOrNot = "确定要退出智能数据采集系统吗?";
        public const string strConfirm = "确认";

        public static string[] productStatusList = { "合格品", "不合格品", "待处理品", "废料", "边角料", "复卷合格", "复卷不合格" };

        public static string[] deviceErrDescList = { "A-晶点孔洞", "B-厚薄暴筋", "C-皱折", "D-端面错位(毛糙)", "DC-待处理", "E-油污", "W-蚊虫" };
        
        public static string[] inspectionErrorList =
        {
            "规格不符", "卷径不达标", "米数不足", "基重超标", "厚度不符", "电晕值不达标", "孔洞", "亮斑", "条纹", "杂质、晶点或鱼眼", "压纹不清", "皱折", 
            "颜色太深", "颜色太浅", "年轮色差", "蚊虫", "异物", "水印", "溅墨", "刀丝、拖墨", "漏印", "露白", "图案缺失", "图案分离", "油墨牢度不够", 
            "印刷颜色超出标准", "印刷色相不符", "印刷出头方向错误", "印刷面收卷方向错误", "电晕面收卷方向错误", "压纹收卷方向错误", "收卷过松", "收卷过紧", 
            "暴筋", "厚薄不均", "端面错位", "切边断裂", "贴黑胶带或铝薄", "拉伸性能不达标", "PH超标", "WVTR不符", "摩擦系数不符", "耐水压不符",
        };


        public static string[] castErrorList =
        {
            "加热温度异常", "吸料异常", "螺杆无动作", "水接头漏水", "机器跳停", "摇摆无动作", "轴承坏", "收卷切刀无动作", "温水机组漏水", "温水机组不加热", 
            "冷冻机跳停", "循环水泵坏", "电晕不工作", "气管漏气", "气路有水", "模头水箱开锅", "其它",
        };

        public static string[] printErrorList =
        {
            "烘箱温度加不上", "轴承坏", "机器无动作", "前后速度不一致", "气路漏气", "其它",
        };

        public static string[] slitErrorList =
        {
            "收卷皮带断", "收卷轴漏气", "收卷有黑点", "设定数据不准", "其它",
        };

        public static string[] packingErrorList =
        {
            "没有升降", "平台不旋转", "设定圈数不准", "其它",
        };

        public static string[] rebuildErrorList =
        {
            "机器不动作", "切粒太粗", "机器有异声", "皮带断", "其它",
        };

        public static string[] commonErrorList =
        {
            "空压机停机", "压缩空气有水", "循环泵坏", "地坑泵坏", "跳电", "其它",
        };


        public static string[] errSPCDescList = { "数据超出控制限", "至少连续9个点位于中心线同一侧", "7个点连续上升或者下降", "15个点落在中心线附近", "5个连续点中至少4个距离中心线较远" };
        public const string errCraftDesc = "数据超出规格限";

        public static string dispatchPartBase = "型齿轮";

        public static string[] dispatchOperatorNameBase = 
        {
            "蔡文亮", 
            "蔡文亮", "章其",  "刘清",   "王玉",   "洪峰",   "许晴",   "凌云",  "刘亦铭", "管陵",   "张勤", 
            "李洪",   "李群",  "陈逸飞", "赵琪君", "王欣欣", "刘丽",   "邱明慧", "吴琴",  "曾子铭", "王洪江", 
            "柳佳慧", "秦珊",  "方方",   "李云",   "黄宏",   "尹建玲", "张鑫灵", "梅芬",  "万金环", "于隐", 
            "李月",   "王青青", "博辛", "程子洁", "藤青", "刘鹤", "朱良云", "丁晓", "张小芳", "灵灵", "于隐",
            "蔡文亮", "章其",  "刘清",   "王玉",   "洪峰",   "许晴",   "凌云",  "刘亦铭", "管陵",   "张勤", 
            "李洪",   "李群",  "陈逸飞", "赵琪君", "王欣欣", "刘丽",   "邱明慧", "吴琴",  "曾子铭", "王洪江", 
            "柳佳慧", "秦珊",  "方方",   "李云",   "黄宏",   "尹建玲", "张鑫灵", "梅芬",  "万金环", "于隐", 
            "蔡文亮", "章其",  "刘清",   "王玉",   "洪峰",   "许晴",   "凌云",  "刘亦铭", "管陵",   "张勤", 
            "李洪",   "李群",  "陈逸飞", "赵琪君", "王欣欣", "刘丽",   "邱明慧", "吴琴",  "曾子铭", "王洪江", 
            "柳佳慧", "秦珊",  "方方",   "李云",   "黄宏",   "尹建玲", "张鑫灵", "梅芬",  "万金环", "于隐", 
            "李月",   "王青青", "博辛", "程子洁", "藤青", "刘鹤", "朱良云", "丁晓", "张小芳", "灵灵", "于隐",
            "蔡文亮", "章其",  "刘清",   "王玉",   "洪峰",   "许晴",   "凌云",  "刘亦铭", "管陵",   "张勤", 
            "李洪",   "李群",  "陈逸飞", "赵琪君", "王欣欣", "刘丽",   "邱明慧", "吴琴",  "曾子铭", "王洪江", 
            "柳佳慧", "秦珊",  "方方",   "李云",   "黄宏",   "尹建玲", "张鑫灵", "梅芬",  "万金环", "于隐" 
        };
    }
}
