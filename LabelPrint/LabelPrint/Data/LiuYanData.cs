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


    public class LiuYanUserinputData : ProcessData
    {
        public String LiuYanMachineNo;
        public String desc;
        public String BigRollNo;
        public String ProductState;// = GetProductState();
        public String ProductQuality;// = GetProductQuality();
        public String ShowRealWeight;// = GetShowRealWeight();
        public String Desc;
        public String Weight;
        public String JonitCount;

        //public String LittleRollNo;

        public PlateInfo CurPlatInfo;
        public LiuYanUserinputData()
        {
            ColumnList = Column;
            TableName = "LiuYan";
        }
        /*
         * 包含的栏位内容：客户名称、产品代号、材料名称、基重、宽度、生产者、生产日期、卷重/卷长、卷号；
         */
        override public void UpdatePrintPrintData(DynamicPrintLabelData DyData)
        {
            base.UpdatePrintPrintData(DyData);
            DyData.BigRollNoStr = BigRollNo;
            DyData.RollWeightLength = Weight;
        }
        static String[] Column =
          {
            "SN",
            "Date",
            "Time",
            "WorkClassType",
            "WorkTimeType",
            "ProductCode",
            "Width",
            "LittleRollCount",
            "RecipeCode",
            "CustomerName",
           // "MaterialName",
            "BatchNo",
            "ManHour",
            "WorkNo",
            "WorkerNo",
            "LiuYanMachineNo",
            "ProductDesc",
            "BigRollNo",
        };
        enum ColumnType
        {
            SN,
            Date,
            Time,
            WorkClassType,
            WorkTimeType,
            ProductCode,
            Width,
            LittleRollCount,
            RecipeCode,
            CustomerName,
            //MaterialName,
            BatchNo,
            ManHour,
            WorkNo,
            WorkerNo,
            LiuYanMachineNo,
            ProductDesc,
            BigRollNo,
            MAX_COLUMN
        }
        public override String[] SetColumnDataArray()
        {
            String[] values = new String[Column.Length];
            System.Diagnostics.Debug.Assert((int)ColumnType.MAX_COLUMN == Column.Length);
            values[(int)ColumnType.Date] = DateTime.Now.ToString("yyyy-MM-dd");
            values[(int)ColumnType.Time] = DateTime.Now.ToString("HH-mm-ss");


            values[(int)ColumnType.WorkTimeType] = WorkTType.ToString();
            values[(int)ColumnType.WorkClassType] = WorkClsType.ToString();

            values[(int)ColumnType.ProductCode] = ProductCode;
            values[(int)ColumnType.Width] = Width;
            values[(int)ColumnType.LittleRollCount] = LittleRollCount;

            values[(int)ColumnType.RecipeCode] = RecipeCode;
            values[(int)ColumnType.CustomerName] = CustomerName;
            //values[(int)ColumnType.MaterialName] = MaterialName;
            values[(int)ColumnType.BatchNo] = BatchNo;


            values[(int)ColumnType.ManHour] = WorkHour;
            values[(int)ColumnType.WorkNo] = WorkNo;
            values[(int)ColumnType.WorkerNo] = WorkerNo;
            values[(int)ColumnType.LiuYanMachineNo] = LiuYanMachineNo;
            values[(int)ColumnType.ProductDesc] = Desc;
            values[(int)ColumnType.BigRollNo] = BigRollNo;
            return values;
        }

        //public Boolean GetSelItemFromDB(int id)
        //{
        //    mySQLClass a = new mySQLClass();
        //    int offset = 1;
        //    String str;
        //    System.Data.DataTable dtEditor;
        //    //            id = 100;
        //    dtEditor = mySQLClass.queryDataTableAction("sampledatabase", "Select * from Cut where id=" + id, null);

        //    object dateObject = dtEditor.Rows[0][(int)ColumnType.Date + offset];
        //    WorkDate = dateObject.ToString();
        //    dateObject = dtEditor.Rows[0][(int)ColumnType.Time + offset];
        //    WorkTime += " " + dateObject.ToString();

        //    str = dataRows[0][(int)ColumnType.WorkTimeType + offset].ToString();
        //    WorkTType = (WorkTimeType)Enum.Parse(typeof(WorkTimeType), str);

        //    str = dataRows[0][(int)ColumnType.WorkClassType + offset].ToString();
        //    WorkClsType = (WorkClassType)Enum.Parse(typeof(WorkClassType), str);
        //    //ProductOrder = 
        //    dateObject = dtEditor.Rows[0][(int)ColumnType.ProductCode + offset];
        //    ProductCode = dateObject.ToString();

        //    dateObject = dtEditor.Rows[0][(int)ColumnType.Width + offset];
        //    Width = dateObject.ToString();

        //    dateObject = dtEditor.Rows[0][(int)ColumnType.MaterialCode + offset];
        //    RawMaterialCode = dateObject.ToString();

        //    dateObject = dtEditor.Rows[0][(int)ColumnType.CustomerName + offset];
        //    CustomerName = dateObject.ToString();

        //    dateObject = dtEditor.Rows[0][(int)ColumnType.MaterialName + offset];
        //    MaterialName = dateObject.ToString();
        //    dateObject = dtEditor.Rows[0][(int)ColumnType.BatchNo + offset];
        //    BatchNo = dateObject.ToString();

        //    dateObject = dtEditor.Rows[0][(int)ColumnType.ManHour + offset];
        //    WorkHour = dateObject.ToString();

        //    dateObject = dtEditor.Rows[0][(int)ColumnType.WorkNo + offset];
        //    WorkNo = dateObject.ToString();
        //    dateObject = dtEditor.Rows[0][(int)ColumnType.WorkerNo + offset];
        //    WorkerNo = dateObject.ToString();


        //    dateObject = dtEditor.Rows[0][(int)ColumnType.ProductDesc + offset];
        //    Desc = dateObject.ToString();

        //    dateObject = dtEditor.Rows[0][(int)ColumnType.BigRollNo + offset];
        //    BigRollNo = dateObject.ToString();

        //    //Date_Time = dateObject==null ? null : (String)dateObject;
        //    return true;
        //}
        public override void SetInputDataFromDataBase(DataTable dt)
        {
            int offset = 1;
            String str;
            DataRowCollection dataRows = dt.Rows;
            WorkDate = dataRows[0][(int)ColumnType.Date + offset].ToString();
            WorkTime = dataRows[0][(int)ColumnType.Time + offset].ToString();

            str = dataRows[0][(int)ColumnType.WorkTimeType + offset].ToString();
            WorkTType = (WorkTimeType)Enum.Parse(typeof(WorkTimeType), str);

            str = dataRows[0][(int)ColumnType.WorkClassType + offset].ToString();
            WorkClsType = (WorkClassType)Enum.Parse(typeof(WorkClassType), str);

            ProductCode = dataRows[0][(int)ColumnType.ProductCode + offset].ToString();
            Width = dataRows[0][(int)ColumnType.Width + offset].ToString();
            LittleRollCount = dataRows[0][(int)ColumnType.LittleRollCount + offset].ToString();

            RecipeCode = dataRows[0][(int)ColumnType.RecipeCode + offset].ToString();
            CustomerName = dataRows[0][(int)ColumnType.CustomerName + offset].ToString();
           // MaterialName = dataRows[0][(int)ColumnType.MaterialName + offset].ToString();

            BatchNo = dataRows[0][(int)ColumnType.BatchNo + offset].ToString();
            WorkHour = dataRows[0][(int)ColumnType.ManHour + offset].ToString();
            WorkNo = dataRows[0][(int)ColumnType.WorkNo + offset].ToString();
            WorkerNo = dataRows[0][(int)ColumnType.WorkerNo + offset].ToString();

            LiuYanMachineNo = dataRows[0][(int)ColumnType.LiuYanMachineNo + offset].ToString();
            Desc = dataRows[0][(int)ColumnType.ProductDesc + offset].ToString();
            BigRollNo = dataRows[0][(int)ColumnType.BigRollNo + offset].ToString();
        }

        public override void PrintLabel()
        {
            LiuYanLabel label = new LiuYanLabel();
            label.Printlabel(this);
        }
    }

    /*
     * 3. 流延系统：
   查询条件：日期范围/生产批次/客户名称/工单编号/产品代号
   列表信息：序号/工号/生产日期/时间/产品代号/配方号/工单编号/生产批次/客户名称/大卷编号/备注 
   标签画面：
     a. 原材料代码改成配方号，原材料名称删除。
     b. 甲乙丙丁班改成甲乙丙班。
     */
    class LiuYanSysData : ProcessSysData
    {
        String[] FilmPrintTitle = { "序号", "工号", "生产日期", "时间", "产品代号", "配方号", "工单编号", "生产批次", "客户名称", "大卷编号", "备注"  };
        String[] dataBase = { "id", "WorkerNo", "Date", "Time", "ProductCode", "RecipeCode", "WorkNo", "BatchNo", "CustomerName", "BigRollNo", "ProductDesc" };

        public String BatchNo;
        public String ProductCode;
        //public String BigRollNo;
        //public String LittleRollNo;
        public String Customer;
        //    public String Recipe;


        String table = "LiuYan";

        public override String CreateSelectOpiton()
        {
            String Str = "";
            Str = "Date between '" + Date1 + "' and '" + Date2 + "'";
            if (BatchNo != "" && BatchNo != null)
            {
                Str += " and " + "BatchNo='" + BatchNo + "'";
            }
            if (Customer != "" && Customer != null)
            {
                Str += " and " + "CustomerName='" + Customer + "'";
            }
            if (WorkNo != "" && WorkNo != null)
            {
                Str += " and " + "WorkNo='" + WorkNo + "'";
            }
            if (ProductCode != "" && ProductCode != null)
            {
                Str += " and " + "ProductCode='" + ProductCode + "'";
            }
            //if (BigRollNo != "" && BigRollNo != null)
            //{
            //    Str += " and " + "BigRollNo='" + BigRollNo + "'";
            //}
            //if (LittleRollNo != "" && LittleRollNo != null)
            //{
            //    Str += " and " + "LittleRollNo='" + LittleRollNo + "'";
            //}

            //if (Recipe != "" && Recipe != null)
            //{
            //    Str += " and " + "Recipe='" + Recipe + "'";
            //}
            return Str;
        }
        public override String CreateSlectStrForColumn()
        {
            String SelStr = createSelectStr(table, dataBase, FilmPrintTitle);
            return SelStr;
        }
    }
}
