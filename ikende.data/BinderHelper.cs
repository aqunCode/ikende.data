using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
namespace IKende.Data
{
	public class BinderHelper
	{
		private static Dictionary<Type, IConvert> mIListConvert;
		static BinderHelper()
		{
			BinderHelper.mIListConvert = new Dictionary<Type, IConvert>();
			BinderHelper.AddCustomConvert(new Assembly[]
			{
				typeof(BinderHelper).Assembly
			});
		}
		internal static void Full(object obj, NameValueCollection data, string prefix, bool ispostback)
		{
			ClassBinder binder = Utils.GetBinder(obj.GetType());
			binder.FullData(obj, data, prefix, ispostback);
		}
		private static IConvert GetListConvert(Type key)
		{
			IConvert result;
			lock (BinderHelper.mIListConvert)
			{
				IConvert convert = null;
				if (!BinderHelper.mIListConvert.TryGetValue(key, out convert))
				{
					Type typeFromHandle = typeof(ToIList<>);
					convert = (IConvert)Activator.CreateInstance(typeFromHandle.MakeGenericType(new Type[]
					{
						key.GetGenericArguments()[0]
					}));
					BinderHelper.mIListConvert.Add(key, convert);
				}
				result = convert;
			}
			return result;
		}
		internal static object CreateInstance(Type type, NameValueCollection data)
		{
			return BinderHelper.CreateInstance(type, data, null);
		}
		internal static object CreateInstance(Type type, NameValueCollection data, string prefix)
		{
			object result;
			if (Utils.Converts.ContainsKey(type))
			{
				IConvert convert = Utils.Converts[type];
				bool flag;
				result = convert.Parse(data, null, prefix, out flag);
			}
			else
			{
				ClassBinder binder = Utils.GetBinder(type);
				result = binder.CreateObject(data, prefix);
			}
			return result;
		}
		internal static T CreateInstance<T>(NameValueCollection data, string prefix)
		{
			return (T)BinderHelper.CreateInstance(typeof(T), data, prefix);
		}
		internal static T CreateInstance<T>(NameValueCollection data)
		{
			return BinderHelper.CreateInstance<T>(data, null);
		}
		public static void AddCustomConvert(params Assembly[] assemblies)
		{
			Utils.AddCustomConvert(assemblies);
		}
		public static T GetObject<T>() where T : new()
		{
			return BinderHelper.GetObject<T>(null);
		}
		public static T GetObject<T>(string prefix) where T : new()
		{
			return (T)BinderHelper.GetObject(typeof(T), prefix);
		}
		public static object GetObject(Type type, string prefix)
		{
            
			object obj = Activator.CreateInstance(type);
			BinderHelper.Full(obj, HttpContext.Current.Request.Params, prefix, false);
			return obj;
		}
		public static T[] GetValues<T>(string key)
		{
			T[] result = null;
			Type typeFromHandle = typeof(T);
			IConvert convert = null;
			object obj = null;
			if (!Utils.Converts.TryGetValue(typeFromHandle, out convert))
			{
				if (typeFromHandle.IsEnum)
				{
					Type type = typeof(ToEnumArray<>).MakeGenericType(new Type[]
					{
						typeFromHandle
					});
					convert = (IConvert)Activator.CreateInstance(type);
					Utils.AddCustomConvert(typeFromHandle, convert);
				}
			}
			bool flag = false;
			if (convert != null)
			{
				obj = convert.Parse(HttpContext.Current.Request.Params, key, null, out flag);
			}
			if (flag)
			{
				result = (T[])obj;
			}
			return result;
		}
		public static T GetValue<T>(string key)
		{
			T result = default(T);
			object value = BinderHelper.GetValue(typeof(T), key);
			if (value != null)
			{
				result = (T)value;
			}
			return result;
		}
		public static object GetValue(Type type, string key)
		{
			object result = null;
			IConvert convert = null;
			object obj = null;
			if (!Utils.Converts.TryGetValue(type, out convert))
			{
				if (type.IsEnum)
				{
					Type type2 = typeof(ToEnum<>).MakeGenericType(new Type[]
					{
						type
					});
					convert = (IConvert)Activator.CreateInstance(type2);
					Utils.AddCustomConvert(type, convert);
				}
			}
			bool flag = false;
			if (convert != null)
			{
				obj = convert.Parse(HttpContext.Current.Request.Params, key, null, out flag);
			}
			if (flag)
			{
				result = obj;
			}
			return result;
		}
		public static IList<T> GetList<T>() where T : new()
		{
			return BinderHelper.GetList<T>(HttpContext.Current.Request.Params, "");
		}
		public static IList<T> GetList<T>(NameValueCollection data, string prefix) where T : new()
		{
			return (IList<T>)BinderHelper.GetList(typeof(T), data, prefix);
		}
		public static IList GetList(Type type, NameValueCollection data, string prefix)
		{
			IConvert listConvert = BinderHelper.GetListConvert(type);
			bool flag;
			return (IList)listConvert.Parse(data, "", prefix, out flag);
		}
	}
}
