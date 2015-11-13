using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
namespace IKende.Data
{
	public class ToIList<T> : IConvert
	{
		private static List<PropertyInfo> mProperties = null;
		public static List<PropertyInfo> Properties
		{
			get
			{
				List<PropertyInfo> result;
				lock (typeof(ToIList<T>))
				{
					if (ToIList<T>.mProperties == null)
					{
						ToIList<T>.mProperties = new List<PropertyInfo>();
						PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
						for (int i = 0; i < properties.Length; i++)
						{
							PropertyInfo item = properties[i];
							ToIList<T>.mProperties.Add(item);
						}
					}
					result = ToIList<T>.mProperties;
				}
				return result;
			}
		}
		public object Parse(NameValueCollection data, string key, string prefix, out bool succeed)
		{
			IList<T> list = null;
			succeed = true;
			Type type = Type.GetType("System.Collections.Generic.List`1");
			type = type.MakeGenericType(new Type[]
			{
				typeof(T)
			});
			list = (IList<T>)Activator.CreateInstance(type);
			int num = 0;
			Dictionary<PropertyInfo, string[]> dictionary = new Dictionary<PropertyInfo, string[]>();
			foreach (PropertyInfo current in ToIList<T>.Properties)
			{
				string[] array = Utils.GetValues(data, current.Name, prefix);
				if (array != null && array.Length > num)
				{
					num = array.Length;
				}
				dictionary.Add(current, array);
			}
			for (int i = 0; i < num; i++)
			{
				NameValueCollection nameValueCollection = new NameValueCollection();
				foreach (PropertyInfo current in ToIList<T>.Properties)
				{
					string[] array = dictionary[current];
					if (array != null && i < array.Length)
					{
						nameValueCollection.Add(current.Name, array[i]);
					}
				}
				T item = BinderHelper.CreateInstance<T>(nameValueCollection);
				list.Add(item);
			}
			return list;
		}
	}
}
