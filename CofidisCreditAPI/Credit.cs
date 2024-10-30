using System.Data;
using System;
using System.Data.SqlClient;
using System.Text.Json.Serialization;

namespace CofidisCreditAPI
{
    public class Credit
    {

        public string ID { get; set;}

        public Person Person { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowNamedFloatingPointLiterals)]
        public double CreditValue{ get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowNamedFloatingPointLiterals)]
        public double CreditPayed { get; set; }

        public DateTime Term { get; set; }
        
        public Credit(string ID ,Person person, double CreditValue, double CreditPayed, DateTime Term) {
            this.ID = ID;
            this.Person = person;
            this.CreditValue = CreditValue;
            this.CreditPayed = CreditPayed;
            this.Term = Term;
        }

        public bool CheckCreditState()
        {
            return ((DateTime.Now) < this.Term) || CreditPayed == this.CreditValue;
        }

        public double MissingCredit()
        {
            return this.CreditValue - this.CreditPayed;
        }

    }
}
