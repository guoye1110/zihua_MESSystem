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
        const int RESPONSE_OK = 0;
        const int RESPONSE_FAIL = 1;
        const int RESPONSE_NO_NEED = 2;

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
        //const int MIN_PACKET_LEN_PURE_FRAME = 12;   //header(4) + len(2) + communicationtype(1) + status(1) + data(0) + CRC(4)
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
        const int COMMUNICATION_TYPE_DEVICE_ANDON_STATUS_UPDATED_FROM_PC = 0x8C;  //触屏申请从服务器端更新报警状态
        const int COMMUNICATION_TYPE_DEVICE_ANDON_STATUS_UPDATED_TO_PC = 0xA6;  //触屏申请更新服务器端报警状态
        const int COMMUNICATION_TYPE_CAST_TRANSFER_SHEET_TO_TOUCHPAD = 0x8D;  //流延请求流转单下发
        const int COMMUNICATION_TYPE_CAST_TRANSFER_SHEET_TO_PC = 0x8E;  //流延流转单上传

        const int COMMUNICATION_TYPE_LARGE_ROLL_ARRIVED_PRINT = 0x8F;  //印刷上卷处扫描大卷
        const int COMMUNICATION_TYPE_PRINT_TRANSFER_SHEET_TO_TOUCHPAD = 0x96;  //印刷请求流转单下发
        const int COMMUNICATION_TYPE_PRINT_TRANSFER_SHEET_TO_PC = 0x97;  //印刷流转单上传

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
            //int boardIDNotSetFlag;  //flag to indicate board ID is null, the user need to set an ID to the board
            //int qualityDataSN;
            int myBoardIDFromIPAddress;

            zihua_showBoardClient m_showBoardClient;
            zihua_printerClient m_printClient;


            public void processClientDataCollectBoard(int communicationType, int packetLen)
            {
                int i, j;
                int index;
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
                string fName;
                string machineName;
                string inputBarcode;
                string updateStr;
                string commandText;
                string [] param;
                string[] strArray;
                string[,] tableArray;
                int start;
                DataTable dt;
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
                    len = packetLen - MIN_PACKET_LEN;
                    str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                    strArray = str.Split(';', '&');

                    switch (communicationType)
                    {
                        case COMMUNICATION_TYPE_LOGIN_TO_PC:  //0xA1
                            //strArray[0]: workerID; strArray[1]: password
                            result = RESPONSE_FAIL;
                            dt = toolClass.getDataTableForAccount(strArray[0]);
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (strArray[1] == "1111") //dr[mySQLClass.EMPLOYEE_PASSWORD_COLIUMN].ToString())
                                {
                                    result = RESPONSE_OK;
                                }
                            }
                            break;
                        case COMMUNICATION_TYPE_SETTINGS_TO_PC:  //0xA0
                            result = RESPONSE_OK;
                            break;
                        //an worker put a sack of material into feed bin
                        case COMMUNICATION_TYPE_MATERIAL_BARCODE_TO_PC: //0x80
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED)
                            {
                                //strArray 0: machineID, 1: feedbin ID, 2: quantity, 3: workID
                                if (strArray[0] != (myBoardIndex + 1).ToString())  //one robert work with 2 machines, so if this is not for this machine, simply return;
                                {
                                    result = RESPONSE_OK;
                                    break;
                                }

                                index = Convert.ToInt32(strArray[1]);

                                dName = gVariable.internalMachineName[myBoardIndex];
                                tName = gVariable.materialListTableName;
                                dispatchImpl = gVariable.dispatchSheet[myBoardIndex];

                                commandText = "select * from `" + tName + "` where dispatchcode = '" + dispatchImpl.dispatchCode + "'";
                                tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                                if (tableArray == null)
                                {
                                    Console.WriteLine("material sheet for dispatch code " + dispatchImpl + "not found!");
                                    result = RESPONSE_FAIL;
                                    break;
                                }

                                dName = gVariable.globalDatabaseName;
                                tName = gVariable.materialFeedingTableName;
                                fName = gVariable.materialFeedingFileName;

                                param = new string[10];
                                param[0] = tableArray[0, 6 + index * 5];
                                param[1] = tableArray[0, 0];
                                param[2] = tableArray[0, 1];
                                param[3] = "";
                                param[4] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                param[5] = toolClass.getNameByIDAndIDByName(null, strArray[3]);
                                param[6] = strArray[2];
                                param[7] = gVariable.dispatchSheet[myBoardIndex].batchNum;
                                param[8] = gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                                param[9] = "";

                                mySQLClass.writeFeedBinTable(dName, tName, fName, param);

                                result = RESPONSE_OK;
                            }
                            else
                            {
                                result = RESPONSE_FAIL;
                            }
                        	break;
                        case COMMUNICATION_TYPE_GET_MATERIAL_DATA_FROM_PC: //0x81            -- this is an empty material table
                        case COMMUNICATION_TYPE_MATERIAL_DATA_UPDATE:   //0x82               -- this is a table with data
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED)  //only after dispatch is applied
                            {
                                dName = gVariable.internalMachineName[myBoardIndex];
                                tName = gVariable.materialListTableName;
                                dispatchImpl = gVariable.dispatchSheet[myBoardIndex];

                                commandText = "select * from `" + tName + "` where dispatchcode = '" + dispatchImpl.dispatchCode + "'";
                                tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                                if (tableArray == null)
                                {
                                    Console.WriteLine("material sheet for dispatch code " + dispatchImpl + "not found!");
                                    result = RESPONSE_FAIL;
                                    break;
                                }

                                machineName = gVariable.internalMachineName[myBoardIndex];
                                str = fixedCountStrPlus(machineName, 10) + fixedCountStrPlus(dispatchImpl.productCode, 12) + fixedCountStrPlus(dispatchImpl.batchNum + gVariable.machineCodeArrayDatabase[myBoardIndex], 10) +
                                      fixedCountStrPlus(dispatchImpl.BOMCode, 10) + fixedCountStrPlus(dispatchImpl.operatorName, 8) +
                                      fixedCountStrPlus(dispatchImpl.planTime1, 16) + fixedCountStrPlus(dispatchImpl.planTime2, 16) + fixedCountStrPlus(dispatchImpl.realStartTime, 16) +
                                      fixedCountStrPlus(dispatchImpl.realFinishTime, 16) +
                                      fixedCountStrPlus(tableArray[0, 6], 12) + fixedCountStrPlus(tableArray[0, 11], 12) + fixedCountStrPlus(tableArray[0, 16], 12) + fixedCountStrPlus(tableArray[0, 21], 12) +
                                      fixedCountStrPlus(tableArray[0, 26], 12) + fixedCountStrPlus(tableArray[0, 31], 12) + fixedCountStrPlus(tableArray[0, 36], 12) +
                                      fixedCountStrPlus(tableArray[0, 7], 12) + fixedCountStrPlus(tableArray[0, 12], 12) + fixedCountStrPlus(tableArray[0, 17], 12) + fixedCountStrPlus(tableArray[0, 22], 12) +
                                      fixedCountStrPlus(tableArray[0, 27], 12) + fixedCountStrPlus(tableArray[0, 32], 12) + fixedCountStrPlus(tableArray[0, 37], 12) +
                                      fixedCountStrPlus(tableArray[0, 8], 12) + fixedCountStrPlus(tableArray[0, 13], 12) + fixedCountStrPlus(tableArray[0, 18], 12) + fixedCountStrPlus(tableArray[0, 27], 12) +
                                      fixedCountStrPlus(tableArray[0, 28], 12) + fixedCountStrPlus(tableArray[0, 33], 12) + fixedCountStrPlus(tableArray[0, 38], 12) +
                                      fixedCountStrPlus(tableArray[0, 9], 12) + fixedCountStrPlus(tableArray[0, 14], 12) + fixedCountStrPlus(tableArray[0, 19], 12) + fixedCountStrPlus(tableArray[0, 28], 12) +
                                      fixedCountStrPlus(tableArray[0, 29], 12) + fixedCountStrPlus(tableArray[0, 34], 12) + fixedCountStrPlus(tableArray[0, 39], 12) +
                                      fixedCountStrPlus(tableArray[0, 10], 12) + fixedCountStrPlus(tableArray[0, 15], 12) + fixedCountStrPlus(tableArray[0, 20], 12) + fixedCountStrPlus(tableArray[0, 29], 12) +
                                      fixedCountStrPlus(tableArray[0, 30], 12) + fixedCountStrPlus(tableArray[0, 35], 12) + fixedCountStrPlus(tableArray[0, 41], 12);

                                aa = System.Text.Encoding.Default.GetBytes(str);
                                len = aa.Length;

                                //input real dispatch data here
                                for (i = 0; i < len; i++)
                                    onePacket[i + PROTOCOL_DATA_POS] = aa[i];
                                len += MIN_PACKET_LEN;
                                //send machine status info to touchpad
                                sendDataToClient(onePacket, len, communicationType);
                                result = RESPONSE_NO_NEED;
                            }
                            else
                            {
                                result = RESPONSE_FAIL;
                            }
                        	break;
                        case COMMUNICATION_TYPE_MATERIAL_COMPLETE:   //0x83
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED)  //only after dispatch is applied
                            {
                                len = packetLen - MIN_PACKET_LEN;
                                str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                                strArray = str.Split(';');

                                tName = gVariable.materialListTableName;
                                dName = toolClass.getDatabaseNameByMachineID(myBoardIndex + 1);

                                updateStr = "update `" + tName + "` set currentlyUsed1 = '" + strArray[9] + "', currentLeft1 = '" + strArray[10] +
                                            "', currentLeft2 = '" + strArray[14] + "', currentLeft2 = '" + strArray[15] +
                                            "', currentLeft3 = '" + strArray[19] + "', currentLeft3 = '" + strArray[20] +
                                            "', currentLeft4 = '" + strArray[24] + "', currentLeft4 = '" + strArray[25] +
                                            "', currentLeft5 = '" + strArray[29] + "', currentLeft5 = '" + strArray[30] +
                                            "', currentLeft6 = '" + strArray[34] + "', currentLeft6 = '" + strArray[35] +
                                            "', currentLeft7 = '" + strArray[39] + "', currentLeft7 = '" + strArray[40] + "', materialStatus = '0000000'" +
                                            "' where dispatchCode = '" + gVariable.dispatchSheet[myBoardIndex].dispatchCode + "'";

                                mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, updateStr);
                                result = RESPONSE_OK;
                            }
                            else
                            {
                                result = RESPONSE_FAIL;
                            }
                            break;

                        case COMMUNICATION_TYPE_MATERIAL_ANDON_TO_PC:   //0x84
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED)  //only after dispatch is applied
                            {
                                str = null;
                                errorStr = null;
                                for (i = 0; i < 7; i++)
                                {
                                    j = Convert.ToInt32(strArray[8 + i * 2].Trim());
                                    if (j >= gVariable.ALARM_STATUS_APPLIED)
                                    {
                                        if (str != null)
                                        {
                                            str += "," + (i + 1);
                                        }
                                        else
                                        {
                                            str = (i + 1).ToString();
                                        }
                                        errorStr += "1";
                                    }
                                    else
                                    {
                                        errorStr += "0";
                                    }
                                }
                                alarmTableStructImpl.operatorName = strArray[6].Trim();
                                alarmTableStructImpl.alarmFailureCode = DateTime.Now.ToString("MMddHHmm") + "-" + (gVariable.andonAlarmIndex + 1);
                                alarmTableStructImpl.errorDesc = "号料仓缺料";
                                alarmTableStructImpl.dispatchCode = strArray[0].Trim();
                                alarmTableStructImpl.feedBinID = "";
                                alarmTableStructImpl.machineName = gVariable.internalMachineName[myBoardIndex];
                                alarmTableStructImpl.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                alarmTableStructImpl.type = gVariable.ALARM_TYPE_MATERIAL;
                                alarmTableStructImpl.category = gVariable.ALARM_CATEGORY_DEVICE_START;
                                alarmTableStructImpl.status = gVariable.ALARM_STATUS_APPLIED;
                                alarmTableStructImpl.startID = gVariable.ALARM_NO_STARTID;
                                alarmTableStructImpl.indexInTable = gVariable.ALARM_NO_INDEX_IN_TABLE;

                                alarmTableStructImpl.workshop = gVariable.machineWorkshopDatabase[myBoardIndex];

                                alarmTableStructImpl.mailList = toolClass.getAlarmMailList();

                                alarmIDInTable = mySQLClass.writeAlarmTable(databaseNameThis, gVariable.alarmListTableName, alarmTableStructImpl);
                                toolClass.processNewAlarm(databaseNameThis, alarmIDInTable);

                                tName = gVariable.materialListTableName;
                                dName = toolClass.getDatabaseNameByMachineID(myBoardIndex + 1);
                                commandText = "update `" + tName + "` set materialStatus = '" + errorStr + "' where dispatchCode = '" + gVariable.dispatchSheet[myBoardIndex].dispatchCode + "'";
                                mySQLClass.pureDatabaseNonQueryAction(gVariable.globalDatabaseName, commandText);

                                gVariable.andonAlarmIndex++;
                                result = RESPONSE_OK;
                            }
                            else
                            {
                                result = RESPONSE_FAIL;
                            }
                            break;
                        case COMMUNICATION_TYPE_MATERIAL_ANDON_STATUS_UPDATE:  //0x85
                            tName = gVariable.materialListTableName;
                            dName = toolClass.getDatabaseNameByMachineID(myBoardIndex + 1);

                            commandText = "select errorStatus from `" + tName + "` where dispatchCode = '" + gVariable.dispatchSheet[myBoardIndex].dispatchCode + "'";
                            tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                            if(tableArray != null)
                            {
                                str = tableArray[0, 0];
                            }
                            else
                                str = null;
                            
                            aa = System.Text.Encoding.Default.GetBytes(str);
                            len = aa.Length;

                            //input real dispatch data here
                            for (i = 0; i < len; i++)
                                onePacket[i + PROTOCOL_DATA_POS] = aa[i];
                            len += MIN_PACKET_LEN;
                            //send machine status info to touchpad
                            sendDataToClient(onePacket, len, communicationType);
                            result = RESPONSE_NO_NEED;
                            break;

                        //case COMMUNICATION_TYPE_CAST_BARCODE_TO_PC: // 0x86     -- no such function
                        //    break;

                        //these 2 functions are the same, we simply send dispatch data to touchpad
                        case COMMUNICATION_TYPE_APPLY_DISPATCH_TO_PC:  //0x87
                        case COMMUNICATION_TYPE_DISPATCH_UPDATE_TO_PC:  //0x89
                        case 0x90: //print process
                        case 0x92:
                        case 0x99:  //slit process
                        case 0x9B:
                            ret = dispatchTools.getCurrentProductTask(myBoardIndex);
                            if(ret < 0)
                            {
                                result = RESPONSE_FAIL;
                                break;
                            }
                            result = dispatchTools.getCurrentDispatch(myBoardIndex);
                            if (result < 0)
                            {
                                result = RESPONSE_FAIL;
                                break;
                            }

                            dispatchImpl = gVariable.dispatchSheet[myBoardIndex];

                            if (ret == 0)  //this is a new task
                            {
                                gVariable.productTaskSheet[myBoardIndex].status = gVariable.MACHINE_STATUS_DISPATCH_APPLIED;
                                gVariable.productTaskSheet[myBoardIndex].realStartTime = null;
                            }
                            else
                            {
                                //this is an old task, don't change anything
                            }

                            if(result == 0)  //this is a new dispatch
                            {
                                gVariable.dispatchSheet[myBoardIndex].status = gVariable.MACHINE_STATUS_DISPATCH_APPLIED;
                                gVariable.machineCurrentStatus[myBoardIndex] = gVariable.MACHINE_STATUS_DISPATCH_APPLIED;
                            }
                            else
                            {
                                //this is an old dispatch, don't change anything
                            }

                            machineName = gVariable.internalMachineName[myBoardIndex];

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
                            len += MIN_PACKET_LEN;
                            //send machine status info to touchpad
                            sendDataToClient(onePacket, len, communicationType);
                            result = RESPONSE_NO_NEED;
                            break;
                        case COMMUNICATION_TYPE_DISPATCH_START_TO_PC:  //0x88
                        case 0x91:
                        case 0x9A:
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED)
                            {
                                gVariable.dispatchSheet[myBoardIndex].realStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                gVariable.machineCurrentStatus[myBoardIndex] = gVariable.MACHINE_STATUS_DISPATCH_STARTED;
                                gVariable.dispatchSheet[myBoardIndex].qualifiedNumber = 0;
                                gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber = 0;
                                gVariable.dispatchSheet[myBoardIndex].status = gVariable.MACHINE_STATUS_DISPATCH_STARTED;

                                gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DEVICE, myBoardIndex] = 0;
                                gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DATA, myBoardIndex] = 0;

                                //record start info for a dispatch, null means dispatch code is defined by myBoardIndex
                                mySQLClass.updateDispatchTable(gVariable.internalMachineName[myBoardIndex], gVariable.dispatchListTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_STARTED, null);
                                //also set this dispatch in global database to the status of started
                                mySQLClass.updateDispatchTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_STARTED, null);

                                //there is no update of start time after the completion of the previous production task, so this is a start of a product task
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
                            }
                            onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                            len = MIN_PACKET_LEN;
                            sendDataToClient(onePacket, len, communicationType);
                            result = RESPONSE_NO_NEED;
                            break;
                        case COMMUNICATION_TYPE_TASK_COMPLETED_TO_PC:   //0x8A, product task completed, if only dispatch complete, won't come to this place
                        case 0x93:
                        case 0x9C:
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                            {
                                gVariable.machineCurrentStatus[myBoardIndex] = gVariable.MACHINE_STATUS_DISPATCH_COMPLETED; //quality/craft/andon data are illegal
                                gVariable.dispatchSheet[myBoardIndex].status = gVariable.MACHINE_STATUS_DISPATCH_COMPLETED;
                                gVariable.dispatchSheet[myBoardIndex].realFinishTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                gVariable.productTaskSheet[myBoardIndex].status = gVariable.MACHINE_STATUS_DISPATCH_COMPLETED;
                                gVariable.productTaskSheet[myBoardIndex].realFinishTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                //record start info for a dispatch, null means dispatch code is defined by myBoardIndex
                                mySQLClass.updateDispatchTable(gVariable.internalMachineName[myBoardIndex], gVariable.dispatchListTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_COMPLETED, null);
                                //also set this dispatch in global database to the status of completed
                                mySQLClass.updateDispatchTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_COMPLETED, null);

                                mySQLClass.updateDispatchTable(gVariable.internalMachineName[myBoardIndex], gVariable.productTaskListTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_COMPLETED, null);
                                mySQLClass.updateDispatchTable(gVariable.globalDatabaseName, gVariable.globalProductTaskTableName, myBoardIndex, gVariable.MACHINE_STATUS_DISPATCH_COMPLETED, null);
                                onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                                len = MIN_PACKET_LEN;
                                sendDataToClient(onePacket, len, communicationType);
                                result = RESPONSE_NO_NEED;
                            }
                            else
                            {
                                result = RESPONSE_FAIL;
                            }
                            break;
                        case COMMUNICATION_TYPE_DEVICE_ANDON_TO_PC: //0x8B
                        case 0x94:
                        case 0x9D:
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
                            result = RESPONSE_OK;
                            break;
                        case COMMUNICATION_TYPE_DEVICE_ANDON_STATUS_UPDATED_FROM_PC:   //0x8C
                        case 0x95:
                        case 0x9E:
                            machineName = gVariable.internalMachineName[myBoardIndex];

                            dName = gVariable.internalMachineName[myBoardIndex];
                            tName = gVariable.alarmListTableName;

                            commandText = "select * from `" + tName + "` where status < '" + gVariable.ALARM_STATUS_COMPLETED + "'";
                            tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                            if (tableArray == null)
                            {
                                Console.WriteLine("No uncompleted andon alarm found!");
                                result = RESPONSE_OK;
                                break;
                            }

                            str = machineName;
                            for (i = 0; i < 3; i++)
                            {
                                str = fixedCountStrPlus(tableArray[i, 3], 10) + ";" + fixedCountStrPlus(tableArray[i, 7], 8) + ";" + fixedCountStrPlus(tableArray[i, 6], 12) + ";" +
                                      fixedCountStrPlus(tableArray[i, 1], 20) + ";" + fixedCountStrPlus(tableArray[i, 14], 8);
                            }
                            aa = System.Text.Encoding.Default.GetBytes(str);
                            len = aa.Length;

                            //input real dispatch data here
                            for (i = 0; i < len; i++)
                                onePacket[i + PROTOCOL_DATA_POS] = aa[i];
                            len += MIN_PACKET_LEN;
                            //send machine status info to touchpad
                            sendDataToClient(onePacket, len, communicationType);
                            result = RESPONSE_NO_NEED;
                            break;
                        case COMMUNICATION_TYPE_DEVICE_ANDON_STATUS_UPDATED_TO_PC:  //0xA6
                            dName = gVariable.internalMachineName[myBoardIndex];
                            tName = gVariable.alarmListTableName;

                            len = packetLen - MIN_PACKET_LEN;
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
                            result = RESPONSE_OK;
                            break;
                        case COMMUNICATION_TYPE_CAST_TRANSFER_SHEET_TO_TOUCHPAD:   //0x8D
                            dispatchImpl = gVariable.dispatchSheet[myBoardIndex];
                            if(dispatchImpl.batchNum.Length < gVariable.batchNumLength)  //
                            {
                                Console.WriteLine("COMMUNICATION_TYPE_CAST_TRANSFER_SHEET_TO_TOUCHPAD, dispatch format error!");
                                break;
                            }
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                            {
                                machineName = gVariable.internalMachineName[myBoardIndex];
                                if (gVariable.currentLargeRollIndex[myBoardIndex] >= '0' && gVariable.currentLargeRollIndex[myBoardIndex] <= '9')
                                {
                                    errorStr = gVariable.productStatusList[gVariable.currentLargeRollIndex[myBoardIndex] - '0'];
                                }
                                else
                                {
                                    if (gVariable.currentLargeRollIndex[myBoardIndex] >= 'A' && gVariable.currentLargeRollIndex[myBoardIndex] <= 'G')
                                        errorStr = gVariable.deviceErrDescList[gVariable.currentLargeRollIndex[myBoardIndex] - 'A'];
                                    else
                                    {
                                        errorStr = "数据错误：" + (gVariable.currentLargeRollIndex[myBoardIndex] - 'A');
                                    }
                                }

                                str = fixedCountStrPlus(dispatchImpl.batchNum + gVariable.currentLargeRollIndex[myBoardIndex], 10) + fixedCountStrPlus(dispatchImpl.productCode, 12) +
                                      fixedCountStrPlus(dispatchImpl.BOMCode, 10) + fixedCountStrPlus(dispatchImpl.batchNum, 8) + fixedCountStrPlus(gVariable.currentLargeRollIndex[myBoardIndex].ToString(), 10) +
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
                                errorStr = " ";

                                str = fixedCountStrPlus(dispatchImpl.batchNum + gVariable.currentLargeRollIndex[myBoardIndex], 10) + fixedCountStrPlus(dispatchImpl.productCode, 12) +
                                      fixedCountStrPlus(dispatchImpl.BOMCode, 10) + fixedCountStrPlus(dispatchImpl.batchNum, 8) + fixedCountStrPlus(gVariable.currentLargeRollIndex[myBoardIndex].ToString(), 10) +
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
                            len += MIN_PACKET_LEN;
                            //send machine status info to touchpad
                            sendDataToClient(onePacket, len, communicationType);
                            result = RESPONSE_NO_NEED;
                            break;
                        case COMMUNICATION_TYPE_CAST_TRANSFER_SHEET_TO_PC:  
                            dName = gVariable.internalMachineName[myBoardIndex];  //0x8E
                            tName = gVariable.alarmListTableName;

                            len = packetLen - MIN_PACKET_LEN;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                            str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                            strArray = str.Split(';');
                            i = 17;

                            updateStr = "update `" + tName + "` set number1 = '" + strArray[i++] + "', number2 = '" + strArray[i++] + "', number3 = '" + strArray[i++] +
                                        "' number4 = '" + strArray[i++] + "', number5 = '" + strArray[i++] + "', number6 = '" + strArray[i++] + "', number7 = '" + strArray[i++] +   
                                        "' number8 = '" + strArray[i++] + "', number9 = '" + strArray[i++] + "', number10 = '" + strArray[i++] + "', number11 = '" + strArray[i++] +   
                                        "' number12 = '" + strArray[i++] + "', number13 = '" + strArray[i++] + "', number14 = '" + strArray[i++] + "', number15 = '" + strArray[i++] +   
                                        "' number16 = '" + strArray[i++] + "', number17 = '" + strArray[i++] + "', number18 = '" + strArray[i++] + 
                                       "' where transferCode = '" + strArray[1] + "'";
                            mySQLClass.updateTableItems(dName, updateStr);
                            result = RESPONSE_OK;
                            break;
                        case COMMUNICATION_TYPE_LARGE_ROLL_ARRIVED_PRINT:  //印刷上卷处扫描大卷
                            len = packetLen - MIN_PACKET_LEN;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                            str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                            strArray = str.Split(';');

                            inputBarcode = strArray[0];

                            //first tell printing SW server a new cast roll arrived, we can tell printing SW client side -- which client should we go
                            //if (machineID >= gVariable.printingProcess[0] && machineID <= gVariable.printingProcess[gVariable.printingProcess.Length - 1])
                            m_printClient.notify_printerClient(inputBarcode, 0); //, machineID);

                            break;

                            //then send transfer sheet to printing touch pad
                            dispatchImpl = gVariable.dispatchSheet[myBoardIndex];
                            if(dispatchImpl.batchNum.Length < gVariable.batchNumLength)  //
                            {
                                Console.WriteLine("COMMUNICATION_TYPE_LARGE_ROLL_ARRIVED_PRINT, dispatch format error!");
                                break;
                            }
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                            {
                                machineName = gVariable.internalMachineName[myBoardIndex];
                                if (gVariable.currentRollStatus[myBoardIndex] >= '0' && gVariable.currentRollStatus[myBoardIndex] <= '9')
                                {
                                    errorStr = gVariable.productStatusList[gVariable.currentRollStatus[myBoardIndex] - '0'];
                                }
                                else
                                {
                                    if (gVariable.currentRollStatus[myBoardIndex] >= 'A' && gVariable.currentRollStatus[myBoardIndex] <= 'F')
                                        errorStr = gVariable.deviceErrDescList[gVariable.currentRollStatus[myBoardIndex] - 'A'];
                                    else if (gVariable.currentRollStatus[myBoardIndex] >= 'a' && gVariable.currentRollStatus[myBoardIndex] <= 'f')
                                        errorStr = gVariable.deviceErrDescList[gVariable.currentRollStatus[myBoardIndex] - 'a'];
                                    else
                                    {
                                        errorStr = "数据错误：" + (gVariable.currentRollStatus[myBoardIndex] - 'A');
                                    }
                                }

                                str = fixedCountStrPlus(dispatchImpl.batchNum + gVariable.currentLargeRollIndex[myBoardIndex], 10) + fixedCountStrPlus(dispatchImpl.productCode, 12) +
                                      fixedCountStrPlus(dispatchImpl.BOMCode, 10) + fixedCountStrPlus(dispatchImpl.batchNum, 8) + fixedCountStrPlus(gVariable.currentLargeRollIndex[myBoardIndex].ToString(), 10) +
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
                                aa = System.Text.Encoding.Default.GetBytes("0");
                                len = aa.Length;
                            }

                            //input real dispatch data here
                            for (i = 0; i < len; i++)
                                onePacket[i + PROTOCOL_DATA_POS] = aa[i];
                            len += MIN_PACKET_LEN;
                            //send machine status info to touchpad
                            sendDataToClient(onePacket, len, communicationType);
                            result = RESPONSE_NO_NEED;
                            break;
                        case COMMUNICATION_TYPE_LARGE_ROLL_ARRIVED_SLIT:  //分切上卷处扫描大卷
                            len = packetLen - MIN_PACKET_LEN;  //MIN_PACKET_LEN include one byte of data, so we need to delete this byte
                            str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                            strArray = str.Split(';');

                            inputBarcode = strArray[0];
                            //inputBarcode = "1806507091L509320030100";  //for testing

                            //first tell printing SW server a new large roll arrived, we can tell printing SW client side -- which client should we go
                            m_printClient.notify_printerClient(inputBarcode, 0);

                            break;

                            //then send transfer sheet to slitting touch pad
                            dispatchImpl = gVariable.dispatchSheet[myBoardIndex];
                            if(dispatchImpl.batchNum.Length < gVariable.batchNumLength)  //
                            {
                                Console.WriteLine("COMMUNICATION_TYPE_LARGE_ROLL_ARRIVED_PRINT, dispatch format error!");
                                break;
                            }
                            if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_STARTED)
                            {
                                machineName = gVariable.internalMachineName[myBoardIndex];
                                if (gVariable.currentRollStatus[myBoardIndex] >= '0' && gVariable.currentRollStatus[myBoardIndex] <= '9')
                                {
                                    errorStr = gVariable.productStatusList[gVariable.currentRollStatus[myBoardIndex] - '0'];
                                }
                                else
                                {
                                    if (gVariable.currentRollStatus[myBoardIndex] >= 'A' && gVariable.currentRollStatus[myBoardIndex] <= 'F')
                                        errorStr = gVariable.deviceErrDescList[gVariable.currentRollStatus[myBoardIndex] - 'A'];
                                    else if (gVariable.currentRollStatus[myBoardIndex] >= 'a' && gVariable.currentRollStatus[myBoardIndex] <= 'f')
                                        errorStr = gVariable.deviceErrDescList[gVariable.currentRollStatus[myBoardIndex] - 'a'];
                                    else
                                    {
                                        errorStr = "数据错误：" + (gVariable.currentRollStatus[myBoardIndex] - 'A');
                                    }
                                }

                                str = fixedCountStrPlus(dispatchImpl.batchNum + gVariable.currentLargeRollIndex[myBoardIndex], 10) + fixedCountStrPlus(dispatchImpl.productCode, 12) +
                                      fixedCountStrPlus(dispatchImpl.BOMCode, 10) + fixedCountStrPlus(dispatchImpl.batchNum, 8) + fixedCountStrPlus(gVariable.currentLargeRollIndex[myBoardIndex].ToString(), 10) +
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
                                aa = System.Text.Encoding.Default.GetBytes("0");
                                len = aa.Length;
                            }

                            //input real dispatch data here
                            for (i = 0; i < len; i++)
                                onePacket[i + PROTOCOL_DATA_POS] = aa[i];
                            len += MIN_PACKET_LEN;
                            //send machine status info to touchpad
                            sendDataToClient(onePacket, len, communicationType);
                            result = RESPONSE_NO_NEED;
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
                            //qualityDataSN = 0;

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
                                len += MIN_PACKET_LEN - 2; //pure frame include packet header and CRC, it's 16, so need to minus 2, touchpad need a 2 byte CRC
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

                        case COMMUNICATION_TYPE_MATERIAL_DATA_REQ:
                            readMESString(COMMUNICATION_TYPE_MATERIAL_DATA_REQ);

                            str = putVariableToTouchpadFormat(COMMUNICATION_TYPE_MATERIAL_DATA_REQ);

                            aa = System.Text.Encoding.Default.GetBytes(str);

                            len = aa.Length;
                            for (i = 0; i < len; i++)
                                onePacket[i + PROTOCOL_DATA_POS] = aa[i];

                            len += MIN_PACKET_LEN;
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
                            len += MIN_PACKET_LEN;
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
                        case -2: //COMMUNICATION_TYPE_CYCLE_TIME_TO_PC:
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

                    if(result != RESPONSE_NO_NEED)
                    {
                        onePacket[PROTOCOL_COMMUNICATION_STATUS_POS] = (byte)result;
                        len = MIN_PACKET_LEN;
                        sendDataToClient(onePacket, len, communicationType);
                    }
                    else
                    {
                        //response already sent, no more action here needed
                    }
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
 
           

            //we only know dispatch code, need to get more information related to this dispatch code, which machine this process works on, including cast/print/slit(slit could be worked on several machines)
            int getMachineIDsByDispatchCode(string dispatchCode, int [] machineIDArray)
            {
                int i;
                int index;
                string str;
                string commandText;
                string dName;
                DataTable dTable;

                try
                {
                    machineIDArray[0] = Convert.ToInt32(dispatchCode.Remove(0, 5).Remove(1));
                    machineIDArray[1] = -1;    //may be there is no print procress or print process is not started
                    machineIDArray[2] = -1;    //slit process is not started

                    for (i = 0; i < gVariable.printingProcess.Length; i++)
                    {
                        dName = toolClass.getDatabaseNameByMachineID(i + 1);
                        str = dispatchCode.Remove(11) + "Y" + (i + 1);
                        commandText = "select * from `" + gVariable.dispatchListTableName + "` where dispatchcode = '" + str + "'";
                        dTable = mySQLClass.queryDataTableAction(dName, commandText, null);
                        if (dTable != null)
                        {
                            machineIDArray[1] = (i + 1);
                            break;
                        }
                    }

                    index = 2;
                    for (i = 0; i < gVariable.slittingProcess.Length; i++)
                    {
                        dName = toolClass.getDatabaseNameByMachineID(i + 1);
                        str = dispatchCode.Remove(11) + "F" + (i + 1);
                        commandText = "select * from `" + gVariable.dispatchListTableName + "` where dispatchcode = '" + str + "'";
                        dTable = mySQLClass.queryDataTableAction(dName, commandText, null);
                        if (dTable != null)
                        {
                            machineIDArray[index++] = (i + 1);
                        }
                    }

                    return index;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("getMachineIDsByDispatchCode() failed !" + ex);
                }
                return -1;
            }

            public void debugInfoDisplay(int packetLen)
            {
                int i;
                int len;
                string str;
                string[] strArray;

                len = packetLen - MIN_PACKET_LEN;
                str = System.Text.Encoding.Default.GetString(onePacket, PROTOCOL_DATA_POS, len);
                strArray = str.Split(';');

                for (i = 0; i < strArray.Length; i++)
                {
                    Console.WriteLine("line " + (i + 1) + ": " + strArray[i]);
                }
            }


            public ClientThread(Socket ClientSocket)
            {
                Console.WriteLine("a new Clientthread generated, ClientSocket is :" + ClientSocket);
                this.clientSocketInServer = ClientSocket;
                handshakeWithClientOK = 0;
                m_printClient = new zihua_printerClient(this);
                m_showBoardClient = new zihua_showBoardClient(this);
            }

            void clientThreadRemove()
            {
                clientSocketInServer.Close();
                clientSocketInServer = null;
                m_printClient.printerClientRemoveEvent();
                m_printClient = null;
                m_showBoardClient = null;
            }
        }
    }
}