using System;
using System.Linq;
using System.Text;
using CofidisCreditAPI;
using Microsoft.AspNetCore.Mvc;

namespace CofidisTest
{
    [TestClass]
    public class CofidisAPITest
    {
        CreditCheck creditCheck = new CreditCheck("Server=localhost;Database=MicroCreditDB;Trusted_Connection=True;");
        ChaveDigital chaveDigital = new ChaveDigital("Server=localhost;Database=MicroCreditDB;Trusted_Connection=True;");

        private static readonly List<string> FirstNames = new List<string>
        {
            "John", "Jane", "Michael", "Sarah", "Chris", "Anna", "David", "Emily", "James", "Olivia"
        };

        private static readonly List<string> LastNames = new List<string>
        {
            "Smith", "Johnson", "Brown", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin"
        };

        private static readonly Random Random = new Random();
        [TestMethod]
        public void TestCreditLimit()
        {

            string firstName = FirstNames[Random.Next(FirstNames.Count)];
            string lastName = LastNames[Random.Next(LastNames.Count)];
            string fullName = $"{firstName} {lastName}";
            string NIF = Convert.ToString(Random.Next(100000000, 999999999));
            double salary = -1;
            Person person = chaveDigital.CreatePerson(new Person(NIF, fullName, salary)).Value;
            double value = (double)((OkObjectResult)creditCheck.GetCreditLimit(NIF).Result).Value;
            Assert.AreEqual(0.0, value, 0);
            chaveDigital.EditMontlyIncome(NIF, 800);
            value = (double)((OkObjectResult)creditCheck.GetCreditLimit(NIF).Result).Value;
            Assert.AreEqual(1000, value, 0);
            chaveDigital.EditMontlyIncome(NIF, 1700);
            value = (double)((OkObjectResult)creditCheck.GetCreditLimit(NIF).Result).Value;
            Assert.AreEqual(2000, value, 0);
            chaveDigital.EditMontlyIncome(NIF, 2300);
            value = (double)((OkObjectResult)creditCheck.GetCreditLimit(NIF).Result).Value;
            Assert.AreEqual(5000, value, 0);
        }

        [TestMethod]
        public void TestChaveDigital()
        {
            string firstName = FirstNames[Random.Next(FirstNames.Count)];
            string lastName = LastNames[Random.Next(LastNames.Count)];
            string fullName = $"{firstName} {lastName}";
            string NIF = Convert.ToString(Random.Next(100000000, 999999999));
            double salary = Random.Next(0,5000);
            ActionResult<Person> personResult = chaveDigital.CreatePerson(new Person(NIF, fullName, salary));
            Person person = (Person)((OkObjectResult)personResult.Result).Value;
            LinkedList<Person> people = (LinkedList<Person>)((OkObjectResult)chaveDigital.ListPeople().Result).Value;

            Assert.IsTrue(people.Contains(person));
            
            
 
            
            Person person_test = (Person)((OkObjectResult)chaveDigital.GetPerson(NIF).Result).Value;

            Assert.AreEqual(person, person_test);

            salary = Random.Next(0, 5000);

            chaveDigital.EditMontlyIncome(NIF, salary);

            person_test = (Person)((OkObjectResult)chaveDigital.GetPerson(NIF).Result).Value;

            Assert.AreEqual(salary, person_test.MonthlyIncome);
        }
    }
}