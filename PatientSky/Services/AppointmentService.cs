using PatientSky.Data;
using PatientSky.Models;
using PatientSky.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PatientSky.Services
{
    public class AppointmentService
    {

        private readonly AppDbContext _db = new AppDbContext();


        /// <summary>
        /// Used for getting taken apoitments from defined start, end date and list of calendarIds ( Doctors )
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="calendarIds"></param>
        /// <returns>List of object TakenAppointmentDTO</returns>
        public List<TakenAppointmentDTO> GetTakenAppointments(DateTime start, DateTime end, List<string> calendarIds)
        {

            var doctorAppointmentsDTOs = _db.Apointments.Where(x => calendarIds.Contains(x.CalendarId))
                .Join(_db.Doctors, x => x.CalendarId, y => y.CalendarId, (appointment, doctor) => new TakenAppointmentDTO { Appointment = appointment, Doctor = doctor })
                .Where(x => x.Appointment.Start >= start && x.Appointment.End <= end).OrderBy(x => x.Doctor.CalendarId).ThenBy(x => x.Appointment.Start).ToList();

         
            return doctorAppointmentsDTOs;

        }

        public List<TimeSlot> GetTimeslotsInTimePeriod(List<string> calendarIds ,DateTime fromDate, DateTime toDate, int duration)
        {
            TimeSpan durationSpan = new TimeSpan(0, duration, 0);

            List<TimeSlot> timeSlots = _db.TimeSlots.Where(x => x.Start >= fromDate && x.End <= toDate).Where(x =>x.End - x.Start >= durationSpan).Where(x=> calendarIds.Contains(x.CalendarId)).OrderBy(x=>x.CalendarId).ThenBy(x=>x.Start).ToList();


            // 

            return timeSlots;
        }


        /// <summary>
        /// Used for getting available appointments from List of TakenAppoitnemt object and duration
        /// </summary>
        /// <param name="takenAppointments"></param>
        /// <param name="duration"></param>
        /// <returns>List of AvailableAppoitnemtnDTO</returns>
        public List<AvailableAppointmentDTO> GetAvailableAppointments(List<TakenAppointmentDTO> takenAppointments, List<TimeSlot> timeSlots, int duration)
        {
            DateTime dayCheck = new DateTime();
            DateTime fromTime = new DateTime();
            DateTime toTime = new DateTime();

            TimeSpan durationSpan = new TimeSpan(0, duration, 0);

            List<AvailableAppointmentDTO> availableAppointmentDTOs = new List<AvailableAppointmentDTO>();

            // getting datetime from start of available time and saving it to fromTime, and end to toTime. Calculating if endtime - starttime is bigger than required duration
            // if it is save that object to availableAppointmentsDTOs

            foreach(var takenAppointment in takenAppointments)
            {
                // checking if tineSaved.day is equal to current appointment day
                // = if it is equal we are not in the same day 
                // != if it is not equal that means this appointment is in the new day 

               
                if (dayCheck.Day != takenAppointment.Appointment.Start.Day)
                {
                    // looping once per new day
                    // work hours start at 8:00h. Setting fromTime to this date 8:00h

                    fromTime = new DateTime(takenAppointment.Appointment.Start.Year, takenAppointment.Appointment.Start.Month, takenAppointment.Appointment.Start.Day, 8, 0, 0);
                    toTime = takenAppointment.Appointment.Start;


                    // setting day to this appointment day so this wont loop unless it is a new day
                    dayCheck = takenAppointment.Appointment.Start;

                } 


                if (CheckIfAppointmentIsAvailable(fromTime, toTime, durationSpan))
                {
                    List<TimeSlot> timeSlotsInThisPeriod = timeSlots.Where(x => x.Start >= fromTime && x.End <= toTime)
                                                                    .Where(x=>x.CalendarId == takenAppointment.Appointment.CalendarId).ToList();

                    availableAppointmentDTOs.Add(CreateAvailableAppointment(takenAppointment, fromTime, toTime, durationSpan, timeSlotsInThisPeriod));
                }

            }

            return availableAppointmentDTOs;



        }


        private bool CheckIfAppointmentIsAvailable(DateTime fromTime, DateTime toTime, TimeSpan duration)
        {
            if (toTime - fromTime < duration)
                return false;

            return true;

        }


        private AvailableAppointmentDTO CreateAvailableAppointment(TakenAppointmentDTO takenAppointment, DateTime fromTime, DateTime toTime, TimeSpan duration, List<TimeSlot> timeSlots)
        {
            AvailableAppointmentDTO availableAppointmentDTO = new AvailableAppointmentDTO
            {
                RequestedTimeWithDoctor = duration.Minutes,
                Doctor = takenAppointment.Doctor,
                AvailableDateTime = new AvailableTimeModelDTO
                {
                    AvailableFromTime = fromTime,
                    AvailableToTime = toTime - duration,
                    DoctorUnavailableFrom = toTime
                },
                TimeSlots = timeSlots
            };

            return availableAppointmentDTO;

            
        }

    }
}
