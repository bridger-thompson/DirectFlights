using DirectFlights.Shared;

namespace DirectFlights.Server.Repository
{
    public interface IDataRepo
    {
        public Task<IEnumerable<FlightDetail>> GetFlights(string departAirport, string arriveAirport, string departDate);
        public Task<IEnumerable<string>> GetAirports();
        public Task<IEnumerable<FlightDetail>> GetAllFlightsOfId(int flightDetailId);
        public Task<Seat> GetSeat(int seatId);
        public Task<IEnumerable<FlightTotal>> GetFlightTotal(int flightId, DateTime departDate);
    }
}
