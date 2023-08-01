using System.IO;
using System.Threading.Tasks;

namespace pfDataSource.Services
{
	public class DirectoryWatcherService : IDirectoryWatcherService
	{
		private readonly FileSystemWatcher fileSystemWatcher;
        private readonly IPushToAwsService pushToAwsService;
        private readonly IDataSourceConfigurationService dataSourceConfigurationService;

		public DirectoryWatcherService(
            IPushToAwsService pushToAwsService,
            IDataSourceConfigurationService dataSourceConfigurationService)
		{
			fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            fileSystemWatcher.NotifyFilter =
                NotifyFilters.FileName |
                NotifyFilters.LastWrite |
                NotifyFilters.Size;

            this.pushToAwsService = pushToAwsService;
            this.dataSourceConfigurationService = dataSourceConfigurationService;
		}

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
            => PushFile(e.FullPath);

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
            => PushFile(e.FullPath);

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
            => PushFile(e.FullPath);

        public void Configure(string path, string extension)
		{
            fileSystemWatcher.Path = path;
            fileSystemWatcher.Filter = $"*{extension}";
		}

        private void PushFile(string path)
        {
            var t = PushFileAsync(path);
            t.Wait();
        }

        private async Task PushFileAsync(string path)
        {
            var configuration = await this.dataSourceConfigurationService.GetAsync();
            await this.pushToAwsService.Push(path, configuration.Name);
        }
	}
}

