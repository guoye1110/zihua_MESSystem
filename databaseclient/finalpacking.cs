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
		private const int TOTAL_DATAGRAM_NUM = TOTAL_LENGTH_INDEX;

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
			public uint? rollNumber;
			public float? totalWeight;
			public float? totalLength;
		}

		public string Serialize(finalpacking_t st)
		{
			string str = null;

			str += st.machineID		+ ";" + st.barcode 		+ ";" + st.packageBarcode	+ ";" + st.uploadTime + ";";
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
			st.rollNumber = Convert.ToUInt32(input[ROLL_NUMBER_INDEX],10);
			st.scanTime = input[SCAN_TIME_INDEX];
			st.totalLength = Convert.ToSingle(input[TOTAL_LENGTH_INDEX]);
			st.totalWeight = Convert.ToSingle(input[TOTAL_WEIGHT_INDEX]);
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

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			getDatabaseInsertStringFromExcel(ref insertString, c_TableName);
			insertStringSplitted = insertString.Split(new char[2]{',','@'});

			inputArray = Format(st);

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + ";" + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

				myCommand.CommandText = "update ";
				myCommand.CommandText += "`" + c_TableName + "` ";
				myCommand.CommandText += "set ";

				for (int i=0;i<inputArray.Length;i++){
					if (inputArray[i] == null || inputArray[i] == "")	continue;

					myCommand.CommandText += "`" + insertStringSplitted[i+1] + "`=" + inputArray[i];
					if (i != inputArray.Length )
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
