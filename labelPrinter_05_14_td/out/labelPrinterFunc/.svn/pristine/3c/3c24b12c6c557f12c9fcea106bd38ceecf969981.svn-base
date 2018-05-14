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
using System.Windows.Forms;
using System.Data;
using DotNet.Utilities;
using LabelPrint.Print;
namespace LabelPrint.Data
{

    public class MiscCutData
    {
        public String ProductCode;
        public String Width;
        //public String RecipeCode;
        //public String Fixture;
        //public String CustomerName;
        //public String BatchNo;
        //public int PlateNo;
        //public int CurRollNum;
        public String MiscIndexInOneProcess;
    };



    public partial class CutUserinputData : ProcessData
    {
        public String desc;
        public String BigRollNo;
        public String ProductState;// = GetProductState();
        public int ProductStateIndex = 0;
        public String ProductQuality;// = GetProductQuality();
        public int ProductQualityIndex = 0;
        public String ShowRealWeight;// = GetShowRealWeight();
        public String Desc;
        public String Weight;
        public String JointCount;
        public String CutMachineNo;
        public String LittleRollNo;

        public PlateInfo CurPlatInfo;
        public SetWorkProductData SetWorkProductInfo;


        public String PlateRollPerLay;
        public String PlateLayer;
        public String PlateRollNum;

        #region multiple working

        public String ProductCode1;
        public String ProductCode2;
        public String ProductCode3;
        public String RawMaterialCode1;
        public String RawMaterialCode2;
        public String RawMaterialCode3;
        public String CustomerName1;
        public String CustomerName2;
        public String CustomerName3;
        public String MaterialName1;
        public String MaterialName2;
        public String MaterialName3;
        public String BatchNo1;
        public String BatchNo2;
        public String BatchNo3;
        public String LittleRollCount1;
        public String LittleRollCount2;
        public String LittleRollCount3;
        public String Width1;
        public String Width2;
        public String Width3;
        #endregion 

        public CutUserinputData()
        {
            ColumnList = Column;
            CurPlatInfo = new PlateInfo(6,9);
            TableName = "Cut";
            SetWorkProductInfo = new SetWorkProductData();
        }

        public Boolean CheckUserInput()
        {
            if (BigRollNo.Length != 3)
            {
                System.Windows.Forms.MessageBox.Show("大卷号必须是3位");
                return false;
            }

            //if (!checkWorkNoLen())
            //{
            //    return false;
            //}
            return true;
        }

  

        public static String[] Column =
        {
            "SN",
            "Date",
            "Time",
            "MState",
            "WorkClassType",
            "WorkTimeType",
            "ProductOrder",
            "ProductCode",
            "ProductName",
            "Width",
            "MaterialCode",
            "CustomerName",
            "MaterialName",
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
            "InputBarcode",
            "OutputBarcode",
            "PlateNo",
            "PlateRollPerLay",
            "PlateLayer",
            "PlateRollNum",
          //  "RawMaterialCode",
            "ProductLength",
            "ProductWeight",
        };


        //// public String RawMaterialCode = null;
        //public String productLength = null;
        //public String productName = null;
        //public String productWeight = null;
        enum ColumnType
        {
            SN,
            Date,
            Time,
            MState,
            WorkClassType,
            WorkTimeType,
            ProductOrder,
            ProductCode,
            ProductName,
            Width,
            MaterialCode,
            CustomerName,
            MaterialName,
            BatchNo,
            Recipe,
            ManHour,
            WorkNo,
            WorkerNo,
            CutMachineNo,
            JointCount,
            ProductDesc,
            BigRollNo,
            LittleRollNo,
            LittleWeight,
            Quality,
            State,
            InputBarcode,
            OutputBarcode,
            PlateNo,
            PlateRollPerLay,
            PlateLayer,
            PlateRollNum,
          //  RawMaterialCode,
            ProductLength,
            ProductWeight,

            MAX_COLUMN
        };
        public static int GetIndexByString(String str)
        {
            for (int i = 0; i < Column.Length; i++)
            {
                if (str == "id")
                    return 0;
                else 
                if (str == Column[i])
                    return i+1;
            }
            return -1;
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

            values[(int)ColumnType.ProductOrder] = ProductCode;
            values[(int)ColumnType.ProductCode] = ProductCode;
            values[(int)ColumnType.ProductName] = ProductName;
            values[(int)ColumnType.Width] = Width;
            values[(int)ColumnType.MaterialCode] = RawMaterialCode;
            values[(int)ColumnType.CustomerName] = CustomerName;
            values[(int)ColumnType.MaterialName] = MaterialName;
            values[(int)ColumnType.BatchNo] = BatchNo;
            values[(int)ColumnType.Recipe] = RecipeCode;
            values[(int)ColumnType.ManHour] = WorkHour;
            values[(int)ColumnType.WorkNo] = WorkNo;
            values[(int)ColumnType.WorkerNo] = WorkerNo;
            values[(int)ColumnType.CutMachineNo] = CutMachineNo;
            values[(int)ColumnType.JointCount] = JointCount;
            values[(int)ColumnType.ProductDesc] = Desc;
            values[(int)ColumnType.BigRollNo] = BigRollNo;
            values[(int)ColumnType.LittleRollNo] = LittleRollNo;
            values[(int)ColumnType.LittleWeight] = Weight;
            values[(int)ColumnType.Quality] = ProductQuality;
            values[(int)ColumnType.State] = ProductState;

            values[(int)ColumnType.InputBarcode] = InputBarcode;
            values[(int)ColumnType.OutputBarcode] = OutputBarcode;
            values[(int)ColumnType.PlateNo] = CurPlatInfo.PLateNo.ToString();


            PlateRollPerLay = CurPlatInfo.LittleRollPerlayer.ToString();
            PlateLayer = CurPlatInfo.Layer.ToString();
            PlateRollNum = CurPlatInfo.getMaxLittleRoll().ToString();


            values[(int)ColumnType.PlateRollPerLay] = PlateRollPerLay;
            values[(int)ColumnType.PlateLayer] = PlateLayer;
            values[(int)ColumnType.PlateRollNum] = PlateRollNum;

            values[(int)ColumnType.ProductLength] = ProductLength;
            values[(int)ColumnType.ProductWeight] = ProductWeight;
            return values;
        }

        
        public override void SetInputDataFromDataBase(DataTable dt)
        {
            int offset = 1;
            String str;
            DataTable dtEditor;
            dtEditor = dt;
            object dateObject = dtEditor.Rows[0][(int)ColumnType.Date + offset];
            WorkDate = dateObject.ToString();
            dateObject = dtEditor.Rows[0][(int)ColumnType.Time + offset];
            WorkTime = " " + dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.MState + offset];
            MState = " " + dateObject.ToString();



            dateObject = dtEditor.Rows[0][(int)ColumnType.WorkTimeType + offset];
            str = dateObject.ToString();
            WorkTType = (WorkTimeType)Enum.Parse(typeof(WorkTimeType),str);
            dateObject = dtEditor.Rows[0][(int)ColumnType.WorkClassType + offset];
            str = dateObject.ToString();
            WorkClsType =(WorkClassType)Enum.Parse(typeof(WorkClassType),str);

            dateObject = dtEditor.Rows[0][(int)ColumnType.ProductOrder + offset];
            //ProductOrder = 
            dateObject = dtEditor.Rows[0][(int)ColumnType.ProductCode + offset];
            ProductCode = dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.Width + offset];
            Width = dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.MaterialCode + offset];
            RawMaterialCode = dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.CustomerName + offset];
            CustomerName = dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.MaterialName + offset];
            MaterialName = dateObject.ToString();
            dateObject = dtEditor.Rows[0][(int)ColumnType.BatchNo + offset];
            BatchNo = dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.Recipe + offset];
            RecipeCode = dateObject.ToString();
            dateObject = dtEditor.Rows[0][(int)ColumnType.ManHour + offset];
            WorkHour = dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.WorkNo + offset];
            WorkNo = dateObject.ToString();
            dateObject = dtEditor.Rows[0][(int)ColumnType.WorkerNo + offset];
            WorkerNo = dateObject.ToString();
            dateObject = dtEditor.Rows[0][(int)ColumnType.CutMachineNo + offset];
            CutMachineNo = dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.JointCount + offset];
            JointCount = dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.ProductDesc + offset];
            Desc = dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.BigRollNo + offset];
            BigRollNo = dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.LittleRollNo + offset];
            LittleRollNo = dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.LittleWeight + offset];
            Weight = dateObject.ToString();
            dateObject = dtEditor.Rows[0][(int)ColumnType.Quality + offset];
            ProductQuality = dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.State + offset];
            ProductState = dateObject.ToString();


            dateObject = dtEditor.Rows[0][(int)ColumnType.InputBarcode + offset];
            InputBarcode = dateObject.ToString();
            dateObject = dtEditor.Rows[0][(int)ColumnType.OutputBarcode + offset];
            OutputBarcode = dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.PlateNo + offset];
            String PlatoNoStr = dateObject.ToString();


            bool result = int.TryParse(PlatoNoStr, out CurPlatInfo.PLateNo);
            if (result)
            {

            }
            else
            {
      
            }
            dateObject = dtEditor.Rows[0][(int)ColumnType.PlateRollPerLay + offset];
            PlateRollPerLay = dateObject.ToString();

            result = int.TryParse(PlateRollPerLay, out CurPlatInfo.LittleRollPerlayer);

            dateObject = dtEditor.Rows[0][(int)ColumnType.PlateLayer + offset];
            PlateLayer = dateObject.ToString();
            result = int.TryParse(PlateLayer, out CurPlatInfo.Layer);
   
            dateObject = dtEditor.Rows[0][(int)ColumnType.PlateRollNum + offset];
            PlateRollNum = dateObject.ToString();
            //Date_Time = dateObject==null ? null : (String)dateObject;

            dateObject = dtEditor.Rows[0][(int)ColumnType.ProductLength + offset];
            ProductLength = dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.ProductWeight + offset];
            ProductWeight = dateObject.ToString();

            return ;
        }

 
        public override void PrintLabel()
        {

            CutLabel label = new CutLabel();
            label.Printlabel(this);
        }

        public override Boolean ParseBarCode(String barcode)
        {
            //XXXXXXXXXX(工单编码) + X（工序）+X（机台号）+XXXXXXXX（日期）+XX（卷号）+XXX（分卷号）+X（客户序号）+X（质量编码）；
            BigRollBarcode bar = new BigRollBarcode();
            if (!bar.ParseBarcode(barcode))
                return false;
            WorkNo = bar.WorkNoStr;
            BigRollNo = bar.BigRStr;
            //  LittleRollNo = bar.LittleRStr;
            BatchNo = bar.BatchNo;
            //int workset = int.Parse(bar.VendorStr);
            // MType = (workset == 0) ? ManufactureType.M_SINGLE : ManufactureType.M_MULTIPLE;
            return true;
        }

        public Boolean ParsePlateInfo(String fixtureNum)
        {
            Boolean res;
            int totalNum = 0;
            res =  ParsePlateInfo(fixtureNum, ref CurPlatInfo.LittleRollPerlayer, ref CurPlatInfo.Layer);
            if (res) { 
                PlateRollPerLay = CurPlatInfo.LittleRollPerlayer.ToString();
                PlateLayer = CurPlatInfo.Layer.ToString();
                totalNum = CurPlatInfo.getMaxLittleRoll();
                PlateRollNum = totalNum.ToString();
            }
            return res;
        }
        public Boolean ParsePlateInfo(String fixtureNum, out String plateRollPerLay, out String plateLayer, out String plateRollNum)
        {
            Boolean res;
            int totalNum = 0;
            int  LittleRollPerlayer = 0;
            int Layer = 0;
            res = ParsePlateInfo(fixtureNum, ref LittleRollPerlayer, ref Layer);
            if (res)
            {
                plateRollPerLay = CurPlatInfo.LittleRollPerlayer.ToString();
                plateLayer = CurPlatInfo.Layer.ToString();
                totalNum = CurPlatInfo.getMaxLittleRoll();
                plateRollNum = totalNum.ToString();
            }
            else
            {
                plateRollPerLay = null;
                plateLayer = null;
                plateRollNum = null;
            }
            return res;
        }
    }
    class CutSysData:ProcessSysData
    {
        static String[] CutTitle = { "序号", "生产日期", "时间", "生产批号", "客户名称", "产品代号", "产品名称", "配方编号", "大卷编号", "小卷编号", "小卷重量", "产品质量", "产品状态", "铲板号", "备注" };
        static String[] dataBase = { "id", "Date", "Time", "BatchNo", "CustomerName", "ProductCode", "ProductName", "Recipe", "BigRollNo", "LittleRollNo", "LittleWeight", "Quality", "State", "PlateNo", "ProductDesc" };

        public String BatchNo;
        public String ProductCode;
        public String BigRollNo;
        public String LittleRollNo;
        public String Customer;
        public String Recipe;
        public String PlateNo;


        String table = "Cut";

       public static int GetIndexByString(String str)
        {
            for (int i=0; i<dataBase.Length; i++)
            {
                if (str == dataBase[i])
                    return i;
            }
            return -1;
        }
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
            if (Recipe != "" && Recipe != null)
            {
                Str += " and " + "Recipe='" + Recipe + "'";
            }
            if (PlateNo != "" && PlateNo != null)
            {
                Str += " and " + "PlateNo='" + PlateNo + "'";
            }
            return Str;
        }

        public override  String CreateSlectStrForColumn()
        {
            String SelStr = createSelectStr(table, dataBase, CutTitle);
            return SelStr;
        }


        public void PrintBangMaReport(List<DataTable> dtList)
        {
            Report_BangMa tp = new Report_BangMa();
         //   tp.ProductCode = "MDF3332";
           // tp.BatchNo = "1803420";
          //  tp.MDate = "2018-03-26";
          //  tp.PlateNo = "1";
         //   tp.VendorName = "abcdefg";
            tp.PrintPriview(dtList, "Hello");
        }

        public void PrintSplitCutDailyReport(List<DataTable> dtList)
        {
            Report_Cut tp = new Report_Cut();
            //Report_BangMa tp = new Report_BangMa();
            tp.PrintPriview(dtList, "Hello");

        }

        public override String GetTableName()
        {
            return table;
        }
        //public DataTable GetDTSelItemsFromDB(string selOption)
        //{
        //    return GetDTSelItemsFromDB(selOption, table);
        //}

        //public DataTable GetDTSelItemsFromDB()
        //{
        //    String SelStr = "select * from " + table + " where " + CreateSelectOpiton();

        //    return GetDTSelItemsFromDB(SelStr);

        //}
        // public DataTable UpdateDBView2(DataGridView dgview)

        //{
        //    DataTable dt;
        //    String SelStr = CreateSlectStrForColumn();

        // //   SelStr += " where " + CreateSelectOption(); //ok
        //                                                //SelStr += " where to_days(Date) = to_days(now())"; //test ok
        //                                                //dt = mySQLClass.updateDBView("sampledatabase", createSelectStr("Cut", dataBase, CutTitle), dataGridView1);
        //                                                //SelStr += " where " + "LittleRollNo='080-01'";   //ok
        //    dt = mySQLClass.updateDBView(databaseName, SelStr, dgview);
        //    //SysData.SetCurDataTable(dt);

        //    return dt;
        //}

    }

}
