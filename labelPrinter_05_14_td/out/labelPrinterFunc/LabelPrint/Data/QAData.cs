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
    public class QAUserinputData : ProcessData
    {
        public String QAMachineNo;
        public String desc;
        public String BigRollNo;
        public String ProductState;// = GetProductState();
        public String ProductQuality;// = GetProductQuality();
        public String ShowRealWeight;// = GetShowRealWeight();
        public String Desc;
        public String Weight;
        public String JonitCount;
        //public String CutMachineNo;
        public String LittleRollNo;

        public PlateInfo CurPlatInfo;
        public QAUserinputData()
        {
            ColumnList = Column;
            TableName = "QA";
        }
        /*
         * 大标签包含的栏位内容：客户名称、原材料代码、产品批号、材料名称、宽度、生产者、生产日期、卷重/卷长、卷号、检验结果；
         */
        override public void UpdatePrintPrintData(DynamicPrintLabelData DyData)
        {
            //DyData.DataTime = WorkDate + WorkTime;

            //DyData.RawMaterialCode = RawMaterialCode;
            //DyData.MaterialName = MaterialName;
            //DyData.CustomName = CustomerName;
            //DyData.BatchNo = BatchNo;
            //DyData.WorkNo = WorkNo;
            //DyData.WorkerNo = WorkerNo;
            //DyData.DataTime = WorkDate + WorkTime;
            //DyData.Width = Width;
            //DyData.LittleRollNoStr = LittleRollCount;
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
            "ProductOrder",
            "ProductCode",
            "ProductName",
            "Width",
            "RecipeCode",
            "CustomerName",
            //"MaterialName",
            "BatchNo",
            "Recipe",
            "ManHour",
            "WorkNo",
            "WorkerNo",
            "CutMachineNo",
            "JointCount",
            "ProductDesc",
            "BigRollNo",
            "LittleRollNo",
            "LittleWeight",
            "Quality",
            "State",
        };
        enum ColumnType
        {
            SN,
            Date,
            Time,
            WorkClassType,
            WorkTimeType,
            ProductOrder,
            ProductCode,
            ProductName,
            Width,
            RecipeCode,
            CustomerName,
           // MaterialName,
            BatchNo,
            Recipe,
            ManHour,
            WorkNo,
            WorkerNo,
            QAMachineNo,
            JointCount,
            ProductDesc,
            BigRollNo,
            LittleRollNo,
            LittleWeight,
            Quality,
            State,
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
            values[(int)ColumnType.Date] = DateTime.Now.ToString("yyyy-MM-dd");
            values[(int)ColumnType.Time] = DateTime.Now.ToString("HH-mm-ss");

            values[(int)ColumnType.WorkTimeType] = WorkTType.ToString();
            values[(int)ColumnType.WorkClassType] = WorkClsType.ToString();

            values[(int)ColumnType.ProductCode] = ProductCode;
            values[(int)ColumnType.Width] = Width;

            values[(int)ColumnType.RecipeCode] = RecipeCode;
            values[(int)ColumnType.CustomerName] = CustomerName;
            //values[(int)ColumnType.MaterialName] = MaterialName;

            values[(int)ColumnType.BatchNo] = BatchNo;
           values[(int)ColumnType.ManHour] = WorkHour;
            values[(int)ColumnType.WorkNo] = WorkNo;
            values[(int)ColumnType.WorkerNo] = WorkerNo;

            values[(int)ColumnType.QAMachineNo] = QAMachineNo;
            values[(int)ColumnType.ProductDesc] = Desc;
            values[(int)ColumnType.BigRollNo] = BigRollNo;
            values[(int)ColumnType.LittleRollNo] = LittleRollNo;
            values[(int)ColumnType.LittleWeight] = Weight;
            values[(int)ColumnType.Quality] = ProductQuality;
            values[(int)ColumnType.State] = ProductState;

            return values;
        }
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
            //   LittleRollCount = dataRows[0][(int)ColumnType.LittleRollCount + offset].ToString();

            RecipeCode = dataRows[0][(int)ColumnType.RecipeCode + offset].ToString();
            CustomerName = dataRows[0][(int)ColumnType.CustomerName + offset].ToString();
            //MaterialName = dataRows[0][(int)ColumnType.MaterialName + offset].ToString();
            BatchNo = dataRows[0][(int)ColumnType.BatchNo + offset].ToString();
            WorkHour = dataRows[0][(int)ColumnType.ManHour + offset].ToString();
            WorkNo = dataRows[0][(int)ColumnType.WorkNo + offset].ToString();
            WorkerNo = dataRows[0][(int)ColumnType.WorkerNo + offset].ToString();
            QAMachineNo = dataRows[0][(int)ColumnType.QAMachineNo + offset].ToString();
            Desc = dataRows[0][(int)ColumnType.ProductDesc + offset].ToString();
            BigRollNo = dataRows[0][(int)ColumnType.BigRollNo + offset].ToString();
            LittleRollNo = dataRows[0][(int)ColumnType.LittleRollNo + offset].ToString();
            Weight = dataRows[0][(int)ColumnType.LittleWeight + offset].ToString();
            ProductQuality = dataRows[0][(int)ColumnType.Quality + offset].ToString();
            ProductState = dataRows[0][(int)ColumnType.State + offset].ToString();

        }
        public override void PrintLabel()
        {
            QulityCheckLabel label = new QulityCheckLabel();
            label.Printlabel(this);
        }
    }



    class QASysData : ProcessSysData
    {
        String[] PackTitle = { "序号","工号","检验日期", "时间", "产品代号", "配方号", "工单编号", "生产批号", "客户名称",  "大卷编号", "小卷编号" , "产品质量", "产品状态","备注" };
        String[] dataBase = { "id", "WorkerNo", "Date", "Time", "ProductCode", "RecipeCode", "WorkNo", "BatchNo", "CustomerName", "BigRollNo", "LittleRollNo", "产品质量", "产品状态", "ProductDesc" };

        public String BatchNo;
        public String ProductCode;
        public String BigRollNo;
        public String LittleRollNo;
        public String Customer;
        //    public String Recipe;


        String table = "QA";

        public override String CreateSelectOpiton()
        {
            String Str = "";
            Str = "Date between '" + Date1 + "' and '" + Date2 + "'";
            if (BatchNo != "" && BatchNo != null)
            {
                Str += " and " + "BatchNo='" + BatchNo + "'";
            }
            if (ProductCode != "" && ProductCode != null)
            {
                Str += " and " + "ProductCode='" + ProductCode + "'";
            }
            if (BigRollNo != "" && BigRollNo != null)
            {
                Str += " and " + "BigRollNo='" + BigRollNo + "'";
            }
            if (LittleRollNo != "" && LittleRollNo != null)
            {
                Str += " and " + "LittleRollNo='" + LittleRollNo + "'";
            }
            if (Customer != "" && Customer != null)
            {
                Str += " and " + "CustomerName='" + Customer + "'";
            }
            //if (Recipe != "" && Recipe != null)
            //{
            //    Str += " and " + "Recipe='" + Recipe + "'";
            //}
            return Str;
        }
        public override String CreateSlectStrForColumn()
        {
            String SelStr = createSelectStr(table, dataBase, PackTitle);
            return SelStr;
        }

    }
}
