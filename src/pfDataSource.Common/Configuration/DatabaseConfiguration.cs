using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pfDataSource.Common.Configuration
{
	public class DatabaseConfiguration : DataSourceConfiguration
	{
		public const string Postgres = "PostgreSQL";
		public const string SqlServer = "SQL Server";
		public const string MySql = "MySQL";

		[Required]
		public string ConnectionString { get; set; }
		public string ServerTechnology { get; set; }

        public List<DatabaseQuery> Queries { get; set; }
	}

	public class DatabaseQuery
	{
		public string Name { get; set; }
		public string Query { get; set; }
		public string CronExpression { get; set; }
		public bool Enabled { get; set; }
	}
}

