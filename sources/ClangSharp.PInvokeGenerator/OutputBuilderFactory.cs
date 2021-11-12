// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Abstractions;
using ClangSharp.CSharp;
using ClangSharp.XML;

namespace ClangSharp
{
    internal sealed class OutputBuilderFactory
    {
        private readonly PInvokeGeneratorConfiguration _config;
        private readonly bool _writeSourceLocation;
        private readonly Dictionary<string, IOutputBuilder> _outputBuilders;

        public OutputBuilderFactory(PInvokeGeneratorConfiguration config)
        {
            _config = config;
            _writeSourceLocation = config.GenerateSourceLocationAttribute;
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

            var outputBuilder = _config.OutputMode switch
            {
                PInvokeGeneratorOutputMode.CSharp => (IOutputBuilder) new CSharpOutputBuilder(name, _config, writeSourceLocation: _writeSourceLocation),
                PInvokeGeneratorOutputMode.Xml => new XmlOutputBuilder(name, _config),
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

            var outputBuilder = new CSharpOutputBuilder(name, _config, isTestOutput: true, writeSourceLocation: _writeSourceLocation);

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
