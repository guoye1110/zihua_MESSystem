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
        public const int NUM_OF_FEEDING_MACHINE = 7;
        public const int STACK_NUM_ONE_MACHINE = 7;

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
            //int machineIDForPrint;
            //material code for stacks(����Ӧ��ԭ�ϱ��)
            string[,] materialCodeForStack = new string[NUM_OF_FEEDING_MACHINE, STACK_NUM_ONE_MACHINE]; 
            //total sack num for this dispatch, got from dispatch info��������ԭ������Ĵ�����
            int[,] sackNumTotalForDispatch = new int[NUM_OF_FEEDING_MACHINE, STACK_NUM_ONE_MACHINE];
            //sack num received from warehouse(���˹��Ѿ������˼���ԭ��, ����ӦС�ڵ��� sackNumNeededForStack[])
            int[,] sackNumReceivedFromWarehouse = new int[NUM_OF_FEEDING_MACHINE, STACK_NUM_ONE_MACHINE];
            //total sack num needed for this stack(or feed bin) for multiple dispatches in a period of time(���ڰ��˹����ԣ�ĳ���������Ҫ����ԭ�ϣ����ܰ���������������ܳ��������������������)
            int[,] sackNumNeededForStack = new int[NUM_OF_FEEDING_MACHINE, STACK_NUM_ONE_MACHINE];
            //sack num left in the stack for the current time������е�ʣ�������
            int[,] sackNumLeftInStack = new int[NUM_OF_FEEDING_MACHINE, STACK_NUM_ONE_MACHINE];
            //material left in feed bin(�����е����Ϲ�����)
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
                /*
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
                        case COMMUNICATION_TYPE_WAREHOUE_OUT_START:  //ԭ�ϳ������İ��˹��ϰ���
                            getMaterialInfoForAllMachines();
                            sendMaterialInfoToPrintSW();
                            break;
                        case COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE:  //һ������ԭ�ϳ�����, �յ����������ǩ
                            dealWithMaterialOut();
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE:  //һ���������ϻ��������, �յ���������ǩ
                            dealWithMaterialIn();
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_CAST_PROCESS_START:  //�����豸�����ϰ���
                            printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - CAST_PROCESS_PC1 + gVariable.castingProcess[0];
                            dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');

                            sendDispatchToClient(dName, communicationType);
                            break;
                        case COMMUNICATION_TYPE_CAST_BARCODE_UPLOAD: //���һ������յ�������ӱ�ǩ
                            len = onePacket[PROTOCOL_LEN_POS] + onePacket[PROTOCOL_LEN_POS + 1] * 0x100 - MIN_PACKET_LEN_PURE_FRAME;

                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_PRINT_PROCESS_START:  //ӡˢ�豸�����ϰ���
                            printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - PRINT_PROCESS_PC1 + gVariable.printingProcess[0];
                            dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
                            sendDispatchToClient(dName, communicationType);
                            break;
                        case COMMUNICATION_TYPE_PRINT_BARCODE_UPLOAD:  //���һ������յ����ӡˢ��ǩ
                            len = onePacket[PROTOCOL_LEN_POS] + onePacket[PROTOCOL_LEN_POS + 1] * 0x100 - MIN_PACKET_LEN_PURE_FRAME;

                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_SLIT_PROCESS_START:  //���й��ϰ���
                            //get string sent from client
                            len = onePacket[PROTOCOL_LEN_POS] + onePacket[PROTOCOL_LEN_POS + 1] * 0x100;
                            //there are 4 extra bytes indicating which slit machine we are using, so we need to minus 4
                            str = System.Text.Encoding.GetEncoding("gb2312").GetString(onePacket, PROTOCOL_DATA_POS + 4, len - MIN_PACKET_LEN_PURE_FRAME - 4);

                            printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - SLIT_PROCESS_PC1 + gVariable.slittingProcess[0];
                            dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');

                            getDispatchByCodeAndSendToClient(dName, communicationType, str);
                            break;
                        case COMMUNICATION_TYPE_SLIT_BARCODE_UPLOAD:  //����һ��С���깤���յ�С����б�ǩ
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_INSPECTION_PROCESS_START:  //�ʼ쿪ʼ
                            //get slit machine id from onepacket
                            printingSWPCID = onePacket[PROTOCOL_DATA_POS + DISPATCHCODE_LENGTH + 1] - 0x31 + gVariable.slittingProcess[0];
                            dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
                            sendDispatchToClient(dName, communicationType);
                            break;
                        case COMMUNICATION_TYPE_INSPECTION_BARCODE_UPLOAD:  //�ʼ����ϴ����յ��ʼ��ǩ
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_REUSE_PROCESS_START:  //�����Ͽ���
                            printingSWPCID = onePacket[PROTOCOL_DATA_POS + DISPATCHCODE_LENGTH + 1] - 0x31 + gVariable.slittingProcess[0];
                            dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
                            sendDispatchToClient(dName, communicationType);
                            break;
                        case COMMUNICATION_TYPE_REUSE_BARCODE_UPLOAD:  //�յ�������ʹ�õ�С���ǩ
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_PACKING_PROCESS_START: //���������
                            printingSWPCID = onePacket[PROTOCOL_DATA_POS + DISPATCHCODE_LENGTH + 1] - 0x31 + gVariable.slittingProcess[0];
                            dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
                            sendDispatchToClient(dName, communicationType);
                            break;
                        case COMMUNICATION_TYPE_PACKING_BARCODE_UPLOAD:  //����������һ��������յ������ǩ
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_PRINTING_HEART_BEAT:  //�յ���ӡɨ��������͵�������
                            str = getBasicInfoRecordToString();
                            sendStringToClient(str, communicationType);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("processLabelPrintingFunc(" + communicationType + "," + packetLen + ") for printingMachineID = " + machineIDForPrint + "failed, " + ex);
                }
                */
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

                //sendStringToClient(str, COMMUNICATION_TYPE_WAREHOUE_OUT_START);
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
                    MessageBox.Show("��ӡɨ������ĳ������Ϣ�У�Ͷ���豸��Ÿ�ʽ����", "��ʾ��Ϣ", MessageBoxButtons.OK);
                    return -1;
                }

                if (toolClass.isDigitalNum(strArray[2]) == 1)
                {
                    inoutMaterialStackID = Convert.ToInt32(strArray[2]);
                }
                else
                {
                    MessageBox.Show("��ӡɨ������ĳ������Ϣ�У��ϲ���Ÿ�ʽ����", "��ʾ��Ϣ", MessageBoxButtons.OK);
                    return -1;
                }

                if (toolClass.isDigitalNum(strArray[3]) == 1)
                {
                    inoutMaterialQuantity = Convert.ToInt32(strArray[3]);
                }
                else
                {
                    MessageBox.Show("��ӡɨ������ĳ������Ϣ�У�ԭ��������ʽ����", "��ʾ��Ϣ", MessageBoxButtons.OK);
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