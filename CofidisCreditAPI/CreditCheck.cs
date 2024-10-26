using System.Data;
using System;
using System.Data.SqlClient;

namespace CofidisCreditAPI
{
    public class CreditCheck
    {

        private readonly string _connectionString;
        public CreditCheck(string connectionString) {
            _connectionString = connectionString;
        }

        public decimal GetCreditLimit(decimal monthlyIncome)
        {
            decimal creditLimit = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                using (SqlCommand command = new SqlCommand("GetCreditLimit", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;


                    command.Parameters.Add(new SqlParameter("@MonthlyIncome", SqlDbType.Decimal)
                    {
                        Value = monthlyIncome
                    });


                    SqlParameter creditLimitParam = new SqlParameter("@CreditLimit", SqlDbType.Decimal)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(creditLimitParam);


                    connection.Open();


                    command.ExecuteNonQuery();


                    creditLimit = (decimal)creditLimitParam.Value;
                }
            }

            return creditLimit;
        }

    }
}
