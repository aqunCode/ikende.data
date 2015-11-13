using System;
using System.Collections.Specialized;
namespace IKende.Data
{
	public interface IConvert
	{
		object Parse(NameValueCollection data, string key, string prefix, out bool succeed);
	}
}
