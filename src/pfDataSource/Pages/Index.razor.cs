using pfDataSource.Common.Configuration;

namespace pfDataSource.Pages
{
	public partial class Index
	{

        private string displayName;
        private string tempFilesPath;
        private DataSourceConfiguration configuration;

        private async Task OnFormSubmit()
        {
            configuration.DisplayName = displayName;
            await ConfigurationService.SaveAsync(configuration);
        }

        protected override async Task OnInitializedAsync()
        {
            configuration = await ConfigurationService.GetAsync();
            if (configuration is null)
            {
                configuration = new Common.Configuration.DataSourceConfiguration()
                {
                    DisplayType = typeof(Pages.Partials.EmptyConfiguration).FullName,
                    SourceType = typeof(Common.Configuration.EmptyConfiguration).Name
                };
                tempFilesPath = AppDomain.CurrentDomain.BaseDirectory;
            }
        }

    }
 }