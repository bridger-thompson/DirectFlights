using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class staff
    {
        public staff()
        {
            FlightBookings = new HashSet<FlightBooking>();
            PassengerManifests = new HashSet<PassengerManifest>();
            Payments = new HashSet<Payment>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<FlightBooking> FlightBookings { get; set; }
        public virtual ICollection<PassengerManifest> PassengerManifests { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
