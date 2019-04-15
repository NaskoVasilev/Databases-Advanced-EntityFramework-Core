using ADONETExercise;
using System;
using System.Data.SqlClient;

namespace MinnionNames
{
    class StartUp
    {
        static void Main(string[] args)
        {
            int id = int.Parse(Console.ReadLine());

            using(SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                string villainName = GetNameById(connection, id);

                if(villainName == null)
                {
                    Console.WriteLine($"No villain with ID {id} exists in the database.");
                    return;
                }

                Console.WriteLine($"Villain: {villainName}");
                PrintMinions(connection, id, villainName);
            }
        }

        private static void PrintMinions(SqlConnection connection, int id, string villainName)
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("Id", id);

                using(SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{(long)reader[0]}. {(string)reader[1]} {(int)reader[2]}");
                    }

                    if (!reader.HasRows)
                    {
                        Console.WriteLine($"Villain: {villainName}.(no minions)");
                    }
                }
            }
        }

        private static string GetNameById(SqlConnection connection, int id)
        {
            string query = "SELECT Name FROM Villains WHERE Id = @id";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                return (string)command.ExecuteScalar();
            }
        }
    }
}
