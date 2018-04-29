using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using MESSystem.common;

namespace MESSystem.quality
{
    public partial class SPCFunctions
    {
        const int ONE_SIDE_POINT_LIMIT = 9;  //FOR ALARM_CATEGORY_DATA_SAME_SIDE, 9 points at same side of the chart
        const int ONE_TREND_POINT_LIMIT = 7;   //ALARM_CATEGORY_DATA_ONE_TREND, 7 points lies in the same trend
        const int VALUE_SMALL_POINT_LIMIT = 15;   //ALARM_CATEGORY_DATA_SMALL_CHANGE, 15 points have very close values
        const int VALUE_LOCATE_FAR_APART = 4;   //ALARM_CATEGORY_DATA_LOCATE_APART, 4 out of 5 points have large values near limit

        const int TYPE_XBAR_CHART = 0;
        const int TYPE_S_CHART = 1;
        const int TYPE_ORI_DATA_CHART = 2;
        const int TYPE_C_CHART = 3;

        //we get below 2 values by get_XBar_S_Chart()
//        public float totalAverageForChecking;
//        public float deltaAverageForChecking;
        public float[] oneGroupAverage = new float[gVariable.numOfGroupsInSChart];
        public float[] standardGroupDelta = new float[gVariable.numOfGroupsInSChart];

        //spec limits for this data item in quality data
        public float[] specLowerLimitArray = new float[gVariable.maxQualityDataNum];
        public float[] specUpperLimitArray = new float[gVariable.maxQualityDataNum];

        //we get below arrays by readSmallPartOfDataToArray()
        //SPC control limits/center value for XBar and S chart
        public float[,] controlCenterValueArray = new float[gVariable.maxQualityDataNum, 6]; //high1, center1, low1, high2, center2, low2
        public float[,] dataInPoint = new float[gVariable.maxCurveNum, gVariable.totalPointNumForSChart];   //data buffer for 36 curves, quality data item num should be less than 16, so this buffer can stand for both
        public int[,] timeInPoint = new int[gVariable.maxCurveNum, gVariable.totalPointNumForSChart];   //time buffer for 36 curves
        public int[,] statusInPoint = new int[gVariable.maxCurveNum, gVariable.totalPointNumForSChart];   //status buffer for 36 curves

        public string[] qualityItemName = new string[gVariable.maxQualityDataNum];
        public int[] qualityChartType = new int[gVariable.maxQualityDataNum];

        public string databaseName;
        public int alarmIDInFactoryAlarmList;
        public string dispatchCodeAlarm;
        public int alarmIDInTable;
//        System.Windows.Forms.Timer aTimer;

        gVariable.alarmTableStruct alarmTableStructImpl;

        System.Timers.Timer aTimer;

        //if gVariable.checkDataCorrectness is enabled, then whenever a new connection is available, we will start his function at once
        public void checkForSPCRules()
        {
            //we should not check for data correctness if this variable is set to 0
            if (gVariable.checkDataCorrectness == 0)
                return;

            //get database name from board index
            databaseName = gVariable.DBHeadString + (boardIndex + 1).ToString().PadLeft(3, '0');

            aTimer = new System.Timers.Timer();
            aTimer.Interval = gVariable.SPC_CHECKING_SPAN;
            aTimer.Enabled = true;
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(timer_checkSPCDetails);   
        }


        public void timer_checkSPCDetails(Object source, EventArgs e)
        {
            int i, j;
            int nextRecordIndex;
            int controllable;
            int returnedCategory;
            int firstDataPointID;
            int qualityDataItemNum;  //the number of effective column number for quality data
            string cmdText;
            MySqlParameter[] param;
            string[] recordArray = new string[50];

            try
            {
                //we should not check for data correctness if this variable is set to 0
                if (gVariable.checkDataCorrectness == 0)
                    return;
             
                if(aTimer.Enabled == false)
                {
//                    Console.WriteLine(DateTime.Now.ToString() + "returned");
                    return;
                }

                if (gVariable.SPCTriggeredAlarmArray[boardIndex] > 0)
                    return;  //there are still SPC checking alarm, wait for all SPC alarms are processed then start next checking again 

                aTimer.Enabled = false;
//                Console.WriteLine(DateTime.Now.ToString() + "00000");
                controllable = 1;

                //first get dispatch code
                cmdText = "select * from `" + gVariable.dispatchListTableName + "` where status = '" + gVariable.MACHINE_STATUS_DISPATCH_APPLIED + "' order by id desc";
                dispatchCodeAlarm = mySQLClass.getColumnInfoByCommandText(databaseName, gVariable.dispatchListTableName, cmdText, mySQLClass.DISPATCH_CODE_IN_DISPATCHLIST_DATABASE);  // get newest dispatch
                if (dispatchCodeAlarm == null)  //dispatch generation not ready yet
                {
                    aTimer.Enabled = true;
                    return;
                }

                qualityDataItemNum = toolClass.getCraftQualityData(databaseName, gVariable.qualityListTableName, dispatchCodeAlarm, specUpperLimitArray, specLowerLimitArray);

                //last parameter of -1 means we want recent data in this table, so didnot provide a starting id
                //return: the ID of the first data point we read in the following function
                firstDataPointID = mySQLClass.readSmallPartOfDataForSPC(databaseName, dispatchCodeAlarm + "_quality", qualityDataItemNum, gVariable.totalPointNumForSChart, dataInPoint, timeInPoint, statusInPoint, -1);
                if (firstDataPointID < 0)  //illegal ID, which means there are not enough quality data point for this table to check SPC function
                {
                    aTimer.Enabled = true;
                    return;
                }

                //not in controllable mode, calculate PPK to see if we entered controllable mode
                if (gVariable.dataForThisBoardIsUnderSPCControl[boardIndex] == gVariable.SPC_DATA_UNCONTROLLABLE)
                {
                    nextRecordIndex = 0;
                    for (i = 0; i < qualityDataItemNum; i++)
                    {
                        //get next quality item in 0_qualityList table, and also put content into recordArray. We need to go through all the quality items for this dispatch
                        nextRecordIndex = mySQLClass.getNextRecordByOneStrColumn(databaseName, gVariable.qualityListTableName, "dispatchCode", dispatchCodeAlarm, nextRecordIndex, recordArray);

                        qualityItemName[i] = recordArray[mySQLClass.QUALITY_LIST_ID_ITEM_NAME - 1]; //this is quality item name 
                        qualityChartType[i] = Convert.ToInt16(recordArray[mySQLClass.QUALITY_LIST_ID_CHART_TYPE].Trim()); //this is quality chart type, could be CHART_TYPE_NO_SPC/0, CHART_TYPE_SPC_C/1, CHART_TYPE_SPC_XBAR_S/2

                        CalculateSPC(specUpperLimitArray[i], specLowerLimitArray[i], i, dataInPoint, qualityChartType[i]);
                        if ( getSPCCpk() < gVariable.SPC_CPK_CONTROLLABLE_LIMIT)
                            controllable = 0;  //still not in controllable mode
                        else
                        {
                            if (qualityChartType[i] == gVariable.CHART_TYPE_SPC_XBAR_S)
                            {
                                //already in controllable mode, get UCL/SCL data and record them in database, so for this dispatch, we use this data as control limits
                                get_XBar_S_Chart(i, null, gVariable.SPC_DATA_GET_UCL_SCL, dataInPoint, timeInPoint, controlCenterValueArray);

                                //save control limits to global parameter, then to database
                                gVariable.qualityList[boardIndex].controlUpperLimit1[i] = controlCenterValueArray[i, 0];
                                gVariable.qualityList[boardIndex].controlCenterValue1[i] = controlCenterValueArray[i, 1];
                                gVariable.qualityList[boardIndex].controlLowerLimit1[i] = controlCenterValueArray[i, 2];
                                gVariable.qualityList[boardIndex].controlUpperLimit2[i] = controlCenterValueArray[i, 3];
                                gVariable.qualityList[boardIndex].controlCenterValue2[i] = controlCenterValueArray[i, 4];
                                gVariable.qualityList[boardIndex].controlLowerLimit2[i] = controlCenterValueArray[i, 5];

                                param = new MySqlParameter[] { new MySqlParameter("@controlCenterValue1", controlCenterValueArray[i, 1]), new MySqlParameter("@controlCenterValue2", controlCenterValueArray[i, 4]), 
                                                           new MySqlParameter("@controlLowerLimit1", controlCenterValueArray[i, 2]), new MySqlParameter("@controlLowerLimit2", controlCenterValueArray[i, 5]),
                                                           new MySqlParameter("@controlUpperLimit1", controlCenterValueArray[i, 0]), new MySqlParameter("@controlUpperLimit2", controlCenterValueArray[i, 3]) };

                                cmdText = "update `" + gVariable.qualityListTableName + "` set controlCenterValue1 = @controlCenterValue1, controlCenterValue2 = @controlCenterValue2, controlLowerLimit1 = @controlLowerLimit1, " +
                                   "controlLowerLimit2 = @controlLowerLimit2, controlUpperLimit1 = @controlUpperLimit1, controlUpperLimit2 = @controlUpperLimit2 where id = '" + (nextRecordIndex + i) + "'";

                                //update 0_qualityList by dispatchCode, if a dispatch has 4 quality data, we need to update controlLimuts and centerValue for all these 4 quality items
                                mySQLClass.databaseNonQueryAction(databaseName, cmdText, param, gVariable.notAppendRecord);
                            //    Console.WriteLine(cmdText);
                            }
                            else if (qualityChartType[i] == gVariable.CHART_TYPE_SPC_C)
                            {
                                //already in controllable mode, get UCL/SCL data and record them in database, so for this dispatch, we use this data as control limits
                                get_C_Chart(i, null, gVariable.SPC_DATA_GET_UCL_SCL, dataInPoint, timeInPoint, controlCenterValueArray);

                                param = new MySqlParameter[] { new MySqlParameter("@controlCenterValue1", controlCenterValueArray[i, 1]), new MySqlParameter("@controlLowerLimit1", controlCenterValueArray[i, 2]), 
                                                           new MySqlParameter("@controlUpperLimit1", controlCenterValueArray[i, 0]) };

                                cmdText = "update `" + gVariable.qualityListTableName + "` set controlCenterValue1 = @controlCenterValue1, controlLowerLimit1 = @controlLowerLimit1, " +
                                          "controlUpperLimit1 = @controlUpperLimit1 where id = '" + (nextRecordIndex + i) + "'";

                                //update 0_qualityList by dispatchCode, if a dispatch has 4 quality data, we need to update controlLimuts and centerValue for all these 4 quality items
                                mySQLClass.databaseNonQueryAction(databaseName, cmdText, param, gVariable.notAppendRecord);
                            }
                        }
                    }

                    if (controllable == 1)  //no item in quality table exeeds controllable limit( CPK > 1.33), we believe this dispatch is in controllable status
                    {
                        gVariable.dataForThisBoardIsUnderSPCControl[boardIndex] = gVariable.SPC_DATA_CONTROLLABLE;
                    }
                }
                else if (gVariable.dataForThisBoardIsUnderSPCControl[boardIndex] == gVariable.SPC_DATA_CONTROLLABLE && gVariable.newQualityDataArrivedFlag[boardIndex] == 1)
                {  
                    //it is now in controllable mode and new quality data have come, start SPC checking
                    gVariable.newQualityDataArrivedFlag[boardIndex] = 0;

                    //get SPC data in oneGroupAverage(for xbar-chart) and standardGroupDelta(for s-chart) array
                    for (i = 0; i < qualityDataItemNum; i++)
                    {
                        if (qualityChartType[i] == gVariable.CHART_TYPE_SPC_XBAR_S)
                        {
                            //i means the index in dataInPoint[,] and timeinPoint[,] 
                            get_XBar_S_Chart(i, null, gVariable.SPC_DATA_ONLY, dataInPoint, timeInPoint, controlCenterValueArray);

                            //for xbar chart data checking,  
                            returnedCategory = checkSPCChartError(i, firstDataPointID, controlCenterValueArray[i, 1], controlCenterValueArray[i, 0], controlCenterValueArray[i, 2], oneGroupAverage);

                            /*
                            if (returnedCategory != gVariable.ALARM_CATEGORY_QUALITY_DATA_OVERFLOW)  //it's for test of alarm trigger function
                                                    {
                                                        triggerAnAlarm(TYPE_ORI_DATA_CHART, returnedCategory, i, firstDataPointID);
                                                        Console.WriteLine(DateTime.Now.ToString() + "11111a");
                                                        aTimer.Enabled = true;
                                                        return;
                                                    }
                            */

                            if (returnedCategory != gVariable.ALARM_CATEGORY_NORMAL)
                            {
                                triggerAnAlarm(TYPE_XBAR_CHART, returnedCategory, i, firstDataPointID);
//                                Console.WriteLine(DateTime.Now.ToString() + "11111b");
                                aTimer.Enabled = true;
                                return;
                            }

                            //for S chart data checking
//                            controlCenterValueArray[i, 3] = 19; //temp
                            returnedCategory = checkSPCChartError(i, firstDataPointID, controlCenterValueArray[i, 4], controlCenterValueArray[i, 3], controlCenterValueArray[i, 5], standardGroupDelta);
                            if (returnedCategory != gVariable.ALARM_CATEGORY_NORMAL)
                            {
                                triggerAnAlarm(TYPE_S_CHART, returnedCategory, i, firstDataPointID);
//                                Console.WriteLine(DateTime.Now.ToString() + "11111c");
                                aTimer.Enabled = true;
                                return;
                            }
                        }
                        else if (qualityChartType[i] == gVariable.CHART_TYPE_SPC_C)
                        {
                            //i means the index in dataInPoint[,] and timeinPoint[,] 
                            get_C_Chart(i, null, gVariable.SPC_DATA_ONLY, dataInPoint, timeInPoint, controlCenterValueArray);

                            for (j = 0; j < totalPointNumForChart; j++)
                            {
                                oneGroupAverage[j] = dataInPoint[i, j];
                            }
                            //for C chart data checking 
                            returnedCategory = checkSPCChartError(i, firstDataPointID, controlCenterValueArray[i, 1], controlCenterValueArray[i, 0], controlCenterValueArray[i, 2], oneGroupAverage);
                            /*                        if (returnedCategory != gVariable.ALARM_CATEGORY_QUALITY_DATA_OVERFLOW)  //it's for test of alarm trigger function
                                                    {
                                                        triggerAnAlarm(TYPE_ORI_DATA_CHART, returnedCategory, i, firstDataPointID);
                                                        Console.WriteLine(DateTime.Now.ToString() + "11111a");
                                                        aTimer.Enabled = true;
                                                        return;
                                                    }
                            if (returnedCategory != gVariable.ALARM_CATEGORY_NORMAL)
                            {
                                triggerAnAlarm(TYPE_C_CHART, returnedCategory, i, firstDataPointID);
//                                Console.WriteLine(DateTime.Now.ToString() + "11111b");
                                aTimer.Enabled = true;
                                return;
                            }
                             */
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("spc data checking failed fail! " + ex);
            }
//            Console.WriteLine(DateTime.Now.ToString() + "11111c");
            aTimer.Enabled = true;
        }

        //chartType means xbar or s chart, 
        //category means failed by which rule defined in SPC checking
        //whichQulityItem, there are many items for quality data, which of the item triggered the alarm
        //startingIDInTable: there are maybe thansands of data for this quality data item, from which index we found SPC error and want to trigger an alarm  
        void triggerAnAlarm(int chartType, int alarmCategory, int whichQulityItem, int startingIDInTable)
        {
            try
            {
                if (chartType == TYPE_XBAR_CHART)
                {
                    alarmTableStructImpl.errorDesc = "在平均值 (XBAR) 图中" + gConstText.errSPCDescList[alarmCategory - gVariable.ALARM_CATEGORY_SPC_DATA_START];
                    alarmTableStructImpl.type = gVariable.ALARM_TYPE_QUALITY_DATA;
                }
                else if (chartType == TYPE_S_CHART)
                {
                    alarmTableStructImpl.errorDesc = "在方差 (s) 图中" + gConstText.errSPCDescList[alarmCategory - gVariable.ALARM_CATEGORY_SPC_DATA_START];
                    alarmTableStructImpl.type = gVariable.ALARM_TYPE_QUALITY_DATA;
                }
                else if (chartType == TYPE_ORI_DATA_CHART)
                {
                    alarmTableStructImpl.errorDesc = "数据超出规格限";
                    alarmTableStructImpl.type = gVariable.ALARM_TYPE_QUALITY_DATA;
                }
                else if (chartType == TYPE_C_CHART)
                {
                    alarmTableStructImpl.errorDesc = "在样本缺陷数 (C) 图中" + gConstText.errSPCDescList[alarmCategory - gVariable.ALARM_CATEGORY_SPC_DATA_START];
                    alarmTableStructImpl.type = gVariable.ALARM_TYPE_QUALITY_DATA;
                }

                alarmTableStructImpl.alarmFailureCode = DateTime.Now.ToString("yyMMddHHmmss") + "_" + (gVariable.andonAlarmIndex + 1);
                alarmTableStructImpl.dispatchCode = gVariable.dispatchSheet[boardIndex].dispatchCode;
                alarmTableStructImpl.machineCode = gVariable.machineCodeArray[boardIndex];
                alarmTableStructImpl.machineName = gVariable.machineNameArray[boardIndex];
                alarmTableStructImpl.operatorName = gVariable.SPCMonitoringSystem;
                alarmTableStructImpl.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                alarmTableStructImpl.category = alarmCategory;
                alarmTableStructImpl.status = gVariable.ALARM_STATUS_APPLIED;
                alarmTableStructImpl.startID = startingIDInTable;
                alarmTableStructImpl.indexInTable = whichQulityItem;
                alarmTableStructImpl.workshop = gVariable.allMachineWorkshopForZihua[0];
                alarmTableStructImpl.mailList = toolClass.getAlarmMailList();
                gVariable.andonAlarmIndex++;

                //return value: ID of the new generated alarm in alarm list table for this machine
                alarmIDInTable = mySQLClass.writeAlarmTable(databaseName, gVariable.alarmListTableName, alarmTableStructImpl);

//                if(chartType == TYPE_ORI_DATA_CHART)
                  gVariable.SPCTriggeredAlarmArray[boardIndex] = alarmIDInTable;
//                else
//                    gVariable.SPCTriggeredAlarmArray[alarmCategory - gVariable.ALARM_CATEGORY_SPC_DATA_START, boardIndex] = alarmIDInTable;
                toolClass.processNewAlarm(databaseName, alarmIDInTable);
            }
            catch (Exception ex)
            {
                Console.Write("spc data trigger Alarm failed! " + ex);
            }
        }

        //We defined 5 kinds of SPC errors below in gVariable.cs, we will check if these defined errors occurred in our chart one by one
        //
        //ALARM_CATEGORY_SPC_DATA_OVERFLOW = 3;   //SPC data out of control limit ucl/lcl
        //ALARM_CATEGORY_SPC_DATA_SAME_SIDE = 4;   //9 points at same side of the chart
        //ALARM_CATEGORY_SPC_DATA_ONE_TREND = 5;   //7 points lies in the same trend
        //ALARM_CATEGORY_SPC_DATA_SMALL_CHANGE = 6;   //15 points lies within 1/3 of LCL and UCL 
        //ALARM_CATEGORY_SPC_DATA_LOCATE_APART = 7;   //4 out of 5 ponits lies at 2/3 of LCL and UCL 
        //
        //dataIndex means which data item in qualityData list or craft data list we are working on, 0 may mean "厚重" or "压力"
        //firstDataPointID means the index of the first data in data table we are working on, could be 13000, if there are 13270 pieces of data in table
        public int checkSPCChartError(int dataIndex, int firstDataPointID, float totalAverage, float vUpperLimit, float vLowerLimit, float [] onePieceData)
        {
            int i;
            int ret;
            int flagOfThree;
            int direction;
            int currentErrorIndex;
            float previous;
            float smallDeltaPlus, smallDeltaMinus;

            currentErrorIndex = 0;
            ret = gVariable.ALARM_CATEGORY_NORMAL;

            try
            {
                // ALARM_CATEGORY_SPC_DATA_OVERFLOW = 3;   //data out of limit
                for (i = 0; i < numOfGroupsInChart; i++)
                {
                    if (onePieceData[i] > vUpperLimit || onePieceData[i] < vLowerLimit)
                    {
                        ret = gVariable.ALARM_CATEGORY_SPC_DATA_OVERFLOW;
                        setQualityDataStatus(dataIndex, firstDataPointID, i, gVariable.ALARM_CATEGORY_SPC_DATA_OVERFLOW, 1, pointNumInGroup);
                    }
                }

                if (ret == gVariable.ALARM_CATEGORY_SPC_DATA_OVERFLOW)
                    return ret;

                //definition of direction: larger than 100 means onePieceData[i] is smaller than totalAverage, 104 means continuous 4 point smaller than totalAverage
                //6 means continuous 6 point larger than totalAverage
                direction = 0;

                currentErrorIndex = gVariable.ALARM_CATEGORY_SPC_DATA_SAME_SIDE; // value is 4, whether 9 points at same side of the chart
                for (i = 0; i < numOfGroupsInChart; i++)
                {
                    if (onePieceData[i] >= totalAverage)
                    {
                        if (direction > 100)
                            direction = 1;
                        else
                            direction++;

                        if (direction >= ONE_SIDE_POINT_LIMIT)
                        {
                            setQualityDataStatus(dataIndex, firstDataPointID, i - ONE_SIDE_POINT_LIMIT + 1, gVariable.ALARM_CATEGORY_SPC_DATA_SAME_SIDE, ONE_SIDE_POINT_LIMIT, pointNumInGroup);
                            return gVariable.ALARM_CATEGORY_SPC_DATA_SAME_SIDE;
                        }
                    }
                    else
                    {
                        if (direction < 100)
                            direction = 101;
                        else
                            direction++;

                        if (direction >= ONE_SIDE_POINT_LIMIT + 100)
                        {
                            setQualityDataStatus(dataIndex, firstDataPointID, i - ONE_SIDE_POINT_LIMIT + 1, gVariable.ALARM_CATEGORY_SPC_DATA_SAME_SIDE, ONE_SIDE_POINT_LIMIT, pointNumInGroup);
                            return gVariable.ALARM_CATEGORY_SPC_DATA_SAME_SIDE;
                        }
                    }
                }

                direction = 0;  //larger than 100 means onePieceData[i] is decreasing
                previous = onePieceData[0];
                currentErrorIndex = gVariable.ALARM_CATEGORY_SPC_DATA_ONE_TREND; //value is 5, whether 7 points lies in the same trend
                for (i = 0; i < numOfGroupsInChart; i++)
                {
                    if (onePieceData[i] > previous)
                    {
                        if (direction > 100)
                            direction = 1;
                        else
                            direction++;

                        if (direction >= ONE_TREND_POINT_LIMIT)
                        {
                            setQualityDataStatus(dataIndex, firstDataPointID, i - ONE_TREND_POINT_LIMIT + 1, gVariable.ALARM_CATEGORY_SPC_DATA_ONE_TREND, ONE_TREND_POINT_LIMIT, pointNumInGroup);
                            return gVariable.ALARM_CATEGORY_SPC_DATA_ONE_TREND;
                        }
                    }
                    else if (onePieceData[i] < previous)
                    {
                        if (direction < 100)
                            direction = 101;
                        else
                            direction++;

                        if (direction >= ONE_TREND_POINT_LIMIT + 100)
                        {
                            setQualityDataStatus(dataIndex, firstDataPointID, i - ONE_TREND_POINT_LIMIT + 1, gVariable.ALARM_CATEGORY_SPC_DATA_ONE_TREND, ONE_TREND_POINT_LIMIT, pointNumInGroup);
                            return gVariable.ALARM_CATEGORY_SPC_DATA_ONE_TREND;
                        }
                    }
                    previous = onePieceData[i];
                }

                direction = 0;
                smallDeltaPlus = (vUpperLimit - totalAverage) / 3;
                smallDeltaMinus = (totalAverage - vLowerLimit) / 3;
                currentErrorIndex = gVariable.ALARM_CATEGORY_SPC_DATA_SMALL_CHANGE; // value is 6, whether 15 points lies within 1/3 of LCL and UCL 
                for (i = 0; i < numOfGroupsInChart; i++)
                {
                    if (onePieceData[i] <= totalAverage + smallDeltaPlus && onePieceData[i] >= totalAverage - smallDeltaMinus)
                    {
                        direction++;

                        if (direction >= VALUE_SMALL_POINT_LIMIT)
                        {
                            setQualityDataStatus(dataIndex, firstDataPointID, i - VALUE_SMALL_POINT_LIMIT + 1, gVariable.ALARM_CATEGORY_SPC_DATA_SMALL_CHANGE, VALUE_SMALL_POINT_LIMIT, pointNumInGroup);
                            return gVariable.ALARM_CATEGORY_SPC_DATA_SMALL_CHANGE;
                        }
                    }
                    else
                    {
                        direction = 0;
                    }
                }

                direction = 0;
                flagOfThree = 1;
                currentErrorIndex = gVariable.ALARM_CATEGORY_SPC_DATA_LOCATE_APART; //value is 7, whether 4 out of 5 ponits lies at 2/3 of LCL and UCL 
                for (i = 0; i < numOfGroupsInChart; i++)
                {
                    if (onePieceData[i] > totalAverage + smallDeltaPlus || onePieceData[i] < totalAverage - smallDeltaMinus)
                    {
                        direction++;

                        if (direction > VALUE_LOCATE_FAR_APART - 1)
                            flagOfThree = 1;

                        if (direction >= VALUE_LOCATE_FAR_APART)
                        {
                            setQualityDataStatus(dataIndex, firstDataPointID, i - VALUE_LOCATE_FAR_APART + 1, gVariable.ALARM_CATEGORY_SPC_DATA_LOCATE_APART, VALUE_LOCATE_FAR_APART, pointNumInGroup);
                            return gVariable.ALARM_CATEGORY_SPC_DATA_LOCATE_APART;
                        }
                    }
                    else
                    {
                        if (flagOfThree == 1)
                        {
                            if (i >= numOfGroupsInChart)  //this is the last point, so this rule is not broken
                                break;

                            flagOfThree = 2;
                        }
                        else if (flagOfThree == 2)
                        {
                            flagOfThree = 0;
                        }
                        direction = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("checkSPCChartError failed, currentErrorIndex under cheching is " + currentErrorIndex + "!" + ex);
            }

            return gVariable.ALARM_CATEGORY_NORMAL;
        }

        //update quality data status to error cause, like this data is ALARM_CATEGORY_SPC_DATA_SAME_SIDE
        //firstDataPointID : data index for the start of alarm
        //groupIndex : error data starts from which group
        private void setQualityDataStatus(int dataIndex, int firstDataPointID, int groupIndex, int status, int numOfGroup, int numOfPoint)
        {
            int i, j;
            string str;
            string cmdText;
            MySqlParameter[] param;

            try
            {
                firstDataPointID--; //from ID to index
                str = "status" + (dataIndex + 1);
                param = new MySqlParameter[] { new MySqlParameter("@" + str, status) };
                for (i = groupIndex; i < numOfGroup + groupIndex; i++)
                {
                    for (j = 0; j < numOfPoint; j++)
                    {
                        cmdText = "update `" + dispatchCodeAlarm + "_quality` set " + str + " = @" + str + " where id = '" + (firstDataPointID + i * numOfPoint + j + 1) + "'";
                        //update 0_qualityList by dispatchCode, if a dispatch has 4 quality data, we need to update controlLimuts and centerValue for all these 4 quality items
                        mySQLClass.databaseNonQueryAction(databaseName, cmdText, param, gVariable.notAppendRecord);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("setQualityDataStatus failed!" + ex);
            }
        }

        private void setQualityDataStatusInner(int dataIndex, int firstDataPointID, int groupIndex, int status)
        {
            int j;

            for (j = 0; j < gVariable.pointNumInSChartGroup; j++)
            {

            }
        }

        public int isProcessUnderControl()
        {

            return gVariable.SPC_DATA_CONTROLLABLE;
        }
    }
}
