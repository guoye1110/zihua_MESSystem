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

    public class RcvInputData : ProcessData
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
        //再造料标签：
        //包含的栏位内容：客户名称、配方，产品批号、生产日期、重量
        public RcvInputData()
        {
            ColumnList = Column;
            TableName = "Recovery";
        }
#pragma warning disable CS0114 // 'RcvInputData.UpdatePrintPrintData(DynamicPrintLabelData)' hides inherited member 'ProcessData.UpdatePrintPrintData(DynamicPrintLabelData)'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.
        void UpdatePrintPrintData(DynamicPrintLabelData DyData)
#pragma warning restore CS0114 // 'RcvInputData.UpdatePrintPrintData(DynamicPrintLabelData)' hides inherited member 'ProcessData.UpdatePrintPrintData(DynamicPrintLabelData)'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.
        {
           // DyData.CustomName = Vendor;
            DyData.Recipe = Recipe;
            DyData.DataTime = WorkDate +""+ WorkTime;
           // DyData.RollWeightLength = StackWeight;
        }

        static String[] Column =
        {
            "SN",
            "Date",
            "Time",
        "WorkProcess",
        "Recipe",
        "Color",
        //"Vendor",
        "WeightPerBag",
        //"StackWeight",
        //"Bags_x",
        //"Bags_y",
        //"Bags_xy",
        "Worker_No",
        "Description",
        };
        enum ColumnType
        {
            SN,
            DATE,
            TIME,
            WORKPROCESS,
            RECIPE,
            COLOR,
            //VENDOR,
            WEIGHTPERBAG,
            //STACKWEIGHT,
            //STACKHEIGHT,
            //BAGS_X,
            //BAGS_Y,
            ////BAGS_Z,
            //BAGS_XY,
            //PLATE_X,
            //PLATE_Y,
            //PLATE_XY,
            WORKER_NO,
            DESC,
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
            values[(int)ColumnType.DATE] = DateTime.Now.ToString("yyyy-MM-dd");
            values[(int)ColumnType.TIME] = DateTime.Now.ToString("HH-mm-ss");
            values[(int)ColumnType.WORKPROCESS] = WorkProcess;
            values[(int)ColumnType.RECIPE] = Recipe;
            values[(int)ColumnType.COLOR] = Color;
            //values[(int)ColumnType.VENDOR] = Vendor;
            values[(int)ColumnType.WEIGHTPERBAG] = WeightPerBag;
            //values[(int)ColumnType.STACKWEIGHT] = StackWeight;
            //values[(int)ColumnType.BAGS_X] = Bags_x;
            //values[(int)ColumnType.BAGS_Y] = Bags_y;
            //values[(int)ColumnType.BAGS_XY] = Bags_xy;
            values[(int)ColumnType.WORKER_NO] = WorkerNo;
            //values[(int)ColumnType.DATETIME] = Date_Time;
            values[(int)ColumnType.DESC] = Desc;
            return values;
        }
        public override void SetInputDataFromDataBase(DataTable dt)
        {
            int offset = 1;
            DataRowCollection dataRows = dt.Rows;
            WorkDate = dataRows[0][(int)ColumnType.DATE + offset].ToString();
            WorkTime = dataRows[0][(int)ColumnType.TIME + offset].ToString();
            WorkProcess = dataRows[0][(int)ColumnType.WORKPROCESS + offset].ToString();
            Recipe = dataRows[0][(int)ColumnType.RECIPE + offset].ToString();
            Color = dataRows[0][(int)ColumnType.COLOR + offset].ToString();
            //Vendor = dataRows[0][(int)ColumnType.VENDOR + offset].ToString();
            //StackWeight = dataRows[0][(int)ColumnType.STACKWEIGHT + offset].ToString();
            WeightPerBag = dataRows[0][(int)ColumnType.WEIGHTPERBAG + offset].ToString();
            //Bags_x = dataRows[0][(int)ColumnType.BAGS_X + offset].ToString();
            //Bags_y = dataRows[0][(int)ColumnType.BAGS_Y + offset].ToString();
            //Bags_xy = dataRows[0][(int)ColumnType.BAGS_XY + offset].ToString();
            WorkerNo = dataRows[0][(int)ColumnType.WORKER_NO + offset].ToString();
            Desc = dataRows[0][(int)ColumnType.DESC + offset].ToString();
        }
        public override void PrintLabel()
        {
            RecoveryLabel label = new RecoveryLabel();
            label.Printlabel(this);
        }

    }

    class RecoverySysData : ProcessSysData
    {
        String[] PackTitle = { "序号", "生产日期", "时间", "配方", "工号","备注" };
        String[] dataBase = { "id", "Date", "Time", "Recipe",  "Worker_No","ProductDesc" };

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
