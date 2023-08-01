using System;
using System.Collections.Generic;
using System.Linq;

namespace pfDataSource.Common
{
	public static class ObjectExtensions
	{
		public static IEnumerable<string> GetProperties(this object obj)
		{
			var type = obj.GetType();
			var props = type.GetProperties();
			return props.Select(p => p.Name);
		}

		public static object GetPropertyValue(this object obj, string propertyName)
		{
            var type = obj.GetType();
			var property = type.GetProperty(propertyName);
			return property.GetValue(obj);
        }
	}
}

