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
	public class apsrulesDB : mySQLClass
	{
        //index
        private const int SALES_ORDER_INDEX = 1;
		private const int ASSIGNED_MACHINEID_1_INDEX = 2;
        private const int ASSIGNED_MACHINEID_2_INDEX = 3;
		private const int ASSIGNED_MACHINEID_3_INDEX = 4;
		private const int ASSIGNED_STARTTIME_INDEX = 5;
		private const int ASSIGNED_ENDTIME_INDEX = 6;
		private const int IGNORE_ENDTIME_INDEX = 7;
        private const int TOTAL_DATAGRAM_NUM = IGNORE_ENDTIME_INDEX;

		private const string c_dbName = "globaldatabase";
        private const string c_TableName = "apsrules";
        private const string c_FileName = "..\\..\\data\\globalTables\\apsrules.xlsx";

		private static string m_DBinsertString;

		public struct apsrule_t{
			public string salesOrder;
            public int? assignedMachineID1;
			public int? assignedMachineID2;
			public int? assignedMachineID3;
			public string assignedStartTime;
			public string assignedEndTime;
			public string ignoreEndTime;
		}

		public apsrulesDB()
		{
			mySQLClass.getDatabaseInsertStringFromExcel(ref m_DBinsertString, c_FileName);
		}

		public string Serialize(apsrule_t st)
		{
			string str = null;

			str += st.salesOrder	+ ";" + st.assignedMachineID1	+ ";" + st.assignedMachineID2 + ";" + st.assignedMachineID3	+ ";";
			str += st.assignedStartTime	+ ";" + st.assignedEndTime	+ ";" + st.ignoreEndTime;

			return str;
		}

		public string[] Format(apsrule_t st)
		{
			string str;
			string[] strArray;

			str = Serialize(st);
			strArray = str.Split(';');
			return strArray;
		}

		public apsrule_t ? Deserialize(string strInput)
		{
			string[] input;
			apsrule_t st = new apsrule_t();

			input = strInput.Split(';');

			if (input.Length < TOTAL_DATAGRAM_NUM)
				return null;

			st.salesOrder = input[SALES_ORDER_INDEX];
			if (input[ASSIGNED_MACHINEID_1_INDEX]!="")	st.assignedMachineID1 = Convert.ToUInt16(input[ASSIGNED_MACHINEID_1_INDEX]);
			if (input[ASSIGNED_MACHINEID_2_INDEX]!="")	st.assignedMachineID2 = Convert.ToUInt16(input[ASSIGNED_MACHINEID_2_INDEX]);
			if (input[ASSIGNED_MACHINEID_3_INDEX]!="")	st.assignedMachineID3 = Convert.ToUInt16(input[ASSIGNED_MACHINEID_3_INDEX]);
			st.assignedStartTime = input[ASSIGNED_STARTTIME_INDEX];
			st.assignedEndTime = input[ASSIGNED_ENDTIME_INDEX];
			st.ignoreEndTime = input[IGNORE_ENDTIME_INDEX];

			return st;
		}

		public apsrule_t[] readallrecords()
		{
			apsrule_t? dd;
			string[] recordArray;
			string[] insertStringSplitted;
			apsrule_t[] stArray=null;
			string[] stringSeparators = new string[] { ",@" };
			string insertString=null;

			//getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
			insertString = m_DBinsertString;
			insertStringSplitted = insertString.Split(stringSeparators, StringSplitOptions.None);
			
			string commandText = "select * from `" + c_TableName + "` ";

			recordArray = mySQLClass.databaseCommonReadingUnsplitted(c_dbName, commandText);
			if (recordArray!=null){
				stArray = new apsrule_t[recordArray.Length];
				for (int i=0;i<recordArray.Length;i++){
					dd = Deserialize(recordArray[i]);
					stArray[i] = dd.Value;
				}
			}
			return stArray;
		}

		public apsrule_t[] readrecord_BySalesOrder(string SalesOrder)
		{
			apsrule_t? dd;
			string[] recordArray;
			string[] insertStringSplitted;
			apsrule_t[] stArray=null;
			string[] stringSeparators = new string[] { ",@" };
			string insertString=null;

			//getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
			insertString = m_DBinsertString;
			insertStringSplitted = insertString.Split(stringSeparators, StringSplitOptions.None);
			
			string commandText = "select * from `" + c_TableName + "` ";
			commandText += "where `" + insertStringSplitted[SALES_ORDER_INDEX] + "`=" + "\'" + SalesOrder + "\'";

			recordArray = mySQLClass.databaseCommonReadingUnsplitted(c_dbName, commandText);
			if (recordArray!=null){
				stArray = new apsrule_t[recordArray.Length];
				for (int i=0;i<recordArray.Length;i++){
					dd = Deserialize(recordArray[i]);
					stArray[i] = dd.Value;
				}
			}
			return stArray;
		}

        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(apsrule_t st)
        {
            int index;
            string[] itemName;
			string insertString=null;
			string connectionString;
			string[] inputArray;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			//mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
			insertString = m_DBinsertString;

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
		
		public int updaterecord_BySalesOrder(apsrule_t st, string SalesOrder)
		{
			string insertString=null;
			string[] insertStringSplitted;
			string connectionString;
			string[] inputArray;
            string[] stringSeparators = new string[] { ",@" };

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			//getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
			insertString = m_DBinsertString;
			//insertStringSplitted = insertString.Split(',@');
            //insertStringSplitted = insertString.Split(new char[2]{',','@'});
            insertStringSplitted = insertString.Split(stringSeparators, StringSplitOptions.None);

			inputArray = Format(st);

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

				myCommand.CommandText = "update ";
				myCommand.CommandText += "`" + c_TableName + "` ";
				myCommand.CommandText += "set ";

                bool first = true;
				for (int i=0;i<inputArray.Length;i++){
					if (inputArray[i] == null || inputArray[i] == "")	continue;
                    if (!first)
                        myCommand.CommandText += ",";
                    first = false;

					if (i==inputArray.Length-1)
						myCommand.CommandText += "`" + insertStringSplitted[i+1].Remove(insertStringSplitted[i+1].Length-1) + "`=" + "\'" + inputArray[i] + "\'";
					else
						myCommand.CommandText += "`" + insertStringSplitted[i+1] + "`=" + "\'" + inputArray[i] + "\'";					
				}

				myCommand.CommandText += " where `" + insertStringSplitted[SALES_ORDER_INDEX] + "`=" + "\'" + SalesOrder + "\'";

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(c_dbName + ":" + c_TableName + ": updaterecord failed! " + ex);
            }
            return -1;
		}

		public int deleterecord_BySalesOrder(string SalesOrder)
		{
			string insertString=null;
			string[] insertStringSplitted;
			string connectionString;
			string[] inputArray;
            string[] stringSeparators = new string[] { ",@" };

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			//getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
			insertString = m_DBinsertString;
			//insertStringSplitted = insertString.Split(',@');
            //insertStringSplitted = insertString.Split(new char[2]{',','@'});
            insertStringSplitted = insertString.Split(stringSeparators, StringSplitOptions.None);

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

				myCommand.CommandText = "delete from ";
				myCommand.CommandText += "`" + c_TableName + "` ";
				myCommand.CommandText += " where `" + insertStringSplitted[SALES_ORDER_INDEX] + "`=" + "\'" + SalesOrder + "\'";

				myCommand.ExecuteNonQuery();
				myConnection.Close();
				
				return 0;
			}
			catch (Exception ex)
			{
				Console.WriteLine(c_dbName + ":" + c_TableName + ": deleterecord failed! " + ex);
			}
			return -1;
		}
	}
}
