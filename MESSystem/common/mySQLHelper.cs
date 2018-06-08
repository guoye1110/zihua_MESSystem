using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

using MySql.Data.MySqlClient;

namespace MESSystem.common
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
        // return: 0 / -1
        public static int pureDatabaseNonQueryAction(string databaseName, string commandText)
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
                ret = myCommand.ExecuteNonQuery();

                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("pureDatabaseNonQueryAction(): database " + databaseName + ": " + commandText + " failed!" + ex);

                if (myConnection != null)
                    myConnection.Close();

                ret = -1;
            }
            return ret;
        }

        // perform insert / update / delete function
        // return: 0 / -1
        public static int redoIDIncreamentAfterRecordDeleted(string databaseName, string tableName)
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

                myCommand.CommandText = "ALTER TABLE `" + databaseName + "`.`" + tableName + "` DROP id";
                myCommand.ExecuteNonQuery();

                myCommand.CommandText = "ALTER TABLE `" + databaseName + "`.`" + tableName + "` ADD id INT NOT NULL PRIMARY KEY AUTO_INCREMENT FIRST";
                myCommand.ExecuteNonQuery();

                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("redoIDIncreamentAfterRecordDeleted() for " + databaseName + "." + tableName + " failed!" + ex);

                if (myConnection != null)
                    myConnection.Close();

                ret = -1;
            }
            return ret;
        }


        // perform insert / update / delete function
        // return: true / false, and if this is an append action, we need to return ID of the last appended record
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
                //                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Parameters.AddRange(param);
                myCommand.ExecuteNonQuery();

                if (appendOrNot == gVariable.appendRecord)
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

        //
        public static List<int> queryRecordIDToArray(string databaseName, string commandText, int total, int id1, int id2)
        {
            int i, j;
            List<int> idList = new List<int>();
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
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("queryRecordIDToArray(): database " + databaseName + ": " + commandText + " failed!" + ex);

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


        public int ExcuteNonQuery(string databaseName, string commandText, MySqlParameter[] param)
        {
            int ret; 
            MySqlConnection myConnection = new MySqlConnection("database = " + databaseName + "; " + connectionString);

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = myConnection;
            myCommand.CommandText = commandText;
            myCommand.Parameters.AddRange(param);

            try
            {
                myCommand.Connection.Open();
                myCommand.ExecuteNonQuery();
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
    }
}
