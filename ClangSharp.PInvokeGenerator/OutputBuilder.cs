using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClangSharp
{
    internal sealed class OutputBuilder : IDisposable
    {
        private string _outputFile;
        private StringBuilder _output;
        private SortedSet<string> _usings;
        private ConfigurationOptions _config;
        private int _indentation;

        public OutputBuilder(string outputFile, ConfigurationOptions config, bool isMethodClass)
        {
            _outputFile = outputFile;
            _output = new StringBuilder();
            _usings = new SortedSet<string>();
            _config = config;
            _indentation = 0;

            WriteIndented("namespace");
            Write(' ');
            WriteLine(_config.Namespace);
            WriteBlockStart();

            if (isMethodClass)
            {
                WriteLine();
                WriteIndented("public static");
                Write(' ');

                if (_config.GenerateUnsafeCode)
                {
                    Write("unsafe");
                    Write(' ');
                }

                Write("partial class");
                Write(' ');
                WriteLine(_config.MethodClassName);
                WriteBlockStart();
                WriteIndented("private const string libraryPath = ");
                Write('"');
                Write(_config.LibraryPath);
                Write('"');
                WriteLine(';');
                WriteLine();
            }
        }

        public string OutputFile => _outputFile;

        public void AddUsing(string @namespace)
        {
            _usings.Add(@namespace);
        }

        public void Dispose()
        {
            while (_indentation != 0)
            {
                WriteBlockEnd();
            }

            Directory.CreateDirectory(Path.GetDirectoryName(_outputFile));

            using (var sw = new StreamWriter(_outputFile))
            {
                if (_usings.Count != 0)
                {
                    foreach (var @using in _usings)
                    {
                        sw.Write("using");
                        sw.Write(' ');
                        sw.Write(@using);
                        sw.WriteLine(';');
                    }

                    sw.WriteLine();
                }

                sw.Write(_output);
            }

            _outputFile = null;
            _output = null;
            _indentation = 0;
            _config = null;
        }

        public void WriteBlockStart()
        {
            WriteIndentedLine('{');
            _indentation++;
        }

        public void WriteBlockEnd()
        {
            _indentation--;
            WriteIndentedLine('}');
        }

        public void Write<T>(T value)
        {
            _output.Append(value);
        }

        public void WriteIndentation()
        {
            for (var i = 0; i < _indentation; i++)
            {
                _output.Append("    ");
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
            _output.AppendLine();
        }
    }
}
