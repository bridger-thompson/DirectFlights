using System.ComponentModel.DataAnnotations;

namespace DirectFlights.Client.Models
{
    public class FlightPlanModel
    {
        [Required]
        public DateTime DepartDate { get; set; } = DateTime.Now;
        [Required]
        public string DepartAirport { get; set; }
        [Required]
        public string ArriveAirport { get; set; }
    }
}
