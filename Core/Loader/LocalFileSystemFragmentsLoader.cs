﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Loader
{
    public class LocalFileSystemFragmentsLoader : IFragmentsLoader
    {
        private readonly ILoader m_Loader;
        private readonly Configuration m_Config;

        public LocalFileSystemFragmentsLoader(ILoader loader, Configuration conf)
        {
            m_Loader = loader;
            m_Config = conf;
        }

        public async Task<IEnumerable<ISourceFile>> Load(IEnumerable<ISourceFile> srcFiles)
        {
            if (srcFiles == null)
            {
                srcFiles = Enumerable.Empty<ISourceFile>();
            }

            var resFiles = srcFiles.ToDictionary(f => f.Location.ToId(), f => f, StringComparer.CurrentCultureIgnoreCase);

            if (m_Config.Fragments?.Any() == true)
            {
                foreach (var fragment in m_Config.Fragments)
                {
                    await AddFiles(resFiles, m_Config.FragmentsFolder.Combine(fragment), fragment);
                }
            }

            if (!string.IsNullOrEmpty(m_Config.Theme)) 
            {
                await AddFiles(resFiles, m_Config.ThemesFolder.Combine(m_Config.Theme), m_Config.Theme);
            }

            return resFiles.Values;
        }

        private async Task AddFiles(Dictionary<string, ISourceFile> srcFiles, Location loc, string fragName) 
        {
            var newSrcFiles = await m_Loader.Load(loc);

            if (newSrcFiles != null) 
            {
                foreach (var newSrcFile in newSrcFiles) 
                {
                    var id = newSrcFile.Location.ToId();

                    if (!srcFiles.ContainsKey(id))
                    {
                        srcFiles.Add(id, newSrcFile);
                    }
                    else 
                    {
                        throw new DuplicateFragmentSourceFileException(fragName, id);
                    }
                }
            }
        }
    }
}
