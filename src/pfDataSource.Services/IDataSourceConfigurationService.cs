using pfDataSource.Db.Models;
using System;
using System.Threading.Tasks;

namespace pfDataSource.Services
{
	public interface IDataSourceConfigurationService
	{

        Task<Models.DataSourceConfiguration> GetAsync();

		Task SaveAsync(Models.DataSourceConfiguration configuration);
	}
}

