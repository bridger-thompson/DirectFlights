using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class PassengerManifest
    {
        public int Id { get; set; }
        public int FlightBookingId { get; set; }
        public DateTime BoardingDate { get; set; }
        public int StaffId { get; set; }

        public virtual FlightBooking FlightBooking { get; set; } = null!;
        public virtual staff Staff { get; set; } = null!;
    }
}
