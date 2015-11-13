using System;
namespace IKende.Data
{
	public  interface IEnumValue
	{
		object GetValue(string value, out bool succeed);
	}
}
