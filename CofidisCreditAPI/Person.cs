using System.Data;
using System;
using System.Data.SqlClient;

namespace CofidisCreditAPI
{
    public class Person
    {
        public string NIF { get; set; }
        public string Name { get; set; }

        public int Montly_Income { get; set; }
        
        public Person(string nif, string name, int monthly_income) {
            this.Name = name ?? throw new ArgumentNullException(nameof(name)); ;
            this.NIF = nif ?? throw new ArgumentNullException(nameof(nif)); ;
            this.Montly_Income = monthly_income;
        }

    }
}
