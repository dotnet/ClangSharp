using System;
using System.Collections.Generic;
using System.Text;

namespace ClangSharp
{
    public sealed class OutputBuilder
    {
        public const string DefaultIndentationString = "    ";

        private readonly string _name;
        private readonly List<string> _contents;
        private readonly StringBuilder _currentLine;
        private readonly SortedSet<string> _usingDirectives;
        private readonly string _indentationString;

        private int _indentationLevel;

        public OutputBuilder(string name, string indentationString = DefaultIndentationString)
        {
            _name = name;
            _contents = new List<string>();
            _currentLine = new StringBuilder();
            _usingDirectives = new SortedSet<string>();
            _indentationString = indentationString;
        }

        public IEnumerable<string> Contents => _contents;

        public string IndentationString => _indentationString;

        public string Name => _name;

        public IEnumerable<string> UsingDirectives => _usingDirectives;

        public void AddUsingDirective(string namespaceName)
        {
            _usingDirectives.Add(namespaceName);
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
            DecreaseIndentation();
            WriteIndentedLine('}');
        }

        public void Write<T>(T value)
        {
            _currentLine.Append(value);
        }

        public void WriteIndentation()
        {
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
            WriteLine();
        }

        public void WriteLine()
        {
            _contents.Add(_currentLine.ToString());
            _currentLine.Clear();
        }
    }
}
