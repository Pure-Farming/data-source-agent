using System;
using System.Configuration;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.JSInterop;
using pfDataSource.Common;
using Serilog;
using Newtonsoft.Json;
using pfDataSource.Services;
using pfDataSource.Common.Configuration;
using Blace.Components;

namespace pfDataSource.Pages
{
	public partial class Configure
	{
        private DataSourceConfiguration configuration;
        private FileConfiguration fileConfiguration;
        private DatabaseConfiguration databaseConfiguration;
        private DatabaseQuery tempQuery = new DatabaseQuery();
        private Editor<Data.SqlEditorFile> Edit;
        private Data.SqlEditorFile tempEditorFile = new Data.SqlEditorFile();

        private bool useFileBasedMethod = false;

        private bool watchDirectory = true;

        private string configurationDisplayTypeName = typeof(Partials.EmptyConfiguration).FullName;
        private Type configurationDisplayType => Type.GetType(configurationDisplayTypeName);
        private Dictionary<string, object> connectionConfiguration = new Dictionary<string, object>();
        private IJSObjectReference module;
        private EditContext? editContext;
        private bool editContextValidates = false;


        protected override async Task OnInitializedAsync()
        {
            configuration = await DataSourceConfigurationService.GetAsync();
            if (configuration is null) configuration = new DataSourceConfiguration();
            configuration.FullName = configuration.FullName?.Replace("com.purefarming.data-source.", string.Empty);

            fileConfiguration = new FileConfiguration();
            databaseConfiguration = new DatabaseConfiguration();
            databaseConfiguration.Queries = new List<DatabaseQuery>();

            if (configuration.Configuration is not null)
            {
                if (configuration.DisplayType == "FileConfiguration")
                {
                    fileConfiguration = (FileConfiguration)configuration.Configuration;
                }
                else
                {
                    databaseConfiguration = (DatabaseConfiguration)configuration.Configuration;
                 
                }

                watchDirectory = fileConfiguration.WatchDirectory;
            }

            editContext = new(configuration);

            await JS.InvokeVoidAsync("console.log", JsonConvert.SerializeObject(databaseConfiguration));

            editContextValidates = editContext.Validate();

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

        private async Task OnAddQuery()
        {
            tempQuery = new DatabaseQuery();
            tempEditorFile = new Data.SqlEditorFile();
            await Edit.Open(tempEditorFile, new Blace.Editing.EditorOptions
            {
                Syntax = Blace.Editing.Syntax.Sql,
                Theme = Blace.Editing.Theme.Sqlserver
            });
            await JS.InvokeVoidAsync("addNewModal", "true");
        }

        private async Task OnDismissAdd()
        {
            await Edit.Close();
            await JS.InvokeVoidAsync("addNewModal", "false");
        }

        private async Task OnSaveAdd()
        {
            tempQuery.Query = tempEditorFile.Content;
            var existing = databaseConfiguration.Queries.FirstOrDefault(q => q.Name == tempQuery.Name);
            if (existing == null)
                databaseConfiguration.Queries.Add(tempQuery);

            await JS.InvokeVoidAsync("addNewModal", "false");
        }

        private void SetDataSourceType(Type type, object config)
        {
            configurationDisplayTypeName = type.FullName;
            configuration.DisplayType = type.FullName;
            configuration.SourceType = $"pfDataSource.Common.Configuration.{type.Name}";
            configuration.Configuration = config;
            connectionConfiguration = new Dictionary<string, object> { { "Item", config } };
        }

        private async Task OnEditClick(DatabaseQuery item)
        {
            tempQuery = item;
            tempEditorFile = new Data.SqlEditorFile()
            {
                Sql = tempQuery.Query
            };

            await Edit.Open(tempEditorFile, new Blace.Editing.EditorOptions
            {
                Syntax = Blace.Editing.Syntax.Sql,
                Theme = Blace.Editing.Theme.Sqlserver
            });
            await JS.InvokeVoidAsync("addNewModal", "true");
        }

        private void OnDeleteClick(DatabaseQuery item)
        {
            databaseConfiguration.Queries.Remove(item);
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

