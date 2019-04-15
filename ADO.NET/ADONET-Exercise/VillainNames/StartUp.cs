using System;
using ADONETExercise;
using System.Data.SqlClient;

namespace VillainNames
{
    class StartUp
    {
        static void Main(string[] args)
        {
            string query = @"  SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                                    FROM Villains AS v 
                                    JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                                GROUP BY v.Id, v.Name 
                                  HAVING COUNT(mv.VillainId) > 3 
                                ORDER BY COUNT(mv.VillainId)";

            using(SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                using(SqlCommand command = new SqlCommand(query, connection))
                {
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = (string)reader[0];
                            int age = (int)reader[1];

                            Console.WriteLine($"{name} - {age}");
                        }
                    }
                }
            }
        }
    }
}
