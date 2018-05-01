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
	public class machine.dispatchlist : mySQLClass
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
		private const int BARCODE_INDEX = 27;
		private const int BARCODE_FOR_REUSE_INDEX = 28;
		private const int QUANTITY_OF_REUSED_INDEX = 29;
		private const int MULTI_PRODUCT_INDEX = 30;
		private const int PRODUCT_CODE2_INDEX = 31;
		private const int PRODUCT_CODE3_INDEX = 32;
		private const int OPERATOR_NAME2_INDEX = 33;
		private const int OPERATOR_NAME3_INDEX = 34;
		private const int BATCH_NUMBER_INDEX = 35;

		private const int TOTAL_DATAGRAM_NUM = BATCH_NUMBER_INDEX+1;

		private const string c_dispatchListTableName = "0_dispatchlist";
        private const string c_dispatchListFileName = "..\\..\\data\\machine\\dispatchList.xlsx";

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
            public int qualifyNumber;  //合格数量
            public int unqualifyNumber;  //不合格数量
            public string processName; //工序（工艺路线中包含多个工序）
            public string startTime;	  //实际开始时间
            public string completeTime;	  //实际完工时间
            public string prepareTimePoint;   //调整时间（试产时间），开工后先经过调整时间，然后再正式生产
            public int status;	  //0：dummy/prepared 工单准备完成但未发布，1:工单完工，新工单未发布，2：工单发布至设备 3：工单已启动 4：试产（调整时间） OK
            public int toolLifeTimes;  //刀具寿命次数
            public int toolUsedTimes;  //刀具使用次数
            public int outputRatio;  //单件系数
            public string serialNumber; //流水号
            public string reportor; //报工员工号, 报工员和操作员可能不是同一个人
            public string workshop;	 //车间名称，或者是流水线的名字
            public string workshift;	 //班次（早中晚班）
            public string salesOrderCode; //订单号
            public string BOMCode; // 
            public string customer;
            public string barCode;
            public string barcodeForReuse;
            public string quantityOfReused;
            public string multiProduct;
            public string productCode2;
            public string productCode3;
            public string operatorName2; //操作员
            public string operatorName3; //操作员
            public string batchNum; //批次号，previously used inside Zihua, now we don't need this in new system, but need it to be compatible with the old system
        }

		public machine.dispatchlist(int printingSWPCID)
        {
        	m_dbName = gVariable.DBHeadString + printingSWPCID.ToString().PadLeft(3, '0');
		}
		
		public dispatchlist_t? parseinput(string strInput)
		{
			string[] input;
			dispatchlist_t st_dispatchlist;

			input = strInput.Split(';');

			if (input.Length < LEFT_IN_FEEDBIN_INDEX)
				return null;

			st_dispatchlist.barCode = input[BARCODE_INDEX];
			st_dispatchlist.barcodeForReuse = input[BARCODE_FOR_REUSE_INDEX];
			st_dispatchlist.batchNum = input[BATCH_NUMBER_INDEX];
			st_dispatchlist.BOMCode = input[BOM_CODE_INDEX];
			st_dispatchlist.completeTime= input[COMPLETE_TIME_INDEX];
			st_dispatchlist.customer = input[CUSTOMER_INDEX];
			st_dispatchlist.dispatchCode = input[DISPATCH_CODE_INDEX];
			st_dispatchlist.forcastNum = input[FORCAST_NUM_INDEX];
			st_dispatchlist.machineID = input[MACHINE_ID_INDEX];
			st_dispatchlist.multiProduct = input[MULTI_PRODUCT_INDEX];
			st_dispatchlist.operatorName = input[OPERATOR_NAME_INDEX];
			st_dispatchlist.operatorName2 = input[OPERATOR_NAME2_INDEX];
			st_dispatchlist.operatorName3 = input[OPERATOR_NAME3_INDEX];
			st_dispatchlist.outputRatio = input[OUTPUT_RATIO_INDEX];
			st_dispatchlist.planTime1 = input[PLAN_TIME1_INDEX];
			st_dispatchlist.planTime2 = input[PLAN_TIME2_INDEX];
			st_dispatchlist.prepareTimePoint = input[PREPARE_TIMEPOINT_INDEX];
			st_dispatchlist.processName = input[PROCESS_NAME_INDEX];
			st_dispatchlist.productCode = input[PRODUCT_CODE_INDEX];
			st_dispatchlist.productCode2 = input[PRODUCT_CODE2_INDEX];
			st_dispatchlist.productCode3 = input[PRODUCT_CODE3_INDEX];
			st_dispatchlist.productName = input[PRODUCT_NAME_INDEX];
			st_dispatchlist.qualifyNumber = input[QUALIFY_NUM_INDEX];
			st_dispatchlist.quantityOfReused = input[QUANTITY_OF_REUSED_INDEX];
			st_dispatchlist.receiveNum = input[RECEIVE_NUM_INDEX]
			st_dispatchlist.reportor = input[REPORTER_INDEX];
			st_dispatchlist.salesOrderCode = input[SALES_ORDER_CODE_INDEX];
			st_dispatchlist.serialNumber = input[SERIAL_NUMBER_INDEX];
			st_dispatchlist.startTime = input[START_TIME_INDEX];
			st_dispatchlist.status = input[STATUS_INDEX];
			st_dispatchlist.toolLifeTimes = input[TOOL_LIFETIME_INDEX];
			st_dispatchlist.toolUsedTimes = input[TOOL_USED_TIMES_INDEX];
			st_dispatchlist.unqualifyNumber = input[UNQUALIFY_NUM_INDEX];
			st_dispatchlist.workshift = input[WORKSHIFT_INDEX];
			st_dispatchlist.workshop = input[WORKSHOP_INDEX];
			
			return st_dispatchlist;
		}

        //return 0 written to table successfully
        //      -1 exception occurred
        public int writerecord(productcastinglist_t st_productcasting)
        {
            int num;
            int index;
            string[] itemName;
			string insertString;
			string connectionString;

			connectionString = "data source = " + gVariable.hostString + "; user id = root; PWD = ; Charset=utf8";
			mySQLClass.getDatabaseInsertStringFromExcel(ref insertString, c_productcastinglistFileName);

            try
            {
                index = 0;
                itemName = insertString.Split(',', ')');

                MySqlConnection myConnection = new MySqlConnection("database = " + c_dbName + "; " + connectionString);
                myConnection.Open();

                MySqlCommand myCommand = myConnection.CreateCommand();

                myCommand.CommandText = "insert into `" + c_productcastinglistTableName + "`" + insertString;

                myCommand.Parameters.AddWithValue("@id", 0);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.machineID);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.barCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.scanTime);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.dispatchCode);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.salesOrderCode);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.batchNum);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.largeIndex);
				myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.workerID);
                myCommand.Parameters.AddWithValue(itemName[index++], st_productcasting.productCode);

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

