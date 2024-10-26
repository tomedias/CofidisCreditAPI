using System.Data;
using System;
using System.Data.SqlClient;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace CofidisCreditAPI
{
    public class ChaveDigital
    {

        private readonly string _connectionString;
        public ChaveDigital(string connectionString) {
            _connectionString = connectionString;
        }

        public Person GetPerson(string NIF)
        {

            string query = "SELECT * FROM Persons WHERE NIF = @NIF";
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
                        Console.WriteLine($"Person found {reader["NIF"]} and {reader["name"]}");
                        return new Person((string)reader["NIF"], (string)reader["name"]);
                    }


                    reader.Close();
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return new Person("Test", "Test");

        }

    }
}
