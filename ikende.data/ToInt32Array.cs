using System;
namespace IKende.Data
{
	[Convert(typeof(int[])), Convert(typeof(int?[]))]
	internal class ToInt32Array : ToArray<int>
	{
		protected override bool Parse(string value, out int result)
		{
			return int.TryParse(value, out result);
		}
	}
}
