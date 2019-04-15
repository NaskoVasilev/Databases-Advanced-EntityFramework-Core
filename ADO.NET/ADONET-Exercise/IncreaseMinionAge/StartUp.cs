using ADONETExercise;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace IncreaseMinionAge
{
    class StartUp
    {
        static void Main(string[] args)
        {
            int[] minionsIds = Console.ReadLine()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string updateMinionsQuery = @" UPDATE Minions
                                                  SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                               WHERE Id = @Id";

                foreach (var minionId in minionsIds)
                {
                    using (SqlCommand command = new SqlCommand(updateMinionsQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", minionId);
                        command.ExecuteScalar();
                    }
                }

                string getMinionsQuery = @"SELECT Name, Age FROM Minions";

                using (SqlCommand command = new SqlCommand(getMinionsQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader[0]} {reader[1]}");
                        }
                    }
                }
            }
        }
    }
}
