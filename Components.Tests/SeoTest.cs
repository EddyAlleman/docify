﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Components.Tests.Properties;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.CLI;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Data;

namespace Components.Tests
{
    public class SeoTest
    {
        private const string INCLUDE_PATH = @"seo\_includes\seo.cshtml";

        [Test]
        public async Task DefaultParamsTest()
        {
            var site = ComponentsTest.NewSite("<head>\r\n{% seo %}\r\n</head>", INCLUDE_PATH,
                ComponentsTest.GetData<Metadata>("title: p1\r\ndescription: d1"),
                ComponentsTest.GetData<Configuration>("title: t1\r\ndescription: sd1"));

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual($"<head>\r\n{Resources.seo1}\r\n</head>", res);
        }

        [Test]
        public async Task OgParamsTest()
        {
            var site = ComponentsTest.NewSite("<head>\r\n{% seo { og: true, twitter: false, linkedin: false} %}\r\n</head>", INCLUDE_PATH,
                ComponentsTest.GetData<Metadata>("title: p1\r\ndescription: d1"),
                ComponentsTest.GetData<Configuration>("title: t1\r\ndescription: sd1"));

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual($"<head>\r\n{Resources.seo2}\r\n</head>", res);
        }

        [Test]
        public async Task TwitterParamsTest()
        {
            var site = ComponentsTest.NewSite("<head>\r\n{% seo { og: false, twitter: true, linkedin: false} %}\r\n</head>", INCLUDE_PATH,
                ComponentsTest.GetData<Metadata>("title: p1\r\ndescription: d1"),
                ComponentsTest.GetData<Configuration>("title: t1\r\ndescription: sd1"));

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual($"<head>\r\n{Resources.seo3}\r\n</head>", res);
        }

        [Test]
        public async Task LiParamsTest()
        {
            var site = ComponentsTest.NewSite("<head>\r\n{% seo { og: false, twitter: false, linkedin: true} %}\r\n</head>", INCLUDE_PATH,
                ComponentsTest.GetData<Metadata>("title: p1\r\ndescription: d1"),
                ComponentsTest.GetData<Configuration>("title: t1\r\ndescription: sd1"));

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual($"<head>\r\n{Resources.seo4}\r\n</head>", res);
        }

        [Test]
        public async Task ImageAndImagePngTest()
        {
            var site = ComponentsTest.NewSite("{% seo %}", INCLUDE_PATH,
                ComponentsTest.GetData<Metadata>("title: p1\r\ndescription: d1\r\nimage: img1.svg\r\nimage-png: img1.png"));
            site.MainPage.SubPages.Add(new Page("Page1", "{% seo %}", ComponentsTest.GetData<Metadata>("title: p1\r\nimage: img2.png")));

            var compiler = new DocifyEngine("", "", "", Environment_e.Test).Resove<ICompiler>();
            var files = await compiler.Compile(site).ToListAsync();

            var r1 = files.First(f => f.Location.ToId() == "index.html");
            var r2 = files.First(f => f.Location.ToId() == "Page1::index.html");
            
            Assert.AreEqual(Resources.seo5, r1.AsTextContent());
            Assert.AreEqual(Resources.seo6, r2.AsTextContent());
        }
    }
}
