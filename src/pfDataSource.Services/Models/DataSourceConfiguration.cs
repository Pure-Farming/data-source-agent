using System;
using System.ComponentModel.DataAnnotations;

namespace pfDataSource.Services.Models
{
	public class DataSourceConfiguration
	{
		public int Id { get; set; }

        public string DisplayName { get; set; }
   
        [Required]
        public string Name { get; set; }

        [Required]
        public string FullName { get; set; }
		public string SourceType { get; set; }
		public string DisplayType { get; set; }
        public string TempFilesPath { get; set; }
		public object Configuration { get; set; }
		public string S3BucketArn { get; set; }


        public Type GetTypeInfo() => System.Type.GetType(SourceType);

	}
}

