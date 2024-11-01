using System.Data;
using System;
using System.Data.SqlClient;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace CofidisCreditAPI
{
    public class ChaveDigital : ControllerBase
    {

        private readonly string _connectionString;
        public ChaveDigital(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ActionResult<Person> GetPerson(string NIF)
        {

            string query = "SELECT * FROM govpt WHERE NIF = @NIF";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NIF", NIF);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        Person person = new Person((string)reader["NIF"], (string)reader["name"], Convert.ToDouble(reader["Monthly_Income"]));
                        return Ok(person); 
                    }


                    reader.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return NotFound($"Person with NIF {NIF} not found.");

        }


        public ActionResult<LinkedList<Person>> ListPeople()
        {
            string query = "SELECT * FROM govpt";
            LinkedList<Person> people = new LinkedList<Person>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        people.AddLast(new Person((string)reader["NIF"], (string)reader["name"], Convert.ToDouble(reader["Monthly_Income"])));
                    }
                    
       
                    reader.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return people.IsNullOrEmpty() ? NoContent() : Ok(people);
        }


        public ActionResult<Person> CreatePerson(Person person)
        {

            string query = "INSERT INTO govpt (nif, name, monthly_income) VALUES (@NIF, @name,@monthly_income)";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                string ID = Guid.NewGuid().ToString();
                command.Parameters.AddWithValue("@NIF", person.NIF);
                command.Parameters.AddWithValue("@name", person.Name);
                command.Parameters.AddWithValue("@monthly_income", person.MonthlyIncome);


                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return Ok(person);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return StatusCode(500, "An error occurred while creating the person.");
        }
        


        public ActionResult<Person> EditMontlyIncome(string NIF, double new_income)
        {


            string query = "UPDATE govpt SET monthly_income = @monthly_income WHERE nif = @NIF";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                string ID = Guid.NewGuid().ToString();
                command.Parameters.AddWithValue("@NIF", NIF);
                command.Parameters.AddWithValue("@monthly_income", new_income);


                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return Ok($"The Income for NIF {NIF} has been updated.");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return NotFound($"Person with NIF {NIF} not found.");
        }




    } 
}
