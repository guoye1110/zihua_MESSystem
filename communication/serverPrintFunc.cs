using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using MESSystem.quality;
using MESSystem.common;

namespace MESSystem.communication
{
    public partial class communicate
    {
        public const int NUM_OF_FEEDING_MACHINE = 5;
        public const int STACK_NUM_ONE_MACHINE = 8;

        public const int DISPATCHCODE_LENGTH = 11;

        public const int WAREHOUSE_INPUT_OUTPUT_PC = 101;
        public const int CAST_PROCESS_PC1 = 141;
        public const int CAST_PROCESS_PC2 = 142;
        public const int CAST_PROCESS_PC3 = 143;
        public const int CAST_PROCESS_PC4 = 144;
        public const int CAST_PROCESS_PC5 = 145;
        public const int PRINT_PROCESS_PC1 = 161;
        public const int PRINT_PROCESS_PC2 = 162;
        public const int PRINT_PROCESS_PC3 = 163;
        public const int SLIT_PROCESS_PC1 = 181;
        public const int SLIT_PROCESS_PC2 = 181;
        public const int SLIT_PROCESS_PC3 = 181;
        public const int SLIT_PROCESS_PC4 = 181;
        public const int SLIT_PROCESS_PC5 = 181;
        public const int INSPECTION_PROCESS_PC1 = 201;
        public const int REBUILD_PROCESS_PC1 = 221;
        public const int PACKING_PROCESS_PC1 = 241;

        public partial class ClientThread
        {
            int machineIDForPrint;
            //material code for stacks(码垛对应的原料编号)
            string[,] materialCodeForStack = new string[NUM_OF_FEEDING_MACHINE, STACK_NUM_ONE_MACHINE]; 
            //total sack num for this dispatch, got from dispatch info（工单中原料需求的袋数）
            int[,] sackNumTotalForDispatch = new int[NUM_OF_FEEDING_MACHINE, STACK_NUM_ONE_MACHINE];
            //sack num received from warehouse(搬运工已经搬来了几袋原料, 数量应小于等于 sackNumNeededForStack[])
            int[,] sackNumReceivedFromWarehouse = new int[NUM_OF_FEEDING_MACHINE, STACK_NUM_ONE_MACHINE];
            //total sack num needed for this stack(or feed bin) for multiple dispatches in a period of time(对于搬运工而言，某个码垛中需要几袋原料，可能包括多个工单，可能超过码垛容量，需多次送料)
            int[,] sackNumNeededForStack = new int[NUM_OF_FEEDING_MACHINE, STACK_NUM_ONE_MACHINE];
            //sack num left in the stack for the current time（码垛中的剩余袋数）
            int[,] sackNumLeftInStack = new int[NUM_OF_FEEDING_MACHINE, STACK_NUM_ONE_MACHINE];
            //material left in feed bin(料箱中的余料公斤数)
            int[,] kiloNumLeftInFeedBin = new int[NUM_OF_FEEDING_MACHINE, STACK_NUM_ONE_MACHINE];

            int inoutMaterialQuantity;
            int inoutFeedMachineID;
            int inoutMaterialStackID;

            void initVariables()
            {
                int i, j;

                handshakeWithClientOK = 1;

                inoutMaterialQuantity = 0;
                inoutFeedMachineID = 0;
                inoutMaterialStackID = 0;

                for (i = 0; i < NUM_OF_FEEDING_MACHINE; i++)
                {
                    for (j = 0; j < STACK_NUM_ONE_MACHINE; j++)
                    {
                        materialCodeForStack[i, j] = null;
                        sackNumTotalForDispatch[i, j] = 0;
                        sackNumReceivedFromWarehouse[i, j] = 0;
                        sackNumNeededForStack[i, j] = 0;
                        sackNumLeftInStack[i, j] = 0;
                        kiloNumLeftInFeedBin[i, j] = 0;
                    }
                }
            }

            public void processLabelPrintingFunc(int communicationType, int packetLen)
            {
                //int i;
                int len;
                int printingSWPCID;
                string str;
                string dName;
                //string commandText;
                //string[,] tableArray;

                try
                {
                    switch (communicationType)
                    {
                        case COMMUNICATION_TYPE_HANDSHAKE_PRINT_MACHINE_ID:
                            machineIDForPrint = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 + onePacket[PROTOCOL_DATA_POS + 2] * 0x10000 + onePacket[PROTOCOL_DATA_POS + 3] * 0x1000000;
                            initVariables();
                            //confirm the receive of handshake packet
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_WAREHOUE_OUT_START:  //原料出库区的搬运工上班了
                            getMaterialInfoForAllMachines();
                            sendMaterialInfoToPrintSW();
                            break;
                        case COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE:  //一铲车的原料出库了, 收到出库铲车标签
                            dealWithMaterialOut();
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE:  //一铲车的余料回收入库了, 收到入库铲车标签
                            dealWithMaterialIn();
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_CAST_PROCESS_START:  //流延设备工人上班了
                            printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - CAST_PROCESS_PC1 + gVariable.castingProcess[0];
                            dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');

                            sendDispatchToClient(dName, communicationType);
                            break;
                        case COMMUNICATION_TYPE_CAST_BARCODE_UPLOAD: //完成一个大卷，收到大卷流延标签
                            len = onePacket[PROTOCOL_LEN_POS] + onePacket[PROTOCOL_LEN_POS + 1] * 0x100 - MIN_PACKET_LEN_PURE_FRAME;

                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_PRINT_PROCESS_START:  //印刷设备工人上班了
                            printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - PRINT_PROCESS_PC1 + gVariable.printingProcess[0];
                            dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
                            sendDispatchToClient(dName, communicationType);
                            break;
                        case COMMUNICATION_TYPE_PRINT_BARCODE_UPLOAD:  //完成一个大卷，收到大卷印刷标签
                            len = onePacket[PROTOCOL_LEN_POS] + onePacket[PROTOCOL_LEN_POS + 1] * 0x100 - MIN_PACKET_LEN_PURE_FRAME;

                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_SLIT_PROCESS_START:  //分切工上班了
                            //get string sent from client
                            len = onePacket[PROTOCOL_LEN_POS] + onePacket[PROTOCOL_LEN_POS + 1] * 0x100;
                            //there are 4 extra bytes indicating which slit machine we are using, so we need to minus 4
                            str = System.Text.Encoding.GetEncoding("gb2312").GetString(onePacket, PROTOCOL_DATA_POS + 4, len - MIN_PACKET_LEN_PURE_FRAME - 4);

                            printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - SLIT_PROCESS_PC1 + gVariable.slittingProcess[0];
                            dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');

                            getDispatchByCodeAndSendToClient(dName, communicationType, str);
                            break;
                        case COMMUNICATION_TYPE_SLIT_BARCODE_UPLOAD:  //分切一个小卷完工，收到小卷分切标签
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_INSPECTION_PROCESS_START:  //质检开始
                            //get slit machine id from onepacket
                            printingSWPCID = onePacket[PROTOCOL_DATA_POS + DISPATCHCODE_LENGTH + 1] - 0x31 + gVariable.slittingProcess[0];
                            dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
                            sendDispatchToClient(dName, communicationType);
                            break;
                        case COMMUNICATION_TYPE_INSPECTION_BARCODE_UPLOAD:  //质检结果上传，收到质检标签
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_REUSE_PROCESS_START:  //再造料开工
                            printingSWPCID = onePacket[PROTOCOL_DATA_POS + DISPATCHCODE_LENGTH + 1] - 0x31 + gVariable.slittingProcess[0];
                            dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
                            sendDispatchToClient(dName, communicationType);
                            break;
                        case COMMUNICATION_TYPE_REUSE_BARCODE_UPLOAD:  //收到再造料使用的小卷标签
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_PACKING_PROCESS_START: //打包工开工
                            printingSWPCID = onePacket[PROTOCOL_DATA_POS + DISPATCHCODE_LENGTH + 1] - 0x31 + gVariable.slittingProcess[0];
                            dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
                            sendDispatchToClient(dName, communicationType);
                            break;
                        case COMMUNICATION_TYPE_PACKING_BARCODE_UPLOAD:  //打包工序完成一个大包，收到大包标签
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_PRINTING_HEART_BEAT:  //收到打印扫描软件发送的心跳包
                            str = getBasicInfoRecordToString();
                            sendStringToClient(str, communicationType);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("processLabelPrintingFunc(" + communicationType + "," + packetLen + ") for printingMachineID = " + machineIDForPrint + "failed, " + ex);
                }
            }

            //send material info for all feeding machine one by one
            void sendMaterialInfoToPrintSW()
            {
                int i, j;
                string str;

                str = null;
                for (i = 0; i < NUM_OF_FEEDING_MACHINE; i++)
                {
                    for (j = 0; j < STACK_NUM_ONE_MACHINE; j++)
                    {
                        str += materialCodeForStack[i, j] + ";" + sackNumNeededForStack[i, j].ToString() + ";";
                    }
                }

                sendStringToClient(str, COMMUNICATION_TYPE_WAREHOUE_OUT_START);
            }


            void getMaterialInfoForAllMachines()
            {
                int i, j, k;
                int requiredNum;
                int numForOneSack;
                //int standardSackNumForStack; //number of sacks for a stack for a kind of material
                string dName;
                string ingredient;
                string today;
                string endDay;
                string commandText;
                string[,] tableArray;
                gVariable.dispatchSheetStruct[] dispatchList;

                today = DateTime.Now.Date.ToString("yyyy-MM-dd 08:00:00");
                endDay = DateTime.Now.Date.AddDays(2).ToString("yyyy-MM-dd 08:00:00");

                try
                {
                    for (i = 0; i < NUM_OF_FEEDING_MACHINE; i++)
                    {
                        //get dispatch list for this machine in defined period
                        dName = gVariable.DBHeadString + (i + 1).ToString().PadLeft(3, '0');
                        dispatchList = mySQLClass.getDispatchListInPeriodOfTime(dName, gVariable.dispatchListTableName, today, endDay, gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED,
                                                                                gVariable.TIME_CHECK_TYPE_PLANNED_START);

                        if (dispatchList == null)
                            continue;

                        ingredient = null;
                        for (j = 0; j < dispatchList.Length; j++)
                        {
                            //deal with one dispatch and its related material table, material informaion stored in tableArray
                            commandText = "select * from `" + gVariable.materialListTableName + "` where dispatchCode = '" + dispatchList[j].dispatchCode + "'";
                            tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                            //materialTypeNum = tableArray[j, mySQLClass.MATERIAL_LIST_NUM_OF_TYPE];

                            //the next dispatch uses different materials, we will consider it when current dispatch are all completed
                            if (ingredient != null && ingredient != tableArray[j, mySQLClass.BOM_CODE_IN_DISPATCHLIST_DATABASE])
                                break;

                            for (k = 0; k < gVariable.maxMaterialTypeNum; k++)
                            {
                                requiredNum = Convert.ToInt32(tableArray[j, mySQLClass.MATERIAL_LIST_MATERIAL_REQUIRED1 + k * mySQLClass.MATERIAL_LIST_CYCLE_SPAN]);
                                numForOneSack = Convert.ToInt32(tableArray[j, mySQLClass.MATERIAL_LIST_FULL_PACK_NUM1 + k * mySQLClass.MATERIAL_LIST_CYCLE_SPAN]);
                                sackNumTotalForDispatch[i, k] = requiredNum / numForOneSack;

                                //need one more sack
                                if (requiredNum % numForOneSack != 0)
                                    sackNumTotalForDispatch[i, k]++;

                                //how many sacks are needed for this stack
                                sackNumNeededForStack[i, k] += sackNumTotalForDispatch[i, k];

                                if (k == 0)
                                    sackNumNeededForStack[i, k] -= sackNumLeftInStack[i, k];

                                materialCodeForStack[i, k] = tableArray[j, mySQLClass.MATERIAL_LIST_MATERIAL_CODE1 + k * mySQLClass.MATERIAL_LIST_CYCLE_SPAN];
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("getMaterialInfoForAllMachines failed!" + ex);
                }
            }

            void dealWithMaterialOut()
            {
                int ret;

                ret = getMaterialInputOuput();
                if(ret == 0)
                    sackNumReceivedFromWarehouse[inoutFeedMachineID - 1, inoutMaterialStackID - 1] += inoutMaterialQuantity;
            }

            void dealWithMaterialIn()
            {
                int ret;

                ret = getMaterialInputOuput();
                if(ret == 0)
                    sackNumReceivedFromWarehouse[inoutFeedMachineID - 1, inoutMaterialStackID - 1] -= inoutMaterialQuantity;
            }


            int getMaterialInputOuput()
            {
                int len;
                string str;
                string[] strArray;

                len = onePacket[PROTOCOL_LEN_POS] + onePacket[PROTOCOL_LEN_POS + 1] * 0x100;
                str = System.Text.Encoding.GetEncoding("gb2312").GetString(onePacket, PROTOCOL_DATA_POS, len - MIN_PACKET_LEN_PURE_FRAME);
                strArray = str.Split(';');

                if (toolClass.isDigitalNum(strArray[1]) == 1)
                {
                    inoutFeedMachineID = Convert.ToInt32(strArray[1]);
                }
                else
                {
                    MessageBox.Show("打印扫描软件的出入库信息中，投料设备序号格式有误！", "提示信息", MessageBoxButtons.OK);
                    return -1;
                }

                if (toolClass.isDigitalNum(strArray[2]) == 1)
                {
                    inoutMaterialStackID = Convert.ToInt32(strArray[2]);
                }
                else
                {
                    MessageBox.Show("打印扫描软件的出入库信息中，料仓序号格式有误！", "提示信息", MessageBoxButtons.OK);
                    return -1;
                }

                if (toolClass.isDigitalNum(strArray[3]) == 1)
                {
                    inoutMaterialQuantity = Convert.ToInt32(strArray[3]);
                }
                else
                {
                    MessageBox.Show("打印扫描软件的出入库信息中，原料数量格式有误！", "提示信息", MessageBoxButtons.OK);
                    return -1;
                }

                //save to materialInOuputRecord table

                return 0;
            }

            void getDispatchByCodeAndSendToClient(string dName, int communicationType, string str)
            {
                int i;
                int len;
                string dispatchCode;
                string commandText;
                string[,] tableArray;
                gVariable.dispatchSheetStruct[] dispatchList;

                dispatchCode = str.Remove(DISPATCHCODE_LENGTH);
                commandText = "select * from `" + gVariable.dispatchListTableName + "` where dispatchCode = '" + dispatchCode + "'";
                dispatchList = mySQLClass.getDispatchListInternal(dName, gVariable.dispatchListTableName, commandText, 1);
                if (dispatchList == null)
                {
                    onePacket[PROTOCOL_DATA_POS] = 0xff;
                    len = MIN_PACKET_LEN;
                    sendDataToClient(onePacket, len, communicationType);
                }
                else
                {
                    commandText = "select * from `" + gVariable.dispatchListTableName + "` where dispatchcode = '" + dispatchList[0].dispatchCode + "'";
                    tableArray = mySQLClass.databaseCommonReading(dName, commandText);

                    if (str.Substring(24, 1) == "6")  //want a multi-product
                    {
                        tableArray[0, mySQLClass.PRODUCT_CODE2_IN_DISPATCHLIST_DATABASE] = "CPE4104";
                        tableArray[0, mySQLClass.PRODUCT_CODE3_IN_DISPATCHLIST_DATABASE] = "CPE4421";
                    }

                    str = null;
                    for (i = 1; i < tableArray.Length; i++)
                    {
                        str += tableArray[0, i] + ';';
                    }
                    sendStringToClient(str, communicationType);
                }
            }

            void sendDispatchToClient(string dName, int communicationType)
            {
                int i;
                int len;
                string str;
                string commandText;
                string[,] tableArray;
                gVariable.dispatchSheetStruct[] dispatchList;

                commandText = "select * from `" + gVariable.dispatchListTableName + "` where status = '0' order by id DESC";
                dispatchList = mySQLClass.getDispatchListInternal(dName, gVariable.dispatchListTableName, commandText, 1);
                if (dispatchList == null)
                {
                    onePacket[PROTOCOL_DATA_POS] = 0xff;
                    len = MIN_PACKET_LEN;
                    sendDataToClient(onePacket, len, communicationType);
                }
                else
                {
                    commandText = "select * from `" + gVariable.dispatchListTableName + "` where dispatchcode = '" + dispatchList[0].dispatchCode + "'";
                    tableArray = mySQLClass.databaseCommonReading(dName, commandText);

                    str = null;
                    for (i = 1; i < tableArray.Length; i++)
                    {
                        str += tableArray[0, i] + ';';
                    }
                    sendStringToClient(str, communicationType);
                }
            }

            string getBasicInfoRecordToString()
            {
                int i;
                string str;
                string commandText;
                string[,] tableArray;

                commandText = "select * from `" + gVariable.employeeTableName + "` where id = 3";
                tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                if(tableArray == null)
                {
                    return null;
                }
                else
                {
                    str = gVariable.basicInfoDatabaseName + ";" + gVariable.employeeTableName + ";3;";

                    tableArray[0, 4] = "66";
                    for (i = 1; i < tableArray.Length; i++)
                    {
                        str += tableArray[0, i] + ';';
                    }
                    return str;
                }
            }
        }
    }
}