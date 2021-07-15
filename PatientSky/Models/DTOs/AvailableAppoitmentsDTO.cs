using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientSky.Models.DTOs
{
    public class AvailableAppointmentDTO
    {
        public Doctor Doctor { get; set; }
        public int RequestedTimeWithDoctor { get; set; }
        public AvailableTimeModelDTO AvailableDateTime { get; set; }
        public List<TimeSlot> TimeSlots { get; set; }
        
    }

    public class AvailableTimeModelDTO
    {
        public DateTime AvailableFromTime { get; set; }
        public DateTime AvailableToTime { get; set; }
        public DateTime DoctorUnavailableFrom { get; set; }
    }

    public class AvailableTimeSlot
    {
        public string TimeSlotId { get; set; }
        public string TypeId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }


  
}
