using ADONETExercise;
using System;
using System.Data.SqlClient;

namespace AddMinion
{
    class StartUp
    {
        static void Main(string[] args)
        {
            string[] minionInfo = Console.ReadLine().Split();
            string[] vilainInfo = Console.ReadLine().Split();

            string minionName = minionInfo[1];
            string townName = minionInfo[3];

            string villainName = vilainInfo[1];

            using(SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    int? townId = GetTownIdByName(connection, townName, transaction);
                    if (townId == null)
                    {
                        InsertTown(connection, townName, transaction);
                        townId = GetTownIdByName(connection, townName, transaction);
                    }

                    int? villainId = GetVillainByName(connection, villainName, transaction);
                    if (villainId == null)
                    {
                        InsertVillain(connection, villainName, transaction);
                        villainId = GetVillainByName(connection, villainName, transaction);
                    }

                    int minnionId = GetMinnionByName(connection, minionName, transaction);
                    AddVillianMinnion(connection, villainId, minnionId, villainName, minionName, transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Queries was rollbacked!");
                }
            }
        }

        private static void AddVillianMinnion(SqlConnection connection, int? villainId, int minionId, string villainName, string minionName, SqlTransaction transaction)
        {
            string query = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@villainId, @minionId)";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@villainId", villainId);
                command.Parameters.AddWithValue("@minionId", minionId);
                command.ExecuteNonQuery();
                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
            }
        }

        private static int GetMinnionByName(SqlConnection connection, string minionName, SqlTransaction transaction)
        {
            string query = "SELECT Id FROM Minions WHERE Name = @Name";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@Name", minionName);
                return (int)command.ExecuteScalar();
            }
        }

        private static void InsertVillain(SqlConnection connection, string villainName, SqlTransaction transaction)
        {
            string query = @"INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";

            using(SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@villainName", villainName);
                command.ExecuteNonQuery();
                Console.WriteLine($"Villain {villainName} was added to the database.");
            }
        }

        private static int? GetVillainByName(SqlConnection connection, string villainName, SqlTransaction transaction)
        {
            string query = @"SELECT Id FROM Villains WHERE Name = @Name";

            using(SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@Name", villainName);
                return (int?)command.ExecuteScalar();
            }
        }

        private static void InsertTown(SqlConnection connection, string townName, SqlTransaction transaction)
        {
            string query = "INSERT INTO Towns (Name) VALUES (@townName)";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@townName", townName);
                command.ExecuteNonQuery();
                Console.WriteLine($"Town {townName} was added to the database.");
            }
        }

        private static int? GetTownIdByName(SqlConnection connection, string townName, SqlTransaction transaction)
        {
            string query = @"SELECT Id FROM Towns WHERE Name = @townName";

            using(SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@townName", townName);
                return (int?)command.ExecuteScalar();
            }
        }
    }
}
