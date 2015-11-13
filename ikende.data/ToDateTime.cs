using System;
using System.Collections.Specialized;
namespace IKende.Data
{
	[Convert(typeof(DateTime)), Convert(typeof(DateTime?))]
	internal class ToDateTime : IConvert
	{
		public object Parse(NameValueCollection data, string key, string prefix, out bool succeed)
		{
			string value = Utils.GetValue(data, key, prefix);
			DateTime dateTime;
			succeed = DateTime.TryParse(value, out dateTime);
			return dateTime;
		}
	}
}
