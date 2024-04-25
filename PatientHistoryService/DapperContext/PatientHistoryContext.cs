using System.Data;
using System.Data.SqlClient;

namespace PatientHistoryService.DapperContext
{
    public class PatientHistoryContext
    {
        private readonly IConfiguration _configuration;

        private readonly string _connectionString;

        public PatientHistoryContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    }
}
