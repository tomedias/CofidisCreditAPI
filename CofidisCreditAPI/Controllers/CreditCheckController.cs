using Microsoft.AspNetCore.Mvc;

namespace CofidisCreditAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreditCheckController : ControllerBase
    {
       

        private readonly ILogger<CreditCheckController> _logger;
        CreditCheck creditCheck = new CreditCheck("Server=localhost;Database=MicroCreditDB;Trusted_Connection=True;");

        public CreditCheckController(ILogger<CreditCheckController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetCreditLimit")]
        public ActionResult<decimal> GetCreditLimit([FromQuery] decimal monthlyIncome)
        {
            if (monthlyIncome <= 0)
            {
                return BadRequest("Monthly income must be greater than zero.");
            }

            decimal creditLimit = creditCheck.GetCreditLimit(monthlyIncome);
            return Ok(creditLimit);
        }
    }
}
