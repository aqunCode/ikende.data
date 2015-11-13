using System;
using System.IO;
using System.Web;
namespace IKende.Data
{
	public class PostFile
	{
		public HttpPostedFile File
		{
			get;
			set;
		}
		public string FileName
		{
			get
			{
				return this.File.FileName;
			}
		}
		public int Length
		{
			get
			{
				return this.File.ContentLength;
			}
		}
		public string Ext
		{
			get
			{
				return Path.GetExtension(this.File.FileName);
			}
		}
		public byte[] GetData()
		{
			byte[] array = new byte[this.File.ContentLength];
			this.File.InputStream.Position = 0L;
			this.File.InputStream.Read(array, 0, array.Length);
			return array;
		}
        public ArraySegment<byte> ToData(byte[] data)
        {
            
            int count =(int)this.File.InputStream.Length;
            this.File.InputStream.Read(data, 0, count);
            return new ArraySegment<byte>(data, 0, count);
        }
		public ArraySegment<byte> GetData(byte[] data)
		{
			this.File.InputStream.Position = 0L;
			this.File.InputStream.Read(data, 0, this.Length);
			return new ArraySegment<byte>(data, 0, this.Length);
		}
	}
}
