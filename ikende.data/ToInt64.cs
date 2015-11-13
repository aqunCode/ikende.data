using System;
using System.Collections.Specialized;
namespace IKende.Data
{
	[Convert(typeof(long)), Convert(typeof(long?))]
	internal class ToInt64 : IConvert
	{
		public object Parse(NameValueCollection data, string key, string prefix, out bool succeed)
		{
			string value = Utils.GetValue(data, key, prefix);
			long num = 0L;
			succeed = long.TryParse(value, out num);
			return num;
		}
	}
}
