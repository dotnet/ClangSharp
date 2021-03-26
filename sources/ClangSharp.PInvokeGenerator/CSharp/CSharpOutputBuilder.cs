// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace ClangSharp.CSharp
{
    internal sealed partial class CSharpOutputBuilder
    {
        public const string DefaultIndentationString = "    ";

        private readonly string _name;
        private readonly List<string> _contents;
        private readonly StringBuilder _currentLine;
        private readonly SortedSet<string> _usingDirectives;
        private readonly SortedSet<string> _staticUsingDirectives;
        private readonly string _indentationString;
        private readonly bool _isTestOutput;

        private int _indentationLevel;
        private MarkerMode _markerMode;

        public CSharpOutputBuilder(string name, string indentationString = DefaultIndentationString, bool isTestOutput = false, MarkerMode markerMode = MarkerMode.None)
        {
            _name = name;
            _contents = new List<string>();
            _currentLine = new StringBuilder();
            _usingDirectives = new SortedSet<string>();
            _staticUsingDirectives = new SortedSet<string>();
            _indentationString = indentationString;
            _isTestOutput = isTestOutput;
            _markerMode = markerMode;
        }

        public IEnumerable<string> Contents => _contents;

        public bool HasPendingLine => _currentLine.Length > 0;

        public string IndentationString => _indentationString;

        public bool IsTestOutput => _isTestOutput;

        public string Name => _name;

        public string Extension { get; } = ".cs";

        public bool NeedsNewline { get; set; }

        public bool NeedsSemicolon { get; set; }

        public IEnumerable<string> StaticUsingDirectives => _staticUsingDirectives;

        public IEnumerable<string> UsingDirectives => _usingDirectives;

        public void AddUsingDirective(string namespaceName)
        {
            if (namespaceName.StartsWith("static "))
            {
                _staticUsingDirectives.Add(namespaceName);
            }
            else
            {
                _usingDirectives.Add(namespaceName);
            }
        }

        public void DecreaseIndentation()
        {
            if (_indentationLevel == 0)
            {
                throw new InvalidOperationException();
            }

            _indentationLevel--;
        }

        public void IncreaseIndentation()
        {
            _indentationLevel++;
        }

        public void WriteBlockStart()
        {
            WriteIndentedLine('{');
            IncreaseIndentation();
        }

        public void WriteBlockEnd()
        {
            // We don't need a newline if immediately closing the scope
            NeedsNewline = false;

            // We don't need a semicolon if immediately closing the scope
            NeedsSemicolon = false;

            DecreaseIndentation();
            WriteIndentedLine('}');
        }

        public void Write<T>(T value)
        {
            _currentLine.Append(value);
        }

        public void WriteIndentation()
        {
            WriteNewlineIfNeeded();

            for (var i = 0; i < _indentationLevel; i++)
            {
                _currentLine.Append(_indentationString);
            }
        }

        public void WriteIndented<T>(T value)
        {
            WriteIndentation();
            Write(value);
        }

        public void WriteIndentedLine<T>(T value)
        {
            WriteIndentation();
            WriteLine(value);
        }

        public void WriteLine<T>(T value)
        {
            Write(value);
            WriteNewline();
        }

        public void WriteNewline()
        {
            _contents.Add(_currentLine.ToString());
            _currentLine.Clear();
            NeedsNewline = false;
        }

        public void WriteNewlineIfNeeded()
        {
            if (NeedsNewline)
            {
                WriteNewline();
            }
        }

        public void WritePendingLine()
        {
            if (HasPendingLine)
            {
                WriteNewline();
            }
        }

        public void WriteSemicolon()
        {
            Write(';');
            NeedsSemicolon = false;
            NeedsNewline = true;
        }

        public void WriteSemicolonIfNeeded()
        {
            if (NeedsSemicolon)
            {
                WriteSemicolon();
            }
        }

        public void BeginMarker(string kind, params KeyValuePair<string, object>[] attributes)
        {
            if (_markerMode != MarkerMode.Xml)
            {
                return;
            }

            Write("/*M*/<");
            Write(kind);
            foreach (var kvp in attributes)
            {
                Write(' ');
                Write(kvp.Key);
                Write('=');
                Write('"');
                Write(kvp.Value);
                Write('"');
            }

            Write("/*M*/>");
        }

        public void EndMarker(string kind)
        {
            if (_markerMode != MarkerMode.Xml)
            {
                return;
            }

            Write("/*M*/</");
            Write(kind);
            Write("/*M*/>");
        }

        private void AddCppAttributes(IEnumerable<string> attrs, string prefix = null, string postfix = null)
        {
            var attributeList = string.Join("^", attrs);
            if (string.IsNullOrWhiteSpace(attributeList))
            {
                return;
            }

            if (prefix is null)
            {
                WriteIndentation();
            }
            else
            {
                WriteNewlineIfNeeded();
                Write(prefix);
            }

            Write("[CppAttributeList(\"");
            Write(attributeList);
            Write("\")]");

            if (postfix is null)
            {
                NeedsNewline = true;
            }
            else
            {
                Write(postfix);
            }
        }

        private void AddNativeInheritanceAttribute(string inheritedFromName, string prefix = null, string postfix = null, string attributePrefix = null)
        {
            if (prefix is null)
            {
                WriteIndentation();
            }
            else
            {
                WriteNewlineIfNeeded();
                Write(prefix);
            }

            Write('[');

            if (attributePrefix != null)
            {
                Write(attributePrefix);
            }

            Write("NativeInheritance");
            Write('(');
            Write('"');
            Write(PInvokeGenerator.EscapeString(inheritedFromName));
            Write('"');
            Write(')');
            Write(']');

            if (postfix is null)
            {
                NeedsNewline = true;
            }
            else
            {
                Write(postfix);
            }
        }

        private void AddVtblIndexAttribute(long vtblIndex, string prefix = null, string postfix = null, string attributePrefix = null)
        {
            if (prefix is null)
            {
                WriteIndentation();
            }
            else
            {
                WriteNewlineIfNeeded();
                Write(prefix);
            }

            Write('[');

            if (attributePrefix != null)
            {
                Write(attributePrefix);
            }

            Write("VtblIndex");
            Write('(');
            Write(vtblIndex);
            Write(')');
            Write(']');

            if (postfix is null)
            {
                NeedsNewline = true;
            }
            else
            {
                Write(postfix);
            }
        }

        private void AddNativeTypeNameAttribute(string nativeTypeName, string prefix = null, string postfix = null, string attributePrefix = null)
        {
            if (string.IsNullOrWhiteSpace(nativeTypeName))
            {
                return;
            }

            if (prefix is null)
            {
                WriteIndentation();
            }
            else
            {
                WriteNewlineIfNeeded();
                Write(prefix);
            }

            Write('[');

            if (attributePrefix != null)
            {
                Write(attributePrefix);
            }

            Write("NativeTypeName(\"");
            Write(PInvokeGenerator.EscapeString(nativeTypeName));
            Write("\")]");

            if (postfix is null)
            {
                NeedsNewline = true;
            }
            else
            {
                Write(postfix);
            }
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach (var line in _contents)
            {
                result.Append(line);
                result.Append('\n');
            }

            result.Append(_currentLine);
            return result.ToString();
        }
    }
}
