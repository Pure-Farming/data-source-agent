using System;
namespace pfDataSource.Services
{
	public interface IDirectoryWatcherService
	{
		void Configure(string path, string extension);
	}
}

