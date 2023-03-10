using DirectFlights.Shared;

namespace DirectFlights.Server.Repository
{
    public interface IDataRepo
    {
        public Task<IEnumerable<FlightDetail>> GetFlights(string departAirport, string arriveAirport, string departDate);
        public Task<IEnumerable<Airport>> GetAirports();
        public Task<Airport> GetAirportById(int id);
        public Task<IEnumerable<FlightDetail>> GetAllFlightsOfId(int flightDetailId);
        public Task<Seat> GetSeat(int seatId);
        public Task<IEnumerable<FlightTotal>> GetFlightTotal(int upperLimit, DateTime departDate);
        public Task<int> GetSeatId(string seatName);
        public Task<Passenger> GetPassenger(string name);
        public Task<Passenger> CreatePassenger(Passenger passenger);
        public Task CreateFlightReservation(FlightReservation reservation);
        public Task<FlightSeatClass> GetSeatClass(int flightDetailId, int seatId);
        public Task<FlightSchedule> GetFlightSchedule(int scheduleId);
        public Task<IEnumerable<AirlineTotal>> GetAirlineTotal();
        public Task<IEnumerable<Airline>> GetAirlines();
        public Task<Airline> GetAirlineById(int id);
        public Task<IEnumerable<PlaneType>> GetPlaneTypes();
        public Task<PlaneType> GetPlaneTypeById(int id);
        public Task<IEnumerable<FlightScheduleTemplate>> GetFlightScheduleTemplates();
        public Task CreateFlightRoute(FlightScheduleTemplate template);
    }
}
