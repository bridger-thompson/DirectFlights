using DirectFlights.Server.Data;
using DirectFlights.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Collections.Specialized;
using System.Numerics;
using System.Web;

namespace DirectFlights.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes")]
    [Authorize]
    public class FlightController : ControllerBase
    {
        private readonly ILogger<FlightController> logger;
        private readonly FlightApplication app;
        public FlightController(ILogger<FlightController> logger, FlightApplication app)
        {
            this.logger = logger;
            this.app = app;
        }

        [HttpGet("{departAirport}/{arriveAirport}/{departDate}")]
        public async Task<IEnumerable<FlightDetailDTO>> GetFlights(string departAirport, string arriveAirport, string departDate)
        {
            var flights = await app.GetFlights(departAirport, arriveAirport, departDate);
            return flights;
        }

        [HttpGet("airports")]
        public async Task<IEnumerable<Airport>> GetAirports()
        {
            var airports = await app.GetAirports();
            return airports;
        }

        [HttpGet("airlines")]
        public async Task<IEnumerable<Airline>> GetAirlines()
        {
            var airlines = await app.GetAirlines();
            return airlines;
        }

        [HttpGet("planeTypes")]
        public async Task<IEnumerable<PlaneType>> GetPlaneTypes()
        {
            var planeType = await app.GetPlaneTypes();
            return planeType;
        }

        [HttpGet("{flightDetailId}")]
        public async Task<FlightDetailDTO> GetFlight(int flightDetailId)
        {
            return await app.GetFlight(flightDetailId);
        }

        [HttpPost("{flightDetailId}/{seatName}/{passenger}")]
        public async Task CreateReservation(int flightDetailId, string seatName, string passenger)
        {
            try
            {
                Passenger user = new() { Id = null, Name = passenger };

                await app.CreateReservation(flightDetailId, seatName, user);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("total/{upperLimit}/{departDate}")]
        public async Task<IEnumerable<FlightTotal>> GetFlightTotal(int upperLimit, long departDate)
        {
            var date = new DateTime(departDate, DateTimeKind.Utc);
            return await app.GetFlightTotal(upperLimit, date);
        }

        [HttpGet("total/airlines")]
        public async Task<IEnumerable<AirlineTotal>> GetAirlineTotal()
        {
            var totals = await app.GetAirlineTotal();
            return totals;
        }

        [HttpGet("routes")]
        public async Task<IEnumerable<FlightScheduleTemplate>> GetFlightScheduleTemplates()
		{
            var routes = await app.GetFlightScheduleTemplates();
            return routes;
		}

        [HttpPost("newroute/{schedule}")]
        public async Task CreateNewFlightRoute(string schedule)
        {
            NameValueCollection values = HttpUtility.ParseQueryString(schedule);
            var dictionary = values.AllKeys.ToDictionary(k => k, k => values[k]);
            
            int flightNumber = Int32.Parse(dictionary["FlightNumber"]);
            int segmentNumber = Int32.Parse(dictionary["SegmentNumber"]);
            int airlineId = Int32.Parse(dictionary["AirlineId"]);
            int departureAirportId = Int32.Parse(dictionary["DepartureAirportId"]);
            int arrivalAirportId = Int32.Parse(dictionary["ArrivalAirportId"]);
            TimeOnly takeoffTime = TimeOnly.Parse(dictionary["TakeOffTime"]);
            TimeOnly landingTime = TimeOnly.Parse(dictionary["LandingTime"]);
            int planeTypeId = Int32.Parse(dictionary["PlaneTypeId"]);

            await app.CreateNewRoute(flightNumber, segmentNumber, 
                airlineId, departureAirportId, arrivalAirportId,
                takeoffTime, landingTime, planeTypeId);
        }
    }
}
