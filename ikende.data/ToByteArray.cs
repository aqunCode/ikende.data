using System;
namespace IKende.Data
{
	[Convert(typeof(byte[])), Convert(typeof(byte?[]))]
	internal class ToByteArray : ToArray<byte>
	{
		protected override bool Parse(string value, out byte result)
		{
			return byte.TryParse(value, out result);
		}
	}
}
