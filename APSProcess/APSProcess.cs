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
using MESSystem.dispatchManagement;

namespace MESSystem.APSDLL
{
    public partial class APSProcess
    {
        const int MAX_NUM_MACHINE_TYPE = 3;

        const int MACHINE_TYPE_CAST = 0;
        const int MACHINE_TYPE_PRINT = 1;
        const int MACHINE_TYPE_SLIT = 2;

        const int CAST_MACHINE_1 = 0;
        const int CAST_MACHINE_2 = 1;
        const int CAST_MACHINE_3 = 2;
        const int CAST_MACHINE_4 = 3;
        const int CAST_MACHINE_5 = 4;
        const int CAST_MACHINE_6 = 5;
        const int CAST_MACHINE_7 = 6;

        const int PRINT_MACHINE_X = -1;  //no print process is needed
        const int PRINT_MACHINE_1 = 7;
        const int PRINT_MACHINE_2 = 8;
        const int PRINT_MACHINE_3 = 9;
        const int PRINT_MACHINE_4 = 10;
        const int PRINT_MACHINE_5 = 11;

        const int SLIT_MACHINE_1 = 12;
        const int SLIT_MACHINE_2 = 13;
        const int SLIT_MACHINE_3 = 14;
        const int SLIT_MACHINE_4 = 15;
        const int SLIT_MACHINE_5 = 16;

        const int ONE_DISPATCH_WEIGHT = 20;   //normal output weight for a dispatch

        //const int productBatchMax = 20;

        const int FORWARD_APS_PLAN = 0;
        const int REVERSED_APS_PLAN = 1;

        //an hour after cast process starts, print process starts if print is needed
        const int GAP_BETWEEN_CAST_PRINT = 3600;
        //an hour after cast process starts, slit process starts if print is not needed
        const int GAP_BETWEEN_CAST_SLIT = 3600;
        //an hour after print process starts, slit process starts
        const int GAP_BETWEEN_PRINT_SLIT = 3600;

        //const int BATCH_NUM_FOR_30_YEARS = 360;
        //BATCH_INC_NUM number of dispatches will build up to a batch, many batches will build up to a batch order
        //const int BATCH_INC_NUM = 6;

        const int MACHINE_ORDER_WEIGHT_MORE = 0; //更加看重设备顺序，尽量满足产品规格书设备列表中顺序靠前的设备
        const int TIME_EQUALITY_WEIGHT_MORE = 1; //更加看重设备负载的均衡，尽量满足所有设备按时间的平均分配

        int machineSelectionMethod = TIME_EQUALITY_WEIGHT_MORE;

        static int APS_MACHINE_NUM = gVariable.machineNameArrayAPS.Length;

        //different machine has different output ability, this is output weight of kilogram in an hour
        int[] eachMachineOutputForOneHour; // = { 300, 320, 920, 320, 920, 800, 350, 90, 100, 100, 95, 87, 980, 980, 980, 980, 980 }; 

        string[] processNameArray = { "流延工序", "印刷工序", "分切工序" };
        string[] workshopNameArray = { "流延车间", "流延车间", "印刷车间", "流延车间" };

        const string BREATHABLE_FILM = "MPF";
        const string UNBREATHABLE_FILM = "CPE";

        string[] supplementaryMaterial = { "铲板", "纸芯", "纸板", "牛皮纸", "瓦楞纸", "缠绕膜", "包装膜" }; 

        const int APS_TOTAL_PERIOD = (60 * 24);  //2 months' time

        const int MIN_HOUR_FOR_VACANCY = 4; //if we a machine has 4 hours of free time, it can be regarded as vacancy, we can insert a small dispatch in this period 
        const int MAX_NUM_VACANT_PERIOD = 500; //max number of vacant period in this 2 month of APS time

        gVariable.productBatchStruct productBatchImpl;
        gVariable.productStruct productImpl;
        gVariable.BOMListStruct BOMImpl;
        //gVariable.castCraftStruct castCraftImpl;
        //gVariable.castQualityStruct castQualityImpl;
        //gVariable.printCraftStruct printCraftImpl;
        //gVariable.printQualityStruct printQualityImpl;
        //gVariable.slitCraftStruct slitCraftImpl;
        //gVariable.slitQualityStruct slitQualityImpl;

        string[,] machineWorkingPlanStatus = new string[APS_MACHINE_NUM, APS_TOTAL_PERIOD];  //60 for 2 months, 24 for 24 hors
        int[,] machineVacantDuration = new int[APS_MACHINE_NUM, MAX_NUM_VACANT_PERIOD * 2];  //first: start time, second: duration; third: start time, fouth: duration ... maximum 500 pieces

        //record start/end time for the best plan of all machines by now 
        int[] startTimeTypeMachine = new int[MAX_NUM_MACHINE_TYPE];
        int[] endTimeTypeMachine = new int[MAX_NUM_MACHINE_TYPE];
        //record machine used for the best plan by now, maybe next machine is better
        int[] machineIndexForAllType = new int[MAX_NUM_MACHINE_TYPE];

        int calendarStartTimeStamp;

        //machine that would be used in APS
        int[] castMachineList = null;
        int[] printMachineList = null;
        int[] slitMachineList = null;

        //record start/end time for the cast/print/slit machine in the current plan
        int castStartTimeStamp;
        int castEndTimeStamp;
        int printStartTimeStamp;
        int printEndTimeStamp;
        int slitStartTimeStamp;
        int slitEndTimeStamp;

        int APSPlanDirection;

        public APSProcess()
        {
            initVariables();
        }

        void initVariables()
        {
            int i, j;

            BOMImpl.materialName = new string[gVariable.maxMaterialTypeNum];
            BOMImpl.materialCode = new string[gVariable.maxMaterialTypeNum];
            BOMImpl.materialQuantity = new double[gVariable.maxMaterialTypeNum];

            APSPlanDirection = FORWARD_APS_PLAN;

            for (i = 0; i < APS_MACHINE_NUM; i++)
            {
                for (j = 0; j < APS_TOTAL_PERIOD; j++)
                    machineWorkingPlanStatus[i, j] = null;

            }
        }

        public void runAPSProcess(int productBatchID, int[] assignedMachineByUserArray, int requiredStartTimeStamp, int requiredEndTimeStamp)
        {
            int ret;
            int APSStartTimeStamp;
            int APSEndTimeStamp;

            //machine ID and machine name
            //getMachineInfoFromDatabase();
            
            //we need to get the newest material info from ERP before APS, so we know whether we have enough material for APS and pop up warning message if not enough
            getMaterialInfoFromERP();

            //get batch order info and product info
            getOrderAndProductInfoForAPS(productBatchID);

            calendarStartTimeStamp = ConvertDateTimeInt(DateTime.Now.Date.AddDays(1)); // +8 * 3600;  //new work shift start from 8:00 the next day, so APS stats from 8:00 in the morning of the next day

            //APS operator assigned a start time
            if (requiredStartTimeStamp != -1)
                APSStartTimeStamp = requiredStartTimeStamp;  //
            else
                APSStartTimeStamp = calendarStartTimeStamp + 8 * 3600;

            //APS complete time is assigned in APS UI, so we use requiredEndTimeStamp as APSEndTimeStamp
            if (requiredEndTimeStamp != -1)
            {
                APSPlanDirection = REVERSED_APS_PLAN;
                APSEndTimeStamp = requiredEndTimeStamp;
            }
            else  //no APS complete time is assigned in APS UI, we use batch order delivery time as APSEndTimeStamp
            {
                APSPlanDirection = FORWARD_APS_PLAN;
                APSEndTimeStamp = toolClass.timeStringToTimeStamp(productBatchImpl.deliveryTime);
            }

            //get machine plan table data to machineWorkingPlanStatus[], to see when the machine is occupied and when it is free for APS
            getWorkingPlanForAllMachines(APSPlanDirection, APSStartTimeStamp, APSEndTimeStamp);

            //check whether all material needed for this sale order are ready, if not pop up a message box for warning. We need to get an answer from the batchman when the material will be available,
            //if we know when will the material will be available, we can input this date in our APS UI screen as APS start time
            if (checkForMaterialReadiness(productImpl.BOMCode, (int)(Convert.ToDouble(productBatchImpl.requiredNum))) < 0)
            {
                //failed in material readiness, return directly
                return;  
            }

            ret = startAPSAction(APSPlanDirection, productBatchID, APSStartTimeStamp, APSEndTimeStamp, assignedMachineByUserArray);
            if (ret == 0)
            {
            }

            checkSecondCondition();

            castMachineList = null;
            printMachineList = null;
            slitMachineList = null;

            GC.Collect();
        }

        public void cancelAPSProcess(int productBatchID)
        {
            int i;
            int ret;
            string databaseName;
            string commandText;

            commandText = "select * from `" + gVariable.productBatchTableName + "` where id = " + (productBatchID);
            mySQLClass.readProductBatchInfo(ref productBatchImpl, commandText);

            commandText = "delete from `" + gVariable.globalDispatchTableName + "` where salesOrderBatchCode = '" + productBatchImpl.salesOrderBatchCode + "'";
            mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);
            mySQLClass.redoIDIncreamentAfterRecordDeleted(gVariable.globalDatabaseName, gVariable.globalDispatchTableName);

            //delete all dispatch table in dispatch list
            for (i = 0; i < APS_MACHINE_NUM; i++)
            {
                databaseName = gVariable.DBHeadString + (i + 1).ToString().PadLeft(3, '0');
                commandText = "delete from `" + gVariable.machineWorkingPlanTableName + "` where salesOrderBatchCode = '" + productBatchImpl.salesOrderBatchCode + "'";
                ret = mySQLClass.pureDatabaseNonQueryAction(databaseName, commandText);
                if(ret > 0)
                    mySQLClass.redoIDIncreamentAfterRecordDeleted(databaseName, gVariable.machineWorkingPlanTableName);
            }

            commandText = "delete from `" + gVariable.globalMaterialTableName + "` where salesOrderBatchCode = '" + productBatchImpl.salesOrderBatchCode + "'";
            mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);
            mySQLClass.redoIDIncreamentAfterRecordDeleted(gVariable.globalDatabaseName, gVariable.globalMaterialTableName);

            commandText = "delete from `" + gVariable.globalProductTaskTableName + "` where salesOrderBatchCode = '" + productBatchImpl.salesOrderBatchCode + "'";
            mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);
            mySQLClass.redoIDIncreamentAfterRecordDeleted(gVariable.globalDatabaseName, gVariable.globalProductTaskTableName);

            commandText = "delete from `" + gVariable.globalMaterialTaskTableName + "` where salesOrderBatchCode = '" + productBatchImpl.salesOrderBatchCode + "'";
            mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);
            mySQLClass.redoIDIncreamentAfterRecordDeleted(gVariable.globalDatabaseName, gVariable.globalMaterialTaskTableName);
        }


        void getMaterialInfoFromERP()
        {

        }

        //put machine working status into buffer of machineWorkingPlanStatus, it inludes the status for all machines for 2 month starting from APSStartDateString 
        //2 month are separated to 60 * 24 hours, -1 means vacant, >=0 means the ID in global batch order table
        //APSStartDateString should be like "2018-12-24"
        void getWorkingPlanForAllMachines(int APSPlanDirection, int startTimeStamp, int completeTimeStamp)
        {
            int i, j, k;
            int start;
            int dispatchNum;
            int durationTime;
            string commandText;
            string databaseName;
            string[,] tableArray;
            //string[] productBatchArray = new string[productBatchMax];

            try
            {
                //we only consider plans (including dispatch plans and maintenance plans) that will be completed later than APS start time
                commandText = "select * from `" + gVariable.machineWorkingPlanTableName + "` where timeStamp2 >= \'" + startTimeStamp + "\'"; // and timeStamp1 <= \'" + completeTimeStamp + "\'";

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
                        start = (Convert.ToInt32(tableArray[j, mySQLClass.PLAN_START_TIME_STAMP_INDEX]) - startTimeStamp) / 3600;  //from second value to hour value;
                        durationTime = Convert.ToInt32(tableArray[j, mySQLClass.PLAN_KEEP_DURATION_INDEX]); 

                        for(k = start; k < start + durationTime; k++)
                        {
                            if (k >= APS_TOTAL_PERIOD)  //we don't consider plans later than APS_TOTAL_PERIOD 
                                break;

                            machineWorkingPlanStatus[i, k] = tableArray[j, mySQLClass.PLAN_DISPATCH_CODE_INDEX];  //store dispatch code into array
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("getWorkingPlanForAlmachines() failed!" + ex);
            }
        }

        private void getOrderAndProductInfoForAPS(int productBatchID)
        {
            string commandText;

            try
            {
                //get product batch number index, 1805304, 04 is the current batch index for cast machine 3 
                commandText = "select * from `" + gVariable.productBatchTableName + "` where id = " + (productBatchID);
                mySQLClass.readProductBatchInfo(ref productBatchImpl, commandText);

                commandText = "select * from `" + gVariable.productTableName + "` where productCode = '" + productBatchImpl.productCode + "'";
                mySQLClass.readProductInfo(ref productImpl, commandText);

                commandText = "select * from `" + gVariable.bomTableName + "` where BOMCode = '" + productImpl.BOMCode + "'";
                mySQLClass.readBOMInfo(ref BOMImpl, commandText);

                eachMachineOutputForOneHour = toolClass.getMachineCapability(productBatchImpl.productCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("getOrderAndProductInfo failed! " + ex);
            }
        }

        //how long (in hours) will this machine need to complete one dispatch
        void getSalesOrderWorkingTime(int[] productBatchWorkingTime)
        {
            int i;

            for (i = 0; i < APS_MACHINE_NUM; i++)
            {
                productBatchWorkingTime[i] = (int)(Convert.ToDouble(productBatchImpl.requiredNum) / eachMachineOutputForOneHour[i]);
            }
        }

        //
        private int startAPSAction(int APSPlanDirection, int productBatchID, int APSStartTimeStamp, int APSEndTimeStamp, int[] assignedMachineByUserArray)
        {
            int i;
            int ret;
            int hour;
            int total;
            //int castDuration;
            int workingTime;
            int workingTimeDeltaStamp;
            int productBatchStartTimeStamp; 
            int productBatchEndTimeStamp;
            int productNumForOneBatch;  // this is a limitation for the current batch, normal limitation is 20 tons but if the batch order is 22, this limitaton becomes 11
            int monthIndex;
            string monthIndexStr;
            int productBatchIndex;
            int backcastStart, backCastEnd, backPrintstart, backPrintEnd, backSlitStart, backSlitEnd;
            string planTime1;  //cast start time
            //string planTime2;  //cast end time
            int[] productNumInCurrentBatch = new int[MAX_NUM_MACHINE_TYPE]; //how many products are produced in the current dispatch
            string productBatchNumString; //产品批次号

            //output for a standard 12 hour dispatch
            //int[] numForStandardDispatch = new int[MAX_NUM_MACHINE_TYPE];
            //planned output for the current dispatch
            int[] numForThisDispatch = new int[MAX_NUM_MACHINE_TYPE];
            //planned product still needed for a batch order
            int[] numForSalesOrderleft = new int[MAX_NUM_MACHINE_TYPE];
            string updateStr;

            productBatchIndex = 0;
            try
            {
                //
                if (APSPlanDirection == FORWARD_APS_PLAN)
                    ret = generateForwardAPSPlans(productImpl.routeCode, APSStartTimeStamp, APSEndTimeStamp, assignedMachineByUserArray);
                else //if (APSPlanDirection == REVERSED_APS_PLAN)
                    ret = generateReversedAPSPlans(productImpl.routeCode, APSStartTimeStamp, APSEndTimeStamp, assignedMachineByUserArray);
                if (ret < 0)
                    return ret;

                //we need to get output number for a batch, normally separate a batch order equally
                total = (int)(Convert.ToDouble(productBatchImpl.requiredNum));
                if (total > gVariable.PRODUCT_BATCH_MAX_OUTPUT_NUM)
                {
                    i = total / gVariable.PRODUCT_BATCH_MAX_OUTPUT_NUM + 1;
                    productNumForOneBatch = total / i;
                }
                else
                {
                    productNumForOneBatch = total;
                }

                //castDuration = castEndTimeStamp - castStartTimeStamp;

                //planTime1 = GetTimeFromInt(castStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
                //planTime2 = GetTimeFromInt(castEndTimeStamp).ToString("yyyy-MM-dd HH:mm");
                //hour = Convert.ToInt32(planTime1.Remove(0, 11).Remove(2));

                //get month index starting from 2018/01, we have a batch number table for every machine each month, the batch number will be increased by one and the recent value is stored in 
                //table of productBatchCurrentIndex in global database.
                //monthIndex = getMonthIndexForDispatchBatch(planTime1);

                //
                for (i = 0; i < MAX_NUM_MACHINE_TYPE; i++)
                {
                    productNumInCurrentBatch[i] = 0;

                    if (machineIndexForAllType[i] < 0)
                    {
                        //numForStandardDispatch[i] = 0;
                        numForSalesOrderleft[i] = 0;
                        numForThisDispatch[i] = 0;
                        continue;
                    }

                    //all required number we need to process for a batch order
                    numForSalesOrderleft[i] = (int)(Convert.ToDouble(productBatchImpl.requiredNum));
                    //how much we can process in 12 hours for each machine(these machines are already scheduled by APS)
                    //numForStandardDispatch[i] = eachMachineOutputForOneHour[machineIndexForAllType[i]] * 12;
                }

                productBatchStartTimeStamp = castStartTimeStamp;
                productBatchEndTimeStamp = slitEndTimeStamp;

                productBatchNumString = null;

                backcastStart = castStartTimeStamp;
                backCastEnd = castEndTimeStamp;
                backPrintstart = printStartTimeStamp;
                backPrintEnd = printEndTimeStamp;
                backSlitStart = slitStartTimeStamp;
                backSlitEnd = slitEndTimeStamp; ;

                planTime1 = GetTimeFromInt(castStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
                //get month index starting from 2018/01, we have a batch number table for every machine each month, the batch number will be increased by one and the recent value is stored in 
                //table of productBatchCurrentIndex in global database.
                monthIndex = getMonthIndexForDispatchBatch(planTime1);
                monthIndexStr = (18 + monthIndex / 12).ToString().PadLeft(2, '0') + (monthIndex % 12).ToString().PadLeft(2, '0');

                while (true)
                {
                    planTime1 = GetTimeFromInt(castStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
                    hour = Convert.ToInt32(planTime1.Remove(0, 11).Remove(2));
                    if (hour < 8)
                    {
                        workingTime = 8 - hour;
                    }
                    else if (hour < 20)
                    {
                        workingTime = 20 - hour;
                    }
                    else
                    {
                        workingTime = 32 - hour;
                    }

                    for (i = 0; i < MAX_NUM_MACHINE_TYPE; i++)
                    {
                        //one type of machine(probably print machine is not used in this batch order)
                        if (machineIndexForAllType[i] < 0)
                            continue;

                        //how many products we can process for this dispatch in a shif 
                        numForThisDispatch[i] = eachMachineOutputForOneHour[machineIndexForAllType[i]] * workingTime * gVariable.EFFECTIVE_WORKING_HOUR_ONE_SHIFT / gVariable.PHYSICAL_WORKING_HOUR_ONE_SHIFT;

                        if (numForThisDispatch[i] > numForSalesOrderleft[i])
                            numForThisDispatch[i] = numForSalesOrderleft[i];  //only a little number left, this dispatch will process them all

                        productNumInCurrentBatch[i] += numForThisDispatch[i];

                        //product batch number genration, it is only related to cast process, so only when i == 0 will perform this calculation below
                        if (i == 0)
                        {
                            //get product batch string, if we use batch order for APS function, we won't enter "else if" and "else", "else if" and "else" are for large sales orders
                            if (productNumInCurrentBatch[0] <= productNumForOneBatch)
                            {
                                //keep production in this batch
                                productBatchIndex = getCurrentProductBatchIndex(monthIndex, machineIndexForAllType[0] + 1);
                                productBatchNumString = monthIndexStr + (machineIndexForAllType[0] + 1) + productBatchIndex.ToString().PadLeft(2, '0');
                            }
                            else //if (productNumInCurrentBatch[0] > productNumForOneBatch && productNumInCurrentBatch[0] < gVariable.PRODUCT_BATCH_MAX_OUTPUT_NUM)  
                            {
                                //keep production in this batch, but need to increase batch index, so next time production will be in new batch
                                //for example, batch limitation is 12000, but we still have 18000
                                productBatchIndex = getCurrentProductBatchIndex(monthIndex, machineIndexForAllType[0] + 1);
                                productBatchNumString = monthIndexStr + (machineIndexForAllType[0] + 1) + productBatchIndex.ToString().PadLeft(2, '0');

                                //product batch number increase by one, need to get new month value, maybe it's now new month 
                                planTime1 = GetTimeFromInt(castStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
                                monthIndex = getMonthIndexForDispatchBatch(planTime1);
                                productBatchIndex = getCurrentProductBatchIndex(monthIndex, machineIndexForAllType[0] + 1);
                                setCurrentProductBatchIndex(monthIndex, machineIndexForAllType[0] + 1, productBatchIndex + 1);
                            }
                            /*
                            else //if(productNumInCurrentBatch[0] > gvariable.PRODUCT_BATCH_MAX_OUTPUT_NUM)   
                            {
                                //product batch number increase by one, need to get new month value, maybe it's now new month 
                                //for example, batch limitation is 12000, but we still have 38000
                                planTime1 = GetTimeFromInt(castStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
                                monthIndex = getMonthIndexForDispatchBatch(planTime1);
                                productBatchIndex = getCurrentProductBatchIndex(monthIndex, machineIndexForAllType[0] + 1) + 1;
                                setCurrentProductBatchIndex(monthIndex, machineIndexForAllType[0] + 1, productBatchIndex);

                                //get product batch number string
                                productBatchNumString = DateTime.Now.Date.ToString("yyMM") + (machineIndexForAllType[0] + 1) + productBatchIndex.ToString().PadLeft(2, '0');

                                productNumInCurrentBatch[0] = 0;
                            }
                             */
                        }
                        //end of get product batch string
                    }

                    workingTimeDeltaStamp = getDispatchFromSalesOrder(numForThisDispatch, productBatchNumString);

                    ret = 0;
                    for (i = 0; i < MAX_NUM_MACHINE_TYPE; i++)
                    {
                        //one type of machine(probably print machine is not used in this batch order)
                        if (machineIndexForAllType[i] < 0)
                            continue;

                        numForSalesOrderleft[i] -= numForThisDispatch[i];
                        if (numForSalesOrderleft[i] > 0)  //one of the processes is still unfinished
                            ret = 1;
                    }

                    if (ret == 0)
                        break;

                    castStartTimeStamp += workingTimeDeltaStamp;
                    printStartTimeStamp += workingTimeDeltaStamp;
                    slitStartTimeStamp += workingTimeDeltaStamp;
                }

                castStartTimeStamp = backcastStart;
                castEndTimeStamp = backCastEnd;
                printStartTimeStamp = backPrintstart;
                printEndTimeStamp = backPrintEnd;
                slitStartTimeStamp = backSlitStart;
                slitEndTimeStamp = backSlitEnd;

                getProductTaskSheetFromSalesOrder(total, productBatchNumString);

                //APS success, set the status of this batch order to already scheduled
                updateStr = "update `" + gVariable.productBatchTableName + "` set orderStatus = '" + gVariable.SALES_ORDER_STATUS_APS_OK + "', APSTime = '" + DateTime.Now.ToString("yy-MM-dd HH:mm" ) +
                            "', planTime1 = '" + GetTimeFromInt(productBatchStartTimeStamp).ToString("yyyy-MM-dd HH:mm") + "', planTime2 = '" +
                            GetTimeFromInt(productBatchEndTimeStamp).ToString("yyyy-MM-dd HH:mm") + "', batchNum = '" + productBatchNumString + "' where id = '" + productBatchID + "'";

                mySQLClass.updateTableItems(gVariable.globalDatabaseName, updateStr);

                setCurrentProductBatchIndex(monthIndex, machineIndexForAllType[0] + 1, productBatchIndex + 1);

                return 0;
            
            }
            catch (Exception ex)
            {
                Console.WriteLine("startAPSAction failed! " + ex);
                return -1;
            }
        }

        int generateForwardAPSPlans(string routeCode, int APSStartTimeStamp, int APSEndTimeStamp, int[] assignedMachineByUserArray)
        {
            int i, j, k;
            int tmpResult;
            int gotBestPlan;
            int APSSuccess;
            int startTimeStamp;
            int endTimeStamp;
            //a batch order can be completed in cast/print/slit process in xxx hours seperately
            int[] productBatchWorkingTime = new int[APS_MACHINE_NUM];

            try
            {
                APSSuccess = 0;

                //first get cast machine list
                assignAllPossibleMachines(routeCode, assignedMachineByUserArray);

                //total working time for one batch order by all machines
                getSalesOrderWorkingTime(productBatchWorkingTime);

                for (i = 0; i < MAX_NUM_MACHINE_TYPE; i++)
                {
                    startTimeTypeMachine[i] = -1;
                    endTimeTypeMachine[i] = -1;
                }

                gotBestPlan = 0;

                for (i = 0; i < castMachineList.Length; i++)
                {
                    //APS started from APSStartTimeStamp
                    tmpResult = fillOutPlanForOneMachine(MACHINE_TYPE_CAST, castMachineList[i], APSStartTimeStamp, APSEndTimeStamp, productBatchWorkingTime);
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
                            if (assignedMachineByUserArray[MACHINE_TYPE_SLIT] != -1)
                            {
                                //the user asigned slit machine, we use it
                                slitMachineList = new int[1];
                                slitMachineList[0] = assignedMachineByUserArray[MACHINE_TYPE_SLIT];
                            }
                            else if (castMachineList[i] == CAST_MACHINE_3 && productImpl.slitOnline == "1")  //还需要判断该产品是否需要在线分切，假如在线分切
                            {
                                //the cast machine #3 can only work with slit machine 3
                                slitMachineList = new int[1];
                                slitMachineList[0] = SLIT_MACHINE_3;
                            }
                            else if (castMachineList[i] == CAST_MACHINE_5 && productImpl.slitOnline == "1")  //还需要判断该产品是否需要在线分切，假如在线分切
                            {
                                //the cast machine #5 can only work with slit machine 5
                                slitMachineList = new int[1];
                                slitMachineList[0] = SLIT_MACHINE_5;
                            }
                            else
                            {
                                //no assignment, we need to try every slit machine one by one
                                slitMachineList = new int[5];
                                slitMachineList[0] = SLIT_MACHINE_1;
                                slitMachineList[1] = SLIT_MACHINE_2;
                                slitMachineList[2] = SLIT_MACHINE_3;
                                slitMachineList[3] = SLIT_MACHINE_4;
                                slitMachineList[4] = SLIT_MACHINE_5;
                            }

                            //there is no print process for this batch order, so go to slit process directly
                            for (j = 0; j < slitMachineList.Length; j++)
                            {
                                //cast speed is faster than slit machine, slit can start after cast by GAP_BETWEEN_CAST_SLIT
                                if (eachMachineOutputForOneHour[slitMachineList[j]] < eachMachineOutputForOneHour[castMachineList[i]])
                                {
                                    startTimeStamp = castStartTimeStamp + GAP_BETWEEN_CAST_SLIT;
                                    //get printer final end time
                                    endTimeStamp = APSEndTimeStamp;
                                }
                                else  //cast speed is slower than slit machine
                                {
                                    //cast machine should complete batch order before slit machine by GAP_BETWEEN_CAST_SLIT
                                    startTimeStamp = castEndTimeStamp + GAP_BETWEEN_CAST_SLIT - productBatchWorkingTime[slitMachineList[j]] * 3600;
                                    //get printer final end time
                                    endTimeStamp = APSEndTimeStamp;
                                }

                                //slit machine need to run after cast machine, so the speed is the same as cast machine, we use productBatchWorkingTime as a total working time of slit machine for this batch order
                                tmpResult = fillOutPlanForOneMachine(MACHINE_TYPE_SLIT, slitMachineList[j], startTimeStamp, endTimeStamp, productBatchWorkingTime);
                                if (tmpResult < 0)
                                {
                                    continue;
                                }

                                //slit Machine got a plan
                                if (machineSelectionMethod == MACHINE_ORDER_WEIGHT_MORE)  //order of machine in castMachineList/printMachineList/slitMachineList is more important
                                {
                                    //there is no previous best slit time or this slit time is better than previous best, new end earlier than old start
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
                                }
                                else //if (machineSelectionMethod == TIME_EQUALITY_WEIGHT_MORE)  //time balance, we got the earlist complete time 
                                {
                                    //there is no previous best slit time or this slit time is better than previous best, new end earlier than old end
                                    if (startTimeTypeMachine[MACHINE_TYPE_SLIT] == -1 || slitEndTimeStamp < endTimeTypeMachine[MACHINE_TYPE_SLIT])
                                    {
                                        //there is no workable plan by now 
                                        //or this new plan is better than the previous one, we use this new one
                                        gotBestPlan = 1;
                                    }
                                    else if (slitEndTimeStamp > endTimeTypeMachine[MACHINE_TYPE_SLIT])
                                    {
                                        //this plan is worse than previous ones, so just discard this
                                    }
                                    else if (castEndTimeStamp < endTimeTypeMachine[MACHINE_TYPE_CAST])
                                    {
                                        //slit plan for the current cast machine and previous best cast machine are similar, now check for cast plan
                                        //new cast plan is better, so we adopt the new plan
                                        gotBestPlan = 1;
                                    }
                                }

                                if (gotBestPlan == 1)
                                {
                                    //we got the best plan 
                                    startTimeTypeMachine[MACHINE_TYPE_CAST] = castStartTimeStamp;
                                    endTimeTypeMachine[MACHINE_TYPE_CAST] = castEndTimeStamp;
                                    machineIndexForAllType[MACHINE_TYPE_CAST] = castMachineList[i];

                                    startTimeTypeMachine[MACHINE_TYPE_SLIT] = slitStartTimeStamp;
                                    endTimeTypeMachine[MACHINE_TYPE_SLIT] = slitEndTimeStamp;
                                    machineIndexForAllType[MACHINE_TYPE_SLIT] = slitMachineList[j];

                                    APSSuccess = 1;
                                }

                                gotBestPlan = 0;
                            }
                            //at the end of this loop, we got the best plan for slit machine by using the current cast machine, then we need to try next cast machine to see if there is a better plan
                        }
                        else //if (machineIndexForAllType[MACHINE_TYPE_PRINT] == PRINT_MACHINE_X)
                        {
                            //print process is necessary
                            if (assignedMachineByUserArray[MACHINE_TYPE_PRINT] != -1)
                            {
                                //the user assigned print machine, use it
                                printMachineList = new int[1];
                                printMachineList[0] = assignedMachineByUserArray[MACHINE_TYPE_PRINT];
                            }
                            else
                            {
                                //try all the print machines one by one, but 柔印机 is not considered here
                                printMachineList = new int[4];
                                printMachineList[0] = PRINT_MACHINE_1;
                                printMachineList[1] = PRINT_MACHINE_2;
                                printMachineList[2] = PRINT_MACHINE_3;
                                printMachineList[3] = PRINT_MACHINE_4;
                            }

                            //slit process
                            if (assignedMachineByUserArray[MACHINE_TYPE_SLIT] != -1)
                            {
                                //the user assigned slit machine
                                slitMachineList = new int[1];
                                slitMachineList[0] = assignedMachineByUserArray[MACHINE_TYPE_SLIT];
                            }
                            else
                            {
                                slitMachineList = new int[5];
                                slitMachineList[0] = SLIT_MACHINE_1;
                                slitMachineList[1] = SLIT_MACHINE_2;
                                slitMachineList[2] = SLIT_MACHINE_3;
                                slitMachineList[3] = SLIT_MACHINE_4;
                                slitMachineList[4] = SLIT_MACHINE_5;
                            }
                            //try all print machines by using this cast machine plan (cast machine i)
                            for (j = 0; j < printMachineList.Length; j++)
                            {
                                //cast speed is faster than print machine
                                if (eachMachineOutputForOneHour[printMachineList[j]] < eachMachineOutputForOneHour[castMachineList[i]])
                                {
                                    startTimeStamp = castStartTimeStamp + GAP_BETWEEN_CAST_PRINT;
                                    //get printer final end time
                                    endTimeStamp = APSEndTimeStamp;
                                }
                                else  //cast speed is slower than print machine
                                {
                                    //cast machine should complete batch order before print machine by GAP_BETWEEN_CAST_PRINT
                                    startTimeStamp = castEndTimeStamp + GAP_BETWEEN_CAST_PRINT - productBatchWorkingTime[printMachineList[j]] * 3600;
                                    //get printer final end time
                                    endTimeStamp = APSEndTimeStamp;
                                }

                                //slit machine need to run after cast machine, so the speed is the same as cast machine, we use productBatchWorkingTime as a total working time of slit machine for this batch order
                                tmpResult = fillOutPlanForOneMachine(MACHINE_TYPE_PRINT, printMachineList[j], startTimeStamp, endTimeStamp, productBatchWorkingTime);
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
                                        //print speed is faster than slit machine
                                        if (eachMachineOutputForOneHour[slitMachineList[k]] < eachMachineOutputForOneHour[printMachineList[j]])
                                        {
                                            startTimeStamp = printStartTimeStamp + GAP_BETWEEN_PRINT_SLIT;
                                            //get printer final end time
                                            endTimeStamp = APSEndTimeStamp;
                                        }
                                        else  //print speed is slower than slit machine
                                        {
                                            //cast machine should complete batch order before print machine by GAP_BETWEEN_CAST_PRINT
                                            startTimeStamp = printEndTimeStamp + GAP_BETWEEN_PRINT_SLIT - productBatchWorkingTime[slitMachineList[k]] * 3600;
                                            //get printer final end time
                                            endTimeStamp = APSEndTimeStamp;
                                        }
                                        //slit machine need to run after cast machine, so the speed is the same as cast machine, we use productBatchWorkingTime as a total working time of slit machine for this batch order
                                        tmpResult = fillOutPlanForOneMachine(MACHINE_TYPE_SLIT, slitMachineList[k], startTimeStamp, endTimeStamp, productBatchWorkingTime);
                                        if (tmpResult < 0)
                                        {
                                            continue;
                                        }

                                        //slit Machine got a plan
                                        if (machineSelectionMethod == MACHINE_ORDER_WEIGHT_MORE)  //order of machine in castMachineList/printMachineList/slitMachineList is more important
                                        {
                                            //we've got the first or best slit machine plan for this print machine, now compare this plans and the previous best plan to see which one is better
                                            //there is no previous best slit time or this slit time is better than previous best
                                            if (startTimeTypeMachine[MACHINE_TYPE_SLIT] == -1 || slitEndTimeStamp < startTimeTypeMachine[MACHINE_TYPE_SLIT])
                                            {
                                                //we got the best plan by now
                                                //it is the first workable plan, input data directly to print and slit array for cast machine 0
                                                //or new print machine has a better slit plan than previous print machines
                                                gotBestPlan = 1;
                                            }
                                            else if (startTimeTypeMachine[MACHINE_TYPE_SLIT] > slitEndTimeStamp)
                                            {
                                                //the new slit plan is much worse than the previous best plan recorded in startTimeTypeMachine, just discard the new one
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
                                        }
                                        else //if (machineSelectionMethod == TIME_EQUALITY_WEIGHT_MORE)  //time balance, we got the earlist complete time 
                                        {
                                            //we've got the first or best slit machine plan for this print machine, now compare this plans and the previous best plan to see which one is better
                                            //there is no previous best slit time or this slit time is better than previous best
                                            if (startTimeTypeMachine[MACHINE_TYPE_SLIT] == -1 || slitEndTimeStamp < endTimeTypeMachine[MACHINE_TYPE_SLIT])
                                            {
                                                //we got the best plan by now
                                                //it is the first workable plan, input data directly to print and slit array for cast machine 0
                                                //or new print machine has a better slit plan than previous print machines
                                                gotBestPlan = 1;
                                            }
                                            else if (slitEndTimeStamp > endTimeTypeMachine[MACHINE_TYPE_SLIT])
                                            {
                                                //the new slit plan is worse than the previous best plan recorded in startTimeTypeMachine, just discard the new one
                                            }
                                            else
                                            {
                                                //slit plans are similar, need to continue check for print plan
                                                //this print plan is better than the previoud best plan, use this one
                                                if (printEndTimeStamp < endTimeTypeMachine[MACHINE_TYPE_PRINT])
                                                {
                                                    gotBestPlan = 1;
                                                }
                                                else if (printEndTimeStamp > endTimeTypeMachine[MACHINE_TYPE_PRINT])
                                                {
                                                    //the new print plan for this cast machine is much worse than the previous best plan recorded in startTimeTypeMachine, just discard the new one
                                                }
                                                else if (castEndTimeStamp < endTimeTypeMachine[MACHINE_TYPE_CAST])
                                                {
                                                    //both slit plans and print plans are similar, need to continue check for cast plan
                                                    //this cast plan is better than the previoud best plan, use this one
                                                    gotBestPlan = 1;
                                                }
                                            }
                                        }

                                        //record this plan for 4 types of machines
                                        if (gotBestPlan == 1)
                                        {
                                            //we got the best plan 
                                            startTimeTypeMachine[MACHINE_TYPE_CAST] = castStartTimeStamp;
                                            endTimeTypeMachine[MACHINE_TYPE_CAST] = castEndTimeStamp;
                                            machineIndexForAllType[MACHINE_TYPE_CAST] = castMachineList[i];

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

                if (APSSuccess == 1)
                {
                    //APS action completed, do some routine work 
                    afterAPSAction(APSSuccess);

                    return 0;
                }
                else
                    return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("generateAPSPlans failed!" + ex);
                return -1;
            }
        }


        //routeCode - 工艺路线, what kind of material this is and waht kind of machine can be adopted
        //assignedMachineByUserArray: whether machine is assigned by user in APS UI
        void assignAllPossibleMachines(string routeCode, int[] assignedMachineByUserArray)
        {
            int i;
            string commandText;
            int[] breathableFilmCast = { CAST_MACHINE_1, CAST_MACHINE_3, CAST_MACHINE_5};
            int[] unbreathableFilmCast = { CAST_MACHINE_4, CAST_MACHINE_2};
            string[] strArray1;
            string[] strArray2;
            string[,] tableArray;

            //sorting all machines by one of the machine's main characters
            commandText = "select * from `" + gVariable.machineTableName + "`";
            tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
            if (tableArray == null)
            {
                MessageBox.Show("read machine info in basic data filed！", "提示信息", MessageBoxButtons.OK);
                System.Environment.Exit(0);
            }
            else
            {
                //get prepare time for breathable cast machine in strArray1
                strArray1 = new string[breathableFilmCast.Length];
                for(i = 0; i < breathableFilmCast.Length; i++)
                {
                    strArray1[i] = tableArray[breathableFilmCast[i], 8];
                }
                toolClass.stringSortingRecordIndex(strArray1, breathableFilmCast);

                //get prepare time for unbreathable cast machine in strArray2
                strArray2 = new string[unbreathableFilmCast.Length];
                for(i = 0; i < unbreathableFilmCast.Length; i++)
                {
                    strArray2[i] = tableArray[unbreathableFilmCast[i], 8];
                }
                toolClass.stringSortingRecordIndex(strArray2, unbreathableFilmCast);
            }

            //if cast machine is assigned
            if (assignedMachineByUserArray[MACHINE_TYPE_CAST] != -1)
            {
                castMachineList = new int[1];
                castMachineList[0] = assignedMachineByUserArray[MACHINE_TYPE_CAST];
            }

            //if print machine is assigned
            if (assignedMachineByUserArray[MACHINE_TYPE_PRINT] != -1)
            {
                printMachineList = new int[1];
                printMachineList[0] = assignedMachineByUserArray[MACHINE_TYPE_PRINT];
            }

            //if slit machine is assigned
            if (assignedMachineByUserArray[MACHINE_TYPE_SLIT] != -1)
            {
                slitMachineList = new int[1];
                slitMachineList[0] = assignedMachineByUserArray[MACHINE_TYPE_SLIT];
            }

            //first set it to 0, if no print process needed, the following code will set it to -1
            machineIndexForAllType[MACHINE_TYPE_PRINT] = 0;

            //根据工艺路线确定流延设备
            switch (routeCode)
            {
                case "A00":  //非透气膜 -- 2/4 流延机 + 2/4 分切机, 不印刷
                    if (castMachineList == null)  //cast machine is not assigned
                    {
                        castMachineList = new int[unbreathableFilmCast.Length];
                        for (i = 0; i < unbreathableFilmCast.Length; i++)
                        {
                            castMachineList[i] = unbreathableFilmCast[i];
                        }
                    }
                    else
                    {
                        //keep the assigned cast machine
                    }
                    machineIndexForAllType[MACHINE_TYPE_PRINT] = PRINT_MACHINE_X;
                    break;
                case "A01":  //非透气印刷膜 -- 2/4 流延机 + 1/2/3 印刷机 + 2/4 分切机
                    if (castMachineList == null)  //cast machine is not assigned
                    {
                        castMachineList = new int[unbreathableFilmCast.Length];
                        for (i = 0; i < unbreathableFilmCast.Length; i++)
                        {
                            castMachineList[i] = unbreathableFilmCast[i];
                        }
                    }
                    else
                    {
                        //keep the assigned cast machine
                    }
                    break;
                case "A10":  //透气膜 -- 1/3/5 流延机 + 1/3/5 分切机
                    if (castMachineList == null)  //cast machine is not assigned
                    {
                        castMachineList = new int[breathableFilmCast.Length];
                        for (i = 0; i < breathableFilmCast.Length; i++)
                        {
                            castMachineList[i] = breathableFilmCast[i];
                        }
                    }
                    else
                    {
                        //keep the assigned cast machine
                    }
                    machineIndexForAllType[MACHINE_TYPE_PRINT] = PRINT_MACHINE_X;
                    break;
                case "A11":  //透气印刷膜 -- 1/2/3 流延机 + 1/2/3 印刷机 + 1/2/3 分切机 
                    if (castMachineList == null)  //cast machine is not assigned
                    {
                        castMachineList = new int[breathableFilmCast.Length];
                        for (i = 0; i < breathableFilmCast.Length; i++)
                        {
                            castMachineList[i] = breathableFilmCast[i];
                        }
                    }
                    else
                    {
                        //keep the assigned cast machine
                    }
                    break;
                default:
                    Console.WriteLine("\r\n product route code error!\r\n");
                    if (castMachineList == null)  //cast machine is not assigned
                    {
                        castMachineList = new int[2];
                        castMachineList[0] = CAST_MACHINE_4;
                        castMachineList[1] = CAST_MACHINE_2;
                    }
                    else
                    {
                        //keep the assigned cast machine
                    }
                    machineIndexForAllType[MACHINE_TYPE_PRINT] = PRINT_MACHINE_X;
                    break;
            }

            //if we are using forwad APS, we only need to confirm cast machine, print/slit machine should be assigned after cast machine is confirmed 
            if (APSPlanDirection == FORWARD_APS_PLAN)
                return;

            //we are in reversed APS, we need to confirm cast/print/slit machine, so we can start from slit machine, that is reverse APS
            if (assignedMachineByUserArray[MACHINE_TYPE_PRINT] != -1)
            {
                //user assigned print machine in UI
                printMachineList = new int[1];
                printMachineList[0] = assignedMachineByUserArray[MACHINE_TYPE_PRINT];
            }
            else
            {
                //cast Machine already OK, try print and slit machine
                if (machineIndexForAllType[MACHINE_TYPE_PRINT] == PRINT_MACHINE_X)
                {
                    //if cast machine #3/#5 don't need printing, it will do slit online, so slit machine 3 or 5 is the only choice
                    //if we have a print process, we should use slit machine 1/6/7, which are separate slit machines
                    if (assignedMachineByUserArray[MACHINE_TYPE_SLIT] != -1)
                    {
                        //the user asigned slit machine, we use it
                        slitMachineList = new int[1];
                        slitMachineList[0] = assignedMachineByUserArray[MACHINE_TYPE_SLIT];
                    }
                    else
                    {
                        //no assignment, we need to try every slit machine one by one
                        if (productImpl.slitOnline == "1")
                        {
                            slitMachineList = new int[2];
                            slitMachineList[0] = SLIT_MACHINE_3;
                            slitMachineList[1] = SLIT_MACHINE_5;
                        }
                        else
                        {
                            slitMachineList = new int[5];
                            slitMachineList[0] = SLIT_MACHINE_1;
                            slitMachineList[1] = SLIT_MACHINE_2;
                            slitMachineList[2] = SLIT_MACHINE_3;
                            slitMachineList[3] = SLIT_MACHINE_4;
                            slitMachineList[4] = SLIT_MACHINE_5;
                        }
                    }
                }
                else
                {
                    //print process is necessary
                    if (assignedMachineByUserArray[MACHINE_TYPE_PRINT] != -1)
                    {
                        //the user assigned print machine, use it
                        printMachineList = new int[1];
                        printMachineList[0] = assignedMachineByUserArray[MACHINE_TYPE_PRINT];
                    }
                    else
                    {
                        //try all the print machines one by one, 柔印机 is not considered here
                        printMachineList = new int[4];
                        printMachineList[0] = PRINT_MACHINE_1;
                        printMachineList[1] = PRINT_MACHINE_3;
                        printMachineList[2] = PRINT_MACHINE_4;
                        printMachineList[3] = PRINT_MACHINE_2;
                    }

                    //slit process
                    if (assignedMachineByUserArray[MACHINE_TYPE_SLIT] != -1)
                    {
                        //the user assigned slit machine
                        slitMachineList = new int[1];
                        slitMachineList[0] = assignedMachineByUserArray[MACHINE_TYPE_SLIT];
                    }
                    else
                    {
                        //no assignment, we need to try every slit machine one by one
                        slitMachineList = new int[5];
                        slitMachineList[0] = SLIT_MACHINE_1;
                        slitMachineList[1] = SLIT_MACHINE_2;
                        slitMachineList[2] = SLIT_MACHINE_3;
                        slitMachineList[3] = SLIT_MACHINE_4;
                        slitMachineList[4] = SLIT_MACHINE_5;
                    }
                }
            }
        }

        int generateReversedAPSPlans(string routeCode, int APSStartTimeStamp, int APSEndTimeStamp, int[] assignedMachineByUserArray)
        {
            int i, j, k;
            int tmpResult;
            int gotBestPlan;
            int APSSuccess;
            int startTimeStamp;
            int endTimeStamp;
            //a batch order can be completed in cast/print/slit process in xxx hours seperately
            int[] productBatchWorkingTime = new int[APS_MACHINE_NUM];

            try
            {
                APSSuccess = 0;

                assignAllPossibleMachines(routeCode, assignedMachineByUserArray);

                for (i = 0; i < MAX_NUM_MACHINE_TYPE; i++)
                {
                    startTimeTypeMachine[i] = -1;
                    endTimeTypeMachine[i] = -1;
                }

                gotBestPlan = 0;

                for (i = 0; i < slitMachineList.Length; i++)
                {
                    //add judgment here, if a slit machine is not supported, continue
                    //

                    //total working time for one batch order by using this machine
                    getSalesOrderWorkingTime(productBatchWorkingTime);

                    //APS started from dispatchStartTimeStamp
                    tmpResult = fillOutPlanForOneMachine(MACHINE_TYPE_SLIT, slitMachineList[i], APSStartTimeStamp, APSEndTimeStamp, productBatchWorkingTime);
                    
                    if (tmpResult < 0)
                    {
                        continue;
                    }
                    else
                    {
                        //slit Machine got a plan, try print and cast machine
                        if (machineIndexForAllType[MACHINE_TYPE_PRINT] == PRINT_MACHINE_X)
                        {
                            //there is no print process for this batch order, so go to cast process directly
                            for (j = 0; j < castMachineList.Length; j++)
                            {
                                //cast speed is faster than slit machine
                                if (eachMachineOutputForOneHour[slitMachineList[i]] < eachMachineOutputForOneHour[castMachineList[j]])
                                {
                                    //slit machine need to run after print machine, so the 2 machine could finish at the same time, 
                                    //otherwise, slit machine will have no printed roll to cut, so the machine end time is fixed
                                    if (APSStartTimeStamp > slitStartTimeStamp - GAP_BETWEEN_CAST_SLIT)
                                        continue;  //there is not enough time for cast machine to start before slit machine, so this cast machine is not suitable for APS

                                    startTimeStamp = APSStartTimeStamp;
                                    //get printer final end time, if exceed this value, slit machine will not have enough time to finish the batch order before slitEndTimeStamp 
                                    endTimeStamp = slitStartTimeStamp + productBatchWorkingTime[castMachineList[j]] * 3600;
                                }
                                else  //cast speed is slower than slit machine
                                {
                                    //cast machine should should complete batch order before slit machine by GAP_BETWEEN_CAST_SLIT
                                    startTimeStamp = APSStartTimeStamp;
                                    //get printer final end time, if exceed this value, slit machine will not have enough time to finish the batch order before slitEndTimeStamp 
                                    endTimeStamp = slitEndTimeStamp - GAP_BETWEEN_CAST_SLIT;
                                }

                                //cast machine need to run before slit machine, and the start time should be still APSStartTimeStamp, and end time should be a little it earlier than slit start time
                                tmpResult = fillOutPlanForOneMachine(MACHINE_TYPE_CAST, castMachineList[j], startTimeStamp, endTimeStamp, productBatchWorkingTime);
                                if (tmpResult < 0)
                                {
                                    continue;
                                }

                                //cast Machine got a plan
                                if (machineSelectionMethod == MACHINE_ORDER_WEIGHT_MORE)  //order of machine in castMachineList/printMachineList/slitMachineList is more important
                                {
                                    //there is no previous best cast time or this cast time is better than previous best
                                    if (startTimeTypeMachine[MACHINE_TYPE_CAST] == -1 || castStartTimeStamp > endTimeTypeMachine[MACHINE_TYPE_CAST])
                                    {
                                        //there is no workable plan by now 
                                        //or this new plan is better than the previous one, we use this new one
                                        gotBestPlan = 1;
                                    }
                                    else if (castStartTimeStamp < endTimeTypeMachine[MACHINE_TYPE_CAST])
                                    {
                                        //this plan is worse than previous ones, so just discard this
                                    }
                                    else if (slitEndTimeStamp > endTimeTypeMachine[MACHINE_TYPE_SLIT])  //even we are in machine order weight more mode, slit machine still work in time_equality_weight_more mode
                                    {
                                        //cast plan for the current slit machine and previous best slit machine are similar, now check for slit plan
                                        //new slit plan is better, so we adopt the new plan
                                        gotBestPlan = 1;
                                    }
                                }
                                else //if (machineSelectionMethod == TIME_EQUALITY_WEIGHT_MORE)  //time balance, try to get the last start time 
                                {
                                    //there is no previous best cast time or this cast time is better than previous best
                                    if (startTimeTypeMachine[MACHINE_TYPE_CAST] == -1 || castEndTimeStamp > endTimeTypeMachine[MACHINE_TYPE_CAST])
                                    {
                                        //there is no workable plan by now 
                                        //or this new plan is better than the previous one, we use this new one
                                        gotBestPlan = 1;
                                    }
                                    else if (castStartTimeStamp < endTimeTypeMachine[MACHINE_TYPE_CAST])
                                    {
                                        //this plan is worse than previous ones, so just discard this
                                    }
                                    else if (slitEndTimeStamp > endTimeTypeMachine[MACHINE_TYPE_SLIT])
                                    {
                                        //cast plan for the current slit machine and previous best slit machine are similar, now check for slit plan
                                        //new slit plan is better, so we adopt the new plan
                                        gotBestPlan = 1;
                                    }
                                }

                                if (gotBestPlan == 1)
                                {
                                    //we got the best plan 
                                    startTimeTypeMachine[MACHINE_TYPE_CAST] = castStartTimeStamp;
                                    endTimeTypeMachine[MACHINE_TYPE_CAST] = castEndTimeStamp;
                                    machineIndexForAllType[MACHINE_TYPE_CAST] = castMachineList[j];

                                    startTimeTypeMachine[MACHINE_TYPE_SLIT] = slitStartTimeStamp;
                                    endTimeTypeMachine[MACHINE_TYPE_SLIT] = slitEndTimeStamp;
                                    machineIndexForAllType[MACHINE_TYPE_SLIT] = slitMachineList[i];

                                    APSSuccess = 1;
                                }

                                gotBestPlan = 0;
                            }
                            //at the end of this loop, we got the best plan for cast machine by using the current slit machine, then we need to try next slit machine to see if there is a better plan
                        }
                        else //if (machineIndexForAllType[MACHINE_TYPE_PRINT] != PRINT_MACHINE_X)
                        {
                            //print process is necessary

                            //try all print machines by using this slit machine plan (slit machine i)
                            for (j = 0; j < printMachineList.Length; j++)
                            {
                                //print speed is faster than slit machine
                                if (eachMachineOutputForOneHour[slitMachineList[i]] < eachMachineOutputForOneHour[printMachineList[j]])
                                {
                                    //slit machine need to run after print machine, so the 2 machine could finish at the same time, 
                                    //otherwise, slit machine will have no printed roll to cut, so the machine end time is fixed
                                    if(APSStartTimeStamp > slitStartTimeStamp - GAP_BETWEEN_PRINT_SLIT)
                                        continue;  //there is not enough time for printer to start before slit machine, so this printer is not suitable for APS

                                    startTimeStamp = APSStartTimeStamp;
                                    //get printer final end time, if exceed this value, slit machine will not have enough time to finish the batch order before slitEndTimeStamp 
                                    endTimeStamp = slitStartTimeStamp + productBatchWorkingTime[printMachineList[j]] * 3600;  
                                }
                                else  //print speed is slower than slit machine
                                {
                                    //print machine should should complete batch order before slit machine by GAP_BETWEEN_PRINT_SLIT
                                    startTimeStamp = APSStartTimeStamp;
                                    //get printer final end time, if exceed this value, slit machine will not have enough time to finish the batch order before slitEndTimeStamp 
                                    endTimeStamp = slitEndTimeStamp - GAP_BETWEEN_PRINT_SLIT;
                                }

                                //try to get a plan for this printer
                                tmpResult = fillOutPlanForOneMachine(MACHINE_TYPE_PRINT, printMachineList[j], startTimeStamp, endTimeStamp, productBatchWorkingTime);
                                if (tmpResult < 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    //print Machine got a plan, now try all cast machines based on this print machine plan 
                                    for (k = 0; k < castMachineList.Length; k++)
                                    {
                                        //cast speed is faster than print machine
                                        if (eachMachineOutputForOneHour[printMachineList[j]] < eachMachineOutputForOneHour[castMachineList[k]])
                                        {
                                            //print machine need to run after cast machine, so the 2 machine could finish at the same time, 
                                            //otherwise, print machine will have no casted roll to print, so the machine end time is fixed
                                            if (APSStartTimeStamp > printStartTimeStamp - GAP_BETWEEN_CAST_PRINT)
                                                continue;  //there is not enough time for printer to start before slit machine, so this printer is not suitable for APS

                                            startTimeStamp = APSStartTimeStamp;
                                            //get cast final end time, if exceed this value, print machine will not have enough time to finish the batch order before printEndTimeStamp 
                                            endTimeStamp = printStartTimeStamp + productBatchWorkingTime[castMachineList[k]] * 3600;
                                        }
                                        else  //cast speed is slower than print machine
                                        {
                                            //cast machine should complete batch order before print machine by GAP_BETWEEN_CAST_PRINT
                                            startTimeStamp = APSStartTimeStamp;
                                            //get printer final end time, if exceed this value, slit machine will not have enough time to finish the batch order before slitEndTimeStamp 
                                            endTimeStamp = printEndTimeStamp - GAP_BETWEEN_CAST_PRINT;
                                        }

                                        //cast machine need to run after cast machine, so the speed is the same as cast machine, we use productBatchWorkingTime as a total working time of slit machine for this batch order
                                        tmpResult = fillOutPlanForOneMachine(MACHINE_TYPE_CAST, castMachineList[k], startTimeStamp, endTimeStamp, productBatchWorkingTime);
                                        if (tmpResult < 0)
                                        {
                                            continue;
                                        }

                                        if (machineSelectionMethod == MACHINE_ORDER_WEIGHT_MORE)  //order of machine in castMachineList/printMachineList/slitMachineList is more important
                                        {
                                            //we've got the first or best cast machine plan for this print machine, now compare this plans and the previous best plan to see which one is better
                                            if (startTimeTypeMachine[MACHINE_TYPE_CAST] == -1 || castStartTimeStamp > endTimeTypeMachine[MACHINE_TYPE_CAST])
                                            {
                                                //we got the best plan by now
                                                //it is the first workable plan, input data directly to print and cast array for slit machine 0
                                                //or new print machine has a better cast plan than previous print machines
                                                gotBestPlan = 1;
                                            }
                                            else if (castStartTimeStamp < startTimeTypeMachine[MACHINE_TYPE_CAST])
                                            {
                                                //the new cast plan for this print machine is much worse than the previous best plan recorded in startTimeTypeMachine, just discard the new one
                                            }
                                            else
                                            {
                                                //cast plans are similar, need to continue check for print plan
                                                //this print plan is better than the previoud best plan, use this one
                                                if (printStartTimeStamp > endTimeTypeMachine[MACHINE_TYPE_PRINT])
                                                {
                                                    gotBestPlan = 1;
                                                }
                                                else if (printStartTimeStamp < endTimeTypeMachine[MACHINE_TYPE_PRINT])
                                                {
                                                    //the new print plan for this cast machine is much worse than the previous best plan recorded in startTimeTypeMachine, just discard the new one
                                                }
                                                else if (slitEndTimeStamp > startTimeTypeMachine[MACHINE_TYPE_SLIT]) //slit machine doesnot follow MACHINE_ORDER_WEIGHT_MORE, slit always works in TIME_EQUALITY_WEIGHT_MORE
                                                {
                                                    //both cast plans and print plans are similar, need to continue check for slit plan
                                                    //this slit plan is better than the previoud best plan, use this one
                                                    gotBestPlan = 1;
                                                }
                                            }
                                        }
                                        else //if (machineSelectionMethod == TIME_EQUALITY_WEIGHT_MORE)  //time balance, try to get the last start time 
                                        {
                                            //we've got the first or best cast machine plan for this print machine, now compare this plans and the previous best plan to see which one is better
                                            if (startTimeTypeMachine[MACHINE_TYPE_CAST] == -1 || castStartTimeStamp > startTimeTypeMachine[MACHINE_TYPE_CAST])
                                            {
                                                //we got the best plan by now
                                                //it is the first workable plan, input data directly to print and cast array for slit machine 0
                                                //or new print machine has a better cast plan than previous print machines
                                                gotBestPlan = 1;
                                            }
                                            else if (castStartTimeStamp < startTimeTypeMachine[MACHINE_TYPE_CAST])
                                            {
                                                //the new cast plan for this print machine is much worse than the previous best plan recorded in startTimeTypeMachine, just discard the new one
                                            }
                                            else
                                            {
                                                //cast plans are similar, need to continue check for print plan
                                                //this print plan is better than the previoud best plan, use this one
                                                if (printStartTimeStamp > startTimeTypeMachine[MACHINE_TYPE_PRINT])
                                                {
                                                    gotBestPlan = 1;
                                                }
                                                else if (printStartTimeStamp < startTimeTypeMachine[MACHINE_TYPE_PRINT])
                                                {
                                                    //the new print plan for this cast machine is much worse than the previous best plan recorded in startTimeTypeMachine, just discard the new one
                                                }
                                                else if (slitStartTimeStamp > startTimeTypeMachine[MACHINE_TYPE_SLIT])
                                                {
                                                    //both cast plans and print plans are similar, need to continue check for slit plan
                                                    //this slit plan is better than the previoud best plan, use this one
                                                    gotBestPlan = 1;
                                                }
                                            }
                                        }

                                        //record this plan for 3 types of machines
                                        if (gotBestPlan == 1)
                                        {
                                            //we got the best plan 
                                            startTimeTypeMachine[MACHINE_TYPE_CAST] = castStartTimeStamp;
                                            endTimeTypeMachine[MACHINE_TYPE_CAST] = castEndTimeStamp;
                                            machineIndexForAllType[MACHINE_TYPE_CAST] = castMachineList[k];

                                            startTimeTypeMachine[MACHINE_TYPE_PRINT] = printStartTimeStamp;
                                            endTimeTypeMachine[MACHINE_TYPE_PRINT] = printEndTimeStamp;
                                            machineIndexForAllType[MACHINE_TYPE_PRINT] = printMachineList[j];

                                            startTimeTypeMachine[MACHINE_TYPE_SLIT] = slitStartTimeStamp;
                                            endTimeTypeMachine[MACHINE_TYPE_SLIT] = slitEndTimeStamp;
                                            machineIndexForAllType[MACHINE_TYPE_SLIT] = slitMachineList[i];

                                            APSSuccess = 1;
                                            gotBestPlan = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (APSSuccess == 1)
                {
                    //APS action completed, do some routine work 
                    afterAPSAction(APSSuccess);

                    return 0;
                }
                else
                    return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("generateAPSPlans failed!" + ex);
                return -1;
            }
        }


        void afterAPSAction(int APSSuccess)
        {
            //if no plan found, return -1
            if (APSSuccess == 0)
            {
                string str;

                str = "订单" + productBatchImpl.salesOrderBatchCode + "排程失败，请检查交货时间等信息，谢谢！";
                MessageBox.Show(str, "信息提示", MessageBoxButtons.OK);

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
            }
        }

        //find free time for every machine, then put dispatch to suitable machines to make sure the batch order can be completed by delivery time 
        //machineindex: from 0 to 17, alogether 18 machines
        //APSStartTimeStamp: start time for APS
        //APSDeliveryTimeStamp: complete time for this APS plan
        //return: time stamp for the start time of the plan 
        //        -1 failed to find a plan
        int fillOutPlanForOneMachine(int type, int machineIndex, int startTimeStamp, int endTimeStamp, int [] productBatchWorkingTime)
        {
            int j, k;
            int flag;
            int len;
            int ret;
            int productBatchWorkingTimeForThisMachine;
            //int currentTimeStamp;

            ret = -1;
            try
            {
                productBatchWorkingTimeForThisMachine = productBatchWorkingTime[machineIndex];
                flag = 0;

                if (APSPlanDirection == FORWARD_APS_PLAN)
                {
                    k = 0;
                    //APS_TOTAL_PERIOD is 2 month counted by hours, we need to finish a batch order within 2 month
                    //j is the offset of hours starting from APSStartTimeStamp
                    for (j = 0; j < APS_TOTAL_PERIOD; j++)
                    {
                        //this array stores dispatch code in this time slot, if it is null, that means no dispatch for this time
                        if (machineWorkingPlanStatus[machineIndex, j] == null)  //free time for this machine
                        {
                            if (flag == 0)  //remember start position for free time, we need this value to calculate the start point for this dispatch, flag will keep on increasing
                                k = j;

                            if (calendarStartTimeStamp + j * 3600 <= startTimeStamp)
                            {
                                //it is still too early to start checking, we need to wait until APSStartTimeStamp to check if the machine is free
                                flag = 0;
                                k = j;
                            }
                            else if (flag >= productBatchWorkingTimeForThisMachine)  //free time larger than required for this batch order
                            {
                                if (calendarStartTimeStamp + j * 3600 < endTimeStamp)
                                {
                                    switch (type)
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
                else //if(APSPlanDirection == REVERSE_APS_PLAN)
                {
                    //APS start is earlier than today 
                    //if (calendarStartTimeStamp > startTimeStamp)
                    {
                        len = (endTimeStamp - calendarStartTimeStamp) / 3600;
                    }
                    //else
                    //{
                    //    len = (endTimeStamp - startTimeStamp) / 3600;
                    //}

                    k = len;
                    //APS_TOTAL_PERIOD is 2 month counted by hours, we need to finish a batch order within 2 month
                    //j is the offset of hours starting from APSStartTimeStamp
                    for (j = len; j >= 0; j--)
                    {
                        //this array stores dispatch code in this time slot, if it is null, that means no dispatch for this time
                        if (machineWorkingPlanStatus[machineIndex, j] == null)  //free time for this machine
                        {
                            if (flag == 0)  //remember start position for free time, we need this value to calculate the start point for this dispatch, flag will keep on increasing
                                k = j;

                            if (calendarStartTimeStamp + j * 3600 > endTimeStamp)
                            {
                                //it is outside the delivery time, we need move backward to check if the machine is free
                                flag = 0;
                                k = j;
                            }
                            else if (flag >= productBatchWorkingTimeForThisMachine)  //free time larger than required for this batch order
                            {
                                switch (type)
                                {
                                    case MACHINE_TYPE_CAST:
                                        castStartTimeStamp = endTimeStamp - (len - j) * 3600;
                                        castEndTimeStamp = endTimeStamp - (len - k) * 3600;
                                        break;
                                    case MACHINE_TYPE_PRINT:
                                        printStartTimeStamp = endTimeStamp - (len - j) * 3600;
                                        printEndTimeStamp = endTimeStamp - (len - k) * 3600;
                                        break;
                                    case MACHINE_TYPE_SLIT:
                                        slitStartTimeStamp = endTimeStamp - (len - j) * 3600;
                                        slitEndTimeStamp = endTimeStamp - (len - k) * 3600;
                                        break;
                                }
                                ret = 0;
                                break;
                            }
                            flag++;
                        }
                        else //if(machineWorkingPlanStatus[i, j] != null)  //this machine is busy, we should restart calculating free period again
                        {
                            flag = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (APSPlanDirection == FORWARD_APS_PLAN)
                    Console.WriteLine("fillOutPlanForOneMachine failed for forward APS plan! " + ex);
                else
                    Console.WriteLine("fillOutPlanForOneMachine failed for reverse APS plan! " + ex);
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


        //get month index value starting from 2018/1, so 2018/5 will return 4, 2020/1 will return 24
        int getMonthIndexForDispatchBatch(string timeStr)
        {
            int year;
            int month;

            try
            {
                year = Convert.ToInt32(timeStr.Substring(0, 4));
                month = Convert.ToInt32(timeStr.Substring(5, 2));

                return (year - 2018) * 12 + month;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getMonthIndexForDispatchBatch() failed for format error! " + ex);
                return 0;
            }
        }

        //get current batch Index value(产品批次号)
        //productBatchCurrentIndexTableName is a database table that recorded the newest batch ID for all months starting from 2018-01 until 2048-01 
        int getCurrentProductBatchIndex(int monthIndex, int machineID)
        {
            int ret;
            string[,] tableArray;
            string commandText;

            commandText = "select * from `" + gVariable.productBatchCurrentIndexTableName + "` where id = '" + monthIndex + "'";
            tableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
            if (tableArray != null)
            {
                ret = Convert.ToInt32(tableArray[0, machineID]);
            }
            else
                ret = 0;

            return ret;
        }

        //set current batch Index value(产品批次号)
        //productBatchCurrentIndexTableName is a database table that recorded the newest batch ID for all months starting from 2018-01 until 2048-01 
        void setCurrentProductBatchIndex(int monthIndex, int machineID, int value)
        {
            string str;
            string commandText;

            str = "dispatchIndex" + machineID;
            commandText = "update `" + gVariable.productBatchCurrentIndexTableName + "` set " + str + " = '" + value + "' where id = '" + monthIndex + "'";
            mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);
        }

        private void getProductTaskSheetFromSalesOrder(int outputNum, string productBatchNumString)
        {
            string dName;

            //finshiTimeStampForReturn = 0;
            try
            {
                dName = gVariable.globalDatabaseName;

                gVariable.dispatchSheetStruct productionTaskSheet = new gVariable.dispatchSheetStruct();
                productionTaskSheet.machineID = (machineIndexForAllType[MACHINE_TYPE_CAST] + 1).ToString();
                productionTaskSheet.batchNum = productBatchNumString;
                productionTaskSheet.dispatchCode = productBatchNumString + "L" + (machineIndexForAllType[MACHINE_TYPE_CAST] - gVariable.castingProcess[0] + 2);
                //cast and feed process stat at the same time
                productionTaskSheet.planTime1 = GetTimeFromInt(castStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
                productionTaskSheet.planTime2 = GetTimeFromInt(castEndTimeStamp).ToString("yyyy-MM-dd HH:mm");
                productionTaskSheet.productCode = productImpl.productCode;
                productionTaskSheet.productName = productImpl.productName;
                productionTaskSheet.operatorName = "未定";
                productionTaskSheet.plannedNumber = outputNum;
                productionTaskSheet.outputNumber = 0;
                productionTaskSheet.qualifiedNumber = 0;
                productionTaskSheet.unqualifiedNumber = 0;
                productionTaskSheet.processName = processNameArray[0];
                productionTaskSheet.realStartTime = "";
                productionTaskSheet.prepareTimePoint = "";
                productionTaskSheet.realFinishTime = "";
                productionTaskSheet.status = gVariable.MACHINE_STATUS_DISPATCH_GENERATED;
                productionTaskSheet.waitForCheck = 0;
                productionTaskSheet.wastedOutput = 0;
                productionTaskSheet.workTeam = "";
                productionTaskSheet.multiProduct = "0";
                productionTaskSheet.batchNum1 = "";
                productionTaskSheet.batchNum2 = "";
                productionTaskSheet.batchNum3 = "";
                productionTaskSheet.workshop = workshopNameArray[0];
                productionTaskSheet.workshift = "";
                productionTaskSheet.salesOrderCode = productBatchImpl.originalSalesOrderCode;
                productionTaskSheet.salesOrderBatchCode = productBatchImpl.salesOrderBatchCode;
                productionTaskSheet.BOMCode = productImpl.BOMCode;
                productionTaskSheet.customer = productBatchImpl.customer;

                mySQLClass.writeDataToDispatchListTable(dName, gVariable.globalProductTaskTableName, productionTaskSheet);
                dispatchTools.generateMaterialList(dName, gVariable.globalMaterialTaskTableName, productionTaskSheet.dispatchCode, productionTaskSheet.machineID, 
                                                   outputNum, productBatchNumString, productBatchImpl.salesOrderBatchCode, productImpl);

                if (machineIndexForAllType[MACHINE_TYPE_PRINT] != -1)
                {
                    //print process
                    productionTaskSheet.machineID = (machineIndexForAllType[MACHINE_TYPE_PRINT] + 1).ToString();
                    productionTaskSheet.dispatchCode = productBatchNumString + "Y" + (machineIndexForAllType[MACHINE_TYPE_PRINT] - gVariable.printingProcess[0] + 2);
                    productionTaskSheet.planTime1 = GetTimeFromInt(printStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
                    productionTaskSheet.planTime2 = GetTimeFromInt(printEndTimeStamp).ToString("yyyy-MM-dd HH:mm");
                    productionTaskSheet.productCode = productImpl.productCode;
                    productionTaskSheet.productName = productImpl.productName;
                    productionTaskSheet.operatorName = "未定";
                    productionTaskSheet.plannedNumber = outputNum;
                    productionTaskSheet.outputNumber = 0;
                    productionTaskSheet.qualifiedNumber = 0;
                    productionTaskSheet.unqualifiedNumber = 0;
                    productionTaskSheet.processName = processNameArray[1];
                    productionTaskSheet.realStartTime = "";
                    productionTaskSheet.prepareTimePoint = "";
                    productionTaskSheet.realFinishTime = "";
                    productionTaskSheet.status = gVariable.MACHINE_STATUS_DISPATCH_GENERATED;
                    productionTaskSheet.waitForCheck = 0;
                    productionTaskSheet.wastedOutput = 0;
                    productionTaskSheet.workTeam = "";
                    productionTaskSheet.multiProduct = "0";
                    productionTaskSheet.batchNum1 = "";
                    productionTaskSheet.batchNum2 = "";
                    productionTaskSheet.batchNum3 = "";
                    productionTaskSheet.workshop = workshopNameArray[1];
                    productionTaskSheet.workshift = "";
                    productionTaskSheet.salesOrderCode = productBatchImpl.originalSalesOrderCode;
                    productionTaskSheet.salesOrderBatchCode = productBatchImpl.salesOrderBatchCode;
                    productionTaskSheet.BOMCode = productImpl.BOMCode;
                    productionTaskSheet.customer = productBatchImpl.customer;

                    mySQLClass.writeDataToDispatchListTable(dName, gVariable.globalProductTaskTableName, productionTaskSheet);
                    dispatchTools.generatePrintInkList(dName, gVariable.globalMaterialTaskTableName, productionTaskSheet.dispatchCode, productionTaskSheet.machineID, outputNum,
                                                       productBatchNumString, productBatchImpl.salesOrderBatchCode, productImpl);
                }

                //slit process
                productionTaskSheet.machineID = (machineIndexForAllType[MACHINE_TYPE_SLIT] + 1).ToString();
                productionTaskSheet.dispatchCode = productBatchNumString + "F" + (machineIndexForAllType[MACHINE_TYPE_SLIT] - gVariable.slittingProcess[0] + 2);
                productionTaskSheet.planTime1 = GetTimeFromInt(slitStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
                productionTaskSheet.planTime2 = GetTimeFromInt(slitEndTimeStamp).ToString("yyyy-MM-dd HH:mm");
                productionTaskSheet.productCode = productImpl.productCode;
                productionTaskSheet.productName = productImpl.productName;
                productionTaskSheet.operatorName = "未定";
                productionTaskSheet.plannedNumber = outputNum;
                productionTaskSheet.outputNumber = 0;
                productionTaskSheet.qualifiedNumber = 0;
                productionTaskSheet.unqualifiedNumber = 0;
                productionTaskSheet.processName = processNameArray[2];
                productionTaskSheet.realStartTime = "";
                productionTaskSheet.prepareTimePoint = "";
                productionTaskSheet.realFinishTime = "";
                productionTaskSheet.status = gVariable.MACHINE_STATUS_DISPATCH_GENERATED;
                productionTaskSheet.waitForCheck = 0;
                productionTaskSheet.wastedOutput = 0;
                productionTaskSheet.workTeam = "";
                productionTaskSheet.multiProduct = "0";
                productionTaskSheet.batchNum1 = "";
                productionTaskSheet.batchNum2 = "";
                productionTaskSheet.batchNum3 = "";
                productionTaskSheet.workshop = workshopNameArray[2];
                productionTaskSheet.workshift = "";
                productionTaskSheet.salesOrderCode = productBatchImpl.originalSalesOrderCode;
                productionTaskSheet.salesOrderBatchCode = productBatchImpl.salesOrderBatchCode;
                productionTaskSheet.BOMCode = productImpl.BOMCode;
                productionTaskSheet.customer = productBatchImpl.customer;

                mySQLClass.writeDataToDispatchListTable(dName, gVariable.globalProductTaskTableName, productionTaskSheet);
                dispatchTools.generateSupplementaryList(dName, gVariable.globalMaterialTaskTableName, productionTaskSheet.dispatchCode, productionTaskSheet.machineID, outputNum,
                                                        productBatchNumString, productBatchImpl.salesOrderBatchCode, productImpl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("getDispatchFromSalesOrder failed! " + ex);
            }
        }

        
        //
        private int getDispatchFromSalesOrder(int[] outNumForThisDispatch, string productBatchNumString)
        {
            int timeV;
            int monthIndex;
            //int productBatchIndex;
            string timeStr;
            //string productBatchIndexStr;
            int processIndex;
            int timeStampDelta;
            int finshiTimeStamp;
            //int batchNum2Index;
            //int feedMachineIndex;
            int castMachineIndex;
            int printMachineIndex;
            int slitMachineIndex;
            string planTime1;
            string planTime2;
            string workShift;
            string dName;
            string databaseName;

            processIndex = 0;
            timeStampDelta = 0;

            try
            {
                dName = gVariable.globalDatabaseName;

                castMachineIndex = machineIndexForAllType[MACHINE_TYPE_CAST];
                printMachineIndex = machineIndexForAllType[MACHINE_TYPE_PRINT];
                slitMachineIndex = machineIndexForAllType[MACHINE_TYPE_SLIT];

                //current date
                planTime1 = GetTimeFromInt(castStartTimeStamp).ToString("yyyy-MM-dd HH:mm:ss");

                //previous date
                planTime2 = GetTimeFromInt(castStartTimeStamp - 24 * 3600).ToString("yyyy-MM-dd");

                //get month index starting from 2018/01, we have a batch number table for every machine each month, the batch number will be increased by one and the recent value is stored in 
                //table of productBatchCurrentIndex in global database.
                monthIndex = getMonthIndexForDispatchBatch(planTime1);

                timeV = Convert.ToInt32(planTime1.Remove(0, 11).Remove(2));
                if (timeV < 8)
                {
                    workShift = "夜班";
                    timeStampDelta = 3600 * (8 - timeV);
                    //early in the morning means this is the night shift of the previous date
                    timeStr = planTime2.Remove(0, 8) + "2";
                }
                else if (timeV < 20)
                {
                    workShift = "日班";
                    timeStampDelta = 3600 * (20 - timeV);
                    timeStr = planTime1.Remove(0, 8).Remove(2) + "1";
                }
                else if (timeV < 24)
                {
                    workShift = "夜班";
                    timeStampDelta = 3600 * (24 - timeV + 8);
                    timeStr = planTime1.Remove(0, 8).Remove(2) + "2";
                }
                else
                {
                    workShift = "日班";
                    timeStampDelta = 3600 * 12;
                    timeStr = planTime1.Remove(0, 8).Remove(2) + "1";
                }

                gVariable.dispatchSheetStruct dispatchSheet = new gVariable.dispatchSheetStruct();

                if (outNumForThisDispatch[MACHINE_TYPE_CAST] > 0)
                {
                    //processIndex++;
                    //cast process
                    dispatchSheet.machineID = (castMachineIndex + 1).ToString();
                    dispatchSheet.batchNum = productBatchNumString;
                    dispatchSheet.dispatchCode = productBatchNumString + timeStr + "L" + (castMachineIndex - gVariable.castingProcess[0] + 2);
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
                    dispatchSheet.plannedNumber = outNumForThisDispatch[MACHINE_TYPE_CAST];
                    dispatchSheet.outputNumber = 0;
                    dispatchSheet.qualifiedNumber = 0;
                    dispatchSheet.unqualifiedNumber = 0;
                    dispatchSheet.processName = processNameArray[processIndex];
                    dispatchSheet.realStartTime = "";
                    dispatchSheet.prepareTimePoint = "";
                    dispatchSheet.realFinishTime = "";
                    dispatchSheet.status = gVariable.MACHINE_STATUS_DISPATCH_GENERATED;
                    dispatchSheet.waitForCheck = 0;
                    dispatchSheet.wastedOutput = 0;
                    dispatchSheet.workTeam = "";
                    dispatchSheet.batchNum2 = "1";
                    dispatchSheet.batchNum3 = "";
                    dispatchSheet.workshop = workshopNameArray[processIndex];
                    dispatchSheet.workshift = workShift;
                    dispatchSheet.salesOrderCode = productBatchImpl.originalSalesOrderCode;
                    dispatchSheet.salesOrderBatchCode = productBatchImpl.salesOrderBatchCode;
                    dispatchSheet.BOMCode = productImpl.BOMCode;
                    dispatchSheet.customer = productBatchImpl.customer;

                    mySQLClass.writeDataToDispatchListTable(dName, gVariable.globalDispatchTableName, dispatchSheet);
                    dispatchTools.generateMaterialList(dName, gVariable.globalMaterialTableName, dispatchSheet.dispatchCode, dispatchSheet.machineID, outNumForThisDispatch[MACHINE_TYPE_CAST], 
                                                       productBatchNumString, productBatchImpl.salesOrderBatchCode, productImpl);
                    databaseName = gVariable.DBHeadString + dispatchSheet.machineID.ToString().PadLeft(3, '0');
                    mySQLClass.writeDataToWorkingPlanTable(databaseName, gVariable.machineWorkingPlanTableName, dispatchSheet, castStartTimeStamp, timeStampDelta);

                }

                processIndex++;

                if (printMachineIndex != -1 && outNumForThisDispatch[MACHINE_TYPE_PRINT] > 0)
                {
                    //print process
                    dispatchSheet.machineID = (printMachineIndex + 1).ToString();
                    dispatchSheet.dispatchCode = productBatchNumString + timeStr + "Y" + (printMachineIndex - gVariable.printingProcess[0] + 2);
                    //dispatchSheet.dispatchCode = productBatchImpl.salesOrderBatchCode + productBatchIndexStr + "Y" + (printMachineIndex - gVariable.printingProcess[0] + 2) + productBatchIndex.ToString().PadLeft(2, '0');
                    dispatchSheet.planTime1 = GetTimeFromInt(printStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
                    if (printEndTimeStamp > printStartTimeStamp && printEndTimeStamp - printStartTimeStamp < timeStampDelta)
                        finshiTimeStamp = printEndTimeStamp; //this is ythe last print dispatch, we use end time stamp
                    else
                        finshiTimeStamp = printStartTimeStamp + timeStampDelta;  //finish time is start time plus production time needed for output number, normally one shift -> 12 hours
                    dispatchSheet.planTime2 = GetTimeFromInt(finshiTimeStamp).ToString("yyyy-MM-dd HH:mm");
                    dispatchSheet.productCode = productImpl.productCode;
                    dispatchSheet.productName = productImpl.productName;
                    dispatchSheet.operatorName = "未定";
                    dispatchSheet.plannedNumber = outNumForThisDispatch[MACHINE_TYPE_PRINT];
                    dispatchSheet.outputNumber = 0;
                    dispatchSheet.qualifiedNumber = 0;
                    dispatchSheet.unqualifiedNumber = 0;
                    dispatchSheet.processName = processNameArray[processIndex];
                    dispatchSheet.realStartTime = "";
                    dispatchSheet.prepareTimePoint = "";
                    dispatchSheet.realFinishTime = "";
                    dispatchSheet.status = gVariable.MACHINE_STATUS_DISPATCH_GENERATED;
                    dispatchSheet.waitForCheck = 0;
                    dispatchSheet.wastedOutput = 0;
                    dispatchSheet.workTeam = "";
                    dispatchSheet.batchNum2 = "1";
                    dispatchSheet.batchNum3 = "";
                    dispatchSheet.workshop = workshopNameArray[processIndex];
                    dispatchSheet.workshift = workShift;
                    dispatchSheet.salesOrderCode = productBatchImpl.originalSalesOrderCode;
                    dispatchSheet.salesOrderBatchCode = productBatchImpl.salesOrderBatchCode;
                    dispatchSheet.BOMCode = productImpl.BOMCode;
                    dispatchSheet.customer = productBatchImpl.customer;

                    mySQLClass.writeDataToDispatchListTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, dispatchSheet);
                    dispatchTools.generatePrintInkList(dName, gVariable.globalMaterialTableName, dispatchSheet.dispatchCode, dispatchSheet.machineID, outNumForThisDispatch[MACHINE_TYPE_CAST],
                                            productBatchNumString, productBatchImpl.salesOrderBatchCode, productImpl);
                    databaseName = gVariable.DBHeadString + dispatchSheet.machineID.ToString().PadLeft(3, '0');
                    mySQLClass.writeDataToWorkingPlanTable(databaseName, gVariable.machineWorkingPlanTableName, dispatchSheet, printStartTimeStamp, timeStampDelta);
                }
                processIndex++;

                if (outNumForThisDispatch[MACHINE_TYPE_SLIT] > 0)
                {
                    //slit process
                    dispatchSheet.machineID = (slitMachineIndex + 1).ToString();
                    dispatchSheet.dispatchCode = productBatchNumString + timeStr + "F" + (slitMachineIndex - gVariable.slittingProcess[0] + 2);
                    //dispatchSheet.dispatchCode = productBatchImpl.salesOrderBatchCode + productBatchIndexStr + "F" + (slitMachineIndex - gVariable.slittingProcess[0] + 2) + productBatchIndex.ToString().PadLeft(2, '0');
                    dispatchSheet.planTime1 = GetTimeFromInt(slitStartTimeStamp).ToString("yyyy-MM-dd HH:mm");
                    if (slitEndTimeStamp > slitStartTimeStamp && slitEndTimeStamp - slitStartTimeStamp < timeStampDelta)
                        finshiTimeStamp = slitEndTimeStamp; //this is ythe last slit dispatch, we use end time stamp
                    else
                        finshiTimeStamp = slitStartTimeStamp + timeStampDelta;  //finish time is start time plus production time needed for output number, normally one shift -> 12 hours

                    dispatchSheet.planTime2 = GetTimeFromInt(finshiTimeStamp).ToString("yyyy-MM-dd HH:mm");
                    dispatchSheet.productCode = productImpl.productCode;
                    dispatchSheet.productName = productImpl.productName;
                    dispatchSheet.operatorName = "未定";
                    dispatchSheet.plannedNumber = outNumForThisDispatch[MACHINE_TYPE_SLIT];
                    dispatchSheet.outputNumber = 0;
                    dispatchSheet.qualifiedNumber = 0;
                    dispatchSheet.unqualifiedNumber = 0;
                    dispatchSheet.processName = processNameArray[processIndex];
                    dispatchSheet.realStartTime = "";
                    dispatchSheet.prepareTimePoint = "";
                    dispatchSheet.realFinishTime = "";
                    dispatchSheet.status = gVariable.MACHINE_STATUS_DISPATCH_GENERATED;
                    dispatchSheet.waitForCheck = 0;
                    dispatchSheet.wastedOutput = 0;
                    dispatchSheet.workTeam = "";
                    dispatchSheet.batchNum2 = "1";
                    dispatchSheet.batchNum3 = "";
                    dispatchSheet.workshop = workshopNameArray[processIndex];
                    dispatchSheet.workshift = workShift;
                    dispatchSheet.salesOrderCode = productBatchImpl.originalSalesOrderCode;
                    dispatchSheet.salesOrderBatchCode = productBatchImpl.salesOrderBatchCode;
                    dispatchSheet.BOMCode = productImpl.BOMCode;
                    dispatchSheet.customer = productBatchImpl.customer;

                    mySQLClass.writeDataToDispatchListTable(dName, gVariable.globalDispatchTableName, dispatchSheet);
                    dispatchTools.generateSupplementaryList(dName, gVariable.globalMaterialTableName, dispatchSheet.dispatchCode, dispatchSheet.machineID, outNumForThisDispatch[MACHINE_TYPE_CAST],
                                                            productBatchNumString, productBatchImpl.salesOrderBatchCode, productImpl);
                    databaseName = gVariable.DBHeadString + dispatchSheet.machineID.ToString().PadLeft(3, '0');
                    mySQLClass.writeDataToWorkingPlanTable(databaseName, gVariable.machineWorkingPlanTableName, dispatchSheet, slitStartTimeStamp, timeStampDelta);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("getDispatchFromSalesOrder failed! " + ex);
            }

            return timeStampDelta;
        }

        /*
        void generateMaterialList(string databaseName, string tableName, string dispatchCode, string machineID, int outputNum, string BOMCode, string batchNum)
        {
            int i;
            //string str;
            //string num;
            string commandText;
            //string[,] tableArray;
            gVariable.BOMListStruct BOMImpl = new gVariable.BOMListStruct();
            gVariable.materialListStruct materialSheet = new gVariable.materialListStruct();

            try
            {
                BOMImpl.materialName = new string[gVariable.maxMaterialTypeNum];
                BOMImpl.materialCode = new string[gVariable.maxMaterialTypeNum];
                BOMImpl.materialQuantity = new double[gVariable.maxMaterialTypeNum];

                materialSheet.materialCode = new string[gVariable.maxMaterialTypeNum];
                materialSheet.materialRequired = new int[gVariable.maxMaterialTypeNum];

                commandText = "select * from `" + gVariable.bomTableName + "` where BOMCode = '" + productImpl.BOMCode + "'";
                mySQLClass.readBOMInfo(ref BOMImpl, commandText);

                materialSheet.salesOrderBatchCode = productBatchImpl.salesOrderBatchCode;
                materialSheet.salesOrderCode = productBatchImpl.originalSalesOrderCode;
                materialSheet.dispatchCode = dispatchCode;  //
                materialSheet.machineID = machineID;
                materialSheet.machineCode = "00"; // gVariable.machineCodeArrayZihua[Convert.ToInt32(machineID) - 1];
                materialSheet.machineName = gVariable.machineNameArrayAPS[Convert.ToInt32(machineID) - 1];
                //materialSheet.status = "0";
                materialSheet.numberOfTypes = BOMImpl.numberOfTypes;
                materialSheet.batchNum = batchNum;

                for (i = 0; i < materialSheet.numberOfTypes; i++)
                {
                    materialSheet.materialCode[i] = BOMImpl.materialCode[i];
                    materialSheet.materialRequired[i] = (int)(BOMImpl.materialQuantity[i] * outputNum);  //material requirement by kilogram
                }

                mySQLClass.writeDataToMaterialListTable(databaseName, tableName, materialSheet);
            }
            catch (Exception ex)
            {
                Console.WriteLine("generateMaterialList failed! " + ex);
            }
        }

        void generatePrintInkList(string databaseName, string tableName, string dispatchCode, string machineID, int outputNum, string batchNum)
        {
            int i;
            double quantity;
            //string str;
            //string num;
            string commandText;
            string[,] tableArray;
            gVariable.materialListStruct materialSheet = new gVariable.materialListStruct();

            try
            {
                materialSheet.materialCode = new string[gVariable.maxMaterialTypeNum];
                materialSheet.materialRequired = new int[gVariable.maxMaterialTypeNum];

                commandText = "select typeNum, oil1, num1, oil2, num2, oil3, num3, oil4, num4, oil5, num5, oil6, num6, oil7, num7 from `" + gVariable.inkBomTableName + "` where BOMName = '" + productImpl.inkRatio + "'";
                tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (tableArray == null)
                    return;

                materialSheet.salesOrderBatchCode = productBatchImpl.salesOrderBatchCode;
                materialSheet.salesOrderCode = productBatchImpl.originalSalesOrderCode;
                materialSheet.dispatchCode = dispatchCode;  //
                materialSheet.machineID = machineID;
                materialSheet.machineCode = "00"; // gVariable.machineCodeArrayZihua[Convert.ToInt32(machineID) - 1];
                materialSheet.machineName = gVariable.machineNameArrayAPS[Convert.ToInt32(machineID) - 1];
                //materialSheet.status = "0";
                materialSheet.numberOfTypes = Convert.ToInt32(tableArray[0, 0]);
                materialSheet.batchNum = batchNum;

                for (i = 0; i < materialSheet.numberOfTypes; i++)
                {
                    quantity = Convert.ToDouble(tableArray[0, 2 + i * 2]);
                    materialSheet.materialCode[i] = tableArray[0, 1 + i * 2];
                    materialSheet.materialRequired[i] = (int)(quantity * outputNum / 1000);  //quantity is weight of kg for oil by 1000 kg of product
                }

                mySQLClass.writeDataToMaterialListTable(databaseName, tableName, materialSheet);
            }
            catch (Exception ex)
            {
                Console.WriteLine("generatePrintInkList failed! " + ex);
            }
        }

        void generateSupplementaryList(string databaseName, string tableName, string dispatchCode, string machineID, int outputNum, string batchNum)
        {
            int i;
            //string str;
            //string num;
            string commandText;
            string[,] tableArray;
            gVariable.materialListStruct materialSheet = new gVariable.materialListStruct();

            try
            {
                materialSheet.materialCode = new string[gVariable.maxMaterialTypeNum];
                materialSheet.materialRequired = new int[gVariable.maxMaterialTypeNum];

                commandText = "select stackNumOneTon, paperCoreOneTon, paperBoardOneton, craftPaperOneTon, corrugatedPaperPerTon, wrappingFilmOneTon, packingFilmOneTon from `" + gVariable.productTableName + "` where productCode = '" + productImpl.productCode + "'";
                tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (tableArray == null)
                    return;

                materialSheet.salesOrderBatchCode = productBatchImpl.salesOrderBatchCode;
                materialSheet.salesOrderCode = productBatchImpl.originalSalesOrderCode;
                materialSheet.dispatchCode = dispatchCode;  //
                materialSheet.machineID = machineID;
                materialSheet.machineCode = "00"; // gVariable.machineCodeArrayZihua[Convert.ToInt32(machineID) - 1];
                materialSheet.machineName = gVariable.machineNameArrayAPS[Convert.ToInt32(machineID) - 1];
                //materialSheet.status = "0";
                materialSheet.numberOfTypes = 7;  //
                materialSheet.batchNum = batchNum;

                for (i = 0; i < materialSheet.numberOfTypes; i++)
                {
                    materialSheet.materialCode[i] = supplementaryMaterial[i];

                    if (tableArray[0, i] == null || tableArray[0, i] == "")
                        materialSheet.materialRequired[i] = 0;
                    else
                        materialSheet.materialRequired[i] = (int)(Convert.ToDouble(tableArray[0, i]) * outputNum / 1000) + 1;  //quantity is weight of kg for oil by 1000 kg of product
                }

                mySQLClass.writeDataToMaterialListTable(databaseName, tableName, materialSheet);
            }
            catch (Exception ex)
            {
                Console.WriteLine("generateSupplementaryList failed! " + ex);
            }
        }
        */
        
        private void checkSecondCondition()
        {
            try
            {
                if (productBatchImpl.productCode.Substring(0, 3) == BREATHABLE_FILM)  //
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
                Console.WriteLine("checkSecondCondition failed! " + ex);
            }
        }        
    }
}
