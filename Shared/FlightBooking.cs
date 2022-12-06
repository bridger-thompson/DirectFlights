using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class FlightBooking
    {
        public FlightBooking()
        {
            PassengerManifests = new HashSet<PassengerManifest>();
        }

        public int Id { get; set; }
        public int FlightReservationId { get; set; }
        public DateTime BookDate { get; set; }
        public int StaffId { get; set; }

        public virtual FlightReservation FlightReservation { get; set; } = null!;
        public virtual staff Staff { get; set; } = null!;
        public virtual ICollection<PassengerManifest> PassengerManifests { get; set; }
    }
}
