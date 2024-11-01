using System.Data;
using System;
using System.Data.SqlClient;
using System.Text.Json.Serialization;

namespace CofidisCreditAPI
{
    public class Person
    {
        public string NIF { get; set; }

        public string Name { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowNamedFloatingPointLiterals)]
        public double MonthlyIncome { get; set; }

        public Person(string nif, string name, double monthlyIncome)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name)); ;
            this.NIF = nif ?? throw new ArgumentNullException(nameof(nif)); ;
            this.MonthlyIncome = monthlyIncome;
        }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Person other = (Person)obj;
            return NIF == other.NIF && Name == other.Name && MonthlyIncome == other.MonthlyIncome;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(NIF, Name, MonthlyIncome);
        }

    }
    
}
