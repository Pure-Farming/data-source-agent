using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace pfDataSource.Services.Databases
{
	public interface IDatabaseQueryable
	{
		Task<IEnumerable<dynamic>> Query(string query, string connectionString);
	}
}

