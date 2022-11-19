using DirectFlights.Shared;

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
            var flights = new List<FlightDetail>()
            {
                new FlightDetail(){
                    Airline = "Airline1",
                    DepartAirport = "SLC",
                    ArriveAirport = "LAX",
                    DepartTime = new DateTime(2022, 11, 19, 9, 30, 0),
                    ArriveTime = new DateTime(2022, 11, 19, 15, 30, 0),
                    Seats = new Seat[] {
                        new Seat() { Name = "First Class", Cost = 250 },
                        new Seat() { Name = "Business Class", Cost = 180 },
                        new Seat() { Name = "Couch Class", Cost = 150 }
                    }
                },
                new FlightDetail(){
                    Airline = "Airline2",
                    DepartAirport = "SLC",
                    ArriveAirport = "LAX",
                    DepartTime = new DateTime(2022, 11, 19, 11, 30, 0),
                    ArriveTime = new DateTime(2022, 11, 19, 18, 0, 0),
                    Seats = new Seat[] {
                        new Seat() { Name = "First Class", Cost = 240 },
                        new Seat() { Name = "Business Class", Cost = 170 },
                        new Seat() { Name = "Couch Class", Cost = 190 }
                    }
                },
                new FlightDetail(){
                    Airline = "Airline3",
                    DepartAirport = "SLC",
                    ArriveAirport = "LAX",
                    DepartTime = new DateTime(2022, 11, 19, 15, 30, 0),
                    ArriveTime = new DateTime(2022, 11, 20, 2, 30, 0),
                    Seats = new Seat[] {
                        new Seat() { Name = "First Class", Cost = 200 },
                        new Seat() { Name = "Business Class", Cost = 130 },
                        new Seat() { Name = "Couch Class", Cost = 100 }
                    }
                }
            };
            var ourFlights = new List<FlightDetail>();
            foreach (var flight in flights)
            {
                if (flight.DepartAirport == departAirport && flight.ArriveAirport == arriveAirport && flight.DepartTime.DayOfYear == departDate.DayOfYear)
                {
                    ourFlights.Add(flight);
                }
            }
            return ourFlights;
        }

        public async Task<IEnumerable<string>> GetAirports()
        {

            var airports = new List<string>()
            {
                "SLC", "LAX", "PHX"
            };
            return airports;
        }
    }
}
