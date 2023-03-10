using DirectFlights.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using System.Web;

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

        public async Task<IEnumerable<Airport>> GetAirports()
        {

            var airports = await client.GetFromJsonAsync<IEnumerable<Airport>>("api/Flight/airports");
            return airports;
        }

        public async Task<IEnumerable<Airline>> GetAirlines()
        {

            var airlines = await client.GetFromJsonAsync<IEnumerable<Airline>>("api/Flight/airlines");
            return airlines;
        }

        public async Task<IEnumerable<PlaneType>> GetPlaneTypes()
        {

            var planeTypes = await client.GetFromJsonAsync<IEnumerable<PlaneType>>("api/Flight/planeTypes");
            return planeTypes;
        }

        public async Task<IEnumerable<FlightScheduleTemplate>> GetFlightScheduleTemplates()
		{
            var routes = await client.GetFromJsonAsync<IEnumerable<FlightScheduleTemplate>>("api/Flight/routes");
            return routes;
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

        public async Task AddTicketToDB(int flightDetailId, string seatName, string passenger)
        {

            try
            {
                await client.PostAsync($"/api/Flight/{flightDetailId}/{seatName}/{passenger}", null);
                logger.LogInformation("Registered Ticket");
            }
            catch
            {
                throw;
            }

            
        }

        public async Task SendEmail(string toAddress)
        {
            toAddress = toAddress.Replace("[\"", "");
            toAddress = toAddress.Replace("\"]", "");
            await client.PostAsync($"/api/Mail/{toAddress}", null);
        }

        public async Task<IEnumerable<FlightTotal>> GetFlightTotal(int upperLimit, DateTime departDate)
        {
            var totals = await client.GetFromJsonAsync<IEnumerable<FlightTotal>>($"api/Flight/total/{upperLimit}/{departDate.Ticks}");
            return totals;
        }

        public async Task<IEnumerable<AirlineTotal>> GetAirlineTotal()
        {
            var totals = await client.GetFromJsonAsync<IEnumerable<AirlineTotal>>($"api/Flight/total/airlines");
            return totals;
        }

        public async Task CreateNewFlightRoute(FlightScheduleTemplate schedule)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["FlightNumber"] = schedule.FlightNumber.ToString();
            queryString["SegmentNumber"] = schedule.SegmentNumber.ToString();
            queryString["AirlineId"] = schedule.AirlineId.ToString();
            queryString["DepartureAirportId"] = schedule.DepartureAirportId.ToString();
            queryString["ArrivalAirportId"] = schedule.ArrivalAirportId.ToString();
            queryString["TakeOffTime"] = schedule.TakeOffTime.ToString();
            queryString["LandingTime"] = schedule.LandingTime.ToString();
            queryString["PlaneTypeId"] = schedule.PlaneTypeId.ToString();

            var result = await client.PostAsync($"api/Flight/newroute/{queryString}", null);
        }
    }
}
