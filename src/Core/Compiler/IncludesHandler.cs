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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Compiler.Context;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Helpers;
using Xarial.Docify.Core.Plugin;
using Xarial.Docify.Core.Plugin.Extensions;
using YamlDotNet.Serialization;

namespace Xarial.Docify.Core.Compiler
{
    public class IncludesHandler : IIncludesHandler
    {
        public const string START_TAG = "{%";
        public const string END_TAG = "%}";
        
        private const string NAME_PARAMS_SPLIT_SYMBOL = " ";

        private readonly IDynamicContentTransformer m_Transformer;

        private readonly PlaceholdersParser m_PlcParser;

        private readonly IIncludesHandlerExtension m_Ext;

        public IncludesHandler(IDynamicContentTransformer transformer, IIncludesHandlerExtension ext) 
        {
            m_Transformer = transformer;
            m_PlcParser = new PlaceholdersParser(START_TAG, END_TAG);

            m_Ext = ext;
        }
        
        private async Task<string> Render(string name, IMetadata param, 
            ISite site, IPage page, string url)
        {
            var include = site.Includes.FirstOrDefault(i => string.Equals(i.Name,
                name, StringComparison.CurrentCultureIgnoreCase));
            
            if (include != null)
            {
                //TODO: check if any plugins register for this include to find the conflict

                var data = ComposeDataParameters(name, param, site, page);
                data = data.Merge(include.Data);

                return await m_Transformer.Transform(include.RawContent, include.Id,
                    new ContextModel(site, page, data, url));
            }
            else 
            {
                var data = ComposeDataParameters(name, param, site, page);
                return await m_Ext.ResolveInclude(name, data, page);
            }
        }

        private static IMetadata ComposeDataParameters(string name, IMetadata param, ISite site, IPage page)
        {
            Dictionary<string, object> GetData(IMetadata data, string name)
            {
                var extrData = data.GetParameterOrDefault<Dictionary<string, object>>("$" + name);

                if (extrData != null)
                {
                    return extrData.ToDictionary(k => k.Key.ToString(), k => k.Value);
                }
                else
                {
                    return null;
                }
            }

            //page can be null if the include used on asset file, check this as now page of asset is added so it should never be null
            if (page != null)
            {
                param = param.Merge(GetData(page.Data, name));
            }

            param = param.Merge(GetData(site.Configuration, name));
            
            return param;
        }

        private Task ParseParameters(string includeRawContent, out string name, out IMetadata param) 
        {
            includeRawContent = includeRawContent.Trim();

            if (includeRawContent.Contains(NAME_PARAMS_SPLIT_SYMBOL))
            {
                name = includeRawContent.Substring(0, includeRawContent.IndexOf(NAME_PARAMS_SPLIT_SYMBOL));
                var paramStr = includeRawContent.Substring(includeRawContent.IndexOf(NAME_PARAMS_SPLIT_SYMBOL) + 1);

                var yamlDeserializer = new MetadataSerializer();

                param = yamlDeserializer.Deserialize<Metadata>(paramStr);
            }
            else 
            {
                name = includeRawContent;
                param = new Metadata();
            }

            return Task.CompletedTask;
        }

        public async Task<string> ResolveAll(string rawContent, ISite site, IPage page, string url)
        {
            var replacement = await m_PlcParser.ReplaceAsync(rawContent, async (string includeRawContent) => 
            {
                var name = "";
                try
                {
                    IMetadata data;
                    await ParseParameters(includeRawContent, out name, out data);
                    var replace = await Render(name, data, site, page, url);
                    return await ResolveAll(replace, site, page, url);
                }
                catch(Exception ex)
                {
                    throw new IncludeResolveException(name, url, ex);
                }
            });

            return replacement;
        }
    }
}
