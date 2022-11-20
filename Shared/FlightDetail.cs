using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectFlights.Shared
{
    public class FlightDetail
    {
        public int Id { get; set; }
        public DateTime DepartTime { get; set; }
        public DateTime ArriveTime { get; set; }
        public string DepartAirport { get; set; }
        public string ArriveAirport { get; set; }
        public string Airline { get; set; }
        public IEnumerable<Seat> Seats { get; set; }
    }
}
