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
        public async Task<IEnumerable<Airport>> GetAirports()
        {
            var airports = await context.Airports.ToListAsync();
            return airports;
        }

        public async Task<Airport> GetAirportById(int id)
        {
            var airport = await context.Airports.Where(a => a.Id == id).FirstOrDefaultAsync();
            return airport;
        }

        public async Task<IEnumerable<Airline>> GetAirlines()
        {
            var airlines = await context.Airlines.ToListAsync();
            return airlines;
        }

        public async Task<Airline> GetAirlineById(int id)
        {
            var airline = await context.Airlines.Where(a => a.Id == id).FirstOrDefaultAsync();
            return airline;
        }

        public async Task<IEnumerable<PlaneType>> GetPlaneTypes()
        {
            var types = await context.PlaneTypes.ToListAsync();
            return types;
        }

        public async Task<PlaneType> GetPlaneTypeById(int id)
        {
            var plane = await context.PlaneTypes.Where(a => a.Id == id).FirstOrDefaultAsync();
            return plane;
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

        public async Task<int> GetSeatId(string seatName)
        {
            var seat = await context.SeatClasses.Where(s => s.Name == seatName).FirstOrDefaultAsync();

            if (seat == null)
            {
                logger.LogError("Seat Class is null. Could not get seat: " + seatName);
            }
            return seat.Id;
        }

        public async Task<Passenger> GetPassenger(string name)
        {
            var existing = await context.Passengers.Where(f => f.Name == name).FirstOrDefaultAsync();
            Passenger passenger = new();
            if (existing == null)
            {
                logger.LogError($"Passenger doesn't exist with name {name}");
            } 
            else
            {
                passenger = existing;
            }
            return passenger;
        }

        public async Task<Passenger> CreatePassenger(Passenger passenger)
        {
            var existing = await context.Passengers.Where(f => f.Name == passenger.Name).FirstOrDefaultAsync();
            if(existing != null)
            {
                logger.LogError($"Passenger {passenger.Name} already exists");
            } 
            else
            {
                await context.Passengers.AddAsync(passenger);
                await context.SaveChangesAsync();
                Passenger newPassenger = await context.Passengers.Where(f => f.Name == passenger.Name).FirstOrDefaultAsync();
                return newPassenger;
            }
            return null;
        }

        public async Task CreateFlightReservation(FlightReservation reservation)
        {
            if(reservation != null)
            {
                try
                {
                    await context.FlightReservations.AddAsync(reservation);
                    await context.SaveChangesAsync();
                } 
                catch (Exception e)
                {
                    logger.LogError($"Flight reservation creation failed because of {e}");
                    throw;
                }
            }
        }

        public async Task<FlightSeatClass> GetSeatClass(int flightDetailId, int seatId)
        {
            var seatClass = await context.FlightSeatClasses.Where(s => s.SeatId == seatId && s.FlightId == flightDetailId).FirstOrDefaultAsync();

            if (seatClass == null)
            {
                logger.LogError("Flight Seat Class is null. Could not get seat: " + seatId + " With flight id: " + flightDetailId);
            }
            return seatClass;
        }

        public async Task<IEnumerable<FlightTotal>> GetFlightTotal(int upperLimit, DateTime departDate)
        {
            var table = await context.GetFlightTotals(upperLimit, departDate).ToListAsync();
            return table;
        }

        public async Task<FlightSchedule> GetFlightSchedule(int scheduleId)
        {
            var schedule = await context.FlightSchedules.Where(s => s.Id == scheduleId).FirstOrDefaultAsync();
            return schedule;
        }

        public async Task<IEnumerable<AirlineTotal>> GetAirlineTotal()
        {
            var totals = await context.AirlineTotals.ToListAsync();
            return totals;
        }

        public async Task<IEnumerable<FlightScheduleTemplate>> GetFlightScheduleTemplates()
        {
            var routes = await context.FlightScheduleTemplates
                .ToListAsync();
            return routes;
        }
    
        public async Task CreateFlightRoute(FlightScheduleTemplate template)
        {
            if(template != null)
            {
                await context.FlightScheduleTemplates.AddAsync(template);
                await context.SaveChangesAsync();
            }
        }
    }
}
