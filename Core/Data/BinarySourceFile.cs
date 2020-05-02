﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class BinarySourceFile : IBinarySourceFile
    {
        public byte[] Content { get; }

        public Location Location { get; }

        public BinarySourceFile(Location path, byte[] content)
        {
            Location = path;
            Content = content;
        }

        public override string ToString()
        {
            return Location.ToString();
        }
    }
}
