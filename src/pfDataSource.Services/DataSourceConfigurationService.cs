using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using pfDataSource.Services.Models;
using pfDataSource.Db;
using System.Threading.Tasks;

namespace pfDataSource.Services
{
	public class DataSourceConfigurationService : IDataSourceConfigurationService
	{
        private readonly ApplicationDbContext context;
        private readonly IEncryptionProvider encryptionProvider;

		public DataSourceConfigurationService(
            ApplicationDbContext context,
            IEncryptionProvider encryptionProvider)
		{
            this.context = context;
            this.encryptionProvider = encryptionProvider;
		}

        public async Task<DataSourceConfiguration> GetAsync()
        {
            var found = await this.context.SourceConfigurations.FirstOrDefaultAsync();
            if (found is null) return null;

            var a = Assembly.GetAssembly(typeof(Common.Configuration.EmptyConfiguration));
            var sourceType = a.GetType(found.SourceType);

            return new DataSourceConfiguration
            {
                DisplayName = found.DisplayName,
                Name = found.PureFarmingSourceName,
                FullName = found.PureFarmingFullSourceName,
                SourceType = found.SourceType,
                DisplayType = found.DisplayType,
                TempFilesPath = found.TempFilesPath,
                Aws = new DataSourceConfiguration.AwsConfiguration
                {
                    S3BucketArn = found.AwsS3BucketArn,
                    SecretId = string.IsNullOrWhiteSpace(found.AwsSecrectId) ? null : encryptionProvider.Decrypt(found.AwsSecrectId),
                    SecretKey = string.IsNullOrWhiteSpace(found.AwsSecretKey) ? null : encryptionProvider.Decrypt(found.AwsSecretKey)
                },
                Configuration = found.Configuration is null ?
                    null :
                    JsonConvert.DeserializeObject(found.Configuration, sourceType)
            };
        }

        public async Task SaveAsync(DataSourceConfiguration configuration)
        {
            var found = await this.context.SourceConfigurations.FirstOrDefaultAsync();
            if (found is null)
            {
                found = new Db.Models.SourceConfiguration();
                this.context.SourceConfigurations.Add(found);
            }

            found.DisplayName = configuration.DisplayName;
            found.PureFarmingFullSourceName = $"com.purefarming.data-source.{configuration.FullName}";
            found.PureFarmingSourceName = configuration.Name;
            found.SourceType = configuration.SourceType;
            found.DisplayType = configuration.DisplayType;
            found.AwsS3BucketArn = configuration.Aws?.S3BucketArn;
            found.TempFilesPath = configuration.TempFilesPath;
            found.AwsSecrectId = string.IsNullOrWhiteSpace(configuration.Aws?.SecretId) ? null : encryptionProvider.Encrypt(configuration.Aws.SecretId);
            found.AwsSecretKey = string.IsNullOrWhiteSpace(configuration.Aws?.SecretKey) ? null : encryptionProvider.Encrypt(configuration.Aws.SecretKey);

            if (configuration.Configuration is not null)
                found.Configuration = JsonConvert.SerializeObject(configuration.Configuration);

            await this.context.SaveChangesAsync();
        }
    }
}

