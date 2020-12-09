using System;
using System.Data;
using System.Data.SqlClient;

namespace SqlServerSample
{
    static class TransactionProvider
    {
        //usuwanie pojazdow wyprodukowanych w zeszlym roku
        static public void Transaction1(SqlConnection connection)
        {
            Console.Clear();
            Console.Write("Removing ");
            System.Threading.Thread.Sleep(333);
            Console.Write(". ");
            System.Threading.Thread.Sleep(333);
            Console.Write(". ");
            System.Threading.Thread.Sleep(333);
            Console.WriteLine(". ");
            System.Threading.Thread.Sleep(333);

            string queryString = "delete from Pojazdy where year(data_produkcji)=2019;";
            SqlTransaction transaction = null;
            try
            {
                transaction = connection.BeginTransaction();
                using (SqlCommand command = new SqlCommand(queryString, connection, transaction))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    transaction.Commit();
                    transaction.Dispose();
                    Console.WriteLine($"Done. Rows affected: {rowsAffected}");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                Console.WriteLine("  Message: {0}", ex.Message);
                try
                {
                    if(transaction!=null)
                        transaction.Rollback();
                }
                catch(Exception ex2)
                {
                    Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                    Console.WriteLine("  Message: {0}", ex2.Message);
                }
            }
            Console.WriteLine("Dowolny klawisz aby kontynuowac.");
            Console.ReadKey(true);
        }

        //dodawanie trzech pojazdow
        static public void Transaction2(SqlConnection connection)
        {
            Console.Clear();
            (string[] marka,
                string[] vin,
                DateTime?[] data_produkcji,
                decimal?[] cena,
                bool[] wynik_przegladu) = GetDataFromUser();

            SqlTransaction transaction = null;
            string queryString = "insert Into POjazdy (marka,vin,data_produkcji,cena,wynik_przegladu) values" +
                        " (@marka, @vin, @data_produkcji, @cena, @wynik);";
            try
            {
                transaction = connection.BeginTransaction();
                using (SqlCommand command = new SqlCommand(queryString, connection, transaction))
                {
                    command.Parameters.Add(new SqlParameter("@marka", SqlDbType.VarChar, 20));
                    command.Parameters.Add(new SqlParameter("@vin", SqlDbType.VarChar, 20));
                    command.Parameters.Add(new SqlParameter("@data_produkcji", SqlDbType.DateTime));
                    command.Parameters.Add(new SqlParameter("@cena", SqlDbType.Decimal));
                    command.Parameters[3].Scale = 2;
                    command.Parameters[3].Precision = 15;
                    command.Parameters.Add(new SqlParameter("@wynik", SqlDbType.Bit));

                    for (int i=0;i<3; i++)
                    {
                        command.Parameters[0].Value = marka[i];
                        command.Parameters[1].Value = vin[i];
                        command.Parameters[2].Value = data_produkcji[i].HasValue ? (object)data_produkcji[i] : DBNull.Value;
                        command.Parameters[3].Value = cena[i].HasValue ? (object)cena[i] : DBNull.Value;
                        command.Parameters[4].Value = wynik_przegladu[i];
                        command.Prepare();

                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    Console.WriteLine($"Done.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                Console.WriteLine("  Message: {0}", ex.Message);
                try
                {
                    if (transaction != null)
                        transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                    Console.WriteLine("  Message: {0}", ex2.Message);
                }
            }
            Console.WriteLine("Dowolny klawisz aby kontynuowac.");
            Console.ReadKey(true);
        }

        //aktualizacja przegladu
        static public void Transaction3(SqlConnection connection)
        {
            Console.Clear();
            Console.Write("Updating ");
            System.Threading.Thread.Sleep(333);
            Console.Write(". ");
            System.Threading.Thread.Sleep(333);
            Console.Write(". ");
            System.Threading.Thread.Sleep(333);
            Console.WriteLine(". ");
            System.Threading.Thread.Sleep(333);

            string queryString1 = "select top(1) * from pojazdy " +
                                  "where data_produkcji is not null " +
                                  "order by data_produkcji; ";
            string queryString2 = "update pojazdy set wynik_przegladu = 0 where pojazd_id = @id";
            string queryString3 = @"INSERT INTO Pojazdy (marka, vin, data_produkcji, cena, wynik_przegladu) VALUES " +
                                  "(@marka, @vin, @data, @cena, 1);";

            int id = 0;
            string marka = null, vin = null; ;
            decimal cena = 0;

            SqlTransaction transaction = null;

            try
            {
                transaction = connection.BeginTransaction();
                using (SqlCommand command = new SqlCommand(queryString1, connection, transaction))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            id = (int)reader[0];
                            marka = (string)reader[1];
                            vin = (string)reader[2];
                            cena = (decimal)reader[4];
                        }
                    }

                    if (marka == null)
                    {
                        Console.WriteLine("Brak pojazdu do zaaktualizowania");
                        return;
                    }

                    command.CommandText = queryString2;
                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                    command.Parameters[0].Value = id;
                    command.Prepare();
                    command.ExecuteNonQuery();

                    command.CommandText = queryString3;
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlParameter("@marka", SqlDbType.VarChar, 20));
                    command.Parameters.Add(new SqlParameter("@vin", SqlDbType.VarChar, 20));
                    command.Parameters.Add(new SqlParameter("@data", SqlDbType.DateTime));
                    command.Parameters.Add(new SqlParameter("@cena", SqlDbType.Decimal));
                    command.Parameters[0].Value = marka;
                    command.Parameters[1].Value = vin;
                    command.Parameters[2].Value = DateTime.Now;
                    command.Parameters[3].Value = cena;
                    command.Parameters[3].Precision = 15;
                    command.Parameters[3].Scale = 2;
                    command.Prepare();
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    Console.WriteLine($"Done");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                Console.WriteLine("  Message: {0}", ex.Message);
                try
                {
                    if (transaction != null)
                        transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                    Console.WriteLine("  Message: {0}", ex2.Message);
                }
            }
            Console.WriteLine("Dowolny klawisz aby kontynuowac.");
            Console.ReadKey(true);
        }

        //usuwanie pojazdow starszyz niz podana data
        static public void Transaction4(SqlConnection connection)
        {
            Console.Clear();

            SqlTransaction transaction = null;
            string queryString = "delete from Pojazdy where data_produkcji< @date; ";
            try
            {
                transaction = connection.BeginTransaction();
                using (SqlCommand command = new SqlCommand(queryString, connection, transaction))
                {
                    DateTime date;
                    string userContent;
                    Console.WriteLine("Podaj date (format obslugiwany przec C# np. YYYY-DD-MMTHH:MM:SS): ");
                    do
                    {
                        userContent = Console.ReadLine();
                    } while (!DateTime.TryParse(userContent, out date));

                    command.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime));
                    command.Parameters[0].Value = date;

                    int rowsAffected = command.ExecuteNonQuery();

                    transaction.Commit();
                    Console.WriteLine($"Done. Rows affected: {rowsAffected}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                Console.WriteLine("  Message: {0}", ex.Message);
                try
                {
                    if (transaction != null)
                        transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                    Console.WriteLine("  Message: {0}", ex2.Message);
                }
            }
            Console.WriteLine("Dowolny klawisz aby kontynuowac.");
            Console.ReadKey(true);
        }

        //wypisywanie wszystkich
        static public void Transaction5(SqlConnection connection)
        {
            Console.Clear();

            string queryString1 = "select * from pojazdy ";
            int i = 0;
            SqlTransaction transaction = null;
            try
            {
                transaction = connection.BeginTransaction();
                using (SqlCommand command = new SqlCommand(queryString1, connection,transaction))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine();
                        Console.WriteLine($"   Nr.{"id",4}{"marka",25}{"vin",25}{"data_produkcji",25}{"cena",15}{"wynik przegladu",25}");
                        Console.WriteLine(new string('-', 125));
                        while (reader.Read())
                        {
                            int id = (int)reader[0];
                            string marka = (string)reader[1];
                            string vin = (string)reader[2];
                            DateTime? date = null;
                            if (!reader.IsDBNull(3))
                                date = (DateTime)reader[3];
                            string cena = null;
                            if (!reader.IsDBNull(4))
                                cena = ((decimal)reader[4]).ToString("0.##");
                            int wynik = (bool)reader[5] ? 1 : 0;
                            Console.WriteLine($"{++i,5}.{id,5}{marka,25}{vin,25}{date,23}{cena,15}{wynik,25}");
                            Console.WriteLine(new string('-', 125));

                        }
                        Console.WriteLine();
                        reader.Close();
                    }
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                Console.WriteLine("  Message: {0}", ex.Message);
                try
                {
                    if (transaction != null)
                        transaction.Rollback();
                }
                catch(Exception ex2)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex2.GetType());
                    Console.WriteLine("  Message: {0}", ex2.Message);
                }
            }
            Console.WriteLine("Dowolny klawisz aby kontynuowac.");
            Console.ReadKey(true);
        }
       
        //sprawdzanie poprawnosci danych wpisywanych przez uzytkownika
        static private bool InvalidData(string data)
        {
            for (int i = 0; i < data.Length; i++)
                if (!char.IsLetterOrDigit(data[i]))
                    return true;
            return false;
        }
        static private (string[], string[], DateTime?[], decimal?[], bool[]) GetDataFromUser()
        {
            string[] marka = new string[3];
            string[] vin = new string[3];
            DateTime?[] data_produkcji = new DateTime?[3];
            decimal?[] cena = new decimal?[3];
            bool[] wynik_przegladu = new bool[3];

            string s;
            DateTime date = DateTime.Now;
            bool success = false;
            decimal price = 0;
            for(int i=0;i<3;i++)
            {
                do
                {
                    Console.WriteLine("Podaj marke samochodu (max. 20 znakow, min. 1):");
                    marka[i] = Console.ReadLine();
                } while (marka[i].Length < 1 && marka[i].Length > 20 || InvalidData(marka[i]));

                do
                {
                    Console.WriteLine("Podaj VIN samochodu (max. 20 znakow, min. 1):");
                    vin[i] = Console.ReadLine();
                } while (vin[i].Length < 1 || vin[i].Length > 20 || InvalidData(vin[i]));
                
                do
                {
                    Console.WriteLine("Podaj date produkcji (np. w formacie RRRR-MM-DDTHH:MM:SS) lub wpisz NULL");
                    s = Console.ReadLine();
                    if(s.ToUpper()=="NULL")
                    {
                        data_produkcji[i] = null;
                        break;
                    }
                } while (!(success=DateTime.TryParse(s,out date)));
                if (success) data_produkcji[i] = date;

                do
                {
                    Console.WriteLine("Podaj cene z dokladnosci do dwoch cyfr po kropce (np. 2500.00) lub wpisz NULL");
                    s = Console.ReadLine();
                    if (s.ToUpper() == "NULL")
                    {
                        cena[i] = null;
                        break;
                    }
                } while (!(success = decimal.TryParse(s,out price)));
                if (success) cena[i] = price;

                Console.WriteLine("Wciśnij T, jeżeli ma pozytywny wynik przegladu. Dowolny inny klawisz w przeciwnym przypadku");
                wynik_przegladu[i] = Console.ReadKey(true).Key == ConsoleKey.T ? true : false; 
            }
            return (marka, vin, data_produkcji, cena, wynik_przegladu);
        }
    }
}
