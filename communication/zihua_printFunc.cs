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
using MESSystem.communication;
using MESSystem.communication.communicate;

namespace MESSystem.zhihua_printerClient {

	public class printerClient {		
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
        private const int COMMUNICATION_TYPE_INSPECTION_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xC4;
		//再造料工序
        private const int COMMUNICATION_TYPE_REUSE_PROCESS_BARCODE_UPLOAD = 0xC5;
		//打包工序		
        private const int COMMUNICATION_TYPE_PACKING_PROCESS_PACKAGE_BARCODE_UPLOAD = 0xC6;
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

		private ClientThread m_ClientThread=null;
		private int m_machineIDForPrint;

		public void printerClient(ClientThread cThread)
		{
			m_ClientThread = cThread;
		}

		private void initVariables()
		{
			handshakeWithClientOK = 1;
		
			inoutMaterialQuantity = 0;
			inoutFeedMachineID = 0;
			inoutMaterialStackID = 0;
		
			for (int i = 0; i < NUM_OF_FEEDING_MACHINE; i++){
				for (int j = 0; j < STACK_NUM_ONE_MACHINE; j++){
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
			
			try	{
				for (i = 0; i < NUM_OF_FEEDING_MACHINE; i++) {
					sackNumLeftInStack[i, ] = mySQLClass.getFeedCurrentLeft(i, STACK_NUM_ONE_MACHINE);
				
					//get dispatch list for this machine in defined period
					dName = gVariable.DBHeadString + (i + 1).ToString().PadLeft(3, '0');
					
					dispatchList = mySQLClass.getDispatchListInPeriodOfTime(dName, gVariable.dispatchListTableName, today, endDay, gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED,
																			gVariable.TIME_CHECK_TYPE_PLANNED_START);
	
					if (dispatchList == null)
						continue;

					ingredient = null;
					for (j = 0; j < dispatchList.Length; j++) {
						//deal with one dispatch and its related material table, material informaion stored in tableArray
						commandText = "select * from `" + gVariable.materialListTableName + "` where dispatchCode = '" + dispatchList[j].dispatchCode + "'";
						tabledArray = mySQLClass.databaseCommonReading(gVariable.globalDatabaseName, commandText);
						//materialTypeNum = tableArray[j, mySQLClass.MATERIAL_LIST_NUM_OF_TYPE];
	
						//the next dispatch uses different materials, we will consider it when current dispatch are all completed
						if (ingredient != null && ingredient != tableArray[0, mySQLClass.BOM_CODE_IN_DISPATCHLIST_DATABASE])
							break;
						ingredient = tableArray[0, mySQLClass.BOM_CODE_IN_DISPATCHLIST_DATABASE];
	
						for (k = 0; k < gVariable.maxMaterialTypeNum; k++) {
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
			for (i = 0; i < NUM_OF_FEEDING_MACHINE; i++) {
				for (j = 0; j < STACK_NUM_ONE_MACHINE; j++)	{
					str += materialCodeForStack[i, j] + ";" + sackNumNeededForStack[i, j].ToString() + ";";
				}
			}
		
			m_ClientThread.sendStringToClient(str, COMMUNICATION_TYPE_WAREHOUE_OUT_START);
		}

		private int setMaterialWareInOut(byte[] onePacket, int packetLen)
		{
			int len;
			string strInput;
			globaldatabase.materialinoutrecord.materialinoutrecord_t? materialinouotrecord;

			//MIN_PACKET_LEN include one byte of data, so we need to delete this byte
			len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;
			strInput = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);

			materialinouotrecord = globaldatabase.materialinoutrecord.parseinput(strInput);
			if (materialinouotrecord == null){
				return RESULT_ERR_DATA;
			}
			return globaldatabase.materialinoutrecord.writerecord(materialinouotrecord.Value);
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

		public static string sendDispatchCodeToClient(int communicationType, byte[] onePacket, int len)
		{
			string str;
			dispatchlistDB dispatchlist_db;
			dispatchlistDB.dispatchlist_t? st_dispatchlist;

			int printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - CAST_PROCESS_PC1 + gVariable.castingProcess[0];

			dispatchlist_db = new dispatchlistDB(printingSWPCID);

			st_dispatchlist = dispatchlist_db.currentDispatch;
			if (st_dispatchlist != null){
				str = st_dispatchlist.Value.dispatchCode + ";" + st_dispatchlist.Value.productCode;
				m_ClientThread.sendStringToClient(str, communicationType);
				return null;
			}
			onePacket[PROTOCOL_DATA_POS] = 0xff;
			len = MIN_PACKET_LEN;
			m_ClientThread.sendDataToClient(onePacket, len, communicationType);

			return st_dispatchlist.Value.dispatchCode;
		}

		private int setBarcode(byte[] onePacket, int packetLen)
		{
			int len;
			string strInput;
			string[] strInputArray;

			if ()
			productcastinglistDB.productcastinglist_t st_productcasting;

			//MIN_PACKET_LEN include one byte of data, so we need to delete this byte
			len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;
			strInput = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
			strInputArray = strInput.Split(";");
			if (strInputArray.Length != 2)	return -1;
			
			st_productcasting.dispatchCode = strInputArray[0].Substring(0,12);
			st_productcasting.scanTime = strInputArray[0].Substring(12,4);
			st_productcasting.largeIndex = strInputArray[0].Substring(16,3);
			st_productcasting.machineID = st_productcasting.dispatchCode.Substring(11,1);
			st_productcasting.batchNum = st_productcasting.dispatchCode.Substring(0,7);
			st_productcasting.barCode = strInputArray[0];
			st_productcasting.weight = strInputArray[1];
			
			return productcastinglistDB.writerecord(st_productcasting);
		}

		private int setDispatchOperator(int index, string dispatchCode, byte[] onePacket, int packetLen)
		{
			int len;
			string strInput;
			dispatchlistDB.dispatchlist_t st_dispatch = {0};
			string dispatchCode;

			//MIN_PACKET_LEN include one byte of data, so we need to delete this byte
			len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;
			strInput = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);

			st_dispatch.operatorName = strInput;
			
			return dispatchlistDB.updaterecordby_dispatchcode(index, dispatchlistDB.format(st_dispatch), dispatchCode);
		}

		private int setDispatchNotes(int index,       byte[] onePacket, int packetLen)
		{
			int len;
			string strInput;
			string[] strInputArray;
			dispatchlistDB.dispatchlist_t st_dispatch = {0};
			string dispatchCode;

			//MIN_PACKET_LEN include one byte of data, so we need to delete this byte
			len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;
			strInput = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
			strInputArray = strInput.Split(';');

			dispatchCode = strInputArray[0];
			st_dispatch.notes = strInputArray[1];
			
			return dispatchlistDB.updaterecordby_dispatchcode(index, dispatchlistDB.format(st_dispatch), dispatchCode);
		}

		private int setMaterialBarcode(int communicationType, byte[] onePacket, int packetLen)
		{
			int len;
			string strInput;
			productprintlistDB.productprintlist_t? in_productprint;

			//MIN_PACKET_LEN include one byte of data, so we need to delete this byte
			len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;
			strInput = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
			
			in_productprint = globaldatabase.productprintlist.parseinput(strInput);
			if (in_productprint == null){
				return RESULT_ERR_DATA;
			}

			if (communicationType == COMMUNICATION_TYPE_PRINT_PROCESS_MATERIAL_BARCODE_UPLOAD){
				return globaldatabase.productcastinglist.writerecord(in_productprint.Value);
			}

			if (communicationType == COMMUNICATION_TYPE_PRINT_PROCESS_PRODUCT_BARCODE_UPLOAD){
				return globaldatabase.productprintlist.updateProductScancode(in_productprint.Value);
			}
		}

		public void processLabelPrintingFunc(int communicationType, byte[] onePacket, int packetLen)
		{
			int result;
			int printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - CAST_PROCESS_PC1 + gVariable.castingProcess[0];

			try	{
				switch (communicationType)
				{
				//握手信息
				case COMMUNICATION_TYPE_HANDSHAKE_PRINT_MACHINE_ID:
					m_machineIDForPrint = onePacket[PROTOCOL_DATA_POS];
					onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
					m_ClientThread.sendDataToClient(onePacket, MIN_PACKET_LEN, communicationType);
					break;

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
					setDispatchOperator(printingSWPCID, dispatchCode , onePacket, packetLen);
					break;
				case COMMUNICATION_TYPE_CAST_PROCESS_PRODUCT_BARCODE_UPLOAD:
					result = setCastBarcode(onePacket, packetLen);
					m_ClientThread.sendResponseOKBack(result);
					break;
				case COMMUNICATION_TYPE_CAST_PROCESS_END:
					string dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
					result=setDispatchNotes(printingSWPCID, onePacket, packetLen);
					m_ClientThread.sendResponseOKBack(result);
					break

				/*-----------------------------------------------------------------------------------------------
				印刷工序和流延工序基本相同，工单，数据库不同而已
				-------------------------------------------------------------------------------------------------*/
				case COMMUNICATION_TYPE_PRINT_PROCESS_START:
					string dispatchCode = sendDispatchCodeToClient(COMMUNICATION_TYPE_PRINT_PROCESS_START, onePacket, packetLen);
					setDispatchOperator(printingSWPCID, dispatchCode , onePacket, packetLen);
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
					result=setDispatchFinished(dName, onePacket, packetLen);
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
			catch (Exception ex) {
                    Console.WriteLine("processLabelPrintingFunc(" + communicationType + "," + packetLen + ") for printingMachineID = " + machineIDForPrint + "failed, " + ex);
			}
		}
	}
}
	

