using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class PlaneType
    {
        public PlaneType()
        {
            FlightScheduleTemplates = new HashSet<FlightScheduleTemplate>();
            PlaneTypeSeatClasses = new HashSet<PlaneTypeSeatClass>();
            Planes = new HashSet<Plane>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<FlightScheduleTemplate> FlightScheduleTemplates { get; set; }
        public virtual ICollection<PlaneTypeSeatClass> PlaneTypeSeatClasses { get; set; }
        public virtual ICollection<Plane> Planes { get; set; }
    }
}
