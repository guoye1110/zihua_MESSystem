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
        public static int buildNewDatabase = 1;

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

        //board ID larger than that is an ID of an App that want a redo of handshake, but should not initialize parameters 
        //board ID less than that is an ID of normal data collect board, which has the function of dispatch apply/start/complete, and also craft/quality related functions
        public const int ID_APPS_NOT_INIT_DATA = 0x100000;

        //board ID larger than that is an ID of an App that want to do a handshake, which means a start of communication
        public const int ID_OTHERTHAN_TOUCHPAD = 0x1000000;

        public static int startingSerialNumber;  //this is an 10 byte number, like 1705310001, 1705310115, indicating the serial number for the current dispatch
        public static int nextSerialNumber;  //this is an 10 byte number, like 1705310001, 1705310115, indicating the serial number for next dispatch
        //public static string[] processName;

        //definition for Zihua worikng process *****************************
        public const int LINE_NUM_ZIHUA = 7;  //the major product line is cast line, there are 7 cast lines
        public static int MACHINE_NAME_ALL_FOR_SELECTION;
        public static int[] allMachineIDForZihua = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
        //we will get this value from database
        public static string[] machineNameArray; // = { "1号流延机", "2号流延机", "3号流延机", "4号流延机", "5号流延机", "6号中试机", "7号吹膜机", "1号印刷机", "2号印刷机", "3号印刷机", "4号印刷机", "5号柔印机", "1号分切机", "3号分切机", "5号分切机", "6号分切机", "7号分切机" };
        //public static string[] packingMachineNameArray = { "1号打包机", "2号打包机", "3号打包机", "4号打包机", "5号打包机" };
        //public static string[] machineNameArrayPackingIncluded; // = new string[machineNameArray.Length + packingMachineNameArray.Length];  
        public static string[] machineNameArrayForSelection; // = new string[machineNameArray.Length + packingMachineNameArray.Length + 1];
        public static string[] allMachineWorkshopForZihua; // = { "一车间", "二车间", "二车间", "二车间", "二车间", "一车间", "一车间", "一车间", "一车间", "一车间", "一车间", "一车间", "二车间", "二车间", "二车间", "一车间", "一车间", "一车间" };

        //not used any more
        public static string[] machineCodeArray;

        //for these machines below, we will generate serial number for every piece of product
        //public static int[] feedingProcess = { 1, 2, 3, 4, 5 };
        public static int[] castingProcess = { 1, 2, 3, 4, 5};
        public static int[] printingProcess = { 6, 7, 8, 9, 10 };
        public static int[] slittingProcess = { 11, 12, 13, 14, 15};

        // salesorder: "S171109"
        public const int salesOrderLength = 7;

        // batchNum: "1801306"
        public const int batchNumLength = 7;

        // dispatchCode:"S17110906L302"
        public const int dispatchCodeLength = 13;

        // salesorder: "S171109", batchNum: "1801306", dispatchCode:"S17110906L302", cast process:"L", machine ID:"3", time:"1801201431", large roll ID:"05"  
        //"S17110906L302L3180120143105";
        public const int castBarcodeLength = 27;

        //salesorder: "S171109", batchNum: "1801306", dispatchCode:"S17110906L302", print process:"P", machine ID:"2", time:"1801201431", large roll ID:"05"  
        //"S17110906L302P2180120143105";
        public const int printBarcodeLength = 27;

        //salesorder: "S171109", batchNum: "1801306", dispatchCode:"S17110906L302", slit process:"S", machine ID:"1", time:"1801201431", large roll ID:"05", small roll ID:"001", customer:"0", inspection:"0"  
        //"S17110906L302S118012014310500100";
        public const int slitBarcodeLength = 32;

        //salesorder: "S171109", batchNum: "1801306", dispatchCode:"S17110906L302", inspection process:"I", machine ID:"0", time:"1801201431", large roll ID:"05", small roll ID:"001", customer:"0", inspection:"3"  
        //"S17110906L302I018012014310500103";
        public const int inspectionBarcodeLength = 32;

        //salesorder: "S171109", batchNum: "1801306", dispatchCode:"S17110906L302", reuse process:"R", machine ID:"1", time:"1801201431", large roll ID:"05", small roll ID:"001", customer:"0", inspection:"0"  
        //"S17110906L302R118012014310500100";
        public const int reuseBarcodeLength = 32;
 
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
        public const int SALES_ORDER_STATUS_ERP_PUBLISHED = 0;
        public const int SALES_ORDER_STATUS_APS_OK = 1;
        public const int SALES_ORDER_STATUS_CONFIRMED = 2;
        public const int SALES_ORDER_STATUS_APPLIED = 3;
        public const int SALES_ORDER_STATUS_STARTED = 4;
        public const int SALES_ORDER_STATUS_COMPLETED = 5;
        public const int SALES_ORDER_STATUS_CANCELLED = 6;

        public const int MATERIAL_SHEET_STATUS_GENERATED = 0;
        public const int MATERIAL_SHEET_STATUS_APPLIED = 2;
        public const int MATERIAL_SHEET_STATUS_CONFIRMED = 3;
        public const int MATERIAL_SHEET_STATUS_ERROR = 4;
        public const int MATERIAL_SHEET_STATUS_SHORTAGE = 5;
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

        public static int maxMachineNum = 33; //machineNameArray.Length;

        public const int maxCurveNum = 36;  //we now have altogether 36 kind of data that need to be draw by curve

        public const int fileDataLength = 1024 * 1024 * 8;

        public const int maxErrorCodeNum = 40;  //an error code list may have a lot of error status
        public const int maxCraftParamNum = 8;  //there could be 10 kinds of craft parameter at most, like voltage, cuttent, spin speed
        public const int maxQualityDataNum = 16;  //there could be 10 kinds of quality data at most, like length, temparature, height
        public const int maxMaterialTypeNum = 8; //there could be 8 kinds of material for a dispatch 
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
        public const string materialListTableName = "0_materialList";
        public const string materialListFileName = "..\\..\\data\\machine\\materialList.xlsx";
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
        public const string workProcedureTableName = "procedureList";
        public const string workProcedureFileName = "..\\..\\data\\basicData\\procedureList.xlsx";
        public const string bomTableName = "bomList";
        public const string bomFileName = "..\\..\\data\\basicData\\bom.xlsx";
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

        //global database
        public const string globalAlarmListTableName = "AlarmList";
        public const string globalAlarmListFileName = "..\\..\\data\\machine\\alarmList.xlsx";
        public const string salesOrderTableName = "salesOrderList";
        public const string salesOrderFileName = "..\\..\\data\\globalTables\\salesOrderList.xlsx";
        public const string globalDispatchTableName = "dispatchList";
        public const string globalDispatchFileName = "..\\..\\data\\machine\\dispatchList.xlsx";  //make sure dispatch in different places have the same format
        public const string globalMaterialTableName = "materialList";
        public const string globalMaterialFileName = "..\\..\\data\\machine\\materialList.xlsx";
        public const string materialInventoryTableName = "materialInventory";
        public const string materialInventoryFileName = "..\\..\\data\\globalTables\\materialInventory.xlsx";
        public const string materialInOutRecordTableName = "materialInOutRecord";
        public const string materialInOutRecordFileName = "..\\..\\data\\globalTables\\materialInOutRecord.xlsx";
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
        public const string dispatchCurrentIndexTableName = "dispatchCurrentIndex";
        public const string dispatchCurrentIndexFileName = "..\\..\\data\\globalTables\\dispatchCurrentIndex.xlsx";

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
        public const int ALARM_STATUS_APPLIED = 0;
        public const int ALARM_STATUS_SIGNED = 1;
        public const int ALARM_STATUS_COMPLETED = 2;
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
        public static string[] strAlarmTypeInDetail = { "设备安灯报警", "物料安灯报警", "质量数据报警", "工艺参数报警", "工单未完成报警"};

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
        public const int totalPointNumForSChart = pointNumInSChartGroup * numOfGroupsInSChart;  

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
        public static string [] clientPCConnectionStatusArray = new string[maxClientPC];
        public static string [] clientPCAccountListArray = new string[maxClientPC];
        //end of host server PC and client PC

//        public static Semaphore[] semaphoreForAlarmPush;
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
        public static string [] scannedData = new string[maxMachineNum + 1];

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

        public static string [] today = new string[maxMachineNum + 1];
        public static string [] today_old = new string[maxMachineNum + 1];

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

        public static string text1,  text2,  text3,  text4,  text5,  text6,  text7,  text8,  text9,  text10, text11, text12, text13, text14, text15;
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

        public static dispatchSheetStruct[] dispatchSheet = new dispatchSheetStruct[maxMachineNum];  //工单表
        public static machineStatusStruct [] machineStatus = new machineStatusStruct[maxMachineNum];  //设备状态表
        public static craftListStruct [] craftList = new craftListStruct[maxMachineNum];	      //工艺参数表
        public static qualityListStruct[] qualityList = new qualityListStruct[maxMachineNum];	    //质量检测表
        public static materialListStruct[] materialList = new materialListStruct[maxMachineNum];       //物料配送单列表

        public static beatPeriodStruct [] beatPeriodInfo = new beatPeriodStruct[maxMachineNum]; //beat setting value for currents
        public static ADCChannelStruct [] ADCChannelInfo = new ADCChannelStruct[maxMachineNum]; // ADC settings
        public static uartSettingStruct [] uartSettingInfo = new uartSettingStruct[maxMachineNum];
        public static GPIOSettingStruct [] GPIOSettingInfo = new GPIOSettingStruct[maxMachineNum];

        //故障编码表结构
        public struct errorCodeListStruct
        {
            public int errorCodeListSize;  //错误编码数量
            public string[] errorCode;
            public string[] errorCodeDesc;
        }

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
        }

        //产品表结构
        public struct productStruct
        {
            public string customer;
            public string productCode;	 //产品编码
            public string productName;  //产品名称
            public string BOMCode;
            public string routeCode; //工艺路线
            public string productWeight;
            public string productThickness;
            public string baseColor;
            public string patternType;
            public string steelRollType;
            public string rubberRollType;
            public string castSpec;
            public string printSpec;
            public string slitSpec;
            public string castCraft;
            public string printCraft;
            public string slitCraft;
            public string castQuality;
            public string printQuality;
            public string slitQuality;
        }

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
        }

        //设备状态持续时间表表 -- 用于记录设备运行/待机/停止的持续时间，用于生产状态的图形显示介面
        public struct machineStatusRecordStruct
        {
            public string recordTime;
            public string machineStatus;
            public string statusStart;
            public string keepMinutes;
        }

        //工单表结构
        public struct dispatchSheetStruct
        {
            public string machineID;   //设备序号 
            public string dispatchCode;  //dispatch code
            public string planTime1;	//预计开始时间
            public string planTime2;  //预计结束时间
            public string productCode;	 //产品编码
            public string productName;  //产品名称
            public string operatorName; //操作员
            public int plannedNumber;	//计划生产数量
            public int outputNumber;  //接收数量
            public int qualifiedNumber;  //合格数量
            public int unqualifiedNumber;  //不合格数量
            public string processName; //工序（工艺路线中包含多个工序）
            public string realStartTime;	  //实际开始时间
            public string realFinishTime;	  //实际完工时间
            public string prepareTimePoint;   //调整时间（试产时间），开工后先经过调整时间，然后再正式生产
            public int status;	  //0：dummy/prepared 工单准备完成但未发布，1:工单完工，新工单未发布，2：工单发布至设备 3：工单已启动 4：试产（调整时间） OK
            public int toolLifeTimes;  //刀具寿命次数
            public int toolUsedTimes;  //刀具使用次数
            public int outputRatio;  //单件系数
            public string serialNumber; //流水号
            public string reportor; //报工员工号, 报工员和操作员可能不是同一个人
            public string workshop;	 //车间名称，或者是流水线的名字
            public string workshift;	 //班次（早中晚班）
            public string salesOrderCode; //订单号
            public string BOMCode; // 
            public string customer;
            public string barCode;
            public string barcodeForReuse;
            public string quantityOfReused;
            public string multiProduct;
            public string productCode2;
            public string productCode3;
            public string operatorName2; //操作员
            public string operatorName3; //操作员
            public string batchNum; //批次号，previously used inside Zihua, now we don't need this in new system, but need it to be compatible with the old system
        }

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
            public int collectedNumber;  //设备采集件数
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
            public int toolLifeTimes;  //刀具寿命次数
            public int toolUsedTimes;  //刀具使用次数
        }


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
        }

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
        }

        //设备安灯报警表
        public struct alarmTableStruct
        {
            public string errorDesc;  //error description
            public string dispatchCode;
            public string alarmFailureCode;  //alarm sheet code
            public string machineCode;
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
        }

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
        }

        //生产物料表 -- 附属于某一个工单
        public struct materialListStruct
        {
            public string salesOrderCode;
            public string dispatchCode;  //
            public string machineID;
            public string machineCode;
            public string machineName;
            public string status;
            public int numberOfTypes;  //number of kinds of material
            public string[] materialName;  //物料名称（包括计量单位）
            public string[] materialCode;  //物料编码
            public int[] materialRequired;  //物料需求数量
            public int[] fullPackNum;  //一个码垛可存放的袋数
        }

        //原料配比表 -- 基础数据的一部分，与工单及设备无关
        public struct BOMListStruct
        {
            public string BOMCode;
            public int numberOfTypes;  //number of kinds of material
            public string[] materialCode;  //物料编码
            public string[] materialName;  //物料名称（包括计量单位）
            public int[] materialQuantity;  //物料数量
        }

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
        }

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
        }
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
        }

        public struct castQualityStruct
        {
            public string castQualityCode;
            public string castQualityReelWeight;
            public string castQualityReelThickness;
            public string castQualityReelCorona;
            public string castQualityReelWidth;
            public string castQualityReelDiameter;
            public string castQualityReelLength;
        }

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
        }

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
        }

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
        }

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
        }

        public struct beatPeriodStruct
        {
            public int deviceSelection;
            public float idleCurrentHigh;
            public float idleCurrentLow;
            public float workCurrentHigh;
            public float workCurrentLow;
            public int gapValue;
            public int peakValue;
        }

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
        }

        public struct uartSettingStruct
        {
            public int selectedUart;
            public int[] uartBaudrate;
            public string[] uartInputData;
            public string[] uartOutputData;
        }

        public struct GPIOSettingStruct
        {
//            public int[] GPIOPINEnabled;
            public int[] GPIOTriggerVoltage;
            public string[] GPIOName;
        }
    }

    public class gConstText
    {
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
        public const string deviceAlarmListTitle2 = "设备编码";
        public const string deviceAlarmListTitle3 = "设备名称";
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

        public static string[] deviceErrNoList = { "502010", "502020", "502030", "502040", "502050", "502060", "502070", "502080", "502090", "502100", "502110", "502120", "502130", "502140", "502150", "502160", "502170", "502180" };
        public static string[] deviceErrDescList = { "电气故障-数控系统", "电气故障-PLC控制", "电气故障-电柜元件", "电气故障-传感器", "电气故障-电机", "机械故障-上下料装置", "机械故障-工装夹具", "机械故障-刀辅具", "机械故障-主轴", "机械故障-进给轴", "机械故障-设备本体", "流体故障-液压", "流体故障-润滑", "流体故障-冷却", "流体故障-气动", "操作故障-编程", "操作故障-操作", "操作故障-零件定位夹紧" };

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
