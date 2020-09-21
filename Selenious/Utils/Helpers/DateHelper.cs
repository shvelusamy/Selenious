namespace Selenious.Utils.Helpers
{
    using System;
    using System.Globalization;

    public static class DateHelper
    {
        public enum ShortMonth
        {
            Jan = 01,
            Feb = 02,
            Mar = 03,
            Apr = 04,
            May = 05,
            Jun = 06,
            Jul = 07,
            Aug = 08,
            Sep = 09,
            Oct = 10,
            Nov = 11,
            Dec = 12,
            None = 0,
        }

        public enum LongMonth
        {
            January = 01,
            February = 02,
            March = 03,
            April = 04,
            May = 05,
            June = 06,
            July = 07,
            August = 08,
            September = 09,
            October = 10,
            November = 11,
            December = 12,
            None = 0,
        }

        /// <summary>
        /// Returns the current date and time.
        /// </summary>
        public static string CurrentDateTimeStamp
        {
            get
            {
                return DateTime.Now.ToString("ddMMyyyyHHmmss", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Returns tomorrow's date
        /// </summary>
        public static string TomorrowDate
        {
            get
            {
                return DateTime.Now.AddDays(1).ToString("ddMMyyyy", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Returns future date
        /// </summary>
        public static string GetFutureDate(int numberDaysToAddToNow)
        {
            return DateTime.Now.AddDays(numberDaysToAddToNow).ToString("ddMMyyyy", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns abbriveated month name, by passing desired month name.
        /// </summary>
        public static string GetShortMonthName(LongMonth monthName)
        {
            return DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName((int)monthName);
        }

        /// <summary>
        /// Returns abbriveated month name, by passing desired month number.
        /// </summary>
        public static string GetShortMonthName(int monthIndex)
        {
            return DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(monthIndex);
        }

        /// <summary>
        /// Returns full month name, by passing abbreviated month name.
        /// </summary>
        public static string GetLongMonthName(ShortMonth monthName)
        {
            return DateTimeFormatInfo.CurrentInfo.GetMonthName((int)monthName);
        }

        /// <summary>
        /// Returns full month name, by passing month index.
        /// </summary>
        public static string GetLongMonthName(int monthIndex)
        {
            return DateTimeFormatInfo.CurrentInfo.GetMonthName(monthIndex);
        }
    }
}