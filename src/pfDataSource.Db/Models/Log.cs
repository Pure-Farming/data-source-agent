using System;
namespace pfDataSource.Db.Models
{
	public class Log
	{
        public int Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
		public string Severity { get; set; }
		public string Message { get; set; }
	}
}

