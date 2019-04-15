using ADONETExercise;
using System;
using System.Data.SqlClient;

namespace RemoveVillain
{
    class StartUp
    {
        static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());
            string villainName = "";
            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string getVillainNameQuery = "SELECT Name FROM Villains WHERE Id = @villainId";
                using (SqlCommand command =new SqlCommand(getVillainNameQuery, connection))
                {
                    command.Parameters.AddWithValue("@villainId", villainId);
                    villainName = (string)command.ExecuteScalar();

                    if(villainName == null)
                    {
                        Console.WriteLine("No such villain was found.");
                        return;
                    }
                }

                string deleteVillainMinnionsQuery = @"DELETE FROM MinionsVillains WHERE VillainId = @villainId";

                using (SqlCommand command = new SqlCommand(deleteVillainMinnionsQuery, connection))
                {
                    command.Parameters.AddWithValue("@villainId", villainId);
                    int result = command.ExecuteNonQuery();

                    Console.WriteLine($"{villainName} was deleted.");
                    Console.WriteLine($"{result} minions were released.");
                }

                string deleteVillainQuery = @"DELETE FROM Villains
                                                        WHERE Id = @villainId";

                using (SqlCommand command = new SqlCommand(deleteVillainQuery, connection))
                {
                    command.Parameters.AddWithValue("@villainId", villainId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
