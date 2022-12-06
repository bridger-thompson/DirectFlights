using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class FlightSeatClass
    {
        public FlightSeatClass()
        {
            FlightReservations = new HashSet<FlightReservation>();
        }

        public int Id { get; set; }
        public int SeatId { get; set; }
        public int FlightId { get; set; }
        public decimal SuggestedCost { get; set; }

        public virtual FlightSchedule Flight { get; set; } = null!;
        public virtual SeatClass Seat { get; set; } = null!;
        public virtual ICollection<FlightReservation> FlightReservations { get; set; }
    }
}
