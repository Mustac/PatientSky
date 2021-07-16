using Microsoft.AspNetCore.Mvc;
using PatientSky.Models.DTOs;
using PatientSky.Services;

namespace PatientSky.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApointmentController : ControllerBase
    {

        private readonly AppointmentService _apointmentService;

        public ApointmentController(AppointmentService apointmentService)
        {
            _apointmentService = apointmentService;
        }


        /// <summary>
        /// Get available appointments
        /// </summary>
        /// <param name="appointmentCheckDTO"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("Available")]
        public IActionResult AvailableDoctors(AppointmentCheckDTO appointmentCheckDTO)
        {


            if (appointmentCheckDTO.Duration <= 14)
                return BadRequest("Minimal duration time with doctor is 15 minutes");

            var takenAppoitments = _apointmentService.GetTakenAppointments(appointmentCheckDTO.Start, appointmentCheckDTO.End, appointmentCheckDTO.CalendarId);

            var timeSlots = _apointmentService.GetTimeslotsInTimePeriod(appointmentCheckDTO.CalendarId, appointmentCheckDTO.Start, appointmentCheckDTO.End, appointmentCheckDTO.Duration);

            var freeAppointments = _apointmentService.GetAvailableAppointments(appointmentCheckDTO.Start, appointmentCheckDTO.End, takenAppoitments, timeSlots, appointmentCheckDTO.Duration);

            return Ok(freeAppointments);
        }
    }
}
