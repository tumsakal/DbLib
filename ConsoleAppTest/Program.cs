using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbLib;
namespace ConsoleAppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Database dB = Database.Of("System.Data.SqlClient", $"Server=.;Database=MyTestDb;Integrated Security=SSPI;");
            //dB.Insert("Customers", new string[] { "Id", "FirstName", "LastName", "Gender" }, new object[] { 4, "a", "b", "c" });
            //dB.Update("Customers", new string[] { "Id", "FirstName", "LastName", "Gender" }, new object[] { 4, "A", "B", "C" });
            //dB.Delete("Customers", "Id", 4);
            //string sql = "SELECT COUNT(*) FROM Customers;";
            //int count = dB.ExecuteScalar<int>(sql);
            //Console.WriteLine($"Customer : {count}");
            //!Console.Read();
            //string last_staff_id = dB.ExecuteScalar<string>($"SELECT TOP 1 Id FROM Staffs WHERE Id LIKE '{"ST"}%' ORDER BY Id DESC");
            //Console.WriteLine($"Last Staff ID : {last_staff_id}");
            string sql = "SELECT * FROM Customers;";
            var res = dB.Query<string>(sql, reader => $"Id: {reader[0]}, Name: {reader[1]} {reader[2]}, Gender: {reader[3]}.");
            foreach (var item in res)
            {
                Console.WriteLine(item);
            }
            Console.Read();
        }
    }
}
