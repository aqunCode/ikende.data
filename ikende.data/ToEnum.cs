using System;
using System.Collections.Specialized;
namespace IKende.Data
{
	public class ToEnum<T> : IConvert where T : struct
	{
		public object Parse(NameValueCollection data, string key, string prefix, out bool succeed)
		{
			string value = Utils.GetValue(data, key, prefix);
			object result;
			if (string.IsNullOrEmpty(value))
			{
				succeed = true;
				result = default(T);
			}
			else
			{
				IEnumValue enumConvert = Utils.GetEnumConvert(typeof(T));
				result = enumConvert.GetValue(value, out succeed);
			}
			return result;
		}
	}
}
