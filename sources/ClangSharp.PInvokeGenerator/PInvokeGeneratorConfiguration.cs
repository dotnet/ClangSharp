// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

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

    internal readonly HashSet<string> _excludedNames;
    private readonly HashSet<string> _forceRemappedNames;
    private readonly HashSet<string> _includedNames;
    private readonly HashSet<string> _nativeTypeNamesToStrip;
    private readonly HashSet<string> _withManualImports;
    private readonly HashSet<string> _traversalNames;
    internal readonly HashSet<string> _withReadonlys;
    internal readonly HashSet<string> _withSetLastErrors;
    internal readonly HashSet<string> _withSuppressGCTransitions;

    internal readonly Dictionary<string, string> _remappedNames;
    internal readonly Dictionary<string, string> _remappedTypeNames;
    internal readonly Dictionary<string, string> _remappedFieldNames;
    internal readonly Dictionary<string, AccessSpecifier> _withAccessSpecifiers;
    internal readonly Dictionary<string, IReadOnlyList<string>> _withAttributes;
    internal readonly Dictionary<string, string> _withCallConvs;
    internal readonly Dictionary<string, string> _withClasses;
    internal readonly Dictionary<string, Guid> _withGuids;
    internal readonly Dictionary<string, string> _withLengths;
    private readonly Dictionary<string, string> _withLibraryPaths;
    internal readonly Dictionary<string, string> _withNamespaces;
    private readonly Dictionary<string, (string, PInvokeGeneratorTransparentStructKind)> _withTransparentStructs;
    internal readonly Dictionary<string, string> _withTypes;
    internal readonly Dictionary<string, IReadOnlyList<string>> _withUsings;
    internal readonly Dictionary<string, string> _withPackings;

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

        _excludedNames = new HashSet<string>(QualifiedNameComparer.Default);
        _forceRemappedNames = new HashSet<string>(QualifiedNameComparer.Default);
        _includedNames = new HashSet<string>(QualifiedNameComparer.Default);
        _nativeTypeNamesToStrip = new HashSet<string>(StringComparer.Ordinal);
        _withManualImports = new HashSet<string>(StringComparer.Ordinal);
        _traversalNames = new HashSet<string>(StringComparer.Ordinal);
        _withReadonlys = new HashSet<string>(QualifiedNameComparer.Default);
        _withSetLastErrors = new HashSet<string>(QualifiedNameComparer.Default);
        _withSuppressGCTransitions = new HashSet<string>(QualifiedNameComparer.Default);

        _remappedNames = new Dictionary<string, string>(QualifiedNameComparer.Default);
        _remappedTypeNames = new Dictionary<string, string>(QualifiedNameComparer.Default);
        _remappedFieldNames = new Dictionary<string, string>(QualifiedNameComparer.Default);
        _withAccessSpecifiers = new Dictionary<string, AccessSpecifier>(QualifiedNameComparer.Default);
        _withAttributes = new Dictionary<string, IReadOnlyList<string>>(QualifiedNameComparer.Default);
        _withCallConvs = new Dictionary<string, string>(QualifiedNameComparer.Default);
        _withClasses = new Dictionary<string, string>(StringComparer.Ordinal);
        _withGuids = new Dictionary<string, Guid>(QualifiedNameComparer.Default);
        _withLengths = new Dictionary<string, string>(QualifiedNameComparer.Default);
        _withLibraryPaths = new Dictionary<string, string>(StringComparer.Ordinal);
        _withNamespaces = new Dictionary<string, string>(StringComparer.Ordinal);
        _withTransparentStructs = new Dictionary<string, (string, PInvokeGeneratorTransparentStructKind)>(StringComparer.Ordinal);
        _withTypes = new Dictionary<string, string>(QualifiedNameComparer.Default);
        _withUsings = new Dictionary<string, IReadOnlyList<string>>(QualifiedNameComparer.Default);
        _withPackings = new Dictionary<string, string>(QualifiedNameComparer.Default);

        if ((outputMode == PInvokeGeneratorOutputMode.Xml) && (options & PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles) == 0 && ((options & PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit) != 0 || (options & PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit) != 0))
        {
            // we can't mix XML and C#! we're in XML mode, not generating multiple files, and generating tests; fail
            throw new ArgumentException("Can't generate tests in XML mode without multiple files.", nameof(options));
        }
        else if ((options & PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode) != 0 && (options & PInvokeGeneratorConfigurationOptions.GeneratePreviewCode) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options));
        }
        else if ((options & PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode) != 0 && (options & PInvokeGeneratorConfigurationOptions.GenerateLatestCode) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options));
        }
        else if ((options & PInvokeGeneratorConfigurationOptions.GenerateLatestCode) != 0 && (options & PInvokeGeneratorConfigurationOptions.GeneratePreviewCode) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options));
        }

        if ((options & PInvokeGeneratorConfigurationOptions.GeneratePreviewCode) != 0)
        {
            // While users shouldn't have passed it in like this, we can simplify
            // our own downstream checks be having preview also opt into "latest".
            options |= PInvokeGeneratorConfigurationOptions.GenerateLatestCode;
        }
        _options = options;

        if ((_options & PInvokeGeneratorConfigurationOptions.NoDefaultRemappings) == 0)
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

    public bool DontUseUsingStaticsForEnums => (_options & PInvokeGeneratorConfigurationOptions.DontUseUsingStaticsForEnums) != 0;

    public bool ExcludeAnonymousFieldHelpers => (_options & PInvokeGeneratorConfigurationOptions.ExcludeAnonymousFieldHelpers) != 0;

    public bool ExcludeComProxies => (_options & PInvokeGeneratorConfigurationOptions.ExcludeComProxies) != 0;

    public bool ExcludeEmptyRecords => (_options & PInvokeGeneratorConfigurationOptions.ExcludeEmptyRecords) != 0;

    public bool ExcludeEnumOperators => (_options & PInvokeGeneratorConfigurationOptions.ExcludeEnumOperators) != 0;

    public bool ExcludeFnptrCodegen
    {
        get
        {
            return GenerateCompatibleCode || (_options & PInvokeGeneratorConfigurationOptions.ExcludeFnptrCodegen) != 0;
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

    public bool ExcludeFunctionsWithBody => (_options & PInvokeGeneratorConfigurationOptions.ExcludeFunctionsWithBody) != 0;

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

    public bool ExcludeNIntCodegen => GenerateCompatibleCode || (_options & PInvokeGeneratorConfigurationOptions.ExcludeNIntCodegen) != 0;

    public bool GenerateAggressiveInlining => (_options & PInvokeGeneratorConfigurationOptions.GenerateAggressiveInlining) != 0;

    public bool GenerateCallConvMemberFunction => (_options & PInvokeGeneratorConfigurationOptions.GenerateCallConvMemberFunction) != 0;

    public bool GenerateCompatibleCode => (_options & PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode) != 0;

    public bool GenerateCppAttributes => (_options & PInvokeGeneratorConfigurationOptions.GenerateCppAttributes) != 0;

    public bool GenerateDisableRuntimeMarshalling => (_options & PInvokeGeneratorConfigurationOptions.GenerateDisableRuntimeMarshalling) != 0;

    public bool GenerateDocIncludes => (_options & PInvokeGeneratorConfigurationOptions.GenerateDocIncludes) != 0;

    public bool GenerateExplicitVtbls => (_options & PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls) != 0;

    public bool GenerateFileScopedNamespaces => (_options & PInvokeGeneratorConfigurationOptions.GenerateFileScopedNamespaces) != 0;

    public bool GenerateFixedBufferIndexerOverloads => (_options & PInvokeGeneratorConfigurationOptions.GenerateFixedBufferIndexerOverloads) != 0;

    public bool GenerateGenericPointerWrapper => (_options & PInvokeGeneratorConfigurationOptions.GenerateGenericPointerWrapper) != 0;

    public bool GenerateGuidMember => (_options & PInvokeGeneratorConfigurationOptions.GenerateGuidMember) != 0;

    public bool GenerateHelperTypes => (_options & PInvokeGeneratorConfigurationOptions.GenerateHelperTypes) != 0;

    public bool GenerateLatestCode => (_options & PInvokeGeneratorConfigurationOptions.GenerateLatestCode) != 0;

    public bool GenerateMacroBindings => (_options & PInvokeGeneratorConfigurationOptions.GenerateMacroBindings) != 0;

    public bool GenerateMarkerInterfaces => (_options & PInvokeGeneratorConfigurationOptions.GenerateMarkerInterfaces) != 0;

    public bool GenerateMultipleFiles => (_options & PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles) != 0;

    public bool GenerateNativeBitfieldAttribute => (_options & PInvokeGeneratorConfigurationOptions.GenerateNativeBitfieldAttribute) != 0;

    public bool GenerateNativeInheritanceAttribute => (_options & PInvokeGeneratorConfigurationOptions.GenerateNativeInheritanceAttribute) != 0;

    public bool GeneratePreviewCode => (_options & PInvokeGeneratorConfigurationOptions.GeneratePreviewCode) != 0;

    public bool GenerateSetsLastSystemErrorAttribute => (_options & PInvokeGeneratorConfigurationOptions.GenerateSetsLastSystemErrorAttribute) != 0;

    public bool GenerateSourceLocationAttribute => (_options & PInvokeGeneratorConfigurationOptions.GenerateSourceLocationAttribute) != 0;

    public bool GenerateTemplateBindings => (_options & PInvokeGeneratorConfigurationOptions.GenerateTemplateBindings) != 0;

    public bool GenerateTestsNUnit => (_options & PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit) != 0;

    public bool GenerateTestsXUnit => (_options & PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit) != 0;

    public bool GenerateTrimmableVtbls => (_options & PInvokeGeneratorConfigurationOptions.GenerateTrimmableVtbls) != 0;

    public bool GenerateUnixTypes => (_options & PInvokeGeneratorConfigurationOptions.GenerateUnixTypes) != 0;

    public bool GenerateUnmanagedConstants => (_options & PInvokeGeneratorConfigurationOptions.GenerateUnmanagedConstants) != 0;

    public bool GenerateVtblIndexAttribute => (_options & PInvokeGeneratorConfigurationOptions.GenerateVtblIndexAttribute) != 0;

    public bool StripEnumMemberTypeName => (_options & PInvokeGeneratorConfigurationOptions.StripEnumMemberTypeName) != 0;

    public bool DontUseUsingStaticsForGuidMember => (_options & PInvokeGeneratorConfigurationOptions.DontUseUsingStaticsForGuidMember) != 0;

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

    public bool LogExclusions => (_options & PInvokeGeneratorConfigurationOptions.LogExclusions) != 0;

    public bool LogPotentialTypedefRemappings => (_options & PInvokeGeneratorConfigurationOptions.LogPotentialTypedefRemappings) != 0;

    public bool LogVisitedFiles => (_options & PInvokeGeneratorConfigurationOptions.LogVisitedFiles) != 0;

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

    public IReadOnlyCollection<string> ForceRemappedNames => _forceRemappedNames;

    [AllowNull]
    public IReadOnlyDictionary<string, string> RemappedTypeNames
    {
        get
        {
            return _remappedTypeNames;
        }

        init
        {
            AddRange(_remappedTypeNames, value);
        }
    }

    [AllowNull]
    public IReadOnlyDictionary<string, string> RemappedFieldNames
    {
        get
        {
            return _remappedFieldNames;
        }

        init
        {
            AddRange(_remappedFieldNames, value);
        }
    }

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
    public IReadOnlyCollection<string> WithReadonlys
    {
        get
        {
            return _withReadonlys;
        }

        init
        {
            AddRange(_withReadonlys, value);
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

    private static void AddRange<TValue>(Dictionary<string, TValue> dictionary, IEnumerable<KeyValuePair<string, TValue>>? keyValuePairs)
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

    private static void AddRange<TInput, TValue>(Dictionary<string, TValue> dictionary, IEnumerable<KeyValuePair<string, TInput>>? keyValuePairs, Func<TInput, TValue> convert)
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

    private static void AddRange<TInput>(HashSet<TInput> hashSet, IEnumerable<TInput>? keys)
    {
        if (keys != null)
        {
            foreach (var key in keys)
            {
                _ = hashSet.Add(key);
            }
        }
    }

    private static void AddRange<TInput, TValue>(HashSet<TValue> hashSet, IEnumerable<TInput>? keys, Func<TInput, TValue> convert)
    {
        if (keys != null)
        {
            foreach (var key in keys)
            {
                _ = hashSet.Add(convert(key));
            }
        }
    }

    private static void AddRange<TInput>(HashSet<string> hashSet, IEnumerable<KeyValuePair<string, TInput>>? keyValuePairs, Func<TInput, bool> shouldAdd)
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
