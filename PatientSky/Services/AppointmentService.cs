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

        public List<TimeSlot> GetTimeslotsInTimePeriod(List<string> calendarIds, DateTime fromDate, DateTime toDate, int duration)
        {
            TimeSpan durationSpan = new TimeSpan(0, duration, 0);

            List<TimeSlot> timeSlots = _db.TimeSlots.Where(x => x.Start >= fromDate && x.End <= toDate)
                .Where(x => calendarIds.Contains(x.CalendarId))
                .OrderBy(x => x.CalendarId).ThenBy(x => x.Start).ToList();

            return timeSlots;
        }

        public List<AvailableAppointmentDTO> GetAvailableAppointments(DateTime start, DateTime end, List<TakenAppointmentDTO> takenAppointmentDTO, List<TimeSlot> timeSlots, int durration)
        {
            List<AvailableAppointmentDTO> availableAppointmentsDTO = new List<AvailableAppointmentDTO>();

            TimeSpan requestedMeetingDuration = new TimeSpan(0, durration, 0);

            bool firstPass = true;

            foreach(var takenAppointment in takenAppointmentDTO)
            {
                if (firstPass)
                {
                    firstPass = false;
                    start = start < DateTime.Parse($"{start.Year}-{start.Month}-{start.Day}T08:00:00") ? DateTime.Parse($"{start.Year}-{start.Month}-{start.Day}T08:00:00"):start;

                    if(takenAppointment.Appointment.Start - start >= requestedMeetingDuration)
                    {
                        List<TimeSlot> slots = timeSlots.GetAvailableTimeSlots(takenAppointment.Appointment.CalendarId, start, takenAppointment.Appointment.Start);
                        availableAppointmentsDTO.Add(CreateAvailableAppointment(start, takenAppointment.Appointment.Start, requestedMeetingDuration, takenAppointment.Doctor, slots));
                    }
                } 



            }


            return availableAppointmentsDTO;
        }


        private AvailableAppointmentDTO CreateAvailableAppointment(DateTime fromTime, DateTime toTime, TimeSpan meetingDuration, Doctor doctor, List<TimeSlot> slots) 
        {

            AvailableAppointmentDTO availableAppointment = new AvailableAppointmentDTO
            {
                Doctor = doctor,
                AvailableDateTime = new AvailableTimeModelDTO
                {
                    AvailableFromTime = fromTime,
                    AvailableToTime = toTime - meetingDuration,
                    DoctorUnavailableFrom = toTime,
                    RequestedTimeWithDoctor = (int)meetingDuration.TotalMinutes
                },
                TimeSlots = slots
            };

            return availableAppointment;
        }
   
    }
}
            