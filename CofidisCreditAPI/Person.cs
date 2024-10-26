using System.Data;
using System;
using System.Data.SqlClient;

namespace CofidisCreditAPI
{
    public class Person
    {
        public string nif { get; set; }
        public string name { get; set; }
        
        public Person(string nif, string name) {
            this.name = name;
            this.nif = nif;
        }

        
        public string getName()
        {
            return name;
        }

        public string getNif()
        {
            return nif;
        }
    }
}
