using Npgsql;
using System.Data.Common;

namespace pfDataSource.Services.Databases
{
    public class PostgresQueryable : DatabaseQueryable
    {
        protected override DbConnection GetConnection(string connectionString)
            => new NpgsqlConnection(connectionString);

        public static IDatabaseQueryable Create() => new PostgresQueryable();
    }
}

