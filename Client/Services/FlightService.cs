using DirectFlights.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Json;

namespace DirectFlights.Client.Services
{
    public class FlightService
    {
        private readonly HttpClient client;
        private readonly ILogger<FlightService> logger;

        public FlightService(HttpClient client, ILogger<FlightService> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        public async Task<IEnumerable<FlightDetailDTO>> GetFlights(string departAirport, string arriveAirport, DateTime departDate)
        {
            try
            {
                return await client.GetFromJsonAsync<IEnumerable<FlightDetailDTO>>($"api/Flight/{departAirport}/{arriveAirport}/{departDate.ToLongDateString()}");
            }
            catch (AccessTokenNotAvailableException exception)
            {
                logger.LogError("Failed to retrieve flights. Access token not available: " + exception);
                return null;
            }
        }

        public async Task<IEnumerable<string>> GetAirports()
        {

            var airports = await client.GetFromJsonAsync<IEnumerable<string>>("api/Flight/airports");
            return airports;
        }

        public async Task<string> RegisterPassengerFlight()
        {
            logger.LogInformation("Registers Passenger on flight");
            return null; 
        }

        public async Task<string> GetTotal(int flightDetailId, string seatName, int numTickets, decimal rate)
        {
            var flight = await client.GetFromJsonAsync<FlightDetailDTO>($"api/Flight/{flightDetailId}");
            foreach (var seat in flight.SeatClasses)
            {
                if (seat.Name == seatName)
                {
                    return (seat.Cost * rate * numTickets).ToString("C2");
                }
            }
            throw new Exception("Unable to get total cost");
        }

        public async Task AddTicketToDB(int flightDetailId, string seatName)
        {
            logger.LogInformation("Registered Ticket");
            logger.LogInformation("Email (not) sent");
        }

        public async Task SendEmail(string toAddress)
        {
            toAddress = toAddress.Replace("[\"", "");
            toAddress = toAddress.Replace("\"]", "");
            await client.PostAsync($"/api/Mail/{toAddress}", null);
        }

        public async Task<IEnumerable<FlightTotal>> GetFlightTotal(int flightId, DateTime departDate)
        {
            var totals = await client.GetFromJsonAsync<IEnumerable<FlightTotal>>($"api/Flight/total/{flightId}/{departDate.ToUniversalTime().ToLongDateString()}");
            return totals;
        }
    }
}
