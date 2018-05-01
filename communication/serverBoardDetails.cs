using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using MESSystem.common;

namespace MESSystem.communication
{
    public partial class communicate
    {
        //return type definition
        const int RESULT_OK = 0;
        const int RESULT_ERR_NO_DATA_RECEIVED = 1;
        const int RESULT_SET_BOARD_ID_COMPLETE = 2;
        const int RESULT_ERR_SYSTEM_NOT_READY = 3;
        const int RESULT_NOT_COMPLETE_YET = 6;
        const int RESULT_ERR_FILE_OPEN_FAIL = 10;
        const int RESULT_ERR_FILE_READ_FAIL = 11;
        const int RESULT_ERR_FILE_WRITE_FAIL = 12;
        const int RESULT_ERR_TIMEOUT = 20;
        const int RESULT_ERR_DATA = 21;
        const int RESULT_ERR_CRC = 22;
        const int RESULT_ERR_BOARDID = 23;
        const int RESULT_ERR_DATA_OVERFLLOW = 24;
        const int RESULT_ERR_DATA_NOT_COMPLETE = 25;
        const int RESULT_ERR_FILE_NOT_FOUND = 26;
        const int RESULT_ERR_NO_MEMORY = 27;
        const int RESULT_ERR_NOT_STARTED_YET = 28;
        const int RESULT_ERR_DATA_NUM_WRONG = 29;
        const int RESULT_ERR_NO_DATA_AVAILABLE = 30;

        //error port setting type
        const int RESULT_ERR_PORT_INDEX_EXCEED_LIMIT = 30;
        const int RESULT_ERR_PORT_INDEX_REDUNDANT = 31;
        const int RESULT_ERR_PORT_ADC_NO_RANGE = 32;
        const int RESULT_ERR_PORT_TYPE_NOT_DEFINED = 33;
        const int RESULT_ERR_PORT_TYPE_INDEX_NOT_DEFINED = 34;
        const int RESULT_ERR_PORT_REPORT_FREQ_NOT_DEFINED = 35;

        //when we got data from data collect board, it contains a column item of dataType which indicates whant kind of data this is, dataType includes the following definitions:
        const int DATA_TYPE_ADC_DEVICE = 0; //device type definition
        const int DATA_TYPE_VOL_CUR_DEVICE = 1;
        const int DATA_TYPE_RF_DEVICE = 2;
        const int DATA_TYPE_CRAFT_PARAM = 3;
        const int DATA_TYPE_QUALITY_DATA = 4;
        const int DATA_TYPE_BEAT_PERIOD = 5;
        const int DATA_TYPE_GPIO_DEVICE = 10;
        const int DATA_TYPE_SCANNER_DEVICE = 11;
        const int DATA_TYPE_PRINTER_DEVICE = 12;
        const int DATA_TYPE_RFID_DEVICE = 13;
        const int DATA_TYPE_CAN_DEVICE = 14;
        const int DATA_TYPE_MODBUS_DEVICE = 15;
        const int DATA_TYPE_ETHERNET_DEVICE = 16;
        const int DATA_TYPE_USB_DEVICE = 17;
        const int DATA_TYPE_WIFI_DEVICE = 18;
        const int DATA_TYPE_MES_INSTRUCTION = 19;
        const int DATA_TYPE_DATA_UNKNOWN = 30;  //when we get a wrong data packet which dailed in length/CRC check, we will send a failed response to client, the data type will be set to this one 


        //communication between MES/PC server/data collect board/touchpad
        const int COMMUNICATION_TYPE_START_HANDSHAKE_WITHOUT_ID_TO_PC = 0;
        const int COMMUNICATION_TYPE_REDO_HANDSHAKE_TO_BOARD = 0x82;
        const int COMMUNICATION_TYPE_START_HANDSHAKE_WITH_ID_TO_PC = 3;
        const int COMMUNICATION_TYPE_ADC_SETTING_TO_BOARD = 0x81;
        const int COMMUNICATION_TYPE_BEAT_SETTING_TO_BOARD = 0x85;
        const int COMMUNICATION_TYPE_UART_SETTING_TO_BOARD = 0x84;
        const int COMMUNICATION_TYPE_GPIO_SETTING_TO_BOARD = 0x83;
        const int COMMUNICATION_TYPE_SET_BOARD_TIME_TO_TOUCHPAD = 0x86;
        const int COMMUNICATION_TYPE_STOP_COMMUNICATION_TO_BOARD = 0x0c;
        const int COMMUNICATION_TYPE_START_COMMUNICATION_TO_BOARD = 0x0d;
        const int COMMUNICATION_TYPE_COLLECTED_DATA_SEND_TO_PC = 0x0e;
        const int COMMUNICATION_TYPE_SEND_DUMMY_MACHINE_CODE_TO_PC = 0x10;
        const int COMMUNICATION_TYPE_DISPATCH_START_TO_PC = 0x18;  //工单启动
        const int COMMUNICATION_TYPE_DISPATCH_DATA_TO_TOUCHPAD = 0x91;  //工单下发
        const int COMMUNICATION_TYPE_DISPATCH_COMPLETED_TO_PC = 0x12;        //报工上传
        const int COMMUNICATION_TYPE_ERROR_LIST_TO_TOUCHPAD = 0x93;     //故障列表下发, this function is emplemented at firstScreen.cs
        const int COMMUNICATION_TYPE_MATERIAL_DATA_REQ = 0x22;  //物料单请求
        const int COMMUNICATION_TYPE_SCANNING_TO_PC = 0x29;  //触摸屏扫描枪数据上传
        const int COMMUNICATION_TYPE_MATERIAL_INQUIRY_TO_TOUCHPAD = 0x94;  //物料单下发
        const int COMMUNICATION_TYPE_MATERIAL_ANDON_TO_PC = 0x15;  //物料预警上传
        const int COMMUNICATION_TYPE_CRAFT_PARAMETER_REQUEST = 0x16;  //工艺参数请求
        const int COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_TOUCHPAD = 0x96;  //工艺参数下发
        const int COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_PC = 0x17;  //工艺参数上传
        const int COMMUNICATION_TYPE_QUALITY_DATA_REQUEST = 0x20;  //质量数据请求
        const int COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD = 0x98;  //质量数据下发
        const int COMMUNICATION_TYPE_QUALITY_DATA_TO_PC = 0x19;  //质量数据上传
        const int COMMUNICATION_TYPE_MACHINE_STATUS_TO_PC = 0x1A;  //设备运行状态上传
        const int COMMUNICATION_TYPE_MACHINE_STATUS_REQUEST = 0x13;  //设备运行状态请求
        const int COMMUNICATION_TYPE_MACHINE_STATUS_DISPLAY = 0x8E;  //设备运行状态下发, internal use, won't be sent to board
        const int COMMUNICATION_TYPE_MACHINE_STATUS_TO_TOUCHPAD = 0x1D; //设备运行状态下发
        const int COMMUNICATION_TYPE_DEVICE_ANDON_TO_PC = 0x1B;  //设备安灯报警上传
        const int COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_TO_TOUCHPAD = 0x9c;  //设备安灯报警查询结果下发
        const int COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_TO_REQ = 0x21; //设备安灯报警查询
        const int COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_DEAL_TO_PC = 0x26;  //安灯报警改变状态(包括各类安灯的签到，解决，取消)
        const int COMMUNICATION_TYPE_STAFF_INQUIRY_TO_TOUCHPAD = 0x9D;  //车间人员查询, this function is emplemented at firstScreen.cs
        const int COMMUNICATION_TYPE_SETTING_TO_PC = 0x1E;  //接口功能设置上传

        const int COMMUNICATION_TYPE_ADJEST_TIME_TO_EDS = 0x1F;  //调整时间上传
        const int COMMUNICATION_TYPE_CYCLE_TIME_TO_PC = 0x9E;  //生产节拍时间上传

        const int COMMUNICATION_TYPE_MACHINE_CRAFT_PARAM_ALARM_TO_PC = 0x2c;
        const int COMMUNICATION_TYPE_DATA_ALARM_TO_PC = 0x2e;
        const int COMMUNICATION_TYPE_GPIO_ALARM_TO_PC = 0x2f;

        //communication between PC server/PC client
        //communication type equal or larger than this value means this is a server/client PC communication, need to be processed in serverPCFunc.cs
        const int COMMUNICATION_TYPE_CLIENT_PC = 0xB0;
        const int COMMUNICATION_TYPE_CLIENT_PC_HANDSHAKE = 0xB0;
        const int COMMUNICATION_TYPE_CLIENT_DISCONNECTED = 0xB2;
        const int COMMUNICATION_TYPE_CLIENT_HEART_BEAT = 0xB3;
        const int COMMUNICATION_TYPE_NEW_ALARM_TO_CLIENT = 0xB4;
        const int COMMUNICATION_TYPE_ALARM_UPDATED_TO_SERVER = 0xB5;
        const int COMMUNICATION_TYPE_ALARM_UPDATED_TO_CLIENT = 0xB6;
        const int COMMUNICATION_TYPE_BARCODE_TO_CLIENT = 0xB7;
        const int COMMUNICATION_TYPE_BARCODE_TO_SERVER = 0xB8;

        //communication between PC host and label printing SW
        //guoye???
        const int COMMUNICATION_TYPE_HANDSHAKE_PRINT_MACHINE_ID = 0xC0;  //set machine ID for label printing function
        //guoye: 鏍规嵁鏂扮殑鎵撳嵃杞欢閫氳鍗忚
        const int COMMUNICATION_TYPE_WAREHOUE_OUT_START = 0xB5;  //printing SW started and ask for material info, server send material info for all feeding machine to printing SW 
        const int COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE = 0xB6;  //printing machine send barcode info to server whever a stack of material is moved out of the warehouse
        const int COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE = 0xB7;  //printing machine send barcode info to server whever a stack of material is moved into the warehouse
        const int COMMUNICATION_TYPE_CAST_PROCESS_START = 0xB8;  //printing SW started cast process, server need to send dispatch info to printing SW
        const int COMMUNICATION_TYPE_CAST_BARCODE_UPLOAD = 0xB9;  //printing SW send large roll info to server
        const int COMMUNICATION_TYPE_CASE_SHIFT = 0xBA;
        const int COMMUNICATION_TYPE_PRINT_PROCESS_START = 0xBB;
        const int COMMUNICATION_TYPE_PRINT_BARCODE_UPLOAD = 0xC7;
        const int COMMUNICATION_TYPE_SLIT_PROCESS_START = 0xC8;
        const int COMMUNICATION_TYPE_SLIT_BARCODE_UPLOAD = 0xC9;
        const int COMMUNICATION_TYPE_INSPECTION_PROCESS_START = 0xCA;
        const int COMMUNICATION_TYPE_INSPECTION_BARCODE_UPLOAD = 0xCB;
        const int COMMUNICATION_TYPE_REUSE_PROCESS_START = 0xCC;
        const int COMMUNICATION_TYPE_REUSE_BARCODE_UPLOAD = 0xCD;
        const int COMMUNICATION_TYPE_PACKING_PROCESS_START = 0xCE;
        const int COMMUNICATION_TYPE_PACKING_BARCODE_UPLOAD = 0xCF;
        const int COMMUNICATION_TYPE_PRINTING_HEART_BEAT = 0xD0;
        //end of communication between PC host and label printing SW

        //communication between host server and email forwarder, we only need this function when host server cannot access internet, so we need another email server to send out emails
        const int COMMUNICATION_TYPE_EMAIL_HEART_BEAT = 0xfd;  //to indicate the client is still alive
        const int COMMUNICATION_TYPE_EMAIL_FORWARDER = 0xfe;  //email sending client
        const int BOARD_ID_EMAIL_FORWARDER = 0xffffff;
        //end of communication between host server and email forwarder

        //communication between PC host and App
        const int COMMUNICATION_TYPE_APP_WORKING_BOARD_ID_TO_PC = 0x50;  //which board this app is working on 
        const int COMMUNICATION_TYPE_APP_FILE_NAME_TO_PC = 0x51;
        const int COMMUNICATION_TYPE_APP_FILE_DATA_TO_PC = 0x52;
        const int COMMUNICATION_TYPE_APP_FILE_END_TO_PC = 0x53;
        const int COMMUNICATION_TYPE_APP_FILE_FROM_PC = 0x54;
        const int COMMUNICATION_TYPE_APP_DEVICE_NAME = 0x55;

        //data packet related definition
        const int PROTOCOL_HEAD_POS = 0;
        const int PROTOCOL_LEN_POS = 4;
        const int PROTOCOL_COMMUNICATION_TYPE_POS = 6;
        const int PROTOCOL_TIME_POS = 7;
        const int PROTOCOL_PACKET_INDEX_POS = 19;
        const int PROTOCOL_RESERVED_DATA_POS = 23;
        const int PROTOCOL_DATA_TYPE_POS = 27;
        const int PROTOCOL_OLD_DATA_POS = 7;
        const int PROTOCOL_DATA_POS = 28;

        //'w', 'I', 'F', 'i', 74, 1, 0x0, 0x65, 
        const int PROTOCOL_OLD_HEADER_LEN = 7;   //before data
        const int PROTOCOL_HEADER_LEN = 28;   //before data
        const int PROTOCOL_BOARDID_LEN = 4;
        const int PROTOCOL_FILENAME_LEN = 2;
        const int PROTOCOL_DATETIME_LEN = 12;
        const int PROTOCOL_DATAINDEX_LEN = 4;
        const int PROTOCOL_CRC_LEN = 4;
        const int PROTOCOL_OLD_CRC_LEN = 2;

        //a data packet without read data, so it contains only data packet header and CRC
        const int MIN_PACKET_LEN_MINUS_ONE = 32;   //header(4) + len(2) + communicationtype(1) + time(12) + index(4) + reserved(4) + type(1) + CRC(4)
        const int MIN_PACKET_LEN_PURE_FRAME = 32;   //header(4) + len(2) + communicationtype(1) + time(12) + index(4) + reserved(4) + type(1) + CRC(4)
        //a data packet with minimum length of real data (real data length is 1)
        const int MIN_PACKET_LEN = 33;   //header(4) + len(2) + communicationtype(1) + time(12) + index(4) + reserved(4) + type(1) + data(1) + CRC(4)

        const int MIN_PACKET_LEN_FOR_APP = 9;   //header(4) + len(2) + communicationtype(1) + CRC(2)
        const int MAX_PACKET_LEN = 9000;  //max data length for a data packet
        //this debug string is used to compare the string received by cabel/Wifi and the string from uart, to confirm network communication has no problem(we suppose uart should be 100% OK)  
        const int PROTOCOL_DEBUG_STR_LEN = 12; //time format string, like 20171014183413

        const int MAX_RECEIVE_DATA_LEN = 2000000;  //if host server is busy, we may receive many packet in a milli-second, we will put all these data packet in our buffer
        const int BOARD_ID_NOT_DEFINED = 0xff;
        const int LEN_OF_PORTPARAMETER = 10;  //port parameter has 8 parameters, length is 10
        const int LEN_OF_DATE_AND_TIME = 12;  //FORMAT: 1603081123484  16/3/8 11:23:48 //Thursday

        const int TOTAL_DEVICE_NUM_IN_FORM2 = 4;

        const int RESPONSE_LEN = MIN_PACKET_LEN;

        const int MAX_NUM_BOARD = 200;  //max number of boards can be connected to a database server
        const int MAX_PORT_NUM = 40;  //max number of ports for a data collect board

        //for following definitions are used for switch case, to indicating which item in structure we are working on 
        const int ERROR_LIST_errorCode = 0;
        const int ERROR_LIST_errorCodeDesc = 1;

        const int MAX_NUM_OF_VOLCUR = 4;
        const int MAX_NUM_OF_BEAT = 1;
        //        const int MAX_NUM_OF_RF = 1;
        const int MAX_NUM_OF_GPIO = 16;
        const int MAX_NUM_OF_CRAFT_PARAM = 8;
        const int MAX_NUM_OF_QUALITY_DATA = 8;

        const int ADC_RANGE_LEN = 4;
        const int BEAT_VALUE_LEN = 6;

        //upper range should be larger than 0, otherwise, we regard it as not defined
        const float ADC_RANG_NOT_DEFINED = 0;

        const int ALARM_REPORTED = 0;
        const int ALARM_SIGNED = 1;
        const int ALARM_PROCESSED = 2;
        const int ALARM_CANCELLED = 3;

        const int VOLTAGE_VALUE = 220;

        static string[] alarmStatusList = { "303010", "303020", "303030", "303040" }; //报警， 签到， 处理， 取消，defined by MES system
        const string NO_DATA_AVAILABLE = "-99999";
        static int toolUsedTimesNow;  //on handshake, we set this value to 0, every time we complete a dispatch, we add this value to product output number 

        static float[] rangLowADC = { 0, 0f, 0f, 4.5f, 3.3f, 110f, 120f, 345f };
        static float[] rangHighADC = { 60f, 60f, 8.5f, 8.5f, 4.2f, 140f, 150f, 370f };

        [Serializable]
        public partial class ClientThread
        {
            int myBoardID;   //board ID for this connection, start from 1
            int myBoardIndex;  //board index for this connection, start from 0

            //every data packet sent to client(board/emulator/app) will have an index, for some of the data packets like dispatch/alarm/material/craft/quality/status related packets, we need to get 
            //a response from the client to make sure these important instructions are received by the counter part successfully, we use the index in the response packet to make the confirmation 
            int dataPacketIndex;

            //when work in emulation mode, we have no MES data, so we need to create some dispatch, this index can be 1 to 99, so we can emulate the test case of different dispatch  
            //0 may lead to dispatch name of XXXX_0000, 1  may lead to dispatch name of XXXX_0001 ...
            int dispatchAppearIndex;  //-- only for emulator

            //description of every kind of port, this is mainly for on site instant curve display
            int[] craftDataForCurveIndex = new int[MAX_NUM_OF_CRAFT_PARAM];  //craftDataForCurveIndex[2] = 4 means ADC channel 2 is used by this machine in curve 4, craftDataForCurveIndex[2] = -1 means this ADC channel is not used
            int[] volcurDataForCurveIndex = new int[MAX_NUM_OF_VOLCUR]; //
            int[] qualityDataForCurveIndex = new int[MAX_NUM_OF_QUALITY_DATA]; //qualityDataForCurveIndex[2] = 4 means quality data 2 is used as curve 4
            int[] beatDataForCurveIndex = new int[MAX_NUM_OF_BEAT]; //

            int[] workingGPIOArray = new int[MAX_NUM_OF_GPIO];  //workingGPIOArray[2] = 1 means GPIO index 2 is used by this machine, workingGPIOArray[2] = -1 means this GPIO is not used

            string volcurDataTableName;
            string craftDataTableName;
            string qualityDataTableName;
            string beatDataTableName;

            int workingTimePoints;
            int powerConsumed;
            int currentPower;
            //float overflowCurrentValue;
            int dispatchStartTimeStamp;
            int beatTimeStamp;

            //how many data has been recorded in craft/current table
            int numOfCraftDataInTable;
            int numOfCurrentDataInTable;

            public string databaseNameThis;
            public int scannerDataFlag;  //this is a barcode data from scanner, need to get saleOder/process/material/outputNum from this data

            public int deviceFailureIndexAlarm;

            public int numOfDeviceAlarm;
            public gVariable.alarmTableStruct alarmTableStructImpl;
            public gVariable.alarmTableStruct[] deviceAlarmListTable = new gVariable.alarmTableStruct[gVariable.maxDeviceAlarmNum];  //设备安灯列表，向 MES 询问当前设备安灯情况
            public byte[] deviceStatusAlarm = new byte[gVariable.maxDeviceAlarmNum];

            //public gVariable.materialAlarmTableStruct materialAlarmTable;  //物料安灯

            private Socket clientSocketInServer;
            private byte[] receiveBytes = new byte[MAX_RECEIVE_DATA_LEN];  //received data, maybe part of packet
            private byte[] checkBytes = new byte[MAX_RECEIVE_DATA_LEN];   //packet need to be processed, start from first byte of a packet
            private byte[] onePacket = new byte[MAX_PACKET_LEN];

            private int recCount;
            private int checkLeftCount;
            //private int fileIndex;

            private int handshakeWithClientOK = 0;
            private int appHandshakeCompleted = 0;

            object settingFileLocker = new object();

            //int emptyPacketIndex;

            //the user from PC decided to write one of the setting data to board
            public void writeSettingsToBoard(int settingIndex)
            {
                int i, v;
                int index, pos;
                int len;
                float f;
                byte[] onePacket = new byte[200];
                byte[] buf;
                string str;

                if (myBoardIndex < 0)
                    return;

                onePacket[0] = (byte)'w';
                onePacket[1] = (byte)'I';
                onePacket[2] = (byte)'F';
                onePacket[3] = (byte)'i';

                try
                {
                    switch (settingIndex)
                    {
                        case gVariable.ADC_SETTING_DATA_TO_BOARD:
                            //format:
                            //0: 5 / 10 v
                            //1 - 8: channel enabled or not
                            // 9-12, 17-20, 25-28, 33-36, 41-44, 49-52, 57-60, 65-68: range upper
                            //13-16, 21-24, 29-32, 37-40, 45-48, 53-56, 61-64, 69-72: range lower
                            //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_ADC_SETTING_TO_BOARD;

                            pos = PROTOCOL_DATA_POS;

                            onePacket[pos++] = (byte)(gVariable.craftList[myBoardIndex].workingVoltage);

                            for (index = 0; index < gVariable.maxCraftParamNum; index++)
                            {
                                if (index < gVariable.craftList[myBoardIndex].paramNumber)
                                    onePacket[pos++] = 1;
                                else
                                    onePacket[pos++] = 0;
                            }

                            for (index = 0; index < gVariable.maxCraftParamNum; index++)
                            {
                                str = gVariable.craftList[myBoardIndex].rangeUpperLimit[index].ToString("f2");

                                buf = System.Text.Encoding.Default.GetBytes(str);
                                len = buf.Length;
                                if (len > ADC_RANGE_LEN)
                                    len = ADC_RANGE_LEN;
                                for (i = 0; i < (ADC_RANGE_LEN - len); i++)
                                    onePacket[pos++] = (byte)'0';

                                for (i = 0; i < len; i++)
                                    onePacket[pos++] = buf[i];

                                str = gVariable.craftList[myBoardIndex].rangeLowerLimit[index].ToString("f2");

                                buf = System.Text.Encoding.Default.GetBytes(str);
                                len = buf.Length;
                                if (len > ADC_RANGE_LEN)
                                    len = ADC_RANGE_LEN;
                                for (i = 0; i < (ADC_RANGE_LEN - len); i++)
                                    onePacket[pos++] = (byte)'0';

                                for (i = 0; i < len; i++)
                                    onePacket[pos++] = buf[i];
                            }

                            len = pos + PROTOCOL_CRC_LEN;
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_ADC_SETTING_TO_BOARD);
                            break;
                        case gVariable.UART_SETTING_DATA_TO_BOARD:  //server want to write something to board uart
                            //format:
                            //0-2: for baudrate, 
                            //3: for uart index
                            //4 - ... for output string
                            //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_UART_SETTING_TO_BOARD;
                            pos = PROTOCOL_DATA_POS;

                            onePacket[pos++] = (byte)gVariable.uartSettingInfo[myBoardIndex].uartBaudrate[0];
                            onePacket[pos++] = (byte)gVariable.uartSettingInfo[myBoardIndex].uartBaudrate[1];
                            onePacket[pos++] = (byte)gVariable.uartSettingInfo[myBoardIndex].uartBaudrate[2];

                            index = gVariable.uartSettingInfo[myBoardIndex].selectedUart;
                            onePacket[pos++] = (byte)index;
                            if (index < gVariable.MAX_NUM_UART)
                            {
                                str = gVariable.uartSettingInfo[myBoardIndex].uartOutputData[index];
                                buf = System.Text.Encoding.Default.GetBytes(str);
                                len = buf.Length;
                                for (i = 0; i < len; i++)
                                    onePacket[pos++] = buf[i];
                            }

                            len = pos + PROTOCOL_CRC_LEN;
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_UART_SETTING_TO_BOARD);
                            break;
                        case gVariable.GPIO_SETTING_DATA_TO_BOARD:
                            //format:
                            //0-7: high trigger or low trigger
                            //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_GPIO_SETTING_TO_BOARD;
                            pos = PROTOCOL_DATA_POS;

                            for (i = 0; i < gVariable.numOfGPIOs; i++)
                                onePacket[pos++] = (byte)(gVariable.GPIOSettingInfo[myBoardIndex].GPIOTriggerVoltage[i]);

                            len = pos + PROTOCOL_CRC_LEN;
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_GPIO_SETTING_TO_BOARD);
                            break;
                        case gVariable.BEAT_SETTING_DATA_TO_BOARD:
                            //format:
                            //0-5: default working current 
                            //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_BEAT_SETTING_TO_BOARD;
                            pos = PROTOCOL_DATA_POS;

                            onePacket[pos++] = 0;  //internal volcur device, not external

                            //current low level 
                            f = gVariable.beatPeriodInfo[myBoardIndex].idleCurrentLow;
                            str = f.ToString("f3");
                            buf = System.Text.Encoding.Default.GetBytes(str);
                            len = buf.Length;
                            if (len > BEAT_VALUE_LEN)
                                len = BEAT_VALUE_LEN;
                            for (i = 0; i < (BEAT_VALUE_LEN - len); i++)
                                onePacket[pos++] = (byte)'0';
                            for (i = 0; i < len; i++)
                                onePacket[pos++] = buf[i];

                            //current low thresheold
                            f = gVariable.beatPeriodInfo[myBoardIndex].idleCurrentHigh;
                            str = f.ToString("f3");
                            buf = System.Text.Encoding.Default.GetBytes(str);
                            len = buf.Length;
                            if (len > BEAT_VALUE_LEN)
                                len = BEAT_VALUE_LEN;
                            for (i = 0; i < (BEAT_VALUE_LEN - len); i++)
                                onePacket[pos++] = (byte)'0';
                            for (i = 0; i < len; i++)
                                onePacket[pos++] = buf[i];

                            //current high level
                            f = gVariable.beatPeriodInfo[myBoardIndex].workCurrentLow;
                            str = f.ToString("f3");
                            buf = System.Text.Encoding.Default.GetBytes(str);
                            len = buf.Length;
                            if (len > BEAT_VALUE_LEN)
                                len = BEAT_VALUE_LEN;
                            for (i = 0; i < (BEAT_VALUE_LEN - len); i++)
                                onePacket[pos++] = (byte)'0';
                            for (i = 0; i < len; i++)
                                onePacket[pos++] = buf[i];

                            //current high threshold
                            f = gVariable.beatPeriodInfo[myBoardIndex].workCurrentHigh;
                            str = f.ToString("f3");
                            buf = System.Text.Encoding.Default.GetBytes(str);
                            len = buf.Length;
                            if (len > BEAT_VALUE_LEN)
                                len = BEAT_VALUE_LEN;
                            for (i = 0; i < (BEAT_VALUE_LEN - len); i++)
                                onePacket[pos++] = (byte)'0';
                            for (i = 0; i < len; i++)
                                onePacket[pos++] = buf[i];

                            //interval between 2 spikes
                            v = gVariable.beatPeriodInfo[myBoardIndex].gapValue;
                            str = v.ToString();
                            buf = System.Text.Encoding.Default.GetBytes(str);
                            len = buf.Length;
                            for (i = 0; i < (4 - len); i++)
                                onePacket[pos++] = (byte)'0';
                            for (i = 0; i < len; i++)
                                onePacket[pos++] = buf[i];

                            //how many continuous high level could  
                            v = gVariable.beatPeriodInfo[myBoardIndex].peakValue;
                            str = v.ToString();
                            buf = System.Text.Encoding.Default.GetBytes(str);
                            len = buf.Length;
                            for (i = 0; i < (4 - len); i++)
                                onePacket[pos++] = (byte)'0';
                            for (i = 0; i < len; i++)
                                onePacket[pos++] = buf[i];

                            for (i = 0; i < BEAT_VALUE_LEN * 4; i++)
                                onePacket[pos++] = (byte)'0';

                            len = pos + PROTOCOL_CRC_LEN;
                            sendDataToClient(onePacket, len, COMMUNICATION_TYPE_BEAT_SETTING_TO_BOARD);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(DateTime.Now.ToString() + ":board" + (myBoardIndex + 1) + "setting data to board failed! " + ex);
                }

                gVariable.whatSettingDataModified = gVariable.NO_SETTING_DATA_TO_BOARD;
            }


            //sData: string for received data made of filename(8 bytes) + datetime(15 bytes) + ' ' + index(5 bytes) + ' ' + data
            //dataArray: an array conteains date index and data without filename, we use this array to get time, index and ADC data from different channels
            public void FileAppendFucntion(byte[] checkBytes)
            {
                int i, v;
                int alarmIDInTable;
                int pos;
                int num;
                int packetLen;
                int len;
                float f;
                float[] fValue = new float[MAX_NUM_OF_QUALITY_DATA];
                string content, str;
                int yy, mm, dd, h, m, s;
                int timeStamp;
                System.DateTime time;
                int dataTypeValue; //indicating what kind of data this is, ADC, VolCur, GPIO, RFID, or didspatch instruction, etc
                byte[] dataArray = new byte[MAX_PACKET_LEN];
                int startIndex;
                string cmdText;
                MySqlParameter[] param;

                try
                {
                    yy = 0;
                    mm = 0;
                    dd = 0;
                    h = 0;
                    m = 0;
                    s = 0;
                    dataTypeValue = checkBytes[PROTOCOL_DATA_TYPE_POS];

                    packetLen = checkBytes[PROTOCOL_LEN_POS] + checkBytes[PROTOCOL_LEN_POS + 1] * 0x100;

                    //for beat data, it is a special kind of data, not following my rule
                    if (checkBytes[PROTOCOL_COMMUNICATION_TYPE_POS] == COMMUNICATION_TYPE_CYCLE_TIME_TO_PC)
                    {
                        len = packetLen - MIN_PACKET_LEN;
                        Array.Copy(checkBytes, PROTOCOL_DATA_POS, onePacket, 0, len);
                    }
                    else if (clientFromTouchpad == 0 && (checkBytes[PROTOCOL_COMMUNICATION_TYPE_POS] == COMMUNICATION_TYPE_QUALITY_DATA_TO_PC || checkBytes[PROTOCOL_COMMUNICATION_TYPE_POS] == COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_PC))
                    {
                        len = packetLen - MIN_PACKET_LEN;
                        Array.Copy(checkBytes, PROTOCOL_DATA_POS, onePacket, 0, len);
                    }
                    else
                    {
                        len = packetLen - MIN_PACKET_LEN;  //add by one means we need dataType value that is one byte ahead of data in data packet
                        Array.Copy(checkBytes, PROTOCOL_DATA_POS, onePacket, 0, len);
                    }

                    content = System.Text.Encoding.ASCII.GetString(onePacket, 0, len);

                    Array.Copy(checkBytes, PROTOCOL_TIME_POS, dataArray, 0, packetLen - PROTOCOL_TIME_POS - PROTOCOL_CRC_LEN);  //index and read data
                    if (gVariable.debugMode > 1)
                    {
                        string sData;

                        sData = System.Text.Encoding.ASCII.GetString(dataArray, 0, PROTOCOL_DEBUG_STR_LEN);
                        writeDataTolog(sData);
                    }

                    try
                    {
                        //for quality data, it is not sent by board directly, it is from touchpad, with no time stamp
                        if (checkBytes[PROTOCOL_COMMUNICATION_TYPE_POS] == COMMUNICATION_TYPE_QUALITY_DATA_TO_PC)
                        {
                            time = DateTime.Now;
                            dataTypeValue = DATA_TYPE_QUALITY_DATA;
                        }
                        else if (checkBytes[PROTOCOL_COMMUNICATION_TYPE_POS] == COMMUNICATION_TYPE_CYCLE_TIME_TO_PC)
                        {
                            //for cycle time, the tiem value from board sometimes not correct, so we use packet receive time as trigger point to calculate cycle time
                            time = DateTime.Now;
                            dataTypeValue = DATA_TYPE_BEAT_PERIOD;
                        }
                        else
                        {
                            yy = 2000 + (dataArray[0] - '0') * 10 + dataArray[1] - '0';
                            mm = (dataArray[2] - '0') * 10 + dataArray[3] - '0';
                            dd = (dataArray[4] - '0') * 10 + dataArray[5] - '0';
                            h = (dataArray[6] - '0') * 10 + dataArray[7] - '0';
                            m = (dataArray[8] - '0') * 10 + dataArray[9] - '0';
                            s = (dataArray[10] - '0') * 10 + dataArray[11] - '0';

                            time = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(yy, mm, dd, h, m, s));
                        }
                        timeStamp = (int)(time - gVariable.worldStartTime).TotalSeconds;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("date and time format error" + ex);
                        return;
                    }

                    switch (dataTypeValue)
                    {
                        case DATA_TYPE_ADC_DEVICE:
                            gVariable.currentDataIndex[DATA_TYPE_ADC_DEVICE]++;

                            pos = PROTOCOL_DATA_POS - PROTOCOL_TIME_POS;  //21, start position of adc data in buffer

                            num = gVariable.craftList[myBoardIndex].paramNumber;

                            //get start quality index in 0_qualityList table with current dispatch code
                            startIndex = mySQLClass.getNextRecordByOneStrColumn(databaseNameThis, gVariable.craftListTableName, "dispatchCode", gVariable.dispatchSheet[myBoardIndex].dispatchCode, 0, null);

                            for (i = 0; i < MAX_NUM_OF_CRAFT_PARAM && craftDataForCurveIndex[i] >= 0; i++)
                            {
                                //if channel i is not -1, that means this channel is connected to ADC sensor, so we record its data
                                //                                if (craftDataForCurveIndex[i] != -1)
                                {
                                    v = dataArray[pos + i * 2] * 0x100 + dataArray[pos + i * 2 + 1];

                                    if (v >= 0x8000)
                                        f = (float)(0xffff - v) / 0x7fff * (-1);
                                    else
                                        f = (float)(v) / 0x7fff;

                                    //when range is defined and uppder/lower range not the same, we believe this range is legal
                                    if (gVariable.craftList[myBoardIndex].rangeUpperLimit[i] != gVariable.craftList[myBoardIndex].rangeLowerLimit[i])
                                    {
                                        f = (gVariable.craftList[myBoardIndex].rangeUpperLimit[i] - gVariable.craftList[myBoardIndex].rangeLowerLimit[i]) * f + gVariable.craftList[myBoardIndex].rangeLowerLimit[i];
                                    }

                                    //only when this current board is selected in room function, will these data be displayed on screen
                                    if (databaseNameThis == gVariable.currentCurveDatabaseName)   //only when the current board is selected in room function, will these data be displayed on screen
                                    {
                                        if (craftDataForCurveIndex[i] < gVariable.maxCurveNum)  //too many ADC data, we don't display it on screen
                                        {
                                            //craftDataForCurveIndex[i] means curve index for this ADC channel
                                            gVariable.curveTextArray[craftDataForCurveIndex[i]] = f.ToString("f4");
                                        }
                                    }
                                    fValue[i] = f;

                                    if (i < num)
                                    {
                                        param = new MySqlParameter[] { new MySqlParameter("@paramValue", f) };
                                        cmdText = "update 0_craftlist set paramValue = @paramValue where id = '" + (startIndex + i) + "'";

                                        //update 0_qualityList by dispatchCode, if a dispatch has 4 quality data, we need to update controlLimuts and centerValue for all these 4 quality items
                                        mySQLClass.databaseNonQueryAction(databaseNameThis, cmdText, param, gVariable.notAppendRecord);
                                    }

                                    if (gVariable.checkDataCorrectness == 1)
                                    {
                                        if (gVariable.craftList[myBoardIndex].paramUpperLimit[i] != gVariable.craftList[myBoardIndex].paramLowerLimit[i] && (f < gVariable.craftList[myBoardIndex].paramLowerLimit[i] || f > gVariable.craftList[myBoardIndex].paramUpperLimit[i]))
                                        {
                                            //if we already had one alarm of the same kind for this machine, a new one should not bo triggered, until the current one be dismissed
                                            //or if there are only a few data inside this table, less than minDataNumToTriggerAlarm, we donot trigger this alarm
                                            if (gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_CRAFT_DATA, myBoardIndex] != 0 || numOfCraftDataInTable < gVariable.minDataNumToTriggerAlarm)
                                            {
                                                ;
                                            }
                                            else
                                            {
                                                //set flag to 1, so same kind of alarm won't be triggered if the old one is not dismissed
                                                gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_CRAFT_DATA, myBoardIndex] = 1;

                                                alarmTableStructImpl.errorDesc = "参数" + gVariable.craftList[myBoardIndex].paramName[i] + "超出规格限";
                                                alarmTableStructImpl.alarmFailureCode = DateTime.Now.ToString("yyMMddHHmmss") + "_" + (gVariable.andonAlarmIndex + 1);
                                                alarmTableStructImpl.machineCode = gVariable.machineCodeArray[myBoardIndex];
                                                alarmTableStructImpl.machineName = gVariable.machineNameArray[myBoardIndex];
                                                alarmTableStructImpl.dispatchCode = gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                                                alarmTableStructImpl.operatorName = gVariable.SPCMonitoringSystem;
                                                alarmTableStructImpl.time = DateTime.Now.ToString();
                                                alarmTableStructImpl.type = gVariable.ALARM_TYPE_CRAFT_DATA;
                                                alarmTableStructImpl.category = gVariable.ALARM_CATEGORY_CRAFT_DATA_OVERFLOW;
                                                alarmTableStructImpl.status = gVariable.ALARM_STATUS_APPLIED;
                                                //numOfCraftDataInTable + 1 means we've already have numOfCraftDataInTable number of data in database, now we got another piece of data
                                                alarmTableStructImpl.startID = numOfCraftDataInTable + 1;
                                                alarmTableStructImpl.indexInTable = i;
                                                alarmTableStructImpl.workshop = gVariable.allMachineWorkshopForZihua[0];
                                                alarmTableStructImpl.mailList = toolClass.getAlarmMailList();

                                                alarmIDInTable = mySQLClass.writeAlarmTable(databaseNameThis, gVariable.alarmListTableName, alarmTableStructImpl);
                                                toolClass.processNewAlarm(databaseNameThis, alarmIDInTable);
                                                gVariable.andonAlarmIndex++;
                                            }
                                        }
                                    }
                                }
                            }

                            if (i != 0)  //craft data not null
                                numOfCraftDataInTable = mySQLClass.writeMultipleFloatToTable(databaseNameThis, craftDataTableName, mySQLClass.DATA_TYPE_CRAFT_DATA, timeStamp, fValue, i, "");

                            break;

                        case DATA_TYPE_VOL_CUR_DEVICE:
                            //for Voltage/current data, we have four data for one packet, each data is 9 byte including decimal point
                            gVariable.currentDataIndex[DATA_TYPE_VOL_CUR_DEVICE]++;

                            //voltage is needed as defined in database table of product
                            f = (float)Convert.ToDouble(content.Remove(9));
                            if (databaseNameThis == gVariable.currentCurveDatabaseName)
                                gVariable.curveTextArray[volcurDataForCurveIndex[0]] = f.ToString("f4");
                            fValue[0] = f;

                            f = (float)Convert.ToDouble(content.Remove(0, 9).Remove(9));
                            if (databaseNameThis == gVariable.currentCurveDatabaseName)
                                gVariable.curveTextArray[volcurDataForCurveIndex[1]] = f.ToString("f4");
                            fValue[1] = f;
                            gVariable.currentValueNow[myBoardIndex] = f;

                            if (f > gVariable.beatPeriodInfo[myBoardIndex].idleCurrentHigh)
                                workingTimePoints++; //current value larger than threshold

                            powerConsumed += (int)(VOLTAGE_VALUE * f / 2);

                            currentPower = (int)(VOLTAGE_VALUE * f);

                            //power is needed as defined in database table of product
                            f = (float)Convert.ToDouble(content.Remove(0, 18).Remove(9));
                            if (databaseNameThis == gVariable.currentCurveDatabaseName)
                                gVariable.curveTextArray[volcurDataForCurveIndex[2]] = f.ToString("f4");
                            fValue[2] = f;

                            f = (float)Convert.ToDouble(content.Remove(0, 27));
                            if (databaseNameThis == gVariable.currentCurveDatabaseName)
                                gVariable.curveTextArray[volcurDataForCurveIndex[3]] = f.ToString("f4");
                            fValue[3] = f;

                            mySQLClass.writeMultipleFloatToTable(databaseNameThis, volcurDataTableName, mySQLClass.DATA_TYPE_VOLCUR_DATA, timeStamp, fValue, 4, "");
                            f = fValue[1];
                            powerConsumed += (int)(VOLTAGE_VALUE * f / 2);
                            currentPower = (int)(VOLTAGE_VALUE * f);

                            if (gVariable.machineCurrentStatus[myBoardIndex] == gVariable.MACHINE_STATUS_DISPATCH_STARTED)  //we can use quality/craft/andon data now
                            {
                                //Console.WriteLine(DateTime.Now.ToString() + "beat value: " + f + "; current data index: " + gVariable.currentDataIndex[DATA_TYPE_VOL_CUR_DEVICE]);

                                //get beat time value from start to end of a product, 0 means beat not finished yet
                                v = beatCalculationImpl.beatDataInput(myBoardIndex, f, gVariable.currentDataIndex[DATA_TYPE_VOL_CUR_DEVICE]);
                                if (v > 0)
                                {
                                    gVariable.machineStatus[myBoardIndex].productBeat = v;
                                    if (databaseNameThis == gVariable.currentCurveDatabaseName)
                                        gVariable.curveTextArray[beatDataForCurveIndex[0]] = v.ToString();

                                    fValue[0] = v;
                                    numOfCurrentDataInTable = mySQLClass.writeMultipleFloatToTable(databaseNameThis, beatDataTableName, mySQLClass.DATA_TYPE_BEAT_DATA, timeStamp, fValue, 1, null);
                                }
                                else if (v == 0)  //we are in working mode
                                {
                                    workingTimePoints++; //current value larger than threshold, so this is working time
                                }
                                else  //machine is now in idle mode
                                {

                                }
                            }

                            if (gVariable.maxMachineCurrentValue[myBoardIndex] != 0 && f > gVariable.maxMachineCurrentValue[myBoardIndex])
                            {
                                /*
                                if (gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DEVICE, myBoardIndex] != 0 || numOfCurrentDataInTable < gVariable.minDataNumToTriggerAlarm)
                                {
                                    ;
                                }
                                else
                                {
                                    //set flag to 1, so same kind of alarm won't be triggered if the old one is not dismissed
                                    gVariable.typeAlarmAlreadyAlive[gVariable.ALARM_TYPE_DEVICE, myBoardIndex] = 1;

                                    alarmTableStructImpl.errorDesc = "参数" + gVariable.craftList[myBoardIndex].paramName[i] + "超出规格限";
                                    alarmTableStructImpl.alarmFailureCode = DateTime.Now.ToString("yyMMddHHmmss") + "_" + (gVariable.andonAlarmIndex + 1);
                                    alarmTableStructImpl.machineCode = gVariable.machineCodeArray[myBoardIndex];
                                    alarmTableStructImpl.machineName = gVariable.machineNameArray[myBoardIndex];
                                    alarmTableStructImpl.dispatchCode = gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                                    alarmTableStructImpl.operatorName = gVariable.SPCMonitoringSystem;
                                    alarmTableStructImpl.timeOfWarning = DateTime.Now.ToString();
                                    alarmTableStructImpl.type = gVariable.ALARM_TYPE_CURRENT_VALUE;
                                    alarmTableStructImpl.category = gVariable.ALARM_CATEGORY_CRAFT_DATA_OVERFLOW;
                                    alarmTableStructImpl.status = gVariable.ALARM_STATUS_APPLIED;
                                    //numOfCurrentDataInTable + 1 means we've already have numOfCraftDataInTable number of data in database, now we got another piece of data
                                    alarmTableStructImpl.startID = numOfCurrentDataInTable + 1;
                                    alarmTableStructImpl.indexInTable = 1;   //this version only support one current value for a machine, need to be modified
                                    alarmTableStructImpl.workshop = gVariable.allMachineWorkshopForZihua[0];
                                    alarmTableStructImpl.mailList = toolClass.getAlarmMailList();

                                    alarmIDInTable = mySQLClass.writeAlarmTable(databaseNameThis, gVariable.alarmListTableName, alarmTableStructImpl); 
                                    toolClass.processNewAlarm(databaseNameThis, alarmIDInTable);
                                    gVariable.andonAlarmIndex++;
                                }

                                overflowCurrentValue = f;
                                 */
                            }
                            break;

                        case DATA_TYPE_RF_DEVICE:  //this case is not support now, because RF data will be sent by quality data, there is no separate RF data any more
                            /*
                            gVariable.currentDataIndex[DATA_TYPE_RF_DEVICE]++;
                            str = content.Remove(9);
                            f = (float)Convert.ToDouble(str);
                            if (databaseNameThis == gVariable.currentCurveDatabaseName)   //only when the current board is selected in room function, will these data be displayed on screen
                            {
                               gVariable.curveTextArray[RFDataForCurveIndex[2]] = f.ToString("f4");
                               gVariable.textRF = f.ToString("f4");
                            }
                            mySQLClass.writeMultipleFloatToTable(databaseNameThis, dispatchCodeStrRF, myBoardIndex + 1, timeStamp, f, 4, null);
                            */
                            break;

                        case DATA_TYPE_GPIO_DEVICE:
                            //for GPIODATA, one packet contains 16 bit, stands for 16 GPIO interrupt pins
                            //index 16 means GPIO
                            gVariable.currentDataIndex[DATA_TYPE_GPIO_DEVICE]++;
                            i = myBoardIndex + 1;
                            //Console.Write("Board " + i + " received an interrupt.");

                            /*
                            tableName = gVariable.gpioTableName;
                            interruptV = (int)(dataArray[22] * 0x100 + dataArray[23]);

                            i = 0;
                            alarmBitIndex = -1;  //to indicate whether a real alarm comes
                            v = interruptV ^ gVariable.gpioStatus;
                            if (v != 0)  //means gpio interrupt status has changed
                            {
                                for (i = 0; i < gVariable.numOfGPIOs; i++)
                                {
                                    //if this gpio pin status has changed and this gpio has been assigned to a product, we record this change in database in the product table
                                    if ((v & (1 << i)) != 0 && tableName != null)
                                    {
                                        mySQLClass.writeGPIOStatusToTable(databaseNameThis, tableName, gVariable.dispatchSheet[myBoardIndex].dispatchCode, timeStamp, i, interruptV);

                                        alarmBitIndex = i;
                                        Console.Write("an interrupt comes");
                                    }
                                }
                                gVariable.gpioStatus = interruptV;
                            }

                            //default GPIO value is high, high to low means something special happened, we need to set an alarm
                            if (alarmBitIndex != -1 && ((gVariable.gpioStatus & (1 << alarmBitIndex)) == 0))
                            {
                                //we will save alarm info in setAlarm()
                                processNewAlarm(alarmBitIndex, yy, mm, dd, h, m, s);

                                //We also need to record this status in GPIO table, because the GPIO status in different product table formed the complete GPIO status, a LAN client want t display
                                //current GPIO status, it does not know which product it should check because it does not know which procut had a gpio interrupt recently, so we added this GPIO status table.
                                //                                mySQLClass.writeGPIOStatusToTable(databaseNameThis, tableName, myBoardIndex + 1, timeStamp, interruptV);

                            }
                            else
                            {
                                //low to high, alarm pin go back to normal, don't need any special attention 
                            }
                             */
                            break;

                        case DATA_TYPE_QUALITY_DATA:

                            gVariable.currentDataIndex[DATA_TYPE_QUALITY_DATA]++;
                            num = gVariable.qualityList[myBoardIndex].checkItemNumber;

                            //get start quality index in 0_qualityList table with current dispatch code
                            startIndex = mySQLClass.getNextRecordByOneStrColumn(databaseNameThis, gVariable.qualityListTableName, "dispatchCode", gVariable.dispatchSheet[myBoardIndex].dispatchCode, 0, null);

                            f = 0;
                            for (i = 0; i < MAX_NUM_OF_QUALITY_DATA; i++)
                            {
                                str = gVariable.qualityList[myBoardIndex].checkResultData[i];
                                if (toolClass.isNumericOrNot(str) == 1)
                                    f = (float)Convert.ToDouble(str);
                                else
                                    f = 0;

                                if (databaseNameThis == gVariable.currentCurveDatabaseName)
                                    gVariable.curveTextArray[qualityDataForCurveIndex[i]] = f.ToString("f4");

                                fValue[i] = f;

                                if (i < num)
                                {
                                    if (f >= gVariable.qualityList[myBoardIndex].specLowerLimit[i] && f <= gVariable.qualityList[myBoardIndex].specUpperLimit[i])
                                        str = "Y";
                                    else
                                        str = "N";

                                    param = new MySqlParameter[] { new MySqlParameter("@checkResultData", f), new MySqlParameter("@checkResult", str) };
                                    cmdText = "update 0_qualitylist set checkResultData = @checkResultData, checkResult = @checkResult where id = '" + (startIndex + i) + "'";

                                    //update 0_qualityList by dispatchCode, if a dispatch has 4 quality data, we need to update controlLimuts and centerValue for all these 4 quality items
                                    mySQLClass.databaseNonQueryAction(databaseNameThis, cmdText, param, gVariable.notAppendRecord);
                                }

                            }
                            //new quality data arrived, so we can start next SPC checking
                            gVariable.newQualityDataArrivedFlag[myBoardIndex] = 1;

                            //Console.WriteLine(f.ToString());

                            if (i != 0)  //quality data not null
                            {
                                mySQLClass.writeMultipleFloatToTable(databaseNameThis, qualityDataTableName, mySQLClass.DATA_TYPE_QUALITY_DATA, timeStamp, fValue, i,
                                                                     gVariable.dispatchSheet[myBoardIndex].serialNumber.Remove(gVariable.LENGTH_DISPATCH_CODE + 1, 2) + qualityDataSN.ToString().PadLeft(2, '0'));
                                //here we use update dispatch function to update qualitfied/unqualified number of product whenever a new product is completed, null means dispatch code is defined by myBoardIndex
                                mySQLClass.updateDispatchTable(databaseNameThis, gVariable.dispatchListTableName, myBoardIndex, gVariable.dispatchSheet[myBoardIndex].status, null);
                            }
                            break;
                        case DATA_TYPE_BEAT_PERIOD:
                            gVariable.currentDataIndex[DATA_TYPE_BEAT_PERIOD]++;

                            gVariable.machineStatus[myBoardIndex].productBeat++;
                            f = gVariable.machineStatus[myBoardIndex].productBeat;

                            if (databaseNameThis == gVariable.currentCurveDatabaseName)
                                gVariable.curveTextArray[beatDataForCurveIndex[0]] = f.ToString("f");
                            fValue[0] = f;

                            mySQLClass.writeMultipleFloatToTable(databaseNameThis, beatDataTableName, mySQLClass.DATA_TYPE_BEAT_DATA, timeStamp, fValue, 1, "");

                            if (gVariable.workshopReport != gVariable.WORKSHOP_REPORT_NONE)
                            {
                                if (content.Remove(1) == "a")
                                    gVariable.dispatchSheet[myBoardIndex].qualifiedNumber++;
                                else
                                    gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber++;

                                gVariable.dispatchSheet[myBoardIndex].outputNumber++;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("database write fail" + ex);
                }
            }

            public ClientThread(Socket ClientSocket)
            {
                this.clientSocketInServer = ClientSocket;
                handshakeWithClientOK = 0;
            }


            private void initialADCValue()
            {
                int i;

                for (i = 0; i < gVariable.craftList[myBoardIndex].paramNumber; i++)
                {
                    //ADC range is not defined in touchpad / setting function, we need to use range definition from PC 
                    if (gVariable.ADCChannelInfo[myBoardIndex].channelEnabled[i] == 1 && gVariable.craftList[myBoardIndex].rangeUpperLimit[i] == ADC_RANG_NOT_DEFINED)
                    {
                        gVariable.craftList[myBoardIndex].rangeLowerLimit[i] = ADC_RANG_NOT_DEFINED;
                        gVariable.craftList[myBoardIndex].rangeUpperLimit[i] = ADC_RANG_NOT_DEFINED;
                    }
                }
            }

            //touchpad need a fixed length string, so we restruct a string to satisfy this target,
            //the problem is .net will count a 2 byte Chinese char as 1 in length, so we need to consider 
            //first change string to array, then calculate the real length of this string, finally change 
            //new array back to string
            public string getFixedCountString(string str, int len)
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

                return str1;
            }


            //return: 0 WebBrowserReadyState to receive data
            //        1 Not ready yet 
            public void notReadyForCommunication()
            {
                int len;

                //the following instruction happens in:
                //If communication is already undergoing, but host PC restarted. Data collect board will not re-do handshake, 
                //because it is already inside the process of communication. At this time, host PC need to stop the 
                //communication and ask board to re-do handshake then restart communication again.
                len = RESPONSE_LEN;
                Console.WriteLine("Board " + (myBoardIndex + 1) + " data got, need a redo of handshake on " + DateTime.Now.ToString());
                //onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = COMMUNICATION_TYPE_REDO_HANDSHAKE_TO_BOARD;
                onePacket[PROTOCOL_DATA_POS] = RESULT_OK;
                sendDataToClient(onePacket, len, COMMUNICATION_TYPE_REDO_HANDSHAKE_TO_BOARD);
            }

            //read dispatch/material/craft/quality data from MES or our internal data generator(in emulation mode) to global variables for future use
            public int readMESString(int tableIndex)
            {
                int i, j;
                int dispatchAppearIndexTmp;
                int num;
                string outStr = " ";
                string dName, tName;
                string today;
                string commandText;
                string errorDesc = null;
                string alarmFailureCode = null;
                string operatorName = null;
                string time = null;
                string time1 = null;
                string time2 = null;
                int status = 0;
                int type = 0;
                string[] strArray;
                string[,] tableArray;

                try
                {
                    switch (tableIndex)
                    {
                        case COMMUNICATION_TYPE_DISPATCH_DATA_TO_TOUCHPAD:
                            outStr = "read Dispatch";
                            if (gVariable.faultData == 0)
                            {
                                ServiceReference2.ReturnActiveDispatch[] dispatchDataMES;

                                ServiceReference2.ActiveDispatchOrderServiceClient getDispatch = new ServiceReference2.ActiveDispatchOrderServiceClient();
                                dispatchDataMES = getDispatch.queryActiveDispatchOrder(gVariable.machineStatus[myBoardIndex].machineCode);

                                if (dispatchDataMES == null)
                                    return RESULT_ERR_NO_DATA_AVAILABLE;

                                if (dispatchDataMES.Length > 1)
                                    return RESULT_ERR_DATA_NUM_WRONG;

                                gVariable.dispatchSheet[myBoardIndex].dispatchCode = dispatchDataMES[0].dispatchNo;
                                gVariable.dispatchSheet[myBoardIndex].planTime1 = dispatchDataMES[0].planStartTime;
                                gVariable.dispatchSheet[myBoardIndex].planTime2 = dispatchDataMES[0].planEndTime;
                                gVariable.dispatchSheet[myBoardIndex].productCode = dispatchDataMES[0].productNo;
                                gVariable.dispatchSheet[myBoardIndex].productName = dispatchDataMES[0].productName;
                                gVariable.dispatchSheet[myBoardIndex].operatorName = dispatchDataMES[0].operationUser;
                                gVariable.dispatchSheet[myBoardIndex].plannedNumber = Convert.ToInt32(dispatchDataMES[0].planNum);
                                gVariable.dispatchSheet[myBoardIndex].qualifiedNumber = Convert.ToInt32(dispatchDataMES[0].qualifiedNum);
                                gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber = Convert.ToInt32(dispatchDataMES[0].disqualifiedNum);
                                gVariable.dispatchSheet[myBoardIndex].processName = dispatchDataMES[0].currentProcess;
                                gVariable.dispatchSheet[myBoardIndex].realStartTime = "";
                                gVariable.dispatchSheet[myBoardIndex].prepareTimePoint = "";
                                gVariable.dispatchSheet[myBoardIndex].realFinishTime = "";
                                if (dispatchDataMES[0].entireLifeCount == "")
                                    gVariable.dispatchSheet[myBoardIndex].toolLifeTimes = 0;
                                else
                                    gVariable.dispatchSheet[myBoardIndex].toolLifeTimes = Convert.ToInt32(dispatchDataMES[0].entireLifeCount);
                                gVariable.machineStatus[myBoardIndex].toolUsedTimes = toolUsedTimesNow;
                                gVariable.dispatchSheet[myBoardIndex].toolUsedTimes = toolUsedTimesNow;
                                gVariable.dispatchSheet[myBoardIndex].outputRatio = Convert.ToInt32(dispatchDataMES[0].singleFactor);
                                //gVariable.dispatchSheet[myBoardIndex].materialCode = dispatchDataMES[0].materialCode;
                            }
                            else
                            {
                                if (dispatchAppearIndex != 0 && gVariable.machineCommunicationType[myBoardIndex] == gVariable.typeAllFromBoard)  // app not control dispatch instructions like apply/start/complete
                                {
                                    //if there is already dispatch exists, and this request comes from App, and board ID is not 6, we should not generate a new dispatch, just use the previous one
                                    //because if we want touchpad and App to generate new dispatch by themselves, there will be confliction, so we only support board 6 to be able to geenrae/control 
                                    //a new dispatch by App, for other board/machine, App can only read dispatch/quality/craft info, it cannot start/complete a dispatch or update quality data 
                                    dispatchAppearIndexTmp = dispatchAppearIndex - 1;
                                }
                                else
                                    dispatchAppearIndexTmp = dispatchAppearIndex;

                                dName = gVariable.internalMachineName[myBoardIndex];
                                tName = gVariable.dispatchListTableName;

                                //find oldest published dispatch, and saet it as our target dispatch 
                                commandText = "select * from `" + tName + "` where status = '0' order by planTime1";

                                tableArray = mySQLClass.databaseCommonReading(dName, commandText);
                                if(tableArray != null)
                                {
                                    //tableArray[0, 0] means the ID of the first dispatch by commandText 
                                    gVariable.dispatchSheet[myBoardIndex] = mySQLClass.getDispatchByID(dName, tName, Convert.ToInt32(tableArray[0, 0]));
                                }
                                else
                                {
                                    //no new dispatch found
                                    return RESULT_ERR_NO_DATA_RECEIVED;
                                }
                                //gVariable.dispatchSheet[myBoardIndex].serialNumber = gVariable.startingSerialNumber; //continue with previous serail number
                            }

                            //we will generate material sheet after doing APS, so this fake material sheet is not useful any more
                            /*
                            strArray = "j.1.05.001;40;Y.03.04.12;5".Split(',', ';');

                            if (strArray.Length / 2 > gVariable.maxMaterialTypeNum)
                                num = gVariable.maxMaterialTypeNum;
                            else
                                num = strArray.Length / 2;

                            for (i = 0; i < num; i++)
                            {
                                gVariable.materialList[myBoardIndex].materialCode[i] = strArray[i * 2];
                                gVariable.materialList[myBoardIndex].materialRequired[i] = Convert.ToInt16(strArray[i * 2 + 1]);
                            }

                            gVariable.materialList[myBoardIndex].numberOfTypes = num;
                            //dispatchCodeAlarm = gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                             */
                            break;

                        case COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_TOUCHPAD:
                            outStr = "read craft";
                            if (gVariable.faultData == 0)
                            {
                                ServiceReference8.returnCraftPram[] craftInfoMES;

                                ServiceReference8.CraftPramServiceClient getCraft = new ServiceReference8.CraftPramServiceClient();
                                craftInfoMES = getCraft.queryByDispatchNo(gVariable.dispatchSheet[myBoardIndex].dispatchCode);

                                if (craftInfoMES == null)
                                    num = 0;
                                else
                                    num = craftInfoMES.Length;

                                if (num > gVariable.maxCraftParamNum)
                                    num = gVariable.maxCraftParamNum;
                                gVariable.craftList[myBoardIndex].paramNumber = num;
                                gVariable.craftList[myBoardIndex].workingVoltage = 0;  //we are working at 5V voltage system

                                for (i = 0; i < num; i++)
                                {
                                    gVariable.craftList[myBoardIndex].id[i] = (int)craftInfoMES[i].Id;
                                    gVariable.craftList[myBoardIndex].paramName[i] = craftInfoMES[i].name;
                                    gVariable.craftList[myBoardIndex].paramLowerLimit[i] = (float)craftInfoMES[i].btmLmt;
                                    gVariable.craftList[myBoardIndex].paramUpperLimit[i] = (float)craftInfoMES[i].topLmt;
                                    gVariable.craftList[myBoardIndex].paramDefaultValue[i] = (float)craftInfoMES[i].defVal;
                                    gVariable.craftList[myBoardIndex].paramUnit[i] = craftInfoMES[i].unit;
                                    gVariable.craftList[myBoardIndex].paramValue[i] = Convert.ToInt32(craftInfoMES[i].actualVal);
                                }
                            }
                            else
                            {
                                if (gVariable.CompanyIndex == gVariable.ZIHUA_ENTERPRIZE || gVariable.CompanyIndex == gVariable.DONGFENG_20)
                                {
                                    getCraftDataDesc();
                                }
                                else //if( (gVariable.CompanyIndex == gVariable.DONGFENG_23)
                                {
                                    gVariable.craftList[myBoardIndex].paramNumber = 6;
                                    gVariable.craftList[myBoardIndex].workingVoltage = 0;

                                    gVariable.craftList[myBoardIndex].paramName[0] = "管内气压";
                                    gVariable.craftList[myBoardIndex].paramLowerLimit[0] = rangLowADC[0];
                                    gVariable.craftList[myBoardIndex].paramUpperLimit[0] = rangHighADC[0];
                                    gVariable.craftList[myBoardIndex].paramDefaultValue[0] = 112;
                                    gVariable.craftList[myBoardIndex].paramUnit[0] = "帕斯卡";
                                    gVariable.craftList[myBoardIndex].paramValue[0] = 0;

                                    gVariable.craftList[myBoardIndex].paramName[1] = "管内温度";
                                    gVariable.craftList[myBoardIndex].paramLowerLimit[1] = rangLowADC[1];
                                    gVariable.craftList[myBoardIndex].paramUpperLimit[1] = rangHighADC[1];
                                    gVariable.craftList[myBoardIndex].paramDefaultValue[1] = 120;
                                    gVariable.craftList[myBoardIndex].paramUnit[1] = "度";
                                    gVariable.craftList[myBoardIndex].paramValue[1] = 0;

                                    gVariable.craftList[myBoardIndex].paramName[2] = "夹紧压力 1";
                                    gVariable.craftList[myBoardIndex].paramLowerLimit[2] = rangLowADC[2];
                                    gVariable.craftList[myBoardIndex].paramUpperLimit[2] = rangHighADC[2];
                                    gVariable.craftList[myBoardIndex].paramDefaultValue[2] = (rangLowADC[2] + rangHighADC[2]) / 2;
                                    gVariable.craftList[myBoardIndex].paramUnit[2] = "牛顿";
                                    gVariable.craftList[myBoardIndex].paramValue[2] = 0;

                                    gVariable.craftList[myBoardIndex].paramName[3] = "主轴转速";
                                    gVariable.craftList[myBoardIndex].paramLowerLimit[3] = rangLowADC[3];
                                    gVariable.craftList[myBoardIndex].paramUpperLimit[3] = rangHighADC[3];
                                    gVariable.craftList[myBoardIndex].paramDefaultValue[3] = (rangLowADC[2] + rangHighADC[2]) / 2;
                                    gVariable.craftList[myBoardIndex].paramUnit[3] = "圈/秒";
                                    gVariable.craftList[myBoardIndex].paramValue[3] = 0;

                                    gVariable.craftList[myBoardIndex].paramName[4] = "表面温度";
                                    gVariable.craftList[myBoardIndex].paramLowerLimit[4] = rangLowADC[4];
                                    gVariable.craftList[myBoardIndex].paramUpperLimit[4] = rangHighADC[4];
                                    gVariable.craftList[myBoardIndex].paramDefaultValue[4] = 4.0f;
                                    gVariable.craftList[myBoardIndex].paramUnit[4] = "度";
                                    gVariable.craftList[myBoardIndex].paramValue[4] = 0;

                                    gVariable.craftList[myBoardIndex].paramName[5] = "夹紧压力 2";
                                    gVariable.craftList[myBoardIndex].paramLowerLimit[5] = rangLowADC[5];
                                    gVariable.craftList[myBoardIndex].paramUpperLimit[5] = rangHighADC[5];
                                    gVariable.craftList[myBoardIndex].paramDefaultValue[5] = 120;
                                    gVariable.craftList[myBoardIndex].paramUnit[5] = "牛顿";
                                    gVariable.craftList[myBoardIndex].paramValue[5] = 0;

                                    gVariable.craftList[myBoardIndex].paramName[6] = "dd";
                                    gVariable.craftList[myBoardIndex].paramLowerLimit[6] = 1;
                                    gVariable.craftList[myBoardIndex].paramUpperLimit[6] = 10;
                                    gVariable.craftList[myBoardIndex].paramDefaultValue[6] = 2;
                                    gVariable.craftList[myBoardIndex].paramUnit[6] = "豪米";
                                    gVariable.craftList[myBoardIndex].paramValue[6] = 0;

                                    gVariable.craftList[myBoardIndex].paramName[7] = "ee";
                                    gVariable.craftList[myBoardIndex].paramLowerLimit[7] = 1;
                                    gVariable.craftList[myBoardIndex].paramUpperLimit[7] = 10;
                                    gVariable.craftList[myBoardIndex].paramDefaultValue[7] = 2;
                                    gVariable.craftList[myBoardIndex].paramUnit[7] = "豪米";
                                    gVariable.craftList[myBoardIndex].paramValue[7] = 0;
                                }
                            }
                            break;
                        case COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD:
                            outStr = "read quality";
                            if (gVariable.faultData == 0)
                            {
                                ServiceReference5.ReturnChkItem[] qualityItemMES;

                                ServiceReference5.ChkItemListServiceClient getQuality = new ServiceReference5.ChkItemListServiceClient();
                                qualityItemMES = getQuality.queryChkItemByDispatchNo(gVariable.dispatchSheet[myBoardIndex].dispatchCode);

                                if (qualityItemMES == null)
                                    num = 0;
                                else
                                    num = qualityItemMES.Length;

                                if (num > gVariable.maxQualityDataNum)
                                    num = gVariable.maxQualityDataNum;

                                gVariable.qualityList[myBoardIndex].checkItemNumber = num;

                                for (i = 0; i < num; i++)
                                {
                                    gVariable.qualityList[myBoardIndex].id[i] = (int)qualityItemMES[i].Id;
                                    gVariable.qualityList[myBoardIndex].checkItem[i] = qualityItemMES[i].checkItem;
                                    gVariable.qualityList[myBoardIndex].checkRequirement[i] = qualityItemMES[i].checkReq;
                                    gVariable.qualityList[myBoardIndex].controlCenterValue1[i] = 0;
                                    gVariable.qualityList[myBoardIndex].controlCenterValue2[i] = 0;
                                    gVariable.qualityList[myBoardIndex].specLowerLimit[i] = (float)Convert.ToDouble(qualityItemMES[i].lowerToleranceLimit);
                                    gVariable.qualityList[myBoardIndex].controlLowerLimit1[i] = 0;
                                    gVariable.qualityList[myBoardIndex].controlLowerLimit2[i] = 0;
                                    gVariable.qualityList[myBoardIndex].specUpperLimit[i] = (float)Convert.ToDouble(qualityItemMES[i].toleranceLimit);
                                    gVariable.qualityList[myBoardIndex].controlUpperLimit1[i] = 0;
                                    gVariable.qualityList[myBoardIndex].controlUpperLimit2[i] = 0;
                                    gVariable.qualityList[myBoardIndex].checkResultData[i] = qualityItemMES[i].checkResult;
                                    gVariable.qualityList[myBoardIndex].checkResult[i] = qualityItemMES[i].judgeResult;
                                }

                            }
                            else
                            {
                                if (gVariable.CompanyIndex == gVariable.ZIHUA_ENTERPRIZE || gVariable.CompanyIndex == gVariable.DONGFENG_20)
                                {
                                    getQualityDataDesc();
                                }
                                else //if( (gVariable.CompanyIndex == gVariable.DONGFENG_23)
                                {
                                    gVariable.qualityList[myBoardIndex].checkItemNumber = 2;

                                    gVariable.qualityList[myBoardIndex].checkItem[0] = "槽宽";
                                    gVariable.qualityList[myBoardIndex].checkRequirement[0] = "在限定范围内";
                                    gVariable.qualityList[myBoardIndex].specLowerLimit[0] = 1;
                                    gVariable.qualityList[myBoardIndex].controlLowerLimit1[0] = 1;
                                    gVariable.qualityList[myBoardIndex].specUpperLimit[0] = 2;
                                    gVariable.qualityList[myBoardIndex].controlUpperLimit1[0] = 2;
                                    gVariable.qualityList[myBoardIndex].checkResultData[0] = "3";
                                    gVariable.qualityList[myBoardIndex].checkResult[0] = "Y";

                                    gVariable.qualityList[myBoardIndex].checkItem[1] = "槽深";
                                    gVariable.qualityList[myBoardIndex].checkRequirement[1] = "在限定范围内";
                                    gVariable.qualityList[myBoardIndex].specLowerLimit[1] = 1;
                                    gVariable.qualityList[myBoardIndex].controlLowerLimit1[1] = 1;
                                    gVariable.qualityList[myBoardIndex].specUpperLimit[1] = 2;
                                    gVariable.qualityList[myBoardIndex].controlUpperLimit1[1] = 2;
                                    gVariable.qualityList[myBoardIndex].checkResultData[1] = "3";
                                    gVariable.qualityList[myBoardIndex].checkResult[1] = "Y";
                                }
                            }
                            break;
                        case COMMUNICATION_TYPE_MATERIAL_DATA_REQ:
                            outStr = "read material";
                            if (gVariable.faultData == 0)
                            {
                                ServiceReference7.ReturnMaterialInfo[] materialList;

                                ServiceReference7.MaterialAndonServiceClient getMaterial = new ServiceReference7.MaterialAndonServiceClient();
                                materialList = getMaterial.queryActiveMaterial(gVariable.machineStatus[myBoardIndex].machineCode);

                                strArray = materialList[0].batchNo.Split(',');

                                if (strArray.Length > gVariable.maxMaterialTypeNum)
                                    num = gVariable.maxMaterialTypeNum;
                                else
                                    num = strArray.Length;

                                gVariable.materialList[myBoardIndex].dispatchCode = materialList[0].dispatchOrderNo;
                                gVariable.materialList[myBoardIndex].machineCode = materialList[0].deviceNo;
                                for (i = 0; i < num; i++)
                                {
                                    gVariable.materialList[myBoardIndex].materialCode[i] = strArray[i];
                                    gVariable.materialList[myBoardIndex].materialRequired[i] = (int)materialList[0].planNum;
                                }

                                gVariable.materialList[myBoardIndex].numberOfTypes = num;

                            }
                            else
                            {
                                gVariable.materialList[myBoardIndex].numberOfTypes = 2;

                                gVariable.materialList[myBoardIndex].dispatchCode = gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                                gVariable.materialList[myBoardIndex].machineID = gVariable.machineStatus[myBoardIndex].machineID;
                                gVariable.materialList[myBoardIndex].machineCode = gVariable.machineStatus[myBoardIndex].machineCode;
                                if (gVariable.CompanyIndex == gVariable.DONGFENG_20)
                                {
                                    gVariable.materialList[myBoardIndex].materialName[0] = "铝锭(公斤)";
                                    gVariable.materialList[myBoardIndex].materialName[1] = "合金(克)";
                                    gVariable.materialList[myBoardIndex].materialName[2] = "";
                                    gVariable.materialList[myBoardIndex].materialCode[0] = "P1750034";
                                    gVariable.materialList[myBoardIndex].materialCode[1] = "P1750087";
                                    gVariable.materialList[myBoardIndex].materialCode[2] = "";
                                    gVariable.materialList[myBoardIndex].materialRequired[0] = 26;
                                    gVariable.materialList[myBoardIndex].materialRequired[1] = 25;
                                    gVariable.materialList[myBoardIndex].materialRequired[2] = 11;
                                }
                                else
                                {
                                    gVariable.materialList[myBoardIndex].materialName[0] = "PE 料(公斤)";
                                    gVariable.materialList[myBoardIndex].materialName[1] = "色母料(公斤)";
                                    gVariable.materialList[myBoardIndex].materialName[2] = "EC7000";
                                    gVariable.materialList[myBoardIndex].materialCode[0] = "Y.1.06.0002";
                                    gVariable.materialList[myBoardIndex].materialCode[1] = "Y.1.06.0001";
                                    gVariable.materialList[myBoardIndex].materialCode[2] = "Y.1.05.0026";
                                    gVariable.materialList[myBoardIndex].materialRequired[0] = 25;
                                    gVariable.materialList[myBoardIndex].materialRequired[1] = 2;
                                    gVariable.materialList[myBoardIndex].materialRequired[2] = 21;
                                }
                            }
                            break;

                        case COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_TO_REQ:
                            outStr = "read andon list";

                            ServiceReference4.returnDeviceAndon[] deviceAndonResponse;

                            try
                            {
                                if (gVariable.faultData == 0)
                                {
                                    ServiceReference4.DeviceAndonServiceClient deviceAndonEnquiry = new ServiceReference4.DeviceAndonServiceClient();
                                    deviceAndonResponse = deviceAndonEnquiry.queryDeviceAndonInfo(gVariable.machineStatus[myBoardIndex].machineCode);


                                    if (deviceAndonResponse == null)
                                        num = 0;
                                    else
                                        num = deviceAndonResponse.Length;

                                    numOfDeviceAlarm = 0;
                                    for (i = 0; i < num; i++)
                                    {
                                        //only alarms that are still alive will be sent to touch pad
                                        if (deviceAndonResponse[i].status != alarmStatusList[ALARM_PROCESSED] && deviceAndonResponse[i].status != alarmStatusList[ALARM_CANCELLED])
                                        {
                                            deviceAlarmListTable[i].machineCode = deviceAndonResponse[i].deviceNo;
                                            deviceAlarmListTable[i].alarmFailureCode = deviceAndonResponse[i].faultNo;
                                            deviceAlarmListTable[i].machineName = deviceAndonResponse[i].deviceName;
                                            deviceAlarmListTable[i].workshop = deviceAndonResponse[i].workshop;
                                            deviceAlarmListTable[i].errorDesc = deviceAndonResponse[i].faultDesc;
                                            for (j = 0; j < gConstText.deviceErrNoList.Length; j++)
                                            {
                                                if (deviceAndonResponse[i].faultDesc == gConstText.deviceErrNoList[j])
                                                {
                                                    deviceAlarmListTable[i].errorDesc = gConstText.deviceErrDescList[j];
                                                    break;
                                                }
                                            }
                                            deviceAlarmListTable[i].operatorName = deviceAndonResponse[i].tsUser.ToString();
                                            deviceAlarmListTable[i].time = deviceAndonResponse[i].time.ToString();

                                            //change MES alarm status to my status
                                            if (deviceAndonResponse[i].status == null)
                                                deviceAlarmListTable[i].status = 0;
                                            else
                                            {
                                                for (j = 0; j < alarmStatusList.Length; j++)
                                                {
                                                    if (alarmStatusList[j] == deviceAndonResponse[i].status)
                                                        break;
                                                }

                                                if (j > alarmStatusList.Length)
                                                    j = 0;

                                                deviceAlarmListTable[i].status = j;
                                            }

                                            deviceStatusAlarm[i] = (byte)deviceAlarmListTable[i].status;
                                            numOfDeviceAlarm++;
                                            if (numOfDeviceAlarm >= gVariable.maxDeviceAlarmNum)  //touch pad only support this number of alarms
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    dName = gVariable.internalMachineName[myBoardIndex];
                                    tName = gVariable.alarmListTableName;

                                    num = mySQLClass.getRecordNumInTable(dName, tName);

                                    today = DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss");

                                    j = 0;
                                    numOfDeviceAlarm = 0;
                                    for (i = num; i >= 1; i--)
                                    {
                                        alarmTableStructImpl = mySQLClass.getAlarmTableContent(dName, tName, i);
                                        if (alarmTableStructImpl.dispatchCode == null)
                                            break;

                                        if (type != gVariable.ALARM_TYPE_DEVICE || status >= ALARM_PROCESSED)
                                            continue;

                                        if (string.Compare(time, today) < 0)
                                            break;

                                        deviceAlarmListTable[j].machineCode = gVariable.machineStatus[myBoardIndex].machineCode;
                                        deviceAlarmListTable[j].alarmFailureCode = alarmFailureCode;
                                        deviceAlarmListTable[j].deviceFailureIndex = 2;
                                        deviceAlarmListTable[j].machineName = gVariable.machineStatus[myBoardIndex].machineName;
                                        if (gVariable.CompanyIndex == gVariable.ZIHUA_ENTERPRIZE)
                                        {
                                            deviceAlarmListTable[j].workshop = "流延车间";
                                        }
                                        else if (gVariable.CompanyIndex == gVariable.DONGFENG_20)
                                        {
                                            deviceAlarmListTable[j].workshop = "压铸工车间";
                                        }
                                        else //if(gVariable.CompanyIndex == gVariable.DONGFENG_23)
                                        {
                                            deviceAlarmListTable[j].workshop = "粗加工车间";
                                        }
                                        deviceAlarmListTable[j].errorDesc = errorDesc;
                                        deviceAlarmListTable[j].operatorName = operatorName;
                                        deviceAlarmListTable[j].time = time;
                                        deviceAlarmListTable[j].time1 = time1;
                                        deviceAlarmListTable[j].time2 = time2;
                                        deviceAlarmListTable[j].status = status;

                                        j++;
                                        numOfDeviceAlarm++;

                                        if (j >= gVariable.maxDeviceAlarmNum)
                                            break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.Write("device andon alarm list requst fail" + ex);
                                return RESULT_ERR_NO_DATA_RECEIVED;
                            }
                            break;
                    }
                    return RESULT_OK;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(DateTime.Now.ToString() + ": Board " + (myBoardIndex + 1) + outStr + " failed！ " + ex);
                    return RESULT_ERR_NO_DATA_RECEIVED;
                }
            }

            //this function is not used in 23
            private void getCraftDataDesc()
            {
                int i, num;
                string dataNum;
                string space;
                string[] filePath = { "..\\..\\data\\craftDataDesc23.txt", "..\\..\\data\\craftDataDescZihua.txt", "..\\..\\data\\craftDataDesc20.txt" };
                StreamReader streamReader;

                try
                {
                    streamReader = new StreamReader(filePath[gVariable.CompanyIndex], System.Text.Encoding.Default);
                    dataNum = streamReader.ReadLine().Trim();

                    num = Convert.ToInt16(dataNum);
                    gVariable.craftList[myBoardIndex].paramNumber = num;
                    gVariable.craftList[myBoardIndex].workingVoltage = 0;

                    for (i = 0; i < num; i++)
                    {
                        space = streamReader.ReadLine();  //empty line
                        gVariable.craftList[myBoardIndex].paramName[i] = streamReader.ReadLine().Trim();  //name of the parameter
                        gVariable.craftList[myBoardIndex].paramLowerLimit[i] = (float)Convert.ToDouble(streamReader.ReadLine().Trim()); //spec low thresh
                        gVariable.craftList[myBoardIndex].paramUpperLimit[i] = (float)Convert.ToDouble(streamReader.ReadLine().Trim());  //spec high thresh
                        gVariable.craftList[myBoardIndex].rangeLowerLimit[i] = (float)Convert.ToDouble(streamReader.ReadLine().Trim()); //range low thresh
                        gVariable.craftList[myBoardIndex].rangeUpperLimit[i] = (float)Convert.ToDouble(streamReader.ReadLine().Trim());  //range high thresh
                        gVariable.craftList[myBoardIndex].paramDefaultValue[i] = 0;
                        gVariable.craftList[myBoardIndex].paramUnit[i] = streamReader.ReadLine().Trim(); //unit
                        gVariable.craftList[myBoardIndex].paramValue[i] = 0;
                    }

                    streamReader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("craftDataDesc.txt 文件格式有误" + ex);
                }
            }

            //this function is not used in 23
            private void getQualityDataDesc()
            {
                int i, num;
                string dataNum;
                string space;
                string[] filePath = { "..\\..\\data\\qualityDataDesc23.txt", "..\\..\\data\\qualityDataDescZihua.txt", "..\\..\\data\\qualityDataDesc20.txt" };
                StreamReader streamReader;

                try
                {
                    streamReader = new StreamReader(filePath[gVariable.CompanyIndex], System.Text.Encoding.Default);
                    dataNum = streamReader.ReadLine().Trim();

                    num = Convert.ToInt16(dataNum);
                    gVariable.qualityList[myBoardIndex].checkItemNumber = num;

                    for (i = 0; i < num; i++)
                    {
                        space = streamReader.ReadLine();  //empty line
                        gVariable.qualityList[myBoardIndex].checkItem[i] = streamReader.ReadLine().Trim();  //name of the parameter
                        gVariable.qualityList[myBoardIndex].checkRequirement[i] = streamReader.ReadLine().Trim(); //description
                        gVariable.qualityList[myBoardIndex].specLowerLimit[i] = (float)Convert.ToDouble(streamReader.ReadLine().Trim()); //low thresh;
                        gVariable.qualityList[myBoardIndex].specUpperLimit[i] = (float)Convert.ToDouble(streamReader.ReadLine().Trim()); //high thresh;
                        gVariable.qualityList[myBoardIndex].checkResultData[i] = "";
                        gVariable.qualityList[myBoardIndex].unit[i] = streamReader.ReadLine().Trim(); //unit
                        gVariable.qualityList[myBoardIndex].chartType[i] = Convert.ToInt16(streamReader.ReadLine().Trim()); //chart type: 0 no SPC; 1:C chart SPC; 2:XBar-S
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("qualityDataDesc.txt 文件格式有误" + ex);
                }
            }


            //copy data from our internal global variables to MES format vaiables, then send them to MES
            public int writeMESString(int tableIndex)
            {
                int i;
                int num;
                int status;
                string ret = " ";
                string outStr = "";

                try
                {
                    switch (tableIndex)
                    {
                        case COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_PC:
                            outStr = " craft update:";
                            ServiceReference8.returnCraftPram[] craftInfoMES;

                            craftInfoMES = new ServiceReference8.returnCraftPram[gVariable.maxCraftParamNum];

                            num = gVariable.craftList[myBoardIndex].paramNumber;

                            for (i = 0; i < num; i++)
                            {
                                craftInfoMES[i] = new ServiceReference8.returnCraftPram();

                                craftInfoMES[i].Id = gVariable.craftList[myBoardIndex].id[i];
                                craftInfoMES[i].IdSpecified = true;
                                craftInfoMES[i].dispatchNo = gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                                craftInfoMES[i].name = gVariable.craftList[myBoardIndex].paramName[i];
                                craftInfoMES[i].btmLmt = Convert.ToDouble(gVariable.craftList[myBoardIndex].paramLowerLimit[i]);
                                craftInfoMES[i].topLmt = Convert.ToDouble(gVariable.craftList[myBoardIndex].paramUpperLimit[i]);
                                craftInfoMES[i].defVal = Convert.ToDouble(gVariable.craftList[myBoardIndex].paramDefaultValue[i]);
                                craftInfoMES[i].unit = gVariable.craftList[myBoardIndex].paramUnit[i];
                                craftInfoMES[i].actualVal = gVariable.craftList[myBoardIndex].paramValue[i].ToString();
                            }

                            if (gVariable.faultData == 0)
                            {
                                ServiceReference8.CraftPramServiceClient addCraft = new ServiceReference8.CraftPramServiceClient();
                                ret = addCraft.addCraftPram(craftInfoMES);
                                Console.WriteLine(DateTime.Now.ToString() + ":Board " + (myBoardIndex + 1) + outStr + ret);
                            }
                            break;
                        case COMMUNICATION_TYPE_DISPATCH_COMPLETED_TO_PC:
                            outStr = " dispatch completed";
                            ServiceReference2.ReturnActiveDispatch[] dispatchDataMES;

                            dispatchDataMES = new ServiceReference2.ReturnActiveDispatch[1];

                            dispatchDataMES[0] = new ServiceReference2.ReturnActiveDispatch();

                            dispatchDataMES[0].dispatchNo = gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                            dispatchDataMES[0].planStartTime = gVariable.dispatchSheet[myBoardIndex].planTime1;
                            dispatchDataMES[0].planEndTime = gVariable.dispatchSheet[myBoardIndex].planTime2;
                            dispatchDataMES[0].productNo = gVariable.dispatchSheet[myBoardIndex].productCode;
                            dispatchDataMES[0].productName = gVariable.dispatchSheet[myBoardIndex].productName;
                            if (gVariable.dispatchSheet[myBoardIndex].operatorName == null)
                                dispatchDataMES[0].operationUser = "10059";
                            else
                                dispatchDataMES[0].operationUser = gVariable.dispatchSheet[myBoardIndex].operatorName;
                            dispatchDataMES[0].planNum = gVariable.dispatchSheet[myBoardIndex].plannedNumber.ToString();
                            dispatchDataMES[0].qualifiedNum = gVariable.dispatchSheet[myBoardIndex].qualifiedNumber.ToString();
                            dispatchDataMES[0].disqualifiedNum = gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber.ToString();
                            dispatchDataMES[0].currentProcess = gVariable.dispatchSheet[myBoardIndex].processName;
                            if (gVariable.dispatchSheet[myBoardIndex].realStartTime != null)
                                dispatchDataMES[0].startDate = gVariable.dispatchSheet[myBoardIndex].realStartTime;

                            dispatchDataMES[0].entireLifeCount = gVariable.dispatchSheet[myBoardIndex].toolLifeTimes.ToString();
                            dispatchDataMES[0].useNumber = (toolUsedTimesNow + dispatchDataMES[0].qualifiedNum + dispatchDataMES[0].disqualifiedNum).ToString();
                            dispatchDataMES[0].singleFactor = gVariable.dispatchSheet[myBoardIndex].outputRatio.ToString();

                            if (gVariable.faultData == 0)
                            {
                                ServiceReference2.ActiveDispatchOrderServiceClient addDispatch = new ServiceReference2.ActiveDispatchOrderServiceClient();
                                ret = addDispatch.addActiveDispatch(dispatchDataMES[0]);
                                Console.WriteLine(DateTime.Now.ToString() + ":Board " + (myBoardIndex + 1) + outStr + ret);
                            }
                            break;

                        case COMMUNICATION_TYPE_MACHINE_STATUS_TO_PC:
                            outStr = " machine status update:";
                            ServiceReference3.deviceStatusInfo deviceStatusInfoMES;
                            deviceStatusInfoMES = new ServiceReference3.deviceStatusInfo();

                            deviceStatusInfoMES.dispatchOrderNo = gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                            deviceStatusInfoMES.deviceNo = gVariable.machineStatus[myBoardIndex].machineCode;
                            deviceStatusInfoMES.workTime = gVariable.machineStatus[myBoardIndex].totalWorkingTime;
                            deviceStatusInfoMES.prdBeat = gVariable.machineStatus[myBoardIndex].productBeat;
                            deviceStatusInfoMES.energyConsumption = gVariable.machineStatus[myBoardIndex].powerConsumed;
                            deviceStatusInfoMES.standbyTime = gVariable.machineStatus[myBoardIndex].standbyTime;
                            deviceStatusInfoMES.power = gVariable.machineStatus[myBoardIndex].power;
                            deviceStatusInfoMES.deviceCollectNum = gVariable.machineStatus[myBoardIndex].collectedNumber;
                            deviceStatusInfoMES.speed = gVariable.machineStatus[myBoardIndex].revolution;
                            deviceStatusInfoMES.pressure = gVariable.machineStatus[myBoardIndex].pressure;
                            deviceStatusInfoMES.prepareTime = gVariable.machineStatus[myBoardIndex].prepareTime;
                            deviceStatusInfoMES.realWorkTime = gVariable.machineStatus[myBoardIndex].workingTime;

                            if (gVariable.faultData == 0)
                            {
                                ServiceReference3.DeviceStatusServiceClient deviceStatus = new ServiceReference3.DeviceStatusServiceClient();
                                ret = deviceStatus.addDeviceStatusCurrent(deviceStatusInfoMES);
                                Console.WriteLine(DateTime.Now.ToString() + ":Board " + (myBoardIndex + 1) + outStr + ret);
                            }
                            break;

                        case COMMUNICATION_TYPE_DEVICE_ANDON_TO_PC:
                            outStr = " device Andon upload:";
                            ServiceReference1.addDeviceAndonInfo deviceAndonRequest = new ServiceReference1.addDeviceAndonInfo();
                            ServiceReference1.DeviceFailureServiceClient deviceAndon = new ServiceReference1.DeviceFailureServiceClient();

                            deviceAndonRequest.deviceNo = gVariable.machineStatus[myBoardIndex].machineCode;

                            deviceAndonRequest.failureNo = gVariable.errorCodeList.errorCode[Convert.ToInt16(alarmTableStructImpl.deviceFailureIndex)]; //"303010";
                            deviceAndonRequest.userNo = alarmTableStructImpl.operatorName; //"admin"; //gVariable.dispatchSheet[myBoardIndex].operatorName;

                            if (gVariable.faultData == 0)
                            {
                                ret = deviceAndon.addDeviceAndonInfo(deviceAndonRequest.deviceNo, deviceAndonRequest.failureNo, deviceAndonRequest.userNo);
                                Console.WriteLine(DateTime.Now.ToString() + ":Board " + (myBoardIndex + 1) + outStr + ret);
                            }

                            //                            readMESString(COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_TO_TOUCHPAD);

                            break;

                        case COMMUNICATION_TYPE_QUALITY_DATA_TO_PC:
                            outStr = " quality data upload:";
                            ServiceReference5.ReturnChkItem[] qualityItemMES;

                            qualityItemMES = new ServiceReference5.ReturnChkItem[gVariable.maxQualityDataNum];

                            num = gVariable.qualityList[myBoardIndex].checkItemNumber;

                            for (i = 0; i < num; i++)
                            {
                                qualityItemMES[i] = new ServiceReference5.ReturnChkItem();

                                qualityItemMES[i].Id = gVariable.qualityList[myBoardIndex].id[i];
                                qualityItemMES[i].IdSpecified = true;
                                qualityItemMES[i].dispatchNo = gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                                qualityItemMES[i].checkItem = gVariable.qualityList[myBoardIndex].checkItem[i];
                                qualityItemMES[i].checkReq = gVariable.qualityList[myBoardIndex].checkRequirement[i];
                                qualityItemMES[i].lowerToleranceLimit = gVariable.qualityList[myBoardIndex].specLowerLimit[i].ToString();
                                qualityItemMES[i].lowerControlLimit = gVariable.qualityList[myBoardIndex].controlLowerLimit1[i].ToString();
                                qualityItemMES[i].toleranceLimit = gVariable.qualityList[myBoardIndex].specUpperLimit[i].ToString();
                                qualityItemMES[i].upperControlLimit = gVariable.qualityList[myBoardIndex].controlUpperLimit1[i].ToString();
                                qualityItemMES[i].checkResult = gVariable.qualityList[myBoardIndex].checkResultData[i];  //data to MES
                                qualityItemMES[i].judgeResult = gVariable.qualityList[myBoardIndex].checkResult[i];  //
                            }

                            if (gVariable.faultData == 0)
                            {
                                ServiceReference5.ChkItemListServiceClient addQuality = new ServiceReference5.ChkItemListServiceClient();
                                ret = addQuality.addTwProductProcessCheck(qualityItemMES);
                                Console.WriteLine(DateTime.Now.ToString() + ":Board " + (myBoardIndex + 1) + outStr + ret);
                            }
                            break;
                        case COMMUNICATION_TYPE_MATERIAL_ANDON_TO_PC:
                            outStr = " material andon upload:";
                            //ServiceReference7.addMaterialAndonInfoResponse response;
                            ServiceReference7.MaterialAndonServiceClient materialAndon = new ServiceReference7.MaterialAndonServiceClient();

                            if (gVariable.faultData == 0)
                            {
                                //alarmTableStructImpl.operatorName value comes from touchpad in putPDATouchpadArrayToVariables()
                                ret = materialAndon.addMaterialAndonInfo(gVariable.dispatchSheet[myBoardIndex].dispatchCode, alarmTableStructImpl.operatorName);
                                Console.WriteLine(DateTime.Now.ToString() + ":Board " + (myBoardIndex + 1) + outStr + "by" + alarmTableStructImpl.operatorName + ret);
                            }
                            break;

                        case COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_DEAL_TO_PC:
                            outStr = " device andon status update:";
                            if (clientFromTouchpad == 1)
                            {
                                ServiceReference4.DeviceAndonServiceClient deviceAndonDispose = new ServiceReference4.DeviceAndonServiceClient();

                                for (i = 0; i < gVariable.maxDeviceAlarmNum; i++)
                                {
                                    if (deviceAlarmListTable[i].status != deviceStatusAlarm[i])
                                    {
                                        deviceAlarmListTable[i].status = deviceStatusAlarm[i];
                                        if (gVariable.faultData == 0)
                                        {
                                            if (deviceStatusAlarm[i] >= alarmStatusList.Length)
                                            {
                                                status = 0;
                                            }
                                            else
                                            {
                                                status = deviceStatusAlarm[i];
                                            }

                                            ret = deviceAndonDispose.disposedDeviceAndon(deviceAlarmListTable[i].operatorName, deviceAlarmListTable[i].alarmFailureCode, alarmStatusList[status]);
                                            Console.WriteLine(DateTime.Now.ToString() + ":Board " + (myBoardIndex + 1) + outStr + "by" + deviceAlarmListTable[i].operatorName + ret);

                                            deviceStatusAlarm[i] = (byte)deviceAlarmListTable[i].status;
                                        }
                                    }
                                }
                            }
                            break;
                    }

                    return RESULT_OK;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(DateTime.Now.ToString() + ":Board " + (myBoardIndex + 1) + outStr + "write MES failed ! " + ex);
                    return RESULT_ERR_FILE_OPEN_FAIL;
                }
            }

            //data that ned to be sent to toucpad will first get their format transformed into touchpad format
            //touchpad need a fixed length format, so we merge all related internal global variables into a string with the fixed length
            public string putVariableToTouchpadFormat(int index)
            {
                int i;
                int num;
                string str = null;
                string time1, time2, time3;
                string str1, str2, str3, str4, str5, str6, str7, str8, str9, str10, str11;

                try
                {

                    switch (index)
                    {
                        case COMMUNICATION_TYPE_DISPATCH_DATA_TO_TOUCHPAD: //both original sheet and report sheet
                            time1 = gVariable.dispatchSheet[myBoardIndex].planTime1.Replace("-", "").Replace(" ", "").Replace(":", "");
                            if (time1.Length > 13)
                                time1 = time1.Remove(13);
                            time2 = gVariable.dispatchSheet[myBoardIndex].planTime2.Replace("-", "").Replace(" ", "").Replace(":", "");
                            if (time2.Length > 13)
                                time2 = time2.Remove(13);
                            if (gVariable.dispatchSheet[myBoardIndex].realStartTime != null)
                            {
                                time3 = gVariable.dispatchSheet[myBoardIndex].realStartTime.Replace("-", "").Replace(" ", "").Replace(":", "");
                                if (time3.Length > 13)
                                    time3 = time3.Remove(13);
                            }
                            else
                                time3 = null;

                            if (gVariable.dispatchSheet[myBoardIndex].productCode == null)
                                str1 = null;
                            else
                                str1 = gVariable.dispatchSheet[myBoardIndex].productCode.ToString();

                            if (gVariable.dispatchSheet[myBoardIndex].productName == null)
                                str2 = null;
                            else
                                str2 = gVariable.dispatchSheet[myBoardIndex].productName.ToString();

                            str3 = gVariable.dispatchSheet[myBoardIndex].plannedNumber.ToString();
                            str4 = gVariable.dispatchSheet[myBoardIndex].qualifiedNumber.ToString();
                            str5 = gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber.ToString();
                            str6 = gVariable.dispatchSheet[myBoardIndex].outputRatio.ToString();
                            str = getFixedCountString(gVariable.dispatchSheet[myBoardIndex].dispatchCode, 21) + "&" + getFixedCountString(time1, 13) + "&" + getFixedCountString(time2, 13) + "&" +
                                  getFixedCountString(str1, 21) + "&" + getFixedCountString(str2, 31) + "&" +
                                  getFixedCountString(gVariable.dispatchSheet[myBoardIndex].operatorName, 37) + "&" + getFixedCountString(str3, 7) + "&" +
                                  getFixedCountString(str4, 7) + "&" + getFixedCountString(str5, 7) + "&" +
                                  getFixedCountString(gVariable.dispatchSheet[myBoardIndex].processName, 31) + "&" + getFixedCountString(time3, 13) + "&" +
                                  getFixedCountString(((gVariable.dispatchSheet[myBoardIndex].toolUsedTimes + toolUsedTimesNow) + "/" + gVariable.dispatchSheet[myBoardIndex].toolLifeTimes), 11) + "&" +
                                  getFixedCountString(str6, 7) + "&" + getFixedCountString("11", 13) + ";";
                            break;
                        case COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_TOUCHPAD:
                            for (i = 0; i < gVariable.craftList[myBoardIndex].paramNumber; i++)
                            {
                                str1 = gVariable.craftList[myBoardIndex].paramUpperLimit[i].ToString();
                                str2 = gVariable.craftList[myBoardIndex].paramLowerLimit[i].ToString();
                                str3 = gVariable.craftList[myBoardIndex].paramDefaultValue[i].ToString();
                                str4 = gVariable.craftList[myBoardIndex].paramValue[i].ToString();
                                str += getFixedCountString(gVariable.craftList[myBoardIndex].paramName[i], 13) + "&" + getFixedCountString(str2, 9) + "&" +
                                      getFixedCountString(str1, 9) + "&" + getFixedCountString(str3, 9) + "&" +
                                      getFixedCountString(gVariable.craftList[myBoardIndex].paramUnit[i], 9) + "&" + getFixedCountString(str4, 9) + ";";
                            }

                            //touchpad need a fixed length data, so if craft data has less lines, add to max limit
                            if (i < gVariable.maxCraftParamNum)
                            {
                                for (; i < gVariable.maxCraftParamNum; i++)
                                {
                                    str1 = " ";
                                    str += getFixedCountString(str1, 13) + "&" + getFixedCountString(str1, 9) + "&" + getFixedCountString(str1, 9) + "&" + getFixedCountString(str1, 9) + "&" +
                                          getFixedCountString(str1, 9) + "&" + getFixedCountString(str1, 9) + ";";
                                }
                            }
                            break;
                        case COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD:
                            for (i = 0; i < gVariable.qualityList[myBoardIndex].checkItemNumber; i++)
                            {
                                str1 = gVariable.qualityList[myBoardIndex].specLowerLimit[i].ToString();
                                str2 = gVariable.qualityList[myBoardIndex].controlLowerLimit1[i].ToString();
                                str3 = gVariable.qualityList[myBoardIndex].specUpperLimit[i].ToString();
                                str4 = gVariable.qualityList[myBoardIndex].controlUpperLimit1[i].ToString();
                                str5 = " "; // gVariable.qualityList[myBoardIndex].checkResultData[i].ToString();
                                str += getFixedCountString(gVariable.qualityList[myBoardIndex].checkItem[i], 21) + "&" + getFixedCountString(gVariable.qualityList[myBoardIndex].checkRequirement[i], 21) + "&" +
                                       getFixedCountString(str1, 11) + "&" + getFixedCountString(str2, 11) + "&" +
                                       getFixedCountString(str3, 11) + "&" + getFixedCountString(str4, 11) + "&" +
                                       getFixedCountString(str5, 11) + "&" + getFixedCountString(gVariable.qualityList[myBoardIndex].checkResult[i], 5) + ";";
                            }
                            //touchpad need a fixed length data, so if quality data has less lines, add to max limit
                            if (i < gVariable.maxQualityDataNum)
                            {
                                for (; i < gVariable.maxQualityDataNum; i++)
                                {
                                    str1 = " ";
                                    str += getFixedCountString(str1, 21) + "&" + getFixedCountString(str1, 21) + "&" + getFixedCountString(str1, 11) + "&" + getFixedCountString(str1, 11) + "&" +
                                           getFixedCountString(str1, 11) + "&" + getFixedCountString(str1, 11) + "&" + getFixedCountString(str1, 11) + "&" + getFixedCountString(str1, 5) + ";";
                                }
                            }
                            break;

                        case COMMUNICATION_TYPE_MATERIAL_DATA_REQ:
                            for (i = 0; i < gVariable.materialList[myBoardIndex].numberOfTypes; i++)
                            {
                                str1 = gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                                str2 = gVariable.materialList[myBoardIndex].materialCode[i];
                                str3 = gVariable.machineStatus[myBoardIndex].machineCode;
                                str4 = gVariable.materialList[myBoardIndex].materialRequired[i].ToString();
                                //touch pad deoe not support number of material for the time being
                                str += getFixedCountString(str1, 21) + "&" + getFixedCountString(str2, 21) + "&" + getFixedCountString(str3, 11) + "&" + getFixedCountString(str4, 5) + ";";
                            }
                            for (; i < gVariable.maxMaterialTypeNum; i++)
                            {
                                str1 = " ";
                                str2 = " ";
                                str3 = " ";
                                str4 = " ";
                                //touch pad deoe not support number of material for the time being
                                str += getFixedCountString(str1, 21) + "&" + getFixedCountString(str2, 21) + "&" + getFixedCountString(str3, 11) + "&" + getFixedCountString(str4, 5) + ";";
                            }

                            //str += getFixedCountString(gVariable.dispatchSheet[myBoardIndex].operatorName, 36);
                            break;

                        case COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_TO_REQ:
                            for (i = 0; i < numOfDeviceAlarm; i++)
                            {
                                str1 = deviceAlarmListTable[i].alarmFailureCode;
                                str2 = deviceAlarmListTable[i].machineCode;
                                str3 = deviceAlarmListTable[i].machineName;
                                str4 = deviceAlarmListTable[i].workshop;
                                str5 = deviceAlarmListTable[i].errorDesc;
                                str6 = deviceAlarmListTable[i].operatorName;
                                str7 = deviceAlarmListTable[i].time;

                                if (deviceAlarmListTable[i].status >= gVariable.strAlarmStatus.Length)
                                    deviceAlarmListTable[i].status = 0;

                                str8 = gVariable.strAlarmStatus[deviceAlarmListTable[i].status];

                                str += getFixedCountString(str1, 11) + "&" + getFixedCountString(str2, 31) + "&" + getFixedCountString(str3, 41) + "&" + getFixedCountString(str4, 21) + "&" +
                                       getFixedCountString(str5, 41) + "&" + getFixedCountString(str6, 35) + "&" + getFixedCountString(str7, 13) + "&" + getFixedCountString(str8, 21) + ";" + " ; ; ;";  //last 3 space are for andon status
                            }
                            for (; i < gVariable.maxDeviceAlarmNum; i++)
                            {
                                str1 = " ";
                                str2 = " ";
                                str3 = " ";
                                str4 = " ";
                                str5 = " ";
                                str6 = " ";
                                str7 = " ";
                                str8 = "0";

                                str += getFixedCountString(str1, 11) + "&" + getFixedCountString(str2, 31) + "&" + getFixedCountString(str3, 41) + "&" + getFixedCountString(str4, 21) + "&" +
                                       getFixedCountString(str5, 41) + "&" + getFixedCountString(str6, 35) + "&" + getFixedCountString(str7, 13) + "&" + getFixedCountString(str8, 21) + ";" + " ; ; ;";

                            }
                            break;

                        case COMMUNICATION_TYPE_ERROR_LIST_TO_TOUCHPAD:
                            num = gVariable.errorDescList.Length;

                            for (i = 0; i < num; i++)
                            {
                                str1 = gVariable.errorDescList[i].failureNo;
                                str2 = gVariable.errorDescList[i].failureName;

                                str += getFixedCountString(str1, 8) + "&" + getFixedCountString(str2, 22) + ";";
                            }
                            break;
                        case COMMUNICATION_TYPE_MACHINE_STATUS_DISPLAY:
                            str1 = gVariable.machineStatus[myBoardIndex].machineCode;
                            str2 = gVariable.machineStatus[myBoardIndex].machineName;
                            str3 = gVariable.machineStatus[myBoardIndex].totalWorkingTime.ToString();
                            str4 = gVariable.machineStatus[myBoardIndex].productBeat.ToString();
                            str5 = gVariable.machineStatus[myBoardIndex].powerConsumed.ToString();
                            str6 = gVariable.machineStatus[myBoardIndex].standbyTime.ToString();
                            str7 = gVariable.machineStatus[myBoardIndex].power.ToString();
                            str8 = gVariable.machineStatus[myBoardIndex].collectedNumber.ToString();
                            str9 = gVariable.machineStatus[myBoardIndex].revolution.ToString();
                            str10 = gVariable.machineStatus[myBoardIndex].prepareTime.ToString();
                            str11 = gVariable.machineStatus[myBoardIndex].workingTime.ToString();

                            str += getFixedCountString(str1, 31) + "&" + getFixedCountString(str2, 41) + "&" + getFixedCountString(str3, 11) + "&" + getFixedCountString(str4, 9) + "&" +
                                   getFixedCountString(str5, 5) + "&" + getFixedCountString(str6, 9) + "&" + getFixedCountString(str7, 5) + "&" + getFixedCountString(str8, 7) + "&" +
                                   getFixedCountString(str9, 7) + "&" + getFixedCountString(str10, 5) + "&" + getFixedCountString(str11, 5) + ";";
                            break;
                    }

                    return str;
                }
                catch (Exception ex)
                {
                    Console.Write("No MES directory under data" + ex);
                    Console.Write("putVariableToTouchpadFormat" + index + "信息失败！");
                    return null;
                }
            }


            //we got string from touchpad, it contains multi-column contents, we need to cut these contents by column and put them into different global variables accordingly
            public void putPDATouchpadArrayToVariables(string strInput, int tableIndex)
            {
                int i, j;
                int id, status;
                int flag;
                int deviceFailureIndex;
                int alarmIDInTable;
                float value;
                string dName, tName;
                string now;
                string operatorName;
                string mailList;
                string discuss;
                string solution;
                string[] strArray;
                string strTmp;
                string updateStr;
                byte[] aa;
                System.DateTime timeStampNow;

                try
                {
                    i = 0;
                    switch (tableIndex)
                    {
                        case COMMUNICATION_TYPE_SETTING_TO_PC:
                            strArray = strInput.Split(';', '&', '-');

                            i = 2;
                            strTmp = strArray[i++].Trim();
                            if (toolClass.isNumericOrNot(strTmp) == 1)
                            {
                                value = (float)Convert.ToDouble(strTmp);
                                gVariable.beatPeriodInfo[myBoardIndex].workCurrentHigh = value + value / 10;
                                gVariable.beatPeriodInfo[myBoardIndex].workCurrentLow = value - value / 10;

                                gVariable.beatPeriodInfo[myBoardIndex].idleCurrentHigh = 0.2f;
                                gVariable.beatPeriodInfo[myBoardIndex].idleCurrentLow = 0.1f;
                            }

                            i++;
                            for (j = 0; i < 4 + MAX_NUM_OF_CRAFT_PARAM * 2 && i < strArray.Length; j++)
                            {
                                strTmp = strArray[i++].Trim();
                                if (toolClass.isNumericOrNot(strTmp) == 1)
                                    gVariable.craftList[myBoardIndex].rangeLowerLimit[j] = (float)Convert.ToDouble(strTmp);
                                else
                                    gVariable.craftList[myBoardIndex].rangeLowerLimit[j] = 0;

                                if (i >= strArray.Length)
                                    break;

                                strTmp = strArray[i++].Trim();
                                if (toolClass.isNumericOrNot(strTmp) == 1)
                                    gVariable.craftList[myBoardIndex].rangeUpperLimit[j] = (float)Convert.ToDouble(strTmp);
                                else
                                    gVariable.craftList[myBoardIndex].rangeUpperLimit[j] = 0;
                            }

                            for (; j < MAX_NUM_OF_CRAFT_PARAM; j++)
                            {
                                gVariable.craftList[myBoardIndex].rangeLowerLimit[j] = 0;
                                gVariable.craftList[myBoardIndex].rangeUpperLimit[j] = 0;
                            }
                            break;

                        case COMMUNICATION_TYPE_DEVICE_ANDON_TO_PC:
                            strArray = strInput.Split(';', '&');
                            if (clientFromTouchpad == 1)
                            {
                                //                            alarmTableStructImpl.machineCode = strArray[i++].Trim();
                                //                            alarmTableStructImpl.machineName = strArray[i++].Trim();
                                aa = System.Text.Encoding.Default.GetBytes(strArray[2]);

                                if (aa[0] > 30)
                                    aa[0] = 0;

                                if (aa[0] < gConstText.deviceErrDescList.Length)
                                    deviceFailureIndex = aa[0];
                                else
                                    deviceFailureIndex = 0;
                                alarmTableStructImpl.operatorName = strArray[3].Trim();
                            }
                            else
                            {
                                deviceFailureIndex = Convert.ToInt16(strArray[0]);
                                alarmTableStructImpl.operatorName = strArray[1].Trim();
                            }

                            alarmTableStructImpl.alarmFailureCode = DateTime.Now.ToString("yyMMddHHmmss") + "_" + (gVariable.andonAlarmIndex + 1);
                            alarmTableStructImpl.errorDesc = gVariable.errorCodeList.errorCodeDesc[deviceFailureIndex];
                            alarmTableStructImpl.dispatchCode = gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                            alarmTableStructImpl.machineCode = gVariable.machineCodeArray[myBoardIndex];
                            alarmTableStructImpl.machineName = gVariable.machineNameArray[myBoardIndex];
                            alarmTableStructImpl.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            alarmTableStructImpl.type = gVariable.ALARM_TYPE_DEVICE;
                            alarmTableStructImpl.category = deviceFailureIndex + gVariable.ALARM_CATEGORY_DEVICE_START;
                            alarmTableStructImpl.status = gVariable.ALARM_STATUS_APPLIED;
                            alarmTableStructImpl.startID = gVariable.ALARM_NO_STARTID;
                            alarmTableStructImpl.indexInTable = gVariable.ALARM_NO_INDEX_IN_TABLE;

                            alarmTableStructImpl.workshop = gVariable.allMachineWorkshopForZihua[0];

                            alarmTableStructImpl.mailList = toolClass.getAlarmMailList();

                            alarmIDInTable = mySQLClass.writeAlarmTable(databaseNameThis, gVariable.alarmListTableName, alarmTableStructImpl);
                            toolClass.processNewAlarm(databaseNameThis, alarmIDInTable);
                            gVariable.andonAlarmIndex++;
                            break;
                        case COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_DEAL_TO_PC:
                            dName = gVariable.internalMachineName[myBoardIndex];
                            tName = gVariable.alarmListTableName;
                            now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            strArray = strInput.Split(';', '&');
                            if (clientFromTouchpad == 1)
                            {
                                for (i = 0; i < gVariable.maxDeviceAlarmNum; i++)
                                {
                                    if (strArray[i * 3 + 1] == "1")
                                        deviceStatusAlarm[i] = 2;
                                    else if (strArray[i * 3 + 2] == "1")
                                        deviceStatusAlarm[i] = 3;
                                    else if (strArray[i * 3] == "1")
                                        deviceStatusAlarm[i] = 1;

                                    //last parameter of 0 means we want to get ID value, then change it to a integer, so  
                                    j = Convert.ToInt32(mySQLClass.getAnothercolumnFromDatabaseByOneColumn(dName, tName, "alarmFailureCode", deviceAlarmListTable[i].alarmFailureCode, 0));

                                    now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                    updateStr = null;
                                    if (deviceStatusAlarm[i] == gVariable.ALARM_STATUS_SIGNED)
                                    {
                                        updateStr = "update `" + tName + "` set operatorName = '" + deviceAlarmListTable[i].operatorName + "', time1 = '" + now + "', status = '" + deviceStatusAlarm[i] +
                                        "' where id = '" + j + "'";
                                        //mySQLClass.updateAlarmTable(dName, tName, j, deviceAlarmListTable[i].operatorName, now, null, null, deviceStatusAlarm[i],
                                        //                            gVariable.ALARM_INHISTORY_UNCHANGED, null, null, null);
                                    }
                                    else if (deviceStatusAlarm[i] == gVariable.ALARM_STATUS_COMPLETED || deviceAlarmListTable[i].status == gVariable.ALARM_STATUS_CANCELLED)
                                    {
                                        updateStr = "update `" + tName + "` set completer = '" + deviceAlarmListTable[i].operatorName + "', time2 = '" + now + "', status = '" + deviceStatusAlarm[i] +
                                        "' where id = '" + j + "'";
                                        //mySQLClass.updateAlarmTable(dName, tName, j, null, null, deviceAlarmListTable[i].operatorName, now, deviceStatusAlarm[i],
                                        //                                  gVariable.ALARM_INHISTORY_UNCHANGED, null, null, null);
                                    }
                                    mySQLClass.updateTableItems(dName, updateStr);
                                }
                            }
                            else  //for app alarm status update function
                            {
                                id = mySQLClass.searchForOneRecordFromDatabase(dName, tName, "alarmFailureCode", strArray[0]);
                                if (id <= 0)
                                {
                                    gVariable.infoWriter.WriteLine("For " + dName + tName + "alarm  status update function, deviceFailureNo we got from Android app is not correct!");
                                    break;
                                }
                                status = Convert.ToInt32(strArray[1]);

                                operatorName = strArray[2];
                                if (strArray.Length > 4)
                                    mailList = strArray[3];
                                else
                                    mailList = null;

                                if (strArray.Length > 5)
                                    discuss = strArray[4];
                                else
                                    discuss = null;

                                if (strArray.Length > 6)
                                    solution = strArray[5];
                                else
                                    solution = null;

                                updateStr = null;
                                if (status == gVariable.ALARM_STATUS_SIGNED)
                                {
                                    updateStr = "update `" + tName + "` set operatorName = '" + operatorName + "', time1 = '" + now + "', status = '" + status + "', mailList = '" + mailList +
                                                "', discuss = '" + discuss + "', solution = '" + solution + "' where id = '" + id + "'";
                                    //mySQLClass.updateAlarmTable(dName, tName, id, operatorName, now, null, null, status, gVariable.ALARM_INHISTORY_UNCHANGED, mailList, discuss, solution);
                                }
                                else if (status == gVariable.ALARM_STATUS_COMPLETED || status == gVariable.ALARM_STATUS_CANCELLED)
                                {
                                    updateStr = "update `" + tName + "` set completer = '" + operatorName + "', time2 = '" + now + "', status = '" + status + "', mailList = '" + mailList +
                                                "', discuss = '" + discuss + "', solution = '" + solution + "' where id = '" + id + "'";
                                    //mySQLClass.updateAlarmTable(dName, tName, id, null, null, operatorName, now, status, gVariable.ALARM_INHISTORY_UNCHANGED, mailList, discuss, solution);
                                }
                                mySQLClass.updateTableItems(dName, updateStr);
                            }
                            break;
                        case COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_PC: //new craft
                            strArray = strInput.Split(';', '&');
                            j = 0;
                            for (i = 0; i < gVariable.craftList[myBoardIndex].paramNumber; i++, j++)
                            {
                                strTmp = strArray[j].Trim();
                                if (toolClass.isNumericOrNot(strTmp) == 0)
                                    strTmp = "0";

                                gVariable.craftList[myBoardIndex].paramValue[i] = (float)Convert.ToDouble(strTmp);
                            }
                            break;
                        case COMMUNICATION_TYPE_QUALITY_DATA_TO_PC:
                            strArray = strInput.Split(';', '&');
                            j = 0;
                            flag = 1;

                            if (gVariable.qualityList[myBoardIndex].checkItemNumber > MAX_NUM_OF_QUALITY_DATA)
                                gVariable.qualityList[myBoardIndex].checkItemNumber = MAX_NUM_OF_QUALITY_DATA;

                            for (i = 0; i < gVariable.qualityList[myBoardIndex].checkItemNumber; i++, j++)
                            {
                                strTmp = strArray[j].Trim();
                                if (toolClass.isNumericOrNot(strTmp) == 0)
                                    strTmp = NO_DATA_AVAILABLE;

                                gVariable.qualityList[myBoardIndex].checkResultData[i] = strTmp;

                                value = (float)Convert.ToDouble(strTmp);
                                if (value >= gVariable.qualityList[myBoardIndex].specLowerLimit[i] && value <= gVariable.qualityList[myBoardIndex].specUpperLimit[i])
                                {
                                    gVariable.qualityList[myBoardIndex].checkResult[i] = "Y";
                                }
                                else
                                {
                                    gVariable.qualityList[myBoardIndex].checkResult[i] = "N";
                                    flag = 0;
                                }
                            }

                            if (toolClass.isDigitalNum(strArray[i].Trim()) == 1)   //serial number should not be saved into quality list table, it is inside quality data table
                            {
                                //if (clientFromTouchpad == 0)
                                //    gVariable.qualityList[myBoardIndex].serialNumber = Convert.ToInt32(strArray[i].Trim());
                                //else
                                //    gVariable.qualityList[myBoardIndex].serialNumber = Convert.ToInt32(strArray[MAX_NUM_OF_QUALITY_DATA].Trim());
                            }
                            else
                            {
                                //gVariable.qualityList[myBoardIndex].serialNumber = 0;
                            }

                            if (flag == 1)
                                gVariable.dispatchSheet[myBoardIndex].qualifiedNumber++;
                            else
                                gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber++;

                            gVariable.dispatchSheet[myBoardIndex].outputNumber++;
                            gVariable.dispatchSheet[myBoardIndex].toolUsedTimes++;
                            gVariable.machineStatus[myBoardIndex].toolUsedTimes++;
                            gVariable.machineStatus[myBoardIndex].collectedNumber++;
                            break;

                        case COMMUNICATION_TYPE_DISPATCH_COMPLETED_TO_PC:
                            strArray = strInput.Split(';', '&');
                            if (clientFromTouchpad == 1)
                            {
                                if (toolClass.isDigitalNum(strArray[7]) == 1)
                                    gVariable.dispatchSheet[myBoardIndex].qualifiedNumber = gVariable.dispatchSheet[myBoardIndex].plannedNumber; // Convert.ToInt32(strArray[7].Trim());
                                if (toolClass.isDigitalNum(strArray[8]) == 1)
                                    gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber = Convert.ToInt32(strArray[8].Trim());

                                gVariable.dispatchSheet[myBoardIndex].outputNumber = gVariable.dispatchSheet[myBoardIndex].qualifiedNumber + gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber;

                                gVariable.dispatchSheet[myBoardIndex].reportor = strArray[13].Trim();
                                gVariable.dispatchSheet[myBoardIndex].realFinishTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                gVariable.dispatchSheet[myBoardIndex].qualifiedNumber = Convert.ToInt32(strArray[0]);
                                gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber = Convert.ToInt32(strArray[1]);
                                gVariable.dispatchSheet[myBoardIndex].reportor = strArray[2].Trim();
                                gVariable.dispatchSheet[myBoardIndex].realFinishTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            }

                            gVariable.machineStatus[myBoardIndex].collectedNumber = gVariable.dispatchSheet[myBoardIndex].qualifiedNumber + gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber;
                            gVariable.dispatchSheet[myBoardIndex].toolUsedTimes = toolUsedTimesNow + gVariable.machineStatus[myBoardIndex].collectedNumber;
                            gVariable.machineStatus[myBoardIndex].toolUsedTimes = toolUsedTimesNow + gVariable.machineStatus[myBoardIndex].collectedNumber;
                            toolUsedTimesNow = gVariable.dispatchSheet[myBoardIndex].toolUsedTimes;
                            break;

                        case COMMUNICATION_TYPE_MACHINE_STATUS_TO_PC: //Shiyan machine status when one piece completed
                            Console.WriteLine("status in board");
                            i = 0;
                            strArray = strInput.Split(';', '&');
                            i++;
                            gVariable.machineStatus[myBoardIndex].machineName = strArray[i++];
                            timeStampNow = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now);
                            gVariable.machineStatus[myBoardIndex].totalWorkingTime = (int)((timeStampNow - gVariable.worldStartTime).TotalSeconds - gVariable.machineStartTimeStamp[myBoardIndex]) / 2;
                            gVariable.machineStatus[myBoardIndex].workingTime = workingTimePoints * 4 / 10;
                            if (gVariable.machineStatus[myBoardIndex].totalWorkingTime < gVariable.machineStatus[myBoardIndex].workingTime)
                                gVariable.machineStatus[myBoardIndex].workingTime = gVariable.machineStatus[myBoardIndex].totalWorkingTime;
                            gVariable.machineStatus[myBoardIndex].powerConsumed = powerConsumed / 1000;
                            gVariable.machineStatus[myBoardIndex].standbyTime = gVariable.machineStatus[myBoardIndex].totalWorkingTime - gVariable.machineStatus[myBoardIndex].workingTime;
                            gVariable.machineStatus[myBoardIndex].power = currentPower;
                            gVariable.machineStatus[myBoardIndex].collectedNumber = gVariable.dispatchSheet[myBoardIndex].unqualifiedNumber + gVariable.dispatchSheet[myBoardIndex].qualifiedNumber;
                            break;

                        case COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_TO_REQ:
                            i = 0;
                            strArray = strInput.Split(';', '&');
                            alarmTableStructImpl.alarmFailureCode = strArray[i++];
                            alarmTableStructImpl.status = Convert.ToInt32(strArray[i++]);
                            break;

                        case COMMUNICATION_TYPE_MATERIAL_ANDON_TO_PC:
                            i = 0;
                            strArray = strInput.Split(';', '&');

                            alarmTableStructImpl.errorDesc = null;
                            if (clientFromTouchpad == 1)
                            {
                                //material list is empty, no material alarm is allowed
                                if (gVariable.materialList[myBoardIndex].dispatchCode == null || gVariable.materialList[myBoardIndex].dispatchCode.Length < 2)
                                    break;

                                if (strArray.Length < gVariable.maxMaterialTypeNum)
                                {
                                    i = strArray.Length;
                                    alarmTableStructImpl.operatorName = gVariable.dispatchSheet[myBoardIndex].operatorName;
                                }
                                else
                                {
                                    i = gVariable.maxMaterialTypeNum;
                                    alarmTableStructImpl.operatorName = strArray[8].Trim();
                                }

                                alarmTableStructImpl.errorDesc = "缺料：";
                                for (j = 0; j < i; j++)
                                {
                                    if (strArray[j] == "1")  //this is an material interrupt
                                    {
                                        alarmTableStructImpl.errorDesc += gVariable.materialList[myBoardIndex].materialCode[j] + "; ";
                                    }
                                }
                            }
                            else
                            {
                                i = strArray.Length / 2;  //number of lack-of-item 

                                for (i = 0; i < strArray.Length - 1; i += 2)
                                {
                                    alarmTableStructImpl.errorDesc += strArray[i] + ": 缺";
                                    alarmTableStructImpl.errorDesc += strArray[i + 1] + "; ";
                                }

                                alarmTableStructImpl.operatorName = "admin";
                            }
                            alarmTableStructImpl.alarmFailureCode = DateTime.Now.ToString("yyMMddHHmmss") + "_" + (gVariable.andonAlarmIndex + 1);
                            alarmTableStructImpl.dispatchCode = gVariable.dispatchSheet[myBoardIndex].dispatchCode;
                            alarmTableStructImpl.machineCode = gVariable.machineCodeArray[myBoardIndex];
                            alarmTableStructImpl.machineName = gVariable.machineNameArray[myBoardIndex];
                            alarmTableStructImpl.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            alarmTableStructImpl.type = gVariable.ALARM_TYPE_MATERIAL;
                            alarmTableStructImpl.category = gVariable.ALARM_CATEGORY_MATERIAL;
                            alarmTableStructImpl.status = gVariable.ALARM_STATUS_APPLIED;
                            alarmTableStructImpl.startID = gVariable.ALARM_NO_STARTID;
                            alarmTableStructImpl.indexInTable = gVariable.ALARM_NO_INDEX_IN_TABLE;

                            alarmIDInTable = mySQLClass.writeAlarmTable(databaseNameThis, gVariable.alarmListTableName, alarmTableStructImpl);
                            //this is a material alarm, we put it into last alarm array, so when we start a dispatch later, we can close this material alarm because material shold already be available
                            gVariable.IDForLastAlarmByMachine[myBoardIndex] = alarmIDInTable;
                            toolClass.processNewAlarm(databaseNameThis, alarmIDInTable);
                            gVariable.andonAlarmIndex++;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.Write("error change array to variable" + ex);
                    Console.Write("putPDATouchpadArrayToVariables() fail, tableIndex is " + tableIndex + " 信息失败！");
                    Console.WriteLine(ex.ToString());
                }
            }


            public void writeDataTolog(string sData)
            {
                try
                {
                    if (gVariable.debugMode == 2)
                    {
                        gVariable.dataLogWriter[myBoardIndex].WriteLine(sData);
                    }
                    else if (gVariable.debugMode == 3)
                    {
                        Console.WriteLine(sData);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }


            //implement ADC setting data from ADC setting screen
            public void getSettingCurveData()
            {
                int i;

                //now try to put ADC settings to curve 
                try
                {
                    //input default craft data
                    gVariable.craftList[myBoardIndex].paramNumber = MAX_NUM_OF_CRAFT_PARAM; //gVariable.ADCChannelInfo[myBoardIndex].channelNum;
                    for (i = 0; i < MAX_NUM_OF_CRAFT_PARAM; i++)
                    {
                        if (gVariable.ADCChannelInfo[myBoardIndex].channelEnabled[i] == 1)
                        {
                            gVariable.craftList[myBoardIndex].paramName[i] = gVariable.ADCChannelInfo[myBoardIndex].channelTitle[i];
                            gVariable.craftList[myBoardIndex].paramUpperLimit[i] = gVariable.ADCChannelInfo[myBoardIndex].upperLimit[i];
                            gVariable.craftList[myBoardIndex].paramLowerLimit[i] = gVariable.ADCChannelInfo[myBoardIndex].lowerLimit[i];
                            gVariable.craftList[myBoardIndex].paramUnit[i] = gVariable.ADCChannelInfo[myBoardIndex].channelUnit[i];
                            gVariable.craftList[myBoardIndex].rangeUpperLimit[i] = gVariable.ADCChannelInfo[myBoardIndex].upperRange[i];
                            gVariable.craftList[myBoardIndex].rangeLowerLimit[i] = gVariable.ADCChannelInfo[myBoardIndex].lowerRange[i];
                        }
                        else
                        {
                            gVariable.craftList[myBoardIndex].paramName[i] = "未定义";
                            gVariable.craftList[myBoardIndex].paramLowerLimit[i] = 0;
                            gVariable.craftList[myBoardIndex].paramUpperLimit[i] = 0;
                            gVariable.craftList[myBoardIndex].paramUnit[i] = "未定义";
                            gVariable.craftList[myBoardIndex].rangeUpperLimit[i] = ADC_RANG_NOT_DEFINED;
                            gVariable.craftList[myBoardIndex].rangeLowerLimit[i] = ADC_RANG_NOT_DEFINED;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write("getDefaultCurveData() error: " + ex);
                }
            }


            //function: read information related to this board in infoArray, this array will be send to data collect board
            //          directly to tell the board what kind of action it needs to do
            //          and at the same time, we will save worker ID, product ID, port type into parameters, so 
            //          FileAppendFucntion() can check these parameters when it get new data from board and save them in 
            //          correct database location
            //input:    doWeWantToChangedispatchUI  1 means we need to consider both dispatchUI and curve
            //                                 0 means we only need to change curve data, dispatchUI will keep the same
            private void readBoardInfoFromMachineDataStruct(int doWeWantToChangedispatchUI)
            {
                int i, index;
                string dispatchCode;

                for (i = 0; i < MAX_NUM_OF_CRAFT_PARAM; i++)
                    craftDataForCurveIndex[i] = -1;

                for (i = 0; i < MAX_NUM_OF_VOLCUR; i++)
                    volcurDataForCurveIndex[i] = -1;

                for (i = 0; i < MAX_NUM_OF_QUALITY_DATA; i++)
                    qualityDataForCurveIndex[i] = -1;

                for (i = 0; i < MAX_NUM_OF_BEAT; i++)
                    beatDataForCurveIndex[i] = -1;

                if (doWeWantToChangedispatchUI == 1)
                    getdispatchUIInfoIngVariable(databaseNameThis);

                dispatchCode = toolClass.getCurveInfoIngVariable(databaseNameThis, myBoardIndex, gVariable.CURRENT_READING);

                craftDataTableName = dispatchCode + gVariable.craftTableNameAppendex;
                volcurDataTableName = dispatchCode + gVariable.volcurTableNameAppendex;
                qualityDataTableName = dispatchCode + gVariable.qualityTableNameAppendex;
                beatDataTableName = dispatchCode + gVariable.beatTableNameAppendex;

                if (gVariable.machineCurrentStatus[myBoardIndex] == gVariable.MACHINE_STATUS_DISPATCH_COMPLETED)
                {
                    index = 0;

                    for (i = 0; i < toolClass.dummy_craftNum; i++)
                        craftDataForCurveIndex[i] = index++;

                    for (i = 0; i < toolClass.dummy_volcurNum; i++)
                        volcurDataForCurveIndex[i] = index++;

                    for (i = 0; i < toolClass.dummy_qualityNum; i++)
                        qualityDataForCurveIndex[i] = index++;

                    for (i = 0; i < toolClass.dummy_beatNum; i++)
                        beatDataForCurveIndex[i] = index++;
                }
            }

            public void getdispatchUIInfoIngVariable(string databaseName)
            {
                int i;
                int curveIndex;
                int dispatchAlreadyExists;
                string tableName;

                dispatchAlreadyExists = 0;
                gVariable.dispatchSheet[myBoardIndex].workshop = gConstText.strOfUnkown;

                try
                {
                    curveIndex = 0;

                    //if there is no dispatch, we use dummy dispatch, so we need first write this dummy dispatch into database
                    if (gVariable.dispatchSheet[myBoardIndex].dispatchCode == gVariable.dummyDispatchTableName)
                    {
                        //first check whether this dispatch is already inside dispatch list, if yes, this functon will do nothing
                        dispatchAlreadyExists = mySQLClass.writeDataToDispatchListTable(databaseNameThis, gVariable.dispatchListTableName, gVariable.dispatchSheet[myBoardIndex]);
                    }
                    else
                    {
                        if (gVariable.dispatchSheet[myBoardIndex].status == gVariable.MACHINE_STATUS_DISPATCH_PUBLISHED)
                        {
                            gVariable.dispatchSheet[myBoardIndex].status = gVariable.MACHINE_STATUS_DISPATCH_APPLIED;
                            //-1 means we will only update status for this dispatch 
                            mySQLClass.updateDispatchTable(databaseNameThis, gVariable.dispatchListTableName, -1, gVariable.MACHINE_STATUS_DISPATCH_APPLIED, gVariable.dispatchSheet[myBoardIndex].dispatchCode);
                            //record this info in global database
                            mySQLClass.updateDispatchTable(gVariable.globalDatabaseName, gVariable.globalDispatchTableName, -1, gVariable.MACHINE_STATUS_DISPATCH_STARTED, gVariable.dispatchSheet[myBoardIndex].dispatchCode);

                            //first check whether this dispatch is already inside dispatch list, if yes, this functon will do nothing
                            dispatchAlreadyExists = 0;
                        }
                        else
                        {
                            dispatchAlreadyExists = 1;
                        }
                    }

                    for (i = 0; i < gVariable.craftList[myBoardIndex].paramNumber; i++)
                    {
                        //this dispatch does not exist and 
                        if (dispatchAlreadyExists == 0 && i == 0)
                        {
                            //generate data table for craft parameter
                            tableName = gVariable.dispatchSheet[myBoardIndex].dispatchCode + gVariable.craftTableNameAppendex;
                            mySQLClass.createDataTable(databaseNameThis, tableName, mySQLClass.DATA_TYPE_CRAFT_DATA);
                            mySQLClass.writeCraftDescToCraftList(databaseNameThis, gVariable.craftListTableName, gVariable.craftList[myBoardIndex], gVariable.dispatchSheet[myBoardIndex].dispatchCode);
                        }
                        craftDataForCurveIndex[i] = curveIndex++; //this ADC channel is working, if index is 4 and curveIndex is 3, that means channel 4 is used as curve 3 
                    }

                    //for every machine, we have voltage/current module
                    for (i = 0; i < MAX_NUM_OF_VOLCUR; i++)
                    {
                        if (dispatchAlreadyExists == 0 && i == 0) //this table is just generated, we need to write all contents, if already exists, we don't modify it
                        {
                            //create voltage/current data table
                            tableName = gVariable.dispatchSheet[myBoardIndex].dispatchCode + gVariable.volcurTableNameAppendex;
                            mySQLClass.createDataTable(databaseNameThis, tableName, mySQLClass.DATA_TYPE_VOLCUR_DATA);
                        }
                        volcurDataForCurveIndex[i] = curveIndex++;
                    }

                    for (i = 0; i < gVariable.qualityList[myBoardIndex].checkItemNumber; i++)
                    {
                        if (dispatchAlreadyExists == 0 && i == 0)
                        {
                            //create quality data table
                            tableName = gVariable.dispatchSheet[myBoardIndex].dispatchCode + gVariable.qualityTableNameAppendex;
                            mySQLClass.createDataTable(databaseNameThis, tableName, mySQLClass.DATA_TYPE_QUALITY_DATA);
                            mySQLClass.writeQualityDescToQualityList(databaseNameThis, gVariable.qualityListTableName, gVariable.qualityList[myBoardIndex], gVariable.dispatchSheet[myBoardIndex].dispatchCode);
                        }
                        qualityDataForCurveIndex[i] = curveIndex++;  //gVariable.qualityList[myBoardIndex] number i is curve index curveIndex 
                    }

                    if (dispatchAlreadyExists == 0)
                    {
                        //create beat data table 
                        tableName = gVariable.dispatchSheet[myBoardIndex].dispatchCode + gVariable.beatTableNameAppendex;
                        mySQLClass.createDataTable(databaseNameThis, tableName, mySQLClass.DATA_TYPE_BEAT_DATA);
                    }
                    beatDataForCurveIndex[0] = curveIndex;

                    gVariable.beatDataForCurveIndex = curveIndex;  //we record this index in a gobal variable so when we want to draw a beat curve separately, we know the index in curve table
                    //end of beat data

                    if (dispatchAlreadyExists == 0)  //material already exist, don't try to add more material any more
                    {
                        if (gVariable.machineCurrentStatus[myBoardIndex] >= gVariable.MACHINE_STATUS_DISPATCH_APPLIED)
                        {
                            mySQLClass.writeDataToMaterialListTable(databaseNameThis, gVariable.materialListTableName, gVariable.materialList[myBoardIndex]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write("getdispatchUIInfoIngVariable() board info reading error" + ex);
                }
            }


            public void useFixedBeatSetting(int index)
            {
                gVariable.beatPeriodInfo[index].idleCurrentLow = 0;
                gVariable.beatPeriodInfo[index].idleCurrentHigh = 0.5f;
                gVariable.beatPeriodInfo[index].workCurrentLow = 1.0f;
                gVariable.beatPeriodInfo[index].workCurrentHigh = 2.0f;

                gVariable.beatPeriodInfo[index].gapValue = 30;
                gVariable.beatPeriodInfo[index].peakValue = 3;
            }

            public bool getSettingDataByIndex(int index)
            {
                int i;
                int num;
                int MAX_COLUMN_NUM = 6;

                char[] buf = new char[MAX_COLUMN_NUM * gVariable.maxMachineNum * 4 * 2];
                byte[] bbuf = new byte[MAX_COLUMN_NUM * 4];
                StreamReader streamReader;

                try
                {
                    lock (settingFileLocker)
                    {
                        streamReader = new StreamReader("..\\..\\init\\internalData.ini");
                        if (streamReader == null)
                            return false;

                        if (index != 0)
                        {
                            num = streamReader.Read(buf, 0, MAX_COLUMN_NUM * 4 * index);
                            if (num != MAX_COLUMN_NUM * 4 * index)
                            {

                                Console.WriteLine("read data failed 111!");
                                streamReader.Close();
                                return false;
                            }
                        }

                        num = streamReader.Read(buf, 0, MAX_COLUMN_NUM * 4);
                        if (num != MAX_COLUMN_NUM * 4)
                        {
                            Console.WriteLine("read data failed 222!");
                            streamReader.Close();
                            return false;
                        }

                        for (i = 0; i < MAX_COLUMN_NUM * 4; i++)
                        {
                            if (buf[i] == 'A')
                                bbuf[i] = (byte)'0';
                            else
                                bbuf[i] = (byte)(buf[i] - 20);
                        }

                        gVariable.beatPeriodInfo[index].idleCurrentLow = 0;
                        gVariable.beatPeriodInfo[index].idleCurrentHigh = (float)Convert.ToDouble(System.Text.Encoding.ASCII.GetString(bbuf, 4, 4));
                        gVariable.beatPeriodInfo[index].workCurrentLow = (float)Convert.ToDouble(System.Text.Encoding.ASCII.GetString(bbuf, 8, 4));
                        gVariable.beatPeriodInfo[index].workCurrentHigh = (float)Convert.ToDouble(System.Text.Encoding.ASCII.GetString(bbuf, 12, 4));

                        gVariable.beatPeriodInfo[index].gapValue = (int)Convert.ToDouble(System.Text.Encoding.ASCII.GetString(bbuf, 16, 4));
                        gVariable.beatPeriodInfo[index].peakValue = (int)Convert.ToDouble(System.Text.Encoding.ASCII.GetString(bbuf, 20, 4));

                        streamReader.Close();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    index++;
                    Console.WriteLine("board " + index + " get beat setting data failed!" + ex);
                    return false;
                }

            }


            //
            private bool checkWhetherToContinue()
            {
                if (myBoardIndex < 0 || myBoardIndex > gVariable.maxMachineNum + 1)
                    return false;

                gVariable.today[myBoardIndex] = DateTime.Now.Date.ToString("yyyy-MM-dd");
                if (gVariable.today[myBoardIndex] != gVariable.today_old[myBoardIndex])
                {
                    if (DateTime.Now.Hour >= 5 && DateTime.Now.Hour < 7 && gVariable.machineCurrentStatus[myBoardIndex] <= gVariable.MACHINE_STATUS_DISPATCH_COMPLETED)
                    {
                        gVariable.today_old[myBoardIndex] = gVariable.today[myBoardIndex];

                        clientSocketInServer.Close();

                        return true;
                    }
                }

                return false;
            }

            //when the operator appplying for new dispatch from touch pad, we clear all data related to old dispatch
            private void clearMachineStatus()
            {
                gVariable.machineStatus[myBoardIndex].productBeat = 0;
                gVariable.machineStatus[myBoardIndex].powerConsumed = 0;
                gVariable.machineStatus[myBoardIndex].standbyTime = 0;
                gVariable.machineStatus[myBoardIndex].power = 0;
                gVariable.machineStatus[myBoardIndex].collectedNumber = 0;
                gVariable.machineStatus[myBoardIndex].revolution = 0;
                gVariable.machineStatus[myBoardIndex].prepareTime = 0;
                gVariable.machineStatus[myBoardIndex].workingTime = 0;
            }


            private void generateFailedResponse()
            {
                prepareForDataNeedSending(onePacket, COMMUNICATION_TYPE_DISPATCH_DATA_TO_TOUCHPAD, DATA_TYPE_MES_INSTRUCTION);

            }

            public void sendStringToClient(string str, int type)
            {
                int j;
                int len;
                byte[] buf;
                byte[] outPacket = new byte[MAX_PACKET_LEN]; 

                buf = System.Text.Encoding.Default.GetBytes(str);

                for (j = 0; j < buf.Length; j++)
                {
                    outPacket[j + PROTOCOL_DATA_POS] = buf[j];
                }
                len = buf.Length + MIN_PACKET_LEN_MINUS_ONE;
                sendDataToClient(outPacket, len, type);
            }

            public void sendDataToClient(byte[] onePacket, int len, int type)
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

            //before send data to board/app/emulator, we need to fisrt generate a data packet with the standard format
            private void prepareForDataNeedSending(byte[] onePacket, int communicationType, int dataType)
            {
                DateTime now;

                onePacket[0] = (byte)'w';
                onePacket[1] = (byte)'I';
                onePacket[2] = (byte)'F';
                onePacket[3] = (byte)'i';

                onePacket[PROTOCOL_COMMUNICATION_TYPE_POS] = (byte)communicationType;

                now = DateTime.Now;
                onePacket[PROTOCOL_TIME_POS + 0] = (byte)(now.Year % 100 / 10 + '0');
                onePacket[PROTOCOL_TIME_POS + 1] = (byte)(now.Year % 100 % 10 + '0');
                onePacket[PROTOCOL_TIME_POS + 2] = (byte)(now.Month / 10 + '0');
                onePacket[PROTOCOL_TIME_POS + 3] = (byte)(now.Month % 10 + '0');
                onePacket[PROTOCOL_TIME_POS + 4] = (byte)(now.Day / 10 + '0');
                onePacket[PROTOCOL_TIME_POS + 5] = (byte)(now.Day % 10 + '0');
                onePacket[PROTOCOL_TIME_POS + 6] = (byte)(now.Hour / 10 + '0');
                onePacket[PROTOCOL_TIME_POS + 7] = (byte)(now.Hour % 10 + '0');
                onePacket[PROTOCOL_TIME_POS + 8] = (byte)(now.Minute / 10 + '0');
                onePacket[PROTOCOL_TIME_POS + 9] = (byte)(now.Minute % 10 + '0');
                onePacket[PROTOCOL_TIME_POS + 10] = (byte)(now.Second / 10 + '0');
                onePacket[PROTOCOL_TIME_POS + 11] = (byte)(now.Second % 10 + '0');

                onePacket[PROTOCOL_PACKET_INDEX_POS + 0] = (byte)(dataPacketIndex);
                onePacket[PROTOCOL_PACKET_INDEX_POS + 1] = (byte)(dataPacketIndex >> 8);
                onePacket[PROTOCOL_PACKET_INDEX_POS + 2] = (byte)(dataPacketIndex >> 16);
                onePacket[PROTOCOL_PACKET_INDEX_POS + 3] = (byte)(dataPacketIndex >> 24);

                onePacket[PROTOCOL_RESERVED_DATA_POS + 0] = 0;
                onePacket[PROTOCOL_RESERVED_DATA_POS + 1] = 0;
                onePacket[PROTOCOL_RESERVED_DATA_POS + 2] = 0;
                onePacket[PROTOCOL_RESERVED_DATA_POS + 3] = 0;

                //this index will only increase when a new instruction is to be sent to board/app
                dataPacketIndex++;
                onePacket[PROTOCOL_DATA_TYPE_POS] = (byte)dataType;
            }

        }
    }
}