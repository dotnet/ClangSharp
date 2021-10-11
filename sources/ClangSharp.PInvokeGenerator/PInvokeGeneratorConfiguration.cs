// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClangSharp.Abstractions;

namespace ClangSharp
{
    public sealed class PInvokeGeneratorConfiguration
    {
        private const string DefaultMethodClassName = "Methods";

        private readonly Dictionary<string, string> _remappedNames;
        private readonly Dictionary<string, AccessSpecifier> _withAccessSpecifiers;
        private readonly Dictionary<string, IReadOnlyList<string>> _withAttributes;
        private readonly Dictionary<string, string> _withCallConvs;
        private readonly Dictionary<string, string> _withLibraryPaths;
        private readonly Dictionary<string, string> _withTypes;
        private readonly Dictionary<string, IReadOnlyList<string>> _withUsings;

        private PInvokeGeneratorConfigurationOptions _options;

        public PInvokeGeneratorConfiguration(string libraryPath, string namespaceName, string outputLocation, string testOutputLocation, PInvokeGeneratorOutputMode outputMode = PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions options = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, string headerFile = null, string methodClassName = null, string methodPrefixToStrip = null, IReadOnlyDictionary<string, string> remappedNames = null, string[] traversalNames = null, IReadOnlyDictionary<string, string> withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withAttributes = null, IReadOnlyDictionary<string, string> withCallConvs = null, IReadOnlyDictionary<string, string> withLibraryPaths = null, string[] withSetLastErrors = null, IReadOnlyDictionary<string, string> withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>> withUsings = null)
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

            if (outputMode is not PInvokeGeneratorOutputMode.CSharp and not PInvokeGeneratorOutputMode.Xml)
            {
                throw new ArgumentOutOfRangeException(nameof(options));
            }

            if (outputMode == PInvokeGeneratorOutputMode.Xml &&
                (options & PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles) == 0 &&
                ((options & PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit) != 0 ||
                 (options & PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit) != 0))
            {
                // we can't mix XML and C#! we're in XML mode, not generating multiple files, and generating tests; fail
                throw new ArgumentException("Can't generate tests in XML mode without multiple files.",
                    nameof(options));
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
            _withAccessSpecifiers = new Dictionary<string, AccessSpecifier>();
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
            OutputMode = outputMode;
            OutputLocation = Path.GetFullPath(outputLocation);
            TestOutputLocation = !string.IsNullOrWhiteSpace(testOutputLocation) ? Path.GetFullPath(testOutputLocation) : string.Empty;

            // Normalize the traversal names to use \ rather than / so path comparisons are simpler
            TraversalNames = traversalNames.Select(traversalName => traversalName.Replace('\\', '/')).ToArray();
            WithSetLastErrors = withSetLastErrors;

            if (!_options.HasFlag(PInvokeGeneratorConfigurationOptions.NoDefaultRemappings))
            {
                if (!ExcludeNIntCodegen)
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
            AddRange(_withAccessSpecifiers, withAccessSpecifiers, ConvertStringToAccessSpecifier);
            AddRange(_withAttributes, withAttributes);
            AddRange(_withCallConvs, withCallConvs);
            AddRange(_withLibraryPaths, withLibraryPaths);
            AddRange(_withTypes, withTypes);
            AddRange(_withUsings, withUsings);
        }

        public bool DontUseUsingStaticsForEnums => _options.HasFlag(PInvokeGeneratorConfigurationOptions.DontUseUsingStaticsForEnums);

        public bool ExcludeComProxies => _options.HasFlag(PInvokeGeneratorConfigurationOptions.ExcludeComProxies);

        public bool ExcludeEmptyRecords => _options.HasFlag(PInvokeGeneratorConfigurationOptions.ExcludeEmptyRecords);

        public bool ExcludeEnumOperators => _options.HasFlag(PInvokeGeneratorConfigurationOptions.ExcludeEnumOperators);

        public string[] ExcludedNames { get; }

        public bool GenerateAggressiveInlining => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateAggressiveInlining);

        public bool GenerateCompatibleCode => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode);

        public bool GenerateExplicitVtbls => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls);

        public bool GenerateMacroBindings => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateMacroBindings);

        public bool ExcludeFnptrCodegen
        {
            get
            {
                return GenerateCompatibleCode || _options.HasFlag(PInvokeGeneratorConfigurationOptions.ExcludeFnptrCodegen);
            }

            set
            {
                if (value)
                {
                    _options |= PInvokeGeneratorConfigurationOptions.ExcludeFnptrCodegen;
                }
                else
                {
                    _options &= ~PInvokeGeneratorConfigurationOptions.ExcludeFnptrCodegen;
                }
            }
        }

        public bool ExcludeNIntCodegen => GenerateCompatibleCode || _options.HasFlag(PInvokeGeneratorConfigurationOptions.ExcludeNIntCodegen);

        public bool GenerateMultipleFiles => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles);

        public bool GenerateTestsNUnit => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit);

        public bool GenerateTestsXUnit => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit);

        public bool GenerateUnixTypes => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateUnixTypes);

        public string HeaderText { get; }

        public string LibraryPath { get;}

        public bool LogExclusions => _options.HasFlag(PInvokeGeneratorConfigurationOptions.LogExclusions);

        public bool LogVisitedFiles => _options.HasFlag(PInvokeGeneratorConfigurationOptions.LogVisitedFiles);

        public bool ExcludeFunctionsWithBody => _options.HasFlag(PInvokeGeneratorConfigurationOptions.ExcludeFunctionsWithBody);

        public bool ExcludeAnonymousFieldHelpers => _options.HasFlag(PInvokeGeneratorConfigurationOptions.ExcludeAnonymousFieldHelpers);

        public bool LogPotentialTypedefRemappings => _options.HasFlag(PInvokeGeneratorConfigurationOptions.LogPotentialTypedefRemappings);

        public bool GenerateCppAttributes => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateCppAttributes);

        public bool GenerateNativeInheritanceAttribute => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateNativeInheritanceAttribute);

        public bool GenerateTemplateBindings => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTemplateBindings);

        public bool GenerateUnmanagedConstants => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateUnmanagedConstants);

        public bool GenerateVtblIndexAttribute => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateVtblIndexAttribute);

        public bool GenerateSourceLocationAttribute => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateSourceLocationAttribute);

        public string MethodClassName { get; }

        public string MethodPrefixToStrip { get;}

        public string Namespace { get; }

        public PInvokeGeneratorOutputMode OutputMode { get; }

        public string OutputLocation { get; }

        public IReadOnlyDictionary<string, string> RemappedNames => _remappedNames;

        public string TestOutputLocation { get; }

        public string[] TraversalNames { get; }

        public IReadOnlyDictionary<string, AccessSpecifier> WithAccessSpcifier => _withAccessSpecifiers;

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

        private static void AddRange<TInput, TValue>(Dictionary<string, TValue> dictionary, IEnumerable<KeyValuePair<string, TInput>> keyValuePairs, Func<TInput, TValue> convert)
        {
            if (keyValuePairs != null)
            {
                foreach (var keyValuePair in keyValuePairs)
                {
                    // Use the indexer, rather than Add, so that any
                    // default mappings can be overwritten if desired.
                    dictionary[keyValuePair.Key] = convert(keyValuePair.Value);
                }
            }
        }

        private static AccessSpecifier ConvertStringToAccessSpecifier(string input)
        {
            if (input.Equals("internal", StringComparison.OrdinalIgnoreCase))
            {
                return AccessSpecifier.Internal;
            }
            else if (input.Equals("private", StringComparison.OrdinalIgnoreCase))
            {
                return AccessSpecifier.Private;
            }
            else if (input.Equals("private protected", StringComparison.OrdinalIgnoreCase))
            {
                return AccessSpecifier.PrivateProtected;
            }
            else if (input.Equals("protected", StringComparison.OrdinalIgnoreCase))
            {
                return AccessSpecifier.Protected;
            }
            else if (input.Equals("protected internal", StringComparison.OrdinalIgnoreCase))
            {
                return AccessSpecifier.ProtectedInternal;
            }
            else if (input.Equals("public", StringComparison.OrdinalIgnoreCase))
            {
                return AccessSpecifier.Public;
            }
            else
            {
                return AccessSpecifier.None;
            }
        }
    }
}
