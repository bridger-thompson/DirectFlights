using DirectFlights.Server.Data;
using DirectFlights.Shared;
using NUnit.Framework;
using System.Collections.Generic;
using FluentAssertions;
using System.Linq;
using System.Threading.Tasks;

namespace TestDirectFlights
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestGetFlights()
        {
            var testRepo = new TestRepo();
            var app = new FlightApplication(testRepo);
            Assert.IsNotNull(app);
            IEnumerable<FlightDetailDTO> flights = await app.GetFlights("airport1", "airport2", "2022/12/6");
            flights.Count().Should().Be(2);
            foreach (var flight in flights)
            {
                flight.SeatClasses.Count.Should().Be(3);
            }
        }

        [Test]
        public async Task TestGetFlight()
        {
            var testRepo = new TestRepo();
            var app = new FlightApplication(testRepo);
            Assert.IsNotNull(app);
            FlightDetailDTO flight = await app.GetFlight(1);
            flight.SeatClasses.Count.Should().Be(3);

            flight = await app.GetFlight(2);
            flight.SeatClasses.Count.Should().Be(2);
        }
    }
}