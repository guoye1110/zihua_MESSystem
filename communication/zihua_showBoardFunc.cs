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
            public class zihua_showBoardClient
            {
                //communication between server and show board SW
                private const int COMMUNICATION_TYPE_HANDSHAKE = 3;
				private const int COMMUNICATION_TYPE_HEART_BEAT = 0xB3;
                private const int COMMUNICATION_TYPE_PUSH_DATA = 0xD0;
				private const int COMMUNICATION_TYPE_UPDATE_FORMAT = 0xD1;
                //end of communication between server and show board SW

				private const int TOTAL_SHOWBOARD_COUNT = 5;
				//By default, each show board's push time is 30 seconds
				private const int[] SHOWBOARD_PUSHTIME = {30, 30, 30, 30, 30};

                private ClientThread m_ClientThread = null;
                private int m_machineIDForShowBoard;
				private DateTime m_lastPushTime;

				private string[] showBoardTitleString = {"13;8;紫华吹膜车间生产情况电子看板;1:120:0:0:0:1:0:28:0;;;;;;;;",
														 "",
														 "",
														 "",
														 ""};
				
				string[][] showBoardCellStringArray = 
				{
					{"生产车间", "", "班次", "", "", "", "正常生产", "产线缺料", 
					 "当前日期", "", "当前时间", "", "","", "设备故障", "计划停机",
					 "生产线", "产品名称", "生产批次号","客户名称", "计划产量", "实际产量","达成率", "合格率",
					 "一号流延机","", "", "","", "", "","", "", "",
					 "六号中试机","", "", "","", "", "","", "", "",
					 "七号吹膜机","", "", "","", "", "","", "", "",
					 "一号印刷机","", "", "","", "", "","", "", "",
					 "二号印刷机","", "", "","", "", "","", "", "",
					 "三号印刷机","", "", "","", "", "","", "", "",
					 "四号印刷机","", "", "","", "", "","", "", "",
					 "五号柔印机","", "", "","", "", "","", "", ""},
					{""},
					{""},
					{""},
					{""}
				};

				string[][] showBoardCellAttrArray = 
				{
					{"0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0",
					"0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:2:0:18:0", "0:60:240:0:0:3:0:18:0",
					"0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0",
					"0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:1:0:18:0", "0:60:240:0:0:5:0:18:0",
					"1:80:0:0:0:0:0:18:0", "", "", "", "", "", "", "",
					"0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0",
					"0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0",
					"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", "",
					"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", "",
					"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", "",
					"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", "",
					"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", "",
					"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", "",
					"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", "",
					"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", ""},
					{""},
					{""},
					{""},
					{""}
				};
				
				//模拟滚动信息
				string rollingDisplayData = "一二三四五六七八九十，十九八七六五四三二一，一二三四五六七八九十，十九八七六五四三二一，一二三四五六七八九十，十九八七六五四三二一，一二三四五六七八九十，十九八七六五四三二一，一二三四五六七八九十，十九八七六五四三二一，";

				private int get_showboard_index()
				{
					return 0;
				}
				
				private string generateShowBoardData(int index)
				{
					//get current dipatch and output data
				
					//fill production data into showBoardCellStringArray and showBoardCellAttrArray
				
					//merge showBoardTitleString, showBoardCellStringArray, showBoardCellAttrArray and rollingDisplayData together
				}
				private string generateShowBoardFormat(int index)
				{
					//get current dipatch and output data
				
					//fill production data into showBoardCellStringArray and showBoardCellAttrArray
				
					//merge showBoardTitleString, showBoardCellStringArray, showBoardCellAttrArray and rollingDisplayData together
				}

                public zihua_showBoardClient(ClientThread cThread)
                {
                    m_ClientThread = cThread;
					m_machineIDForShowBoard = 0;
                }

				private int send_data(int index)
                {
                	string send_data = generateShowBoardData(index);
					return m_ClientThread.sendStringToClient(send_data, COMMUNICATION_TYPE_PUSH_DATA);
				}

				private int send_format(int index)
				{
					string send_data = generateShowBoardFormat(index);
					return m_ClientThread.sendStringToClient(send_data, COMMUNICATION_TYPE_UPDATE_FORMAT);
				}
				
                //return value: -1: no action
                //				 0: OK
                //				>0: Fail
                private int HandleHandShake(int communicationType, byte[] onePacket, int packetLen)
                {
                	if (communicationType == COMMUNICATION_TYPE_HANDSHAKE_PRINT_MACHINE_ID){
                    	m_machineIDForShowBoard = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100;
						Console.WriteLine("Machine " + m_machineIDForShowBoard + " Handshake!");
						m_ClientThread.handshakeWithClientOK = 1;
                    	return 0;
                	}
					return -1;
                }

				private int HandleHeartBeat(int communicationType, byte[] onePacket, int packetLen)
                {
					if (communicationType == COMMUNICATION_TYPE_HEART_BEAT) {
						int index = get_showboard_index();
						int result = DateTime.Compare(DateTime.Now, m_lastPushTime.AddSeconds(SHOWBOARD_PUSHTIME[index]));
						if (result>0) {
							send_data(index);
							m_lastPushTime = DateTime.Now;
						}
					}
				}

                public void processShowBoardFunc(int communicationType, byte[] onePacket, int packetLen)
                {
                    int result;
                    string dispatchCode=null, productCode=null;
                    //int printingSWPCID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 - CAST_PROCESS_PC1 + gVariable.castingProcess[0];

                    try
                    {
                        result = HandleHandShake(communicationType, onePacket, packetLen);
                        if (result >= 0) {
							m_ClientThread.sendResponseOKBack(result);

							//Send the format and data first time
							int index = get_showboard_index();
							send_format(index);
							send_data(index);
							m_lastPushTime = DataTime.Now;
                        }

                    	result = HandleHeartBeat(communicationType, onePacket, packetLen);
						if (result >= 0) m_ClientThread.sendResponseOKBack(result);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("processShowBoardFunc(" + communicationType + "," + packetLen + ") for printingMachineID = " + m_machineIDForShowBoard + "failed, " + ex);
                    }
                }
            }
        }
    }
}
	

