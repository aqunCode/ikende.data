using System;
using System.Collections.Specialized;
namespace IKende.Data
{
	[Convert(typeof(bool?)), Convert(typeof(bool))]
	internal class ToBoolean : IConvert
	{
		public object Parse(NameValueCollection data, string key, string prefix, out bool succeed)
		{
			string value = Utils.GetValue(data, key, prefix);
			bool flag = false;
			succeed = bool.TryParse(value, out flag);
			return flag;
		}
	}
}
