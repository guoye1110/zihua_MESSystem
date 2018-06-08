using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MESSystem.OEEManagement
{
    class OEEUtils
    {
        private static int[] MONTH_SOLAR = { 1, 3, 5, 7, 8, 10, 12 };
        private static int[] MONTH_LUNAR = { 4, 6, 9, 11 };

        public static DateTime ConvertToStartOfDay(DateTime time)
        {
            const int hour = 0;
            const int minute = 0;
            const int second = 0;
            return new DateTime(time.Year, time.Month, time.Day,
                hour, minute, second);
        }

        public static DateTime ConvertToEndOfDay(DateTime time)
        {
            const int hour = OEETypes.END_OF_DAY_HOUR;
            const int minute = OEETypes.END_OF_DAY_MINUTE;
            const int second = OEETypes.END_OF_DAY_SECOND;
            return new DateTime(time.Year, time.Month, time.Day,
                hour, minute, second);
        }

        public static bool IsLeapYear(int year)
        {
            if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0))
                return true;
            else
                return false;
        }

        public static DateTime ConvertToDayStartOfMonth(int year, int month)
        {
            return new DateTime(year, month, OEETypes.DAY_START);
            
        }

        public static DateTime ConvertToDayEndOfMonth(int year, int month)
        {
            if (month == OEETypes.MONTH_LEAP)
            {
                if (IsLeapYear(year))
                    return new DateTime(year, month, OEETypes.DAY_LEAP_END);
                else
                    return new DateTime(year, month, OEETypes.DAY_NOLEAP_END);
            }
            else
            {
                if (Array.IndexOf(MONTH_SOLAR, month) >= 0)
                    return new DateTime(year, month, OEETypes.DAY_SOLAR_END);
                else
                    return new DateTime(year, month, OEETypes.DAY_LUNAR_END);
            }
        }
    }
}
