using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace pfDataSource.Services.Databases
{
	public abstract class DatabaseQueryable : IDatabaseQueryable
	{
        protected abstract DbConnection GetConnection(string connectionString);

        public async Task<IEnumerable<dynamic>> Query(string query, string connectionString)
        {
            IEnumerable<dynamic> results = Enumerable.Empty<dynamic>();
            using (var c = GetConnection(connectionString))
            {
                try
                {
                    await c.OpenAsync();
                    results = await c.QueryAsync(query);
                }
                catch (Exception e)
                {
                    await c.CloseAsync();
                }
                finally
                {
                    await c.CloseAsync();
                }
            }

            return results;
        }
    }
}

