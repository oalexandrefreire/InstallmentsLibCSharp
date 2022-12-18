using System;

namespace NextBusinessDay
{
    public static class BusinessDayLib
    {
        public static DateTime GetNextBusinessDay(DateTime dateTime)
        {
            if (dateTime.DayOfWeek == DayOfWeek.Saturday)
                dateTime = dateTime.AddDays(2);
            else if (dateTime.DayOfWeek == DayOfWeek.Sunday)
                dateTime = dateTime.AddDays(1);

            //if (dateTime.Day == 31 && dateTime.Month == 12) //----- Feriado bancário
            //    dateTime.AddDays(1);

            //var publicHolidays = DateSystem.GetPublicHoliday(dateTime, dateTime, CountryCode.BR);

            //if (publicHolidays.Count() > 0)
            //    dateTime = dateTime.AddDays(1);
            //else
            return dateTime;
        }
    }
   
}
