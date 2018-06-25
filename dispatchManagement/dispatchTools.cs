using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MESSystem.common;
using MESSystem.mainUI;
using MESSystem.communication;

namespace MESSystem.dispatchManagement
{
    public class dispatchTools
    {

        //when copy MES parameters from one board to system UI, we need a deep clone function to copy all the contents into new parameters
        public static void cloneMESParameters(int myBoardIndex)
        {
//            gVariable.dispatchSheetForUI = toolClass.DeepClone(gVariable.productTaskSheet[myBoardIndex]);
//            gVariable.machineStatusForUI = toolClass.DeepClone(gVariable.machineStatus[myBoardIndex]);
//            gVariable.craftParamForUI = toolClass.DeepClone(gVariable.craftList[myBoardIndex]);
//            gVariable.qualityDataForUI = toolClass.DeepClone(gVariable.qualityList[myBoardIndex]);
//            gVariable.materialListTableForUI = toolClass.DeepClone(gVariable.materialListTable[myBoardIndex]);
        }



        public static DataTable getDispatchListFromDatabase(string databaseName, string tableName)
        {
            string commandText;

            try
            {
                commandText = "select * from `" + tableName + "`";
                return mySQLClass.queryDataTableAction(databaseName, commandText, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("getDispatchListFromDatabase failed! "+ ex);
                return null;
            }
        }

        //get quality data item list which inludes all items in a quality table
        public static int getQualityDataItemByDispatch(string databaseName, string tableName, string dispatchCode, string[] dataItemListArray)
        {
            int index, nextRecordIndex;
            string[] qualityRecordArray = new string[50];

            index = 0; //means quality data item index in quality data table starting from the first data item for the current didpatch 
            try
            {
                nextRecordIndex = 0;  //means quality data item index in quality data table starting from the first dispatch 
                while (true)
                {
                    //this function get whole record in quality list by the input dispatch code
                    nextRecordIndex = mySQLClass.getNextRecordByOneStrColumn(databaseName, tableName, "dispatchCode", dispatchCode, nextRecordIndex, qualityRecordArray);
                    if (nextRecordIndex == -1)
                    {
                        break;
                    }

                    dataItemListArray[index] = qualityRecordArray[mySQLClass.QUALITY_LIST_ID_ITEM_NAME];
                    index++;
                }
            }
            catch (Exception ex)
            {
                index = -1;
                Console.Write("getCurveInfoIngVariable() failed with exception: " + ex);
            }
            return index;
        }


        public static string getCurveInfoIngVariable(string databaseName, int myBoardIndex, int whoReadMe)
        {
            int i; //, num;
            int index, nextRecordIndex;
            int curveIndex, curveIndexPre;
            string dispatchCode;
            string commandText;
            string[] recordArray = new string[100];
            string[] titleVolCur = { "C相电压", "主轴相电流", "所有相总功率", "功率因数" };
            string[] unitVolCur = { "V", "A", "kW", "%" };
            float[] upperLimitVolCur = { 390, 40.0f, 32, 55 };
            float[] lowerLimitVolCur = { 370, 20.0f, 7, 45 };

            dispatchCode = "无工单";

            try
            {
                index = 0;
                curveIndex = 0;
                gVariable.totalCurveNum[myBoardIndex] = 0;

                //we use newest dispatch
                if (whoReadMe == gVariable.CURRENT_READING)
                {
                    //if dispatch already started, all dispatch/quality/craft/material info are put in gVariables, so don't need to read anything
                    if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED) //there is no dummy dispatch, so real dispatch is active, try to find it
                    {
                        //get newest dispatch in dispathList Table
                        commandText = "select * from `" + gVariable.dispatchListTableName + "` where status = '" + gVariable.MACHINE_STATUS_DISPATCH_APPLIED + "' order by id desc";
                        dispatchCode = mySQLClass.getColumnInfoByCommandText(databaseName, gVariable.dispatchListTableName, commandText, mySQLClass.DISPATCH_CODE_IN_DISPATCHLIST_DATABASE);  
                         if (dispatchCode == null)  //there is no new dispatch, so we keep in dummy mode
                            dispatchCode = gVariable.dummyDispatchTableName;
                    }
                    else
                    {
                        dispatchCode = gVariable.dummyDispatchTableName;
                    }
                }
                else
                {
                    //we are review old dispatch data, but maybe current dispatch is still undergoing, so don' change table name for craft/quality/volcur/beat
                    dispatchCode = gVariable.dispatchUnderReview;
                }

                //for craft
                while (true)
                {
                    nextRecordIndex = mySQLClass.getNextRecordByOneStrColumn(databaseName, gVariable.craftListTableName, "dispatchCode", dispatchCode, index, recordArray);
                    if (nextRecordIndex == -1)
                        break;

                    //dispatchBasedPortTableName is used when check database table name by port index, its index is port index
                    if (databaseName == gVariable.currentCurveDatabaseName)
                    {
                        gVariable.curveOrNot[curveIndex] = 1;
                        //all craft data will share one craft table name
                        gVariable.dispatchBasedCurveTableName[curveIndex] = dispatchCode + gVariable.craftTableNameAppendex;
                    }

                    //recordArray[0] is dispatch code, 
                    gVariable.curveTitle[curveIndex] = recordArray[1] + "(" + recordArray[5] + ")";  //title of this curve + ( unit )
                    gVariable.curveUpperLimit[curveIndex] = (float)Convert.ToDouble(recordArray[3]);  // limits for the curve
                    gVariable.curveLowerLimit[curveIndex] = (float)Convert.ToDouble(recordArray[2]);  //
                    curveIndex++;

                    index = nextRecordIndex;
                }
                if (databaseName == gVariable.currentCurveDatabaseName)
                    gVariable.numOfCurveForOneType[gVariable.CRAFT_DATA_IN_DATABASE] = curveIndex;

                //for voltage/current
                for (index = 0; index < 4; index++)
                {
                    //dispatchBasedPortTableName is used when check database table name by port index
                    if (databaseName == gVariable.currentCurveDatabaseName)
                    {
                        gVariable.curveOrNot[curveIndex] = 1;
                        //all vol/cur/power/factor data share one table name
                        gVariable.dispatchBasedCurveTableName[curveIndex] = dispatchCode + gVariable.volcurTableNameAppendex;
                    }

                    gVariable.curveTitle[curveIndex] = titleVolCur[index] + "(" + unitVolCur[index] + ")";  //title of this curve + (unit)
                    //all 0 means no limit
                    gVariable.curveUpperLimit[curveIndex] = upperLimitVolCur[index];  // limits for the curve
                    gVariable.curveLowerLimit[curveIndex] = lowerLimitVolCur[index];  //
                    curveIndex++;
                }
                if (databaseName == gVariable.currentCurveDatabaseName)
                    gVariable.numOfCurveForOneType[gVariable.VOLCUR_DATA_IN_DATABASE] = 4;

                //for beat
                index = 0;
                {
                    if (databaseName == gVariable.currentCurveDatabaseName)   //only when the current board is selected in room function, will these data be displayed on screen
                    {
                        gVariable.curveOrNot[curveIndex] = 1;
                        gVariable.dispatchBasedCurveTableName[curveIndex] = dispatchCode + gVariable.beatTableNameAppendex;
                    }

                    //recordArray[0] is dispatch code, 
                    gVariable.curveTitle[curveIndex] = "生产节拍(秒)";
                    gVariable.curveUpperLimit[curveIndex] = 0;
                    gVariable.curveLowerLimit[curveIndex] = 0;
                    curveIndex++;
                }
                if (databaseName == gVariable.currentCurveDatabaseName)
                    gVariable.numOfCurveForOneType[gVariable.BEAT_DATA_IN_DATABASE] = 1;

                curveIndexPre = curveIndex;

                //for quality
                index = 0;
                while (true)
                {
                    nextRecordIndex = mySQLClass.getNextRecordByOneStrColumn(databaseName, gVariable.qualityListTableName, "dispatchCode", dispatchCode, index, recordArray);
                    if (nextRecordIndex == -1)
                        break;

                    if (databaseName == gVariable.currentCurveDatabaseName)   //only when the current board is selected in room function, will these data be displayed on screen
                    {
                        gVariable.curveOrNot[curveIndex] = 1;
                        //all quality data share one table name
                        gVariable.dispatchBasedCurveTableName[curveIndex] = dispatchCode + gVariable.qualityTableNameAppendex;
                    }

                    //recordArray[0] is dispatch code, 
                    gVariable.curveTitle[curveIndex] = recordArray[1];  //title of this curve
                    gVariable.curveUpperLimit[curveIndex] = (float)Convert.ToDouble(recordArray[6]);  // limits for the curve
                    gVariable.curveLowerLimit[curveIndex] = (float)Convert.ToDouble(recordArray[4]);  //
                    curveIndex++;

                    index = nextRecordIndex;
                }
                if (databaseName == gVariable.currentCurveDatabaseName)
                    gVariable.numOfCurveForOneType[gVariable.QUALITY_DATA_IN_DATABASE] = curveIndex - curveIndexPre;

                if (databaseName == gVariable.currentCurveDatabaseName)   //only when the current board is selected in room function, will these data be displayed on screen
                    gVariable.totalCurveNum[myBoardIndex] = curveIndex;

                for (i = 0; i < curveIndex; i++)
                    gVariable.curveTextArray[i] = "0";
            }
            catch (Exception ex)
            {
                Console.Write("getCurveInfoIngVariable() failed with exception: " + ex);
            }
            return dispatchCode;
        }

        //由任务单产生工单
        //if there is a dispatch that is still undergoiing, just return this dispatch
        //if there is no dispatch undergoing, generate a new dispatch from current product task and return this new one
        public static int getDispatchFromTaskSheet(int machineID)
        {
            //int i;
            //int num;
            int hour;
            int status;
            int myBoardIndex;
            int outputNumForOneShift;
            int[] outputNumArray;
            string batchNum;
            string salesOrderBatchCode;
            string dispatchCode;
            string productCode;
            string timeStr;
            string machineStr;
            string planTime1, planTime2;
            string dName;
            string tName;
            string commandText;
            string[,] tableArray;
            gVariable.productStruct productImpl = new gVariable.productStruct();

            try
            {
                gVariable.dispatchSheetStruct tmpDispatchSheet;

                status = gVariable.MACHINE_STATUS_DISPATCH_APPLIED;// == 2
                //status = gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED;// == 0

                tmpDispatchSheet = new gVariable.dispatchSheetStruct();
                
                dispatchCode = null;
                planTime1 = null;
                planTime2 = null;

                if (machineID >= gVariable.slittingProcess[0])
                    machineStr = "F" + (machineID - gVariable.slittingProcess[0] + 1).ToString();
                else if(machineID >= gVariable.printingProcess[0])
                    machineStr = "Y" + (machineID - gVariable.printingProcess[0] + 1).ToString();
                else
                    machineStr = "L" + (machineID - gVariable.castingProcess[0] + 1).ToString();

                myBoardIndex = machineID - 1;
                dName = gVariable.internalMachineName[myBoardIndex];
                tName = gVariable.productTaskListTableName;
                //taskSheetStr = batchNum + machineStr;

                //find earlist product task sheet that are applied/started, and set it as our target task sheet, 0 means published, 1 means completed, 2 means applied, 3 means started 
                commandText = "select * from `" + tName + "` where status >= '" + status + "' or status = '0' order by planTime1"; // DESC"; // and dispatchCode = '" + taskSheetStr + "'";  ;
                tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                if (tableArray != null)  //working task sheet found, dispatch must have existed, try to get it
                {
                    batchNum = tableArray[0, 32];
                    productCode = tableArray[0, 5];
                    salesOrderBatchCode = tableArray[0, 39];

                    //get output capability for this machine
                    outputNumArray = toolClass.getMachineCapability(productCode);
                    outputNumForOneShift = outputNumArray[myBoardIndex] * gVariable.EFFECTIVE_WORKING_HOUR_ONE_SHIFT;

                    //dispatch and its related task sheet share the same batchNum
                    commandText = "select * from `" + gVariable.dispatchListTableName + "` where status != '1' and batchNum = '" + batchNum + "'";
                    tmpDispatchSheet = mySQLClass.getDispatchByCommand(dName, commandText);
                    if (tmpDispatchSheet.batchNum != "0" && tmpDispatchSheet.batchNum != null)
                    {
                        //working dispatch found, we put this dispatch into global buffer and return
                        gVariable.dispatchSheet[myBoardIndex] = tmpDispatchSheet;
                        return 0;
                    }
                    else
                    {
                        //no dispatch found, we continue to work generate a new one based on task sheet
                    }
                }
                else  //no working task sheet, so there should be no dispatch applied or started, if there is no new task, return fail, else generate a new dispatch
                {
                    //find earlist product task sheet that is published
                    commandText = "select * from `" + tName + "` where status = '0' order by planTime1"; // DESC"; // and dispatchCode = '" + taskSheetStr + "'";
                    tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                    if (tableArray != null)  //published task sheet found, generate a new dispatch from it
                    {
                        batchNum = tableArray[0, 32];
                        productCode = tableArray[0, 5];
                        salesOrderBatchCode = tableArray[0, 39];

                        //get output capability for this machine
                        outputNumArray = toolClass.getMachineCapability(productCode);
                        outputNumForOneShift = outputNumArray[myBoardIndex] * gVariable.EFFECTIVE_WORKING_HOUR_ONE_SHIFT;
                    }
                    else
                    {
                        return -1;  //there is no task sheet for this machine right now, so no dispatch available
                    }
                }

                timeStr = DateTime.Now.ToString("T");
                hour = Convert.ToInt32(timeStr.Remove(2));

                if (hour >= 19 && hour <= 23)
                {
                    timeStr = DateTime.Now.ToString("dd");
                    dispatchCode = batchNum + timeStr + "2" + machineStr;
                    planTime1 = DateTime.Now.Date.ToString("yyyy-MM-dd") + " 20:00";
                    planTime2 = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd") + " 08:00"; ;
                }
                else if (hour >= 0 && hour < 7)
                {
                    timeStr = DateTime.Now.Date.AddDays(-1).ToString("dd");
                    dispatchCode = batchNum + timeStr + "2" + machineStr;
                    planTime1 = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd") + " 20:00";
                    planTime2 = DateTime.Now.Date.ToString("yyyy-MM-dd") + " 08:00";
                }
                else if (hour >= 7 && hour < 19)
                {
                    timeStr = DateTime.Now.Date.ToString("dd");
                    dispatchCode = batchNum + timeStr + "1" + machineStr;
                    planTime1 = DateTime.Now.Date.ToString("yyyy-MM-dd") + " 08:00";
                    planTime2 = DateTime.Now.Date.ToString("yyyy-MM-dd") + " 20:00";
                }

                generateNewDispatchByTasktable(ref tmpDispatchSheet, tableArray, dispatchCode, planTime1, planTime2, outputNumForOneShift);
                if (tmpDispatchSheet.batchNum != "0")
                {
                    //new dispatch found, we put this dispatch into global buffer and return
                    gVariable.dispatchSheet[myBoardIndex] = tmpDispatchSheet;
                }
                else
                {
                    //generate new dispatc failed
                    return -1;
                }
                mySQLClass.writeDataToDispatchListTable(dName, gVariable.dispatchListTableName, tmpDispatchSheet);

                commandText = "select * from `" + gVariable.productTableName + "` where productCode = '" + productCode + "'";
                mySQLClass.readProductInfo(ref productImpl, commandText);
                
                if (machineID >= gVariable.slittingProcess[0])
                {
                    generateSupplementaryList(dName, gVariable.materialListTableName, dispatchCode, machineID.ToString(), outputNumForOneShift, batchNum, salesOrderBatchCode, productImpl);
                }
                else if (machineID >= gVariable.printingProcess[0])
                {
                    generatePrintInkList(dName, gVariable.materialListTableName, dispatchCode, machineID.ToString(), outputNumForOneShift, batchNum, salesOrderBatchCode, productImpl);
                }
                else
                {
                    generateMaterialList(dName, gVariable.materialListTableName, dispatchCode, machineID.ToString(), outputNumForOneShift, batchNum, salesOrderBatchCode, productImpl);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("getDispatchFromSalesOrder failed! " + ex);
            }

            return 0;
        }

        static void generateNewDispatchByTasktable(ref gVariable.dispatchSheetStruct dispatchSheet, string[,] tableArray, string dispatchCode, string planTime1, string planTime2, int outputNumForOneShift)
        {
            int i;
            int plannedNum;
            int outputNum;

            i = 1;

            dispatchSheet.machineID = tableArray[0, i++];
            dispatchSheet.dispatchCode = dispatchCode;
            dispatchSheet.planTime1 = planTime1;
            dispatchSheet.planTime2 = planTime2;
            i += 3;
            dispatchSheet.productCode = tableArray[0, i++];
            dispatchSheet.productName = tableArray[0, i++];
            dispatchSheet.operatorName = tableArray[0, i++];

            plannedNum = Convert.ToInt32(tableArray[0, i++]);
            outputNum = Convert.ToInt32(tableArray[0, i++]);

            if(plannedNum <= outputNum) //plan completed, but dispatch still not complete, maybe they want to have more output, continue with max capability
            {
                plannedNum = outputNumForOneShift;
            }
            else
            {
                if (plannedNum - outputNum > outputNumForOneShift)
                    plannedNum = outputNumForOneShift; //still have a lot to produce
                else
                    plannedNum = plannedNum - outputNum;  //this value is OK
            }

            dispatchSheet.plannedNumber = plannedNum;
            dispatchSheet.outputNumber = 0;
            dispatchSheet.qualifiedNumber = 0;
            dispatchSheet.unqualifiedNumber = 0;
            i += 2;
            dispatchSheet.processName = tableArray[0, i++];
            dispatchSheet.realStartTime = tableArray[0, i++];
            dispatchSheet.realFinishTime = tableArray[0, i++];
            dispatchSheet.prepareTimePoint = tableArray[0, i++];
            dispatchSheet.status = gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED;
            dispatchSheet.waitForCheck = 0;
            dispatchSheet.wastedOutput = 0;
            dispatchSheet.workTeam = "";
            i += 4;
            dispatchSheet.batchNum2 = tableArray[0, i++];
            dispatchSheet.batchNum3 = tableArray[0, i++];
            dispatchSheet.workshop = tableArray[0, i++];
            dispatchSheet.workshift = tableArray[0, i++];
            dispatchSheet.salesOrderCode = tableArray[0, i++];
            dispatchSheet.BOMCode = tableArray[0, i++];
            dispatchSheet.customer = tableArray[0, i++];
            dispatchSheet.multiProduct = tableArray[0, i++];
            dispatchSheet.productCode2 = tableArray[0, i++];
            dispatchSheet.productCode3 = tableArray[0, i++];
            dispatchSheet.operatorName2 = tableArray[0, i++];
            dispatchSheet.operatorName3 = tableArray[0, i++];
            dispatchSheet.batchNum = tableArray[0, i++];
            dispatchSheet.batchNum1 = tableArray[0, i++];
            dispatchSheet.rawMateialcode = tableArray[0, i++];
            dispatchSheet.productLength = tableArray[0, i++];
            dispatchSheet.productDiameter = tableArray[0, i++];
            dispatchSheet.productWeight = tableArray[0, i++];
            dispatchSheet.slitWidth = tableArray[0, i++];
            dispatchSheet.salesOrderBatchCode = tableArray[0, i++];
            dispatchSheet.productCode4 = tableArray[0, i++];
            dispatchSheet.operatorName4 = tableArray[0, i++];
            dispatchSheet.notes = tableArray[0, i++];
            dispatchSheet.comments = tableArray[0, i++];
        }

        public static void generateMaterialList(string databaseName, string tableName, string dispatchCode, string machineID, int outputNum, string batchNum, string salesOrderBatchCode, gVariable.productStruct productImpl)
        {
            int i;
            //string str;
            //string num;
            string commandText;
            gVariable.BOMListStruct BOMImpl = new gVariable.BOMListStruct();
            gVariable.materialListStruct materialSheet = new gVariable.materialListStruct();

            try
            {
                BOMImpl.materialName = new string[gVariable.maxMaterialTypeNum];
                BOMImpl.materialCode = new string[gVariable.maxMaterialTypeNum];
                BOMImpl.materialQuantity = new double[gVariable.maxMaterialTypeNum];

                materialSheet.materialCode = new string[gVariable.maxMaterialTypeNum];
                materialSheet.materialRequired = new int[gVariable.maxMaterialTypeNum];
                materialSheet.previousLeft = new int[gVariable.maxMaterialTypeNum];
                materialSheet.currentlyUsed = new int[gVariable.maxMaterialTypeNum];
                materialSheet.currentLeft = new int[gVariable.maxMaterialTypeNum];

                commandText = "select * from `" + gVariable.bomTableName + "` where BOMCode = '" + productImpl.BOMCode + "'";
                mySQLClass.readBOMInfo(ref BOMImpl, commandText);

                materialSheet.salesOrderBatchCode = salesOrderBatchCode;
                materialSheet.salesOrderCode = salesOrderBatchCode.Remove(7);
                materialSheet.dispatchCode = dispatchCode;  //
                materialSheet.machineID = machineID;
                materialSheet.machineCode = "00"; // gVariable.machineCodeArrayZihua[Convert.ToInt32(machineID) - 1];
                materialSheet.machineName = gVariable.machineNameArrayAPS[Convert.ToInt32(machineID) - 1];
                //materialSheet.status = "0";
                materialSheet.numberOfTypes = BOMImpl.numberOfTypes;
                materialSheet.batchNum = batchNum;
                materialSheet.materialStatus = "0000000";

                for (i = 0; i < materialSheet.numberOfTypes; i++)
                {
                    materialSheet.materialCode[i] = BOMImpl.materialCode[i];
                    materialSheet.materialRequired[i] = (int)(BOMImpl.materialQuantity[i] * outputNum);  //material requirement by kilogram
                    materialSheet.previousLeft[i] = 0;
                    materialSheet.currentlyUsed[i] = 0;
                    materialSheet.currentLeft[i] = 0;
                }

                mySQLClass.writeDataToMaterialListTable(databaseName, tableName, materialSheet);
            }
            catch (Exception ex)
            {
                Console.WriteLine("generateMaterialList failed! " + ex);
            }
        }

        public static void generatePrintInkList(string databaseName, string tableName, string dispatchCode, string machineID, int outputNum, string batchNum, string salesOrderBatchCode, gVariable.productStruct productImpl)
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
                materialSheet.previousLeft = new int[gVariable.maxMaterialTypeNum];
                materialSheet.currentlyUsed = new int[gVariable.maxMaterialTypeNum];
                materialSheet.currentLeft = new int[gVariable.maxMaterialTypeNum];

                commandText = "select typeNum, oil1, num1, oil2, num2, oil3, num3, oil4, num4, oil5, num5, oil6, num6, oil7, num7 from `" + gVariable.inkBomTableName + "` where BOMName = '" + productImpl.inkRatio + "'";
                tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (tableArray == null)
                    return;

                materialSheet.salesOrderBatchCode = salesOrderBatchCode;
                materialSheet.salesOrderCode = salesOrderBatchCode.Remove(7);
                materialSheet.dispatchCode = dispatchCode;  //
                materialSheet.machineID = machineID;
                materialSheet.machineCode = "00"; // gVariable.machineCodeArrayZihua[Convert.ToInt32(machineID) - 1];
                materialSheet.machineName = gVariable.machineNameArrayAPS[Convert.ToInt32(machineID) - 1];
                //materialSheet.status = "0";
                materialSheet.numberOfTypes = Convert.ToInt32(tableArray[0, 0]);
                materialSheet.batchNum = batchNum;
                materialSheet.materialStatus = "0000000";

                for (i = 0; i < materialSheet.numberOfTypes; i++)
                {
                    quantity = Convert.ToDouble(tableArray[0, 2 + i * 2]);
                    materialSheet.materialCode[i] = tableArray[0, 1 + i * 2];
                    materialSheet.materialRequired[i] = (int)(quantity * outputNum / 1000);  //quantity is weight of kg for oil by 1000 kg of product
                    materialSheet.previousLeft[i] = 0;
                    materialSheet.currentlyUsed[i] = 0;
                    materialSheet.currentLeft[i] = 0;
                }

                mySQLClass.writeDataToMaterialListTable(databaseName, tableName, materialSheet);
            }
            catch (Exception ex)
            {
                Console.WriteLine("generatePrintInkList failed! " + ex);
            }
        }

        public static void generateSupplementaryList(string databaseName, string tableName, string dispatchCode, string machineID, int outputNum, string batchNum, string salesOrderBatchCode, gVariable.productStruct productImpl)
        {
            int i;
            //string str;
            //string num;
            string commandText;
            string[,] tableArray;
            gVariable.materialListStruct materialSheet = new gVariable.materialListStruct();
            string[] supplementaryMaterial = { "铲板", "纸芯", "纸板", "牛皮纸", "瓦楞纸", "缠绕膜", "包装膜" }; 

            try
            {
                materialSheet.materialCode = new string[gVariable.maxMaterialTypeNum];
                materialSheet.materialRequired = new int[gVariable.maxMaterialTypeNum];
                materialSheet.previousLeft = new int[gVariable.maxMaterialTypeNum];
                materialSheet.currentlyUsed = new int[gVariable.maxMaterialTypeNum];
                materialSheet.currentLeft = new int[gVariable.maxMaterialTypeNum];

                commandText = "select stackNumOneTon, paperCoreOneTon, paperBoardOneton, craftPaperOneTon, corrugatedPaperPerTon, wrappingFilmOneTon, packingFilmOneTon from `" + gVariable.productTableName + "` where productCode = '" + productImpl.productCode + "'";
                tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if (tableArray == null)
                    return;

                materialSheet.salesOrderBatchCode = salesOrderBatchCode;
                materialSheet.salesOrderCode = salesOrderBatchCode.Remove(7);
                materialSheet.dispatchCode = dispatchCode;  //
                materialSheet.machineID = machineID;
                materialSheet.machineCode = "00"; // gVariable.machineCodeArrayZihua[Convert.ToInt32(machineID) - 1];
                materialSheet.machineName = gVariable.machineNameArrayAPS[Convert.ToInt32(machineID) - 1];
                //materialSheet.status = "0";
                materialSheet.numberOfTypes = 7;  //
                materialSheet.batchNum = batchNum;
                materialSheet.materialStatus = "0000000";

                for (i = 0; i < materialSheet.numberOfTypes; i++)
                {
                    materialSheet.materialCode[i] = supplementaryMaterial[i];

                    if (tableArray[0, i] == null || tableArray[0, i] == "")
                        materialSheet.materialRequired[i] = 0;
                    else
                        materialSheet.materialRequired[i] = (int)(Convert.ToDouble(tableArray[0, i]) * outputNum / 1000) + 1;  //quantity is weight of kg for oil by 1000 kg of product

                    materialSheet.previousLeft[i] = 0;
                    materialSheet.currentlyUsed[i] = 0;
                    materialSheet.currentLeft[i] = 0;
                }

                mySQLClass.writeDataToMaterialListTable(databaseName, tableName, materialSheet);
            }
            catch (Exception ex)
            {
                Console.WriteLine("generateSupplementaryList failed! " + ex);
            }
        }


        //if there is no dispatch running, we get the next dispatch, otherwise, get the currently running dispatch
        //return: 0 got a new dispatch
        //        1 continue with the current dispatch
        //       -1 failed to find a dispatch
        public static int getCurrentDispatch(int machineID)
        {
            int ret;
            int myBoardIndex;
            int castIndex;
            string dName;
            string tName;
            string commandText;
            string[,] tableArray;

            try
            {
                ret = 0;
                myBoardIndex = machineID - 1;
                dName = gVariable.internalMachineName[myBoardIndex];
                tName = gVariable.dispatchListTableName;

                //find oldest dispatch that are applied/started, and set it as our target dispatch, 0 means published, 1 means completed, 2 means applied, 3 means started 
                commandText = "select * from `" + tName + "` where status > 1 order by planTime1 DESC";

                tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                if (tableArray != null)
                {
                    commandText = "select * from `" + tName + "` where id = '" + Convert.ToInt32(tableArray[0, 0]) + "'";
                    //tableArray[0, 0] means the ID of the first dispatch by commandText 
                    gVariable.dispatchSheet[myBoardIndex] = mySQLClass.getDispatchByCommand(dName, commandText);
                    ret = 1;
                }
                else  //find new dispatches
                {
                    commandText = "select * from `" + tName + "` where status = 0 order by planTime1 DESC";

                    tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                    if (tableArray != null)
                    {
                        commandText = "select * from `" + tName + "` where id = '" + Convert.ToInt32(tableArray[0, 0]) + "'";
                        //tableArray[0, 0] means the ID of the first dispatch by commandText 
                        gVariable.dispatchSheet[myBoardIndex] = mySQLClass.getDispatchByCommand(dName, commandText);
                        ret = 0;
                    }
                    else
                    {
                        return -1;
                    }
                }

                castIndex = Convert.ToInt32(gVariable.dispatchSheet[myBoardIndex].dispatchCode.Remove(0, 4).Remove(1));

                gVariable.workingMachineForCurrentDispatch[castIndex, 0] = castIndex;  //first put cast machine index
                if (machineID >= gVariable.printingProcess[0] && machineID < gVariable.slittingProcess[0])
                    gVariable.workingMachineForCurrentDispatch[castIndex, 1] = machineID - 1;  //if this is a printing machine, put machine index into array
                else if (machineID >= gVariable.slittingProcess[0] && machineID < gVariable.packingProcess[0])
                    gVariable.workingMachineForCurrentDispatch[castIndex, 2] = machineID - 1;  //if this is a slitting machine, put machine index into array

                return ret;
            }
            catch(Exception ex)
            {
                Console.WriteLine("getCurrentDispatch for machineID " + machineID + " failed!" + ex);
                return -1;
            }
        }

        //if there is no dispatch running, we get the next dispatch, otherwise, get the currently running dispatch
        //return: 0 got a new task
        //        1 continue with the current task
        //       -1 failed to find a task
        public static int getCurrentProductTask(int machineID)
        {
            int myBoardIndex;
            string dName;
            string tName;
            string commandText;
            string[,] tableArray;

            try
            {
                myBoardIndex = machineID - 1;
                dName = gVariable.internalMachineName[myBoardIndex];
                tName = gVariable.productTaskListTableName;

                //find oldest product tasks that are applied/started, and set it as our target product task, 0 means published, 1 means completed, 2 means applied, 3 means started 
                commandText = "select * from `" + tName + "` where status > 1 order by planTime1 DESC";

                tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                if (tableArray != null)
                {
                    //an applied task is found
                    commandText = "select * from `" + tName + "` where id = '" + Convert.ToInt32(tableArray[0, 0]) + "'";
                    //tableArray[0, 0] means the ID of the first dispatch by commandText 
                    gVariable.productTaskSheet[myBoardIndex] = mySQLClass.getDispatchByCommand(dName, commandText);
                    return 1;
                }
                else  //find new product tasks
                {
                    commandText = "select * from `" + tName + "` where status = 0 order by planTime1 DESC";

                    tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                    if (tableArray != null)
                    {
                        commandText = "select * from `" + tName + "` where id = '" + Convert.ToInt32(tableArray[0, 0]) + "'";
                        //tableArray[0, 0] means the ID of the first dispatch by commandText 
                        gVariable.productTaskSheet[myBoardIndex] = mySQLClass.getDispatchByCommand(dName, commandText);
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("getCurrentDispatch for machineID " + machineID + " failed!" + ex);
                return -1;
            }
        }

        public static string getMaterialRequirementForOneMachine(int machineID)
        {
            int j;
            string str;
            string dName;
            string tName;
            string materialRequiresheetCode;
            string commandText;
            string[,] tableArray;
            float[] weightRequiredOfEachStack = new float[7];
            string[] materialCodeOfEachStack = new string[7];

            str = null;
            try
            {
                materialRequiresheetCode = getMaterialRequireSheet(machineID);
                if (materialRequiresheetCode == null)
                {
                    for (j = 0; j < 7; j++)
                    {
                        str += ";;";
                    }
                    return str;
                }
                dName = gVariable.internalMachineName[machineID - 1];
                tName = gVariable.materialRequirementTableName;
                //find this material request sheet
                commandText = "select * from `" + tName + "` where materialRequiresheetCode = '" + materialRequiresheetCode + "'"; 
                tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                if (tableArray != null)  //
                {
                    for (j = 0; j < gVariable.maxMaterialTypeNum; j++)
                    {
                        materialCodeOfEachStack[j] = tableArray[0, 8 + j * 2];
                        weightRequiredOfEachStack[j] = (float)Convert.ToDouble(tableArray[0, 8 + j * 2 + 1]);
                    }
                }
                for (j = 0; j < 7; j++)
                {
                    str += materialCodeOfEachStack[j] + ";" + weightRequiredOfEachStack[j].ToString() + ";";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("getMaterialInfoForOneMachines failed!" + ex);
            }
            return str;
        }

        //return material requirement sheet code (物料需求单)
        public static string getMaterialRequireSheet(int machineID)
        {
            //int i;
            //int num;
            //int ret;
            int status;
            int myBoardIndex;
            int requiredOutputNum;
            int qualifiedOutputNum;
            int[] outputNumArray;
            string batchNum;
            string salesOrderBatchCode;
            string dispatchCode;
            string productCode;
            string machineStr;
            string planTime1;
            string dName;
            string tName;
            string commandText;
            string[,] tableArray;
            gVariable.productStruct productImpl = new gVariable.productStruct();

            try
            {
                //material requirement function only occurs to cast machine, generate material requirement sheet within 24 hours(24小时物料需求单)
                if (machineID >= gVariable.printingProcess[0])
                    return null;

                status = gVariable.MACHINE_STATUS_DISPATCH_APPLIED;// == 2

                dispatchCode = null;
                planTime1 = null;
                //planTime2 = null;

                if (machineID >= gVariable.slittingProcess[0])
                    machineStr = "F" + (machineID - gVariable.slittingProcess[0] + 1).ToString();
                else if (machineID >= gVariable.printingProcess[0])
                    machineStr = "Y" + (machineID - gVariable.printingProcess[0] + 1).ToString();
                else
                    machineStr = "L" + (machineID - gVariable.castingProcess[0] + 1).ToString();

                myBoardIndex = machineID - 1;
                dName = gVariable.internalMachineName[myBoardIndex];
                tName = gVariable.productTaskListTableName;
                //taskSheetStr = batchNum + machineStr;

                //find earlist product task sheet that are applied/started, and set it as our target task sheet, 0 means published, 1 means completed, 2 means applied, 3 means started 
                commandText = "select * from `" + tName + "` where status >= '" + status + "' order by planTime1"; // DESC"; // and dispatchCode = '" + taskSheetStr + "'";  ;
                tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                if (tableArray != null)  //working task sheet found, dispatch must have existed, try to get it
                {
                    dispatchCode = tableArray[0, 2];
                    batchNum = tableArray[0, 32];
                    productCode = tableArray[0, 5];
                    salesOrderBatchCode = tableArray[0, 39];
                    requiredOutputNum = Convert.ToInt32(tableArray[0, 8]);
                    qualifiedOutputNum = Convert.ToInt32(tableArray[0, 10]);
                    if (qualifiedOutputNum >= requiredOutputNum)   //qualifiedOutputNum is only recorded when the work is off duty, so this means the batch of work is completed by previous team
                    {
                        return null;
                    }
                    requiredOutputNum = requiredOutputNum - qualifiedOutputNum; 

                    //get output capability for this machine
                    outputNumArray = toolClass.getMachineCapability(productCode);  
                }
                else  //no working task sheet, so there should be no dispatch applied or started, try to find a nearest product task
                {
                    //find earlist product task sheet that is published
                    commandText = "select * from `" + tName + "` where status = '0' order by planTime1"; // DESC"; // and dispatchCode = '" + taskSheetStr + "'";
                    tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                    if (tableArray != null)  //published task sheet found, generate a new dispatch from it
                    {
                        planTime1 = tableArray[0, 3];
                        //ret = planTime1.CompareTo(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm"));
                        //if(ret < 0)
                        //{
                            //there is no production task today, don't  
                            //return -1;
                        //}
                        dispatchCode = tableArray[0, 2];
                        batchNum = tableArray[0, 32];
                        productCode = tableArray[0, 5];
                        salesOrderBatchCode = tableArray[0, 39];
                        requiredOutputNum = Convert.ToInt32(tableArray[0, 8]);

                        //get output capability for this machine
                        outputNumArray = toolClass.getMachineCapability(productCode);
                    }
                    else
                    {
                        return null;  //there is no task sheet for this machine right now, so no material requirment
                    }
                }

                //if the output requested for this production task is larger than the capability of 2 work shifts(24 hours)
                if (requiredOutputNum > outputNumArray[myBoardIndex] * gVariable.EFFECTIVE_WORKING_HOUR_ONE_SHIFT * 2)
                {
                    requiredOutputNum = outputNumArray[myBoardIndex] * gVariable.EFFECTIVE_WORKING_HOUR_ONE_SHIFT * 2;
                }
                else
                {
                    ;
                }

                commandText = "select * from `" + gVariable.productTableName + "` where productCode = '" + productCode + "'";
                mySQLClass.readProductInfo(ref productImpl, commandText);

                return getMaterialRequired(dName, gVariable.materialRequirementTableName, machineID.ToString(), requiredOutputNum, batchNum, salesOrderBatchCode, productImpl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("getMaterialInfoForAllMachines failed!" + ex);
            }

            return null;
        }

        public static string getMaterialRequired(string databaseName, string tableName, string machineID, int outputNum, string batchNum, string salesOrderBatchCode, gVariable.productStruct productImpl)
        {
            int i;
            //string str;
            //string num;
            string commandText;
            gVariable.BOMListStruct BOMImpl = new gVariable.BOMListStruct();
            gVariable.materialRequirementStruct materialRequirementImpl = new gVariable.materialRequirementStruct();

            try
            {
                BOMImpl.materialName = new string[gVariable.maxMaterialTypeNum];
                BOMImpl.materialCode = new string[gVariable.maxMaterialTypeNum];
                BOMImpl.materialQuantity = new double[gVariable.maxMaterialTypeNum];

                materialRequirementImpl.materialCode = new string[gVariable.maxMaterialTypeNum];
                materialRequirementImpl.materialRequired = new int[gVariable.maxMaterialTypeNum];

                commandText = "select * from `" + gVariable.bomTableName + "` where BOMCode = '" + productImpl.BOMCode + "'";
                mySQLClass.readBOMInfo(ref BOMImpl, commandText);

                materialRequirementImpl.materialRequiresheetCode = DateTime.Now.Date.ToString("yyMdd") + batchNum.Remove(0, 4);
                materialRequirementImpl.salesOrderBatchCode = salesOrderBatchCode;
                materialRequirementImpl.batchNum = batchNum;
                materialRequirementImpl.salesOrderCode = salesOrderBatchCode.Remove(7);
                materialRequirementImpl.status = "0";
                materialRequirementImpl.machineID = machineID;
                materialRequirementImpl.numberOfTypes = BOMImpl.numberOfTypes;

                for (i = 0; i < materialRequirementImpl.numberOfTypes; i++)
                {
                    materialRequirementImpl.materialCode[i] = BOMImpl.materialCode[i];
                    materialRequirementImpl.materialRequired[i] = (int)(BOMImpl.materialQuantity[i] * outputNum);  //material requirement by kilogram
                }

                mySQLClass.writeDataToMaterialRequirementTable(databaseName, tableName, materialRequirementImpl);

                return materialRequirementImpl.materialRequiresheetCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("generateMaterialList failed! " + ex);
                return null;
            }
        }
    }
}
