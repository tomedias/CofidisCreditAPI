using Microsoft.AspNetCore.Mvc;

namespace CofidisCreditAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChaveDigitalController : ControllerBase
    {


        private readonly ILogger<ChaveDigitalController> _logger;
        ChaveDigital chaveDigital = new ChaveDigital("Server=localhost;Database=MicroCreditDB;Trusted_Connection=True;");

        public ChaveDigitalController(ILogger<ChaveDigitalController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Login")]
        public ActionResult<Person> Login([FromQuery] string NIF)
        {
            return Ok(chaveDigital.GetPerson(NIF));
        }
    }
}
