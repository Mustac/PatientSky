﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientSky.Models
{

    public class Timeslot
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("calendar_id")]
        public string CalendarId { get; set; }

        [JsonProperty("type_id")]
        public string TypeId { get; set; }

        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("end")]
        public DateTime End { get; set; }

        [JsonProperty("public_bookable")]
        public bool PublicBookable { get; set; }

        [JsonProperty("out_of_office")]
        public bool OutOfOffice { get; set; }
    }


}
