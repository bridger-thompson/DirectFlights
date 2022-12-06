using System.ComponentModel.DataAnnotations;

namespace DirectFlights.Client.Models
{
    public class FlightPlanModel
    {
        [Required]
        public DateTime DepartDate { get; set; } = DateTime.Now;
        [Required]
        public string FromAirport { get; set; }
        [Required]
        public string ToAirport { get; set; }
    }
}
