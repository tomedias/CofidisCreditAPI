using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

namespace CofidisCreditAPI.Controllers
 
{

    
    [ApiController]
    [Route("api/[controller]")]
    public class CreditCheckController : ControllerBase
    {

        private readonly ILogger<CreditCheckController> _logger;
        private readonly double UnemploymentRate = 0.066f;
        private readonly double Inflation = 0.0216;
        private readonly CreditCheck _creditCheck;
        private readonly ChaveDigital _chaveDigital;

        public CreditCheckController(ILogger<CreditCheckController> logger)
        {
            _logger = logger;
            _creditCheck = new CreditCheck("Server=localhost;Database=MicroCreditDB;Trusted_Connection=True;");
            _chaveDigital = new ChaveDigital("Server=localhost;Database=MicroCreditDB;Trusted_Connection=True;");
        }

        [HttpGet("credit-limit")]
        public ActionResult<double> GetCreditLimit([FromQuery] string NIF)
        {
            if (string.IsNullOrEmpty(NIF))
            {
                return BadRequest("NIF cannot be null or empty.");
            }

            // Assuming you have a method to fetch person data
            
            double creditLimit = _creditCheck.GetCreditLimit(NIF);

            Console.WriteLine($"Found credit of {creditLimit}");
            return creditLimit!=-1 ?  Ok(creditLimit) : NotFound("Person not found.");
        }



        //Simulate ChaveDigital login method as an external service
        private Person LoginChaveDigital(string NIF)
        {
            string baseUrl = "http://localhost:5000/api/ChaveDigital/login";
            using (HttpClient client = new HttpClient())
            {
                string requestUrl = $"{baseUrl}?NIF={NIF}";

                try
                {
                    HttpResponseMessage response = client.GetAsync(requestUrl).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        return JsonConvert.DeserializeObject<Person>(responseBody) ?? throw new HttpRequestException();
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Request error: " + e.Message);
                }
            }
            return null;
        }

        [HttpGet("credit-risk")]
        public double AccessCreditRisk(String NIF)
        {
            Person person = LoginChaveDigital(NIF);
            LinkedList<Credit> creditList = _creditCheck.GetCreditList(person);
            int invalidCount = creditList.Count(credit => !credit.CheckCreditState());
            double percentageFailedCredit = (double)invalidCount / creditList.Count;
            double result = percentageFailedCredit + UnemploymentRate + Inflation;
            return Math.Round(result, 2);
        }

        [HttpGet("CreditList")]
        public ActionResult<LinkedList<Credit>> GetCredits(string NIF)
        {
            var person = LoginChaveDigital(NIF);
            if (person == null)
            {
                return NotFound("Person not found.");
            }

            var creditList = _creditCheck.GetCreditList(person);
            if (creditList.IsNullOrEmpty())
            {
                return NotFound("No credit records found.");
            }

            return Ok(creditList);
        }

        [HttpPost("request-credit")]
        public ActionResult<string> RequestCredit(String NIF, double creditValue, int creditDuration)
        {
            var person = LoginChaveDigital(NIF);
            if (person == null)
            {
                return NotFound("Person not found.");
            }

            if (person.MonthlyIncome <= 0)
            {
                return BadRequest("Monthly income must be greater than zero.");
            }

            if (creditValue <= 0)
            {
                return BadRequest("Credit amount must be greater than zero.");
            }

            var creditList = _creditCheck.GetCreditList(person);
            double totalMissingCredit = creditList.Sum(credit => credit.MissingCredit());
            double creditLimit = _creditCheck.GetCreditLimit(person.NIF); //EXPECTED NOT TO FAIL OFC
            if ((totalMissingCredit + creditValue) > creditLimit)
            {
                return BadRequest($"Credit request exceeds limit. Current limit: {creditLimit}, unpaid credit: {totalMissingCredit}");
            }
            Credit credit = _creditCheck.CreateCredit(person, creditValue, creditDuration);
            return credit != null
                ? Ok($"Your credit has been requested successfully with ID: {credit.Id}")
                : StatusCode(500, "An error occurred while creating your credit request.");
        }


        [HttpPut("pay-credit")]
        public ActionResult<bool> PayCredit(String NIF, String credit_id, double payment)
        {
            var person = LoginChaveDigital(NIF);
            if (person == null)
            {
                return NotFound("Person not found.");
            }

            double paymentLeft = _creditCheck.PayCredit(person, payment, credit_id);
            return paymentLeft!=-1 ? Ok($"Payment successfull, {paymentLeft} of payment left.") : StatusCode(500, "Failed to process credit payment.");
        }
    }
        
    
}
