using System;
using System.Collections.Specialized;
using System.Web;
namespace IKende.Data
{
	[Convert(typeof(PostFile))]
	internal class ToFile : IConvert
	{
		public object Parse(NameValueCollection data, string key, string prefix, out bool succeed)
		{
			object result;
			if (HttpContext.Current.Request.Files.Count > 0)
			{
				succeed = true;
				result = new PostFile
				{
					File = HttpContext.Current.Request.Files[0]
				};
			}
			else
			{
				succeed = false;
				result = null;
			}
			return result;
		}
	}
}
