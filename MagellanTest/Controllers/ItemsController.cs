using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System.Data;
using System.Xml.Linq;

namespace MagellanTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {

        string connectionString = "Host=localhost;Port=5432;User Id=postgres;Password=remoter;Database=part";
        
        

        private NpgsqlConnection Get_Connection()
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection;
        }


        [HttpGet]
        [Route("getTotalCost")]
        public String Get_Total_Cost(string item_name)
        {
            //TODO: protect against injection
            DataTable l = GetData($"SELECT Get_Total_Cost('{item_name}') from item WHERE item_name = '{item_name}';");
            return l.ToString();
        }
        
        
        [HttpGet]
        [Route("test")]
        public String test()
        {
            DataTable l = GetData("select * from item;");
            return l.ToString();
        }


        [HttpPost]
        [Route("Insert")]
        public async Task<IActionResult> SetData(string name, int parent_id, int cost, DateOnly dt)
        {
            try
            {
                await using var connection = Get_Connection();
                await connection.OpenAsync();

                var sql = @"INSERT INTO item (name, parent_id, cost, req_date) VALUES (@Name, @ParentId, @Cost, @Date)";

                await using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("ParentId", parent_id);
                cmd.Parameters.AddWithValue("Cost", cost);
                cmd.Parameters.AddWithValue("Date", dt);

                await cmd.ExecuteNonQueryAsync();

                return Ok("Insert success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Insert fail: {ex.Message}");
            }
        }


        protected DataTable GetData(string sql)
        {
            
            using (NpgsqlConnection connection = Get_Connection())
            {
                DataTable dt = new DataTable();

                NpgsqlCommand command = new NpgsqlCommand(sql, connection);

                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                    return dt;
                }
            }

        }
    }
}
