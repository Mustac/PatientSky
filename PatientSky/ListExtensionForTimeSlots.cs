using PatientSky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientSky
{
    public static class ListExtensionForTimeSlots
    {
        public static List<TimeSlot> GetAvailableTimeSlots(this List<TimeSlot> timeSlots, string calendarId, DateTime fromTime, DateTime toTime)
        {
            return timeSlots.Where(x => x.CalendarId == calendarId).Where(x => x.Start >= fromTime && x.End <= toTime).ToList();
        }
    }
}
