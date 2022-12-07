using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectFlights.Shared
{
    public class FlightDetailDTO
    {
        public int Id { get; set; }
        public string Airline { get; set; }
        public string FromAirport { get; set; }
        public DateTime DepartureDate { get; set; }
        public string ToAirport { get; set; }
        public DateTime ArrivalDate { get; set; }
        public List<Seat> SeatClasses { get; set; }
    }
}
