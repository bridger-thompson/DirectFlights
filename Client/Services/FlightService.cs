using DirectFlights.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Json;

namespace DirectFlights.Client.Services
{
    public class FlightService
    {
        private readonly HttpClient client;
        private readonly ILogger<FlightService> logger;
        private List<FlightDetail> flights = new List<FlightDetail>()
            {
                new FlightDetail(){
                    Id = 1,
                    Airline = "Airline1",
                    DepartAirport = "SLC",
                    ArriveAirport = "LAX",
                    DepartTime = new DateTime(2022, 11, 19, 9, 30, 0),
                    ArriveTime = new DateTime(2022, 11, 19, 15, 30, 0),
                    Seats = new Seat[] {
                        new Seat() { Id = 1, Name = "First Class", Cost = 250 },
                        new Seat() { Id = 2, Name = "Business Class", Cost = 180 },
                        new Seat() { Id = 3, Name = "Couch Class", Cost = 150 }
                    }
                },
                new FlightDetail(){
                    Id = 2,
                    Airline = "Airline2",
                    DepartAirport = "SLC",
                    ArriveAirport = "LAX",
                    DepartTime = new DateTime(2022, 11, 19, 11, 30, 0),
                    ArriveTime = new DateTime(2022, 11, 19, 18, 0, 0),
                    Seats = new Seat[] {
                        new Seat() { Id = 1, Name = "First Class", Cost = 240 },
                        new Seat() { Id = 2, Name = "Business Class", Cost = 170 },
                        new Seat() { Id = 3, Name = "Couch Class", Cost = 190 }
                    }
                },
                new FlightDetail(){
                    Id = 3,
                    Airline = "Airline3",
                    DepartAirport = "SLC",
                    ArriveAirport = "LAX",
                    DepartTime = new DateTime(2022, 11, 19, 15, 30, 0),
                    ArriveTime = new DateTime(2022, 11, 20, 2, 30, 0),
                    Seats = new Seat[] {
                        new Seat() { Id = 1, Name = "First Class", Cost = 200 },
                        new Seat() { Id = 2, Name = "Business Class", Cost = 130 },
                        new Seat() { Id = 3, Name = "Couch Class", Cost = 100 }
                    }
                }
            };

        public FlightService(HttpClient client, ILogger<FlightService> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        public async Task<IEnumerable<FlightDetail>> GetFlights(string departAirport, string arriveAirport, DateTime departDate)
        {
            try
            {
                return await client.GetFromJsonAsync<IEnumerable<FlightDetail>>($"api/Flight/flights/{departAirport}/{arriveAirport}");
            }
            catch (AccessTokenNotAvailableException exception)
            {
                logger.LogError("Failed to retrieve flights. Access token not available: " + exception);
                return null;
            }
            //var ourFlights = new List<FlightDetail>();
            //foreach (var flight in flights)
            //{
            //    if (flight.DepartAirport == departAirport && flight.ArriveAirport == arriveAirport /*&& flight.DepartTime.DayOfYear == departDate.DayOfYear*/)
            //    {
            //        ourFlights.Add(flight);
            //    }
            //}
            //return ourFlights;
        }

        public async Task<IEnumerable<string>> GetAirports()
        {

            var airports = new List<string>()
            {
                "SLC", "LAX", "PHX"
            };
            return airports;
        }

        public async Task<string> RegisterPassengerFlight()
        {
            logger.LogInformation("Registers Passenger on flight");
            return null; 
        }

        public async Task<string> GetTotal(int flightDetailId, int seatId, int numTickets, double rate)
        {
            foreach (var flight in flights)
            {
                if (flight.Id == flightDetailId)
                {
                    foreach (var seat in flight.Seats)
                    {
                        if (seat.Id == seatId)
                        {
                            return (seat.Cost * rate * numTickets).ToString("C2");
                        }
                    }
                }
            }
            throw new Exception("Ticket Cost not found");
        }

        public async Task AddTicketToDB(int flightDetailId, int seatId)
        {
            logger.LogInformation("Registered Ticket");
            logger.LogInformation("Email (not) sent");
        }
    }
}
