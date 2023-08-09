using System.IO;
using System.Threading.Tasks;
using pfDataSource.Common.Configuration;
using pfDataSource.Services.Models;

namespace pfDataSource.Services.Jobs
{
	public class FileCheckJob : Job<FileConfiguration>
	{
		public FileCheckJob(
            IPushToAwsService pushToAwsService,
            IDataSourceConfigurationService dataSourceConfigurationService)
            : base(pushToAwsService, dataSourceConfigurationService) { }

        public override async Task Run(string ingestionName, FileConfiguration input)
        {
            var directory = input.Path;
            var directoryInfo = new DirectoryInfo(directory);
            var files = directoryInfo.EnumerateFiles(input.FileType);
            foreach (var f in files)
                await PushResultsToAws(f.FullName);
        }
    }
}

