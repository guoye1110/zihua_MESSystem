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
            string clientAccount;

            public void sendInstructionToClient()
            {
                int i;
                int alarmIDInTable;
                string databaseName;

                try
                {
                    while (true)
                    {
                        while (true)
                        {
                            //waiting for alarm flag
                            if (gVariable.activeAlarmInfoUpdatedCounterpart == 1)
                                break;

                            toolClass.nonBlockingDelay(1000);

                            //if we found this is a board/app connection instead of server/client PC connection, we should exit from this thread
                            if (sendInstructionToClientFlag == 0)
                                break;
                        }

                        if (sendInstructionToClientFlag == 0)
                            break;

                        for(i = 0; i < gVariable.activeAlarmTotalNumber; i++)
                        {
                            if (gVariable.activeAlarmNewStatus[i] != gVariable.ACTIVE_ALARM_OLD_ALARM)  //need to tell client PC
                            {
                                databaseName = gVariable.activeAlarmDatabaseNameArray[i];
                                alarmIDInTable = gVariable.activeAlarmIDArray[i];

                                if (gVariable.activeAlarmNewStatus[i] == gVariable.ACTIVE_ALARM_NEW_ARRIVED)  
                                {
                                    //this is a new arrived alarm, client PC need to popup this alarm
                                    newAlarmInfoSendToClientPC(databaseName, alarmIDInTable, COMMUNICATION_TYPE_NEW_ALARM_TO_CLIENT);
                                }
                                else
                                {
                                    //this is an old alarm, but content is changed, if it is already on the screen of the client PC, content need to be updated
                                    newAlarmInfoSendToClientPC(databaseName, alarmIDInTable, COMMUNICATION_TYPE_ALARM_UPDATED_TO_CLIENT);
                                }

                                gVariable.activeAlarmNewStatus[i] = gVariable.ACTIVE_ALARM_OLD_ALARM;
                            }
                        }

                        gVariable.activeAlarmInfoUpdatedCounterpart = 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.Write("sendInstructionToClient faile! " + ex);
                }
            }


            //there is only one socket receive function in MESSystem for a host server, after host server got data packet in com.cs, it will make a judgement of where comes this packet by analyzing 
            //communication type, if the data packet comes from a client PC, then com.cs will direct the data packet to the function below for processing
            //every client PC will trigger a new cleintThread class, then a new clientServer function, every cientSever functon will call its processClientInstruction() function during handshake process,
            //so processClientInstruction() is only responsible for one server-client-PC connection 
            void processClientInstruction(int communicationType, int packetLen)
            {
                int len;
                int index; //index in activeAlarmNewStatus and activeAlarmIDArray
                int IDInTable;
                string databaseName;
                string tableName;

                try
                {
                    tableName = gVariable.alarmListTableName;
                    switch (communicationType)
                    {
                        case COMMUNICATION_TYPE_CLIENT_PC_HANDSHAKE:
                            len = packetLen - MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                            clientAccount = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                            //got handshake packet from client PC, response OK
                            onePacket[PROTOCOL_DATA_POS] = 0;
                            len = MIN_PACKET_LEN;
                            sendDataToClientPC(onePacket, len, COMMUNICATION_TYPE_CLIENT_PC_HANDSHAKE);

                            handshakeWithClientOK = 1;

                            newPCClientConnectedOrDisaconnected(clientAccount, gVariable.PC_CLIENT_CONNECTED);
                            break;
                        case COMMUNICATION_TYPE_CLIENT_DISCONNECTED:
                            //the PC client want to disconnect, don't try to response to this request, simply disconnect
                            newPCClientConnectedOrDisaconnected(clientAccount, gVariable.PC_CLIENT_DISCONNECTED);
                            break;
                        case COMMUNICATION_TYPE_NEW_ALARM_TO_CLIENT:
                            //client PC got this data correctly
                            break;
                        case COMMUNICATION_TYPE_ALARM_UPDATED_TO_SERVER:
                            len = packetLen - MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                            databaseName = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS + 2, len - 2);
                            IDInTable = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100;

                            index = toolClass.getIndexInActiveAlarmArray(databaseName, IDInTable);
                            if (index >= 0)  //this alarm is active in host server
                            {
                                alarmTableStructImpl = mySQLClass.getAlarmTableContent(databaseName, tableName, IDInTable);

                                //put all related info into the popup alarm on screen
                                SetAlarmClass.activeAlarmInstanceArray[index].SetAlarmDataOnScreen(alarmTableStructImpl.signer, alarmTableStructImpl.time1, alarmTableStructImpl.completer, alarmTableStructImpl.time2,
                                                                                                   alarmTableStructImpl.status, alarmTableStructImpl.mailList, alarmTableStructImpl.discuss, alarmTableStructImpl.solution);

                                gVariable.activeAlarmNewStatus[index] = gVariable.ACTIVE_ALARM_STATUS_CHANGED;  //this is an active alarm, client PC modified its content, we need to update it on screen
                                gVariable.activeAlarmInfoUpdatedLocally = 1;

                                //this is a host server, we need to inform all our client PCs about this
                                gVariable.activeAlarmInfoUpdatedCounterpart = 1;
                            }
                            else
                            {
                                //this alarm is not in active mode(not displayed on screen), donot do anything
                            }

                            break;
                        case COMMUNICATION_TYPE_ALARM_UPDATED_TO_CLIENT:
                            break;
                        case COMMUNICATION_TYPE_CLIENT_HEART_BEAT:
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("processClientInstruction(" + communicationType + "," + packetLen + ") for myBoardIndex " + myBoardIndex + "failed, " + ex);
                }
            }


            public static void newPCClientConnectedOrDisaconnected(string account, int connectedOrDisconnected)
            {
                int i, j;

                gVariable.clientPCConnectionInfoUpdatedMutex.WaitOne();

                if (connectedOrDisconnected == gVariable.PC_CLIENT_CONNECTED)
                {
                    gVariable.clientPCAccountListArray[gVariable.clientPCConnectionNumber] = account;
                    gVariable.clientPCConnectionNumber++;
                }
                else //if(connectedOrDisconnected == gVariable.PC_CLIENT_DISCONNECTED)
                {
                    for (i = 0; i < gVariable.clientPCConnectionNumber; i++)
                    {
                        if (gVariable.clientPCAccountListArray[i] == account)
                            break;
                    }

                    if (i < gVariable.clientPCConnectionNumber)
                    {
                        //remove this account and all the accont after this simply move one position ahead 
                        for (j = i; j < gVariable.clientPCConnectionNumber - 1; j++)
                            gVariable.clientPCAccountListArray[j] = gVariable.clientPCAccountListArray[j + 1];
                    }
                    gVariable.clientPCConnectionNumber--;
                }

                gVariable.clientPCConnectionInfoUpdatedMutex.ReleaseMutex();
            }

            public void newAlarmInfoSendToClientPC(string databaseName, int alarmIDInTable, int communicationType)
            {
                int i;
                byte[] machineName;
                byte[] alarmInfo;

                try
                {
                    machineName = System.Text.Encoding.Default.GetBytes(databaseName);
                    alarmInfo = new byte[machineName.Length + MIN_PACKET_LEN_MINUS_ONE + 2];

                    //alarmInfo[PROTOCOL_COMMUNICATION_TYPE_POS] = (byte)communicationType;

                    alarmInfo[PROTOCOL_DATA_POS] = (byte)alarmIDInTable;
                    alarmInfo[PROTOCOL_DATA_POS + 1] = (byte)(alarmIDInTable / 0x100);

                    for (i = 0; i < machineName.Length; i++)
                        alarmInfo[PROTOCOL_DATA_POS + 2 + i] = machineName[i];

                    sendDataToClientPC(alarmInfo, machineName.Length + MIN_PACKET_LEN_MINUS_ONE + 2, communicationType);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("newAlarmInfoSendToClientPC failed, maybe client exited. " + ex);
                }
            }

            public void sendDataToClientPC(byte[] onePacket, int len, int type)
            {
                try
                {
                    onePacket[0] = (byte)'w';
                    onePacket[1] = (byte)'I';
                    onePacket[2] = (byte)'F';
                    onePacket[3] = (byte)'i';
                    onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = (byte)type;
                    onePacket[PROTOCOL_LEN_POS] = (byte)(len % 0x100);
                    onePacket[PROTOCOL_LEN_POS + 1] = (byte)(len / 0x100);
                    toolClass.addCrc32Code(onePacket, len);
                    clientSocketInServer.Send(onePacket, len, 0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("sendDataToClientPC failed, maybe client exited. " + ex);
                }
            }
        }
    }
}
