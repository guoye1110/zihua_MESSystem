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
                private int[] m_pushTimeInSeconds = new int[TOTAL_SHOWBOARD_COUNT] { 30, 30, 30, 30, 30 };

                private ClientThread m_ClientThread = null;
                private int m_machineIDForShowBoard;
				private DateTime m_lastPushTime;

				/*private string[] showBoardTitleString = {"13;8;紫华吹膜车间生产情况电子看板;1:120:0:0:0:1:0:28:0;;;;;;;;",
														 "",
														 "",
														 "",
														 ""};*/
				
				string[][,] showBoardCellStringArray_Template = new string[TOTAL_SHOWBOARD_COUNT][,]
				{
					new string[,]{
						         {"紫华吹膜车间生产情况电子看板", "", "", "", "", "", "", ""},
								 {"生产车间", "",         "班次",      "",         "",         "",        "正常生产", "产线缺料"}, 
					 			 {"当前日期", "<Date>",         "当前时间",  "<Time>",         "",         "",        "设备故障", "计划停机"},
					 			 {"",         "", "", "", "", "", "", ""},
					 			 {"生产线",   "产品名称", "生产批次号","客户名称", "计划产量", "实际产量","达成率",   "合格率"},
								 {"1#印刷机", "", "", "", "", "", "", ""},
								 {"2#印刷机", "", "", "", "", "", "", ""},
								 {"3#印刷机", "", "", "", "", "", "", ""},
								 {"4#印刷机", "", "", "", "", "", "", ""},
								 {"5#分切机", "", "", "", "", "", "", ""},
								 {"7#分切机", "", "", "", "", "", "", ""}
								 },
					new string[,]{{""}},
					new string[,]{{""}},
					new string[,]{{""}},
					new string[,]{{""}}
				};

				string[][,] showBoardCellAttrArray_Template = new string[TOTAL_SHOWBOARD_COUNT][,]
				{
					new string[,]{
								 {"1:120:0:0:0:1:0:28:0",  "", "", "", "", "", "", ""},
								 {"0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:2:0:18:0", "0:60:240:0:0:3:0:18:0"},
								 {"0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:1:0:18:0", "0:60:240:0:0:5:0:18:0"},
								 {"1:80:0:0:0:0:0:18:0",   "", "", "", "", "", "", ""},
								 {"0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0", "0:60:240:0:0:0:0:18:0"},
								 {"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", ""},
								 {"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", ""},
								 {"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", ""},
								 {"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", ""},
								 {"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", ""},
								 {"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", ""},
								 {"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", ""},
								 {"0:60:240:0:0:0:0:18:0", "", "", "", "", "", "", ""}
								 },
					new string[,]{{""}},
					new string[,]{{""}},
					new string[,]{{""}},
					new string[,]{{""}}
				};
				
				//模拟滚动信息
				string rollingDisplayData = "一二三四五六七八九十，十九八七六五四三二一，一二三四五六七八九十，十九八七六五四三二一，一二三四五六七八九十，十九八七六五四三二一，一二三四五六七八九十，十九八七六五四三二一，一二三四五六七八九十，十九八七六五四三二一，";

				//获取machineID或固定IP与5个看板的对应关系
				private int get_showboard_index()
				{
					return 0;
				}
				
				private string generateShowBoardData(int index)
				{
					int row,col;
                    string[][,] showBoardCellStringArray = (string[][,])showBoardCellStringArray_Template.Clone();

					//get current dipatch and output data
				
					//fill production data into showBoardCellStringArray and showBoardCellAttrArray
					
				
					//merge showBoardTitleString, showBoardCellStringArray, showBoardCellAttrArray and rollingDisplayData together
					string str1 = null;

					//行数+列数
					str1 += showBoardCellStringArray[index].GetLength(0) + ";" + showBoardCellStringArray[index].GetLength(1) + ";";
					for (row=0;row<showBoardCellStringArray[index].GetLength(0);row++) {
						for(col=0;col<showBoardCellStringArray[index].GetLength(1);col++) {
							if (showBoardCellStringArray[index][row,col] == "<Date>")
								str1 += DateTime.Now.ToString("yyyy-MM-dd");
							else if (showBoardCellStringArray[index][row,col] == "<Time>")
								str1 += DateTime.Now.ToString("HH:mm");
							else 
								str1 += showBoardCellStringArray[index][row,col];
							if (row != showBoardCellStringArray[index].GetLength(0) || col != showBoardCellStringArray[index].GetLength(1))
								str1 += ";";
						}
					}
                    return (str1.Length +  ";" + 0 + ";" + 0 + ";" + str1);
				}

				private string generateShowBoardFormat(int index)
				{
					int row,col;
                    string[][,] showBoardCellAttrArray = (string[][,])showBoardCellAttrArray_Template.Clone();

					//get current dipatch and output data
				
					//fill production data into showBoardCellStringArray and showBoardCellAttrArray
				
					//merge showBoardTitleString, showBoardCellStringArray, showBoardCellAttrArray and rollingDisplayData together
					string str1 = null;

					//行数+列数
					str1 += showBoardCellAttrArray[index].GetLength(0) + ";" + showBoardCellAttrArray[index].GetLength(1) + ";";
					for (row=0;row<showBoardCellAttrArray[index].GetLength(0);row++) {
						for(col=0;col<showBoardCellAttrArray[index].GetLength(1);col++) {
							str1 += showBoardCellAttrArray[index][row,col];
							if (row != showBoardCellAttrArray[index].GetLength(0) || col != showBoardCellAttrArray[index].GetLength(1))
								str1 += ";";
						}
					}
                    return (str1.Length +  ";" + 0 + ";" + 0 + ";" + str1);
				}

                public zihua_showBoardClient(ClientThread cThread)
                {
                    m_ClientThread = cThread;
					m_machineIDForShowBoard = 0;
                }

				private int send_data(int index)
                {
                	string send_data = generateShowBoardData(index);
					m_ClientThread.sendStringToClient(send_data, COMMUNICATION_TYPE_PUSH_DATA);
                    return 0;
				}

				private int send_format(int index)
				{
					string send_data = generateShowBoardFormat(index);
					m_ClientThread.sendStringToClient(send_data, COMMUNICATION_TYPE_UPDATE_FORMAT);
                    return 0;
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
                        return 0;
					}
                    return -1;
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
							m_lastPushTime = DateTime.Now;
                        }

                    	result = HandleHeartBeat(communicationType, onePacket, packetLen);
                        if (result >= 0)
                        {
                            m_ClientThread.sendResponseOKBack(result);

                            //Send data
                            int index = get_showboard_index();
                            result = DateTime.Compare(DateTime.Now, m_lastPushTime.AddSeconds(m_pushTimeInSeconds[index]));
                            if (result > 0)
                            {
                                send_data(index);
                                m_lastPushTime = DateTime.Now;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("processShowBoardFunc(" + communicationType + "," + packetLen + ") for ShowBoardMachineID = " + m_machineIDForShowBoard + "failed, " + ex);
                    }
                }
            }
        }
    }
}
	

