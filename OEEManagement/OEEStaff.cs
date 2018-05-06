using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using MESSystem.common;
namespace MESSystem.OEEManagement
{
    public class OEEStaff
    {
        /**************************************************** Constant ****************************************************/

        /***************************************************** Types ******************************************************/


        /********************************************************Variables*************************************************/
        private string _name;
        private string _id;
        private string _workshop;

        //gVariable.dispatchSheetStruct[] dispatches;
        /***************************************************** Property ***************************************************/
        public string Name
        {
            get { return _name; }
        }

        public string Workshop
        {
            get { return _workshop; }
        }

        public string ID
        {
            get { return _id; }
        }
        /***************************************************** Functions ***************************************************/
        public OEEStaff(string id, string name, string workshop)
        {
            _id = id;
            _name = name;
            _workshop = workshop;
        }


        public OEETypes.OutputInHours QueryLabourHour(DateTime startTime, DateTime endTime, OEEFactory factory)
        {
            DateTime queryStartTime = OEEUtils.ConvertToStartOfDay(startTime);
            DateTime queryEndTime = OEEUtils.ConvertToEndOfDay(endTime);
            OEEMachineGroup[] machineGroups = factory.MachineGroups;
            DataTable dt = null;

            for (int groupIndex = 0; groupIndex < machineGroups.Count(); groupIndex++)
            {
                foreach (OEEMachine machine in machineGroups[groupIndex].Machines)
                {
                    DataTable temp = new DataTable();
                    string dataBaseName = machine.getDeviceDatabaseName();
                    string commandText = "select startTime, completeTime, qualifyNum, unqualifyNum from " + "`" + gVariable.dispatchListTableName + "`" +
                        " where completeTime >= '" + queryStartTime.ToString(OEETypes.QUERY_TIME_FORMAT) + "' and completeTime <= '" + queryEndTime.ToString(OEETypes.QUERY_TIME_FORMAT) + "' and reportor = '" + _id + "'";
                    temp = mySQLClass.queryDataTableAction(dataBaseName, commandText, null);

                    try
                    {
                        object[] obj = new object[temp.Columns.Count];
                        if (dt == null)
                            dt = temp.Clone();
                        for (int i = 0; i < temp.Rows.Count; i++)
                        {
                            temp.Rows[i].ItemArray.CopyTo(obj, 0);
                            dt.Rows.Add(obj);
                        }
                    }
                    catch (Exception)
                    {

                    }

                }
            }

            return CalculateLabourHour(dt);
        }

        private OEETypes.OutputInHours CalculateLabourHour(DataTable dt)
        {
            OEETypes.OutputInHours monthStatus;
            TimeSpan span = new TimeSpan();
            int totalNum = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DateTime startTime = Convert.ToDateTime(dt.Rows[i][0].ToString());
                DateTime endTime = Convert.ToDateTime(dt.Rows[i][1].ToString());
                span += endTime.Subtract(startTime);
                int qualifyNum = Convert.ToInt32(dt.Rows[i][2].ToString());
                int unqualifyNum = Convert.ToInt32(dt.Rows[i][3].ToString());
                totalNum += qualifyNum + qualifyNum;
            }

            monthStatus.hours = span;
            monthStatus.output = totalNum;

            return monthStatus;

        }

        public DataTable QueryLabourHourDetails(int year, int month, string name, OEEFactory factory)
        {
            DateTime queryStartTime = OEEUtils.ConvertToDayStartOfMonth(year, month);
            DateTime queryEndTime = OEEUtils.ConvertToDayEndOfMonth(year, month);
            OEEMachineGroup[] machineGroups = factory.MachineGroups;
            DataTable dt = null;

            for (int groupIndex = 0; groupIndex < machineGroups.Count(); groupIndex++)
            {
                foreach (OEEMachine machine in machineGroups[groupIndex].Machines)
                {
                    DataTable temp = new DataTable();
                    string dataBaseName = machine.getDeviceDatabaseName();
                    string commandText = "select dispatchCode," +
                                                "productCode," +
                                                "productName," +
                                                "processName," +
                                                "serialNumber," +
                                                "startTime," +
                                                "completeTime," +
                                                "qualifyNum, " +
                                                "unqualifyNum," +
                                                "machineID from " +
                                                "`" + gVariable.dispatchListTableName + "`" +
                                                " where completeTime >= '" + queryStartTime.ToString(OEETypes.QUERY_TIME_FORMAT) +
                                                "' and completeTime <= '" + queryEndTime.ToString(OEETypes.QUERY_TIME_FORMAT) +
                                                "' and reportor = '" + _id + "'";
                    temp = mySQLClass.queryDataTableAction(dataBaseName, commandText, null);

                    try
                    {
                        object[] obj = new object[temp.Columns.Count];
                        if (dt == null)
                            dt = temp.Clone();
                        for (int i = 0; i < temp.Rows.Count; i++)
                        {
                            temp.Rows[i].ItemArray.CopyTo(obj, 0);
                            dt.Rows.Add(obj);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            return dt;
        }
    }
}
