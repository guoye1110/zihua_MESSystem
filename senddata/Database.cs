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
using System.Threading;

namespace tcpClient
{
    public class mySQLClass
    {
        public static mySQLClass mySQL;

        public static string connectionString;

        //basic infomation table, including employee info and production info
        const int infoTableNum = 2;


        public mySQLClass()
        {
            MySqlConnection myConnection;
 
            connectionString = "data source = " + Form1.HostIP + "; user id = root; PWD = ; Charset=utf8";

            try
            {
                myConnection = new MySqlConnection(connectionString);
                myConnection.Open();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("抱歉，请确认 wamp server 已启动，否则本系统无法运行。", "信息提示", MessageBoxButtons.OK);
                System.Environment.Exit(0);
            }
        }

        public static string [] readDataFromDatabase(string databaseName, string tableName, int ID)
        {
            int j;
            string commandText;
            string[] tableArray;
            DataTable dTable;

            try
            {
                commandText = "select * from `" + tableName + "` where id = " + ID;

                tableArray = null;
                dTable = queryDataTableAction(databaseName, commandText, null);
                if (dTable != null && dTable.Rows.Count != 0)
                {
                    j = 0;

                    tableArray = new string[dTable.Rows[0].ItemArray.Length];
                    for (j = 0; j < dTable.Rows[0].ItemArray.Length; j++)
                    {
                        tableArray[j] = dTable.Rows[0].ItemArray[j].ToString();
                    }
                }
                return tableArray;
            }
            catch (Exception ex)
            {
                Console.WriteLine("readDataFromDatabase failed!" + ex);
            }
            return null;
        }

        public static DataTable queryDataTableAction(string databaseName, string commandText, MySqlParameter[] param)
        {
            MySqlConnection myConnection;

            myConnection = null;
            try
            {
                myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                MySqlCommand myCommand = myConnection.CreateCommand();
                myCommand.CommandText = commandText;

                if (param != null)
                    myCommand.Parameters.AddRange(param);

                MySqlDataAdapter sda = new MySqlDataAdapter(myCommand);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("queryDataTableAction(): database " + databaseName + ": " + commandText + " failed!" + ex);

                if (myConnection != null)
                    myConnection.Close();

                return null;
            }
        }
    }
}
