using DirectFlights.Server.Data;
using DirectFlights.Shared;
using Microsoft.EntityFrameworkCore;

namespace DirectFlights.Server.Repository
{
    public class FlightRepo : IDataRepo
    {
        private readonly FlightDBContext context;
        private readonly ILogger<FlightRepo> logger;

        public FlightRepo(FlightDBContext context, ILogger<FlightRepo> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public async Task<IEnumerable<string>> GetAirports()
        {
            var airports = await context.Airports.Select(airport => airport.Name).ToListAsync();
            return airports;
        }

        public async Task<IEnumerable<FlightDetail>> GetAllFlightsOfId(int flightDetailId)
        {
            return await context.FlightDetails.Where(f => f.Id == flightDetailId).ToListAsync();
        }

        public async Task<IEnumerable<FlightDetail>> GetFlights(string departAirport, string arriveAirport, string departDate)
        {
            var date = DateTime.Parse(departDate);
            var flights = await context.FlightDetails
                .Where(flight => flight.FromAirport == departAirport && flight.ToAirport == arriveAirport && flight.DepartureDate.Date == date.Date)
                .ToListAsync();
            return flights;
        }

        public async Task<Seat> GetSeat(int seatId)
        {
            var flight_seat = await context.FlightSeatClasses.Where(f => f.Id == seatId).FirstOrDefaultAsync();
            var seat_class = await context.SeatClasses.Where(s => s.Id == flight_seat.SeatId).FirstOrDefaultAsync();
            Seat seat = new();
            if (flight_seat == null)
            {
                logger.LogError("Flight Seat Class is null. Could not get seat: " + seatId);
            }
            else if (seat_class == null)
            {
                logger.LogError("Seat Class is null. Could not get seat: " + seatId);
            }
            else
            {
                seat = new Seat()
                {
                    Name = seat_class.Name,
                    Cost = flight_seat.SuggestedCost
                };
            }
            return seat;
        }

        public async Task<IEnumerable<FlightTotal>> GetFlightTotal(int flightId, DateTime departDate)
        {
            var table = await context.GetFlightTotals(flightId, departDate).ToListAsync();
            return table;
        }
    }
}
