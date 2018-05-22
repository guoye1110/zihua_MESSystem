using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using MESSystem.common;

namespace MESSystem.OEEManagement
{
    public class OEEMachine
    {
        /*************************************************Constant**************************************************/
        private const string DATABASE_PREFIX = "00";

        private const int CASTING_DATABASE_INDEX = 6;
        private const int PRINTER_DATABASE_INDEX = 11;
        private const int CUTTER_DATABASE_INDEX = 14;

        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

        /**************************************************Types*******************************************************/


        /********************************************************Variables*************************************************/
        private string _name;
        private OEETypes.GROUP_TYPE _type;
        private int _id;
        private OEETypes.MachineStatus _status;

        private float plannedOutput;
        private float qualifiedOutput;
        private float realOutput;
        private int maintainanceHours;
        private int prepareHours;

        private List<OEETypes.StatusPoint> statusPoints;
        private DataTable hoursDataTable;
        private DataTable maintainanceDataTable;

        /********************************************Property**************************************************/

        public string Name
        {
            get { return _name; }
        }

        public OEETypes.GROUP_TYPE Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public OEETypes.MachineStatus Status
        {
            get { return _status; }
        }

        /*****************************************Functions*********************************************/
        public OEEMachine(int deviceID, string name, OEETypes.GROUP_TYPE type)
        {
            _id = deviceID;
            _type = type;
            _name = (_id + 1).ToString() + "号" + name;
            _status = new OEETypes.MachineStatus()
            {
                outputPlanned = 0,
                outputQualified = 0,
               // statusPoints = new List<OEETypes.StatusPoint>(),
               // hoursDataTable = new DataTable()
            };
            

            plannedOutput = 0;
            qualifiedOutput = 0;
            maintainanceHours = 0;
            prepareHours = 0;

            statusPoints = new List<OEETypes.StatusPoint>();
            hoursDataTable = new DataTable();
            DataColumn dc1 = new DataColumn("time");
            dc1.DataType = typeof(System.String);
            DataColumn dc2 = new DataColumn("value1");
            dc2.DataType = typeof(System.String);
            DataColumn dc3 = new DataColumn("value2");
            dc2.DataType = typeof(System.String);
            hoursDataTable.Columns.Add(dc1);
            hoursDataTable.Columns.Add(dc2);
            hoursDataTable.Columns.Add(dc3);

            maintainanceDataTable = new DataTable();
            DataColumn dc4 = new DataColumn("planTime1");
            dc4.DataType = typeof(System.String);
            DataColumn dc5 = new DataColumn("planTime2");
            dc5.DataType = typeof(System.String);
            maintainanceDataTable.Columns.Add(dc4);
            maintainanceDataTable.Columns.Add(dc5);

        }

        /// <summary>
        /// 查询设备产能及设备状态变化信息
        /// </summary>
        /// <param name="startTime">查询起始日期</param>
        /// <param name="endTime">查询终止日期</param>
        public void QueryMachineStatus(DateTime startTime, DateTime endTime)
        {
            statusPoints.Clear();
            QueryCapacity(startTime, endTime);
            QueryHours(startTime, endTime);
            QueryMaintaincePeriod(startTime, endTime);
            QueryPrepareTime(startTime, endTime);
            _status.outputPlanned = plannedOutput;
            _status.outputQualified = qualifiedOutput;
            _status.outputReal = realOutput;
            _status.maintainanceHours = maintainanceHours;
            _status.prepareHours = prepareHours;
            _status.statusPoints = statusPoints;
            _status.hoursDataTable = hoursDataTable;
            _status.maintainanceTable = maintainanceDataTable;
        }

        public int QueryEnergyConsumption(DateTime startTime, DateTime endTime, ref List<OEETypes.EnergyConsumption> energyList)
        {
            gVariable.dispatchSheetStruct[] dispatches;
            dispatches = QueryStartedDispatches(startTime, endTime);
            int powerConsumed = 0;
            try
            {
                foreach (gVariable.dispatchSheetStruct dispatch in dispatches)
                {
                    string dispatchCode = dispatch.dispatchCode;
                    powerConsumed += QueryPowerConsumed(dispatchCode, ref energyList);
                }
            }
            catch (Exception)
            {

            }

            return powerConsumed;
        }


        private gVariable.dispatchSheetStruct[] QueryStartedDispatches(DateTime startTime, DateTime endTime)
        {
            gVariable.dispatchSheetStruct[] dispatches;
            string dataBaseName = getDeviceDatabaseName();
            DateTime queryStartTime = OEEUtils.ConvertToStartOfDay(startTime);
            DateTime queryEndTime = OEEUtils.ConvertToEndOfDay(endTime);

            if (null != dataBaseName)
                dispatches = mySQLClass.getDispatchListInPeriodOfTime(dataBaseName, gVariable.dispatchListTableName,
                queryStartTime.ToString(OEETypes.QUERY_TIME_FORMAT), queryEndTime.ToString(OEETypes.QUERY_TIME_FORMAT),
                gVariable.MACHINE_STATUS_DISPATCH_STARTED, gVariable.TIME_CHECK_TYPE_REAL_START);
            else
                dispatches = null;
            return dispatches;
        }

        private int QueryPowerConsumed(string dispatchCode, ref List<OEETypes.EnergyConsumption> energyList)
        {
            int retVal = 0;
            string dataBaseName = getDeviceDatabaseName();
            DataTable dt = new DataTable();
            string commandText = "select * from `" + gVariable.materialListTableName +
                "` where dispatchCode = '" + dispatchCode + "'";
            dt = mySQLClass.queryDataTableAction(dataBaseName, commandText, null);
            for (int row = 0; row < dt.Rows.Count; row++)
            {
                OEETypes.EnergyConsumption energy;
                energy.dispatchCode = dt.Rows[row][1].ToString();
                energy.workingTime = Convert.ToInt32(dt.Rows[row][5].ToString());
                energy.prepareTime = Convert.ToInt32(dt.Rows[row][6].ToString());
                energy.standbyTime = Convert.ToInt32(dt.Rows[row][7].ToString());
                energy.power = Convert.ToInt32(dt.Rows[row][8].ToString());
                energy.powerConsumed = Convert.ToInt32(dt.Rows[row][9].ToString());
                retVal += energy.powerConsumed * OEETypes.POWER_VOLTAGE;

                energyList.Add(energy);
            }
            return retVal;
        }

        private void QueryCapacity(DateTime startTime, DateTime endTime)
        {
            string dataBaseName = getDeviceDatabaseName();
            DateTime queryStartTime = OEEUtils.ConvertToStartOfDay(startTime);
            DateTime queryEndTime = OEEUtils.ConvertToEndOfDay(endTime);

            gVariable.dispatchSheetStruct[] dispatches;

            if (null != dataBaseName)
                dispatches = mySQLClass.getDispatchListInPeriodOfTime(dataBaseName, gVariable.dispatchListTableName,
                    queryStartTime.ToString(OEETypes.QUERY_TIME_FORMAT), queryEndTime.ToString(OEETypes.QUERY_TIME_FORMAT),
                    gVariable.MACHINE_STATUS_DISPATCH_COMPLETED, gVariable.TIME_CHECK_TYPE_PLANNED_START);
            else
                dispatches = null;

            if (dispatches != null)
                CalculateCapacity(dispatches);
        }

        // Query machineStatusRecord
        private void QueryHours(DateTime startTime, DateTime endTime)
        {
           
            DateTime queryStartTime = OEEUtils.ConvertToStartOfDay(startTime);
            DateTime queryEndTime = OEEUtils.ConvertToEndOfDay(endTime);
            string dataBaseName = getDeviceDatabaseName();
            DateTime startTimePoint = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            System.Console.WriteLine("database name: {0}", this.Name);
            double startTime_offset = (queryStartTime - startTimePoint).TotalSeconds;
            double endTime_offset = (queryEndTime - startTimePoint).TotalSeconds;

            string commandText = "select * from " + "`" + gVariable.machineStatusRecordTableName + "`" +
                    " where time >= '" + startTime_offset + "' and time <= '" + endTime_offset + "'";

           hoursDataTable = mySQLClass.queryDataTableAction(dataBaseName, commandText, null);
           CalculateStatus(hoursDataTable);
        }

        private void QueryMaintaincePeriod(DateTime startTime, DateTime endTime)
        {
            DateTime queryStartTime = OEEUtils.ConvertToStartOfDay(startTime);
            DateTime queryEndTime = OEEUtils.ConvertToEndOfDay(endTime);
            string dataBaseName = getDeviceDatabaseName();
          

            string commandText = "select planTime1, planTime2 from " + "`" + gVariable.machineWorkingPlanTableName + "`" +
                    " where (typeID >= 2 and ((planTime1 < '" + queryStartTime + "' and planTime2 >= '" + queryStartTime + "')" +
                    " or (planTime1 <= '" + queryStartTime + "' and planTime2 <= '" + queryEndTime + "')" +
                    " or (planTime2 <= '" + queryEndTime + "' and planTime2 > '" + queryEndTime + "')))";

            maintainanceDataTable = mySQLClass.queryDataTableAction(dataBaseName, commandText, null);
            CalculateMaintainance(maintainanceDataTable, queryStartTime, queryEndTime);
        }

        private void QueryPrepareTime(DateTime startTime, DateTime endTime)
        {
            DateTime queryStartTime = OEEUtils.ConvertToStartOfDay(startTime);
            DateTime queryEndTime = OEEUtils.ConvertToEndOfDay(endTime);
            string dataBaseName = getDeviceDatabaseName();
            DataTable prepareDataTable = new DataTable();

            string commandText = "select startTime, prepareTimePoint from " + "`" + gVariable.dispatchListTableName + "`" +
                    " where (prepareTimePoint >= '" + queryStartTime + "' and startTime <= '" + queryEndTime + "')";
            prepareDataTable = mySQLClass.queryDataTableAction(dataBaseName, commandText, null);
            CalculatePrepareTime(prepareDataTable, queryStartTime, queryEndTime);
        }

        private void CalculateCapacity(gVariable.dispatchSheetStruct[] completedispatches)
        {
            plannedOutput = 0;
            qualifiedOutput = 0;
            realOutput = 0;

            try
            {
                foreach (gVariable.dispatchSheetStruct dispatch in completedispatches)
                {
                    plannedOutput += dispatch.plannedNumber;
                    qualifiedOutput += dispatch.qualifiedNumber;
                    realOutput += (dispatch.qualifiedNumber + dispatch.unqualifiedNumber);

                    System.Console.WriteLine("machineID: {0}, planTimeStart: {1}, planTimeEnd: {2}, status: {3}," +
                        "plannedNumber: {4}, qualifiedNumber: {5}",
                        dispatch.machineID, dispatch.planTime1, dispatch.planTime2, dispatch.status,
                        dispatch.outputNumber, dispatch.qualifiedNumber);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CalculateCapacity failed" + ex);
            }
        }

        private void CalculateStatus(DataTable dt)
        {
            //const int FIELD_ID = 0;
            const int FIELD_TIME = 1;
            const int FIELD_VALUE1 = 2;
            // const int FIELD_VALUE2 = 3;

            int oldStatus = OEETypes.INVALID_MACHINE_STATUS;

            object timePoint, status;
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    status = row[FIELD_VALUE1];
                    timePoint = row[FIELD_TIME];
                    System.Console.WriteLine("status: {0}, timePoint {1}", status, timePoint);
                    if (StatusChanged(status, ref oldStatus))
                        RecordStatus(status, timePoint);
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CalculateStatus failed " + ex);
            }
        }

        private bool StatusChanged(object status, ref int oldStatus)
        {
            int nowStatus = Convert.ToInt32(status);

            if (nowStatus != oldStatus)
            {
                oldStatus = nowStatus;
                return true;
            }
            else
                return false;

        }

        private void RecordStatus(object status, object statusPoint)
        {
            OEETypes.StatusPoint item;
            DateTime startPoint = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime;
            TimeSpan toNow;

            item.status = Convert.ToInt32(status);
            lTime = long.Parse(statusPoint.ToString() + "0000000");
            toNow = new TimeSpan(lTime);
            item.time = startPoint.Add(toNow);
            System.Console.WriteLine("status: {0}, dateTime: {1}", item.status, item.time);
            statusPoints.Add(item);
        }

        private void CalculateMaintainance(DataTable dt, DateTime startTime, DateTime endTime)
        {
            DateTime timestamp1, timestamp2;
            DateTime startPoint, endPoint;
            TimeSpan timespan = new TimeSpan();
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    timestamp1 = Convert.ToDateTime(row["timestamp1"]);
                    timestamp2 = Convert.ToDateTime(row["timestamp2"]);
                    if (timestamp1 < startTime)
                        startPoint = startTime;
                    else
                        startPoint = timestamp1;
                    if (timestamp2 >= endTime)
                        endPoint = endTime;
                    else
                        endPoint = endTime;
                    timespan += endPoint.Subtract(startPoint);
                }
                maintainanceHours = (int)Math.Ceiling(timespan.TotalHours);
            }
            catch (Exception)
            {

            }
        }

        private void CalculatePrepareTime(DataTable prepareDataTable, DateTime startTime, DateTime endTime)
        {
            DateTime startPoint;
            DateTime endPoint;
            TimeSpan timespan = new TimeSpan();

            prepareHours = 0;
            try
            {
                foreach (DataRow row in prepareDataTable.Rows)
                {
                    DateTime realStart = Convert.ToDateTime(row[0]);
                    DateTime preparePoint = Convert.ToDateTime(row[1]);
                    if (realStart < startTime)
                        startPoint = startTime;
                    else
                        startPoint = realStart;
                    if (preparePoint > endTime)
                        endPoint = endTime;
                    else
                        endPoint = preparePoint;
                    timespan += endPoint.Subtract(startPoint);
                }
                prepareHours = (int)Math.Ceiling(timespan.TotalHours);
            }
            catch (Exception)
            {

            }
        }

        public string getDeviceDatabaseName()
        {
            string dataBaseName;
            int tableIndex;

            switch (_type)
            {
                case OEETypes.GROUP_TYPE.CASTING:
                    tableIndex = _id + CASTING_DATABASE_INDEX;
                    break;
                case OEETypes.GROUP_TYPE.CUTTER:
                    tableIndex = _id + CUTTER_DATABASE_INDEX;
                    break;
                case OEETypes.GROUP_TYPE.PRINTER:
                    tableIndex = _id + PRINTER_DATABASE_INDEX;
                    break;
                default:
                    _name = "";
                    return null;
            }

            //dataBaseName = gVariable.DBHeadString + DATABASE_PREFIX + tableIndex.ToString();
            dataBaseName = gVariable.DBHeadString + (tableIndex + 1).ToString().PadLeft(3, '0');
            return dataBaseName;
        }
    }

}
