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

namespace Xarial.Docify.Lib.Plugins
{
    public class CodeSelectorOptions
    {
        public bool LeftAlign { get; set; }
        public string[] Regions { get; set; }
        public string[] ExcludeRegions { get; set; }
    }

    [Flags]
    public enum SnippetInfo_e 
    {
        Jagged = 0,
        TopBoundary = 1,
        BottomBoundary = 2,
        Full = TopBoundary | BottomBoundary
    }

    public class Snippet 
    {
        public SnippetInfo_e Info { get; }
        public string Code { get; }

        public Snippet(string code, SnippetInfo_e info) 
        {
            Info = info;
            Code = code;
        }
    }

    public static class CodeSnippetHelper
    {
        private class ProcessingSnippet
        {
            public string[] Lines { get; set; }
            public SnippetInfo_e Info { get; set; }
        }

        private const string REGION_BLOCK = "---";
        
        private static string GetCommentLineSymbol(string codeLang) 
        {
            switch (codeLang.ToLower()) 
            {
                case "cs":
                case "csharp":
                case "c#":
                case "js":
                case "javascript":
                case "java":
                    return "//";

                case "vb":
                case "vbnet":
                case "vb.net":
                case "vba":
                    return "'";
                    
                default:
                    return "";
            }
        }
        
        public static Snippet[] Select(string rawCode, string codeLang, CodeSelectorOptions opts)
        {
            var srcLines = Regex.Split(rawCode, "\r\n|\r|\n");

            var result = new List<ProcessingSnippet>();
            result.Add(new ProcessingSnippet()
            {
                Lines = srcLines,
                Info = SnippetInfo_e.Full
            });

            var commentSymbol = GetCommentLineSymbol(codeLang);

            ProcessRegions(ref result, opts.Regions, true, commentSymbol);
            ProcessRegions(ref result, opts.ExcludeRegions, false, commentSymbol);
            HideRegions(result, commentSymbol);

            if (opts.LeftAlign)
            {
                AlignLeft(result);
            }

            return result.Select(snip => new Snippet(string.Join(Environment.NewLine, snip.Lines), snip.Info)).ToArray();
        }

        private static void AlignLeft(List<ProcessingSnippet> result)
        {
            for (int i = 0; i < result.Count; i++)
            {
                var lines = result[i].Lines;
                var indent = lines.Min(l => string.IsNullOrEmpty(l) ? int.MaxValue : l.TakeWhile(Char.IsWhiteSpace).Count());

                if (indent > 0)
                {
                    lines = lines.Select(l => string.IsNullOrEmpty(l) ? l : l.Substring(indent)).ToArray();
                    result[i].Lines = lines;
                }
            }
        }

        private static void ProcessRegions(ref List<ProcessingSnippet> result, string[] regs, 
            bool inner, string commentSymbol)
        {
            if (regs?.Any() == true)
            {
                var newResult = new List<ProcessingSnippet>();

                for (int i = 0; i < result.Count; i++)
                {
                    var lineGroups = SelectRegionLines(result[i], commentSymbol, regs, inner);
                    newResult.AddRange(lineGroups);
                }

                result = newResult;
            }
        }

        private static void HideRegions(List<ProcessingSnippet> result, string commentSymbol)
        {
            for (int i = 0; i < result.Count; i++)
            {
                var lines = result[i].Lines;
                lines = lines.Where(l =>
                {
                    string regName;
                    return !(IsRegionEnd(l, commentSymbol) || IsRegionStart(l, commentSymbol, out regName));
                }).ToArray();

                if (result[i].Lines.Length != lines.Length)
                {
                    result[i].Lines = lines;
                }
            }
        }
        
        private static IEnumerable<ProcessingSnippet> SelectRegionLines(
            ProcessingSnippet snippet, string commentSymbol, 
            string[] regions, bool inner)
        {
            var result = new List<ProcessingSnippet>();

            var curGroupType = SnippetInfo_e.Jagged;
            var curGroup = new List<string>();
            
            var foundRegions = new List<string>();

            bool isRecordingRegion = false;
            int relRegLevel = 0;

            bool? isFirstCodeLine = null;

            bool hasLinesAfterLastGroup = false;

            void FlushCurrentGroup() 
            {
                if (curGroup.Any())
                {
                    result.Add(new ProcessingSnippet()
                    {
                        Lines = curGroup.ToArray(),
                        Info = snippet.Info == SnippetInfo_e.Full ? curGroupType : snippet.Info
                    });
                    curGroup.Clear();
                    curGroupType = SnippetInfo_e.Jagged;
                    hasLinesAfterLastGroup = false;
                }
            }

            void UpdateCurGroup(string appendLine) 
            {
                if (!curGroup.Any()) 
                {
                    if (isFirstCodeLine.Value)
                    {
                        curGroupType |= SnippetInfo_e.TopBoundary;
                    }
                }

                curGroup.Add(appendLine);
            }

            for (int i = 0; i < snippet.Lines.Length; i++)
            {
                string regName = "";
                var isRegStart = IsRegionStart(snippet.Lines[i], commentSymbol, out regName);
                var isRegEnd = IsRegionEnd(snippet.Lines[i], commentSymbol);

                var isStart = isRegStart
                    && regions.Contains(regName, StringComparer.CurrentCultureIgnoreCase)
                    && !isRecordingRegion;

                var isEnd = isRegEnd && relRegLevel == 1;

                if (!isFirstCodeLine.HasValue)
                {
                    if (!isRegStart)
                    {
                        isFirstCodeLine = true;
                    }
                }
                else 
                {
                    isFirstCodeLine = false;
                }

                if (!(isRegEnd || isRegEnd)) 
                {
                    hasLinesAfterLastGroup = true;
                }

                if (isStart)
                {
                    if (!inner)
                    {
                        FlushCurrentGroup();
                    }

                    if (!foundRegions.Contains(regName, StringComparer.CurrentCultureIgnoreCase))
                    {
                        foundRegions.Add(regName);
                    }

                    isRecordingRegion = true;
                    relRegLevel = 1;
                }
                else if (isRecordingRegion)
                {
                    if (!isEnd)
                    {   
                        if (inner)
                        {
                            UpdateCurGroup(snippet.Lines[i]);
                        }
                    }

                    if (isRegEnd)
                    {
                        relRegLevel--;
                    }

                    if (isEnd)
                    {
                        isRecordingRegion = false;
                        FlushCurrentGroup();
                    }

                    if (isRegStart)
                    {
                        relRegLevel++;
                    }
                }
                else if(!inner)
                {
                    UpdateCurGroup(snippet.Lines[i]);
                }
            }

            if (!inner) 
            {
                FlushCurrentGroup();
            }

            var missingRegs = regions.Except(foundRegions, StringComparer.CurrentCultureIgnoreCase);

            if (missingRegs.Any())
            {
                throw new Exception($"Missing regions: {string.Join(',', missingRegs)}");
            }

            if (snippet.Info == SnippetInfo_e.Full)
            {
                if (!hasLinesAfterLastGroup)
                {
                    result.Last().Info |= SnippetInfo_e.BottomBoundary;
                }
            }

            return result;
        }

        private static bool IsRegionStart(string line, string commentSymbol, out string name)
        {
            line = line.Trim();
            var regBlock = commentSymbol + REGION_BLOCK;

            if (line.StartsWith(regBlock)) 
            {
                name = line.Substring(regBlock.Length).Trim();
                return !string.IsNullOrEmpty(name);
            }
            else
            {
                name = "";
                return false;
            }
        }

        private static bool IsRegionEnd(string line, string commentSymbol)
        {
            line = line.Trim();
            var regBlock = commentSymbol + REGION_BLOCK;
            return line == regBlock;
        }
    }
}
