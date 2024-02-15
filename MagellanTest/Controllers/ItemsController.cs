using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;

namespace MagellanTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {

        string connectionString = "Host=localhost;Port=5432;Password=remoter;Database=Part";
        NpgsqlConnection connection;
        


        private void Connect()
        {
            connection = new NpgsqlConnection(connectionString);

            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            
        }

        private void test()
        {
            DataTable l = GetData(new string("select * from Item;"));
            Console.WriteLine(l.ToString());
        }

        protected DataTable GetData(string sql)
        {
            Connect();
            DataTable dt = new DataTable();

            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = sql;

            NpgsqlDataReader reader = command.ExecuteReader();
            dt.Load(reader);
            return dt;
        }
    }
}
