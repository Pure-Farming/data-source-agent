using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using pfDataSource.Services.Models;
using pfDataSource.Db;
using System.Threading.Tasks;
using System;
using pfDataSource.Db.Models;
using System.Configuration;
using Serilog;
using Serilog.Sinks.Debug;

namespace pfDataSource.Services
{
	public class DataSourceConfigurationService : IDataSourceConfigurationService
	{
        private readonly ApplicationDbContext context;
        internal readonly IEncryptionProvider encryptionProvider;
        internal readonly ILogger logger;

        public DataSourceConfigurationService() { }


        public DataSourceConfigurationService(
            ApplicationDbContext context,
            IEncryptionProvider encryptionProvider,
            ILogger logger)
        {
            this.context = context;
            this.encryptionProvider = encryptionProvider;
            this.logger = logger;
        }

        public async Task<DataSourceConfiguration> GetAsync()
        {
            var found = await this.context.SourceConfigurations.FirstOrDefaultAsync();
            if (found is null) return null;

            var a = Assembly.GetAssembly(typeof(Common.Configuration.EmptyConfiguration));
            var sourceType = a.GetType(found.SourceType);

            var secretId = string.IsNullOrWhiteSpace(found.AwsSecrectId) ? null : encryptionProvider.Decrypt(found.AwsSecrectId);
            var secretKey = string.IsNullOrWhiteSpace(found.AwsSecretKey) ? null : encryptionProvider.Decrypt(found.AwsSecretKey);

            return BuildConfigurationObject(found, secretId, secretKey, sourceType);

        }

        internal DataSourceConfiguration BuildConfigurationObject(Db.Models.SourceConfiguration found, string SecretId, string SecretKey, Type sourceType = null)
        {
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
                    SecretId = SecretId,
                    SecretKey = SecretKey
                },
                Configuration = found.Configuration is null ?
                    null :
                    JsonConvert.DeserializeObject<Models.FileConfiguration>(found.Configuration)
            };
        }

        internal SourceConfiguration BuildSourceObject(Db.Models.SourceConfiguration found, DataSourceConfiguration configuration, string secrectId, string secretKey)
        {
            found.DisplayName = configuration.DisplayName;
            found.PureFarmingFullSourceName = $"com.purefarming.data-source.{configuration.FullName}";
            found.PureFarmingSourceName = configuration.Name;
            found.SourceType = configuration.SourceType;
            found.DisplayType = configuration.DisplayType;
            found.AwsS3BucketArn = configuration.Aws?.S3BucketArn;
            found.TempFilesPath = configuration.TempFilesPath;
            found.AwsSecrectId = secrectId;
            found.AwsSecretKey = secretKey;

            if (configuration.Configuration is not null)
                found.Configuration = JsonConvert.SerializeObject(configuration.Configuration);

            return found;
        }

        public async Task SaveAsync(DataSourceConfiguration configuration)
        {
            var found = await this.context.SourceConfigurations.FirstOrDefaultAsync();
            if (found is null)
            {
                found = new Db.Models.SourceConfiguration();
                this.context.SourceConfigurations.Add(found);
            }

            var secrectId = string.IsNullOrWhiteSpace(configuration.Aws?.SecretId) ? null : encryptionProvider.Encrypt(configuration.Aws.SecretId);
            var secretKey = string.IsNullOrWhiteSpace(configuration.Aws?.SecretKey) ? null : encryptionProvider.Encrypt(configuration.Aws.SecretKey);

            found = BuildSourceObject(found, configuration, secrectId, secretKey);

            await this.context.SaveChangesAsync();
        }
    }
}

