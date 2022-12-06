using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class Airline
    {
        public Airline()
        {
            FlightScheduleTemplates = new HashSet<FlightScheduleTemplate>();
            Planes = new HashSet<Plane>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<FlightScheduleTemplate> FlightScheduleTemplates { get; set; }
        public virtual ICollection<Plane> Planes { get; set; }
    }
}
