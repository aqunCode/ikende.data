using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
namespace IKende.Data
{
    [Convert(typeof(IList<PostFile>)), Convert(typeof(List<PostFile>))]
    internal class ToFiles : IConvert
    {
        public object Parse(NameValueCollection data, string key, string prefix, out bool succeed)
        {
            List<PostFile> list = new List<PostFile>();
            HttpFileCollection files = HttpContext.Current.Request.Files;
            if (files.Count > 0)
            {
                succeed = true;
                for (int i = 0; i < files.Count; i++)
                {
                    if (files[i].InputStream.Length > 0)
                        list.Add(new PostFile
                        {
                            File = files[i]
                        });
                }
            }
            succeed = false;
            return list;
        }
    }
}
