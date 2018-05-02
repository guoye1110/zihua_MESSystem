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
	public class globaldatabase.productprintlist : mySQLClass
	{
		//index
        private const int MACHINE_ID_INDEX = 1;
		private const int MATERIAL_BARCODE_INDEX = 2;
		private const int MATERIAL_SCAN_TIME_INDEX = 3;
		private const int PRODUCT_BARCODE_INDEX = 4;
		private const int PRODUCT_SCAN_TIME_INDEX = 5;
		private const int DISPATCH_CODE_INDEX = 6;
		private const int SALES_ORDER_CODE_INDEX = 7;
		private const int BATCH_NUM_INDEX = 8;
		private const int LARGE_INDEX_INDEX = 9;
		private const int WORKER_ID_INDEX = 10;
		private const int TOTAL_DATAGRAM_NUM = WORKER_ID_INDEX+1;

		private const string c_dbName = "globaldatabase";
        private const string c_productprintlistTableName = "productprintlist";
        private const string c_productprintlistFileName = "..\\..\\data\\globalTables\\productPrintList.xlsx";

		public struct productprintlist_t{
			string machineID;
			string materialBarCode;
			string materialScanTime;
			string productBarCode;
			string productScanTime;
			string dispatchCode;
			string salesOrderCode;
			string batchNum;
			string largeIndex;
			string workerID;
		}

		public productprintlist_t? parseinput(string strInput)
		{
			string[] input;
			productprintlist_t st_productprint;

			input = strInput.Split(';');

			if (input.Length < TOTAL_DATAGRAM_NUM)
				return null;

			st_productprint.batchNum = input[BATCH_NUM_INDEX];
			st_productprint.dispatchCode = input[DISPATCH_CODE_INDEX];
			st_productprint.largeIndex = input[LARGE_INDEX_INDEX];
			st_productprint.machineID = input[MACHINE_ID_INDEX];
			st_productprint.materialBarCode = input[MATERIAL_BARCODE_INDEX];
			st_productprint.materialScanTime = input[MATERIAL_SCAN_TIME_INDEX];
			st_productprint.productBarCode = input[PRODUCT_BARCODE_INDEX];
			st_productprint.productScanTime = input[PRODUCT_SCAN_TIME_INDEX];
			st_productprint.salesOrderCode = input[SALES_ORDER_CODE_INDEX];
			st_productprint.workerID = input[WORKER_ID_INDEX];
			
			return st_productprint;
		}

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

        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(productprintlist_t st_productprint)
        {
            int num;
            int index;
            string[] itemName;
			string insertString;
			string connectionString;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_productprintlistFileName);

            try
            {
                index = 0;
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
                myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.salesOrderCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.batchNum);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.largeIndex);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productprint.workerID);

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
	}
}

