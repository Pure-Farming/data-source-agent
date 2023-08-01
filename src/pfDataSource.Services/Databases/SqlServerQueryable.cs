using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace pfDataSource.Services.Databases
{
	public class SqlServerQueryable : DatabaseQueryable
	{
        protected override DbConnection GetConnection(string connectionString)
            => new SqlConnection(connectionString);

        public static IDatabaseQueryable Create() => new SqlServerQueryable();
    }
}

