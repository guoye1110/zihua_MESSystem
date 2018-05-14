using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
namespace LabelPrint.Util
{
    public class ExcelHelper
    {
        const int LEN_1 = 1;
        const int LEN_4 = 4;
        const int LEN_8 = 8;
        const int LEN_40 = 40;
        const int LEN_100 = 100;
        const int LEN_200 = 200;

        const int EXCEL_FIRSTLINE_DATA = 0;  //read every line to data table
        const int EXCEL_FIRSTLINE_TITLE = 1; //so we will not read first line into table

        const int DECREASE_ORDER = 0;  //output in decreasing order 
        const int INCREASE_ORDER = 1;  //output in increasing order 

        //read data from excel into standard data table class
        //input:  fileUrl: file path + file name
        //      firstLine: EXCEL_FIRSTLINE_DATA   -- the first line of excel file is data
        //                 EXCEL_FIRSTLINE_TITLE  -- the first line of excel file is title, should not be put in database  
        //return: DataTable
        public static DataTable readExcelToDataTable(string fileUrl, int firstLine)
        {
            string cmdText;

            //            const string cmdText = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1';";
            //suport both xls and xlsx, HDR=Yes means first line is titel not data
            if (firstLine == EXCEL_FIRSTLINE_DATA)
                cmdText = "Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=No; IMEX=1'";
            else //if(firstLine == EXCEL_FIRSTLINE_TITLE)
                cmdText = "Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";

            DataTable dt = null;
            OleDbDataAdapter da;
            OleDbConnection conn;

            conn = new OleDbConnection(string.Format(cmdText, fileUrl));
            try
            {
                if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                System.Data.DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                string sheetName = schemaTable.Rows[0]["TABLE_NAME"].ToString().Trim();

                string strSql = "select * from [" + sheetName + "]";
                da = new OleDbDataAdapter(strSql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dt = ds.Tables[0];
                da.Dispose();

                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("readExcelToDataTable() fail for :" + fileUrl + "! " + ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return null;
        }
    }
}
