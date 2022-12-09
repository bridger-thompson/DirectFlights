﻿using DirectFlights.Server.Data;
using DirectFlights.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

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
        public async Task<IEnumerable<string>> GetAirports()
        {
            var airports = await app.GetAirports();
            return airports;
        }

        [HttpGet("{flightDetailId}")]
        public async Task<FlightDetailDTO> GetFlight(int flightDetailId)
        {
            return await app.GetFlight(flightDetailId);
        }

        [HttpGet("total/{flightId}/{departDate}")]
        public async Task<IEnumerable<FlightTotal>> GetFlightTotal(int flightId, long departDate)
        {
            var date = new DateTime(departDate, DateTimeKind.Utc);
            return await app.GetFlightTotal(flightId, date);
        }
    }
}
