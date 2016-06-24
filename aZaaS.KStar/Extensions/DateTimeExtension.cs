using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime? ToDateTime(this string dateString)
        {
            return string.IsNullOrEmpty(dateString) ? null : (DateTime?)DateTime.Parse(dateString);
        }

        public static DateTime? ToBeginDateTime(this string dateString, string dateFormat = "yyyy-MM-dd", bool useMinValueIfNull = false)
        {
            return dateString.ToDateTime().ToBeginDateTime(dateFormat, useMinValueIfNull);
        }

        public static DateTime? ToEndDateTime(this string dateString, string dateFormat = "yyyy-MM-dd", bool useMaxValueIfNull = false)
        {
            return dateString.ToDateTime().ToEndDateTime(dateFormat, useMaxValueIfNull);
        }

        public static DateTime? ToBeginDateTime(this DateTime? date, string dateFormat = "yyyy-MM-dd", bool useMinValueIfNull = false)
        {
            if (date == null)
            {
                if (!useMinValueIfNull)
                    return null;
                else
                    date = (DateTime?)DateTime.MinValue;
            }

            return DateTime.Parse(string.Format("{0} 01:00:00", date.Value.ToString(dateFormat))); //TODO:Find a better solution?
        }

        public static DateTime? ToEndDateTime(this DateTime? date, string dateFormat = "yyyy-MM-dd", bool useMaxValueIfNull = false)
        {
            if (date == null)
            {
                if (!useMaxValueIfNull)
                    return null;
                else
                    date = (DateTime?)DateTime.MaxValue;
            }

            return DateTime.Parse(string.Format("{0} 23:59:59", date.Value.ToString(dateFormat)));
        }
    }
}
