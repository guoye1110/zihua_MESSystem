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
        private const int MATERIAL_REQUIRED1_INDEX = 7;
        private const int MATERIAL_PREVIOUS_LEFT1_INDEX = 8;
        private const int MATERIAL_CURRENTLY_USED1_INDEX = 9;
        private const int MATERIAL_CURRENT_LEFT1_INDEX = 10;
        private const int MATERIAL_CODE2_INDEX = 11;
		private const int MATERIAL_REQUIRED2_INDEX = 12;
        private const int MATERIAL_PREVIOUS_LEFT2_INDEX = 13;
        private const int MATERIAL_CURRENTLY_USED2_INDEX = 14;
        private const int MATERIAL_CURRENT_LEFT2_INDEX = 15;
        private const int MATERIAL_CODE3_INDEX = 16;
		private const int MATERIAL_REQUIRED3_INDEX = 17;
        private const int MATERIAL_PREVIOUS_LEFT3_INDEX = 18;
        private const int MATERIAL_CURRENTLY_USED3_INDEX = 19;
        private const int MATERIAL_CURRENT_LEFT3_INDEX = 20;
        private const int MATERIAL_CODE4_INDEX = 21;
		private const int MATERIAL_REQUIRED4_INDEX = 22;
        private const int MATERIAL_PREVIOUS_LEFT4_INDEX = 23;
        private const int MATERIAL_CURRENTLY_USED4_INDEX = 24;
        private const int MATERIAL_CURRENT_LEFT4_INDEX = 25;
        private const int MATERIAL_CODE5_INDEX = 26;
		private const int MATERIAL_REQUIRED5_INDEX = 27;
        private const int MATERIAL_PREVIOUS_LEFT5_INDEX = 28;
        private const int MATERIAL_CURRENTLY_USED5_INDEX = 29;
        private const int MATERIAL_CURRENT_LEFT5_INDEX = 30;
        private const int MATERIAL_CODE6_INDEX = 31;
		private const int MATERIAL_REQUIRED6_INDEX = 32;
        private const int MATERIAL_PREVIOUS_LEFT6_INDEX = 33;
        private const int MATERIAL_CURRENTLY_USED6_INDEX = 34;
        private const int MATERIAL_CURRENT_LEFT6_INDEX = 35;
        private const int MATERIAL_CODE7_INDEX = 36;
        private const int MATERIAL_REQUIRED7_INDEX = 37;
        private const int MATERIAL_PREVIOUS_LEFT7_INDEX = 38;
        private const int MATERIAL_CURRENTLY_USED7_INDEX = 39;
        private const int MATERIAL_CURRENT_LEFT7_INDEX = 40;
        private const int SALESORDER_BATCHCODE_INDEX = 41;
        private const int BATCH_NUM_INDEX = 42;
        private const int MATERIAL_STATUS_INDEX = 43;
        private const int TOTAL_DATAGRAM_NUM = MATERIAL_STATUS_INDEX;

		private string m_dbName = null;
        private const string c_TableName = "0_materiallist";
        private const string c_FileName = "..\\..\\data\\machine\\materialList.xlsx";

		public struct material_t{
			public string salesOrderCode;
            public string dispatchCode;
            public string machineID;
            public string machineName;
            public string numOfTypes;
            public string materialCode1;
            public float? materialRequired1;
            public float? previousLeft1;
            public float? currentlyUsed1;
            public float? currentLeft1;
            public string materialCode2;
            public float? materialRequired2;
            public float? previousLeft2;
            public float? currentlyUsed2;
            public float? currentLeft2;
            public string materialCode3;
            public float? materialRequired3;
            public float? previousLeft3;
            public float? currentlyUsed3;
            public float? currentLeft3;
            public string materialCode4;
            public float? materialRequired4;
            public float? previousLeft4;
            public float? currentlyUsed4;
            public float? currentLeft4;
            public string materialCode5;
            public float? materialRequired5;
            public float? previousLeft5;
            public float? currentlyUsed5;
            public float? currentLeft5;
            public string materialCode6;
            public float? materialRequired6;
            public float? previousLeft6;
            public float? currentlyUsed6;
            public float? currentLeft6;
            public string materialCode7;
            public float? materialRequired7;
            public float? previousLeft7;
            public float? currentlyUsed7;
            public float? currentLeft7;
            public string salesOrderBatchCode;
            public string batchNum;
            public string materialStatus;
        };

		public materiallistDB(int index)
       	{
       		m_dbName = gVariable.DBHeadString + index.ToString().PadLeft(3, '0');
		}

		public string Serialize(material_t st)
		{
			string str = null;

			str += st.salesOrderCode+ ";" + st.dispatchCode 	 + ";" + st.machineID 	  + ";" + st.machineName	+ ";" + st.numOfTypes	+ ";";
            str += st.materialCode1 + ";" + st.materialRequired1 + ";" + st.previousLeft1 + ";" + st.currentlyUsed1 + ";" + st.currentLeft1 + ";";
            str += st.materialCode2 + ";" + st.materialRequired2 + ";" + st.previousLeft2 + ";" + st.currentlyUsed2 + ";" + st.currentLeft2 + ";";
			str += st.materialCode3	+ ";" + st.materialRequired3 + ";" + st.previousLeft3 + ";" + st.currentlyUsed3 + ";" + st.currentLeft3 + ";"; 
            str += st.materialCode4	+ ";" + st.materialRequired4 + ";" + st.previousLeft4 + ";" + st.currentlyUsed4 + ";" + st.currentLeft4 + ";";
			str += st.materialCode5	+ ";" + st.materialRequired5 + ";" + st.previousLeft5 + ";" + st.currentlyUsed5 + ";" + st.currentLeft5 + ";"; 
            str += st.materialCode6	+ ";" + st.materialRequired6 + ";" + st.previousLeft6 + ";" + st.currentlyUsed6 + ";" + st.currentLeft6 + ";";
            str += st.materialCode7 + ";" + st.materialRequired7 + ";" + st.previousLeft7 + ";" + st.currentlyUsed7 + ";" + st.currentLeft7 + ";"; 
            str += st.salesOrderBatchCode + ";" + st.batchNum + ";" + st.materialStatus;

			return str;
		}

		public string[] Format(material_t st)
		{
			string str;
			string[] strArray;

			str = Serialize(st);
			strArray = str.Split(';');
			return strArray;
		}

		public material_t? Deserialize(string strInput)
		{
			string[] input;
			material_t st = new material_t();

			input = strInput.Split(';');

            if (input.Length < TOTAL_DATAGRAM_NUM)
				return null;

			st.salesOrderCode = input[SALES_ORDER_CODE_INDEX];
			st.dispatchCode = input[DISPATCH_CODE_INDEX];
			st.machineID = input[MACHINE_ID_INDEX];
			st.machineName = input[MACHINE_NAME_INDEX];
            st.numOfTypes = input[NUM_OF_TYPES_INDEX];
			st.materialCode1 = input[MATERIAL_CODE1_INDEX];
            if (input[MATERIAL_REQUIRED1_INDEX] != "") st.materialRequired1 = Convert.ToSingle(input[MATERIAL_REQUIRED1_INDEX]);
            if (input[MATERIAL_PREVIOUS_LEFT1_INDEX] != "") st.previousLeft1 = Convert.ToSingle(input[MATERIAL_PREVIOUS_LEFT1_INDEX]);
            if (input[MATERIAL_CURRENTLY_USED1_INDEX] != "") st.currentlyUsed1 = Convert.ToSingle(input[MATERIAL_CURRENTLY_USED1_INDEX]);
            if (input[MATERIAL_CURRENT_LEFT1_INDEX] != "") st.currentLeft1 = Convert.ToSingle(input[MATERIAL_CURRENT_LEFT1_INDEX]);
            st.materialCode2 = input[MATERIAL_CODE2_INDEX];
			if (input[MATERIAL_REQUIRED2_INDEX]!="")	st.materialRequired2 = Convert.ToSingle(input[MATERIAL_REQUIRED2_INDEX]);
            if (input[MATERIAL_PREVIOUS_LEFT2_INDEX] != "") st.previousLeft2 = Convert.ToSingle(input[MATERIAL_PREVIOUS_LEFT2_INDEX]);
            if (input[MATERIAL_CURRENTLY_USED2_INDEX] != "") st.currentlyUsed2 = Convert.ToSingle(input[MATERIAL_CURRENTLY_USED2_INDEX]);
            if (input[MATERIAL_CURRENT_LEFT2_INDEX] != "") st.currentLeft2 = Convert.ToSingle(input[MATERIAL_CURRENT_LEFT2_INDEX]);
            st.materialCode3 = input[MATERIAL_CODE3_INDEX];
			if (input[MATERIAL_REQUIRED3_INDEX]!="")	st.materialRequired3 = Convert.ToSingle(input[MATERIAL_REQUIRED3_INDEX]);
            if (input[MATERIAL_PREVIOUS_LEFT3_INDEX] != "") st.previousLeft3 = Convert.ToSingle(input[MATERIAL_PREVIOUS_LEFT3_INDEX]);
            if (input[MATERIAL_CURRENTLY_USED3_INDEX] != "") st.currentlyUsed3 = Convert.ToSingle(input[MATERIAL_CURRENTLY_USED3_INDEX]);
            if (input[MATERIAL_CURRENT_LEFT3_INDEX] != "") st.currentLeft3 = Convert.ToSingle(input[MATERIAL_CURRENT_LEFT3_INDEX]);
            st.materialCode4 = input[MATERIAL_CODE4_INDEX];
			if (input[MATERIAL_REQUIRED4_INDEX]!="")	st.materialRequired4 = Convert.ToSingle(input[MATERIAL_REQUIRED4_INDEX]);
            if (input[MATERIAL_PREVIOUS_LEFT4_INDEX] != "") st.previousLeft4 = Convert.ToSingle(input[MATERIAL_PREVIOUS_LEFT4_INDEX]);
            if (input[MATERIAL_CURRENTLY_USED4_INDEX] != "") st.currentlyUsed4 = Convert.ToSingle(input[MATERIAL_CURRENTLY_USED4_INDEX]);
            if (input[MATERIAL_CURRENT_LEFT4_INDEX] != "") st.currentLeft4 = Convert.ToSingle(input[MATERIAL_CURRENT_LEFT4_INDEX]);
            st.materialCode5 = input[MATERIAL_CODE5_INDEX];
			if (input[MATERIAL_REQUIRED5_INDEX]!="")	st.materialRequired5 = Convert.ToSingle(input[MATERIAL_REQUIRED5_INDEX]);
            if (input[MATERIAL_PREVIOUS_LEFT5_INDEX] != "") st.previousLeft5 = Convert.ToSingle(input[MATERIAL_PREVIOUS_LEFT5_INDEX]);
            if (input[MATERIAL_CURRENTLY_USED5_INDEX] != "") st.currentlyUsed5 = Convert.ToSingle(input[MATERIAL_CURRENTLY_USED5_INDEX]);
            if (input[MATERIAL_CURRENT_LEFT5_INDEX] != "") st.currentLeft5 = Convert.ToSingle(input[MATERIAL_CURRENT_LEFT5_INDEX]);
            st.materialCode6 = input[MATERIAL_CODE6_INDEX];
			if (input[MATERIAL_REQUIRED6_INDEX]!="")	st.materialRequired6 = Convert.ToSingle(input[MATERIAL_REQUIRED6_INDEX]);
            if (input[MATERIAL_PREVIOUS_LEFT6_INDEX] != "") st.previousLeft6 = Convert.ToSingle(input[MATERIAL_PREVIOUS_LEFT6_INDEX]);
            if (input[MATERIAL_CURRENTLY_USED6_INDEX] != "") st.currentlyUsed6 = Convert.ToSingle(input[MATERIAL_CURRENTLY_USED6_INDEX]);
            if (input[MATERIAL_CURRENT_LEFT6_INDEX] != "") st.currentLeft6 = Convert.ToSingle(input[MATERIAL_CURRENT_LEFT6_INDEX]);
            st.materialCode7 = input[MATERIAL_CODE7_INDEX];
			if (input[MATERIAL_REQUIRED7_INDEX]!="")	st.materialRequired7 = Convert.ToSingle(input[MATERIAL_REQUIRED7_INDEX]);
            if (input[MATERIAL_PREVIOUS_LEFT7_INDEX] != "") st.previousLeft7 = Convert.ToSingle(input[MATERIAL_PREVIOUS_LEFT7_INDEX]);
            if (input[MATERIAL_CURRENTLY_USED7_INDEX] != "") st.currentlyUsed7 = Convert.ToSingle(input[MATERIAL_CURRENTLY_USED7_INDEX]);
            if (input[MATERIAL_CURRENT_LEFT7_INDEX] != "") st.currentLeft7 = Convert.ToSingle(input[MATERIAL_CURRENT_LEFT7_INDEX]);
            st.salesOrderBatchCode = input[SALESORDER_BATCHCODE_INDEX];
            st.batchNum = input[BATCH_NUM_INDEX];
            st.materialStatus = input[MATERIAL_STATUS_INDEX];

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
					
                /*myCommand.Parameters.AddWithValue(itemName[index++], st.salesOrderCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st.dispatchCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st.machineID);
				myCommand.Parameters.AddWithValue(itemName[index++], st.machineName);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode1);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialRequired1.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode2);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialRequired2.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode3);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialRequired3.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode4);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialRequired4.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode5);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialRequired5.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode6);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialRequired6.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode7);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialRequired7.ToString());*/

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
			material_t? dd=null;
			string[] recordArray;
            string insertString = null;
            string[] insertStringSplitted;
			string[] stringSeparators = new string[] { ",@" };

			getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
			insertStringSplitted = insertString.Split(stringSeparators, StringSplitOptions.None);

			string commandText = "select * from `" + c_TableName + "`";
			commandText += "where `" + insertStringSplitted[DISPATCH_CODE_INDEX] + "`=" + "\'" + dispatchCode + "\'";
			
			recordArray = databaseCommonReadingUnsplitted(m_dbName, commandText);
			if (recordArray!=null){
				dd = Deserialize(recordArray[0]);
			}
			return dd;
		}

		//2018.6.15 每次原料出库，修改“目标设备”对应的原料单的materialStatus
		public int updaterecord_ByDispatchcode(material_t st, string dispatchCode)
		{
			string insertString=null;
			string[] insertStringSplitted;
			string connectionString;
			string[] inputArray;
            string[] stringSeparators = new string[] { ",@" };

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
			//insertStringSplitted = insertString.Split(',@');
            //insertStringSplitted = insertString.Split(new char[2]{',','@'});
            insertStringSplitted = insertString.Split(stringSeparators, StringSplitOptions.None);

			inputArray = Format(st);

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + m_dbName + "; " + connectionString);
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
					myCommand.CommandText += "`" + insertStringSplitted[i+1] + "`=" + "\'" + inputArray[i] + "\'";
				}

				myCommand.CommandText += " where `" + insertStringSplitted[DISPATCH_CODE_INDEX] + "`=" + "\'" + dispatchCode + "\'";

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(m_dbName + ":" + c_TableName + ": update product barcode failed! " + ex);
            }
            return -1;
		}
	}
}

