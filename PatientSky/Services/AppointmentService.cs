using PatientSky.Data;
using PatientSky.Helpers;
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
            // Retriving all the takenAppointments withing the date range and calendarId
            // Parsing them to List of TakenAppointmentsDTO object

            var doctorAppointments = _db.Apointments.Where(x => calendarIds.Contains(x.CalendarId))
                .Join(_db.Doctors, x => x.CalendarId, y => y.CalendarId, (appointment, doctor) => new TakenAppointmentDTO { Appointment = appointment, Doctor = doctor })
                .Where(x => x.Appointment.Start >= start && x.Appointment.End <= end).OrderBy(x => x.Doctor.CalendarId).ThenBy(x => x.Appointment.Start).ToList();


            // inserting start time at 0 position
            // so we can take range of time from this object to next one in list
            // if duration of requested meeting is lower or equal to it

            doctorAppointments.Insert(0, new TakenAppointmentDTO
            {
                Appointment = new Appointment
                {
                    Start=start,
                    End = start,
                    OutOfOffice = true
                },
                Doctor = new Doctor()
            });

            // inserting end time at last position
            // so we can take range of time from previous object to this object
            // if duration of requested meeting is lower or equal to it

            doctorAppointments.Insert(doctorAppointments.Count, new TakenAppointmentDTO
            {
                Appointment = new Appointment
                {
                    Start = end,
                    End = end,
                    OutOfOffice = true
                }
            }) ;

            
            TakenAppointmentDTO thisAppointment = new TakenAppointmentDTO {Appointment = new Appointment { Id="1" } };
            TakenAppointmentDTO nextAppointment = new TakenAppointmentDTO();

            int count = 0;
            int index = 0;
            
            while (thisAppointment != nextAppointment && index + 1 < doctorAppointments.Count)
            {
                thisAppointment = doctorAppointments[index];
                nextAppointment = doctorAppointments[index + 1];


                if(thisAppointment.Appointment.End.Date < nextAppointment.Appointment.Start.Date)
                {
                    doctorAppointments.Insert(index, new TakenAppointmentDTO
                    {
                        Appointment = new Appointment
                        {
                            Start = DateTime.Parse($"{doctorAppointments[index].Appointment.End.Year}-{doctorAppointments[index].Appointment.End.Month}" +
                                                       $"-{doctorAppointments[index].Appointment.End.Day}T20:00:00"),

                            End = start.ChangeTime(8,0).AddDays(1),
                            OutOfOffice = true
                        },
                        Doctor = new Doctor()

                    });

                    index++;
                }



                index++;
            }

            return doctorAppointments;

        }


        /// <summary>
        /// Used for getting taken apoitments from defined start, end date and list of calendarIds ( Doctors ) and duration
        /// </summary>
        /// <param name="calendarIds"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public List<TimeSlot> GetTimeslotsInTimePeriod(DateTime fromDate, DateTime toDate, List<string> calendarIds, int duration)
        {
            TimeSpan durationSpan = new TimeSpan(0, duration, 0);

            List<TimeSlot> timeSlots = _db.TimeSlots.Where(x => x.Start >= fromDate && x.End <= toDate)
                .Where(x => calendarIds.Contains(x.CalendarId))
                .OrderBy(x => x.CalendarId).ThenBy(x => x.Start).ToList();

            

            return timeSlots;
        }


        /// <summary>
        /// Used for getting taken apoitments from defined start, end date and List of TakenAppointments, List of TimeSlots, and duration
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="takenAppointments"></param>
        /// <param name="timeSlots"></param>
        /// <param name="durration"></param>
        /// <returns></returns>
        public List<AvailableAppointmentDTO> GetAvailableAppointments(DateTime start, DateTime end, List<TakenAppointmentDTO> takenAppointments, List<TimeSlot> timeSlots, int durration)
        {
            
            List<AvailableAppointmentDTO> availableAppointmentsDTO = new List<AvailableAppointmentDTO>();

            TimeSpan requestedMeetingDuration = new TimeSpan(0, durration, 0);

            DateTime lastAppointmentTime = new DateTime();
            
            
            for(int i = 0; i<takenAppointments.Count; i++)
            {
                if (lastAppointmentTime.Date < takenAppointments[i].Appointment.End.Date)
                {
                    lastAppointmentTime = takenAppointments[i].Appointment.End;
                } else if (lastAppointmentTime.Date == takenAppointments[i].Appointment.Start.Date)
                {
                    if (takenAppointments[i].Appointment.Start - lastAppointmentTime >= requestedMeetingDuration)
                    {
                        availableAppointmentsDTO.Add(
                            CreateAvailableAppointment(
                                lastAppointmentTime,
                                takenAppointments[i].Appointment.Start,
                                requestedMeetingDuration,
                                takenAppointments[i].Doctor,
                                timeSlots.Where(x => x.CalendarId == takenAppointments[i].Appointment.CalendarId)
                                .Where(x => x.Start >= lastAppointmentTime && x.End <= takenAppointments[i].Appointment.Start).ToList()
                                ));
                    }
                }
                lastAppointmentTime = takenAppointments[i].Appointment.End;
            }


            return availableAppointmentsDTO;
        }

        /// <summary>
        /// Used for creating AvaliableAppointmentDTO object
        /// </summary>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <param name="meetingDuration"></param>
        /// <param name="doctor"></param>
        /// <param name="slots"></param>
        /// <returns></returns>
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
            