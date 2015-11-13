using System;
namespace IKende.Data
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ViewStateAttribute : Attribute
	{
		public bool ByPostData
		{
			get;
			set;
		}
	}
}
