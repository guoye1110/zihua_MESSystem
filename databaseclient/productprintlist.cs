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
	public class productprintlistDB : mySQLClass
	{
		//index
        private const int MACHINE_ID_INDEX = 1;
		private const int MATERIAL_BARCODE_INDEX = 2;
		private const int MATERIAL_SCAN_TIME_INDEX = 3;
		private const int PRODUCT_BARCODE_INDEX = 4;
		private const int PRODUCT_SCAN_TIME_INDEX = 5;
		private const int DISPATCH_CODE_INDEX = 6;
		private const int BATCH_NUM_INDEX = 7;
		private const int LARGE_INDEX_INDEX = 8;
		private const int WEIGHT_INDEX = 9;
		private const int ERROR_STATUS_INDEX = 10;
		private const int TOTAL_DATAGRAM_NUM = ERROR_STATUS_INDEX;

		private const string c_dbName = "globaldatabase";
        private const string c_TableName = "productprintlist";
        private const string c_FileName = "..\\..\\data\\globalTables\\productPrintList.xlsx";

		public struct productprint_t{
            public string machineID;
            public string materialBarCode;
            public string materialScanTime;
            public string productBarCode;
            public string productScanTime;
            public string dispatchCode;
            public string batchNum;
            public string largeIndex;
            public float? weight;
			public string errorStatus;
		}

		public string Serialize(productprint_t st)
		{
			string str = null;

			str += st.machineID  		+ ";" + st.materialBarCode 	+ ";" + st.materialScanTime	+ ";" + st.productBarCode	+ ";";
			str += st.productScanTime	+ ";" + st.dispatchCode		+ ";" + st.batchNum 		+ ";" + st.largeIndex		+ ";";
			str += st.weight 			+ ";" + st.errorStatus;
			return str;
		}

		public string[] Format(productprint_t st)
		{
			string str;
			string[] strArray;

			str = Serialize(st);
			strArray = str.Split(';');
			return strArray;
		}

		public productprint_t? Deserialize(string strInput)
		{
			string[] input;
			productprint_t st = new productprint_t();

			input = strInput.Split(';');

			if (input.Length < TOTAL_DATAGRAM_NUM)
				return null;

			st.machineID = input[MACHINE_ID_INDEX];
			st.materialBarCode = input[MATERIAL_BARCODE_INDEX];
			st.materialScanTime = input[MATERIAL_SCAN_TIME_INDEX];
			st.productBarCode = input[PRODUCT_BARCODE_INDEX];
			st.productScanTime = input[PRODUCT_SCAN_TIME_INDEX];
			st.dispatchCode = input[DISPATCH_CODE_INDEX];
			st.batchNum = input[BATCH_NUM_INDEX];
			st.largeIndex = input[LARGE_INDEX_INDEX];
			if (input[WEIGHT_INDEX]!="")	st.weight = Convert.ToSingle(input[WEIGHT_INDEX]);
			st.errorStatus = input[ERROR_STATUS_INDEX];

			return st;
		}

		public int updaterecord_ByMaterialBarCode(productprint_t st, string barcode)
		{
			string insertString=null;
			string[] insertStringSplitted;
			string connectionString;
			string[] inputArray;
			string[] stringSeparators = new string[] { ",@" };

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

				bool first = true;;
				for (int i=0;i<inputArray.Length;i++){
					if (inputArray[i] == null || inputArray[i] == "")	continue;
					if (!first)	myCommand.CommandText += ",";
					first = false;

					if (i==inputArray.Length-1)
						myCommand.CommandText += "`" + insertStringSplitted[i+1].Remove(insertStringSplitted[i+1].Length-1) + "`=" + "\'" + inputArray[i] + "\'";
					else
						myCommand.CommandText += "`" + insertStringSplitted[i+1] + "`=" + "\'" + inputArray[i] + "\'";
				}

				myCommand.CommandText += "where `" + insertStringSplitted[MATERIAL_BARCODE_INDEX] + "`=" + "\'" + barcode + "\'";

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

		public productprint_t? readlastrecord_ByMachineID(int machineId)
		{
			string commandText;
			string[] recordArray;
			string insertString=null;
			productprint_t? result;
			string[] stringSeparators = new string[] { ",@" };
			string[] insertStringSplitted;
			
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
			insertStringSplitted = insertString.Split(stringSeparators, StringSplitOptions.None);
			
			commandText = "select * from `" + c_TableName + "` order by id DESC";
			commandText += " where `";
			commandText += insertStringSplitted[MACHINE_ID_INDEX] + "`=" + machineId;
			recordArray = mySQLClass.databaseCommonReadingUnsplitted(c_TableName, commandText);
			if (recordArray == null)	return null;

			result = Deserialize(recordArray[0]);
			return result;
		}
		
        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(productprint_t st)
        {
            int index;
            string[] itemName;
			string insertString=null;
			string connectionString;
			string[] inputArray;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_FileName);

			inputArray = Format(st);

            try
            {
                index = 1;
                itemName = insertString.Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + c_TableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);

				for (index=1;index<=TOTAL_DATAGRAM_NUM;index++)
					myCommand.Parameters.AddWithValue(itemName[index], inputArray[index-1]);
					
                /*myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.machineID);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.materialBarCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.materialScanTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.productBarCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.productScanTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.dispatchCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.batchNum);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.largeIndex);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.weight);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.errorStatus);*/

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(c_dbName + ":" + c_TableName + ": write record failed! " + ex);
            }
            return -1;
        }

/*
		public int updateProductScancode(productprintlist_t st_productprint)
        {
			string insertString;
			string[] insertStringSplitted;
			string connectionString;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_productprintlistFileName);
			insertStringSplitted = insertString.Split(',@');

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "update ";
                myCommand.CommandText += "`" + c_productprintlistTableName + "` ";
				myCommand.CommandText += "set ";
				myCommand.CommandText += "`" + insertStringSplitted[PRODUCT_BARCODE_INDEX] + "`=" + st_productprint.productBarCode + ",";
				myCommand.CommandText += "`" + insertStringSplitted[PRODUCT_SCAN_TIME_INDEX] + "`=" + st_productprint.productScanTime;
				myCommand.CommandText += "where ";
				myCommand.CommandText += "`" + insertStringSplitted[MATERIAL_BARCODE_INDEX] + "`=" + st_productprint.materialBarCode;

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(c_dbName + ":" + c_productprintlistTableName + ": update product barcode failed! " + ex);
            }
            return -1;			
		}
*/
	}
}

