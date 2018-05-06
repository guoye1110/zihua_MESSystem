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
	public class productslitlistDB : mySQLClass
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
		private const int SMALL_INDEX_INDEX = 9;
		private const int CUSTOMER_INDEX_INDEX = 10;
		private const int ERROR_STATUS_INDEX = 11;
		private const int NUM_OF_JOINS = 12;
		private const int WEIGHT_INDEX = 13;
		private const int TOTAL_DATAGRAM_NUM = WEIGHT_INDEX+1;

		private const string c_dbName = "globaldatabase";
        private const string c_productslitlistTableName = "productslitlist";
        private const string c_productslitlistFileName = "..\\..\\data\\globalTables\\productSlitList.xlsx";

		public struct productslit_t{
			string machineID;
			string materialBarCode;
			string materialScanTime;
			string productBarCode;
			string productScanTime;
			string dispatchCode;
			string batchNum;
			string largeIndex;
			string smallIndex;
			string customerIndex;
			string errorStatus;
			string numOfJoins;
			string weight;
		}

		public string Serialize(productslit_t st)
		{
			string str = null;

			str += st.machineID  		+ ";" + st.materialBarCode 	+ ";" + st.materialScanTime	+ ";" + st.productBarCode	+ ";";
			str += st.productScanTime	+ ";" + st.dispatchCode		+ ";" + st.batchNum 		+ ";" + st.largeIndex		+ ";";
			str += st.smallIndex		+ ";" + st.customerIndex	+ ";" + st.errorStatus		+ ";" + st.numOfJoins		+ ";";
			str += st.weight;
			return str;
		}

		public string[] Format(productslit_t st)
		{
			string str;
			string[] strArray;

			str = Serialize(st);
			strArray = str.Split(';');
			return strArray;
		}

		public productslit_t? Deserialize(string strInput)
		{
			string[] input;
			productslit_t st;

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
			st.smallIndex = input[SMALL_INDEX_INDEX];
			st.customerIndex = input[CUSTOMER_INDEX_INDEX];
			st.errorStatus = input[ERROR_STATUS_INDEX];
			st.numOfJoins = input[NUM_OF_JOINS];
			st.weight = input[WEIGHT_INDEX];

			return st;
		}

		public int updaterecord_ByMaterialBarCode(string[] strArray, string barcode)
		{
			string insertString;
			string[] insertStringSplitted;
			string connectionString;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			getDatabaseInsertStringFromExcel(ref insertString, c_productslitlistTableName);
			insertStringSplitted = insertString.Split(',@');

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + ";" + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

				myCommand.CommandText = "update ";
				myCommand.CommandText += "`" + c_productslitlistTableName + "` ";
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
                Console.WriteLine(c_dbName + ":" + c_productslitlistTableName + ": update product barcode failed! " + ex);
            }
            return -1;
		}

#if 0
		public productprintlist_t[] readrecordBy(string materialScancode)
		{
			string commandText;
			string[] recordArray;
			productprintlist_t[] st_productprint, result;
			
			recordArray = mySQLClass.databaseCommonReadingUnsplitted(c_productprintlistTableName, commandText);
			if (recordArray == null)	return null;

			st_productprint = new productprintlist_t[recordArray.GetLength(0)];

			for (int i=0;i<recordArray.GetLength(0);i++){
				result = parseinput(recordArray[i]);
				if (result != null)	st_productprint[i] = result.Value;
			}
			return st_productprint;
		}
#endif
        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(productslitlist_t st_slit)
        {
            int num;
            int index;
            string[] itemName;
			string insertString;
			string connectionString;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_productslitlistTableName);

            try
            {
                index = 0;
                itemName = insertString.Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + c_productslitlistTableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], st_slit.machineID);
				myCommand.Parameters.AddWithValue(itemName[index++], st_slit.materialBarCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_slit.materialScanTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st_slit.productBarCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_slit.productScanTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st_slit.dispatchCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st_slit.batchNum);
                myCommand.Parameters.AddWithValue(itemName[index++], st_slit.largeIndex);
                myCommand.Parameters.AddWithValue(itemName[index++], st_slit.smallIndex);				
                myCommand.Parameters.AddWithValue(itemName[index++], st_slit.customerIndex);
                myCommand.Parameters.AddWithValue(itemName[index++], st_slit.errorStatus);
                myCommand.Parameters.AddWithValue(itemName[index++], st_slit.numOfJoins);
				myCommand.Parameters.AddWithValue(itemName[index++], st_slit.weight);

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(c_dbName + ":" + c_productslitlistTableName + ": write record failed! " + ex);
            }
            return -1;
        }

#if 0
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
#endif
	}
}

