﻿
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WYF.Common;


namespace WYF.DataEntity
{
    /// <summary>
    /// 类实例反射构造工具，可提升构建效率
    /// </summary>
    public class TypesContainer
    {
        private static ConcurrentDictionary<string, Type> typesDict = new ConcurrentDictionary<string, Type>();
        private static ConcurrentDictionary<Type, object> instancesDict = new ConcurrentDictionary<Type, object>();
        private static readonly object _lockObject = new object();

    
        static Func<Type,object> typeCreator=(Type) => Activator.CreateInstance(Type);

        






        /// <summary>
        /// 获取或者注册（加载）类
        /// </summary>
        /// <param name="typeName">类全限定名</param>
        /// <returns></returns>
        //public static Type GetOrRegister(string typeName)
        //{
        //    if (string.IsNullOrEmpty(typeName))
        //    {
        //        return null;
        //    }

        //    return typesDict.GetOrAdd(typeName, tp => {
        //        Type type= Type.GetType(tp);
        //        if (type == null)
        //        {
        //            // 加载 DLL
        //            string[] parts = tp.Split('.');
        //            string desiredPart = parts[0] + "." + parts[1]+".dll"; // "WYF.Form"

        //            Assembly assembly = Assembly.LoadFrom(desiredPart);
        //            type=assembly.GetType(tp);
        //        }
        //        return type;
        //    });

        //}



        public static Type GetOrRegister(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentException("typeName cannot be null or empty.", nameof(typeName));
            }

            return typesDict.GetOrAdd(typeName.Trim(), clsName =>
            {
                try
                {
                    Type type = Type.GetType(clsName,true,true);


                    return type;
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"{Instance.GetClusterName()}: {clsName} not found.", e);
                }
            });
        }

        public static T GetOrRegisterSingletonInstance<T>(string typeName)
        {
            Type t = GetOrRegister(typeName);
            if (t!=null)
            {
                object ret = instancesDict.GetValueOrDefault(t);
                if (ret==null)
                {
                    lock (_lockObject)
                    {
                        ret = instancesDict.GetValueOrDefault(t);
                        if (ret == null)
                        {
                            ret = CreateInstance<T>(t);
                            instancesDict.TryAdd(t, ret);
                        }
                    }
                }
                return (T)ret;
            }
            return default(T);
        }

        public static T CreateInstance<T>(Type type)
        {
            if (type==null)
            {
                throw new Exception("failed to  Create Instance,type is Null.");
            }

            try
            {
                //return (type)=> Activator.CreateInstance<object>(type);
                return (T)Activator.CreateInstance(type);
                //return typeCreator(type);
            }
            catch (Exception ex)
            {
                throw new Exception($"{type.Name} failed to  Create Instance:{ex.Message}");
            }

        }


        public static  T CreateInstance<T>(string className)
        {
            Type type = GetOrRegister(className);
            if (type == null)
            {
                throw new Exception($"{className} failed to  Create Instance");
            }
            return CreateInstance<T>(type);
        }

    }

 
}
