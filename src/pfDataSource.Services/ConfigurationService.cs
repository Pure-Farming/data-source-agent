using System.Threading.Tasks;
using Hangfire;
using Hangfire.Storage;
using pfDataSource.Common.Configuration;
using pfDataSource.Services.Jobs;
using pfDataSource.Services.Models;

namespace pfDataSource.Services
{
	public class ConfigurationService : IConfigurationService
	{
		private const string FileJobName = "PollFiles";
		private const string DbJobName = "DbQuery";

		private readonly IDataSourceConfigurationService dataSourceConfigurationService;
		private readonly IDirectoryWatcherService directoryWatcherService;

		public ConfigurationService(
			IDataSourceConfigurationService dataSourceConfigurationService,
			IDirectoryWatcherService directoryWatcherService)
		{
			this.dataSourceConfigurationService = dataSourceConfigurationService;
			this.directoryWatcherService = directoryWatcherService;
		}

		public async Task Configure()
		{
			var configuration = await this.dataSourceConfigurationService.GetAsync();
            RemoveRecurringJobs();

			if (configuration.SourceType == typeof(FileConfiguration).FullName) {
				ConfigureFile(configuration);
			}
			//else if (configuration.SourceType == typeof(DatabaseConfiguration).FullName)
               // ConfigureDatabase(configuration);
		}

		private static void RemoveRecurringJobs()
		{
			using var connection = JobStorage.Current.GetConnection();
			foreach (var recurringJob in connection.GetRecurringJobs())
				RecurringJob.RemoveIfExists(recurringJob.Id);
		}

		private static void ConfigureDatabase(Models.DataSourceConfiguration configuration)
		{
			var dbConfiguration = configuration.Configuration as DatabaseConfiguration;
			if (dbConfiguration == null) return;

			for (var i = 0; i < dbConfiguration.Queries.Count; i++)
			{
				RecurringJob.AddOrUpdate<DbCheckJob>(
					$"{DbJobName}_{i}",
					x => x.Run(configuration.Name, dbConfiguration, dbConfiguration.Queries[i]),
					dbConfiguration.Queries[i].CronExpression);
			}
		}

		private void ConfigureFile(Models.DataSourceConfiguration configuration)
		{
			var fileConfiguration = configuration.Configuration as FileConfiguration;
			if (fileConfiguration == null) return;

			if (fileConfiguration.WatchDirectory)
			{
				directoryWatcherService.Configure(fileConfiguration.Path, fileConfiguration.FileType);
				return;
			}

			RecurringJob.AddOrUpdate<FileCheckJob>(FileJobName, x => x.Run(configuration.Name, fileConfiguration), fileConfiguration.CronExpression);
		}
	}
}
