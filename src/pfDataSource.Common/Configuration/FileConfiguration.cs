using System;
namespace pfDataSource.Common.Configuration
{
	public class FileConfiguration : DataSourceConfiguration
	{
		public string Path { get; set; }
		public string FileType { get; set; }
		public int SubmissionDelay { get; set; } = 0;
		public bool WatchDirectory { get; set; }
		public string CronExpression { get; set; }
	}
}

