using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using LabelPrint.Util;
using System.Data;
using System.Windows.Forms;
using LabelPrint.Print;
namespace LabelPrint.Data
{
    public class ProcessData
    {
        public ManufactureType MType;
        public WorkClassType WorkClsType;
        public WorkTimeType WorkTType;
        public String ProductCode;
        public String RawMaterialCode;
        public String CustomerName;
        public String MaterialName;
        public String BatchNo;
        public String RecipeCode;
        public String PlateNo;

        public String WorkNo;
        public String WorkDate;
        public String WorkTime;
        public String WorkerNo;
        public String WorkHour;
        public String LittleRollCount;
        public String Width;
        public String[] ColumnList;
        public String TableName;
        public String InputBarcode;
        public String OutputBarcode;
        public String MState;
        public String PackBarcode;
       // public String Product_State;
       // public String Roll_Weight;


        // public String RawMaterialCode = null;
        public String ProductLength = null;
        public String ProductName = null;
        public String ProductWeight = null;



        public String ProductState;// = GetProductState();
        public int ProductStateIndex = 0;
        public String ProductQuality;// = GetProductQuality();
        public int ProductQualityIndex = 0;
        /// <summary>
        /// 
        /// </summary>
        public String MachineID;


        public static String[] ProductStateStr = { "合格品", "残次品" };
        public static String[] ProductQualityStr = { "A", "B", "C", "D", "DC", "E", "W" };
        public static String[] ProductQualityStrForComBoList = { "A-晶点孔洞", "B-厚薄暴筋", "C-皱折", "D-端面错位(毛糙)", "DC-待处理", "E-油污", "W-蚊虫" };

        public virtual void UpdatePrintPrintData(DynamicPrintLabelData DyData)
        {
            //DyData.DataTime = WorkDate + WorkTime;
            DyData.RawMaterialCode = RawMaterialCode;
            DyData.MaterialName = MaterialName;
            DyData.RecipeCode = RecipeCode;
            DyData.CustomName = CustomerName;
            DyData.BatchNo = BatchNo;
            DyData.WorkNo = WorkNo;
            DyData.DataTime = WorkDate +" " + WorkTime;
            DyData.WorkerNo = WorkerNo;
            DyData.Width = Width;
            DyData.ProductCode = ProductCode;
            DyData.RollWeightLength = "0/0";

            DyData.ProductLength = ProductLength;
            DyData.ProductName = ProductName;
            DyData.ProductWeight = ProductWeight;
            //  DyData.LittleRollNoStr = LittleRollCount;
        }


        public RollBarcode ParseBarcode(String barcode)
        {
            RollBarcode Rollbar = null;
            if (barcode == null || barcode == "")
                return null;

            if (barcode.Length == BigRollBarcode.TotalLen)
            {
                Rollbar = new BigRollBarcode(barcode);
            }
            else if (barcode.Length == LittleRollBarcode.TotalLen)
            {
                Rollbar = new LittleRollBarcode(barcode);
            }

            if (Rollbar == null || Rollbar.Valid == false)
                return null;
            else
                return Rollbar;
                
        }

         public Boolean ParseBigRollBarCode(String barcode, out String workno, out String batchno, out String bigroll)
        {
            workno = null;
            bigroll = null;
            batchno = null;
            RollBarcode bar = ParseBarcode(barcode);
            if (bar == null)
                return false;

            //BigRollBarcode bar = new BigRollBarcode();
            //if (!bar.ParseBarcode(barcode))
            //{
            //    return false;
            //}

            workno = bar.WorkNoStr;
            bigroll = bar.BigRStr;
            batchno = bar.BatchNo;
            return true;
        }
        //XXXXXXXXXX(工单编码) + X（工序）+X（机台号）+XXXXXXXX（日期）+XX（卷号）+XXX（分卷号）+X（客户序号）+X（质量编码）；
        public Boolean ParseLittleRollBarCode(String barcode, out String workno, out String batchno, out String bigroll, out String littleroll)
        {
            workno = null;
            bigroll = null;
            batchno = null;
            littleroll = null;
            RollBarcode bar = ParseBarcode(barcode);
            if (bar == null)
                return false;

            workno = bar.WorkNoStr;
            bigroll = bar.BigRStr;
            batchno = bar.BatchNo;
            littleroll = bar.LittleRStr;
            return true;
        }


        public Boolean GetLittleRollBarcode(LittleRollBarcode barcode, String BigRollNo, String LittleRollNo, String Vendor, String QA)
        {
            barcode.WorkNoStr = WorkNo;
            barcode.BatchNo = BatchNo;
            //barcode.ProcessStr = "P";
            //barcode.MachineIDStr = MachineID;
            barcode.TimeStr = "1801201431";
            barcode.BigRStr = BigRollNo;
            barcode.LittleRStr = LittleRollNo;
            barcode.VendorStr = "0";
            barcode.QAStr = "0";
            return true;
        }
        public Boolean checkWorkNoLen()
        {
            if (gVariable.checkWorkNoLen) { 
                if (WorkNo.Length != RollBarcode.WorkNoStrLen)
                {
                    System.Windows.Forms.MessageBox.Show("工单号长度必须是" + RollBarcode.WorkNoStrLen);
                    return false;
                }
            }
            return true;
        }

        public String GetRealBigRollNo(String BigRollNo)
        {
                return BigRollNo;
        }
        public String GetRealLittleRollNo(String LittleRollNo)
        {
            if (LittleRollNo.Length == 6)
                return LittleRollNo.Substring(4, 2);
            else
                return LittleRollNo;
        }

        static public String[] WorkTimeTypes =
        {
            "日班",
        //    "中班",
            "晚班",
        };
        
        static public String[] WorkClassTypes =
        {
            "甲",
            "乙",
            "丙",
         //   "丁"
        };

        public String GetWorkTimeType()
        {
            String str1 = "";
            switch (WorkTType)
            {
                case WorkTimeType.DAYWORK:
                    str1 = "日班";
                    break;
                //case WorkTimeType.MIDDLEWORK:
                //    str1 = "中班";
                //    break;
                case WorkTimeType.NIGHTWORK:
                    str1 = "晚班";
                    break;
            }
            return str1;
        }

        public String GetWorkClassType()
        {
            String str = "";
            switch (WorkClsType)
            {
                case WorkClassType.JIA:
                    str = "甲";
                    break;
                case WorkClassType.YI:
                    str = "乙";
                    break;
                case WorkClassType.BING:
                    str = "丙";
                    break;
                //case WorkClassType.DING:
                //    str = "丁";
                //    break;
            }
            return str;

        }

        static String[] reportTitle =
        {
            "出库清单",
            "流延清单",
            "分切清单",
            "打印清单",
            "质检清单",
            "打包清单",
            "再造料清单"
        };

        static String columnStr;
        static String column;
        public mySQLClass mySQL;
        public String DatabaseName = "FilmPaper";
        public String BasicDataBase = "basicinfo";
        public String GlobalDataBase = "globaldatabase";
        public  String createColum(String [] Column)
        {
            columnStr = "id int(1) AUTO_INCREMENT primary key";
            int itemLength = 40;
            String defaultTableItem = " varchar(" + itemLength + ")";
            foreach (String item in Column)
            {
                columnStr += "," + item + defaultTableItem;
            }

            return columnStr;
        }
        public  String createinsertPart1(String[]  Column)
        {
            String str = "insert into Cut (";

            for (int i = 1; i < Column.Length; i++)
            {
                str += Column[i] + ",";
            }
            str += ") (";
            for (int i = 1; i < Column.Length; i++)
            {
                str += "@" + Column[i] + ",";
            }
            str += ")";

            return str;
        }
        public void CreateDataTable()
        {
            CreateDataTable(TableName, ColumnList);
            CreateJIaoJieDataTable();
        }
        public void CreateDataTable(String table,String [] Column)
        {
            mySQL = new mySQLClass();
            mySQLClass.createDatabase(DatabaseName);
            string createString = table + " (";


            column = "id int(1) AUTO_INCREMENT primary key";
            int itemLength = 40;
            String defaultTableItem = " varchar(" + itemLength + ")";
            foreach (String item in Column)
            {
                column += "," + item + defaultTableItem;
            }
            createString += column;
            createString += ") ENGINE = MYISAM CHARSET=utf8";

            mySQLClass.createDataTable(DatabaseName, createString);

        }

        enum JiaoJieColumnType
        {
            Date,
            Time,
            MState,
            WorkClassType,
            WorkTimeType,
            ProductCode,
            BatchNo,
            ManHour,
            WorkNo,
            WorkerNo,
            JiaoJiRecord,
        }
        public MySqlParameter CreateParameter(string str, String value)
        {
            MySqlParameter parameter = new MySqlParameter("@" + str, MySqlDbType.VarChar, 50);
            parameter.Value = value;
            return parameter;
        }

        public void updateItemCommand(string table, string[] columnArray, string[] value,int id)
        {
            String Part1 = "Update " + table + " SET ";
            String Part2 = "";
            String Part3 = "where id="+id;
            String ItemValue = "";
            for (int i = 1; i < columnArray.Length; i++)
            {
                if (value[i] == null)
                    ItemValue = "";
                else
                    ItemValue = value[i];
                if (i == 1)
                    Part2 += columnArray[i]+"='"+ItemValue+"'";
                else
                    Part2 += ","+ columnArray[i] + "='" + ItemValue + "'";
            }
            String cmd = Part1 + Part2 + Part3;
            mySQLClass.ExcuteNonQuery(DatabaseName, cmd, null);
        }
        public void updateItemCommandByBarcode(string table, string[] columnArray, string[] value, string outbarcode)
        {
            String Part1 = "Update " + table + " SET ";
            String Part2 = "";
            String Part3 = "where outputBarcode='" + outbarcode+"'";
            String ItemValue = "";
            for (int i = 1; i < columnArray.Length; i++)
            {
                if (value[i] == null)
                    ItemValue = "";
                else
                    ItemValue = value[i];
                if (i == 1)
                    Part2 += columnArray[i] + "='" + ItemValue + "'";
                else
                    Part2 += "," + columnArray[i] + "='" + ItemValue + "'";
            }
            String cmd = Part1 + Part2 + Part3;
            mySQLClass.ExcuteNonQuery(DatabaseName, cmd, null);
        }

        public void createInsertCommand(string table, string[] columnArray, string[] value)
        {
            String Part1 = "insert into " + table;
            String Part2 = "";
            String Part3 = "";

            int j = 0;
            int count = 0;
            for (int y = 1; y < value.Length; y++)
            {
                if (value[y] != null || value[y] != "")
                {
                    count++;
                }
            }
            MySqlParameter[] array = new MySqlParameter[count];
            for (int i = 1; i < columnArray.Length; i++)
            {
                if (value[i] != null || value[i] != "")
                {
                    if (i == 1)
                        Part2 += columnArray[i];
                    else
                        Part2 += "," + columnArray[i];

                    if (i == 1)
                        Part3 += "@" + columnArray[i];
                    else
                        Part3 += "," + "@" + columnArray[i];

                    array[j++] = CreateParameter(columnArray[i], value[i]);
                }
            }
            String cmd = Part1 + " (" + Part2 + " ) values(" + Part3 + ")";
            mySQLClass.ExcuteNonQuery(DatabaseName, cmd, array);
        }

        public String SearchBigLittleRollByBatchNoAndPlate(String  Batchno, String plateno)
        {
            System.Data.DataTable dt = null;

            String Finalstr = null;
            String bigroll;
            String smallroll;
            dt = mySQLClass.queryDataTableAction(DatabaseName, "select BigRollNo,LittleRollNo from " + TableName + " where BatchNo='" + Batchno + "' and PlateNo='"+ plateno +"'", null);

            if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
                return null;

            try
            {

                for (int i = 0; i< dt.Rows.Count; i++)
                {
                    bigroll = (String)dt.Rows[i][0];
                    smallroll = (String)dt.Rows[i][1];
                    Finalstr += bigroll + "-" + smallroll+";";
                } 
            }
            catch (Exception e)
            {
                Log.d("ProcessData", e.Message);
            }

            return Finalstr;
        }

        public Boolean SearchWeightLengthByBatNoAndPlate(String Batchno, String plateno ,out float totalWeight, out int totalLength)
        {
            
            System.Data.DataTable dt = null;

            //String Finalstr = null;
            String littleWeight;
            //String length;
            String str;
            float weight = 0;
            Boolean ret;
            //  float totalWeight = 0;
            totalWeight = 0;
            totalLength = 0;
            dt = mySQLClass.queryDataTableAction(DatabaseName, "select LittleWeight,ProductLength from " + TableName + " where BatchNo='" + Batchno + "' and PlateNo='" + plateno + "'", null);

            if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
                return false;

            try
            {

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    littleWeight = (String)dt.Rows[i][0];

                    ret = float.TryParse(littleWeight, out  weight);

                    if (ret)
                    {
                        totalWeight += weight;
                    }

                    str = (String)dt.Rows[i][1];
                    totalLength += Convert.ToInt32(str.Remove(str.Length - 1, 1));
                   // Finalstr += littleweight + "-" + length + ";";
                }
            }
            catch (Exception e)
            {
                Log.d("ProcessData", e.Message);
            }
            return true;
        }

        public Boolean SearchDBByOutBarCode()
        {
            System.Data.DataTable dt = null;
            Boolean findBarcode = false;
            String outbarcode = null;
            dt = mySQLClass.queryDataTableAction(DatabaseName, "select OutputBarcode from " + TableName + " where OutputBarcode='" + OutputBarcode + "'", null);

            if (dt == null|| dt.Rows==null||dt.Rows.Count==0)
                return findBarcode;

            try
            {
                outbarcode = (String)dt.Rows[0][0];
            }
            catch (Exception e)
            {
                Log.d("ProcessData", e.Message);
                findBarcode = false;
                outbarcode = null;
            }

            if (outbarcode != null)
                findBarcode = true;
            return findBarcode;
        }

        /// <summary>
        /// //
        /// </summary>
        /// <returns></returns>
        public  object[] GetComboStrsForProductCode()
        {
            System.Data.DataTable dt = null;
            dt = mySQLClass.queryDataTableAction(BasicDataBase, "select productCode from productspec", null);

            if (dt == null)
                return null;
            object[] productCodes = new object[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
                productCodes[i] = dt.Rows[i][0];
            return productCodes;
        }
        public object[] GetComboStrsForCustomerCode()
        {
            System.Data.DataTable dt = null;
            dt = mySQLClass.queryDataTableAction(BasicDataBase, "select customerCode from customerlist", null);

            if (dt == null)
                return null;
            object[] customerCode = new object[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
                customerCode[i] = dt.Rows[i][0];
            return customerCode;
        }


        public object[] GetComboStrsForMaterialCode()
        {
            System.Data.DataTable dt = null;
            object[] materialNames = null;
            try { 
            dt = mySQLClass.queryDataTableAction(BasicDataBase, "select materialCode from material", null);

            if (dt == null)
                return null;
            materialNames = new object[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
                materialNames[i] = dt.Rows[i][0];
            }
            catch (Exception e)
            {
                Log.d("ProcessData", e.Message);
            }
            return materialNames;

        }

        public String GetCustomerCodeByCustomerName(String customerName)
        {
            System.Data.DataTable dt = null;
            String customerCode = null ;
            dt = mySQLClass.queryDataTableAction(BasicDataBase, "select customerCode from customerlist where customerName='"+ customerName+"'", null);

            if (dt == null)
                return null;

            try
            {
                customerCode = (String)dt.Rows[0][0];
            }
            catch (Exception e)
            {
                Log.d("ProcessData", e.Message);
            }
  
            return customerCode;
        }
        public String GetCustomerNameByCustomerCode(String customerCode)
        {
            System.Data.DataTable dt = null;
            String customerName = null;
            dt = mySQLClass.queryDataTableAction(BasicDataBase, "select customerName from customerlist where customerCode='" + customerCode + "'", null);

            if (dt == null)
                return null;

            try
            {
                customerName = (String)dt.Rows[0][0];
            }
            catch (Exception e)
            {
                Log.d("ProcessData", e.Message);
            }

            return customerName;
        }
        public String GetProductNameByProductCode(String productCode)
        {
            System.Data.DataTable dt = null;
            String productName = null;
            dt = mySQLClass.queryDataTableAction(BasicDataBase, "select productName from productspec where productCode='" + productCode + "'", null);

            if (dt == null)
                return null;

            try
            {
                productName = (String)dt.Rows[0][0];
            }
            catch (Exception e)
            {

                Log.d("ProcessData", e.Message);
            }

            return productName;
        }

        public Boolean GetInfoByMaterialName(String materialName, out String materialCode, out String vendor, out String unit, out String fullstacknum)
        {

            System.Data.DataTable dt = null;
            dt = mySQLClass.queryDataTableAction(BasicDataBase, "select materialCode,Vendor,unit,fullStackNum from material where materialName='" + materialName + "'", null);

            if (dt == null)
            {
                vendor = null;
                materialCode = null;
                unit = null;
                fullstacknum = null;
                return false;
            }
            try
            {
                materialCode = (String)dt.Rows[0][0];
                vendor = (String)dt.Rows[0][1];
                unit = (String)dt.Rows[0][2];
                fullstacknum = (String)dt.Rows[0][3];
            }
            catch (Exception e)
            {
                Log.d("ProcessData", e.Message);
                materialCode = null;
                vendor = null;
                unit = null;
                fullstacknum = null;
                return false;
            }
            return true;
        }

        public Boolean GetInfoByMaterialCode(String materialCode , out String materialName, out String vendor, out String unit, out String fullstacknum)
        {

            System.Data.DataTable dt = null;
            dt = mySQLClass.queryDataTableAction(BasicDataBase, "select materialName,Vendor,unit,fullStackNum from material where materialCode='" + materialCode + "'", null);

            if (dt == null)
            {
                materialName = null;
                vendor = null;
                unit = null;
                fullstacknum = null;
                return false;
            }
            try
            {
                materialName = (String)dt.Rows[0][0];
                vendor = (String)dt.Rows[0][1];
                unit = (String)dt.Rows[0][2];
                fullstacknum = (String)dt.Rows[0][3];
            }
            catch (Exception e)
            {
                Log.d("ProcessData", e.Message);
                materialName = null;
                vendor = null;
                unit = null;
                fullstacknum = null;
                return false;
            }
            return true;
        }
        public Boolean ParsePlateInfo(String fixtureNum, ref int rollPerLayer, ref int layer )
        {
            int value1;
            int value2;
            Boolean result1 = false;
            Boolean result2 = false;
            if (fixtureNum == null || fixtureNum == "") {
                return false;
            }

            int index = fixtureNum.IndexOf('*');
            String str1 = fixtureNum.Substring(0, index);
            String str2 = fixtureNum.Substring(index+1);
            result1 = int.TryParse(str1, out value1);
            result2 = int.TryParse(str2, out value2);
            if (result1 && result2)
            {
                rollPerLayer = value1;
                layer = value2;
            }

            return true;
        }

        public Boolean GetInfoByProductCodeExt(String productcode, out String width, out String bom, out String fixtureNum,
    out String customer, out String RawMaterialCode, out String productLength, out String productName, out String productWeight)
        {

            System.Data.DataTable dt = null;
            dt = mySQLClass.queryDataTableAction(BasicDataBase, "select productWidth, Bom, fixtureNum, customer, RawMaterialCode, productLength, productName, productWeight from productspec where productcode='" + productcode + "'", null);

            if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
            {
                width = null;
                bom = null;
                fixtureNum = null;
                customer = null;
                RawMaterialCode = null;
                productLength = null;
                productName = null;
                productWeight = null;
                return false;
            }
            try
            {
                width = (String)dt.Rows[0][0];
                bom = (String)dt.Rows[0][1];
                fixtureNum = (String)dt.Rows[0][2];
                customer = (String)dt.Rows[0][3];
                RawMaterialCode = (String)dt.Rows[0][4];
                productLength = (String)dt.Rows[0][5];
                productName = (String)dt.Rows[0][6];
                productWeight = (String)dt.Rows[0][7];
            }
            catch (Exception e)
            {
                Log.d("ProcessData", e.Message);
                width = null;
                bom = null;
                fixtureNum = null;
                customer = null;
                RawMaterialCode = null;
                productLength = null;
                productName = null;
                productWeight = null;
                return false;
            }
            return true;
        }
        public Boolean  GetInfoByProductCode(String productcode,out  String width, out String bom, out String fixtureNum, 
            out String customer)
        {
            
            System.Data.DataTable dt = null;
            dt = mySQLClass.queryDataTableAction(BasicDataBase, "select productWidth, Bom, fixtureNum, customer from productspec where productcode='"+ productcode+"'", null);

            if (dt == null||dt.Rows==null||dt.Rows.Count==0) {
                width = null;
                bom = null;
                fixtureNum = null;
                customer = null;
                return false;
            }
            try { 
            width = (String)dt.Rows[0][0];
            bom = (String)dt.Rows[0][1];
            fixtureNum = (String)dt.Rows[0][2];
            customer = (String)dt.Rows[0][3];
            }
            catch (Exception e)
            {
                Log.d("ProcessData", e.Message);
                width = null;
                bom = null;
                fixtureNum = null;
                customer = null;
                return false;
            }
            return true;
        }

        public Boolean GetRecoveryInfoByProductCode(String productcode, out String productWeight, out String bom, out String Color)
        {

            System.Data.DataTable dt = null;
            dt = mySQLClass.queryDataTableAction(BasicDataBase, "select productWeight, Bom,  baseColor from productspec where productcode='" + productcode + "'", null);

            if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
            {
                productWeight = null;
                bom = null;
                Color = null;

                return false;
            }
            try
            {
                productWeight = (String)dt.Rows[0][0];
                bom = (String)dt.Rows[0][1];
                Color = (String)dt.Rows[0][2];
            }
            catch (Exception e)
            {
                Log.d("ProcessData", e.Message);
                productWeight = null;
                bom = null;
                Color = null;
                return false;
            }
            return true;
        }
        public Boolean GetProductInfoBySaleOrder(String saleOrderCode, out String productCode, out String productName, out String customer)
        {

            System.Data.DataTable dt = null;
            dt = mySQLClass.queryDataTableAction(GlobalDataBase, "select productCode, productName, customer from salesorderlist where salesOrderCode='" + saleOrderCode + "'", null);

            if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
            {
                productCode = null;
                productName = null;
                customer = null;
                return false;
            }
            productCode = (String)dt.Rows[0][0];
            productName = (String)dt.Rows[0][1];
            customer = (String)dt.Rows[0][2];
            return true;
        }

        public Boolean UpdateMStateInDB(String barcode)
        {

            Boolean bSuccess;
            String cmd = "update "+ TableName + " Set MState='1'" + " where OutputBarcode='" + barcode + "'";
            bSuccess = mySQLClass.updateDB(DatabaseName, cmd);


            return bSuccess;
        }

        public virtual Boolean ParseBarCode(String barcode)
        {
            //XXXXXXXXXX(工单编码) + X（工序）+X（机台号）+XXXXXXXX（日期）+XX（卷号）+XXX（分卷号）+X（客户序号）+X（质量编码）；
            try { 
            String BCWorkNo = barcode.Substring(0, 18);
            String BCProcess = barcode.Substring(18, 1);
            String BCDateTime = barcode.Substring(19, 8);
            String BCBigRollNo = barcode.Substring(27, 2);
            String BCLittleRollNo = barcode.Substring(29, 3);
            String BCCustomerNo = barcode.Substring(32, 2);
            String BCQuality = barcode.Substring(34, 1);
                WorkNo = BCWorkNo;
            }
            catch (Exception e)
            {
                Log.d("ProcessData", e.Message);
                return false;
            }
            //BigRollNo = BCBigRollNo;
            //return true;
            return true;
        }

        public virtual String[] SetColumnDataArray()
        {
            return null;
        }
        public  void insertOneRow()
        {
           // OutputBarcode = "S17110906L302J118032622560500101";
            //if (!SearchDBByOutBarCode())
          //  { 
                String[] values = SetColumnDataArray();
                createInsertCommand(TableName, ColumnList, values);
      //      }
            //else
            //{
            //    String[] values = SetColumnDataArray();

            //    updateItemCommandByBarcode(TableName, ColumnList, values, OutputBarcode);
            //}
        }



        public void insertOneRowMSateZero()
        {
            MState = "0";
            insertOneRow();
            //String[] values = SetColumnDataArray();
            //createInsertCommand(TableName, ColumnList, values);
        }
        public void UpdateOneRow(int id)
        {
            String[] values = SetColumnDataArray();
            updateItemCommand(TableName, ColumnList, values, id);
        }


        public virtual void SetInputDataFromDataBase(DataTable dt)
        {

        }
        public Boolean GetSelItemFromDB(int id)
        {
            DataTable dtEditor;
            mySQLClass a = new mySQLClass();
            String cmd = "Select * from "+TableName+" where id=" + id;
            dtEditor = mySQLClass.queryDataTableAction(DatabaseName, cmd, null);
            SetInputDataFromDataBase(dtEditor);

            return true;
        }

        public Boolean GetLastItemFromDB()
        {
            DataTable dtEditor;
            mySQLClass a = new mySQLClass();
            String cmd = "Select * from " + TableName + " order by id desc limit 1";
            dtEditor = mySQLClass.queryDataTableAction(DatabaseName, cmd, null);
            if (dtEditor == null || dtEditor.Rows == null || dtEditor.Rows.Count == 0)
                return false;
            SetInputDataFromDataBase(dtEditor);
            return true;
        }

        public virtual void PrintLabel()
        {

        }
        public String GetDateTime()
        {
            String str = "";
            str += WorkDate;
            str += " ";
            str += WorkTime;
            return str;
        }
        public String GetDateTimeForBarcode()
        {
            String str = RollBarcode.CreateDateTimeforBarcode(WorkDate, WorkTime);
            return str;
        }
        public void UpdateDateTime()
        {
            WorkDate = DateTime.Now.ToString("yyyy-MM-dd");
            WorkTime = DateTime.Now.ToString("HH:mm:ss");

        }

        public Boolean ParseWorkNo(String workno,out String batchNo, out String DeviceNo, out String WorkNoSn)

        {
            try {

                RollBarcode.ParseWorkNo(workno, out batchNo, out DeviceNo, out WorkNoSn);
            }
            catch (Exception e)
            {
                Log.d("ProcessData", e.Message);
                //   orderNo = null;
                batchNo = null;
                DeviceNo = null;
                WorkNoSn = null;
                return false;
            }
            return true;
        }


        #region JiaoJIe Recorad
        String JiaoJiaTable = "JIaoJie";
        enum JiaoJIeColumnType
        {  
            SN,
            Date,
            Time,
            MState,
            WorkClassType,
            WorkTimeType,
            ProductCode,
            BatchNo,
            WorkNo,
            WorkerNo,
            JiaoJiRecord,
            MAX_COLUMN
        };  


        public static String[] JiaJieColumn =
            {
            "SN",
            "Date",
            "Time",
            "MState",
            "WorkClassType",
            "WorkTimeType",
            "ProductCode",
            "BatchNo",
            "WorkNo",
            "WorkerNo",
            "JiaoJiRecord",
            };

        public String JiaoJiRecord;

        public void CreateJIaoJieDataTable(String table)
        {
            CreateDataTable(table, JiaJieColumn);
        }
        public void CreateJIaoJieDataTable()
        {

            CreateJIaoJieDataTable(JiaoJiaTable);
        }

        public virtual String [] SetJiaoJieColumnDataArray()
        {
            String[] values = new String[JiaJieColumn.Length];
            System.Diagnostics.Debug.Assert((int)JiaoJIeColumnType.MAX_COLUMN == JiaJieColumn.Length);
            values[(int)JiaoJIeColumnType.Date] = WorkDate;// DateTime.Now.ToString("yyyy-MM-dd");
            values[(int)JiaoJIeColumnType.Time] = WorkTime;// DateTime.Now.ToString("HH-mm-ss");
            values[(int)JiaoJIeColumnType.MState] = MState;

            values[(int)JiaoJIeColumnType.WorkClassType] = WorkClsType.ToString();
            values[(int)JiaoJIeColumnType.WorkTimeType] = WorkTType.ToString();
            values[(int)JiaoJIeColumnType.ProductCode] = ProductCode;
            values[(int)JiaoJIeColumnType.BatchNo] = BatchNo;
            values[(int)JiaoJIeColumnType.WorkNo] = WorkNo;
            values[(int)JiaoJIeColumnType.WorkerNo] = WorkerNo;
            values[(int)JiaoJIeColumnType.JiaoJiRecord] = JiaoJiRecord;

            return values;
        }

        public void InsertJIaoJieRecord()
        {
            UpdateDateTime();
            String[] values = SetJiaoJieColumnDataArray();
            
            createInsertCommand(JiaoJiaTable, JiaJieColumn, values);
        }

        #endregion


    }

    public class ProcessSysData
    {
        public String databaseName = "FilmPaper";
        public int RowIndex = 0;
        public DataTable CurDT = null;
        public String Date1;
        public String Date2;
        public String WorkNo;
        public void PrintDataTablePriew(DataTable dt)
        {
            Report_BangMa tp = new Report_BangMa();
            //Report_Cut tp = new Report_Cut();
            tp.PrintPriview(dt, "Hello");
        }
        public virtual  String CreateSlectStrForColumn()
        {
            return null;
        }

        public virtual String CreateSelectOpiton()
        {
            return null;
        }

        public String createSelectStr(String Table,  String [] dataBase, String[] Title)
        {
            String SelStr = "select ";
            for (int i = 0; i < Title.Length; i++)
            {
                if (i == 0)
                    SelStr += dataBase[i] + " as " + Title[i];
                else
                    SelStr += "," + dataBase[i] + " as " + Title[i];
            }
            SelStr += " from " + Table;
            return SelStr;
        }

       public void SetDateTimePickerFormat(DateTimePicker dateTimePicker1, DateTimePicker dateTimePicker2)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom; //设置为显示格式为自定义
            dateTimePicker1.CustomFormat = "yyyy-MM-dd"; //设置显示格式
            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Format = DateTimePickerFormat.Custom; //设置为显示格式为自定义
            dateTimePicker2.CustomFormat = "yyyy-MM-dd"; //设置显示格式
            dateTimePicker2.Value = DateTime.Now;
        }

        public DataTable UpdateDBView(DataGridView dgview ,String SelStr)
        {
            DataTable dt;
            mySQLClass a = new mySQLClass();
            dt = mySQLClass.updateDBView(databaseName, SelStr, dgview);
            CurDT = dt;
            return dt;
        }
        public DataTable UpdateDBViewBy2Date(DataGridView dgview)
        {
           DataTable dt = UpdateDBViewBy2Date(dgview, Date1, Date2);
           CurDT = dt;
           return dt;
        }
        public DataTable UpdateDBViewBy2Date(DataGridView dgview, String date1,String date2)
        {
            DataTable dt;
            mySQLClass a = new mySQLClass();
            String SelStr = CreateSlectStrForColumn();
            SelStr += " where Date between '" + date1 + "' and '" + date2 + "'";
            dt = mySQLClass.updateDBView(databaseName, SelStr, dgview);
            CurDT = dt;
            return dt;
        }

        public DataTable UpdateDBViewBySelection(DataGridView dgview)
        {
            DataTable dt;
            String SelStr = CreateSlectStrForColumn();

            SelStr += " where " + CreateSelectOpiton(); //ok
            dt = UpdateDBView(dgview, SelStr);
            CurDT = dt;
            return dt;
        }

        public Boolean UpdateMStateInDB(String barcode, String TableName)
        {

            Boolean bSuccess;
            String cmd = "update " + TableName + " Set MState='1'" + " where OutputBarcode='" + barcode + "'";
            bSuccess = mySQLClass.updateDB(databaseName, cmd);


            return bSuccess;
        }


        public virtual String GetTableName()
        {
            return null;
        }


        public DataTable GetDTSelItemsFromDB()
        {
            DataTable dt;
            mySQLClass a = new mySQLClass();
            String table = GetTableName();
            String SelStr = "select * from " + table + " where " + CreateSelectOpiton();

            dt = mySQLClass.queryDataTableAction(databaseName, SelStr, null);
            return dt;
        }







    }
}
