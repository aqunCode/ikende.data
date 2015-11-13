using System;
namespace IKende.Data
{
	[Convert(typeof(float?[])), Convert(typeof(float[]))]
	internal class ToFloatArray : ToArray<float>
	{
		protected override bool Parse(string value, out float result)
		{
			return float.TryParse(value, out result);
		}
	}
}
