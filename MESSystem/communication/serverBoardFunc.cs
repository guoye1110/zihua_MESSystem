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
using MESSystem.dispatchManagement;

namespace MESSystem.communication
{
    public partial class communicate
    {

        const int TOUCH_PAD_ID_START = 1;
        const int PRINT_MACHINE_ID_START = 100;
        const int SHOW_BOARD_ID_START = 1000;
        const int MOBIL_DEVICE_ID_START = 2000;

        //data packet related definition
        const int PROTOCOL_HEAD_POS = 0;
        const int PROTOCOL_LEN_POS = 4;
        const int PROTOCOL_COMMUNICATION_TYPE_POS = 6;
        const int PROTOCOL_COMMUNICATION_STATUS_POS = 7;
        const int PROTOCOL_DATA_POS = 8;
        const int PROTOCOL_CRC_LEN = 4;

        const int MIN_PACKET_LEN_MINUS_ONE = 12;   //header(4) + len(2) + communicationtype(1) + status(1) + data(0) + CRC(4)
        const int MIN_PACKET_LEN_PURE_FRAME = 12;   //header(4) + len(2) + communicationtype(1) + status(1) + data(0) + CRC(4)
        const int MIN_PACKET_LEN = 12;   //header(4) + len(2) + communicationtype(1) + status(1) + data(0) + CRC(4)

        const int MIN_PACKET_LEN_FOR_APP = 9;   //header(4) + len(2) + communicationtype(1) + CRC(2)
        const int MAX_PACKET_LEN = 9000;  //max data length for a data packet

        const int COMMUNICATION_TYPE_HANDSHAKE_WITH_MACHINE_ID = 3;  //握手

        //communication with touch pad
        const int COMMUNICATION_TYPE_MATERIAL_BARCODE_TO_PC = 0x80;  //物料标签扫描上传
        const int COMMUNICATION_TYPE_GET_MATERIAL_DATA_FROM_PC = 0x81;  //拉取物料单
        const int COMMUNICATION_TYPE_MATERIAL_DATA_UPDATE = 0x82;  //物料单更新
        const int COMMUNICATION_TYPE_MATERIAL_COMPLETE = 0x83;  //物料单报工
        const int COMMUNICATION_TYPE_MATERIAL_ANDON_TO_PC = 0x84;  //物料安灯报警上传
        const int COMMUNICATION_TYPE_MATERIAL_ANDON_STATUS_UPDATE = 0x85;  //物料安灯报警状态更新

        const int COMMUNICATION_TYPE_CAST_BARCODE_TO_PC = 0x86;  //标签上传
        const int COMMUNICATION_TYPE_APPLY_DISPATCH_TO_PC = 0x87;  //工单下发
        const int COMMUNICATION_TYPE_DISPATCH_START_TO_PC = 0x88;  //工单启动
        const int COMMUNICATION_TYPE_DISPATCH_UPDATE_TO_PC = 0x89;  //工单更新
        const int COMMUNICATION_TYPE_TASK_COMPLETED_TO_PC = 0x8A;   //任务单完工上传
        const int COMMUNICATION_TYPE_DEVICE_ANDON_TO_PC = 0x8B;  //设备安灯报警上传
        const int COMMUNICATION_TYPE_DEVICE_ANDON_STATUS_UPDATED = 0x8C;  //报警状态更新
        const int COMMUNICATION_TYPE_CAST_TRANSFER_SHEET_TO_TOUCHPAD = 0x8D;  //请求流转单下发
        const int COMMUNICATION_TYPE_CAST_TRANSFER_SHEET_TO_PC = 0x8E;  //流转单上传
        //const int COMMUNICATION_TYPE_TRANSFER_SHEET_TO_PC = 0x8E;  //流转单上传

        const int COMMUNICATION_TYPE_LARGE_ROLL_ARRIVED_PRINT = 0x8F;  //印刷上卷处扫描大卷
        const int COMMUNICATION_TYPE_LARGE_ROLL_ARRIVED_SLIT = 0x98;  //分切上卷处扫描大卷

        const int COMMUNICATION_TYPE_SETTINGS_TO_PC = 0xA0;  //设置上传
        const int COMMUNICATION_TYPE_LOGIN_TO_PC = 0xA1;  //用户登录
        const int COMMUNICATION_TYPE_MODIFY_PASSWORD_TO_PC = 0xA2;  //修改密码
        const int COMMUNICATION_TYPE_MODIFY_PRIVILEGE_TO_PC = 0xA3;  //修改权限
        const int COMMUNICATION_TYPE_TIME_SYNC_TO_PC = 0xA5;  //时间同步

        const int COMMUNICATION_TYPE_HEART_BEAT_TO_PC = 0xAF;  //心跳包上传
        //end of communication with touch pad

        //communication between PC host and print SW 
        //0xB0 - 0xCF
        //end of communication between PC host and print SW 

        //communication between PC host and show board TV
        private const int COMMUNICATION_TYPE_PUSH_DATA = 0xD0;
        private const int COMMUNICATION_TYPE_UPDATE_FORMAT = 0xD1;
        //end of communication between PC host and show board TV

        //communication between PC host and mobile phone App
        const int COMMUNICATION_TYPE_APP_WORKING_BOARD_ID_TO_PC = 0xD8;  //which board this app is working on 
        const int COMMUNICATION_TYPE_APP_FILE_NAME_TO_PC = 0xD9;
        const int COMMUNICATION_TYPE_APP_FILE_DATA_TO_PC = 0xDA;
        const int COMMUNICATION_TYPE_APP_FILE_END_TO_PC = 0xDB;
        const int COMMUNICATION_TYPE_APP_FILE_FROM_PC = 0xDC;
        const int COMMUNICATION_TYPE_APP_DEVICE_NAME = 0xDD;
        //end of communication between PC host and App

        public partial class ClientThread
        {
            int clientFromTouchpad = 0;
            int boardIDNotSetFlag;  //flag to indicate board ID is null, the user need to set an ID to the board
            int qualityDataSN;
            int myBoardIDFromIPAddress;

            zihua_showBoardClient m_showBoardClient;
            zihua_printerClient m_printClient;


            public void processClientDataCollectBoard(int communicationType, int packetLen)
            {
                int i, j;
                int ret;
                int result;
                int len;  //temprary length value, may be cahnged any time
                byte[] timeArray = new byte[10];
                byte[] header = new byte[2000];
                byte[] aa = null;
                int alarmIDInTable;
                int deviceFailureIndex;
                int alarmMachineID;
                string str;
                string errorStr;
                string dName;
                string tName;
                string machineName;
                string updateStr;
                string commandText;
                string[] strArray;
                string[,] tableArray;
                int start;
                DateTime now;
                gVariable.dispatchSheetStruct dispatchImpl;
                System.DateTime timeStampNow;

                header[0] = (byte)'H';
                header[1] = (byte)'M';
                header[2] = (byte)'I';
                header[3] = (byte)'F';

                result = RESULT_OK;
                try
                {
                    len = packetLen - MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                    str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                    strArray = str.Split(';', '&');

                    switch (communicationType)
                    {
                        case COMMUNICATION_TYPE_LOGIN_TO_PC:
                        case COMMUNICATION_TYPE_SETTINGS_TO_PC:
                            result = RESULT_OK;
                            break;

                        case COMMUNICATION_TYPE_MATERIAL_BARCODE_TO_PC: //Robert put a sack into feed bin
                            commandText = "select * from `" + gVariable.materialDeliveryTableName + "` where targetMachine = '" + (myBoardIndex + 1) + "' order by inoutputTime DESC limit 1";
                            tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                            if (tableArray == null)
                            {
                                Console.WriteLine("delivery record for target machineID " + (myBoardIndex + 1) + "not found!");
                                result = RESULT_OK;
                            }
                            else
                            {
                                //di
                            }
                        	break;
                        case COMMUNICATION_TYPE_GET_MATERIAL_DATA_FROM_PC:
                            commandText = "select * from `" + gVariable.materialDeliveryTableName + "` where targetMachine = '" + (myBoardIndex + 1) + "' order by inoutputTime DESC limit 1";
                            tableArray = mySQLClass.databaseCommonReading(gVariable.basicInfoDatabaseName, commandText);
                            if (tableArray == null)
                            {
                                Console.WriteLine("delivery record for target machineID " + (myBoardIndex + 1) + "not found!");
                                result = RESULT_OK;
                            }
                            else
                            {
                                //di
                            }
                        	break;
                        case COMMUNICATION_TYPE_MATERIAL_DATA_UPDATE:
                            break;
                        case COMMUNICATION_TYPE_MATERIAL_COMPLETE:
                            break;

                        case COMMUNICATION_TYPE_MATERIAL_ANDON_TO_PC:
                        	break;
                        case COMMUNICATION_TYPE_MATERIAL_ANDON_STATUS_UPDATE:
                            break;

                        //these 2 functions are the same, we simply send dispatch data to touchpad
                        case COMMUNICATION_TYPE_APPLY_DISPATCH_TO_PC:
                        case COMMUNICATION_TYPE_DISPATCH_UPDATE_TO_PC:
                            dispatchTools.getCurrentProductTask(myBoardIndex);
                            dispatchTools.getCurrentDispatch(myBoardIndex);

                            dispatchImpl = gVariable.dispatchSheet[myBoardIndex];
                            gVariable.dispatchSheet[myBoardIndex].status = gVariable.MACHINE_STATUS_DISPATCH_APPLIED;
                            gVariable.productTaskSheet[myBoardIndex].status = gVariable.MACHINE_STATUS_DISPATCH_APPLIED;

                            gVariable.productTaskSheet[myBoardIndex].realStartTime = null;

                            machineName = gVariable.internalMachineName[Convert.ToInt16(dispatchImpl.machineID) - 1];

                            str = fixedCountStrPlus(dispatchImpl.batchNum, 8) + fixedCountStrPlus(dispatchImpl.productCode, 14) + fixedCountStrPlus(dispatchImpl.productName, 14) +
                                  fixedCountStrPlus(machineName, 8) 		  + fixedCountStrPlus(dispatchImpl.operatorName, 10)+ fixedCountStrPlus(dispatchImpl.customer, 18) +
                                  fixedCountStrPlus(dispatchImpl.BOMCode, 10) + fixedCountStrPlus(dispatchImpl.slitWidth, 6) 		+ fixedCountStrPlus(dispatchImpl.productWeight, 6) +
                                  fixedCountStrPlus(dispatchImpl.planTime1, 18) + fixedCountStrPlus(dispatchImpl.realStartTime, 18) + fixedCountStrPlus(dispatchImpl.planTime2, 18) +
                                  fixedCountStrPlus(dispatchImpl.realFinishTime, 18) + fixedCountStrPlus(dispatchImpl.workshift, 6) + fixedCountStrPlus(dispatchImpl.workTeam, 6) +
                                  fixedCountStrPlus(dispatchImpl.wastedOutput.ToString(), 6) + fixedCountStrPlus(dispatchImpl.unqualifiedNumber.ToString(), 6) + fixedCountStrPlus(dispatchImpl.plannedNumber.ToString(), 6) +
                                  fixedCountStrPlus(dispatchImpl.qualifiedNumber.ToString(), 6); // + fixedCountStrPlus(productTaskImpl.forcastNum.ToString(), 6) + fixedCountStrPlus(productTaskImpl.qualifiedNumber.ToString(), 6);

                            aa = System.Text.Encoding.Default.GetBytes(str);
                            len = aa.Length;

                            //input real dispatch data here
                            for (i = 0; i < len; i++)
                                onePacket[i + PROTOCOL_DATA_POS] = aa[i];
                            len += MIN_PACKET_LEN_PURE_FRAME; //pure frame include packet header and CRC, it's 32
                            //send machine status info to touchpad
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_DISPATCH_START_TO_PC:
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED)
                            {
                                gVariable.dispatchSheet[myBoardIndex].realStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                gVariable.machineCurrentStatus[myBoardIndex] = gVariable.MACHINE_STATUS_DISPATCH_STARTED;
                                gVariable.dispatchSheet[myBoardIndex].qualifiedNumber = 0;
                                gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber = 0;
                                gVariable.dispatchSheet[myBoardIndex].status = gVariable.MACHINE_STATUS_DISPATCH_STARTED;

                                gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DEVICE, myBoardIndex] = 0;
                                gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_MATERIAL, myBoardIndex] = 0;
                                gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DATA, myBoardIndex] = 0;

                                //record start info for a dispatch, null means dispatch code is defined by myBoardIndex
                                mySQLClass.updateDispatchTable(gVariable.internalMachineName[myBoardIndex], gVariable.dispatchListTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_STARTED, null);

                                //there is no update of start time after the completion of the previous production task, so this is a start of dispatch, not a product task, we should not change this value
                                if (gVariable.productTaskSheet[myBoardIndex].realStartTime == null)
                                {
                                    gVariable.productTaskSheet[myBoardIndex].realStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                    gVariable.productTaskSheet[myBoardIndex].qualifiedNumber = 0;
                                    gVariable.productTaskSheet[myBoardIndex].unqualifiedNumber = 0;
                                    gVariable.productTaskSheet[myBoardIndex].status = gVariable.MACHINE_STATUS_DISPATCH_STARTED;
                                    mySQLClass.updateDispatchTable(gVariable.internalMachineName[myBoardIndex], gVariable.productTaskListTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_STARTED, null);

                                    //also save status in global product task list, because some time we want to list all tasks with some specifiv attribute, if we don't use global
                                    //table, we have to check for every machine database
                                    mySQLClass.updateDispatchTable(gVariable.globalDatabaseName, gVariable.globalProductTaskTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_STARTED, null);
                                }
                                //also set this dispatch in global database to the status of started
                                mySQLClass.updateDispatchTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_STARTED, null);
                            }
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_TASK_COMPLETED_TO_PC:   //product task completed, if only dispatch complete, won't come to this place
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                            {
                                gVariable.machineCurrentStatus[myBoardIndex] = gVariable.MACHINE_STATUS_DISPATCH_COMPLETED; //quality/craft/andon data are illegal
                                gVariable.dispatchSheet[myBoardIndex].status = gVariable.MACHINE_STATUS_DISPATCH_COMPLETED;
                                gVariable.dispatchSheet[myBoardIndex].realFinishTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                gVariable.productTaskSheet[myBoardIndex].status = gVariable.MACHINE_STATUS_DISPATCH_COMPLETED;
                                gVariable.productTaskSheet[myBoardIndex].realFinishTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                //record start info for a dispatch, null means dispatch code is defined by myBoardIndex
                                //mySQLClass.updateDispatchTable(gVariable.internalMachineName[myBoardIndex], gVariable.dispatchListTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_COMPLETED, null);
                                mySQLClass.updateDispatchTable(gVariable.internalMachineName[myBoardIndex], gVariable.productTaskListTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_COMPLETED, null);
                                mySQLClass.updateDispatchTable(gVariable.globalDatabaseName, gVariable.globalProductTaskTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_COMPLETED, null);
                                //also set this dispatch in global database to the status of completed
                                //mySQLClass.updateDispatchTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_COMPLETED, null);
                            }
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_DEVICE_ANDON_TO_PC:
                            len = packetLen - MIN_PACKET_LEN;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                            str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                            strArray = str.Split(';');

                            alarmMachineID = toolClass.getMachineIDByMachineName(strArray[0]);
                            deviceFailureIndex = Convert.ToInt16(strArray[4].Trim());
                            alarmTableStructImpl.operatorName = strArray[3].Trim();
                            alarmTableStructImpl.alarmFailureCode = DateTime.Now.ToString("MMdd") + "-" + (gVariable.andonAlarmIndex + 1);
                            alarmTableStructImpl.errorDesc = gVariable.deviceErrDescList[deviceFailureIndex];
                            alarmTableStructImpl.dispatchCode = gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                            alarmTableStructImpl.feedBinID = "0";
                            alarmTableStructImpl.machineName = strArray[0];
                            alarmTableStructImpl.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            alarmTableStructImpl.type = gVariable.ALARM_TYPE_DEVICE;
                            alarmTableStructImpl.category = deviceFailureIndex + gVariable.ALARM_CATEGORY_DEVICE_START;
                            alarmTableStructImpl.status = gVariable.ALARM_STATUS_APPLIED;
                            alarmTableStructImpl.startID = gVariable.ALARM_NO_STARTID;
                            alarmTableStructImpl.indexInTable = gVariable.ALARM_NO_INDEX_IN_TABLE;

                            alarmTableStructImpl.workshop = gVariable.machineWorkshopDatabase[alarmMachineID - 1];

                            alarmTableStructImpl.mailList = toolClass.getAlarmMailList();

                            alarmIDInTable = mySQLClass.writeAlarmTable(databaseNameThis, gVariable.alarmListTableName, alarmTableStructImpl);
                            toolClass.processNewAlarm(databaseNameThis, alarmIDInTable);
                            gVariable.andonAlarmIndex++;
                            break;
                        case COMMUNICATION_TYPE_DEVICE_ANDON_STATUS_UPDATED:
                            dName = gVariable.internalMachineName[myBoardIndex];
                            tName = gVariable.alarmListTableName;

                            len = packetLen - MIN_PACKET_LEN_MINUS_ONE;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                            str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                            strArray = str.Split(';', '&');

                            str = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                            updateStr = null;
                            i = 0;
                            if (deviceStatusAlarm[i] == gVariable.ALARM_STATUS_SIGNED)
                            {
                                updateStr = "update `" + tName + "` set operatorName = '" + strArray[3] + "', time1 = '" + str + "', status = '" + strArray[5] +
                                       "' where alarmFailureCode = '" + strArray[1] + "'";
                            }
                            else if (deviceStatusAlarm[i] == gVariable.ALARM_STATUS_COMPLETED || deviceAlarmListTable[i].status == gVariable.ALARM_STATUS_CANCELLED)
                            {
                                updateStr = "update `" + tName + "` set completer = '" + strArray[3] + "', time2 = '" + str + "', status = '" + strArray[5] +
                                       "' where alarmFailureCode = '" + strArray[1] + "'";
                            }
                            mySQLClass.updateTableItems(dName, updateStr);
                            break;
                        case COMMUNICATION_TYPE_CAST_TRANSFER_SHEET_TO_TOUCHPAD:
                            dispatchImpl = gVariable.dispatchSheet[myBoardIndex];
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                            {
                                machineName = gVariable.internalMachineName[myBoardIndex];
                                if (gVariable.castRollIndex[myBoardIndex] >= '0' && gVariable.castRollIndex[myBoardIndex] <= '9')
                                {
                                    errorStr = gVariable.productStatusList[gVariable.castRollIndex[myBoardIndex] - '0'];
                                }
                                else
                                {
                                    if (gVariable.castRollIndex[myBoardIndex] >= 'A' && gVariable.castRollIndex[myBoardIndex] <= 'G')
                                        errorStr = gVariable.productStatusList[gVariable.castRollIndex[myBoardIndex] - 'A'];
                                    else
                                    {

                                    }
                                }

                                str = fixedCountStrPlus(dispatchImpl.batchNum + gVariable.castRollIndex[myBoardIndex], 10) + fixedCountStrPlus(dispatchImpl.productCode, 12) +
                                      fixedCountStrPlus(dispatchImpl.BOMCode, 10) + fixedCountStrPlus(dispatchImpl.batchNum, 8) + fixedCountStrPlus(gVariable.castRollIndex[myBoardIndex].ToString(), 10) +
                                      fixedCountStrPlus(dispatchImpl.machineID, 2) + fixedCountStrPlus(dispatchImpl.slitWidth, 6) + fixedCountStrPlus(dispatchImpl.productWeight, 6) +
                                      fixedCountStrPlus(dispatchImpl.workTeam, 6) + fixedCountStrPlus(dispatchImpl.operatorName, 8) + fixedCountStrPlus("", 18) +
                                      fixedCountStrPlus(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), 16) +
                                      fixedCountStrPlus("", 6) + fixedCountStrPlus("", 8) + fixedCountStrPlus("", 18) + fixedCountStrPlus("", 16);
                                    
                                      //fixedCountStrPlus("", 8) + fixedCountStrPlus("", 4) + fixedCountStrPlus("", 6) +
                                      //fixedCountStrPlus("", 8) + fixedCountStrPlus("", 4) + fixedCountStrPlus("", 6) +
                                      //fixedCountStrPlus("", 8) + fixedCountStrPlus("", 4) + fixedCountStrPlus("", 6) +
                                      //fixedCountStrPlus("", 8) + fixedCountStrPlus("", 4) + fixedCountStrPlus("", 6) +
                                      //fixedCountStrPlus("", 8) + fixedCountStrPlus("", 4) + fixedCountStrPlus("", 6) +
                                      //fixedCountStrPlus("", 8) + fixedCountStrPlus("", 4) + fixedCountStrPlus("", 6);

                                aa = System.Text.Encoding.Default.GetBytes(str);
                                len = aa.Length;
                            }
                            else  //send touch pad a empty transfer sheet
                            {
                                machineName = gVariable.internalMachineName[myBoardIndex];
                                if (gVariable.castRollIndex[myBoardIndex] >= '0' && gVariable.castRollIndex[myBoardIndex] <= '9')
                                {
                                    errorStr = gVariable.productStatusList[gVariable.castRollIndex[myBoardIndex] = '0'];
                                }
                                else
                                {

                                }

                                str = fixedCountStrPlus(dispatchImpl.batchNum + gVariable.castRollIndex[myBoardIndex], 10) + fixedCountStrPlus(dispatchImpl.productCode, 12) +
                                      fixedCountStrPlus(dispatchImpl.BOMCode, 10) + fixedCountStrPlus(dispatchImpl.batchNum, 8) + fixedCountStrPlus(gVariable.castRollIndex[myBoardIndex].ToString(), 10) +
                                      fixedCountStrPlus(dispatchImpl.machineID, 2) + fixedCountStrPlus(dispatchImpl.slitWidth, 6) + fixedCountStrPlus(dispatchImpl.productWeight, 6) +
                                      fixedCountStrPlus(dispatchImpl.workTeam, 6) + fixedCountStrPlus(dispatchImpl.operatorName, 8) + fixedCountStrPlus("", 18) +
                                      fixedCountStrPlus(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), 16) +
                                      fixedCountStrPlus("", 6) + fixedCountStrPlus("", 8) + fixedCountStrPlus("", 18) + fixedCountStrPlus("", 16);

                                aa = System.Text.Encoding.Default.GetBytes(str);
                                len = aa.Length;
                            }

                            //input real dispatch data here
                            for (i = 0; i < len; i++)
                                onePacket[i + PROTOCOL_DATA_POS] = aa[i];
                            len += MIN_PACKET_LEN_PURE_FRAME; //pure frame include packet header and CRC, it's 32
                            //send machine status info to touchpad
                            sendDataToClient(onePacket, len, communicationType);
                            break;
                        case COMMUNICATION_TYPE_CAST_TRANSFER_SHEET_TO_PC:
                            len = packetLen - MIN_PACKET_LEN;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                            str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                            strArray = str.Split(';');

                            break;
                        case COMMUNICATION_TYPE_START_HANDSHAKE_WITHOUT_ID_TO_PC:  //this case will not happen any more
                            if (boardIDNotSetFlag == 0)
                                MessageBox.Show("这块数据采集板没有 ID，请换用含有合法 ID 的数据采集板，谢谢！", "信息提示", MessageBoxButtons.OK);
                            boardIDNotSetFlag = 1;
                            break;

                        case COMMUNICATION_TYPE_LARGE_ROLL_ARRIVED_PRINT:  //印刷上卷处扫描大卷
                            zihua_printerClient.notify_printerClient("1806507091L509320030");
                            break;

                        case COMMUNICATION_TYPE_LARGE_ROLL_ARRIVED_SLIT:  //分切上卷处扫描大卷
                            zihua_printerClient.notify_printerClient("1806507091L509320030100");
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
                                //numOfCraftDataInTable = 0;
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
                            else
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
                                //numOfCraftDataInTable = 0;

                                if (myBoardIDFromIPAddress != myBoardID)
                                    Console.WriteLine("board ID:" + myBoardID + " is different from IP ID of " + myBoardIDFromIPAddress);
                            }

                            handshakeWithClientOK = 1;

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

                            //gVariable.dispatchSheet[myBoardIndex].dispatchCode = gVariable.dummyDispatchTableName;  //no dispatch yet, we set it to dummy

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

                        //After board get its time setting, it will send out this packet to indicate communication OK, and a dummy machine code means this board will be running by 
                        //dummy dispatch until real dispatch comes(then switch to real dispatch)
                        case COMMUNICATION_TYPE_SEND_DUMMY_MACHINE_CODE_TO_PC:
                            //                                                    gVariable.currentDispatchCode = "dummy";  //only used for curve display, may not be the same as gVariable.dispatchSheet[myBoardIndex].dispatchCode
                            //toolClass.getDummyData(myBoardIndex);
                            //we got related data, put it in database in the following functiuon, including all data type and dispatch if they does not exist before
                            //readBoardInfoFromMachineDataStruct(1);

                            toolClass.nonBlockingDelay(200);

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
                            prepareForDataNeedSending(onePacket, COMMUNICATION_TYPE_DISPATCH_DATA_FROM_PC, DATA_TYPE_MES_INSTRUCTION);
                            
                            //we get real data of dispatch info from MES
                            ret = readMESString(COMMUNICATION_TYPE_DISPATCH_DATA_FROM_PC);
                            if (ret == RESULT_OK)
                            {
                                str = putVariableToTouchpadFormat(COMMUNICATION_TYPE_DISPATCH_DATA_FROM_PC);
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
                                sendDataToClient(onePacket, len, COMMUNICATION_TYPE_DISPATCH_DATA_FROM_PC);
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
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_DISPATCH_DATA_FROM_PC);
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
                            //gVariable.machineStatus[myBoardIndex].power = currentPower;
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
                                //FileAppendFucntion(checkBytes);
                                qualityDataSN++;
                            }
                            break;

                        //the operator started dispatch from touch pad

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

                            /*
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
                            */
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

                            //FileAppendFucntion(checkBytes);
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
                                //saveUploadedRollInfoIntoDispatchRecord(myBoardIndex, "1804306121L320120030400;452.3");
                                
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

                            //if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                            //    FileAppendFucntion(checkBytes);
                            break;

                        default:
                            break;
                    }

                    onePacket[PROTOCOL_COMMUNICATION_STATUS_POS] = (byte)result;
                    len = MIN_PACKET_LEN;
                    sendDataToClient(onePacket, len, communicationType);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("processClientDataCollectBoard(" + communicationType + "," + packetLen + ") for myBoardIndex " + myBoardIndex + "failed, " + ex);
                }
            }

            public string fixedCountStrPlus(string str, int len)
            {
                int i;
                string str1;
                byte[] dataArray;
                byte[] arr = new byte[100];

                if (str == null)
                    str = "";

                dataArray = Encoding.GetEncoding("gb2312").GetBytes(str);

                if (len > dataArray.Length)
                {
                    for (i = 0; i < len; i++)
                    {
                        if (i < dataArray.Length)
                            arr[i] = dataArray[i];
                        else
                            arr[i] = 0x20;
                    }
                }
                else
                {
                    for (i = 0; i < len; i++)
                    {
                        arr[i] = dataArray[i];
                    }
                }

                str1 = System.Text.Encoding.GetEncoding("gb2312").GetString(arr, 0, len);

                return str1 + ";";
            }
            
            public ClientThread(Socket ClientSocket)
            {
                this.clientSocketInServer = ClientSocket;
                handshakeWithClientOK = 0;
                m_printClient = new zihua_printerClient(this);
                m_showBoardClient = new zihua_showBoardClient(this);
            }
        }
    }
}