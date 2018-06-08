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
	public class dispatchlistDB : mySQLClass
	{
		//index
		private const int MACHINE_ID_INDEX = 1;
		private const int DISPATCH_CODE_INDEX = 2;
		private const int PLAN_TIME1_INDEX = 3;
		private const int PLAN_TIME2_INDEX = 4;
		private const int PRODUCT_CODE_INDEX = 5;
		private const int PRODUCT_NAME_INDEX = 6;
		private const int OPERATOR_NAME_INDEX = 7;
		private const int FORCAST_NUM_INDEX = 8;
		private const int RECEIVE_NUM_INDEX = 9;
		private const int QUALIFY_NUM_INDEX = 10;
		private const int UNQUALIFY_NUM_INDEX = 11;
        private const int PROCESS_NAME_INDEX = 12;
		private const int START_TIME_INDEX = 13;
		private const int COMPLETE_TIME_INDEX= 14;
		private const int PREPARE_TIMEPOINT_INDEX = 15;
		private const int STATUS_INDEX = 16;
		private const int TOOL_LIFETIME_INDEX = 17;
		private const int TOOL_USED_TIMES_INDEX = 18;
		private const int OUTPUT_RATIO_INDEX = 19;
		private const int SERIAL_NUMBER_INDEX = 20;
		private const int REPORTER_INDEX = 21;
		private const int WORKSHOP_INDEX = 22;
		private const int WORKSHIFT_INDEX = 23;
		private const int SALES_ORDER_CODE_INDEX = 24;
		private const int BOM_CODE_INDEX = 25;
		private const int CUSTOMER_INDEX = 26;
		private const int MULTI_PRODUCT_INDEX = 27;
		private const int PRODUCT_CODE2_INDEX = 28;
		private const int PRODUCT_CODE3_INDEX = 29;
		private const int OPERATOR_NAME2_INDEX = 30;
		private const int OPERATOR_NAME3_INDEX = 31;
		private const int BATCH_NUMBER_INDEX = 32;
		private const int PRODUCT_COLOR_INDEX = 33;
		private const int RAW_MATERIAL_CODE_INDEX = 34;
		private const int PRODUCT_LENGTH_INDEX = 35;
		private const int PRODUCT_DIAMETER_INDEX = 36;
		private const int PRODUCT_WEIGHT_INDEX = 37;
		private const int SLIT_WIDTH_INDEX = 38;
		private const int PRINT_SIDE_INDEX = 39;
		private const int PRODUCT_CODE4_INDEX = 40;
		private const int OPERATOR_NAME4_INDEX = 41;
		private const int NOTES_INDEX = 42;
		private const int COMMENTS_INDDEX = 43;
		private const int TOTAL_DATAGRAM_NUM = COMMENTS_INDDEX;

		private const string c_TableName = "0_dispatchlist";
        private const string c_FileName = "..\\..\\data\\machine\\dispatchList.xlsx";

		private string m_dbName;

        public struct dispatchlist_t
        {
            public string machineID;   //设备序号 
            public string dispatchCode;  //dispatch code
            public string planTime1;	//预计开始时间
            public string planTime2;  //预计结束时间
            public string productCode;	 //产品编码
            public string productName;  //产品名称
            public string operatorName; //操作员
            public string forcastNum;
			public string receiveNum;
            public uint? qualifyNumber;  //合格数量
            public uint? unqualifyNumber;  //不合格数量
            public string processName; //工序（工艺路线中包含多个工序）
            public string startTime;	  //实际开始时间
            public string completeTime;	  //实际完工时间
            public string prepareTimePoint;   //调整时间（试产时间），开工后先经过调整时间，然后再正式生产
            public string status;	  //0：dummy/prepared 工单准备完成但未发布，1:工单完工，新工单未发布，2：工单发布至设备 3：工单已启动 4：试产（调整时间） OK
            public uint? waitForCheck;  //待检品
            public uint? wastedOutput;  //废料
            public string workTeam;  //班别(甲乙丙)
            public string serialNumber; //流水号
            public string reportor; //报工员工号, 报工员和操作员可能不是同一个人
            public string workshop;	 //车间名称，或者是流水线的名字
            public string workshift;	 //班次（早中晚班）
            public string salesOrderCode; //订单号
            public string BOMCode; // 
            public string customer;
            public string multiProduct;
            public string productCode2;
            public string productCode3;
            public string operatorName2; //操作员
            public string operatorName3; //操作员
            public string batchNum; //批次号，previously used inside Zihua, now we don't need this in new system, but need it to be compatible with the old system
			public string productColor;
			public string rawMaterialCode;
			public float? productLength;
			public float? productDiameter;
			public float? productWeight;
			public string slitWidth;
			public string printSize;
			public string productCode4;
			public string operatorName4;
            public string notes;
			public string comments;
        }

		public dispatchlistDB(int index)
       	{
       		m_dbName = gVariable.DBHeadString + index.ToString().PadLeft(3, '0');
		}
		
		public string Serialize(dispatchlist_t st)
		{
			string str = null;

			str += st.machineID  	+ ";" + st.dispatchCode 	+ ";" + st.planTime1 		+ ";" + st.planTime2 		+ ";";
			str += st.productCode	+ ";" + st.productName 		+ ";" + st.operatorName 	+ ";" + st.forcastNum 		+ ";";
			str += st.receiveNum  	+ ";" + st.qualifyNumber 	+ ";" + st.unqualifyNumber 	+ ";" + st.processName 		+ ";";
			str += st.startTime   	+ ";" + st.completeTime 	+ ";" + st.prepareTimePoint	+ ";" + st.status 			+ ";";
			str += st.waitForCheck  + ";" + st.wastedOutput 	+ ";" + st.workTeam 		+ ";" + st.serialNumber 	+ ";";
			str += st.reportor 		+ ";" + st.workshop 		+ ";" + st.workshift 		+ ";" + st.salesOrderCode 	+ ";";
			str += st.BOMCode 		+ ";" + st.customer 		+ ";" + st.multiProduct 	+ ";" + st.productCode2 	+ ";";
			str += st.productCode3 	+ ";" + st.operatorName2 	+ ";" + st.operatorName3 	+ ";" + st.batchNum 		+ ";";
			str += st.productColor 	+ ";" + st.rawMaterialCode 	+ ";" + st.productLength 	+ ";" + st.productDiameter	+ ";";
			str += st.productWeight + ";" + st.slitWidth 		+ ";" + st.printSize		+ ";" + st.productCode4 	+ ";";
			str += st.operatorName4 + ";" + st.notes			+ ";" + st.comments;

			return str;
		}

		public string[] Format(dispatchlist_t st)
		{
			string str;
			string[] strArray;

			str = Serialize(st);
			strArray = str.Split(';');
			return strArray;
		}
		
		public dispatchlist_t? Deserialize(string strInput)
		{
			string[] input;
			dispatchlist_t st = new dispatchlist_t();

			input = strInput.Split(';');

			if (input.Length < TOTAL_DATAGRAM_NUM)
				return null;

			st.machineID = input[MACHINE_ID_INDEX];
			st.dispatchCode = input[DISPATCH_CODE_INDEX]; 
			st.planTime1 = input[PLAN_TIME1_INDEX];
			st.planTime2 = input[PLAN_TIME2_INDEX];
			st.productCode = input[PRODUCT_CODE_INDEX];
			st.productName = input[PRODUCT_NAME_INDEX];
			st.operatorName = input[OPERATOR_NAME_INDEX];
			st.forcastNum = input[FORCAST_NUM_INDEX];
			st.receiveNum = input[RECEIVE_NUM_INDEX];
			if (input[QUALIFY_NUM_INDEX]!="")	st.qualifyNumber = Convert.ToUInt32(input[QUALIFY_NUM_INDEX],10);
			if (input[UNQUALIFY_NUM_INDEX]!="")	st.unqualifyNumber = Convert.ToUInt32(input[UNQUALIFY_NUM_INDEX],10);
			st.processName = input[PROCESS_NAME_INDEX];
			st.startTime = input[START_TIME_INDEX];
			st.completeTime = input[COMPLETE_TIME_INDEX];
			st.prepareTimePoint = input[PREPARE_TIMEPOINT_INDEX];
			st.status = input[STATUS_INDEX];
			if (input[TOOL_LIFETIME_INDEX]!="")	st.waitForCheck = Convert.ToUInt32(input[TOOL_LIFETIME_INDEX],10);
			if (input[TOOL_USED_TIMES_INDEX]!="")	st.wastedOutput = Convert.ToUInt32(input[TOOL_USED_TIMES_INDEX],10);
			if (input[OUTPUT_RATIO_INDEX]!="")	st.workTeam = input[OUTPUT_RATIO_INDEX];
			st.serialNumber = input[SERIAL_NUMBER_INDEX];
			st.reportor = input[REPORTER_INDEX];
			st.workshop = input[WORKSHOP_INDEX];
			st.workshift = input[WORKSHIFT_INDEX];
			st.salesOrderCode = input[SALES_ORDER_CODE_INDEX];
			st.BOMCode = input[BOM_CODE_INDEX];
			st.customer = input[CUSTOMER_INDEX];
			st.multiProduct = input[MULTI_PRODUCT_INDEX];
			st.productCode2 = input[PRODUCT_CODE2_INDEX];
			st.productCode3 = input[PRODUCT_CODE3_INDEX];
			st.operatorName2 = input[OPERATOR_NAME2_INDEX];
			st.operatorName3 = input[OPERATOR_NAME3_INDEX];
			st.batchNum = input[BATCH_NUMBER_INDEX];
			st.productColor = input[PRODUCT_COLOR_INDEX];
			st.rawMaterialCode = input[RAW_MATERIAL_CODE_INDEX];
			if (input[PRODUCT_LENGTH_INDEX]!="")	st.productLength = Convert.ToSingle(input[PRODUCT_LENGTH_INDEX]);
			if (input[PRODUCT_DIAMETER_INDEX]!="")	st.productDiameter = Convert.ToSingle(input[PRODUCT_DIAMETER_INDEX]);
			if (input[PRODUCT_WEIGHT_INDEX]!="")	st.productWeight = Convert.ToSingle(input[PRODUCT_WEIGHT_INDEX]);
			st.slitWidth = input[SLIT_WIDTH_INDEX];
			st.printSize = input[PRINT_SIDE_INDEX];
			st.productCode4 = input[PRODUCT_CODE4_INDEX];
			st.operatorName4 = input[OPERATOR_NAME4_INDEX];
			st.notes = input[NOTES_INDEX];
			st.comments = input[COMMENTS_INDDEX];
			
			return st;
		}

		public dispatchlist_t[] readallrecord_Ordered()
		{
			dispatchlist_t? dd;
			string[] recordArray;
			dispatchlist_t[] st_dispatchlist=null;
			string commandText = "select * from `" + c_TableName + "` order by id DESC";
			recordArray = mySQLClass.databaseCommonReadingUnsplitted(m_dbName, commandText);
			if (recordArray!=null){
				st_dispatchlist = new dispatchlist_t[recordArray.Length];
				for (int i=0;i<recordArray.Length;i++){
					dd = Deserialize(recordArray[i]);
					st_dispatchlist[i] = dd.Value;
				}
			}
			return st_dispatchlist;
		}

		public dispatchlist_t[] readrecord_ByDispatchcode(string dispatchCode)
		{
			dispatchlist_t? dd;
			string[] recordArray;
			string[] insertStringSplitted;
			dispatchlist_t[] st_dispatchlist=null;
			string[] stringSeparators = new string[] { ",@" };
			string insertString=null;

			getDatabaseInsertStringFromExcel(ref insertString, c_FileName);
			insertStringSplitted = insertString.Split(stringSeparators, StringSplitOptions.None);
			
			string commandText = "select * from `" + c_TableName + "` ";
			commandText += "where `" + insertStringSplitted[DISPATCH_CODE_INDEX] + "`=" + "\'" + dispatchCode + "\'";

			recordArray = mySQLClass.databaseCommonReadingUnsplitted(m_dbName, commandText);
			if (recordArray!=null){
				st_dispatchlist = new dispatchlist_t[recordArray.Length];
				for (int i=0;i<recordArray.Length;i++){
					dd = Deserialize(recordArray[i]);
					st_dispatchlist[i] = dd.Value;
				}
			}
			return st_dispatchlist;
		}

		public int updaterecord_ByDispatchcode(dispatchlist_t st, string dispatchCode)
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

        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(dispatchlist_t st)
        {
            //int num;
            int index;
            string[] itemName;
			string insertString=null;
			string connectionString;
			string[] inputArray;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_TableName);

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

				/*myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.machineID);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.dispatchCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.planTime1);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.planTime2);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.productCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.productName);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.operatorName);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.forcastNum);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.receiveNum);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.qualifyNumber.ToString());
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.unqualifyNumber.ToString());
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.processName);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.startTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.completeTime);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.prepareTimePoint);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.status);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.waitForCheck.ToString());
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.wastedOutput.ToString());
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.workTeam.ToString());
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.serialNumber);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.reportor);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.workshop);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.workshift);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.salesOrderCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.BOMCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.customer);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.multiProduct);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.productCode2);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.productCode3);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.operatorName2);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.operatorName3);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.batchNum);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.productColor);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.rawMaterialCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.productLength.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.productDiameter.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.productWeight.ToString());
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.slitWidth);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.printSize);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.productCode4);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.operatorName4);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.notes);
				myCommand.Parameters.AddWithValue(itemName[index++], st_dispatchlist.comments);*/

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(m_dbName+":"+c_TableName+": write record failed!" + ex);
            }
            return -1;
        }

/*
		public int updateDispatchNotes(string dispatchCode, string notes)
        {
		}

		public int updateDispatchStatus(string[] in_dispatch)
        {
			string insertString;
			string[] insertStringSplitted;
			string connectionString;

			if (in_dispatch.Length != TOTAL_DATAGRAM_NUM-1)	return gVariable.RESULT_ERR_DATA;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_dispatchListFileName);
			insertStringSplitted = insertString.Split(',@');

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + m_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

				myCommand.CommandText = "update ";
				myCommand.CommandText += "`" + c_dispatchListTableName + "` ";
				myCommand.CommandText += "set ";

				for (int i=0;i<in_dispatch.Length;i++){
					myCommand.CommandText += "`" + insertStringSplitted[i+1] + "`=" + in_dispatch[i];
					if (i != in_dispatch.Length )
						myCommand.CommandText += ",";
				}

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(c_dbName + ":" + c_productprintlistTableName + ": update product barcode failed! " + ex);
            }
            return -1;

		}

		public int updateDispatchStatus(dispatchlist_t st_dispatchlist)
        {
			string insertString;
			string[] insertStringSplitted;
			string connectionString;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_dispatchListFileName);
			insertStringSplitted = insertString.Split(',@');

            try
            {
                MySqlConnection myConnection = new MySqlConnection("database = " + m_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

				if (st_dispatchlist.status == gVariable.MACHINE_STATUS_DISPATCH_COMPLETED){
                	myCommand.CommandText = "update ";
                	myCommand.CommandText += "`" + c_dispatchListTableName + "` ";
					myCommand.CommandText += "set ";
					myCommand.CommandText += "`" + insertStringSplitted[OPERATOR_NAME_INDEX] + "`=" + st_dispatchlist.operatorName + ",";
					myCommand.CommandText += "`" + insertStringSplitted[RECEIVE_NUM_INDEX] + "`=" + st_dispatchlist.receiveNum + ",";
					myCommand.CommandText += "`" + insertStringSplitted[QUALIFY_NUM_INDEX] + "`=" + st_dispatchlist.qualifyNumber + ",";
					myCommand.CommandText += "`" + insertStringSplitted[UNQUALIFY_NUM_INDEX] + "`=" + st_dispatchlist.unqualifyNumber + ",";
					myCommand.CommandText += "`" + insertStringSplitted[COMPLETE_TIME_INDEX] + "`=" + st_dispatchlist.completeTime + ",";
					myCommand.CommandText += "`" + insertStringSplitted[STATUS_INDEX] + "`=" + st_dispatchlist.status + ",";
					myCommand.CommandText += "`" + insertStringSplitted[TOOL_LIFETIME_INDEX] + "`=" + st_dispatchlist.status + ",";
					myCommand.CommandText += "`" + insertStringSplitted[TOOL_USED_TIMES_INDEX] + "`=" + st_dispatchlist.status + ",";
					myCommand.CommandText += "`" + insertStringSplitted[OUTPUT_RATIO_INDEX] + "`=" + st_dispatchlist.status + ",";
					myCommand.CommandText += "`" + insertStringSplitted[REPORTER_INDEX] + "`=" + st_dispatchlist.status + ",";
					myCommand.CommandText += "`" + insertStringSplitted[STATUS_INDEX] + "`=" + st_dispatchlist.status + ",";
					myCommand.CommandText += "`" + insertStringSplitted[WORKSHIFT_INDEX] + "`=" + st_dispatchlist.status + ",";
					myCommand.CommandText += "`" + insertStringSplitted[RECEIVE_NUM_INDEX] + "`=" + st_dispatchlist.productScanTime;
				}

                myCommand.ExecuteNonQuery();
                myConnection.Close();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(c_dbName + ":" + c_productprintlistTableName + ": update product barcode failed! " + ex);
            }
            return -1;
		}	

*/
			}
	}

