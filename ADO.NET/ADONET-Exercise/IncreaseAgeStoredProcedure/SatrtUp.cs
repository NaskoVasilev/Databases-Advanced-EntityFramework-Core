using ADONETExercise;
using System;
using System.Data.SqlClient;

namespace IncreaseAgeStoredProcedure
{
    class SatrtUp
    {
        static void Main(string[] args)
        {
            int id = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string executeProcedureQuety = @"EXECUTE usp_GetOlder @id";

                using(SqlCommand command = new SqlCommand(executeProcedureQuety, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }

                string getMinionsByIdQuery = "SELECT Name, Age FROM Minions WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(getMinionsByIdQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        Console.WriteLine($"{reader[0]} – {reader[1]} years old");
                    }
                }
            }
        }
    }
}
