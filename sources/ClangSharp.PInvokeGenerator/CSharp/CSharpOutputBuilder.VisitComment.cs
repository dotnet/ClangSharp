// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Security;
using ClangSharp.Interop;

namespace ClangSharp.CSharp;

internal partial class CSharpOutputBuilder
{
    public void WriteDocComment(in CXComment fullComment)
    {
        if (fullComment.Kind == CXCommentKind.CXComment_Null)
        {
            return;
        }

        var summaryParts = new List<string>();
        var remarksParts = new List<string>();
        string? returnText = null;
        var paramParts = new List<(string Name, string Text)>();

        for (uint i = 0; i < fullComment.NumChildren; i++)
        {
            var child = fullComment.GetChild(i);
            switch (child.Kind)
            {
                case CXCommentKind.CXComment_Paragraph:
                    var text = GetParagraphText(child).Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        summaryParts.Add(text);
                    }

                    break;

                case CXCommentKind.CXComment_ParamCommand:
                    var paramName = child.ParamCommandComment_ParamName.ToString();
                    var paramText = GetParagraphText(child.BlockCommandComment_Paragraph).Trim();
                    paramParts.Add((paramName, paramText));
                    break;

                case CXCommentKind.CXComment_BlockCommand:
                    var cmd = child.BlockCommandComment_CommandName.ToString();
                    var body = GetParagraphText(child.BlockCommandComment_Paragraph).Trim();
                    if (cmd is "brief" or "summary")
                    {
                        summaryParts.Add(body);
                    }
                    else if (cmd is "return" or "returns")
                    {
                        returnText = body;
                    }
                    else
                    {
                        remarksParts.Add($"{cmd}: {body}");
                    }

                    break;
            }
        }

        if (summaryParts.Count == 1)
        {
            WriteIndented("/// <summary>");
            Write(summaryParts[0]);
            WriteLine("</summary>");
        }
        else if (summaryParts.Count > 1)
        {
            WriteIndentedLine("/// <summary>");
            foreach (var part in summaryParts)
            {
                WriteIndented("/// <para>");
                Write(part);
                WriteLine("</para>");
            }

            WriteIndentedLine("/// </summary>");
        }

        foreach (var (name, paramText) in paramParts)
        {
            WriteIndented("/// <param name=");
            Write('"');
            Write(name);
            Write('"');
            Write('>');
            Write(paramText);
            WriteLine("</param>");
        }

        if (returnText is not null)
        {
            WriteIndented("/// <returns>");
            Write(returnText);
            WriteLine("</returns>");
        }

        if (remarksParts.Count == 1)
        {
            WriteIndented("/// <remarks>");
            Write(remarksParts[0]);
            WriteLine("</remarks>");
        }
        else if (remarksParts.Count > 1)
        {
            WriteIndentedLine("/// <remarks>");
            foreach (var part in remarksParts)
            {
                WriteIndented("/// <para>");
                Write(part);
                WriteLine("</para>");
            }

            WriteIndentedLine("/// </remarks>");
        }
    }

    private static string GetParagraphText(CXComment para)
    {
        if (para.Kind is not CXCommentKind.CXComment_Paragraph)
        {
            throw new InvalidOperationException("Expected a paragraph comment");
        }

        var sb = new System.Text.StringBuilder();
        for (uint i = 0; i < para.NumChildren; i++)
        {
            var child = para.GetChild(i);
            if (child.Kind == CXCommentKind.CXComment_Text)
            {
                _ = sb.Append(child.TextComment_Text.ToString());
            }
        }

        return SecurityElement.Escape(sb.ToString());
    }
}
