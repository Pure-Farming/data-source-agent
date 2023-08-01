using System.Threading.Tasks;
using pfDataSource.Common.Configuration;
using pfDataSource.Services.Databases;

namespace pfDataSource.Services.Jobs
{
	public class DbCheckJob : Job<DatabaseConfiguration, DatabaseQuery>
	{
        public DbCheckJob(
            IPushToAwsService pushToAwsService,
            IDataSourceConfigurationService dataSourceConfigurationService)
            : base(pushToAwsService, dataSourceConfigurationService) { }

        public override async Task Run(string ingestionName, DatabaseConfiguration inputOne, DatabaseQuery inputTwo)
        {
            var q = GetDbQuery(inputOne);
            if (q == null) return;

            var result = await q.Query(inputTwo.Query, inputOne.ConnectionString);

            await PushResultsToAws(result);
        }

        private IDatabaseQueryable GetDbQuery(DatabaseConfiguration configuration)
        {
            if (configuration.ServerTechnology == DatabaseConfiguration.Postgres)
                return PostgresQueryable.Create();

            if (configuration.ServerTechnology == DatabaseConfiguration.SqlServer)
                return SqlServerQueryable.Create();

            if (configuration.ServerTechnology == DatabaseConfiguration.MySql)
                return MySqlQueryable.Create();

            return null;
        }
    }
}

