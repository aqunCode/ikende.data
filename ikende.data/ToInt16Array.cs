using System;
namespace IKende.Data
{
	[Convert(typeof(short?[])), Convert(typeof(short[]))]
	internal class ToInt16Array : ToArray<short>
	{
		protected override bool Parse(string value, out short result)
		{
			return short.TryParse(value, out result);
		}
	}
}
