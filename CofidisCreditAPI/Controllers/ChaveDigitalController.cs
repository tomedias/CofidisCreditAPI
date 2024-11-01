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
            var person = _chaveDigital.GetPerson(NIF);
            if (person == null)
            {
               
                return NotFound($"Person with NIF {NIF} not found.");
            }
            return Ok(person);
        }

        
        [HttpPost("register")]
        public ActionResult<Person> Register([FromQuery] string NIF, [FromQuery] string name, [FromQuery] double monthlyIncome)
        {
            if (string.IsNullOrWhiteSpace(NIF) || string.IsNullOrWhiteSpace(name) || monthlyIncome <= 0)
            {
                return BadRequest("Invalid input. Please provide a valid NIF, name, and a positive monthly income.");
            }

            var newPerson = new Person(NIF, name, monthlyIncome);
            var createdPerson = _chaveDigital.CreatePerson(newPerson);

            if (createdPerson == null)
            {
                return StatusCode(500, "An error occurred while creating the person.");
            }

            return CreatedAtAction(nameof(Login), new { NIF = createdPerson.NIF }, createdPerson);
        }


        [HttpPut("update-income")]
        public ActionResult<Person> UpdateIncome([FromQuery] string NIF, [FromQuery] double monthlyIncome)
        {
           
            var updatedPerson = _chaveDigital.EditMontlyIncome(NIF, monthlyIncome);
            if (updatedPerson == false)
            {
               
                return NotFound($"Person with NIF {NIF} not found.");
            }

            return Ok(updatedPerson);
        }

        
        [HttpGet("list-people")]
        public ActionResult<LinkedList<Person>> ListPeople()
        {
            var people = _chaveDigital.ListPeople();
            if (people.IsNullOrEmpty())
            {
                
                return NoContent();
            }

            return Ok(people);
        }
    }
}
