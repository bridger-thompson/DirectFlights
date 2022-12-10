using System.ComponentModel.DataAnnotations;

namespace DirectFlights.Client.Models
{
    public class FlightTotalModel
    {
        [Required]
        public DateTime DepartDate { get; set; } = DateTime.Parse("8/30/22").Date;
        [Required]
        public int NumIds { get; set; } = 5000;
    }
}
