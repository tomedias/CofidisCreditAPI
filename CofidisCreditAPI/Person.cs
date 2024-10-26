using System.Data;
using System;
using System.Data.SqlClient;

namespace CofidisCreditAPI
{
    public class Person
    {
        public string NIF { get; set; }
        public string NAME { get; set; }
        
        public Person(string nif, string name) {
            this.NAME = name ?? throw new ArgumentNullException(nameof(name)); ;
            this.NIF = nif ?? throw new ArgumentNullException(nameof(nif)); ;
        }

        
        public string getName()
        {
            return NAME;
        }

        public string getNif()
        {
            return NIF;
        }
    }
}
