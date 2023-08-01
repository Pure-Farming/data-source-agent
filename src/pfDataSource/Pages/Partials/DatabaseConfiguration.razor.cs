using System;
using Blace.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace pfDataSource.Pages.Partials
{
	public partial class DatabaseConfiguration
	{
        [Parameter, AllowNull]
        public Common.Configuration.DatabaseConfiguration Item { get; set; }

        private Common.Configuration.DatabaseQuery tempQuery = new Common.Configuration.DatabaseQuery();
        private Editor<Data.SqlEditorFile> Edit;
        private Data.SqlEditorFile tempEditorFile = new Data.SqlEditorFile();

        private async Task OnEditClick(Common.Configuration.DatabaseQuery item)
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

        private async Task OnAddQuery()
        {
            tempQuery = new Common.Configuration.DatabaseQuery();
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
            var existing = Item.Queries.FirstOrDefault(q => q.Name == tempQuery.Name);
            if (existing == null)
                Item.Queries.Add(tempQuery);

            await JS.InvokeVoidAsync("addNewModal", "false");
        }

        protected override void OnInitialized()
        {
            Edit = new Editor<Data.SqlEditorFile>();
            if (Item.Queries == null)
                Item.Queries = new List<Common.Configuration.DatabaseQuery>();
            
            base.OnInitialized();
        }
    }
}

