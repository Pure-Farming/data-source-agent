using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using pfDataSource.Common;

namespace pfDataSource.Services.Jobs
{
	public abstract class Job
	{
		private readonly IPushToAwsService pushToAwsService;
		private readonly IDataSourceConfigurationService dataSourceConfigurationService;

		protected Job(
			IPushToAwsService pushToAwsService,
			IDataSourceConfigurationService dataSourceConfigurationService)
		{
			this.pushToAwsService = pushToAwsService;
			this.dataSourceConfigurationService = dataSourceConfigurationService;
		}

		protected async Task PushResultsToAws(IEnumerable<object> input)
		{
			var configuration = await this.dataSourceConfigurationService.GetAsync();
			var fileName = $"{Guid.NewGuid()}.csv";
			var filePath = Path.Combine(configuration.TempFilesPath, fileName);

			var builder = new CsvBuilder<object>();
			builder.Build(input);
			await builder.WriteAsync(filePath);

			await PushResultsToAws(filePath);
		}

		protected async Task PushResultsToAws(string filePath)
		{
            var configuration = await this.dataSourceConfigurationService.GetAsync();
            await this.pushToAwsService.Push(filePath, configuration.Name);
        }
    }

	public abstract class Job<T> : Job
	{
		public Job(
            IPushToAwsService pushToAwsService,
            IDataSourceConfigurationService dataSourceConfigurationService)
            : base(pushToAwsService, dataSourceConfigurationService) { }

		public abstract Task Run(string ingestionName, T input);
	}

	public abstract class Job<T1, T2> : Job
	{
        public Job(
            IPushToAwsService pushToAwsService,
            IDataSourceConfigurationService dataSourceConfigurationService)
            : base(pushToAwsService, dataSourceConfigurationService) { }

        public abstract Task Run(string ingestionName, T1 inputOne, T2 inputTwo);
	}
}

