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

        public async Task<IEnumerable<FlightDetail>> GetFlights(string departAirport, string arriveAirport, DateTime departDate)
        {
            try
            {
                return await client.GetFromJsonAsync<IEnumerable<FlightDetail>>($"api/Flight/{departAirport}/{arriveAirport}");
            }
            catch (AccessTokenNotAvailableException exception)
            {
                logger.LogError("Failed to retrieve flights. Access token not available: " + exception);
                return null;
            }
        }

        public async Task<IEnumerable<string>> GetAirports()
        {

            return await client.GetFromJsonAsync<IEnumerable<string>>("api/Flight/airports");
        }

        public async Task<string> RegisterPassengerFlight()
        {
            logger.LogInformation("Registers Passenger on flight");
            return null; 
        }

        public async Task<string> GetTotal(int flightDetailId, int seatId, int numTickets, double rate)
        {
            var flight = await client.GetFromJsonAsync<FlightDetail>($"api/Flight/{flightDetailId}");
            foreach (var seat in flight.Seats)
            {
                if (seat.Id == seatId)
                {
                    return (seat.Cost * rate * numTickets).ToString("C2");
                }
            }
            throw new Exception("Unable to get total cost");
        }

        public async Task AddTicketToDB(int flightDetailId, int seatId)
        {
            logger.LogInformation("Registered Ticket");
            logger.LogInformation("Email (not) sent");
        }
    }
}
