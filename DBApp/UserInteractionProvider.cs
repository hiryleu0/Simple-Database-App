using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SqlServerSample
{
    static class UserInteractionProvider
    {
        static public void Run(SqlConnection connection)
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("Polaczono z baza danych.");
                Console.WriteLine("Prosze wcisnac odpowiedni klawisz na klawiaturze w celu wykonia operacji:");
                Console.WriteLine("\n");

                Console.WriteLine("1 - Usuniecie z tabeli pojazdow wyprodukowanych w zeszlym roku.\n");
                Console.WriteLine("2 - Dodanie trzech nowych pojazdow do tabeli na podst. danych uzytkownika.\n");
                Console.WriteLine("3 - Znalezienie najstarszego pojazdu, zmiana wyniku przegladu i dodanie poprawionego rekordu.\n");
                Console.WriteLine("4 - Usuwanie pojazdow strszych niz podana przez uzytkownika data.\n");
                Console.WriteLine("5 - Wyswietlenie zawartosci tabeli Pojazdy.\n\n");
                Console.WriteLine("Q - Wyjście z programu.");

            GettingKey:
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        TransactionProvider.Transaction1(connection);
                        break;
                    case ConsoleKey.D2:
                        TransactionProvider.Transaction2(connection);
                        break;
                    case ConsoleKey.D3:
                        TransactionProvider.Transaction3(connection);
                        break;
                    case ConsoleKey.D4:
                        TransactionProvider.Transaction4(connection);
                        break;
                    case ConsoleKey.D5:
                        TransactionProvider.Transaction5(connection);
                        break;
                    case ConsoleKey.Q:
                        running = false;
                        break;
                    default:
                        goto GettingKey;
                }
                Console.Clear();
            }
        }
    }
}
