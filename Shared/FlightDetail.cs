using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class FlightDetail
    {
        public int Id { get; set; }
        public string Airline { get; set; }
        public string FromAirport { get; set; }
        public DateTime DepartureDate { get; set; }
        public string ToAirport { get; set; }
        public DateTime ArrivalDate { get; set; }
        public int? SeatId { get; set; }
    }
}
