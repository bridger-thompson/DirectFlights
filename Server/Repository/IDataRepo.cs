using DirectFlights.Shared;

namespace DirectFlights.Server.Repository
{
    public interface IDataRepo
    {
        public Task<IEnumerable<FlightDetail>> GetFlights(string departAirport, string arriveAirport, string departDate);
        public Task<IEnumerable<string>> GetAirports();
        public Task<FlightDetail> GetFlight(int flightDetailId);
    }
}
