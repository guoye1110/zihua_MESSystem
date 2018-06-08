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
	public class feedbininventoryDB : mySQLClass
	{
		//index
		private const int DISPATCH_CODE_INDEX = 1;
		private const int DISPATCH_STATUS_INDEX = 2;
		private const int STATUS_CHANGE_TIME_INDEX = 3;
		private const int BIN_LEFT_1_PREVIOUS = 4;
		private const int QUANTITY_USED_1_CURRENT = 5;
		private const int BIN_LEFT_2_PREVIOUS = 6;
		private const int QUANTITY_USED_2_CURRENT = 7;
		private const int BIN_LEFT_3_PREVIOUS = 8;
		private const int QUANTITY_USED_3_CURRENT = 9;
		private const int BIN_LEFT_4_PREVIOUS = 10;
		private const int QUANTITY_USED_4_CURRENT = 11;
		private const int BIN_LEFT_5_PREVIOUS = 12;
		private const int QUANTITY_USED_5_CURRENT = 13;
		private const int BIN_LEFT_6_PREVIOUS = 14;
		private const int QUANTITY_USED_6_CURRENT = 15;
		private const int BIN_LEFT_7_PREVIOUS = 16;
		private const int QUANTITY_USED_7_CURRENT = 17;
		private const int TOTAL_DATAGRAM_NUM = QUANTITY_USED_7_CURRENT;

		private const string m_dbName = null;
        private const string c_TableName = "0_feedbininventory";
        private const string c_FileName = "..\\..\\data\\machine\\feedBinInventory.xlsx";

		public struct feedbin_t{
            public string dispatchCode;
            public string dispatchStatus;
            public string statusChangeTime;
			
            public float? binLeft1;
			public float? quantityUsed1;
            public float? binLeft2;
			public float? quantityUsed2;
            public float? binLeft3;
			public float? quantityUsed3;
            public float? binLeft4;
			public float? quantityUsed4;
            public float? binLeft5;
			public float? quantityUsed5;
            public float? binLeft6;
			public float? quantityUsed6;
            public float? binLeft7;
			public float? quantityUsed7;
		};

		public feedbininventoryDB(int index)
       	{
       		m_dbName = gVariable.DBHeadString + index.ToString().PadLeft(3, '0');
		}

		public string Serialize(feedbin_t st)
		{
			string str = null;

			str += st.dispatchCode	+ ";" + st.dispatchStatus 	+ ";" + st.statusChangeTime		+ ";";
			str += st.binLeft1  	+ ";" + st.quantityUsed1 	+ ";" + st.binLeft2 	+ ";" + st.quantityUsed2	+ ";";
			str += st.binLeft3  	+ ";" + st.quantityUsed3 	+ ";" + st.binLeft4 	+ ";" + st.quantityUsed4	+ ";";
			str += st.binLeft5  	+ ";" + st.quantityUsed5 	+ ";" + st.binLeft6 	+ ";" + st.quantityUsed6	+ ";";
			str += st.binLeft7  	+ ";" + st.quantityUsed7;
			return str;
		}

		public string[] Format(feedbin_t st)
		{
			string str;
			string[] strArray;

			str = Serialize(st);
			strArray = str.Split(';');
			return strArray;
		}

		public feedbin_t? Deserialize(string strInput)
		{
			string[] input;
			feedbin_t st = new feedbin_t();

			input = strInput.Split(';');

			if (input.Length < TOTAL_DATAGRAM_NUM)
				return null;

			st.dispatchCode = input[DISPATCH_CODE_INDEX];
			st.dispatchStatus = input[DISPATCH_STATUS_INDEX];
			st.statusChangeTime = input[STATUS_CHANGE_TIME_INDEX];
			
			if (input[BIN_LEFT_1_PREVIOUS]!="")		st.binLeft1 = Convert.ToSingle(input[BIN_LEFT_1_PREVIOUS]);
			if (input[QUANTITY_USED_1_CURRENT]!="")	st.quantityUsed1 = Convert.ToSingle(input[QUANTITY_USED_1_CURRENT]);
			if (input[BIN_LEFT_2_PREVIOUS]!="")		st.binLeft2 = Convert.ToSingle(input[BIN_LEFT_2_PREVIOUS]);
			if (input[QUANTITY_USED_2_CURRENT]!="")	st.quantityUsed2 = Convert.ToSingle(input[QUANTITY_USED_2_CURRENT]);
			if (input[BIN_LEFT_3_PREVIOUS]!="")		st.binLeft3 = Convert.ToSingle(input[BIN_LEFT_3_PREVIOUS]);
			if (input[QUANTITY_USED_3_CURRENT]!="")	st.quantityUsed3 = Convert.ToSingle(input[QUANTITY_USED_3_CURRENT]);
			if (input[BIN_LEFT_4_PREVIOUS]!="")		st.binLeft4 = Convert.ToSingle(input[BIN_LEFT_4_PREVIOUS]);
			if (input[QUANTITY_USED_4_CURRENT]!="")	st.quantityUsed4 = Convert.ToSingle(input[QUANTITY_USED_4_CURRENT]);
			if (input[BIN_LEFT_5_PREVIOUS]!="")		st.binLeft5 = Convert.ToSingle(input[BIN_LEFT_5_PREVIOUS]);
			if (input[QUANTITY_USED_5_CURRENT]!="")	st.quantityUsed5 = Convert.ToSingle(input[QUANTITY_USED_5_CURRENT]);
			if (input[BIN_LEFT_6_PREVIOUS]!="")		st.binLeft6 = Convert.ToSingle(input[BIN_LEFT_6_PREVIOUS]);
			if (input[QUANTITY_USED_6_CURRENT]!="")	st.quantityUsed6 = Convert.ToSingle(input[QUANTITY_USED_6_CURRENT]);
			if (input[BIN_LEFT_7_PREVIOUS]!="")		st.binLeft7 = Convert.ToSingle(input[BIN_LEFT_7_PREVIOUS]);
			if (input[QUANTITY_USED_7_CURRENT]!="")	st.quantityUsed7 = Convert.ToSingle(input[QUANTITY_USED_7_CURRENT]);

			return st;
		}

        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(feedbin_t st)
        {
            int index;
            string[] itemName;
			string insertString=null;
			string connectionString;
			string[] inputArray;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			getDatabaseInsertStringFromExcel(ref insertString, c_FileName);

			inputArray = Format(st);

            try
            {
                index = 1;
                itemName = insertString.Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + m_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + c_TableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
				for (index=1;index<=TOTAL_DATAGRAM_NUM;index++)
					myCommand.Parameters.AddWithValue(itemName[index], inputArray[index-1]);

                /*myCommand.Parameters.AddWithValue(itemName[index++], st.dispatchCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st.dispatchStatus);
                myCommand.Parameters.AddWithValue(itemName[index++], st.statusChangeTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st.binLeft1.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.quantityUsed1.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.binLeft2.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.quantityUsed2.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.binLeft3.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.quantityUsed3.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.binLeft4.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.quantityUsed4.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.binLeft5.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.quantityUsed5.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.binLeft6.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.quantityUsed6.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.binLeft7.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.quantityUsed7.ToString());*/

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(m_dbName + "write to " + c_TableName + " failed!" + ex);
            }
            return -1;
        }

		public feedbin_t[] readrecord_byDispatchCode(string dispatchCode)
        {
			feedbin_t? dd;
			string[] recordArray;
			feedbin_t[] st_feedbin=null;
			string insertString;
			string[] insertStringSplitted;
			string[] stringSeparators = new string[] { ",@" };

			getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
			insertStringSplitted = insertString.Split(stringSeparators, StringSplitOptions.None);

			string commandText = "select * from `" + c_TableName + "`";
			commandText += "where `" + insertStringSplitted[DISPATCH_CODE_INDEX] + "`=" + "\'" + dispatchCode + "\'";
			
			recordArray = databaseCommonReadingUnsplitted(m_dbName, commandText);
			if (recordArray!=null){
				st_feedbin = new feedbin_t[recordArray.Length];
				for (int i=0;i<recordArray.Length;i++){
					dd = Deserialize(recordArray[i]);
					st_feedbin[i] = dd.Value;
				}
			}
			return st_feedbin;
		}
	}
}

