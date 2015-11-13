using System;
using System.Collections.Specialized;
namespace IKende.Data
{
	[Convert(typeof(short?)), Convert(typeof(short))]
	internal class ToInt16 : IConvert
	{
		public object Parse(NameValueCollection data, string key, string prefix, out bool succeed)
		{
			string value = Utils.GetValue(data, key, prefix);
			short num = 0;
			succeed = short.TryParse(value, out num);
			return num;
		}
	}
}
