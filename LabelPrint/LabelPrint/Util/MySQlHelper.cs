using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace LabelPrint.Util
{
    public partial class mySQLClass
    {
        // perform query function
        // return: DataTable
        public static DataTable queryDataTableAction(string databaseName, string commandText, MySqlParameter[] param)
        {
            MySqlConnection myConnection;

            myConnection = null;
            try
            {
                myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                MySqlCommand myCommand = myConnection.CreateCommand();
                myCommand.CommandText = commandText;

                if(param != null)
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

        // perform insert / update / delete function
        // return: if this is an append action, we need to return ID of the last appended record
        //         otherwise 0 means OK
        //                  -1 means failed
        public static int databaseNonQueryAction(string databaseName, string commandText, MySqlParameter[] param, int appendOrNot)
        {
            int ret;
            MySqlConnection myConnection;

            ret = 0;
            myConnection = null;
            try
            {
                myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();
                MySqlCommand myCommand = myConnection.CreateCommand();
                myCommand.CommandText = commandText;
                myCommand.Parameters.AddRange(param);
                myCommand.ExecuteNonQuery();

                if (appendOrNot == APPEND_RECORD)   //we added new record to the database table
                    ret = (int)myCommand.LastInsertedId;

                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("databaseNonQueryAction(): database " + databaseName + ": " + commandText + " failed!" + ex);

                if (myConnection != null)
                    myConnection.Close();

                ret = -1;
            }
            return ret;
        }

        //get table ID list for all records ID between id1 and id2
        //we can also use this function to get number of records between id1 and id2, the result is idList.Length
        public static List<int> queryRecordIDToArray(string databaseName, string commandText, int total, int id1, int id2)
        {
            int i, j;
            List<int> idList = new List<int>();
            MySqlConnection myConnection;
            MySqlDataReader myReader;

            myConnection = null;
            myReader = null;
            try
            {
                myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();
                MySqlCommand myCommand = myConnection.CreateCommand();
                myCommand.CommandText = commandText;
                myReader = myCommand.ExecuteReader();
                if (myReader != null)
                {
                    i = 0;
                    j = 0;
                    while (myReader.Read())
                    {
                        if (i >= id1)
                        {
                            idList.Add(myReader.GetInt32(0));
                            j++;
                        }
                        i++;
                        if (i >= id2)
                            break;

                        if (j > total)
                            break;
                    }
                    myReader.Close();
                }
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("queryRecordIDToArray(): database " + databaseName + ": " + commandText + " failed!" + ex);

                if(myReader != null)
                    myReader.Close();

                if (myConnection != null)
                    myConnection.Close();

                return null;
            }
            return idList;
        }


        //some special conditions are included in commandText, so we cannot use 
        //an example of the commandText is "select * from table1 where id > 20 and time > 1027-09-09"
        public static int queryRecordNumAction(string databaseName, string commandText)
        {
            int num;
            MySqlConnection myConnection;
            MySqlDataReader myReader;

            myConnection = null;
            try
            {
                myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();
                MySqlCommand myCommand = myConnection.CreateCommand();
                myCommand.CommandText = commandText;
                myReader = myCommand.ExecuteReader();
                num = 0;
                while (myReader.Read())
                    num++;

                myReader.Close();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("queryRecordNumAction(): database " + databaseName + ": " + commandText + " failed!" + ex);

                if (myConnection != null)
                    myConnection.Close();

                num = 0;
            }
            return num;
        }

        //there is no condition in commandText
        //an example of the commandText is "select * from table1"
        public static int queryRecordNumSimple(string databaseName, string commandText)
        {
            int ret;
            MySqlConnection myConnection;

            ret = 0;
            myConnection = null;
            try
            {
                myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);
                myConnection.Open();
                MySqlCommand myCommand = myConnection.CreateCommand();
                myCommand.CommandText = commandText;
                ret = Convert.ToInt32(myCommand.ExecuteScalar());
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("queryRecordNumAction(): database " + databaseName + ": " + commandText + " failed!" + ex);

                if (myConnection != null)
                    myConnection.Close();

                ret = -1;
            }
            return ret;
        }


        public static  int ExcuteNonQuery(string databaseName, string commandText, MySqlParameter[] param)
        {
            int ret; 
            MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = myConnection;
            myCommand.CommandText = commandText;
            if (param!=null)
            myCommand.Parameters.AddRange(param);
           // myCommand.CommandType = CommandType.StoredProcedure;
            try
            {
                myCommand.Connection.Open();
                myCommand.ExecuteNonQuery();
                ret = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(" " + ex);
                ret = -1;
            }
            finally
            {
                myCommand.Connection.Close();
            }
            return ret;
        }


        // return: true or false
        public int BatchDelete(string databaseName, string commandText, MySqlParameter[] param)
        {
            int result;
            int ret;

            MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = myConnection;
            myCommand.CommandText = commandText;
            myCommand.Parameters.AddRange(param);
            myCommand.Parameters.Add(new MySqlParameter("ReturnValue", MySqlDbType.Int32, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));

            try
            {
                myCommand.Connection.Open();
                myCommand.ExecuteNonQuery();
                result = (int)myCommand.Parameters["ReturnValue"].Value;
                if (result > 0)
                    ret = -1;
                else
                    ret = 0;
            }
            catch
            {
                ret = -1;
            }
            finally
            {
                myCommand.Connection.Close();
            }
            return ret;
        }


        //get all information about this user account, could be used to confirm whether this user has input a correct password
        public static DataTable getDataTableForAccount(string userAccount)
        {
            int flag;
            DataTable dt;
            string commandText;

            dt = null;
            try
            {
                commandText = "select * from `" + gVariable.employeeTableName + "` where workerID = " + "\'" + userAccount + "\'";
                dt = mySQLClass.queryDataTableAction(gVariable.basicInfoDatabaseName, commandText, null);

                flag = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    flag = 1;
                    break;
                }

                //workID not found, so try name
                if (flag == 0)
                {
                    commandText = "select * from `" + gVariable.employeeTableName + "` where name = " + "\'" + userAccount + "\'";
                    dt = mySQLClass.queryDataTableAction(gVariable.basicInfoDatabaseName, commandText, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("getDataTableForAccount failed!" + ex);
            }
            return dt;
        }

        //employee table column index definition
        public const int EMPLOYEE_ID_COLIUMN = 0;
        public const int EMPLOYEE_WORKERID_COLIUMN = 1;
        public const int EMPLOYEE_NAME_COLIUMN = 2;
        public const int EMPLOYEE_SEX_COLIUMN = 3;
        public const int EMPLOYEE_AGE_COLIUMN = 4;
        public const int EMPLOYEE_POSTIOIN_COLIUMN = 5;
        public const int EMPLOYEE_DEPARTMENT_COLIUMN = 6;
        public const int EMPLOYEE_DATE_COLIUMN = 7;
        public const int EMPLOYEE_PASSWORD_COLIUMN = 8;
        public const int EMPLOYEE_RANK_COLIUMN = 9;
        public const int EMPLOYEE_PRIVILEGE1_COLIUMN = 10;
        public const int EMPLOYEE_PRIVILEGE2_COLIUMN = 11;
        public const int EMPLOYEE_PRIVILEGE3_COLIUMN = 12;
        public const int EMPLOYEE_PRIVILEGE4_COLIUMN = 13;
        public const int EMPLOYEE_MAIL_ADDR_COLIUMN = 14;
    }
}
