using System;
using System.Globalization;

namespace Libs.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTimeSeconds(this DateTime self)
        {
            return new DateTimeOffset(self).ToUnixTimeSeconds();
        }

        public static long ToLocalUnixTimeSeconds(this DateTime self)
        {
            return new DateTimeOffset(self.ToLocalTime()).ToUnixTimeSeconds();
        }

        public static long ToUniversalUnixTimeSeconds(this DateTime self)
        {
            return new DateTimeOffset(self.ToUniversalTime()).ToUnixTimeSeconds();
        }

        public static long ToUnixTimeMilliseconds(this DateTime self)
        {
            return new DateTimeOffset(self).ToUnixTimeMilliseconds();
        }

        public static long ToLocalUnixTimeMilliseconds(this DateTime self)
        {
            return new DateTimeOffset(self.ToLocalTime()).ToUnixTimeMilliseconds();
        }

        public static long ToUniversalUnixTimeMilliseconds(this DateTime self)
        {
            return new DateTimeOffset(self.ToUniversalTime()).ToUnixTimeMilliseconds();
        }

        public static int GetWeekOfYear(this DateTime self, CalendarWeekRule calendarWeekRule, DayOfWeek firstDayOfWeek)
        {
            return self.GetWeekOfYear(calendarWeekRule, firstDayOfWeek, CultureInfo.CurrentCulture.Calendar);
        }

        public static int GetWeekOfYear(this DateTime self, CalendarWeekRule calendarWeekRule, DayOfWeek firstDayOfWeek, Calendar calendar)
        {
            return calendar.GetWeekOfYear(self, calendarWeekRule, firstDayOfWeek);
        }

        public static int GetIso8601WeekOfYear(this DateTime self)
        {
            return self.GetIso8601WeekOfYear(CultureInfo.CurrentCulture.Calendar);
        }

        public static int GetIso8601WeekOfYear(this DateTime self, Calendar calendar)
        {
            DayOfWeek day = calendar.GetDayOfWeek(self);
            if(day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                self = self.AddDays(3);
            }

            return calendar.GetWeekOfYear(self, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static bool IsLeapYear(this DateTime self)
        {
            return DateTime.IsLeapYear(self.Year);
        }
    }
}
