using System.ComponentModel.DataAnnotations;

namespace DirectFlights.Client.Models
{
    public class RouteModel
    {
        [Required]
        public int FlightNumber { get; set; }
        [Required]
        public int SegmentNumber { get; set; }
        [Required]
        public int AirlineId { get; set; }
        [Required]
        public int DepartureAirportId { get; set; }
        [Required]
        public int ArrivalAirportId { get; set; }
        [Required]
        public TimeOnly TakeOffTime { get; set; } = TimeOnly.FromDateTime(DateTime.Now);
        [Required]
        public TimeOnly LandingTime { get; set; } = TimeOnly.FromDateTime(DateTime.Now.AddHours(2));
        [Required]
        public int PlaneTypeId { get; set; }
    }
}
