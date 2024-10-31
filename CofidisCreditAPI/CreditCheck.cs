using System.Data;
using System;
using System.Data.SqlClient;

namespace CofidisCreditAPI
{
    public class CreditCheck
    {

        private readonly string _connectionString;
        public CreditCheck(string connectionString)
        {
            _connectionString = connectionString;
        }

        public double GetCreditLimit(double monthlyIncome)
        {

            if(monthlyIncome <0)
            {
                return 0;
            }
            double creditLimit = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                using (SqlCommand command = new SqlCommand("GetCreditLimit", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;


                    command.Parameters.Add(new SqlParameter("@MonthlyIncome", SqlDbType.Decimal)
                    {
                        Value = Convert.ToDecimal(monthlyIncome)
                    });


                    SqlParameter creditLimitParam = new SqlParameter("@CreditLimit", SqlDbType.Decimal)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(creditLimitParam);


                    connection.Open();


                    command.ExecuteNonQuery();


                    creditLimit = Convert.ToDouble(creditLimitParam.Value);
                }
            }

            return creditLimit;
        }


        public LinkedList<Credit> GetCreditList(Person person)
        {

            LinkedList<Credit> creditList = new LinkedList<Credit>();
            string query = "SELECT * FROM credits WHERE NIF = @NIF";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NIF", person.NIF);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        creditList.AddLast(new Credit((string)reader["credit_id"], person, Convert.ToDouble(reader["Credit_Taken"]), Convert.ToDouble(reader["Credit_Payed"]), 
                            (DateTime)reader["credit_request_date"],(DateTime)reader["credit_term"]));
                    }


                    reader.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return creditList;
        }


        public Credit CreateCredit(Person person, double credit, int credit_duration)
        {
            string query = "INSERT INTO credits (credit_id, nif, credit_taken, credit_payed, credit_request_date, credit_term) VALUES (@ID,@NIF, @credit_taken, @credit_payed, @credit_request_date,@credit_term)";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                string ID = Guid.NewGuid().ToString().Substring(0, 8);
                command.Parameters.AddWithValue("@ID", ID);
                command.Parameters.AddWithValue("@NIF", person.NIF);
                command.Parameters.AddWithValue("@credit_taken", credit);
                command.Parameters.AddWithValue("@credit_payed", 0.0M);
                DateTime start = DateTime.Now;
                DateTime end = start.AddYears(credit_duration);
                command.Parameters.AddWithValue("@credit_request_date", start);
                command.Parameters.AddWithValue("@credit_term", end);

                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return new Credit(ID, person, credit, 0.0, start,end);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return null;
        }

        public bool PayCredit(Person person, double payment, string credit_id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {

                    using (SqlCommand command = new SqlCommand("PayCredit", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        command.Parameters.Add(new SqlParameter("@NIF", SqlDbType.NVarChar)
                        {
                            Value = person.NIF
                        });
                        command.Parameters.Add(new SqlParameter("@credit_id", SqlDbType.NVarChar)
                        {
                            Value = credit_id
                        });
                        command.Parameters.Add(new SqlParameter("@paymentAmount", SqlDbType.Decimal)
                        {
                            Value = Convert.ToDecimal(payment)
                        });
                        SqlParameter newCreditPayedParam = new SqlParameter("@newCreditPayed", SqlDbType.Decimal);
                        newCreditPayedParam.Precision = 18;
                        newCreditPayedParam.Scale = 2;
                        newCreditPayedParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(newCreditPayedParam);





                        connection.Open();


                       
                        int affectedRows = command.ExecuteNonQuery();

                        
                        decimal newCreditPayed = (decimal)newCreditPayedParam.Value;

                        if (affectedRows > 0)
                        {
                            Console.WriteLine($"Payment processed successfully. New credit paid amount: {newCreditPayed}");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("No rows were updated. Please check the NIF and Credit ID.");
                            return false;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
               
                Console.WriteLine($"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error: {ex.Message}");
            }
            return false;



        }

        
    }




    
}
