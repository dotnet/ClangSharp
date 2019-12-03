// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace ClangSharp
{
    public sealed class PInvokeGeneratorConfiguration
    {
        private const string DefaultMethodClassName = "Methods";

        private readonly Dictionary<string, string> _remappedNames;
        private readonly Dictionary<string, IReadOnlyList<string>> _withAttributes;
        private readonly Dictionary<string, string> _withCallConvs;
        private readonly Dictionary<string, IReadOnlyList<string>> _withNamespaces;
        private readonly Dictionary<string, string> _withTypes;
        private readonly PInvokeGeneratorConfigurationOptions _options;

        public PInvokeGeneratorConfiguration(string libraryPath, string namespaceName, string outputLocation, PInvokeGeneratorConfigurationOptions options = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, string headerFile = null, string methodClassName = null, string methodPrefixToStrip = null, IReadOnlyDictionary<string, string> remappedNames = null, string[] traversalNames = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withNamespaces = null, IReadOnlyDictionary<string, string> withTypes = null)
        {
            if (excludedNames is null)
            {
                excludedNames = Array.Empty<string>();
            }

            if (string.IsNullOrWhiteSpace(libraryPath))
            {
                throw new ArgumentNullException(nameof(libraryPath));
            }

            if (string.IsNullOrWhiteSpace(methodClassName))
            {
                methodClassName = DefaultMethodClassName;
            }

            if (string.IsNullOrWhiteSpace(methodPrefixToStrip))
            {
                methodPrefixToStrip = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(namespaceName))
            {
                throw new ArgumentNullException(nameof(namespaceName));
            }

            if (string.IsNullOrWhiteSpace(outputLocation))
            {
                throw new ArgumentNullException(nameof(outputLocation));
            }

            if (traversalNames is null)
            {
                traversalNames = Array.Empty<string>();
            }

            _options = options;
            _remappedNames = new Dictionary<string, string>();
            _withAttributes = new Dictionary<string, IReadOnlyList<string>>();
            _withCallConvs = new Dictionary<string, string>();
            _withNamespaces = new Dictionary<string, IReadOnlyList<string>>();
            _withTypes = new Dictionary<string, string>();

            ExcludedNames = excludedNames;
            HeaderText = string.IsNullOrWhiteSpace(headerFile) ? string.Empty : File.ReadAllText(headerFile);
            LibraryPath = libraryPath;
            MethodClassName = methodClassName;
            MethodPrefixToStrip = methodPrefixToStrip;
            Namespace = namespaceName;
            OutputLocation = Path.GetFullPath(outputLocation);
            TraversalNames = traversalNames;

            if (!_options.HasFlag(PInvokeGeneratorConfigurationOptions.NoDefaultRemappings))
            {
                _remappedNames.Add("intptr_t", "IntPtr");
                _remappedNames.Add("ptrdiff_t", "IntPtr");
                _remappedNames.Add("size_t", "UIntPtr");
                _remappedNames.Add("uintptr_t", "UIntPtr");
            }

            AddRange(_remappedNames, remappedNames);
            AddRange(_withAttributes, withAttributes);
            AddRange(_withCallConvs, withCallConvs);
            AddRange(_withNamespaces, withNamespaces);
            AddRange(_withTypes, withTypes);
        }

        public string[] ExcludedNames { get; }

        public bool GenerateCompatibleCode => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode);

        public bool GenerateMultipleFiles => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles);

        public bool GenerateUnixTypes => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateUnixTypes);

        public string HeaderText { get; }

        public string LibraryPath { get;}

        public string MethodClassName { get; }

        public string MethodPrefixToStrip { get;}

        public string Namespace { get; }

        public string OutputLocation { get; }

        public IReadOnlyDictionary<string, string> RemappedNames => _remappedNames;

        public string[] TraversalNames { get; }

        public IReadOnlyDictionary<string, IReadOnlyList<string>> WithAttributes => _withAttributes;

        public IReadOnlyDictionary<string, string> WithCallConvs => _withCallConvs;

        public IReadOnlyDictionary<string, IReadOnlyList<string>> WithNamespaces => _withNamespaces;

        public IReadOnlyDictionary<string, string> WithTypes => _withTypes;

        private static void AddRange<TValue>(Dictionary<string, TValue> dictionary, IEnumerable<KeyValuePair<string, TValue>> keyValuePairs)
        {
            if (keyValuePairs != null)
            {
                foreach (var keyValuePair in keyValuePairs)
                {
                    // Use the indexer, rather than Add, so that any
                    // default mappings can be overwritten if desired.
                    dictionary[keyValuePair.Key] = keyValuePair.Value;
                }
            }
        }
    }
}
