using DirectFlights.Server.Data;
using DirectFlights.Shared;
using Microsoft.EntityFrameworkCore;

namespace DirectFlights.Server.Repository
{
    public class FlightRepo : IDataRepo
    {
        private readonly FlightDBContext context;
        private readonly ILogger<FlightRepo> logger;
        private readonly ILogger factoryLogger;

        public FlightRepo(FlightDBContext context, ILogger<FlightRepo> logger, ILoggerFactory loggerFactory)
        {
            this.context = context;
            this.logger = logger;
            factoryLogger = loggerFactory.CreateLogger("DataAccessLayer");
        }
        public async Task<IEnumerable<Airport>> GetAirports()
        {
            var airports = await context.Airports.ToListAsync();
            factoryLogger.LogDebug("Retrieved airports: {airports}", airports);
            return airports;
        }

        public async Task<Airport> GetAirportById(int id)
        {
            var airport = await context.Airports.Where(a => a.Id == id).FirstOrDefaultAsync();
            factoryLogger.LogDebug("Retrieved airport by id {id}: {airport}", id, airport);
            return airport;
        }

        public async Task<IEnumerable<Airline>> GetAirlines()
        {
            var airlines = await context.Airlines.ToListAsync();
            factoryLogger.LogDebug("Retrieved airlines: {airlines}", airlines);
            return airlines;
        }

        public async Task<Airline> GetAirlineById(int id)
        {
            var airline = await context.Airlines.Where(a => a.Id == id).FirstOrDefaultAsync();
            factoryLogger.LogDebug("Retrieved airline by id {id}: {airline}", id, airline);
            return airline;
        }

        public async Task<IEnumerable<PlaneType>> GetPlaneTypes()
        {
            var types = await context.PlaneTypes.ToListAsync();
            factoryLogger.LogDebug("Retrieved plane types: {types}", types);
            return types;
        }

        public async Task<PlaneType> GetPlaneTypeById(int id)
        {
            var plane = await context.PlaneTypes.Where(a => a.Id == id).FirstOrDefaultAsync();
            factoryLogger.LogDebug("Retrieved plane by id {id}: {plane}", id, plane);
            return plane;
        }

        public async Task<IEnumerable<FlightDetail>> GetAllFlightsOfId(int flightDetailId)
        {
            var flights = await context.FlightDetails.Where(f => f.Id == flightDetailId).ToListAsync();
            factoryLogger.LogDebug("Retrieved flight details by id {id}: {flights}", flightDetailId, flights);
            return flights;
        }

        public async Task<IEnumerable<FlightDetail>> GetFlights(string departAirport, string arriveAirport, string departDate)
        {
            var date = DateTime.Parse(departDate);
            var flights = await context.FlightDetails
                .Where(flight => flight.FromAirport == departAirport && flight.ToAirport == arriveAirport && flight.DepartureDate.Date == date.Date)
                .ToListAsync();
            factoryLogger.LogDebug("Retrieved flights with depart Airport {departAirport}, arrive Airport {arriveAirport}, and depart Date {date}", departAirport, arriveAirport, date.ToString());
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
                factoryLogger.LogDebug("Retrieved seat by id {seatId}: {seat}", seatId, seat);
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
            factoryLogger.LogDebug("Retrieved seat id by name {seatName}: {id}", seatName, seat.Id);
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
                factoryLogger.LogDebug("Found passenger with name {name}: {passenger}", name, passenger);
            }
            return passenger;
        }

        public async Task<Passenger> CreatePassenger(Passenger passenger)
        {
            var existing = await context.Passengers.Where(f => f.Name == passenger.Name).FirstOrDefaultAsync();
            if (existing != null)
            {
                logger.LogError($"Passenger {passenger.Name} already exists");
            }
            else
            {
                await context.Passengers.AddAsync(passenger);
                await context.SaveChangesAsync();
                factoryLogger.LogInformation("Created new passenger: {passenger}", passenger);
                Passenger newPassenger = await context.Passengers.Where(f => f.Name == passenger.Name).FirstOrDefaultAsync();
                return newPassenger;
            }
            return null;
        }

        public async Task CreateFlightReservation(FlightReservation reservation)
        {
            if (reservation != null)
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
            if (template != null)
            {
                await context.FlightScheduleTemplates.AddAsync(template);
                await context.SaveChangesAsync();
            }
        }
    }
}
