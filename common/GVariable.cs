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

        public const int NUM_OF_FEEDING_MACHINE = 7; //7����ˮ�ߣ�ÿ��һ��Ͷ���豸
        public const int STACK_NUM_ONE_MACHINE = 7; //ÿ��͵���豸��8���ϲ֣�ÿ���ϲֲ�ͬ��ԭ��
        //public static int[] allMachineIDForZihua = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

        //this machine array includes only cast/print/slit machines, normally used in APS process
        public static string[] machineNameArrayAPS = 
                { 
                   "1�����ӻ�", "2�����ӻ�", "3�����ӻ�", "4�����ӻ�", "5�����ӻ�", "6�����Ի�", "7�Ŵ�Ĥ��", 
                   "1��ӡˢ��", "2��ӡˢ��", "3��ӡˢ��", "4��ӡˢ��", "5����ӡ��", 
                   "1�ŷ��л�", "3�ŷ��л�", "5�ŷ��л�", "6�ŷ��л�", "7�ŷ��л�" 
                };

        //these machines will connected with touchscreen or data collect board, so will have device alarm or material alarm, which includes feed/cast/print/slit
        public static string[] machineNameArrayTouchScreen = new string[24];
        //these machines will have its own database, will communicate with touchscreen/label printer
        public static string[] machineNameArrayDatabase;
        /*   { 
             "1�����ӻ�", "2�����ӻ�", "3�����ӻ�", "4�����ӻ�", "5�����ӻ�", "6�����Ի�", "7�Ŵ�Ĥ��", 
             "1��ӡˢ��", "2��ӡˢ��", "3��ӡˢ��", "4��ӡˢ��", "5����ӡ��", 
             "1�ŷ��л�", "3�ŷ��л�", "5�ŷ��л�", "6�ŷ��л�", "7�ŷ��л�",
             "1�Ŵ����", "2�Ŵ����", "3�Ŵ����", "4�Ŵ����",
             "1�Ž�����", "2��ѹƬ��", "3��ѹƬ��", "4��ѹƬ��", "5��ѹƬ��", 
           }; */

        //public static string[] machineNameArrayForSelection; // = new string[machineNameArray.Length + packingMachineNameArray.Length + 1];
        public static string[] machineWorkshopDatabase; // = { "һ����", "������", "������", "������", "������", "һ����", "һ����", "һ����", "һ����", "һ����", "һ����", "һ����", "������", "������", "������", "һ����", "һ����", "һ����" };
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

        public static string programTitle = "�ϻ���ҵ����������Ϣ����ϵͳ";
        public static string enterpriseTitle = "�ϻ���ҵ";

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

        //        public int status; //0��ERP published; 1��APS OK; 2��production started(the first dispatch started); 3��sales order completed; 4��sales order cancelled  

        //production  management

        public const int PHYSICAL_WORKING_HOUR_ONE_SHIFT = 12; //one shift is 12 hours
        public const int EFFECTIVE_WORKING_HOUR_ONE_SHIFT = 10; //one shift is 12 hours, but we need to do something like "��Ĥ��", so real effective working time is about 10 hours

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

        public static string[] taskListCheckDispatch = { "��Ѳ���", "���ͱ�", "�豸����" };
        public static string[] taskListMaintainDispatch = { "�豸ά��������" };
        public static string[] taskListRepairDispatch = { "�豸ά�ޱ�" };

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
        public const string productBatchTableName = "productBatchList";   //product batch -- ��Ʒ���ε����൱�ڰ�һ�������г�С�� 20 �ֵĶ�������
        public const string productBatchFileName = "..\\..\\data\\globalTables\\productBatchList.xlsx";
        public const string globalProductTaskTableName = "productTaskListTable";   //product task -- ���񵥣������ε��ֽ⵽��ͬ���� 
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

        public static string[] salesorderStatus = { "�ѵ���", "�ѷֲ�", "���ų�", "��ȷ��", "�ѷ���", "������", "�ѿ���", "���깤", "����ȡ��", "��׼ȡ��" };

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
        public static string[] strAlarmStatus = { "������", "��ǩ��", "�Ѵ���", "��ȡ��" };

        public static int ALARM_STATUS_ALL_FOR_SELECTION;
        public static string[] strAlarmStatusForSelection = { "������", "��ǩ��", "�Ѵ���", "��ȡ��", "����״̬" };

        public const int ALARM_TYPE_UNDEFINED = -1;  //undefined alarm type means all kinds of alarm need to be considered
        public const int ALARM_TYPE_DEVICE = 0;  //no chart
        public const int ALARM_TYPE_MATERIAL = 1;  //no chart
        //we need to discern the differece between this 2 kinds of alarm, because in SPCanalyze.cs, we need to know whether it is XBar chart or S chart caused this alarm so display XBar/S data in red color accordingly
        public const int ALARM_TYPE_QUALITY_DATA = 2;  //chart type defined in quality table, for Xbar chart/C chart
        public const int ALARM_TYPE_CRAFT_DATA = 3;  //chart_type_no_spc
        public const int ALARM_TYPE_CURRENT_VALUE = 4;  //dispatch should be completed as planned, but fails to complete 
        public const int ALARM_TYPE_ALL_IN_DETAIL = 5;  // all kinds in 
        public const int ALARM_TYPE_TOTAL_NUM = 5;  //total number of alatm types  
        public static string[] strAlarmTypeInDetail = { "�豸���Ʊ���", "���ϰ��Ʊ���", "�������ݱ���", "���ղ�������", "����δ��ɱ���" };

        public const int ALARM_TYPE_DATA = 2;  //including quality/craft data alarm
        public static int ALARM_TYPE_ALL_FOR_SELECTION;
        public static string[] strAlarmTypeForSelection = { "�豸���Ʊ���", "���ϰ��Ʊ���", "�������ݱ���", "�������Ͱ��Ʊ���" };

        public const string SPCMonitoringSystem = "SPC���ϵͳ";

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

        //private int[] startLineNum = new int[12]; //��������ʼ��¼������ 

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

        public static errorCodeListStruct errorCodeList;	//��������
        //public static salesOrderStruct salesOrderImpl = new salesOrderStruct();
        public static BOMListStruct[] BOMList = new BOMListStruct[maxMachineNum];       //ԭ����ȱ�

        public static dispatchSheetStruct[] productTaskSheet = new dispatchSheetStruct[maxMachineNum];  //������
        public static dispatchSheetStruct[] dispatchSheet = new dispatchSheetStruct[maxMachineNum];  //������
        public static machineStatusStruct[] machineStatus = new machineStatusStruct[maxMachineNum];  //�豸״̬��
        public static craftListStruct[] craftList = new craftListStruct[maxMachineNum];	      //���ղ�����
        public static qualityListStruct[] qualityList = new qualityListStruct[maxMachineNum];	    //��������
        public static materialListStruct[] materialList = new materialListStruct[maxMachineNum];       //�������͵��б�

        public static beatPeriodStruct[] beatPeriodInfo = new beatPeriodStruct[maxMachineNum]; //beat setting value for currents
        public static ADCChannelStruct[] ADCChannelInfo = new ADCChannelStruct[maxMachineNum]; // ADC settings
        public static uartSettingStruct[] uartSettingInfo = new uartSettingStruct[maxMachineNum];
        public static GPIOSettingStruct[] GPIOSettingInfo = new GPIOSettingStruct[maxMachineNum];

        //���ϱ����ṹ
        public struct errorCodeListStruct
        {
            public int errorCodeListSize;  //�����������
            public string[] errorCode;
            public string[] errorCodeDesc;
        };

        //������ṹ
        public struct salesOrderStruct
        {
            public string ID;  //ID in sales order table
            public string salesOrderCode;
            public string deliveryTime;  //������
            public string productCode;	 //��Ʒ����
            public string productName;  //��Ʒ����
            public string requiredNum;	//�ƻ���������
            public string unit;  //�����ĵ�λ
            public string customer; //�ͻ���
            public string publisher; //�·���
            public string ERPTime; //ERP �·�ʱ��
            public string APSTime; //APS �ų�ȷ��ʱ��(�����ֹ�����)
            public string planTime1;	//Ԥ�ƿ���ʱ��
            public string planTime2;  //Ԥ���깤ʱ��
            public string realStartTime;	//Ԥ�ƿ���ʱ��
            public string realFinishTime;  //Ԥ���깤ʱ��
            public string source; //0: �ֹ�����; 1: ERP ����
            public string status; //0��ERP published; 1��APS OK; 2��production started(the first dispatch started); 3��sales order completed; 4��sales order   
            public string cutTime; //�ֲ�ʱ��
            public string cutResult; //�ֲ���
        };

        //���ε���ṹ
        public struct productBatchStruct
        {
            public string ID;  //ID in sales order table
            public string salesOrderBatchCode;  //���ζ����ţ��Ѷ����������ε���ԭ������ + �±�
            public string originalSalesOrderCode;  //ԭ���Ķ�����
            public string deliveryTime;  //������
            public string productCode;	 //��Ʒ����
            public string productName;  //��Ʒ����
            public string requiredNum;	//�ƻ���������
            public string unit;  //�����ĵ�λ
            public string customer; //�ͻ���
            public string publisher; //�·���
            public string source; //������Դ -- 0: �ֹ�����; 1: ERP ����
            public string ERPTime; //ERP �·�ʱ��
            public string APSTime; //APS �ų�ȷ��ʱ��(�����ֹ�����)
            public string planTime1;	//Ԥ�ƿ���ʱ��
            public string planTime2;  //Ԥ���깤ʱ��
            public string realStartTime;	//Ԥ�ƿ���ʱ��
            public string realFinishTime;  //Ԥ���깤ʱ��
            public string status; //0��ERP published; 1��APS OK; 2��production started(the first dispatch started); 3��sales order completed; 4��sales order   
            public string cutTime; //�ֲ�ʱ��
            public string batchNum;  //�������κ�
            public string canceller; //���ε�ȡ����
            public string cancelReason;  //ȡ��ԭ��
            public string cancelTime;   //ȡ��ʱ��
            public string approver;   //��׼��
            public string productCode2;
            public string customer2;
            public string productCode3;
            public string customer3;
            public string productCode4;
            public string customer4;
			
        };

        //��Ʒ��ṹ
        public struct productStruct
        {
            public string customer;
            public string productCode;	 //��Ʒ����
            public string productName;  //��Ʒ����
            public string BOMCode;    //�䷽
            public string routeCode; //����·��
            public string productWeight;
            public string productDiameter;
            public string productWidth;
            public string productfixtureNum;
            public string productLength;
            public string baseColor;  //��ɫ
            public string patternType;  //ѹ������
            public string RawMaterialCode;  //�ͻ�ԭ���ϴ���
            public string inkRatio;   //��ī���
            public string numOfMaterialType;  //ԭ������
            public string materialcode1;
            public string materialcode2;
            public string materialcode3;
            public string materialcode4;
            public string materialcode5;
            public string materialcode6;
            public string materialcode7;

            public string rollNumLayer;                 //�ײ����	                   
            public string layerNum;                     //�Ѹ߲���	              
            public string rollNumStack;                 //ÿ���̾���	            
            public string totalWeightStack;             //ÿ�����Ʒ����(kg)	    
            public string stackType;                    //��������	              
            public string stackLength;                  //���峤(mm)	            
            public string stackwidth;                   //�����(mm)	            
            public string recycle;                      //�Ƿ����                
            public string stackLossRate;                //���������1:            
            public string stackNumOneTon;               //�ֲ�Ʒ��������(��)	    
            public string paperCoreDiameter;            //ֽо�ھ�(mm)	          
            public string paperCoreThickness;           //ֽо�ں�(mm)	          
            public string paperCoreLength;              //ֽо����(mm)	          
            public string paperCoreOneStack;            //����ֽо����(m)	        
            public string paperCorreLossRate;           //ֽо�����1:	          
            public string paperCoreOneTon;              //�ֲ�Ʒֽо����(m)	      
            public string paperBoardOneStack;           //����ֽ������(��)	      
            public string paperBoardLossRate;           //ֽ�������1:	          
            public string paperBoardOneton;             //��Ʒֽ������(��) 	      
            public string craftPaperOneStack;           //����ţƤֽ����(m2/����)	
            public string craftPaperLossRate;           //ţƤֽ�����1��         
            public string craftPaperOneTon;             //�ֲ�ƷţƤֽ����(m2)    
            public string corrugatedPaperOneStack;      //��������ֽ����(m2/����) 
            public string corrugatedPaperLossRate;      //����ֽ�����1��         
            public string corrugatedPaperPerTon;        //�ֲ�Ʒ����ֽ����(m2)	  
            public string wrappingFilmOneStack;         //�������Ĥ����(kg/����)	
            public string wrappingFilmOneTon;           //�ֲ�Ʒ����Ĥ����(kg)	  
            public string others;                       //����	                  
            public string packingFilmOneStack;          //�����װ��Ĥ��(kg/����)	
            public string packingFilmOneTon;            //�ֲ�Ʒ��װ��Ĥ����(kg)" 
            public string craftSheetCode1;              //���յ����1
            public string craftSheetCode2;              //���յ����2
            public string craftSheetCode3;              //���յ����3
            public string criteria;                     //�����׼
            public string multiIngredient;              //�Ƿ�����䷽
            public string slitOnline;                   //�Ƿ����߷���
        };

        //�豸����ʱ��� -- for APS
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

        //�豸״̬����ʱ���� -- ���ڼ�¼�豸����/����/ֹͣ�ĳ���ʱ�䣬��������״̬��ͼ����ʾ����
        public struct machineStatusRecordStruct
        {
            public string recordTime;
            public string machineStatus;
            public string statusStart;
            public string keepMinutes;
        };

        //�������񵥱�ṹ������ָһ�����ε���������������Сʱ�ͻ��������ܳ��� 3 �죬Ա������λ�����壬��ʱ�ϳ�
        //������ṹ��������������Сʱ�ͻ�������� 12 ��Сʱ��һ����Σ�ָ�����ˣ���ʱ��� 12 ��Сʱ
        //��ʹ��ͬһ�ṹ��
        public struct dispatchSheetStruct
        {
            public string machineID;   //�豸��� 
            public string dispatchCode;  //dispatch code
            public string planTime1;	//Ԥ�ƿ�ʼʱ��
            public string planTime2;  //Ԥ�ƽ���ʱ��
            public string productCode;	 //��Ʒ����
            public string productName;  //��Ʒ����
            public string operatorName; //����Ա
            public float plannedNumber;	//�ƻ���������
            public float outputNumber;  //��������
            public float qualifiedNumber;  //�ϸ�����
            public float unqualifiedNumber;  //���ϸ�����
            public string processName; //���򣨹���·���а����������
            public string realStartTime;	  //ʵ�ʿ�ʼʱ��
            public string realFinishTime;	  //ʵ���깤ʱ��
            public string prepareTimePoint;   //����ʱ�䣨�Բ�ʱ�䣩���������Ⱦ�������ʱ�䣬Ȼ������ʽ����
            public int status;	  //0��dummy/prepared ����׼����ɵ�δ������1:�����깤���¹���δ������2�������������豸 3������������ 4���Բ�������ʱ�䣩 OK
            public int waitForCheck;  //����Ʒ
            public int wastedOutput;  //����
            public string workTeam;  //���(���ұ�)
            public string batchNum2; //����ʱ,��������Ʒ�����κ�
            public string batchNum3; //����ʱ,���ĸ���Ʒ�����κ�
            public string workshop;	 //�������ƣ���������ˮ�ߵ�����
            public string workshift;	 //��Σ�������ࣩ
            public string salesOrderCode; //������
            public string BOMCode; // 
            public string customer;
            public string multiProduct; //�������
            public string productCode2; //�����еĵڶ��ֲ�Ʒ
            public string productCode3; //�����еĵ����ֲ�Ʒ
            public string operatorName2; //����Ա2
            public string operatorName3; //����Ա3
            public string batchNum; //���κ�
            public string batchNum1; //����ʱ,�ڶ�����Ʒ�����κ�
            public string rawMateialcode;  //ԭ���ϴ��루�ͻ��Ĳ�Ʒ���룬���ڿͻ������ϻ��Ĳ�Ʒ���ǿͻ���ԭ�ϣ�
            public string productLength; //�� 
            public string productDiameter;  //��
            public string productWeight;  //����
            public string slitWidth;  //���п�� 
            public string salesOrderBatchCode;  //���ζ�����
            public string productCode4; //�����еĵ������ֲ�Ʒ
            public string operatorName4; //����Ա4
            public string notes;   //���Ӱ��¼
            public string comments;  //�쵼��ʾ
        };

        //�豸״̬��ṹ
        public struct machineStatusStruct
        {
            public string recordTime;  //��¼ʱ��
            public string machineID;  //�豸���
            public string machineCode;  //�豸����
            public string machineName;  //�豸��
            public string dispatchCode;  //��ǰ������
            public string startTime;	  //����ʱ��
            public string offTime;	  //�ػ�ʱ��
            public int totalWorkingTime;  //��ҵ��ʱ��
            public float collectedNumber;  //�豸�ɼ�����
            public int productBeat;  //��������
            public int workingTime;  //����ʱ��
            public int prepareTime;  //����ʱ�����ʱ��
            public int standbyTime;  //����ʱ��
            public int power; //����
            public int powerConsumed;  //�ܺ�
            public int pressure;  //ѹ��
            public int revolution;  //ת��
            public int maintenancePeriod;   //��������
            public int lastMaintenance;   //���һ�α���
            public int waitForCheck;  //����Ʒ
            public int wastedOutput;  //����
        };


        //���ղ�����ṹ
        public struct craftListStruct
        {
            public int paramNumber;  //���ղ�������
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

        //��������ṹ
        public struct qualityListStruct
        {
            public int checkItemNumber;  //��������������
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

        //�豸���Ʊ�����
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

        //ԭ�ϱ� -- ����ԭ���б�
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

        //�������ϱ� -- ������ĳһ������
        public struct materialListStruct
        {
            public string salesOrderCode;
            public string dispatchCode;
            public string machineID;
            public string machineCode;
            public string machineName;
            //public string status;  //like 
            public int numberOfTypes;  //number of kinds of material
            //public string[] materialName;  //�������ƣ�����������λ��
            public string[] materialCode;  //���ϱ���
            public int[] materialRequired;  //������������
            public int[] previousLeft;  //��һ�����������
            public int[] currentlyUsed;  //������������
            public int[] currentLeft;  //�ϲ��е�����
            public string salesOrderBatchCode;
            public string batchNum;
            public string materialStatus;  //"0000000": ��ȱ��; "1000000":1���ϲ�ȱ��; "1111111": 7���ϲ�ȱ�� 
        };

        //���������, ���ϰ��˹�ÿ�찴��Ӳֿ�����
        public struct materialRequirementStruct
        {
            public string materialRequiresheetCode;
            public string salesOrderCode;
            public string salesOrderBatchCode;
            public string batchNum;
            public string status;  //0:generated; 1:started; 2:completed 
            public string machineID;
            public int numberOfTypes;  //number of kinds of material
            public string[] materialCode;  //���ϱ���
            public int[] materialRequired;  //������������
        };

        //ԭ����ȱ� -- �������ݵ�һ���֣��빤�����豸�޹�
        public struct BOMListStruct
        {
            public string BOMCode;
            public int numberOfTypes;  //number of kinds of material
            public string[] materialCode;  //���ϱ���
            public string[] materialName;  //�������ƣ�����������λ��
            public double[] materialQuantity;  //��������
        };

        //Ա���б�  -- �������ݵ�һ����
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

        
        public const string exitStr = "�˳�";
        public const string dispatchUIListTitle0 = "���";
        public const string dispatchUIListTitle1 = "��ɫ";
        public const string dispatchUIListTitle2 = "������Ŀ";
        public const string dispatchUIListTitle3 = "����Ҫ��";
        public const string dispatchUIListTitle4 = "��������";
        public const string dispatchUIListTitle5 = "��������";
        public const string dispatchUIListTitle6 = "��������";
        public const string dispatchUIListTitle7 = "��������";
        public const string dispatchUIListTitle8 = "��������";
        public const string dispatchUIListTitle9 = "������";
        public const string dispatchUIListTitle10 = "����ж�";

        public const string deviceAlarmScreenTitle = "�豸�����б�";
        public const string deviceAlarmListTitle0 = "���";
        public const string deviceAlarmListTitle1 = "���Ʊ��";
        public const string deviceAlarmListTitle2 = "�豸����";
        public const string deviceAlarmListTitle3 = "�ϲֱ��";
        public const string deviceAlarmListTitle4 = "��������";
        public const string deviceAlarmListTitle5 = "��������";
        public const string deviceAlarmListTitle6 = "������";
        public const string deviceAlarmListTitle7 = "����ʱ��";
        public const string deviceAlarmListTitle8 = "����״̬";
        public const string deviceAlarmListTitle9 = "ǩ����";
        public const string deviceAlarmListTitle10 = "������";
        public const string deviceAlarmListTitle11 = "ǩ��ʱ��";
        public const string deviceAlarmListTitle12 = "�������";
        public const string deviceAlarmListTitle13 = "�����ʩ";
        public const string deviceAlarmListTitle14 = "����/ȡ��ʱ��";

        public const string materialAlarmScreenTitle = "���ϰ����б�";
        public const string materialAlarmListTitle0 = "��ѡ";
        public const string materialAlarmListTitle1 = "���";
        public const string materialAlarmListTitle2 = "�ɹ�����";
        public const string materialAlarmListTitle3 = "�������κ�";
        public const string materialAlarmListTitle4 = "�豸����(���յ�)";

        public const string deviceAlarmListItem1 = "ǩ��";
        public const string deviceAlarmListItem2 = "���ȷ��";
        public const string deviceAlarmListItem3 = "ȡ������";

        public static string[] SPCAnalyzeDataTitle = { "��������", "CPK����", "����ͼ����" };

        public static string[,] SPCAnalyzeDataStr = new string[3, 8]
        {
            {"��������", "�������", "�������", "�������", "���ֵ", "��Сֵ", "ƽ��ֵ", ""},
            {"Cp", "Cpk", "Cpl", "Cpu", "Pp", "Ppk", "Ppl", "Ppu"},
            {"XBar��������", "XBar������", "XBar��������", "Sͼ��������", "Sͼ������", "Sͼ��������", "", ""},
        };

        public static string[] alarmListTitle = { "�豸���Ʊ�����¼�б�", "���ϰ��Ʊ�����¼�б�", "�������ݱ�����¼�б�", "�������ݱ�����¼�б�", "���ղ���������¼�б�", "����δ��ɱ�����¼�б�", "���а��Ʊ�����¼�б�" };
        public static string[] alarmListHistoryTitle = { "�豸���Ʊ��������б�", "���ϰ��Ʊ��������б�", "�������ݱ��������б�", "���ղ������������б�", "����δ��ɱ��������б�", "���а��Ʊ��������б�" };

        public const string strOfUnkown = "δ֪";
        public const string serverPCUserName = "������ʹ����";

        public const string strExitOrNot = "ȷ��Ҫ�˳��������ݲɼ�ϵͳ��?";
        public const string strConfirm = "ȷ��";

        public static string[] productStatusList = { "�ϸ�Ʒ", "���ϸ�Ʒ", "������Ʒ", "����", "�߽���", "����ϸ�", "�����ϸ�" };

        public static string[] deviceErrDescList = { "A-����׶�", "B-�񱡱���", "C-����", "D-�����λ(ë��)", "DC-������", "E-����", "W-�ó�" };
        
        public static string[] inspectionErrorList =
        {
            "��񲻷�", "�������", "��������", "���س���", "��Ȳ���", "����ֵ�����", "�׶�", "����", "����", "���ʡ����������", "ѹ�Ʋ���", "����", 
            "��ɫ̫��", "��ɫ̫ǳ", "����ɫ��", "�ó�", "����", "ˮӡ", "��ī", "��˿����ī", "©ӡ", "¶��", "ͼ��ȱʧ", "ͼ������", "��ī�ζȲ���", 
            "ӡˢ��ɫ������׼", "ӡˢɫ�಻��", "ӡˢ��ͷ�������", "ӡˢ���վ������", "�������վ������", "ѹ���վ������", "�վ����", "�վ����", 
            "����", "�񱡲���", "�����λ", "�б߶���", "���ڽ���������", "�������ܲ����", "PH����", "WVTR����", "Ħ��ϵ������", "��ˮѹ����",
        };


        public static string[] castErrorList =
        {
            "�����¶��쳣", "�����쳣", "�ݸ��޶���", "ˮ��ͷ©ˮ", "������ͣ", "ҡ���޶���", "��л�", "�վ��е��޶���", "��ˮ����©ˮ", "��ˮ���鲻����", 
            "�䶳����ͣ", "ѭ��ˮ�û�", "���β�����", "����©��", "��·��ˮ", "ģͷˮ�俪��", "����",
        };

        public static string[] printErrorList =
        {
            "�����¶ȼӲ���", "��л�", "�����޶���", "ǰ���ٶȲ�һ��", "��·©��", "����",
        };

        public static string[] slitErrorList =
        {
            "�վ�Ƥ����", "�վ���©��", "�վ��кڵ�", "�趨���ݲ�׼", "����",
        };

        public static string[] packingErrorList =
        {
            "û������", "ƽ̨����ת", "�趨Ȧ����׼", "����",
        };

        public static string[] rebuildErrorList =
        {
            "����������", "����̫��", "����������", "Ƥ����", "����",
        };

        public static string[] commonErrorList =
        {
            "��ѹ��ͣ��", "ѹ��������ˮ", "ѭ���û�", "�ؿӱû�", "����", "����",
        };


        public static string[] errSPCDescList = { "���ݳ���������", "��������9����λ��������ͬһ��", "7�����������������½�", "15�������������߸���", "5��������������4�����������߽�Զ" };
        public const string errCraftDesc = "���ݳ��������";

        public static string dispatchPartBase = "�ͳ���";

        public static string[] dispatchOperatorNameBase = 
        {
            "������", 
            "������", "����",  "����",   "����",   "���",   "����",   "����",  "������", "����",   "����", 
            "���",   "��Ⱥ",  "���ݷ�", "������", "������", "����",   "������", "����",  "������", "���齭", 
            "���ѻ�", "��ɺ",  "����",   "����",   "�ƺ�",   "������", "������", "÷��",  "���", "����", 
            "����",   "������", "����", "���ӽ�", "����", "����", "������", "����", "��С��", "����", "����",
            "������", "����",  "����",   "����",   "���",   "����",   "����",  "������", "����",   "����", 
            "���",   "��Ⱥ",  "���ݷ�", "������", "������", "����",   "������", "����",  "������", "���齭", 
            "���ѻ�", "��ɺ",  "����",   "����",   "�ƺ�",   "������", "������", "÷��",  "���", "����", 
            "������", "����",  "����",   "����",   "���",   "����",   "����",  "������", "����",   "����", 
            "���",   "��Ⱥ",  "���ݷ�", "������", "������", "����",   "������", "����",  "������", "���齭", 
            "���ѻ�", "��ɺ",  "����",   "����",   "�ƺ�",   "������", "������", "÷��",  "���", "����", 
            "����",   "������", "����", "���ӽ�", "����", "����", "������", "����", "��С��", "����", "����",
            "������", "����",  "����",   "����",   "���",   "����",   "����",  "������", "����",   "����", 
            "���",   "��Ⱥ",  "���ݷ�", "������", "������", "����",   "������", "����",  "������", "���齭", 
            "���ѻ�", "��ɺ",  "����",   "����",   "�ƺ�",   "������", "������", "÷��",  "���", "����" 
        };
    }
}
