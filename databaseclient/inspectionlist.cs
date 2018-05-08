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
	public class inspectionlistDB : mySQLClass
	{
		//index
        private const int MATERIAL_SCANTIME_INDEX = 1;
		private const int MATERIAL_BARCODE_INDEX = 2;
		private const int PRODUCT_SCANTIME_INDEX =3;
		private const int PRODUCT_BARCODE_INDEX = 4;
		private const int DISPATCH_CODE_INDEX = 5;
		private const int BATCH_NUM_INDEX = 6;
		private const int INSPECTOR_INDEX = 7;
		private const int CHECKING_RESULT_INDEX = 8;
		private const int TOTAL_DATAGRAM_NUM = CHECKING_RESULT_INDEX+1;

		private const string c_dbName = "globaldatabase";
        private const string c_inspectionlistTableName = "inspectionlist";
        private const string c_inspectionlistFileName = "..\\..\\data\\globalTables\\inspectionList.xlsx";

		public struct inspection_t{
			public string materialScanTime;
            public string materialBarCode;
            public string productScanTime;
            public string productBarCode;
            public string dispatchCode;
            public string batchNum;
            public string inspector;
            public string checkingResult;
			public inspection_t(int value){
				this.materialScanTime = null;
				this.materialBarCode = null;
				this.productScanTime = null;
				this.productBarCode = null;
				this.dispatchCode = null;
				this.batchNum = null;
				this.inspector = null;
				this.checkingResult = null;
			}
		}

		public string Serialize(inspection_t st)
		{
			string str = null;

			str += st.materialScanTime 	+ ";" + st.materialBarCode	+ ";" + st.productScanTime	+ ";" + st.productBarCode + ";";
			str += st.dispatchCode		+ ";" + st.batchNum			+ ";" + st.inspector		+ ";" + st.checkingResult;
			return str;
		}

		public string[] Format(inspection_t st)
		{
			string str;
			string[] strArray;

			str = Serialize(st);
			strArray = str.Split(';');
			return strArray;
		}
		
		public inspection_t? Deserialize(string strInput)
		{
			string[] input;
			inspection_t st;

			input = strInput.Split(';');

			if (input.Length < TOTAL_DATAGRAM_NUM)
				return null;

			st.materialScanTime = input[MATERIAL_SCANTIME_INDEX];
			st.materialBarCode = input[MATERIAL_BARCODE_INDEX];
			st.productScanTime = input[PRODUCT_SCANTIME_INDEX];
			st.productBarCode = input[PRODUCT_BARCODE_INDEX];
			st.dispatchCode = input[DISPATCH_CODE_INDEX];
			st.batchNum = input[BATCH_NUM_INDEX];
			st.inspector = input[INSPECTOR_INDEX];
			st.checkingResult = input[CHECKING_RESULT_INDEX];

			return st;
		}

		public int updaterecord_ByMaterialBarCode(string[] strArray, string barcode)
		{
			string insertString=null;
			string[] insertStringSplitted;
			string connectionString;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			getDatabaseInsertStringFromExcel(ref insertString, c_inspectionlistTableName);
			insertStringSplitted = insertString.Split(new char[2]{',','@'});

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + ";" + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

				myCommand.CommandText = "update ";
				myCommand.CommandText += "`" + c_inspectionlistTableName + "` ";
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
                Console.WriteLine(c_dbName + ":" + c_inspectionlistTableName + ": update product barcode failed! " + ex);
            }
            return -1;
		}


        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(inspection_t st)
        {
            int num;
            int index;
            string[] itemName;
			string insertString=null;
			string connectionString;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_inspectionlistFileName);

            try
            {
                index = 0;
                itemName = insertString.Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + c_inspectionlistTableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], st.materialScanTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialBarCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st.productScanTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st.productBarCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st.dispatchCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st.batchNum);
				myCommand.Parameters.AddWithValue(itemName[index++], st.inspector);
                myCommand.Parameters.AddWithValue(itemName[index++], st.checkingResult);

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(c_dbName + "write to " + c_inspectionlistTableName + " failed!" + ex);
            }
            return -1;
        }
	}
}
