using System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Remoting.Messaging;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Collections.Specialized;
//Copyright (c) 2005-2006 JianHan Fan	
//Mail:henryfan@msn.com
//All rights reserved.
//-------------------------------------
namespace IKende.Data
{
    public delegate void AsyncDelegate();
    public delegate void AsyncDelegate<T>(T t);
    public delegate void AsyncDelegate<T, T1>(T t, T1 t1);
    public delegate void AsyncDelegate<T, T1, T2>(T t, T1 t1, T2 t2);
    public delegate void AsyncDelegate<T, T1, T2, T3>(T t, T1 t1, T2 t2, T3 t3);
    public delegate void AsyncDelegate<T, T1, T2, T3, T4>(T t, T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate bool EventFindHandler<T>(T data);
    /// <summary>
	/// Validater 的摘要说明。
	/// </summary>
	public partial class Utils
	{

        private static Dictionary<Type, IEnumValue> mEnumConvert = new Dictionary<Type, IEnumValue>(128);
        private static IDictionary<Type, IConvert> mConverts;
        private static Dictionary<Type, ClassBinder> mClassBinders = new Dictionary<Type, ClassBinder>();
        internal static IDictionary<Type, IConvert> Converts
        {
            get
            {
                if (Utils.mConverts == null)
                {
                    Utils.LoadConverts();
                }
                return Utils.mConverts;
            }
        }
        internal static string GetValue(NameValueCollection data, string key, string prefix)
        {
            string text = data[key];
            string result;
            if (string.IsNullOrEmpty(prefix))
            {
                result = text;
            }
            else
            {
                if (string.IsNullOrEmpty(text))
                {
                    text = data[prefix + "." + key];
                }
                if (string.IsNullOrEmpty(text))
                {
                    text = data[prefix + "_" + key];
                }
                if (string.IsNullOrEmpty(text))
                {
                    text = data[prefix + key];
                }
                result = text;
            }
            return result;
        }
        public static IEnumValue GetEnumConvert(Type type)
        {
            IEnumValue enumValue;
            lock (Utils.mEnumConvert)
            {
                if (!Utils.mEnumConvert.TryGetValue(type, out enumValue))
                {
                    Type type2 = typeof(EnumValue<>).MakeGenericType(new Type[]
					{
						type
					});
                    enumValue = (IEnumValue)Activator.CreateInstance(type2);
                    Utils.mEnumConvert.Add(type, enumValue);
                }
            }
            return enumValue;
        }
        internal static string[] GetValues(NameValueCollection data, string key, string prefix)
        {
            string[] values = data.GetValues(key);
            string[] result;
            if (string.IsNullOrEmpty(prefix))
            {
                result = values;
            }
            else
            {
                if (values.Length == 0)
                {
                    values = data.GetValues(prefix + "." + key);
                }
                if (values.Length == 0)
                {
                    values = data.GetValues(prefix + "_" + key);
                }
                if (values.Length == 0)
                {
                    values = data.GetValues(prefix + key);
                }
                result = values;
            }
            return result;
        }
        internal static IConvert GetConvert(Type type)
        {
            IConvert result = null;
            Utils.Converts.TryGetValue(type, out result);
            return result;
        }
        private static void LoadConverts()
        {
            lock (typeof(Utils))
            {
                if (Utils.mConverts == null)
                {
                    Utils.mConverts = new Dictionary<Type, IConvert>();
                    Utils.LoadBaseConvert();
                }
            }
        }
        private static void AddConvert(Type convert, ConvertAttribute[] ca)
        {
            if (ca.Length > 0)
            {
                for (int i = 0; i < ca.Length; i++)
                {
                    ConvertAttribute convertAttribute = ca[i];
                    if (Utils.Converts.ContainsKey(convertAttribute.Convert))
                    {
                        Utils.Converts[convertAttribute.Convert] = (IConvert)Activator.CreateInstance(convert);
                    }
                    else
                    {
                        Utils.Converts.Add(convertAttribute.Convert, (IConvert)Activator.CreateInstance(convert));
                    }
                }
            }
        }
        private static void LoadBaseConvert()
        {
            Assembly assembly = typeof(Utils).Assembly;
            Utils.LoadConvertByAssembly(assembly);
        }
        private static void LoadConvertByAssembly(Assembly ass)
        {
            Type[] types = ass.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (type.GetInterface(typeof(IConvert).FullName) != null)
                {
                    Utils.AddConvert(type, Utils.GetTypeAttributes<ConvertAttribute>(type, false));
                }
            }
        }
        public static void AddCustomConvert(params Assembly[] assemblies)
        {
            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly ass = assemblies[i];
                Utils.LoadConvertByAssembly(ass);
            }
        }
        internal static ClassBinder GetBinder(Type type)
        {
            if (type.GetConstructor(new Type[0]) == null)
            {
                throw new Exception(string.Format("{0}不存在默认构造函数,无法构建数据绑定!", type.Name));
            }
            ClassBinder result;
            if (Utils.mClassBinders.ContainsKey(type))
            {
                result = Utils.mClassBinders[type];
            }
            else
            {
                lock (Utils.mClassBinders)
                {
                    if (!Utils.mClassBinders.ContainsKey(type))
                    {
                        Utils.mClassBinders.Add(type, new ClassBinder(type));
                    }
                    result = Utils.mClassBinders[type];
                }
            }
            return result;
        }
        public static void AddCustomConvert(Type type, IConvert convert)
        {
            lock (Utils.mConverts)
            {
                if (!Utils.mConverts.ContainsKey(type))
                {
                    Utils.mConverts.Add(type, convert);
                }
            }
        }

        public static long DirSize(DirectoryInfo d)
        {
            long Size = 0;

            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                Size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                Size += DirSize(di);
            }
            return (Size);
        }
        private Utils()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//

		}
        private static Random mRan = new Random();
        private static string mRanSeed = "abcdefghijklmnopqrstuvwxyz";
        public static string GetRanString(int length)
        {
            string result = "";
            for (int i = 0; i < length; i++)
            {
                result += mRanSeed.Substring(mRan.Next(25), 1);
            }
            return result;
        }
		public static bool IsEmpty(System.Array value)
		{
			if(value == null || value.Length ==0)
				return true;
			return false;
		}
		public static bool IsEmpty(System.Collections.IList value)
		{
			if(value == null || value.Count ==0)
				return true;
			return false;
		}
		public static bool IsEmpty(System.Collections.IDictionary value)
		{
			if(value == null || value.Count ==0)
				return true;
			return false;
		}
		public static bool IsEmpty(string value)
		{
            return string.IsNullOrEmpty(value);
		}
        public static string GetPath(System.Reflection.Assembly assemble)
        {
            return System.IO.Path.GetDirectoryName(assemble.Location) + @"\";
        }
        public static string GetPath(string path)
        {
            if (path.Substring(path.Length - 1, 1) != @"\")
                return path + @"\";
            return path;
        }
		public static bool IsEmpty(DateTime value)
		{
			if(value == DateTime.MinValue)
				return true;
			return false;
		}
		public static bool IsInt(string value)
		{
            int _default;
            return int.TryParse(value,out _default);
		}
		public static bool IsInt(object value)
		{
			return IsInt(value.ToString());
		}
		public static bool IsNumber(string value)
		{
			return Regex.IsMatch(value,@"^(\d+|(\d+\.\d+)+)$",RegexOptions.IgnoreCase);
		}
		public static bool IsNumber(object value)
		{
			return IsNumber(value.ToString());
		}
		public static bool IsDateTime(string value)
		{
            DateTime _default;
            return DateTime.TryParse(value,out _default);
		}
		public static bool IsDateTime(object value)
		{
			return IsDateTime(value.ToString());
		}
		public static Type GetType(string fullname)
		{

            string[] typename = fullname.Split(new char[] { ',' });
            Assembly ass = Assembly.Load(typename[1]);
            return ass.GetType(typename[0], true);
		}
        public static Type GetTypeByCache(string fullname)
        {
            if (!mTypeTable.ContainsKey(fullname))
            {
                lock (mTypeTable)
                {
                    if (!mTypeTable.ContainsKey(fullname))
                    {
                        Type type = Type.GetType(fullname);
                        mTypeTable.Add(fullname, type);
                    }
                }
            }
            return mTypeTable[fullname];
       
        }
        private static Dictionary<string, Type> mTypeTable = new Dictionary<string, Type>();
        /// <summary>
        /// 把对象序列化并返回相应的字节
        /// </summary>
        /// <param name="pObj">需要序列化的对象</param>
        /// <returns>byte[]</returns>
        public static byte[] SerializeObject(object pObj)
        {
            if (pObj == null)
                return null;
            using (System.IO.MemoryStream _memory = new System.IO.MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(_memory, pObj);
                _memory.Position = 0;
                byte[] read = new byte[_memory.Length];
                _memory.Read(read, 0, read.Length);
                _memory.Close();
                return read;
            }


        }
        public static string SerializeObjectToString(object pOjb)
        {
            Byte[] bytes = SerializeObject(pOjb);
            return System.Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// 把字节反序列化成相应的对象
        /// </summary>
        /// <param name="pBytes">字节流</param>
        /// <returns>object</returns>
        public static object DeserializeObject(byte[] pBytes)
        {
            return DeserializeObject(pBytes, pBytes.Length);
        }
        public static object DeserializeObject(byte[] pBytes, int index, int count)
        {
            object _newOjb = null;
            if (pBytes == null)
                return _newOjb;
            using (System.IO.MemoryStream _memory = new System.IO.MemoryStream(pBytes, index, count))
            {
                _memory.Position = 0;
                BinaryFormatter formatter = new BinaryFormatter();
                _newOjb = formatter.Deserialize(_memory);
                _memory.Close();
                return _newOjb;
            }
        }
        public static object DeserializeObject(byte[] pBytes,int count)
        {
            return DeserializeObject(pBytes, 0, count);
        
        }
        public static object DeserializeObjectByString(string text)
        {
            Byte[] bytes = System.Convert.FromBase64String(text);
            return DeserializeObject(bytes);
        }
        public static string UpperDate(DateTime time)
        {
            string number = "〇一二三四五六七八九";
            System.Text.StringBuilder date = new System.Text.StringBuilder();
            string[] infos = new string[] { time.Year.ToString(), time.Month.ToString("00"), time.Day.ToString("00") };
            int value;
            for (int i = 0; i < infos[0].Length; i++)
            {
                value = int.Parse(infos[0].Substring(i, 1));
                date.Append(number.Substring(value, 1));
            }
            date.Append("年");

            for (int i = 0; i < infos[1].Length; i++)
            {
                value = int.Parse(infos[1].Substring(i, 1));
                if (i == 0)
                {
                    if (value > 0)
                        date.Append("十");
                }
                else
                {
                    if (value > 0)
                        date.Append(number.Substring(value, 1));
                }

            }
            date.Append("月");
            for (int i = 0; i < infos[2].Length; i++)
            {
                value = int.Parse(infos[2].Substring(i, 1));
                if (i == 0)
                {
                    if (value > 0)
                    {
                        if (value > 1)
                            date.Append(number.Substring(value, 1));
                        date.Append("十");
                    }

                }
                else
                {
                    if (value > 0)
                    {
                        date.Append(number.Substring(value, 1));
                    }
                }
            }
            date.Append("日");
            return date.ToString();
        }
        public static T[] ListToArray<T>(System.Collections.IList list)
        {
            T[] items = new T[list.Count];
            list.CopyTo(items, 0);
            return items;
        }
        public static int GetByteCount(string hexString)
        {
            int numHexChars = 0;
            char c;
        
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                    numHexChars++;
            }
         
            if (numHexChars % 2 != 0)
            {
                numHexChars--;
            }
            return numHexChars / 2; 
        }
      
        public static byte[] GetBytes(string hexString, out int discarded)
        {
            discarded = 0;
            string newString = "";
            char c;
           
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                    newString += c;
                else
                    discarded++;
            }
          
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = newString.Substring(0, newString.Length - 1);
            }

            int byteLength = newString.Length / 2;
            byte[] bytes = new byte[byteLength];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new String(new Char[] { newString[j], newString[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }
        public static string ToString(Encoding coding, byte[] data, int index, int count)
        {
            return coding.GetString(data, index, count);
        }
        public static string ToString(byte[] bytes)
        {
            string hexString = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString += bytes[i].ToString("X2");
            }
            return hexString;
        }
    
      
        public static bool InHexFormat(string hexString)
        {
            bool hexFormat = true;

            foreach (char digit in hexString)
            {
                if (!IsHexDigit(digit))
                {
                    hexFormat = false;
                    break;
                }
            }
            return hexFormat;
        }

       
        public static bool IsHexDigit(Char c)
        {
            int numChar;
            int numA = Convert.ToInt32('A');
            int num1 = Convert.ToInt32('0');
            c = Char.ToUpper(c);
            numChar = Convert.ToInt32(c);
            if (numChar >= numA && numChar < (numA + 6))
                return true;
            if (numChar >= num1 && numChar < (num1 + 10))
                return true;
            return false;
        }
   
        private static byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return newByte;
        }

        public static byte[] MD5Crypto(byte[] data)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(data);
        }
        public static string MD5Crypto(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);

            data = MD5Crypto(data);
            return ToString(data);
        }

        public static T[] GetFieldAttributes<T>(FieldInfo field, bool inhert) where T : Attribute
        {
            T[] abs = (T[])field.GetCustomAttributes(typeof(T), inhert);
            return abs;
        }
        public static T[] GetTypeAttributes<T>(Type type,bool inhert) where T : Attribute
        {
            T[] abs = (T[])type.GetCustomAttributes(typeof(T),inhert);
            return abs;
        }
        public static T[] GetPropertyAttributes<T>(PropertyInfo pi, bool inhert) where T : Attribute
        {
            T[] abs = (T[])pi.GetCustomAttributes(typeof(T), inhert);
            return abs;
        }
        public static T[] GetMethodAttributes<T>(MethodInfo mi, bool inhert) where T : Attribute
        {
            T[] abs = (T[])mi.GetCustomAttributes(typeof(T), inhert);
            return abs;
        }
        public static T[] GetParemeterAttributes<T>(ParameterInfo pi, bool inhert) where T : Attribute
        {
            T[] abs = (T[])pi.GetCustomAttributes(typeof(T), inhert);
            return abs;
        }
        public static void Action(AsyncDelegate handler)
        {
            handler.BeginInvoke(ac =>
            {
                
                    GetEndHandler<AsyncDelegate>(ac).EndInvoke(ac);
                
            }, null);
        }
        public static void Action<T>(T t, AsyncDelegate<T> handler)
        {
            handler.BeginInvoke(t, ac => {
                GetEndHandler<AsyncDelegate<T>>(ac).EndInvoke(ac);
            }, null);
        }
        public static void Action<T, T1>(T t, T1 t1, AsyncDelegate<T,T1> handler)
        {
            handler.BeginInvoke(t,t1, ac =>
            {
                GetEndHandler<AsyncDelegate<T,T1>>(ac).EndInvoke(ac);
            }, null);
        }
      
        public static void Action<T, T1, T2>(T t, T1 t1, T2 t2, AsyncDelegate<T,T1,T2> handler)
        {
            handler.BeginInvoke(t, t1,t2, ac =>
            {
                GetEndHandler<AsyncDelegate<T, T1,T2>>(ac).EndInvoke(ac);
            }, null);
        }
        public static void Action<T, T1, T2, T3>(T t, T1 t1, T2 t2, T3 t3, AsyncDelegate<T,T1,T2,T3> handler)
        {
            handler.BeginInvoke(t, t1,t2,t3, ac =>
            {
                GetEndHandler<AsyncDelegate<T, T1,T2,T3>>(ac).EndInvoke(ac);
            }, null);
        }
        public static void Action<T, T1, T2, T3, T4>(T t, T1 t1, T2 t2, T3 t3, T4 t4, AsyncDelegate<T, T1, T2, T3,T4> handler)
        {
            handler.BeginInvoke(t, t1,t2,t3,t4, ac =>
            {
                GetEndHandler<AsyncDelegate<T, T1,T2,T3,T4>>(ac).EndInvoke(ac);
            }, null);
        }
        static T GetEndHandler<T>(IAsyncResult e)
        {
            return (T)((AsyncResult)e).AsyncDelegate;
        }

        static Dictionary<string, XmlSerializer> mXmlSerializerTable = new Dictionary<string, XmlSerializer>();
        static object mLockSerializer = new object();
        public static XmlSerializer GetXmlSerializer(Type type, XmlRootAttribute xRoot)
        {
            string key = type.FullName + (xRoot != null ? xRoot.ElementName : "");
            if (!mXmlSerializerTable.ContainsKey(key))
            {
                lock (mLockSerializer)
                {
                    if (!mXmlSerializerTable.ContainsKey(key))
                    {
                        if (xRoot != null)
                        {
                            mXmlSerializerTable.Add(key, new XmlSerializer(type, xRoot));
                        }
                        else
                        {
                            mXmlSerializerTable.Add(key, new XmlSerializer(type));
                        }
                    }
                }
            }
            return mXmlSerializerTable[key];
        }
        public void SendMail(string from, string password, string title, string body,bool ishtml, System.Collections.Generic.IEnumerable<string> tos,
            System.Collections.Generic.IEnumerable<string> asttachments)
        {
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.From = new System.Net.Mail.MailAddress(from);
            Attachment data;
            ContentDisposition disposition;  
            foreach (string to in tos)
            {
                msg.To.Add(to);
            }
            if (asttachments != null)
            {
                foreach (string path in asttachments)
                {
                    data = new Attachment(path, MediaTypeNames.Application.Octet);
                    disposition = data.ContentDisposition;
                    disposition.CreationDate = System.IO.File.GetCreationTime(path);
                    disposition.ModificationDate = System.IO.File.GetLastWriteTime(path);
                    disposition.ReadDate = System.IO.File.GetLastAccessTime(path);
                    msg.Attachments.Add(data);  

                }
            }
            msg.Subject = title;
            msg.Body = body;
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = ishtml;
            msg.Priority = System.Net.Mail.MailPriority.Normal;
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
           
                client.Credentials = new System.Net.NetworkCredential(msg.From.Address, password);
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                client.Host = "smtp." + msg.From.Host;
                client.Send(msg);
            
        }

        public static bool AppSettingValue(string name,out string value)
        {
            value= System.Configuration.ConfigurationSettings.AppSettings[name];
            return !string.IsNullOrEmpty(value);
        }
        public static string AppSettingValue(string name)
        {
            return System.Configuration.ConfigurationSettings.AppSettings[name];
        }
        public static T AppSettingValue<T>(string name)
        {
            return (T)Convert.ChangeType(AppSettingValue(name), typeof(T));
        }
        public static void Is<T>(object obj, AsyncDelegate<T> handler)
        {
            if (obj is T)
            {
                handler((T)obj);
            }
        }
        public static IEnumerable<T> Find<T>(System.Collections.IEnumerable source, EventFindHandler<T> handler)
        {
            List<T> result = new List<T>();
            if (source != null)
            {
                foreach (object item in source)
                {
                    if (handler((T)item))
                    {
                        result.Add((T)item);
                    }
                }
            }
            return result;
        }

        public static string AssemblyPath
        {
            get
            {
                return System.IO.Path.GetDirectoryName(typeof(Utils).Assembly.Location)+@"\";
            }
        }
    }


    public static class M2M
    {
        public static void MemberCopyTo(this object source, object target)
        {
            MemberCopyToImpl(source, target);
        }
        public static T MemberCopyTo<T>(this object source) where T : new()
        {
            T target = new T();
            MemberCopyToImpl(source, target);
            return target;
        }

        public static void MemberCopyToImpl(object source, object target)
        {
            ModuleCast mc = ModuleCast.GetCast(source.GetType(), target.GetType());
            mc.Cast(source, target);
        }
    }

    class ModuleCast
    {
        private List<CastProperty> mProperties = new List<CastProperty>();

        static Dictionary<Type, Dictionary<Type, ModuleCast>> mCasters = new Dictionary<Type, Dictionary<Type, ModuleCast>>(256);

        private static Dictionary<Type, ModuleCast> GetModuleCast(Type sourceType)
        {
            Dictionary<Type, ModuleCast> result;
            lock (mCasters)
            {
                if (!mCasters.TryGetValue(sourceType, out result))
                {
                    result = new Dictionary<Type, ModuleCast>(8);
                    mCasters.Add(sourceType, result);
                }
            }
            return result;
        }

        public static ModuleCast GetCast(Type sourceType, Type targetType)
        {
            Dictionary<Type, ModuleCast> casts = GetModuleCast(sourceType);
            ModuleCast result;
            lock (casts)
            {
                if (!casts.TryGetValue(targetType, out result))
                {
                    result = new ModuleCast(sourceType, targetType);
                    casts.Add(targetType, result);
                }
            }
            return result;
        }

        public ModuleCast(Type sourceType, Type targetType)
        {
            foreach (PropertyInfo sp in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (PropertyInfo tp in targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (sp.Name == tp.Name && sp.PropertyType == tp.PropertyType)
                    {
                        CastProperty cp = new CastProperty();
                        cp.SourceProperty = new PropertyHandler(sp);
                        cp.TargetProperty = new PropertyHandler(tp);
                        mProperties.Add(cp);
                    }
                }
            }
        }

        public void Cast(object source, object target)
        {
            for (int i = 0; i < mProperties.Count; i++)
            {
                CastProperty cp = mProperties[i];
                cp.TargetProperty.Set(target, cp.SourceProperty.Get(source));
            }
        }

        public class CastProperty
        {
            public PropertyHandler SourceProperty
            {
                get;
                set;
            }

            public PropertyHandler TargetProperty
            {
                get;
                set;
            }
        }
    }
}
