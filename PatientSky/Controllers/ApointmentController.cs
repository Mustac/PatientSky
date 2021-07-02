using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientSky.Data;
using PatientSky.Models;
using PatientSky.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PatientSky.Extensions;

namespace PatientSky.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApointmentController : ControllerBase
    {
        private readonly AppDbContext _db = new AppDbContext();


        /// <summary>
        /// Endpoint for requesting available meetings
        /// </summary>
        /// <param name="apointmentCheck"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetAvailable")]
        public IActionResult GetAvailableMeetings(ApointmentCheckDTO apointmentCheck)
        {
            // minimal duration you can request is 15 min
            if (apointmentCheck.Duration < 14)
                return BadRequest("Minimal time you can request is 15 min");

            List<AvailableTimeModelDTO> availableTimes = new List<AvailableTimeModelDTO>();

            List<DoctorDTO> doctors = new List<DoctorDTO>();
           
            foreach(var calendarId in apointmentCheck.CalendarId)
            {
                List<Appointment> apointments = new List<Appointment>();

                // if calendarId does not exist inside doctor list, add that doctor to a list ( saving so that we can loop through small list instead of accessing database all the time )
                if (!doctors.Any(x => x.CalendarId == calendarId))
                {
                    var doctor = _db.Doctors.FirstOrDefault(x => x.CalendarId == calendarId);
                    doctors.Add(new DoctorDTO { FirstName = doctor.FirstName, LastName = doctor.LastName, CalendarId = calendarId });
                }

                // get apoitments using calendarid and order them from lower time to higher time
                if (calendarId != null)
                {
                    apointments = _db.Apointments.Where(x => x.CalendarId == calendarId).OrderBy(x=>x.Start).ToList();
                }

                DateTime previousApointmentEndTime = new DateTime();

                
                foreach(var apointment in apointments)
                {
                        // Check if its new day and add first apointment end time to the previousApointment
                    if (previousApointmentEndTime.Day != apointment.Start.Day)
                    {
                        previousApointmentEndTime = apointment.End;
                    }
                    else if(apointment.Start.Day == previousApointmentEndTime.Day)
                    {

                        int minutesLastApointmentEnd = previousApointmentEndTime.Hour * 60 + previousApointmentEndTime.Minute;
                        int minutesThisApointmentStart = apointment.Start.Hour * 60 + apointment.Start.Minute;
                      
                        // Extension on int to give back timespan
                        DateTime availableToTime = apointment.Start.Add(-apointmentCheck.Duration.MinutesToTimeSpan());

                        if(minutesThisApointmentStart - minutesLastApointmentEnd >= apointmentCheck.Duration)
                        {
                            var doctor = doctors.FirstOrDefault(x => x.CalendarId == apointment.CalendarId);

                            availableTimes.Add(new AvailableTimeModelDTO { Doctor = new DoctorDTO { FirstName = doctor.FirstName, LastName = doctor.LastName, CalendarId = doctor.CalendarId},
                                AvailableFromTime = previousApointmentEndTime, AvailableToTime=availableToTime, DoctorsNextMeeting=apointment.Start,  RequestedTimeWithDoctor=apointmentCheck.Duration});

                        }
                        //saving this apoitment end time to previousApoitment time for next time it loops
                        previousApointmentEndTime = apointment.End;

                    }

                    
                    


                }




            }

           
            

            return Ok(availableTimes);
        }
    }
}
