using System;
using System.Collections.Generic;
using System.Globalization;

namespace Common.Extensions
{
    public static class DateExtension
    {
        private static bool configured;
        private static string _dateTimeFormat;
        private static string _dateFormat;
        private static string _timeFormat;
        private static List<string> _formats = new List<string>();

        public static void Configure(string dateTimeFormat, string dateFormat, string timeFormat)
        {
            configured = true;
            _dateTimeFormat = dateTimeFormat;
            _dateFormat = dateFormat;
            _timeFormat = timeFormat;
            _formats.Add(_dateTimeFormat);
            _formats.Add(_dateFormat);
            _formats.Add(_timeFormat);
        }

        private static void checkIfConfigured()
        {
            if (!configured)
            {
                throw new Exception("please configure DateExtension static class in Startup.cs");
            }
        }

        public static string ConvertToDateTimeString(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToString(_dateTimeFormat);
            }

            return "";
        }
        //public static string ConvertToDateString(this DateTime? date)
        //{
        //    if (date.HasValue)
        //    {
        //        return date.Value.ToString(_dateFormat);
        //    }

        //    return "";
        //}

        //public static string ConvertToTimeString(this DateTime? date)
        //{
        //    if (date.HasValue)
        //    {
        //        return date.Value.ToString(_timeFormat);
        //    }

        //    return "";
        //}


        //public static string ConvertToDateTimeString(this DateTime date)
        //{
        //    return date.ToString(_dateTimeFormat);
        //}

        //public static string ConvertToDateString(this DateTime date)
        //{
        //    return date.ToString(_dateFormat);
        //}

        //public static string ConvertToTimeString(this DateTime date)
        //{
        //    return date.ToString(_timeFormat);
        //}

        public static DateTime? ConvertToDateTime(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return DateTime.ParseExact(value, _formats.ToArray(), CultureInfo.InvariantCulture);
            }

            return null;
        }

        public static TimeSpan? ConvertToTimeSpan(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("ص", "am");
                value = value.Replace("م", "pm");
                return DateTime.ParseExact(value, _timeFormat, CultureInfo.InvariantCulture).TimeOfDay;
            }

            return null;
        }

        public static string ConvertToTimeSpanString(this TimeSpan? timeSpan)
        {
            if (timeSpan.HasValue)
            {
                DateTime time = DateTime.Today.Add(timeSpan.Value);

                return time.ToString(_timeFormat);
            }

            return null;
        }
    }
}
