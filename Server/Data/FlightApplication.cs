using DirectFlights.Server.Repository;
using DirectFlights.Shared;
using Microsoft.AspNetCore.Http.Features;
using System.Runtime.Serialization.Formatters;

namespace DirectFlights.Server.Data
{
    public class FlightApplication
    {
        private readonly IDataRepo repo;
        private ILogger<FlightApplication> logger;

        public FlightApplication(IDataRepo repo)
        {
            this.repo = repo;
        }

        public async Task<IEnumerable<Airport>> GetAirports()
        {
            return await repo.GetAirports();
        }

        public async Task<IEnumerable<Airline>> GetAirlines()
        {
            return await repo.GetAirlines();
        }

        public async Task<IEnumerable<PlaneType>> GetPlaneTypes()
        {
            return await repo.GetPlaneTypes();
        }

        public async Task<IEnumerable<FlightScheduleTemplate>> GetFlightScheduleTemplates()
		{
            return await repo.GetFlightScheduleTemplates();
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
            var passengerExist = await repo.GetPassenger(passenger.Name);
            if (passengerExist.Name == null)
            {
                passenger = await repo.CreatePassenger(passenger);
            }
            else
            {
                passenger = passengerExist;
            }


            if(await repo.GetAllFlightsOfId(flightDetailId) != null)
            {
                int seatId = await repo.GetSeatId(seatName);
                Seat seat = await repo.GetSeat(seatId);
                FlightSeatClass flightSeatClass = await repo.GetSeatClass(flightDetailId, seatId);
                FlightSchedule flightSchedule = await repo.GetFlightSchedule(flightDetailId);

                if(passenger.Id != null) {
                    FlightReservation reservation = new()
                    {
                        PassengerId = (int)passenger.Id,
                        FlightScheduleId = flightSchedule.Id,
                        ClassId = flightSeatClass.Id,
                        ReservationDate = DateOnly.FromDateTime(DateTime.Now),
                        SeatCost = seat.Cost,
                        Class = flightSeatClass,
                        FlightSchedule = flightSchedule,
                        Passenger = passenger
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
                else
                {
                    logger.LogError("Passenger Id is null");
                }                
            }
        }

        public async Task<IEnumerable<FlightTotal>> GetFlightTotal(int upperLimit, DateTime departDate)
        {
            return await repo.GetFlightTotal(upperLimit, departDate);
        }

        public async Task<IEnumerable<AirlineTotal>> GetAirlineTotal()
        {
            return await repo.GetAirlineTotal();
        }

    }
}
