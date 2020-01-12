﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    internal static class MetadataExtension
    {
        private const string INHERIT_LIST_SYMBOL = "$";

        internal static T Merge<T>(this T thisParams,
            Dictionary<string, dynamic> baseParams)
            where T : Metadata, new()
        {
            var resParams = new T();

            if (baseParams != null)
            {
                foreach (var baseParam in baseParams)
                {
                    resParams.Add(baseParam.Key, baseParam.Value);
                }
            }

            foreach (var thisParam in thisParams ?? new T())
            {
                var val = thisParam.Value;

                var isDef = val != null && (!(val is string) || !string.IsNullOrEmpty(val));

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

        private static dynamic UpdateValue(Dictionary<string, dynamic> baseParams, string thisParamName, dynamic val)
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

                    (val as List<object>).InsertRange(0, baseParams[thisParamName]);
                }
            }

            return val;
        }

        internal static T GetParameterOrDefault<T>(this Metadata data, string name) 
        {
            T val;
            TryGetParameter(data, name, out val);
            return val;
        }

        internal static T GetRemoveParameterOrDefault<T>(this Metadata data, string name) 
        {
            T val;
            
            if (TryGetParameter(data, name, out val)) 
            {
                data.Remove(name);
            }

            return val;
        }

        private static bool TryGetParameter<T>(Metadata data, string name, out T val) 
        {
            dynamic dynVal;

            if (data.TryGetValue(name, out dynVal))
            {
                if (dynVal is T)
                {
                    val = (T)dynVal;
                }
                else
                {
                    val = Convert.ChangeType(dynVal, typeof(T));
                }

                return true;
            }
            else
            {
                val = default(T);
                return false;
            }
        }
    }
}
