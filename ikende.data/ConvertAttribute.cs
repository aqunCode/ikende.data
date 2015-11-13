using System;
namespace IKende.Data
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ConvertAttribute : Attribute
	{
		public Type Convert
		{
			get;
			set;
		}
		public ConvertAttribute()
		{
		}
		public ConvertAttribute(Type convert)
		{
			this.Convert = convert;
		}
	}
}
