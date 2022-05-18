using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Exc
{
    public class Program
    {
        const string SqlConnectionString = "Server=.;Database=MinionsDB;Integrated Security = true";
        public static void Main(string[] args)
        {
            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();


                //Create database
                //string createDatabase = "CREATE DATABASE MinionsDB;";
                //NonQueryExecution(connection, "");
                //Create table
                //var createTableStatements = GetCreateTableStatements();
                //foreach (var query in createTableStatements)
                //{
                //    NonQueryExecution(connection, query);
                //}
                //InsertData into table
                //var insertStatements = IsertDataStatements();
                //foreach (var query in insertStatements)
                //{
                //    NonQueryExecution(connection, query);
                //}
                //2 zadacha
                //GetVillainNamesOfWhoHasMoreThan3Minions(connection);
                //3 Minion Names
                //MinionNames(connection);
                //4 Add minion and set Villains:
                //AddMinionAndSetVillainsTowns(connection);
                //5.	Change Town Names Casing
                //PrintAffectedTownNAmesByCountry(connection);

            }
        }

        private static void PrintAffectedTownNAmesByCountry(SqlConnection connection)
        {
            string countryName = Console.ReadLine();
            updateTownName(connection, countryName);
            List<string> result = new List<string>();
            var townsAffectedQuery = @" SELECT t.Name 
                                       FROM Towns as t
                                       JOIN Countries AS c ON c.Id = t.CountryCode
                                      WHERE c.Name = @countryName";
            using (var townAffectedCommand = new SqlCommand(townsAffectedQuery, connection))
            {
                townAffectedCommand.Parameters.AddWithValue("@countryName", countryName);
                var reader = townAffectedCommand.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        result.Add((string)reader[0]);
                    }
                }
            };
            Console.WriteLine("[" + String.Join(", ", result) + "]");
        }

        private static void updateTownName(SqlConnection connection, string countryName)
        {
            var updateTownNameQuery = @"UPDATE Towns
                                         SET Name = UPPER(Name)
                                         WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";
            using (var updateComman = new SqlCommand(updateTownNameQuery, connection))
            {
                updateComman.Parameters.AddWithValue("@countryName", countryName);
                var result = updateComman.ExecuteNonQuery();
                if (result == 0)
                {
                    Console.WriteLine($"No town names were affected.");

                }
                else
                {
                    Console.WriteLine($"{result} town names were affected.");

                }


            };
        }

        private static void AddMinionAndSetVillainsTowns(SqlConnection connection)
        {
            string[] minionInformation = Console.ReadLine().Split(" ").ToArray();
            string minionName = minionInformation[1];
            int minionAge = int.Parse(minionInformation[2]);
            string minionTown = minionInformation[3];
            string villainName = Console.ReadLine().Split(" ").ToArray()[1];
            var townId = getTownId(connection, minionTown);
            var villainId = GetVillainIdAndAddItToDB(connection, villainName);

            addMiniontoDB(minionName, minionAge, townId, villainId, villainName, connection);
        }

        private static int GetVillainIdAndAddItToDB(SqlConnection connection, string villainName)
        {
            int villainId = 0;
            string villainStringQuery = "SELECT Id FROM Villains WHERE Name = @Name";
            using (var commandVillain = new SqlCommand(villainStringQuery, connection))
            {
                commandVillain.Parameters.AddWithValue("@Name", villainName);
                var result = commandVillain.ExecuteScalar();

                if (result == null)
                {
                    addVillainToDB(villainName, connection);
                    villainId = GetVillainIdAndAddItToDB(connection, villainName);
                }
                else
                {
                    villainId = int.Parse(string.Format("{0}", result));

                }
            };
            return villainId;
        }

        private static int getTownId(SqlConnection connection, string minionTown)
        {
            string townSelectedQuery = "SELECT Id FROM Towns WHERE Name = @townName";
            var townId = 0;
            using (var command = new SqlCommand(townSelectedQuery, connection))
            {
                command.Parameters.AddWithValue("@townName", minionTown);
                var result = command.ExecuteScalar();

                if (result == null)
                {
                    addTownToDB(minionTown, connection);
                    townId = getTownId(connection, minionTown);
                }
                else
                {
                    townId = int.Parse(string.Format("{0}", result));
                }
            };
            return (int)townId;
        }

        private static void addMiniontoDB(string minionName, int minionAge, int townId, int villainId, string villainName, SqlConnection connection)
        {
            var minionId = 0;
            var minionAddStringQuery = "INSERT INTO Minions (Name, Age, TownId) VALUES (@nam, @age, @townId)";
            using (var command = new SqlCommand(minionAddStringQuery, connection))
            {
                command.Parameters.AddWithValue("@nam", minionName);
                command.Parameters.AddWithValue("@age", minionAge);
                command.Parameters.AddWithValue("@townId", townId);
                var result = command.ExecuteNonQuery();

            }

            string minionFoundIfQuery = "SELECT Id FROM Minions WHERE Name = @Name";
            using (var commandVillain = new SqlCommand(minionFoundIfQuery, connection))
            {
                commandVillain.Parameters.AddWithValue("@Name", minionName);
                var result = commandVillain.ExecuteScalar();


                minionId = int.Parse(string.Format("{0}", result));

            };

            var setVillainToMinionQuery = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@minionId, @villainId)";
            using (var command = new SqlCommand(setVillainToMinionQuery, connection))
            {
                command.Parameters.AddWithValue("@villainId", villainId);
                command.Parameters.AddWithValue("@minionId", minionId);

                var result = command.ExecuteNonQuery();
                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
            };
        }

        private static void addVillainToDB(string villainName, SqlConnection connection)
        {
            var villainAddStringQuery = "INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4);";
            using (var command = new SqlCommand(villainAddStringQuery, connection))
            {
                command.Parameters.AddWithValue("@villainName", villainName);
                var result = command.ExecuteNonQuery();
                Console.WriteLine($"Villain {villainName} was added to the database.");
            };
        }

        private static void addTownToDB(string townSelected, SqlConnection connection)
        {
            var townAddStringCommand = "INSERT INTO Towns (Name) VALUES (@townName)";
            using (var command = new SqlCommand(townAddStringCommand, connection))
            {
                command.Parameters.AddWithValue("townName", townSelected);
                var result = command.ExecuteNonQuery();
                Console.WriteLine($"Town {townSelected} was added to the database.");
            };

        }

        private static void MinionNames(SqlConnection connection)
        {
            var currentVillainId = int.Parse(Console.ReadLine());
            string vaillainNameQuery = @$"SELECT Name FROM Villains WHERE Id = @Id";
            string minionsQuery = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";
            using (var command = new SqlCommand(vaillainNameQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", currentVillainId);
                var result = command.ExecuteScalar();

                if (result == null)
                {
                    Console.WriteLine($"No villain with ID {currentVillainId} exists in the database.");
                }
                else
                {

                    Console.WriteLine($"Villain: {result}");
                    using (var minionCommand = new SqlCommand(minionsQuery, connection))
                    {
                        minionCommand.Parameters.AddWithValue("@Id", currentVillainId);
                        using (var reader = minionCommand.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                Console.WriteLine("(no minions)");
                            }
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader[0]}.{reader[1]} {reader[2]}");
                            }
                        }
                    }


                }

            };
        }

        private static void GetVillainNamesOfWhoHasMoreThan3Minions(SqlConnection connection)
        {
            var selectVillainsWhoHaveMoreThan3Minions = @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                                                        FROM Villains AS v
                                                        JOIN MinionsVillains AS mv ON v.Id = mv.VillainId
                                                        GROUP BY v.Id, v.Name
                                                        HAVING COUNT(mv.VillainId) > 3
                                                        ORDER BY COUNT(mv.VillainId)";
            using (var command = new SqlCommand(selectVillainsWhoHaveMoreThan3Minions, connection))
            {

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader[0]} - {reader[1]}");
                    }
                }

            };
        }

        private static void NonQueryExecution(SqlConnection connection, string query)
        {
            using (var command = new SqlCommand(query, connection))
            {
                var result = command.ExecuteNonQuery();
            };
        }

        private static object ExecuteScalar(SqlConnection connection,
            string query)
        {
            using (var command = new SqlCommand(query, connection))
            {

                var result = command.ExecuteScalar();
                return result;
            };
        }

        private static string[] GetCreateTableStatements()
        {
            var result = new string[]
            {
                "CREATE TABLE Countries(Id INT PRIMARY KEY, Name VARCHAR(50))",
                "CREATE TABLE Towns(Id INT PRIMARY KEY, Name VARCHAR(50), CountryCode INT FOREIGN KEY REFERENCES Countries(Id))",
                "CREATE TABLE Minions(Id INT PRIMARY KEY, Name VARCHAR(50), Age INT, TownId INT FOREIGN KEY REFERENCES Towns(Id))",
                "CREATE TABLE EvilnessFactor(Id INT PRIMARY KEY, Name VARCHAR(50))",
                "CREATE TABLE Villains(Id INT PRIMARY KEY, Name VARCHAR(50), EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactor(Id))",
                "CREATE TABLE MinionsVillains(MinionId INT FOREIGN KEY REFERENCES Minions(Id), VillainId INT FOREIGN KEY REFERENCES Villains(Id),CONSTRAINT PK_MinionsVillains PRIMARY KEY(MinionId, VillainId))"

            };
            return result;

        }
        private static string[] IsertDataStatements()
        {
            var result = new string[]
            {
                "INSERT INTO Countries(Id, Name) VALUES (1, 'Bulgaria'), (2,'Norway'), (3, 'Cyprus'), (4,'Greece'), (5, 'UK');",
                "INSERT INTO Towns(Id, Name, CountryCode) VALUES (1, 'Sofia', 1), (2, 'Oslo', 2), (3,'Larnaca', 3), (4, 'Athens', 4), (5, 'London', 5);",
                "INSERT INTO Minions VALUES (1, 'Stoqn', 12, 1), (2, 'Petyr', 22, 1), (3,'Ivan', 23, 3), (4, 'Kiro', 42, 4), (5, 'Jhon', 15, 5);",
                "INSERT INTO EvilnessFactor VALUES (1, 'super good'), (2, 'good'), (3, 'bad'), (4, 'evil'), (5, 'super evil');",
                "INSERT INTO Villains VALUES (1, 'Gru', 1), (2, 'NotEvil', 2), (3, 'Guru', 3), (4, 'Jojo', 4), (5, 'Toto', 5);  ",
                "INSERT INTO MinionsVillains VALUES (1, 1), (2, 2), (3, 3), (4, 4), (5, 5);"

            };
            return result;

        }
    }
}
