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

        [HttpPost(Name = "Register")]
        public ActionResult<Person> Register(string NIF, string name, double monthlyIncome)
        {
            return Ok(chaveDigital.CreatePerson(new Person(NIF,name,monthlyIncome)));
        }

        [HttpPut(Name = "UpdateIncome")]

        public ActionResult<Person> Update(string NIF, double monthlyIncome)
        {
            return Ok(chaveDigital.EditMontlyIncome(NIF, monthlyIncome));
        }

        [HttpGet(Name = "ListPeople")]
        public ActionResult<LinkedList<Person>> ListPeople()
        {
            return Ok(chaveDigital.ListPeople());
        }
        
    }
}
