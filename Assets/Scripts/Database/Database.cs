using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class Database : MonoBehaviour
{

    private string dbName = "URI=file:LifeData.db";
    // Start is called before the first frame update
    void Start()
    {
        CreateDB();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // public void CreateDB()
    // {
    //     using (var connection = new SqliteConnection(dbName))
    //     {
    //         command.CommandText = "CREATE TABLE IF NOT EXISTS organisms (id INT, species VARCHAR(20), parents INT, children INT, events INT);";
    //         command.ExecuteNonQuery();
    //         command.CommandText = "CREATE TABLE IF NOT EXISTS events (id INT, actors int, hex INT, time VARCHAR(20), type VARCHAR(20);";
    //         command.ExecuteNonQuery();
    //         command.CommandText = "CREATE TABLE IF NOT EXISTS hexes (id INT, traffic INT, events INT);";
    //         command.ExecuteNonQuery();
    //     }
    //     connection.Close();
    // }

    public void CreateDB()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS organisms (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        species TEXT,
                        parents INTEGER,
                        children INTEGER,
                        events INTEGER
                    );";
                command.ExecuteNonQuery();

                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS events (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        actors INTEGER,
                        hex INTEGER,
                        time TEXT,
                        type TEXT
                    );";
                command.ExecuteNonQuery();

                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS hexes (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        traffic INTEGER,
                        events INTEGER
                    );";
                command.ExecuteNonQuery();
            }
        }
    }





}
