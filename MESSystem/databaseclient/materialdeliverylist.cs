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
	public class materialdeliverylistDB : mySQLClass
	{
		//index
        private const int MATERIAL_CODE_INDEX = 1;
		private const int MATERIAL_BATCH_NUM_INDEX = 2;
		private const int DIRECTION_INDEX =3;
		private const int INOUTPUT_TIME_INDEX = 4;
		private const int INOUTPUT_QUANTITY_INDEX= 5;
		private const int TARGET_MACHINE_INDEX = 6;
		private const int TARGET_FEEDBIN_INDEX = 7;
		private const int DELIVERY_WORKER_INDEX = 8;
		private const int TOTAL_DATAGRAM_NUM = DELIVERY_WORKER_INDEX;

		private const string c_dbName = "globaldatabase";
        private const string c_TableName = "materialdeliverylist";
        private const string c_FileName = "..\\..\\data\\globalTables\\materialDeliveryList.xlsx";

		public struct materialdelivery_t {
			public string materialCode;
            public string materialBatchNum;
            public string direction;
            public string inoutputTime;
            public string inoutputQuantity;
            public string targetMachine;
            public string targetFeedBinIndex;
            public string deliveryWorker;
		}

		public string Serialize(materialdelivery_t st)
		{
			string str = null;

			str += st.materialCode		+ ";" + st.materialBatchNum	+ ";" + st.direction			+ ";" + st.inoutputTime	+ ";";
			str += st.inoutputQuantity	+ ";" + st.targetMachine	+ ";" + st.targetFeedBinIndex	+ ";" + st.deliveryWorker;
			return str;
		}

		public string[] Format(materialdelivery_t st)
		{
			string str;
			string[] strArray;

			str = Serialize(st);
			strArray = str.Split(';');
			return strArray;
		}
		
		public materialdelivery_t? Deserialize(string strInput)
		{
			string[] input;
			materialdelivery_t st = new materialdelivery_t();

			input = strInput.Split(';');

			if (input.Length < TOTAL_DATAGRAM_NUM-1)
				return null;

			st.deliveryWorker = input[DELIVERY_WORKER_INDEX];
			st.direction = input[DIRECTION_INDEX];
			st.inoutputQuantity = input[INOUTPUT_QUANTITY_INDEX];
			st.inoutputTime = input[INOUTPUT_TIME_INDEX];
			st.materialBatchNum = input[MATERIAL_BATCH_NUM_INDEX];
			st.materialCode = input[MATERIAL_CODE_INDEX];
			st.targetFeedBinIndex = input[TARGET_FEEDBIN_INDEX];
			st.targetMachine = input[TARGET_MACHINE_INDEX];

			return st;
		}

        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(materialdelivery_t st)
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
				for (index=1;index<=TOTAL_DATAGRAM_NUM;index++)
					myCommand.Parameters.AddWithValue(itemName[index], inputArray[index-1]);

                /*myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st.materialBatchNum);
                myCommand.Parameters.AddWithValue(itemName[index++], st.direction);
				myCommand.Parameters.AddWithValue(itemName[index++], st.inoutputTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st.inoutputQuantity);
                myCommand.Parameters.AddWithValue(itemName[index++], st.targetMachine);
				myCommand.Parameters.AddWithValue(itemName[index++], st.targetFeedBinIndex);
                myCommand.Parameters.AddWithValue(itemName[index++], st.deliveryWorker);*/

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

