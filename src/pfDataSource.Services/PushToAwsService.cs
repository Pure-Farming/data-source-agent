using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;

namespace pfDataSource.Services
{
	public interface IPushToAwsService
	{
		Task Push(string filePath, string ingestion);
	}

	public class PushToAwsService : IPushToAwsService
	{
		private readonly IAmazonS3 amazonS3;
		private readonly IDataSourceConfigurationService dataSourceConfigurationService;

		public PushToAwsService(
			IAmazonS3 amazonS3,
			IDataSourceConfigurationService dataSourceConfigurationService)
		{
			this.amazonS3 = amazonS3;
			this.dataSourceConfigurationService = dataSourceConfigurationService;
		}

        public async Task Push(string filePath, string ingestion)
        {
			var configuration = await this.dataSourceConfigurationService.GetAsync();
			var fileInfo = new FileInfo(filePath);
			var fileName = fileInfo.Name;
			var s3BucketPath = Path.Combine(ingestion, DateTime.Now.ToString("yyyy-MM-dd"), fileName);

			await this.amazonS3.UploadObjectFromFilePathAsync(configuration.S3BucketArn,
				s3BucketPath,
				filePath,
				new Dictionary<string,  object>());
        }
    }
}

