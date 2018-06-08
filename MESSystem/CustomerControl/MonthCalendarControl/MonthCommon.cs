using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CustomControl.MonthCalendarControl
{
    class MonthCommon
    {
        public const int GRID_COLUMN_COUNT = 7;
        public const int GRID_ROW_COUNT = 6;

        public const int MONTH_START = 1;
        public const int MONTH_END = 12;

        public const string TIME_START = "0:00";
        public const string TIME_END = "23:30";

        public const string ATTR_INTERNAL_START_TIME = "StartTime";
        public const string ATTR_INTERNAL_END_TIME = "EndTime";
    };

    public enum TimeIntervalType { Start, End, Delete };

    public enum CellOptionStyle { TimeInterval, None };

    public struct CustomDataType
    {
        public string attr;
        public int id;
        // if starttime is in this month, endtime is in the next month, endtime is null
        public string startTime;
        // if starttime is in previous month, endtime is in this month, starttime is null
        public string endTime;
    }

    public struct CustomAttribute
    {
        public Color Color;
        public string Text;
    }

    public interface IGridDataAttributeProvider
    {
        List<CustomDataType> GetProviderDate(string attributeName, int year, int month);
        bool SetSelectedPeroid(string attributeName, int id, string start, string end);
    }
}
