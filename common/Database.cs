using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Web;
using System.Web.Services;
using System.Threading;

namespace MESSystem.common
{
    public partial class mySQLClass
    {
        const int DATA_TYPE_INT = 0;
        const int DATA_TYPE_FLOAT = 1;
        const int DATA_TYPE_DOUBLE = 2;
        const int DATA_TYPE_STRING = 3;

        const int LEN_1 = 1;
        const int LEN_4 = 4;
        const int LEN_8 = 8;
        const int LEN_40 = 40;
        const int LEN_100 = 100;
        const int LEN_200 = 200;
        const int LEN_400 = 400;

        static object numOfAlarmForTodayLocker = new object();
        static object readSmallLocker = new object();
        static object readAllLocker = new object();
        static object writeOneFloatLocker = new object();
        static object writeFloatLocker = new object();
        static object writeIntLocker = new object();

        public static string connectionString;

        public static Mutex newReadDataMutex = new Mutex();

        //index in quality data table, data table has 4 columns, ID/wokerIndex/time/data .../sn/status ...
        const int ID_VALUE_IN_DATABASE = 0;
        const int TIME_VALUE_IN_DATABASE = 1;
        const int DATA_VALUE_IN_DATABASE = 2;
        const int DATA_STATUS_IN_DATABASE = 19;

        //index in device alarm table
        public const int ID_VALUE_IN_ALARM_DATABASE = 0;
        public const int ERROR_DESC_IN_ALARM_DATABASE = 1;
        public const int DISPATCH_CODE_IN_ALARM_DATABASE = 2;
        public const int ALARM_FAIL_NO_IN_ALARM_DATABASE = 3;
        //public const int MACHINE_CODE_IN_ALARM_DATABASE = 4;
        public const int FEED_BIN_ID_IN_ALARM_DATABASE = 4;
        public const int MACHINE_NAME_IN_ALARM_DATABASE = 5;
        public const int OPERATOR_NAME_IN_ALARM_DATABASE = 6;
        public const int TIME_IN_ALARM_DATABASE = 7;
        public const int SIGNER_IN_ALARM_DATABASE = 8;
        public const int TIME1_IN_ALARM_DATABASE = 9;
        public const int COMPLETER_IN_ALARM_DATABASE = 10;
        public const int TIME2_IN_ALARM_DATABASE = 11;
        public const int TYPE_IN_ALARM_DATABASE = 12;
        public const int CATEGORY_IN_ALARM_DATABASE = 13;
        public const int STATUS_IN_ALARM_DATABASE = 14;
        //in history means an alarm is put in experience list
        public const int INHISTORY_IN_ALARM_DATABASE = 15;   
        //start ID means when we doing a SPC checking, we may need 225 point data to check for break of rules, if alarm occurs, we need to record the start ID of our checking, so we can redraw this chart easily
        public const int STARTID_IN_ALARM_DATABASE = 16;
        public const int INDEX_TABLE_IN_ALARM_DATABASE = 17;
        public const int WORKSHOP_IN_ALARM_DATABASE = 18;
        public const int MAILLIST_IN_ALARM_DATABASE = 19;
        public const int DISCUSS_IN_ALARM_DATABASE = 20;
        public const int SOLUTION_IN_ALARM_DATABASE = 21;

        //index in sales order table
        public const int ID_VALUE_IN_SALESORDER_DATABASE = 0;
        public const int ORDER_CODE_IN_SALESORDER_DATABASE = 1;
        public const int DELIVERY_TIME_IN_SALESORDER_DATABASE = 2;
        public const int PRODUCT_CODE_IN_SALESORDER_DATABASE = 3;
        public const int PRODUCT_NAME_IN_SALESORDER_DATABASE = 4;
        public const int REQUIRED_NUM_IN_SALESORDER_DATABASE = 5;
        public const int UNIT_IN_SALESORDER_DATABASE = 6;
        public const int CUSTOMER_IN_SALESORDER_DATABASE = 7;
        public const int PUBLISHER_IN_SALESORDER_DATABASE = 8;
        public const int SOURCE_IN_SALESORDER_DATABASE = 9;
        public const int ERP_TIME_IN_SALESORDER_DATABASE = 10;
        public const int APS_TIME_IN_SALESORDER_DATABASE = 11;
        public const int PLANNED_START_TIME_IN_SALESORDER_DATABASE = 12;
        public const int PLANNED_COMPLETE_TIME_IN_SALESORDER_DATABASE = 13;
        public const int REAL_START_TIME_IN_SALESORDER_DATABASE = 14;
        public const int REAL_COMPLETE_TIME_IN_SALESORDER_DATABASE = 15;
        public const int STATUS_IN_SALESORDER_DATABASE = 16;
        public const int RESULT_IN_SALESORDER_DATABASE = 17;

        //index in machine list table
        const int INTERNAL_CODE_IN_LIST = 1;
        const int MACHINE_CODE_IN_LIST = 2;
        const int MACHINE_NAME_IN_LIST = 3;
        const int MACHINE_TYPE_IN_LIST = 4;
        const int MACHINE_VENDOR_IN_LIST = 5;
        const int PURCHASE_DATE_IN_LIST = 6;
        const int PREPARE_TIME_IN_LIST = 7;
        const int WORKING_TIME_IN_LIST = 8;
        const int PREFER_VALUE_IN_LIST = 9;
        const int MAINTAIN_PERIOD_IN_LIST = 10;
        const int MAINTAIN_HOUR_IN_LIST = 11;

        //index in product list table
        public const int PRODUCT_ITEM_CUSTOMER = 1;
        public const int PRODUCT_ITEM_PRODUCT_CODE = 2;
        public const int PRODUCT_ITEM_PRODUCT_NAME = 3;
        public const int PRODUCT_ITEM_BOM = 4;
        public const int PRODUCT_ITEM_ROUTE_CODE = 5;
        public const int PRODUCT_ITEM_PRODUCT_WEIGHT = 6;
        public const int PRODUCT_ITEM_PRODUCT_THICKNESS = 7;
        public const int PRODUCT_ITEM_BASE_COLOR = 8;
        public const int PRODUCT_ITEM_PATTERN_TYPE = 9;
        public const int PRODUCT_ITEM_STEEL_ROLL = 10;
        public const int PRODUCT_ITEM_RUBBER_ROLL = 11;
        public const int PRODUCT_ITEM_CAST_SPEC = 12;
        public const int PRODUCT_ITEM_PRINT_SPEC = 13;
        public const int PRODUCT_ITEM_SLIT_SPEC = 14;
        public const int PRODUCT_ITEM_CAST_CRAFT = 15;
        public const int PRODUCT_ITEM_PRINT_CRAFT = 16;
        public const int PRODUCT_ITEM_SLIT_CRAFT = 17;
        public const int PRODUCT_ITEM_CAST_QUALITY = 18;
        public const int PRODUCT_ITEM_PRINT_QUALITY = 19;
        public const int PRODUCT_ITEM_SLIT_QUALITY = 20;


        //index in material table, which listed all kinds of materials for this factory
        public const int MATERIAL_TABLE_ID = 0;
        public const int MATERIAL_TABLE_CODE = 1;
        public const int MATERIAL_TABLE_NAME = 2;
        public const int MATERIAL_TABLE_STAGE = 3;
        public const int MATERIAL_TABLE_TYPE = 4;
        public const int MATERIAL_TABLE_MINPACKET = 5;
        public const int MATERIAL_TABLE_UNIT = 6;
        public const int MATERIAL_TABLE_PACKET = 7;
        public const int MATERIAL_TABLE_VENDOR = 8;
        public const int MATERIAL_TABLE_FULL_PACK_NUM = 9;

        //index in BOM list table
        public static int MAX_MATERIAL_TYPE_IN_BOM = gVariable.maxMaterialTypeNum;

        public const int BOM_ITEM_ID = 0;
        public const int BOM_ITEM_BOM_CODE = 1;
        public const int BOM_ITEM_TYPE_NUMBER = 2;
        public const int BOM_ITEM_MATERIAL_CODE1 = 3;
        public const int BOM_ITEM_MATERIAL_NAME1 = 4;
        public const int BOM_ITEM_QUANTITY1 = 5;
        public const int BOM_ITEM_MATERIAL_CODE2 = 6;
        public const int BOM_ITEM_MATERIAL_NAME2 = 7;
        public const int BOM_ITEM_QUANTITY2 = 8;
        public const int BOM_ITEM_MATERIAL_CODE3 = 9;
        public const int BOM_ITEM_MATERIAL_NAME3 = 10;
        public const int BOM_ITEM_QUANTITY3 = 11;
        public const int BOM_ITEM_MATERIAL_CODE4 = 12;
        public const int BOM_ITEM_MATERIAL_NAME4 = 13;
        public const int BOM_ITEM_QUANTITY4 = 14;
        public const int BOM_ITEM_MATERIAL_CODE5 = 15;
        public const int BOM_ITEM_MATERIAL_NAME5 = 16;
        public const int BOM_ITEM_QUANTITY5 = 17;
        public const int BOM_ITEM_MATERIAL_CODE6 = 18;
        public const int BOM_ITEM_MATERIAL_NAME6 = 19;
        public const int BOM_ITEM_QUANTITY6 = 20;
        public const int BOM_ITEM_MATERIAL_CODE7 = 21;
        public const int BOM_ITEM_MATERIAL_NAME7 = 22;
        public const int BOM_ITEM_QUANTITY7 = 23;
        public const int BOM_ITEM_MATERIAL_CODE8 = 24;
        public const int BOM_ITEM_MATERIAL_NAME8 = 25;
        public const int BOM_ITEM_QUANTITY8 = 26;

        //index in cast craft
        public const int CAST_CRAFT_CODE = 1; 
        public const int CAST_CRAFT_C1 = 2;
        public const int CAST_CRAFT_C2 = 3;
        public const int CAST_CRAFT_C3 = 4;
        public const int CAST_CRAFT_C4 = 5;
        public const int CAST_CRAFT_C5 = 6;
        public const int CAST_CRAFT_C6 = 7;
        public const int CAST_CRAFT_C7 = 8;
            
        //index in cast quality
        public const int CAST_QUALITY_CODE = 1;
        public const int CAST_QUALITY_REEL_WEIGHT = 2;
        public const int CAST_QUALITY_REEL_THINKNESS = 3;
        public const int CAST_QUALITY_REEL_CORONA = 4;
        public const int CAST_QUALITY_REEL_WIDTH = 5;
        public const int CAST_QUALITY_REEL_DIAMETER = 6;
        public const int CAST_QUALITY_REEL_LENGTH = 7;

        //index in print craft
        public const int PRINTER_CRAFT_CODE = 1;
        public const int PRINTER_CRAFT_LINE_SPEED = 2;
        public const int PRINTER_CRAFT_OVEN_TEMPERATURE = 3;
        public const int PRINTER_CRAFT_KNIFE_PRESSURE = 4;
        public const int PRINTER_CRAFT_KNIFE_ANGLE = 5;
        public const int PRINTER_CRAFT_INK_VICOSITY = 6;
        public const int PRINTER_CRAFT_PRINT_ROLL_PRESSURE = 7;
        public const int PRINTER_CRAFT_RELEASE_STRAIN = 8;
        public const int PRINTER_CRAFT_INLET_STRAIN = 9;
        public const int PRINTER_CRAFT_OUTLET_STRAIN = 10;

        //index in print quality
        public const int PRINTER_QUALITY_CODE = 1;
        public const int PRINTER_QUALITY_REEL_WIDTH = 2;
        public const int PRINTER_QUALITY_REEL_DIAMETER = 3;
        public const int PRINTER_QUALITY_CORONA_SIDE = 4;
        public const int PRINTER_QUALITY_CORONA_DEGREE = 5;
        public const int PRINTER_QUALITY_PATTERN_DIRECTION = 6;
        public const int PRINTER_QUALITY_PATTERN_POSITION = 7;
        public const int PRINTER_QUALITY_PATTERN_COMPLETENESS = 8;
        public const int PRINTER_QUALITY_DEFECTS = 9;

        //index in slit
        public const int SLIT_CRAFT_CODE = 1;
        public const int SLIT_CRAFT_LINE_SPEED = 2;
        public const int SLIT_CRAFT_RELEASE_STRAIN = 3;
        public const int SLIT_CRAFT_TUCK_INIT_STRAIN1 = 4;
        public const int SLIT_CRAFT_TUCK_TAPER1 = 5;
        public const int SLIT_CRAFT_TUCKSLIT_NUMBER1 = 6;
        public const int SLIT_CRAFT_TUCK_INIT_STRAIN2 = 7;
        public const int SLIT_CRAFT_TUCK_TAPER2 = 8;
        public const int SLIT_CRAFT_TUCKSLIT_NUMBER2 = 9;

        //index in slit quality
        public const int SLIT_QUALITY_CODE = 1;
        public const int SLIT_QUALITY_REEL_WIDTH = 2;
        public const int SLIT_QUALITY_REEL_DIAMETER = 3;
        public const int SLIT_QUALITY_LENGTH = 4;
        public const int SLIT_QUALITY_WEIGHT = 5;
        public const int SLIT_QUALITY_REEL_THICKNESS = 6;
        public const int SLIT_QUALITY_REEL_CORONA = 7;
        public const int SLIT_QUALITY_PATTERN_DIRECTION = 8;
        public const int SLIT_QUALITY_PATTERN_COMPLETENESS = 9;
        public const int SLIT_QUALITY_DEFECTS = 10;

        //index in machine working plan AND machine status
        public const int MACHINE_PLAN_RECORD_TIME = 1;
        public const int MACHINE_PLAN_MACHINE_STATUS = 2;
        public const int MACHINE_PLAN_STATUS_START = 3;
        public const int MACHINE_PLAN_KEEP_MINUTES = 4;

        //index in dispatchList table
        public const int ID_VALUE_IN_DISPATCHLIST_DATABASE = 0;
        public const int MACHINE_ID_IN_DISPATCHLIST_DATABASE = 1;
        public const int DISPATCH_CODE_IN_DISPATCHLIST_DATABASE = 2;
        public const int PLANNED_STARTTIME_IN_DISPATCHLIST_DATABASE = 3;
        public const int PLANNED_COMPLETETIME_IN_DISPATCHLIST_DATABASE = 4;
        public const int PRODUCT_CODE_IN_DISPATCHLIST_DATABASE = 5;
        public const int PRODUCT_NAME_IN_DISPATCHLIST_DATABASE = 6;
        public const int OPERATOR_NAME_IN_DISPATCHLIST_DATABASE = 7;
        public const int FORCAST_NUM_IN_DISPATCHLIST_DATABASE = 8;
        public const int RECEIVE_NUM_IN_DISPATCHLIST_DATABASE = 9;
        public const int QUALIFIED_NUM_IN_DISPATCHLIST_DATABASE = 10;
        public const int UNQUALIFIED_NUM_IN_DISPATCHLIST_DATABASE = 11;
        public const int PROCESS_NAME_IN_DISPATCHLIST_DATABASE = 12;
        public const int START_TIME_IN_DISPATCHLIST_DATABASE = 13;
        public const int COMPLETE_TIME_IN_DISPATCHLIST_DATABASE = 14;
        public const int PREPARE_TIME_IN_DISPATCHLIST_DATABASE = 15;
        public const int STATUS_IN_DISPATCHLIST_DATABASE = 16;
        public const int EQUIPMENT_LIFETIME_IN_DISPATCHLIST_DATABASE = 17;
        public const int EQUIPMENT_USEDTIME_IN_DISPATCHLIST_DATABASE = 18;
        public const int OUTPUT_RATIO_IN_DISPATCHLIST_DATABASE = 19;
        public const int SERIAL_NO_IN_DISPATCHLIST_DATABASE = 20;
        public const int REPORTOR_IN_DISPATCHLIST_DATABASE = 21;
        public const int WORKSHOP_IN_DISPATCHLIST_DATABASE = 22;
        public const int WORK_SHIFT_IN_DISPATCHLIST_DATABASE = 23;
        public const int SALESORDER_CODE_IN_DISPATCHLIST_DATABASE = 24;
        public const int BOM_CODE_IN_DISPATCHLIST_DATABASE = 25;
        public const int CUSTOMER_IN_DISPATCHLIST_DATABASE = 26;
        public const int MULTI_PRODUCT_IN_DISPATCHLIST_DATABASE = 27;
        public const int PRODUCT_CODE2_IN_DISPATCHLIST_DATABASE = 28;
        public const int PRODUCT_CODE3_IN_DISPATCHLIST_DATABASE = 29;
        public const int OPERATOR2_NAME_IN_DISPATCHLIST_DATABASE = 30;
        public const int OPERATOR3_NAME_IN_DISPATCHLIST_DATABASE = 31;
        public const int BATCH_NUMBER_IN_DISPATCHLIST_DATABASE = 32;
        public const int PRODUCT_COLOR_IN_DISPATCHLIST_DATABASE = 33;
        public const int RAW_MATERIAL_IN_DISPATCHLIST_DATABASE = 34;
        public const int PRODUCT_LENGTH_IN_DISPATCHLIST_DATABASE = 35;
        public const int PRODUCT_DIAMETER_IN_DISPATCHLIST_DATABASE = 36;
        public const int PRODUCT_WEIGHT_IN_DISPATCHLIST_DATABASE = 37;
        public const int SLIT_WIDTH_IN_DISPATCHLIST_DATABASE = 38;
        public const int PRINT_SIDE_IN_DISPATCHLIST_DATABASE = 39;

        public const int MATERIAL_LIST_CYCLE_SPAN = 4;
        //index in material list table, which listed all kinds of materials for a dispatch
        public const int MATERIAL_LIST_ID = 0;
        public const int MATERIAL_LIST_SALES_CODE = 1;
        public const int MATERIAL_LIST_DISPATCH_CODE = 2;
        public const int MATERIAL_LIST_MACHINE_CODE = 3;
        public const int MATERIAL_LIST_MACHINE_NAME = 4;
        public const int MATERIAL_LIST_STATUS = 5;
        public const int MATERIAL_LIST_NUM_OF_TYPE = 6;
        public const int MATERIAL_LIST_MATERIAL_NAME1 = 7;
        public const int MATERIAL_LIST_MATERIAL_CODE1 = 8;
        public const int MATERIAL_LIST_MATERIAL_REQUIRED1 = 9;
        public const int MATERIAL_LIST_FULL_PACK_NUM1 = 10;
        public const int MATERIAL_LIST_MATERIAL_NAME2 = 11;
        public const int MATERIAL_LIST_MATERIAL_CODE2 = 12;
        public const int MATERIAL_LIST_MATERIAL_REQUIRED2 = 13;
        public const int MATERIAL_LIST_FULL_PACK_NUM2 = 14;
        public const int MATERIAL_LIST_MATERIAL_NAME3 = 15;
        public const int MATERIAL_LIST_MATERIAL_CODE3 = 16;
        public const int MATERIAL_LIST_MATERIAL_REQUIRED3 = 17;
        public const int MATERIAL_LIST_FULL_PACK_NUM3 = 18;
        public const int MATERIAL_LIST_MATERIAL_NAME4 = 19;
        public const int MATERIAL_LIST_MATERIAL_CODE4 = 20;
        public const int MATERIAL_LIST_MATERIAL_REQUIRED4 = 21;
        public const int MATERIAL_LIST_FULL_PACK_NUM4 = 22;
        public const int MATERIAL_LIST_MATERIAL_NAME5 = 23;
        public const int MATERIAL_LIST_MATERIAL_CODE5 = 24;
        public const int MATERIAL_LIST_MATERIAL_REQUIRED5 = 25;
        public const int MATERIAL_LIST_FULL_PACK_NUM5 = 26;
        public const int MATERIAL_LIST_MATERIAL_NAME6 = 27;
        public const int MATERIAL_LIST_MATERIAL_CODE6 = 28;
        public const int MATERIAL_LIST_MATERIAL_REQUIRED6 = 29;
        public const int MATERIAL_LIST_FULL_PACK_NUM6 = 30;
        public const int MATERIAL_LIST_MATERIAL_NAME7 = 31;
        public const int MATERIAL_LIST_MATERIAL_CODE7 = 32;
        public const int MATERIAL_LIST_MATERIAL_REQUIRED7 = 33;
        public const int MATERIAL_LIST_FULL_PACK_NUM7 = 34;
        public const int MATERIAL_LIST_MATERIAL_NAME8 = 35;
        public const int MATERIAL_LIST_MATERIAL_CODE8 = 36;
        public const int MATERIAL_LIST_MATERIAL_REQUIRED8 = 37;
        public const int MATERIAL_LIST_FULL_PACK_NUM8 = 38;

        //index in float data table, like craft data table, curvol data table, quality data table, etc.
        const int ID_VALUE_IN_DATA_DATABASE = 0;
        const int TIME_VALUE_IN_DATA_DATABASE = 1;
        const int DATA_VALUE1_IN_DATA_DATABASE = 2;
        const int DATA_VALUE2_IN_DATA_DATABASE = 3;

        //employee table column index definition
        public const int EMPLOYEE_ID_COLIUMN = 0;
        public const int EMPLOYEE_WORKERID_COLIUMN = 1;
        public const int EMPLOYEE_NAME_COLIUMN = 2;
        public const int EMPLOYEE_SEX_COLIUMN = 3;
        public const int EMPLOYEE_AGE_COLIUMN = 4;
        public const int EMPLOYEE_POSTIOIN_COLIUMN = 5;
        public const int EMPLOYEE_DEPARTMENT_COLIUMN = 6;
        public const int EMPLOYEE_DATE_COLIUMN = 7;
        public const int EMPLOYEE_PASSWORD_COLIUMN = 8;
        public const int EMPLOYEE_RANK_COLIUMN = 9;
        public const int EMPLOYEE_PRIVILEGE1_COLIUMN = 10;
        public const int EMPLOYEE_PRIVILEGE2_COLIUMN = 11;
        public const int EMPLOYEE_PRIVILEGE3_COLIUMN = 12;
        public const int EMPLOYEE_PRIVILEGE4_COLIUMN = 13;
        public const int EMPLOYEE_MAIL_ADDR_COLIUMN = 14;

        //0_machineWorkingPlan
        public const int MAINTENANCE_ID_INDEX = 1;
        public const int SALES_ORDER_INDEX = 2;
        public const int DISPATCH_CODE_INDEX = 3;
        public const int START_TIME_INDEX = 4;
        public const int START_TIME_STAMP_INDEX = 5;
        public const int KEEP_DURATION_INDEX = 6;
        public const int END_TIME_INDEX = 7;
        public const int END_TIME_STAMP_INDEX = 8;


        //global and machine dispatch list table
        const string strDispatchList1 = "(id int(1) AUTO_INCREMENT primary key, machineID varchar(40), dispatchCode varchar(40), planTime1 varchar(20), planTime2 varchar(20), productCode varchar(40), " +
                                         "productName varchar(40), operatorName varchar(40), forcastNum int(1), receiveNum int(1), qualifyNum int(1), unqualifyNum int(1), processName varchar(30), " +
                                         "startTime varchar(20), completeTime varchar(20), prepareTimePoint varchar(20), status int(1), toolLifeTimes int(1), toolUsedTimes int(1), outputRatio int(1), " +
                                         "serialNumber int(1), reportor varchar(14), workShop varchar(40), workShift varchar(20), salesOrderCode varchar(40), BOMCode varchar(40)) ENGINE = MYISAM CHARSET=utf8";
        const string strDispatchList2 = " value(@id, @machineID, @dispatchCode, @planTime1, @planTime2, @productCode, @productName, @operatorName, @forcastNum, @receiveNum, @qualifyNum, @unqualifyNum, @processName, " +
                                        "@startTime, @completeTime, @prepareTimePoint, @status, @toolLifeTimes, @toolUsedTimes, @outputRatio, @serialNumber, @reportor, @workShop, @workShift, " +
                                        "@salesOrderCode, @BOMCode)";

        //machine material list table
        const string strBOMList1 = "(id int(1) AUTO_INCREMENT primary key, dispatchCode varchar(40), machineCode varchar(40), materialName varchar(40), materialCode varchar(40), " + 
                                        "materialQuantity int(1)) ENGINE = MYISAM CHARSET=utf8";
        const string strBOMList2 = " value(@id, @dispatchCode, @machineCode, @materialName, @materialCode, @materialQuantity)";

        //machine craft parameter list table
        const string strCraftList1 = "(id int(1) AUTO_INCREMENT primary key, dispatchCode varchar(40), paramName varchar(40), paramLowerLimit float(1), paramUpperLimit float(1), paramDefaultValue float(1), " + 
                                      "paramUnit varchar(20), paramValue float(1), rangeLowerLimit float(1), rangeUpperLimit float(1)) ENGINE = MYISAM CHARSET=utf8";
        const string strCraftList2 = " value(@id, @dispatchCode, @paramName, @paramLowerLimit, @paramUpperLimit, @paramDefaultValue, @paramUnit, @paramValue, @rangeLowerLimit, @rangeUpperLimit)";

        //machine craft data table
        const string strDataCraft1 = "(id int(1) AUTO_INCREMENT primary key, time int(1), value1 float(1), value2 float(1), value3 float(1), value4 float(1), value5 float(1), value6 float(1), value7 float(1), " + 
                                      "value8 float(1)) ENGINE = MYISAM CHARSET=utf8";
        const string strDataCraft2 = " value(@id, @time, @value1, @value2, @value3, @value4, @value5, @value6, @value7, @value8)";

        //machine overall current data table
        const string strDataCurrent1 = "(id int(1) AUTO_INCREMENT primary key, time int(1), value float(1)) ENGINE = MYISAM CHARSET=utf8";
        const string strDataCurrent2 = " value(@id, @time, @value)";

        //machine voltage/current data table
        const string strDataVolCur1 = "(id int(1) AUTO_INCREMENT primary key, time int(1), value1 float(1), value2 float(1), value3 float(1), value4 float(1)) ENGINE = MYISAM CHARSET=utf8";
        const string strDataVolCur2 = " value(@id, @time, @value1, @value2, @value3, @value4)";

        //machine quality list table
        public const int QUALITY_LIST_ID_ITEM_NAME = 1;
        public const int QUALITY_LIST_ID_CHART_TYPE = 15;
        const string strQualityList1 = "(id int(1) AUTO_INCREMENT primary key, dispatchCode varchar(40), checkItem varchar(40), checkRequirement varchar(40), controlCenterValue1 float(1), controlCenterValue2 float(1)," + 
                                        "specLowerLimit float(1), controlLowerLimit1 float(1), controlLowerLimit2 float(1), specUpperLimit float(1), controlUpperLimit1 float(1), controlUpperLimit2 float(1), " + 
                                        "sampleRatio int(1), checkResultData varchar(10), checkResult varchar(10), unit varchar(20), chartType int(1)) ENGINE = MYISAM CHARSET=utf8";
        const string strQualityList2 = " value(@id, @dispatchCode, @checkItem, @checkRequirement, @controlCenterValue1, @controlCenterValue2, @specLowerLimit, @controlLowerLimit1, @controlLowerLimit2, @specUpperLimit," +
                                        "@controlUpperLimit1, @controlUpperLimit2, @sampleRatio, @checkResultData, @checkResult, @unit, @chartType)";

        //machine quality data table
        const string strDataQuality1 = "(id int(1) AUTO_INCREMENT primary key, time int(1), value1 float(1), value2 float(1), value3 float(1), value4 float(1), value5 float(1), value6 float(1), value7 float(1), " + 
                                        "value8 float(1), value9 float(1), value10 float(1), value11 float(1), value12 float(1), value13 float(1), value14 float(1), value15 float(1), value16 float(1), " +
                                        "sn varchar(30), status1 smallint(1), status2 smallint(1), status3 smallint(1), status4 smallint(1), status5 smallint(1), status6 smallint(1), status7 smallint(1), " +
                                        "status8 smallint(1), status9 smallint(1), status10 smallint(1), status11 smallint(1), status12 smallint(1), status13 smallint(1), status14 smallint(1), status15 smallint(1), " +
                                        "status16 smallint(1)) ENGINE = MYISAM CHARSET=utf8";
        const string strDataQuality2 = " value(@id, @time, @value1, @value2, @value3, @value4, @value5, @value6, @value7, @value8, @value9, @value10, @value11, @value12, @value13, @value14, @value15, @value16, @sn, " +
                                        "@status1, @status2, @status3, @status4, @status5, @status6, @status7, @status8, @status9, @status10, @status11, @status12, @status13, @status14, @status15, @status16)";

        //machine beat data table
        const string strDataBeat1 = "(id int(1) AUTO_INCREMENT primary key, time int(1), value1 int(1), value2 int(1), value3 int(1), value4 int(1)) ENGINE = MYISAM CHARSET=utf8";
        const string strDataBeat2 = " value(@id, @time, @value1, @value2, @value3, @value4)";

        //machine repair table -- dispatch here means:repairing didpatch
        const string strrepairList1 = "(id int(1) AUTO_INCREMENT primary key, dispatchCode varchar(20), machineName varchar(40), time varchar(19), timeSpan int(1), affectedTime int(1), " + 
                                      "descript varchar(400), analysis varchar(400), solution varchar(400), substitute varchar(100), asist varchar(100), owner varchar(40), notes varchar(400)) " + 
                                      "ENGINE = MYISAM CHARSET=utf8";
        const string strrepairList2 = " value(@id, @dispatchCode, @machineName, @time, @timeSpan, @affectedTime, @descript, @analysis, @solution, @substitute, @asist, @owner, @notes)";

        //machine maintenance table
        const string strmaintainList1 = "(id int(1) AUTO_INCREMENT primary key, dispatch varchar(20), maintenanceCode varchar(20), machineName varchar(40), parts varchar(40), content varchar(200), gap int(1), " +
                                         " time1 varchar(19), time2 varchar(19), personNum int(1), duration int(1), substitute varchar(100), asist varchar(100), owner varchar(40), " + 
                                         "notes varchar(400)) ENGINE = MYISAM CHARSET=utf8";
        const string strmaintainList2 = " value(@id, @dispatch, @maintenanceCode, @machineName, @parts, @content, @gap, @time1, @time2, @personNum, @duration, @substitute, @asist, @owner, @notes)";

        //machine everydayChecking table
        const string strDailyChecking1 = "(id int(1) AUTO_INCREMENT primary key, dispatch varchar(20), checkCode varchar(20), machineName varchar(40), time varchar(19), shift varchar(10), " +
                                       " chechPart varchar(40), checkPoints varchar(400), checker varchar(40), notes varchar(400)) ENGINE = MYISAM CHARSET=utf8";
        const string strDailyChecking2 = " value(@id, @dispatch, @checkCode, @machineName, @time, @shift, @chechPart, @checkPoints, @checker, @notes)";

        //machine add oil table
        const string strDataOil1 = "(id int(1) AUTO_INCREMENT primary key, dispatch varchar(20), addOilCode varchar(20), machineName varchar(40), parts varchar(40), pointOfOil int(1), span int(1), " + 
                                        "oilType varchar(40), OKStandard varchar(40), time varchar(19), checker varchar(40), notes varchar(400)) ENGINE = MYISAM CHARSET=utf8";
        const string strDataOil2 = " value(@id, @dispatch, @addOilCode, @machineName, @parts, @pointOfOil, @span, @oilType, @OKStandard, @time, @checker, @notes)";

        //machine washup table
        const string strDataWashup1 = "(id int(1) AUTO_INCREMENT primary key, dispatch varchar(20), addOilCode varchar(20), machineName varchar(40), parts varchar(40), pointOfOil int(1), span int(1), " +
                                        "oilType varchar(40), OKStandard varchar(40), time varchar(19), checker varchar(40), notes varchar(400)) ENGINE = MYISAM CHARSET=utf8";
        const string strDataWashup2 = " value(@id, @dispatch, @addOilCode, @machineName, @parts, @pointOfOil, @span, @oilType, @OKStandard, @time, @checker, @notes)";

        //setting1 -- ADC  
        const string strDataSetting11 = "(id int(1) AUTO_INCREMENT primary key, dispatchCode varchar(40), channelEnabled int(1), channelTitle varchar(40), channelUnit varchar(20), lowerRange float(1), " + 
                                         "upperRange float(1), lowerLimit float(1), upperLimit float(1), workingVoltage int(1)) ENGINE = MYISAM CHARSET=utf8";
        const string strDataSetting12 = " value(@id, @dispatchCode, @channelEnabled, @channelTitle, @channelUnit, @lowerRange, @upperRange, @lowerLimit, @upperLimit, @workingVoltage)";

        //setting2 -- UART  
        const string strDataSetting21 = "(id int(1) AUTO_INCREMENT primary key, dispatchCode varchar(40), uartDeviceType int(1), uartBaudrate int(1)) ENGINE = MYISAM CHARSET=utf8";
        const string strDataSetting22 = " value(@id, @dispatchCode, @uartDeviceType, @uartBaudrate)";

        //setting3  -- GPIO
        const string strDataSetting31 = "(id int(1) AUTO_INCREMENT primary key, dispatchCode varchar(40)) ENGINE = MYISAM CHARSET=utf8";
        const string strDataSetting32 = " value(@id, @dispatchCode)";

        //setting4 -- beat
        const string strDataSetting41 = "(id int(1) AUTO_INCREMENT primary key, dispatchCode varchar(40), idleCurrentHigh float(1), idleCurrentLow float(1), workCurrentHigh float(1), workCurrentLow float(1), " + 
                                         "errorCurrentHigh float(1), errorCurrentLow float(1)) ENGINE = MYISAM CHARSET=utf8";
        const string strDataSetting42 = " value(@id, @dispatchCode, @idleCurrentHigh, @idleCurrentLow, @workCurrentHigh, @workCurrentLow, @errorCurrentHigh, @errorCurrentLow)";

        //global and machine alarm
        const string strAlarmDesc1 = "(id int(1) AUTO_INCREMENT primary key, errorDesc varchar(200), dispatchCode varchar(40), alarmFailureCode varchar(40), machineCode varchar(40), machineName varchar(40), " +
                                      "operatorName varchar(30), time varchar(20), signer varchar(30), time1 varchar(20), completer varchar(30), time2 varchar(20), type smallint(1), category smallint(1), " +
                                      "status smallint(1), inHistory smallint(1), startID int(1), indexInTable int(1), workshop varchar(40), MailList varchar(1000), discuss varchar(4000), " + 
                                      "solution varchar(4000)) ENGINE = MYISAM CHARSET=utf8";
        const string strAlarmDesc2 = " value(@id, @errorDesc, @dispatchCode, @alarmFailureCode, @machineCode, @machineName, @operatorName, @time, @signer, @time1, @completer, @time2, @type, @category, " +
                                      "@status, @inHistory, @startID, @indexInTable, @workshop, @MailList, @discuss, @solution)";

        //machine status
        const string strMachineStatus1 = "(id int(1) AUTO_INCREMENT primary key, dispatchCode varchar(40), totalWorkingTime int(1), collectedNumber int(1), productBeat int(1), workingTime int(1), " +
                                          "prepareTime int(1), standbyTime int(1), power int(1), powerConsumed int(1), revolution int(1), maintenancePeriod int(1), lastMaintenance int(1), " + 
                                          "toolLifeTimes int(1), toolUsedTimes int(1)) ENGINE = MYISAM CHARSET=utf8";
        const string strMachineStatus2 = " value(@id, @dispatchCode, @totalWorkingTime, @collectedNumber, @productBeat, @workingTime, @prepareTime, @standbyTime, @power, @powerConsumed, @revolution, " +
                                         "@maintenancePeriod, @lastMaintenance, @toolLifeTimes, @toolUsedTimes)";

        //table index in one database for createDataTableString and insertDataTableString, and we will use this index to indicate which database table we are working on
        public const int DATA_TYPE_DISPATCH_LIST = 0;
        public const int DATA_TYPE_MATERIAL_LIST = 1;
        public const int DATA_TYPE_CRAFT_LIST = 2;
        public const int DATA_TYPE_QUALITY_LIST = 3;
        public const int DATA_TYPE_SETTING1 = 4;
        public const int DATA_TYPE_SETTING2 = 5;
        public const int DATA_TYPE_SETTING3 = 6;
        public const int DATA_TYPE_SETTING4 = 7;
        public const int DATA_TYPE_ALARM_DATA = 8;
        public const int DATA_TYPE_MACHINE_STATUS = 9;
        public const int DATA_TYPE_CRAFT_DATA = 10;
        public const int DATA_TYPE_VOLCUR_DATA = 11;
        public const int DATA_TYPE_QUALITY_DATA = 12;
        public const int DATA_TYPE_BEAT_DATA = 13;
        public const int DATA_TYPE_CURRENT_VALUE = 14;
        public const int DATA_TYPE_DAILYCHECK = 15;
        public const int DATA_TYPE_ADDOIL = 16;
        public const int DATA_TYPE_WASHUP = 17;
        public const int DATA_TYPE_MAINTAIN = 18;
        public const int DATA_TYPE_REPAIR = 19;

        //refer to the type above, from dispatch_list to machine_status, all 0, for craft/volcur/quality/beat, how many data items exits in the table
        static int[] tableDataNumMax = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, gVariable.maxCraftParamNum, 4, gVariable.maxQualityDataNum, 4 };

        public static string[] createDataTableString = { strDispatchList1, strBOMList1, strCraftList1, strQualityList1, strDataSetting11, strDataSetting21, strDataSetting31, strDataSetting41, strAlarmDesc1,  
                                           strMachineStatus1, strDataCraft1, strDataVolCur1, strDataQuality1, strDataBeat1, strDataCurrent1, strDailyChecking1, strDataOil1, strDataWashup1, 
                                           strmaintainList1, strrepairList1};
        public static string[] insertDataTableString = { strDispatchList2, strBOMList2, strCraftList2, strQualityList2, strDataSetting12, strDataSetting22, strDataSetting32, strDataSetting42, strAlarmDesc2, 
                                           strMachineStatus2, strDataCraft2, strDataVolCur2, strDataQuality2, strDataBeat2, strDataCurrent2, strDailyChecking2, strDataOil2, strDataWashup2,
                                           strmaintainList2, strrepairList2};

        //table name for all tables, it is used for output content at console when exception occurs
        static string[] tableNameList = { "dispatch list", "material", "craft list", "quality list", "setting1", "setting2", "setting3", "setting4", "alarm", "status", "craft data", "volcur data", "quality data", 
                                          "beat", "dataCurrent", "dailyChecking", "addOil", "washup", "maintenance", "repair", "salesOrder"};

        //table name index used for infoTableName and infoTableFileName
        public const int TABLE_NAME_EMPLOYEE = 0;
        public const int TABLE_NAME_MATERIAL = 1;
        public const int TABLE_NAME_PRODUCT = 2;
        public const int TABLE_NAME_MACHINE = 3;
        public const int TABLE_NAME_PROCEDURE = 4;
        public const int TABLE_NAME_BOM = 5;
        public const int TABLE_NAME_PRODUCT_SPEC = 6;
        public const int TABLE_NAME_CAST_SPEC = 7;
        public const int TABLE_NAME_PRINT_SPEC = 8;
        public const int TABLE_NAME_SLIT_SPEC = 9;
        public const int TABLE_NAME_CAST_CRAFT = 10;
        public const int TABLE_NAME_PRINT_CRAFT = 11;
        public const int TABLE_NAME_SLIT_CRAFT = 12;
        public const int TABLE_NAME_CAST_QUALITY = 13;
        public const int TABLE_NAME_PRINT_QUALITY = 14;
        public const int TABLE_NAME_SLIT_QUALITY = 15;
        public const int TABLE_NAME_SALES_ORDER = 16;

        static string insertStringForDispatch;
        static string insertStringForMaterial;
        static string insertStringForWorkingPlan;

        //info database1 is a global database which contains tables that will becomes larger and larger during production because more data is generated
        public static string[] infoDatabaseName1 = 
        {
            gVariable.globalDatabaseName, gVariable.globalDatabaseName, gVariable.globalDatabaseName, gVariable.globalDatabaseName, gVariable.globalDatabaseName, 
            gVariable.globalDatabaseName, gVariable.globalDatabaseName, gVariable.globalDatabaseName, gVariable.globalDatabaseName, gVariable.globalDatabaseName, 
            gVariable.globalDatabaseName, gVariable.globalDatabaseName, gVariable.globalDatabaseName, gVariable.globalDatabaseName, gVariable.globalDatabaseName, 
            gVariable.globalDatabaseName, gVariable.globalDatabaseName, gVariable.globalDatabaseName,
        };
        public static string[] infoTableName1 = 
        { 
        	gVariable.globalAlarmListTableName, gVariable.globalDispatchTableName, gVariable.globalMaterialTableName, gVariable.salesOrderTableName,  gVariable.inspectionListTableName,
            gVariable.reuseMaterialTableName,   gVariable.wasteMaterialTableName, gVariable.changePartRecordTableName, gVariable.partsInventoryTableName, gVariable.materialDeliveryTableName,
            gVariable.finalPackingTableName, gVariable.materialFeedingTableName, gVariable.productCastListTableName, gVariable.productPrintListTableName, gVariable.productSlitListTableName,
            gVariable.binInventoryTableName,  gVariable.dispatchCurrentIndexTableName, gVariable.productBatchTableName,
        };
        public static string[] infoTableFileName1 = 
        {
            gVariable.globalAlarmListFileName, gVariable.dispatchListFileName, gVariable.materialListFileName, gVariable.salesOrderFileName, gVariable.inspectionListFileName,
            gVariable.reuseMaterialFileName,   gVariable.wasteMaterialFileName, gVariable.changePartRecordFileName, gVariable.partsInventoryFileName, gVariable.materialDeliveryFileName,
            gVariable.finalPackingFileName, gVariable.materialFeedingFileName, gVariable.productCastListFileName, gVariable.productPrintListFileName, gVariable.productSlitListFileName,
            gVariable.binInventoryFileName, gVariable.dispatchCurrentIndexFileName, gVariable.productBatchFileName
        };

        //info database2 is basic info
        public static string[] infoDatabaseName2 = 
        {
            gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, 
            gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, 
            gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, 
            gVariable.basicInfoDatabaseName, gVariable.basicInfoDatabaseName, 
        };
        public static string[] infoTableName2 = 
        { 
        	gVariable.employeeTableName, gVariable.materialTableName, gVariable.productTableName, gVariable.machineTableName, gVariable.workProcedureTableName, gVariable.bomTableName, 
            gVariable.castSpecTableName, gVariable.printSpecTableName, gVariable.slitSpecTableName, gVariable.castCraftTableName, gVariable.printCraftTableName, gVariable.inkBomTableName, 
            gVariable.slitCraftTableName, gVariable.castQualityTableName, gVariable.printQualityTableName, gVariable.slitQualityTableName, gVariable.customerListTableName, gVariable.vendorListTableName, 
            gVariable.warehouseListTableName, gVariable.packingTableName, 
        };

        public static string[] infoTableFileName2 = 
        {
            gVariable.employeeFileName, gVariable.materialFileName, gVariable.productFileName, gVariable.machineFileName, gVariable.workProcedureFileName, gVariable.bomFileName, 
            gVariable.castSpecFileName, gVariable.printSpecFileName, gVariable.slitSpecFileName, gVariable.castCraftFileName, gVariable.printCraftFileName, gVariable.inkBomFileName, 
            gVariable.slitCraftFileName, gVariable.castQualityFileName, gVariable.printQualityFileName, gVariable.slitQualityFileName, gVariable.customerListFileName, gVariable.vendorListFileName, 
            gVariable.warehouseListFileName, gVariable.packingFileName,
        };

        public mySQLClass()
        {
            string str;
            string hostString;
            MySqlConnection myConnection;

            hostString = getDatabaseHost();
//            gVariable.communicationHostIP = getCommunicationHostIP();
            getCompanyIndex();
            str = getMESData();

            if (str == "1")
                gVariable.faultData = 0;
            else
                gVariable.faultData = 1;

            if (hostString == "localhost" || hostString == "127.0.0.1")
            {
                gVariable.thisIsHostPC = true;
                gVariable.hostString = "localhost";
                connectionString = "data source = " + hostString + "; user id = root; PWD = ; Charset=utf8";
            }
            else
            {
                gVariable.thisIsHostPC = false;
                gVariable.hostString = hostString;
                connectionString = "data source = " + hostString + "; user id = zihua; PWD = auhiz; Charset=utf8";
//                connectionString = "data source = " + hostString + "; user id = root; PWD = ; Charset=utf8";
            }

            try
            {
                myConnection = new MySqlConnection(connectionString);
                myConnection.Open();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("connection fail!" + ex);
                MessageBox.Show("抱歉，请确认 wamp server 已启动，否则本系统无法运行。", "信息提示", MessageBoxButtons.OK);
                System.Environment.Exit(0);
            }

            getDispatchInsertString();
            getMaterialInsertString();
            getWorkingPlanInsertString();
        }

        private string getDatabaseHost()
        {
            string filePath = "..\\..\\init\\host.ini";
            StreamReader streamReader;
            string hostString;

            try
            {
                streamReader = new StreamReader(filePath, System.Text.Encoding.Default);
                hostString = streamReader.ReadLine().Trim();
                streamReader.Close();

                return hostString;
            }
            catch (Exception ex)
            {
                Console.Write("host 文件异常");
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        /*
        private string getCommunicationHostIP()
        {
            string filePath = "..\\..\\init\\ipaddr.ini";
            StreamReader streamReader;
            string IPString;

            try
            {
                streamReader = new StreamReader(filePath, System.Text.Encoding.Default);
                IPString = streamReader.ReadLine().Trim();
                streamReader.Close();

                return IPString;
            }
            catch (Exception ex)
            {
                Console.Write("host 文件异常");
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        */

        private string getMESData()
        {
            string filePath = "..\\..\\init\\MES.txt";
            StreamReader streamReader;
            string MESStr;

            try
            {
                streamReader = new StreamReader(filePath, System.Text.Encoding.Default);
                MESStr = streamReader.ReadLine().Trim();
                streamReader.Close();

                return MESStr;
            }
            catch (Exception ex)
            {
                Console.Write("host 文件异常");
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private void getCompanyIndex()
        {
            string filePath = "..\\..\\init\\company.txt";
            StreamReader streamReader;
            string str;

            try
            {
                streamReader = new StreamReader(filePath, System.Text.Encoding.Default);
                str = streamReader.ReadLine().Trim();
                streamReader.Close();

                if (str == "0")
                    gVariable.CompanyIndex = gVariable.DONGFENG_23;
                else if (str == "1")
                    gVariable.CompanyIndex = gVariable.ZIHUA_ENTERPRIZE;
                else
                    gVariable.CompanyIndex = gVariable.DONGFENG_20;
            }
            catch (Exception ex)
            {
                Console.Write("host 文件异常");
                Console.WriteLine(ex.ToString());
                gVariable.CompanyIndex = 0;
            }
        }

        public static void getAllDatabaseName()
        {
            string queryString = "SELECT SCHEMA_NAME FROM information_schema SCHEMATA";

            try
            {
                MySqlConnection myConnection = new MySqlConnection(connectionString);

                MySqlCommand cmd = new MySqlCommand(queryString, myConnection);

/*            $query = mysql_query($sql);
            while($rs = mysql_fetch_array($query))
            {
                echo $rs[0]."<br/>";
            }
*/                
                myConnection.Open();
                cmd.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + queryString + "generating fail!" + ex);
            }

        }

        //clear all database and generate info table database
        public static void buildBasicDatabase()
        {
            int i;
            string dname;

            try
            {
                //we have 12 extra board, so we need to 
                for (i = 0; i < gVariable.maxMachineNum + 1; i++)
                {
                    dname = gVariable.internalMachineName[i];

                    deleteDatabase(dname);
                    createDatabase(dname);

                    createDataTableFromExcel(dname, gVariable.machineWorkingPlanTableName, gVariable.machineWorkingPlanFileName, LEN_40, 0);
                    createDataTableFromExcel(dname, gVariable.machineStatusRecordTableName, gVariable.machineStatusRecordFileName, LEN_40, 0);

                    //a special case for H015, we need this dispatch for testing 
                    if(i == 14)
                        createDataTableFromExcel(dname, gVariable.dispatchListTableName, gVariable.dispatchListFileName, LEN_40, 1);
                    else
                        createDataTableFromExcel(dname, gVariable.dispatchListTableName, gVariable.dispatchListFileName, LEN_40, 0);

                    createDataTableFromExcel(dname, gVariable.machineStatusListTableName, gVariable.machineStatusListFileName, LEN_40, 0);
                    createDataTableFromExcel(dname, gVariable.craftListTableName, gVariable.craftListFileName, LEN_40, 0);
                    createDataTableFromExcel(dname, gVariable.qualityListTableName, gVariable.qualityListFileName, LEN_40, 0);
                    createDataTableFromExcel(dname, gVariable.currentValueTableName, gVariable.currentValueFileName, LEN_40, 0);
                    createDataTableFromExcel(dname, gVariable.repairListTableName, gVariable.repairListFileName, LEN_100, 0);
                    createDataTableFromExcel(dname, gVariable.maintainListTableName, gVariable.maintainListFileName, LEN_100, 0);
                    createDataTableFromExcel(dname, gVariable.dailyCheckListTableName, gVariable.dailyCheckListFileName, LEN_100, 0);
                    createDataTableFromExcel(dname, gVariable.addOilListTableName, gVariable.addOilListFileName, LEN_100, 0);
                    createDataTableFromExcel(dname, gVariable.washupListTableName, gVariable.washupListFileName, LEN_100, 0);
                    createDataTableFromExcel(dname, gVariable.alarmListTableName, gVariable.alarmListFileName, LEN_400, 0);

                    //a special case for H008, we need this dispatch for testing 
                    if (i == 7)
                        createDataTableFromExcel(dname, gVariable.materialListTableName, gVariable.materialListFileName, LEN_40, 1);
                    else
                        createDataTableFromExcel(dname, gVariable.materialListTableName, gVariable.materialListFileName, LEN_40, 0);

                    createDataTableFromExcel(dname, gVariable.stackInventoryListTableName, gVariable.stackInventoryListFileName, LEN_40, 0);
                    createDataTableFromExcel(dname, gVariable.feedBinInventoryListTableName, gVariable.feedBinInventoryListFileName, LEN_40, 0);
                }

                //basic info
                deleteDatabase(gVariable.basicInfoDatabaseName);
                createDatabase(gVariable.basicInfoDatabaseName);

                //all machine alarms
                deleteDatabase(gVariable.globalDatabaseName);
                createDatabase(gVariable.globalDatabaseName);

                for (i = 0; i < gVariable.maxCurveNum; i++)
                    gVariable.dataNumForCurve[i] = 0;

                //create basic information table, including employee, product, material, working craft tables
                for (i = 0; i < infoTableName1.Length; i++)  //these tables have no sample data
                {
                    if (i == 0 || i == 2)   //alarm/material table need no predefined data, while sales order need predefined data from excel file for testing
                        createDataTableFromExcel(infoDatabaseName1[i], infoTableName1[i], infoTableFileName1[i], LEN_400, 0);
                    else if (i == 1)
                    {
                        //if we are working in bulletin board mode, we need to prepare dispatch for all the machines
                        if (gVariable.workshopReport == gVariable.WORKSHOP_REPORT_BULLETIN)
                        {
                            createDataTableFromExcel(infoDatabaseName1[i], infoTableName1[i], infoTableFileName1[i], LEN_400, 1);
                        }
                        else
                        {
                            createDataTableFromExcel(infoDatabaseName1[i], infoTableName1[i], infoTableFileName1[i], LEN_400, 0);
                        }
                    }
                    else
                        createDataTableFromExcel(infoDatabaseName1[i], infoTableName1[i], infoTableFileName1[i], LEN_40, 1);
                }

                for (i = 0; i < infoTableName2.Length; i++)  //need to look for sample data in excel files
                {
                    createDataTableFromExcel(infoDatabaseName2[i], infoTableName2[i], infoTableFileName2[i], LEN_40, 1);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("buildBasicDatabase failed!" + ex);
            }

        }

        //return 0: not exist
        public static int databaseExistsOrNot(string databaseName)
        {
            int ret;
            String str;
            MySqlDataReader myReader;

            try
            {
                databaseName = databaseName.ToLower();

                //information_schema is the top level database, in contains all the information of database under localhost
                MySqlConnection myConnection = new MySqlConnection("database = information_schema; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "show databases";

                myReader = myCommand.ExecuteReader();

                ret = 0;
                if (!myReader.HasRows)
                {
                    myConnection.Close();
                    return ret;
                }

                while (true)
                {
                    if (myReader.Read())
                    {
                        str = (string)myReader.GetValue(0);
                        if (str == databaseName)
                        {
                            ret = 1;
                            break;
                        }
                    }
                    else
                        break;
                }

                myReader.Close();
                myConnection.Close();
                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine("tableExistsOrNot for " + databaseName + " failed!" + ex);
                return 0;
            }
        }


        public static int tableExistsOrNot(string databaseName, string tableName)
        {
            try
            {
                int ret;
                string str;
                MySqlDataReader myReader;
                object[] nameStr = new object[20];

                ret = databaseExistsOrNot(databaseName.ToLower());
                if (ret == 0)
                    return 0;

                //mySQL dose not support upper letter to be table name, so we set it to lower case before comparation
                //we may have problem in the futuren need more consideration here
                tableName = tableName.ToLower();
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "show tables";
                myReader = myCommand.ExecuteReader();

                ret = 0;
                if (!myReader.HasRows)
                {
                    myConnection.Close();
                    return ret;
                }

                while (true)
                {
                    if (myReader.Read())
                    {
                        str = (string)myReader.GetValue(0);
                        if (str == tableName)
                        {
                            ret = 1;
                            break;
                        }
                    }
                    else
                        break;
                }

                myReader.Close();
                myConnection.Close();
                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine("tableExistsOrNot for " + tableName + " failed!" + ex);
                return 0;
            }
        }


        public static int recordExistsOrNot(string databaseName, string tableName, string recordName, string recordValue)
        {
            try
            {
                int ret;
//                string str;
                object[] nameStr = new object[20];

                //mySQL dose not support upper letter to be table name, so we set it to lower case before comparation
                //we may have problem in the futuren need more consideration here
                tableName = tableName.ToLower();
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                myCommand.CommandText = "select * from `" + tableName + "` where " + recordName + " = " + "\'" + recordValue + "\'";
                MySqlDataReader myReader = myCommand.ExecuteReader();

                if (myReader != null && myReader.HasRows == true)
                    ret = 1;
                else
                    ret = 0;

                myReader.Close();
                myConnection.Close();
                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine("recordExistsOrNot for " + databaseName + "|" + tableName + "|" + recordValue + " failed!" + ex);
                return 0;
            }
        }

        public static void createDatabase(String databaseName)
        {
            try
            {
                String createString = "create database IF NOT EXISTS " + databaseName;

                MySqlConnection myConnection = new MySqlConnection(connectionString);

                MySqlCommand cmd = new MySqlCommand(createString, myConnection);
                myConnection.Open();
                cmd.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "generating fail!" + ex);
            }
        }


        //stringTypeIndex could be float, int or string type
        public static void createDataTable(String databaseName, string tableName, int stringTypeIndex)
        {
            try
            {
                String createString;

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                createString = "create table IF NOT EXISTS `" + tableName + "`" + createDataTableString[stringTypeIndex];

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                MySqlCommand cmd = new MySqlCommand(createString, myConnection);
                myConnection.Open();
                cmd.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Table " + tableName + " generating fail!" + ex);
            }
        }

        public static gVariable.dispatchSheetStruct getDispatchByID(string databaseName, string tableName, int id)
        {
            MySqlDataReader myReader;
            gVariable.dispatchSheetStruct dispatchList = new gVariable.dispatchSheetStruct();

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "select * from `" + tableName.ToLower() + "`" + " where id = " + "'" + id + "'";

                myReader = myCommand.ExecuteReader();
                if (myReader.Read())
                {
                    //We don't need record id, so jump over id
                    if (!myReader.IsDBNull(MACHINE_ID_IN_DISPATCHLIST_DATABASE))
                        dispatchList.machineID = myReader.GetString(MACHINE_ID_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(DISPATCH_CODE_IN_DISPATCHLIST_DATABASE))
                        dispatchList.dispatchCode = myReader.GetString(DISPATCH_CODE_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(PLANNED_STARTTIME_IN_DISPATCHLIST_DATABASE))
                        dispatchList.planTime1 = myReader.GetString(PLANNED_STARTTIME_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(PLANNED_COMPLETETIME_IN_DISPATCHLIST_DATABASE))
                        dispatchList.planTime2 = myReader.GetString(PLANNED_COMPLETETIME_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(PRODUCT_CODE_IN_DISPATCHLIST_DATABASE))
                        dispatchList.productCode = myReader.GetString(PRODUCT_CODE_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(PRODUCT_NAME_IN_DISPATCHLIST_DATABASE))
                        dispatchList.productName = myReader.GetString(PRODUCT_NAME_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(OPERATOR_NAME_IN_DISPATCHLIST_DATABASE))
                        dispatchList.operatorName = myReader.GetString(OPERATOR_NAME_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(FORCAST_NUM_IN_DISPATCHLIST_DATABASE))
                        dispatchList.plannedNumber = Convert.ToInt32(myReader.GetString(FORCAST_NUM_IN_DISPATCHLIST_DATABASE));

                    if (!myReader.IsDBNull(RECEIVE_NUM_IN_DISPATCHLIST_DATABASE))
                        dispatchList.outputNumber = Convert.ToInt32(myReader.GetString(RECEIVE_NUM_IN_DISPATCHLIST_DATABASE));

                    if (!myReader.IsDBNull(QUALIFIED_NUM_IN_DISPATCHLIST_DATABASE))
                        dispatchList.qualifiedNumber = Convert.ToInt32(myReader.GetString(QUALIFIED_NUM_IN_DISPATCHLIST_DATABASE));

                    if (!myReader.IsDBNull(UNQUALIFIED_NUM_IN_DISPATCHLIST_DATABASE))
                        dispatchList.unqualifiedNumber = Convert.ToInt32(myReader.GetString(UNQUALIFIED_NUM_IN_DISPATCHLIST_DATABASE));

                    if (!myReader.IsDBNull(PROCESS_NAME_IN_DISPATCHLIST_DATABASE))
                        dispatchList.processName = myReader.GetString(PROCESS_NAME_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(START_TIME_IN_DISPATCHLIST_DATABASE))
                        dispatchList.realStartTime = myReader.GetString(START_TIME_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(COMPLETE_TIME_IN_DISPATCHLIST_DATABASE))
                        dispatchList.realFinishTime = myReader.GetString(COMPLETE_TIME_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(PREPARE_TIME_IN_DISPATCHLIST_DATABASE))
                        dispatchList.prepareTimePoint = myReader.GetString(PREPARE_TIME_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(STATUS_IN_DISPATCHLIST_DATABASE))
                        dispatchList.status = Convert.ToInt32(myReader.GetString(STATUS_IN_DISPATCHLIST_DATABASE));

                    if (!myReader.IsDBNull(EQUIPMENT_LIFETIME_IN_DISPATCHLIST_DATABASE))
                        dispatchList.toolLifeTimes = Convert.ToInt32(myReader.GetString(EQUIPMENT_LIFETIME_IN_DISPATCHLIST_DATABASE));

                    if (!myReader.IsDBNull(EQUIPMENT_USEDTIME_IN_DISPATCHLIST_DATABASE))
                        dispatchList.toolUsedTimes = Convert.ToInt32(myReader.GetString(EQUIPMENT_USEDTIME_IN_DISPATCHLIST_DATABASE));

                    if (!myReader.IsDBNull(OUTPUT_RATIO_IN_DISPATCHLIST_DATABASE))
                        dispatchList.outputRatio = Convert.ToInt32(myReader.GetString(OUTPUT_RATIO_IN_DISPATCHLIST_DATABASE));

                    if (!myReader.IsDBNull(SERIAL_NO_IN_DISPATCHLIST_DATABASE))
                        dispatchList.serialNumber = myReader.GetString(SERIAL_NO_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(REPORTOR_IN_DISPATCHLIST_DATABASE))
                        dispatchList.reportor = myReader.GetString(REPORTOR_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(WORKSHOP_IN_DISPATCHLIST_DATABASE))
                        dispatchList.workshop = myReader.GetString(WORKSHOP_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(WORK_SHIFT_IN_DISPATCHLIST_DATABASE))
                        dispatchList.workshift = myReader.GetString(WORK_SHIFT_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(SALESORDER_CODE_IN_DISPATCHLIST_DATABASE))
                        dispatchList.salesOrderCode = myReader.GetString(SALESORDER_CODE_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(BOM_CODE_IN_DISPATCHLIST_DATABASE))
                        dispatchList.BOMCode = myReader.GetString(BOM_CODE_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(CUSTOMER_IN_DISPATCHLIST_DATABASE))
                        dispatchList.customer = myReader.GetString(CUSTOMER_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(MULTI_PRODUCT_IN_DISPATCHLIST_DATABASE))
                        dispatchList.multiProduct = myReader.GetString(MULTI_PRODUCT_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(PRODUCT_CODE2_IN_DISPATCHLIST_DATABASE))
                        dispatchList.productCode2 = myReader.GetString(PRODUCT_CODE2_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(PRODUCT_CODE3_IN_DISPATCHLIST_DATABASE))
                        dispatchList.productCode3 = myReader.GetString(PRODUCT_CODE3_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(OPERATOR2_NAME_IN_DISPATCHLIST_DATABASE))
                        dispatchList.operatorName2 = myReader.GetString(OPERATOR2_NAME_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(OPERATOR3_NAME_IN_DISPATCHLIST_DATABASE))
                        dispatchList.operatorName3 = myReader.GetString(OPERATOR3_NAME_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(BATCH_NUMBER_IN_DISPATCHLIST_DATABASE))
                        dispatchList.batchNum = myReader.GetString(BATCH_NUMBER_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(PRODUCT_COLOR_IN_DISPATCHLIST_DATABASE))
                        dispatchList.productColor = myReader.GetString(PRODUCT_COLOR_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(RAW_MATERIAL_IN_DISPATCHLIST_DATABASE))
                        dispatchList.rawMateialcode = myReader.GetString(RAW_MATERIAL_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(PRODUCT_LENGTH_IN_DISPATCHLIST_DATABASE))
                        dispatchList.productLength = myReader.GetString(PRODUCT_LENGTH_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(PRODUCT_DIAMETER_IN_DISPATCHLIST_DATABASE))
                        dispatchList.productDiameter = myReader.GetString(PRODUCT_DIAMETER_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(PRODUCT_WEIGHT_IN_DISPATCHLIST_DATABASE))
                        dispatchList.productWeight = myReader.GetString(PRODUCT_WEIGHT_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(SLIT_WIDTH_IN_DISPATCHLIST_DATABASE))
                        dispatchList.slitWidth = myReader.GetString(SLIT_WIDTH_IN_DISPATCHLIST_DATABASE);

                    if (!myReader.IsDBNull(PRINT_SIDE_IN_DISPATCHLIST_DATABASE))
                        dispatchList.printSide = myReader.GetString(PRINT_SIDE_IN_DISPATCHLIST_DATABASE);
                }
                myReader.Close();
                myConnection.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "table:" + tableName + "getDispatchByID() failed for ID of " + id + "! " + ex);
            }
            return dispatchList;
        }

        public static gVariable.dispatchSheetStruct[] getDispatchListByCommand(string databaseName, string tableName, string commandText)
        {
            int dispatchNum;
            string text;

            MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
            myConnection.Open();

            MySqlCommand myCommand = myConnection.CreateCommand();

            myCommand.CommandText = "select count(*) from `" + tableName + "`" + commandText;
            dispatchNum = Convert.ToInt32(myCommand.ExecuteScalar());

            text = "select * from `" + tableName + "`" + commandText;
            return getDispatchListInternal(databaseName, tableName, text, dispatchNum);
        }


        //list dispatches between time1(start) and time2(end)
        //timeCheckType:
        //TIME_CHECK_TYPE_PLANNED_START = 0;
        //TIME_CHECK_TYPE_PLANNED_FINISH = 1;
        //TIME_CHECK_TYPE_REAL_START = 2;
        //TIME_CHECK_TYPE_REAL_FINISH = 3;
        //TIME_CHECK_TYPE_REAL_START_FINISH = 4;
        //TIME_CHECK_TYPE_REAL_ONE_POINT = 5;
        public static gVariable.dispatchSheetStruct[] getDispatchListInPeriodOfTime(string databaseName, string tableName, string time1, string time2, int dispatchStatus, int timeCheckType)
        {
            int dispatchNum;
            //int index;
            string commandText;
            string statusStr;

            dispatchNum = 0;
            commandText = null;
            try
            {
                if(dispatchStatus == gVariable.MACHINE_STATUS_DISPATCH_UNPUBLISHED)
                {
                    statusStr = " and status <= '" + dispatchStatus + "'";
                }
                else if (dispatchStatus != gVariable.MACHINE_STATUS_DISPATCH_ALL)
                {
                    statusStr = " and status = '" + dispatchStatus + "'";
                }
                else
                {
                    statusStr = null;
                }

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                if (timeCheckType == gVariable.TIME_CHECK_TYPE_PLANNED_START)
                {
                    myCommand.CommandText = "select count(*) from `" + tableName + "`" + " where planTime1 >= " + "'" + time1 + "' and planTime2 <= " + "'" + time2 + "'" + statusStr;
                    dispatchNum = Convert.ToInt32(myCommand.ExecuteScalar());

                    commandText = "select * from `" + tableName.ToLower() + "`" + " where planTime1 >= " + "'" + time1 + "' and planTime2 <= " + "'" + time2 + "'" + statusStr;
                }
                else if (timeCheckType == gVariable.TIME_CHECK_TYPE_REAL_START)
                {
                    myCommand.CommandText = "select count(*) from `" + tableName + "`" + " where startTime >= " + "'" + time1 + "' and startTime <= " + "'" + time2 + "'" + statusStr;
                    dispatchNum = Convert.ToInt32(myCommand.ExecuteScalar());

                    commandText = "select * from `" + tableName.ToLower() + "`" + " where startTime >= " + "'" + time1 + "' and startTime <= " + "'" + time2 + "'" + statusStr;
                }
                else if (timeCheckType == gVariable.TIME_CHECK_TYPE_REAL_FINISH)
                {
                    myCommand.CommandText = "select count(*) from `" + tableName + "`" + " where completeTime >= " + "'" + time1 + "' and completeTime <= " + "'" + time2 + "'" + statusStr;
                    dispatchNum = Convert.ToInt32(myCommand.ExecuteScalar());

                    commandText = "select * from `" + tableName.ToLower() + "`" + " where completeTime >= " + "'" + time1 + "' and completeTime <= " + "'" + time2 + "'" + statusStr;
                }
                else if (timeCheckType == gVariable.TIME_CHECK_TYPE_REAL_START_FINISH)
                {
                    myCommand.CommandText = "select count(*) from `" + tableName + "`" + " where completeTime >= " + "'" + time1 + "' and startTime <= " + "'" + time2 + "'" + statusStr;
                    dispatchNum = Convert.ToInt32(myCommand.ExecuteScalar());

                    commandText = "select * from `" + tableName.ToLower() + "`" + " where completeTime >= " + "'" + time1 + "' and startTime <= " + "'" + time2 + "'" + statusStr;
                }
                else if (timeCheckType == gVariable.TIME_CHECK_TYPE_REAL_ONE_POINT)
                {
                    myCommand.CommandText = "select count(*) from `" + tableName + "`" + " where startTime <= " + "'" + time1 + "' and completeTime >= " + "'" + time2 + "'" + statusStr;
                    dispatchNum = Convert.ToInt32(myCommand.ExecuteScalar());

                    commandText = "select * from `" + tableName.ToLower() + "`" + " where startTime <= " + "'" + time1 + "' and completeTime >= " + "'" + time2 + "'" + statusStr;
                }

                myConnection.Close();

                if (dispatchNum == 0)
                {
                    return null;
                }

                return getDispatchListInternal(databaseName, tableName, commandText, dispatchNum);
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "table:" + tableName + "getDispatchListInPeriodOfTime() failed between time " + time1 + " and time " + time2 + "! " + ex);
                return null;
            }
        }


        public static gVariable.dispatchSheetStruct[] getDispatchListInternal(string databaseName, string tableName, string commandText, int dispatchNum)
        {
            int index;
            const int MAX_DISPATCH_NUM = 5000;  //we only display 1000 dispatches at most
            MySqlDataReader myReader;
            gVariable.dispatchSheetStruct[] dispatchList; // = new gVariable.dispatchSheetStruct[MAX_DISPATCH_NUM];

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();
                myCommand.CommandText = commandText;

                dispatchList = new gVariable.dispatchSheetStruct[dispatchNum];

                index = 0;
                myReader = myCommand.ExecuteReader();
                if (myReader != null)
                {
                    while (true)
                    {
                        if (!myReader.Read()) //no more data in this table
                        {
                            break;
                        }
                        else
                        {
//                            dispatchList[index] = new gVariable.dispatchSheetStruct();

                            //We don't need record id, so jump over id
                            if (!myReader.IsDBNull(MACHINE_ID_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].machineID = myReader.GetString(MACHINE_ID_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(DISPATCH_CODE_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].dispatchCode = myReader.GetString(DISPATCH_CODE_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(PLANNED_STARTTIME_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].planTime1 = myReader.GetString(PLANNED_STARTTIME_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(PLANNED_COMPLETETIME_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].planTime2 = myReader.GetString(PLANNED_COMPLETETIME_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(PRODUCT_CODE_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].productCode = myReader.GetString(PRODUCT_CODE_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(PRODUCT_NAME_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].productName = myReader.GetString(PRODUCT_NAME_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(OPERATOR_NAME_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].operatorName = myReader.GetString(OPERATOR_NAME_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(FORCAST_NUM_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].plannedNumber = Convert.ToInt32(myReader.GetString(FORCAST_NUM_IN_DISPATCHLIST_DATABASE));

                            if (!myReader.IsDBNull(RECEIVE_NUM_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].outputNumber = Convert.ToInt32(myReader.GetString(RECEIVE_NUM_IN_DISPATCHLIST_DATABASE));

                            if (!myReader.IsDBNull(QUALIFIED_NUM_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].qualifiedNumber = Convert.ToInt32(myReader.GetString(QUALIFIED_NUM_IN_DISPATCHLIST_DATABASE));

                            if (!myReader.IsDBNull(UNQUALIFIED_NUM_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].unqualifiedNumber = Convert.ToInt32(myReader.GetString(UNQUALIFIED_NUM_IN_DISPATCHLIST_DATABASE));

                            if (!myReader.IsDBNull(PROCESS_NAME_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].processName = myReader.GetString(PROCESS_NAME_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(START_TIME_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].realStartTime = myReader.GetString(START_TIME_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(COMPLETE_TIME_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].realFinishTime = myReader.GetString(COMPLETE_TIME_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(PREPARE_TIME_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].prepareTimePoint = myReader.GetString(PREPARE_TIME_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(STATUS_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].status = Convert.ToInt32(myReader.GetString(STATUS_IN_DISPATCHLIST_DATABASE));

                            if (!myReader.IsDBNull(EQUIPMENT_LIFETIME_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].toolLifeTimes = Convert.ToInt32(myReader.GetString(EQUIPMENT_LIFETIME_IN_DISPATCHLIST_DATABASE));

                            if (!myReader.IsDBNull(EQUIPMENT_USEDTIME_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].toolUsedTimes = Convert.ToInt32(myReader.GetString(EQUIPMENT_USEDTIME_IN_DISPATCHLIST_DATABASE));

                            if (!myReader.IsDBNull(OUTPUT_RATIO_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].outputRatio = Convert.ToInt32(myReader.GetString(OUTPUT_RATIO_IN_DISPATCHLIST_DATABASE));

                            if (!myReader.IsDBNull(SERIAL_NO_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].serialNumber = myReader.GetString(SERIAL_NO_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(REPORTOR_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].reportor = myReader.GetString(REPORTOR_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(WORKSHOP_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].workshop = myReader.GetString(WORKSHOP_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(WORK_SHIFT_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].workshift = myReader.GetString(WORK_SHIFT_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(SALESORDER_CODE_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].salesOrderCode = myReader.GetString(SALESORDER_CODE_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(BOM_CODE_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].BOMCode = myReader.GetString(BOM_CODE_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(CUSTOMER_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].customer = myReader.GetString(CUSTOMER_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(MULTI_PRODUCT_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].multiProduct = myReader.GetString(MULTI_PRODUCT_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(PRODUCT_CODE2_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].productCode2 = myReader.GetString(PRODUCT_CODE2_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(PRODUCT_CODE3_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].productCode3 = myReader.GetString(PRODUCT_CODE3_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(OPERATOR2_NAME_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].operatorName2 = myReader.GetString(OPERATOR2_NAME_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(OPERATOR3_NAME_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].operatorName3 = myReader.GetString(OPERATOR3_NAME_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(BATCH_NUMBER_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].batchNum = myReader.GetString(BATCH_NUMBER_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(PRODUCT_COLOR_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].productColor = myReader.GetString(PRODUCT_COLOR_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(RAW_MATERIAL_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].rawMateialcode = myReader.GetString(RAW_MATERIAL_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(PRODUCT_LENGTH_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].productLength = myReader.GetString(PRODUCT_LENGTH_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(PRODUCT_DIAMETER_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].productDiameter = myReader.GetString(PRODUCT_DIAMETER_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(PRODUCT_WEIGHT_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].productWeight = myReader.GetString(PRODUCT_WEIGHT_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(SLIT_WIDTH_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].slitWidth = myReader.GetString(SLIT_WIDTH_IN_DISPATCHLIST_DATABASE);

                            if (!myReader.IsDBNull(PRINT_SIDE_IN_DISPATCHLIST_DATABASE))
                                dispatchList[index].printSide = myReader.GetString(PRINT_SIDE_IN_DISPATCHLIST_DATABASE);

                            index++;

                            if (index >= dispatchNum)
                                break;

                            if (index > MAX_DISPATCH_NUM)
                                break;
                        }
                    }
                }
                myReader.Close();
                myConnection.Close();

                if (index == 0)
                    return null;
                else
                    return dispatchList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "table:" + tableName + "getDispatchListInternal() failed ! " + ex);
                return null;
            }
        }

        public static int readMachineStatusForOneDay(String databaseName, String tableName, int time1, int time2)
        {
            int i, j;
            int index;
            int time;

            i = 0;
            try
            {
                index = toolClass.getBoardIndexByDatabaseName(databaseName);

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                myCommand.CommandText = "select * from `" + tableName + "` where time > " + time1 + " and time < " + time2;

                MySqlDataReader myReader = myCommand.ExecuteReader();

                if (myReader.Read())
                {
                    //if we record current(amphere) value from a certain time point(not from 0:00), we need to put data in currentStatusForOneDay[] before this time point to 0
                    time = (Convert.ToInt32(myReader.GetString(TIME_VALUE_IN_DATA_DATABASE)));
                    if (time < time1 || time > time2)
                        return -1;

                    i = (time - time1) / (60 * gVariable.onePointstandForHowManyMinutes);
                    for (j = 0; j < i; j++)
                        gVariable.currentStatusForOneDay[j] = 0;

                    gVariable.currentStatusForOneDay[i++] = Convert.ToInt32(myReader.GetString(DATA_VALUE1_IN_DATA_DATABASE));

                }

                while (myReader.Read())
                {
                    gVariable.currentStatusForOneDay[i] = Convert.ToInt32(myReader.GetString(DATA_VALUE1_IN_DATA_DATABASE));
                    gVariable.dispatchAlarmIDForOneDay[index, i++] = Convert.ToInt32(myReader.GetString(DATA_VALUE2_IN_DATA_DATABASE));
                }
                myReader.Close();
                myConnection.Close();

                return i;
            }
            catch (Exception ex)
            {
                Console.WriteLine("readFloatFromTableForToday database " + databaseName + tableName + " failed!" + ex);
                return -1;
            }
        }

        //not used for the time being
        public static int readFloatFromTableForToday(String databaseName, String tableName, int time1, int time2)
        {
            int i, j;
            int time;

            i = 0;
            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                myCommand.CommandText = "select * from `" + tableName + "` where time > " + time1 +" and time < " + time2;

                MySqlDataReader myReader = myCommand.ExecuteReader();

                if (myReader.Read())
                {
                    //if we record current(amphere) value from a certain time point(not from 0:00), we need to put data in currentStatusForOneDay[] before this time point to 0
                    time = (Convert.ToInt32(myReader.GetString(TIME_VALUE_IN_DATA_DATABASE)));
                    i = (time - time1) / (60 * gVariable.onePointstandForHowManyMinutes);
                    for (j = 0; j < i; j++)
                        gVariable.currentStatusForOneDay[j] = 0;

//                    gVariable.currentStatusForOneDay[i++] = (float)(Convert.ToDouble(myReader.GetString(DATA_VALUE1_IN_DATA_DATABASE)));
                }

//                while (myReader.Read())
//                    gVariable.currentStatusForOneDay[i++] = (float)(Convert.ToDouble(myReader.GetString(DATA_VALUE1_IN_DATA_DATABASE)));

                myReader.Close();
                myConnection.Close();

                return i;
            }
            catch (Exception ex)
            {
                Console.WriteLine("readFloatFromTableForToday database " + databaseName + tableName + " failed!" + ex);
                return -1;
            }
        }

        public static int writeTwoIntToTable(String databaseName, String tableName, int time, int value1, int value2)
        {
            try
            {
                lock (writeOneFloatLocker)
                {
                    //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                    String insertString = "insert into `" + tableName.ToLower() + "` value(@id, @time, @value1, @value2)";

                    MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                    myConnection.Open();

                    MySqlCommand myCommand = myConnection.CreateCommand();

                    myCommand.CommandText = insertString;

                    myCommand.Parameters.AddWithValue("@id", 0);
                    myCommand.Parameters.AddWithValue("@time", time);
                    myCommand.Parameters.AddWithValue("@value1", value1);
                    myCommand.Parameters.AddWithValue("@value2", value2);

                    myCommand.ExecuteNonQuery();

                    myConnection.Close();

                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("writeOneIntTable database " + databaseName + tableName + " failed!" + ex);
                return -1;
            }
        }


        public static int writeOneFloatToTable(String databaseName, String tableName, int time, float f)
        {
            try
            {
                lock (writeOneFloatLocker)
                {
                    //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                    String insertString = "insert into `" + tableName.ToLower() + "` value(@id, @time, @value)";

                    MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                    myConnection.Open();

                    MySqlCommand myCommand = myConnection.CreateCommand();

                    myCommand.CommandText = insertString;

                    myCommand.Parameters.AddWithValue("@id", 0);
                    myCommand.Parameters.AddWithValue("@time", time);
                    myCommand.Parameters.AddWithValue("@value", f);

                    myCommand.ExecuteNonQuery();

                    myConnection.Close();

                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("writeOneFloatToTable database " + databaseName + tableName + " failed!" + ex);
                return -1;
            }
        }


        public static int writeMultipleFloatToTable(String databaseName, String tableName, int type, int time, float[] value, int len, string sn)
        {
            int i, j;
            int ret;
            MySqlParameter[] param;

            try
            {
                lock (writeFloatLocker)
                {
                    if (len > tableDataNumMax[type])
                        len = tableDataNumMax[type];

                    //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                    String commandText = "insert into `" + tableName.ToLower() + "`" + insertDataTableString[type];

                    if (type == DATA_TYPE_QUALITY_DATA)  //for quality data, we need to record 1 serial number and 16 status value, status means whether this value is legel or out of spec
                        param = new MySqlParameter[tableDataNumMax[type] + 3 + 16];
                    else
                        param = new MySqlParameter[tableDataNumMax[type] + 2];

                    param[0] = new MySqlParameter("@id", 0);
                    param[1] = new MySqlParameter("@time", time);

                    for (i = 0; i < len; i++)
                        param[i + 2] = new MySqlParameter("@value" + (i + 1), value[i]);

                    for (; i < tableDataNumMax[type]; i++)
                        param[i + 2] = new MySqlParameter("@value" + (i + 1), -1);

                    if (type == DATA_TYPE_QUALITY_DATA)  //for quality data, we need to record serial number and status value, status means whether this value is legel or out of spec
                    {
                        param[i + 2] = new MySqlParameter("@sn", sn);

                        i++;
                        for (j = 0; j < 16; j++)
                        {
                            param[i + j + 2] = new MySqlParameter("@status" + (j + 1), (object)0);  //I think this is a bug of mySQL, 0 will be regarded as MySqlDbType type numeration
                        }
                    }

                    //return the ID of the appended record
                    ret = databaseNonQueryAction(databaseName, commandText, param, gVariable.appendRecord);

                    return ret;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("writeMultipleFloatToTable database " + databaseName + tableNameList[type] + tableName + " failed!" + ex);
                return -1;
            }
        }


        //for all kinds of alarm, we will save an alarm in 2 places, one inside the alarm table of a machine, the other one is in a alarm table for the whole factory
        //then in the future when we update the status of an alarm, we will only update the table in machine, not factory alarm table
        //so when we want to get an alarm list for the whole fatctory through a period of time, we can get it from factory alarm table, but if we want to know the details of an alarm,
        //we need to check for alarm content in machine alarm table
        //type:device alarm/material alarm/craft data alarm/Quality data alarm
        //category: for craft/quality alarm, data overflow or other SPC error index (5 judgment conditions)
        //inHistory: whether this alarm is inside history review lib
        //startID: for SPC errors, we need to check for totalPointNumForNoSPCChart(125) number of data, where is the start ID for these data in craft/quality table  
        //indexInTable: this alarm comes from which item in craft/quality tablelist
        //return: ID of the new generated alarm in alarm list table for this machine
        public static int writeAlarmTable(string databaseName, string tableName, gVariable.alarmTableStruct alarmTableStructImpl)
        {
            int index;
            int machineIndex;
            int alarmIDInTable;  //this value means the ID in machine alarm table
            int itemNum;
            string insertString;
            string[] itemName;

            insertString = null;
            try
            {
                machineIndex = Convert.ToInt16(databaseName.Remove(0, 1)) - 1;

                itemNum = getDatabaseInsertStringFromExcel(ref insertString, gVariable.alarmListFileName);
                if (itemNum < 0)
                {
                    Console.WriteLine("writeAlarmTable failed, since excel file has problem!");
                    return -1; //failed to get insert string
                }
                index = 0;
                itemName = insertString.Remove(0, 11).Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + tableName.ToLower() + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.errorDesc);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.dispatchCode);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.alarmFailureCode);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.feedBinID);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.machineName);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.operatorName);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.time);
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.type);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.category);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.status);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.ALARM_INHISTORY_FALSE); //this alarm is not inside history list since it is just generated
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.startID);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.indexInTable);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.workshop);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.mailList);
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], "");

                myCommand.ExecuteNonQuery();

                gVariable.IDForLastAlarmByMachine[machineIndex] = (int)myCommand.LastInsertedId;
                alarmIDInTable = (int)myCommand.LastInsertedId;

                myConnection.Close();

                index = 0;
                myConnection = new MySqlConnection("database = " + gVariable.globalDatabaseName + "; " + connectionString);
                myConnection.Open();

                myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + gVariable.globalAlarmListTableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.errorDesc);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.dispatchCode);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.alarmFailureCode);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.feedBinID);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.machineName);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.operatorName);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.time);
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.type);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.category);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.status);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.ALARM_INHISTORY_FALSE); //this alarm is not inside history list since it is just generated
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.startID);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.indexInTable);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.workshop);
                myCommand.Parameters.AddWithValue(itemName[index++], alarmTableStructImpl.mailList);
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], "");

                myCommand.ExecuteNonQuery();

                gVariable.IDForLastAlarmByFactory[machineIndex] = (int)myCommand.LastInsertedId;
                myConnection.Close();

                return alarmIDInTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "Device alarm table " + tableName + " write fail!" + ex);
                return -1;
            }
        }



        public static int getRecordNumInTable(String databaseName, String tableName)
        {
            int num;

            num = 0;
            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                myCommand.CommandText = "select count(*) from `" + tableName + "`";
                num = Convert.ToInt32(myCommand.ExecuteScalar());

                myConnection.Close();

                return num;
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "GetRecordNumInTable " + tableName + " failed!" + ex);
                return -1;
            }
        }

        public static int getNumOfRecordByCondition(string databaseName, string commandText)
        {
            int num;

            try
            {
                lock (numOfAlarmForTodayLocker)
                {
                    MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                    myConnection.Open();

                    MySqlCommand myCommand = myConnection.CreateCommand();

                    myCommand.CommandText = commandText;

                    num = Convert.ToInt32(myCommand.ExecuteScalar());

                    myConnection.Close();

                    return num;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("getNumOfRecordByCondition() failed in " + databaseName + " by " + commandText + "! " + ex);

                return 0;
            }
        }

        //timeArray[0] is type and id for the first alarm between time1 and time2, type * 0x1000000 + id 
        //timeArray[1] is trigger time for the first alarm brtween time1 and time2, triggered in which minute start from 00:00:00 
        //timeArray[2] is complete time for the first alarm brtween time1 and time2, triggered in which minute start from 00:00:00 
        //timeArray[3] is type for the second alarm brtween time1 and time2 
        //timeArray[4] is trigger time for the second alarm brtween time1 and time2, triggered in which minute start from 00:00:00 
        //timeArray[5] is complete time for the second alarm brtween time1 and time2, triggered in which minute start from 00:00:00 
        public static int getAlarmTimeFrameForPeriodOfTime(string databaseName, string tableName, string time1, string time2, ref int[] timeArray)
        {
            int num;
            int timeInt;
            int time1Int, time2Int;
            string time, time3;

            num = 0;
            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                myCommand.CommandText = "select * from `" + tableName + "` where time <= '" + time2 + "' and time2 >= '" + time1 +"'";

                MySqlDataReader myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    timeArray[num * 3] = myReader.GetInt16(TYPE_IN_ALARM_DATABASE) * 0x1000000 + myReader.GetInt16(ID_VALUE_IN_ALARM_DATABASE);
                    time = myReader.GetString(TIME_IN_ALARM_DATABASE);
                    timeInt = toolClass.timeStringToTimeStamp(time);
                    time1Int = toolClass.timeStringToTimeStamp(time1);
                    time2Int = toolClass.timeStringToTimeStamp(time2);
                    if (timeInt >= time1Int)
                        timeArray[num * 3 + 1] = (timeInt - time1Int) / (60 / gVariable.onePointstandForHowManyMinutes);
                    else
                        continue;

                    time3 = myReader.GetString(TIME2_IN_ALARM_DATABASE);
                    if (time3 == null || time3 == "")
                    {
                        timeInt = toolClass.ConvertDateTimeInt(DateTime.Now);
                        if(timeInt > time2Int)
                            timeArray[num * 3 + 2] = (time2Int - time1Int) / ( 60 / gVariable.onePointstandForHowManyMinutes);
                        else
                            timeArray[num * 3 + 2] = (timeInt - time1Int) / (60 / gVariable.onePointstandForHowManyMinutes);
                    }
                    else
                    {
                        timeArray[num * 3 + 2] = (toolClass.timeStringToTimeStamp(time3) - time1Int) / 60;
                    }
                    num++;
                    if (num > 1000)  //if alarm exceeds 2000, just ignore it
                        break;
                }

                myReader.Close();
                myConnection.Close();

                return num;
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "Device alarm table " + tableName + " read fail!" + ex);
                return -1;
            }
        }

        //get alarm content
        //type:device alarm/material alarm/craft data alarm/Quality data alarm
        //category: for craft/quality alarm, data overflow or other SPC error index (5 judgment conditions)
        //inHistory: whether this alarm is inside history review lib
        //startID: for SPC errors, we need to check for totalPointNumForNoSPCChart(125) number of data, where is the start ID for these data in craft/quality table  
        //indexInTable: this alarm comes from which item in craft/quality tablelist
        //return: alarm table, if this alarm id doesnot exist, table.dispatchCode will be null
        public static gVariable.alarmTableStruct getAlarmTableContent(string databaseName, string tableName, int id) 
        {
            gVariable.alarmTableStruct alarmTableStructImpl;

            alarmTableStructImpl = new gVariable.alarmTableStruct();
            alarmTableStructImpl.dispatchCode = null;
            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                myCommand.CommandText = "select * from `" + tableName + "` where id = " + id;

                MySqlDataReader myReader = myCommand.ExecuteReader();

                if (myReader.Read())
                {
                    alarmTableStructImpl.errorDesc = myReader.GetString(ERROR_DESC_IN_ALARM_DATABASE);
                    if (!myReader.IsDBNull(DISPATCH_CODE_IN_ALARM_DATABASE))
                        alarmTableStructImpl.dispatchCode = myReader.GetString(DISPATCH_CODE_IN_ALARM_DATABASE);
                    if (!myReader.IsDBNull(ALARM_FAIL_NO_IN_ALARM_DATABASE))
                        alarmTableStructImpl.alarmFailureCode = myReader.GetString(ALARM_FAIL_NO_IN_ALARM_DATABASE);
                    if (!myReader.IsDBNull(FEED_BIN_ID_IN_ALARM_DATABASE))
                        alarmTableStructImpl.feedBinID = myReader.GetString(FEED_BIN_ID_IN_ALARM_DATABASE);
                    if (!myReader.IsDBNull(MACHINE_NAME_IN_ALARM_DATABASE))
                        alarmTableStructImpl.machineName = myReader.GetString(MACHINE_NAME_IN_ALARM_DATABASE);
                    if (!myReader.IsDBNull(OPERATOR_NAME_IN_ALARM_DATABASE))
                        alarmTableStructImpl.operatorName = myReader.GetString(OPERATOR_NAME_IN_ALARM_DATABASE);
                    alarmTableStructImpl.time = myReader.GetString(TIME_IN_ALARM_DATABASE);
                    alarmTableStructImpl.signer = myReader.GetString(SIGNER_IN_ALARM_DATABASE);
                    alarmTableStructImpl.time1 = myReader.GetString(TIME1_IN_ALARM_DATABASE);
                    alarmTableStructImpl.completer = myReader.GetString(COMPLETER_IN_ALARM_DATABASE);
                    alarmTableStructImpl.time2 = myReader.GetString(TIME2_IN_ALARM_DATABASE);
                    alarmTableStructImpl.type = myReader.GetInt16(TYPE_IN_ALARM_DATABASE);
                    alarmTableStructImpl.category = myReader.GetInt16(CATEGORY_IN_ALARM_DATABASE);
                    alarmTableStructImpl.status = myReader.GetInt16(STATUS_IN_ALARM_DATABASE);
                    alarmTableStructImpl.inHistory = myReader.GetInt16(INHISTORY_IN_ALARM_DATABASE);
                    alarmTableStructImpl.startID = myReader.GetInt32(STARTID_IN_ALARM_DATABASE);
                    alarmTableStructImpl.indexInTable = myReader.GetInt32(INDEX_TABLE_IN_ALARM_DATABASE);
                    if (!myReader.IsDBNull(WORKSHOP_IN_ALARM_DATABASE))
                        alarmTableStructImpl.workshop = myReader.GetString(WORKSHOP_IN_ALARM_DATABASE);
                    if (!myReader.IsDBNull(MAILLIST_IN_ALARM_DATABASE))
                        alarmTableStructImpl.mailList = myReader.GetString(MAILLIST_IN_ALARM_DATABASE);
                    if (!myReader.IsDBNull(DISCUSS_IN_ALARM_DATABASE))
                        alarmTableStructImpl.discuss = myReader.GetString(DISCUSS_IN_ALARM_DATABASE);
                    if (!myReader.IsDBNull(SOLUTION_IN_ALARM_DATABASE))
                        alarmTableStructImpl.solution = myReader.GetString(SOLUTION_IN_ALARM_DATABASE);
                }

                myReader.Close();
                myConnection.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "Device alarm table " + tableName + " read fail!" + ex);
            }
            return alarmTableStructImpl;
        }

        //get alarm info for alarms that in history table including alarm ID/failureNo/desc/discussion/solution
        public static int getAlarmContentInHistoryTable(string databaseName, string tableName, int indexInTable, int category, ref int[] alarmIDArray, ref string[] alarmFailureCodeArray, ref string[] errDescArray, ref string[] discussArray, ref string[] solutionArray)
        {
            int i;

            try
            {
                i = 0;

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                myCommand.CommandText = "select * from `" + tableName + "` where inHistory = " + 1 + " and category = " + category + " and indexInTable = " + indexInTable + " order by id desc";

                MySqlDataReader myReader = myCommand.ExecuteReader();

                while(myReader.Read())
                {
                    alarmIDArray[i] = myReader.GetInt16(ID_VALUE_IN_ALARM_DATABASE);
                    if (!myReader.IsDBNull(ALARM_FAIL_NO_IN_ALARM_DATABASE))
                        alarmFailureCodeArray[i] = myReader.GetString(ALARM_FAIL_NO_IN_ALARM_DATABASE);
                    if (!myReader.IsDBNull(ERROR_DESC_IN_ALARM_DATABASE))
                        errDescArray[i] = myReader.GetString(ERROR_DESC_IN_ALARM_DATABASE);
                    if (!myReader.IsDBNull(DISCUSS_IN_ALARM_DATABASE))
                        discussArray[i] = myReader.GetString(DISCUSS_IN_ALARM_DATABASE);
                    if (!myReader.IsDBNull(SOLUTION_IN_ALARM_DATABASE))
                        solutionArray[i] = myReader.GetString(SOLUTION_IN_ALARM_DATABASE);
                    i++;

                    //we cannot support more than this number
                    if (i >= gVariable.MAX_ALARM_NUM_ONE_CATEGORY_IN_HISTORY)
                        break;
                }

                myReader.Close();
                myConnection.Close();

                return i;
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "Device alarm table " + tableName + " read fail!" + ex);
                return -1;
            }
        }


        //the only thing we can change for an alarm is status/discuss
        /*
        public static void updateAlarmTable(String databaseName, String tableName, int id, string signer, string time1, string completer, string time2, int status, int inHistory, string mailList, string discuss, string solution)
        {
            int i;
            string str;
            String updateString;

            updateString = null;
            try
            {
                for (i = 0; i < 2; i++)
                {
                    MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                    myConnection.Open();

                    MySqlCommand myCommand = myConnection.CreateCommand();

                    //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                    if (signer != null)
                    {
                        updateString = "update `" + tableName + "` set signer = '" + signer + "' where id = '" + id + "'";
                        myCommand.CommandText = updateString;
                        myCommand.ExecuteNonQuery();
                    }

                    if (time1 != null)
                    {
                        updateString = "update `" + tableName + "` set time1 = '" + time1 + "' where id = '" + id + "'";
                        myCommand.CommandText = updateString;
                        myCommand.ExecuteNonQuery();
                    }

                    if (completer != null)
                    {
                        updateString = "update `" + tableName + "` set completer = '" + completer + "' where id = '" + id + "'";
                        myCommand.CommandText = updateString;
                        myCommand.ExecuteNonQuery();
                    }

                    if (time2 != null)
                    {
                        updateString = "update `" + tableName + "` set time2 = '" + time2 + "' where id = '" + id + "'";
                        myCommand.CommandText = updateString;
                        myCommand.ExecuteNonQuery();
                    }

                    if (status != gVariable.ALARM_STATUS_UNCHANGED)
                    {
                        updateString = "update `" + tableName + "` set status = '" + status + "' where id = '" + id + "'";
                        myCommand.CommandText = updateString;
                        myCommand.ExecuteNonQuery();
                    }

                    if (inHistory != gVariable.ALARM_INHISTORY_UNCHANGED)
                    {
                        updateString = "update `" + tableName + "` set inHistory = '" + inHistory + "' where id = '" + id + "'";
                        myCommand.CommandText = updateString;
                        myCommand.ExecuteNonQuery();
                    }

                    if (mailList != null)
                    {
                        updateString = "update `" + tableName + "` set mailList = '" + mailList + "' where id = '" + id + "'";
                        myCommand.CommandText = updateString;
                        myCommand.ExecuteNonQuery();
                    }

                    if (discuss != null)
                    {
                        updateString = "update `" + tableName + "` set discuss = '" + discuss + "' where id = '" + id + "'";
                        myCommand.CommandText = updateString;
                        myCommand.ExecuteNonQuery();
                    }

                    if (solution != null)
                    {
                        updateString = "update `" + tableName + "` set solution = '" + solution + "' where id = '" + id + "'";
                        myCommand.CommandText = updateString;
                        myCommand.ExecuteNonQuery();
                    }

                    myConnection.Close();

                    //get alarm fail No. by id in machine alarm table
                    str = getAnothercolumnFromDatabaseByOneColumn(databaseName, tableName, "id", id.ToString(), ALARM_FAIL_NO_IN_ALARM_DATABASE);

                    //the second time, we use database of allalarm, instead of machine name as database
                    databaseName = gVariable.globalDatabaseName;
                    tableName = gVariable.globalAlarmListTableName;

                    //get alarm ID in all alarm table
                    str = getAnothercolumnFromDatabaseByOneColumn(databaseName, tableName, "alarmFailureCode", str, ID_VALUE_IN_ALARM_DATABASE);
                    id = Convert.ToInt16(str);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "table " + tableName + " update fail!" + ex);
            }
        }
         */

        public static void updateTableItems(String databaseName, String updateString)
        {
            try
            {

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = updateString;

                myCommand.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("updateTableItems Failed for string: " + updateString + ". " + ex);
            }
        }


        /*
        public static void updateStatusByID(String databaseName, String tableName, int id, int status)
        {
            string updateString;

            //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
            updateString = "update `" + tableName + "` set orderStatus = '" + status + "' where id = '" + id + "'";

            try
            {

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = updateString;

                myCommand.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("update " + databaseName + "table " + tableName + " update fail!" + ex);
            }
        }
        */

        //if myBoardIndex == -1, ignore this value, only update status
        public static void updateDispatchTable(String databaseName, String tableName, int myBoardIndex, int status, string dispatchCodeV)
        {
            string dispatchCode;
            int qualified;
            int unqualified;
            string reportor;
            string prepareTime;
            string start;
            string finish;
            string updateString;
            int usedTime;

            try
            {
                if (myBoardIndex >= 0)
                {
                    dispatchCode = gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                    qualified = gVariable.dispatchSheet[myBoardIndex].qualifiedNumber;
                    unqualified = gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber;
                    reportor = gVariable.dispatchSheet[myBoardIndex].reportor;
                    prepareTime = gVariable.dispatchSheet[myBoardIndex].prepareTimePoint;
                    start = gVariable.dispatchSheet[myBoardIndex].realStartTime;
                    finish = gVariable.dispatchSheet[myBoardIndex].realFinishTime;
                    usedTime = gVariable.dispatchSheet[myBoardIndex].toolUsedTimes;

                    updateString = "update `" + tableName + "` set qualifyNum = " + qualified + ", unqualifyNum = " + unqualified + ", toolUsedTimes = " + usedTime + ", reportor = '" + reportor +
                        "', startTime = '" + start + "', completeTime = '" + finish + "', status = '" + status + "', prepareTimePoint = '" + prepareTime + "' where dispatchcode = '" + dispatchCode + "'";
                }
                else  //myBoardIndex < 0 means this is a global dispatch, dispatchCode is critical 
                {
                    updateString = "update `" + tableName + "` set status = '" + status + "' where dispatchcode = '" + dispatchCodeV + "'";

                }
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = updateString;

                myCommand.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "table " + tableName + " update fail!" + ex);
            }
        }

        //return 1 means dispatch already exist
        //       0 means dispatch not exist, now has been written to table successfully
        //      -1 exception occurred
        public static int writeDataToSalesOrderTable(string tableName, string salesOrderFileName, gVariable.salesOrderStruct salesOrderImpl)
        {
            int index;
            int itemNum;
            string str;
            string insertString;
            string[] itemName;

            insertString = null;
            try
            {
                itemNum = getDatabaseInsertStringFromExcel(ref insertString, salesOrderFileName);
                if (itemNum < 0)
                {
                    Console.WriteLine("writeDataToSalesOrderTable failed, since excel file has problem!");
                    return -1; //failed to get insert string
                }
                index = 0;
                itemName = insertString.Remove(0, 11).Split(',', ')');
                
                MySqlConnection myConnection = new MySqlConnection("database = globaldatabase; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                str = "insert into `salesOrderList`" + insertString;
                myCommand.CommandText = str;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.salesOrderCode);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.deliveryTime);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.productCode);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.productName);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.requiredNum);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.unit);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.customer);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.publisher);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.source);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.ERPTime);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.APSTime);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.planTime1);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.planTime2);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.realStartTime);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.realFinishTime);
                myCommand.Parameters.AddWithValue(itemName[index++], salesOrderImpl.status);

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("database globaldatabaseName add salesOrder to table salesOrderList write fail!" + ex);
            }
            return -1;
        }

        public static string [,] databaseCommonReading(string databaseName, string commandText)
        {
            int i, j;
            string[,] tableArray;
            DataTable dTable;

            try
            {
                tableArray = null;
                dTable = queryDataTableAction(databaseName, commandText, null);
                if (dTable != null && dTable.Rows.Count != 0)
                {
                    j = 0;

                    tableArray = new string[dTable.Rows.Count, dTable.Rows[0].ItemArray.Length];
                    for (i = 0; i < dTable.Rows.Count; i++)
                    {
                        for (j = 0; j < dTable.Rows[0].ItemArray.Length; j++)
                        {
                            tableArray[i, j] = dTable.Rows[i].ItemArray[j].ToString();
                        }
                    }
                }
                return tableArray;
            }
            catch (Exception ex)
            {
                Console.WriteLine("databaseCommonReading failed!" + ex);
            }
            return null;
        }

        public static int readProductInfo(ref gVariable.productStruct productImpl, string commandText)
        {
            DataTable dTable;

            try
            {
                dTable = queryDataTableAction(gVariable.basicInfoDatabaseName, commandText, null);
                if (dTable != null && dTable.Rows.Count != 0)
                {
                    productImpl.customer = dTable.Rows[0].ItemArray[PRODUCT_ITEM_CUSTOMER].ToString();
                    productImpl.productCode = dTable.Rows[0].ItemArray[PRODUCT_ITEM_PRODUCT_CODE].ToString();
                    productImpl.productName = dTable.Rows[0].ItemArray[PRODUCT_ITEM_PRODUCT_NAME].ToString();
                    productImpl.BOMCode = dTable.Rows[0].ItemArray[PRODUCT_ITEM_BOM].ToString();
                    productImpl.routeCode = dTable.Rows[0].ItemArray[PRODUCT_ITEM_ROUTE_CODE].ToString();
                    productImpl.productWeight = dTable.Rows[0].ItemArray[PRODUCT_ITEM_PRODUCT_WEIGHT].ToString();
                    productImpl.productThickness = dTable.Rows[0].ItemArray[PRODUCT_ITEM_PRODUCT_THICKNESS].ToString();
                    productImpl.baseColor = dTable.Rows[0].ItemArray[PRODUCT_ITEM_BASE_COLOR].ToString();
                    productImpl.patternType = dTable.Rows[0].ItemArray[PRODUCT_ITEM_PATTERN_TYPE].ToString();
                    productImpl.steelRollType = dTable.Rows[0].ItemArray[PRODUCT_ITEM_STEEL_ROLL].ToString();
                    productImpl.rubberRollType = dTable.Rows[0].ItemArray[PRODUCT_ITEM_RUBBER_ROLL].ToString();
                    productImpl.castSpec = dTable.Rows[0].ItemArray[PRODUCT_ITEM_CAST_SPEC].ToString();
                    productImpl.printSpec = dTable.Rows[0].ItemArray[PRODUCT_ITEM_PRINT_SPEC].ToString();
                    productImpl.slitSpec = dTable.Rows[0].ItemArray[PRODUCT_ITEM_SLIT_SPEC].ToString();
                    productImpl.castCraft = dTable.Rows[0].ItemArray[PRODUCT_ITEM_CAST_CRAFT].ToString();
                    productImpl.printCraft = dTable.Rows[0].ItemArray[PRODUCT_ITEM_PRINT_CRAFT].ToString();
                    productImpl.slitCraft = dTable.Rows[0].ItemArray[PRODUCT_ITEM_SLIT_CRAFT].ToString();
                    productImpl.castQuality = dTable.Rows[0].ItemArray[PRODUCT_ITEM_CAST_QUALITY].ToString();
                    productImpl.printQuality = dTable.Rows[0].ItemArray[PRODUCT_ITEM_PRINT_QUALITY].ToString();
                    productImpl.slitQuality = dTable.Rows[0].ItemArray[PRODUCT_ITEM_SLIT_QUALITY].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("readProductInfo failed!" + ex);
                return -1;
            }

            return 0;
        }

        
        public static int readBOMInfo(ref gVariable.BOMListStruct BOMListImpl, string commandText)
        {
            int i;
            DataTable dTable;

            try
            {
                dTable = queryDataTableAction(gVariable.basicInfoDatabaseName, commandText, null);
                if (dTable != null && dTable.Rows.Count != 0)
                {
                    BOMListImpl.BOMCode = dTable.Rows[0].ItemArray[BOM_ITEM_BOM_CODE].ToString();
                    BOMListImpl.numberOfTypes = Convert.ToInt32(dTable.Rows[0].ItemArray[BOM_ITEM_TYPE_NUMBER].ToString());

                    for (i = 0; i < BOMListImpl.numberOfTypes; i++)
                    {
                        BOMListImpl.materialCode[i] = dTable.Rows[0].ItemArray[BOM_ITEM_MATERIAL_CODE1 + i * 3].ToString();
                        BOMListImpl.materialName[i] = dTable.Rows[0].ItemArray[BOM_ITEM_MATERIAL_NAME1 + i * 3].ToString();
                        BOMListImpl.materialQuantity[i] = Convert.ToDouble(dTable.Rows[0].ItemArray[BOM_ITEM_QUANTITY1 + i * 3]);
                    }
                    for (; i < MAX_MATERIAL_TYPE_IN_BOM; i++)
                    {
                        BOMListImpl.materialCode[i] = "";
                        BOMListImpl.materialName[i] = "";
                        BOMListImpl.materialQuantity[i] = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("readBOMInfo failed!" + ex);
                return -1;
            }

            return 0;
        }

        public static int readCastCraftInfo(ref gVariable.castCraftStruct castCraftImpl, string commandText)
        {
            DataTable dTable;

            try
            {
                dTable = queryDataTableAction(gVariable.basicInfoDatabaseName, commandText, null);
                if (dTable != null && dTable.Rows.Count != 0)
                {
                    castCraftImpl.castCraftCode = dTable.Rows[0].ItemArray[CAST_CRAFT_CODE].ToString();
                    castCraftImpl.castCraft_C1 = dTable.Rows[0].ItemArray[CAST_CRAFT_C1].ToString();
                    castCraftImpl.castCraft_C2 = dTable.Rows[0].ItemArray[CAST_CRAFT_C2].ToString();
                    castCraftImpl.castCraft_C3 = dTable.Rows[0].ItemArray[CAST_CRAFT_C3].ToString();
                    castCraftImpl.castCraft_C4 = dTable.Rows[0].ItemArray[CAST_CRAFT_C4].ToString();
                    castCraftImpl.castCraft_C5 = dTable.Rows[0].ItemArray[CAST_CRAFT_C5].ToString();
                    castCraftImpl.castCraft_C6 = dTable.Rows[0].ItemArray[CAST_CRAFT_C6].ToString();
                    castCraftImpl.castCraft_C7 = dTable.Rows[0].ItemArray[CAST_CRAFT_C7].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("readCastCraftInfo failed!" + ex);
                return -1;
            }

            return 0;
        }

                                                                                                       
        public static int readCastQualityInfo(ref gVariable.castQualityStruct castQualityImpl, string commandText)
        {
            DataTable dTable;

            try
            {
                dTable = queryDataTableAction(gVariable.basicInfoDatabaseName, commandText, null);
                if (dTable != null && dTable.Rows.Count != 0)
                {
                    castQualityImpl.castQualityCode = dTable.Rows[0].ItemArray[CAST_QUALITY_CODE].ToString();                   
                    castQualityImpl.castQualityReelWeight = dTable.Rows[0].ItemArray[CAST_QUALITY_REEL_WEIGHT].ToString();
                    castQualityImpl.castQualityReelThickness = dTable.Rows[0].ItemArray[CAST_QUALITY_REEL_THINKNESS].ToString();
                    castQualityImpl.castQualityReelCorona = dTable.Rows[0].ItemArray[CAST_QUALITY_REEL_CORONA].ToString();
                    castQualityImpl.castQualityReelWidth = dTable.Rows[0].ItemArray[CAST_QUALITY_REEL_WIDTH].ToString();
                    castQualityImpl.castQualityReelDiameter = dTable.Rows[0].ItemArray[CAST_QUALITY_REEL_DIAMETER].ToString();
                    castQualityImpl.castQualityReelLength = dTable.Rows[0].ItemArray[CAST_QUALITY_REEL_LENGTH].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("readCastQualityInfo failed!" + ex);
                return -1;
            }

            return 0;
        }
                                                                                                            
                                                                                                             
        public static int readPrintCraftInfo(ref gVariable.printCraftStruct printCraftImpl, string commandText)
        {
            DataTable dTable;

            try
            {
                dTable = queryDataTableAction(gVariable.basicInfoDatabaseName, commandText, null);
                if (dTable != null && dTable.Rows.Count != 0)
                {
                    printCraftImpl.printerCraftCode = dTable.Rows[0].ItemArray[PRINTER_CRAFT_CODE].ToString();
                    printCraftImpl.printerCraftLineSpeed = dTable.Rows[0].ItemArray[PRINTER_CRAFT_LINE_SPEED].ToString();
                    printCraftImpl.printerCraftOvenTemperature = dTable.Rows[0].ItemArray[PRINTER_CRAFT_OVEN_TEMPERATURE].ToString();
                    printCraftImpl.printerCraftKnifePressure = dTable.Rows[0].ItemArray[PRINTER_CRAFT_KNIFE_PRESSURE].ToString();
                    printCraftImpl.printerCraftKnifeAngle = dTable.Rows[0].ItemArray[PRINTER_CRAFT_KNIFE_ANGLE].ToString();
                    printCraftImpl.printerCraftInkVicosity = dTable.Rows[0].ItemArray[PRINTER_CRAFT_INK_VICOSITY].ToString();
                    printCraftImpl.printerCraftPrintRollPressure = dTable.Rows[0].ItemArray[PRINTER_CRAFT_PRINT_ROLL_PRESSURE].ToString();
                    printCraftImpl.printerCraftReleaseStrain = dTable.Rows[0].ItemArray[PRINTER_CRAFT_RELEASE_STRAIN].ToString();
                    printCraftImpl.printerCraftInletStrain = dTable.Rows[0].ItemArray[PRINTER_CRAFT_INLET_STRAIN].ToString();
                    printCraftImpl.printerCraftOutletStrain = dTable.Rows[0].ItemArray[PRINTER_CRAFT_OUTLET_STRAIN].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("readPrintCraftInfo failed!" + ex);
                return -1;
            }

            return 0;
        }


        public static int readPrintQualityInfo(ref gVariable.printQualityStruct printQualityImpl, string commandText)
        {
            DataTable dTable;

            try
            {
                dTable = queryDataTableAction(gVariable.basicInfoDatabaseName, commandText, null);
                if (dTable != null && dTable.Rows.Count != 0)
                {
                    printQualityImpl.printerQualityCode = dTable.Rows[0].ItemArray[PRINTER_QUALITY_CODE].ToString();
                    printQualityImpl.printerQualityReelWidth = dTable.Rows[0].ItemArray[PRINTER_QUALITY_REEL_WIDTH].ToString();
                    printQualityImpl.printerQualityReelDiameter = dTable.Rows[0].ItemArray[PRINTER_QUALITY_REEL_DIAMETER].ToString();
                    printQualityImpl.printerQualityCoronaSide = dTable.Rows[0].ItemArray[PRINTER_QUALITY_CORONA_SIDE].ToString();
                    printQualityImpl.printerQualityCoronaDegree = dTable.Rows[0].ItemArray[PRINTER_QUALITY_CORONA_DEGREE].ToString();
                    printQualityImpl.printerQualityPatternDirection = dTable.Rows[0].ItemArray[PRINTER_QUALITY_PATTERN_DIRECTION].ToString();
                    printQualityImpl.printerQualityPatternPosition = dTable.Rows[0].ItemArray[PRINTER_QUALITY_PATTERN_POSITION].ToString();
                    printQualityImpl.printerQualityPatternCompleteness = dTable.Rows[0].ItemArray[PRINTER_QUALITY_PATTERN_COMPLETENESS].ToString();
                    printQualityImpl.printerQualityDefects = dTable.Rows[0].ItemArray[PRINTER_QUALITY_DEFECTS].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("readPrintQualityInfo failed!" + ex);
                return -1;
            }

            return 0;
        }
                                                                                                               
        public static int readSlitCraftInfo(ref gVariable.slitCraftStruct slitCraftImpl, string commandText)
        {
            DataTable dTable;

            try
            {
                dTable = queryDataTableAction(gVariable.basicInfoDatabaseName, commandText, null);
                if (dTable != null && dTable.Rows.Count != 0)
                {
                    slitCraftImpl.slitCraftCode = dTable.Rows[0].ItemArray[SLIT_CRAFT_CODE].ToString();
                    slitCraftImpl.slitCraftLineSpeed = dTable.Rows[0].ItemArray[SLIT_CRAFT_LINE_SPEED].ToString();
                    slitCraftImpl.slitCraftReleaseStrain = dTable.Rows[0].ItemArray[SLIT_CRAFT_RELEASE_STRAIN].ToString();
                    slitCraftImpl.slitCraftTuckInitStrain1 = dTable.Rows[0].ItemArray[SLIT_CRAFT_TUCK_INIT_STRAIN1].ToString();
                    slitCraftImpl.slitCraftTuckTaper1 = dTable.Rows[0].ItemArray[SLIT_CRAFT_TUCK_TAPER1].ToString();
                    slitCraftImpl.slitCraftTuckNumber1 = dTable.Rows[0].ItemArray[SLIT_CRAFT_TUCKSLIT_NUMBER1].ToString();
                    slitCraftImpl.slitCraftTuckInitStrain2 = dTable.Rows[0].ItemArray[SLIT_CRAFT_TUCK_INIT_STRAIN2].ToString();
                    slitCraftImpl.slitCraftTuckTaper2 = dTable.Rows[0].ItemArray[SLIT_CRAFT_TUCK_TAPER2].ToString();
                    slitCraftImpl.slitCraftTuckNumber2 = dTable.Rows[0].ItemArray[SLIT_CRAFT_TUCKSLIT_NUMBER2].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("readSlitCraftInfo failed!" + ex);
                return -1;
            }

            return 0;
        }
                                                                                                               
        public static int readSlitQualityInfo(ref gVariable.slitQualityStruct slitQualityImpl, string commandText)
        {
            DataTable dTable;

            try
            {
                dTable = queryDataTableAction(gVariable.basicInfoDatabaseName, commandText, null);
                if (dTable != null && dTable.Rows.Count != 0)
                {
                    slitQualityImpl.slitQualityCode = dTable.Rows[0].ItemArray[SLIT_QUALITY_CODE].ToString();
                    slitQualityImpl.slitQualityReelWidth = dTable.Rows[0].ItemArray[SLIT_QUALITY_REEL_WIDTH].ToString();
                    slitQualityImpl.slitQualityReelDiameter = dTable.Rows[0].ItemArray[SLIT_QUALITY_REEL_DIAMETER].ToString();
                    slitQualityImpl.slitQualityReelLength = dTable.Rows[0].ItemArray[SLIT_QUALITY_LENGTH].ToString();
                    slitQualityImpl.slitQualityReelWeight = dTable.Rows[0].ItemArray[SLIT_QUALITY_WEIGHT].ToString();
                    slitQualityImpl.slitQualityReelThickness = dTable.Rows[0].ItemArray[SLIT_QUALITY_REEL_THICKNESS].ToString();
                    slitQualityImpl.slitQualityReelCorona = dTable.Rows[0].ItemArray[SLIT_QUALITY_REEL_CORONA].ToString();
                    slitQualityImpl.slitQualityPatternDirection = dTable.Rows[0].ItemArray[SLIT_QUALITY_PATTERN_DIRECTION].ToString();
                    slitQualityImpl.slitQualityPatternCompleteness = dTable.Rows[0].ItemArray[SLIT_QUALITY_PATTERN_COMPLETENESS].ToString();
                    slitQualityImpl.slitQualityDefects = dTable.Rows[0].ItemArray[SLIT_QUALITY_DEFECTS].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("readSlitQualityInfo failed!" + ex);
                return -1;
            }

            return 0;
        }
                                                                                                       
        public static int readSalesOrderInfo(ref gVariable.salesOrderStruct salesOrderImpl, string commandText)
        {
            DataTable dTable;

            try
            {
                dTable = queryDataTableAction(gVariable.globalDatabaseName, commandText, null);
                if (dTable != null && dTable.Rows.Count != 0)
                {
                    salesOrderImpl.ID = dTable.Rows[0].ItemArray[ID_VALUE_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.salesOrderCode = dTable.Rows[0].ItemArray[ORDER_CODE_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.deliveryTime = dTable.Rows[0].ItemArray[DELIVERY_TIME_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.productCode = dTable.Rows[0].ItemArray[PRODUCT_CODE_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.productName = dTable.Rows[0].ItemArray[PRODUCT_NAME_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.requiredNum = dTable.Rows[0].ItemArray[REQUIRED_NUM_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.unit = dTable.Rows[0].ItemArray[UNIT_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.customer = dTable.Rows[0].ItemArray[CUSTOMER_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.publisher = dTable.Rows[0].ItemArray[PUBLISHER_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.ERPTime = dTable.Rows[0].ItemArray[ERP_TIME_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.APSTime = dTable.Rows[0].ItemArray[APS_TIME_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.planTime1 = dTable.Rows[0].ItemArray[PLANNED_START_TIME_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.planTime2 = dTable.Rows[0].ItemArray[PLANNED_COMPLETE_TIME_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.realStartTime = dTable.Rows[0].ItemArray[REAL_START_TIME_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.realFinishTime = dTable.Rows[0].ItemArray[REAL_COMPLETE_TIME_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.source = dTable.Rows[0].ItemArray[SOURCE_IN_SALESORDER_DATABASE].ToString();
                    salesOrderImpl.status = dTable.Rows[0].ItemArray[STATUS_IN_SALESORDER_DATABASE].ToString();

                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("readDataFromSalesOrderTable failed!" + ex);
            }
            return -1;
        }

        public static gVariable.machineListStruct[] readMachineList(string databaseName, string commandText)
        {
            const int MAX_MACHINE_NUM = 400;  //we only display 400 machines at most
            int i;
//            int num;
            DataTable dTable;
            gVariable.machineListStruct [] machineList;

            try
            {
                machineList = new gVariable.machineListStruct[MAX_MACHINE_NUM];

                dTable = queryDataTableAction(databaseName, commandText, null);
                if (dTable != null && dTable.Rows.Count != 0)
                {
                    for (i = 0; i < dTable.Rows.Count; i++)
                    {
                        machineList[i].machineCode = dTable.Rows[i].ItemArray[MACHINE_CODE_IN_LIST].ToString();
                        machineList[i].machineName = dTable.Rows[i].ItemArray[MACHINE_NAME_IN_LIST].ToString();
                        machineList[i].machineType = dTable.Rows[i].ItemArray[MACHINE_TYPE_IN_LIST].ToString();
                        machineList[i].machineVendor = dTable.Rows[i].ItemArray[MACHINE_VENDOR_IN_LIST].ToString();
                        machineList[i].purchaseDate = dTable.Rows[i].ItemArray[PURCHASE_DATE_IN_LIST].ToString();
                        machineList[i].machinePrepareTime = dTable.Rows[i].ItemArray[PREPARE_TIME_IN_LIST].ToString();
                        machineList[i].machineWorkingTime = dTable.Rows[i].ItemArray[WORKING_TIME_IN_LIST].ToString();
                        machineList[i].preferValue = dTable.Rows[i].ItemArray[PREFER_VALUE_IN_LIST].ToString();
                        machineList[i].maintenancePeriod = dTable.Rows[i].ItemArray[MAINTAIN_PERIOD_IN_LIST].ToString();
                        machineList[i].maintenanceHour = dTable.Rows[i].ItemArray[MAINTAIN_HOUR_IN_LIST].ToString();
                    }
                }

                return machineList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("readMachinePlanFromTable failed!" + ex);
            }
            return null;
        }


        public static int readMachineStatusRecordFromTable(ref gVariable.machineStatusRecordStruct machineStatusRecordImpl, string commandText)
        {
            DataTable dTable;

            try
            {
                dTable = queryDataTableAction(gVariable.globalDatabaseName, commandText, null);
                if (dTable != null && dTable.Rows.Count != 0)
                {
                    machineStatusRecordImpl.recordTime = dTable.Rows[0].ItemArray[MACHINE_PLAN_RECORD_TIME].ToString();
                    machineStatusRecordImpl.machineStatus = dTable.Rows[0].ItemArray[MACHINE_PLAN_MACHINE_STATUS].ToString();
                    machineStatusRecordImpl.statusStart = dTable.Rows[0].ItemArray[MACHINE_PLAN_STATUS_START].ToString();
                    machineStatusRecordImpl.keepMinutes = dTable.Rows[0].ItemArray[MACHINE_PLAN_KEEP_MINUTES].ToString();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("readmachineStatusRecordFromTable failed!" + ex);
            }
            return -1;
        }


        static void getDispatchInsertString()
        {
            getDatabaseInsertStringFromExcel(ref insertStringForDispatch, gVariable.dispatchListFileName);
        }


        static void getMaterialInsertString()
        {
            getDatabaseInsertStringFromExcel(ref insertStringForMaterial, gVariable.materialListFileName);
        }


        static void getWorkingPlanInsertString()
        {
            getDatabaseInsertStringFromExcel(ref insertStringForWorkingPlan, gVariable.machineWorkingPlanFileName);
        }

        //return 1 means dispatch already exist
        //       0 means dispatch not exist, now has been written to table successfully
        //      -1 exception occurred
        public static int writeDataToDispatchListTable(String databaseName, String tableName, gVariable.dispatchSheetStruct dispatchImpl)
        {
            int num;
            int index;
            //int itemNum;
            string[] itemName;
            //string insertString;

            //insertString = null;
            try
            {
                //itemNum = getDatabaseInsertStringFromExcel(ref insertString, gVariable.dispatchListFileName);
                //if (itemNum < 0)
                //{
                //    Console.WriteLine("database " + databaseName + "table " + tableName + " write failed, excel read error!");
                //    return -1;
                //}

                index = 0;
                itemName = insertStringForDispatch.Remove(0, 11).Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                myCommand.CommandText = "select count(*) from `" + tableName + "` where dispatchCode = " + "\'" + dispatchImpl.dispatchCode + "\'";

                //already exists
                num = Convert.ToInt32(myCommand.ExecuteScalar());
                if (num > 0)
                {
                    myConnection.Close();
                    return 1;
                }

                myCommand.CommandText = "insert into `" + tableName + "`" + insertStringForDispatch;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.machineID);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.dispatchCode);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.planTime1);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.planTime2);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.productCode);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.productName);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.operatorName);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.plannedNumber);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.outputNumber);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.qualifiedNumber);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.unqualifiedNumber);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.processName);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.realStartTime);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.realFinishTime);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.prepareTimePoint);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.status);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.toolLifeTimes);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.toolUsedTimes);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.outputRatio);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.serialNumber);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.reportor);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.workshop);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.workshift);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.salesOrderCode);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.BOMCode);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.customer);

                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.multiProduct);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.productCode2);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.productCode3);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.operatorName2);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.operatorName3);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.batchNum);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.productColor);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.rawMateialcode);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.productLength);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.productDiameter);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.productWeight);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.slitWidth);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.printSide);

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("writeDataToDispatchListTable " + databaseName + "add dispatch to table " + tableName + " write fail!" + ex);
            }
            return -1;
        }

        public static int writeDataToWorkingPlanTableFromCalendar(string databaseName, string tableName, string attribute, int id, string start, string end)
        {
            int index;
            int timeStamp1;
            int timeStamp2;
            int duration;
            string[] itemName;

            //insertString = null;
            try
            {
                if (start != null)
                {
                    timeStamp1 = toolClass.timeStringToTimeStamp(start);
                }
                else
                {
                    timeStamp1 = 0;
                }

                if (end != null)
                {
                    timeStamp2 = toolClass.timeStringToTimeStamp(end);
                }
                else
                {
                    timeStamp2 = 0;
                }
                duration = timeStamp2 - timeStamp1;

                index = 0;
                itemName = insertStringForWorkingPlan.Remove(0, 11).Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + tableName + "`" + insertStringForWorkingPlan;

                myCommand.Parameters.AddWithValue("@id", 0);
                if (attribute == "Repaired") 
                    myCommand.Parameters.AddWithValue(itemName[index++], "0");
                else if (attribute == "Scheduled")
                    myCommand.Parameters.AddWithValue(itemName[index++], "1");
                else if (attribute == "Maintained")
                    myCommand.Parameters.AddWithValue(itemName[index++], id);

                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], start);
                myCommand.Parameters.AddWithValue(itemName[index++], timeStamp1);
                myCommand.Parameters.AddWithValue(itemName[index++], (duration / 3600).ToString());
                myCommand.Parameters.AddWithValue(itemName[index++], end);
                myCommand.Parameters.AddWithValue(itemName[index++], timeStamp2);
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], "");
                myCommand.Parameters.AddWithValue(itemName[index++], "");

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("writeDataToWorkingPlanTable " + databaseName + "add dispatch to table " + tableName + " write fail!" + ex);
            }
            return -1;
        }


        public static int writeDataToWorkingPlanTable(String databaseName, String tableName, gVariable.dispatchSheetStruct dispatchImpl, int timeStamp, int duration)
        {
            //int num;
            int index;
            //int itemNum;
            string[] itemName;
            //string insertString;

            //insertString = null;
            try
            {
                //itemNum = getDatabaseInsertStringFromExcel(ref insertString, gVariable.machineWorkingPlanFileName);
                //if (itemNum < 0)
                {
                //    Console.WriteLine("database " + databaseName + "table " + tableName + " write failed, excel read error!");
                //    return -1;
                }

                index = 0;
                itemName = insertStringForWorkingPlan.Remove(0, 11).Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + tableName + "`" + insertStringForWorkingPlan;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], "1");
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.salesOrderCode);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.dispatchCode);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.planTime1);
                myCommand.Parameters.AddWithValue(itemName[index++], timeStamp.ToString());
                myCommand.Parameters.AddWithValue(itemName[index++], (duration / 3600).ToString());
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.planTime2);
                myCommand.Parameters.AddWithValue(itemName[index++], (timeStamp + duration).ToString());
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.productCode);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.productName);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.plannedNumber);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.customer);
                myCommand.Parameters.AddWithValue(itemName[index++], dispatchImpl.workshift);
                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("writeDataToWorkingPlanTable " + databaseName + "add dispatch to table " + tableName + " write fail!" + ex);
            }
            return -1;
        }


        public static void writeDataToMachineStatusTable(String databaseName, String tableName, int myBoardIndex)
        {
            int index;
            int itemNum;
            string[] itemName;
            string insertString;

            insertString = null;
            try
            {
                itemNum = getDatabaseInsertStringFromExcel(ref insertString, gVariable.machineStatusListFileName);
                if (itemNum < 0)
                {
                    Console.WriteLine("writeDataToMachineStatusTable(), database " + databaseName + "table " + tableName + " write failed, excel read error!");
                }

                index = 0;
                itemName = insertString.Remove(0, 11).Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + tableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.dispatchSheet[myBoardIndex].dispatchCode);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.machineStatus[myBoardIndex].totalWorkingTime);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.machineStatus[myBoardIndex].collectedNumber);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.machineStatus[myBoardIndex].productBeat);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.machineStatus[myBoardIndex].workingTime);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.machineStatus[myBoardIndex].prepareTime);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.machineStatus[myBoardIndex].standbyTime);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.machineStatus[myBoardIndex].power);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.machineStatus[myBoardIndex].powerConsumed);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.machineStatus[myBoardIndex].revolution);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.machineStatus[myBoardIndex].toolLifeTimes);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.machineStatus[myBoardIndex].toolUsedTimes);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.machineStatus[myBoardIndex].maintenancePeriod);
                myCommand.Parameters.AddWithValue(itemName[index++], gVariable.machineStatus[myBoardIndex].lastMaintenance);

                myCommand.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "table " + tableName + " write fail!" + ex);
            }
        }


        public static void writeDataToMaterialListTable(string databaseName, string tableName, gVariable.materialListStruct materialListImpl)
        {
            int i;
            int index;
            //int itemNum;
            //string insertString;
            string[] itemName;

            //insertString = null;
            try
            {
                //itemNum = getDatabaseInsertStringFromExcel(ref insertString, gVariable.materialListFileName);
                //if(itemNum < 0)
                {
                 //   Console.WriteLine("database " + databaseName + "table " + tableName + " write failed, excel read error!");
                 //   return;
                }

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + tableName + "`" + insertStringForMaterial;

                index = 0;
                itemName = insertStringForMaterial.Remove(0, 11).Split(',', ')');

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], materialListImpl.salesOrderCode);
                myCommand.Parameters.AddWithValue(itemName[index++], materialListImpl.dispatchCode);
                myCommand.Parameters.AddWithValue(itemName[index++], materialListImpl.machineID);
                myCommand.Parameters.AddWithValue(itemName[index++], materialListImpl.machineName);
                //myCommand.Parameters.AddWithValue(itemName[index++], materialListImpl.status);
                myCommand.Parameters.AddWithValue(itemName[index++], materialListImpl.numberOfTypes);

                for (i = 0; i < MAX_MATERIAL_TYPE_IN_BOM; i++)
                {
                    //if (index >= itemName.Length - 1)
                    //    break;

                    //real material data
                    //myCommand.Parameters.AddWithValue(itemName[index++], materialListImpl.materialName[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], materialListImpl.materialCode[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], materialListImpl.materialRequired[i]);
                    //myCommand.Parameters.AddWithValue(itemName[index++], materialListImpl.previousLeft);
                    //myCommand.Parameters.AddWithValue(itemName[index++], materialListImpl.currentLeft);
                    //myCommand.Parameters.AddWithValue(itemName[index++], materialListImpl.currentUsed);
                    //myCommand.Parameters.AddWithValue(itemName[index++], materialListImpl.fullPackNum[i]);
                }
                myCommand.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "table " + tableName + " write fail!" + ex);
            }
        }

        public static void writeCraftDescToCraftList(String databaseName, String tableName, gVariable.craftListStruct craftListImpl, string dispatchCode)
        {
            int i;
            int index;
            int itemNum;
            string insertString;
            string[] itemName;

            insertString = null;
            try
            {
                itemNum = getDatabaseInsertStringFromExcel(ref insertString, gVariable.craftListFileName);
                if (itemNum < 0)
                {
                    Console.WriteLine("database " + databaseName + "table " + tableName + " write failed, excel read error!");
                    return;
                }

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + tableName + "`" + insertString;

                index = 0;
                itemName = insertString.Remove(0, 11).Split(',', ')');

                for (i = 0; i < craftListImpl.paramNumber; i++)
                {
                    myCommand.Parameters.AddWithValue("@id", 0);
                    myCommand.Parameters.AddWithValue(itemName[index++], dispatchCode);
                    myCommand.Parameters.AddWithValue(itemName[index++], craftListImpl.paramName[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], craftListImpl.paramLowerLimit[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], craftListImpl.paramUpperLimit[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], craftListImpl.paramDefaultValue[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], craftListImpl.paramUnit[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], craftListImpl.paramValue[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], craftListImpl.rangeLowerLimit[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], craftListImpl.rangeUpperLimit[i]);

                    myCommand.ExecuteNonQuery();

                    myCommand.Parameters.Clear();
                    index = 0;
                }
                myConnection.Close();
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "table " + tableName + " write fail!" + ex);
            }
        }

        public static void writeQualityDescToQualityList(String databaseName, String tableName, gVariable.qualityListStruct qualityListImpl, string dispatchCode)
        {
            int i;
            int index;
            int itemNum;
            string insertString;
            string[] itemName;

            insertString = null;

            try
            {
                itemNum = getDatabaseInsertStringFromExcel(ref insertString, gVariable.qualityListFileName);
                if (itemNum < 0)
                {
                    Console.WriteLine("database " + databaseName + "table " + tableName + " write failed, excel read error!");
                    return;
                }

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + tableName + "`" + insertString;

                index = 0;
                itemName = insertString.Remove(0, 11).Split(',', ')');


                for (i = 0; i < qualityListImpl.checkItemNumber; i++)
                {
                    myCommand.Parameters.AddWithValue("@id", 0);
                    myCommand.Parameters.AddWithValue(itemName[index++], dispatchCode);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.checkItem[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.checkRequirement[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.controlCenterValue1[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.controlCenterValue2[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.specLowerLimit[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.controlLowerLimit1[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.controlLowerLimit2[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.specUpperLimit[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.controlUpperLimit1[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.controlUpperLimit2[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.sampleRatio[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.checkResultData[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.checkResult[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.unit[i]);
                    myCommand.Parameters.AddWithValue(itemName[index++], qualityListImpl.chartType[i]);

                    myCommand.ExecuteNonQuery();

                    myCommand.Parameters.Clear();
                    index = 0;
                }
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "table " + tableName + " write fail!" + ex);
            }
        }

/*
        public static void writeDataToSettingADC(String databaseName, String tableName, string dispatchCode, string channelEnabled, string channelTitle, string channelUnit, float lowerRange, float upperRange, float lowerLimit, float upperLimit, int workingVoltage)
        {
            String str;

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                str = "insert into `" + tableName + "`" + strDataSetting12;
                myCommand.CommandText = str;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue("@dispatchCode", dispatchCode);
                myCommand.Parameters.AddWithValue("@channelEnabled", channelEnabled);
                myCommand.Parameters.AddWithValue("@channelTitle", channelTitle);
                myCommand.Parameters.AddWithValue("@channelUnit", channelUnit);
                myCommand.Parameters.AddWithValue("@lowerRange", lowerRange);
                myCommand.Parameters.AddWithValue("@upperRange", upperRange);
                myCommand.Parameters.AddWithValue("@lowerLimit", lowerLimit);
                myCommand.Parameters.AddWithValue("@upperLimit", upperLimit);
                myCommand.Parameters.AddWithValue("@workingVoltage", workingVoltage);

                myCommand.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "table " + tableName + " write fail!" + ex);
            }
        }

        public static void writeDataToSettingUART(String databaseName, String tableName, string dispatchCode, int uartDeviceType, int uartBaudrate)
        {
            String str;

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                str = "insert into `" + tableName + "`" + strDataSetting22;
                myCommand.CommandText = str;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue("@dispatchCode", dispatchCode);
                myCommand.Parameters.AddWithValue("@uartDeviceType", uartDeviceType);
                myCommand.Parameters.AddWithValue("@uartBaudrate", uartBaudrate);

                myCommand.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "table " + tableName + " write fail!" + ex);
            }
        }

        public static void writeDataToSettingBEAT(String databaseName, String tableName, string dispatchCode, float idleCurrentHigh, float idleCurrentLow, float workCurrentHigh, float workCurrentLow, float errorCurrentHigh, float errorCurrentLow)
        {
            String str;

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                str = "insert into `" + tableName + "`" + strDataSetting42;
                myCommand.CommandText = str;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue("@dispatchCode", dispatchCode);
                myCommand.Parameters.AddWithValue("@idleCurrentHigh", idleCurrentHigh);
                myCommand.Parameters.AddWithValue("@idleCurrentLow", idleCurrentLow);
                myCommand.Parameters.AddWithValue("@workCurrentHigh", workCurrentHigh);
                myCommand.Parameters.AddWithValue("@workCurrentLow", workCurrentHigh);
                myCommand.Parameters.AddWithValue("@errorCurrentHigh", errorCurrentHigh);
                myCommand.Parameters.AddWithValue("@errorCurrentLow", errorCurrentLow);

                myCommand.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "table " + tableName + " write fail!" + ex);
            }
        }
       */

        //read data from one table to global parameters gVariable.timeInPoint and gVariable.dataInPoint
        public static int readOneTableForMoreData(string databaseName, string tableName, int numOfPointsNeeded)
        {
            string str;
            object o;
            int i;
            int startRecordIndex;
            int recordNum;
            MySqlDataReader myReader;

            i = 0; // startRecordIndex;

            try
            {
                if (databaseName == null || tableName == null)
                    return 0;

                lock (readSmallLocker)
                {
                    MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                    myConnection.Open();

                    MySqlCommand myCommand = myConnection.CreateCommand();

                    str = "select count(*) from `" + tableName + "`";
                    myCommand.CommandText = str;

                    o = myCommand.ExecuteScalar();
                    if (o == null)
                        recordNum = 0;
                    else
                        recordNum = Convert.ToInt32(o);

                    if (recordNum == 0)   //no data for this channel
                        return 0;

                    if (recordNum > numOfPointsNeeded)
                    {
                        startRecordIndex = recordNum - numOfPointsNeeded;
                    }
                    else
                    {
                        startRecordIndex = 0;
                    }

                    str = "select * from `" + tableName + "` where id >= " + startRecordIndex;
                    myCommand.CommandText = str;

                    myReader = myCommand.ExecuteReader();
                    //index means the first curve index for this kind of data
                    while (myReader.Read())
                    {
                        gVariable.timeInPoint[0, i] = myReader.GetInt32(TIME_VALUE_IN_DATABASE);
                        gVariable.dataInPoint[0, i] = myReader.GetFloat(DATA_VALUE_IN_DATABASE);

                        i++;

                        ///maybe new data came, so there may be more than numOfPointsNeeded pieces of data, in that case, we only take numOfPointsNeeded
                        if (i > numOfPointsNeeded)
                            break;
                    }

                    myReader.Close();
                    myConnection.Close();
                }
            }
            catch (Exception ex)
            {
                gVariable.infoWriter.WriteLine(databaseName + ":" + tableName + "readOneTableForMoreData fail!" + ex);
            }

            return i;
        }


        //read recent data from all kinds of tables for one machine
        //type == gVariable.ALL_DATA_IN_DATABASE, we need to read craft/volcur/beat/quality data one by one, used for curve display; otherwise read only one kind of data, used for SPC checking
        //return: 
        public static void readSmallPartOfDataToArray(String databaseName, int numOfPointsNeeded)
        {
            string str;
            object o;
            int i, j, k;
            int vTime;
            int numOfCurveForThisDataType;
            int dataType;
            int index, startRecordIndex;
            int recordNum;
            MySqlDataReader myReader;

            index = 0;
            try
            {
                lock (readSmallLocker)
                {
                    MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                    myConnection.Open();

                    MySqlCommand myCommand = myConnection.CreateCommand();

                    dataType = gVariable.CRAFT_DATA_IN_DATABASE;

                    for (j = 0; j < gVariable.totalCurveNum[gVariable.boardIndexSelected]; )
                    {
                        numOfCurveForThisDataType = gVariable.numOfCurveForOneType[dataType];  //number of curves for this data type

                        if (gVariable.dispatchBasedCurveTableName[j] == null) //no more curves
                            break;  //this should not happen, just in case

                        //                    if (gVariable.curveOrNot[j] == 0)  //this port should not be displayed as curve
                        //                    {
                        //                        index--;  //index are used for curve port only
                        //                        continue;
                        //                    }

                        str = "select count(*) from `" + gVariable.dispatchBasedCurveTableName[j] + "`";
                        myCommand.CommandText = str;

                        o = myCommand.ExecuteScalar();
                        if (o == null)
                            recordNum = 0;
                        else
                            recordNum = Convert.ToInt32(o);

                        if (recordNum == 0)   //no data for this channel
                        {
                            for (k = 0; k < numOfCurveForThisDataType; k++)
                                gVariable.dataNumForCurve[k + j] = 0;

                            dataType++;
                            j += numOfCurveForThisDataType;
                            continue;
                        }

                        if (recordNum > numOfPointsNeeded)
                        {
                            startRecordIndex = recordNum - numOfPointsNeeded;
                        }
                        else
                        {
                            startRecordIndex = 0;
                        }
                        str = "select * from `" + gVariable.dispatchBasedCurveTableName[j] + "` where id >= " + startRecordIndex;
                        myCommand.CommandText = str;

                        myReader = myCommand.ExecuteReader();
                        i = 0; // startRecordIndex;
                        //index means the first curve index for this kind of data
                        while (myReader.Read())
                        {
                            index = j;
                            vTime = myReader.GetInt32(TIME_VALUE_IN_DATABASE);
                            for (k = 0; k < numOfCurveForThisDataType; k++)
                            {
                                gVariable.timeInPoint[index, i] = vTime;
                                gVariable.dataInPoint[index, i] = myReader.GetFloat(DATA_VALUE_IN_DATABASE + k);
                                if (gVariable.upperLimitValueForPie[index] < gVariable.dataInPoint[index, i])
                                    gVariable.upperLimitValueForPie[index] = gVariable.dataInPoint[index, i];
                                if (gVariable.lowerLimitValueForPie[index] > gVariable.dataInPoint[index, i])
                                    gVariable.lowerLimitValueForPie[index] = gVariable.dataInPoint[index, i];

                                index++;
                            }

                            i++;

                            ///maybe new data came, so there may be more than numOfPointsNeeded pieces of data, in that case, we only take numOfPointsNeeded
                            if (i >= numOfPointsNeeded)
                                break;
                        }

                        numOfCurveForThisDataType += j;
                        for (k = j; k < numOfCurveForThisDataType; k++)
                        {
                            gVariable.numOfPointsForPie[k] = i;
                            gVariable.dataNumForCurve[k] = recordNum;

                            //put the last data for this curve into curve text array which will be displayed in multiCurve screen
                            //gVariable.curveTextArray[k] = gVariable.dataInPoint[k, i].ToString("f4");
                        }

                        //we've done with this data type, go to the next
                        j = numOfCurveForThisDataType;

                        dataType++;  //type from craft -> volcur -> quality ->beat

                        i--;

                        myReader.Close();
                    }
                    myConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(databaseName + ":" + gVariable.dispatchBasedCurveTableName[index] + "get record data fail, small screen!" + ex);
            }
        }

        //read recent data from all kinds of tables for one machine
        //qualityDataItemNum: number data items in this quality table
        //numOfPointsNeeded: number of data we want from this table
        //startingID: if less than 0, it means we need to get recent numOfPointsNeeded of data from this table, other wise reading starts from startingID 
        //return: the ID of the first data point in database table, or -1 which mean there is not enough data for numOfPointsNeeded
        public static int readSmallPartOfDataForSPC(string databaseName, string tableName, int qualityDataItemNum, int numOfPointsNeeded, float[,] dataInPoint, int[,] timeInPoint, int[,] statusInPoint, int startingID)
        {
            string str;
            object o;
            int i, k;
            int vTime;
            int index, startRecordIndex;
            int recordNum;
            MySqlDataReader myReader;

            index = 0;
            try
            {
                lock (readSmallLocker)
                {
                    MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                    myConnection.Open();

                    MySqlCommand myCommand = myConnection.CreateCommand();

                    str = "select count(*) from `" + tableName + "`";
                    myCommand.CommandText = str;

                    o = myCommand.ExecuteScalar();
                    if (o == null || (int)o == 0)
                    {
                        recordNum = 0;
                        return -1;
                    }
                    else
                        recordNum = Convert.ToInt32(o);

                    if (startingID < 0)
                    {
                        if (recordNum >= numOfPointsNeeded)
                        {
                            startRecordIndex = recordNum - numOfPointsNeeded + 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else  //we have a starting ID
                    {
                        if (startingID + numOfPointsNeeded - 1 <= recordNum)
                            startRecordIndex = startingID;
                        else
                            return -1;
                    }

                    str = "select * from `" + tableName + "` where id >= " + startRecordIndex;
                    myCommand.CommandText = str;

                    myReader = myCommand.ExecuteReader();
                    i = 0; // startRecordIndex;
                    index = 0;
                    //index means the first curve index for this kind of data
                    while (myReader.Read())
                    {
                        vTime = myReader.GetInt32(TIME_VALUE_IN_DATABASE);
                        for (k = 0; k < qualityDataItemNum; k++)
                        {
                            timeInPoint[index, i] = vTime;
                            dataInPoint[index, i] = myReader.GetFloat(DATA_VALUE_IN_DATABASE + k);
                            if (gVariable.upperLimitValueForPie[index] < dataInPoint[index, i])
                                gVariable.upperLimitValueForPie[index] = dataInPoint[index, i];
                            if (gVariable.lowerLimitValueForPie[index] > dataInPoint[index, i])
                                gVariable.lowerLimitValueForPie[index] = dataInPoint[index, i];

                            if(statusInPoint != null)  //for quality data, we need to get its data stauts
                                statusInPoint[index, i] = (int)myReader.GetFloat(DATA_STATUS_IN_DATABASE + k);

                            //curve index increase by one
                            index++;
                        }

                        i++;
                        index = 0;

                        ///maybe new data came, so there may be more than numOfPointsNeeded pieces of data, in that case, we only take numOfPointsNeeded
                        if (i >= numOfPointsNeeded)
                            break;
                    }

                    myReader.Close();
                    myConnection.Close();

                    return startRecordIndex;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(databaseName + ":" + gVariable.dispatchBasedCurveTableName[index] + "get record data fail, small screen!" + ex);
                return -1;
            }
        }


        public static void readDatabaseToFile(string databaseName)
        {
            int totalDataNum;
            string str;
            string tableName;
            object o;
            float f;
            MySqlDataReader myReader;
            StreamWriter dataWriter;

            try
            {
                tableName = "dummy_10";
                dataWriter = new StreamWriter("D:\\DataCollector\\PC host\\logs\\" + databaseName, true, System.Text.Encoding.ASCII); //.Default);

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();
                MySqlCommand myCommand = myConnection.CreateCommand();

                str = "select count(*) from " + tableName;
                myCommand.CommandText = str;

                o = myCommand.ExecuteScalar();
                if (o == null)
                    totalDataNum = 0;
                else
                    totalDataNum = Convert.ToInt32(o);

                if (totalDataNum <= 100)
                    return;

                str = "select * from " + tableName;
                myCommand.CommandText = str;

                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    f = myReader.GetFloat(DATA_VALUE_IN_DATABASE);
                    dataWriter.Write("{0:X000}\r\n", f.ToString());
                }

                dataWriter.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Read database and write to data file failed for board !" + databaseName + ". Exception info:" + ex);
            }
        }


        public static int getCurrentDataForToday(string databaseName, string tableName, float[] fBuffer)
        {
            int i;
            int timeStampToday;
            string str;
            MySqlDataReader myReader;

            try
            {
                timeStampToday = (int)(TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now) - gVariable.worldStartTime).TotalSeconds;;

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                str = "select * from `" + tableName + "` where time >= " + timeStampToday;
                myCommand.CommandText = str;

                myReader = myCommand.ExecuteReader();
                i = 0;
                while (myReader.Read())
                    fBuffer[i++] = (float)myReader.GetDouble(4);
                myReader.Close();

                return i;
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "getCurrentDataForToday for " + tableName + " failed!" + ex);

                return 0;
            }
        }




        //index: which channel
        //posPercent: position of the scroll bar in percentage
        //dataNumPercent: 0 - 100, 100 means only dislay 100 pieces of data, 0 means display whole data list 
        //numOfDataPointDisplayed: number of data point that will be displayed on screen for one curve 
        public static int readAllDataToArrayByPercent(String databaseName, int index, int dataNumPercent, int posPercent)
        {
            int num;
            int i, j, k;
            int indexForThisDataType;
            int totalDataNum;
            int numOfDataPointWanted;
            int dataStartPosition;
            string tableName;

            tableName = gVariable.dispatchBasedCurveTableName[index];

            if (tableName == null)
                return 0;

            j = 0;
            k = 0;
            for (i = 0; i < gVariable.maxCurveTypes; i++)
            {
                k = j;
                j += gVariable.numOfCurveForOneType[i];
                if (j > index)
                    break;
            }
            //the index for this curve in the data type it belongs to 
            indexForThisDataType = index - k;

            try
            {
                totalDataNum = gVariable.dataNumForCurve[index];

                //we can display at least minDataForOneScreen of data point in one screen, if total number is less than tat, just display all
                if (totalDataNum <= gVariable.minDataForOneScreen)
                {
                    numOfDataPointWanted = totalDataNum;
                    dataStartPosition = 0;
                }
                else
                {
                    numOfDataPointWanted = (totalDataNum - gVariable.minDataForOneScreen) * (gVariable.PERCENTAGE_VALUE_FOR_ONE_CURVE - dataNumPercent) / gVariable.PERCENTAGE_VALUE_FOR_ONE_CURVE + gVariable.minDataForOneScreen;
                    dataStartPosition = (int)((long)(totalDataNum - numOfDataPointWanted) * posPercent / gVariable.PERCENTAGE_VALUE_FOR_ONE_CURVE);
                }

                num = readAllDataToArrayByNum(databaseName, tableName, indexForThisDataType, dataStartPosition, numOfDataPointWanted, totalDataNum);
                //                gVariable.oneCurvePointNumNeeded = numOfDataPointWanted;
                //                gVariable.oneCurveStartPointPos = dataStartPosition;

                return num;
            }
            catch (Exception ex)
            {
                Console.WriteLine("oneCuve data reading all data fail!" + ex);
                return 0;
            }
        }


        //indexForThisDataType: for example there are 3 kind of data(3 items) for quality data table, indexForThisDataType = 0, means the first item
        public static int readAllDataToArrayByNum(String databaseName, string tableName, int indexForThisDataType, int dataStartPosition, int numOfDataPointWanted, int totalDataNum)
        {
            int i, j, k, v, w;
            int addedPointIndex;
            int numOfDataPointDisplayed;
            int dataEndPosition;
            int interval, delta, div, addPointForinterval;
            int time;
            float f;
            string str;
            MySqlDataReader myReader;

            try
            {
                lock (readAllLocker)
                {
                    MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                    myConnection.Open();
                    MySqlCommand myCommand = myConnection.CreateCommand();

                    //get rules for data selection from data file, if total number of points less than maxDataForOneScreen, just take all
                    //if larger, we may need to choose one from several data points, now define the rule of selection, div is selection base
                    if (numOfDataPointWanted <= gVariable.maxDataForOneScreen)
                    {
                        numOfDataPointDisplayed = numOfDataPointWanted;

                        v = dataStartPosition + 1;
                        w = dataStartPosition + numOfDataPointWanted + 1;
                        //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                        str = "select * from `" + tableName + "` where id >= " + v + " && id <= " + w;
                        myCommand.CommandText = str;

                        myReader = myCommand.ExecuteReader();
                        i = 0;
                        while (myReader.Read())
                        {
                            time = myReader.GetInt32(TIME_VALUE_IN_DATABASE);
                            f = (float)myReader.GetDouble(DATA_VALUE_IN_DATABASE + indexForThisDataType);

                            gVariable.oneCurveTimeInPoint[i] = time;
                            gVariable.oneCurveDataInPoint[i] = f;
                            gVariable.oneCurveIndexInPoint[i] = i + v;

                            i++;
                            if (i > numOfDataPointWanted)
                                break;
                        }
                        myReader.Close();
                    }
                    else
                    {
                        numOfDataPointDisplayed = gVariable.maxDataForOneScreen;

                        div = numOfDataPointWanted / gVariable.maxDataForOneScreen;
                        if (div * gVariable.maxDataForOneScreen != numOfDataPointWanted)
                            div++;

                        //we need to add this number of data to the choosed data list
                        delta = gVariable.maxDataForOneScreen - numOfDataPointWanted / div;

                        if (delta == 0)
                        {
                            interval = gVariable.maxDataForOneScreen;
                        }
                        else
                        {
                            //div selection method is still not enough for data selection, we need to add more data point to data list
                            //every interval point we need to add one to choosed list, until delta number of points are added
                            interval = numOfDataPointWanted / delta;
                        }
                        //we need to add this data point at the middle of div
                        addPointForinterval = div / 2;

                        addedPointIndex = 0;  //number of data points that are already added for intervl purpose
                        dataEndPosition = numOfDataPointWanted + dataStartPosition - 1;
                        j = dataStartPosition; //used for div checking
                        k = dataStartPosition;  //used for interval checking
                        for (i = dataStartPosition; i < dataEndPosition; )
                        {
                            v = i + 1;
                            if (v <= totalDataNum)
                            {
                                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                                str = "select * from `" + tableName + "` where id = " + v;
                                myCommand.CommandText = str;

                                myReader = myCommand.ExecuteReader();
                                myReader.Read();

                                time = myReader.GetInt32(TIME_VALUE_IN_DATABASE);
                                f = (float)myReader.GetDouble(DATA_VALUE_IN_DATABASE + indexForThisDataType);

                                gVariable.oneCurveTimeInPoint[addedPointIndex] = time;
                                gVariable.oneCurveDataInPoint[addedPointIndex] = f;
                                gVariable.oneCurveIndexInPoint[addedPointIndex] = v;

                                myReader.Close();

                                addedPointIndex++;
                            }

                            if (j + div == k + interval)
                            {
                                j += div;
                                k += interval;
                                i = j;

                                v = i + addPointForinterval;
                                if (v <= totalDataNum)
                                {
                                    //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                                    str = "select * from `" + tableName + "` where id = " + v;
                                    myCommand.CommandText = str;

                                    myReader = myCommand.ExecuteReader();
                                    myReader.Read();

                                    time = myReader.GetInt32(TIME_VALUE_IN_DATABASE);
                                    f = (float)myReader.GetDouble(DATA_VALUE_IN_DATABASE);

                                    gVariable.oneCurveTimeInPoint[addedPointIndex] = time;
                                    gVariable.oneCurveDataInPoint[addedPointIndex] = f;
                                    gVariable.oneCurveIndexInPoint[addedPointIndex] = i;

                                    myReader.Close();

                                    addedPointIndex++;
                                }
                            }
                            else if (j + div > k + interval)
                            {
                                k += interval;
                                i = k;
                            }
                            else // if (j + div < k + interval)
                            {
                                j += div;
                                i = j;
                            }

                            if (addedPointIndex >= gVariable.maxDataForOneScreen)
                                break;
                        }
                    }
                    myConnection.Close();
                }
                return numOfDataPointDisplayed;
            }
            catch (Exception ex)
            {
                Console.WriteLine("oneCuve data reading all data fail!" + ex);
                return 0;
            }
        }

        public static void deleteDatabase(String databaseName)
        {
//            just for testing this function
//            readDatabaseToExcel(); 
            try
            {
                String createString = "drop database if exists " + databaseName;

                MySqlConnection myConnection = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(createString, myConnection);
                myConnection.Open();
                cmd.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "Delete database fail!" + ex);
            }
        }

        public static void clearTable(String databaseName, String tableName)
        {
            String str;
            MySqlCommand cmd;
            MySqlConnection myConnection;

            try
            {
                //delete and truncate are similar, but after delete function, new ID will still follow the original ID, while after truncate, ID will start from 1  
//                str = "delete from `" + tableName + "`";
                str = "truncate table `" + tableName + "`";
                myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                cmd = new MySqlCommand(str, myConnection);
                myConnection.Open();
                cmd.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "Clear table fail!" + ex);
            }
        }

        public static void deleteTable(string databaseName, String tableName)
        {
            try
            {
                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                String clearDataTableString = "drop table if exists `" + tableName + "`";

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                MySqlCommand cmd = new MySqlCommand(clearDataTableString, myConnection);
                myConnection.Open();
                cmd.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("drop table fail!" + ex);
            }
        }

        //get title string from an excel file 
        //firstLineChinese: 0 get the second line of an excel file, normally English column name 
        //                  1 get the first line of an excel file, normally Chinese characters in our project  
        //return: number of items in this table
        public static int getListTitleFromExcel(string fileName, ref string [] titleArray, int firstLineOrSecond)
        {
            int i;
            DataTable excelTable;

            i = -1;
            try
            {
                if (fileName == null)
                    return -1;

                if (firstLineOrSecond == gVariable.EXCEL_FIRSTLINE_TITLE)
                    excelTable = readExcelToDataTable(fileName, gVariable.EXCEL_FIRSTLINE_TITLE);
                else
                    excelTable = readExcelToDataTable(fileName, gVariable.EXCEL_FIRSTLINE_DATA);

                if (excelTable == null)
                    return -1;

                foreach (DataRow dr in excelTable.Rows)
                {
                    titleArray = new string[dr.ItemArray.Length];

                    for (i = 0; i < dr.ItemArray.Length; i++)
                    {
                        titleArray[i] = dr.ItemArray[i].ToString();
                    }
                    break;
                }

                return i;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getListTitleFromExcel() failed in get title string from " + fileName + "! " + ex);
                return -1;
            }
        }


        //get insert string for a database table from its excel file
        //return: number of items in this table
        public static int getDatabaseInsertStringFromExcel(ref string insertString, string fileName)
        {
            int i;
            DataTable excelTable;

            i = -1;
            insertString = null;
            try
            {
                if (fileName == null)
                    return -1;

                excelTable = readExcelToDataTable(fileName, gVariable.EXCEL_FIRSTLINE_TITLE);
                if (excelTable == null)
                    return -1;

                insertString = " value(@id";

                foreach (DataRow dr in excelTable.Rows)
                {
                    for (i = 0; i < dr.ItemArray.Length; i++)
                    {
                        insertString += ",@" + dr.ItemArray[i];
                    }
                    insertString += ")";
                    break;
                }

                return i;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getDatabaseInsertStringFromExcel() failed in get insert string for " + fileName + "! " + ex);
                return -1;
            }
        }

        //create string for a database table from its excel file
        //return: number of items in this table
        public static int getDatabaseCreateStringFromExcel(string tableName, ref string createString, string fileName, int itemLength)
        {
            int i;
            string defaultTableItem = " varchar(" + itemLength + ")";
            DataTable excelTable;

            i = -1;
            try
            {
                if (fileName == null)
                    return -1;

                excelTable = readExcelToDataTable(fileName, gVariable.EXCEL_FIRSTLINE_TITLE);
                if (excelTable == null)
                    return -1;

                createString = "create table " + tableName + " (id int(1) AUTO_INCREMENT primary key";

                foreach (DataRow dr in excelTable.Rows)
                {
                    for (i = 0; i < dr.ItemArray.Length; i++)
                    {
                        createString += ", " + dr.ItemArray[i] + defaultTableItem;
                    }
                    createString += " ) ENGINE = MYISAM CHARSET=utf8";
                    break;
                }

                return i;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getDatabaseCreateStringFromExcel() failed in get create string for " + tableName + "! " + ex);
                return -1;
            }
        }


        //itemLength: we set all item to char type, this is char length
        //addRecordFlag: indicating whether we need to add more record into this database on creation 
        public static int createDataTableFromExcel(string databaseName, string tableName, string fileName, int itemLength, int addRecordFlag)
        {
            int i;
            int ret;
            string path;
            string defaultTableItem = " varchar(" + itemLength + ")";
            string createString;
            DataTable excelTable;

            ret = 1;
            try
            {
                if (fileName == null)
                    return -1;

                path = fileName;
                excelTable = readExcelToDataTable(path, gVariable.EXCEL_FIRSTLINE_TITLE);
                if (excelTable == null)
                    return -1;

                createString = "create table " + tableName + " (id int(1) AUTO_INCREMENT primary key";

                foreach (DataRow dr in excelTable.Rows)
                {
                    for (i = 0; i < dr.ItemArray.Length; i++)
                    {
                        createString += ", " + dr.ItemArray[i] + defaultTableItem;
                    }
                    createString += " ) ENGINE = MYISAM CHARSET=utf8";
                    break;
                }

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                MySqlCommand cmd = new MySqlCommand(createString, myConnection);
                myConnection.Open();
                cmd.ExecuteNonQuery();
                myConnection.Close();

                if (addRecordFlag == 1)
                {
                    ret = writeExcelTableToDatabase(excelTable, databaseName, tableName, null);
                    return ret;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "Table " + tableName + " generating fail:" + ex);
                return ret;
            }
        }

        //read data from excel
        //input:  fileUrl: file url
        //return: DataTable
        public static DataTable readExcelToDataTable(string fileUrl, int firstLine)
        {
            string cmdText;

            //            const string cmdText = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1';";
            //suport both xls and xlsx, HDR=Yes means first line is titel not data
            if (firstLine == gVariable.EXCEL_FIRSTLINE_DATA)
                cmdText = "Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=No; IMEX=1'";
            else //if(firstLine == gVariable.EXCEL_FIRSTLINE_TITLE)
                cmdText = "Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";

            DataTable dt = null;
            OleDbDataAdapter da;
            OleDbConnection conn;

            conn = new OleDbConnection(string.Format(cmdText, fileUrl));
            try
            {
                if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                System.Data.DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                string sheetName = schemaTable.Rows[0]["TABLE_NAME"].ToString().Trim();

                string strSql = "select * from [" + sheetName + "]";
                da = new OleDbDataAdapter(strSql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dt = ds.Tables[0];
                da.Dispose();

                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("readExcelToDataTable() fail for :" + fileUrl + "! " + ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return null;
        }


        //insertString is null, means dTable comes from excel file, so we need to generate insert string by the first line of dTable, it has no column of ID
        //insertString is not null, means dTable comes from database, so has an extra column of ID
        public static int writeExcelTableToDatabase(DataTable dTable, string databaseName, string tableName, string insertString)
        {
            int i;
            int value;
            int row;
            int len;
            int ret;
            int flag;
            int firstByte;
            string str;
            string commandText;
            string[] strArray;

            ret = 0;
            row = 0;
            len = 0;
            flag = 0;
            firstByte = 0;
            strArray = null;
            try
            {
                foreach (DataRow dr in dTable.Rows)
                {
                    if (row == 0)
                    {
                        if (insertString == null)
                        {
                            strArray = new string[dr.ItemArray.Length];
                            insertString = " value(@id";

                            for (i = 0; i < dr.ItemArray.Length; i++)
                            {
                                insertString += ", @" + dr.ItemArray[i];
                                strArray[i] = "@" + dr.ItemArray[i].ToString();
                            }
                            insertString += ")";
                            row = 1;
                            firstByte = 1;
                            len = dr.ItemArray.Length + 1; //excel column number plus 1 for ID at the beginning
                            continue;
                        }
                        else
                        {
                            str = insertString.Remove(insertString.Length - 1);
                            strArray = str.Remove(0, 7).Split(',');

                            firstByte = 0;
                            len = dr.ItemArray.Length; //excel column number
                        }
                    }

                    commandText = "insert into `" + tableName + "`" + insertString;

                    MySqlParameter[] param = new MySqlParameter[len];

                    param[0] = new MySqlParameter("@id", 0);
                    for (i = 1; i < len; i++)
                    {
                        if (dr[i - firstByte] == null)
                            param[i] = new MySqlParameter(strArray[i - firstByte].Trim(), "");
                        else
                        {
                            str = dr[i - firstByte].ToString();
                            value = 0;
                            flag = toolClass.isDigitalNum(str);
                            if(flag == 1)
                            {
                                value = Convert.ToInt32(dr[i - firstByte].ToString().Trim());
                            }
                            if (value > 42000 && value < 44000)
                            {
                                str = DateTime.FromOADate(value).ToString("yyyy-MM-dd");
                                param[i] = new MySqlParameter(strArray[i - firstByte].Trim(), str);
                            }
                            else
                            {
                                param[i] = new MySqlParameter(strArray[i - firstByte].Trim(), dr[i - firstByte].ToString().Trim());
                            }
                        }
                    }

                    ret = databaseNonQueryAction(databaseName, commandText, param, gVariable.appendRecord);

                }
                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine("writeExcelTableToDatabase(" + databaseName + ", " + tableName + ") failed! " + ex);
                return -1;
            }
        }

        /*

        public int readDatabaseToExcel(int index)
        {
            DataTable dt;

            dt = readDatabaseToDataTable(gVariable.basicInfoDatabaseName, index);
            writeDataTableToExcel(index, dt);

            return 0;
        }


        //read database into data table
        //return: 0 OK
        //        otherwise fail
        public static DataTable readDatabaseToDataTable(String databaseName, int tableIndex)
        {
            string str;
            DataTable dt;

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                str = "select * from " + gVariable.infoTableName[tableIndex];
                myCommand.CommandText = str;

                using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(myCommand.CommandText, myConnection))
                {
                    dt = new DataTable();
                    dataAdapter.Fill(dt);
                }

                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Read database to data table failed" + ex);
                return null;
            }
            return dt;
        }

        //write Datatable into excel
        //input:  fileUrl file URL
        //        dt DataTable from database
        //return: 0 OK
        public int writeDataTableToExcel(int tableIndex, DataTable dt)
        {
            string title = "";
            try
            {
                FileStream fs = new FileStream(writeExcelURL[tableIndex], FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(new BufferedStream(fs), System.Text.Encoding.Default);

                for (int i = 0; i < dt.Columns.Count - 1; i++)
                {
                    title += infoTableColumnName[tableIndex, i] + ",";
//                    title += dt.Columns[i].ColumnName + ","; //column title, switch to next column
                }

                title = title.Substring(0, title.Length - 1) + "\n";
                sw.Write(title);

                foreach (DataRow row in dt.Rows)
                {
                    string line = "";
                    for (int i = 1; i < dt.Columns.Count; i++)
                        line += row[i].ToString().Trim() + ","; //content, switch to next column

                    line = line.Substring(0, line.Length - 1) + "\n";
                    sw.Write(line);
                }
                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Write excel file failed" + ex);
                return -1;
            }
            return 0;
        }
*/
        public static int searchForOneRecordFromDatabase(String databaseName, string tableName, string columnName, string columnValue)
        {
            int ret;
            string str;
            MySqlDataReader myReader;

            ret = 0;
            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                str = "select * from `" + tableName + "` where " + columnName + " = " + "\'" + columnValue + "\'";

                myCommand.CommandText = str;

                myReader = myCommand.ExecuteReader();
                if (myReader != null)
                {
                    if (myReader.Read())
                        ret = myReader.GetInt32(0);
                    else
                        ret = -1;
                }
                else
                    ret = -1;
                myReader.Close();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "Get" + tableName + "upper and lower limit fail!" + ex);
                return -1;
            }

            return ret;
        }

        //get record number in a table by conditions
        public static int getRecordNumByColumnContent(String databaseName, String tableName, string[] columnName, string[] columnContent)
        {
            int i, j;
            int num;
            string str;

            num = 0;
            try
            {
                i = columnName.Length;

                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                str = "select count(*) from `" + tableName + "` where ";

                for (j = 0; j < i; j++)
                {
                    if (j != 0)
                        str += " and ";
                    str += columnName[j] + " = " + "\'" + columnContent[j] + "\'";
                }

                myCommand.CommandText = str;
                num = Convert.ToInt32(myCommand.ExecuteScalar());

                myConnection.Close();

                return num;
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "Get record num in table " + tableName + " failed!" + ex);
                return -1;
            }
        }

        //read all columns for a record (column index is defined by columnNumNeeded, starts from column one, because column 0 is ID)
        //tableName: table name in database
        //index: index for record in this table that need to be read out
        //dataArray[]: buffer that contains read out data
        //columnNumNeeded: how many column contents in one record do we need (start from column one)
        //return: = -1 fail
        //        >= 0 OK
        public static int getOneWholeRecordFromDatabaseByIndex(String databaseName, string tableName, int index, string[] dataArray)
        {
            int i;
            int ret;
            string str;

            MySqlDataReader myReader;

            ret = 0;
            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                str = "select * from `" + tableName + "` where id = " + (index + 1);  //all index in mySQL starts from 1, not 0, so we need to add 1

                myCommand.CommandText = str;

                myReader = myCommand.ExecuteReader();
                if (myReader != null)
                {
                    if (!myReader.Read())  //no more data in this table
                        ret = -1;
                    else
                    {
                        //We don't need record id, so start from index 1
                        for (i = 1; i < myReader.FieldCount; i++)
                        {
                            if (!myReader.IsDBNull(i))
                                dataArray[i - 1] = myReader.GetString(i);
                            else
                                dataArray[i - 1] = "";
                        }
                    }
                }
                else
                    ret = -1;

                myReader.Close();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "Get" + tableName + "record content by index failed, maybe index too large" + ex);
                return -1;
            }

            return ret;
        }

        //read all columns for a record 
        //tableName: table name in database
        //columnName:name of this column
        //columnContent: content of this column
        //index:  we search for records needed from this index
        //contArray: output record
        //return: = -1 fail
        //        >= 0 OK
        public static int getOneWholeRecordFromDatabaseByOneStrColumn(String databaseName, string tableName, string columnName, string columnContent, string[] dataArray)
        {
            int i;
            int ret;
            string str;

            MySqlDataReader myReader;

            ret = 0;
            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                str = "select * from `" + tableName + "` where " + columnName + " = " + "\'" + columnContent + "\'";

                myCommand.CommandText = str;

                myReader = myCommand.ExecuteReader();
                if (myReader != null)
                {
                    if (!myReader.Read())  //no more data in this table
                        ret = -1;
                    else
                    {
                        ret = Convert.ToInt32(myReader.GetString(0));
                        //We don't need record id, so start from index 1
                        for (i = 1; i < myReader.FieldCount; i++)
                        {
                            if (!myReader.IsDBNull(i))
                                dataArray[i - 1] = myReader.GetString(i);
                            else
                                dataArray[i - 1] = "";
                        }
                    }
                }
                else
                    ret = -1;

                myReader.Close();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "Get" + tableName + "record content by index failed, maybe index too large" + ex);
                return -1;
            }

            return ret;
        }

        //read one columns for a record (column name is defined by columnName)
        //tableName: table name in database
        //columnName: column name in this table
        //index: index for record in this table that need to be read out
        //return: column contents
        public static string getOneRecordFromDatabaseByIndex(String databaseName, string tableName, string columnName, int index)
        {
//            int ret;
            int i;
            string str;
            string returnStr;

            MySqlDataReader myReader;

            i = 0;
            returnStr = null;
            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                str = "select * from `" + tableName + "` where id = " + index;

                myCommand.CommandText = str;

                myReader = myCommand.ExecuteReader();
                if (myReader != null)
                {
                    if (myReader.Read())
                        returnStr = myReader.GetString(i);
                    else
                        returnStr = null;
                }
                else
                    returnStr = null;
                myReader.Close();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "Get" + tableName + "record content by index failed, maybe index too large" + ex);
                return null;
            }

            return returnStr;
        }

        //index is current index, try to find next record with the same column content, if index is 0, we need to find index 1
        public static int getNextRecordByOneStrColumn(String databaseName, string tableName, string columnName, string columnContent, int index, string[] dataArray)
        {
            int i;
            int ret;
            int recordNum;
            string str;

            MySqlDataReader myReader;

            ret = -1;
            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                str = "select count(*) from `" + tableName + "`";
                myCommand.CommandText = str;
                recordNum = Convert.ToInt32(myCommand.ExecuteScalar());

                if (recordNum == 0)
				{
	                myConnection.Close();
                    return -1;
				}

                str = "select * from " + tableName + " where id > " + index + " and " + columnName + " = " + "\'" + columnContent + "\'";
                myCommand.CommandText = str;

                myReader = myCommand.ExecuteReader();
                if (myReader != null)
                {
                    if (!myReader.Read())  //no more data in this table
                        ret = -1;
                    else
                    {
                        //get index for this record
                        ret = Convert.ToInt32(myReader.GetString(0));

                        if (dataArray != null)
                        {
                            //We don't need record id, so start from index 1
                            for (i = 1; i < myReader.FieldCount; i++)
                            {
                                if (!myReader.IsDBNull(i))
                                    dataArray[i - 1] = myReader.GetString(i);
                                else
                                    dataArray[i - 1] = "";
                            }
                        }
                    }
                }
                else
                    ret = -1;

                myReader.Close();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "Get" + tableName + " record content by index failed!!" + ex);
                return -1;
            }

            return ret;
        }


        //read one columns for a record (column name is defined by columnName)
        //tableName: table name in database
        //commandText: 
        //index: index for column in this record that need to be read out
        //return: column contents
        public static string getColumnInfoByCommandText(String databaseName, string tableName, string commandText, int index)
        {
            //string str;
            string returnStr;

            MySqlDataReader myReader;

            returnStr = null;
            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = commandText;

                myReader = myCommand.ExecuteReader();
                if (myReader != null)
                {
                    if (myReader.Read())
                    {
                        if (!myReader.IsDBNull(index))
                            returnStr = myReader.GetString(index);  //column index
                        else
                            returnStr = null;
                    }
                    else
                        returnStr = null;
                }
                else
                    returnStr = null;

                myReader.Close();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("database " + databaseName + "Get" + tableName + " and read last record, then return the " + index + "column content fail! " + ex);
                return null;
            }

            return returnStr;
        }

        //read one columns for a record (column name is defined by columnName)
        //tableName: table name in database
        //columnName: column name in this table
        //index: index for record in this table that need to be read out
        //return: column contents
        public static string getAnothercolumnFromDatabaseByOneColumn(String databaseName, string tableName, string columnName1, string columnContent1, int columnNameIndex2)
        {
            string str;
            MySqlDataReader myReader;

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                //when table name has special characters like '-', we may fail in table processing, like a table name of aa-bb, we need to set it to `aa-bb' to deal with a table
                str = "select * from `" + tableName + "` where " + columnName1 + " = " + "\'" + columnContent1 + "\'";

                myCommand.CommandText = str;

                myReader = myCommand.ExecuteReader();
                if (myReader != null)
                {
                    if (!myReader.Read())
                        str = null;
                    else
                        str = myReader.GetString(columnNameIndex2);
                }
                else
                    str = null;

                myReader.Close();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("writeDataToDispatchListTable() " + databaseName + "Get" + tableName + "record content by index failed, maybe index too large" + ex);
                return null;
            }

            return str;
        }

        //we start dispatch is declared, we need to remove temprary tables
        public static void removeDummyDatabaseTable(string databaseName)
        {
            clearTable(databaseName, "dummy_craft");
            clearTable(databaseName, "dummy_volcur");
        }
    }
}
