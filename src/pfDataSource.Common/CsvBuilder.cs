using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pfDataSource.Common
{
	public interface ICsvBuilder<T>
	{
		void Build(IEnumerable<T> input);
		Task<bool> WriteAsync(string path);
    }

	public class CsvBuilder<T> : ICsvBuilder<T>
	{
		private readonly StringBuilder @string;
		private int countColumns;
		private Dictionary<string, int> columnMapping;
		private string[] columns;

		public CsvBuilder()
		{
			@string = new StringBuilder();
			columnMapping = new Dictionary<string, int>();
		}

		public void Build(IEnumerable<T> input)
		{
            var first = input.First();
			BuildHeaders(first);
			BuildRows(input);
        }

		private void BuildHeaders(T input)
		{
			columns = input.GetProperties().ToArray();
			countColumns = columns.Length;
			var index = 0;
			foreach(var c in columns)
			{
				var suffix = index >= countColumns ? string.Empty : ",";
				columnMapping.Add(c, index);
				@string.AppendFormat("\"{0}\"{1}", c, suffix);
				index += 1;
			}
			@string.AppendLine();
        }

		private void BuildRows(IEnumerable<T> rows)
		{
			foreach(var row in rows)
			{
				for(var colIndex = 0; colIndex < countColumns; colIndex++)
				{
					var colName = columns[colIndex];
					var value = row.GetPropertyValue(colName);
					var suffix = colIndex < (countColumns - 1) ? "," : string.Empty;
					@string.AppendFormat("\"{0}\"{1}", value, suffix);
				}
				@string.AppendLine();
			}
		}

		public Task<bool> WriteAsync(string path)
		{
			try
			{
				File.WriteAllText(path, @string.ToString());
				return Task.FromResult(true);
			}
			catch
			{
				return Task.FromResult(false);
			}
		}

		public StringBuilder GetString()
		{
			return @string;	
		}
	}
}

