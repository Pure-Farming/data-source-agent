using System;
namespace pfDataSource.Db.Models
{
	public class SourceConfiguration
	{
		public int Id { get; set; }
		public string SourceType { get; set; }
		public string DisplayType { get; set; }
		public string DisplayName { get; set; }
		public string TempFilesPath { get; set; }
		public string PureFarmingSourceName { get; set; }
		public string PureFarmingFullSourceName { get; set; }
		public string AwsSecrectId { get; set; }
		public string AwsSecretKey { get; set; }
		public string AwsS3BucketArn { get; set; }
		public string Configuration { get; set; }
	}
}

