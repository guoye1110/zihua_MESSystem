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
        private const int PLATE_NO_INDEX = 14;
		//2018.06.07 新增字段ID1，ID2, 班次，班别，productCode,  customer,  theoryWeight，realLength, threoryLength
		private const int OPERATOR_NAME_INDEX = 15;
		private const int OPERATOR_NAME2_INDEX = 16;
		private const int WORKSHIFT_INDEX = 17;
		private const int WORKTEAM_INDEX = 18;
		private const int PRODUCT_CODE_INDEX = 19;
		private const int CUSTOMER_INDEX = 20;
		private const int THEORY_WEIGHT_INDEX = 21;
		private const int REAL_LENGTH_INDEX = 22;
		private const int THEORY_LENGTH_INDEX = 23;
        private const int TOTAL_DATAGRAM_NUM = THEORY_LENGTH_INDEX;

		private const string c_dbName = "globaldatabase";
        private const string c_TableName = "productslittinglist";
        private const string c_FileName = "..\\..\\data\\globalTables\\productSlitList.xlsx";

		public struct productslit_t{
			public string machineID;
            public string materialBarCode;
            public string materialScanTime;
            public string productBarCode;
            public string productScanTime;
            public string dispatchCode;
            public string batchNum;
            public string largeIndex;
            public string smallIndex;
            public string customerIndex;
            public string errorStatus;
            public string numOfJoins;
            public float? weight;
            public string plateNo;
			//2018.06.07 新增字段ID1，ID2, 班次，班别，productCode,  customer,  theoryWeight，realLength, threoryLength
			public string operatorName; //操作�?
            public string operatorName2;//操作�?
			public string workshift;	//班次（早中晚班）
			public string workTeam;		//班别(甲乙�?
			public string productCode;	//产品编码
			public string customer;		//客户
			public float? theoryWeigth;
			public float? realLength;
			public float? theoryLength;
        }

		public string Serialize(productslit_t st)
		{
			string str = null;

			str += st.machineID  		+ ";" + st.materialBarCode 	+ ";" + st.materialScanTime	+ ";" + st.productBarCode	+ ";";
			str += st.productScanTime	+ ";" + st.dispatchCode		+ ";" + st.batchNum 		+ ";" + st.largeIndex		+ ";";
			str += st.smallIndex		+ ";" + st.customerIndex	+ ";" + st.errorStatus		+ ";" + st.numOfJoins		+ ";";
			str += st.weight            + ";" + st.plateNo;
			//2018.06.07 新增字段ID1，ID2, 班次，班别，productCode,  customer,  theoryWeight，realLength, threoryLength
			str += ";" + st.operatorName + ";" + st.operatorName2 + ";" + st.workshift 	  + ";" + st.workTeam;
			str += ";" + st.productCode  + ";" + st.customer 	  + ";" + st.theoryWeigth + ";" + st.realLength + ";" + st.theoryLength;
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
			productslit_t st = new productslit_t();

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
			if (input[WEIGHT_INDEX]!="")	st.weight = Convert.ToSingle(input[WEIGHT_INDEX]);
            st.plateNo = input[PLATE_NO_INDEX];
			//2018.06.07 新增字段ID1，ID2, 班次，班别，productCode,  customer,  theoryWeight，realLength, threoryLength
			st.operatorName = input[OPERATOR_NAME_INDEX];
			st.operatorName2 = input[OPERATOR_NAME2_INDEX];
			st.workshift = input[WORKSHIFT_INDEX];
			st.workTeam = input[WORKTEAM_INDEX];
			st.productCode = input[PRODUCT_CODE_INDEX];
			st.customer = input[CUSTOMER_INDEX];
			if (input[THEORY_WEIGHT_INDEX]!="")	st.theoryWeigth = Convert.ToSingle(input[THEORY_WEIGHT_INDEX]);
			if (input[REAL_LENGTH_INDEX]!="") st.realLength = Convert.ToSingle(input[REAL_LENGTH_INDEX]);
			if (input[THEORY_LENGTH_INDEX]!="")	st.theoryLength = Convert.ToSingle(input[THEORY_LENGTH_INDEX]);

			return st;
		}

		public int updaterecord_ByMaterialBarCode(productslit_t st, string barcode)
		{
			string insertString = null;
			string[] insertStringSplitted;
			string connectionString;
			string[] inputArray;
			string[] stringSeparators = new string[] { ",@" };

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
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

				bool first = true;
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

		public productslit_t? readlastrecord_ByMachineID(int machineId)
		{
			string commandText;
			string[] recordArray;
			string insertString=null;
			productslit_t? result;
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
        public int writerecord(productslit_t st)
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
				for(index=1;index<=TOTAL_DATAGRAM_NUM;index++)
					myCommand.Parameters.AddWithValue(itemName[index], inputArray[index-1]);
				
                /*myCommand.Parameters.AddWithValue(itemName[index++], st_slit.machineID);
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
				myCommand.Parameters.AddWithValue(itemName[index++], st_slit.weight.ToString());*/

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

