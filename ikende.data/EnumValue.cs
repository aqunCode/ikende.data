using System;
namespace IKende.Data
{
	internal class EnumValue<T> : IEnumValue where T : struct
	{
		public object GetValue(string value, out bool succeed)
		{
			succeed = true;
			T t;
			if (!Enum.TryParse<T>(value, out t))
			{
				t = default(T);
			}
			return t;
		}
	}
}
