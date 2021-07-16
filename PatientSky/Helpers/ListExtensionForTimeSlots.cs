using PatientSky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientSky.Helpers
{
    public static class MyExtensionMethods
    {
        public static List<TimeSlot> GetAvailableTimeSlots(this List<TimeSlot> timeSlots, string calendarId, DateTime fromTime, DateTime toTime)
        {
            return timeSlots.Where(x => x.CalendarId == calendarId).Where(x => x.Start >= fromTime && x.End <= toTime).ToList();
        }

        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hours, minutes, 0);
        }
    }
}
