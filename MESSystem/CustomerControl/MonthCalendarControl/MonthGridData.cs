using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomControl.MonthCalendarControl
{
    class MonthGridData
    {
        int _year;
        int _month;

        int _days;

        int _index;

        private DateTime today;
        private List<string> attrNameList;
        private DataTable _dt;
        private DataTable _dtAttr;
        private List<CustomDataType> _customData;



        public int Index
        {
            get { return _index; }
        }

        public int Year
        {
            set { _year = value; }
            get { return _year; }
        }

        public int Month
        {
            set
            {
                if (value > MonthCommon.MONTH_END)
                {
                    _month = MonthCommon.MONTH_START;
                    _year++;
                }
                else if (value < MonthCommon.MONTH_START)
                {
                    _month = MonthCommon.MONTH_END;
                    _year--;
                }
                else
                    _month = value;
                UpdateDaysInMonth();
            }
            get { return _month; }
        }

        public int Days
        {
            get { return _days; }
        }

        public DataTable Data
        {
            get { return _dt; }
        }

        public DataTable DataAttribute
        {
            get { return _dtAttr; }
        }


        public List<CustomDataType> CustomData
        {
            set
            {
                _customData = value;
                UpdateExternalDateAttribute();
            }
            get { return _customData; }
        }

        public delegate void UpdateMonthInfo(object sender, int index);
        public event UpdateMonthInfo OnPropertyChanged;

        public MonthGridData(int index, List<string> attr)
        {
            _year = 0;
            _month = 0;
            _index = index;

            attrNameList = attr;


            SetToday();
            _dt = new DataTable();
            _dtAttr = new DataTable();
            for (int i = 0; i < MonthCommon.GRID_COLUMN_COUNT; i++)
                _dt.Columns.Add(new DataColumn());
            for (int i = 0; i < attrNameList.Count; i++)
            {
                DataColumn column = new DataColumn();
                column.ColumnName = attrNameList[i];
                column.DataType = System.Type.GetType("System.Boolean");
                _dtAttr.Columns.Add(column);
            }
            _dtAttr.Columns.Add(new DataColumn("StartTime", System.Type.GetType("System.String")));
            _dtAttr.Columns.Add(new DataColumn("EndTime", System.Type.GetType("System.String")));
            _dtAttr.Columns.Add(new DataColumn("id", System.Type.GetType("System.Int32")));

        }

        public void UpdateAttribute(int day, string attr, bool value)
        {
            _dtAttr.Rows[day][attr] = value;
        }

        private void SetToday()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            today = new DateTime(year, month, day);
        }

        private void UpdateDaysInMonth()
        {
            int startCellIndex = 0;
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            Calendar calendar = cultureInfo.Calendar;
            DateTime startDate = new DateTime(_year, _month, 1, calendar);
            DayOfWeek startDay = calendar.GetDayOfWeek(startDate);

            _dt.Clear();
            _dtAttr.Clear();

            // Add a blank row for _dtAttr
            _dtAttr.Rows.Add(_dtAttr.NewRow());

            _days = calendar.GetDaysInMonth(_year, _month);

            if (startDay != DayOfWeek.Sunday)
                startCellIndex = (int)startDay - 1;
            else
                startCellIndex = 6;

            int cellCount = MonthCommon.GRID_COLUMN_COUNT * MonthCommon.GRID_ROW_COUNT;
            string[] values = new string[MonthCommon.GRID_COLUMN_COUNT];
            int date = 1;

            int col = 0;
            for (int i = 0; i < cellCount; i++)
            {
                if (i >= startCellIndex && i < startCellIndex + _days)
                {
                    values[col] = date.ToString();
                    UpdateInternalDateAttribute(date);
                    date++;
                }
                else
                    values[col] = ""; // Need to be set null here

                col++;
                if (((i + 1) % (MonthCommon.GRID_COLUMN_COUNT) == 0) && (i >= MonthCommon.GRID_COLUMN_COUNT - 1) && (i <= cellCount))
                {
                    col = 0;
                    _dt.Rows.Add(values);
                }

            }

            OnPropertyChanged(this, _index);
        }

        private void UpdateInternalDateAttribute(int date)
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            Calendar calendar = cultureInfo.Calendar;

            DateTime current = new DateTime(_year, _month, date);
            DataRow row;
            row = _dtAttr.NewRow();
            for (int i = 0; i < attrNameList.Count; i++)
            {
                string str = attrNameList[i];

                switch (str)
                {
                    case "Today":
                        if (current == today)
                            row["Today"] = true; // "Today"
                        else
                            row["Today"] = false;
                        break;
                    case "Holiday":
                        DayOfWeek dayOfWeek = calendar.GetDayOfWeek(current);
                        if ((dayOfWeek == DayOfWeek.Saturday) ||
                                (dayOfWeek == DayOfWeek.Sunday))
                            row["Holiday"] = true;  // "Holiday"
                        else
                            row["Holiday"] = false;
                        break;
                    default:
                        row[str] = false;
                        break;
                }
            }
            row["StartTime"] = "";
            row["EndTime"] = "";
            row["id"] = -1;
            _dtAttr.Rows.Add(row);
        }

        private void UpdateExternalDateAttribute()
        {
            Dictionary<int, CustomDataType> customDataInDay = new Dictionary<int, CustomDataType>();
            DateTime startTime, endTime;
            int startDay, endDay;
            for (int i = 0; i < _customData.Count; i++)
            {
                if ((_customData[i].startTime != "") && (_customData[i].endTime != ""))
                {
                    startTime = Convert.ToDateTime(_customData[i].startTime);
                    endTime = Convert.ToDateTime(_customData[i].endTime);
                    startDay = startTime.Day;
                    endDay = endTime.Day;

                    for (int day = startDay; day <= endDay; day++)
                    {
                        CustomDataType data = new CustomDataType();
                        data.id = _customData[i].id;
                        data.attr = _customData[i].attr;
                        if (day == startDay)
                            data.startTime = ExtractHourMinute(startTime);
                        else
                            data.startTime = MonthCommon.TIME_START;
                        if (day == endDay)
                            data.endTime = ExtractHourMinute(endTime);
                        else data.endTime = MonthCommon.TIME_END;
                        customDataInDay.Add(day, data);
                    }
                }
                else
                {
                    if ((_customData[i].startTime != "") && (_customData[i].endTime == ""))
                    {
                        CustomDataType data = new CustomDataType();
                        startTime = Convert.ToDateTime(_customData[i].startTime);

                        data.id = _customData[i].id;
                        data.attr = _customData[i].attr;
                        data.startTime = ExtractHourMinute(startTime);
                        data.endTime = "";
                        customDataInDay.Add(startTime.Day, data);
                    }
                }
            }

            for (int day = MonthCommon.MONTH_START; day <= _days; day++)
                MarkEachDayAttribute(day, customDataInDay);


        }

        private void MarkEachDayAttribute(int day, Dictionary<int, CustomDataType> customData)
        {

            if (customData.ContainsKey(day))
            {
                CustomDataType dataType = customData[day];
                _dtAttr.Rows[day][dataType.attr] = true;
                if (dataType.attr == attrNameList[0])
                {
                    _dtAttr.Rows[day]["id"] = dataType.id;
                    _dtAttr.Rows[day]["StartTime"] = dataType.startTime;
                    _dtAttr.Rows[day]["EndTime"] = dataType.endTime;
                }
            }

        }

        public void ClearExternalDateAttribute(string attr)
        {
            for (int day = MonthCommon.MONTH_START; day <= _days; day++)
            {
                _dtAttr.Rows[day][attr] = false;
                if (attr == attrNameList[0])
                {
                    _dtAttr.Rows[day]["id"] = -1;
                    _dtAttr.Rows[day]["StartTime"] = "";
                    _dtAttr.Rows[day]["EndTime"] = "";
                }
            }
        }

        private string ExtractHourMinute(DateTime time)
        {
            int hour = time.Hour;
            int minute = time.Minute;
            if (minute == 0)
                return hour.ToString() + ":" + "00";
            else
                return hour.ToString() + ":" + minute.ToString();
        }
    }
}
