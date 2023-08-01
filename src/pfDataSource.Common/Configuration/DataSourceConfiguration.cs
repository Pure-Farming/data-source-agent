using System;
namespace pfDataSource.Common.Configuration
{
	public abstract class DataSourceConfiguration
	{
		public DataSourceConfiguration()
		{
		}
	}

	public class EmptyConfiguration : DataSourceConfiguration { }
}

