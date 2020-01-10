﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Data;

namespace Xarial.Docify.Core.Loader
{
    public class LocalFileSystemLoaderConfig
    {
        private const string IGNORE_FILE_PARAM_NAME = "ignore";

        public string Path { get; }
        public List<string> Ignore { get; }

        public LocalFileSystemLoaderConfig(string path, IEnumerable<string> ignore)
        {
            Path = path;
            Ignore = ignore?.ToList() ?? new List<string>();
        }

        public LocalFileSystemLoaderConfig(string path, Configuration conf) 
            : this(path, GetIgnoreFiles(conf))
        {
        }

        private static List<string> GetIgnoreFiles(Configuration conf)
        {
            try
            {
                var vals = conf.GetParameterOrDefault<IEnumerable<string>>(IGNORE_FILE_PARAM_NAME);

                if (vals != null)
                {
                    return vals.ToList();
                }
                else 
                {
                    return null;
                }
            }
            catch 
            {
                throw new InvalidCastException($"Value specified in {IGNORE_FILE_PARAM_NAME} must be an array");
            }
        }
    }
}
