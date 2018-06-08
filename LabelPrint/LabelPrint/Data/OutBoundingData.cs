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


    public class OutBoundingInputData : ProcessData
    {
        //public String RawMaterialName;
        public String RawMaterialGrade;
        public String RawMaterialBatchNo;
        //public String RawMaterialNo;
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

        //再造料标签：
        //包含的栏位内容：客户名称、配方，产品批号、生产日期、重量
        public OutBoundingInputData()
        {
            ColumnList = Column;
            TableName = "OutBounding";
        }
        public override void UpdatePrintPrintData(DynamicPrintLabelData DyData)
        {
            DyData.CustomName = Vendor;
           // DyData.Recipe = Recipe;
            DyData.BatchNo = RawMaterialBatchNo;
            DyData.DataTime = Date_Time;
           // DyData.RollWeightLength = StackWeight;
        }


        static String[] Column =
        {
            "SN",
            "Date",
            "Time",
            "RawMaterialName",
            "RawMaterialGrade",
            "RawMaterialBatchNo",
            "RawMaterialNo",
            "TargetMachineNo",
            "LiaoCangNo",
            //"WorkProcess",
            //"Recipe",
            //"Color",
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
        };
        enum ColumnType
        {
            SN,
            DATE,
            TIME,
            RAWMATERIALNAME,
            RAWMATERIALGRADE,
            RAWMATERIALBATCHNO,
            RAWMATERIALNO,
            TARGETMACHINENO,
            LIAOCANGNO,
            //RECIPE,
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
            MAX_COLUMN
        };
        public override String[] SetColumnDataArray()
        {
            String[] values = new String[Column.Length];
            System.Diagnostics.Debug.Assert((int)ColumnType.MAX_COLUMN == Column.Length);
            values[(int)ColumnType.DATE] = DateTime.Now.ToString("yyyy-MM-dd");
            values[(int)ColumnType.TIME] = DateTime.Now.ToString("HH-mm-ss");
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
            MaterialName = dataRows[0][(int)ColumnType.RAWMATERIALNAME + offset].ToString();
            RawMaterialGrade = dataRows[0][(int)ColumnType.RAWMATERIALGRADE + offset].ToString();
            RawMaterialBatchNo = dataRows[0][(int)ColumnType.RAWMATERIALBATCHNO + offset].ToString();
            LiaoCangNo = dataRows[0][(int)ColumnType.LIAOCANGNO + offset].ToString();
            //Recipe = dataRows[0][(int)ColumnType.RECIPE + offset].ToString();

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
