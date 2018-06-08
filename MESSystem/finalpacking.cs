using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Web;
using System.Web.Services;
using System.Threading;

namespace MESSystem.common
{
	public class finalpackingDB : mySQLClass
	{
        //<打包条码>;<客户名>;<铲板号>;<产品名>;<订单号>;<原材料代码>;<产品代号>;<产品批号>;<卷数>;<基重>;<宽度>;<重量>;<长度>;<小卷条码信息>

        //index
        private const int MACHINE_ID_INDEX = 1;
		private const int UPLOAD_TIME_INDEX = 2;
		private const int SCAN_TIME_INDEX = 3;
		private const int PACKING_BARCODE_INDEX = 4;
		private const int PLATE_NO_INDEX = 5;
		private const int SALES_ORDER_INDEX = 6;
		private const int PRODUCT_CODE_INDEX = 7;
		private const int BATCH_NUM_INDEX = 8;
		private const int ROLL_NUMBER_INDEX = 9;
		private const int TOTAL_WEIGHT_INDEX = 10;
		private const int TOTAL_LENGTH_INDEX = 11;
		private const int WORKER_ID_INDEX = 12;
		private const int ROLL_LIST_INDEX = 13;
        private const int TOTAL_DATAGRAM_NUM = ROLL_LIST_INDEX;

		private const string c_dbName = "globaldatabase";
        private const string c_TableName = "productPackingList";
        private const string c_FileName = "..\\..\\data\\globalTables\\finalPacking.xlsx";

		public struct finalpacking_t{
			public string machineID;
			public string uploadTime;
			public string scanTime;
			public string packageBarcode;
			public string plateNo;
			public string salesOrder;
			public string productCode;
			public string batchNum;
			public uint? rollNumber;
			public float? totalWeight;
			public float? totalLength;
			public string workerID;
            public string rollList;
		}

		public string Serialize(finalpacking_t st)
		{
			string str = null;

			str += st.machineID		+ ";" + st.uploadTime		+ ";" + st.scanTime 	+ ";" + st.packageBarcode	+ ";";
			str += st.plateNo		+ ";" + st.salesOrder		+ ";" + st.productCode 	+ ";" + st.batchNum			+ ";";
            str += st.rollNumber	+ ";" + st.totalWeight		+ ";" + st.totalLength	+ ";" + st.workerID			+ ";";
			str += st.rollList;
			return str;
		}

		public string[] Format(finalpacking_t st)
		{
			string str;
			string[] strArray;

			str = Serialize(st);
			strArray = str.Split(';');
			return strArray;
		}

		public finalpacking_t? Deserialize(string strInput)
		{
			string[] input;
			finalpacking_t st = new finalpacking_t();

			input = strInput.Split(';');

			if (input.Length < TOTAL_DATAGRAM_NUM)
				return null;

			st.machineID = input[MACHINE_ID_INDEX];
			st.uploadTime = input[UPLOAD_TIME_INDEX];
			st.scanTime = input[SCAN_TIME_INDEX];
			st.packageBarcode = input[PACKING_BARCODE_INDEX];
			st.plateNo = input[PLATE_NO_INDEX];
			st.salesOrder = input[SALES_ORDER_INDEX];
			st.productCode = input[PRODUCT_CODE_INDEX];
			st.batchNum = input[BATCH_NUM_INDEX];
			if (input[ROLL_NUMBER_INDEX]!="")	st.rollNumber = Convert.ToUInt32(input[ROLL_NUMBER_INDEX],10);
			if (input[TOTAL_WEIGHT_INDEX]!="")	st.totalWeight = Convert.ToSingle(input[TOTAL_WEIGHT_INDEX]);
			if (input[TOTAL_LENGTH_INDEX]!="")	st.totalLength = Convert.ToSingle(input[TOTAL_LENGTH_INDEX]);
            st.workerID = input[WORKER_ID_INDEX];
            st.rollList = input[ROLL_LIST_INDEX];
			
			return st;
		}

		public finalpacking_t? readrecordBy_packageBarcode(string packageBarcode)
        {
			finalpacking_t? dd=null;
			string[] recordArray;
			string insertString,insertStringSplitted;
			string[] stringSeparators = new string[] { ",@" };

			getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
			insertStringSplitted = insertString.Split(stringSeparators, StringSplitOptions.None);

			string commandText = "select * from `" + c_TableName + "`";
			commandText += "where `" + insertStringSplitted[PACKING_BARCODE_INDEX] + "`=" + "\'" + packageBarcode + "\'";
			
			recordArray = databaseCommonReadingUnsplitted(c_dbName, commandText);
			if (recordArray!=null){
				dd = Deserialize(recordArray[0]);
			}
			return dd;
		}

        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(finalpacking_t st)
        {
            int index;
            string[] itemName;
			string insertString=null;
			string connectionString;
			string[] inputArray;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_FileName);

			inputArray = Format(st);

            try {
                index = 1;
                itemName = insertString.Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + c_TableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
				for (index=1;index<=TOTAL_DATAGRAM_NUM;index++)
					myCommand.Parameters.AddWithValue(itemName[index], inputArray[index-1]);

                /*myCommand.Parameters.AddWithValue(itemName[index++], st.machineID);
				myCommand.Parameters.AddWithValue(itemName[index++], st.barcode);
                myCommand.Parameters.AddWithValue(itemName[index++], st.packageBarcode);
				myCommand.Parameters.AddWithValue(itemName[index++], st.uploadTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st.scanTime);
                myCommand.Parameters.AddWithValue(itemName[index++], st.productCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st.workerID);
				myCommand.Parameters.AddWithValue(itemName[index++], st.rollNumber.ToString());
                myCommand.Parameters.AddWithValue(itemName[index++], st.totalWeight.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.totalLength.ToString());*/

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(c_dbName + "write to " + c_TableName + " failed!" + ex);
            }
            return -1;
        }

		public int updaterecordBy_packageBarcode(finalpacking_t st, string value)
		{
			string insertString=null;
			string[] insertStringSplitted;
			string connectionString;
			string[] inputArray;
			string[] stringSeparators=new string[] {",@"};

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			getDatabaseInsertStringFromExcel(ref insertString, c_TableName);
			insertStringSplitted = insertString.Split(stringSeparators, StringSplitOptions.None);

			inputArray = Format(st);

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + ";" + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

				myCommand.CommandText = "update ";
				myCommand.CommandText += "`" + c_TableName + "` ";
				myCommand.CommandText += "set ";

				bool first=true;
				for (int i=0;i<inputArray.Length;i++){
					if (inputArray[i] == null || inputArray[i] == "")	continue;
					if (!first)		myCommand.CommandText += ",";
					first = false;

					myCommand.CommandText += "`" + insertStringSplitted[i+1] + "`=" + "\'" + inputArray[i] + "\'";
				}

				myCommand.CommandText += "where `" + insertStringSplitted[PACKING_BARCODE_INDEX] + "`=" + "\'" + value + "\'";

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(c_dbName + ":" + c_TableName + ": update product barcode failed! " + ex);
            }
            return -1;
		}

	}
}
