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
	public class materiallistDB : mySQLClass
	{
		//index
        private const int SALES_ORDER_CODE_INDEX = 1;
		private const int DISPATCH_CODE_INDEX = 2;
		private const int MACHINE_ID_INDEX = 3;
		private const int MACHINE_NAME_INDEX = 4;
		private const int NUM_OF_TYPES_INDEX = 5;
		private const int MATERIAL_CODE1_INDEX = 6;
		private const int MATERIAL_REQUIRED1_INDEX = 8;
		private const int MATERIAL_CODE2_INDEX = 9;
		private const int MATERIAL_REQUIRED2_INDEX = 10;
		private const int MATERIAL_CODE3_INDEX = 11;
		private const int MATERIAL_REQUIRED3_INDEX = 12;
		private const int MATERIAL_CODE4_INDEX = 13;
		private const int MATERIAL_REQUIRED4_INDEX = 14;
		private const int MATERIAL_CODE5_INDEX = 15;
		private const int MATERIAL_REQUIRED5_INDEX = 16;
		private const int MATERIAL_CODE6_INDEX = 17;
		private const int MATERIAL_REQUIRED6_INDEX = 18;
		private const int MATERIAL_CODE7_INDEX = 19;
		private const int MATERIAL_REQUIRED7_INDEX = 20;
		private const int TOTAL_DATAGRAM_NUM = MATERIAL_REQUIRED7_INDEX+1;

		private const string m_dbName = null;
        private const string c_TableName = "0_materiallist";
        private const string c_FileName = "..\\..\\data\\machine\\materialList.xlsx";

		public struct material_t{
			public string salesOrderCode;
            public string dispatchCode;
            public string machineID;
            public string machineName;
            public string numOfTypes;
            public string materialCode1;
            public string materialRequired1;
            public string materialCode2;
            public string materialRequired2;
            public string materialCode3;
            public string materialRequired3;
            public string materialCode4;
            public string materialRequired4;
            public string materialCode5;
            public string materialRequired5;
            public string materialCode6;
            public string materialRequired6;
            public string materialCode7;
            public string materialRequired7;
		};

		public materiallistDB(int index)
       	{
       		m_dbName = gVariable.DBHeadString + index.ToString().PadLeft(3, '0');
		}

		public material_t? Deserialize(string strInput)
		{
			string[] input;
			material_t st;

			input = strInput.Split(';');

			if (input.Length < LEFT_IN_FEEDBIN_INDEX)
				return null;

			st.salesOrderCode = input[SALES_ORDER_CODE_INDEX];
			st.dispatchCode = input[DISPATCH_CODE_INDEX];
			st.machineID = input[MACHINE_ID_INDEX];
			st.machineName = input[MACHINE_NAME_INDEX];
			st.materialCode1 = input[MATERIAL_CODE1_INDEX];
			st.materialRequired1 = input[MATERIAL_REQUIRED1_INDEX];
			st.materialCode2 = input[MATERIAL_CODE2_INDEX];
			st.materialRequired2 = input[MATERIAL_REQUIRED2_INDEX];
			st.materialCode3 = input[MATERIAL_CODE3_INDEX];
			st.materialRequired3 = input[MATERIAL_REQUIRED3_INDEX];
			st.materialCode4 = input[MATERIAL_CODE4_INDEX];
			st.materialRequired4 = input[MATERIAL_REQUIRED4_INDEX];
			st.materialCode5 = input[MATERIAL_CODE5_INDEX];
			st.materialRequired5 = input[MATERIAL_REQUIRED5_INDEX];
			st.materialCode6 = input[MATERIAL_CODE6_INDEX];
			st.materialRequired6 = input[MATERIAL_REQUIRED6_INDEX];
			st.materialCode7 = input[MATERIAL_CODE7_INDEX];
			st.materialRequired7 = input[MATERIAL_REQUIRED7_INDEX];

			return st;
		}

        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(material_t st)
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
                myCommand.Parameters.AddWithValue(itemName[index++], st.salesOrderCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st.dispatchCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st.machineID);
				myCommand.Parameters.AddWithValue(itemName[index++], st.machineName);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode1);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialRequired1);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode2);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialRequired2);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode3);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialRequired3);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode4);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialRequired4);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode5);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialRequired5);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode6);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialRequired6);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode7);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialRequired7);

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

		public material_t? readrecord_byDispatchCode(string dispatchCode)
        {
			material_t? dd;
			string[] recordArray;
			string insertString,insertStringSplitted;

			getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
			insertStringSplitted = insertString.Split(new char[2]{',','@'});

			string commandText = "select * from `" + c_TableName + "`";
			commandText += "where `" + insertStringSplitted[DISPATCH_CODE_INDEX] + "`=" + dispatchCode;
			
			recordArray = databaseCommonReadingUnsplitted(m_dbName, commandText);
			if (recordArray!=null){
				dd = Deserialize(recordArray[0]);
			}
			return dd;
		}
	}
}

