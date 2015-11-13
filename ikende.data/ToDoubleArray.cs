using System;
namespace IKende.Data
{
	[Convert(typeof(double[])), Convert(typeof(double?[]))]
	internal class ToDoubleArray : ToArray<double>
	{
		protected override bool Parse(string value, out double result)
		{
			return double.TryParse(value, out result);
		}
	}
}
