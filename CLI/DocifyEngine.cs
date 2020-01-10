﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Composer;
using Xarial.Docify.Core.Loader;
using Xarial.Docify.Core.Logger;
using Xarial.Docify.Core.Publisher;

namespace Xarial.Docify.CLI
{
    public interface IDocifyEngine 
    {
        T Resove<T>();
        Task Build();
    }

    public class DocifyEngine : IDocifyEngine
    {
        private IContainer m_Container;

        public DocifyEngine(string srcDir, string outDir) 
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<LocalFileSystemLoaderConfig>()
                .WithParameter(new TypedParameter(typeof(string), srcDir));

            builder.RegisterType<BaseCompilerConfig>()
                .WithParameter(new TypedParameter(typeof(string), ""));

            builder.RegisterType<LocalFileSystemPublisherConfig>()
                .WithParameter(new TypedParameter(typeof(string), outDir));
            
            builder.RegisterType<LocalFileSystemPublisher>()
                .As<IPublisher>();

            builder.RegisterType<LocalFileSystemLoader>()
                .As<ILoader>();

            builder.RegisterType<LayoutParser>()
                .As<ILayoutParser>();

            builder.RegisterType<BaseSiteComposer>().As<IComposer>();

            builder.RegisterType<ConsoleLogger>().As<ILogger>();

            builder.RegisterType<IncludesHandler>().As<IIncludesHandler>();

            builder.RegisterType<MarkdigRazorLightTransformer>()
                .As<IContentTransformer>()
                .UsingConstructor(typeof(Func<IContentTransformer, IIncludesHandler>));

            builder.RegisterType<BaseCompiler>().As<ICompiler>();

            m_Container = builder.Build();
        }

        public async Task Build()
        {
            var loader = Resove<ILoader>();
            var composer = Resove<IComposer>();
            var compiler = Resove<ICompiler>();
            var publisher = Resove<IPublisher>();

            var elems = await loader.Load();

            var site = composer.ComposeSite(elems, "");

            await compiler.Compile(site);

            var writables = Enumerable.Empty<IWritable>();
            writables = writables.Union(site.MainPage.GetAllPages());
            writables = writables.Union(site.Assets);

            await publisher.Write(writables);
        }

        public T Resove<T>() 
        {
            return m_Container.Resolve<T>();
        }
    }
}
