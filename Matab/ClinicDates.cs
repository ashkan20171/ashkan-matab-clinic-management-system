using System;
using System.Globalization;

namespace Matab
{
    /// <summary>Presentation-only date formatting; never silently rewrites legacy database values.</summary>
    internal static class ClinicDates
    {
        internal static string Format(DateTime value)
        {
            if (ClinicLanguage.IsEnglish)
                return value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            var calendar = new PersianCalendar();
            return string.Format(CultureInfo.InvariantCulture, "{0:0000}/{1:00}/{2:00}",
                calendar.GetYear(value), calendar.GetMonth(value), calendar.GetDayOfMonth(value));
        }
    }
}
