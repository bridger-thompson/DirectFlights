using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DirectFlights.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly MailjetClient mailClient;

        public MailController(IConfiguration config)
        {
            this.mailClient = new(config["MJ_APIKEY_PUBLIC"], config["MJ_APIKEY_PRIVATE"]);
            this.config = config;
        }

        [HttpPost("{email}")]
        public async Task SendEmail(string toAddress)
        {
            var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact("bridger.thompson@students.snow.edu"))
                .WithSubject("Flight Confirmation")
                .WithHtmlPart(
                @"
                    <h1>Flight Confirmed!</h1>
                    <p>This is not a real flight.</p>
                ")
                .WithTo(new SendContact(toAddress))
                .Build();
            var response = await mailClient.SendTransactionalEmailAsync(email);
        }
    }
}
