﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xarial.Docify.Base.Data
{
    public static class MetadataExtension
    {
        private const string INHERIT_LIST_SYMBOL = "$";

        private static readonly JsonSerializer m_JsonSerializer = new JsonSerializer()
        {
            ContractResolver = new YamlNameResolver()
        };

        private class YamlNameResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                var newPrpName = string.Join("", propertyName.Split('-')
                    .Select(w => char.ToUpper(w[0]) + w.Substring(1)));

                return newPrpName;
            }
        }

        public static T ToObject<T>(IDictionary<string, object> data)
        {
            return (T)ToObject(data, typeof(T));
        }

        public static T ToObject<T>(this IMetadata data)
        {
            return (T)ToObject(data, typeof(T));
        }

        public static object ToObject(IDictionary<string, object> data, Type type)
        {
            //TODO: consider removing the double-serialization via JSON and deserialize directly from dictionary
            var obj = JObject.FromObject(data, m_JsonSerializer);
            var res = obj.ToObject(type);
            return res;
        }

        public static T Merge<T>(this T thisParams,
            IDictionary<string, object> baseParams)
            where T : IMetadata
        {
            if (baseParams == null) 
            {
                baseParams = new Dictionary<string, object>();
            }

            var resParams = (T)thisParams.Copy(baseParams);

            foreach (var thisParam in thisParams)
            {
                var val = thisParam.Value;

                var isDef = val != null && (!(val is string) || !string.IsNullOrEmpty((string)val));

                if (isDef)
                {
                    if (resParams.ContainsKey(thisParam.Key))
                    {
                        val = UpdateValue(baseParams, thisParam.Key, val);
                        resParams[thisParam.Key] = val;
                    }
                    else
                    {
                        resParams.Add(thisParam.Key, val);
                    }
                }
            }

            return resParams;
        }

        private static object UpdateValue(IDictionary<string, object> baseParams, string thisParamName, dynamic val)
        {
            if (val is List<object>)
            {
                var inheritPlc = (val as List<object>).IndexOf(INHERIT_LIST_SYMBOL);
                if (inheritPlc != -1)
                {
                    (val as List<object>).RemoveAt(inheritPlc);

                    if (!(baseParams[thisParamName] is List<object>))
                    {
                        throw new InvalidCastException($"Cannot inherit list parameters from the base metadata");
                    }

                    (val as List<object>).InsertRange(0, (List<object>)baseParams[thisParamName]);
                }
            }

            return val;
        }

        public static T GetParameterOrDefault<T>(this IMetadata data, string name)
        {
            T val;
            TryGetParameter(data, name, out val);
            return val;
        }

        public static T GetRemoveParameterOrDefault<T>(this IMetadata data, string name)
        {
            T val;

            if (TryGetParameter(data, name, out val))
            {
                data.Remove(name);
            }

            return val;
        }

        public static bool TryGetParameter<T>(IDictionary<string, object> data, string name, out T val)
        {
            object dynVal;

            if (data.TryGetValue(name, out dynVal))
            {
                if (object.Equals(dynVal, default(T)))
                {
                    val = default(T);
                }
                else if (dynVal is T)
                {
                    val = (T)dynVal;
                }
                else if (dynVal is IEnumerable
                    && IsAssignableToGenericType(typeof(T), typeof(IEnumerable<>), out Type enumerType))
                {
                    var itemType = enumerType.GetGenericArguments().First();
                    
                    var castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast),
                        BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(new Type[] { itemType });
                    
                    val = (T)castMethod.Invoke(null, new object[] { dynVal });
                }
                else if (dynVal is IConvertible)
                {
                    val = (T)Convert.ChangeType(dynVal, typeof(T));
                }
                else
                {
                    throw new InvalidCastException("Failed to convert parameter");
                }

                return true;
            }
            else
            {
                val = default(T);
                return false;
            }
        }
        
        //duplicates the function in Core, might need to put this in xToolkit and use from there
        private static bool IsAssignableToGenericType(Type givenType, Type genericType, out Type specGenericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    specGenericType = it;
                    return true;
                }
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                specGenericType = givenType;
                return true;
            }

            var baseType = givenType.BaseType;

            if (baseType == null)
            {
                specGenericType = null;
                return false;
            }

            return IsAssignableToGenericType(baseType, genericType, out specGenericType);
        }
    }
}
