using Microsoft.Data.Sqlite;
using TG.Attractions;

namespace TG.DataBase
{
    public class DatabaseHelper
    {
        // private string _connectionString = "Data Source=tourist_guide.db";
        private string _connectionString = "Data Source=mydatabase.db";
        

        public void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    CREATE TABLE IF NOT EXISTS Attractions (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        Region TEXT
                    );
                ";
                command.ExecuteNonQuery();
            }
        }

        // public void AddAttraction(Attraction attraction)
        // {
        //     using (var connection = new SqliteConnection(_connectionString))
        //     {
        //         connection.Open();
        //         var command = connection.CreateCommand();
        //         command.CommandText =
        //         @"
        //             INSERT INTO Attractions (Name, Region)
        //             VALUES ($name, $region);
        //         ";
        //         command.Parameters.AddWithValue("$name", attraction.Name);
        //         command.Parameters.AddWithValue("$region", attraction.Region);
        //         command.ExecuteNonQuery();
        //     }
        // }

        // public List<Attraction> GetAttractionsByRegion(string region)
        // {
        //     var attractions = new List<Attraction>();
        //     using (var connection = new SqliteConnection(_connectionString))
        //     {
        //         connection.Open();
        //         var command = connection.CreateCommand();
        //         command.CommandText =
        //         @"
        //             SELECT Id, Name, Region
        //             FROM Attractions
        //             WHERE Region = $region;
        //         ";
        //         command.Parameters.AddWithValue("$region", region);
        //         using (var reader = command.ExecuteReader())
        //         {
        //             while (reader.Read())
        //             {
        //                 attractions.Add(new Attraction
        //                 {
        //                     Id = reader.GetInt32(0),
        //                     Name = reader.GetString(1),
        //                     Region = reader.GetString(2)
        //                 });
        //             }
        //         }
        //     }
        //     return attractions;
        // }

        // internal void UpdateAttraction(Attraction updatedAttraction)
        // {
        //     throw new NotImplementedException();
        // }

        // internal void DeleteAttraction(int id)
        // {
        //     throw new NotImplementedException();
        // }
    }
}