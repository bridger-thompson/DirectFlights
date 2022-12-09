using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class FlightTotal
    {
        public int FlightNumber { get; set; }
        public DateTime DepartureDate { get; set; }
        public decimal? Total { get; set; }
        public decimal? Refund { get; set; }
        public decimal? Profit { get; set; }
    }
}
