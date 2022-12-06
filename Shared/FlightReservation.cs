using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class FlightReservation
    {
        public FlightReservation()
        {
            FlightBookings = new HashSet<FlightBooking>();
            Payments = new HashSet<Payment>();
        }

        public int Id { get; set; }
        public int PassengerId { get; set; }
        public int FlightScheduleId { get; set; }
        public int ClassId { get; set; }
        public DateOnly ReservationDate { get; set; }
        public decimal SeatCost { get; set; }

        public virtual FlightSeatClass Class { get; set; } = null!;
        public virtual FlightSchedule FlightSchedule { get; set; } = null!;
        public virtual Passenger Passenger { get; set; } = null!;
        public virtual ICollection<FlightBooking> FlightBookings { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
