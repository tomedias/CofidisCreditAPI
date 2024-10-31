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


        public DateTime CreditStart { get; set; }
        public DateTime CreditEnd { get; set; }
        
        public Credit(string ID ,Person person, double CreditValue, double CreditPayed, DateTime CreditStart, DateTime CreditEnd) {
            this.ID = ID;
            this.Person = person;
            this.CreditValue = CreditValue;
            this.CreditPayed = CreditPayed;
            this.CreditStart = CreditStart;
            this.CreditEnd = CreditEnd;
        }

        public bool CheckCreditState()
        {
            return ((DateTime.Now) < this.CreditEnd) || CreditPayed == this.CreditValue;
        }

        public double MissingCredit()
        {
            return this.CreditValue - this.CreditPayed;
        }

    }
}
