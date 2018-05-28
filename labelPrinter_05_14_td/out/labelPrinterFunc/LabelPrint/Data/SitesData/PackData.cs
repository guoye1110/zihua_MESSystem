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

    public partial class PackUserinputData : ProcessData
    {
        public String PackMachineNo;
        public String BigRollNo;
        public String Desc;
        public String Weight;
        public String LittleRollNo;
        public String OrderNo;
        //        public PlateInfo CurPlatInfo;



        public PackUserinputData()
        {
            ColumnList = Column;
            TableName = "Pack";
        }
        public Boolean CheckUserInput()
        {
            if (BigRollNo.Length != 3)
            {
                System.Windows.Forms.MessageBox.Show("大卷号必须是3位");
                return false;
            }

            if (!checkWorkNoLen())
            {
                return false;
            }
            return true;
        }


        static String[] Column =
        {
            "SN",
            "Date",
            "Time",
            "MState",
            "WorkClassType",
            "WorkTimeType",
            "ProductCode",
            "Width",
            "LittleRollCount",
            "RecipeCode",
            "CustomerName",
            "MaterialName",
            "RawMaterialCode",
            "BatchNo",
            "ManHour",
            "WorkNo",
            "WorkerNo",
            "PackMachineNo",
            "ProductDesc",
            "BigRollNo",
            "LittleRollNo",
            "InputBarcode",
            "OutputBarcode",
            "PlateNo",
            "ProductLength",
            "ProductWeight",
            "Roll_Weight",
            "OrderNo",
        };
        enum ColumnType
        {
            SN,
            Date,
            Time,
            MState,
            WorkClassType,
            WorkTimeType,
            ProductCode,
            Width,
            LittleRollCount,
            RecipeCode,
            CustomerName,
            RawMaterialCode,
            MaterialName,
            BatchNo,
            ManHour,
            WorkNo,
            WorkerNo,
            PackMachineNo,
            ProductDesc,
            BigRollNo,
            LittleRollNo,
            InputBarcode,
            OutputBarcode,
            PlateNo,
            ProductLength,
            ProductWeight,
            Roll_Weight,
            OrderNo,
            MAX_COLUMN
        }

        public override String[] SetColumnDataArray()
        {
            String[] values = new String[Column.Length];
            System.Diagnostics.Debug.Assert((int)ColumnType.MAX_COLUMN == Column.Length);
            values[(int)ColumnType.Date] = WorkDate;// DateTime.Now.ToString("yyyy-MM-dd");
            values[(int)ColumnType.Time] = WorkTime;// DateTime.Now.ToString("HH-mm-ss");
            values[(int)ColumnType.MState] = MState;
            
            values[(int)ColumnType.WorkTimeType] = WorkTType.ToString();
            values[(int)ColumnType.WorkClassType] = WorkClsType.ToString();

            values[(int)ColumnType.ProductCode] = ProductCode;
            values[(int)ColumnType.Width] = Width;
            values[(int)ColumnType.LittleRollCount] = LittleRollCount;

            values[(int)ColumnType.RecipeCode] = RecipeCode;
            values[(int)ColumnType.CustomerName] = CustomerName;
            values[(int)ColumnType.MaterialName] = MaterialName;
            values[(int)ColumnType.RawMaterialCode] = RawMaterialCode;
            
            values[(int)ColumnType.BatchNo] = BatchNo;


            values[(int)ColumnType.ManHour] = WorkHour;
            values[(int)ColumnType.WorkNo] = WorkNo;
            values[(int)ColumnType.WorkerNo] = WorkerNo;
            values[(int)ColumnType.PackMachineNo] = PackMachineNo;
            values[(int)ColumnType.ProductDesc] = Desc;
            values[(int)ColumnType.BigRollNo] = BigRollNo;
            values[(int)ColumnType.LittleRollNo] = LittleRollNo;
            
            values[(int)ColumnType.InputBarcode] = InputBarcode;
            values[(int)ColumnType.OutputBarcode] = OutputBarcode;
            values[(int)ColumnType.PlateNo] = PlateNo;

            values[(int)ColumnType.ProductLength] = ProductLength;
            values[(int)ColumnType.ProductWeight] = ProductWeight; 
            values[(int)ColumnType.Roll_Weight] = Weight;
            values[(int)ColumnType.OrderNo] = OrderNo;
            return values;
        }

        public override void SetInputDataFromDataBase(DataTable dt)
        {
            int offset = 1;
            String str;
            DataRowCollection dataRows = dt.Rows;
            WorkDate = dataRows[0][(int)ColumnType.Date + offset].ToString();
            WorkTime = dataRows[0][(int)ColumnType.Time + offset].ToString();
            MState = dataRows[0][(int)ColumnType.MState + offset].ToString();

            str = dataRows[0][(int)ColumnType.WorkTimeType + offset].ToString();
            WorkTType = (WorkTimeType)Enum.Parse(typeof(WorkTimeType), str);

            str = dataRows[0][(int)ColumnType.WorkClassType + offset].ToString();
            WorkClsType = (WorkClassType)Enum.Parse(typeof(WorkClassType), str);

            ProductCode = dataRows[0][(int)ColumnType.ProductCode + offset].ToString();
            Width = dataRows[0][(int)ColumnType.Width + offset].ToString();
            LittleRollCount = dataRows[0][(int)ColumnType.LittleRollCount + offset].ToString();

            RecipeCode = dataRows[0][(int)ColumnType.RecipeCode + offset].ToString();
            CustomerName = dataRows[0][(int)ColumnType.CustomerName + offset].ToString();
            MaterialName = dataRows[0][(int)ColumnType.MaterialName + offset].ToString();

            RawMaterialCode = dataRows[0][(int)ColumnType.RawMaterialCode + offset].ToString();

            BatchNo = dataRows[0][(int)ColumnType.BatchNo + offset].ToString();
            WorkHour = dataRows[0][(int)ColumnType.ManHour + offset].ToString();
            WorkNo = dataRows[0][(int)ColumnType.WorkNo + offset].ToString();
            WorkerNo = dataRows[0][(int)ColumnType.WorkerNo + offset].ToString();
            PackMachineNo = dataRows[0][(int)ColumnType.PackMachineNo + offset].ToString();
            Desc = dataRows[0][(int)ColumnType.ProductDesc + offset].ToString();
            BigRollNo = dataRows[0][(int)ColumnType.BigRollNo + offset].ToString();
            LittleRollNo = dataRows[0][(int)ColumnType.LittleRollNo + offset].ToString();

            InputBarcode = dataRows[0][(int)ColumnType.InputBarcode + offset].ToString();
            OutputBarcode = dataRows[0][(int)ColumnType.OutputBarcode + offset].ToString();

            PlateNo = dataRows[0][(int)ColumnType.PlateNo + offset].ToString();
            ProductLength = dataRows[0][(int)ColumnType.ProductLength + offset].ToString();
            ProductWeight = dataRows[0][(int)ColumnType.ProductWeight + offset].ToString();
            Weight = dataRows[0][(int)ColumnType.Roll_Weight + offset].ToString();
            OrderNo = dataRows[0][(int)ColumnType.OrderNo + offset].ToString();

        }

        public Boolean ParseBarcode(String barcode)
        {
            return true;
        }

        public override void PrintLabel()
        {
            PackLabel label = new PackLabel();
            label.Printlabel(this);
        }
        public override Boolean ParseBarCode(String barcode)
        {
            //XXXXXXXXXX(工单编码) + X（工序）+X（机台号）+XXXXXXXX（日期）+XX（卷号）+XXX（分卷号）+X（客户序号）+X（质量编码）；
            LittleRollBarcode bar = new LittleRollBarcode();
            if (!bar.ParseBarcode(barcode))
                return false;
            WorkNo = bar.WorkNoStr;
            BigRollNo = bar.BigRStr;
            LittleRollNo = bar.LittleRStr;
            BatchNo = bar.BatchNo;
            int workset = int.Parse(bar.VendorStr);
            MType = (workset == 0) ? ManufactureType.M_SINGLE : ManufactureType.M_MULTIPLE;
            return true;
        }
    }
    class PackSysData : ProcessSysData
    {


        String[] PackTitle = { "序号", "生产日期", "时间", "生产批号", "客户名称", "产品代号", "大卷编号", "小卷编号" ,"备注"};
        String[] dataBase = { "id", "Date", "Time", "BatchNo", "CustomerName", "ProductCode", "BigRollNo", "LittleRollNo","ProductDesc" };

        public String BatchNo;
        public String ProductCode;
        public String BigRollNo;
        public String LittleRollNo;
        public String Customer;
        //    public String Recipe;


        String table = "Pack";

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
