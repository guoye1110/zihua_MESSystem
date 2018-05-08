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
		//index
        private const int MACHINE_ID_INDEX = 1;
		private const int BARCODE_INDEX = 2;
		private const int PACKING_BARCODE_INDEX= 3;
		private const int UPLOAD_TIME_INDEX = 4;
		private const int SCAN_TIME_INDEX = 5;
		private const int PRODUCT_CODE_INDEX = 6;
		private const int WORKER_ID_INDEX = 7;
		private const int ROLL_NUMBER_INDEX = 8;
		private const int TOTAL_WEIGHT_INDEX = 9;
		private const int TOTAL_LENGTH_INDEX = 10;
		private const int TOTAL_DATAGRAM_NUM = TOTAL_LENGTH_INDEX+1;

		private const string c_dbName = "globaldatabase";
        private const string c_TableName = "finalpacking";
        private const string c_FileName = "..\\..\\data\\globalTables\\finalPacking.xlsx";

		public struct finalpacking_t{
			public string machineID;
			public string barcode;
			public string packageBarcode;
			public string uploadTime;
			public string scanTime;
			public string productCode;
			public string workerID;
			public int rollNumber;
			public int totalWeight;
			public int totalLength;
		}

		public string Serialize(finalpacking_t st)
		{
			string str = null;

			str += st.machineID		+ ";" + st.barCode 		+ ";" + st.packageBarcode	+ ";" + st.uploadTime + ";";
			str += st.scanTime		+ ";" + st.productCode	+ ";" + st.workerID			+ ";" + st.rollNumber + ";";
			str += st.totalWeight	+ ";" + st.totalLength;
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
			finalpacking_t st;

			input = strInput.Split(';');

			if (input.Length < TOTAL_DATAGRAM_NUM)
				return null;

			st.barcode = input[BARCODE_INDEX];
			st.machineID = input[MACHINE_ID_INDEX];
			st.packageBarcode = input[PACKING_BARCODE_INDEX];
			st.productCode = input[PRODUCT_CODE_INDEX];
			st.rollNumber = int.Parse(input[ROLL_NUMBER_INDEX]);
			st.scanTime = input[SCAN_TIME_INDEX];
			st.totalLength = int.Parse(input[TOTAL_LENGTH_INDEX]);
			st.totalWeight = int.Parse(input[TOTAL_WEIGHT_INDEX]);
			st.uploadTime = input[UPLOAD_TIME_INDEX];
			st.workerID = input[WORKER_ID_INDEX];
			
			return st;
		}

        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(finalpacking_t st)
        {
            int index;
            string[] itemName;
			string insertString=null;
			string connectionString;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_FileName);

            try {
                index = 0;
                itemName = insertString.Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + c_TableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
                if (st.machineID!=null) 		myCommand.Parameters.AddWithValue(itemName[index++], st.machineID);
				if (st.barcode!=null) 			myCommand.Parameters.AddWithValue(itemName[index++], st.barcode);
                if (st.packageBarcode!=null)	myCommand.Parameters.AddWithValue(itemName[index++], st.packageBarcode);
				if (st.uploadTime!=null) 		myCommand.Parameters.AddWithValue(itemName[index++], st.uploadTime);
				if (st.scanTime!=null) 			myCommand.Parameters.AddWithValue(itemName[index++], st.scanTime);
                if (st.productCode!=null) 		myCommand.Parameters.AddWithValue(itemName[index++], st.productCode);
				if (st.workerID!=null) 			myCommand.Parameters.AddWithValue(itemName[index++], st.workerID);
				if (st.rollNumber!=null) 		myCommand.Parameters.AddWithValue(itemName[index++], st.rollNumber);
                if (st.totalWeight!=null) 		myCommand.Parameters.AddWithValue(itemName[index++], st.totalWeight);
				if (st.totalLength!=null) 		myCommand.Parameters.AddWithValue(itemName[index++], st.totalLength);

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
			string insertString;
			string[] insertStringSplitted;
			string connectionString;
			string[] strArray;

			strArray = Format(st);

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			getDatabaseInsertStringFromExcel(ref insertString, c_TableName);
			insertStringSplitted = insertString.Split(new char[2]{',','@'});

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + ";" + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

				myCommand.CommandText = "update ";
				myCommand.CommandText += "`" + c_TableName + "` ";
				myCommand.CommandText += "set ";

				for (int i=0;i<strArray.Length;i++){
					if (strArray[i] == null)	continue;

					myCommand.CommandText += "`" + insertStringSplitted[i+1] + "`=" + strArray[i];
					if (i != strArray.Length )
						myCommand.CommandText += ",";
				}

				myCommand.CommandText += "where `" + insertStringSplitted[PACKING_BARCODE_INDEX] + "`=" + value;

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
