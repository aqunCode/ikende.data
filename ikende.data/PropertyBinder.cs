
﻿using System;
using System.Collections.Generic;
using System.Text;
namespace IKende.Data
{

    

    class PropertyBinder
    {
        public PropertyBinder(System.Reflection.PropertyInfo pi)
        {
            Handler = new PropertyHandler(pi);
            BindAttribute[] bas = Utils.GetPropertyAttributes<BindAttribute>(pi, false);
            if (bas.Length > 0)
                Binder = bas[0];
            ViewStateAttribute[] vsa = Utils.GetPropertyAttributes<ViewStateAttribute>(pi, false);
            if (vsa.Length > 0)
                ViewState = vsa[0];

        }

        public ViewStateAttribute ViewState
        {
            get;
            set;
        }

        public PropertyHandler Handler
        {
            get;
            set;
        }

        public BindAttribute Binder
        {
            get;
            set;
        }
        public bool GetValueTypeValue(System.Collections.Specialized.NameValueCollection data, string Prefix, out object value)
        {
            IConvert convert = null;
            value = null;
            Type createtype = Handler.Property.PropertyType;
            if (Binder != null && Binder.Convert == null && Binder.Fungible != null)
                createtype = Binder.Fungible;
            bool succed = false;
            if (Binder != null && !string.IsNullOrEmpty(Binder.Prefix))
                Prefix = Binder.Prefix;
            if (Binder != null)
                convert = Binder.GetConvert();
            if (convert == null)
            {
                if (Utils.Converts.ContainsKey(createtype))
                    convert = Utils.Converts[createtype];
            }
            if (convert != null)
            {
                value = convert.Parse(data, Handler.Property.Name, Prefix, out succed);
            }
            else
            {
                if (createtype.IsEnum)
                {
                    Type tomenutype = typeof(ToEnum<>).MakeGenericType(createtype);
                    convert = (IConvert)Activator.CreateInstance(tomenutype);
                    Utils.AddCustomConvert(createtype, convert);
                    object pvalue = convert.Parse(data, Handler.Property.Name, Prefix, out succed);
                    if (succed)
                        value = Convert.ChangeType(pvalue, createtype);
                }

            }
            return succed;
        }

        public bool GetClassValue(System.Collections.Specialized.NameValueCollection data, string Prefix, out object value)
        {
            IConvert convert = null;
            value = null;
            Type createtype = Handler.Property.PropertyType;
            if (Binder != null && Binder.Convert == null && Binder.Fungible != null)
                createtype = Binder.Fungible;
            bool succed = false;
            if (Binder != null && !string.IsNullOrEmpty(Binder.Prefix))
                Prefix = Binder.Prefix;
            if (Binder != null)
                convert = Binder.GetConvert();
            if (convert == null)
            {
                if (Utils.Converts.ContainsKey(createtype))
                    convert = Utils.Converts[createtype];
            }
            if (convert != null)
            {
                value = convert.Parse(data, Handler.Property.Name, Prefix, out succed);
            }
            else
            {
                if (createtype.IsClass && !createtype.IsInterface &&
                     !createtype.IsAbstract)
                {
                    if (createtype.IsArray)
                    {
                        succed = true;
                        value = Activator.CreateInstance(createtype, 0);
                    }
                    else
                    {
                        ClassBinder cb = Utils.GetBinder(createtype);
                        succed = true;
                        value = cb.CreateObject(data, Prefix);
                    }
                }
            }

            return succed;
        }

        public bool GetValue(System.Collections.Specialized.NameValueCollection data, string Prefix, out object value)
        {
            Type createtype = Handler.Property.PropertyType;
            if (createtype.IsValueType || createtype.IsEnum || createtype == typeof(string))
            {
                return GetValueTypeValue(data, Prefix, out value);
            }
            else
            {
                return GetClassValue(data, Prefix, out value);
            }

        }

        public void FullValue(object source, System.Collections.Specialized.NameValueCollection data, string Prefix, bool ispostback)
        {

            BinderHelper.Full(source, data, Prefix, ispostback);
        }
    }
}
