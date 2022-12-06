using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class Passenger
    {
        public Passenger()
        {
            FlightReservations = new HashSet<FlightReservation>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<FlightReservation> FlightReservations { get; set; }
    }
}
