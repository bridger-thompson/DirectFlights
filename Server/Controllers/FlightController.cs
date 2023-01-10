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
            logger.LogInformation("Getting flights for depart airport {departAirport}, arrive airport {arriveAirport}, and date {departDate}", departAirport, arriveAirport, departDate);
            var flights = await app.GetFlights(departAirport, arriveAirport, departDate);
            return flights;
        }

        [HttpGet("airports")]
        public async Task<IEnumerable<Airport>> GetAirports()
        {
            logger.LogInformation("Getting all airports");
            var airports = await app.GetAirports();
            return airports;
        }

        [HttpGet("airlines")]
        public async Task<IEnumerable<Airline>> GetAirlines()
        {
            logger.LogInformation("Getting all airlines");
            var airlines = await app.GetAirlines();
            return airlines;
        }

        [HttpGet("planeTypes")]
        public async Task<IEnumerable<PlaneType>> GetPlaneTypes()
        {
            logger.LogInformation("Getting plane types");
            var planeType = await app.GetPlaneTypes();
            return planeType;
        }

        [HttpGet("{flightDetailId}")]
        public async Task<FlightDetailDTO> GetFlight(int flightDetailId)
        {
            logger.LogInformation("Getting flight details for id {flightDetailId}", flightDetailId);
            return await app.GetFlight(flightDetailId);
        }

        [HttpPost("{flightDetailId}/{seatName}/{passenger}")]
        public async Task CreateReservation(int flightDetailId, string seatName, string passenger)
        {
            try
            {
                logger.LogInformation("Attempting to create reservation for flight id {flightDetailId} for passenger {passenger}", flightDetailId, passenger);
                Passenger user = new() { Id = null, Name = passenger };
                await app.CreateReservation(flightDetailId, seatName, user);
                logger.LogInformation("Created reservation");

            }
            catch
            {
                logger.LogError("Failed to create reservation for flight id {flightDetailId} for passenger {passenger}", flightDetailId, passenger);
                throw;
            }
        }

        [HttpGet("total/{upperLimit}/{departDate}")]
        public async Task<IEnumerable<FlightTotal>> GetFlightTotal(int upperLimit, long departDate)
        {
            var date = new DateTime(departDate, DateTimeKind.Utc);
            logger.LogInformation("Getting flight totals for date {date}", date.ToString());
            return await app.GetFlightTotal(upperLimit, date);
        }

        [HttpGet("total/airlines")]
        public async Task<IEnumerable<AirlineTotal>> GetAirlineTotal()
        {
            logger.LogInformation("Getting airline totals");
            var totals = await app.GetAirlineTotal();
            return totals;
        }

        [HttpGet("routes")]
        public async Task<IEnumerable<FlightScheduleTemplate>> GetFlightScheduleTemplates()
		{
            logger.LogInformation("Getting flight schedule template");
            var routes = await app.GetFlightScheduleTemplates();
            return routes;
		}

        [HttpPost("newroute/{schedule}")]
        public async Task CreateNewFlightRoute(string schedule)
        {
            logger.LogInformation("Creating new flight route for schedule {schedule}", schedule);
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
