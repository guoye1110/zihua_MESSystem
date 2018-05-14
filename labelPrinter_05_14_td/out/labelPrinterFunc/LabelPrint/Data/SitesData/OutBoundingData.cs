using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabelPrint.Util;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using LabelPrint.Receipt;
using System.Data;
using DotNet.Utilities;

namespace LabelPrint.Data
{


    public partial class OutBoundingInputData : ProcessData
    {

        public String RawMaterialGrade;
        public String RawMaterialBatchNo;

        // public String WorkProcess;
        //public String Recipe;
        //public String Color;
        public String TargetMachineNo;
        public String Vendor;
        public String WeightPerBag;
        public String LiaoCangNo;
        //public String StackWeight;
        //public String StackHeight;
        public String Bags_x;
        public String Bags_y;
       // public String Bags_z;
        public String Bags_xy;
        //public String Worker_No;
        public String Date_Time;
        public String Desc;
        public String NeedBags;
        public String OutBags;


        //public String RawMaterialCode1;
        //public String RawMaterialCode2;
        //public String RawMaterialCode3;
        //public String RawMaterialCode4;
        //public String RawMaterialCode5;
        //public String RawMaterialCode6;
        //public String RawMaterialCode7;
        //public String RawMaterialCode8;

        //public String RawMaterialBatchNo1;
        //public String RawMaterialBatchNo2;
        //public String RawMaterialBatchNo3;
        //public String RawMaterialBatchNo4;
        //public String RawMaterialBatchNo5;
        //public String RawMaterialBatchNo6;
        //public String RawMaterialBatchNo7;
        //public String RawMaterialBatchNo8;

        //public String XuQiuWeight1;
        //public String XuQiuWeight2;
        //public String XuQiuWeight3;
        //public String XuQiuWeight4;
        //public String XuQiuWeight5;
        //public String XuQiuWeight6;
        //public String XuQiuWeight7;
        //public String XuQiuWeight8;


        //public String YiChuKuWeight1;
        //public String YiChuKuWeight2;
        //public String YiChuKuWeight3;
        //public String YiChuKuWeight4;
        //public String YiChuKuWeight5;
        //public String YiChuKuWeight6;
        //public String YiChuKuWeight7;
        //public String YiChuKuWeight8;

        //public String BenCiChuKuWeight1;
        //public String BenCiChuKuWeight2;
        //public String BenCiChuKuWeight3;
        //public String BenCiChuKuWeight4;
        //public String BenCiChuKuWeight5;
        //public String BenCiChuKuWeight6;
        //public String BenCiChuKuWeight7;
        //public String BenCiChuKuWeight8;

        public String XuQiuWeight;
        public String YiChuKuWeight;
        public String BenCiChuKuWeight;

        public String[] targets =
            {
            "目标设备1",
            "目标设备2",
            "目标设备3",
            "目标设备4",
            "目标设备5",
            };

        //再造料标签：
        //包含的栏位内容：客户名称、配方，产品批号、生产日期、重量
        public OutBoundingInputData()
        {
            ColumnList = Column;
            TableName = "OutBounding";
        }

        public Boolean CheckUserInput()
        {

            return true;
        }



        static String[] Column =
        {
            "SN",
            "Date",
            "Time",
            "MState",
            "RawMaterialName",

            "RawMaterialGrade",
            "RawMaterialBatchNo",
            "RawMaterialNo",
            "TargetMachineNo",
            "LiaoCangNo",

            "Vendor",
            "WeightPerBag",

            //"StackWeight",
            "Bags_x",
            "Bags_y",
            "Bags_xy",

            "Worker_No",
            "NeedBags",
            "OutBags",
            "Description",
            "InputBarcode",

            "OutputBarcode",
        //    "RawMaterialCode",
            "XuQiuWeight",
            "YiChuKuWeight",
            "BenCiChuKuWeight",

//"RawMaterialCode1",
//"RawMaterialCode2",
//"RawMaterialCode3",
//"RawMaterialCode4",
//"RawMaterialCode5",
//"RawMaterialCode6",
//"RawMaterialCode7",
//"RawMaterialCode8",


//"RawMaterialBatchNo1",
//"RawMaterialBatchNo2",
//"RawMaterialBatchNo3",
//"RawMaterialBatchNo4",
//"RawMaterialBatchNo5",
//"RawMaterialBatchNo6",
//"RawMaterialBatchNo7",
//"RawMaterialBatchNo8",


//"XuQiuWeight1",
//"XuQiuWeight2",
//"XuQiuWeight3",
//"XuQiuWeight4",
//"XuQiuWeight5",
//"XuQiuWeight6",
//"XuQiuWeight7",
//"XuQiuWeight8",



//"YiChuKuWeight1",
//"YiChuKuWeight2",
//"YiChuKuWeight3",
//"YiChuKuWeight4",
//"YiChuKuWeight5",
//"YiChuKuWeight6",
//"YiChuKuWeight7",
//"YiChuKuWeight8",


//"BenCiChuKuWeight1",
//"BenCiChuKuWeight2",
//"BenCiChuKuWeight3",
//"BenCiChuKuWeight4",
//"BenCiChuKuWeight5",
//"BenCiChuKuWeight6",
//"BenCiChuKuWeight7",
//"BenCiChuKuWeight8"
        };
        enum ColumnType
        {
            SN,
            DATE,
            TIME,
            MState,
            RAWMATERIALNAME,

            RAWMATERIALGRADE,
            RAWMATERIALBATCHNO,
            RAWMATERIALNO,
            TARGETMACHINENO,
            LIAOCANGNO,

            VENDOR,
            WEIGHTPERBAG,
            //STACKWEIGHT,
            BAGS_X,
            BAGS_Y,
            BAGS_XY,

            WORKER_NO,
            NEEDBAGS,
            OUTBAGS,
            DESC,
            INPUTBARCODE,

            OUTPUTBARCODE,
          //  RawMaterialCode,
            XuQiuWeight,
            YiChuKuWeight,
            BenCiChuKuWeight,
            MAX_COLUMN
        };
        public override String[] SetColumnDataArray()
        {
            String[] values = new String[Column.Length];
            System.Diagnostics.Debug.Assert((int)ColumnType.MAX_COLUMN == Column.Length);
            values[(int)ColumnType.DATE] = WorkDate;// DateTime.Now.ToString("yyyy-MM-dd");
            values[(int)ColumnType.TIME] = WorkTime; // DateTime.Now.ToString("HH-mm-ss");
            values[(int)ColumnType.MState] = MState;
            values[(int)ColumnType.RAWMATERIALNAME] = MaterialName;
            values[(int)ColumnType.RAWMATERIALGRADE] = RawMaterialGrade;
            values[(int)ColumnType.RAWMATERIALBATCHNO] = RawMaterialBatchNo;
            values[(int)ColumnType.RAWMATERIALNO] = RawMaterialCode;

            values[(int)ColumnType.TARGETMACHINENO] = TargetMachineNo;
            values[(int)ColumnType.LIAOCANGNO] = LiaoCangNo;
            //values[(int)ColumnType.RECIPE] = Recipe;
            values[(int)ColumnType.VENDOR] = Vendor;
            values[(int)ColumnType.WEIGHTPERBAG] = WeightPerBag;
            //values[(int)ColumnType.STACKWEIGHT] = StackWeight;
            values[(int)ColumnType.BAGS_X] = Bags_x;
            values[(int)ColumnType.BAGS_Y] = Bags_y;
            values[(int)ColumnType.BAGS_XY] = Bags_xy;
            values[(int)ColumnType.WORKER_NO] = WorkerNo;
            values[(int)ColumnType.NEEDBAGS] = NeedBags;
            values[(int)ColumnType.OUTBAGS] = OutBags;
            values[(int)ColumnType.DESC] = Desc;
            values[(int)ColumnType.INPUTBARCODE] = InputBarcode;
            values[(int)ColumnType.OUTPUTBARCODE] = OutputBarcode;

            values[(int)ColumnType.XuQiuWeight] = XuQiuWeight;
            values[(int)ColumnType.YiChuKuWeight] = YiChuKuWeight;
            values[(int)ColumnType.BenCiChuKuWeight] = BenCiChuKuWeight;

            return values;
        }
        /*
         * 出库工序：
XXXXXXX（原料编码）；XX（袋数）； XXX（设备编码）；X（料筐编码）
四个栏位长度不定，中间以“;”分割。

         */
        public void SetInputDataFromScanner(String barcode)
        {
           
            string[] barcodes = barcode.Split(';');
            RawMaterialCode = barcodes[0];
            Bags_xy = barcodes[1];

        }

        public String CreateBarcodeFromInputData()
        {
            //  RawMaterialCode+";"+
            return null;
        }

        public override void SetInputDataFromDataBase(DataTable dt)
        {
            int offset = 1;
            DataRowCollection dataRows = dt.Rows;
            WorkDate = dataRows[0][(int)ColumnType.DATE + offset].ToString();
            WorkTime = dataRows[0][(int)ColumnType.TIME + offset].ToString();
            MState = dataRows[0][(int)ColumnType.MState + offset].ToString(); 

            MaterialName = dataRows[0][(int)ColumnType.RAWMATERIALNAME + offset].ToString();
            RawMaterialGrade = dataRows[0][(int)ColumnType.RAWMATERIALGRADE + offset].ToString();
            RawMaterialBatchNo = dataRows[0][(int)ColumnType.RAWMATERIALBATCHNO + offset].ToString();
            LiaoCangNo = dataRows[0][(int)ColumnType.LIAOCANGNO + offset].ToString();
            //Recipe = dataRows[0][(int)ColumnType.RECIPE + offset].ToString();
            RawMaterialCode = dataRows[0][(int)ColumnType.RAWMATERIALNO + offset].ToString();
            TargetMachineNo = dataRows[0][(int)ColumnType.TARGETMACHINENO + offset].ToString();
            

            Vendor = dataRows[0][(int)ColumnType.VENDOR + offset].ToString();
            WeightPerBag = dataRows[0][(int)ColumnType.WEIGHTPERBAG + offset].ToString();
            //StackWeight = dataRows[0][(int)ColumnType.STACKWEIGHT + offset].ToString();
            Bags_x = dataRows[0][(int)ColumnType.BAGS_X + offset].ToString();
            Bags_y = dataRows[0][(int)ColumnType.BAGS_Y + offset].ToString();
            Bags_xy = dataRows[0][(int)ColumnType.BAGS_XY + offset].ToString();
            WorkerNo = dataRows[0][(int)ColumnType.WORKER_NO + offset].ToString();
            NeedBags = dataRows[0][(int)ColumnType.NEEDBAGS + offset].ToString();
            OutBags = dataRows[0][(int)ColumnType.OUTBAGS + offset].ToString();
            Desc = dataRows[0][(int)ColumnType.DESC + offset].ToString();
            InputBarcode = dataRows[0][(int)ColumnType.INPUTBARCODE + offset].ToString();
            OutputBarcode = dataRows[0][(int)ColumnType.OUTPUTBARCODE + offset].ToString();

           // RawMaterialCode = dataRows[0][(int)ColumnType.RawMaterialCode + offset].ToString();
            XuQiuWeight = dataRows[0][(int)ColumnType.XuQiuWeight + offset].ToString();
            YiChuKuWeight = dataRows[0][(int)ColumnType.YiChuKuWeight + offset].ToString();
            BenCiChuKuWeight = dataRows[0][(int)ColumnType.BenCiChuKuWeight + offset].ToString();
        }

        public override void PrintLabel()
        {
            //FilmPrintLabel label = new FilmPrintLabel();
            //label.Printlabel();
            OutBoundingLabel label = new OutBoundingLabel();
            label.Printlabel(this);
        }
    }
    /*
     * 2. 原料出库：
   查询条件：日期范围/原料批次号/供应商/目标设备/料仓号
   列表信息：序号/工号/出库日期/时间/原料批次号/供应商/目标设备/料仓号/袋数/已出库/总需求
   标签画面：
     a. 配方号和码垛重不需要了。
     b. 打印模式不需要选择，只有手动模式。
     c. 出库袋数，改成已出库袋数。
     d. 在料仓编号后的编辑框后增加"袋"字。
     e. 增加切换至"余料入库"按钮，可切换至余料入库画面。
     */

    //目标设备  has not implement.
    class OutBoundingSysData : ProcessSysData
    {
        String[] PackTitle = { "序号", "工号", "出库日期", "时间", "原料批次号","供应商", "料仓号", "备注"};
        String[] dataBase = { "id", "Worker_No", "Date", "Time", "RawMaterialBatchNo", "Vendor", "LiaoCangNo", "Description" };

        public String RawMaterialBatchNo;
        public String Vendor;
        public String TargetMachineNo;
        public String LiaoCangNo;



        String table = "OutBounding";

        public override String CreateSelectOpiton()
        {
            String Str = "";
            Str = "Date between '" + Date1 + "' and '" + Date2 + "'";
            if (RawMaterialBatchNo != "" && RawMaterialBatchNo != null)
            {
                Str += " and " + "RawMaterialBatchNo='" + RawMaterialBatchNo + "'";
            }
            //if (Recipe != "" && Recipe != null)
            //{
            //    Str += " and " + "Recipe='" + Recipe + "'";
            //}
            if (Vendor != "" && Vendor != null)
            {
                Str += " and " + "Vendor='" + Vendor + "'";
            }
            if (TargetMachineNo != "" && TargetMachineNo != null)
            {
                Str += " and " + "TargetMachineNo='" + TargetMachineNo + "'";
            }
            if (LiaoCangNo != "" && LiaoCangNo != null)
            {
                Str += " and " + "LiaoCangNo='" + LiaoCangNo + "'";
            }
            return Str;
        }
        public override String CreateSlectStrForColumn()
        {
            String SelStr = createSelectStr(table, dataBase, PackTitle);
            return SelStr;
        }
    }
}
