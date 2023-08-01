using System;
using Blace.Editing;

namespace pfDataSource.Data
{
    public class SqlEditorFile : EditorFile
	{
		public SqlEditorFile() : base("SqlEditorFile")
		{
        }

        public string Sql { get; set; }

        protected override Task<string> LoadContent() => Task.FromResult(Sql);

        protected override Task<bool> SaveContent()
        {
            Sql = Content;
            return Task.FromResult(true);
        }
    }
}

