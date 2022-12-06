using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class SeatClass
    {
        public SeatClass()
        {
            FlightSeatClasses = new HashSet<FlightSeatClass>();
            PlaneTypeSeatClasses = new HashSet<PlaneTypeSeatClass>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<FlightSeatClass> FlightSeatClasses { get; set; }
        public virtual ICollection<PlaneTypeSeatClass> PlaneTypeSeatClasses { get; set; }
    }
}
