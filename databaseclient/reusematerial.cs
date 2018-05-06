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
	public class reusematerialDB : mySQLClass
	{
		//index
        private const int REBUILD_DATE_INDEX = 1;
		private const int BOM_CODE_INDEX = 2;
		private const int BARCODE_FOR_REUSE_INDEX =3;
		private const int REBUILD_NUM_INDEX = 4;
		private const int WORKER_ID_INDEX= 5;
		private const int BARCODE1_INDEX = 6;
		private const int BARCODE2_INDEX = 7;
		private const int BARCODE3_INDEX = 8;
		private const int BARCODE4_INDEX = 9;
		private const int BARCODE5_INDEX = 10;
		private const int BARCODE6_INDEX = 11;
		private const int BARCODE7_INDEX = 12;
		private const int BARCODE8_INDEX = 13;
		private const int BARCODE9_INDEX = 14;
		private const int BARCODE10_INDEX = 15;
		private const int TOTAL_DATAGRAM_NUM = BARCODE10_INDEX+1;

		private const string c_dbName = "globaldatabase";
        private const string c_reusematerialTableName = "reusematerial";
        private const string c_reusematerialFileName = "..\\..\\data\\globalTables\\reuseMaterial.xlsx";

		public struct reusematerial_t{
			string rebuildDate;
			string BOMCode;
			string barcodeForReuse;
			string rebuildNum;
			string workerID;
			string barCode1;
			string barCode2;
			string barCode3;
			string barCode4;
			string barCode5;
			string barCode6;
			string barCode7;
			string barCode8;
			string barCode9;
			string barCode10;
		}

		public string Serialize(reusematerial_t st)
		{
			string str = null;

			str += st.rebuildDate 	+ ";" + st.BOMCode 			+ ";" + st.barcodeForReuse	+ ";" + st.rebuildNum	+ ";";
			str += st.workerID		+ ";" + st.numberOfBarCodes	+ ";" + st.barCode1 		+ ";" + st.barCode2 	+ ";";
			str += st.barCode3		+ ";" + st.barCode4			+ ";" + st.barCode5 		+ ";" + st.barCode6 	+ ";";
			str += st.barCode7		+ ";" + st.barCode8			+ ";" + st.barCode9 		+ ";" + st.barCode10;
			return str;
		}

		public string[] Format(reusematerial_t st)
		{
			string str;
			string[] strArray;

			str = Serialize(st);
			strArray = str.Split(';');
			return strArray;
		}
		
		public reusematerial_t? Deserialize(string strInput)
		{
			string[] input;
			reusematerial_t st;

			input = strInput.Split(';');

			if (input.Length < LEFT_IN_FEEDBIN_INDEX)
				return null;

			st.barCode1 = input[BARCODE1_INDEX];
			st.barCode10 = input[BARCODE10_INDEX];
			st.barCode2 = input[BARCODE2_INDEX];
			st.barCode3 = input[BARCODE3_INDEX];
			st.barCode4 = input[BARCODE4_INDEX];
			st.barCode5 = input[BARCODE5_INDEX];
			st.barCode6 = input[BARCODE6_INDEX];
			st.barCode7 = input[BARCODE7_INDEX];
			st.barCode8 = input[BARCODE8_INDEX];
			st.barCode9 = input[BARCODE9_INDEX];
			st.barcodeForReuse = input[BARCODE_FOR_REUSE_INDEX];
			st.BOMCode = input[BOM_CODE_INDEX];
			st.numberOfBarCodes = input[NUM_OF_BAR_CODES_INDEX];
			st.rebuildDate = input[REBUILD_DATE_INDEX];
			st.rebuildNum = input[REBUILD_NUM_INDEX];
			st.workerID = input[WORKER_ID_INDEX];

			return st;
		}

        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(reusematerial_t st)
        {
            int num;
            int index;
            string[] itemName;
			string insertString;
			string connectionString;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_reusematerialFileName);

            try
            {
                index = 0;
                itemName = insertString.Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + c_reusematerialTableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], st.rebuildDate);
				myCommand.Parameters.AddWithValue(itemName[index++], st.BOMCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st.barcodeForReuse);
				myCommand.Parameters.AddWithValue(itemName[index++], st.rebuildNum);
				myCommand.Parameters.AddWithValue(itemName[index++], st.workerID);
                myCommand.Parameters.AddWithValue(itemName[index++], st.numberOfBarCodes);
				myCommand.Parameters.AddWithValue(itemName[index++], st.barCode1);
                myCommand.Parameters.AddWithValue(itemName[index++], st.barCode2);
				myCommand.Parameters.AddWithValue(itemName[index++], st.barCode3);
                myCommand.Parameters.AddWithValue(itemName[index++], st.barCode4);
				myCommand.Parameters.AddWithValue(itemName[index++], st.barCode5);
				myCommand.Parameters.AddWithValue(itemName[index++], st.barCode6);
                myCommand.Parameters.AddWithValue(itemName[index++], st.barCode7);
				myCommand.Parameters.AddWithValue(itemName[index++], st.barCode8);
                myCommand.Parameters.AddWithValue(itemName[index++], st.barCode9);
				myCommand.Parameters.AddWithValue(itemName[index++], st.barCode10);

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(c_dbName + "write to " + c_productcastinglistTableName + " failed!" + ex);
            }
            return -1;
        }
	}
}

