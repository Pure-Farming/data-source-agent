using System;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace pfDataSource.Pages
{
	public partial class Configure
	{
        private Services.Models.DataSourceConfiguration configuration;
        private string configurationDisplayTypeName = typeof(Partials.EmptyConfiguration).FullName;
        private Type configurationDisplayType => Type.GetType(configurationDisplayTypeName);
        private Dictionary<string, object> connectionConfiguration = new Dictionary<string, object>();
        private IJSObjectReference module;

        private List<Tuple<string, string>> ConfigurationTypes = new List<Tuple<string, string>>
        {
            new Tuple<string, string>(typeof(Pages.Partials.EmptyConfiguration).FullName, "Select the Connection Type"),
            new Tuple<string, string>(typeof(Pages.Partials.FileConfiguration).FullName, "File"),
            new Tuple<string, string>(typeof(Pages.Partials.DatabaseConfiguration).FullName, "Database")
        };

        protected override async Task OnInitializedAsync()
        {
            configuration = await DataSourceConfigurationService.GetAsync();
            if (configuration is null) configuration = new Services.Models.DataSourceConfiguration();
            configuration.FullName = configuration.FullName?.Replace("com.purefarming.data-source.", string.Empty);
            OnDataSourceTypeChanged(new ChangeEventArgs()
            {
                Value = configuration.DisplayType
            });
        }

        private async Task OnFormSubmit()
        {
            try
            {
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

