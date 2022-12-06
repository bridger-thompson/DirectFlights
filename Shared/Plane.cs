using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class Plane
    {
        public Plane()
        {
            AvailablePlanes = new HashSet<AvailablePlane>();
        }

        public int Id { get; set; }
        public int PlaneTypeId { get; set; }
        public int AirlineId { get; set; }

        public virtual Airline Airline { get; set; } = null!;
        public virtual PlaneType PlaneType { get; set; } = null!;
        public virtual ICollection<AvailablePlane> AvailablePlanes { get; set; }
    }
}
