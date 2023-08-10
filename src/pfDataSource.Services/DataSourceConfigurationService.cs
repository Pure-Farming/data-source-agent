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

            return BuildConfigurationObject(found);

        }

        internal DataSourceConfiguration BuildConfigurationObject(Db.Models.SourceConfiguration found)
        {
            return new DataSourceConfiguration
            {
                DisplayName = found.DisplayName,
                Name = found.PureFarmingSourceName,
                FullName = found.PureFarmingFullSourceName,
                SourceType = found.SourceType,
                DisplayType = found.DisplayType,
                TempFilesPath = found.TempFilesPath,
                Configuration = found.Configuration is null ?
                    null :
                    JsonConvert.DeserializeObject<Models.FileConfiguration>(found.Configuration)
            };
        }

        internal SourceConfiguration BuildSourceObject(Db.Models.SourceConfiguration found, DataSourceConfiguration configuration)
        {
            found.DisplayName = configuration.DisplayName;
            found.PureFarmingFullSourceName = $"com.purefarming.data-source.{configuration.FullName}";
            found.PureFarmingSourceName = configuration.Name;
            found.SourceType = configuration.SourceType;
            found.DisplayType = configuration.DisplayType;
            found.TempFilesPath = configuration.TempFilesPath;

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

            found = BuildSourceObject(found, configuration);

            await this.context.SaveChangesAsync();
        }
    }
}

