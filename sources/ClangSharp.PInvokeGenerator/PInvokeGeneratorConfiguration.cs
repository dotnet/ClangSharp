// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using ClangSharp.Abstractions;

namespace ClangSharp;

public sealed class PInvokeGeneratorConfiguration
{
    private const string DefaultClassValue = "Methods";
    private const string DefaultLibraryPathValue = @"""""";
    private const string DefaultMethodPrefixToStripValue = "";
    private const string DefaultTestOutputLocationValue = "";

    private readonly string _defaultNamespace;
    private readonly string _headerText;
    private readonly string _libraryPath;
    private readonly string _outputLocation;
    private readonly PInvokeGeneratorOutputMode _outputMode;

    private readonly string _defaultClass;
    private readonly string _methodPrefixToStrip;
    private readonly string _testOutputLocation;

    private readonly SortedSet<string> _excludedNames;
    private readonly SortedSet<string> _forceRemappedNames;
    private readonly SortedSet<string> _includedNames;
    private readonly SortedSet<string> _withManualImports;
    private readonly SortedSet<string> _traversalNames;
    private readonly SortedSet<string> _withSetLastErrors;
    private readonly SortedSet<string> _withSuppressGCTransitions;

    private readonly SortedDictionary<string, string> _remappedNames;
    private readonly SortedDictionary<string, AccessSpecifier> _withAccessSpecifiers;
    private readonly SortedDictionary<string, IReadOnlyList<string>> _withAttributes;
    private readonly SortedDictionary<string, string> _withCallConvs;
    private readonly SortedDictionary<string, string> _withClasses;
    private readonly SortedDictionary<string, Guid> _withGuids;
    private readonly SortedDictionary<string, string> _withLibraryPaths;
    private readonly SortedDictionary<string, string> _withNamespaces;
    private readonly SortedDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> _withTransparentStructs;
    private readonly SortedDictionary<string, string> _withTypes;
    private readonly SortedDictionary<string, IReadOnlyList<string>> _withUsings;

    private PInvokeGeneratorConfigurationOptions _options;

    public PInvokeGeneratorConfiguration(string defaultNamespace, string outputLocation, string headerFile, PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions options)
    {
        if (string.IsNullOrWhiteSpace(defaultNamespace))
        {
            throw new ArgumentNullException(nameof(defaultNamespace));
        }
        _defaultNamespace = defaultNamespace;

        if (string.IsNullOrWhiteSpace(outputLocation))
        {
            throw new ArgumentNullException(nameof(outputLocation));
        }
        _outputLocation = Path.GetFullPath(outputLocation);

        if (outputMode is not PInvokeGeneratorOutputMode.CSharp and not PInvokeGeneratorOutputMode.Xml)
        {
            throw new ArgumentOutOfRangeException(nameof(outputMode));
        }
        _outputMode = outputMode;

        _defaultClass = DefaultClassValue;
        _headerText = string.IsNullOrWhiteSpace(headerFile) ? string.Empty : File.ReadAllText(headerFile);
        _libraryPath = DefaultLibraryPathValue;
        _methodPrefixToStrip = DefaultMethodPrefixToStripValue;
        _testOutputLocation = DefaultTestOutputLocationValue;

        _excludedNames = new SortedSet<string>();
        _forceRemappedNames = new SortedSet<string>();
        _includedNames = new SortedSet<string>();
        _withManualImports = new SortedSet<string>();
        _traversalNames = new SortedSet<string>();
        _withSetLastErrors = new SortedSet<string>();
        _withSuppressGCTransitions = new SortedSet<string>();

        _remappedNames = new SortedDictionary<string, string>();
        _withAccessSpecifiers = new SortedDictionary<string, AccessSpecifier>();
        _withAttributes = new SortedDictionary<string, IReadOnlyList<string>>();
        _withCallConvs = new SortedDictionary<string, string>();
        _withClasses = new SortedDictionary<string, string>();
        _withGuids = new SortedDictionary<string, Guid>();
        _withLibraryPaths = new SortedDictionary<string, string>();
        _withNamespaces = new SortedDictionary<string, string>();
        _withTransparentStructs = new SortedDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>();
        _withTypes = new SortedDictionary<string, string>();
        _withUsings = new SortedDictionary<string, IReadOnlyList<string>>();

        if ((outputMode == PInvokeGeneratorOutputMode.Xml) && !options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles) && (options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit) || options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit)))
        {
            // we can't mix XML and C#! we're in XML mode, not generating multiple files, and generating tests; fail
            throw new ArgumentException("Can't generate tests in XML mode without multiple files.", nameof(options));
        }
        else if (options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode) && options.HasFlag(PInvokeGeneratorConfigurationOptions.GeneratePreviewCode))
        {
            throw new ArgumentOutOfRangeException(nameof(options));
        }
        _options = options;

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
    }

    public string DefaultClass
    {
        get
        {
            return _defaultClass;
        }

        init
        {
            _defaultClass = string.IsNullOrWhiteSpace(value) ? DefaultClassValue : value;
        }
    }

    public bool DontUseUsingStaticsForEnums => _options.HasFlag(PInvokeGeneratorConfigurationOptions.DontUseUsingStaticsForEnums);

    public bool ExcludeAnonymousFieldHelpers => _options.HasFlag(PInvokeGeneratorConfigurationOptions.ExcludeAnonymousFieldHelpers);

    public bool ExcludeComProxies => _options.HasFlag(PInvokeGeneratorConfigurationOptions.ExcludeComProxies);

    public bool ExcludeEmptyRecords => _options.HasFlag(PInvokeGeneratorConfigurationOptions.ExcludeEmptyRecords);

    public bool ExcludeEnumOperators => _options.HasFlag(PInvokeGeneratorConfigurationOptions.ExcludeEnumOperators);

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

    public bool ExcludeFunctionsWithBody => _options.HasFlag(PInvokeGeneratorConfigurationOptions.ExcludeFunctionsWithBody);

    public IReadOnlyCollection<string> ExcludedNames
    {
        get
        {
            return _excludedNames;
        }

        init
        {
            AddRange(_excludedNames, value);
        }
    }

    public bool ExcludeNIntCodegen => GenerateCompatibleCode || _options.HasFlag(PInvokeGeneratorConfigurationOptions.ExcludeNIntCodegen);

    public bool GenerateAggressiveInlining => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateAggressiveInlining);

    public bool GenerateCompatibleCode => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode);

    public bool GenerateCppAttributes => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateCppAttributes);

    public bool GenerateDocIncludes => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateDocIncludes);

    public bool GenerateExplicitVtbls => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls);

    public bool GenerateFileScopedNamespaces => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateFileScopedNamespaces);

    public bool GenerateGuidMember => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateGuidMember);

    public bool GenerateHelperTypes => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateHelperTypes);

    public bool GenerateMacroBindings => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateMacroBindings);

    public bool GenerateMarkerInterfaces => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateMarkerInterfaces);

    public bool GenerateMultipleFiles => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles);

    public bool GenerateNativeInheritanceAttribute => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateNativeInheritanceAttribute);

    public bool GeneratePreviewCode => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GeneratePreviewCode);

    public bool GenerateSetsLastSystemErrorAttribute => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateSetsLastSystemErrorAttribute);

    public bool GenerateSourceLocationAttribute => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateSourceLocationAttribute);

    public bool GenerateTemplateBindings => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTemplateBindings);

    public bool GenerateTestsNUnit => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit);

    public bool GenerateTestsXUnit => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit);

    public bool GenerateTrimmableVtbls => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTrimmableVtbls);

    public bool GenerateUnixTypes => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateUnixTypes);

    public bool GenerateUnmanagedConstants => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateUnmanagedConstants);

    public bool GenerateVtblIndexAttribute => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateVtblIndexAttribute);

    public string HeaderText => _headerText;

    public IReadOnlyCollection<string> IncludedNames
    {
        get
        {
            return _includedNames;
        }

        init
        {
            AddRange(_includedNames, value);
        }
    }

    public string LibraryPath
    {
        get
        {
            return _libraryPath;
        }

        init
        {
            _libraryPath = string.IsNullOrWhiteSpace(value) ? DefaultLibraryPathValue : $@"""{value}""";
        }
    }

    public bool LogExclusions => _options.HasFlag(PInvokeGeneratorConfigurationOptions.LogExclusions);

    public bool LogPotentialTypedefRemappings => _options.HasFlag(PInvokeGeneratorConfigurationOptions.LogPotentialTypedefRemappings);

    public bool LogVisitedFiles => _options.HasFlag(PInvokeGeneratorConfigurationOptions.LogVisitedFiles);

    public string MethodPrefixToStrip
    {
        get
        {
            return _methodPrefixToStrip;
        }

        init
        {
            _methodPrefixToStrip = string.IsNullOrWhiteSpace(value) ? DefaultMethodPrefixToStripValue : value;
        }
    }

    public string DefaultNamespace => _defaultNamespace;

    public PInvokeGeneratorOutputMode OutputMode => _outputMode;

    public string OutputLocation => _outputLocation;

    public IReadOnlyDictionary<string, string> RemappedNames
    {
        get
        {
            return _remappedNames;
        }

        init
        {
            AddRange(_forceRemappedNames, value, ValueStartsWithAt);
            AddRange(_remappedNames, value, RemoveAtPrefix);
        }
    }

    public IReadOnlyCollection<string> ForceRemappedNames => _forceRemappedNames;

    public string TestOutputLocation
    {
        get
        {
            return _testOutputLocation;
        }

        init
        {
            _testOutputLocation = string.IsNullOrWhiteSpace(value) ? DefaultTestOutputLocationValue : Path.GetFullPath(value);
        }
    }

    public IReadOnlyCollection<string> TraversalNames
    {
        get
        {
            return _traversalNames;
        }

        init
        {
            AddRange(_traversalNames, value, NormalizePathSeparators);
        }
    }

    public IReadOnlyDictionary<string, AccessSpecifier> WithAccessSpecifiers
    {
        get
        {
            return _withAccessSpecifiers;
        }

        init
        {
            AddRange(_withAccessSpecifiers, value);
        }
    }

    public IReadOnlyDictionary<string, IReadOnlyList<string>> WithAttributes
    {
        get
        {
            return _withAttributes;
        }

        init
        {
            AddRange(_withAttributes, value);
        }
    }

    public IReadOnlyDictionary<string, string> WithCallConvs
    {
        get
        {
            return _withCallConvs;
        }

        init
        {
            AddRange(_withCallConvs, value);
        }
    }

    public IReadOnlyDictionary<string, string> WithClasses
    {
        get
        {
            return _withClasses;
        }

        init
        {
            AddRange(_withClasses, value);
        }
    }

    public IReadOnlyDictionary<string, Guid> WithGuids
    {
        get
        {
            return _withGuids;
        }

        init
        {
            AddRange(_withGuids, value);
        }
    }

    public IReadOnlyDictionary<string, string> WithLibraryPaths
    {
        get
        {
            return _withLibraryPaths;
        }

        init
        {
            AddRange(_withLibraryPaths, value);
        }
    }

    public IReadOnlyCollection<string> WithManualImports
    {
        get
        {
            return _withManualImports;
        }

        init
        {
            AddRange(_withManualImports, value);
        }
    }

    public IReadOnlyDictionary<string, string> WithNamespaces
    {
        get
        {
            return _withNamespaces;
        }

        init
        {
            AddRange(_withNamespaces, value);
        }
    }

    public IReadOnlyCollection<string> WithSetLastErrors
    {
        get
        {
            return _withSetLastErrors;
        }

        init
        {
            AddRange(_withSetLastErrors, value);
        }
    }

    public IReadOnlyCollection<string> WithSuppressGCTransitions
    {
        get
        {
            return _withSuppressGCTransitions;
        }

        init
        {
            AddRange(_withSuppressGCTransitions, value);
        }
    }

    public IReadOnlyDictionary<string, (string Name, PInvokeGeneratorTransparentStructKind Kind)> WithTransparentStructs
    {
        get
        {
            return _withTransparentStructs;
        }

        init
        {
            AddRange(_withTransparentStructs, value, RemoveAtPrefix);
        }
    }

    public IReadOnlyDictionary<string, string> WithTypes
    {
        get
        {
            return _withTypes;
        }

        init
        {
            AddRange(_withTypes, value);
        }
    }

    public IReadOnlyDictionary<string, IReadOnlyList<string>> WithUsings
    {
        get
        {
            return _withUsings;
        }

        init
        {
            AddRange(_withUsings, value);
        }
    }

    public static AccessSpecifier ConvertStringToAccessSpecifier(string input)
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

    private static void AddRange<TValue>(SortedDictionary<string, TValue> dictionary, IEnumerable<KeyValuePair<string, TValue>> keyValuePairs)
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

    private static void AddRange<TInput, TValue>(SortedDictionary<string, TValue> dictionary, IEnumerable<KeyValuePair<string, TInput>> keyValuePairs, Func<TInput, TValue> convert)
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

    private static void AddRange<TInput>(SortedSet<TInput> hashSet, IEnumerable<TInput> keys)
    {
        if (keys != null)
        {
            foreach (var key in keys)
            {
                _ = hashSet.Add(key);
            }
        }
    }

    private static void AddRange<TInput, TValue>(SortedSet<TValue> hashSet, IEnumerable<TInput> keys, Func<TInput, TValue> convert)
    {
        if (keys != null)
        {
            foreach (var key in keys)
            {
                _ = hashSet.Add(convert(key));
            }
        }
    }

    private static void AddRange<TInput>(SortedSet<string> hashSet, IEnumerable<KeyValuePair<string, TInput>> keyValuePairs, Func<TInput, bool> shouldAdd)
    {
        if (keyValuePairs != null)
        {
            foreach (var keyValuePair in keyValuePairs)
            {
                if (shouldAdd(keyValuePair.Value))
                {
                    _ = hashSet.Add(keyValuePair.Key);
                }
            }
        }
    }

    private static string NormalizePathSeparators(string value) => value.Replace('\\', '/');

    private static string RemoveAtPrefix(string value) => ValueStartsWithAt(value) ? value[1..] : value;

    private static (string, PInvokeGeneratorTransparentStructKind) RemoveAtPrefix((string Name, PInvokeGeneratorTransparentStructKind Kind) value) => (ValueStartsWithAt(value.Name) ? value.Name[1..] : value.Name, value.Kind);

    private static bool ValueStartsWithAt(string value) => value.StartsWith("@");
}
