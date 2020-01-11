﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Fragments.Tests
{
    public class PageLangTest
    {
        const string INCLUDE_PATH = @"page-lang\_includes\page-lang.cshtml";

        [Test]
        public async Task Default_Lang()
        {
            var site = FragmentTest.NewSite();

            var res = await FragmentTest.RenderIncludeNormalize(INCLUDE_PATH, null, site, site.MainPage);

            Assert.AreEqual("en", res);
        }
        
        [Test]
        public async Task Page_Lang()
        {
            var site = FragmentTest.NewSite(FragmentTest.GetData<Metadata>("lang: ru"));

            var res = await FragmentTest.RenderIncludeNormalize(INCLUDE_PATH, null, site, site.MainPage);

            Assert.AreEqual("ru", res);
        }

        [Test]
        public async Task Page_SiteDefLang()
        {
            var site = FragmentTest.NewSite(null, FragmentTest.GetData<Configuration>("page-lang:\r\n  default_lang: fr"));

            var res = await FragmentTest.RenderIncludeNormalize(INCLUDE_PATH, null, site, site.MainPage);

            Assert.AreEqual("fr", res);
        }
    }
}
