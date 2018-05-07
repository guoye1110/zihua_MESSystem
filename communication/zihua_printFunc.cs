using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using MESSystem.alarmFun;
using MESSystem.common;

namespace MESSystem.communication
{
    public partial class communicate
    {
        public partial class ClientThread
        {
            public class zihua_printerClient
            {
                private const int NUM_OF_FEEDING_MACHINE = 7;//7条流水线，每条一个投料设备
                private const int STACK_NUM_ONE_MACHINE = 8;//每个偷料设备有8个料仓，每个料仓不同的原料

                //communication between PC host and label printing SW
                private const int COMMUNICATION_TYPE_HANDSHAKE_PRINT_MACHINE_ID = 3;
                private const int COMMUNICATION_TYPE_PRINTING_HEART_BEAT = 0xB3;
                //出入库工序
                private const int COMMUNICATION_TYPE_WAREHOUE_OUT_START = 0xB5;  //printing SW started and ask for material info, server send material info for all feeding machine to printing SW 
                private const int COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE = 0xB6;  //printing machine send barcode info to server whever a stack of material is moved out of the warehouse
                private const int COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE = 0xB7;  //printing machine send barcode info to server whever a stack of material is moved into the warehouse
                //流延工序
                private const int COMMUNICATION_TYPE_CAST_PROCESS_START = 0xB8;  //printing SW started cast process, server need to send dispatch info to printing SW
                private const int COMMUNICATION_TYPE_CAST_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xB9;  //printing SW send large roll info to server
                private const int COMMUNICATION_TYPE_CAST_PROCESS_END = 0xBA;
                //印刷工序
                private const int COMMUNICATION_TYPE_PRINT_PROCESS_START = 0xBB;
                private const int COMMUNICATION_TYPE_PRINT_PROCESS_MATERIAL_BARCODE_UPLOAD = 0xBC;
                private const int COMMUNICATION_TYPE_PRINT_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xBD;
                private const int COMMUNICATION_TYPE_PRINT_PROCESS_END = 0xBE;
                //分切工序
                private const int COMMUNICATION_TYPE_SLIT_PROCESS_START = 0xBF;
                private const int COMMUNICATION_TYPE_SLIT_PROCESS_MATERIAL_BARCODE_UPLOAD = 0xC0;
                private const int COMMUNICATION_TYPE_SLIT_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xC1;
                private const int COMMUNICATION_TYPE_SLIT_PROCESS_PACKAGE_BARCODE_UPLOAD = 0xC2;
                private const int COMMUNICATION_TYPE_SLIT_PROCESS_END = 0xC3;
                //质检工序
                private const int COMMUNICATION_TYPE_INSPECTION_PROCESS_MATERIAL_BARCODE_UPLOAD = 0xC4;
                private const int COMMUNICATION_TYPE_INSPECTION_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xC5;
                //再造料工序
                private const int COMMUNICATION_TYPE_REUSE_PROCESS_BARCODE_UPLOAD = 0xC6;
                //打包工序		
                private const int COMMUNICATION_TYPE_PACKING_PROCESS_PACKAGE_BARCODE_UPLOAD = 0xC7;
                //end of communication between PC host and label printing SW

                private const int DISPATCHCODE_LENGTH = 11;

                private const int WAREHOUSE_INPUT_OUTPUT_PC = 101;
                private const int CAST_PROCESS_PC1 = 141;
                private const int CAST_PROCESS_PC2 = 142;
                private const int CAST_PROCESS_PC3 = 143;
                private const int CAST_PROCESS_PC4 = 144;
                private const int CAST_PROCESS_PC5 = 145;
                private const int PRINT_PROCESS_PC1 = 161;
                private const int PRINT_PROCESS_PC2 = 162;
                private const int PRINT_PROCESS_PC3 = 163;
                private const int SLIT_PROCESS_PC1 = 181;
                private const int SLIT_PROCESS_PC2 = 181;
                private const int SLIT_PROCESS_PC3 = 181;
                private const int SLIT_PROCESS_PC4 = 181;
                private const int SLIT_PROCESS_PC5 = 181;
                private const int INSPECTION_PROCESS_PC1 = 201;
                private const int REBUILD_PROCESS_PC1 = 221;
                private const int PACKING_PROCESS_PC1 = 241;

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

                private ClientThread m_ClientThread = null;
                private int m_machineIDForPrint;
				private string m_Operator;
				private int index;

                public zihua_printerClient(ClientThread cThread)
                {
                    m_ClientThread = cThread;
                }

                private void initVariables()
                {
                    m_ClientThread.handshakeWithClientOK = 1;

                    inoutMaterialQuantity = 0;
                    inoutFeedMachineID = 0;
                    inoutMaterialStackID = 0;

                    for (int i = 0; i < NUM_OF_FEEDING_MACHINE; i++)
                    {
                        for (int j = 0; j < STACK_NUM_ONE_MACHINE; j++)
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

                private void getMaterialInfoForAllMachines()
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
                            int[] lefts = mySQLClass.getFeedCurrentLeft(i, STACK_NUM_ONE_MACHINE);
                            for (int index = 0; index < lefts.Length; index++)
                                sackNumLeftInStack[i, index] = lefts[index];

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
                                tableArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
                                //materialTypeNum = tableArray[j, mySQLClass.MATERIAL_LIST_NUM_OF_TYPE];

                                //the next dispatch uses different materials, we will consider it when current dispatch are all completed
                                if (ingredient != null && ingredient != tableArray[0, mySQLClass.BOM_CODE_IN_DISPATCHLIST_DATABASE])
                                    break;
                                ingredient = tableArray[0, mySQLClass.BOM_CODE_IN_DISPATCHLIST_DATABASE];

                                for (k = 0; k < gVariable.maxMaterialTypeNum; k++)
                                {
                                    requiredNum = Convert.ToInt32(tableArray[0, mySQLClass.MATERIAL_LIST_MATERIAL_REQUIRED1 + k * mySQLClass.MATERIAL_LIST_CYCLE_SPAN]);
                                    numForOneSack = Convert.ToInt32(tableArray[0, mySQLClass.MATERIAL_LIST_FULL_PACK_NUM1 + k * mySQLClass.MATERIAL_LIST_CYCLE_SPAN]);
                                    sackNumTotalForDispatch[i, k] = requiredNum / numForOneSack;

                                    //need one more sack
                                    if (requiredNum % numForOneSack != 0)
                                        sackNumTotalForDispatch[i, k]++;

                                    //how many sacks are needed for this stack
                                    sackNumNeededForStack[i, k] += sackNumTotalForDispatch[i, k];

                                    if (k == 0)
                                        sackNumNeededForStack[i, k] -= sackNumLeftInStack[i, k];

                                    materialCodeForStack[i, k] = tableArray[0, mySQLClass.MATERIAL_LIST_MATERIAL_CODE1 + k * mySQLClass.MATERIAL_LIST_CYCLE_SPAN];
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("getMaterialInfoForAllMachines failed!" + ex);
                    }
                }

                private void sendMaterialInfoToPrintSW()
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

                    m_ClientThread.sendStringToClient(str, COMMUNICATION_TYPE_WAREHOUE_OUT_START);
                }

                private int setMaterialWareInOut(int communicationType, byte[] onePacket, int packetLen)
                {
                    int len;
                    string strInput;
					materialdeliverylistDB db = new materialdeliverylistDB();
                    materialdeliverylistDB.materialdelivery_t materialdelivery;

					//MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                    len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;
                    strInput = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);

					materialdelivery.materialCode = strInput[0];
					materialdelivery.materialBatchNum = strInput[1];
					materialdelivery.targetMachine = strInput[2];
					materialdelivery.targetFeedBinIndex = strInput[3];
					materialdelivery.inoutputQuantity = strInput[4];
					materialdelivery.direction = (communicationType&COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE) + 1;//1: 出库 2：入库
					materialdelivery.inoutputTime = System.DateTime.Now.ToString();
					materialdelivery.deliveryWorker = m_Operator;
					
                    return db.writerecord(materialdelivery);
                }


                /*void sendDispatchCodeToClient(int communicationType, byte[] onePacket, int len)
                {
                    string str;
                    dispatchlistDB dispatchlist_db;
                    dispatchlistDB.dispatchlist_t[] st_dispatchlist;

                    int printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - CAST_PROCESS_PC1 + gVariable.castingProcess[0];

                    dispatchlist_db = new dispatchlistDB(printingSWPCID);

                    st_dispatchlist = dispatchlist_db.readallrecordOrdered();

                    for (int i=0;i<st_dispatchlist.Length;i++){
                        if (st_dispatchlist[i].status == gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED){
                            str = st_dispatchlist[i].dispatchCode + ";" + st_dispatchlist[i].productCode;
                            m_ClientThread.sendStringToClient(str, communicationType);
                            return;
                        }
                    }
                    onePacket[PROTOCOL_DATA_POS] = 0xff;
                    len = MIN_PACKET_LEN;
                    m_ClientThread.sendDataToClient(onePacket, len, communicationType);
                }*/

                /*public static string sendDispatchCodeToClient(byte[] onePacket, int len)
                {
                    string str;
                    dispatchlistDB dispatchlist_db;
                    dispatchlistDB.dispatchlist_t[] st_dispatchArray;

                    int printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - CAST_PROCESS_PC1 + gVariable.castingProcess[0];
			
                    dispatchlist_db = new dispatchlistDB(printingSWPCID);

                    st_dispatchArray = dispatchlist_db.readallrecord_Ordered();
                    if (st_dispatchArray != null){
                        for (int i=0;i<st_dispatchArray.Length;i++){
                            //Find first published dispatch
                            if (st_dispatchArray[i].status == gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED) {
                                str = st_dispatchArray[i].dispatchCode + ";" + st_dispatchArray[i].productCode;
                                m_ClientThread.sendStringToClient(str, communicationType);
                                return st_dispatchArray[i].dispatchCode;
                            }
                        }
                    }
                    onePacket[PROTOCOL_DATA_POS] = 0xff;
                    len = MIN_PACKET_LEN;
                    m_ClientThread.sendDataToClient(onePacket, len, communicationType);
                    return null;
                }*/

                //return value: -1: no action
                //				 0: OK
                //				>0: Fail
                private int HandleHandShake(int communicationType, byte[] onePacket, int packetLen)
                {
                    m_machineIDForPrint = onePacket[PROTOCOL_DATA_POS];
                    onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                    return m_ClientThread.sendDataToClient(onePacket, MIN_PACKET_LEN, communicationType);
                }

                //return value: -1: no action
                //				 0: OK
                //				>0: Fail
                private int HandleMaterialProcess(int communicationType, byte[] onePacket, int packetLen)
                {
                    int result;

                    if (communicationType == COMMUNICATION_TYPE_WAREHOUE_OUT_START)
                    {
                        getMaterialInfoForAllMachines();
                        return sendMaterialInfoToPrintSW();
                    }
                    if (communicationType == COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE || communicationType == COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE)
                    {
                        result = setMaterialWareInOut(communicationType, onePacket, packetLen);
                        return m_ClientThread.sendResponseOKBack(result);
                    }
                    return -1;
                }

                //return value: -1: no action
                //				 0: OK
                //				>0: Fail
                private int HandleProcessStart(int communicationType, byte[] onePacket, int packetLen, ref string dispatchCode, ref string productCode)
                {
                    dispatchlistDB.dispatchlist_t st_dispatchArray;
                    string operatorID;

                    //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                    int len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;
                    operatorID = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);

                    int printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - CAST_PROCESS_PC1 + gVariable.castingProcess[0];

                    if (communicationType == COMMUNICATION_TYPE_CAST_PROCESS_START || communicationType == COMMUNICATION_TYPE_PRINT_PROCESS_START || communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_START)
                    {
                        dispatchlistDB dispatchlist_db = new dispatchlistDB(printingSWPCID);

                        st_dispatchArray = dispatchlist_db.readallrecord_Ordered();
                        if (st_dispatchArray != null)
                        {
                            for (int i = 0; i < st_dispatchArray.Length; i++)
                            {
                                //Find first published dispatch
                                if (st_dispatchArray[i].status == gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED)
                                {
                                    dispatchCode = st_dispatchArray[i].dispatchCode;
                                    productCode = st_dispatchArray[i].productCode;
                                    //Save operator to it 
                                    dispatchlistDB.dispatchlist_t st;
                                    st.operatorName = operatorID;
                                    return dispatchlist_db.updaterecord_ByDispatchcode(dispatchlist_db.Serialize(st), dispatchCode);
                                }
                            }
                        }
                        return 1;
                    }
                    return -1;
                }

                //return value: -1: no action
                //				 0: OK
                //				>0: Fail
                private int HandleBarcode(int communicationType, byte[] onePacket, int packetLen)
                {
                    int len;
                    string strInput;
                    string[] strInputArray;

                    //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                    len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;
                    strInput = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                    strInputArray = strInput.Split(";");

                    //流延工序
                    if (communicationType == COMMUNICATION_TYPE_CAST_PROCESS_PRODUCT_BARCODE_UPLOAD)
                    {
                        productcastinglistDB.productcast_t st_cast = { 0 };
                        productcastinglistDB db = new productcastinglistDB();

                        if (strInputArray.Length != 2) return -1;

                        st_cast.dispatchCode = strInputArray[0].Substring(0, 12);
                        st_cast.barCode = strInputArray[0];
                        st_cast.batchNum = st_cast.dispatchCode.Substring(0, 7);
                        st_cast.largeIndex = strInputArray[0].Substring(16, 3);
                        st_cast.machineID = m_machineIDForPrint.ToString();
                        //st_cast.scanTime = st_cast.dispatchCode.Substring(0,4) + st_cast.dispatchCode.Substring(7,2) + strInputArray[0].Substring(12,4);
                        st_cast.scanTime = System.DateTime.Now.ToString();
                        st_cast.weight = strInputArray[1];
                        return db.writerecord(st_cast);
                    }

                    //印刷工序
                    if (communicationType == COMMUNICATION_TYPE_PRINT_PROCESS_MATERIAL_BARCODE_UPLOAD)
                    {
                        productprintlistDB.productprint_t st_print = { 0 };
                        productprintlistDB db = new productprintlistDB();

                        st_print.materialScanTime = System.DateTime.Now.ToString();
                        st_print.materialBarCode = strInputArray[0];
                        st_print.largeIndex = strInputArray[0].Substring(16, 3);
                        st_print.machineID = m_machineIDForPrint.ToString();

                        return db.writerecord(st_print);
                    }
                    if (communicationType == COMMUNICATION_TYPE_PRINT_PROCESS_PRODUCT_BARCODE_UPLOAD)
                    {
                        productprintlistDB.productprint_t st_print;
                        productprintlistDB db = new productcastinglistDB();

                        st_print.dispatchCode = strInputArray[1].Substring(0, 12);
                        st_print.batchNum = st_print.dispatchCode.Substring(0, 7);
                        st_print.productScanTime = System.DateTime.Now.ToString();
                        st_print.productBarCode = strInputArray[1];
                        st_print.weight = strInputArray[2];

                        return db.updaterecord_ByMaterialBarCode(db.Serialize(st_print), strInputArray[0]);
                    }
                    //分切工序
                    if (communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_MATERIAL_BARCODE_UPLOAD)
                    {
                        productslitlistDB.productslit_t st_slit = { 0 };
                        productslitlistDB db = new productslitlistDB();

                        st_slit.materialScanTime = System.DateTime.Now.ToString();
                        st_slit.materialBarCode = strInputArray[0];
                        st_slit.largeIndex = strInputArray[0].Substring(16, 3);
                        st_slit.machineID = m_machineIDForPrint.ToString();

                        return db.writerecord(st_slit);
                    }
                    if (communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_PRODUCT_BARCODE_UPLOAD)
                    {
                        productslitlistDB.productslit_t st_slit;
                        productslitlistDB db = new productslitlistDB();

                        st_slit.dispatchCode = strInputArray[1].Substring(0, 12);
                        st_slit.batchNum = st_slit.dispatchCode.Substring(0, 7);
                        st_slit.productScanTime = System.DateTime.Now.ToString();
                        st_slit.productBarCode = strInputArray[1];
                        st_slit.smallIndex = strInputArray[1].Substring(19, 2);
                        st_slit.customerIndex = strInputArray[1].Substring(21, 1);
                        st_slit.errorStatus = strInputArray[1].Substring(22, 1);
                        st_slit.numOfJoins = strInputArray[3];
                        st_slit.weight = strInputArray[2];

                        return db.updaterecord_ByMaterialBarCode(db.Serialize(st_slit), strInputArray[0]);
                    }
                    //质检工序
                    if (communicationType == COMMUNICATION_TYPE_INSPECTION_PROCESS_MATERIAL_BARCODE_UPLOAD)
                    {
                        inspectionlistDB.inspection_t st_inspect = { 0 };
                        inspectionlistDB db = new inspectionlistDB();

                        st_inspect.materialBarCode = strInputArray[0];
                        st_inspect.materialScanTime = System.DateTime.Now.ToString();

                        return db.writerecord(st_inspect);
                    }
                    if (communicationType == COMMUNICATION_TYPE_INSPECTION_PROCESS_PRODUCT_BARCODE_UPLOAD)
                    {
                        inspectionlistDB.inspection_t st_inspect;
                        inspectionlistDB db = new inspectionlistDB();

                        st_inspect.dispatchCode = strInputArray[1].Substring(0, 12);
                        st_inspect.batchNum = st_inspect.dispatchCode.Substring(0, 7);
                        st_inspect.productBarCode = strInputArray[1];
                        st_inspect.productScanTime = System.DateTime.Now.ToString();
                        st_inspect.checkingResult = strInputArray[1].Substring(strInputArray[1].Lenght - 1, 1);
                        st_inspect.inspector = strInputArray[2];

                        return db.updaterecord_ByMaterialBarCode(db.Serialize(st_inspect), strInputArray[0]);
                    }
                    //再造料工序
                    if (communicationType == COMMUNICATION_TYPE_REUSE_PROCESS_BARCODE_UPLOAD)
                    {
                        reusematerialDB.reusematerial_t st_reuse;
                        reusematerialDB db = new reusematerialDB();

                        st_reuse.barcodeForReuse = strInputArray[0];
                        st_reuse.rebuildDate = System.DateTime.Now.ToString();
                        st_reuse.BOMCode = strInputArray[12];
                        st_reuse.rebuildNum = strInputArray[1];
                        st_reuse.workerID = strInputArray[13];
                        st_reuse.barCode1 = strInputArray[2];
                        st_reuse.barCode2 = strInputArray[3];
                        st_reuse.barCode3 = strInputArray[4];
                        st_reuse.barCode4 = strInputArray[5];
                        st_reuse.barCode5 = strInputArray[6];
                        st_reuse.barCode6 = strInputArray[7];
                        st_reuse.barCode7 = strInputArray[8];
                        st_reuse.barCode8 = strInputArray[9];
                        st_reuse.barCode9 = strInputArray[10];
                        st_reuse.barCode10 = strInputArray[11];

                        return db.writerecord(st_reuse);
                    }
                    return -1;
                }

                //return value: -1: no action
                //				 0: OK
                //				>0: Fail
                private int HandleProcessEnd(int communicationType, byte[] onePacket, int packetLen)
                {
                    int len;
                    string strInput;
                    string[] strInputArray;

                    int printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - CAST_PROCESS_PC1 + gVariable.castingProcess[0];

                    //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                    len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;
                    strInput = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                    strInputArray = strInput.Split(';');

                    if (communicationType == COMMUNICATION_TYPE_CAST_PROCESS_END || communicationType == COMMUNICATION_TYPE_PRINT_PROCESS_END || communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_END)
                    {
                        dispatchlistDB db = new dispatchlistDB(printingSWPCID);
                        dispatchlistDB.dispatchlist_t st_dispatch;

                        st_dispatch.notes = strInputArray[1];
                        return dispatchlistDB.updaterecord_ByDispatchcode(db.Serialize(st_dispatch), strInputArray[0]);
                    }
                    return -1;
                }

                public void processLabelPrintingFunc(int communicationType, byte[] onePacket, int packetLen)
                {
                    int result;
                    string dispatchCode, productCode;
                    int printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - CAST_PROCESS_PC1 + gVariable.castingProcess[0];

                    result = HandleHandShake(communicationType, onePacket, packetLen);
                    if (result > 0) m_ClientThread.sendResponseOKBack(result);

                    result = HandleProcessStart(communicationType, onePacket, packetLen, ref dispatchCode, ref productCode);
                    if (result > 0) m_ClientThread.sendResponseOKBack(result);
                    else if (!result)
                    {
                        string str = dispatchCode + ";" + productCode;
                        m_ClientThread.sendStringToClient(str, communicationType);
                    }

                    result = HandleMaterialBarcode(communicationType, onePacket, packetLen);
                    if (result > 0) m_ClientThread.sendResponseOKBack(result);

                    result = HandleProductBarcode(communicationType, onePacket, packetLen);
                    if (result > 0) m_ClientThread.sendResponseOKBack(result);

                    result = HandleProcessEnd(communicationType, onePacket, packetLen);

                    try
                    {
                        switch (communicationType)
                        {
                            //握手信息
                            /*case COMMUNICATION_TYPE_HANDSHAKE_PRINT_MACHINE_ID:
                                m_machineIDForPrint = onePacket[PROTOCOL_DATA_POS];
                                onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                                m_ClientThread.sendDataToClient(onePacket, MIN_PACKET_LEN, communicationType);
                                break;*/

                            /*--------------------------------------------------------------------------------------------
                            原料出库区的搬运工获取物料清单，hXX.dispatchList，根据物料清单中每个料仓需要的物料重量
                            （KG）确定并准备相应的码垛，一码垛对于一个料仓，一个料仓可能有多个码垛（取决于该工单的
                            BOM），将该信息输入并产生扫描标签，贴在每个码垛上，上传server（含条码），存储在
                            globaldatabase.materialinoutrecord；	当该码垛有多余的料要入库，			PC上选择入库，然后扫码垛上的条码，
                            本地根据条码得到该物料信息，然后选择入库，				并上传server（含条码）
                            ---------------------------------------------------------------------------------------------*/
                            case COMMUNICATION_TYPE_WAREHOUE_OUT_START:
                                getMaterialInfoForAllMachines();
                                sendMaterialInfoToPrintSW();
                                break;
                            case COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE:
                            case COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE:
                                result = setMaterialWareInOut(onePacket, packetLen);
                                m_ClientThread.sendResponseOKBack(result);
                                break;

                            /*-----------------------------------------------------------------------------------------------
                            //每一班流延设备工人上班后（本地PC会显示本地记录的上班次的情况），工单在每个工人下班后
                            //就结束了，此时机器还在运行，一个标准大卷还未完成，该卷算在下个班次。从start，生产出的大卷标签上
                            //传，到下班填上交接班记录，该工单在数据库中，hXX.O_dispatchList就完整的封闭了（开始时间，结束时间，交接备注等）。
                            -------------------------------------------------------------------------------------------------*/
                            case COMMUNICATION_TYPE_CAST_PROCESS_START:
                                string dispatchCode = sendDispatchCodeToClient(COMMUNICATION_TYPE_CAST_PROCESS_START, onePacket, packetLen);
                                setDispatchOperator(printingSWPCID, dispatchCode, onePacket, packetLen);
                                break;
                            case COMMUNICATION_TYPE_CAST_PROCESS_PRODUCT_BARCODE_UPLOAD:
                                result = setCastBarcode(onePacket, packetLen);
                                m_ClientThread.sendResponseOKBack(result);
                                break;
                            case COMMUNICATION_TYPE_CAST_PROCESS_END:
                                string dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
                                result = setDispatchNotes(printingSWPCID, onePacket, packetLen);
                                m_ClientThread.sendResponseOKBack(result);
                                break;

                            /*-----------------------------------------------------------------------------------------------
                            印刷工序和流延工序基本相同，工单，数据库不同而已
                            -------------------------------------------------------------------------------------------------*/
                            case COMMUNICATION_TYPE_PRINT_PROCESS_START:
                                string dispatchCode = sendDispatchCodeToClient(COMMUNICATION_TYPE_PRINT_PROCESS_START, onePacket, packetLen);
                                setDispatchOperator(printingSWPCID, dispatchCode, onePacket, packetLen);
                                break;
                            case COMMUNICATION_TYPE_PRINT_PROCESS_MATERIAL_BARCODE_UPLOAD:
                                result = setBarcode(COMMUNICATION_TYPE_PRINT_PROCESS_MATERIAL_BARCODE_UPLOAD, onePacket, packetLen);
                                m_ClientThread.sendResponseOKBack(result);
                                break;
                            case COMMUNICATION_TYPE_PRINT_PROCESS_PRODUCT_BARCODE_UPLOAD:
                                result = setProductBarcode(COMMUNICATION_TYPE_PRINT_PROCESS_PRODUCT_BARCODE_UPLOAD, onePacket, packetLen);
                                m_ClientThread.sendResponseOKBack(result);
                                break;
                            case COMMUNICATION_TYPE_PRINT_PROCESS_END:
                                string dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
                                result = setDispatchFinished(dName, onePacket, packetLen);
                                m_ClientThread.sendResponseOKBack(result);
                                break;

                            /*-----------------------------------------------------------------------------------------------
                            分切工序基本相同，工单，数据库不同而已
                            -------------------------------------------------------------------------------------------------*/
                            case COMMUNICATION_TYPE_SLIT_PROCESS_START:  //分切工上班了
                                string dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
                                sendDispatchToClient(dName, COMMUNICATION_TYPE_SLIT_PROCESS_START, onePacket, packetLen);
                                break;
                            case COMMUNICATION_TYPE_SLIT_PROCESS_MATERIAL_BARCODE_UPLOAD:
                                break;
                            case COMMUNICATION_TYPE_SLIT_PROCESS_PRODUCT_BARCODE_UPLOAD:
                                break;
                            case COMMUNICATION_TYPE_SLIT_PROCESS_PACKAGE_BARCODE_UPLOAD:
                                break;
                            case COMMUNICATION_TYPE_SLIT_PROCESS_END:
                                break;

                            case COMMUNICATION_TYPE_INSPECTION_PROCESS_START:  //质检开始
                                break;
                            case COMMUNICATION_TYPE_INSPECTION_BARCODE_UPLOAD:  //质检结果上传，收到质检标签
                                break;
                            case COMMUNICATION_TYPE_REUSE_PROCESS_START:  //再造料开工
                                break;
                            case COMMUNICATION_TYPE_REUSE_BARCODE_UPLOAD:  //收到再造料使用的小卷标签
                                break;
                            case COMMUNICATION_TYPE_PACKING_PROCESS_START: //打包工开工
                                break;
                            case COMMUNICATION_TYPE_PACKING_BARCODE_UPLOAD:  //打包工序完成一个大包，收到大包标签
                                break;
                            case COMMUNICATION_TYPE_PRINTING_HEART_BEAT:  //收到打印扫描软件发送的心跳包
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("processLabelPrintingFunc(" + communicationType + "," + packetLen + ") for printingMachineID = " + machineIDForPrint + "failed, " + ex);
                    }
                }
            }
        }
    }
}
	

