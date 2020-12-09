using System;
using System.Data;
using System.Data.SqlClient;

namespace SqlServerSample
{
    class Program
    {
        static void Main(string[] args)
        {   
            try
            {
                string connectionString = @"Server = DESKTOP-IJDKMBN; Database = Northwind; User Id = sa; Password = sastudent";             
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    UserInteractionProvider.Run(connection); 
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("All done. Press any key to finish...");
            Console.ReadKey(true);
        }
    }
}