using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientSky.Extensions
{
    public static class IntExtensions
    {

        /// <summary>
        /// Converting int minutes to TimeSpan, Int has to be positive intiger
        /// </summary>
        /// <param name="minute"></param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan MinutesToTimeSpan(this int minute)
        {
            TimeSpan span= new TimeSpan();

            if (minute < 60)
            {
                span = new TimeSpan(0, minute,0);
            }
            else if ( minute>=60 && minute<=3600)
            {
                var hours = minute / 60;
                minute = minute - (60 * hours);
                span = new TimeSpan(0, hours, minute);
            }

            return span;
        }
    }
}
