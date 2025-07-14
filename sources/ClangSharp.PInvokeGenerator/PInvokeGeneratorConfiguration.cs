// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;

namespace ClangSharp;

public sealed class PInvokeGeneratorConfiguration
{
    private const string DefaultClassValue = "Methods";
    private const string DefaultLibraryPathValue = @"""""";
    private const string DefaultMethodPrefixToStripValue = "";
    private const string DefaultTestOutputLocationValue = "";

    private readonly string _language;
    private readonly string _languageStandard;

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
    private readonly SortedSet<Regex> _forceRemappedRegexes;
    private readonly SortedSet<string> _includedNames;
    private readonly SortedSet<string> _nativeTypeNamesToStrip;
    private readonly SortedSet<string> _withManualImports;
    private readonly SortedSet<string> _traversalNames;
    private readonly SortedSet<string> _withSetLastErrors;
    private readonly SortedSet<string> _withSuppressGCTransitions;

    private readonly SortedDictionary<string, string> _remappedNames;
    private readonly SortedDictionary<Regex, string> _remappedRegexes;
    private readonly SortedDictionary<string, AccessSpecifier> _withAccessSpecifiers;
    private readonly SortedDictionary<string, IReadOnlyList<string>> _withAttributes;
    private readonly SortedDictionary<Regex, IReadOnlyList<string>> _withAttributesRegex;
    private readonly SortedDictionary<string, string> _withCallConvs;
    private readonly SortedDictionary<string, string> _withClasses;
    private readonly SortedDictionary<string, Guid> _withGuids;
    private readonly SortedDictionary<string, string> _withLengths;
    private readonly SortedDictionary<string, string> _withLibraryPaths;
    private readonly SortedDictionary<string, string> _withNamespaces;
    private readonly SortedDictionary<string, (string, PInvokeGeneratorTransparentStructKind)> _withTransparentStructs;
    private readonly SortedDictionary<string, string> _withTypes;
    private readonly SortedDictionary<string, IReadOnlyList<string>> _withUsings;
    private readonly SortedDictionary<string, string> _withPackings;

    private PInvokeGeneratorConfigurationOptions _options;

    public PInvokeGeneratorConfiguration(string language, string languageStandard, string defaultNamespace, string outputLocation, string? headerFile, PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions options)
    {
        _language = language;
        _languageStandard = languageStandard;

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

        _excludedNames = [];
        _forceRemappedNames = [];
        _forceRemappedRegexes = new SortedSet<Regex>(new RegexComparer());
        _includedNames = [];
        _nativeTypeNamesToStrip = [];
        _withManualImports = [];
        _traversalNames = [];
        _withSetLastErrors = [];
        _withSuppressGCTransitions = [];

        _remappedNames = [];
        _remappedRegexes = new SortedDictionary<Regex, string>(new RegexComparer());
        _withAccessSpecifiers = [];
        _withAttributes = [];
        _withAttributesRegex = new SortedDictionary<Regex, IReadOnlyList<string>>(new RegexComparer());
        _withCallConvs = [];
        _withClasses = [];
        _withGuids = [];
        _withLengths = [];
        _withLibraryPaths = [];
        _withNamespaces = [];
        _withTransparentStructs = [];
        _withTypes = [];
        _withUsings = [];
        _withPackings = [];

        if ((outputMode == PInvokeGeneratorOutputMode.Xml) && !options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles) && (options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit) || options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit)))
        {
            // we can't mix XML and C#! we're in XML mode, not generating multiple files, and generating tests; fail
            throw new ArgumentException("Can't generate tests in XML mode without multiple files.", nameof(options));
        }
        else if (options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode) && options.HasFlag(PInvokeGeneratorConfigurationOptions.GeneratePreviewCode))
        {
            throw new ArgumentOutOfRangeException(nameof(options));
        }
        else if (options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode) && options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateLatestCode))
        {
            throw new ArgumentOutOfRangeException(nameof(options));
        }
        else if (options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateLatestCode) && options.HasFlag(PInvokeGeneratorConfigurationOptions.GeneratePreviewCode))
        {
            throw new ArgumentOutOfRangeException(nameof(options));
        }

        if (options.HasFlag(PInvokeGeneratorConfigurationOptions.GeneratePreviewCode))
        {
            // While users shouldn't have passed it in like this, we can simplify
            // our own downstream checks be having preview also opt into "latest".
            options |= PInvokeGeneratorConfigurationOptions.GenerateLatestCode;
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

    [AllowNull]
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

    [AllowNull]
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

    public bool GenerateCallConvMemberFunction => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateCallConvMemberFunction);

    public bool GenerateCompatibleCode => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode);

    public bool GenerateCppAttributes => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateCppAttributes);

    public bool GenerateDisableRuntimeMarshalling => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateDisableRuntimeMarshalling);

    public bool GenerateDocIncludes => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateDocIncludes);

    public bool GenerateExplicitVtbls => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls);

    public bool GenerateFileScopedNamespaces => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateFileScopedNamespaces);

    public bool GenerateGenericPointerWrapper => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateGenericPointerWrapper);

    public bool GenerateGuidMember => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateGuidMember);

    public bool GenerateHelperTypes => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateHelperTypes);

    public bool GenerateLatestCode => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateLatestCode);

    public bool GenerateMacroBindings => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateMacroBindings);

    public bool GenerateMarkerInterfaces => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateMarkerInterfaces);

    public bool GenerateMultipleFiles => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles);

    public bool GenerateNativeBitfieldAttribute => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateNativeBitfieldAttribute);

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

    public bool StripEnumMemberTypeName => _options.HasFlag(PInvokeGeneratorConfigurationOptions.StripEnumMemberTypeName);

    public bool DontUseUsingStaticsForGuidMember => _options.HasFlag(PInvokeGeneratorConfigurationOptions.DontUseUsingStaticsForGuidMember);

    public string HeaderText => _headerText;

    [AllowNull]
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

    [AllowNull]
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

    [AllowNull]
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

    [AllowNull]
    public IReadOnlyCollection<string> NativeTypeNamesToStrip
    {
        get
        {
            return _nativeTypeNamesToStrip;
        }

        init
        {
            AddRange(_nativeTypeNamesToStrip, value, StringExtensions.NormalizePath);
            AddRange(_nativeTypeNamesToStrip, value, StringExtensions.NormalizeFullPath);
        }
    }

    public string DefaultNamespace => _defaultNamespace;

    public PInvokeGeneratorOutputMode OutputMode => _outputMode;

    public string OutputLocation => _outputLocation;

    public string Language => _language;

    public string LanguageStandard => _languageStandard;

    [AllowNull]
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

    [AllowNull]
    public IReadOnlyDictionary<Regex, string> RemappedRegexes
    {
        get
        {
            return _remappedRegexes;
        }

        init
        {
            AddRange(_forceRemappedRegexes, value,ValueStartsWithAt);
            AddRange(_remappedRegexes, value,RemoveAtPrefix);
        }
    }

    public IReadOnlyCollection<string> ForceRemappedNames => _forceRemappedNames;
    public IReadOnlyCollection<Regex> ForceRemappedRegexes => _forceRemappedRegexes;

    [AllowNull]
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

    [AllowNull]
    public IReadOnlyCollection<string> TraversalNames
    {
        get
        {
            return _traversalNames;
        }

        init
        {
            AddRange(_traversalNames, value, StringExtensions.NormalizePath);
            AddRange(_traversalNames, value, StringExtensions.NormalizeFullPath);
        }
    }

    [AllowNull]
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

    [AllowNull]
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

    [AllowNull]
    public IReadOnlyDictionary<Regex, IReadOnlyList<string>> WithAttributesRegex
    {
        get
        {
            return _withAttributesRegex;
        }

        init
        {
            AddRange(_withAttributesRegex, value);
        }
    }

    [AllowNull]
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

    [AllowNull]
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

    [AllowNull]
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

    [AllowNull]
    public IReadOnlyDictionary<string, string> WithLengths
    {
        get
        {
            return _withLengths;
        }

        init
        {
            AddRange(_withLengths, value);
        }
    }

    [AllowNull]
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

    [AllowNull]
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

    [AllowNull]
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

    [AllowNull]
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

    [AllowNull]
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

    [AllowNull]
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

    [AllowNull]
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

    [AllowNull]
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

    [AllowNull]
    public IReadOnlyDictionary<string, string> WithPackings
    {
        get
        {
            return _withPackings;
        }

        init
        {
            AddRange(_withPackings, value);
        }
    }

    public static AccessSpecifier ConvertStringToAccessSpecifier(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return input.Equals("internal", StringComparison.OrdinalIgnoreCase) ? AccessSpecifier.Internal
             : input.Equals("private", StringComparison.OrdinalIgnoreCase) ? AccessSpecifier.Private
             : input.Equals("private protected", StringComparison.OrdinalIgnoreCase) ? AccessSpecifier.PrivateProtected
             : input.Equals("protected", StringComparison.OrdinalIgnoreCase) ? AccessSpecifier.Protected
             : input.Equals("protected internal", StringComparison.OrdinalIgnoreCase) ? AccessSpecifier.ProtectedInternal
             : input.Equals("public", StringComparison.OrdinalIgnoreCase) ? AccessSpecifier.Public : AccessSpecifier.None;
    }

    private static void AddRange<TValue,TKey>(SortedDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>>? keyValuePairs) where TKey : notnull
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

    private static void AddRange<TInput, TValue>(SortedDictionary<string, TValue> dictionary, IEnumerable<KeyValuePair<string, TInput>>? keyValuePairs, Func<TInput, TValue> convert)
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

    private static void AddRange<TInput, TValue>(SortedDictionary<Regex, TValue> dictionary, IEnumerable<KeyValuePair<Regex, TInput>>? keyValuePairs, Func<TInput, TValue> convert)
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

    private static void AddRange<TInput>(SortedSet<TInput> hashSet, IEnumerable<TInput>? keys)
    {
        if (keys != null)
        {
            foreach (var key in keys)
            {
                _ = hashSet.Add(key);
            }
        }
    }

    private static void AddRange<TInput, TValue>(SortedSet<TValue> hashSet, IEnumerable<TInput>? keys, Func<TInput, TValue> convert)
    {
        if (keys != null)
        {
            foreach (var key in keys)
            {
                _ = hashSet.Add(convert(key));
            }
        }
    }

    private static void AddRange<TInput>(SortedSet<string> hashSet, IEnumerable<KeyValuePair<string, TInput>>? keyValuePairs, Func<TInput, bool> shouldAdd)
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

    private static void AddRange<TInput>(SortedSet<Regex> hashSet, IEnumerable<KeyValuePair<Regex, TInput>>? keyValuePairs, Func<TInput, bool> shouldAdd)
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

    private static string RemoveAtPrefix(string value) => ValueStartsWithAt(value) ? value[1..] : value;

    private static (string, PInvokeGeneratorTransparentStructKind) RemoveAtPrefix((string Name, PInvokeGeneratorTransparentStructKind Kind) value) => (ValueStartsWithAt(value.Name) ? value.Name[1..] : value.Name, value.Kind);

    private static bool ValueStartsWithAt(string value) => value.StartsWith('@');
}
