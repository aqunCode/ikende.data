using System;
using System.Collections.Specialized;
namespace IKende.Data
{
	[Convert(typeof(double)), Convert(typeof(double?))]
	internal class ToDouble : IConvert
	{
		public object Parse(NameValueCollection data, string key, string prefix, out bool succeed)
		{
			string value = Utils.GetValue(data, key, prefix);
			double num = 0.0;
			succeed = double.TryParse(value, out num);
			return num;
		}
	}
}
