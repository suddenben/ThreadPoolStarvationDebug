
using Microsoft.Data.SqlClient;
using System.Data;

namespace ThreadPoolStarvationDebug.Data
{
    public class SqlDelayService(IConfiguration configuration)
    {
        private readonly string? connectionString = configuration.GetConnectionString("DefaultConnection");

        public async Task ExecuteAsync()
        {
            using SqlConnection connection = new(connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "WAITFOR DELAY '00:00:00.300'";
            command.CommandType = CommandType.Text;
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public void Execute()
        {
            using SqlConnection connection = new(connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "WAITFOR DELAY '00:00:00.300'";
            command.CommandType = CommandType.Text;
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
