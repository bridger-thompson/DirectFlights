using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class Airport
    {
        public Airport()
        {
            FlightLogArrivalAirports = new HashSet<FlightLog>();
            FlightLogDepartAirports = new HashSet<FlightLog>();
            FlightScheduleArrivalAirports = new HashSet<FlightSchedule>();
            FlightScheduleDepartureAirports = new HashSet<FlightSchedule>();
            FlightScheduleTemplateArrivalAirports = new HashSet<FlightScheduleTemplate>();
            FlightScheduleTemplateDepartureAirports = new HashSet<FlightScheduleTemplate>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;

        public virtual ICollection<FlightLog> FlightLogArrivalAirports { get; set; }
        public virtual ICollection<FlightLog> FlightLogDepartAirports { get; set; }
        public virtual ICollection<FlightSchedule> FlightScheduleArrivalAirports { get; set; }
        public virtual ICollection<FlightSchedule> FlightScheduleDepartureAirports { get; set; }
        public virtual ICollection<FlightScheduleTemplate> FlightScheduleTemplateArrivalAirports { get; set; }
        public virtual ICollection<FlightScheduleTemplate> FlightScheduleTemplateDepartureAirports { get; set; }
    }
}
