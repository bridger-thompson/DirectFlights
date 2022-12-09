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

        public async Task CreateReservation(int flightDetailId, string seatName, Passenger passenger)
        {
            if(await repo.GetPassenger(passenger.Id) == null)
            {
                await repo.CreatePassenger(passenger);
            }

            if(await repo.GetAllFlightsOfId(flightDetailId) != null)
            {
                int seatId = await repo.GetSeatId(seatName);
                Seat seat = await repo.GetSeat(seatId);

                FlightReservation reservation = new()
                {
                    PassengerId = passenger.Id,
                    FlightScheduleId = flightDetailId,
                    ClassId = await repo.GetSeatClassId(flightDetailId, seatId),
                    ReservationDate = DateOnly.FromDateTime(DateTime.Now),
                    SeatCost = seat.Cost
                };
                try
                {
                    await repo.CreateFlightReservation(reservation);
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<FlightTotal>> GetFlightTotal(int upperLimit, DateTime departDate)
        {
            return await repo.GetFlightTotal(upperLimit, departDate);
        }

    }
}
