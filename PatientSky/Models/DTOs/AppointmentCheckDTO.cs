using PatientSky.CustomDataValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PatientSky.Models.DTOs
{
    public class AppointmentCheckDTO
    {
        [Required]
        public List<string> CalendarId { get; set; }
        [Required]
        [Range(15,90,ErrorMessage ="The duration must be in range from 15 to 90 minutes")]
        public int Duration { get; set; }
        [Required]
        [CheckIfDateIsInRange(ErrorMessage = "The start date should be after 2019-03-01T13:00:00Z")]
        public DateTime Start { get; set; }
        [Required]
        [CheckIfDateIsInRange(ErrorMessage = "The end date should be before 2019-05-11T15:30:00Z")]
        public DateTime End { get; set; }
    }
}
