using pfDataSource.Db.Models;
using System;
using System.Threading.Tasks;

namespace pfDataSource.Services
{
	public interface IDataSourceConfigurationService
	{

        Task<Common.Configuration.DataSourceConfiguration> GetAsync();

		Task SaveAsync(Common.Configuration.DataSourceConfiguration configuration);
	}
}

