using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class FlightSchedule
    {
        public FlightSchedule()
        {
            FlightLogs = new HashSet<FlightLog>();
            FlightReservations = new HashSet<FlightReservation>();
            FlightSeatClasses = new HashSet<FlightSeatClass>();
        }

        public int Id { get; set; }
        public int FlightNumber { get; set; }
        public int SegmentNumber { get; set; }
        public int AssignedPlane { get; set; }
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string DepartureGate { get; set; } = null!;
        public string ArrivalGate { get; set; } = null!;
        public bool Cancelled { get; set; }

        public virtual Airport ArrivalAirport { get; set; } = null!;
        public virtual AvailablePlane AssignedPlaneNavigation { get; set; } = null!;
        public virtual Airport DepartureAirport { get; set; } = null!;
        public virtual ICollection<FlightLog> FlightLogs { get; set; }
        public virtual ICollection<FlightReservation> FlightReservations { get; set; }
        public virtual ICollection<FlightSeatClass> FlightSeatClasses { get; set; }
    }
}
