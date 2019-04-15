using ADONETExercise;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ChangeTownNames
{
    class StartUp
    {
        static void Main(string[] args)
        {
            string countryName = Console.ReadLine();

            using(SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string updareQuery = @"UPDATE Towns
                                SET Name = UPPER(Name)
                                WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";
                using(SqlCommand command = new SqlCommand(updareQuery, connection))
                {
                    command.Parameters.AddWithValue("@countryName", countryName);
                    int result = command.ExecuteNonQuery();

                    if(result == 0)
                    {
                        Console.WriteLine($"No town names were affected.");
                        return;
                    }
                }

                string getTownsQuery = @" SELECT t.Name 
                                            FROM Towns as t
                                            JOIN Countries AS c ON c.Id = t.CountryCode
                                            WHERE c.Name = @countryName";

                using (SqlCommand command = new SqlCommand(getTownsQuery, connection))
                {
                    List<string> towns = new List<string>();
                    command.Parameters.AddWithValue("@countryName", countryName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine($"No town names were affected.");
                        }

                        while (reader.Read())
                        {
                            towns.Add((string)reader[0]);
                        }
                    }

                    Console.WriteLine($"[{string.Join(", ", towns)}]");
                }
            }
        }
    }
}
