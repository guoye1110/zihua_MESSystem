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
using MESSystem.common;

namespace MESSystem.APSDLL
{
    public partial class APSProcess
    {
        const int MAX_NUM_MACHINE_TYPE = 3;
        const int MAX_NUM_MACHINE_ONE_TYPE = 5;

        const int MACHINE_TYPE_CAST = 0;
        const int MACHINE_TYPE_PRINT = 1;
        const int MACHINE_TYPE_SLIT = 2;
        const int MACHINE_TYPE_FEED = 3;

        const int FEED_MACHINE_1 = 0;
        const int FEED_MACHINE_2 = 1;
        const int FEED_MACHINE_3 = 2;
        const int FEED_MACHINE_4 = 3;
        const int FEED_MACHINE_5 = 4;

        //const int CAST_MACHINE_START = 5;
        const int CAST_MACHINE_1 = 5;
        const int CAST_MACHINE_2 = 6;
        const int CAST_MACHINE_3 = 7;
        const int CAST_MACHINE_4 = 8;
        const int CAST_MACHINE_5 = 9;

        const int PRINT_MACHINE_X = -1;
        const int PRINT_MACHINE_UNKNOWN = 0;
        //const int PRINT_MACHINE_START = 10;
        const int PRINT_MACHINE_2 = 10;
        const int PRINT_MACHINE_3 = 11;
        const int PRINT_MACHINE_4 = 12;

        //const int SLIT_MACHINE_START = 13;
        const int SLIT_MACHINE_1 = 13;
        const int SLIT_MACHINE_3 = 14;
        const int SLIT_MACHINE_5 = 15;
        const int SLIT_MACHINE_6 = 16;
        const int SLIT_MACHINE_7 = 17;

        const int ONE_DISPATCH_WEIGHT = 20;   //normal output weight for a dispatch
        const int APS_MACHINE_NUM = 18;

        const int SALES_ORDER_INDEX = 2;
        const int DISPATCH_CODE_INDEX = 3;
        const int START_TIME_INDEX = 4;
        const int START_TIME_STAMP_INDEX = 5;
        const int KEEP_DURATION_INDEX = 6;

        const int salesOrderMax = 20;

        const int MAX_TIME_STAMP = 2000000000;

        const int LOW_SPEED_FEED_MACHINE = 0;
        const int MEDIUM_SPEED_FEED_MACHINE = 1;
        const int HIGH_SPEED_FEED_MACHINE = 2;

        const int LOW_SPEED_CAST_MACHINE = 3;
        const int MEDIUM_SPEED_CAST_MACHINE = 4;
        const int HIGH_SPEED_CAST_MACHINE = 5;

        const int LOW_SPEED_PRINT_MACHINE = 6;
        const int HIGH_SPEED_PRINT_MACHINE = 7;

        const int LOW_SPEED_SLIT_MACHINE = 8;

        const double LOW_SPEED = 0.45;
        const double HIGH_SPEED = 1.2;

        const int FORWARD_APS_PLAN = 0;
        const int REVERSED_APS_PLAN = 1;

        //an hour after cast process starts, print process starts if print is needed
        const int GAP_BETWEEN_CAST_PRINT = 3600;
        //an hour after cast process starts, slit process starts if print is not needed
        const int GAP_BETWEEN_CAST_SLIT = 3600;
        //an hour after print process starts, slit process starts
        const int GAP_BETWEEN_PRINT_SLIT = 3600;

        int[] outputNumForOneDispatch = { 15, 18, 40, 18, 40, 15, 18, 40, 18, 40, 50, 60, 60, 50, 50, 50, 50, 50 };  //different machine has different output ability, this is output weight based on one dispatch (12 hours)
 
        //string[] operatorNameArray = { "刘刚", "黄晓宏", "张明炯", "李晓霞" };
        string[] processNameArray = { "上料工序", "流延工序", "印刷工序", "分切工序" };
        string[] workshopNameArray = { "流延车间", "流延车间", "印刷车间", "流延车间" };
        string[] machineNameArray; 

        const string BREATHABLE_FILM = "MPF";
        const string UNBREATHABLE_FILM = "CPE";

        const int APS_TOTAL_PERIOD = (60 * 24);  //2 months' time

        const int MIN_HOUR_FOR_VACANCY = 4; //if we a machine has 4 hours of free time, it can be regarded as vacancy, we can insert a small dispatch in this period 
        const int MAX_NUM_VACANT_PERIOD = 500; //max number of vacant period in this 2 month of APS time

        gVariable.salesOrderStruct salesOrderImpl;
        gVariable.productStruct productImpl;
        gVariable.BOMListStruct BOMImpl;
        gVariable.castCraftStruct castCraftImpl;
        gVariable.castQualityStruct castQualityImpl;
        gVariable.printCraftStruct printCraftImpl;
        gVariable.printQualityStruct printQualityImpl;
        gVariable.slitCraftStruct slitCraftImpl;
        gVariable.slitQualityStruct slitQualityImpl;

        string[,] machineWorkingPlanStatus = new string[APS_MACHINE_NUM, APS_TOTAL_PERIOD];  //60 for 2 months, 24 for 24 hors
        int[,] machineVacantDuration = new int[APS_MACHINE_NUM, MAX_NUM_VACANT_PERIOD * 2];  //first: start time, second: duration; third: start time, fouth: duration ... maximum 500 pieces

        //record start/end time for the best plan of all machines by now 
        int[] startTimeTypeMachine = new int[MAX_NUM_MACHINE_TYPE];
        int[] endTimeTypeMachine = new int[MAX_NUM_MACHINE_TYPE];
        //record machine used for the best plan by now, maybe next machine is better
        int[] machineIndexForAllType = new int[MAX_NUM_MACHINE_TYPE + 1];

        int calendarStartTimeStamp;

        //machine that would be used in APS
        int[] castMachineList;
        int[] printMachineList;
        int[] slitMachineList;

        //record start/end time for the cast/print/slit machine in the current plan
        int castStartTimeStamp;
        int castEndTimeStamp;
        int printStartTimeStamp;
        int printEndTimeStamp;
        int slitStartTimeStamp;
        int slitEndTimeStamp;

        int reversedAPS;

        public APSProcess()
        {
            initVariables();
        }

        void initVariables()
        {
            int i, j;

            BOMImpl.materialName = new string[gVariable.maxMaterialTypeNum];
            BOMImpl.materialCode = new string[gVariable.maxMaterialTypeNum];
            BOMImpl.materialQuantity = new int[gVariable.maxMaterialTypeNum];

            machineNameArray = new string[APS_MACHINE_NUM];
            reversedAPS = FORWARD_APS_PLAN;

            for (i = 0; i < APS_MACHINE_NUM; i++)
            {
                for (j = 0; j < APS_TOTAL_PERIOD; j++)
                    machineWorkingPlanStatus[i, j] = null;

            }
        }

        public void runAPSProcess(int salesOrderID, int[] assignedMachineArray, int requiredStartTimeStamp, int requiredCompleteTimeStamp)
        {
            int ret;
            int APSStartTimeStamp;
            int APSDeliveryTimeStamp;

            //machine ID and machine name
            getMachineInfoFromDatabase();
            
            //we need to get the newest material info from ERP before APS, so we know whether we have enough material for APS and pop up warning message if not enough
            getMaterialInfoFromERP();

            //get sales order info and product info
            getOrderAndproductForAPS(salesOrderID);

            calendarStartTimeStamp = ConvertDateTimeInt(DateTime.Now.Date.AddDays(1));
            //APS operator assigned a start time
            if (requiredStartTimeStamp != -1)
                APSStartTimeStamp = requiredStartTimeStamp;  //
            else
                APSStartTimeStamp = ConvertDateTimeInt(DateTime.Now.Date.AddDays(1)) + 8 * 3600;  //new work shift start from 8:00 the next day, so APS stats from 8:00 in the morning of the next day

            if (requiredCompleteTimeStamp != -1)
            {
                reversedAPS = REVERSED_APS_PLAN;
                APSDeliveryTimeStamp = requiredCompleteTimeStamp;

                //get machine plan table data to machineWorkingPlanStatus[], to see when the machine is occupied and when it is free for APS
                getWorkingPlanForAllMachines(reversedAPS, APSDeliveryTimeStamp - APS_TOTAL_PERIOD, APSDeliveryTimeStamp);
            }
            else
            {
                reversedAPS = FORWARD_APS_PLAN;
                APSDeliveryTimeStamp = toolClass.timeStringToTimeStamp(salesOrderImpl.deliveryTime);

                //get machine plan table data to machineWorkingPlanStatus[], to see when the machine is occupied and when it is free for APS
                getWorkingPlanForAllMachines(reversedAPS, calendarStartTimeStamp, calendarStartTimeStamp + APS_TOTAL_PERIOD * 3600);
            }

            //check whether all material needed for this sale order are ready, if not pop up a message box for warning. We need to get an answer from the salesman when the material will be available,
            //if we know when will the material will be available, we can input this date in our APS UI screen as APS start time
            if (checkForMaterialReadiness(productImpl.BOMCode, Convert.ToInt32(salesOrderImpl.requiredNum)) < 0)
            {
                //failed in material readiness, return directly
                return;  
            }

            ret = startAPSAction(reversedAPS, salesOrderID, APSStartTimeStamp, APSDeliveryTimeStamp, assignedMachineArray);
            if (ret == 0)
            {
            }

            checkSecondCondition();  //
        }

        public void cancelAPSProcess(int salesOrderID)
        {
            int i;
            string databaseName;
            string commandText;

            commandText = "select * from `" + gVariable.salesOrderTableName + "` where id = " + (salesOrderID);
            mySQLClass.readSalesOrderInfo(ref salesOrderImpl, commandText);

            commandText = "delete from `" + gVariable.globalDispatchTableName + "` where salesOrderCode = '" + salesOrderImpl.salesOrderCode + "'";
            mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

            //delete all dispatch table in dispatch list
            for (i = 0; i < APS_MACHINE_NUM; i++)
            {
                databaseName = gVariable.DBHeadString + (i + 1).ToString().PadLeft(3, '0');
                commandText = "delete from `" + gVariable.machineWorkingPlanTableName + "` where salesOrderCode = '" + salesOrderImpl.salesOrderCode + "'";
                mySQLClass.pureDatabaseNonQueryAction(databaseName, commandText);
                mySQLClass.redoIDIncreamentAfterRecordDeleted(databaseName, gVariable.machineWorkingPlanTableName, gVariable.machineWorkingPlanFileName);
            }
            mySQLClass.redoIDIncreamentAfterRecordDeleted(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, gVariable.globalDispatchFileName);

            commandText = "delete from `" + gVariable.globalMaterialTableName + "` where salesOrderCode = '" + salesOrderImpl.salesOrderCode + "'";
            mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

            mySQLClass.redoIDIncreamentAfterRecordDeleted(gVariable.globalDatabaseName, gVariable.globalMaterialTableName, gVariable.globalMaterialFileName);
        }


        void getMachineInfoFromDatabase()
        {
            int i;
            int len;
            string commandText;
            string[,] tableArray;

            try
            {
                commandText = "select * from `" + gVariable.machineTableName + "`";

                //get machine info
                tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (tableArray == null)
                {
                    MessageBox.Show("读取 MES 基础数据中的设备列表出错", "提示信息", MessageBoxButtons.OK);
                    return;
                }

                len = tableArray.GetLength(0);
                for(i = 0; i < len; i++)
                    machineNameArray[i] = tableArray[i, 3];
            }
            catch (Exception ex)
            {
                Console.WriteLine("getMachineInfoFromDatabase() failed!" + ex);
            }
        }
        
        void getMaterialInfoFromERP()
        {

        }

        //put machine working status into buffer of machineWorkingPlanStatus, it inludes the status for all machines for 2 month starting from APSStartDateString 
        //2 month are separated to 60 * 24 hours, -1 means vacant, >=0 means the ID in global sales order table
        //APSStartDateString should be like "2018-12-24"
        void getWorkingPlanForAllMachines(int reversedAPS, int startTimeStamp, int completeTimeStamp)
        {
            int i, j, k;
            int start;
            int dispatchNum;
            int durationTime;
            string commandText;
            string databaseName;
            string[,] tableArray;
            string[] salesOrderArray = new string[salesOrderMax];

            try
            {
                commandText = "select * from `" + gVariable.machineWorkingPlanTableName + "` where EffectiveOrNot = \'1\' and timeStamp >= \'" + startTimeStamp + "\' and timeStamp <= \'" + completeTimeStamp + "\'";

                //display all dispatches for different machines
                for (i = 0; i < APS_MACHINE_NUM + 1; i++)
                {
                    databaseName = gVariable.DBHeadString + (i + 1).ToString().PadLeft(3, '0');

                    //get machine plan table
                    tableArray = mySQLClass.databaseCommonReading(databaseName, commandText);
                    if (tableArray == null)
                        continue;

                    dispatchNum = tableArray.GetLength(0);

                    //go through all dispatches one by one, get store dispatch code
                    for (j = 0; j < dispatchNum; j++)
                    {
                        start = (Convert.ToInt32(tableArray[j, START_TIME_STAMP_INDEX]) - startTimeStamp) / 3600;  //from second value to hour value;
                        durationTime = Convert.ToInt32(tableArray[j, KEEP_DURATION_INDEX]); 

                        for(k = start; k < start + durationTime; k++)
                        {
                            if (k >= APS_TOTAL_PERIOD)  //we don't consider plans later than APS_TOTAL_PERIOD 
                                break;

                            machineWorkingPlanStatus[i, k] = tableArray[j, DISPATCH_CODE_INDEX];  //store dispatch code into array
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("getWorkingPlanForAlmachines() failed!" + ex);
            }
        }

        private void getOrderAndproductForAPS(int salesOrderID)
        {
            string commandText;

            try
            {
                commandText = "select * from `" + gVariable.salesOrderTableName + "` where id = " + (salesOrderID);
                mySQLClass.readSalesOrderInfo(ref salesOrderImpl, commandText);

                commandText = "select * from `" + gVariable.productTableName + "` where productCode = '" + salesOrderImpl.productCode + "'";
                mySQLClass.readProductInfo(ref productImpl, commandText);

                commandText = "select * from `" + gVariable.bomTableName + "` where BOMCode = '" + productImpl.BOMCode + "'";
                mySQLClass.readBOMInfo(ref BOMImpl, commandText);

                commandText = "select * from `" + gVariable.castCraftTableName + "` where castCraft = '" + productImpl.castCraft + "'";
                mySQLClass.readCastCraftInfo(ref castCraftImpl, commandText);

                commandText = "select * from `" + gVariable.castQualityTableName + "` where castQuality = '" + productImpl.castQuality + "'";
                mySQLClass.readCastQualityInfo(ref castQualityImpl, commandText);

                commandText = "select * from `" + gVariable.printCraftTableName + "` where printcraft = '" + productImpl.printCraft + "'";
                mySQLClass.readPrintCraftInfo(ref printCraftImpl, commandText);

                commandText = "select * from `" + gVariable.printQualityTableName + "` where printQuality = '" + productImpl.printQuality + "'";
                mySQLClass.readPrintQualityInfo(ref printQualityImpl, commandText);

                commandText = "select * from `" + gVariable.slitCraftTableName + "` where slitCraft = '" + productImpl.slitCraft + "'";
                mySQLClass.readSlitCraftInfo(ref slitCraftImpl, commandText);

                commandText = "select * from `" + gVariable.slitQualityTableName + "` where slitquality = '" + productImpl.slitQuality + "'";
                mySQLClass.readSlitQualityInfo(ref slitQualityImpl, commandText);
            }
            catch (Exception ex)
            {
                Console.WriteLine("getOrderAndProductInfo failed! ", ex);
            }
        }

        //how long (in hours) will this machine need to complete one dispatch
        int getSalesOrderWorkingTime(int machineIndex)
        {
            return (int)((Convert.ToInt32(salesOrderImpl.requiredNum) * 12) / outputNumForOneDispatch[machineIndex]);  
        }

        private int startAPSAction(int reversedAPS, int salesOrderID, int APSStartTimeStamp, int APSDeliveryTimeStamp, int[] assignedMachineArray)
        {
            //int i;
            int ret;
            int index;
            int workingTimeDeltaStamp;
            int salesOrderStartTimeStamp; 
            int salesOrderEndTimeStamp;
            int numForThisDispatch, numForSalesOrderleft;
            string updateStr;

            try
            {
                machineIndexForAllType[MACHINE_TYPE_PRINT] = PRINT_MACHINE_UNKNOWN;

                ret = 0;
                if (reversedAPS == FORWARD_APS_PLAN)
                    ret = generateForwardAPSPlans(productImpl.routeCode, APSStartTimeStamp, APSDeliveryTimeStamp, assignedMachineArray);
//                else //if (reversedAPS == REVERSED_APS_PLAN)
//                    ret = generateReversedAPSPlans(productImpl.routeCode, APSStartTimeStamp, APSDeliveryTimeStamp, assignedMachineArray);
                if (ret < 0)
                    return ret;

                index = 0;

                numForSalesOrderleft = Convert.ToInt32(salesOrderImpl.requiredNum);
                numForThisDispatch = outputNumForOneDispatch[machineIndexForAllType[MACHINE_TYPE_CAST]];

                salesOrderStartTimeStamp = castStartTimeStamp;
                salesOrderEndTimeStamp = slitEndTimeStamp;

                while (true)
                {
                    if (numForThisDispatch > numForSalesOrderleft)
                        numForThisDispatch = numForSalesOrderleft;

                    workingTimeDeltaStamp = getDispatchFromSalesOrder(index, machineIndexForAllType, numForThisDispatch);

                    numForSalesOrderleft -= numForThisDispatch;
                    if (numForSalesOrderleft <= 0)
                        break;

                    index++;
                    castStartTimeStamp += workingTimeDeltaStamp;
                    printStartTimeStamp += workingTimeDeltaStamp;
                    slitStartTimeStamp += workingTimeDeltaStamp;
                }

                //APS success, set the status of this sales order to already scheduled
                updateStr = "update `" + gVariable.salesOrderTableName + "` set orderStatus = '" + gVariable.SALES_ORDER_STATUS_APS_OK + "', APSTime = '" + DateTime.Now.ToString("yy-MM-dd HH:mm" ) +
                            "', planTime1 = '" + GetTimeFromInt(salesOrderStartTimeStamp).ToString("yyyy-MM-dd HH:mm") + "', planTime2 = '" +
                            GetTimeFromInt(salesOrderEndTimeStamp).ToString("yyyy-MM-dd HH:mm") + "' where id = '" + salesOrderID + "'";

                mySQLClass.updateTableItems(gVariable.globalDatabaseName, updateStr);

                return 0;
            
            }
            catch (Exception ex)
            {
                Console.WriteLine("startAPSAction failed! ", ex);
                return -1;
            }
        }

        int generateForwardAPSPlans(string routeCode, int APSStartTimeStamp, int APSDeliveryTimeStamp, int[] assignedMachineArray)
        {
            int i, j, k;
            int tmpResult;
            int gotBestPlan;
            int APSSuccess;
            int salesOrderWorkingTime;
            int[] castMachineList;
            int[] printMachineList;
            int[] slitMachineList;

            try
            {
                APSSuccess = 0;
                //first get cast machine list

                if (assignedMachineArray[MACHINE_TYPE_CAST] != -1)
                {
                    castMachineList = new int[1];
                    castMachineList[0] = assignedMachineArray[MACHINE_TYPE_CAST];
                }
                else
                {
                    switch (routeCode)
                    {
                        case "A00":  //透气膜 -- 2/4 流延机 + 2/4 分切机, 不印刷
                            castMachineList = new int[2];
                            castMachineList[0] = CAST_MACHINE_4;
                            castMachineList[1] = CAST_MACHINE_2;
                            machineIndexForAllType[MACHINE_TYPE_PRINT] = PRINT_MACHINE_X;
                            break;
                        case "A01":  //透气印刷膜 -- 2/4 流延机 + 1/2/3 印刷机 + 2/4 分切机
                            castMachineList = new int[2];
                            castMachineList[0] = CAST_MACHINE_4;
                            castMachineList[1] = CAST_MACHINE_2;
                            break;
                        case "A10":  //非透气膜 -- 1/3/5 流延机 + 1/3/5 分切机
                            castMachineList = new int[3];
                            castMachineList[0] = CAST_MACHINE_5;
                            castMachineList[1] = CAST_MACHINE_3;
                            castMachineList[2] = CAST_MACHINE_1;
                            machineIndexForAllType[MACHINE_TYPE_PRINT] = PRINT_MACHINE_X;
                            break;
                        case "A11":  //非透气印刷膜 -- 1/2/3 流延机 + 1/2/3 印刷机 + 1/2/3 分切机 
                            castMachineList = new int[3];
                            castMachineList[0] = CAST_MACHINE_5;
                            castMachineList[1] = CAST_MACHINE_3;
                            castMachineList[2] = CAST_MACHINE_1;
                            break;
                        default:
                            Console.WriteLine("\r\n product route code error!\r\n");
                            castMachineList = new int[2];
                            castMachineList[0] = CAST_MACHINE_4;
                            castMachineList[1] = CAST_MACHINE_2;
                            machineIndexForAllType[MACHINE_TYPE_PRINT] = PRINT_MACHINE_X;
                            break;
                    }
                }

                for (i = 0; i < MAX_NUM_MACHINE_TYPE; i++)
                {
                    startTimeTypeMachine[i] = -1;
                    endTimeTypeMachine[i] = -1;
                }

                gotBestPlan = 0;

                for (i = 0; i < castMachineList.Length; i++)
                {
                    //total working time for one sales order by using this machine
                    salesOrderWorkingTime = getSalesOrderWorkingTime(castMachineList[i]);

                    //APS started from dispatchStartTimeStamp
                    tmpResult = fillOutPlanForOneMachine(MACHINE_TYPE_CAST, castMachineList[i], APSStartTimeStamp, APSDeliveryTimeStamp, salesOrderWorkingTime);
                    if (tmpResult < 0)
                    {
                        continue;
                    }
                    else
                    {
                        //cast Machine got a plan, try print and slit machine
                        if (machineIndexForAllType[MACHINE_TYPE_PRINT] == PRINT_MACHINE_X)
                        {
                            //if cast machine #3/#5 don't need printing, it will do slit online, so slit machine 3 or 5 is the only choice
                            //if we have a print process, we should use slit machine 1/6/7, which are separate slit machines
                            if (assignedMachineArray[MACHINE_TYPE_SLIT] != -1)
                            {
                                slitMachineList = new int[1];
                                slitMachineList[0] = assignedMachineArray[MACHINE_TYPE_SLIT];
                            }
                            else if (castMachineList[i] == CAST_MACHINE_3)
                            {
                                slitMachineList = new int[1];
                                slitMachineList[0] = SLIT_MACHINE_3;
                            }
                            else if (castMachineList[i] == CAST_MACHINE_5)
                            {
                                slitMachineList = new int[1];
                                slitMachineList[0] = SLIT_MACHINE_5;
                            }
                            else
                            {
                                slitMachineList = new int[3];
                                slitMachineList[0] = SLIT_MACHINE_1;
                                slitMachineList[1] = SLIT_MACHINE_6;
                                slitMachineList[2] = SLIT_MACHINE_7;
                            }

                            //there is no print process for this sales order, so go to slit process directly
                            for (j = 0; j < slitMachineList.Length; j++)
                            {
                                //slit machine need to run after cast machine, so the speed is the same as cast machine, we use salesOrderWorkingTime as a total working time of slit machine for this sales order
                                tmpResult = fillOutPlanForOneMachine(MACHINE_TYPE_SLIT, slitMachineList[j], castStartTimeStamp + GAP_BETWEEN_CAST_SLIT, APSDeliveryTimeStamp, salesOrderWorkingTime);
                                if (tmpResult < 0)
                                {
                                    continue;
                                }

                                //slit Machine got a plan
                                if (startTimeTypeMachine[MACHINE_TYPE_SLIT] == -1 || slitEndTimeStamp < startTimeTypeMachine[MACHINE_TYPE_SLIT])
                                {
                                    //there is no workable plan by now 
                                    //or this new plan is better than the previous one, we use this new one
                                    gotBestPlan = 1;
                                }
                                else if (slitStartTimeStamp > endTimeTypeMachine[MACHINE_TYPE_SLIT])
                                {
                                    //this plan is worse than previous ones, so just discard this
                                }
                                else if (castEndTimeStamp < startTimeTypeMachine[MACHINE_TYPE_CAST])
                                {
                                    //slit plan for the current cast machine and previous best cast machine are similar, now check for cast plan
                                    //new cast plan is better, so we adopt the new plan
                                    gotBestPlan = 1;
                                }

                                if (gotBestPlan == 1)
                                {
                                    //we got the best plan 
                                    startTimeTypeMachine[MACHINE_TYPE_CAST] = castStartTimeStamp;
                                    endTimeTypeMachine[MACHINE_TYPE_CAST] = castEndTimeStamp;
                                    machineIndexForAllType[MACHINE_TYPE_CAST] = castMachineList[i];
                                    machineIndexForAllType[MACHINE_TYPE_FEED] = castMachineList[i] - (CAST_MACHINE_1 - FEED_MACHINE_1);

                                    startTimeTypeMachine[MACHINE_TYPE_SLIT] = slitStartTimeStamp;
                                    endTimeTypeMachine[MACHINE_TYPE_SLIT] = slitEndTimeStamp;
                                    machineIndexForAllType[MACHINE_TYPE_SLIT] = slitMachineList[j];

                                    APSSuccess = 1;
                                }

                                gotBestPlan = 0;
                            }
                            //at the end of this loop, we got the best plan for slit machine by using the current cast machine, then we need to try next cast machine to see if there is a better plan
                        }
                        else
                        {
                            //print process is necessary
                            if (assignedMachineArray[MACHINE_TYPE_PRINT] != -1)
                            {
                                printMachineList = new int[1];
                                printMachineList[0] = assignedMachineArray[MACHINE_TYPE_PRINT];
                            }
                            else
                            {
                                printMachineList = new int[3];
                                printMachineList[0] = PRINT_MACHINE_3;
                                printMachineList[1] = PRINT_MACHINE_4;
                                printMachineList[2] = PRINT_MACHINE_2;
                            }

                            if (assignedMachineArray[MACHINE_TYPE_SLIT] != -1)
                            {
                                slitMachineList = new int[1];
                                slitMachineList[0] = assignedMachineArray[MACHINE_TYPE_SLIT];
                            }
                            else
                            {
                                slitMachineList = new int[3];
                                slitMachineList[0] = SLIT_MACHINE_1;
                                slitMachineList[1] = SLIT_MACHINE_6;
                                slitMachineList[2] = SLIT_MACHINE_7;
                            }
                            //try all print machines by using this cast machine plan (cast machine i)
                            for (j = 0; j < printMachineList.Length; j++)
                            {
                                //slit machine need to run after cast machine, so the speed is the same as cast machine, we use salesOrderWorkingTime as a total working time of slit machine for this sales order
                                tmpResult = fillOutPlanForOneMachine(MACHINE_TYPE_PRINT, printMachineList[j], castStartTimeStamp + GAP_BETWEEN_CAST_PRINT, APSDeliveryTimeStamp, salesOrderWorkingTime);
                                if (tmpResult < 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    //print Machine got a plan
                                    //try all slit machines based on this print machine plan 
                                    for (k = 0; k < slitMachineList.Length; k++)
                                    {
                                        //slit machine need to run after cast machine, so the speed is the same as cast machine, we use salesOrderWorkingTime as a total working time of slit machine for this sales order
                                        tmpResult = fillOutPlanForOneMachine(MACHINE_TYPE_SLIT, slitMachineList[k], printStartTimeStamp + GAP_BETWEEN_PRINT_SLIT, APSDeliveryTimeStamp, salesOrderWorkingTime);
                                        if (tmpResult < 0)
                                        {
                                            continue;
                                        }

                                        //we've got the first or best slit machine plan for this print machine, now compare this plans and the previous best plan to see which one is better
                                        if (startTimeTypeMachine[MACHINE_TYPE_SLIT] == -1 || slitEndTimeStamp < startTimeTypeMachine[MACHINE_TYPE_SLIT])
                                        {
                                            //we got the best plan by now
                                            //it is the first workable plan, input data directly to print and slit array for cast machine 0
                                            //or new print machine has a better slit plan than previous print machines
                                            gotBestPlan = 1;
                                        }
                                        else if (startTimeTypeMachine[MACHINE_TYPE_SLIT] > slitEndTimeStamp)
                                        {
                                            //the new slit plan for this print machine is much worse than the previous best plan recorded in startTimeTypeMachine, just discard the new one
                                        }
                                        else
                                        {
                                            //slit plans are similar, need to continue check for print plan
                                            //this print plan is better than the previoud best plan, use this one
                                            if (printEndTimeStamp < startTimeTypeMachine[MACHINE_TYPE_PRINT])
                                            {
                                                gotBestPlan = 1;
                                            }
                                            else if (printStartTimeStamp > endTimeTypeMachine[MACHINE_TYPE_PRINT])
                                            {
                                                //the new print plan for this cast machine is much worse than the previous best plan recorded in startTimeTypeMachine, just discard the new one
                                            }
                                            else if (castEndTimeStamp < startTimeTypeMachine[MACHINE_TYPE_CAST])
                                            {
                                                //both slit plans and print plans are similar, need to continue check for cast plan
                                                //this cast plan is better than the previoud best plan, use this one
                                                gotBestPlan = 1;
                                            }
                                        }

                                        //record this plan for 4 types of machines
                                        if (gotBestPlan == 1)
                                        {
                                            //we got the best plan 
                                            startTimeTypeMachine[MACHINE_TYPE_CAST] = castStartTimeStamp;
                                            endTimeTypeMachine[MACHINE_TYPE_CAST] = castEndTimeStamp;
                                            machineIndexForAllType[MACHINE_TYPE_CAST] = castMachineList[i];
                                            machineIndexForAllType[MACHINE_TYPE_FEED] = castMachineList[i] - (CAST_MACHINE_1 - FEED_MACHINE_1);

                                            startTimeTypeMachine[MACHINE_TYPE_PRINT] = printStartTimeStamp;
                                            endTimeTypeMachine[MACHINE_TYPE_PRINT] = printEndTimeStamp;
                                            machineIndexForAllType[MACHINE_TYPE_PRINT] = printMachineList[j];

                                            startTimeTypeMachine[MACHINE_TYPE_SLIT] = slitStartTimeStamp;
                                            endTimeTypeMachine[MACHINE_TYPE_SLIT] = slitEndTimeStamp;
                                            machineIndexForAllType[MACHINE_TYPE_SLIT] = slitMachineList[k];

                                            APSSuccess = 1;
                                            gotBestPlan = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //if no plan found, return -1
                if (APSSuccess == 0)
                {
                    string str;

                    str = "订单" + salesOrderImpl.salesOrderCode + "排程失败，请检查交货时间等信息，谢谢！";
                    MessageBox.Show(str, "信息提示", MessageBoxButtons.OK);
                    return -1;

                }
                else
                {
                    //APS successful!
                    castStartTimeStamp = startTimeTypeMachine[MACHINE_TYPE_CAST];
                    castEndTimeStamp = endTimeTypeMachine[MACHINE_TYPE_CAST];

                    printStartTimeStamp = startTimeTypeMachine[MACHINE_TYPE_PRINT];
                    printEndTimeStamp = endTimeTypeMachine[MACHINE_TYPE_PRINT];

                    slitStartTimeStamp = startTimeTypeMachine[MACHINE_TYPE_SLIT];
                    slitEndTimeStamp = endTimeTypeMachine[MACHINE_TYPE_SLIT];
                    
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("generateAPSPlans failed!" + ex);
                return -1;
            }
        }

        //find free time for every machine, then put dispatch to suitable machines to make sure the sales order can be completed by delivery time 
        //machineindex: from 0 to 17, alogether 18 machines
        //APSStartTimeStamp: start time for APS
        //deliveryTimeStamp: delivery time for this sales order
        //return: time stamp for the start time of the plan 
        //        -1 failed to find a plan
        int fillOutPlanForOneMachine(int type, int machineIndex, int APSStartTimeStamp, int APSDeliveryTimeStamp, int salesOrderWorkingTime)
        {
            int j, k;
            int flag;
            int ret;

            ret = -1;
            try
            {
                flag = 0;
                k = 0;

                //APS_TOTAL_PERIOD is 2 month counted by hours, we need to finish a sales order within 2 month
                //j is the offset of hours starting from APSStartTimeStamp
                for (j = 0; j < APS_TOTAL_PERIOD; j++)
                {
                    //this array stores dispatch code in this time slot, if it is null, that means no dispatch for this time
                    if (machineWorkingPlanStatus[machineIndex, j] == null)  //free time for this machine
                    {
                        if (flag == 0)  //remember start position for free time, we need this value to calculate the start point for this dispatch, flag will keep on increasing
                            k = j;

                        if (calendarStartTimeStamp + j * 3600 <= APSStartTimeStamp)
                        {
                            //it is still too early, we need to wait until APSStartTimeStamp to check if the machine is free
                            flag = 0;
                            k = j;
                        }
                        else if (flag >= salesOrderWorkingTime)  //free time larger than 4 hour, we can consider this as vacancy as APS vacant time slot
                        {
                            if (APSStartTimeStamp + j * 3600 < APSDeliveryTimeStamp)
                            {
                                switch(type)
                                {
                                    case MACHINE_TYPE_CAST:
                                        castStartTimeStamp = calendarStartTimeStamp + k * 3600;
                                        castEndTimeStamp = calendarStartTimeStamp + (k + flag) * 3600;
                                        break;
                                    case MACHINE_TYPE_PRINT:
                                        printStartTimeStamp = calendarStartTimeStamp + k * 3600;
                                        printEndTimeStamp = calendarStartTimeStamp + (k + flag) * 3600;
                                        break;
                                    case MACHINE_TYPE_SLIT:
                                        slitStartTimeStamp = calendarStartTimeStamp + k * 3600;
                                        slitEndTimeStamp = calendarStartTimeStamp + (k + flag) * 3600;
                                        break;
                                }
                                ret = 0;
                                break;
                            }
                        }
                        flag++;
                    }
                    else //if(machineWorkingPlanStatus[i, j] != null)  //this machine is busy, we should restart calculating free period again
                    {
                        flag = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("fillOutPlanForOneMachine failed! " + ex);
            }

            return ret;
        }

        int checkForMaterialReadiness(string BOMCode, int plannedNum)
        {
            DialogResult dr;

            if (checkMaterialAndStock(BOMCode, plannedNum) < 0)
            {
                dr = MessageBox.Show("原料数量不足，请问是否从采购人员那里得到了新采购原料的到厂时间？（如果有原料到厂时间我们可以继续派程，否则无法继续）", "需要确认", MessageBoxButtons.YesNo);

                if (dr == DialogResult.No)
                {
                    return -1;
                }

                MessageBox.Show("请在排程画面输入新采购的原料到厂日期后开始排程！", "信息提示", MessageBoxButtons.OK);
            }
            return 0;
        }

        int checkMaterialAndStock(string BOMCode, int plannedNum)
        {
            return 0;
        }


        public int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        //from "1572312336" to DateTime
        public DateTime GetTimeFromString(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        //from 1572312336 to DateTime
        public DateTime GetTimeFromInt(int timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        //
        private int getDispatchFromSalesOrder(int indexInDispatch, int[] machineIndexForAllType, int outputNum)
        {
            int timeV;
            int processIndex;
            int timeStampDelta;
            int finshiTimeStamp;
            int serialNumberIndex;
            int feedMachineIndex;
            int castMachineIndex;
            int printMachineIndex;
            int slitMachineIndex;
            string planTime1;
            string workShift;
            string databaseName;

            processIndex = 0;
            serialNumberIndex = 1;

            feedMachineIndex = machineIndexForAllType[MACHINE_TYPE_FEED];
            castMachineIndex = machineIndexForAllType[MACHINE_TYPE_CAST];
            printMachineIndex = machineIndexForAllType[MACHINE_TYPE_PRINT];
            slitMachineIndex = machineIndexForAllType[MACHINE_TYPE_SLIT];

            planTime1 = GetTimeFromInt(castStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
            timeV = Convert.ToInt32(planTime1.Remove(0, 11).Remove(2));
            if (timeV < 8)
            {
                workShift = "夜班";
                timeStampDelta = 3600 * (8 - timeV);
            }
            else if(timeV < 20)
            {
                workShift = "日班";
                timeStampDelta = 3600 * (20 - timeV);
            }
            else if (timeV < 24)
            {
                workShift = "夜班";
                timeStampDelta = 3600 * (24 - timeV + 8);
            }
            else
            {
                workShift = "日班";
                timeStampDelta = 3600 * 12;
            }

            //finshiTimeStampForReturn = 0;
            try
            {
                gVariable.dispatchSheetStruct dispatchSheet = new gVariable.dispatchSheetStruct();

                //material feeding process
                dispatchSheet.dispatchId = 0;  //not used in our system
                dispatchSheet.machineID = (feedMachineIndex + 1).ToString();
                dispatchSheet.dispatchCode = salesOrderImpl.salesOrderCode + (castMachineIndex + 1) + indexInDispatch.ToString().PadLeft(2, '0') + (processIndex + 1);
                dispatchSheet.planTime1 = planTime1;

                if (castEndTimeStamp > castStartTimeStamp && castEndTimeStamp - castStartTimeStamp < timeStampDelta)
                    finshiTimeStamp = castEndTimeStamp; //this is ythe last cast dispatch, we use end time stamp
                else
                    finshiTimeStamp = castStartTimeStamp + timeStampDelta;  //finish time is start time plus production time needed for output number, normally one shift -> 12 hours

                dispatchSheet.planTime2 = GetTimeFromInt(finshiTimeStamp).ToString("yyyy-MM-dd HH:mm");
                dispatchSheet.productCode = productImpl.productCode;
                dispatchSheet.productName = productImpl.productName;
                dispatchSheet.operatorName = "未定";
                dispatchSheet.plannedNumber = outputNum;
                dispatchSheet.outputNumber = 0;
                dispatchSheet.qualifiedNumber = 0;
                dispatchSheet.unqualifiedNumber = 0;
                dispatchSheet.processName = processNameArray[processIndex];
                dispatchSheet.realStartTime = "";
                dispatchSheet.prepareTimePoint = "";
                dispatchSheet.realFinishTime = "";
                dispatchSheet.status = gVariable.MACHINE_STATUS_DISPATCH_GENERATED;
                dispatchSheet.toolLifeTimes = 0;
                dispatchSheet.toolUsedTimes = 0;
                dispatchSheet.outputRatio = 0;
                dispatchSheet.serialNumber = dispatchSheet.dispatchCode.Remove(0, 1) + "-" + serialNumberIndex.ToString().PadLeft(2, '0');
                dispatchSheet.reportor = "";
                dispatchSheet.workshop = workshopNameArray[processIndex];
                dispatchSheet.workshift = workShift;
                dispatchSheet.salesOrderCode = salesOrderImpl.salesOrderCode;
                dispatchSheet.BOMCode = productImpl.BOMCode;
                dispatchSheet.customer = salesOrderImpl.customer;

                mySQLClass.writeDataToDispatchListTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, dispatchSheet);

                databaseName = gVariable.DBHeadString + dispatchSheet.machineID.ToString().PadLeft(3, '0');
                mySQLClass.writeDataToWorkingPlanTable(databaseName, gVariable.machineWorkingPlanTableName, dispatchSheet, castStartTimeStamp, timeStampDelta);

                generateMaterialList(gVariable.globalDatabaseName, gVariable.globalMaterialTableName, dispatchSheet.dispatchCode, dispatchSheet.machineID, outputNum, productImpl.BOMCode);

                processIndex++;
                //cast process
                dispatchSheet.machineID = (castMachineIndex + 1).ToString();
                dispatchSheet.dispatchCode = salesOrderImpl.salesOrderCode + (castMachineIndex + 1) + indexInDispatch.ToString().PadLeft(2, '0') + (processIndex + 1);
                //cast and feed process stat at the same time
                dispatchSheet.planTime1 = GetTimeFromInt(castStartTimeStamp).ToString("yyyy-MM-dd HH:mm");

                if (castEndTimeStamp > castStartTimeStamp && castEndTimeStamp - castStartTimeStamp < timeStampDelta)
                    finshiTimeStamp = castEndTimeStamp; //this is ythe last cast dispatch, we use end time stamp
                else
                    finshiTimeStamp = castStartTimeStamp + timeStampDelta;  //finish time is start time plus production time needed for output number, normally one shift -> 12 hours
                //finshiTimeStampForReturn = finshiTimeStamp; //this is end time of feed/cast process, we will use this time to start a new dispatch
                dispatchSheet.planTime2 = GetTimeFromInt(finshiTimeStamp).ToString("yyyy-MM-dd HH:mm");
                dispatchSheet.productCode = productImpl.productCode;
                dispatchSheet.productName = productImpl.productName;
                dispatchSheet.operatorName = "未定";
                dispatchSheet.plannedNumber = outputNum;
                dispatchSheet.outputNumber = 0;
                dispatchSheet.qualifiedNumber = 0;
                dispatchSheet.unqualifiedNumber = 0;
                dispatchSheet.processName = processNameArray[processIndex];
                dispatchSheet.realStartTime = "";
                dispatchSheet.prepareTimePoint = "";
                dispatchSheet.realFinishTime = "";
                dispatchSheet.status = gVariable.MACHINE_STATUS_DISPATCH_GENERATED;
                dispatchSheet.toolLifeTimes = 0;
                dispatchSheet.toolUsedTimes = 0;
                dispatchSheet.outputRatio = 0;
                dispatchSheet.serialNumber = dispatchSheet.dispatchCode.Remove(0, 1) + "-" + serialNumberIndex.ToString().PadLeft(2, '0');
                dispatchSheet.reportor = "";
                dispatchSheet.workshop = workshopNameArray[processIndex];
                dispatchSheet.workshift = workShift;
                dispatchSheet.salesOrderCode = salesOrderImpl.salesOrderCode;
                dispatchSheet.BOMCode = productImpl.BOMCode;
                dispatchSheet.customer = salesOrderImpl.customer;

                mySQLClass.writeDataToDispatchListTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, dispatchSheet);
                databaseName = gVariable.DBHeadString + dispatchSheet.machineID.ToString().PadLeft(3, '0');
                mySQLClass.writeDataToWorkingPlanTable(databaseName, gVariable.machineWorkingPlanTableName, dispatchSheet, castStartTimeStamp, timeStampDelta);

                processIndex++;

                if (printMachineIndex != -1)
                {
                    //print process
                    dispatchSheet.machineID = (printMachineIndex + 1).ToString();
                    dispatchSheet.dispatchCode = salesOrderImpl.salesOrderCode + (castMachineIndex + 1) + indexInDispatch.ToString().PadLeft(2, '0') + (processIndex + 1);
                    dispatchSheet.planTime1 = GetTimeFromInt(printStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
                    if (printEndTimeStamp > printStartTimeStamp && printEndTimeStamp - printStartTimeStamp < timeStampDelta)
                        finshiTimeStamp = printEndTimeStamp; //this is ythe last print dispatch, we use end time stamp
                    else
                        finshiTimeStamp = printStartTimeStamp + timeStampDelta;  //finish time is start time plus production time needed for output number, normally one shift -> 12 hours
                    dispatchSheet.planTime2 = GetTimeFromInt(finshiTimeStamp).ToString("yyyy-MM-dd HH:mm");
                    dispatchSheet.productCode = productImpl.productCode;
                    dispatchSheet.productName = productImpl.productName;
                    dispatchSheet.operatorName = "未定";
                    dispatchSheet.plannedNumber = outputNum;
                    dispatchSheet.outputNumber = 0;
                    dispatchSheet.qualifiedNumber = 0;
                    dispatchSheet.unqualifiedNumber = 0;
                    dispatchSheet.processName = processNameArray[processIndex];
                    dispatchSheet.realStartTime = "";
                    dispatchSheet.prepareTimePoint = "";
                    dispatchSheet.realFinishTime = "";
                    dispatchSheet.status = gVariable.MACHINE_STATUS_DISPATCH_GENERATED;
                    dispatchSheet.toolLifeTimes = 0;
                    dispatchSheet.toolUsedTimes = 0;
                    dispatchSheet.outputRatio = 0;
                    dispatchSheet.serialNumber = dispatchSheet.dispatchCode.Remove(0, 1) + "-" + serialNumberIndex.ToString().PadLeft(2, '0');
                    dispatchSheet.reportor = "";
                    dispatchSheet.workshop = workshopNameArray[processIndex];
                    dispatchSheet.workshift = workShift;
                    dispatchSheet.salesOrderCode = salesOrderImpl.salesOrderCode;
                    dispatchSheet.BOMCode = productImpl.BOMCode;
                    dispatchSheet.customer = salesOrderImpl.customer;

                    mySQLClass.writeDataToDispatchListTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, dispatchSheet);
                    databaseName = gVariable.DBHeadString + dispatchSheet.machineID.ToString().PadLeft(3, '0');
                    mySQLClass.writeDataToWorkingPlanTable(databaseName, gVariable.machineWorkingPlanTableName, dispatchSheet, printStartTimeStamp, timeStampDelta);
                }
                processIndex++;

                //slit process
                dispatchSheet.machineID = (slitMachineIndex + 1).ToString();
                dispatchSheet.dispatchCode = salesOrderImpl.salesOrderCode + (castMachineIndex + 1) + indexInDispatch.ToString().PadLeft(2, '0') + (processIndex + 1);
                dispatchSheet.planTime1 = GetTimeFromInt(slitStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
                if (slitEndTimeStamp > slitStartTimeStamp && slitEndTimeStamp - slitStartTimeStamp < timeStampDelta)
                    finshiTimeStamp = slitEndTimeStamp; //this is ythe last slit dispatch, we use end time stamp
                else
                    finshiTimeStamp = slitStartTimeStamp + timeStampDelta;  //finish time is start time plus production time needed for output number, normally one shift -> 12 hours

                dispatchSheet.planTime2 = GetTimeFromInt(finshiTimeStamp).ToString("yyyy-MM-dd HH:mm");
                dispatchSheet.productCode = productImpl.productCode;
                dispatchSheet.productName = productImpl.productName;
                dispatchSheet.operatorName = "未定";
                dispatchSheet.plannedNumber = outputNum;
                dispatchSheet.outputNumber = 0;
                dispatchSheet.qualifiedNumber = 0;
                dispatchSheet.unqualifiedNumber = 0;
                dispatchSheet.processName = processNameArray[processIndex];
                dispatchSheet.realStartTime = "";
                dispatchSheet.prepareTimePoint = "";
                dispatchSheet.realFinishTime = "";
                dispatchSheet.status = gVariable.MACHINE_STATUS_DISPATCH_GENERATED;
                dispatchSheet.toolLifeTimes = 0;
                dispatchSheet.toolUsedTimes = 0;
                dispatchSheet.outputRatio = 0;
                dispatchSheet.serialNumber = dispatchSheet.dispatchCode.Remove(0, 1) + "-" + serialNumberIndex.ToString().PadLeft(2, '0');
                dispatchSheet.reportor = "";
                dispatchSheet.workshop = workshopNameArray[processIndex];
                dispatchSheet.workshift = workShift;
                dispatchSheet.salesOrderCode = salesOrderImpl.salesOrderCode;
                dispatchSheet.BOMCode = productImpl.BOMCode;
                dispatchSheet.customer = salesOrderImpl.customer;

                mySQLClass.writeDataToDispatchListTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, dispatchSheet);
                databaseName = gVariable.DBHeadString + dispatchSheet.machineID.ToString().PadLeft(3, '0');
                mySQLClass.writeDataToWorkingPlanTable(databaseName, gVariable.machineWorkingPlanTableName, dispatchSheet, slitStartTimeStamp, timeStampDelta);
            }
            catch (Exception ex)
            {
                Console.WriteLine("getDispatchFromSalesOrder failed! ", ex);
            }

            return timeStampDelta;
        }

        void generateMaterialList(string databaseName, string tableName, string dispatchCode, string machineID, int outputNum, string BOMCode)
        {
            int i;
            string commandText;
            gVariable.materialListStruct materialSheet = new gVariable.materialListStruct();

            try
            {
                materialSheet.materialName = new string[gVariable.maxMaterialTypeNum];
                materialSheet.materialCode = new string[gVariable.maxMaterialTypeNum];
                materialSheet.materialRequired = new int[gVariable.maxMaterialTypeNum];
                materialSheet.materialUsed = new int[gVariable.maxMaterialTypeNum];
                materialSheet.materialLeft = new int[gVariable.maxMaterialTypeNum];

                commandText = "select * from `" + gVariable.bomTableName + "` where BOMCode = '" + productImpl.BOMCode + "'";
                mySQLClass.readBOMInfo(ref BOMImpl, commandText);

                materialSheet.salesOrderCode = salesOrderImpl.salesOrderCode;
                materialSheet.dispatchCode = dispatchCode;  //
                materialSheet.machineCode = machineID;
                materialSheet.machineName = machineNameArray[Convert.ToInt32(machineID) - 1];
                materialSheet.status = "0";
                materialSheet.numberOfTypes = BOMImpl.numberOfTypes;

                for (i = 0; i < materialSheet.numberOfTypes; i++)
                {
                    materialSheet.materialName[i] = BOMImpl.materialName[i];
                    materialSheet.materialCode[i] = BOMImpl.materialCode[i];
                    materialSheet.materialRequired[i] = BOMImpl.materialQuantity[i];
                    materialSheet.materialUsed[i] = 0;
                    materialSheet.materialLeft[i] = 0;
                }

                mySQLClass.writeDataToMaterialListTable(databaseName, tableName, materialSheet);
            }
            catch (Exception ex)
            {
                Console.WriteLine("generateMaterialList failed! ", ex);
            }
        }

        
        private void checkSecondCondition()
        {
            try
            {
                if (salesOrderImpl.productCode.Substring(0, 3) == BREATHABLE_FILM)  //
                {
                    //only casting machine 2/4 are capable of breathable film
                    //DateTime.Now.ToString("yymmdd");
                }
                else
                {
                    //only casting machine 1/3/5 are capable of unbreathable film

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("checkSecondCondition failed! ", ex);
            }
        }        
    }
}
