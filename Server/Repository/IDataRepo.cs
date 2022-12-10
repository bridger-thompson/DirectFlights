using DirectFlights.Shared;

namespace DirectFlights.Server.Repository
{
    public interface IDataRepo
    {
        public Task<IEnumerable<FlightDetail>> GetFlights(string departAirport, string arriveAirport, string departDate);
        public Task<IEnumerable<Airport>> GetAirports();
        public Task<IEnumerable<FlightDetail>> GetAllFlightsOfId(int flightDetailId);
        public Task<Seat> GetSeat(int seatId);
        public Task<IEnumerable<FlightTotal>> GetFlightTotal(int upperLimit, DateTime departDate);
        public Task<IEnumerable<AirlineTotal>> GetAirlineTotal();
        public Task<IEnumerable<Airline>> GetAirlines();
        public Task<IEnumerable<PlaneType>> GetPlaneTypes();
        public Task<IEnumerable<FlightScheduleTemplate>> GetFlightScheduleTemplates();
    }
}
