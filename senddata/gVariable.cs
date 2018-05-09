using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace clientFunc
{
    public class gVariable
    {
        public const int CLIENT_PC_WAREHOUSE_ID1 = 101;  //出入库

        public const int CLIENT_PC_FEED_ID1 = 121;  //上料系统
        public const int CLIENT_PC_FEED_ID2 = 122;  //上料系统
        public const int CLIENT_PC_FEED_ID3 = 123;  //上料系统
        public const int CLIENT_PC_FEED_ID4 = 124;  //上料系统
        public const int CLIENT_PC_FEED_ID5 = 125;  //上料系统

        public const int CLIENT_PC_CAST_ID1 = 141;  //流延设备
        public const int CLIENT_PC_CAST_ID2 = 142;  //流延设备
        public const int CLIENT_PC_CAST_ID3 = 143;  //流延设备
        public const int CLIENT_PC_CAST_ID4 = 144;  //流延设备
        public const int CLIENT_PC_CAST_ID5 = 145;  //流延设备

        public const int CLIENT_PC_PRINT_ID1 = 161;  //印刷设备
        public const int CLIENT_PC_PRINT_ID2 = 162;  //印刷设备
        public const int CLIENT_PC_PRINT_ID3 = 163;  //印刷设备

        public const int CLIENT_PC_SLIT_ID1 = 181;  //分切设备
        public const int CLIENT_PC_SLIT_ID2 = 182;  //分切设备
        public const int CLIENT_PC_SLIT_ID3 = 183;  //分切设备
        public const int CLIENT_PC_SLIT_ID4 = 184;  //分切设备
        public const int CLIENT_PC_SLIT_ID5 = 185;  //分切设备

        public const int CLIENT_PC_INSPECTION_ID1 = 201;  //质检工序

        public const int CLIENT_PC_REUSE_ID1 = 221;  //再造料工序

        public const int CLIENT_PC_PACKING_ID1 = 241;  //打包工序

        public struct dispatchSheetStruct
        {
            public string machineID;   //设备序号 
            public string dispatchCode;  //dispatch code
            public string planTime1;	//预计开始时间
            public string planTime2;  //预计结束时间
            public string productCode;	 //产品编码
            public string productName;  //产品名称
            public string operatorName; //操作员
            public int plannedNumber;	//计划生产数量
            public int outputNumber;  //接收数量
            public int qualifiedNumber;  //合格数量
            public int unqualifiedNumber;  //不合格数量
            public string processName; //工序（工艺路线中包含多个工序）
            public string realStartTime;	  //实际开始时间
            public string realFinishTime;	  //实际完工时间
            public string prepareTimePoint;   //调整时间（试产时间），开工后先经过调整时间，然后再正式生产
            public int status;	  //0：dummy/prepared 工单准备完成但未发布，1:工单完工，新工单未发布，2：工单发布至设备 3：工单已启动 4：试产（调整时间） OK
            public int toolLifeTimes;  //刀具寿命次数
            public int toolUsedTimes;  //刀具使用次数
            public int outputRatio;  //单件系数
            public string serialNumber; //流水号
            public string reportor; //报工员工号, 报工员和操作员可能不是同一个人
            public string workshop;	 //车间名称，或者是流水线的名字
            public string workshift;	 //班次（早中晚班）
            public string salesOrderCode; //订单号
            public string BOMCode; // 
            public string customer;
            public string barCode;
            public string barcodeForReuse;
            public string quantityOfReused;
            public string multiProduct;
            public string productCode2;
            public string productCode3;
            public string operatorName2; //操作员
            public string operatorName3; //操作员
        }
    }
}
