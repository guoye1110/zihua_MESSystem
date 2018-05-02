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
	public class productcastinglistDB : mySQLClass
	{
		//index
        private const int MACHINE_ID_INDEX = 1;
		private const int BARCODE_INDEX = 2;
		private const int SCAN_TIME_INDEX= 3;
		private const int DISPATCH_CODE_INDEX = 4;
		private const int SALES_ORDER_CODE_INDEX = 5;
		private const int BATCH_NUM_INDEX = 6;
		private const int LARGE_INDEX_INDEX = 7;
		private const int WORKER_ID_INDEX = 8;
		private const int PRODUCT_CODE_INDEX = 9;
		private const int TOTAL_DATAGRAM_NUM = PRODUCT_CODE_INDEX+1;

		private const string c_dbName = "globaldatabase";
        private const string c_productcastinglistTableName = "productcastinglist";
        private const string c_productcastinglistFileName = "..\\..\\data\\globalTables\\productCastList.xlsx";

		public struct productcastinglist_t{
			string machineID;
			string barCode;
			string scanTime;
			string dispatchCode;
			string salesOrderCode;
			string batchNum;
			string largeIndex;
			string workerID;
			string productCode;
		}

		public productcastinglist_t? parseinput(string strInput)
		{
			string[] input;
			productcastinglist_t st_productcasting;

			input = strInput.Split(';');

			if (input.Length < LEFT_IN_FEEDBIN_INDEX)
				return null;

			st_productcasting.barCode = input[BARCODE_INDEX];
			st_productcasting.batchNum = input[BATCH_NUM_INDEX];
			st_productcasting.dispatchCode = input[DISPATCH_CODE_INDEX];
			st_productcasting.largeIndex = input[LARGE_INDEX_INDEX];
			st_productcasting.machineID = input[MACHINE_ID_INDEX];
			st_productcasting.productCode = input[PRODUCT_CODE_INDEX];
			st_productcasting.salesOrderCode = input[SALES_ORDER_CODE_INDEX];
			st_productcasting.scanTime = input[SCAN_TIME_INDEX];
			st_productcasting.workerID = input[WORKER_ID_INDEX];
			
			return st_productcasting;
		}

        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(productcastinglist_t st_productcasting)
        {
            int num;
            int index;
            string[] itemName;
			string insertString;
			string connectionString;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_productcastinglistFileName);

            try
            {
                index = 0;
                itemName = insertString.Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + c_productcastinglistTableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.machineID);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.barCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.scanTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.dispatchCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.salesOrderCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.batchNum);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.largeIndex);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.workerID);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.productCode);

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(c_dbName + "write to " + c_productcastinglistTableName + " failed!" + ex);
            }
            return -1;
        }
	}
}

