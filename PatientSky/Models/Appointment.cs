using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientSky.Models
{
    public class Appointment
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("patient_id")]
        public string PatientId { get; set; }

        [JsonProperty("calendar_id")]
        public string CalendarId { get; set; }

        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("end")]
        public DateTime End { get; set; }

        [JsonProperty("patient_comment")]
        public object PatientComment { get; set; }

        [JsonProperty("note")]
        public object Note { get; set; }

        [JsonProperty("time_slot_type_id")]
        public string TimeSlotTypeId { get; set; }

        [JsonProperty("type_id")]
        public object TypeId { get; set; }

        [JsonProperty("state")]
        public int State { get; set; }

        [JsonProperty("out_of_office_location")]
        public string OutOfOfficeLocation { get; set; }

        [JsonProperty("out_of_office")]
        public bool OutOfOffice { get; set; }

        [JsonProperty("completed")]
        public bool Completed { get; set; }

        [JsonProperty("is_scheduled")]
        public bool IsScheduled { get; set; }
    }
}
