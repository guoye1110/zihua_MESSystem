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
		private const int TOTAL_DATAGRAM_NUM = ERROR_STATUS_INDEX+1;

		private const string c_dbName = "globaldatabase";
        private const string c_productprintlistTableName = "productprintlist";
        private const string c_productprintlistFileName = "..\\..\\data\\globalTables\\productPrintList.xlsx";

		public struct productprint_t{
            public string machineID;
            public string materialBarCode;
            public string materialScanTime;
            public string productBarCode;
            public string productScanTime;
            public string dispatchCode;
            public string batchNum;
            public string largeIndex;
            public string weight;
			public string errorStatus;
			/*public productprint_t(int value){
				this.machineID = null;
				this.materialBarCode = null;
				this.materialScanTime = null;
				this.productBarCode = null;
				this.productScanTime = null;
				this.dispatchCode = null;
				this.batchNum = null;
				this.largeIndex = null;
				this.weight = null;				
			}*/
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
			productprint_t st;

			input = strInput.Split(';');

			if (input.Length < TOTAL_DATAGRAM_NUM-1)
				return null;

			st.machineID = input[MACHINE_ID_INDEX];
			st.materialBarCode = input[MATERIAL_BARCODE_INDEX];
			st.materialScanTime = input[MATERIAL_SCAN_TIME_INDEX];
			st.productBarCode = input[PRODUCT_BARCODE_INDEX];
			st.productScanTime = input[PRODUCT_SCAN_TIME_INDEX];
			st.dispatchCode = input[DISPATCH_CODE_INDEX];
			st.batchNum = input[BATCH_NUM_INDEX];
			st.largeIndex = input[LARGE_INDEX_INDEX];
			st.weight = input[WEIGHT_INDEX];
			st.errorStatus = input[ERROR_STATUS_INDEX];

			return st;
		}

		public int updaterecord_ByMaterialBarCode(string[] strArray, string barcode)
		{
			string insertString=null;
			string[] insertStringSplitted;
			string connectionString;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			getDatabaseInsertStringFromExcel(ref insertString, c_productprintlistTableName);
            insertStringSplitted = insertString.Split(new char[2]{',','@' });

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + ";" + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

				myCommand.CommandText = "update ";
				myCommand.CommandText += "`" + c_productprintlistTableName + "` ";
				myCommand.CommandText += "set ";

				for (int i=0;i<strArray.Length;i++){
					if (strArray[i] == null)	continue;

					myCommand.CommandText += "`" + insertStringSplitted[i+1] + "`=" + strArray[i];
					if (i != strArray.Length )
						myCommand.CommandText += ",";
				}

				myCommand.CommandText += "where `" + insertStringSplitted[MATERIAL_BARCODE_INDEX] + "`=" + barcode;

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

/*
		public productprintlist_t[] readrecordBy(string materialScancode)
		{
			string commandText;
			string[] recordArray;
			productprintlist_t[] st_productprint, result;
			
			recordArray = mySQLClass.databaseCommonReadingUnsplitted(c_productprintlistTableName, commandText);
			if (recordArray == null)	return null;

			st_productprint = new productprintlist_t[recordArray.GetLength(0)];

			for (int i=0;i<recordArray.GetLength(0);i++){
				result = Deserialize(recordArray[i]);
				if (result != null)	st_productprint[i] = result.Value;
			}
			return st_productprint;
		}
*/
		
        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(productprint_t st_productprint)
        {
            int index;
            string[] itemName;
			string insertString=null;
			string connectionString;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_productprintlistFileName);

            try
            {
                index = 1;
                itemName = insertString.Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + c_productprintlistTableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.machineID);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.materialBarCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.materialScanTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.productBarCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.productScanTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.dispatchCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.batchNum);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.largeIndex);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.weight);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.errorStatus);

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(c_dbName + ":" + c_productprintlistTableName + ": write record failed! " + ex);
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

