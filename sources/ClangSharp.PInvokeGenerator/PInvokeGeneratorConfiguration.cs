// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClangSharp
{
    public sealed class PInvokeGeneratorConfiguration
    {
        private const string DefaultMethodClassName = "Methods";

        private readonly Dictionary<string, string> _remappedNames;
        private readonly Dictionary<string, IReadOnlyList<string>> _withAttributes;
        private readonly Dictionary<string, string> _withCallConvs;
        private readonly Dictionary<string, string> _withLibraryPaths;
        private readonly Dictionary<string, string> _withTypes;
        private readonly Dictionary<string, IReadOnlyList<string>> _withUsings;
        private readonly PInvokeGeneratorConfigurationOptions _options;

        public PInvokeGeneratorConfiguration(string libraryPath, string namespaceName, string outputLocation, string testOutputLocation, PInvokeGeneratorConfigurationOptions options = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, string headerFile = null, string methodClassName = null, string methodPrefixToStrip = null, IReadOnlyDictionary<string, string> remappedNames = null, string[] traversalNames = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null)
        {
            if (excludedNames is null)
            {
                excludedNames = Array.Empty<string>();
            }

            if (string.IsNullOrWhiteSpace(libraryPath))
            {
                libraryPath = string.Empty;
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

            if (options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode) && options.HasFlag(PInvokeGeneratorConfigurationOptions.GeneratePreviewCode))
            {
                throw new ArgumentOutOfRangeException(nameof(options));
            }

            if (string.IsNullOrWhiteSpace(outputLocation))
            {
                throw new ArgumentNullException(nameof(outputLocation));
            }

            if (traversalNames is null)
            {
                traversalNames = Array.Empty<string>();
            }

            if (withSetLastErrors is null)
            {
                withSetLastErrors = Array.Empty<string>();
            }

            _options = options;
            _remappedNames = new Dictionary<string, string>();
            _withAttributes = new Dictionary<string, IReadOnlyList<string>>();
            _withCallConvs = new Dictionary<string, string>();
            _withLibraryPaths = new Dictionary<string, string>();
            _withTypes = new Dictionary<string, string>();
            _withUsings = new Dictionary<string, IReadOnlyList<string>>();

            ExcludedNames = excludedNames;
            HeaderText = string.IsNullOrWhiteSpace(headerFile) ? string.Empty : File.ReadAllText(headerFile);
            LibraryPath = $@"""{libraryPath}""";
            MethodClassName = methodClassName;
            MethodPrefixToStrip = methodPrefixToStrip;
            Namespace = namespaceName;
            OutputLocation = Path.GetFullPath(outputLocation);
            TestOutputLocation = !string.IsNullOrWhiteSpace(testOutputLocation) ? Path.GetFullPath(testOutputLocation) : string.Empty;

            // Normalize the traversal names to use \ rather than / so path comparisons are simpler
            TraversalNames = traversalNames.Select(traversalName => traversalName.Replace('\\', '/')).ToArray();
            WithSetLastErrors = withSetLastErrors;

            if (!_options.HasFlag(PInvokeGeneratorConfigurationOptions.NoDefaultRemappings))
            {
                if (GeneratePreviewCodeNint)
                {
                    _remappedNames.Add("intptr_t", "nint");
                    _remappedNames.Add("ptrdiff_t", "nint");
                    _remappedNames.Add("size_t", "nuint");
                    _remappedNames.Add("uintptr_t", "nuint");
                }
                else
                {
                    _remappedNames.Add("intptr_t", "IntPtr");
                    _remappedNames.Add("ptrdiff_t", "IntPtr");
                    _remappedNames.Add("size_t", "UIntPtr");
                    _remappedNames.Add("uintptr_t", "UIntPtr");
                }
            }

            AddRange(_remappedNames, remappedNames);
            AddRange(_withAttributes, withAttributes);
            AddRange(_withCallConvs, withCallConvs);
            AddRange(_withLibraryPaths, withLibraryPaths);
            AddRange(_withTypes, withTypes);
            AddRange(_withUsings, withUsings);
        }

        public string[] ExcludedNames { get; }

        public bool GenerateCompatibleCode => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode);

        public bool GenerateExplicitVtbls => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls);

        public bool GeneratePreviewCodeFnptr => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GeneratePreviewCodeFnptr);

        public bool GeneratePreviewCodeNint => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GeneratePreviewCodeNint);

        public bool GenerateMultipleFiles => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles);

        public bool GenerateTestsNUnit => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit);

        public bool GenerateTestsXUnit => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit);

        public bool GenerateUnixTypes => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateUnixTypes);

        public string HeaderText { get; }

        public string LibraryPath { get;}

        public bool LogExclusions => _options.HasFlag(PInvokeGeneratorConfigurationOptions.LogExclusions);

        public bool LogVisitedFiles => _options.HasFlag(PInvokeGeneratorConfigurationOptions.LogVisitedFiles);

        public string MethodClassName { get; }

        public string MethodPrefixToStrip { get;}

        public string Namespace { get; }

        public string OutputLocation { get; }

        public IReadOnlyDictionary<string, string> RemappedNames => _remappedNames;

        public string TestOutputLocation { get; }

        public string[] TraversalNames { get; }

        public IReadOnlyDictionary<string, IReadOnlyList<string>> WithAttributes => _withAttributes;

        public IReadOnlyDictionary<string, string> WithCallConvs => _withCallConvs;

        public IReadOnlyDictionary<string, string> WithLibraryPaths => _withLibraryPaths;

        public string[] WithSetLastErrors { get; }

        public IReadOnlyDictionary<string, string> WithTypes => _withTypes;

        public IReadOnlyDictionary<string, IReadOnlyList<string>> WithUsings => _withUsings;

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
