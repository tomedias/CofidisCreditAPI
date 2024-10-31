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
        public double Monthly_Income { get; set; }

        public Person(string NIF, string name, double monthlyIncome)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name)); ;
            this.NIF = NIF ?? throw new ArgumentNullException(nameof(NIF)); ;
            this.Monthly_Income = monthlyIncome;
        }


        public override bool Equals(object obj)
        {
            
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

           
            Person other = (Person)obj;
            return Name == other.Name && Name == other.Name && Monthly_Income == other.Monthly_Income;
        }

    }
    
}
