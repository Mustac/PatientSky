using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientSky.Models.DTOs
{
    public class ApointmentCheckDTO
    {
        public List<string> CalendarId { get; set; }
        public int Duration { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
