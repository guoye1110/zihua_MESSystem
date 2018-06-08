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
    public partial class RuKuInputData : OutBoundingInputData
    {

        //public String RawMaterialGrade;
        //public String RawMaterialBatchNo;

        //public String TargetMachineNo;
        //public String Vendor;
        //public String WeightPerBag;
        //public String LiaoCangNo;

        //public String Bags_x;
        //public String Bags_y;

        //public String Bags_xy;

        //public String Date_Time;
        //public String Desc;
        //public String NeedBags;
        //public String OutBags;


        //public String XuQiuWeight;
        //public String YiChuKuWeight;
        //public String BenCiChuKuWeight;


    //    public String[] targets =
    //{
    //        "1号流延机",
    //        "2号流延机",
    //        "3号流延机",
    //        "4号流延机",
    //        "5号流延机",
    //        "6号中试机",
    //        "7号吹膜机",
    //        };


        static public String[] LiaoCangNoStrs =
        {
            "1号料仓",
            "2号料仓",
            "3号料仓",
            "4号料仓",
            "5号料仓",
            "6号料仓",
            "7号料仓",
        };
        //再造料标签：
        //包含的栏位内容：客户名称、配方，产品批号、生产日期、重量
        public RuKuInputData()
        {
         //   ColumnList = Column;
          //  TableName = "OutBounding";
        }


        //static String[] Column =
        //{
        //    "SN",
        //    "Date",
        //    "Time",
        //    "MState",
        //    "RawMaterialName",
        //    "RawMaterialGrade",
        //    "RawMaterialBatchNo",
        //    "RawMaterialNo",
        //    "TargetMachineNo",
        //    "LiaoCangNo",
        //    "Worker_No",
        //    "Description",
        //    "InputBarcode",
        //    "OutputBarcode",
        //    "XuQiuWeight",
        //    "YiChuKuWeight",
        //    "BenCiChuKuWeight",
        //};
        //enum ColumnType
        //{
        //    SN,
        //    Date,
        //    Time,
        //    MState,
        //    RawMaterialName,
        //    RawMaterialGrade,
        //    RawMaterialBatchNo,
        //    RawMaterialNo,
        //    TargetMachineNo,
        //    LiaoCangNo,
        //    Worker_No,
        //    Description,
        //    InputBarcode,
        //    OutputBarcode,
        //    XuQiuWeight,
        //    YiChuKuWeight,
        //    BenCiChuKuWeight,
        //    MAX_COLUMN
        //};
        //public override String[] SetColumnDataArray()
        //{
        //    String[] values = new String[Column.Length];
        //    System.Diagnostics.Debug.Assert((int)ColumnType.MAX_COLUMN == Column.Length);
        //    values[(int)ColumnType.Date] = WorkDate;// DateTime.Now.ToString("yyyy-MM-dd");
        //    values[(int)ColumnType.Time] = WorkTime; // DateTime.Now.ToString("HH-mm-ss");
        //    values[(int)ColumnType.MState] = MState;

        //    values[(int)ColumnType.RawMaterialName] = MaterialName;
        //    values[(int)ColumnType.RawMaterialGrade] = RawMaterialGrade;
        //    values[(int)ColumnType.RawMaterialBatchNo] = RawMaterialBatchNo;
        //    values[(int)ColumnType.RawMaterialNo] = RawMaterialCode;

        //    values[(int)ColumnType.TargetMachineNo] = TargetMachineNo;
        //    values[(int)ColumnType.LiaoCangNo] = LiaoCangNo;

        //    values[(int)ColumnType.Worker_No] = WorkerNo;

        //    values[(int)ColumnType.Description] = Desc;
        //    values[(int)ColumnType.InputBarcode] = InputBarcode;
        //    values[(int)ColumnType.OutputBarcode] = OutputBarcode;


        //    values[(int)ColumnType.XuQiuWeight] = XuQiuWeight;
        //    values[(int)ColumnType.YiChuKuWeight] = YiChuKuWeight;
        //    values[(int)ColumnType.BenCiChuKuWeight] = BenCiChuKuWeight;
        //    return values;
        //}
        /*
         * 出库工序：
XXXXXXX（原料编码）；XX（袋数）； XXX（设备编码）；X（料筐编码）
四个栏位长度不定，中间以“;”分割。

         */
        //public void SetInputDataFromScanner(String barcode)
        //{

        //    string[] barcodes = barcode.Split(';');
        //    RawMaterialCode = barcodes[0];
        //    Bags_xy = barcodes[1];

        //}

        //public String CreateBarcodeFromInputData()
        //{
        //    //  RawMaterialCode+";"+
        //    return null;
        //}

        //public override void SetInputDataFromDataBase(DataTable dt)
        //{
        //    int offset = 1;
        //    DataRowCollection dataRows = dt.Rows;
        //    WorkDate = dataRows[0][(int)ColumnType.Date + offset].ToString();
        //    WorkTime = dataRows[0][(int)ColumnType.Time + offset].ToString();
        //    MState = dataRows[0][(int)ColumnType.MState + offset].ToString();

        //    MaterialName = dataRows[0][(int)ColumnType.RawMaterialName + offset].ToString();
        //    RawMaterialGrade = dataRows[0][(int)ColumnType.RawMaterialGrade + offset].ToString();
        //    RawMaterialBatchNo = dataRows[0][(int)ColumnType.RawMaterialBatchNo + offset].ToString();
        //    LiaoCangNo = dataRows[0][(int)ColumnType.LiaoCangNo + offset].ToString();
        //    //Recipe = dataRows[0][(int)ColumnType.RECIPE + offset].ToString();
        //    RawMaterialCode = dataRows[0][(int)ColumnType.RawMaterialNo + offset].ToString();
        //    TargetMachineNo = dataRows[0][(int)ColumnType.TargetMachineNo + offset].ToString();



        //    WorkerNo = dataRows[0][(int)ColumnType.Worker_No + offset].ToString();

        //    Desc = dataRows[0][(int)ColumnType.Description + offset].ToString();
        //    InputBarcode = dataRows[0][(int)ColumnType.InputBarcode + offset].ToString();
        //    OutputBarcode = dataRows[0][(int)ColumnType.OutputBarcode + offset].ToString();

        //    XuQiuWeight = dataRows[0][(int)ColumnType.XuQiuWeight + offset].ToString();
        //    YiChuKuWeight = dataRows[0][(int)ColumnType.YiChuKuWeight + offset].ToString();
        //    BenCiChuKuWeight = dataRows[0][(int)ColumnType.BenCiChuKuWeight + offset].ToString();

        //}

        //Boolean ParseOutBoundingBarcode()
        //{

        //}

        public override void PrintLabel()
        {
            //FilmPrintLabel label = new FilmPrintLabel();
            //label.Printlabel();
            RukuLabel label = new RukuLabel();
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
    class RukuSysData : ProcessSysData
    {
        String[] PackTitle = { "序号", "工号", "出库日期", "时间", "原料批次号", "供应商", "料仓号", "备注" };
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
