using System.Threading.Tasks;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;

namespace pfDataSource.Services
{
	public interface IAwsCredentialsService
	{
		Task<AWSOptions> GetCredentialsAsync();
	}

	public class AwsCredentialsService : IAwsCredentialsService
	{
		private readonly IConfiguration configuration;
		private readonly IDataSourceConfigurationService dataSourceConfigurationService;

		public AwsCredentialsService(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

        public async Task<AWSOptions> GetCredentialsAsync()
        {
			var options = this.configuration.GetAWSOptions();
			var dataSourceConfiguration = await this.dataSourceConfigurationService.GetAsync();

			if (dataSourceConfiguration == null) return options;
			if (string.IsNullOrWhiteSpace(dataSourceConfiguration.Aws?.SecretId) ||
				string.IsNullOrWhiteSpace(dataSourceConfiguration.Aws?.SecretKey))
				return options;

			options.Credentials = new BasicAWSCredentials(
				dataSourceConfiguration.Aws.SecretId,
				dataSourceConfiguration.Aws.SecretKey);
			return options;
        }
    }
}

