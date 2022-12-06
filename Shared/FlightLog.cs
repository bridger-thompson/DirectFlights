using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class FlightLog
    {
        public int Id { get; set; }
        public int FlightScheduleId { get; set; }
        public int DepartAirportId { get; set; }
        public int? ArrivalAirportId { get; set; }
        public DateTime DepartDate { get; set; }
        public DateTime? ArrivalDate { get; set; }

        public virtual Airport? ArrivalAirport { get; set; }
        public virtual Airport DepartAirport { get; set; } = null!;
        public virtual FlightSchedule FlightSchedule { get; set; } = null!;
    }
}
