using System;
namespace IKende.Data
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
	public class BindAttribute : Attribute
	{
		private IConvert mConvertHandler;
		public string Prefix
		{
			get;
			set;
		}
		public Type Convert
		{
			get;
			set;
		}
		public Type Fungible
		{
			get;
			set;
		}
		public IConvert GetConvert()
		{
			IConvert result;
			if (this.Convert == null)
			{
				result = null;
			}
			else
			{
				if (this.mConvertHandler == null)
				{
					this.mConvertHandler = (IConvert)Activator.CreateInstance(this.Convert);
				}
				result = this.mConvertHandler;
			}
			return result;
		}
	}
}
