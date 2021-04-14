using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Wasteless.Helpers
{
    public static class DateTimeExtensions
    {
        public static IEnumerable<DateTime> DatesTo(this DateTime from, DateTime to)
        {
            return Enumerable.Range(0, (to.Date - from.Date).Days + 1).Select(x => from.AddDays(x));
        }

        public static DateTime? ToDate(this int number)
        {
            if (DateTime.TryParseExact(number.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var result))
                return result;

            return null;
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}