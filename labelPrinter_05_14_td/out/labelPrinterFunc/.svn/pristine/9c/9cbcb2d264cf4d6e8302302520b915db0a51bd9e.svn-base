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
    public partial class FilmPrintUserinputData : ProcessData
    {

        public static String[] PrintProductStateStr = { "合格品", "不合格品","印刷退货","废品" };
        public String BigRollNo;
       // public String ProductState;// = GetProductState();
       // public String ProductQuality;// = GetProductQuality();
       // public String ShowRealWeight;// = GetShowRealWeight();
        public String Desc;
        public String Weight;
        public String PrintMachineNo;
        public String LittleRollNo;

        public PlateInfo CurPlatInfo;
        public FilmPrintUserinputData()
        {
            ColumnList = Column;
            TableName = "FilmPrint";
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

       public  static String[] Column =
          {
            "SN",
            "Date",
            "Time",
            "MState",
            "WorkClassType",
            "WorkTimeType",
            "ProductCode",
            "ProductName",
            "Width",
            "LittleRollCount",
            "RecipeCode",
            "CustomerName",
            "MaterialName",
            "BatchNo",
            "ManHour",
            "WorkNo",
            "WorkerNo",
            "PrintMachineNo",
            "ProductDesc",
            "BigRollNo",
            "InputBarcode",
            "OutputBarcode",
            "Product_State",
            "Roll_Weight",
            "RawMaterialCode",
            "ProductLength",
            "ProductWeight",
         //   "LittleRollNo"
        };
       public  enum ColumnType
        {
            SN,
            Date,
            Time,
            MState,
            WorkClassType,
            WorkTimeType,
            ProductCode,
            ProductName,
            Width,
            LittleRollCount,
            RecipeCode,
            CustomerName,
            MaterialName,
            BatchNo,
            ManHour,
            WorkNo,
            WorkerNo,
            PrintMachineNo,
            ProductDesc,
            BigRollNo,
            InputBarcode,
            OutputBarcode,
            //   LittleRollNo,
            Product_State,
            Roll_Weight,
            RawMaterialCode,
            ProductLength,
            ProductWeight,

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
            values[(int)ColumnType.ProductName] = ProductName;
            
            values[(int)ColumnType.Width] = Width;
            values[(int)ColumnType.LittleRollCount] = LittleRollCount;

            values[(int)ColumnType.RecipeCode] = RecipeCode;
            values[(int)ColumnType.CustomerName] = CustomerName;
            values[(int)ColumnType.MaterialName] = MaterialName;
            values[(int)ColumnType.BatchNo] = BatchNo;
            values[(int)ColumnType.ManHour] = WorkHour;
            values[(int)ColumnType.WorkNo] = WorkNo;
            values[(int)ColumnType.WorkerNo] = WorkerNo;
            values[(int)ColumnType.PrintMachineNo] = PrintMachineNo;
            values[(int)ColumnType.ProductDesc] = Desc;
            values[(int)ColumnType.BigRollNo] = BigRollNo;
            values[(int)ColumnType.InputBarcode] = InputBarcode;
            values[(int)ColumnType.OutputBarcode] = OutputBarcode;
            
            values[(int)ColumnType.Product_State] = Product_State;
            values[(int)ColumnType.Roll_Weight] = Roll_Weight;

            values[(int)ColumnType.RawMaterialCode] = RawMaterialCode;
            values[(int)ColumnType.ProductLength] = ProductLength;
            values[(int)ColumnType.ProductWeight] = ProductWeight;

            // values[(int)ColumnType.LittleRollNo] = LittleRollNo;
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
            ProductName = dataRows[0][(int)ColumnType.ProductName + offset].ToString();
            Width = dataRows[0][(int)ColumnType.Width + offset].ToString();
            LittleRollCount = dataRows[0][(int)ColumnType.LittleRollCount + offset].ToString();

            RecipeCode = dataRows[0][(int)ColumnType.RecipeCode + offset].ToString();
            CustomerName = dataRows[0][(int)ColumnType.CustomerName + offset].ToString();
            MaterialName = dataRows[0][(int)ColumnType.MaterialName + offset].ToString();

            BatchNo = dataRows[0][(int)ColumnType.BatchNo + offset].ToString();
            WorkHour = dataRows[0][(int)ColumnType.ManHour + offset].ToString();
            WorkNo = dataRows[0][(int)ColumnType.WorkNo + offset].ToString();
            WorkerNo = dataRows[0][(int)ColumnType.WorkerNo + offset].ToString();

            PrintMachineNo = dataRows[0][(int)ColumnType.PrintMachineNo + offset].ToString();
            Desc = dataRows[0][(int)ColumnType.ProductDesc + offset].ToString();
            BigRollNo = dataRows[0][(int)ColumnType.BigRollNo + offset].ToString();

            InputBarcode = dataRows[0][(int)ColumnType.InputBarcode + offset].ToString();
            OutputBarcode = dataRows[0][(int)ColumnType.OutputBarcode + offset].ToString();
            
            Product_State = dataRows[0][(int)ColumnType.Product_State + offset].ToString();
            Roll_Weight = dataRows[0][(int)ColumnType.Roll_Weight + offset].ToString();

            RawMaterialCode = dataRows[0][(int)ColumnType.RawMaterialCode + offset].ToString();
            ProductLength = dataRows[0][(int)ColumnType.ProductLength + offset].ToString();
            ProductWeight = dataRows[0][(int)ColumnType.ProductWeight + offset].ToString();

            // LittleRollNo = dataRows[0][(int)ColumnType.LittleRollNo + offset].ToString();
        }

        public override void PrintLabel()
        {
            FilmPrintLabel label = new FilmPrintLabel();
            label.Printlabel(this);
        }
        /*        
         *       
         *小标签（只有分切工序需要）应包含以下内容：
        XXXXXXXXXX(工单编码)+X（工序）+X（机台号）+XXXXXXXX（日期）+ XX（卷号）+ XXX（分卷号）+ X（客户序号）+ X（质量编码）；
        具体每个栏位含义是：
        工单编码：共 11位（将来可能按照用户的需求更改），对应于产品类别信息，配方、品相、色系、克重、客户等；数据来自于服务器通讯下发。
        工序：共一位，（1：出库；2：上料；3：流延；4：印刷；5：分切；6：再造料；7：质量检验）
        机台号：共一位（1 - 5），设备序号。对出库的原料标签不需要该标签；其他工序标签的该栏位或者来自生产产品的设备编号（如流延机、印刷机、分切机或再造料机），或者来源于半产品的自带标签（如质量检验工序和再造料工序，来自于扫描待检验产品和废料回收料的已带标签）。
        日期：共十位，年、月、日、时、分各两位，用数字表示；
        大卷号：共两位，表示大卷号；
        小卷号：共三位，表示小卷编号；
        客户序号：单作为0，套作为序号 1 - 3
        质量编码：共一位，0（未检验），1（合格），2以上为错误原因。
        再造料标签编辑界面中，扫描到的第一个废料卷的标签数据修改工序和机台编号后，就是再造料的标签。
        */
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
    class FilmPrintSysData : ProcessSysData
    {
        String[] FilmPrintTitle = { "序号", "生产日期", "时间", "班别","班次","生产批号", "客户名称", "产品代号", "大卷编号", "工单编号", "卷重", "产品状态", "备注" };
        String[] dataBase = { "id", "Date", "Time", "WorkClassType", "WorkTimeType", "BatchNo", "CustomerName", "ProductCode", "BigRollNo", "WorkNo", "Roll_Weight", "Product_State", "ProductDesc" };
      //  String[] FilmPrintTitle = { "序号", "生产日期", "时间", "班别", "班次", "生产批号", "客户名称", "产品品号", "大卷编号", "工单编号", "卷重",  "备注" };
       // String[] dataBase = { "id", "Date", "Time", "WorkClassType", "WorkTimeType", "BatchNo", "CustomerName", "ProductCode", "BigRollNo", "WorkNo", "Roll_Weight",  "ProductDesc" };

        public String BatchNo;
        public String ProductCode;
        public String BigRollNo;
        //public String LittleRollNo;
        public String Customer;
    //    public String Recipe;


        String table = "FilmPrint";

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
            if (WorkNo != "" && WorkNo != null)
            {
                Str += " and " + "WorkNo='" + WorkNo + "'";
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
            String SelStr = createSelectStr(table, dataBase, FilmPrintTitle);
            return SelStr;
        }
    }
}
