using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DirectFlights.Shared
{
    public partial class FlightScheduleTemplate
    {
        public int Id { get; set; }
        public int FlightNumber { get; set; }
        public int SegmentNumber { get; set; }
        public int AirlineId { get; set; }
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }
        [JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly TakeOffTime { get; set; }
        [JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly LandingTime { get; set; }
        public int PlaneTypeId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateRetired { get; set; }

        public virtual Airline Airline { get; set; } = null!;
        public virtual Airport ArrivalAirport { get; set; } = null!;
        public virtual Airport DepartureAirport { get; set; } = null!;
        public virtual PlaneType PlaneType { get; set; } = null!;
    }
}
