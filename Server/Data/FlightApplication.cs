using DirectFlights.Server.Repository;
using DirectFlights.Shared;

namespace DirectFlights.Server.Data
{
    public class FlightApplication
    {
        private readonly IDataRepo repo;

        public FlightApplication(IDataRepo repo)
        {
            this.repo = repo;
        }

        public async Task<IEnumerable<string>> GetAirports()
        {
            return await repo.GetAirports();
        }

        public async Task<IEnumerable<FlightDetail>> GetFlights(string departAirport, string arriveAirport, string departDate)
        {
            return await repo.GetFlights(departAirport, arriveAirport, departDate);
        }
    }
}
