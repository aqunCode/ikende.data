using System;
using System.Collections.Specialized;
namespace IKende.Data
{
	[Convert(typeof(byte)), Convert(typeof(byte?))]
	internal class ToByte : IConvert
	{
		public object Parse(NameValueCollection data, string key, string prefix, out bool succeed)
		{
			string value = Utils.GetValue(data, key, prefix);
			byte b = 0;
			succeed = byte.TryParse(value, out b);
			return b;
		}
	}
}
