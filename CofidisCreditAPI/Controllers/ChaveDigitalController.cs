using Microsoft.AspNetCore.Mvc;

namespace CofidisCreditAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChafeDigitalController : ControllerBase
    {
       

        private readonly ILogger<ChafeDigitalController> _logger;
        CreditCheck creditCheck = new CreditCheck("Server=localhost;Database=MicroCreditDB;Trusted_Connection=True;");
        ChaveDigital chaveDigital = new ChaveDigital("Server=localhost;Database=MicroCreditDB;Trusted_Connection=True;");

        public ChafeDigitalController(ILogger<ChafeDigitalController> logger)
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
