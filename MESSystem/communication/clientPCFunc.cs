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
        public const int MAX_CLINT_DATA_PACKET_LEN = 10000;

        public static int handshankToServerOK;
        public static Socket clientPCSocket;

        public static void clientPCFuncion()
        {
            int len;
//            int index;
//            int alarmIDInTable;
            int recCount;
            int checkLeftCount;
            int communicationType;

//            string databaseName;
//            string str;

            IPAddress HostIP;
            IPEndPoint point;
            int portNum;
            byte [] receiveBytes = new byte[MAX_CLINT_DATA_PACKET_LEN];  //received data, maybe part of packet
            byte [] checkBytes = new byte[MAX_RECEIVE_DATA_LEN];   //packet need to be processed, start from first byte of a packet

            portNum = 8899;

            try
            {
                //this thread shiuld never exit, so we add a dead cycle here, when connection get lost, just come to this place and continue with connecting
                while (true)
                {
                    handshankToServerOK = 0;

                    //get the IP address of the server PC
                    HostIP = IPAddress.Parse(gVariable.hostString);

                    point = new IPEndPoint(HostIP, portNum);
                    clientPCSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    clientPCSocket.Connect(point);

                    if (clientPCSocket.Connected)
                    {
                        //save client socket, we will need to close socket when client PC exit, so host server konws that client PC exit this application
                        gVariable.clientSocket = clientPCSocket;
                        checkLeftCount = 0;

                        handshakToServer();

                        //keep on receiving and sending new data from/to server after handshake
                        while (true)
                        {
                            recCount = clientPCSocket.Receive(receiveBytes, receiveBytes.Length, 0);
                            communicationType = receiveBytes[PROTOCOL_COMMUNICATION_TYPE_POS];

                            //according to TCP/IP protocol, this 0 means the counter part broke the connection abruptly, so we just exit this thread
                            if (recCount == 0)
                                break;

                            if (checkLeftCount + recCount >= MAX_RECEIVE_DATA_LEN)
                                Console.Write("receive buf overflow left " + checkLeftCount + "; new: " + recCount + " !! \r\n");

                            Array.Copy(receiveBytes, 0, checkBytes, checkLeftCount, recCount);
                            checkLeftCount += recCount;

                            //try to see if the all current instructions in checkBytes have been processed completely, but normally there is only instruction 
                            while (checkLeftCount > 0)
                            {
                                len = checkBytes[PROTOCOL_LEN_POS] + checkBytes[PROTOCOL_LEN_POS + 1] * 0x100;

                                processServerInstruction(checkBytes, len);

                                checkLeftCount -= len;

                                if (checkLeftCount != 0)
                                    Array.Copy(checkBytes, len, checkBytes, 0, checkLeftCount);
                                else //if (checkLeftCount == 0)
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("通讯失败，可能服务器端未启动或者 ..\\..\\init\\host.ini 中输入的 IP 不是服务器的 IP 地址。", "信息提示", MessageBoxButtons.OK);
                gVariable.clientSocket = null;
                return;
            }
        }


        static void processServerInstruction(byte[] checkBytes, int packetLen)
        {
            int len;
            int index;
            int alarmIDInTable;
            int communicationType = 0;
            string databaseName;
            string tableName;
            gVariable.alarmTableStruct alarmTableStructImpl;

            byte[] onePacket = new byte[MAX_CLINT_DATA_PACKET_LEN];

            try
            {
                tableName = gVariable.alarmListTableName;
                communicationType = checkBytes[PROTOCOL_COMMUNICATION_TYPE_POS];

                switch (communicationType)
                {
                    case COMMUNICATION_TYPE_CLIENT_PC_HANDSHAKE:
                        handshankToServerOK = 1;
                        break;
                    case COMMUNICATION_TYPE_NEW_ALARM_TO_CLIENT:
                        //first 2 bytes are alarmIDInTable, databaseName starts from the third byte
                        alarmIDInTable = checkBytes[PROTOCOL_DATA_POS] + checkBytes[PROTOCOL_DATA_POS + 1] * 0x100;
                        databaseName = System.Text.Encoding.Default.GetString(checkBytes, PROTOCOL_DATA_POS + 2, packetLen - MIN_PACKET_LEN_MINUS_ONE - 2);

                        //got new alarm packet from server host, response OK
                        onePacket[PROTOCOL_DATA_POS] = 0;
                        len = MIN_PACKET_LEN;
                        sendDataToServer(onePacket, len, COMMUNICATION_TYPE_NEW_ALARM_TO_CLIENT);

                        toolClass.processNewAlarm(databaseName, alarmIDInTable);
                        break;
                    case COMMUNICATION_TYPE_ALARM_UPDATED_TO_CLIENT:
                        //first 2 bytes are alarmIDInTable, databaseName starts from the third byte
                        alarmIDInTable = checkBytes[PROTOCOL_DATA_POS] + checkBytes[PROTOCOL_DATA_POS + 1] * 0x100;
                        databaseName = System.Text.Encoding.Default.GetString(checkBytes, PROTOCOL_DATA_POS + 2, packetLen - MIN_PACKET_LEN_MINUS_ONE - 2);

                        //got alarm updated packet from server host, response OK
                        onePacket[PROTOCOL_DATA_POS] = 0;
                        len = MIN_PACKET_LEN;
                        sendDataToServer(onePacket, len, COMMUNICATION_TYPE_ALARM_UPDATED_TO_CLIENT);

                        index = toolClass.getIndexInActiveAlarmArray(databaseName, alarmIDInTable);
                        if (index >= 0)  //this alarm is active on screen, try to add new content/status to this alarm window
                        {
                            alarmTableStructImpl = mySQLClass.getAlarmTableContent(databaseName, tableName, alarmIDInTable);

                            //put all related info into the popup alarm on screen
                            SetAlarmClass.activeAlarmInstanceArray[index].SetAlarmDataOnScreen(alarmTableStructImpl.signer, alarmTableStructImpl.time1, alarmTableStructImpl.completer, alarmTableStructImpl.time2,
                                                                                               alarmTableStructImpl.status, alarmTableStructImpl.mailList, alarmTableStructImpl.discuss, alarmTableStructImpl.solution);

                            gVariable.activeAlarmNewStatus[index] = gVariable.ACTIVE_ALARM_STATUS_CHANGED;  //this is an active alarm, client PC modified its content, we need to update it on screen
                            gVariable.activeAlarmInfoUpdatedLocally = 1;  //we don't need to tell counterpart, because we got this info from the host
                            
                        }
                        break;
                    case COMMUNICATION_TYPE_CLIENT_HEART_BEAT:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("processserverInstruction with communication type of " + communicationType + ", packet len of " + packetLen + "failed, " + ex);
            }
        }

        private static void handshakToServer()
        {
            sendDataToServerByString(gVariable.userAccount, COMMUNICATION_TYPE_CLIENT_PC_HANDSHAKE);
        }

        private static void sendDataToServerByString(string strData, int type)
        {
            int i;
            int len;
            byte[] buf;
            byte[] sendBuf = new byte[MAX_CLINT_DATA_PACKET_LEN];

            try
            {
                buf = System.Text.Encoding.Default.GetBytes(strData);
                len = PROTOCOL_HEADER_LEN + buf.Length + PROTOCOL_CRC_LEN;

                sendBuf[0] = 0x77;
                sendBuf[1] = 0x49;
                sendBuf[2] = 0x46;
                sendBuf[3] = 0x69;
                sendBuf[PROTOCOL_LEN_POS] = (byte)(len % 0x100);
                sendBuf[PROTOCOL_LEN_POS + 1] = (byte)(len / 0x100);
                sendBuf[PROTOCOL_COMMUNICATION_TYPE_POS] = (byte)(type);

                for (i = 0; i < buf.Length; i++)
                    sendBuf[PROTOCOL_DATA_POS + i] = buf[i];

                toolClass.addCrc32Code(sendBuf, len);
                clientPCSocket.Send(sendBuf, len, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("通讯失败，可能服务器端未启动。", "信息提示", MessageBoxButtons.OK);
                return;
            }
        }

        private static void sendDataToServer(byte[] onePacket, int len, int type)
        {
            onePacket[0] = (byte)'w';
            onePacket[1] = (byte)'I';
            onePacket[2] = (byte)'F';
            onePacket[3] = (byte)'i';
            onePacket[PROTOCOL_LEN_POS] = (byte)(len % 0x100);
            onePacket[PROTOCOL_LEN_POS + 1] = (byte)(len / 0x100);
            onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = (byte)(type);
            toolClass.addCrc32Code(onePacket, len);
            clientPCSocket.Send(onePacket, len, 0);
        }

        public static void updatedAlarmInfoToServerPC(string databaseName, int alarmIDInTable)
        {
                int i;
                byte[] machineName;
                byte[] alarmInfo;

                machineName = System.Text.Encoding.Default.GetBytes(databaseName);
                alarmInfo = new byte[machineName.Length + MIN_PACKET_LEN_MINUS_ONE + 2];

                alarmInfo[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_ALARM_UPDATED_TO_SERVER;

                alarmInfo[PROTOCOL_DATA_POS] = (byte)alarmIDInTable;
                alarmInfo[PROTOCOL_DATA_POS + 1] = (byte)(alarmIDInTable / 0x100);

                for(i = 0; i < machineName.Length; i++)
                    alarmInfo[PROTOCOL_DATA_POS + 2 + i] = machineName[i];

                sendDataToServer(alarmInfo, machineName.Length + MIN_PACKET_LEN_MINUS_ONE + 2, COMMUNICATION_TYPE_ALARM_UPDATED_TO_SERVER);
        }
    }
}
