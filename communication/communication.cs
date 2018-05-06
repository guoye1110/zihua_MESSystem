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
        public partial class ClientThread
        {
            //int statusAfterDispatchOK;
            beatCalculation beatCalculationImpl;
            int sendInstructionToClientFlag;
            Thread sendInstructionToClientThread;

            public void ClientServer()
            {
                int i, j, fixerrFlag;
                int retry;
                int communicationType;
                //int previousCommunicationType;  //used to record the communication type of the previous packet
                int len;  //temprary length value, may be cahnged any time
                int packetLen;  //used to record the length of the current packet, won't be change until next packet comes
                string[] IPArray;
                System.DateTime timeStampNow;
                byte[] responseFail = new byte[RESPONSE_LEN];

                i = 0;
                len = 0;
                checkLeftCount = 0;
                myBoardIndex = -1;
                myBoardID = -1;
                myBoardIDFromIPAddress = -1;
                workingTimePoints = 0;  //current value larger than threshold of gVariable.beatPeriodInfo[myBoardIndex].idleCurrentHigh, record this value to see how long this machine stays in working mode 
                powerConsumed = 0;
                dispatchAppearIndex = 0;
                beatTimeStamp = 0;
                dispatchStartTimeStamp = 0;
                dataPacketIndex = 0;
//                checkHeartBeatThread = null;

                try
                {
                    sendInstructionToClientFlag = 1;
                    sendInstructionToClientThread = null;
                    if (gVariable.thisIsHostPC == true)
                    {
                        sendInstructionToClientFlag = 1;
                        sendInstructionToClientThread = new Thread((sendInstructionToClient));
                        sendInstructionToClientThread.Start();
                    }
                    beatCalculationImpl = new beatCalculation();  //for beat function

                    //TCP server got a new client connected, we will try to identify which board is connecting, first get its IP, then from the 4th byte of the 
                    //IP address, we get its board index. It is possible that this board index is not correct, since sometimes we may set this ID to a dynamic
                    //IP address, then we will set board ID to -1, and wait for handshape packet to get its board ID.
                    //dynamic ID will be 180 and above, board ID ranges from 1 to 135, so there won't be any conflict
                    IPArray = clientSocketInServer.RemoteEndPoint.ToString().Split('.', ':');
                    myBoardID = Convert.ToInt32(IPArray[3]);

                    Console.WriteLine(DateTime.Now.ToString() + ":new client tries to connect with IP:" + IPArray[0] + "." + IPArray[1] + "." + IPArray[2] + "." + IPArray[3] + 
                                      "; handle: " + clientSocketInServer.Handle + " on: " + DateTime.Now.ToString());


                    j = Convert.ToInt32(IPArray[0]);

                    //we only support this function when host PC is our real server, IP is 172.30.2.24
                    if (j == 172 && myBoardID > 0 && myBoardID <= gVariable.maxMachineNum)
                    {
                        //Console.WriteLine("Thread ID " + threadIndex + " with new board " + myBoardID + " TCP/IP connection OK on " + DateTime.Now.ToString());

                        myBoardIndex = myBoardID - 1;
                        databaseNameThis = gVariable.internalMachineName[myBoardIndex];
                        gVariable.machineStatus[myBoardIndex].machineCode = gVariable.machineCodeArrayDatabase[myBoardIndex]; //databaseNameThis;
                        gVariable.machineStatus[myBoardIndex].machineName = gVariable.machineNameArrayDatabase[myBoardIndex];
                        gVariable.socketArray[myBoardIndex] = clientSocketInServer;

                        //if machine status is started, we believe the board restarted for some reason, just keep in this started status
                        if (gVariable.machineCurrentStatus[myBoardIndex] != gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                        {
                            gVariable.currentDataIndex[DATA_TYPE_VOL_CUR_DEVICE] = 0;

                            beatCalculationImpl.beatInitBVariables(myBoardIndex);

                            gVariable.machineStartTime[myBoardIndex] = DateTime.Now.ToString();
                            timeStampNow = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now);
                            gVariable.machineStartTimeStamp[myBoardIndex] = (int)(timeStampNow - gVariable.worldStartTime).TotalSeconds;
                            gVariable.connectionStatus[myBoardIndex] = 0;
                            gVariable.connectionCount[myBoardIndex] = 0;
                            gVariable.connectionCount_old[myBoardIndex] = 0;
                            gVariable.machineCurrentStatus[myBoardIndex] = gVariable.MACHINE_STATUS_DISPATCH_DUMMY;
                        }

                        if (gVariable.debugMode == 2)
                        {
                            gVariable.dataLogWriter[myBoardIndex].WriteLine(" ----" + DateTime.Now.ToString() + gVariable.internalMachineName[myBoardIndex] + " TCP/IP connection OK");
                            gVariable.infoWriter.WriteLine(" ----" + DateTime.Now.ToString() + ": " + gVariable.internalMachineName[myBoardIndex] + " TCP/IP connection OK");
                        }
                    }
                    else
                    {
                        myBoardID = -1;
                        //Console.WriteLine("New client thread started on: " + DateTime.Now.ToString() + ", try to locate it!!");
                    }

                    retry = 0;
                    boardIDNotSetFlag = 0;
                    qualityDataSN = 0;

                    //previousCommunicationType = -1;

                    while (gVariable.willClose == 0)
                    {
                        try
                        {
                            //if there are setting from PC, and it is for this board, we need to deal with this info 
                            //whereComesTheSettingData < 0 means setting data is in default mode(from touchpad), just move on
                            //whereComesTheSettingData == myBoardIndex means setting data are for this board
                            //this function only works when no dispatch is undergoing
                            if (gVariable.whereComesTheSettingData >= 0 && gVariable.whereComesTheSettingData == myBoardIndex && gVariable.machineCurrentStatus[myBoardIndex] <= gVariable.MACHINE_STATUS_DISPATCH_COMPLETED && myBoardIndex != -1)
                            {
                                if (gVariable.whatSettingDataModified > gVariable.NO_SETTING_DATA_TO_BOARD && gVariable.whatSettingDataModified <= gVariable.BEAT_SETTING_DATA_TO_BOARD)
                                {
                                    writeSettingsToBoard(gVariable.whatSettingDataModified);

                                    //if ADC setting data changed, we need to redo curve data initialization
                                    if (gVariable.whatSettingDataModified == gVariable.ADC_SETTING_DATA_TO_BOARD)
                                    {
                                        gVariable.whatSettingDataModified = gVariable.NO_SETTING_DATA_TO_BOARD;

                                        //new setting s are already sent to board, now we need PC to implement these settings
                                        toolClass.getDummyData(myBoardIndex);
                                        getSettingCurveData();

                                        mySQLClass.removeDummyDatabaseTable(databaseNameThis);

                                        //we got related data, put it in database in the following functiuon, including all data type and dispatch if they does not exist before
                                        readBoardInfoFromMachineDataStruct(1);

                                        //this flag will be used by multiCurve
                                        gVariable.refreshMultiCurve = 1; //if multiCurve is now on the top screen, refresh it with to default data communication
                                    }
                                    gVariable.whatSettingDataModified = gVariable.NO_SETTING_DATA_TO_BOARD;
                                }

                                gVariable.whereComesTheSettingData = gVariable.SETTING_DATA_FROM_TOUCHPAD;
                            }

                            if (checkLeftCount < MAX_RECEIVE_DATA_LEN - 2 * MAX_PACKET_LEN)
                                recCount = clientSocketInServer.Receive(receiveBytes, receiveBytes.Length - 100, 0);
                            else
                                toolClass.nonBlockingDelay(2000);
                        }
                        catch (Exception ex)
                        {
                            if (myBoardIndex < 0)
                            {
                                Console.WriteLine(DateTime.Now.ToString() + ": a board disconnected without really been connected" + ex);
                                gVariable.infoWriter.WriteLine(DateTime.Now.ToString() + ": a board disconnected without really been connected");
                                break;
                            }
                            else
                            {
                                Console.WriteLine(DateTime.Now.ToString() + ": board " + (myBoardIndex + 1) + " connected at " + gVariable.machineStartTime[myBoardIndex] + ", but now disconnected! " + ex);
                                gVariable.infoWriter.WriteLine(DateTime.Now.ToString() + ": board " + (myBoardIndex + 1) + " connected at " + gVariable.machineStartTime[myBoardIndex] + ", but now disconnected! ");
                            }

                            if (myBoardIndex >= 0 && myBoardIndex < gVariable.maxMachineNum)
                                gVariable.socketArray[myBoardIndex] = null;
                            break;
                        }

                        //gVariable.dataLogWriter[fileIndex].WriteLine("get " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                        if (retry > 0)    //checkpoint should set here, we got data from the board! Flag2!
                            retry--;

                        //                       str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                        //                       Console.WriteLine(str + ": new data received");
                        if (recCount != 0)
                        {
                            if (checkLeftCount + recCount >= MAX_RECEIVE_DATA_LEN)
                                Console.Write("receive buf overflow left " + checkLeftCount + "; new: " + recCount + " !! \r\n");

                            Array.Copy(receiveBytes, 0, checkBytes, checkLeftCount, recCount);
                            checkLeftCount += recCount;
                            //                            Console.WriteLine("receive len = " + recCount + "; total len = " + checkLeftCount);

                            while (gVariable.willClose == 0)
                            {
                                if (checkLeftCount < MIN_PACKET_LEN_MINUS_ONE)   //a complete packet is still not available, some data packet has no data column
                                    break;

                                if (myBoardIndex >= 0 && myBoardIndex < gVariable.maxMachineNum)
                                    gVariable.connectionCount[myBoardIndex]++;

                                packetLen = checkBytes[PROTOCOL_LEN_POS] + checkBytes[PROTOCOL_LEN_POS + 1] * 0x100;
                                //                                Console.WriteLine("new packet len = " + packetLen);

                                if (packetLen <= MAX_PACKET_LEN && packetLen > checkLeftCount)  //one packet is not compete in checkbytes
                                {
                                    if (retry == 0)
                                    {
                                        //we need a long time here to wait for the completion of receiving a long data from the counter part, especially when receiving video from Android App
                                        retry = 10;
                                        break;
                                    }
                                    else if (retry != 1)
                                    {
                                        toolClass.nonBlockingDelay(2);
                                        break;
                                    }
                                }

                                if (gVariable.debugMode == 2)
                                {
//                                    for (j = 0; j < 100; j++)
                                    {
//                                        Console.Write(checkBytes[j].ToString() + ",");
                                    }
//                                    Console.WriteLine(" ");
                                }

                                retry = 0;
                                //if packet length larger than max packet, it is a wrong packet
                                if (checkBytes[0] == 'w' && checkBytes[1] == 'I' && checkBytes[2] == 'F' && checkBytes[3] == 'i' && packetLen <= MAX_PACKET_LEN && toolClass.checkCrc32Code(checkBytes, packetLen) == true)
                                {
                                    try
                                    {
                                        //old data format:
                                        //0-3: header
                                        //4-5: length of the packet
                                        //6: communication type
                                        //7 - 8: previously file name for 8 bytes, currently device type and port type for 2 bytes
                                        //9 - 23: time:160302111120045
                                        //24 - 28: index
                                        //29 - : data
                                        //last 2 bytes: CRC
                                        //len = checkBytes[PROTOCOL_LEN_POS];

                                        //new data format:
                                        //0-3: header
                                        //4-5: length of the packet
                                        //6: communication type
                                        //7 - 18: time:160302111120045
                                        //19 - 22: dataPacketIndex
                                        //23 - 26: reserved
                                        //27: dataType
                                        //28 - : data
                                        //last 4 bytes: CRC

                                        Array.Copy(checkBytes, 0, onePacket, 0, packetLen);
                                        // indexV = Convert.ToInt32(System.Text.Encoding.Default.GetString(checkBytes).Remove(0, 37).Remove(8));

                                        communicationType = onePacket[PROTOCOL_COMMUNICATION_TYPE_POS];

                                        //If handshake is not completed, and this is not a data packet that is for handshake, we ask data collect board to do handshake
                                        if (handshakeWithClientOK == 0 && communicationType != COMMUNICATION_TYPE_START_HANDSHAKE_WITH_ID_TO_PC && communicationType != COMMUNICATION_TYPE_SEND_DUMMY_MACHINE_CODE_TO_PC
                                            && communicationType != COMMUNICATION_TYPE_EMAIL_HEART_BEAT && communicationType != COMMUNICATION_TYPE_HANDSHAKE_PRINT_MACHINE_ID)
                                        {
                                            notReadyForCommunication();
                                            len = checkBytes[PROTOCOL_LEN_POS] + checkBytes[PROTOCOL_LEN_POS + 1] * 0x100;
                                            checkLeftCount -= len;

                                            if (checkLeftCount != 0)
                                            {
                                                Array.Copy(checkBytes, len, checkBytes, 0, checkLeftCount);
                                                continue;
                                            }
                                            else //if (checkLeftCount == 0)
                                                break;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                if (communicationType >= COMMUNICATION_TYPE_EMAIL_HEART_BEAT)
                                                {
                                                    //communication with email forwarder
                                                    processClientInstruction(communicationType, packetLen);
                                                }
                                                else if (communicationType >= COMMUNICATION_TYPE_HANDSHAKE_PRINT_MACHINE_ID)
                                                {
                                                    //communication with label printing PC
                                                    processLabelPrintingFunc(communicationType, packetLen);
                                                }
                                                else if (communicationType >= COMMUNICATION_TYPE_CLIENT_PC)
                                                {
                                                    //communication with client PC
                                                    processClientInstruction(communicationType, packetLen);
                                                }
                                                else if (communicationType >= COMMUNICATION_TYPE_APP_WORKING_BOARD_ID_TO_PC && communicationType <= COMMUNICATION_TYPE_APP_DEVICE_NAME)
                                                {
                                                    //communication with mobile devices
                                                    if (appHandshakeCompleted == 1)
                                                        processClientMobileApp(communicationType, packetLen);
                                                }
                                                else
                                                {
                                                    //communication with data collect board
                                                    processClientDataCollectBoard(communicationType, packetLen);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine("deal with the incoming data failed" + ex);
                                            }
                                            //Console.WriteLine("checkLeftCount = " + checkLeftCount + "; len = " + len);

                                            len = checkBytes[PROTOCOL_LEN_POS] + checkBytes[PROTOCOL_LEN_POS + 1] * 0x100;
                                            checkLeftCount -= len;

                                            if (checkLeftCount != 0)
                                                Array.Copy(checkBytes, len, checkBytes, 0, checkLeftCount);
                                            else //if (checkLeftCount == 0)
                                                break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.Write("deal with the incoming data OK, but prepare for next failed!" + ex);
                                    }
                                }
                                else
                                {   //data packet receive error
                                    try
                                    {

                                        Console.WriteLine(DateTime.Now.ToString() + ":board ID " + (myBoardIndex + 1) + " Got data!!");

                                        gVariable.wifiErrorNum++;
                                        i = 1;

                                        toolClass.addCrc32Code(responseFail, RESPONSE_LEN);
                                        clientSocketInServer.Send(responseFail, RESPONSE_LEN, 0);   //checkBytes[4] is the legnth of this packet
                                        if (gVariable.debugMode == 2 && myBoardIndex >= 0)
                                        {
                                            for (j = 0; j < 100; j++)
                                            {
                                                Console.Write(checkBytes[j].ToString() + ",");
                                            }
                                            Console.WriteLine(" ");
                                            //                                            for (j = 0; j < 100; j++)
//                                            {
//                                                gVariable.errorLogWriter[myBoardIndex].Write(checkBytes[j].ToString() + ",");
//                                            }
                                        }
                                        fixerrFlag = 0;
                                        while (checkBytes[i] != 'w' || checkBytes[i + 1] != 'I' || checkBytes[i + 2] != 'F' || checkBytes[i + 3] != 'i')
                                        {
                                            if (i >= checkLeftCount - 4)
                                            {
                                                Array.Copy(checkBytes, i, checkBytes, 0, checkLeftCount - i);
                                                checkLeftCount -= i;  //discard current byte and search for data header
                                                fixerrFlag = 1;
                                                break;
                                            }
                                            i++;
                                        }
                                        if (fixerrFlag == 1)
                                            break;

                                        checkLeftCount -= i;
                                        Array.Copy(checkBytes, i, checkBytes, 0, checkLeftCount);
                                        continue;
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("deal with the wrong data failed!" + ex);
                                        //send data fail means the connection is not available now, need to redo connection, exit from this thread, TCP will create another connection
                                        //all current data will be discarded 
                                        return;
                                    }
                                }
                            }
                        }
                        else  //length is 0, means TCP/IP disconnected, we simply exit from this communication thread 
                        {
                            Console.WriteLine("length is 0, the client with IP:" + IPArray[0] + "." + IPArray[1] + "." + IPArray[2] + "." + IPArray[3] + ", connected on " + gVariable.machineStartTime[myBoardIndex] + ", now closed");
                            break;
                        }
                    }

                    if (myBoardIndex >= 0 && myBoardIndex < gVariable.maxMachineNum)
                        gVariable.socketArray[myBoardIndex] = null;

                    Console.WriteLine("Connection IP:" + IPArray[0] + "." + IPArray[1] + "." + IPArray[2] + "." + IPArray[3] + "; handle: " + clientSocketInServer.Handle + " closed!");
                }
                catch (Exception ex)
                {
                    Console.Write("connection stopped! board ID is " + (myBoardIndex + 1) + ex);
                    if (myBoardIndex >= 0 && myBoardIndex < gVariable.maxMachineNum)
                        gVariable.socketArray[myBoardIndex] = null;
                }
                clientSocketInServer.Close();
            }
        }

        //the entrance function for communication, will be called by firstScreen function after all initialization finished
        public static void comProccess()
        {
            int i;
            Socket socket;
            string strIPAddr = "";
            string HostName = Dns.GetHostName();
            int portNum = 8899;
            IPAddress ip;
            IPEndPoint ipep;
            IPHostEntry IpEntry = Dns.GetHostEntry(HostName);

            try
            {
                for (i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        strIPAddr = IpEntry.AddressList[i].ToString();
//                        if (strIPAddr.Remove(7) == "172.30.")
//                        if (strIPAddr.Remove(7) == "192.168")
//                        if (strIPAddr.Remove(7) == "10.10.1")
//                        if (strIPAddr == gVariable.communicationHostIP)
                            break;
                    }
                }

                if (i >= IpEntry.AddressList.Length)
                {
                    Console.Write("IPv4 address not found!");
                    return;
                }

                ip = IPAddress.Parse(strIPAddr);
                ipep = new IPEndPoint(ip, portNum);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(ipep);
                socket.Listen(10);

//                if (DateTime.Now.Year > 2017 || (DateTime.Now.Year == 2017 && DateTime.Now.Month >= 12))
//                    return;

                while (true)
                {
                    Socket client = socket.Accept();  // TCP server is now in listening mode, trying to find a client

                    //a new client sent out its first TCP data packet, TCP server knows a new connection is now available
                    //although there is still no real data, only TCP packet  -> Flag1
                    ClientThread newclient = new ClientThread(client);
                    //now move to ClientThread()
                    Thread newthread = new Thread(new ThreadStart(newclient.ClientServer));
                    newthread.Start();
                    toolClass.nonBlockingDelay(80);
                }
            }
            catch (Exception ex)
            {
                Console.Write("create multithread fail!" + ex);
            }
        }
    }
}