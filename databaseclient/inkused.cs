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
	public class inkusedDB : mySQLClass
	{
        //index
        private const int MACHINE_ID_INDEX = 1;
		private const int DISPATCH_CODE_INDEX = 2;
        private const int BATCH_NUM_INDEX = 3;
		private const int PRODUCT_CODE_INDEX = 4;
		private const int CUSTOMER_INDEX = 5;
		private const int OPERATOR_NAME_INDEX = 6;
		private const int NUMBER1_INDEX = 7;
		private const int NUMBER2_INDEX = 8;
		private const int NUMBER3_INDEX = 9;
		private const int NUMBER4_INDEX = 10;
		private const int NUMBER5_INDEX = 11;
		private const int NUMBER6_INDEX = 12;
		private const int NUMBER7_INDEX = 13;
		private const int NUMBER8_INDEX = 14;
		private const int NUMBER9_INDEX = 15;
		private const int NUMBER10_INDEX = 16;
		private const int NUMBER11_INDEX = 17;
		private const int NUMBER12_INDEX = 18;
		private const int NUMBER13_INDEX = 19;
		private const int NUMBER14_INDEX = 20;
        private const int TOTAL_DATAGRAM_NUM = NUMBER14_INDEX;

		private const string c_dbName = "globaldatabase";
        private const string c_TableName = "inkUsed";
        private const string c_FileName = "..\\..\\data\\globalTables\\inkUsed.xlsx";

		public struct inkused_t{
			public string machineID;
            public string dispatchCode;
			public string batchNum;
			public string productCode;
			public string customer;
			public string operatorName; //操作员
			public float? number1;
			public float? number2;
			public float? number3;
			public float? number4;
			public float? number5;
			public float? number6;
			public float? number7;
			public float? number8;
			public float? number9;
			public float? number10;
			public float? number11;
			public float? number12;
			public float? number13;
			public float? number14;
		}

		public string Serialize(inkused_t st)
		{
			string str = null;

			str += st.machineID	+ ";" + st.dispatchCode	+ ";" + st.batchNum + ";" + st.productCode	+ ";";
			str += st.customer	+ ";" + st.operatorName	+ ";" + st.number1 	+ ";" + st.number2		+ ";";
            str += st.number3	+ ";" + st.number4		+ ";" + st.number5	+ ";" + st.number6		+ ";";
            str += st.number7	+ ";" + st.number8		+ ";" + st.number9	+ ";" + st.number10		+ ";";
            str += st.number11	+ ";" + st.number12		+ ";" + st.number13	+ ";" + st.number14		+ ";";

			return str;
		}

		public string[] Format(inkused_t st)
		{
			string str;
			string[] strArray;

			str = Serialize(st);
			strArray = str.Split(';');
			return strArray;
		}

		public inkused_t ? Deserialize(string strInput)
		{
			string[] input;
			inkused_t st = new inkused_t();

			input = strInput.Split(';');

			if (input.Length < TOTAL_DATAGRAM_NUM)
				return null;

			st.machineID = input[MACHINE_ID_INDEX];
			st.dispatchCode = input[DISPATCH_CODE_INDEX];
			st.batchNum = input[BATCH_NUM_INDEX];
			st.productCode = input[PRODUCT_CODE_INDEX];
			st.customer = input[CUSTOMER_INDEX];
			st.operatorName = input[OPERATOR_NAME_INDEX];
			if (input[NUMBER1_INDEX]!="")	st.number1 = Convert.ToSingle(input[NUMBER1_INDEX]);
			if (input[NUMBER2_INDEX]!="")	st.number2 = Convert.ToSingle(input[NUMBER2_INDEX]);
			if (input[NUMBER3_INDEX]!="")	st.number3 = Convert.ToSingle(input[NUMBER3_INDEX]);
			if (input[NUMBER4_INDEX]!="")	st.number4 = Convert.ToSingle(input[NUMBER4_INDEX]);
			if (input[NUMBER5_INDEX]!="")	st.number5 = Convert.ToSingle(input[NUMBER5_INDEX]);
			if (input[NUMBER6_INDEX]!="")	st.number6 = Convert.ToSingle(input[NUMBER6_INDEX]);
			if (input[NUMBER7_INDEX]!="")	st.number7 = Convert.ToSingle(input[NUMBER7_INDEX]);
			if (input[NUMBER8_INDEX]!="")	st.number8 = Convert.ToSingle(input[NUMBER8_INDEX]);
			if (input[NUMBER9_INDEX]!="")	st.number9 = Convert.ToSingle(input[NUMBER9_INDEX]);
			if (input[NUMBER10_INDEX]!="")	st.number10 = Convert.ToSingle(input[NUMBER10_INDEX]);
			if (input[NUMBER11_INDEX]!="")	st.number11 = Convert.ToSingle(input[NUMBER11_INDEX]);
			if (input[NUMBER12_INDEX]!="")	st.number12 = Convert.ToSingle(input[NUMBER12_INDEX]);
			if (input[NUMBER13_INDEX]!="")	st.number13 = Convert.ToSingle(input[NUMBER13_INDEX]);
			if (input[NUMBER14_INDEX]!="")	st.number14 = Convert.ToSingle(input[NUMBER14_INDEX]);

			return st;
		}

        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(inkused_t st)
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
	}
}
