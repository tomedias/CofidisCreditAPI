using System.Data;
using System;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CofidisCreditAPI
{
    public class CreditCheck : ControllerBase
    {

        private readonly string _connectionString;
        public CreditCheck(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ActionResult<double> GetCreditLimit(string NIF)
        {
            double creditLimit = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                using (SqlCommand command = new SqlCommand("GetCreditLimit", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@NIF", SqlDbType.VarChar)
                    {
                        Value = NIF
                    });

                    SqlParameter creditLimitParam = new SqlParameter("@CreditLimit", SqlDbType.Float)
                    {
                        Direction = ParameterDirection.Output
                    };

                    SqlParameter returnValueParam = new SqlParameter("@ReturnValue", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    command.Parameters.Add(returnValueParam);
                    command.Parameters.Add(creditLimitParam);
                    connection.Open();
                    command.ExecuteNonQuery();

                    int returnValue = (int)returnValueParam.Value;
                    
                    if (returnValue == -1)
                    {
               
                        Console.WriteLine($"No income found for NIF: {NIF}");
                        return NotFound("Person not found.");
                    }
                    else if (returnValue != 0)
                    {

                        return StatusCode(500, "An error occurred while getting your credit limit.");
                    }
                    creditLimit = Convert.ToDouble(creditLimitParam.Value);
                    
                }
            }

            return Ok(creditLimit);
        }


        public ActionResult<LinkedList<Credit>> GetCreditList(Person person)
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
                    StatusCode(500, "An error occurred while getting your credit list.");
                }
            }
            return creditList.IsNullOrEmpty() ? NoContent() : Ok(creditList);
        }


        public ActionResult<string> CreateCredit(Person person, double credit, int credit_duration)
        {
            string query = "INSERT INTO credits (credit_id, nif, credit_taken, credit_payed, credit_request_date, credit_term) VALUES (@ID,@NIF, @credit_taken, @credit_payed, @credit_request_date,@credit_term)";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                string ID = Guid.NewGuid().ToString().Substring(0, 8);
                command.Parameters.AddWithValue("@ID", ID);
                command.Parameters.AddWithValue("@NIF", person.NIF);
                command.Parameters.AddWithValue("@credit_taken", credit);
                command.Parameters.AddWithValue("@credit_payed", 0.0);
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
                        return Ok($"Your credit has been requested successfully with ID: {ID}");
                    }

                }
                catch (Exception ex)
                {
                    StatusCode(500, "An error occurred while creating your credit request.");
                }
            }
            return NotFound("Person not found.");
        }

        public ActionResult<string> PayCredit(Person person, double payment, string credit_id)
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
                        command.Parameters.Add(new SqlParameter("@paymentAmount", SqlDbType.Float)
                        {
                            Value = payment
                        });
                        SqlParameter newCreditPayedParam = new SqlParameter("@newCreditPayed", SqlDbType.Float);
                        newCreditPayedParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(newCreditPayedParam);





                        connection.Open();
                
                        int affectedRows = command.ExecuteNonQuery();

                        
                        double newCreditPayed = Convert.ToDouble(newCreditPayedParam.Value);

                        if (affectedRows > 0)
                        {
                            Console.WriteLine($"Payment processed successfully. New credit paid amount: {newCreditPayed}");
                            return Ok($"Payment successfull, {newCreditPayed} of payment left.");
                        }
                        else
                        {
                            Console.WriteLine("No rows were updated. Please check the NIF and Credit ID.");
                            return StatusCode(500, "Failed to process credit payment.");
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
            return NotFound("Person not found");



        }

        
    }




    
}
