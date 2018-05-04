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
	public class globaldatabase.materialinoutrecord : mySQLClass
	{
		//index
        private const int MATERIAL_CODE_INDEX = 1;
		private const int MATERIAL_NAME_INDEX = 2;
		private const int DIRECTION_INDEX = 3;
		private const int INOUT_PUT_TIME_INDEX = 4;
		private const int INOUT_PUT_QUANTITY_INDEX = 5;
		private const int TARGET_MACHINE_INDEX = 6;
		private const int FEEDBIN_INDEX_INDEX = 7;
		private const int DILIVERY_WORKER_INDEX = 8;
		private const int LEFT_IN_STACK_INDEX = 9;
		private const int FEED_TIME_INDEX = 10;
		private const int FEEDER_INDEX = 11;
		private const int FEED_QUANTITY_INDEX = 12;
		private const int DISPATCH_CODE_INDEX = 13;
		private const int LEFT_IN_FEEDBIN_INDEX = 14;

		private const string c_dbName = "globaldatabase";
        private const string c_materialInOutRecordTableName = "materialInOutRecord";
        private const string c_materialInOutRecordFileName = "..\\..\\data\\globalTables\\materialInOutRecord.xlsx";

		public struct materialinoutrecord_t{
			string materialCode;
			string materialName;
			string direction;
			string inoutPutTime;
			string inoutputQuantity;
			string targetMachine;
			string feedBinIndex;
			string diliveryWorker;
			string leftInStack;
			string feedTime;
			string feeder;
			string feedQuantity;
			string dispatchCode;
			string leftInFeedbin;
		};

		public materialinoutrecord_t? parseinput(string strInput)
		{
			string[] input;
			materialinoutrecord_t st_material;

			input = strInput.Split(';');

			if (input.Length < LEFT_IN_FEEDBIN_INDEX)
				return null;

			st_material.diliveryWorker = input[DILIVERY_WORKER_INDEX];
			st_material.direction = input[DIRECTION_INDEX];
			st_material.dispatchCode = input[DISPATCH_CODE_INDEX];
			st_material.feedBinIndex = input[FEEDBIN_INDEX_INDEX];
			st_material.feeder = input[FEEDER_INDEX];
			st_material.feedQuantity = input[FEED_QUANTITY_INDEX];
			st_material.feedTime = input[FEED_TIME_INDEX];
			st_material.inoutputQuantity = input[INOUT_PUT_QUANTITY_INDEX];
			st_material.inoutPutTime = input[INOUT_PUT_TIME_INDEX];
			st_material.leftInFeedbin = input[LEFT_IN_FEEDBIN_INDEX];
			st_material.leftInStack = input[LEFT_IN_STACK_INDEX];
			st_material.materialCode = input[MATERIAL_CODE_INDEX];
			st_material.materialName = input[MATERIAL_NAME_INDEX];
			st_material.targetMachine = input[TARGET_MACHINE_INDEX];

			return st_material;
		}

        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(materialinoutrecord_t st_material)
        {
            int num;
            int index;
            string[] itemName;
			string insertString;
			string connectionString;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_materialInOutRecordFileName);

            try
            {
                index = 0;
                itemName = insertString.Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + c_materialInOutRecordTableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], st_material.materialCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st_material.materialName);
                myCommand.Parameters.AddWithValue(itemName[index++], st_material.direction);
				myCommand.Parameters.AddWithValue(itemName[index++], st_material.inoutPutTime);
                myCommand.Parameters.AddWithValue(itemName[index++], st_material.inoutputQuantity);
				myCommand.Parameters.AddWithValue(itemName[index++], st_material.targetMachine);
                myCommand.Parameters.AddWithValue(itemName[index++], st_material.feedBinIndex);
				myCommand.Parameters.AddWithValue(itemName[index++], st_material.diliveryWorker);
                myCommand.Parameters.AddWithValue(itemName[index++], st_material.leftInStack);
				myCommand.Parameters.AddWithValue(itemName[index++], st_material.feedTime);
                myCommand.Parameters.AddWithValue(itemName[index++], st_material.feeder);
				myCommand.Parameters.AddWithValue(itemName[index++], st_material.feedQuantity);
                myCommand.Parameters.AddWithValue(itemName[index++], st_material.dispatchCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st_material.leftInFeedbin);

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(c_dbName + "write to " + c_materialInOutRecordTableName + " failed!" + ex);
            }
            return -1;
        }
	}
}

