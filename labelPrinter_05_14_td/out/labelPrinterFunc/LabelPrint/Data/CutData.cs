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

namespace LabelPrint.Data
{
    public class CutUserinputData : ProcessData
    {
        public String desc;
        public String BigRollNo;
        public String ProductState;// = GetProductState();
        public String ProductQuality;// = GetProductQuality();
        public String ShowRealWeight;// = GetShowRealWeight();
        public String Desc;
        public String Weight;
        public String JointCount;
        public String CutMachineNo;
        public String LittleRollNo;

        public PlateInfo CurPlatInfo;


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
            TableName = "Cut";
        }
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
            MAX_COLUMN
        };

        public override String[] SetColumnDataArray()
        {
            String[] values = new String[Column.Length];
            System.Diagnostics.Debug.Assert((int)ColumnType.MAX_COLUMN == Column.Length);
            values[(int)ColumnType.Date] = DateTime.Now.ToString("yyyy-MM-dd");
            values[(int)ColumnType.Time] = DateTime.Now.ToString("HH-mm-ss");

            values[(int)ColumnType.WorkTimeType] = WorkTType.ToString();
            values[(int)ColumnType.WorkClassType] = WorkClsType.ToString();

            values[(int)ColumnType.ProductOrder] = ProductCode1;
            values[(int)ColumnType.ProductCode] = ProductCode1;
            values[(int)ColumnType.ProductOrder] = ProductCode1;
            values[(int)ColumnType.ProductCode] = ProductCode1;
            values[(int)ColumnType.ProductName] = ProductCode1;
            values[(int)ColumnType.Width] = Width1;
            values[(int)ColumnType.MaterialCode] = RawMaterialCode1;
            values[(int)ColumnType.CustomerName] = CustomerName1;
            values[(int)ColumnType.MaterialName] = MaterialName1;
            values[(int)ColumnType.BatchNo] = BatchNo1;
            values[(int)ColumnType.Recipe] = "recipe";
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
            WorkTime += " " + dateObject.ToString();

            dateObject = dtEditor.Rows[0][(int)ColumnType.WorkTimeType + offset];
            str = dateObject.ToString();
            WorkTType = (WorkTimeType)Enum.Parse(typeof(WorkTimeType),str);
            dateObject = dtEditor.Rows[0][(int)ColumnType.WorkClassType + offset];
            str = dateObject.ToString();
            WorkClsType =(WorkClassType)Enum.Parse(typeof(WorkClassType),str);

            dateObject = dtEditor.Rows[0][(int)ColumnType.ProductOrder + offset];
            //ProductOrder = 
            dateObject = dtEditor.Rows[0][(int)ColumnType.ProductCode + offset];
            ProductCode1 = dateObject.ToString();

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
            //Recipe = dateObject.ToString();
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

            //Date_Time = dateObject==null ? null : (String)dateObject;
            return ;
        }

 
        public override void PrintLabel()
        {

            CutLabel label = new CutLabel();
            label.Printlabel(this);
        }

    }
    class CutSysData:ProcessSysData
    {
        String[] CutTitle = { "序号", "生产日期", "时间", "生产批号", "客户名称", "产品品号", "产品名称", "配方编号", "大卷编号", "小卷编号", "小卷重量", "产品质量", "产品状态","备注" };
        String[] dataBase = { "id", "Date", "Time", "BatchNo", "CustomerName", "ProductCode", "ProductName", "Recipe", "BigRollNo", "LittleRollNo", "LittleWeight", "Quality", "State", "ProductDesc" };

        public String BatchNo;
        public String ProductCode;
        public String BigRollNo;
        public String LittleRollNo;
        public String Customer;
        public String Recipe;


        String table = "Cut";

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
            return Str;
        }

        public override  String CreateSlectStrForColumn()
        {
            String SelStr = createSelectStr(table, dataBase, CutTitle);
            return SelStr;
        }

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
