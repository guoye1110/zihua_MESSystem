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
            int clientFromTouchpad = 0;
            int boardIDNotSetFlag;  //flag to indicate board ID is null, the user need to set an ID to the board
            int qualityDataSN;
            int myBoardIDFromIPAddress;

            public void processClientDataCollectBoard(int communicationType, int packetLen)
            {
                int i, j;
                int ret;
                string str;
                string updateStr;
                int len;  //temprary length value, may be cahnged any time
                byte[] timeArray = new byte[10];
                byte[] header = new byte[2000];
                byte[] aa = null;
                DateTime now;
                int start;
                System.DateTime timeStampNow;

                header[0] = (byte)'H';
                header[1] = (byte)'M';
                header[2] = (byte)'I';
                header[3] = (byte)'F';

                try
                {
                    switch (communicationType)
                    {
                        case COMMUNICATION_TYPE_START_HANDSHAKE_WITHOUT_ID_TO_PC:  //this case will not happen any more
                            if (boardIDNotSetFlag == 0)
                                MessageBox.Show("这块数据采集板没有 ID，请换用含有合法 ID 的数据采集板，谢谢！", "信息提示", MessageBoxButtons.OK);
                            boardIDNotSetFlag = 1;
                            break;
                        case COMMUNICATION_TYPE_START_HANDSHAKE_WITH_ID_TO_PC:////the really Minyi handshake data packet, will show board ID. Flag3!!!!!
                            //at this point with no handshake processed, myBoardID should be obtained from IP address when the first packet is received
                            myBoardIDFromIPAddress = myBoardID;

                            //sendInstructionToClientThread should work only in client PC
                            if (sendInstructionToClientThread != null)
                            {
                                sendInstructionToClientFlag = 0;
                                sendInstructionToClientThread = null;
                            }

                            myBoardID = onePacket[PROTOCOL_DATA_POS] + onePacket[PROTOCOL_DATA_POS + 1] * 0x100 + onePacket[PROTOCOL_DATA_POS + 2] * 0x10000 + onePacket[PROTOCOL_DATA_POS + 3] * 0x1000000;
                            if (myBoardID >= gVariable.ID_OTHERTHAN_TOUCHPAD)  //this is a mobile phone or PDA device that runs an App to talk with PC host for the first time, this is an handshake package
                            {
                                myBoardID -= gVariable.ID_OTHERTHAN_TOUCHPAD;
                                myBoardIndex = myBoardID - 1;
                                appHandshakeCompleted = 1;
                                clientFromTouchpad = 0;
                                numOfCraftDataInTable = 0;
                                databaseNameThis = gVariable.internalMachineName[myBoardIndex];

                                //this is a app's bug, when an app exit dispatch detail screen, it will send out this instruction, but we should not restart handshake here, just ignore this instruction
                                if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED)
                                    break;
                            }
                            else if (myBoardID == BOARD_ID_EMAIL_FORWARDER)
                            {
                                gVariable.emailForwarderHeartBeatNum = 1;
                                gVariable.emailForwarderSocket = clientSocketInServer;

                                onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_EMAIL_FORWARDER;
                                //respond to email forwarder 
                                onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                                len = MIN_PACKET_LEN;
                                sendDataToClient(onePacket, len, COMMUNICATION_TYPE_EMAIL_FORWARDER);
                                break;
                            }
                            else if (myBoardID >= gVariable.ID_APPS_NOT_INIT_DATA)
                            {
                                //an App want to talk with PC host, although this is an handshake requirement, but we should not do init of parameters since this is an App bug during communication
                                myBoardID -= gVariable.ID_APPS_NOT_INIT_DATA;
                                myBoardIndex = myBoardID - 1;
                                appHandshakeCompleted = 1;
                                clientFromTouchpad = 0;
                                databaseNameThis = gVariable.internalMachineName[myBoardIndex];
                                break;
                            }
                            else  //new board
                            {
                                if (gVariable.CompanyIndex == gVariable.ZIHUA_ENTERPRIZE)
                                {
                                    if (myBoardID > gVariable.maxMachineNum)
                                    {
                                        Console.WriteLine("board ID:" + myBoardID + " exceeds the max value!!!");
                                        return;
                                    }
                                }
                                else
                                {
                                }
                                myBoardIndex = myBoardID - 1;

                                clientFromTouchpad = 1;
                                numOfCraftDataInTable = 0;

                                if (myBoardIDFromIPAddress != myBoardID)
                                    Console.WriteLine("board ID:" + myBoardID + " is different from IP ID of " + myBoardIDFromIPAddress);
                            }
                            databaseNameThis = gVariable.internalMachineName[myBoardIndex];
                            gVariable.machineStatus[myBoardIndex].machineID = (myBoardIndex + 1).ToString();
                            gVariable.machineStatus[myBoardIndex].machineCode = gVariable.machineCodeArrayDatabase[myBoardIndex]; //databaseNameThis;
                            gVariable.machineStatus[myBoardIndex].machineName = gVariable.machineNameArrayDatabase[myBoardIndex];
                            gVariable.socketArray[myBoardIndex] = clientSocketInServer;
                            gVariable.machineStartTime[myBoardIndex] = DateTime.Now.ToString();
                            gVariable.machineStatus[myBoardIndex].totalWorkingTime = 0;
                            timeStampNow = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now);
                            gVariable.machineStartTimeStamp[myBoardIndex] = (int)(timeStampNow - gVariable.worldStartTime).TotalSeconds;
                            gVariable.connectionStatus[myBoardIndex] = 0;
                            gVariable.connectionCount[myBoardIndex] = 0;
                            gVariable.connectionCount_old[myBoardIndex] = 0;
                            gVariable.machineCurrentStatus[myBoardIndex] = gVariable.MACHINE_STATUS_DISPATCH_DUMMY;
                            qualityDataSN = 0;

                            toolUsedTimesNow = 0;

                            Console.WriteLine("board " + myBoardID + " handshake OK on " + DateTime.Now.ToString() + ", communication started !!");
                            if (gVariable.debugMode == 1)
                            {
                                gVariable.dataLogWriter[myBoardIndex].WriteLine(" ----" + DateTime.Now.ToString() + gVariable.internalMachineName[myBoardIndex] + " start communication");
                                gVariable.infoWriter.WriteLine(" ----" + DateTime.Now.ToString() + ": " + gVariable.internalMachineName[myBoardIndex] + " start communication");
                            }

                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;  //so first byte in data position is OK, time string starts from the second byte
                            now = DateTime.Now;

                            onePacket[PROTOCOL_DATA_POS + 1] = (byte)(now.Year % 100 / 10 + '0');
                            onePacket[PROTOCOL_DATA_POS + 2] = (byte)(now.Year % 100 % 10 + '0');
                            onePacket[PROTOCOL_DATA_POS + 3] = (byte)(now.Month / 10 + '0');
                            onePacket[PROTOCOL_DATA_POS + 4] = (byte)(now.Month % 10 + '0');
                            onePacket[PROTOCOL_DATA_POS + 5] = (byte)(now.Day / 10 + '0');
                            onePacket[PROTOCOL_DATA_POS + 6] = (byte)(now.Day % 10 + '0');
                            onePacket[PROTOCOL_DATA_POS + 7] = (byte)(now.Hour / 10 + '0');
                            onePacket[PROTOCOL_DATA_POS + 8] = (byte)(now.Hour % 10 + '0');
                            onePacket[PROTOCOL_DATA_POS + 9] = (byte)(now.Minute / 10 + '0');
                            onePacket[PROTOCOL_DATA_POS + 10] = (byte)(now.Minute % 10 + '0');
                            onePacket[PROTOCOL_DATA_POS + 11] = (byte)(now.Second / 10 + '0');
                            onePacket[PROTOCOL_DATA_POS + 12] = (byte)(now.Second % 10 + '0');

                            onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_SET_BOARD_TIME_TO_TOUCHPAD;

                            len = MIN_PACKET_LEN_MINUS_ONE + LEN_OF_DATE_AND_TIME;
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_SET_BOARD_TIME_TO_TOUCHPAD);

                            gVariable.dispatchSheet[myBoardIndex].dispatchCode = gVariable.dummyDispatchTableName;  //no dispatch yet, we set it to dummy

                            //first set SPC status to uncontrollable, then calculate when will it becomes controllable
                            gVariable.dataForThisBoardIsUnderSPCControl[myBoardIndex] = gVariable.SPC_DATA_UNCONTROLLABLE;

                            //this is SPC data checking, we only start data checking function when this function is enabled
                            if (gVariable.checkDataCorrectness == 1)
                            {
                                //as long as a SPC checking function started, it will never top, even if this board stop working,
                                //so we will only start SPC checking function once for a board, if the board disconnected, checking will continue 
                                if (gVariable.SPCCheckingThreadRunning[myBoardIndex] == 0)
                                {
                                    gVariable.SPCCheckingThreadRunning[myBoardIndex] = 1;

                                    SPCFunctions mySPC = new SPCFunctions(myBoardIndex);
                                    mySPC.checkForSPCRules();
                                }
                            }
                            toolClass.nonBlockingDelay(500);
                            break;

                        case COMMUNICATION_TYPE_EMAIL_HEART_BEAT:
                            gVariable.emailForwarderHeartBeatNum++;  //this is a heart beat packet from email forwarder, which means email forwarder is still alive
                            break;
                        case COMMUNICATION_TYPE_SET_BOARD_TIME_TO_TOUCHPAD:  //this data packet is outdated, not used any more
                            //we don't need to make a judgment of the response, if result_OK, we should get machine_code next time, otherwise, we should get handshake type packet
                            break;

                        //After board get its time setting, it will send out this packet to indicate communication OK, and a dummy machine code means this board will be running by 
                        //dummy dispatch until real dispatch comes(then switch to real dispatch)
                        case COMMUNICATION_TYPE_SEND_DUMMY_MACHINE_CODE_TO_PC:
                            //                                                    gVariable.currentDispatchCode = "dummy";  //only used for curve display, may not be the same as gVariable.dispatchSheet[myBoardIndex].dispatchCode
                            //toolClass.getDummyData(myBoardIndex);
                            //we got related data, put it in database in the following functiuon, including all data type and dispatch if they does not exist before
                            //readBoardInfoFromMachineDataStruct(1);

                            //toolClass.nonBlockingDelay(200);

                            //data collect board knows we can send data now althouth no real dispatch
                            handshakeWithClientOK = 1;

                            //tell data collect board we got the dummy machine data packet
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_SEND_DUMMY_MACHINE_CODE_TO_PC);
                            break;

                        case COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_PC:  //after dispatch started, board may apply for craft parameters
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)  //we can use quality/craft/andon data now
                            {
                                len = packetLen - MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte

                                str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                                //                                                    byte[] bs = System.Text.Encoding.GetEncoding("gb2312").GetBytes(str);

                                putPDATouchpadArrayToVariables(str, COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_PC);

                                writeMESString(COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_PC);
                            }
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_PC);
                            break;

                        //flag 4:data collect board now ask for dispatch by sending out a setting packet data,  
                        //try to see if there are beat/ADC range setting data from this instruction, if yes, record them in local variables
                        //then use machine name(we got it by using board ID) to apply for dispatch/craft/quality/material/machine status
                        case COMMUNICATION_TYPE_SETTING_TO_PC:
                            //the touch pad is now appplying for new dispatch, so we clear all data related to old dispatch
                            clearMachineStatus();
                            len = packetLen - MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                            str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);

                            //don't try to say OK to board, because this instruction will make wait function in board jump out of waiting cycle, so data packet will 
                            //be sent to server when dispatch data packet should be sent
                            //say OK to data collect board
                            //onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            //len = MIN_PACKET_LEN;
                            //sendDataToClient(onePacket, len, COMMUNICATION_TYPE_SETTING_TO_PC);

                            //try to see if there are ADC settings in setting package
                            putPDATouchpadArrayToVariables(str, COMMUNICATION_TYPE_SETTING_TO_PC);
                            //                                                    gVariable.currentDispatchCode = gVariable.dispatchSheet[myBoardIndex].dispatchCode;  //only used for curve display, may not be the same as gVariable.dispatchSheet[myBoardIndex].dispatchCode

                            //we will get new dispatch, so remove dummy table and we will get data by using new dispatch/craft/quality data requrements
                            //mySQLClass.removeDummyDatabaseTable(databaseNameThis);

                            //for there is no real MES system(work in emulation mode), we need to emulate different dispatch, so first get the latest dispatch index, then increase by one
                            if (gVariable.faultData == 1)
                                dispatchAppearIndex = mySQLClass.getRecordNumInTable(databaseNameThis, gVariable.dispatchListTableName) - 1;

                            //get dispatch/machine status/craft data from MES, we will merge this data packet with quality data into one packet then send it to Touchpad

                            //generate an empty new data packet, with string header, comunication type, generate time, data packet index, data type
                            //still need more work to input real data, length, CRC
                            prepareForDataNeedSending(onePacket, COMMUNICATION_TYPE_DISPATCH_DATA_TO_TOUCHPAD, DATA_TYPE_MES_INSTRUCTION);
                            
                            //we get real data of dispatch info from MES
                            ret = readMESString(COMMUNICATION_TYPE_DISPATCH_DATA_TO_TOUCHPAD);
                            if (ret == RESULT_OK)
                            {
                                str = putVariableToTouchpadFormat(COMMUNICATION_TYPE_DISPATCH_DATA_TO_TOUCHPAD);
                                aa = System.Text.Encoding.Default.GetBytes(str);
                                len = aa.Length;

                                //input real dispatch data here
                                for (i = 0; i < len; i++)
                                    onePacket[i + PROTOCOL_DATA_POS] = aa[i];
                                len += MIN_PACKET_LEN_PURE_FRAME - 2; //pure frame include packet header and CRC, it's 16, so need to minus 2, touchpad need a 2 byte CRC
                            }
                            else
                            {
                                len = MIN_PACKET_LEN;
                                onePacket[PROTOCOL_DATA_POS] = (byte)0xff;
                                sendDataToClient(onePacket, len, COMMUNICATION_TYPE_DISPATCH_DATA_TO_TOUCHPAD);
                                break;
                            }
                            onePacket[PROTOCOL_LEN_POS] = (byte)(len % 0x100);
                            onePacket[PROTOCOL_LEN_POS + 1] = (byte)(len / 0x100);
                            //a complete data packet is now ready, but we still need to add more data into this packet before we send it to data collect board
                            toolClass.addCrc16Code(onePacket, len);

                            //get machine staus data in old format(touchpad format)
                            start = len;

                            str = putVariableToTouchpadFormat(COMMUNICATION_TYPE_MACHINE_STATUS_DISPLAY);
                            aa = System.Text.Encoding.Default.GetBytes(str);
                            len = aa.Length;
                            for (i = 0; i < len; i++)
                                header[i + PROTOCOL_OLD_DATA_POS] = aa[i];
                            len += PROTOCOL_OLD_HEADER_LEN + PROTOCOL_OLD_CRC_LEN; //9, old format
                            header[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_MACHINE_STATUS_TO_TOUCHPAD;
                            header[PROTOCOL_LEN_POS] = (byte)(len % 0x100);
                            header[PROTOCOL_LEN_POS + 1] = (byte)(len / 0x100);
                            toolClass.addCrc16Code(header, len);
                            j = 0;
                            for (i = start; i < len + start; i++)
                                onePacket[i] = header[j++];

                            //get craft data in old format(touchpad format)
                            start = i;

                            readMESString(COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_TOUCHPAD);
                            str = putVariableToTouchpadFormat(COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_TOUCHPAD);
                            aa = System.Text.Encoding.Default.GetBytes(str);
                            len = aa.Length;
                            for (i = 0; i < len; i++)
                                header[i + PROTOCOL_OLD_DATA_POS] = aa[i];
                            len += PROTOCOL_OLD_HEADER_LEN + PROTOCOL_OLD_CRC_LEN; //9, old format;
                            header[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_TOUCHPAD;
                            header[PROTOCOL_LEN_POS] = (byte)(len % 0x100);
                            header[PROTOCOL_LEN_POS + 1] = (byte)(len / 0x100);
                            toolClass.addCrc16Code(header, len);
                            j = 0;
                            for (i = start; i < len + start; i++)
                                onePacket[i] = header[j++];

                            len = i + PROTOCOL_CRC_LEN;
                            //send initial version of dispach/machine info to touchpad
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_DISPATCH_DATA_TO_TOUCHPAD);
                            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "send out dispatch data");

                            toolClass.systemDelay(2000);

                            gVariable.machineStatus[myBoardIndex].productBeat = 0;
                            gVariable.machineStatus[myBoardIndex].collectedNumber = 0;

                            //if this clientSocketInServer comes from touch pad, we need to to wait for next instruction of COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD
                            //then send quality data info to touch pad, but for Android app, we need to send quality data to clientSocketInServer directly
                            if (clientFromTouchpad == 0)
                            {
                                readMESString(COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD);
                                putVariableToTouchpadFormat(COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD);

                                //read material data here, so we can display them on dispatchUI screen
                                readMESString(COMMUNICATION_TYPE_MATERIAL_DATA_REQ);

                                //we got related data, put it in database in the following functiuon, including all data type and dispatch if they does not exist before
                                readBoardInfoFromMachineDataStruct(1);

                                //this flag will be used by multiCurve
                                gVariable.refreshMultiCurve = 1; //if multiCurve is now in the top screen, refresh it with new board setting according to dispatch sheet
                                gVariable.machineCurrentStatus[myBoardIndex] = gVariable.MACHINE_STATUS_DISPATCH_APPLIED;  //we can use quality/craft/andon data now
                            }
                            break;

                        //we got dispatch/empty craft/quality data from MES and send it to board
                        case COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD:
                            prepareForDataNeedSending(onePacket, COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD, DATA_TYPE_MES_INSTRUCTION);

                            readMESString(COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD);
                            str = putVariableToTouchpadFormat(COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD);
                            aa = System.Text.Encoding.Default.GetBytes(str);
                            len = aa.Length;
                            for (i = 0; i < len; i++)
                                onePacket[i + PROTOCOL_DATA_POS] = aa[i];

                            len += MIN_PACKET_LEN_PURE_FRAME; //pure frame include packet header and CRC, it's 32

                            //send initial version of dispach/machine info to touchpad
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD);

                            //read material data here, so we can display them on dispatchUI screen
                            readMESString(COMMUNICATION_TYPE_MATERIAL_DATA_REQ);

                            gVariable.machineCurrentStatus[myBoardIndex] = gVariable.MACHINE_STATUS_DISPATCH_APPLIED;  //we can use quality/craft/andon data now

                            //we got related data, put it in database in the following functiuon, including all data type and dispatch if they does not exist before
                            readBoardInfoFromMachineDataStruct(1);

                            //this flag will be used by multiCurve
                            gVariable.refreshMultiCurve = 1; //if multiCurve is now in the top screen, refresh it with new board setting according to dispatch sheet

                            //send ADC settings to board, so it know range limits for ADCs
                            //writeSettingsToBoard(ADC_SETTINGS);

                            //send BEAT settings to board, so it know range limits for ADCs
                            //writeSettingsToBoard(BEAT_SETTINGS);
                            break;

                        case COMMUNICATION_TYPE_MACHINE_STATUS_REQUEST:
                            timeStampNow = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now);
                            gVariable.machineStatus[myBoardIndex].totalWorkingTime = (int)((timeStampNow - gVariable.worldStartTime).TotalSeconds - gVariable.machineStartTimeStamp[myBoardIndex]) / 2;
                            gVariable.machineStatus[myBoardIndex].workingTime = workingTimePoints * 4 / 10;
                            if (gVariable.machineStatus[myBoardIndex].totalWorkingTime < gVariable.machineStatus[myBoardIndex].workingTime)
                                gVariable.machineStatus[myBoardIndex].workingTime = gVariable.machineStatus[myBoardIndex].totalWorkingTime;

                            gVariable.machineStatus[myBoardIndex].powerConsumed = powerConsumed / 1000;
                            gVariable.machineStatus[myBoardIndex].standbyTime = gVariable.machineStatus[myBoardIndex].totalWorkingTime - gVariable.machineStatus[myBoardIndex].workingTime;
                            gVariable.machineStatus[myBoardIndex].power = currentPower;
                            gVariable.machineStatus[myBoardIndex].collectedNumber = gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber + gVariable.dispatchSheet[myBoardIndex].qualifiedNumber;
                            gVariable.machineStatus[myBoardIndex].revolution = 3000;

                            str = putVariableToTouchpadFormat(COMMUNICATION_TYPE_MACHINE_STATUS_DISPLAY);
                            aa = System.Text.Encoding.Default.GetBytes(str);
                            len = aa.Length;
                            for (i = 0; i < len; i++)
                                onePacket[i + PROTOCOL_DATA_POS] = aa[i];

                            len += MIN_PACKET_LEN_PURE_FRAME; //pure frame include packet header and CRC, it's 32
                            //send machine status info to touchpad
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_MACHINE_STATUS_TO_TOUCHPAD);
                            break;

                        //get empty craft data from MES and send it to board, and display it on touch pad, only after we applied for dispatch, can we request for craft data
                        case COMMUNICATION_TYPE_CRAFT_PARAMETER_REQUEST:
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED)
                            {
                                //get craft data from MES and send it to touchpad
                                readMESString(COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_TOUCHPAD);

                                str = putVariableToTouchpadFormat(COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_TOUCHPAD);

                                aa = System.Text.Encoding.Default.GetBytes(str);

                                len = aa.Length;
                                for (i = 0; i < len; i++)
                                    onePacket[i + PROTOCOL_DATA_POS] = aa[i];

                                len += MIN_PACKET_LEN_PURE_FRAME; //pure frame include packet header and CRC, it's 32;
                            }
                            else
                            {
                                //we got the request successfully, but there is no dispatch available yet, so there is no craft parameter
                                onePacket[PROTOCOL_DATA_POS] = RESULT_OK; 
                                len = MIN_PACKET_LEN;
                            }
                            //send initial version of dispach/machine info to touchpad
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_TOUCHPAD);

                            break;

                        case COMMUNICATION_TYPE_QUALITY_DATA_REQUEST:  //we need to first apply for dispatch, then quality data
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED)
                            {
                                //get quality data from MES and send it to touchpad
                                readMESString(COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD);

                                str = putVariableToTouchpadFormat(COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD);

                                aa = System.Text.Encoding.Default.GetBytes(str);

                                len = aa.Length;
                                for (i = 0; i < len; i++)
                                    onePacket[i + PROTOCOL_DATA_POS] = aa[i];

                                len += MIN_PACKET_LEN_PURE_FRAME; //pure frame include packet header and CRC, it's 32;;
                            }
                            else
                            {
                                //we got the request successfully, but there is no dispatch available yet, so there is no quality data
                                onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                                len = MIN_PACKET_LEN;
                            }
                            //send initial version of dispach/machine info to touchpad
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD);
                            break;

                        //touch pad send new quality data to PC
                        case COMMUNICATION_TYPE_QUALITY_DATA_TO_PC:
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)  //operator can send quality/craft data to server now
                            {
                                //only when we are using app and destination is board(machine) 6, can we update quality data
                                //                                                        if (clientFromTouchpad != 0 || myBoardIndex == 5)
                                {
                                    len = packetLen - MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte

                                    str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);

                                    putPDATouchpadArrayToVariables(str, COMMUNICATION_TYPE_QUALITY_DATA_TO_PC);

                                    writeMESString(COMMUNICATION_TYPE_QUALITY_DATA_TO_PC);
                                }
                            }
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            //response OK
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_QUALITY_DATA_TO_PC);

                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)  //we can use quality data only when dispatch started
                            {
                                FileAppendFucntion(checkBytes);
                                qualityDataSN++;
                            }
                            break;

                        //the operator started dispatch from touch pad
                        case COMMUNICATION_TYPE_DISPATCH_START_TO_PC:
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED)
                            {
                                //only when we are using app and destination is board(machine) 6, can we start a dispatch
                                //        if (clientFromTouchpad != 0 || myBoardIndex == 5)
                                //start dispatch, first stop receiving incoming data, delete dummy data in database, then start new communication based on dispatch
                                timeStampNow = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now);
                                dispatchStartTimeStamp = (int)(timeStampNow - gVariable.worldStartTime).TotalSeconds;
                                beatTimeStamp = dispatchStartTimeStamp;

                                gVariable.dispatchSheet[myBoardIndex].realStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                gVariable.machineCurrentStatus[myBoardIndex] = gVariable.MACHINE_STATUS_DISPATCH_STARTED;
                                gVariable.dispatchSheet[myBoardIndex].qualifiedNumber = 0;
                                gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber = 0;
                                gVariable.dispatchSheet[myBoardIndex].status = gVariable.MACHINE_STATUS_DISPATCH_STARTED;
                                //record start info for a dispatch, null means dispatch code is defined by myBoardIndex
                                mySQLClass.updateDispatchTable(gVariable.internalMachineName[myBoardIndex], gVariable.dispatchListTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_STARTED, null);
                                //also set this dispatch in global database to the status of started
                                mySQLClass.updateDispatchTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_STARTED, null);

                                gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DEVICE, myBoardIndex] = 0;
                                gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_MATERIAL, myBoardIndex] = 0;
                                gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DATA, myBoardIndex] = 0;

                                //there is a material alarm for this machine, since the operator started dispatch, that means material is ready, we can close this alarm
                                if (gVariable.IDForLastAlarmByMachine[myBoardIndex] > 0)
                                {

                                    updateStr = "update `" + gVariable.alarmListTableName + "` set time2 = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', status = '" + gVariable.ALARM_STATUS_COMPLETED +
                                                "' where id = '" + gVariable.IDForLastAlarmByMachine[myBoardIndex] + "'";
                                    mySQLClass.updateTableItems(gVariable.internalMachineName[myBoardIndex], updateStr);
                                    //mySQLClass.updateAlarmTable(gVariable.internalMachineName[myBoardIndex], gVariable.alarmListTableName, gVariable.IDForLastAlarmByMachine[myBoardIndex],
                                    //                                  null, null, null, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), gVariable.ALARM_STATUS_COMPLETED,
                                    //                                  gVariable.ALARM_INHISTORY_UNCHANGED, null, null, null);
                                }
                                //this flag will be used by multiCurve
                                gVariable.refreshMultiCurve = 1; //if multiCurve is now in the top screen, refresh it with new board setting according to dispatch sheet

                                numOfCraftDataInTable = 0;  //we will re start calculation of craft data in a table since we will use a new table

                                //start quality data serial number record, and this number will increase by one automatically until dispatch completed
                                //qualityDataSN = Convert.ToInt32(gVariable.dispatchSheet[myBoardIndex].serialNumber.Remove(0, gVariable.LENGTH_DISPATCH_CODE + 1));

                            }
                            //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_DISPATCH_START_TO_PC;
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_DISPATCH_START_TO_PC);

                            if (clientFromTouchpad == 0)
                            {
                                //we got related data, put it in database in the following functiuon, including all data type and dispatch if they does not exist before
                                readBoardInfoFromMachineDataStruct(1);
                            }
                            break;

                        //after dispatch complete, touchpad will send machine status to PC automatically
                        case COMMUNICATION_TYPE_MACHINE_STATUS_TO_PC:
                            Console.WriteLine("status from touchpad");

                            len = packetLen - MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte

                            //get full machine info from the received data
                            str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len); //.Remove(0, PROTOCOL_DATA_POS); //.Remove(len);

                            putPDATouchpadArrayToVariables(str, COMMUNICATION_TYPE_MACHINE_STATUS_TO_PC);

                            mySQLClass.writeDataToMachineStatusTable(databaseNameThis, gVariable.machineStatusListTableName, myBoardIndex);

                            writeMESString(COMMUNICATION_TYPE_MACHINE_STATUS_TO_PC);

                            //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_MACHINE_STATUS_TO_PC;
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_MACHINE_STATUS_TO_PC);
                            break;

                        //this case is not used any more, if touchpad sisnot get a correct dispatch data, it wil automatically resend the setting data to get dispatch again
                        case COMMUNICATION_TYPE_DISPATCH_DATA_TO_TOUCHPAD:  //got response from touchpad
                            if (onePacket[PROTOCOL_DATA_POS] != RESULT_OK)  //if we didnot get a OK response, redo handshake again
                            {
                                Console.WriteLine("Board " + (myBoardIndex + 1) + " failed to get response for dispatch sending, need a redo of handshake on " + DateTime.Now.ToString());
                                //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_REDO_HANDSHAKE_TO_BOARD;
                                onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                                len = MIN_PACKET_LEN;
                                sendDataToClient(onePacket, len, COMMUNICATION_TYPE_REDO_HANDSHAKE_TO_BOARD);
                            }
                            break;

                        case COMMUNICATION_TYPE_MATERIAL_ANDON_TO_PC:
                            //if we already had one alarm of the same kind for this machine, a new one should not bo triggered, until the current one be dismissed
                            if (gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_MATERIAL, myBoardIndex] != 0)
                            {
                                break;
                            }
                            //set flag to 1, so same kind of alarm won't be triggered if the old one is not dismissed
                            gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_MATERIAL, myBoardIndex] = 1;

                            len = packetLen - MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                            str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                            putPDATouchpadArrayToVariables(str, COMMUNICATION_TYPE_MATERIAL_ANDON_TO_PC);
                            writeMESString(COMMUNICATION_TYPE_MATERIAL_ANDON_TO_PC);

                            //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_MATERIAL_ANDON_TO_PC;
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_MATERIAL_ANDON_TO_PC);
                            break;

                        //the operator updated status for a certain Andon alarm
                        case COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_DEAL_TO_PC:
                            len = packetLen - MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte

                            str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                            putPDATouchpadArrayToVariables(str, COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_DEAL_TO_PC);
                            writeMESString(COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_DEAL_TO_PC);

                            //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_DEAL_TO_PC;
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_DEAL_TO_PC);
                            break;

                        case COMMUNICATION_TYPE_DEVICE_ANDON_TO_PC:
                            //if we already had one alarm of the same kind for this machine, a new one should not bo triggered, until the current one be dismissed
                            if (gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DEVICE, myBoardIndex] != 0)
                            {
                                break;
                            }

                            //set flag to 1, so same kind of alarm won't be triggered if the old one is not dismissed
                            gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DEVICE, myBoardIndex] = 1;

                            len = packetLen - MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte

                            str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                            putPDATouchpadArrayToVariables(str, COMMUNICATION_TYPE_DEVICE_ANDON_TO_PC);
                            writeMESString(COMMUNICATION_TYPE_DEVICE_ANDON_TO_PC);

                            //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_DEVICE_ANDON_TO_PC;
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_DEVICE_ANDON_TO_PC);
                            break;

                        //got ADC/Current/Voltage/GPIO data from board
                        case COMMUNICATION_TYPE_COLLECTED_DATA_SEND_TO_PC:  //collected by data collect board
                            if (checkWhetherToContinue())
                                return;

                            //this board just connected, send beat setting data to board
                            if (gVariable.connectionStatus[myBoardIndex] == 0)
                            {
                                if (getSettingDataByIndex(myBoardIndex) == false)
                                    useFixedBeatSetting(myBoardIndex);
                                gVariable.beatPeriodInfo[myBoardIndex].deviceSelection = 0;
                                gVariable.whereComesTheSettingData = myBoardIndex;
                                gVariable.whatSettingDataModified = gVariable.BEAT_SETTING_DATA_TO_BOARD;
                            }
                            gVariable.connectionStatus[myBoardIndex] = 1;

                            //so response data only contains 2 filed: file name and data index
                            //Array.Copy(onePacket, PROTOCOL_DATAINDEX_POS, onePacket, PROTOCOL_DATA_POS + 2, PROTOCOL_DATAINDEX_LEN); //copy dataIndex to the end of project name
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = RESPONSE_LEN;
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_COLLECTED_DATA_SEND_TO_PC);

                            FileAppendFucntion(checkBytes);
                            break;
                        case COMMUNICATION_TYPE_MATERIAL_DATA_REQ:
                            readMESString(COMMUNICATION_TYPE_MATERIAL_DATA_REQ);

                            str = putVariableToTouchpadFormat(COMMUNICATION_TYPE_MATERIAL_DATA_REQ);

                            aa = System.Text.Encoding.Default.GetBytes(str);

                            len = aa.Length;
                            for (i = 0; i < len; i++)
                                onePacket[i + PROTOCOL_DATA_POS] = aa[i];

                            len += MIN_PACKET_LEN_PURE_FRAME; //pure frame include packet header and CRC, it's 32;
                            //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_MATERIAL_INQUIRY_TO_TOUCHPAD;
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_MATERIAL_INQUIRY_TO_TOUCHPAD);
                            break;

                        //try to get a device andon list and then it to touchpad
                        case COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_TO_REQ:
                            readMESString(COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_TO_REQ);

                            str = putVariableToTouchpadFormat(COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_TO_REQ);

                            aa = System.Text.Encoding.Default.GetBytes(str);

                            len = aa.Length;
                            for (i = 0; i < len; i++)
                                onePacket[i + PROTOCOL_DATA_POS] = aa[i];

                            //after inquiry, we send machine alarm list to touch pad
                            len += MIN_PACKET_LEN_PURE_FRAME; //pure frame include packet header and CRC, it's 32;
                            //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_TO_TOUCHPAD;
                            //send initial version of dispach/machine info to touchpad
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_TO_TOUCHPAD);
                            break;

                        case COMMUNICATION_TYPE_DISPATCH_COMPLETED_TO_PC:   //dispatch completed
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                            {
                                //only when we are using app and destination is board(machine) 6, can we complete a dispatch
                                //                                                        if (clientFromTouchpad != 0 || myBoardIndex == 5)
                                {
                                    gVariable.machineCurrentStatus[myBoardIndex] = gVariable.MACHINE_STATUS_DISPATCH_COMPLETED; //quality/craft/andon data are illegal

                                    len = packetLen - MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte

                                    str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);

                                    putPDATouchpadArrayToVariables(str, COMMUNICATION_TYPE_DISPATCH_COMPLETED_TO_PC);

                                    if (gVariable.currentCurveDatabaseName == databaseNameThis)  //current board is now on dislay in dispatchUI, we need to refresh dispatchUI screen
                                        gVariable.previousDispatch = null;

                                    //report to MES
                                    writeMESString(COMMUNICATION_TYPE_DISPATCH_COMPLETED_TO_PC);

                                    readBoardInfoFromMachineDataStruct(0);

                                    //remove data/setting related to this dispatch and to to no dispatch mode (dummy dispatch)
                                    //                                                    dealWithDispatchCompleted();

                                    //this flag will be used by multiCurve
                                    gVariable.refreshMultiCurve = 1; //if multiCurve is now in the top screen, refresh it with to default data communication
                                    numOfCraftDataInTable = 0;  //we will re start calculation of craft data in a table since we will use a new table

                                    dispatchAppearIndex++;  //used for emulate case only, that is faultData == 1 

                                    //record start info for a dispatch, null means dispatch code is defined by myBoardIndex
                                    mySQLClass.updateDispatchTable(gVariable.internalMachineName[myBoardIndex], gVariable.dispatchListTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_COMPLETED, null);
                                    //also set this dispatch in global database to the status of completed
                                    mySQLClass.updateDispatchTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_COMPLETED, null);
                                }
                            }
                            qualityDataSN = 0;
                            //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = (byte)communicationType;
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;

                        //got prepare time point from touch pad
                        case COMMUNICATION_TYPE_ADJEST_TIME_TO_EDS:
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)  //we can use quality/craft/andon data now
                            {
                                //only when we are using app and destination is board(machine) 6, can we adjust prepare time
                                //                                                        if (clientFromTouchpad != 0 || myBoardIndex == 5)
                                gVariable.dispatchSheet[myBoardIndex].prepareTimePoint = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                gVariable.machineStatus[myBoardIndex].productBeat = 0;

                                timeStampNow = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now);
                                gVariable.machineStatus[myBoardIndex].prepareTime = (int)(timeStampNow - gVariable.worldStartTime).TotalSeconds - dispatchStartTimeStamp;

                                //record start info for a dispatch, null means dispatch code is defined by myBoardIndex
                                mySQLClass.updateDispatchTable(gVariable.internalMachineName[myBoardIndex], gVariable.dispatchListTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_STARTED, null);
                                //also set this dispatch in global database to the status of completed
                                mySQLClass.updateDispatchTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_STARTED, null);
                            }

                            //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = (byte)communicationType;
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;

                        //got beat cycle time from board
                        case COMMUNICATION_TYPE_CYCLE_TIME_TO_PC:
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)  //we can use quality/craft/andon data now
                            {
                                timeStampNow = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now);
                                j = (int)(timeStampNow - gVariable.worldStartTime).TotalSeconds;

                                gVariable.machineStatus[myBoardIndex].productBeat = j - beatTimeStamp;
                                beatTimeStamp = j;

                                gVariable.machineStatus[myBoardIndex].productBeat++;

                                float[] fValue = new float[10]; 
                                fValue[0] = gVariable.machineStatus[myBoardIndex].productBeat;

                                //mySQLClass.writeMultipleFloatToTable(databaseNameThis, beatDataTableName, mySQLClass.DATA_TYPE_BEAT_DATA, beatTimeStamp, fValue, 1, "");
                            }
                            //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = (byte)communicationType;
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;

                            gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DEVICE, myBoardIndex] = 0;
                            gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_MATERIAL, myBoardIndex] = 0;
                            gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DATA, myBoardIndex] = 0;

                            //we should have these responses for cycle time, but currently data collect board doesnot support this
                            //should add this in board then enable this function
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);

                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                                FileAppendFucntion(checkBytes);
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("processClientDataCollectBoard(" + communicationType + "," + packetLen + ") for myBoardIndex " + myBoardIndex + "failed, " + ex);
                }
            }
        }
    }
}