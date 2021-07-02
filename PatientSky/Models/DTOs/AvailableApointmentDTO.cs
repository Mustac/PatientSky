using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientSky.Models.DTOs
{
    public class AvailableApointmentDTO
    {
        public string CalendarId { get; set; }
        public List<AvailableTimeModelDTO> AvailableDateTime { get; set; }
    }

    public class AvailableTimeModelDTO
    {

        public DoctorDTO Doctor { get; set; }
        public DateTime AvailableFromTime { get; set; }
        public DateTime AvailableToTime { get; set; }
        public int RequestedTimeWithDoctor { get; set; }
        public DateTime DoctorsNextMeeting { get; set; }
    }

    public class DoctorDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CalendarId { get; set; }
    }

}
