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
		private const int BATCH_NUM_INDEX = 5;
		private const int LARGE_INDEX_INDEX = 6;
		private const int WEIGHT_INDEX = 7;
		private const int ERROR_STATUS_INDEX = 8;
		//2016.06.07 新增字段ID1，ID2，ID3， ID4，班次，班别，productCode, customer
		private const int OPERATOR_NAME_INDEX = 9;
		private const int OPERATOR_NAME2_INDEX = 10;
		private const int OPERATOR_NAME3_INDEX = 11;
		private const int OPERATOR_NAME4_INDEX = 12;
		private const int WORKSHIFT_INDEX = 13;
		private const int WORKTEAM_INDEX = 14;
		private const int PRODUCT_CODE_INDEX = 15;
		private const int CUSTOMER_INDEX = 16;
		private const int TOTAL_DATAGRAM_NUM = CUSTOMER_INDEX;

		private const string c_dbName = "globaldatabase";
        private const string c_TableName = "productcastinglist";
        private const string c_FileName = "..\\..\\data\\globalTables\\productCastList.xlsx";

		public struct productcast_t{
			public string machineID;
			public string barCode;
			public string scanTime;
			public string dispatchCode;
			public string batchNum;
			public string largeIndex;
			public float? weight;
			public string errorStatus;
			//2016.06.07 新增字段ID1，ID2，ID3， ID4，班次，班别，productCode, customer
			public string operatorName; //操作员
            public string operatorName2;//操作员
            public string operatorName3;//操作员
			public string operatorName4;//操作员
			public string workshift;	//班次（早中晚班）
			public string workTeam;		//班别(甲乙丙)
			public string productCode;	//产品编码
			public string customer;		//客户
		}

		public string Serialize(productcast_t st)
		{
			string str = null;

			str += st.machineID	+ ";" + st.barCode 		+ ";" + st.scanTime	+ ";" + st.dispatchCode + ";";
			str += st.batchNum	+ ";" + st.largeIndex	+ ";" + st.weight	+ ";" + st.errorStatus;
			//2016.06.07 新增字段ID1，ID2，ID3， ID4，班次，班别，productCode, customer
			str += ";" + st.operatorName + ";" + st.operatorName2 + ";" + st.operatorName3 + ";" + st.operatorName4;
			str += ";" + st.workshift 	 + ";" + st.workTeam 	  + ";" + st.productCode   + ";" + st.customer;
			return str;
		}

		public string[] Format(productcast_t st)
		{
			string str;
			string[] strArray;

			str = Serialize(st);
			strArray = str.Split(';');
			return strArray;
		}
		
		public productcast_t? Deserialize(string strInput)
		{
			string[] input;
			productcast_t st = new productcast_t();

			input = strInput.Split(';');

			if (input.Length < TOTAL_DATAGRAM_NUM-1)
				return null;

			st.barCode = input[BARCODE_INDEX];
			st.batchNum = input[BATCH_NUM_INDEX];
			st.dispatchCode = input[DISPATCH_CODE_INDEX];
			st.largeIndex = input[LARGE_INDEX_INDEX];
			st.machineID = input[MACHINE_ID_INDEX];
			st.scanTime = input[SCAN_TIME_INDEX];
			if (input[WEIGHT_INDEX]!="")	st.weight = Convert.ToSingle(input[WEIGHT_INDEX]);
			st.errorStatus = input[ERROR_STATUS_INDEX];
			//2016.06.07 新增字段ID1，ID2，ID3， ID4，班次，班别，productCode, customer
			st.operatorName  = input[OPERATOR_NAME_INDEX];
			st.operatorName2 = input[OPERATOR_NAME2_INDEX];
			st.operatorName3 = input[OPERATOR_NAME3_INDEX];
			st.operatorName4 = input[OPERATOR_NAME4_INDEX];
			st.workshift = input[WORKSHIFT_INDEX];
			st.workTeam = input[WORKTEAM_INDEX];
			st.productCode = input[PRODUCT_CODE_INDEX];
			st.customer = input[CUSTOMER_INDEX];
			return st;
		}

        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(productcast_t st)
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

                /*myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.machineID);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.barCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.scanTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.dispatchCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.batchNum);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.largeIndex);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.weight);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.errorStatus);*/

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
	}
}

