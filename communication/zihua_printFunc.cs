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
                private const int STACK_NUM_ONE_MACHINE = 7;//每个偷料设备有8个料仓，每个料仓不同的原料

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

                private ClientThread m_ClientThread = null;
                private int m_machineIDForPrint;
				private string m_Operator;

                public zihua_printerClient(ClientThread cThread)
                {
                    m_ClientThread = cThread;
					m_machineIDForPrint = 0;
					m_Operator = null;
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
					float[,] weightRequiredOfEachStack = new float[NUM_OF_FEEDING_MACHINE, STACK_NUM_ONE_MACHINE];
					int[,] sackRequiredOfEachStack = new int[NUM_OF_FEEDING_MACHINE, STACK_NUM_ONE_MACHINE];
					string[,] materialCodeOfEachStack = new string[NUM_OF_FEEDING_MACHINE, STACK_NUM_ONE_MACHINE];
                    gVariable.dispatchSheetStruct[] dispatchs;

                    today = DateTime.Now.Date.ToString("yyyy-MM-dd 08:00:00");
                    endDay = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd 08:00:00");

                    try
                    {
                        for (i = 0; i < communicate.NUM_OF_FEEDING_MACHINE; i++)
                        {
                            //get dispatch list for this machine in defined period
                            dName = gVariable.DBHeadString + (i + 1).ToString().PadLeft(3, '0');

                            dispatchs = mySQLClass.getDispatchListInPeriodOfTime(dName, gVariable.dispatchListTableName, today, endDay, gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED,
                                                                                    gVariable.TIME_CHECK_TYPE_PLANNED_START);

                            if (dispatchs == null)
                                continue;

							materiallistDB db_list = new materiallistDB(i+1);

							//首先计算所有工单每个料仓所需的总重量，方法是从0_materiallist中根据工单号查找到相应的物料需求
							//然后按每个料仓需要的重量累计，保存到weightRequiredOfEachStack。这里唯一的变数是dispatch的BOMCode
							//变了的话，就停止累计。
                            ingredient = null;
                            for (j = 0; j < dispatchs.Length; j++)
                            {
                                //deal with one dispatch and its related material table, material informaion stored in tableArray
                                //string dbName = gVariable.DBHeadString + (j+1).ToString().PadLeft(3, '0');
                                materiallistDB.material_t? materials;

								materials = db_list.readrecord_byDispatchCode(dispatchs[j].dispatchCode);

                                //the next dispatch uses different materials, we will consider it when current dispatch are all completed
                                if (ingredient != null && ingredient != dispatchs[j].BOMCode)
                                    break;
                                ingredient = dispatchs[j].BOMCode;

								materialCodeOfEachStack[i,1] = materials.Value.materialCode1;
								materialCodeOfEachStack[i,2] = materials.Value.materialCode2;
								materialCodeOfEachStack[i,3] = materials.Value.materialCode3;
								materialCodeOfEachStack[i,4] = materials.Value.materialCode4;
								materialCodeOfEachStack[i,5] = materials.Value.materialCode5;
								materialCodeOfEachStack[i,6] = materials.Value.materialCode6;
								materialCodeOfEachStack[i,7] = materials.Value.materialCode7;

								weightRequiredOfEachStack[i,1] += materials.Value.materialRequired1;
								weightRequiredOfEachStack[i,2] += materials.Value.materialRequired2;
								weightRequiredOfEachStack[i,3] += materials.Value.materialRequired3;
								weightRequiredOfEachStack[i,4] += materials.Value.materialRequired4;
								weightRequiredOfEachStack[i,5] += materials.Value.materialRequired5;
								weightRequiredOfEachStack[i,6] += materials.Value.materialRequired6;
								weightRequiredOfEachStack[i,7] += materials.Value.materialRequired7;
                            }
                        }
				
						string str;
						str = null;
						for (i = 1; i < NUM_OF_FEEDING_MACHINE+1; i++)
							for (j = 1; j < STACK_NUM_ONE_MACHINE+1; j++)
								str += materialCodeOfEachStack[i, j] + ";" + weightRequiredOfEachStack[i, j].ToString() + ";";
						
						m_ClientThread.sendStringToClient(str, COMMUNICATION_TYPE_WAREHOUE_OUT_START);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("getMaterialInfoForAllMachines failed!" + ex);
                    }
                }

                private int setMaterialWareInOut(int communicationType, byte[] onePacket, int packetLen)
                {
                    int len;
                    string strInput;
                    string[] strInputArray;
					materialdeliverylistDB db = new materialdeliverylistDB();
                    materialdeliverylistDB.materialdelivery_t materialdelivery;

					//MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                    len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;
                    strInput = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                    strInputArray = strInput.Split(';');

                    materialdelivery.materialCode = strInputArray[0];
                    materialdelivery.materialBatchNum = strInputArray[1];
                    materialdelivery.targetMachine = strInputArray[2];
                    materialdelivery.targetFeedBinIndex = strInputArray[3];
                    materialdelivery.inoutputQuantity = strInputArray[4];
					materialdelivery.direction = ((communicationType&COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE) + 1).ToString();//1: 出库 2：入库
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
                	if (communicationType == COMMUNICATION_TYPE_HANDSHAKE_PRINT_MACHINE_ID){
                    	m_machineIDForPrint = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100;
						m_ClientThread.handshakeWithClientOK = 1;
                    	return 0;
                	}
					return -1;
                }

                //return value: -1: no action
                //				 0: OK
                //				>0: Fail
                private int HandleMaterialProcess(int communicationType, byte[] onePacket, int packetLen)
                {
                    string operatorID;

                    //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                    int len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;

                    if (communicationType == COMMUNICATION_TYPE_WAREHOUE_OUT_START)
                    {
	                    m_Operator = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                        getMaterialInfoForAllMachines();
                        return -1;
                    }
                    if (communicationType == COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE || communicationType == COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE)
                    {
                        return setMaterialWareInOut(communicationType, onePacket, packetLen);
                    }
                    return -1;
                }

                //return value: -1: no action
                //				 0: OK
                //				>0: Fail
                private int HandleProcessStart(int communicationType, byte[] onePacket, int packetLen, ref string dispatchCode, ref string productCode)
                {
                    dispatchlistDB.dispatchlist_t[] st_dispatchArray;

                    //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                    int len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;

                    if (communicationType == COMMUNICATION_TYPE_CAST_PROCESS_START || communicationType == COMMUNICATION_TYPE_PRINT_PROCESS_START || communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_START)
                    {
                        dispatchlistDB dispatchlist_db = new dispatchlistDB(m_machineIDForPrint-CAST_PROCESS_PC1+gVariable.castingProcess[0]);

						m_Operator = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);

                        st_dispatchArray = dispatchlist_db.readallrecord_Ordered();
                        if (st_dispatchArray != null)
                        {
                            for (int i = 0; i < st_dispatchArray.Length; i++)
                            {
                                //Find first published dispatch
                                if (st_dispatchArray[i].status == gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED.ToString())
                                {
                                    dispatchCode = st_dispatchArray[i].dispatchCode;
                                    productCode = st_dispatchArray[i].productCode;
                                    //Save operator to it 
                                    dispatchlistDB.dispatchlist_t st = new dispatchlistDB.dispatchlist_t();
                                    st.operatorName = m_Operator;
                                    dispatchlist_db.updaterecord_ByDispatchcode(st, dispatchCode);
			                        string str = dispatchCode + ";" + productCode;
			                        m_ClientThread.sendStringToClient(str, communicationType);
									return -1;//We have send data to client, same as no action from caller's point of view
                                }
                            }
                        }
                        return 0;
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
                    strInputArray = strInput.Split(';');

                    //流延工序
                    if (communicationType == COMMUNICATION_TYPE_CAST_PROCESS_PRODUCT_BARCODE_UPLOAD)
                    {
                        productcastinglistDB.productcast_t st_cast;
                        productcastinglistDB db = new productcastinglistDB();

                        if (strInputArray.Length != 2) return -1;

                        st_cast.dispatchCode = strInputArray[0].Substring(0, 12);
                        st_cast.barCode = strInputArray[0];
                        st_cast.batchNum = st_cast.dispatchCode.Substring(0, 7);
                        st_cast.largeIndex = strInputArray[0].Substring(16, 3);
                        st_cast.machineID = m_machineIDForPrint.ToString();
                        //st_cast.scanTime = st_cast.dispatchCode.Substring(0,4) + st_cast.dispatchCode.Substring(7,2) + strInputArray[0].Substring(12,4);
                        st_cast.scanTime = System.DateTime.Now.ToString();
                        st_cast.weight = Convert.ToSingle(strInputArray[1]);
						st_cast.errorStatus = strInputArray[0].Substring(19, 1);
                        return db.writerecord(st_cast);
                    }

                    //印刷工序
                    if (communicationType == COMMUNICATION_TYPE_PRINT_PROCESS_MATERIAL_BARCODE_UPLOAD)
                    {
                        productprintlistDB.productprint_t st_print = new productprintlistDB.productprint_t();
                        productprintlistDB db = new productprintlistDB();

                        st_print.materialScanTime = System.DateTime.Now.ToString();
                        st_print.materialBarCode = strInputArray[0];
                        st_print.largeIndex = strInputArray[0].Substring(16, 3);
                        st_print.machineID = m_machineIDForPrint.ToString();

                        return db.writerecord(st_print);
                    }
                    if (communicationType == COMMUNICATION_TYPE_PRINT_PROCESS_PRODUCT_BARCODE_UPLOAD)
                    {
                        productprintlistDB.productprint_t st_print = new productprintlistDB.productprint_t();
                        productprintlistDB db = new productprintlistDB();

                        st_print.dispatchCode = strInputArray[1].Substring(0, 12);
                        st_print.batchNum = st_print.dispatchCode.Substring(0, 7);
                        st_print.productScanTime = System.DateTime.Now.ToString();
                        st_print.productBarCode = strInputArray[1];
                        st_print.weight = Convert.ToSingle(strInputArray[2]);

                        return db.updaterecord_ByMaterialBarCode(st_print, strInputArray[0]);
                    }
                    //分切工序
                    if (communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_MATERIAL_BARCODE_UPLOAD)
                    {
                        productslitlistDB.productslit_t st_slit = new productslitlistDB.productslit_t();
                        productslitlistDB db = new productslitlistDB();

                        st_slit.materialScanTime = System.DateTime.Now.ToString();
                        st_slit.materialBarCode = strInputArray[0];
                        st_slit.largeIndex = strInputArray[0].Substring(16, 3);
                        st_slit.machineID = m_machineIDForPrint.ToString();

                        return db.writerecord(st_slit);
                    }
                    if (communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_PRODUCT_BARCODE_UPLOAD)
                    {
                        productslitlistDB.productslit_t st_slit = new productslitlistDB.productslit_t();
                        productslitlistDB db = new productslitlistDB();

                        st_slit.dispatchCode = strInputArray[1].Substring(0, 12);
                        st_slit.batchNum = st_slit.dispatchCode.Substring(0, 7);
                        st_slit.productScanTime = System.DateTime.Now.ToString();
                        st_slit.productBarCode = strInputArray[1];
                        st_slit.smallIndex = strInputArray[1].Substring(19, 2);
                        st_slit.customerIndex = strInputArray[1].Substring(21, 1);
                        st_slit.errorStatus = strInputArray[1].Substring(22, 1);
                        st_slit.numOfJoins = strInputArray[3];
                        st_slit.weight = Convert.ToSingle(strInputArray[2]);

                        return db.updaterecord_ByMaterialBarCode(st_slit, strInputArray[0]);
                    }
                    //质检工序
                    if (communicationType == COMMUNICATION_TYPE_INSPECTION_PROCESS_MATERIAL_BARCODE_UPLOAD)
                    {
                        inspectionlistDB.inspection_t st_inspect = new inspectionlistDB.inspection_t();
                        inspectionlistDB db = new inspectionlistDB();

                        st_inspect.materialBarCode = strInputArray[0];
                        st_inspect.materialScanTime = System.DateTime.Now.ToString();

                        return db.writerecord(st_inspect);
                    }
                    if (communicationType == COMMUNICATION_TYPE_INSPECTION_PROCESS_PRODUCT_BARCODE_UPLOAD)
                    {
                        inspectionlistDB.inspection_t st_inspect = new inspectionlistDB.inspection_t();
                        inspectionlistDB db = new inspectionlistDB();

                        st_inspect.dispatchCode = strInputArray[1].Substring(0, 12);
                        st_inspect.batchNum = st_inspect.dispatchCode.Substring(0, 7);
                        st_inspect.productBarCode = strInputArray[1];
                        st_inspect.productScanTime = System.DateTime.Now.ToString();
                        st_inspect.checkingResult = strInputArray[1].Substring(strInputArray[1].Length - 1, 1);
                        st_inspect.inspector = strInputArray[2];

                        return db.updaterecord_ByMaterialBarCode(st_inspect, strInputArray[0]);
                    }
                    //再造料工序
                    if (communicationType == COMMUNICATION_TYPE_REUSE_PROCESS_BARCODE_UPLOAD)
                    {
                        reusematerialDB.reusematerial_t st_reuse;
                        reusematerialDB db = new reusematerialDB();

                        st_reuse.barcodeForReuse = strInputArray[0];
                        st_reuse.rebuildDate = System.DateTime.Now.ToString();
                        st_reuse.BOMCode = strInputArray[12];
                        st_reuse.rebuildNum = Convert.ToUInt16(strInputArray[1]);
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
                        dispatchlistDB.dispatchlist_t st_dispatch = new dispatchlistDB.dispatchlist_t();

                        st_dispatch.notes = strInputArray[1];
						st_dispatch.operatorName = m_Operator;
                        return db.updaterecord_ByDispatchcode(st_dispatch, strInputArray[0]);
                    }
                    return -1;
                }

                //return value: -1: no action
                //				 0: OK
                //				>0: Fail
				private int HandlePackingProcess(int communicationType, byte[] onePacket, int packetLen)
                {
                    int len;
                    string strInput;
                    string[] strInputArray;

                    //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                    len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;
                    strInput = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                    strInputArray = strInput.Split(';');

                	if (communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_PACKAGE_BARCODE_UPLOAD){
						finalpackingDB db = new finalpackingDB();
						finalpackingDB.finalpacking_t st_packing = new finalpackingDB.finalpacking_t();

						//<产品代码>;<卷数>;<重量>;<长度>;<打包条码>;<卷条码1>;...;<卷条码N>
						st_packing.productCode = strInputArray[0];
						st_packing.packageBarcode = strInputArray[4];
                        st_packing.totalWeight = Convert.ToSingle(strInputArray[2]);
                        st_packing.totalLength = Convert.ToSingle(strInputArray[3]);
						st_packing.rollNumber = Convert.ToUInt16(strInputArray[1]);
						st_packing.uploadTime = System.DateTime.Now.ToString();
						st_packing.machineID = m_machineIDForPrint.ToString();
						for (int i=0;i<st_packing.rollNumber;i++){
							st_packing.barcode = strInputArray[5+i];
							db.writerecord(st_packing);
						}
						return 0;
					}
					if (communicationType == COMMUNICATION_TYPE_PACKING_PROCESS_PACKAGE_BARCODE_UPLOAD) {
						finalpackingDB db = new finalpackingDB();
						finalpackingDB.finalpacking_t st_packing = new finalpackingDB.finalpacking_t();

						//<打包条码>;<员工>
						st_packing.scanTime = System.DateTime.Now.ToString();
						st_packing.workerID = strInputArray[1];
						return db.updaterecordBy_packageBarcode(st_packing, strInputArray[0]);
					}
					return -1;
				}

                public void processLabelPrintingFunc(int communicationType, byte[] onePacket, int packetLen)
                {
                    int result;
                    string dispatchCode=null, productCode=null;
                    int printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - CAST_PROCESS_PC1 + gVariable.castingProcess[0];

                    try
                    {
                        result = HandleHandShake(communicationType, onePacket, packetLen);
                        if (result >= 0) m_ClientThread.sendResponseOKBack(result);

                        result = HandleMaterialProcess(communicationType, onePacket, packetLen);
                        if (result >= 0) m_ClientThread.sendResponseOKBack(result);

                        result = HandleProcessStart(communicationType, onePacket, packetLen, ref dispatchCode, ref productCode);
                        if (result >= 0) m_ClientThread.sendResponseOKBack(result);

                        result = HandleBarcode(communicationType, onePacket, packetLen);
                        if (result >= 0) m_ClientThread.sendResponseOKBack(result);

                        result = HandleProcessEnd(communicationType, onePacket, packetLen);
	    				if (result >= 0) m_ClientThread.sendResponseOKBack(result);

		    			result = HandlePackingProcess(communicationType, onePacket, packetLen);
			    		if (result >= 0) m_ClientThread.sendResponseOKBack(result);


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
                            /*case COMMUNICATION_TYPE_WAREHOUE_OUT_START:
                                getMaterialInfoForAllMachines();
                                sendMaterialInfoToPrintSW();
                                break;
                            case COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE:
                            case COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE:
                                result = setMaterialWareInOut(onePacket, packetLen);
                                m_ClientThread.sendResponseOKBack(result);
                                break;*/

                            /*-----------------------------------------------------------------------------------------------
                            //每一班流延设备工人上班后（本地PC会显示本地记录的上班次的情况），工单在每个工人下班后
                            //就结束了，此时机器还在运行，一个标准大卷还未完成，该卷算在下个班次。从start，生产出的大卷标签上
                            //传，到下班填上交接班记录，该工单在数据库中，hXX.O_dispatchList就完整的封闭了（开始时间，结束时间，交接备注等）。
                            -------------------------------------------------------------------------------------------------*/
                            /*case COMMUNICATION_TYPE_CAST_PROCESS_START:
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
                                break;*/

                            /*-----------------------------------------------------------------------------------------------
                            印刷工序和流延工序基本相同，工单，数据库不同而已
                            -------------------------------------------------------------------------------------------------*/
                            /*case COMMUNICATION_TYPE_PRINT_PROCESS_START:
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
                                break;*/

                            /*-----------------------------------------------------------------------------------------------
                            分切工序基本相同，工单，数据库不同而已
                            -------------------------------------------------------------------------------------------------*/
                            /*case COMMUNICATION_TYPE_SLIT_PROCESS_START:  //分切工上班了
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
                                break;*/
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("processLabelPrintingFunc(" + communicationType + "," + packetLen + ") for printingMachineID = " + m_machineIDForPrint + "failed, " + ex);
                    }
                }
            }
        }
    }
}
	

