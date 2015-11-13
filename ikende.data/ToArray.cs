using System;
using System.Collections.Specialized;
namespace IKende.Data
{
	internal abstract class ToArray<T> : IConvert where T : struct
	{
		public object Parse(NameValueCollection data, string key, string prefix, out bool succeed)
		{
			T[] array = new T[0];
			succeed = true;
			string[] values = Utils.GetValues(data, key, prefix);
			succeed = (values != null && values.Length > 0);
			if (values != null)
			{
				array = new T[values.Length];
				for (int i = 0; i < values.Length; i++)
				{
					T t;
					if (this.Parse(values[i], out t))
					{
						array[i] = t;
					}
				}
			}
			return array;
		}
		protected abstract bool Parse(string value, out T result);
	}
}
