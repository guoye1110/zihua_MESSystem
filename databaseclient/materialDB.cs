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
	public class materialDB : mySQLClass
	{
		//index
        private const int MATERIAL_CODE_INDEX = 1;
		private const int STACK_SIZE_INDEX = 2;
		private const int STACK_HEIGHT_INDEX =3;
		private const int SACK_WEIGHT_INDEX = 4;
		private const int STACK_WEIGHT_INDEX= 5;
		private const int SACK_NUM_ONE_LAYER_INDEX = 6;
		private const int LAYER_NUM_INDEX = 7;
		private const int FULL_STACK_NUM_INDEX = 8;
		private const int TOTAL_DATAGRAM_NUM = FULL_STACK_NUM_INDEX+1;

		private const string c_dbName = "basicinfo";
        private const string c_TableName = "material";
        private const string c_FileName = "..\\..\\data\\basicData\\material.xlsx";

		public struct material_t {
			public string materialCode;
            public string stackSize;
            public string stackHeight;
            public string sackWeight;
            public string stackWeight;
            public string sackNumOneLayer;
            public string layerNum;
            public string fullStackNum;
		}

		public string Serialize(material_t st)
		{
			string str = null;

			str += st.materialCode	+ ";" + st.stackSize		+ ";" + st.stackHeight	+ ";" + st.sackWeight	+ ";";
			str += st.stackWeight	+ ";" + st.sackNumOneLayer	+ ";" + st.layerNum		+ ";" + st.fullStackNum;
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
			material_t st;

			input = strInput.Split(';');

			if (input.Length < TOTAL_DATAGRAM_NUM-1)
				return null;

			st.materialCode = input[MATERIAL_CODE_INDEX];
			st.stackSize = input[STACK_SIZE_INDEX];
			st.stackHeight = input[STACK_HEIGHT_INDEX];
			st.sackWeight = input[SACK_WEIGHT_INDEX];
			st.stackWeight = input[STACK_HEIGHT_INDEX];
			st.sackNumOneLayer = input[SACK_NUM_ONE_LAYER_INDEX];
			st.layerNum = input[LAYER_NUM_INDEX];
			st.fullStackNum = input[FULL_STACK_NUM_INDEX];

			return st;
		}

		public material_t? readrecord_byMaterialCode(string materialCode)
        {
			material_t? dd=null;
			string[] recordArray;
			string insertString,insertStringSplitted;

			getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
			insertStringSplitted = insertString.Split(new char[2]{',','@'});

			string commandText = "select * from `" + c_TableName + "`";
			commandText += "where `" + insertStringSplitted[MATERIAL_CODE_INDEX] + "`=" + materialCode;
			
			recordArray = databaseCommonReadingUnsplitted(c_dbName, commandText);
			if (recordArray!=null){
				dd = Deserialize(recordArray[0]);
			}
			return dd;
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
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_FileName);

            try
            {
                index = 1;
                itemName = insertString.Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + c_TableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], st.materialCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st.stackSize);
                myCommand.Parameters.AddWithValue(itemName[index++], st.stackHeight);
				myCommand.Parameters.AddWithValue(itemName[index++], st.sackWeight);
				myCommand.Parameters.AddWithValue(itemName[index++], st.stackWeight);
                myCommand.Parameters.AddWithValue(itemName[index++], st.sackNumOneLayer);
				myCommand.Parameters.AddWithValue(itemName[index++], st.layerNum);
                myCommand.Parameters.AddWithValue(itemName[index++], st.fullStackNum);

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

