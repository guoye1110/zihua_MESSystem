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
        const int ONE_DISPATCH_WEIGHT = 20;   //normal output weight for a dispatch
        const int APS_MACHINE_NUM = 18;

        const int SALES_ORDER_INDEX = 2;
        const int DISPATCH_CODE_INDEX = 3;
        const int START_TIME_INDEX = 4;
        const int START_TIME_STAMP_INDEX = 5;
        const int KEEP_DURATION_INDEX = 6;

        const int salesOrderMax = 20;

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

        const int MACHINE_TYPE_CAST = 0;
        const int MACHINE_TYPE_PRINT = 1;
        const int MACHINE_TYPE_SLIT = 2;

        const int FEED_MACHINE_1 = 0;
        const int FEED_MACHINE_2 = 1;
        const int FEED_MACHINE_3 = 2;
        const int FEED_MACHINE_4 = 3;
        const int FEED_MACHINE_5 = 4;

        const int CAST_MACHINE_1 = 5;
        const int CAST_MACHINE_2 = 6;
        const int CAST_MACHINE_3 = 7;
        const int CAST_MACHINE_4 = 8;
        const int CAST_MACHINE_5 = 9;

        const int PRINT_MACHINE_X = -1;
        const int PRINT_MACHINE_UNKNOWN = 0;
        const int PRINT_MACHINE_2 = 10;
        const int PRINT_MACHINE_3 = 11;
        const int PRINT_MACHINE_4 = 12;

        const int SLIT_MACHINE_1 = 13;
        const int SLIT_MACHINE_3 = 14;
        const int SLIT_MACHINE_5 = 15;
        const int SLIT_MACHINE_6 = 16;
        const int SLIT_MACHINE_7 = 17;

        //an hour after cast process starts, print process starts if print is needed
        const int GAP_BETWEEN_CAST_PRINT = 3600;
        //an hour after cast process starts, slit process starts if print is not needed
        const int GAP_BETWEEN_CAST_SLIT = 3600;
        //an hour after print process starts, slit process starts
        const int GAP_BETWEEN_PRINT_SLIT = 3600;

        int[] outputNumForOneDispatch = { 15, 18, 40, 18, 40, 15, 18, 40, 18, 40, 50, 60, 60, 50, 50, 50, 50, 50 };  //different machine has different output ability, this is output weight based on one dispatch (12 hours)
 
        string[] operatorNameArray = { "刘刚", "黄晓宏", "张明炯", "李晓霞" };
        string[] processNameArray = { "上料工序", "流延工序", "印刷工序", "分切工序" };
        string[] workshopNameArray = { "流延车间", "流延车间", "印刷车间", "流延车间" };
        string[] machineNameArray; 

        const string BREATHABLE_FILM = "MPF";
        const string UNBREATHABLE_FILM = "CPE";

        const int APSConsideringPeriod = 60 * 24;  //2 months' time

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

        string[,] machineWorkingPlanStatus = new string[APS_MACHINE_NUM, APSConsideringPeriod];  //60 for 2 months, 24 for 24 hors
        int[,] machineVacantDuration = new int[APS_MACHINE_NUM, MAX_NUM_VACANT_PERIOD * 2];  //first: start time, second: duration; third: start time, fouth: duration ... maximum 500 pieces

        int[] castMachineList;
        int[] printMachineList;
        int[] slitMachineList;

        //use which line(casting machine) for a dispatch
        int feedMachineIndex;
        int castMachineIndex;
        int printMachineIndex;
        int slitMachineIndex;

        int castStartTimeStamp;
        int castEndTimeStamp;
        int printStartTimeStamp;
        int printEndTimeStamp;
        int slitStartTimeStamp;
        int slitEndTimeStamp;

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

            for (i = 0; i < APS_MACHINE_NUM; i++)
            {
                for (j = 0; j < APSConsideringPeriod; j++)
                    machineWorkingPlanStatus[i, j] = null;

            }
        }

        public void runAPSProcess(int salesOrderIndex)
        {
            int ret;
            int APSStartTimeStamp;

            //machine ID and machine name
            machineNameArray = getMachineList();
            
            //we need to get the newest material info from ERP before APS, so we know whether we have enough material for APS and pop up warning message if not enough
            getMaterialInfoFromERP();

            //get sales order info and product info
            getOrderAndproductForAPS(salesOrderIndex);

            APSStartTimeStamp = ConvertDateTimeInt(DateTime.Now.Date) + 8 * 3600;  //new work shift start from 8:00 in the morning

            //get machine plan table data to machineWorkingPlanStatus[], to see when the machine is occupied and when it is free for APS
            getWorkingPlanForAllMachines(APSStartTimeStamp);

            //check whether all material needed for this sale order are ready, if not pop up a message box for warning. We need to get an answer from the salesman when the material will be available,
            //if we know when will the material will be available, we can input this date in our APS UI screen as APS start time
            if (checkForMaterialReadiness(productImpl.BOMCode, Convert.ToInt32(salesOrderImpl.requiredNum)) < 0)
            {
                //failed in material readiness, return directly
                return;  
            }

            ret = checkFirstCondition(APSStartTimeStamp);  //for Zihua, first condition is machine
            if(ret == 0)
            {
                //APS success, set the status of this sales order to already scheduled
                //update 
            }

            checkSecondCondition();  //for Zihua, second condition is time
        }

        public void cancelAPSProcess(int salesOrderIndex)
        {
            string commandText;

            commandText = "select * from `" + gVariable.salesOrderTableName + "` where id = " + (salesOrderIndex + 1);
            mySQLClass.readSalesOrderInfo(ref salesOrderImpl, commandText);

            commandText = "delete from `" + gVariable.globalDispatchTableName + "` where status < '1' and salesOrderCode = '" + salesOrderImpl.salesOrderCode + "'";
            mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

            mySQLClass.redoIDIncreamentAfterRecordDeleted(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, gVariable.globalDispatchFileName);

            commandText = "delete from `" + gVariable.globalMaterialTableName + "` where salesOrderCode = '" + salesOrderImpl.salesOrderCode + "'";
            mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

            mySQLClass.redoIDIncreamentAfterRecordDeleted(gVariable.globalDatabaseName, gVariable.globalMaterialTableName, gVariable.globalDispatchFileName);
        }


        string [] getMachineList()
        {
            int i;
            int len;
            string commandText;
            string[] nameArray;
            string[,] tableArray;

            nameArray = null;
            try
            {
                commandText = "select * from `" + gVariable.machineTableName + "`";

                //get machine info
                tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (tableArray == null)
                {
                    MessageBox.Show("读取 MES 基础数据中的设备列表出错", "提示信息", MessageBoxButtons.OK);
                    return null;
                }

                len = tableArray.GetLength(0);
                nameArray = new string[len];

                for(i = 0; i < len; i++)
                    nameArray[i] = tableArray[i, 3];
            }
            catch (Exception ex)
            {
                Console.WriteLine("getMachineList() failed!" + ex);
            }

            return nameArray;
        }
        
        void getMaterialInfoFromERP()
        {

        }

        //put machine working status into buffer of machineWorkingPlanStatus, it inludes the status for all machines for 2 month starting from APSStartDateString 
        //2 month are separated to 60 * 24 hours, -1 means vacant, >=0 means the ID in global sales order table
        //APSStartDateString should be like "2018-12-24"
        void getWorkingPlanForAllMachines(int APSStartTimeStamp)
        {
            int i, j, k;
            int start;
            int dispatchNum;
            //int currentColor;
            //int salesOrderNum;
            int durationTime;
            string commandText;
            string databaseName;
            string[,] tableArray;
            string[] salesOrderArray = new string[salesOrderMax];

            try
            {
                //salesOrderNum = 0;

                commandText = "select * from `" + gVariable.machineWorkingPlanTableName + "` where EffectiveOrNot = \'1\' and timeStamp > \'" + APSStartTimeStamp + "\'";

                //display all dispatches for different machines
                for (i = 0; i < APS_MACHINE_NUM + 1; i++)
                {
                    databaseName = gVariable.DBHeadString + (i + 1).ToString().PadLeft(3, '0');

                    //get machine plan table
                    tableArray = mySQLClass.databaseCommonReading(databaseName, commandText);
                    if (tableArray == null)
                        continue;

                    dispatchNum = tableArray.GetLength(0);

                    //go through all dispatches one by one, get sales order info and draw time duration on screen
                    for (j = 0; j < dispatchNum; j++)
                    {
                        start = (Convert.ToInt32(tableArray[j, START_TIME_STAMP_INDEX]) - APSStartTimeStamp) / 3600;  //from second value to hour value;
                        durationTime = Convert.ToInt32(tableArray[j, KEEP_DURATION_INDEX]); 

                        for(k = start; k < start + durationTime; k++)
                        {
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

        private void getOrderAndproductForAPS(int salesOrderIndex)
        {
            string commandText;

            try
            {
                commandText = "select * from `" + gVariable.salesOrderTableName + "` where id = " + (salesOrderIndex + 1);
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

        private int checkFirstCondition(int APSStartTimeStamp)
        {
            int index;
            //int currentTimeStamp;
            int APSDeliveryTimeStamp;
            int dispatchStartTimeStamp;
            int workingTimeDeltaStamp;
            int numForThisDispatch, numForSalesOrderleft;
            string str;

            try
            {
                //initial related machine working start and end time
                castStartTimeStamp = -1;
                castEndTimeStamp = -1;
                printStartTimeStamp = -1;
                printEndTimeStamp = -1;
                slitStartTimeStamp = -1;
                slitEndTimeStamp = -1;

                printMachineList = new int[3];
                printMachineList[0] = PRINT_MACHINE_3;
                printMachineList[1] = PRINT_MACHINE_4;
                printMachineList[2] = PRINT_MACHINE_2;

                printMachineIndex = PRINT_MACHINE_UNKNOWN;

                switch (productImpl.routeCode)
                {
                    case "A00":  //透气膜 -- 2/4 流延机 + 2/4 分切机, 不印刷
                        castMachineList = new int[2];
                        castMachineList[0] = CAST_MACHINE_4;
                        castMachineList[1] = CAST_MACHINE_2;
                        printMachineIndex = PRINT_MACHINE_X;
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
                        printMachineIndex = PRINT_MACHINE_X;
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
                        printMachineIndex = PRINT_MACHINE_X;
                        break;
                }

                APSDeliveryTimeStamp = toolClass.timeStringToTimeStamp(salesOrderImpl.deliveryTime);

                //APS started from dispatchStartTimeStamp
                dispatchStartTimeStamp = fillOutWorkingPlan(MACHINE_TYPE_CAST, castMachineList, APSStartTimeStamp, APSDeliveryTimeStamp);
                if (dispatchStartTimeStamp < 0)
                {
                    str = "订单" + salesOrderImpl.salesOrderCode + "排程失败，请检查交货时间等信息，谢谢！";
                    MessageBox.Show(str, "信息提示", MessageBoxButtons.OK);
                }

                index = 0;
                numForSalesOrderleft = Convert.ToInt32(salesOrderImpl.requiredNum);
                numForThisDispatch = outputNumForOneDispatch[castMachineIndex];

                while (true)
                {
                    if (numForThisDispatch > numForSalesOrderleft)
                        numForThisDispatch = numForSalesOrderleft;

                    workingTimeDeltaStamp = getDispatchFromSalesOrder(index, castMachineIndex, printMachineIndex, slitMachineIndex, numForThisDispatch, dispatchStartTimeStamp);

                    numForSalesOrderleft -= numForThisDispatch;
                    if (numForSalesOrderleft <= 0)
                        break;

                    index++;
                    dispatchStartTimeStamp += workingTimeDeltaStamp;
                    printStartTimeStamp += workingTimeDeltaStamp;
                    slitStartTimeStamp += workingTimeDeltaStamp;
                }

                return 0;
            
            }
            catch (Exception ex)
            {
                Console.WriteLine("checkFirstCondition failed! ", ex);
                return -1;
            }
        }

        //find free time for every machine, then put dispatch to suitable machines to make sure the sales order can be completed by delivery time 
        //machineType: could be cast/print/slit machine
        //possibleMachine: if we are planning for cast machine, we should put cast machines that are possible for this planning inside this array, for example, we can only put cast machine 1/2 into this array for breathable film, 3/4/5 for unbreathable film
        //APSStartTimeStamp: start time for APS
        //deliveryTimeStamp: delivery time for this sales order
        //return: time stamp for the fist dispatch start time by APS 
        //        -1 APS failed
        int fillOutWorkingPlan(int machineType, int[] possibleMachine, int APSStartTimeStamp, int APSDeliveryTimeStamp)
        {
            int i, j, k;
            int flag;
            int temp;
            int returnTimeStamp;
            int machineIndex;  //from 0 to 17, alogether 18 machines
            int salesOrderWorkingTime;

            int castTmpStartTimeStamp;
            int castTmpEndTimeStamp;
            int printTmpStartTimeStamp;
            int printTmpEndTimeStamp;
            int slitTmpStartTimeStamp;
            int slitTmpEndTimeStamp;

            //return value
            returnTimeStamp = -1;

            try
            {
                castTmpStartTimeStamp = -1;
                castTmpEndTimeStamp = -1;
                printTmpStartTimeStamp = -1;
                printTmpEndTimeStamp = -1;
                slitTmpStartTimeStamp = -1;
                slitTmpEndTimeStamp = -1;

                //put the sales order in one machine, we try all posible machines one by one
                for (i = 0; i < possibleMachine.Length; i++)
                {
                    machineIndex = possibleMachine[i]; //get machine index
                    flag = 0;
                    k = 0;

                    //total working time for one sales order by using this machine
                    salesOrderWorkingTime = getSalesOrderWorkingTime(machineIndex);

                    //APSConsideringPeriod is 2 month counted by hours, we need to finish a sales order within 2 month
                    //j is the offset of hours starting from APSStartTimeStamp
                    for (j = 0; j < APSConsideringPeriod; j++)
                    {
                        if (machineWorkingPlanStatus[machineIndex, j] == null)  //free time for this machine
                        {
                            if (flag == 0)  //remember start position for free time, we need this value to calculate the start point for this dispatch, flag will keep on increasing
                                k = j;

                            if (flag >= MIN_HOUR_FOR_VACANCY)  //free time larger than 4 hour, we can consider this as vacancy as APS vacant time slot
                            {
                                switch (machineType)
                                {
                                    case MACHINE_TYPE_CAST:
                                        if (flag > salesOrderWorkingTime) //we got a vacanct period long enough to complete this sales order
                                        {
                                            if (APSStartTimeStamp + j * 3600 < APSDeliveryTimeStamp)  //we can complete this sales order before delivery time, but we still need to consider print and slit process
                                            {
                                                //if cast machine #3 don't need printing, it will do slit online, so slit machine 3 is the only choice
                                                //if we have a print process, we should use slit machine 1/6/7, which are separate slit machines
                                                if (machineIndex == CAST_MACHINE_3 && printMachineIndex == PRINT_MACHINE_X)
                                                {
                                                    slitMachineList = new int[1];
                                                    slitMachineList[0] = SLIT_MACHINE_3;
                                                }
                                                else if (machineIndex == CAST_MACHINE_5 && printMachineIndex == PRINT_MACHINE_X)
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
                                                
                                                if (printMachineIndex == PRINT_MACHINE_X)  
                                                {
                                                    //there is no print process for this sales order, so go to slit process directly
                                                    temp = fillOutWorkingPlan(MACHINE_TYPE_SLIT, slitMachineList, APSStartTimeStamp + k * 3600 + GAP_BETWEEN_PRINT_SLIT, APSDeliveryTimeStamp);
                                                    //slitTmpStartTimeStamp = temp;
                                                }
                                                else
                                                {
                                                    //there is print process for this sales order, so go to print process directly
                                                    temp = fillOutWorkingPlan(MACHINE_TYPE_PRINT, printMachineList, APSStartTimeStamp + k * 3600 + GAP_BETWEEN_CAST_PRINT, APSDeliveryTimeStamp);
                                                    //printTmpStartTimeStamp = temp;
                                                }

                                                if (temp == -1)
                                                {
                                                    //failed to APS for slit machine, try next machine
                                                    j = APSConsideringPeriod + 1;
                                                }
                                                else
                                                {
                                                    //this APS process satisfied cast/print/slit machine, so we pre-adopted this plan as a pertential choice, and will do more comparation to see which plan is better

                                                    //this is the first plan that satisfy all conditions
                                                    if (castStartTimeStamp < 0)
                                                    {
                                                        castStartTimeStamp = APSStartTimeStamp + k * 3600;
                                                        castEndTimeStamp = APSStartTimeStamp + (k + flag) * 3600;

                                                        //this APS process satisfied cast machine, so we adopt cast machine
                                                        castMachineIndex = machineIndex;  //set production line(CAST_MACHINE_1 to CAST_MACHINE_5)
                                                        feedMachineIndex = castMachineIndex - (CAST_MACHINE_1 - FEED_MACHINE_1);

                                                        //if (printMachineIndex == PRINT_MACHINE_X)
                                                        //    slitStartTimeStamp = temp;
                                                        //else
                                                        //    printStartTimeStamp = temp;

                                                        returnTimeStamp = castStartTimeStamp;
                                                    }
                                                    else  //this is not the first one, need to compare with previous successful plans
                                                    {
                                                        castTmpStartTimeStamp = APSStartTimeStamp + k * 3600;
                                                        castTmpEndTimeStamp = APSStartTimeStamp + (k + flag) * 3600;

                                                        //printTmpStartTimeStamp = temp;

                                                        //new plan is more reasonable since it completed before the original plan starts, so we adopt this new plan
                                                        if (castStartTimeStamp > castTmpEndTimeStamp)
                                                        {
                                                            //this APS process satisfied cast machine, so we adopt cast machine
                                                            castMachineIndex = machineIndex;  //set production line(CAST_MACHINE_1 to CAST_MACHINE_5)
                                                            feedMachineIndex = castMachineIndex - (CAST_MACHINE_1 - FEED_MACHINE_1);

                                                            castStartTimeStamp = castTmpStartTimeStamp;
                                                            castEndTimeStamp = castTmpEndTimeStamp;

                                                            //printStartTimeStamp = printTmpStartTimeStamp;
                                                            //slitStartTimeStamp = slitTmpStartTimeStamp;

                                                            returnTimeStamp = castStartTimeStamp;
                                                        }
                                                        else //if(castTmpStartTimeStamp > castEndTimeStamp)
                                                        {
                                                            //the original one has hiher priority
                                                        }
                                                    }
                                                }
                                                //we use this condition to jump out of this cycle loop, then go to the next machine
                                                j = APSConsideringPeriod + 1;
                                            }
                                        }
                                        break;
                                    case MACHINE_TYPE_PRINT:
                                        if (flag > salesOrderWorkingTime) //we got a vacanct period long enough to complete this sales order
                                        {
                                            if (APSStartTimeStamp + j * 3600 < APSDeliveryTimeStamp)  //we can complete this sales order before delivery time, but we still need to consider slit process
                                            {
                                                temp = fillOutWorkingPlan(MACHINE_TYPE_SLIT, slitMachineList, APSStartTimeStamp + k * 3600 + GAP_BETWEEN_PRINT_SLIT, APSDeliveryTimeStamp);
                                                if (temp == -1)
                                                {
                                                    //failed to APS for print machine or slit machine, redo APS by this cast machine for next free period
                                                    //APSResult = -1;
                                                }
                                                else
                                                {
                                                    //this is the first plan that satisfy all conditions
                                                    if (printStartTimeStamp < 0)
                                                    {
                                                        printStartTimeStamp = APSStartTimeStamp + k * 3600;
                                                        printEndTimeStamp = APSStartTimeStamp + (k + flag) * 3600;

                                                        //slitStartTimeStamp = temp;

                                                        //this APS process satisfied print machine, so we adopt it
                                                        printMachineIndex = machineIndex;

                                                        returnTimeStamp = printStartTimeStamp;
                                                    }
                                                    else  //this is not the first one, need to compare with previous successful plans
                                                    {
                                                        printTmpStartTimeStamp = APSStartTimeStamp + k * 3600;
                                                        printTmpEndTimeStamp = APSStartTimeStamp + (k + flag) * 3600;

                                                        //slitTmpStartTimeStamp = temp;

                                                        //new plan is more reasonable since it completed before the original plan starts, so we adopt this new plan
                                                        if (printStartTimeStamp > printTmpEndTimeStamp)
                                                        {
                                                            //this APS process satisfied print machine, so we adopt it
                                                            printMachineIndex = machineIndex;

                                                            printStartTimeStamp = printTmpStartTimeStamp;
                                                            printEndTimeStamp = printTmpEndTimeStamp;

                                                            //slitStartTimeStamp = slitTmpStartTimeStamp;

                                                            returnTimeStamp = printStartTimeStamp;
                                                        }
                                                        else //if(printTmpStartTimeStamp > printEndTimeStamp)
                                                        {
                                                            //the original one has hiher priority
                                                        }
                                                    }
                                                }
                                            }
                                            //we use this condition to jump out of this cycle loop, then go to the next machine
                                            j = APSConsideringPeriod + 1;
                                        }
                                        break;
                                    case MACHINE_TYPE_SLIT:
                                        if (flag > salesOrderWorkingTime) //we got a vacanct period long enough to complete this sales order
                                        {
                                            if (APSStartTimeStamp + j * 3600 < APSDeliveryTimeStamp)  //we can complete this sales order before delivery time
                                            {
                                                if (slitStartTimeStamp < 0)
                                                {
                                                    slitStartTimeStamp = APSStartTimeStamp + k * 3600;
                                                    slitEndTimeStamp = APSStartTimeStamp + (k + flag) * 3600;

                                                    //this APS process satisfied print machine, so we adopt it
                                                    slitMachineIndex = machineIndex;  //set production line(CAST_MACHINE_1 to CAST_MACHINE_5)

                                                    returnTimeStamp = slitStartTimeStamp;
                                                }
                                                else  //this is not the first one, need to compare with previous successful plans
                                                {
                                                    slitTmpStartTimeStamp = APSStartTimeStamp + k * 3600;
                                                    slitTmpEndTimeStamp = APSStartTimeStamp + (k + flag) * 3600;

                                                    //new plan is more reasonable since it completed before the original plan starts, so we adopt this new plan
                                                    if (slitStartTimeStamp > slitTmpEndTimeStamp)
                                                    {
                                                        //this APS process satisfied slit machine, so we adopt it
                                                        slitMachineIndex = machineIndex;  //set production line(CAST_MACHINE_1 to CAST_MACHINE_5)

                                                        slitStartTimeStamp = slitTmpStartTimeStamp;
                                                        slitEndTimeStamp = slitTmpEndTimeStamp;

                                                        returnTimeStamp = slitStartTimeStamp;
                                                    }
                                                    else //if(printTmpStartTimeStamp > printEndTimeStamp)
                                                    {
                                                        //the original one has hiher priority
                                                    }
                                                }
                                            }
                                            //we use this condition to jump out of this cycle loop, then go to the next machine
                                            j = APSConsideringPeriod + 1;
                                        }
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

                //failed to put the whole sales order in one machine, try to separate it into several small part and put in different machines
                for (i = 0; i < possibleMachine.Length; i++)
                {
                    flag = 0;
                    k = 0;
                    for (j = 0; j < APSConsideringPeriod; j++)
                    {
                        if (machineWorkingPlanStatus[i, j] == null)  //free time for this machine
                        {
                            if (flag == 0)  //remember start position
                                k = j;
                            flag++;
                        }
                        else //if(machineWorkingPlanStatus[i, j] != null)  //this machine is busy
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("fillOutWorkingPlan failed! " + ex);
            }

            return returnTimeStamp;
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
        private int getDispatchFromSalesOrder(int indexInDispatch, int castMachineIndex, int printMachineIndex, int slitMachineIndex, int outputNum, int timeStamp)
        {
            int timeV;
            int processIndex;
            int timeStampDelta;
            int finshiTimeStamp;
            //int finshiTimeStampForReturn;
            int serialNumberIndex;
            string planTime1;
            string workShift;
            string databaseName;

            processIndex = 0;
            serialNumberIndex = 1;

            planTime1 = GetTimeFromInt(timeStamp).ToString("yyyy-MM-dd HH:mm");
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
                finshiTimeStamp = timeStamp + timeStampDelta;  //finish time is start time plus production time needed for output number, normally one shift -> 12 hours
                dispatchSheet.planTime2 = GetTimeFromInt(finshiTimeStamp).ToString("yyyy-MM-dd HH:mm");
                dispatchSheet.productCode = productImpl.productCode;
                dispatchSheet.productName = productImpl.productName;
                dispatchSheet.operatorName = operatorNameArray[processIndex];
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
                dispatchSheet.customer = productImpl.customer;

                mySQLClass.writeDataToDispatchListTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, dispatchSheet);

                databaseName = gVariable.DBHeadString + dispatchSheet.machineID.ToString().PadLeft(3, '0');
                mySQLClass.writeDataToWorkingPlanTable(databaseName, gVariable.machineWorkingPlanTableName, dispatchSheet, timeStamp, timeStampDelta);

                generateMaterialList(gVariable.globalDatabaseName, gVariable.globalMaterialTableName, dispatchSheet.dispatchCode, dispatchSheet.machineID, outputNum, productImpl.BOMCode);

                processIndex++;
                //cast process
                dispatchSheet.machineID = (castMachineIndex + 1).ToString();
                dispatchSheet.dispatchCode = salesOrderImpl.salesOrderCode + (castMachineIndex + 1) + indexInDispatch.ToString().PadLeft(2, '0') + (processIndex + 1);
                //cast and feed process stat at the same time
                dispatchSheet.planTime1 = GetTimeFromInt(timeStamp).ToString("yyyy-MM-dd HH:mm");
                finshiTimeStamp = timeStamp + timeStampDelta;  //finish time is start time plus production time needed for output number, normally one shift -> 12 hours
                //finshiTimeStampForReturn = finshiTimeStamp; //this is end time of feed/cast process, we will use this time to start a new dispatch
                dispatchSheet.planTime2 = GetTimeFromInt(finshiTimeStamp).ToString("yyyy-MM-dd HH:mm");
                dispatchSheet.productCode = productImpl.productCode;
                dispatchSheet.productName = productImpl.productName;
                dispatchSheet.operatorName = operatorNameArray[processIndex];
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
                dispatchSheet.customer = productImpl.customer;

                mySQLClass.writeDataToDispatchListTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, dispatchSheet);
                databaseName = gVariable.DBHeadString + dispatchSheet.machineID.ToString().PadLeft(3, '0');
                mySQLClass.writeDataToWorkingPlanTable(databaseName, gVariable.machineWorkingPlanTableName, dispatchSheet, timeStamp, timeStampDelta);

                processIndex++;

                if (printMachineIndex != -1)
                {
                    timeStamp += GAP_BETWEEN_CAST_PRINT;

                    //print process
                    dispatchSheet.machineID = (printMachineIndex + 1).ToString();
                    dispatchSheet.dispatchCode = salesOrderImpl.salesOrderCode + (castMachineIndex + 1) + indexInDispatch.ToString().PadLeft(2, '0') + (processIndex + 1);
                    dispatchSheet.planTime1 = GetTimeFromInt(printStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
                    finshiTimeStamp = printStartTimeStamp + timeStampDelta;  //finish time is start time plus production time needed for output number, normally one shift -> 12 hours
                    dispatchSheet.planTime2 = GetTimeFromInt(finshiTimeStamp).ToString("yyyy-MM-dd HH:mm");
                    dispatchSheet.productCode = productImpl.productCode;
                    dispatchSheet.productName = productImpl.productName;
                    dispatchSheet.operatorName = operatorNameArray[processIndex];
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
                    dispatchSheet.customer = productImpl.customer;

                    mySQLClass.writeDataToDispatchListTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, dispatchSheet);
                    databaseName = gVariable.DBHeadString + dispatchSheet.machineID.ToString().PadLeft(3, '0');
                    mySQLClass.writeDataToWorkingPlanTable(databaseName, gVariable.machineWorkingPlanTableName, dispatchSheet, printStartTimeStamp, timeStampDelta);

                    timeStamp += GAP_BETWEEN_PRINT_SLIT;
                }
                else
                {
                    timeStamp += GAP_BETWEEN_CAST_SLIT;
                }
                processIndex++;

                //slit process
                dispatchSheet.machineID = (slitMachineIndex + 1).ToString();
                dispatchSheet.dispatchCode = salesOrderImpl.salesOrderCode + (castMachineIndex + 1) + indexInDispatch.ToString().PadLeft(2, '0') + (processIndex + 1);
                dispatchSheet.planTime1 = GetTimeFromInt(slitStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
                finshiTimeStamp = slitStartTimeStamp + timeStampDelta;  //finish time is start time plus production time needed for output number, normally one shift -> 12 hours
                dispatchSheet.planTime2 = GetTimeFromInt(finshiTimeStamp).ToString("yyyy-MM-dd HH:mm");
                dispatchSheet.productCode = productImpl.productCode;
                dispatchSheet.productName = productImpl.productName;
                dispatchSheet.operatorName = operatorNameArray[processIndex];
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
                dispatchSheet.customer = productImpl.customer;

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
                    DateTime.Now.ToString("yymmdd");
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

        void getselectedSalesOrder(int salesOrderIndex)
        {


        }
    }
}
