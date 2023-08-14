using System;
using System.Configuration;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.JSInterop;
using pfDataSource.Common;
using Serilog;
using Newtonsoft.Json;

namespace pfDataSource.Pages
{
	public partial class Configure
	{
        private Services.Models.DataSourceConfiguration configuration;
        private Common.Configuration.FileConfiguration fileConfiguration;
        private Common.Configuration.DatabaseConfiguration databaseConfiguration;

        private string ServerTechnology = "";
        private string ConnectionString = "";

        private bool useFileBasedMethod = false;

        private Boolean watchDirectory = true;

        private string configurationDisplayTypeName = typeof(Partials.EmptyConfiguration).FullName;
        private Type configurationDisplayType => Type.GetType(configurationDisplayTypeName);
        private Dictionary<string, object> connectionConfiguration = new Dictionary<string, object>();
        private IJSObjectReference module;
        private EditContext? editContext;


        protected override async Task OnInitializedAsync()
        {
            configuration = await DataSourceConfigurationService.GetAsync();
            if (configuration is null) configuration = new Services.Models.DataSourceConfiguration();
            configuration.FullName = configuration.FullName?.Replace("com.purefarming.data-source.", string.Empty);

            fileConfiguration = new Common.Configuration.FileConfiguration();
            databaseConfiguration = new Common.Configuration.DatabaseConfiguration();

            if (configuration.Configuration is not null)
            {
                if (configuration.DisplayType == "FileConfiguration")
                {
                    fileConfiguration = (Common.Configuration.FileConfiguration)configuration.Configuration;
                }
                else
                {
                    databaseConfiguration = (Common.Configuration.DatabaseConfiguration)configuration.Configuration;
                }

                watchDirectory = fileConfiguration.WatchDirectory;
            }

            editContext = new(configuration);

            await JS.InvokeVoidAsync("console.log", JsonConvert.SerializeObject(configuration));

        }

        private void HandleValidSubmit() {

            if(editContext != null && editContext.Validate())
            {
                OnFormSubmit();
            }
        }

        private async Task OnFormSubmit()
        {
           
            try{

                if (watchDirectory)
                {
                    fileConfiguration.CronExpression = string.Empty;
                    fileConfiguration.WatchDirectory = true;
                }
                else
                {
                    fileConfiguration.SubmissionDelay = 0;
                    fileConfiguration.WatchDirectory = false;
                }

                if (configuration.DisplayType == "FileConfiguration")
                {
                    configuration.Configuration = fileConfiguration;
                }
                else
                {
                    configuration.Configuration = databaseConfiguration;
                }

                await DataSourceConfigurationService.SaveAsync(configuration);
                await ConfigurationService.Configure();
                await module.InvokeVoidAsync("showToast", "toastSaveSuccess", true);
            }
            catch (Exception e)
            {
                await JS.InvokeVoidAsync("console.error", e.Message);
                await JS.InvokeVoidAsync("console.error", e.StackTrace);
                await module.InvokeVoidAsync("showToast", "toastSaveFailed", true);
            }
        }

        /*
        private void OnDataSourceTypeChanged(ChangeEventArgs args = null)
        {
            if (configuration == null) return;

            var incomingValue = args?.Value?.ToString();

            if (string.IsNullOrWhiteSpace(incomingValue))
                incomingValue = typeof(Partials.EmptyConfiguration).FullName;

            var displayType = Type.GetType(incomingValue);
            object incomingConfiguration;

            if (displayType.FullName == configuration.DisplayType)
            {
                incomingConfiguration = configuration.Configuration;
            }
            else
            {
                var incomingConfigurationTypeName = $"pfDataSource.Common.Configuration.{displayType.Name}";
                var a = System.Reflection.Assembly.GetAssembly(typeof(Common.Configuration.EmptyConfiguration));
                var incomingCongfigurationType = a.GetType(incomingConfigurationTypeName);
                incomingConfiguration = Activator.CreateInstance(incomingCongfigurationType);
            }

            SetDataSourceType(displayType, incomingConfiguration);
        }
        */

        private void SetDataSourceType(Type type, object config)
        {
            configurationDisplayTypeName = type.FullName;
            configuration.DisplayType = type.FullName;
            configuration.SourceType = $"pfDataSource.Common.Configuration.{type.Name}";
            configuration.Configuration = config;
            connectionConfiguration = new Dictionary<string, object> { { "Item", config } };
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/Configure.js");
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }
        }
    }
}

