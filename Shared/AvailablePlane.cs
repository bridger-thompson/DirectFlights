using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class AvailablePlane
    {
        public AvailablePlane()
        {
            FlightSchedules = new HashSet<FlightSchedule>();
        }

        public int Id { get; set; }
        public int PlaneId { get; set; }
        public bool InMaintenence { get; set; }

        public virtual Plane Plane { get; set; } = null!;
        public virtual ICollection<FlightSchedule> FlightSchedules { get; set; }
    }
}
