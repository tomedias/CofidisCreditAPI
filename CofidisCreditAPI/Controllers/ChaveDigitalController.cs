using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CofidisCreditAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChaveDigitalController : ControllerBase
    {
        private readonly ILogger<ChaveDigitalController> _logger;
        private readonly ChaveDigital _chaveDigital;

        public ChaveDigitalController(ILogger<ChaveDigitalController> logger)
        {
            _logger = logger;
            _chaveDigital = new ChaveDigital("Server=localhost;Database=MicroCreditDB;Trusted_Connection=True;");
        }

        
        [HttpGet("login")]
        public ActionResult<Person> Login([FromQuery] string NIF)
        {
            return _chaveDigital.GetPerson(NIF);
            
        }

        
        [HttpPost("register")]
        public ActionResult<Person> Register([FromQuery] string NIF, [FromQuery] string name, [FromQuery] double monthlyIncome)
        {
            if (string.IsNullOrWhiteSpace(NIF) || string.IsNullOrWhiteSpace(name) || monthlyIncome <= 0)
            {
                return BadRequest("Invalid input. Please provide a valid NIF, name, and a positive monthly income.");
            }

            var newPerson = new Person(NIF, name, monthlyIncome);
            return _chaveDigital.CreatePerson(newPerson);

        }


        [HttpPut("update-income")]
        public ActionResult<Person> UpdateIncome([FromQuery] string NIF, [FromQuery] double monthlyIncome)
        {
           
            return _chaveDigital.EditMontlyIncome(NIF, monthlyIncome);
            
        }

        
        [HttpGet("list-people")]
        public ActionResult<LinkedList<Person>> ListPeople()
        {
            return _chaveDigital.ListPeople();
            
        }
    }
}
