using System;
using System.Reflection;
using System.ComponentModel.DataAnnotations;


namespace pfDataSource.Common.Configuration
{
	public class FileConfiguration
	{
        [Required]
        [RegularExpression(@"(\/.*|[a-zA-Z]:\\(?:([^<>:""\/\\|?*]*[^<>:""\/\\|?*.]\\|..\\)*([^<>:""\/\\|?*]*[^<>:""\/\\|?*.]\\?|..\\))?)", ErrorMessage = "Please provide a valid path. Windows paths must start with the drive, eg C:.")]
        public string Path { get; set; }
        public string FileType { get; set; }
		public int SubmissionDelay { get; set; } = 0;
		public bool WatchDirectory { get; set; }

        [Required]
        [RegularExpression(@"^((\*|\?|\d+((\/|\-){0,1}(\d+))*)\s*){5}$", ErrorMessage = "Cron expression must be in the POSIX format.")]      
        public string CronExpression { get; set; }

	}
}

