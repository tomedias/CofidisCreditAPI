using System.Data;
using System;
using System.Data.SqlClient;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace CofidisCreditAPI
{
    public class ChaveDigital
    {

        private readonly string _connectionString;
        public ChaveDigital(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Person GetPerson(string NIF)
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
                       
                        return new Person((string)reader["NIF"], (string)reader["name"], Convert.ToDouble(reader["Monthly_Income"]));
                    }


                    reader.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return null;

        }


        public LinkedList<Person> ListPeople()
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
            return people;
        }


        public Person CreatePerson(Person person)
        {

            string query = "INSERT INTO govpt (nif, name, monthly_income) VALUES (@NIF, @name,@monthly_income)";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                string ID = Guid.NewGuid().ToString();
                command.Parameters.AddWithValue("@NIF", person.NIF);
                command.Parameters.AddWithValue("@name", person.Name);
                command.Parameters.AddWithValue("@monthly_income", person.Monthly_Income);


                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return person;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return null;
        }
        


        public bool EditMontlyIncome(string NIF, double new_income)
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
                        return true;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return false;
        }




    } 
}
