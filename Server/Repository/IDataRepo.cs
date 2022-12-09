using DirectFlights.Shared;

namespace DirectFlights.Server.Repository
{
    public interface IDataRepo
    {
        public Task<IEnumerable<FlightDetail>> GetFlights(string departAirport, string arriveAirport, string departDate);
        public Task<IEnumerable<string>> GetAirports();
        public Task<IEnumerable<FlightDetail>> GetAllFlightsOfId(int flightDetailId);
        public Task<Seat> GetSeat(int seatId);
        public Task<int> GetSeatId(string seatName);
        public Task<Passenger> GetPassenger(int passengerId);
        public Task CreatePassenger(Passenger passenger);
        public Task CreateFlightReservation(FlightReservation reservation);
        public Task<int> GetSeatClassId(int flightDetailId, int seatId);
    }
}
