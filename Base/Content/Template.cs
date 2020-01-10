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

namespace Xarial.Docify.Base.Content
{
    public class Template : Frame
    {
        public string Name { get; }
        public override string Key => Name;

        public Template(string name, string rawContent,
            Metadata data = null, Template baseTemplate = null)
            : base(rawContent, data, baseTemplate)
        {
            Name = name;
        }
    }
}
