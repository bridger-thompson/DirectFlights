using DirectFlights.Server.Repository;
using DirectFlights.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDirectFlights
{
    public class TestRepo : IDataRepo
    {
        public Task CreateFlightReservation(FlightReservation reservation)
        {
            throw new NotImplementedException();
        }

        public Task<Passenger> CreatePassenger(Passenger passenger)
        {
            throw new NotImplementedException();
        }
		public Task<IEnumerable<Airline>> GetAirlines()
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<AirlineTotal>> GetAirlineTotal()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Airport>> GetAirports()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FlightDetail>> GetAllFlightsOfId(int flightDetailId)
        {
            List<FlightDetail> flights = new()
            {
                new FlightDetail()
                {
                    Id = 1,
                    Airline = "Airline1",
                    FromAirport = "Airport1",
                    DepartureDate = DateTime.Now,
                    ToAirport = "Airport2",
                    ArrivalDate = DateTime.Now.AddHours(2),
                    SeatId = 1,
                },
                new FlightDetail()
                {
                    Id = 1,
                    Airline = "Airline1",
                    FromAirport = "Airport1",
                    DepartureDate = DateTime.Now,
                    ToAirport = "Airport2",
                    ArrivalDate = DateTime.Now.AddHours(2),
                    SeatId = 2,
                },
                new FlightDetail()
                {
                    Id = 1,
                    Airline = "Airline1",
                    FromAirport = "Airport1",
                    DepartureDate = DateTime.Now,
                    ToAirport = "Airport2",
                    ArrivalDate = DateTime.Now.AddHours(2),
                    SeatId = 3,
                },
                new FlightDetail()
                {
                    Id = 2,
                    Airline = "Airline2",
                    FromAirport = "Airport5",
                    DepartureDate = DateTime.Now,
                    ToAirport = "Airport6",
                    ArrivalDate = DateTime.Now.AddHours(2),
                    SeatId = 1,
                },
                new FlightDetail()
                {
                    Id = 2,
                    Airline = "Airline2",
                    FromAirport = "Airport5",
                    DepartureDate = DateTime.Now,
                    ToAirport = "Airport6",
                    ArrivalDate = DateTime.Now.AddHours(2),
                    SeatId = 2,
                },
            };
            return flights.Where(f => f.Id == flightDetailId);
        }

        public async Task<IEnumerable<FlightDetail>> GetFlights(string departAirport, string arriveAirport, string departDate)
        {
            var date = DateTime.Parse(departDate);
            List<FlightDetail> flights = new()
            {
                new FlightDetail()
                {
                    Id = 1,
                    Airline = "Airline1",
                    FromAirport = departAirport,
                    DepartureDate = date,
                    ToAirport = arriveAirport,
                    ArrivalDate = date.AddHours(2),
                    SeatId = 1,
                },
                new FlightDetail()
                {
                    Id = 1,
                    Airline = "Airline1",
                    FromAirport = departAirport,
                    DepartureDate = date,
                    ToAirport = arriveAirport,
                    ArrivalDate = date.AddHours(2),
                    SeatId = 2,
                },
                new FlightDetail()
                {
                    Id = 1,
                    Airline = "Airline1",
                    FromAirport = departAirport,
                    DepartureDate = date,
                    ToAirport = arriveAirport,
                    ArrivalDate = date.AddHours(2),
                    SeatId = 3,
                },
                new FlightDetail()
                {
                    Id = 2,
                    Airline = "Airline2",
                    FromAirport = departAirport,
                    DepartureDate = date,
                    ToAirport = arriveAirport,
                    ArrivalDate = date.AddHours(2),
                    SeatId = 1,
                },
                new FlightDetail()
                {
                    Id = 2,
                    Airline = "Airline2",
                    FromAirport = departAirport,
                    DepartureDate = date,
                    ToAirport = arriveAirport,
                    ArrivalDate = date.AddHours(2),
                    SeatId = 2,
                },
                new FlightDetail()
                {
                    Id = 2,
                    Airline = "Airline2",
                    FromAirport = departAirport,
                    DepartureDate = date,
                    ToAirport = arriveAirport,
                    ArrivalDate = date.AddHours(2),
                    SeatId = 3,
                },
            };
            return flights;
        }

        public Task<FlightSchedule> GetFlightSchedule(int scheduleId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FlightScheduleTemplate>> GetFlightScheduleTemplates()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FlightTotal>> GetFlightTotal(int flightId, DateTime departDate)
        {
            throw new NotImplementedException();
        }

        public Task<Passenger> GetPassenger(string name)
        {
            throw new NotImplementedException();
        }

		public Task<IEnumerable<PlaneType>> GetPlaneTypes()
		{
			throw new NotImplementedException();
		}

		public async Task<Seat> GetSeat(int seatId)
        {
            List<Seat> seats = new()
            {
                new Seat()
                {
                    Name = "First Class",
                    Cost = 250
                },
                new Seat()
                {
                    Name = "Business Class",
                    Cost = 200
                },
                new Seat()
                {
                    Name = "Couch Class",
                    Cost = 150
                }
            };
            if (seatId > 0 && seatId < 4)
            {
                return seats[seatId-1];
            }
            else return null;
        }

        public Task<FlightSeatClass> GetSeatClass(int flightDetailId, int seatId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetSeatId(string seatName)
        {
            throw new NotImplementedException();
        }
    }
}
