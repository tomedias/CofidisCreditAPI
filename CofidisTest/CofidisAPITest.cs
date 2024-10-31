using System;
using System.Text;
using CofidisCreditAPI;

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
            double value = creditCheck.GetCreditLimit(-1);
            Assert.AreEqual(0.0, value, 0);

            value = creditCheck.GetCreditLimit(800);
            Assert.AreEqual(1000, value, 0);

            value = creditCheck.GetCreditLimit(1700);
            Assert.AreEqual(2000, value, 0);

            value = creditCheck.GetCreditLimit(2300);
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
            Person person = chaveDigital.CreatePerson(new Person(NIF, fullName, salary));

            LinkedList<Person> people = chaveDigital.ListPeople();

            Assert.IsTrue(people.Contains(person));

            Person person_test = chaveDigital.GetPerson(NIF);

            Assert.AreEqual(person, person_test);

            salary = Random.Next(0, 5000);

            chaveDigital.EditMontlyIncome(NIF, salary);

            person_test = chaveDigital.GetPerson(NIF);

            Assert.AreEqual(salary, person_test.Monthly_Income);
        }
    }
}