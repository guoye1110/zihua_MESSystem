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
using MESSystem.dispatchManagement;

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
                private const int CAST_PROCESS_PC6 = 146;
                private const int CAST_PROCESS_PC7 = 147;
                private const int PRINT_PROCESS_PC1 = 161;
                private const int PRINT_PROCESS_PC2 = 162;
                private const int PRINT_PROCESS_PC3 = 163;
                private const int PRINT_PROCESS_PC4 = 164;
                private const int PRINT_PROCESS_PC5 = 165;
                private const int SLIT_PROCESS_PC1 = 181;
                private const int SLIT_PROCESS_PC2 = 181;
                private const int SLIT_PROCESS_PC3 = 181;
                private const int SLIT_PROCESS_PC4 = 181;
                private const int SLIT_PROCESS_PC5 = 181;
                private const int INSPECTION_PROCESS_PC1 = 201;
                private const int REBUILD_PROCESS_PC1 = 221;
                private const int REBUILD_PROCESS_PC2 = 222;
                private const int REBUILD_PROCESS_PC3 = 223;
                private const int REBUILD_PROCESS_PC4 = 224;
                private const int REBUILD_PROCESS_PC5 = 225;
                private const int PACKING_PROCESS_PC1 = 241;
                private const int PACKING_PROCESS_PC2 = 242;
                private const int PACKING_PROCESS_PC3 = 243;
                private const int PACKING_PROCESS_PC4 = 244;

                private ClientThread m_ClientThread = null;
                private int m_machineIDForPrint;
				private string m_Operator;

				public delegate void notifyHandler(string barcode);
				public static event notifyHandler notify_client;

                public zihua_printerClient(ClientThread cThread)
                {
                    m_ClientThread = cThread;
					m_machineIDForPrint = 0;
					m_Operator = null;
                }

				public static void notify_printerClient(string barcode)
                {
                	notify_client(barcode);
				}

				private int find_bigRolls(string[] bigRolls, string bigRoll)
				{
					for (int i = 0; i < bigRolls.Length; i++)
					{
						if (bigRolls[i] == null || bigRolls[i] == "") continue;
						if (bigRolls[i] == bigRoll)
							return i;
					}
					return -1;
				}
				
				private int find_free(string[] strArray)
				{
					for (int i = 0; i < strArray.Length; i++)
						if (strArray[i] == null || strArray[i] == "")
							return i;
					return -1;
				}

				private string format_rolls_list(string input_str)
		        {
		            int i, j;
    		        string[] inputSplitted = input_str.Split(';');

            		string[] bigRolls = new string[inputSplitted.Length];
		            string[][] rolls = new string[inputSplitted.Length][];

		            for (i = 0; i < inputSplitted.Length; i++) rolls[i] = new string[inputSplitted.Length];

		            int index = 0;
		            foreach (string input in inputSplitted)
        		    {
                		index = find_bigRolls(bigRolls, input.Substring(0, 3));
		                if (index == -1)
        		        {
        		        	//not found
                		    index = find_free(bigRolls);
		                    bigRolls[index] = input.Substring(0, 3);
                		}
                		rolls[index][find_free(rolls[index])] = input.Remove(0, 4);
            		}
		            for (i = 0; i < inputSplitted.Length; i++)
        		    {
                		Array.Sort(rolls[i]);
		                int val_last, val, val_next;

		                bool fstarted = false;
	        	        for (j = 0; j < rolls[i].Length; j++)
    	        	    {
        	        	    if (rolls[i][j] == null) continue;
            	        	if (j == 0 || fstarted == false || j == inputSplitted.Length - 1)
		                    {
        		                fstarted = true;
                		        continue;
		                    }

		                    val = Convert.ToUInt16(rolls[i][j]);
        		            if (rolls[i][j - 1] == null || rolls[i][j - 1] == "-")
                		    {
                        		//只比较next
		                        val_next = Convert.ToUInt16(rolls[i][j + 1]);
        		                if (val == val_next - 1)
                		            rolls[i][j] = null;
                    		}
                    		else
                    		{
                        		val_last = Convert.ToUInt16(rolls[i][j - 1]);
		                        val_next = Convert.ToUInt16(rolls[i][j + 1]);

        		                if (val == val_last + 1 && val == val_next - 1)
                		            rolls[i][j] = "-";
		                    }
        		        }
		            }

		            string output = "";
    		        for (i = 0; i < inputSplitted.Length; i++)
            		{
		                if (bigRolls[i] == null) continue;

        		        output += bigRolls[i] + "(";
		                for (j = 0; j < inputSplitted.Length; j++)
		                {
        		            if (rolls[i][j] == null) continue;
                		    output += rolls[i][j];

		                    if (j == inputSplitted.Length - 1 || rolls[i][j] == "-") continue;
		                    if (j != inputSplitted.Length - 1 && rolls[i][j + 1] == "-") continue;

		                    output += ",";
                        }

        		        output += ")";
                		if (i == inputSplitted.Length - 1 || bigRolls[i + 1] == null) continue;
		                output += ";";
        		    }

		            Console.WriteLine("output is \"{0}\"", output);

        		    return output;
		        }

                private void getMaterialInfoForAllMachines()
                {
                    int i;
                    string str;

                    str = null;
                    try
                    {
                        for (i = 0; i < communicate.NUM_OF_FEEDING_MACHINE; i++)
                        {
                            str += dispatchTools.getMaterialRequirementForOneMachine(i + 1);
                        }
				
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
                    strInputArray = strInput.Split(';', '\r');

                    if (communicationType == COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE)
                    {
                        materialdelivery.materialCode = strInputArray[0];
                        materialdelivery.materialBatchNum = null;
                        materialdelivery.targetMachine = strInputArray[1];
                        materialdelivery.targetFeedBinIndex = strInputArray[2];
                        materialdelivery.inoutputQuantity = strInputArray[3];
                        materialdelivery.direction = "2";//1: 出库 2：入库
                    }
                    else
                    {
                        materialdelivery.materialCode = strInputArray[0];
                        materialdelivery.materialBatchNum = strInputArray[1];
                        materialdelivery.targetMachine = strInputArray[2];
                        materialdelivery.targetFeedBinIndex = strInputArray[3];
                        materialdelivery.inoutputQuantity = strInputArray[4];
                        materialdelivery.direction = "1";//1: 出库 2：入库
                    }
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

				private void materialBarcodeUpload(string barcode)
                {
                    int machineID;
                    byte[] aa;

					if (m_machineIDForPrint >= PRINT_PROCESS_PC1 && m_machineIDForPrint <= PRINT_PROCESS_PC1+(gVariable.printingProcess[gVariable.printingProcess.Length-1]-gVariable.printingProcess[0]))
					{
                        productprintlistDB.productprint_t st_print = new productprintlistDB.productprint_t();
                        productprintlistDB db = new productprintlistDB();

                        st_print.materialScanTime = System.DateTime.Now.ToString();
                        st_print.materialBarCode = barcode;
                        st_print.largeIndex = barcode.Substring(16, 3);
                        machineID = m_machineIDForPrint-PRINT_PROCESS_PC1+gVariable.printingProcess[0];
                        st_print.machineID = machineID.ToString();

                        db.writerecord(st_print);
						//<原料流延卷条码>;
						string str = st_print.materialBarCode;
						m_ClientThread.sendStringToClient(str, COMMUNICATION_TYPE_PRINT_PROCESS_MATERIAL_BARCODE_UPLOAD);

                        //record current large roll index
                        gVariable.castRollIndex[machineID - 1] = Convert.ToInt32(st_print.largeIndex) + 1;
                        aa = System.Text.Encoding.Default.GetBytes(barcode.Substring(19, 1));
                        gVariable.castRollStatus[machineID - 1] = aa[0];
                    }

					else if (m_machineIDForPrint >= SLIT_PROCESS_PC1 && m_machineIDForPrint <= SLIT_PROCESS_PC1+(gVariable.slittingProcess[gVariable.slittingProcess.Length-1]-gVariable.slittingProcess[0]))
					{
						productslitlistDB.productslit_t st_slit = new productslitlistDB.productslit_t();
						productslitlistDB db = new productslitlistDB();
						
						st_slit.materialScanTime = System.DateTime.Now.ToString();
                        st_slit.materialBarCode = barcode;
                        st_slit.largeIndex = barcode.Substring(16, 3);
						st_slit.machineID = (m_machineIDForPrint-SLIT_PROCESS_PC1+gVariable.slittingProcess[0]).ToString();
					
						db.writerecord(st_slit);
						//<原料流延卷条码>;
						string str = st_slit.materialBarCode;
						m_ClientThread.sendStringToClient(str, COMMUNICATION_TYPE_SLIT_PROCESS_MATERIAL_BARCODE_UPLOAD);
					}
				}
				
                //return value: -1: no action
                //				 0: OK
                //				>0: Fail
                private int HandleHandShake(int communicationType, byte[] onePacket, int packetLen)
                {
                	if (communicationType == COMMUNICATION_TYPE_HANDSHAKE_PRINT_MACHINE_ID){
                    	m_machineIDForPrint = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100;
						Console.WriteLine("Machine " + m_machineIDForPrint + " Handshake!");
						m_ClientThread.handshakeWithClientOK = 1;

						//只有印刷、分切工序需要订阅来自平板发来的barcode
						if (m_machineIDForPrint >= PRINT_PROCESS_PC1 && m_machineIDForPrint <= PRINT_PROCESS_PC1+(gVariable.printingProcess[gVariable.printingProcess.Length-1]-gVariable.printingProcess[0]))
							notify_client += new notifyHandler(materialBarcodeUpload);
						if (m_machineIDForPrint >= SLIT_PROCESS_PC1 && m_machineIDForPrint <= SLIT_PROCESS_PC1+(gVariable.slittingProcess[gVariable.slittingProcess.Length-1]-gVariable.slittingProcess[0]))
							notify_client += new notifyHandler(materialBarcodeUpload);
                    	return 0;
                	}
					return -1;
                }

                //return value: -1: no action
                //				 0: OK
                //				>0: Fail
                private int HandleMaterialProcess(int communicationType, byte[] onePacket, int packetLen)
                {
                    //string operatorID;

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
                    //dispatchlistDB.dispatchlist_t[] st_dispatchArray;

                    int machineID;
                    //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                    int len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;

                    if (communicationType == COMMUNICATION_TYPE_CAST_PROCESS_START || communicationType == COMMUNICATION_TYPE_PRINT_PROCESS_START || communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_START)
                    {
                        dispatchlistDB dispatchlist_db;
						string dName;
						//gVariable.dispatchSheetStruct[] dispatchs;

						switch (communicationType )
						{
						case COMMUNICATION_TYPE_CAST_PROCESS_START:
                            machineID = m_machineIDForPrint - CAST_PROCESS_PC1 + gVariable.castingProcess[0];
                            dispatchTools.getDispatchFromTaskSheet(machineID);
                            dispatchlist_db = new dispatchlistDB(machineID);
                            dName = gVariable.DBHeadString + machineID.ToString().PadLeft(3, '0');
							break;
						case COMMUNICATION_TYPE_PRINT_PROCESS_START:
                            machineID = m_machineIDForPrint - PRINT_PROCESS_PC1 + gVariable.printingProcess[0];
                            dispatchTools.getDispatchFromTaskSheet(machineID);
							dispatchlist_db = new dispatchlistDB(machineID);
							dName = gVariable.DBHeadString + machineID.ToString().PadLeft(3, '0');
							break;
						case COMMUNICATION_TYPE_SLIT_PROCESS_START:
                            machineID = m_machineIDForPrint-SLIT_PROCESS_PC1+gVariable.slittingProcess[0];
                            dispatchTools.getDispatchFromTaskSheet(machineID);
							dispatchlist_db = new dispatchlistDB(machineID);
							dName = gVariable.DBHeadString + machineID.ToString().PadLeft(3, '0');
							break;
						default:
							dispatchlist_db = new dispatchlistDB(0);
							dName = gVariable.DBHeadString + (0).ToString().PadLeft(3, '0');
							break;
						}

						m_Operator = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);

						/*string today = DateTime.Now.Date.ToString("yyyy-MM-dd 08:00:00");
						string endDay = DateTime.Now.Date.AddDays(1,).ToString("yyyy-MM-dd 08:00:00");
						dispatchs = mySQLClass.getDispatchListInPeriodOfTime(dName, gVariable.dispatchListTableName, today, endDay, gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED,
																				gVariable.TIME_CHECK_TYPE_PLANNED_START);

						if (dispatchs != null) {
							//找到planTime1最小的记录
							string latest_planTime1 = endDay;
							int latest_record_index = 0;
							for (int index=0;index<dispatchs.Length;index++) {
								int result = DateTime.Compare(Convert.ToDateTime(dispatchs[index].planTime1), Convert.ToDateTime(latest_planTime1));
								if (result<0) {
									latest_planTime1 = dispatchs[index].planTime1;
									latest_record_index = index;
								}
							}
                            dispatchCode = dispatchs[latest_record_index].dispatchCode;
                            productCode = dispatchs[latest_record_index].productCode;
                            //Save operator to it 
                            dispatchlistDB.dispatchlist_t st = new dispatchlistDB.dispatchlist_t();
                            st.operatorName = m_Operator;
                            dispatchlist_db.updaterecord_ByDispatchcode(st, dispatchCode);
	                        string str = dispatchCode + ";" + productCode;
	                        m_ClientThread.sendStringToClient(str, communicationType);
							return -1;//We have send data to client, same as no action from caller's point of view
						}*/

						//工单动态生产（getDispatchFromTaskSheet），有且只有一个published/started工单，直到上一个工单完成（status = MACHINE_STATUS_DISPATCH_COMPLETED）
                        {
                        	dispatchlistDB.dispatchlist_t[] st_dispatchArray;
                        	st_dispatchArray = dispatchlist_db.readallrecord_Ordered();
                        	if (st_dispatchArray != null)
                        	{
                               	dispatchCode = st_dispatchArray[0].dispatchCode;
                               	productCode = st_dispatchArray[0].productCode;
                               	//Save operator to it 
                               	dispatchlistDB.dispatchlist_t st = new dispatchlistDB.dispatchlist_t();
                               	st.operatorName = m_Operator;
                               	dispatchlist_db.updaterecord_ByDispatchcode(st, dispatchCode);
	                        	string str = dispatchCode + ";" + productCode;
			                    m_ClientThread.sendStringToClient(str, communicationType);
								return -1;//We have send data to client, same as no action from caller's point of view
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
                        st_cast.machineID = (m_machineIDForPrint-CAST_PROCESS_PC1+gVariable.castingProcess[0]).ToString();
                        //st_cast.scanTime = st_cast.dispatchCode.Substring(0,4) + st_cast.dispatchCode.Substring(7,2) + strInputArray[0].Substring(12,4);
                        st_cast.scanTime = System.DateTime.Now.ToString();
                        st_cast.weight = Convert.ToSingle(strInputArray[1]);
						st_cast.errorStatus = strInputArray[0].Substring(19, 1);
                        return db.writerecord(st_cast);
                    }

                    //印刷工序，平板终端==》服务器==》PC扫描终端
#if true
                    if (communicationType == COMMUNICATION_TYPE_PRINT_PROCESS_MATERIAL_BARCODE_UPLOAD)
                    {
                        productprintlistDB.productprint_t st_print = new productprintlistDB.productprint_t();
                        productprintlistDB db = new productprintlistDB();

                        st_print.materialScanTime = System.DateTime.Now.ToString();
                        st_print.materialBarCode = strInputArray[0];
                        st_print.largeIndex = strInputArray[0].Substring(16, 3);
                        st_print.machineID = (m_machineIDForPrint-PRINT_PROCESS_PC1+gVariable.printingProcess[0]).ToString();

                        db.writerecord(st_print);
						//<原料流延卷条码>;
						string str = st_print.materialBarCode;
						m_ClientThread.sendStringToClient(str, communicationType);
						return -1;
                    }
#else
					if (communicationType == COMMUNICATION_TYPE_PRINT_PROCESS_MATERIAL_BARCODE_UPLOAD)
                    {
                        productprintlistDB.productprint_t? st_print;
                        productprintlistDB db = new productprintlistDB();

						st_print = db.readlastrecord_ByMachineID(m_machineIDForPrint-PRINT_PROCESS_PC1+gVariable.printingProcess[0]);
						if (st_print == null || st_print.Value.productBarCode == null)
							return 0;

						//<原料流延卷条码>;
						string str = st_print.Value.materialBarCode;
						m_ClientThread.sendStringToClient(str, communicationType);
						return -1;
                    }
#endif
                    if (communicationType == COMMUNICATION_TYPE_PRINT_PROCESS_PRODUCT_BARCODE_UPLOAD)
                    {
                        productprintlistDB.productprint_t st_print = new productprintlistDB.productprint_t();
                        productprintlistDB db = new productprintlistDB();

                        st_print.dispatchCode = strInputArray[1].Substring(0, 12);
                        st_print.batchNum = st_print.dispatchCode.Substring(0, 7);
                        st_print.productScanTime = System.DateTime.Now.ToString();
                        st_print.productBarCode = strInputArray[1];
                        st_print.weight = Convert.ToSingle(strInputArray[2]);
                        st_print.errorStatus = strInputArray[1].Substring(19, 1);

                        return db.updaterecord_ByMaterialBarCode(st_print, strInputArray[0]);
                    }
                    //分切工序，平板终端==》服务器==》PC扫描终端
#if true
                    if (communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_MATERIAL_BARCODE_UPLOAD)
                    {
                        productslitlistDB.productslit_t st_slit = new productslitlistDB.productslit_t();
                        productslitlistDB db = new productslitlistDB();

                        st_slit.materialScanTime = System.DateTime.Now.ToString();
                        st_slit.materialBarCode = strInputArray[0];
                        st_slit.largeIndex = strInputArray[0].Substring(16, 3);
                        st_slit.machineID = (m_machineIDForPrint-SLIT_PROCESS_PC1+gVariable.slittingProcess[0]).ToString();

                        db.writerecord(st_slit);
						//<原料流延卷条码>;
						string str = st_slit.materialBarCode;
						m_ClientThread.sendStringToClient(str, communicationType);
						return -1;				
                    }
#else
					if (communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_MATERIAL_BARCODE_UPLOAD)
					{
						productslitlistDB.productslit_t? st_slit;
						productslitlistDB db = new productslitlistDB();

						st_slit = db.readlastrecord_ByMachineID(m_machineIDForPrint-SLIT_PROCESS_PC1+gVariable.slittingProcess[0]);
						if (st_slit == null || st_slit.Value.productBarCode == null)
							return 0;

						//<原料流延卷条码>;
						string str = st_slit.Value.materialBarCode;
						m_ClientThread.sendStringToClient(str, communicationType);
						return -1;
					}
#endif
                    if (communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_PRODUCT_BARCODE_UPLOAD)
                    {
                        productslitlistDB.productslit_t st_slit = new productslitlistDB.productslit_t();
                        productslitlistDB db = new productslitlistDB();

                        st_slit.machineID = "15";
                        st_slit.materialBarCode = strInputArray[0];
                        st_slit.dispatchCode = strInputArray[1].Substring(0, 12);
                        st_slit.batchNum = st_slit.dispatchCode.Substring(0, 7);
                        st_slit.productScanTime = System.DateTime.Now.ToString();
                        st_slit.productBarCode = strInputArray[1];
                        st_slit.smallIndex = strInputArray[1].Substring(19, 2);
                        st_slit.largeIndex = strInputArray[1].Substring(16, 3);
                        st_slit.customerIndex = strInputArray[1].Substring(21, 1);
                        st_slit.errorStatus = strInputArray[1].Substring(22, 1);
                        st_slit.numOfJoins = strInputArray[3];
                        st_slit.weight = Convert.ToSingle(strInputArray[2]);
                        st_slit.plateNo = strInputArray[4];

                        return db.writerecord(st_slit);
                        //return db.updaterecord_ByMaterialBarCode(st_slit, strInputArray[0]);
                    }
                    //质检工序
                    if (communicationType == COMMUNICATION_TYPE_INSPECTION_PROCESS_MATERIAL_BARCODE_UPLOAD)
                    {
                        inspectionlistDB.inspection_t st_inspect = new inspectionlistDB.inspection_t();
                        inspectionlistDB db = new inspectionlistDB();

                        st_inspect.materialBarCode = strInputArray[0];
                        st_inspect.materialScanTime = System.DateTime.Now.ToString();

                        db.writerecord(st_inspect);

						//区分流延大卷（20），印刷大卷（20），分切小卷（22）
						{
							string dispatchCode = strInputArray[0].Substring(0,12);
							string process = dispatchCode.Substring(10,1);
							int machineid = Convert.ToUInt16(dispatchCode.Substring(11,1));
							dispatchlistDB db_dispatch=null;
							dispatchlistDB.dispatchlist_t[] dispatchs;
							
							if (process == "L")	db_dispatch = new dispatchlistDB(machineid+gVariable.castingProcess[0]-1);
							if (process == "Y")	db_dispatch = new dispatchlistDB(machineid+gVariable.printingProcess[0]-1);
							if (process == "F")	db_dispatch = new dispatchlistDB(machineid+gVariable.slittingProcess[0]-1);
							
							dispatchs = db_dispatch.readrecord_ByDispatchcode(dispatchCode);
							if (dispatchs == null)	return 1;

							//<产品代码>;
							string str = dispatchs[0].productCode;
							m_ClientThread.sendStringToClient(str, communicationType);
							return -1;
						}
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
                        reusematerialDB.reusematerial_t st_reuse = new reusematerialDB.reusematerial_t();
                        reusematerialDB db = new reusematerialDB();

                        //<设备号>;<再造料序号>;<配方号>;<再造料重量>;<原料重量>;<条码…>;<员工工号>
                        st_reuse.machineID = strInputArray[0];
                        st_reuse.barcodeForReuse = strInputArray[1];
                        st_reuse.rebuildDate = System.DateTime.Now.ToString();
                        st_reuse.rebuildNum = Convert.ToUInt16(strInputArray[3]);
                        st_reuse.originalNum = Convert.ToUInt16(strInputArray[4]);
                        st_reuse.barCode1 = strInputArray[5];
                        st_reuse.barCode2 = strInputArray[6];
                        st_reuse.barCode3 = strInputArray[7];
                        st_reuse.barCode4 = strInputArray[8];
                        st_reuse.barCode5 = strInputArray[9];
                        st_reuse.barCode6 = strInputArray[10];
                        st_reuse.barCode7 = strInputArray[11];
                        st_reuse.barCode8 = strInputArray[12];
                        st_reuse.barCode9 = strInputArray[13];
                        st_reuse.barCode10 = strInputArray[14];
                        st_reuse.BOMCode = strInputArray[2];
                        st_reuse.workerID = strInputArray[15];

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

                    //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                    len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;
                    strInput = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                    strInputArray = strInput.Split(';');

                    if (communicationType == COMMUNICATION_TYPE_CAST_PROCESS_END || communicationType == COMMUNICATION_TYPE_PRINT_PROCESS_END || communicationType == COMMUNICATION_TYPE_SLIT_PROCESS_END)
                    {
                    	dispatchlistDB db;
						switch (communicationType )
						{
						case COMMUNICATION_TYPE_CAST_PROCESS_END:
							db = new dispatchlistDB(m_machineIDForPrint-CAST_PROCESS_PC1+gVariable.castingProcess[0]);
							break;
						case COMMUNICATION_TYPE_PRINT_PROCESS_END:
							db = new dispatchlistDB(m_machineIDForPrint-PRINT_PROCESS_PC1+gVariable.printingProcess[0]);
							break;
						case COMMUNICATION_TYPE_SLIT_PROCESS_END:
							db = new dispatchlistDB(m_machineIDForPrint-SLIT_PROCESS_PC1+gVariable.slittingProcess[0]);
							break;
						default:
							db = new dispatchlistDB(0);
							break;
						}                    
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

                        try
                        {
                        	//<打包条码>;<铲板号>;<订单号>;<产品代号>;<产品批号>;<卷数>;<重量>;<长度>;<小卷条码信息>
                        	st_packing.packageBarcode = strInputArray[0];
							st_packing.plateNo = strInputArray[1];
							st_packing.salesOrder = strInputArray[2];
                            st_packing.productCode = strInputArray[3];
							st_packing.batchNum = strInputArray[4];
							st_packing.rollNumber = Convert.ToUInt16(strInputArray[5]);
                            st_packing.totalWeight = Convert.ToSingle(strInputArray[6]);
                            st_packing.totalLength = Convert.ToSingle(strInputArray[7]);
                            st_packing.uploadTime = System.DateTime.Now.ToString();
                            st_packing.machineID = (m_machineIDForPrint - SLIT_PROCESS_PC1 + gVariable.slittingProcess[0]).ToString();
                            st_packing.workerID = strInputArray[8];
                            //st_packing.smallRoll = strInputArray[6 + i];
                            st_packing.rollList = null;
                            for (int i = 0; i < st_packing.rollNumber; i++)
                            {
                                st_packing.rollList += strInputArray[9 + i] + ",";
                            }
                            //for (int i = 0; i < st_packing.rollNumber; i++)
                            //{
                            //    st_packing.smallRoll = strInputArray[6 + i] + ";";
                            //    db.writerecord(st_packing);
                            // }
                            db.writerecord(st_packing);
                            return 0;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("get data from client PC for packing info failed, possibly small roll number error!" + ex);
                            return -1;
                        }
                    }
					if (communicationType == COMMUNICATION_TYPE_PACKING_PROCESS_PACKAGE_BARCODE_UPLOAD) {
						finalpackingDB db = new finalpackingDB();
						finalpackingDB.finalpacking_t st_packing = new finalpackingDB.finalpacking_t();

						//<打包条码>;<员工>
						st_packing.scanTime = System.DateTime.Now.ToString();
						st_packing.workerID = strInputArray[1];
						db.updaterecordBy_packageBarcode(st_packing, strInputArray[0]);

						//返回：<打包条码>;<铲板号>;<订单号>;<产品代号>;<产品批号>;<卷数>;<重量>;<长度>;<小卷条码信息>
						finalpackingDB.finalpacking_t? pack;
						pack = db.readrecordBy_packageBarcode(strInputArray[0]);
						if (pack!=null) {
							string str;
                            string output = format_rolls_list(pack.Value.rollList);

                            str = pack.Value.packageBarcode + ";" + pack.Value.plateNo + ";" + pack.Value.salesOrder + ";" + pack.Value.productCode + ";";
                            str += pack.Value.batchNum + ";" + pack.Value.rollNumber + ";" + pack.Value.totalWeight + ";" + pack.Value.totalLength + ";" + output;
							m_ClientThread.sendStringToClient(str, communicationType);
							return -1;
						}
						return 0;
					}
					return -1;
				}

                public void processLabelPrintingFunc(int communicationType, byte[] onePacket, int packetLen)
                {
                    int result;
                    string dispatchCode=null, productCode=null;
                    //int printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - CAST_PROCESS_PC1 + gVariable.castingProcess[0];

                    try
                    {
                    	if (communicationType == COMMUNICATION_TYPE_PRINTING_HEART_BEAT)
							m_ClientThread.sendResponseOKBack(0);
						
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


                        //switch (communicationType)
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
	
