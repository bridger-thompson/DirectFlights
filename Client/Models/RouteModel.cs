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
        public DateTime TakeOffTime { get; set; } = DateTime.Now;
        [Required]
        public DateTime LandingTime { get; set; } = DateTime.Now.AddHours(2);
        [Required]
        public int PlaneTypeId { get; set; }
    }
}
