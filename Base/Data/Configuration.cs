﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;

namespace Xarial.Docify.Base.Data
{
    public interface IConfiguration 
    {
        Environment_e Environment { get; set; }
    }

    public class Configuration : Metadata, IConfiguration
    {
        public Environment_e Environment { get; set; }
        public string WorkingFolder { get; set; }
        public Location FragmentsFolder { get; set; }
        public List<string> Fragments { get; set; }
        public Location ThemesFolder { get; set; }
        public string Theme { get; set; }
        
        public Configuration() : this(new Dictionary<string, dynamic>())
        {
        }

        public Configuration(IDictionary<string, dynamic> parameters) : base(parameters) 
        {
            Environment = Environment_e.Test;
        }
    }
}
