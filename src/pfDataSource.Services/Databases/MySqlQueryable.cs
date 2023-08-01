using MySql.Data.MySqlClient;
using System.Data.Common;

namespace pfDataSource.Services.Databases
{
	public class MySqlQueryable : DatabaseQueryable
	{
        protected override DbConnection GetConnection(string connectionString)
            => new MySqlConnection(connectionString);

        public static IDatabaseQueryable Create() => new MySqlQueryable();
    }
}

