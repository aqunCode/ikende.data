using System;
namespace IKende.Data
{
	[Convert(typeof(decimal?[])), Convert(typeof(decimal[]))]
	internal class ToDecimalArray : ToArray<decimal>
	{
		protected override bool Parse(string value, out decimal result)
		{
			return decimal.TryParse(value, out result);
		}
	}
}
