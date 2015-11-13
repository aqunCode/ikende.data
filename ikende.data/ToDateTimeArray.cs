using System;
namespace IKende.Data
{
	[Convert(typeof(DateTime?[])), Convert(typeof(DateTime[]))]
	internal class ToDateTimeArray : ToArray<DateTime>
	{
		protected override bool Parse(string value, out DateTime result)
		{
			return DateTime.TryParse(value, out result);
		}
	}
}
