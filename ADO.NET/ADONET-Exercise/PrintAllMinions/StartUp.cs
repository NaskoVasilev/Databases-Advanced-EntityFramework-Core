using ADONETExercise;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PrintAllMinions
{
    class StartUp
    {
        static void Main(string[] args)
        {
            List<string> minions = new List<string>();

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string query = "SELECT Name FROM Minions";

                using(SqlCommand command =new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            minions.Add((string)reader[0]);
                        }
                    }
                }
            }

            for (int i = 0; i < minions.Count / 2; i++)
            {
                Console.WriteLine(minions[i]);
                Console.WriteLine(minions[minions.Count - i - 1]);
            }

            if(minions.Count % 2 == 1)
            {
                Console.WriteLine(minions[minions.Count / 2]);
            }
        }
    }
}
