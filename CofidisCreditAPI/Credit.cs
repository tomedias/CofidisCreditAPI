using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json.Serialization;

namespace CofidisCreditAPI
{
    public class Credit
    {
        public string Id { get; set; }

        public Person Person { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowNamedFloatingPointLiterals)]
        public double CreditValue { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowNamedFloatingPointLiterals)]
        public double CreditPaid { get; set; }

        public DateTime CreditStart { get; set; }
        public DateTime CreditEnd { get; set; }

        public Credit(string id, Person person, double creditValue, double creditPaid, DateTime creditStart, DateTime creditEnd)
        {
            Id = id;
            Person = person ?? throw new ArgumentNullException(nameof(person));
            CreditValue = creditValue;
            CreditPaid = creditPaid;
            CreditStart = creditStart;
            CreditEnd = creditEnd;
        }

        public bool CheckCreditState()
        {
            return DateTime.Now < CreditEnd || CreditPaid >= CreditValue;
        }

        public double MissingCredit()
        {
            return CreditValue - CreditPaid;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Credit other = (Credit)obj;
            return Id == other.Id &&
                   Person.Equals(other.Person) &&
                   CreditValue == other.CreditValue &&
                   CreditPaid == other.CreditPaid &&
                   CreditStart == other.CreditStart &&
                   CreditEnd == other.CreditEnd;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Person, CreditValue, CreditPaid, CreditStart, CreditEnd);
        }
    }
}
