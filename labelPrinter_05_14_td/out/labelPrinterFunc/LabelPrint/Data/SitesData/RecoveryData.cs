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
namespace LabelPrint.Data
{

    public partial class RcvInputData : ProcessData
    {
        //public String RawMaterialName;
        //public String RawMaterialGrade;
        //public String RawMaterialBatchNo;
        //public String RawMaterialNo;


        public String WorkProcess;
        public String Recipe;
        public String Color;
        //public String Vendor;
        public String WeightPerBag;
        //public String StackWeight;
        //public String Bags_x;
        //public String Bags_y;
        //public String Bags_xy;
        //public String Worker_No;
        //public String Date_Time;
        public String Desc;
        public String RecoveryMachineNo;

        public String OldCode1;
        public String OldCode2;
        public String OldCode3;
        public String OldCode4;
        public String OldCode5;
        public String OldCode6;
        public String OldCode7;
        public String OldCode8;
        public String OldCode9;
        public String OldCode10;

        //再造料标签：
        //包含的栏位内容：客户名称、配方，产品批号、生产日期、重量
        public RcvInputData()
        {
            ColumnList = Column;
            TableName = "Recovery";
        }



        static String[] Column =
        {
            "SN",
            "Date",
            "Time",
            "MState",
           // "WorkProcess",
            "Recipe",
            "Color",
            //"Vendor",
            "WeightPerBag",
            //"StackWeight",
            //"Bags_x",
            //"Bags_y",
            //"Bags_xy",
            "Worker_No",
            "WorkNo",
            "Description",
            "InputBarcode",
            "OutputBarcode",
                "OldCode1",
                "OldCode2",
                "OldCode3",
                "OldCode4",
                "OldCode5",
                "OldCode6",
                "OldCode7",
                "OldCode8",
                "OldCode9",
                "OldCode10",
                "RecoveryMachineNo",
        };
        enum ColumnType
        {
            SN,
            Date,
            Time,
            MState,
          //  WorkProcess,
            Recipe,
            Color,
            //VENDOR,
            WeightPerBag,
            //STACKWEIGHT,
            //STACKHEIGHT,
            //BAGS_X,
            //BAGS_Y,
            ////BAGS_Z,
            //BAGS_XY,
            //PLATE_X,
            //PLATE_Y,
            //PLATE_XY,
            Worker_No,
            WorkNo,
            Description,
            InputBarcode,
            OutputBarcode,
            OldCode1,
            OldCode2,
            OldCode3,
            OldCode4,
            OldCode5,
            OldCode6,
            OldCode7,
            OldCode8,
            OldCode9,
            OldCode10,
            RecoveryMachineNo,
            MAX_COLUMN
        };

        

        //public void CreateDateTable()
        //{
        //    CreateDataTable(TableName, Column);
        //}

        public override String[] SetColumnDataArray()
        {
            String[] values = new String[Column.Length];
            System.Diagnostics.Debug.Assert((int)ColumnType.MAX_COLUMN == Column.Length);
            values[(int)ColumnType.Date] = WorkDate;// DateTime.Now.ToString("yyyy-MM-dd");
            values[(int)ColumnType.Time] = WorkTime;// DateTime.Now.ToString("HH-mm-ss");
            values[(int)ColumnType.MState] = MState;
            
         //   values[(int)ColumnType.WorkProcess] = WorkProcess;
            values[(int)ColumnType.Recipe] = Recipe;
            values[(int)ColumnType.Color] = Color;
            //values[(int)ColumnType.VENDOR] = Vendor;
            values[(int)ColumnType.WeightPerBag] = WeightPerBag;
            //values[(int)ColumnType.STACKWEIGHT] = StackWeight;
            //values[(int)ColumnType.BAGS_X] = Bags_x;
            //values[(int)ColumnType.BAGS_Y] = Bags_y;
            //values[(int)ColumnType.BAGS_XY] = Bags_xy;
            values[(int)ColumnType.Worker_No] = WorkerNo;
            values[(int)ColumnType.WorkNo] = WorkNo;
            values[(int)ColumnType.Description] = Desc;
            values[(int)ColumnType.InputBarcode] = InputBarcode;
            values[(int)ColumnType.OutputBarcode] = OutputBarcode;

            values[(int)ColumnType.OldCode1] = OldCode1;
            values[(int)ColumnType.OldCode2] = OldCode2;
            values[(int)ColumnType.OldCode3] = OldCode3;
            values[(int)ColumnType.OldCode4] = OldCode4;
            values[(int)ColumnType.OldCode5] = OldCode5;
            values[(int)ColumnType.OldCode6] = OldCode6;
            values[(int)ColumnType.OldCode7] = OldCode7;
            values[(int)ColumnType.OldCode8] = OldCode8;
            values[(int)ColumnType.OldCode9] = OldCode9;
            values[(int)ColumnType.OldCode10] = OldCode10;


            values[(int)ColumnType.RecoveryMachineNo] = RecoveryMachineNo;
            return values;
        }
        public override void SetInputDataFromDataBase(DataTable dt)
        {
            int offset = 1;
            DataRowCollection dataRows = dt.Rows;
            WorkDate = dataRows[0][(int)ColumnType.Date + offset].ToString();
            WorkTime = dataRows[0][(int)ColumnType.Time + offset].ToString();
            MState = dataRows[0][(int)ColumnType.MState + offset].ToString();


         //   WorkProcess = dataRows[0][(int)ColumnType.WorkProcess + offset].ToString();
            Recipe = dataRows[0][(int)ColumnType.Recipe + offset].ToString();
            Color = dataRows[0][(int)ColumnType.Color + offset].ToString();
            //Vendor = dataRows[0][(int)ColumnType.VENDOR + offset].ToString();
            //StackWeight = dataRows[0][(int)ColumnType.STACKWEIGHT + offset].ToString();
            WeightPerBag = dataRows[0][(int)ColumnType.WeightPerBag + offset].ToString();
            //Bags_x = dataRows[0][(int)ColumnType.BAGS_X + offset].ToString();
            //Bags_y = dataRows[0][(int)ColumnType.BAGS_Y + offset].ToString();
            //Bags_xy = dataRows[0][(int)ColumnType.BAGS_XY + offset].ToString();
            WorkerNo = dataRows[0][(int)ColumnType.Worker_No + offset].ToString();
            WorkNo = dataRows[0][(int)ColumnType.WorkNo + offset].ToString();
            Desc = dataRows[0][(int)ColumnType.Description + offset].ToString();

            InputBarcode = dataRows[0][(int)ColumnType.InputBarcode + offset].ToString();
            OutputBarcode = dataRows[0][(int)ColumnType.OutputBarcode + offset].ToString();


            OldCode1 = dataRows[0][(int)ColumnType.OldCode1 + offset].ToString();
            OldCode2 = dataRows[0][(int)ColumnType.OldCode2 + offset].ToString();
            OldCode3 = dataRows[0][(int)ColumnType.OldCode3 + offset].ToString();
            OldCode4 = dataRows[0][(int)ColumnType.OldCode4 + offset].ToString();
            OldCode5 = dataRows[0][(int)ColumnType.OldCode5 + offset].ToString();
            OldCode6 = dataRows[0][(int)ColumnType.OldCode6 + offset].ToString();
            OldCode7 = dataRows[0][(int)ColumnType.OldCode7 + offset].ToString();
            OldCode8 = dataRows[0][(int)ColumnType.OldCode8 + offset].ToString();
            OldCode9 = dataRows[0][(int)ColumnType.OldCode9 + offset].ToString();
            OldCode10 = dataRows[0][(int)ColumnType.OldCode10 + offset].ToString();
            RecoveryMachineNo = dataRows[0][(int)ColumnType.RecoveryMachineNo + offset].ToString();
        }
        public override void PrintLabel()
        {
            RecoveryLabel label = new RecoveryLabel();
            label.Printlabel(this);
        }
        public override Boolean ParseBarCode(String barcode)
        {
            //XXXXXXXXXX(工单编码) + X（工序）+X（机台号）+XXXXXXXX（日期）+XX（卷号）+XXX（分卷号）+X（客户序号）+X（质量编码）；
            if (barcode.Length == LittleRollBarcode.getTotalStrLen())
            {
                LittleRollBarcode bar = new LittleRollBarcode();
                if (!bar.ParseBarcode(barcode))
                    return false;
                WorkNo = bar.WorkNoStr;
                //            BigRollNo = bar.BigRStr;
                //          LittleRollNo = bar.LittleRStr;
                BatchNo = bar.BatchNo;
                int workset = int.Parse(bar.VendorStr);
                MType = (workset == 0) ? ManufactureType.M_SINGLE : ManufactureType.M_MULTIPLE;
            }
            else if (barcode.Length == BigRollBarcode.getTotalStrLen())
            {
                BigRollBarcode bar = new BigRollBarcode();
                if (!bar.ParseBarcode(barcode))
                    return false;
                WorkNo = bar.WorkNoStr;
                //            BigRollNo = bar.BigRStr;
                BatchNo = bar.BatchNo;
                MType = ManufactureType.M_SINGLE;
            }
            else
                return false;
            return true;
        }

    }

    class RecoverySysData : ProcessSysData
    {
        String[] PackTitle = { "序号", "生产日期", "时间", "配方", "工号","备注" };
        String[] dataBase = { "id", "Date", "Time", "Recipe",  "Worker_No", "Description" };

        public String Color;
        public String Recipe;
        public String Vendor;
        public String WeightPerBag;
        public String Worker_No;



        String table = "Recovery";

        public override String CreateSelectOpiton()
        {
            String Str = "";
            Str = "Date between '" + Date1 + "' and '" + Date2 + "'";
            //if (Color != "" && Color != null)
            //{
            //    Str += " and " + "Color='" + Color + "'";
            //}
            if (Recipe != "" && Recipe != null)
            {
                Str += " and " + "Recipe='" + Recipe + "'";
            }
            //if (Vendor != "" && Vendor != null)
            //{
            //    Str += " and " + "Vendor='" + Vendor + "'";
            //}
            //if (WeightPerBag != "" && WeightPerBag != null)
            //{
            //    Str += " and " + "WeightPerBag='" + WeightPerBag + "'";
            //}
            if (Worker_No != "" && Worker_No != null)
            {
                Str += " and " + "Worker_No='" + Worker_No + "'";
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
