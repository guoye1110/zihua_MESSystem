using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MESSystem.OEEManagement
{
    public class OEETypes
    {
        public enum GROUP_TYPE { CASTING, PRINTER, CUTTER };

        public const string FONT = "Microsoft Sans Serif";
        public const int INVALID_MACHINE_STATUS = -1;
        public const int END_OF_DAY_HOUR = 23;
        public const int END_OF_DAY_MINUTE = 59;
        public const int END_OF_DAY_SECOND = 59;

        public const int YEAR_START = 2018;
        public const int YEART_END = 2038;

        public const int MONTH_START = 1;
        public const int MONTH_END = 12;
        public const int MONTH_LEAP = 2;

        public const int DAY_START = 1;
        public const int DAY_SOLAR_END = 31;
        public const int DAY_LUNAR_END = 30;
        public const int DAY_LEAP_END = 29;
        public const int DAY_NOLEAP_END = 28;

        public const int POWER_VOLTAGE = 220;
        public const string QUERY_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        public struct StatusPoint
        {
           public DateTime time;
           public int status;
        };

        public struct MachineStatus
        {
            //public string name;
            public float outputPlanned;   //计划产量
            public float outputQualified; //合格产量
            public float outputReal;      //实际产量
            public int maintainanceHours; //保养时间
            public int prepareHours;    //调整时间
            public List<StatusPoint> statusPoints;
            public DataTable hoursDataTable;
            public DataTable maintainanceTable;
        }

        public struct OutputInHours
        {
            public TimeSpan hours;
            public int output;
        }
        
        public struct EnergyConsumption
        {
            public string dispatchCode;
            //public int totalWorkingTime;
           // public int collectedNumber;
           // public int productBeat;
            public int workingTime;
            public int prepareTime;
            public int standbyTime;
            public int power;
            public int powerConsumed;
        }
    }
}
