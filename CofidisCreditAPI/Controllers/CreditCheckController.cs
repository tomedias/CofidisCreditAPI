using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CofidisCreditAPI.Controllers
 
{

    
    [ApiController]
    [Route("api/[controller]")]
    public class CreditCheckController : ControllerBase
    {
       
        private readonly ILogger<CreditCheckController> _logger;
        private readonly double UnemploymentRate = 0.066f;
        private readonly double Inflation = 0.0216;
        CreditCheck creditCheck = new CreditCheck("Server=localhost;Database=MicroCreditDB;Trusted_Connection=True;");

        public CreditCheckController(ILogger<CreditCheckController> logger)
        {
            _logger = logger;
        }

        [HttpGet("CreditLimit")]
        public ActionResult<double> GetCreditLimit(string NIF)
        {
            Person person = LoginChaveDigital(NIF);

            if (person.Monthly_Income <= 0)
            {
                return BadRequest("Monthly income must be greater than zero.");
            }

            double creditLimit = creditCheck.GetCreditLimit(person.Monthly_Income);
            return Ok(creditLimit);
        }


        //Simulate ChaveDigital login method as an external service
        private Person LoginChaveDigital(String NIF)
        {
            string baseUrl = "http://localhost:5000/ChaveDigital";
            using (HttpClient client = new HttpClient())
            {
                
                string requestUrl = $"{baseUrl}?NIF={NIF}";

                try
                {
                    
                    HttpResponseMessage response = client.GetAsync(requestUrl).GetAwaiter().GetResult();

                    
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        Person person = JsonConvert.DeserializeObject<Person>(responseBody) ?? throw new HttpRequestException();
                        return person;
                    }
                    else
                    {
                        Console.WriteLine("Request failed with status code: " + response.StatusCode);
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Request error: " + e.Message);
                }
            }
            return new Person("Test","Test",0);
        }

        [HttpGet("CreditRisk")]
        public double AccessCreditRisk(String NIF)
        {
            Person person = LoginChaveDigital(NIF);
            LinkedList<Credit> creditList = creditCheck.GetCreditList(person);
            int invalidCount = creditList.Count(credit => !credit.CheckCreditState());
            double percentageFailedCredit = (double)invalidCount / creditList.Count;
            double result = percentageFailedCredit + UnemploymentRate + Inflation;
            return Math.Round(result, 2);
        }

        [HttpGet("CreditList")]
        public ActionResult<LinkedList<Credit>> GetCredits(String NIF)
        {
            Person person = LoginChaveDigital(NIF);
            return creditCheck.GetCreditList(person);
        }

        [HttpGet("RequestCredit")]
        public String RequestCredit(String NIF, double creditValue, int creditDuration)
        {
            Person person = LoginChaveDigital(NIF);
            if (person.Monthly_Income <= 0)
            {
                return "Monthly income must be greater than zero.";
            }

            if (creditValue < 0)
            {
                return "Credit must be greater than zero";
            }

            LinkedList<Credit> creditList = creditCheck.GetCreditList(person);
            double totalMissingCredit = creditList.Sum(credit => credit.MissingCredit());
            double creditLimit = creditCheck.GetCreditLimit(person.Monthly_Income);
            if ((totalMissingCredit + creditValue) > creditLimit)
            {
                return $"You cannot request this credit with your current credit limit ({creditLimit}) you currently have {totalMissingCredit} of unpayed credit";
            }
            Credit credit = creditCheck.CreateCredit(person, creditValue, creditDuration);
            return credit != null ? $"Your credit has been requested successfully with ID: {credit.ID}" : "Something went wrong while creating your credit";
        }


        [HttpPut("PayCredit")]
        public bool PayCredit(String NIF, String credit_id, double payment)
        {
            Person person = LoginChaveDigital(NIF);
            return creditCheck.PayCredit(person, payment, credit_id);
        }
    }

    
}
