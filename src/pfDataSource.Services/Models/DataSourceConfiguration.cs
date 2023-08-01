using System;
namespace pfDataSource.Services.Models
{
	public class DataSourceConfiguration
	{
		public int Id { get; set; }
		public string DisplayName { get; set; }
		public string Name { get; set; }
		public string FullName { get; set; }
		public string SourceType { get; set; }
		public string DisplayType { get; set; }
		public string TempFilesPath { get; set; }
		public AwsConfiguration Aws { get; set; }
		public object Configuration { get; set; }

		public Type GetTypeInfo() => System.Type.GetType(SourceType);

		public class AwsConfiguration
		{
			public string SecretId { get; set; }
			public string SecretKey { get; set; }
			public string S3BucketArn { get; set; }
		}
	}
}

