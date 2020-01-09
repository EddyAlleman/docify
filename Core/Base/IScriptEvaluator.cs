﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.Docify.Core.Base
{
    public interface IScriptEvaluator
    {
        Task<string> Evaluate(ICompilable compilable, ContextModel model);
    }
}
