// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace ClangSharp.CSharp
{
    public sealed partial class CSharpOutputBuilder
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

        public string IndentationString => _indentationString;

        public bool IsTestOutput => _isTestOutput;

        public string Name => _name;

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

            Write('<');
            Write(kind);
            foreach ((string key, object value) in attributes)
            {
                Write(' ');
                Write(key);
                Write('=');
                Write('"');
                Write(value);
                Write('"');
            }

            Write('>');
        }

        public void EndMarker(string kind)
        {
            Write("</");
            Write(kind);
            Write('>');
        }
    }
}
