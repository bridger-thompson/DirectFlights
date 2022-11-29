﻿using DirectFlights.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace DirectFlights.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes")]
    public class FlightController : ControllerBase
    {
        private readonly ILogger<FlightController> logger;
        private List<FlightDetail> flights = new()
        {
            new FlightDetail(){
                Id = 1,
                Airline = "Airline1",
                DepartAirport = "SLC",
                ArriveAirport = "LAX",
                DepartTime = new DateTime(2022, 11, 19, 9, 30, 0),
                ArriveTime = new DateTime(2022, 11, 19, 15, 30, 0),
                Seats = new Seat[] {
                    new Seat() { Id = 1, Name = "First Class", Cost = 250 },
                    new Seat() { Id = 2, Name = "Business Class", Cost = 180 },
                    new Seat() { Id = 3, Name = "Couch Class", Cost = 150 }
                }
            },
            new FlightDetail(){
                Id = 2,
                Airline = "Airline2",
                DepartAirport = "SLC",
                ArriveAirport = "LAX",
                DepartTime = new DateTime(2022, 11, 19, 11, 30, 0),
                ArriveTime = new DateTime(2022, 11, 19, 18, 0, 0),
                Seats = new Seat[] {
                    new Seat() { Id = 1, Name = "First Class", Cost = 240 },
                    new Seat() { Id = 2, Name = "Business Class", Cost = 170 },
                    new Seat() { Id = 3, Name = "Couch Class", Cost = 190 }
                }
            },
            new FlightDetail(){
                Id = 3,
                Airline = "Airline3",
                DepartAirport = "SLC",
                ArriveAirport = "LAX",
                DepartTime = new DateTime(2022, 11, 19, 15, 30, 0),
                ArriveTime = new DateTime(2022, 11, 20, 2, 30, 0),
                Seats = new Seat[] {
                    new Seat() { Id = 1, Name = "First Class", Cost = 200 },
                    new Seat() { Id = 2, Name = "Business Class", Cost = 130 },
                    new Seat() { Id = 3, Name = "Couch Class", Cost = 100 }
                }
            }
        };

        public FlightController(ILogger<FlightController> logger)
        {
            this.logger = logger;
        }

        [HttpGet("flights/{departAirport}/{arriveAirport}")]
        public async Task<IEnumerable<FlightDetail>> GetFlights(string departAirport, string arriveAirport)
        {
            logger.LogInformation("here");
            var ourFlights = new List<FlightDetail>();
            foreach (var flight in flights)
            {
                if (flight.DepartAirport == departAirport && flight.ArriveAirport == arriveAirport /*&& flight.DepartTime.DayOfYear == departDate.DayOfYear*/)
                {
                    ourFlights.Add(flight);
                }
            }
            logger.LogInformation(ourFlights.ToString());
            return flights;
        }
    }
}