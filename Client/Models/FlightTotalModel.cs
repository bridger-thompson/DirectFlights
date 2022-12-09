using System.ComponentModel.DataAnnotations;

namespace DirectFlights.Client.Models
{
    public class FlightTotalModel
    {
        [Required]
        public DateTime DepartDate { get; set; } = DateTime.Now.Date;
        [Required]
        public int NumIds { get; set; } = 500;
    }
}
