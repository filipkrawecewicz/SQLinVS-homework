using System;
using System.Data.SQLite;


namespace SQLiteDemo
{
    /*
     This program prints out vinyl records currently rented out to users.      
     */
    class VinylRecordsDBManager
    {

        /*
         This is the entry method that connects to SQLite DB, creates tables and records, finally runs query to find rented records.
         */
        static void Main(string[] args)
        {
            SQLiteConnection DbConnection = GetDBConnection();
            CreateTables(DbConnection);
            InsertRecords(DbConnection);
            GetListOfRentedRecords(DbConnection);
        }
        /*
         This methos contains logic for connecting to SQLite DB
         */
        static SQLiteConnection GetDBConnection()
        {

            SQLiteConnection DbConnection;
            // Create a new database connection:
            DbConnection = new SQLiteConnection("Data Source=database.db;Version=3;New=True;Compress=True;");
            /* 'Data source' above is the place on disc where SQLite file will be created. 
               On my pc: C:\Users\fkraw\source\repos\SQLiteDemo\SQLiteDemo\bin\Debug\net5.0\database.db */

            // Open the connection:
            try
            {
                DbConnection.Open();
            }
            catch (Exception ex)
            { //This will print out error message if it cannot connect to DB
                Console.WriteLine(ex.Message);
            }
            //This returns connection in open state
            return DbConnection;
        }
        /*
         This adds 2 tables joined by primary-foreign key relation. One stores data about vinyl records and the other about rented vinyl records.
         */
        static void CreateTables(SQLiteConnection conn)
        {

            SQLiteCommand DbCommand;
            string Dropsql = "DROP TABLE IF EXISTS VinylCollection"; //Because this is a demo, I recreate tables every time when program is executed
            string Dropsql1 = "DROP TABLE IF EXISTS RentedVinyls";
            string Createsql = "CREATE TABLE IF NOT EXISTS VinylCollection (Name VARCHAR(20) PRIMARY KEY, AvailableCopies INT)";
            string Createsql1 = "CREATE TABLE IF NOT EXISTS RentedVinyls (Name VARCHAR(20), RentedTo VARCHAR(100), StartDate Date, FOREIGN KEY(Name) REFERENCES VinylCollection(Name))";

            /*To execute sequel on the open connection, I have to create command obj first 
             and then assign SQL string and call ExecuteNonQuery on command.
             ExecuteNonQuery is used because create and drop don't return anything.
             */
            DbCommand = conn.CreateCommand();
            DbCommand.CommandText = Dropsql;
            DbCommand.ExecuteNonQuery();
            DbCommand.CommandText = Dropsql1;
            DbCommand.ExecuteNonQuery();
            DbCommand.CommandText = Createsql;
            DbCommand.ExecuteNonQuery();
            DbCommand.CommandText = Createsql1;
            DbCommand.ExecuteNonQuery();

        }

        /*
         This method creates some vinyl records in one table, and some rented vinyl data in the other.
         For example we create vinyl record with name 'Radiohead The Bends' and then another record in 
         RentedVinyls that is showing that someone rented this record.
         */
        static void InsertRecords(SQLiteConnection conn)
        {
            SQLiteCommand DbCommand;
            DbCommand = conn.CreateCommand();
            DbCommand.CommandText = "INSERT INTO VinylCollection (Name, AvailableCopies) VALUES ('Radiohead The Bends', 1);";
            DbCommand.ExecuteNonQuery();
            DbCommand.CommandText = "INSERT INTO VinylCollection (Name, AvailableCopies) VALUES ('2pac Dear Mama vol2', 2);";
            DbCommand.ExecuteNonQuery();
            DbCommand.CommandText = "INSERT INTO VinylCollection (Name, AvailableCopies) VALUES ('Childish Gambino', 3);";
            DbCommand.ExecuteNonQuery();


            DbCommand.CommandText = "INSERT INTO RentedVinyls (Name, RentedTo, StartDate) VALUES ('Radiohead The Bends', 'Bobik', '2022-05-10');";
            DbCommand.ExecuteNonQuery();
            DbCommand.CommandText = "INSERT INTO RentedVinyls (Name, RentedTo, StartDate) VALUES ('2pac Dear Mama vol2', 'Szymon', '2022-01-15');";
            DbCommand.ExecuteNonQuery();

        }

        /*
         This method fetches records from DB that have been rented.
         */
        static void GetListOfRentedRecords(SQLiteConnection conn)
        {

            SQLiteCommand DbCommand = conn.CreateCommand(); //We need command obj first
            DbCommand.CommandText = "SELECT * FROM VinylCollection JOIN RentedVinyls ON VinylCollection.Name = RentedVinyls.Name";
            SQLiteDataReader DataReader = DbCommand.ExecuteReader();//Reader is a special obj that holds query results.

            while (DataReader.Read())//Read() tells reader obj to move to next row in results. If no more rows then returns false and the while loop is exited.
            {
                string val1 = DataReader.GetName(0) + ": " + DataReader.GetString(0); // I use column index number to get values from result set
                string val2 = DataReader.GetName(1) + ": " + DataReader.GetInt16(1);
                string val3 = DataReader.GetName(3) + ": " + DataReader.GetString(3);
                string val4 = DataReader.GetName(4) + ": " + DataReader.GetDateTime(4);

                Console.WriteLine("───────────────────────────────────────────────────────────────────────────────────────────────────────────────────────");
                Console.Write(val1);
                Console.Write("  |  ");
                Console.Write(val2);
                Console.Write("  |  ");
                Console.Write(val3);
                Console.Write("  |  ");
                Console.WriteLine(val4);

            }
            Console.WriteLine("───────────────────────────────────────────────────────────────────────────────────────────────────────────────────────");
            conn.Close();
        }
    }
}