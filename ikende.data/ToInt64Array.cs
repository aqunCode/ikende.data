using System;
namespace IKende.Data
{
	[Convert(typeof(long?[])), Convert(typeof(long[]))]
	internal class ToInt64Array : ToArray<long>
	{
		protected override bool Parse(string value, out long result)
		{
			return long.TryParse(value, out result);
		}
	}
}
