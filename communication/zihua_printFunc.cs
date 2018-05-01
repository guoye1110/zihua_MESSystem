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
		private const int COMMUNICATION_TYPE_WAREHOUE_OUT_START = 0xB5;  //printing SW started and ask for material info, server send material info for all feeding machine to printing SW 
		private const int COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE = 0xB6;  //printing machine send barcode info to server whever a stack of material is moved out of the warehouse
		private const int COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE = 0xB7;  //printing machine send barcode info to server whever a stack of material is moved into the warehouse
		private const int COMMUNICATION_TYPE_CAST_PROCESS_START = 0xB8;  //printing SW started cast process, server need to send dispatch info to printing SW
		private const int COMMUNICATION_TYPE_CAST_BARCODE_UPLOAD = 0xB9;  //printing SW send large roll info to server
		private const int COMMUNICATION_TYPE_CAST_PROCESS_END = 0xBA;
		//private const int COMMUNICATION_TYPE_CASE_SHIFT = 0xBA;
		private const int COMMUNICATION_TYPE_PRINT_PROCESS_START = 0xBB;
        private const int COMMUNICATION_TYPE_PRINT_BARCODE_UPLOAD = 0xC7;
        private const int COMMUNICATION_TYPE_SLIT_PROCESS_START = 0xC8;
        private const int COMMUNICATION_TYPE_SLIT_BARCODE_UPLOAD = 0xC9;
        private const int COMMUNICATION_TYPE_INSPECTION_PROCESS_START = 0xCA;
        private const int COMMUNICATION_TYPE_INSPECTION_BARCODE_UPLOAD = 0xCB;
        private const int COMMUNICATION_TYPE_REUSE_PROCESS_START = 0xCC;
        private const int COMMUNICATION_TYPE_REUSE_BARCODE_UPLOAD = 0xCD;
        private const int COMMUNICATION_TYPE_PACKING_PROCESS_START = 0xCE;
        private const int COMMUNICATION_TYPE_PACKING_BARCODE_UPLOAD = 0xCF;
        private const int COMMUNICATION_TYPE_PRINTING_HEART_BEAT = 0xD0;
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

		private ClientThread m_ClientThread;
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

		void sendDispatchToClient(string dName, int communicationType, byte[] onePacket, int len)
		{
			int i;
			string str;
			string commandText;
			string[,] tableArray;
			gVariable.dispatchSheetStruct[] dispatchList;
			string insertString;

			commandText = "select * from `" + gVariable.dispatchListTableName + "` where status = '1'";
			dispatchList = mySQLClass.getDispatchListInternal(dName, gVariable.dispatchListTableName, commandText, 1);
			if (dispatchList!=null){
				onePacket[PROTOCOL_DATA_POS] = 0xff;
				len = MIN_PACKET_LEN;
				m_ClientThread.sendDataToClient(onePacket, len, communicationType);
				return;
			}
			
			commandText = "select * from `" + gVariable.dispatchListTableName + "` where status = '0' order by id DESC";
			dispatchList = mySQLClass.getDispatchListInternal(dName, gVariable.dispatchListTableName, commandText, 1);
			if (dispatchList == null)
			{
				onePacket[PROTOCOL_DATA_POS] = 0xff;
				len = MIN_PACKET_LEN;
				m_ClientThread.sendDataToClient(onePacket, len, communicationType);
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
				m_ClientThread.sendStringToClient(str, communicationType);
			}
		}

		private int setCastBarcode(byte[] onePacket, int packetLen)
		{
			int len;
			string strInput;
			globaldatabase.productcastinglist.productcastinglist_t? productcasting;

			//MIN_PACKET_LEN include one byte of data, so we need to delete this byte
			len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;
			strInput = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);

			productcasting = globaldatabase.productcastinglist.parseinput(strInput);
			if (productcasting == null){
				return RESULT_ERR_DATA;
			}
			return globaldatabase.productcastinglist.writerecord(productcasting.Value);
		}

		private int setDispatchFinished(string dName, byte[] onePacket, int packetLen)
		{
			int len;
			string strInput;
			gVariable.dispatchSheetStruct dispatchImpl;
			string[] input;

			//MIN_PACKET_LEN include one byte of data, so we need to delete this byte
			len = packetLen - communicate.MIN_PACKET_LEN_MINUS_ONE;
			strInput = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
			
			input = strInput.Split(';');

			dispatchImpl.barCode = input[dispatch_barcode]

			mySQLClass.writeDataToDispatchListTable(dName, gVariable.dispatchCurrentIndexTableName, )

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

				//原料出库区的搬运工获取物料清单
				case COMMUNICATION_TYPE_WAREHOUE_OUT_START:
					getMaterialInfoForAllMachines();
					sendMaterialInfoToPrintSW();
					break;

				//一铲车的原料出库了, 存储在globaldatabase.materialinoutrecord，同时产生铲车标签
				case COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE:
				//一铲车的余料回收入库了, 扫描铲车标签信息，存储在globaldatabase.materialinoutrecord
				case COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE:
					result = setMaterialWareInOut(onePacket, packetLen);
					m_ClientThread.sendResponseOKBack(result);
					break;

				//每一班流延设备工人上班后（本地PC会显示本地记录的上班次的情况），申请工单（工单在每个工人下班后
				//就结束了，此时机器还在运行，一个标准大卷还未完成，该卷算在下个班次。从start，生产出的大卷标签上
				//传，到下班填上交接班记录，该工单在数据库中(O_dispatchList就完整的封闭了（开始时间，结束时间，交接备注等）。
				case COMMUNICATION_TYPE_CAST_PROCESS_START:
					string dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
					sendDispatchToClient(dName, COMMUNICATION_TYPE_CAST_PROCESS_START, onePacket, packetLen);
					break;
				case COMMUNICATION_TYPE_CAST_BARCODE_UPLOAD:
					result = setCastBarcode(onePacket, packetLen);
					m_ClientThread.sendResponseOKBack(result);
					break;
				case COMMUNICATION_TYPE_CAST_PROCESS_END:
					string dName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
					result=setDispatchFinished(dName, onePacket, packetLen);
					m_ClientThread.sendResponseOKBack(result);
					break

				case COMMUNICATION_TYPE_PRINT_PROCESS_START:  //印刷设备工人上班了
					break;
				case COMMUNICATION_TYPE_PRINT_BARCODE_UPLOAD:  //完成一个大卷，收到大卷印刷标签
					break;
				case COMMUNICATION_TYPE_SLIT_PROCESS_START:  //分切工上班了
					break;
				case COMMUNICATION_TYPE_SLIT_BARCODE_UPLOAD:  //分切一个小卷完工，收到小卷分切标签
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
	

