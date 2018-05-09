using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using clientFunc;

namespace tcpClient
{
    public partial class Form1 : Form
    {
        //0 means plant 23, 16 bit CRC; 1 plant zihua, 32 bit CRC 
        public const int PLANT_23 = 0;
        public const int PLANT_ZIHUA = 1;
        public static int systemDefine = PLANT_ZIHUA; 

        string databaseHeader;

        int beatStartIndex;

        const int PROTOCOL_HEAD_POS = 0;
        const int PROTOCOL_LEN_POS = 4;
        const int PROTOCOL_COMMUNICATION_TYPE_POS = 6;
        const int PROTOCOL_TIME_POS = 7;
        const int PROTOCOL_PACKET_INDEX_POS = 19;
        const int PROTOCOL_RESERVED_DATA_POS = 23;
        const int PROTOCOL_DATA_TYPE_POS = 27;
        const int PROTOCOL_OLD_DATA_POS = 7;

        int PROTOCOL_DATA_POS = 28;

        //'w', 'I', 'F', 'i', 74, 1, 0x0, 0x65, 
        const int PROTOCOL_OLD_HEADER_LEN = 7;   //before data
        const int PROTOCOL_HEADER_LEN = 28;   //before data
        const int PROTOCOL_BOARDID_LEN = 4;
        const int PROTOCOL_FILENAME_LEN = 2;
        const int PROTOCOL_DATETIME_LEN = 12;
        const int PROTOCOL_DATAINDEX_LEN = 4;

        int PROTOCOL_CRC_LEN = 4;

        //a data packet without read data, so it contains only data packet header and CRC
        const int MIN_PACKET_LEN_MINUS_ONE = 32;   //header(4) + len(2) + communicationtype(1) + time(12) + index(4) + reserved(4) + type(1) + CRC(4)
        const int MIN_PACKET_LEN_PURE_FRAME = 32;   //header(4) + len(2) + communicationtype(1) + time(12) + index(4) + reserved(4) + type(1) + CRC(4)
    
        //a data packet with minimum length of real data (real data length is 1)
        const int MIN_PACKET_LEN = 33;  //header(4) + len(2) + communicationtype(1) + time(12) + index(4) + reserved(4) + type(1) + data(1) + CRC(4)

        const int MAX_PACKET_LEN = 4000;  //max length we can send to server in one packet

        const int WAIT_BETWEEN_SEND_RECEIVE = 200;

        public const int WM_CLOSE = 0x0010;

        public const int ID_OTHERTHAN_TOUCHPAD = 0x10000000;  //board ID less than that is a real board, otherwise, it is an app from mobile device or PC emulator 

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
        const int COMMUNICATION_TYPE_ERROR_LIST_TO_TOUCHPAD = 0x93;     //故障列表下发, this function is emplemented at faces.cs
        const int COMMUNICATION_TYPE_MATERIAL_DATA_REQ = 0x22;  //物料单请求
        const int COMMUNICATION_TYPE_MATERIAL_INQUIRY_TO_TOUCHPAD = 0x94;  //物料单下发
        const int COMMUNICATION_TYPE_MATERIAL_ANDON_TO_PC = 0x15;  //物料预警上传
        const int COMMUNICATION_TYPE_CRAFT_PARAMETER_REQUEST = 0x16;  //工艺参数请求
        const int COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_TOUCHPAD = 0x96;  //工艺参数下发
        const int COMMUNICATION_TYPE_CRAFT_PARAMETER_TO_PC = 0x17;  //工艺参数上传
        const int COMMUNICATION_TYPE_QUALITY_DATA_REQUEST = 0x20;  //质量数据请求
        const int COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD = 0x98;  //质量数据下发
        const int COMMUNICATION_TYPE_QUALITY_DATA_TO_PC = 0x19;  //质量数据上传
        const int COMMUNICATION_TYPE_MACHINE_STATUS_TO_PC = 0x1A;  //设备运行状态上传
        const int COMMUNICATION_TYPE_MACHINE_STATUS_TO_TOUCHPAD = 0x1D; //设备安灯列表下发
        const int COMMUNICATION_TYPE_DEVICE_ANDON_TO_PC = 0x1B;  //设备安灯报警上传
        const int COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_TO_TOUCHPAD = 0x9c;  //设备安灯报警查询结果下发
        const int COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_TO_REQ = 0x21; //设备安灯报警查询
        const int COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_DEAL_TO_PC = 0x26;  //安灯报警改变状态(包括各类安灯的签到，解决，取消)
        const int COMMUNICATION_TYPE_STAFF_INQUIRY_TO_TOUCHPAD = 0x9D;  //车间人员查询, this function is emplemented at faces.cs
        const int COMMUNICATION_TYPE_SETTING_TO_PC = 0x1E;  //接口功能设置上传

        const int COMMUNICATION_TYPE_ADJEST_TIME_TO_EDS = 0x1F;  //调整时间上传
        const int COMMUNICATION_TYPE_CYCLE_TIME_TO_PC = 0x9E;  //生产节拍时间上传

        //communication between PC host and label printing SW
        private const int COMMUNICATION_TYPE_HANDSHAKE_PRINT_MACHINE_ID = 3;
        private const int COMMUNICATION_TYPE_PRINTING_HEART_BEAT = 0xB3;
        //出入库工序
        private const int COMMUNICATION_TYPE_WAREHOUE_OUT_START = 0xB5;  //printing SW started and ask for material info, server send material info for all feeding machine to printing SW 
        private const int COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE = 0xB6;  //printing machine send barcode info to server whever a stack of material is moved out of the warehouse
        private const int COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE = 0xB7;  //printing machine send barcode info to server whever a stack of material is moved into the warehouse
        //流延工序
        private const int COMMUNICATION_TYPE_CAST_PROCESS_START = 0xB8;  //printing SW started cast process, server need to send dispatch info to printing SW
        private const int COMMUNICATION_TYPE_CAST_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xB9;  //printing SW send large roll info to server
        private const int COMMUNICATION_TYPE_CAST_PROCESS_END = 0xBA;
        //印刷工序
        private const int COMMUNICATION_TYPE_PRINT_PROCESS_START = 0xBB;
        private const int COMMUNICATION_TYPE_PRINT_PROCESS_MATERIAL_BARCODE_UPLOAD = 0xBC;
        private const int COMMUNICATION_TYPE_PRINT_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xBD;
        private const int COMMUNICATION_TYPE_PRINT_PROCESS_END = 0xBE;
        //分切工序
        private const int COMMUNICATION_TYPE_SLIT_PROCESS_START = 0xBF;
        private const int COMMUNICATION_TYPE_SLIT_PROCESS_MATERIAL_BARCODE_UPLOAD = 0xC0;
        private const int COMMUNICATION_TYPE_SLIT_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xC1;
        private const int COMMUNICATION_TYPE_SLIT_PROCESS_PACKAGE_BARCODE_UPLOAD = 0xC2;
        private const int COMMUNICATION_TYPE_SLIT_PROCESS_END = 0xC3;
        //质检工序
        private const int COMMUNICATION_TYPE_INSPECTION_PROCESS_MATERIAL_BARCODE_UPLOAD = 0xC4;
        private const int COMMUNICATION_TYPE_INSPECTION_PROCESS_PRODUCT_BARCODE_UPLOAD = 0xC5;
        //再造料工序
        private const int COMMUNICATION_TYPE_REUSE_PROCESS_BARCODE_UPLOAD = 0xC6;
        //打包工序		
        private const int COMMUNICATION_TYPE_PACKING_PROCESS_PACKAGE_BARCODE_UPLOAD = 0xC7;
        /*const int COMMUNICATION_TYPE_HANDSHAKE_PRINT_MACHINE_ID = 0xC0;  //set machine ID for label printing function
        const int COMMUNICATION_TYPE_WAREHOUE_OUT_START = 0xC1;  //printing SW started and ask for material info, server send material info for all feeding machine to printing SW 
        const int COMMUNICATION_TYPE_WAREHOUSE_OUT_BARCODE = 0xC2;  //printing machine send barcode info to server whever a stack of material is moved out of the warehouse
        const int COMMUNICATION_TYPE_WAREHOUSE_IN_BARCODE = 0xC3;  //printing machine send barcode info to server whever a stack of material is moved into the warehouse
        const int COMMUNICATION_TYPE_CAST_PROCESS_START = 0xC4;  //printing SW started cast process, server need to send dispatch info to printing SW
        const int COMMUNICATION_TYPE_CAST_BARCODE_UPLOAD = 0xC5;  //printing SW send large roll info to server
        const int COMMUNICATION_TYPE_PRINT_PROCESS_START = 0xC6;
        const int COMMUNICATION_TYPE_PRINT_BARCODE_UPLOAD = 0xC7;
        const int COMMUNICATION_TYPE_SLIT_PROCESS_START = 0xC8;
        const int COMMUNICATION_TYPE_SLIT_BARCODE_UPLOAD = 0xC9;
        const int COMMUNICATION_TYPE_INSPECTION_PROCESS_START = 0xCA;
        const int COMMUNICATION_TYPE_INSPECTION_BARCODE_UPLOAD = 0xCB;
        const int COMMUNICATION_TYPE_REUSE_PROCESS_START = 0xCC;
        const int COMMUNICATION_TYPE_REUSE_BARCODE_UPLOAD = 0xCD;
        const int COMMUNICATION_TYPE_PACKING_PROCESS_START = 0xCE;
        const int COMMUNICATION_TYPE_PACKING_BARCODE_UPLOAD = 0xCF;
        const int COMMUNICATION_TYPE_PRINTING_HEART_BEAT = 0xD0;*/
        //end of communication between PC host and label printing SW

        public byte[] data = new byte[1000];
        public int[] adcOverflowFlag = new int[20];
        public int currentOverflowFlag; 

        public static IPAddress HostIP;
        IPEndPoint point;

        int dataPacketIndex;
        int volCurIndex;

        public const int MAXMACHINENUM = 200;
        public const int MAXLABELNUM = 18;
        public const int MaxClientNum = 18;

        //connected machine list label
        public static Label[] labelArray = new Label[MAXLABELNUM];

        public static int selectedComboBoxIndex;  //this index indicates the selection of combo box
        public static int selectedMachineIndex;  //this index indicates the machine ID, starting from 0
        public static int selectedMachineID;  //this index indicates the machine ID, starting from 1
 
        public static int currentCreatedNum;  //
        public static int currentConnectedNum;
        public static int[] createdMachineArray = new int[MAXLABELNUM];
        public static int[] connectedMachineArray = new int[MAXLABELNUM];

        //what kind of communication type we want, as a touchpad or an app
        const int communicateAsTouchpad = 0;
        const int communicateAsApp = 1;
        const int communicateAsPrintingSW = 2;
        public static int[] startCommnuicateAs = new int[MAXMACHINENUM];

        //comunication status includes the following value
        public const int handshakeNot = 0;
        public const int handshakeOK = 1;
        public const int dispatchCompleted = 2;
        public const int dispatchApplied = 3;
        public const int dispatchStarted = 4;
        public static int[] communicationStatusArray = new int[MAXMACHINENUM];

        public static Socket[] socketArray = new Socket[MAXMACHINENUM];

        int beatTestFlag;

        //craft data will not be sent out by communicationFrequency directly, currently the frequency is 4 times lower
        int sendCraftDataThreshold;
        int communicationFrequency;

        //data sending frequency by Hz
        float[] communicationFreqArray = { 0.1f, 0.2f, 0.5f, 1f, 2f, 5f, 10f, 20f, 50f, 100f, 200f, 500f, 1000f };

        string[] printingMachineArray = { "原料出库", "流延设备1", "流延设备2", "流延设备3", "流延设备4", "流延设备5", "印刷设备2", "印刷设备3", "印刷设备4", "分切设备1", "分切设备2", "分切设备3", "分切设备4", "分切设备5", "检验设备", "再造料", "打包" };

        System.Windows.Forms.Timer aTimer;

        byte[] handshake_packet = new byte[100];

        byte[] dummyMachine_packet = new byte[100];

        byte[] data_packet = new byte[100];

        string[] machineCodeArray = new string[200];

        const int REV_LEN = 2000; 
        byte[] receiveByte = new byte[REV_LEN];

        string[] machineCodeZihuaArray =
        {
            "JB-1", "JB-2", "JB-3", "JB-4", "JB-5", "LY-1", "LY-2", "LY-3", "LY-4", "LY-5", 
            "YS-1", "YS-2", "YS-3", "FQ-1", "FQ-2", "FQ-3", "FQ-4", "FQ-5" 
        };

        string[] machineCode23Array =
        {
            "C-166", "C-189", "M-287", "M-168", "B-001", "M-171", "C-167", "C-143", "C-187", "M-166", 
            "C-171", "B-002", "Y-080", "B-003", "M-295", "M-164", "C-182", "C-186", "M-288", "C-164", 
            "Y-070", "Y-109", "Y-094", "C-170", "M-249", "Y-050", "C-175", "M-277", "Y-077", "Y-032", 
            "Y-081", "M-294", "Y-113", "Y-036", "M-051", "Y-091", "Y-071", "Y-027", "Y-108", "Y-093", 
            "M-278", "C-172", "M-057", "M-196", "M-289", "M-009", "Y-082", "M-208", "Y-038", "M-299", 
            "M-240", "Y-112", "C-163", "Y-095", "M-156", "M-286", "Y-083", "Y-034", "C-184", "M-163", 
            "A-001", "Y-126", "M-289", "M-291", "M-290", "A-002", "A-003", "B-004", "B-005", "Y-116", 
            "Y-128", "A-004", "Z-063", "ZH-004-6", "A-005", "A-006", "A-007", "Y-101", "Y-117", "ZH-004-4", 
            "A-008", "Y-064", "A-009", "C-189", "A-010", "M-169", "Y-115", "T-025", "M-283", "C-168", 
            "Y-096", "ZH-004-2", "T-012", "B-006", "Y-124", "T-020", "Y-118", "C-174", "Y-133", "M-241", 
            "M-287", "M-170", "M-180", "Y-125", "Y-134", "Y-065", "Y-129", "Y-078", "Y-088", "Y-066", 
            "C-190", "ZH-004-3", "Y-073", "Y-084", "M-254", "M-165", "M-232", "ZH-004-5", "Y-127", "Y-123", 
            "L-003", "B-007", "M-279", "L-002", "Y-063", "T-023", "Y-074", "Y-097", "Y-135", "Y-121", 
            "Y-104", "M-257", "B-008", "B-009", "B-010", "B-011", "B-012", "B-013", "B-014", "B-015",
            "Q-104", "Q-257", "Q-008", "Q-009", "Q-010", "Q-011", "Q-012", "Q-013", "Q-014", "Q-015",
            "W-104", "W-257", "W-008", "W-009", "W-010", "W-011", "W-012", "W-013", "W-014", "W-015",
            "E-104", "E-257", "E-008", "E-009", "E-010", "E-011", "E-012", "E-013", "E-014", "E-015",
            "R-104", "R-257", "R-008", "R-009", "R-010", "R-011", "R-012", "R-013", "R-014", "R-015",
            "T-104", "T-257", "T-008", "T-009", "T-010", "T-011", "T-012", "T-013", "T-014", "T-015",
            "U-104", "U-257", "U-008", "U-009", "U-010", "U-011", "U-012", "U-013", "U-014", "U-015",
        };

        string[] machineNameArray = new string[200];

        string[] machineNameZihuaArray =
        {
            "铝合金快速熔解炉", "1号低压铸造机", "1号除尘机", "EQRX39加工中心", "EQRX38加工中心", "除气机", "清洗机", "2号除尘机", "2号低压铸造机", "3号除尘机", "插齿机", "预热装置", "在线检测机"
//            "1号流延机", "2号流延机", "3号流延机", "4号流延机", "5号流延机", "1号印刷机", "2号印刷机", "3号印刷机", "1号分切机", "2号分切机", "3号分切机", "4号分切机", "5号分切机" 
        };

        string[] machineName23Array =
        {
            "高精度数控车床", "高精度数控车床", "数控内圆端面磨床", "数控内圆端面磨床", "设备未定", "数控内圆端面磨床", "哈挺精密数控车床", "高精度数控车床", "高精度数控车床", "数控内圆端面磨床", 
            "高速精度数控车床", "设备未定", "磨齿机", " ", "数控内圆磨床", "内圆磨床", "高速精密度数控车床", "高速精密度数数控车", "数控内圆端面磨床", "高精度数控车床", 
            "磨齿机", "磨齿机", "磨齿机", "哈挺精密数控机床", "卧轴圆台平面磨床", "磨齿机", "车磨中心", "内圆磨床", "磨齿机", "磨齿机", 
            "磨齿机", "数控内圆磨床", "磨齿机", "磨齿机", "普通内圆磨床", "磨齿机", "磨齿机", "磨齿机", "磨齿机", "磨齿机", 
            "卧轴圆台平面磨床", "高速精度数控车床", "卧轴圆台平面磨床", "半自动外圆磨床", "数控内圆端面磨床", "内圆磨床", "磨齿机", "万能外圆磨床", "磨齿机", "卧轴圆台平面磨床", 
            "内圆磨床", "磨齿机", "高精度数控车床", "磨齿机", "内圆磨床", "数控内圆端面磨床", "磨齿机", "磨齿机", "高精度数控车床", "内圆磨床", 
            "设备未定", "滚齿机", "数控内圆端面磨床", "数控内圆端面磨床", "数控内圆端面磨床", "设备未定", "设备未定", "设备未定", "设备未定", "磨齿机", 
            "磨齿机", "设备未定", "数控钻铣床", "S数控剃齿机", "设备未定", "设备未定", "设备未定", "数控高效滚齿机", "磨齿机", "数控倒棱机", 
            "设备未定", "数控滚齿机", "设备未定", "哈挺精密数控车床", "设备未定", "数控内圆端面磨床", "磨齿机", "立式加工中心", "卧轴圆台端面磨床", "数控卧式车床", 
            "数控滚齿机", "数控自动滚齿机", "立式加工中心", "设备未定", "数控高效滚齿机", "立式加工中心", "数控高效滚齿机", "车磨中心", "滚齿机", "卧轴圆台端面磨床", 
            "数控内圆端面磨床", "数控内圆端面磨床", "内圆磨床", "数控滚齿机", "数控齿轮倒角机", "数控滚齿机", "磨齿机", "数控径向剃齿机", "数控高效滚齿机", "数控高效滚齿机", 
            "哈挺精密数控车床", "数控自动滚齿机", "数控剃齿机", "数控齿轮倒棱机", "内圆磨床", "数控内圆端面磨床", "卧轴圆台端面磨床", "S数控剃齿机", "磨齿机", "数控椎度齿插齿机", 
            "卧式内拉床", "设备未定", "卧轴圆台端面磨床", "卧式拉床", "数控滚齿机", "立式加工中心", "数控滚齿机", "数控滚齿机", "数控插齿机", "齿轮倒角机", 
            "数控高效滚齿机", "卧轴圆台端面磨床", "设备未定", "设备未定", "设备未定", "设备未定", "设备未定", "设备未定", "设备未定", "设备未定",
            "设备141", "设备142", "设备143", "设备144", "设备145", "设备146", "设备147", "设备148", "设备149", "设备150", 
            "设备151", "设备152", "设备153", "设备154", "设备155", "设备156", "设备157", "设备158", "设备159", "设备160", 
            "设备161", "设备162", "设备163", "设备164", "设备165", "设备166", "设备167", "设备168", "设备169", "设备170", 
            "设备171", "设备172", "设备173", "设备174", "设备175", "设备176", "设备177", "设备178", "设备179", "设备180", 
            "设备181", "设备182", "设备183", "设备184", "设备185", "设备186", "设备187", "设备188", "设备189", "设备190", 
            "设备191", "设备192", "设备193", "设备194", "设备195", "设备196", "设备197", "设备198", "设备199", "设备200", 
        };


        float[] craftUpperRangeLimit = { 60, 60, 60, 1000, 300, 1200, 400, 400 };
        float[] craftLowerRangeLimit = { 0,  0,  0,  800,  200, 1080, 360, 360 };

        //USL = 200, SL = 150, LSL = 100
        //max = 182, min = 115
        //spc data CP = 1.226, cpk = 1.212, pp = 1.265, ppk = 1.250, cpu = 1.240, cpl = 1.212,
        //Xbar chart: UCL = 151.173, CL = 149.433, LCL = 147.649
        //S chart: UCL = 23.208, CL = 13.179, LCL = 3.150
        // not used now, just for 
        public static int[] SPCCollectedData_standard = 
        {
            152,147,146,146,147,136,148,139,115, 
            157,150,150,148,152,151,133,153,145, 
            156,150,121,136,148,134,146,139,147,
            132,129,130,157,132,155,134,135,136, 
            137,138,139,140,141,142,143,144,146, 
            147,140,121,149,150,151,152,153,154,
            155,156,157,158,159,160,161,162,163, 
            164,185,176,177,138,139,178,179,134, 
            152,135,143,139,151,178,179,162,171,
            154,170,171,164,169,135,149,150,145, 
            143,156,152,149,149,151,152,150,162, 
            160,161,147,143,167,145,138,142,139,
            140,120,141,133,123,124,125,140,127, 
            138,129,130,138,140,133,134,135,136, 
            137,138,139,140,141,142,143,144,145,
            146,147,148,149,150,151,152,153,154, 
            155,156,157,158,159,160,161,153,163, 
            164,165,156,167,168,159,170,171,154,
            173,174,153,176,134,178,199,153,146, 
            150,138,154,140,156,159,143,154,161, 
            161,162,163,164,165,156,167,160,159,
            170,138,172,173,174,175,146,133,146, 
            157,163,137,152,147,150,135,146,165, 
            146,143,170,162,148,150,150,161,138,
            130,164,127,145,150,147,139,136,144, 
            145,146,143,147,138,159,165,164,124, 
            182,126,127,128,129,130,150,154,158,
            150,148,149,161,138,149,157,151,142, 
            137,154,139,147,144,161,145,120,162, 
            164,148,168,157,126,158,128,164,130,
        };

        public static int[] SPCCollectedData = 
        {
            152,147,146,146,147,136,148,139,115, 
            157,150,150,148,152,151,133,153,145, 
            156,150,161,136,148,134,146,139,147,
            132,142,140,157,132,155,134,135,136, 
            137,138,139,140,141,142,143,144,146, 
            147,140,141,149,150,151,152,153,154,
            155,156,157,138,159,160,141,162,143, 
            164,145,156,147,138,139,171,159,134, 
            152,135,143,139,151,158,169,162,141,
            154,150,151,164,169,145,149,150,145, 
            143,156,152,149,149,151,152,150,162, 
            160,161,147,143,167,145,138,142,139,
            140,160,141,133,153,154,145,140,127, 
            148,159,130,138,140,163,144,135,136, 
            137,158,159,140,141,142,143,144,145,
            146,147,148,149,150,151,152,153,154, 
            155,146,157,138,159,160,161,153,143, 
            164,165,156,167,138,159,140,151,154,
            143,154,153,146,134,143,139,153,146, 
            150,138,154,140,156,159,143,154,161, 
            161,162,143,164,145,156,137,160,159,
            170,138,152,143,154,165,146,143,146, 
            157,163,137,152,147,150,135,146,165, 
            146,143,160,162,148,161,150,161,158,
            130,154,140,145,150,147,159,136,154, 
            145,146,153,147,138,159,165,164,154, 
            162,146,137,148,149,160,150,154,148,
            150,148,149,161,138,149,157,151,142, 
            137,154,139,147,144,161,145,140,162, 
            159,148,165,157,134,158,138,164,130,
        };

        // ALARM_CATEGORY_SPC_DATA_OVERFLOW = 3;   //data out of limit
        public static int[] SPCCollectedData_spc_overflow = 
        {
            152,147,146,146,147,136,148,139,115, 
            157,150,150,148,152,151,133,153,145, 
            156,150,121,136,148,134,146,139,147,
            132,129,130,157,132,155,134,135,136, 
            137,138,139,140,141,142,143,144,146, 
            147,140,121,149,150,151,152,153,154,
            155,156,157,158,159,160,161,162,163, 
            164,185,176,177,138,139,178,179,134, 
            152,135,143,139,151,178,179,162,171,
            154,170,171,164,169,135,149,150,145, 
            143,156,152,149,149,151,152,150,162, 
            160,161,147,143,167,145,138,142,139,
            140,120,141,133,123,124,125,140,127, 
            138,129,130,138,140,133,134,135,136, 
            137,138,139,140,141,142,143,144,145,
            146,147,148,149,150,151,152,153,154, 
            155,156,157,158,159,160,161,153,163, 
            164,165,156,167,168,159,170,171,154,
            173,174,153,176,134,178,199,153,146, 
            150,138,154,140,156,159,143,154,161, 
            161,162,163,164,165,156,167,160,159,
            170,138,172,173,194,155,146,133,146, 
            157,163,137,152,147,150,135,146,165, 
            146,143,170,162,148,150,150,161,138,
            130,164,127,145,150,147,139,136,144, 
            145,146,143,147,138,159,165,164,124, 
            182,126,127,128,129,130,150,154,158,
            150,148,149,161,138,149,157,151,142, 
            137,154,139,147,144,161,145,120,162, 
            164,148,168,157,126,158,128,164,130,
        };

        //ALARM_CATEGORY_SPC_DATA_SAME_SIDE = 4;   //9 points at same side of the chart
        public static int[] SPCCollectedData_one_side1 = 
        {
            152,135,143,139,151,178,179,162,171,
            160,161,147,143,167,145,138,142,139,
            137,138,139,140,141,142,143,144,145,
            152,147,146,146,147,136,148,139,115, 
            157,150,150,148,152,151,133,153,145, 
            156,150,121,136,148,134,146,139,147,
            140,175,141,133,143,134,125,160,127, 
            138,169,130,138,140,133,134,135,136, 
            132,173,130,157,132,155,134,135,136, 
            137,138,139,140,141,142,143,144,146, 
            154,170,171,164,169,135,149,150,145, 
            143,156,152,149,149,151,152,150,162, 
            155,156,157,158,159,160,161,162,163, 
            147,140,121,149,150,151,152,153,154,
            164,129,176,177,138,139,178,179,134, 
            146,147,148,149,150,151,152,153,154, 
            155,156,157,158,159,160,161,153,163, 
            164,165,156,167,168,159,170,131,154,
            173,129,153,176,134,178,199,153,146, 
            150,138,154,140,156,159,143,154,161, 
            161,142,163,164,165,136,167,160,159,
            170,138,172,173,174,175,146,133,146, 
            157,163,137,152,147,150,135,146,165, 
            146,143,170,162,148,150,150,161,138,
            130,164,127,145,150,147,139,136,144, 
            145,146,143,147,138,159,165,164,124, 
            182,126,127,128,129,130,150,154,158,
            150,148,149,161,138,149,157,151,142, 
            137,154,139,147,144,161,145,120,162, 
            164,148,168,157,126,158,128,164,130,
        };

        //ALARM_CATEGORY_SPC_DATA_ONE_TREND = 5;   //7 points lies in the same trend
        public static int[] SPCCollectedData_one_trend1 = 
        {
            152,135,143,139,151,178,179,162,171,
            137,144,139,140,141,138,142,143,145,
            137,138,139,140,141,142,143,144,146, 
            156,150,121,136,148,134,146,139,147,
            140,175,141,133,143,134,125,160,127, 
            132,173,130,157,132,155,134,135,136, 
            157,150,150,148,152,151,133,153,145, 
            160,161,147,143,167,145,138,142,139,
            154,170,171,164,169,135,149,150,145, 
            173,129,153,176,134,178,199,153,146, 
            143,156,152,149,149,151,152,150,162, 
            152,147,146,146,147,136,148,139,115, 
            138,169,130,138,140,133,134,135,136, 
            155,156,157,158,159,160,161,162,163, 
            147,140,121,149,150,151,152,153,154,
            164,129,176,177,138,139,178,179,134, 
            146,147,148,149,150,151,152,153,154, 
            155,156,157,158,159,160,161,153,163, 
            164,165,156,167,168,159,170,131,154,
            173,129,153,176,134,178,199,153,146, 
            152,147,146,146,147,136,148,139,115, 
            138,169,130,138,140,133,134,135,136, 
            150,138,154,140,156,159,143,154,161, 
            161,142,163,164,165,136,167,160,159,
            170,138,172,173,174,175,146,133,146, 
            157,163,137,152,147,150,135,146,165, 
            146,143,170,162,148,150,150,161,138,
            130,164,127,145,150,147,139,136,144, 
            145,146,143,147,138,159,165,164,124, 
            182,126,127,128,129,130,150,154,158,
            150,148,149,161,138,149,157,151,142, 
            137,154,139,147,144,161,145,120,162, 
            164,148,168,157,126,158,128,164,130,
        };

        //ALARM_CATEGORY_SPC_DATA_SMALL_CHANGE = 6;   //15 points near the center
        public static int[] SPCCollectedData_small_change1 = 
        {
            152,147,146,146,147,136,148,139,115, 
            170,138,172,173,174,175,146,133,146, 
            156,170,141,136,148,150,146,139,147,
            140,175,141,133,143,134,125,160,127, 
            155,156,157,158,159,160,161,153,163, 
            164,165,156,167,168,159,170,131,154,
            161,142,163,154,150,136,156,150,159,
            146,143,170,162,148,150,150,161,148,
            130,164,127,145,150,147,139,136,144, 
            158,146,137,132,129,130,150,154,158,
            137,154,139,147,144,161,145,120,162, 
            162,173,130,157,132,155,164,147,136, 
            164,148,139,157,136,158,138,168,132,
            152,135,143,139,151,170,169,162,151,
            160,161,147,143,167,145,138,142,139,
            157,150,150,148,152,151,133,153,145, 
            147,140,121,149,150,151,152,153,154,
            146,147,148,149,150,151,152,153,154, 
            150,138,154,140,156,159,143,154,161, 
            150,148,149,161,138,149,157,151,142, 
            145,146,143,147,138,159,165,164,124, 
            157,163,137,152,147,150,135,146,165, 
            143,156,152,149,149,151,152,150,162, 
            154,146,151,164,169,135,149,150,145, 
            173,139,153,176,134,168,136,153,146, 
            137,158,139,140,151,155,143,154,145,
            138,169,130,138,140,133,134,135,136, 
            137,158,139,154,161,142,143,144,146, 
            155,156,157,158,159,160,161,162,163, 
            164,149,176,177,148,139,168,159,134, 
        };

        //ALARM_CATEGORY_SPC_DATA_LOCATE_APART = 7;   //4 out of 5 ponits lies at 2/3 of LCL and UCL, failed in s chart  
        public static int[] SPCCollectedData_locate_apart1 = 
        {
            152,147,146,146,147,136,148,139,115, 
            157,150,150,148,152,151,133,153,145, 
            156,150,161,136,148,134,146,139,147,
            132,142,140,157,132,155,134,135,136, 
            137,138,139,140,141,142,143,144,146, 
            147,140,141,149,150,151,152,153,154,
            155,156,157,138,159,160,141,162,143, 
            164,145,156,147,138,139,171,159,134, 
            152,135,143,139,151,158,169,162,141,
            154,170,141,164,169,135,149,150,145, 
            143,156,152,149,149,151,152,150,162, 
            160,161,147,143,167,145,138,142,139,
            140,160,141,133,153,154,145,140,127, 
            148,159,130,138,140,163,144,135,136, 
            137,158,159,140,141,142,143,144,145,
            146,147,148,149,150,151,152,153,154, 
            155,146,157,138,159,160,161,153,143, 
            164,165,156,167,138,159,140,151,154,
            143,154,153,146,134,143,139,153,146, 
            150,138,154,140,156,159,143,154,161, 
            161,162,143,164,145,156,137,160,159,
            170,138,152,143,154,165,146,143,146, 
            157,163,137,152,147,150,135,146,165, 
            146,143,160,162,148,161,150,161,158,
            130,154,140,145,150,147,159,136,154, 
            145,146,153,147,138,159,165,164,154, 
            162,146,137,148,149,160,150,154,148,
            150,148,149,161,138,149,157,151,142, 
            137,154,139,147,144,161,145,140,162, 
            159,148,165,157,134,158,138,164,130,

/*       
            152,147,146,146,147,136,148,139,115, 
            170,138,172,173,174,175,146,133,146, 
            156,150,121,136,148,134,146,139,147,
            140,175,141,133,143,134,125,160,127, 
            155,156,157,158,159,160,161,153,163, 
            164,165,156,167,168,159,170,131,154,
            161,142,163,164,165,136,167,160,159,
            146,143,170,162,148,150,150,161,138,
            130,164,127,145,150,147,139,136,144, 
            182,126,127,128,129,130,150,154,158,
            137,154,139,147,144,161,145,120,162, 
            162,173,130,157,132,155,164,147,136, 
            164,148,168,157,126,158,128,164,130,
            152,135,143,139,151,178,179,162,171,
            160,161,147,143,167,145,138,142,139,
            157,150,150,148,152,151,133,153,145, 
            147,140,121,149,150,151,152,153,154,
            146,147,148,149,150,151,152,153,154, 
            150,138,154,140,156,159,143,154,161, 
            150,148,149,161,138,149,157,151,142, 
            145,146,143,147,138,159,165,164,124, 
            157,163,137,152,147,150,135,146,165, 
            143,156,152,149,149,151,152,150,162, 
            154,146,171,164,169,135,149,150,145, 
            173,129,153,176,134,178,136,153,146, 
            137,158,139,140,151,155,143,154,145,
            138,169,130,138,140,133,134,135,136, 
            137,138,139,140,141,142,143,144,146, 
            155,156,157,158,159,160,161,162,163, 
            164,129,176,177,138,139,178,179,134, 
*/
        };

        //ALARM_CATEGORY_SPC_DATA_SAME_SIDE = 4;   //9 points at same side of the chart
        public static int[] SPCCollectedData_one_side2 = 
        {
            152,135,143,139,151,178,179,162,171,
            160,161,147,143,167,145,138,142,139,
            137,138,139,140,141,142,143,144,145,
            152,147,146,146,147,136,148,139,115, 
            157,150,150,148,152,151,133,153,145, 
            156,150,121,136,148,134,146,139,147,
            140,120,141,133,123,124,125,140,127, 
            138,129,130,138,140,133,134,135,136, 
            132,129,130,157,132,155,134,135,136, 
            137,138,139,140,141,142,143,144,146, 
            154,170,171,164,169,135,149,150,145, 
            143,156,152,149,149,151,152,150,162, 
            155,156,157,158,159,160,161,162,163, 
            147,140,121,149,150,151,152,153,154,
            164,185,176,177,138,139,178,179,134, 
            146,147,148,149,150,151,152,153,154, 
            155,156,157,158,159,160,161,153,163, 
            164,165,156,167,168,159,170,171,154,
            173,174,153,176,134,178,199,153,146, 
            150,138,154,140,156,159,143,154,161, 
            161,162,163,164,165,156,167,160,159,
            170,138,172,173,174,175,146,133,146, 
            157,163,137,152,147,150,135,146,165, 
            146,143,170,162,148,150,150,161,138,
            130,164,127,145,150,147,139,136,144, 
            145,146,143,147,138,159,165,164,124, 
            182,126,127,128,129,130,150,154,158,
            150,148,149,161,138,149,157,151,142, 
            137,154,139,147,144,161,145,120,162, 
            164,148,168,157,126,158,128,164,130,
        };

        //ALARM_CATEGORY_SPC_DATA_ONE_TREND = 5;   //7 points lies in the same trend
        public static int[] SPCCollectedData_one_trend2 = 
        {
            154,170,171,164,169,135,149,150,145, 
            143,156,152,149,149,151,152,150,162, 
            160,161,147,143,167,145,138,142,139,
            152,147,146,146,147,136,148,139,115, 
            156,150,121,136,148,134,146,139,147,
            157,150,150,148,152,151,133,153,145, 
            132,129,130,157,132,155,134,135,136, 
            164,185,176,177,138,139,178,179,134, 
            147,140,121,149,150,151,152,153,154,
            137,138,139,140,141,142,143,144,146, 
            155,156,157,158,159,160,161,162,163, 
            152,135,143,139,151,178,179,162,171,
            140,120,141,133,123,124,125,140,127, 
            138,129,130,138,140,133,134,135,136, 
            137,138,139,140,141,142,143,144,145,
            146,147,148,149,150,151,152,153,154, 
            155,156,157,158,159,160,161,153,163, 
            164,165,156,167,168,159,170,171,154,
            173,174,153,176,134,178,199,153,146, 
            150,138,154,140,156,159,143,154,161, 
            161,162,163,164,165,156,167,160,159,
            170,138,172,173,174,175,146,133,146, 
            157,163,137,152,147,150,135,146,165, 
            146,143,170,162,148,150,150,161,138,
            130,164,127,145,150,147,139,136,144, 
            145,146,143,147,138,159,165,164,124, 
            182,126,127,128,129,130,150,154,158,
            150,148,149,161,138,149,157,151,142, 
            137,154,139,147,144,161,145,120,162, 
            164,148,168,157,126,158,128,164,130,
        };

        //ALARM_CATEGORY_SPC_DATA_ONE_TREND = 5;   //7 points lies in the same trend
        public static int[] SPCCollectedData_small_change2 = 
        {
            155,156,157,158,159,160,161,153,163, 
            164,165,156,167,168,159,170,171,154,
            152,147,146,146,147,136,148,139,115, 
            157,150,150,148,152,151,133,153,145, 
            156,150,121,136,148,134,146,139,147,
            132,129,130,157,132,155,134,135,136, 
            137,138,139,140,141,142,143,144,146, 
            143,156,152,149,149,151,152,150,162, 
            160,161,147,143,167,145,138,142,139,
            140,120,141,133,123,124,125,140,127, 
            138,129,130,138,140,133,134,135,136, 
            137,138,139,140,141,142,143,144,145,
            146,147,148,149,150,151,152,153,154, 
            147,140,121,149,150,151,152,153,154,
            155,156,157,158,159,160,161,162,163, 
            164,185,176,177,138,139,178,179,134, 
            152,135,143,139,151,178,179,162,171,
            154,170,171,164,169,135,149,150,145, 
            173,174,153,176,134,178,199,153,146, 
            150,138,154,140,156,159,143,154,161, 
            161,162,163,164,165,156,167,160,159,
            170,138,172,173,174,175,146,133,146, 
            157,163,137,152,147,150,135,146,165, 
            146,143,170,162,148,150,150,161,138,
            130,164,127,145,150,147,139,136,144, 
            145,146,143,147,138,159,165,164,124, 
            182,126,127,128,129,130,150,154,158,
            150,148,149,161,138,149,157,151,142, 
            137,154,139,147,144,161,145,120,162, 
            164,148,168,157,126,158,128,164,130,
        };


        //23/24/25/26 144.9, 
        //ALARM_CATEGORY_SPC_DATA_LOCATE_APART = 7;   //4 out of 5 ponits lies at 2/3 of LCL and UCL 
        public static int[] SPCCollectedData_locate_apart2 = 
        {
            152,135,143,139,151,178,179,162,171,
            160,161,147,143,167,145,138,142,139,
            137,138,139,140,141,142,143,144,145,
            152,147,146,146,147,136,148,139,115, 
            157,150,150,148,152,151,133,153,145, 
            156,150,121,136,148,134,146,139,147,
            140,120,141,133,123,124,125,140,127, 
            138,129,130,138,140,133,134,135,136, 
            132,129,130,157,132,155,134,135,136, 
            137,138,139,140,141,142,143,144,146, 
            154,170,171,164,169,135,149,150,145, 
            143,156,152,149,149,151,152,150,162, 
            155,156,157,158,159,160,161,162,163, 
            147,140,121,149,150,151,152,153,154,
            164,185,176,177,138,139,178,179,134, 
            146,147,148,149,150,151,152,153,154, 
            155,156,157,158,159,160,161,153,163, 
            164,165,156,167,168,159,170,171,154,
            173,174,153,176,134,178,199,153,146, 
            150,138,154,140,156,159,143,154,161, 
            161,162,163,164,165,156,167,160,159,
            170,138,172,173,174,175,146,133,146, 
            157,163,137,152,147,150,135,146,165, 
            146,143,170,162,148,150,150,161,138,
            130,164,127,145,150,147,139,136,144, 
            145,146,143,147,138,159,165,164,124, 
            182,126,127,128,129,130,150,154,158,
            150,148,149,161,138,149,157,151,142, 
            137,154,139,147,144,161,145,120,162, 
            164,148,168,157,126,158,128,164,130,
        };

        public Form1()
        {
            InitializeComponent();
            initVariables();
        }


        private void initVariables()
        {
            int i;
            const int x = 7;
            const int y = 31;
            const int delta_x = 110;
            const int delta_y = 32;

            data[0] = 0x77;
            data[1] = 0x49;
            data[2] = 0x46;
            data[3] = 0x69;

            dataPacketIndex = 0;
            volCurIndex = 0;
            selectedComboBoxIndex = 0;
            selectedMachineIndex = 0;
            selectedMachineID = 0;
            currentCreatedNum = 0;
            currentOverflowFlag = 0;
            sendCraftDataThreshold = 4;

            beatTestFlag = 0;

            mySQLClass mySQL = new mySQLClass();

            if (Form1.systemDefine == Form1.PLANT_23)
            {
                databaseHeader = "z";

            }
            else
            {
                databaseHeader = "h";

                PROTOCOL_CRC_LEN = 4;
                PROTOCOL_DATA_POS = 28;

                for (i = 0; i < machineNameZihuaArray.Length; i++)
                {
                    machineCodeArray[i] = machineCodeZihuaArray[i];
                    machineNameArray[i] = machineNameZihuaArray[i];
                }
            }

            button11.Enabled = false;   //alarm sign
            button12.Enabled = false;   //alarm complete
            button13.Enabled = false;  //alarm cancel
            //            button17.Enabled = false;  //add new machine


            HostIP = IPAddress.Parse(getCommunicationHostIP()); //IPAddress.Parse("192.168.1.100");

            labelArray[0]  = label2; 
            labelArray[1]  = label3; 
            labelArray[2]  = label4; 
            labelArray[3]  = label5; 
            labelArray[4]  = label6; 
            labelArray[5]  = label7; 
            labelArray[6]  = label8; 
            labelArray[7]  = label9; 
            labelArray[8]  = label10;
            labelArray[9]  = label11;
            labelArray[10] = label12;
            labelArray[11] = label13;
            labelArray[17] = label19;
            labelArray[16] = label18;
            labelArray[15] = label17;
            labelArray[14] = label16;
            labelArray[13] = label15;
            labelArray[12] = label14;

            for (i = 0; i < MAXLABELNUM; i++)
            {
                connectedMachineArray[i] = 0;
                startCommnuicateAs[i] = communicateAsTouchpad;
                communicationStatusArray[i] = handshakeNot;
                createdMachineArray[i] = 0; ;
                labelArray[i].Visible = false;
                socketArray[i] = null;

                labelArray[i].Location = new System.Drawing.Point(x + (i % 5) * delta_x, y + (i / 5) * delta_y);
            }

            for (i = 0; i < 20; i++)
                adcOverflowFlag[i] = 0;  //no ADC overflow in any channel

            for (i = 0; i < MaxClientNum; i++)
            {
                createdMachineArray[i] = i + 1;
                comboBox1.Items.Add((i + 1).ToString() + "号设备");
            }
            comboBox1.SelectedIndex = 0;

            for (i = 0; i < communicationFreqArray.Length; i++)
            {
                comboBox2.Items.Add(communicationFreqArray[i]);
            }
            comboBox2.SelectedIndex = 3;
            communicationFrequency = (int)(1000 / communicationFreqArray[comboBox2.SelectedIndex]);

            for (i = 0; i < printingMachineArray.Length; i++)
            {
                comboBox3.Items.Add(printingMachineArray[i]);
            }
            comboBox3.SelectedIndex = 0;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            aTimer = new System.Windows.Forms.Timer();

            aTimer.Interval = communicationFrequency;
            aTimer.Enabled = true;

            aTimer.Tick += new EventHandler(timer_listview);
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            if (socketArray[selectedMachineIndex] != null)
                socketArray[selectedMachineIndex].Close();
        }


        void inputDataHeader(byte[] packet, int len, int type, int dataType)
        {
            int i;

            i = 0;

            packet[i++] = (byte)'w';
            packet[i++] = (byte)'I';
            packet[i++] = (byte)'F';
            packet[i++] = (byte)'i';

            packet[i++] = (byte)len;
            packet[i++] = (byte) (len / 0x100);

            packet[i++] = (byte)type;

            if (Form1.systemDefine == Form1.PLANT_ZIHUA)
            {
                packet[i++] = (byte)(DateTime.Now.Year % 100 / 10 + '0');
                packet[i++] = (byte)(DateTime.Now.Year % 100 % 10 + '0');
                packet[i++] = (byte)(DateTime.Now.Month / 10 + '0');
                packet[i++] = (byte)(DateTime.Now.Month % 10 + '0');
                packet[i++] = (byte)(DateTime.Now.Day / 10 + '0');
                packet[i++] = (byte)(DateTime.Now.Day % 10 + '0');
                packet[i++] = (byte)(DateTime.Now.Hour / 10 + '0');
                packet[i++] = (byte)(DateTime.Now.Hour % 10 + '0');
                packet[i++] = (byte)(DateTime.Now.Minute / 10 + '0');
                packet[i++] = (byte)(DateTime.Now.Minute % 10 + '0');
                packet[i++] = (byte)(DateTime.Now.Second / 10 + '0');
                packet[i++] = (byte)(DateTime.Now.Second % 10 + '0');

                packet[i++] = (byte)dataPacketIndex;
                packet[i++] = (byte)(dataPacketIndex / 0x100);
                packet[i++] = (byte)(dataPacketIndex / 0x10000);
                packet[i++] = (byte)(dataPacketIndex / 0x1000000);

                packet[i++] = 0;
                packet[i++] = 0;
                packet[i++] = 0;
                packet[i++] = 0;

                packet[i++] = (byte)dataType;
            }
        }

        private void startCommunication()
        {
            int recCount;
            int len;

            if (connectedMachineArray[selectedComboBoxIndex] == 1)
            {
                MessageBox.Show("该设备已经在通讯中, 无法再次开启通讯功能", "信息提示", MessageBoxButtons.OK);
                return;
            }

            try
            {
                connectedMachineArray[selectedComboBoxIndex] = 1;
                len = MIN_PACKET_LEN_MINUS_ONE + 4;

                inputDataHeader(handshake_packet, len, COMMUNICATION_TYPE_START_HANDSHAKE_WITH_ID_TO_PC, DATA_TYPE_ADC_DEVICE);

                handshake_packet[PROTOCOL_DATA_POS] = (byte)selectedMachineID;
                handshake_packet[PROTOCOL_DATA_POS + 1] = 0;
                handshake_packet[PROTOCOL_DATA_POS + 2] = 0;

                if (startCommnuicateAs[selectedMachineIndex] == communicateAsTouchpad)
                    handshake_packet[PROTOCOL_DATA_POS + 3] = 0; //so the final value is ID
                else
                    handshake_packet[PROTOCOL_DATA_POS + 3] = 1; //so the final value is 0x1000000 + ID

                point = new IPEndPoint(HostIP, 8899);
                socketArray[selectedMachineIndex] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketArray[selectedMachineIndex].Connect(point);

                if (socketArray[selectedMachineIndex].Connected)
                {
                    CRC16.addCrcCode(handshake_packet, len);
                    recCount = socketArray[selectedMachineIndex].Send(handshake_packet, len, 0);

                    len = MIN_PACKET_LEN_MINUS_ONE + 12;  //12 is the length of a time value
                    recCount = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);

                    len = MIN_PACKET_LEN_MINUS_ONE + 11;  //length of the dummy data

                    inputDataHeader(dummyMachine_packet, len, COMMUNICATION_TYPE_SEND_DUMMY_MACHINE_CODE_TO_PC, DATA_TYPE_MES_INSTRUCTION);
                    CRC16.addCrcCode(dummyMachine_packet, len);
                    recCount = socketArray[selectedMachineIndex].Send(dummyMachine_packet, len, 0);

                    Thread.Sleep(WAIT_BETWEEN_SEND_RECEIVE);

                    recCount = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
                    communicationStatusArray[selectedMachineIndex] = handshakeOK;

                    currentConnectedNum++;
                    connectedMachineArray[selectedComboBoxIndex] = 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("通讯失败，可能服务器端未启动或者 ..\\..\\init\\init.txt 中输入的 IP 不是服务器的 IP 地址。", "信息提示", MessageBoxButtons.OK);
                return;
            }

        }

        private string getCommunicationHostIP()
        {
            string filePath = "..\\..\\init\\init.txt";
            StreamReader streamReader;
            string IPString;

            try
            {
                streamReader = new StreamReader(filePath, System.Text.Encoding.Default);
                IPString = streamReader.ReadLine().Trim();
                streamReader.Close();

                return IPString;
            }
            catch (Exception ex)
            {
                Console.Write("无法开启 ..\\..\\init\\init.txt 文件, 因此无法得知服务器 ip 地址");
                Console.WriteLine(ex.ToString());
                return null;
            }
        }


        //start communication as app from Android
        private void button1_Click(object sender, EventArgs e)
        {
            startCommnuicateAs[selectedMachineIndex] = communicateAsApp;

            startCommunication();
        }


        //start communcation as touchpad device
        private void button15_Click(object sender, EventArgs e)
        {
            startCommnuicateAs[selectedMachineIndex] = communicateAsTouchpad;

            startCommunication();
        }

        //printing SW handshake with server 
        private void button40_Click(object sender, EventArgs e)
        {
            startCommnuicateAs[selectedMachineIndex] = communicateAsPrintingSW;

            startCommunicationForPrint();
        }


        //stop communication
        private void button2_Click(object sender, EventArgs e)
        {
            socketArray[selectedMachineIndex].Close();
        }

        //exit
        private void button16_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void face_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void timer_listview(Object source, EventArgs e)
        {
            int i, j;

            //if it takes a long time to draw these charts, we wait for the end of chart drawing, then start waiting for 1 second
            aTimer.Enabled = false;

            //for label printing SW, no timer function is needed
            if (startCommnuicateAs[selectedMachineIndex] != communicateAsPrintingSW)
            {
                for (i = 0; i < MAXMACHINENUM; i++)
                {
                    if (communicationStatusArray[i] >= handshakeOK)
                        sendDataToHostPC(i);
                }
            }

            for (i = 0, j = 0; i < MaxClientNum; i++)
            {
                if (connectedMachineArray[i] == 1)
                {
                    if(startCommnuicateAs[i] == communicateAsApp)
                        labelArray[j].Text = createdMachineArray[i].ToString() + "号设备(App)";
                    else
                        labelArray[j].Text = createdMachineArray[i].ToString() + "号设备(Touchpad)";

                    labelArray[j].Visible = true;
                    j++;
                }
            }
            for (; i < MAXLABELNUM; i++)
            {
                labelArray[i].Visible = false;
                socketArray[i] = null;
            }

            aTimer.Interval = communicationFrequency;
            aTimer.Stop();

            aTimer.Enabled = true;
            aTimer.Start();
        }


        private void sendDataToHostPC(int index)
        {
            int i, j;
            int len;
            int v, v1, v2, v3, v4, v5, v6;
            int vh, vl;
            int index0, index1;
            int recCount;
            float f1, f2;
            string str;
            byte[] buf = new byte[10];
            DateTime now;
            byte[] sendByte = new Byte[105];

//            int randV;
//            float d;
//            int randomErrRange;
            float iLowerLimit = 1.019f;
            float iUpperLimit = 1.126f;
//            float vLowerLimit = 200.0f;
//            float vUpperLimit = 233.0f;
//            float cLowerLimit = 13.1f;
//            float cUpperLimit = 19.2f;
//            float fLowerLimit = 50.2f;
//            float fUpperLimit = 23.3f;
            float delta;
//            string filePath = "..\\..\\init\\init.txt";
//            StreamReader streamReader;
//            string IDString, IPString;

            delta = iUpperLimit - iLowerLimit;

            index0 = 0;
            index1 = 0;

            try
            {
                Random rand = new Random();
                now = DateTime.Now;

                if (dataPacketIndex == sendCraftDataThreshold - 1)
                {
                    i = PROTOCOL_DATA_POS;
                    len = MIN_PACKET_LEN_MINUS_ONE + 16;
                    inputDataHeader(data_packet, len, COMMUNICATION_TYPE_COLLECTED_DATA_SEND_TO_PC, DATA_TYPE_ADC_DEVICE);

                    if (Form1.systemDefine == Form1.PLANT_23)
                    {
                        data_packet[PROTOCOL_LEN_POS] = 47;
                        handshake_packet[PROTOCOL_DATA_POS] = (byte)selectedMachineID;

                        data_packet[i++] = 0;
                        data_packet[i++] = 0;   //this byte is not used any more

                        data_packet[i++] = (byte)(now.Year % 100 / 10 + '0');
                        data_packet[i++] = (byte)(now.Year % 100 % 10 + '0');
                        data_packet[i++] = (byte)(now.Month / 10 + '0');
                        data_packet[i++] = (byte)(now.Month % 10 + '0');
                        data_packet[i++] = (byte)(now.Day / 10 + '0');
                        data_packet[i++] = (byte)(now.Day % 10 + '0');
                        data_packet[i++] = (byte)(now.Hour / 10 + '0');
                        data_packet[i++] = (byte)(now.Hour % 10 + '0');
                        data_packet[i++] = (byte)(now.Minute / 10 + '0');
                        data_packet[i++] = (byte)(now.Minute % 10 + '0');
                        data_packet[i++] = (byte)(now.Second / 10 + '0');
                        data_packet[i++] = (byte)(now.Second % 10 + '0');
                        data_packet[i++] = (byte)(now.Millisecond / 100 + '0');
                        data_packet[i++] = (byte)(now.Millisecond % 100 / 10 + '0');
                        data_packet[i++] = (byte)(now.Millisecond % 10 + '0');
                        data_packet[i++] = (byte)(index0 / 10000 + '0');
                        data_packet[i++] = (byte)(index0 / 1000 % 10 + '0');
                        data_packet[i++] = (byte)(index0 / 100 % 10 + '0');
                        data_packet[i++] = (byte)(index0 / 10 % 10 + '0');
                        data_packet[i++] = (byte)(index0 % 10 + '0');
                    }

                    v = rand.Next(13200, 19800);
                    v1 = v / 0x100;
                    v2 = v % 0x100;

                    vh = rand.Next(31000, 32700);
                    v3 = vh / 0x100;
                    v4 = vh % 0x100;

                    vl = rand.Next(100, 2000);
                    v5 = vl / 0x100;
                    v6 = vl % 0x100;

                    if (adcOverflowFlag[0] == 1)
                    {
                        data_packet[i++] = (byte)v3;
                        data_packet[i++] = (byte)v4;
                        f1 = (craftUpperRangeLimit[0] - craftLowerRangeLimit[0]) * vh / 32768 + craftLowerRangeLimit[0];
                        MessageBox.Show("ADC1超上限值" + f1 + "上传成功.", "信息提示", MessageBoxButtons.OK);
                    }
                    else if (adcOverflowFlag[0] == 2)
                    {
                        data_packet[i++] = (byte)v5;
                        data_packet[i++] = (byte)v6;
                        f1 = (craftUpperRangeLimit[0] - craftLowerRangeLimit[0]) * vl / 32768 + craftLowerRangeLimit[0];
                        MessageBox.Show("ADC1超下限值" + f1 + "上传成功.", "信息提示", MessageBoxButtons.OK);
                    }
                    else //if (adcOverflowFlag[0] == 0)
                    {
                        data_packet[i++] = (byte)v1;
                        data_packet[i++] = (byte)v2;
                    }
                    adcOverflowFlag[0] = 0;

                    if (adcOverflowFlag[1] == 1)
                    {
                        data_packet[i++] = (byte)v3;
                        data_packet[i++] = (byte)v4;
                        f1 = (craftUpperRangeLimit[1] - craftLowerRangeLimit[1]) * vh / 32768 + craftLowerRangeLimit[1];
                        MessageBox.Show("ADC2超上限值" + f1 + "上传成功.", "信息提示", MessageBoxButtons.OK);
                    }
                    else if (adcOverflowFlag[1] == 2)
                    {
                        data_packet[i++] = (byte)v5;
                        data_packet[i++] = (byte)v6;
                        f1 = (craftUpperRangeLimit[1] - craftLowerRangeLimit[1]) * vl / 32768 + craftLowerRangeLimit[1];
                        MessageBox.Show("ADC2超下限值" + f1 + "上传成功.", "信息提示", MessageBoxButtons.OK);
                    }
                    else //if (adcOverflowFlag[1] == 0)
                    {
                        data_packet[i++] = (byte)v1;
                        data_packet[i++] = (byte)v2;
                    }
                    adcOverflowFlag[1] = 0;

                    if (adcOverflowFlag[2] == 1)
                    {
                        data_packet[i++] = (byte)v3;
                        data_packet[i++] = (byte)v4;
                        f1 = (craftUpperRangeLimit[2] - craftLowerRangeLimit[2]) * vh / 32768 + craftLowerRangeLimit[2];
                        MessageBox.Show("ADC3超上限值" + f1 + "上传成功.", "信息提示", MessageBoxButtons.OK);
                    }
                    else if (adcOverflowFlag[2] == 2)
                    {
                        data_packet[i++] = (byte)v5;
                        data_packet[i++] = (byte)v6;
                        f1 = (craftUpperRangeLimit[2] - craftLowerRangeLimit[2]) * vl / 32768 + craftLowerRangeLimit[2];
                        MessageBox.Show("ADC3超下限值" + f1 + "上传成功.", "信息提示", MessageBoxButtons.OK);
                    }
                    else //if (adcOverflowFlag[2] == 0)
                    {
                        data_packet[i++] = (byte)v1;
                        data_packet[i++] = (byte)v2;
                    }
                    adcOverflowFlag[2] = 0;

                    if (adcOverflowFlag[3] == 1)
                    {
                        data_packet[i++] = (byte)v3;
                        data_packet[i++] = (byte)v4;
                        f1 = (craftUpperRangeLimit[3] - craftLowerRangeLimit[3]) * vh / 32768 + craftLowerRangeLimit[3];
                        MessageBox.Show("ADC4超上限值" + f1 + "上传成功.", "信息提示", MessageBoxButtons.OK);
                    }
                    else if (adcOverflowFlag[3] == 2)
                    {
                        data_packet[i++] = (byte)v5;
                        data_packet[i++] = (byte)v6;
                        f1 = (craftUpperRangeLimit[3] - craftLowerRangeLimit[3]) * vl / 32768 + craftLowerRangeLimit[3];
                        MessageBox.Show("ADC4超下限值" + f1 + "上传成功.", "信息提示", MessageBoxButtons.OK);
                    }
                    else //if (adcOverflowFlag[3] == 0)
                    {
                        data_packet[i++] = (byte)v1;
                        data_packet[i++] = (byte)v2;
                    }
                    adcOverflowFlag[3] = 0;

                    if (adcOverflowFlag[4] == 1)
                    {
                        data_packet[i++] = (byte)v3;
                        data_packet[i++] = (byte)v4;
                    }
                    else if (adcOverflowFlag[4] == 2)
                    {
                        data_packet[i++] = (byte)v5;
                        data_packet[i++] = (byte)v6;
                    }
                    else //if (adcOverflowFlag[4] == 0)
                    {
                        data_packet[i++] = (byte)v1;
                        data_packet[i++] = (byte)v2;
                    }
                    adcOverflowFlag[4] = 0;

                    if (adcOverflowFlag[5] == 1)
                    {
                        data_packet[i++] = (byte)v3;
                        data_packet[i++] = (byte)v4;
                    }
                    else if (adcOverflowFlag[5] == 2)
                    {
                        data_packet[i++] = (byte)v5;
                        data_packet[i++] = (byte)v6;
                    }
                    else //if (adcOverflowFlag[5] == 0)
                    {
                        data_packet[i++] = (byte)v1;
                        data_packet[i++] = (byte)v2;
                    }
                    adcOverflowFlag[5] = 0;

                    if (adcOverflowFlag[6] == 1)
                    {
                        data_packet[i++] = (byte)v3;
                        data_packet[i++] = (byte)v4;
                    }
                    else if (adcOverflowFlag[6] == 2)
                    {
                        data_packet[i++] = (byte)v5;
                        data_packet[i++] = (byte)v6;
                    }
                    else //if (adcOverflowFlag[6] == 0)
                    {
                        data_packet[i++] = (byte)v1;
                        data_packet[i++] = (byte)v2;
                    }
                    adcOverflowFlag[6] = 0;

                    if (adcOverflowFlag[7] == 1)
                    {
                        data_packet[i++] = (byte)v3;
                        data_packet[i++] = (byte)v4;
                    }
                    else if (adcOverflowFlag[7] == 2)
                    {
                        data_packet[i++] = (byte)v5;
                        data_packet[i++] = (byte)v6;
                    }
                    else //if (adcOverflowFlag[7] == 0)
                    {
                        data_packet[i++] = (byte)v1;
                        data_packet[i++] = (byte)v2;
                    }
                    adcOverflowFlag[7] = 0;

                    //            i += 32;

                    //            sendByte[4] = (byte)(i + 2);

                    CRC16.addCrcCode(data_packet, i + PROTOCOL_CRC_LEN);

                    index0++;
                    recCount = socketArray[index].Send(data_packet, i + PROTOCOL_CRC_LEN, 0);
                    //            sendStr = System.Text.Encoding.ASCII.GetString(sendByte);
                    Thread.Sleep(50);
                    recCount = socketArray[index].Receive(receiveByte, REV_LEN, 0);
                    //            receiveString = System.Text.Encoding.ASCII.GetString(receiveByte);

                    //            sendByte[36] = 0x0d;
                    //            sendByte[37] = 0x0a;
                    //            sendByte[38] = 0;
                    //            streamWriter.WriteLine(sendStr.Remove(38).Remove(0, 9));
                }

                if (dataPacketIndex >= sendCraftDataThreshold - 1)
                    dataPacketIndex = 0;
                else
                    dataPacketIndex++;


                i = PROTOCOL_DATA_POS;
                len = MIN_PACKET_LEN_MINUS_ONE + 36;  //voltage/currnt data length is 9*4 = 36
                inputDataHeader(data_packet, len, COMMUNICATION_TYPE_COLLECTED_DATA_SEND_TO_PC, DATA_TYPE_VOL_CUR_DEVICE);

                if (Form1.systemDefine == Form1.PLANT_23)
                {
                    data_packet[PROTOCOL_LEN_POS] = 67;

                    data_packet[i++] = 1;  //vol/cur data
                    data_packet[i++] = 0;  //this byte is not used any more

                    data_packet[i++] = (byte)(now.Year % 100 / 10 + '0');
                    data_packet[i++] = (byte)(now.Year % 100 % 10 + '0');
                    data_packet[i++] = (byte)(now.Month / 10 + '0');
                    data_packet[i++] = (byte)(now.Month % 10 + '0');
                    data_packet[i++] = (byte)(now.Day / 10 + '0');
                    data_packet[i++] = (byte)(now.Day % 10 + '0');
                    data_packet[i++] = (byte)(now.Hour / 10 + '0');
                    data_packet[i++] = (byte)(now.Hour % 10 + '0');
                    data_packet[i++] = (byte)(now.Minute / 10 + '0');
                    data_packet[i++] = (byte)(now.Minute % 10 + '0');
                    data_packet[i++] = (byte)(now.Second / 10 + '0');
                    data_packet[i++] = (byte)(now.Second % 10 + '0');
                    data_packet[i++] = (byte)(now.Millisecond / 100 + '0');
                    data_packet[i++] = (byte)(now.Millisecond % 100 / 10 + '0');
                    data_packet[i++] = (byte)(now.Millisecond % 10 + '0');
                    data_packet[i++] = (byte)(index1 / 10000 + '0');
                    data_packet[i++] = (byte)(index1 / 1000 % 10 + '0');
                    data_packet[i++] = (byte)(index1 / 100 % 10 + '0');
                    data_packet[i++] = (byte)(index1 / 10 % 10 + '0');
                    data_packet[i++] = (byte)(index1 % 10 + '0');
                }

                f1 = rand.Next(372, 388);
                str = f1.ToString("0000.0000");

                buf = System.Text.Encoding.Default.GetBytes(str);

                for (j = 0; j < 9; j++)
                {
                    data_packet[i++] = (byte)buf[j];
                }

                if (beatTestFlag == 1)
                {
                    f2 = getCurrentValue(volCurIndex + beatStartIndex);
                }
                else
                {
                    if (currentOverflowFlag == 0)
                        f2 = rand.Next(30, 35);
                    else
                        f2 = 200;
                }
                str = f2.ToString("0000.0000");

                buf = System.Text.Encoding.Default.GetBytes(str);

                for (j = 0; j < 9; j++)
                {
                    data_packet[i++] = (byte)buf[j];
                }

                f1 = f1 * f2 / 1000;
                str = f1.ToString("0000.0000");

                buf = System.Text.Encoding.Default.GetBytes(str);

                for (j = 0; j < 9; j++)
                {
                    data_packet[i++] = (byte)buf[j];
                }

                f1 = (int)rand.Next(47, 53);

                str = f1.ToString("0000.0000");

                buf = System.Text.Encoding.Default.GetBytes(str);

                for (j = 0; j < 9; j++)
                {
                    data_packet[i++] = (byte)buf[j];
                }

                if (currentOverflowFlag == 1)
                {
                    currentOverflowFlag = 0;
                    MessageBox.Show("电流超上限值" + f2 + "上传成功.", "信息提示", MessageBoxButtons.OK);
                }

                CRC16.addCrcCode(data_packet, i + PROTOCOL_CRC_LEN);

                index1++;
                recCount = socketArray[index].Send(data_packet, i + PROTOCOL_CRC_LEN, 0);
                //                        sendStr = System.Text.Encoding.ASCII.GetString(sendByte);
                Thread.Sleep(50);
                recCount = socketArray[index].Receive(receiveByte, REV_LEN, 0);

                volCurIndex++;
            }
            catch (Exception ex)
            {
                Console.Write("data sending error" + ex);
                System.Environment.Exit(0);
            }
        }

        public int isDigitalNum(string str)
        {
            int i;
            int ret;
            char[] strArray;

            strArray = str.Trim().ToCharArray();

            ret = 0;
            for (i = 0; i < strArray.Length; i++)
            {
                if (strArray[i] < 0x30 || strArray[i] > 0x39)
                {
                    ret = 0;
                    break;
                }
                ret = 1;
            }
            return ret;
        }

        //apply for dispatch
        private void button3_Click(object sender, EventArgs e)
        {
            int i, j;
            int len;
            string str;
            byte[] buf = new byte[1000];

            if (socketArray[selectedMachineIndex] == null)
            {
                MessageBox.Show("只有开始通讯后才能申请工单.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] >= handshakeOK)
            {
                str = "aaa;bbb;ccc;ddd;110;120;110;150;4.50;8.5;4.50;8.50;3.3;4.2;110;140;120;150;345;370;";

                i = PROTOCOL_DATA_POS;
                len = MIN_PACKET_LEN_MINUS_ONE + str.Length;
                inputDataHeader(data, len, COMMUNICATION_TYPE_SETTING_TO_PC, 0);

                buf = System.Text.Encoding.Default.GetBytes(str);
                for (j = 0; j < str.Length; j++)
                {
                    data[i++] = (byte)buf[j];
                }

                CRC16.addCrcCode(data, len);
                socketArray[selectedMachineIndex].Send(data, len, 0);

                Thread.Sleep(750);

                len = socketArray[selectedMachineIndex].Receive(receiveByte, REV_LEN, 0);
                if (len > 0 && receiveByte[PROTOCOL_DATA_POS] == 0xff)
                {
                    MessageBox.Show("申请工单失败，服务器已无新工单！", "信息提示", MessageBoxButtons.OK);
                    return;
                }
                str = System.Text.Encoding.Default.GetString(receiveByte, 0, 30);
                Console.WriteLine(str);

                len = MIN_PACKET_LEN;
                inputDataHeader(data, len, COMMUNICATION_TYPE_QUALITY_DATA_TO_TOUCHPAD, 0);

                data[PROTOCOL_DATA_POS] = 0;
                CRC16.addCrcCode(data, MIN_PACKET_LEN);
                socketArray[selectedMachineIndex].Send(data, MIN_PACKET_LEN, 0);

                communicationStatusArray[selectedMachineIndex] = dispatchApplied;

                hintThenVanish("工单申请成功！");
            }

        }

        //quality data upload
        private void button4_Click(object sender, EventArgs e)
        {
            int i, j;
            int len;
            int[] v = new int[8];
            string str;
            byte[] buf = new byte[1000];

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后才能上传质量参数.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            i = PROTOCOL_DATA_POS;
 
            Random rand = new Random();

            for (j = 0; j < 8; j++)
            {
                if (Form1.systemDefine == Form1.PLANT_23)
                    v[j] = rand.Next(1, 5);
                else
                    v[j] = rand.Next(100, 200);
            }

            str = v[0] + ";" + v[1] + ";" + v[2] + ";" + v[3] + ";" + v[4] + ";" + v[5] + ";" + v[6] + ";" + v[7] + ";" + "DFEC20170204;";

            buf = System.Text.Encoding.Default.GetBytes(str);
            for (j = 0; j < str.Length; j++)
            {
                data[i++] = (byte)buf[j];
            }

            len = MIN_PACKET_LEN_MINUS_ONE + str.Length;
            inputDataHeader(data, len, COMMUNICATION_TYPE_QUALITY_DATA_TO_PC, 0);

            CRC16.addCrcCode(data, len);
            socketArray[selectedMachineIndex].Send(data, len, 0);

            communicationStatusArray[selectedMachineIndex] = dispatchStarted;

            hintThenVanish("质量数据上传成功！");
        }

        //device andon
        private void button6_Click(object sender, EventArgs e)
        {
            int i, j;
            int len;
            int index;
            string str;
            byte[] buf;

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("只有开始通讯后才能启动安灯报警.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            i = PROTOCOL_DATA_POS;
            str = "";

            Random rand = new Random();

            index = rand.Next(1, 17);

            if (startCommnuicateAs[selectedMachineIndex] == communicateAsTouchpad)
                str = machineCodeArray[selectedMachineIndex] + ";" + machineNameArray[selectedMachineIndex] + ";" + index + ";AAAAbbb      ;";
            else //if (startCommnuicateAs[selectedMachineIndex] == communicateAsApp)
                str = index.ToString() + ";" + "AAAAbbb      ;";

            buf = System.Text.Encoding.GetEncoding("gb2312").GetBytes(str);
            for (j = 0; j < buf.Length; j++)
            {
                data[i++] = (byte)buf[j];
            }

            len = MIN_PACKET_LEN_MINUS_ONE + str.Length;
            inputDataHeader(data, len, COMMUNICATION_TYPE_DEVICE_ANDON_TO_PC, 0);

            CRC16.addCrcCode(data, len);
            socketArray[selectedMachineIndex].Send(data, len, 0);

            hintThenVanish("设备安灯申请成功");
        }

        //material andon
        private void button7_Click(object sender, EventArgs e)
        {
            int i, j;
            int len;
            string str = "a10000;40;a10002;23;a10005;21";
            byte[] buf; ;

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("只有开始通讯后才能启动安灯报警.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (startCommnuicateAs[selectedMachineIndex] == communicateAsTouchpad)
            {
                i = PROTOCOL_DATA_POS;
                len = MIN_PACKET_LEN_MINUS_ONE + 1;
                inputDataHeader(data, len, COMMUNICATION_TYPE_MATERIAL_ANDON_TO_PC, 0);
                data[i] = (byte)'1';
            }
            else //if(startCommnuicateAs[selectedMachineIndex] == communicateAsApp)
            {
                i = PROTOCOL_DATA_POS;
                buf = System.Text.Encoding.GetEncoding("gb2312").GetBytes(str);
                for (j = 0; j < buf.Length; j++)
                {
                    data[i++] = (byte)buf[j];
                }

                len = MIN_PACKET_LEN_MINUS_ONE + str.Length;
                inputDataHeader(data, len, COMMUNICATION_TYPE_MATERIAL_ANDON_TO_PC, 0);
            }

            CRC16.addCrcCode(data, len);
            socketArray[selectedMachineIndex].Send(data, len, 0);

            hintThenVanish("物料安灯申请成功！");
        }

        //beat upload
        private void button5_Click(object sender, EventArgs e)
        {
            //int v;
            int i;
            int len;

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后才能上传节拍.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] >= dispatchStarted)
            {
                Random rand = new Random();

                i = PROTOCOL_DATA_POS;

//                v = rand.Next(35, 127);

                len = MIN_PACKET_LEN_MINUS_ONE + 4;
                inputDataHeader(data, len, COMMUNICATION_TYPE_CYCLE_TIME_TO_PC, 0);
                data[i++] = 0x61;
                data[i++] = 0;
                data[i++] = 0;
                data[i++] = 0;

                CRC16.addCrcCode(data, len);
                socketArray[selectedMachineIndex].Send(data, len, 0);
            }
            hintThenVanish("合格品节拍数据上传成功！");
        }

        //dispatch started
        private void button8_Click(object sender, EventArgs e)
        {
            int i;
            int len;
            byte[] buf = new byte[1000];

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchApplied)
            {
                MessageBox.Show("只有工单申请成功后才能启动工单.", "信息提示", MessageBoxButtons.OK);
                return;
            }
            i = PROTOCOL_DATA_POS;

            len = MIN_PACKET_LEN;
            inputDataHeader(data, len, COMMUNICATION_TYPE_DISPATCH_START_TO_PC, 0);
            data[i] = (byte)'0';

            CRC16.addCrcCode(data, len);
            socketArray[selectedMachineIndex].Send(data, len, 0);
            communicationStatusArray[selectedMachineIndex] = dispatchStarted;
            hintThenVanish("工单启动成功！");
        }

        //dispatch completed
        private void button9_Click(object sender, EventArgs e)
        {
            int V23;
            int i, j;
            int len;
            string str;
            byte[] buf;

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后才能报工.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            i = PROTOCOL_DATA_POS;
            if (startCommnuicateAs[selectedMachineIndex] == communicateAsTouchpad)
            {
                str = "AA;AA;AA;AA;AA;AA;AA;22;1;AA;AA;AA;AA;admin;";
            }
            else
            {
                str = "20;3;admin;";
            }

            sendStringToServer(str, COMMUNICATION_TYPE_DISPATCH_COMPLETED_TO_PC);

            i = PROTOCOL_DATA_POS;

            data[i++] = (byte)' ';
            data[i++] = (byte)';';

            str = machineNameArray[selectedMachineIndex];
            buf = System.Text.Encoding.GetEncoding("gb2312").GetBytes(str);
            for (j = 0; j < buf.Length; j++)  //assume machine name is less than 20 characters
            {
                data[i++] = (byte)buf[j];
            }

            data[i] = (byte)';';

            if (Form1.systemDefine == Form1.PLANT_23)
            {
                V23 = 0;
            }
            else
            {
                V23 = 23;
            }

            //total working time
            data[81 + V23] = 220;
            data[82 + V23] = 3;
            data[83 + V23] = 0;
            data[84 + V23] = 0;

            //product beat
            data[93 + V23] = 52;
            data[94 + V23] = 0;
            data[95 + V23] = 0;
            data[96 + V23] = 0;

            //power consumed
            data[103 + V23] = 120;
            data[104 + V23] = 1;
            data[105 + V23] = 0;
            data[106 + V23] = 0;

            //standby time
            data[109 + V23] = 123;
            data[110 + V23] = 0;
            data[111 + V23] = 0;
            data[112 + V23] = 0;

            //power
            data[119 + V23] = 220;
            data[120 + V23] = 0;
            data[121 + V23] = 0;
            data[122 + V23] = 0;

            //collected product num
            data[125 + V23] = 13;
            data[126 + V23] = 0;
            data[127 + V23] = 0;
            data[128 + V23] = 0;

            //revolution
            data[133 + V23] = 12;
            data[134 + V23] = 5;
            data[135 + V23] = 0;
            data[136 + V23] = 0;

            //prepare time
            data[141 + V23] = 89;
            data[142 + V23] = 0;
            data[143 + V23] = 0;
            data[144 + V23] = 0;

            //working time
            data[147 + V23] = 71;
            data[148 + V23] = 1;
            data[149 + V23] = 0;
            data[150 + V23] = 0;

            i = 151 + V23;

            len = i + PROTOCOL_CRC_LEN;
            inputDataHeader(data, len, COMMUNICATION_TYPE_MACHINE_STATUS_TO_PC, 0);

            CRC16.addCrcCode(data, len);
            socketArray[selectedMachineIndex].Send(data, len, 0);
            hintThenVanish("工单报工成功！");
        }

        //prepare time
        private void button10_Click(object sender, EventArgs e)
        {
            int len;

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后才能允许首检合格.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            len = MIN_PACKET_LEN;
            inputDataHeader(data, len, COMMUNICATION_TYPE_ADJEST_TIME_TO_EDS, 0);
            data[PROTOCOL_DATA_POS] = (byte)'0';

            CRC16.addCrcCode(data, len);
            socketArray[selectedMachineIndex].Send(data, len, 0);
            hintThenVanish("首检合格数据上传成功！");
        }

        //Andon signed
        private void button12_Click(object sender, EventArgs e)
        {
            {
                data[4] = 27;
                data[5] = 0;
                data[6] = (byte)COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_DEAL_TO_PC;
                data[7] = (byte)'1';
                data[8] = (byte)';';
                data[9] = (byte)'1';
                data[10] = (byte)';';
                data[11] = (byte)'a';
                data[12] = (byte)'d';
                data[13] = (byte)'m';
                data[14] = (byte)'i';
                data[15] = (byte)'n';
                data[16] = (byte)';';
                data[17] = (byte)'d';
                data[18] = (byte)'i';
                data[19] = (byte)'s';
                data[20] = (byte)'c';
                data[21] = (byte)'u';
                data[22] = (byte)'s';
                data[23] = (byte)'s';
                data[24] = (byte)';';

                CRC16.addCrcCode(data, 27);
                socketArray[selectedMachineIndex].Send(data, 27, 0);
            }

        }

        //Andon completed
        private void button11_Click(object sender, EventArgs e)
        {
            data[4] = 27;
            data[5] = 0;
            data[6] = (byte)COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_DEAL_TO_PC;
            data[7] = (byte)'1';
            data[8] = (byte)';';
            data[9] = (byte)'2';
            data[10] = (byte)';';
            data[11] = (byte)'a';
            data[12] = (byte)'d';
            data[13] = (byte)'m';
            data[14] = (byte)'i';
            data[15] = (byte)'n';
            data[16] = (byte)';';
            data[17] = (byte)'d';
            data[18] = (byte)'i';
            data[19] = (byte)'s';
            data[20] = (byte)'c';
            data[21] = (byte)'u';
            data[22] = (byte)'s';
            data[23] = (byte)'s';
            data[24] = (byte)';';

            CRC16.addCrcCode(data, 27);
            socketArray[selectedMachineIndex].Send(data, 27, 0);

        }

        //Andon cancelled
        private void button13_Click(object sender, EventArgs e)
        {
            data[4] = 27;
            data[5] = 0;
            data[6] = (byte)COMMUNICATION_TYPE_DEVICE_ANDON_INQUIRY_DEAL_TO_PC;
            data[7] = (byte)'1';
            data[8] = (byte)';';
            data[9] = (byte)'3';
            data[10] = (byte)';';
            data[11] = (byte)'a';
            data[12] = (byte)'d';
            data[13] = (byte)'m';
            data[14] = (byte)'i';
            data[15] = (byte)'n';
            data[16] = (byte)';';
            data[17] = (byte)'d';
            data[18] = (byte)'i';
            data[19] = (byte)'s';
            data[20] = (byte)'c';
            data[21] = (byte)'u';
            data[22] = (byte)'s';
            data[23] = (byte)'s';
            data[24] = (byte)';';

            CRC16.addCrcCode(data, 27);
            socketArray[selectedMachineIndex].Send(data, 27, 0);

        }

        //input normal data for SPC checking
        private void button14_Click(object sender, EventArgs e)
        {
            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后能够输入质量参数.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            sendQualityData(SPCCollectedData);
        }

        private void sendQualityData(int [] SPCCollectedData)
        {
            int i, j, k;
            int len;
            int index;
            int num;
            int v;
            string str;
            byte[] buf = new byte[1000];

            num = SPCCollectedData.Length;

            for (i = 0; i < 30; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    index = PROTOCOL_DATA_POS;

                    v = SPCCollectedData[i * 9 + j];

                    str = v + ";" + v + ";" + v + ";" + v + ";" + v + ";" + v + ";" + v + ";" + v + ";" + "DFEC20170204;";

                    buf = System.Text.Encoding.Default.GetBytes(str);
                    for (k = 0; k < str.Length; k++)
                    {
                        data[index++] = (byte)buf[k];
                    }

                    len = MIN_PACKET_LEN_MINUS_ONE + str.Length;
                    inputDataHeader(data, len, COMMUNICATION_TYPE_QUALITY_DATA_TO_PC, 0);

                    CRC16.addCrcCode(data, len);
                    socketArray[selectedMachineIndex].Send(data, len, 0);

                    Thread.Sleep(100);
                }
            }
            communicationStatusArray[selectedMachineIndex] = dispatchStarted;
        }

        //create a new machine connection
        private void button17_Click(object sender, EventArgs e)
        {
            int i;
            int newCreatedMachineID;

            if (isDigitalNum(textBox2.Text) != 1)
            {
                MessageBox.Show("请在模拟通讯设备栏位中输入数字!", "信息提示", MessageBoxButtons.OK);
                return;
            }

            newCreatedMachineID = Convert.ToInt16(textBox2.Text);
            for (i = 0; i < currentCreatedNum; i++ )
            {
                if (newCreatedMachineID == createdMachineArray[i])
                {
                    MessageBox.Show("该设备已存在，不需要新增!", "信息提示", MessageBoxButtons.OK);
                    return;
                }
            }

            createdMachineArray[currentCreatedNum] = newCreatedMachineID;
            currentCreatedNum++;

            comboBox1.Items.Clear();
            for (i = 0; i < currentCreatedNum; i++)
            {
                comboBox1.Items.Add(createdMachineArray[i].ToString() + "号设备");
            }

            comboBox1.SelectedIndex = selectedComboBoxIndex;

        }


        //a new selection in the combo box
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedComboBoxIndex = comboBox1.SelectedIndex;
            selectedMachineIndex = createdMachineArray[selectedComboBoxIndex] - 1;
            selectedMachineID = selectedMachineIndex + 1;
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

//        string _caption;

        private void hintThenVanish(string str)
        {
            string caption = "提示信息";

//            startTimer();

            MessageBox.Show(str, caption, MessageBoxButtons.OK);
        }


        /*
        private void startTimer()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Enabled = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            IntPtr ptr = FindWindow(null, this._caption);
            if (ptr != IntPtr.Zero)
            {
                PostMessage(ptr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }

            ((System.Windows.Forms.Timer)sender).Enabled = false;
        }
        */

        //spc overflow for xbar-s
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后能够输入质量参数.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            sendQualityData(SPCCollectedData_spc_overflow);
        }

        //spc one direction for xbar-s
        private void button18_Click(object sender, EventArgs e)
        {
            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后能够输入质量参数.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            sendQualityData(SPCCollectedData_one_side1);
        }

        //spc one trend for xbar-s
        private void button19_Click(object sender, EventArgs e)
        {
            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后能够输入质量参数.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            sendQualityData(SPCCollectedData_one_trend1);
        }

        //spc small change for xbar-s
        private void button20_Click(object sender, EventArgs e)
        {
            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后能够输入质量参数.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            sendQualityData(SPCCollectedData_small_change1);
        }

        //spc locate apart for xbar-s
        private void button21_Click(object sender, EventArgs e)
        {
            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后能够输入质量参数.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            sendQualityData(SPCCollectedData_locate_apart1);
        }

        //craft data upload 
        private void button22_Click(object sender, EventArgs e)
        {
        }

        //spc data overflow for c chart
        private void button27_Click(object sender, EventArgs e)
        {
            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后能够输入质量参数.", "信息提示", MessageBoxButtons.OK);
                return;
            }

        }

        //spc one side for c chart
        private void button26_Click(object sender, EventArgs e)
        {
            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后能够输入质量参数.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            sendQualityData(SPCCollectedData_one_side2);
        }

        //spc one trend for c chart
        private void button25_Click(object sender, EventArgs e)
        {
            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后能够输入质量参数.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            sendQualityData(SPCCollectedData_one_trend2);
        }

        //spc small change for c chart
        private void button24_Click(object sender, EventArgs e)
        {
            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后能够输入质量参数.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            sendQualityData(SPCCollectedData_small_change2);
        }

        //spc locate apart for c chart
        private void button23_Click(object sender, EventArgs e)
        {
            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后能够输入质量参数.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            sendQualityData(SPCCollectedData_locate_apart2);
        }

        //ADC1 upper overflow
        private void button28_Click(object sender, EventArgs e)
        {
            adcOverflowFlag[0] = 1;
        }

        //ADC2 upper overflow
        private void button29_Click(object sender, EventArgs e)
        {
            adcOverflowFlag[1] = 1;
        }

        //ADC3 upper overflow
        private void button30_Click(object sender, EventArgs e)
        {
            adcOverflowFlag[2] = 1;
        }

        //ADC4 upper overflow
        private void button31_Click(object sender, EventArgs e)
        {
            adcOverflowFlag[3] = 1;
        }

        //ADC1 lower overflow
        private void button35_Click(object sender, EventArgs e)
        {
            adcOverflowFlag[0] = 2;
        }

        //ADC2 lower overflow
        private void button34_Click(object sender, EventArgs e)
        {
            adcOverflowFlag[1] = 2;
        }

        //ADC3 lower overflow
        private void button33_Click(object sender, EventArgs e)
        {
            adcOverflowFlag[2] = 2;
        }

        //ADC4 lower overflow
        private void button32_Click(object sender, EventArgs e)
        {
            adcOverflowFlag[3] = 2;
        }

        //beat testing with currrent value from database
        private void button36_Click(object sender, EventArgs e)
        {
            string str;

            volCurIndex = 0;

            str = textBox3.Text.Trim();
            if (isDigitalNum(str) == 1)
            {
                beatStartIndex = Convert.ToInt32(str);
                if (beatStartIndex == 0)
                    beatStartIndex = 1;
            }
            else
                beatStartIndex = 1;

            beatTestFlag = 1;
        }

        //beat testing with currrent value from file
        private void button37_Click(object sender, EventArgs e)
        {
            string str;

            volCurIndex = 0;

            str = textBox3.Text.Trim();
            if (isDigitalNum(str) == 1)
            {
                beatStartIndex = Convert.ToInt16(str);
                if (beatStartIndex == 0)
                    beatStartIndex = 1;
            }
            else
                beatStartIndex = 1;

            beatTestFlag = 1;
        }

        float getCurrentValue(int index)
        {
            int id;
            string databaseName;
            string tableName;
            string[] record;

            id = selectedMachineIndex;
            databaseName = databaseHeader + (id + 1).ToString().PadLeft(3, '0');
            tableName = "dummy_volcur";
            record = mySQLClass.readDataFromDatabase(databaseName, tableName, index);

            if (record == null)
                return -1;

            return (float)(Convert.ToDouble(record[3]));
        }

        //current value overflow
        private void button38_Click(object sender, EventArgs e)
        {
            currentOverflowFlag = 1;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            communicationFrequency = (int)(1000 / communicationFreqArray[comboBox2.SelectedIndex]);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //beat value upload
        private void button39_Click(object sender, EventArgs e)
        {
            //int v;
            int i;
            int len;

            if (communicationStatusArray[selectedMachineIndex] < handshakeOK)
            {
                MessageBox.Show("设备尚未握手成功，请先开始模拟通讯.", "信息提示", MessageBoxButtons.OK);
                return;
            }
            if (communicationStatusArray[selectedMachineIndex] < dispatchStarted)
            {
                MessageBox.Show("只有工单启动后才能上传节拍.", "信息提示", MessageBoxButtons.OK);
                return;
            }

            if (communicationStatusArray[selectedMachineIndex] >= dispatchStarted)
            {
                Random rand = new Random();

                i = PROTOCOL_DATA_POS;

//                v = rand.Next(200, 250);

                len = MIN_PACKET_LEN_MINUS_ONE + 4;
                inputDataHeader(data, len, COMMUNICATION_TYPE_CYCLE_TIME_TO_PC, 0);
                data[i++] = 55;
                data[i++] = 0;
                data[i++] = 0;
                data[i++] = 0;

                CRC16.addCrcCode(data, len);
                socketArray[selectedMachineIndex].Send(data, len, 0);
            }
            hintThenVanish("不合格品节拍数据上传成功！");
        }
    }
}
