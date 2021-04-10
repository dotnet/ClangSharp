// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Abstractions;
using ClangSharp.CSharp;
using ClangSharp.XML;

namespace ClangSharp
{
    internal sealed class OutputBuilderFactory
    {
        private readonly PInvokeGeneratorOutputMode _mode;
        private readonly bool _writeSourceLocation;
        private readonly Dictionary<string, IOutputBuilder> _outputBuilders;

        public OutputBuilderFactory(PInvokeGeneratorConfiguration mode)
        {
            _mode = mode.OutputMode;
            _writeSourceLocation = mode.GenerateSourceLocationAttribute;
            _outputBuilders = new Dictionary<string, IOutputBuilder>();
        }

        public IEnumerable<IOutputBuilder> OutputBuilders => _outputBuilders.Values;

        public void Clear() => _outputBuilders.Clear();

        public IOutputBuilder Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var outputBuilder = _mode switch
            {
                PInvokeGeneratorOutputMode.CSharp => (IOutputBuilder) new CSharpOutputBuilder(name, writeSourceLocation: _writeSourceLocation),
                PInvokeGeneratorOutputMode.Xml => new XmlOutputBuilder(name),
                _ => throw new InvalidOperationException()
            };

            _outputBuilders.Add(name, outputBuilder);
            return outputBuilder;
        }

        public CSharpOutputBuilder CreateTests(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var outputBuilder = new CSharpOutputBuilder(name, isTestOutput: true, writeSourceLocation: _writeSourceLocation);

            _outputBuilders.Add(name, outputBuilder);
            return outputBuilder;
        }

        public IOutputBuilder GetOutputBuilder(string name) => string.IsNullOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name)) : _outputBuilders[name];

        public CSharpOutputBuilder GetTestOutputBuilder(string name)
        {
            return string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentNullException(nameof(name))
                : _outputBuilders[name] is CSharpOutputBuilder csharpOutputBuilder && csharpOutputBuilder.IsTestOutput
                ? csharpOutputBuilder
                : throw new ArgumentException("A test output builder was not found with the given name", nameof(name));
        }

        public bool TryGetOutputBuilder(string name, out IOutputBuilder outputBuilder)
        {
            return string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentNullException(nameof(name))
                : _outputBuilders.TryGetValue(name, out outputBuilder);
        }
    }
}
