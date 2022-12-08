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

        public async Task<IEnumerable<FlightDetailDTO>> GetFlights(string departAirport, string arriveAirport, string departDate)
        {
            List<FlightDetailDTO> flightDetails = new();
            var flights = await repo.GetFlights(departAirport, arriveAirport, departDate);
            foreach (var flight in flights)
            {
                Seat seat = await repo.GetSeat(flight.SeatId);
                if (!flightDetails.Any(f => f.Id == flight.Id))
                {
                    FlightDetailDTO f = new()
                    {
                        Id = flight.Id,
                        Airline = flight.Airline,
                        FromAirport = flight.FromAirport,
                        DepartureDate = flight.DepartureDate,
                        ToAirport = flight.ToAirport,
                        ArrivalDate = flight.ArrivalDate,
                        SeatClasses = new(),
                    };
                    f.SeatClasses.Add(seat);
                    flightDetails.Add(f);
                }
                else
                {
                    foreach (var f in flightDetails)
                    {
                        if (f.Id == flight.Id && !f.SeatClasses.Contains(seat))
                        {
                            f.SeatClasses.Add(seat);
                        }
                    }
                }
            }
            return flightDetails;
        }

        public async Task<FlightDetailDTO> GetFlight(int flightDetailId)
        {
            FlightDetailDTO flightDetail = null;
            var flights = await repo.GetAllFlightsOfId(flightDetailId);
            foreach (var flight in flights)
            {
                Seat seat = await repo.GetSeat(flight.SeatId);
                if (flightDetail == null)
                {
                    FlightDetailDTO f = new()
                    {
                        Id = flight.Id,
                        Airline = flight.Airline,
                        FromAirport = flight.FromAirport,
                        DepartureDate = flight.DepartureDate,
                        ToAirport = flight.ToAirport,
                        ArrivalDate = flight.ArrivalDate,
                        SeatClasses = new(),
                    };
                    f.SeatClasses.Add(seat);
                    flightDetail = f;
                }
                else
                {
                    if (flight.Id == flightDetail.Id && !flightDetail.SeatClasses.Contains(seat))
                    {
                        flightDetail.SeatClasses.Add(seat);
                    }
                }
            }
            return flightDetail;
        }

        public async Task<IEnumerable<FlightTotal>> GetFlightTotal(int flightId, DateTime departDate)
        {
            return await repo.GetFlightTotal(flightId, departDate);
        }

    }
}
