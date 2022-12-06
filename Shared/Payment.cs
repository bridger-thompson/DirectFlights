using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class Payment
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public int FlightReservationId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }

        public virtual FlightReservation FlightReservation { get; set; } = null!;
        public virtual staff Staff { get; set; } = null!;
    }
}
