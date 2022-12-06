using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class FlightScheduleTemplateOption
    {
        public string? DepartureAirport { get; set; }
        public string? ArrivalAirport { get; set; }
        public string? Airline { get; set; }
        public string? PlaneType { get; set; }
    }
}
