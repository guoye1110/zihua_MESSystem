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
		private const int BIN_LEFT_1_PREVIOUS = 6;
		private const int QUANTITY_USED_1_CURRENT = 7;
		private const int BIN_LEFT_1_PREVIOUS = 8;
		private const int QUANTITY_USED_1_CURRENT = 9;
		private const int BIN_LEFT_1_PREVIOUS = 10;
		private const int QUANTITY_USED_1_CURRENT = 11;
		private const int BIN_LEFT_1_PREVIOUS = 12;
		private const int QUANTITY_USED_1_CURRENT = 13;
		private const int BIN_LEFT_1_PREVIOUS = 14;
		private const int QUANTITY_USED_1_CURRENT = 15;
		private const int BIN_LEFT_1_PREVIOUS = 16;
		private const int QUANTITY_USED_1_CURRENT = 17;

		private const string m_dbName = null;
        private const string c_TableName = "0_feedbininventory";
        private const string c_FileName = "..\\..\\data\\machine\\feedBinInventory.xlsx";

		public struct feedbin_t{
            public string dispatchCode;
            public string dispatchStatus;
            public string statusChangeTime;
			
            public string binLeft1;
			public string quantityUsed1;
            public string binLeft2;
			public string quantityUsed2;
            public string binLeft3;
			public string quantityUsed3;
            public string binLeft4;
			public string quantityUsed4;
            public string binLeft5;
			public string quantityUsed5;
            public string binLeft6;
			public string quantityUsed6;
            public string binLeft7;
			public string quantityUsed7;
		};

		public feedbininventoryDB(int index)
       	{
       		m_dbName = gVariable.DBHeadString + index.ToString().PadLeft(3, '0');
		}

		public feedbin_t? Deserialize(string strInput)
		{
			string[] input;
			feedbin_t st;

			input = strInput.Split(';');

			if (input.Length < LEFT_IN_FEEDBIN_INDEX)
				return null;

			st.dispatchCode;
			st.dispatchStatus;
			st.statusChangeTime;
			st.binLeft1;
			st.quantityUsed1;
			st.binLeft2;
			st.quantityUsed2;
			st.binLeft3;
			st.quantityUsed3;
			st.binLeft4;
			st.quantityUsed4;
			st.binLeft5;
			st.quantityUsed5;
			st.binLeft6;
			st.quantityUsed6;
			st.binLeft7;
			st.quantityUsed7;

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

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			getDatabaseInsertStringFromExcel(ref insertString, c_FileName);

            try
            {
                index = 1;
                itemName = insertString.Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + m_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + c_TableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], st.dispatchCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st.dispatchStatus);
                myCommand.Parameters.AddWithValue(itemName[index++], st.statusChangeTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st.binLeft1);
				myCommand.Parameters.AddWithValue(itemName[index++], st.quantityUsed1);
				myCommand.Parameters.AddWithValue(itemName[index++], st.binLeft2);
				myCommand.Parameters.AddWithValue(itemName[index++], st.quantityUsed2);
				myCommand.Parameters.AddWithValue(itemName[index++], st.binLeft3);
				myCommand.Parameters.AddWithValue(itemName[index++], st.quantityUsed3);
				myCommand.Parameters.AddWithValue(itemName[index++], st.binLeft4);
				myCommand.Parameters.AddWithValue(itemName[index++], st.quantityUsed4);
				myCommand.Parameters.AddWithValue(itemName[index++], st.binLeft5);
				myCommand.Parameters.AddWithValue(itemName[index++], st.quantityUsed5);
				myCommand.Parameters.AddWithValue(itemName[index++], st.binLeft6);
				myCommand.Parameters.AddWithValue(itemName[index++], st.quantityUsed6);
				myCommand.Parameters.AddWithValue(itemName[index++], st.binLeft7);
				myCommand.Parameters.AddWithValue(itemName[index++], st.quantityUsed7);

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
			string insertString,insertStringSplitted;

			getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
			insertStringSplitted = insertString.Split(new char[2]{',','@'});

			string commandText = "select * from `" + c_TableName + "`";
			commandText += "where `" + insertStringSplitted[DISPATCH_CODE_INDEX] + "`=" + dispatchCode;
			
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

