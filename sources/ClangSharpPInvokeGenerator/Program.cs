// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXDiagnosticSeverity;
using static ClangSharp.Interop.CXErrorCode;
using static ClangSharp.Interop.CXTranslationUnit_Flags;
using static ClangSharp.PInvokeGeneratorConfigurationOptions;

namespace ClangSharp;

internal static partial class Program
{
    private const string Name = "ClangSharpPInvokeGenerator";
    private const string Description = "ClangSharp P/Invoke Binding Generator";

    // The clang release the tool targets, tracked via the assembly version (major.minor.build)
    // so it stays in sync with VersionPrefix and matches the clang/clangsharp lines below it.
    private static readonly string Version = GetVersion();

    private const string WildcardsTitle = "Wildcards:";
    private const string Wildcards = "Many name-matching options accept glob patterns using `*` (matches any run of characters, including qualification separators) and `?` (matches a single character); `::` and `.` are treated as equivalent separators and matching is case-sensitive. An exact match always wins over a glob, and among globs the most specific (the most literal characters) wins. Many `--with-*` options also accept a bare `*` as a catch-all that applies a rule to everything; for value options it is written `*=value` (e.g. --with-access-specifier *=Internal makes all generated code internal). Each such option has a paired `--without-<name>` option that opts a specific declaration (by exact name or glob) back out of its `*` catch-all (e.g. --with-access-specifier *=Internal --without-access-specifier Foo). To opt everything in and exclude piecemeal, use the include (-i) and exclude (-e) options together; they are already an opt-in/opt-out pair and likewise accept globs.";

    private const string MoreInfoTitle = "More information:";
    private const string MoreInfo = "See https://github.com/dotnet/ClangSharp/blob/main/docs/generating-bindings-best-practices.md for a guide on structuring a generation project and using these options.";

    // Canonical boolean `-c` switches. `Inverted` means the underlying flag records the *opposite*
    // of the switch's sense, so `generate-x=false` sets the flag (the legacy `exclude-*`/`dont-*`
    // behavior) and `generate-x=true` (or bare `generate-x`) clears it.
    private static readonly Dictionary<string, (PInvokeGeneratorConfigurationOptions Flag, bool Inverted)> s_booleanConfigSwitches = new(StringComparer.Ordinal)
    {
        ["generate-aggressive-inlining"] = (GenerateAggressiveInlining, false),
        ["generate-callconv-member-function"] = (GenerateCallConvMemberFunction, false),
        ["generate-cpp-attributes"] = (GenerateCppAttributes, false),
        ["generate-disable-runtime-marshalling"] = (GenerateDisableRuntimeMarshalling, false),
        ["generate-doc-includes"] = (GenerateDocIncludes, false),
        ["generate-extern-variables"] = (GenerateExternVariables, false),
        ["generate-file-scoped-namespaces"] = (GenerateFileScopedNamespaces, false),
        ["generate-fixed-buffer-indexer-overloads"] = (GenerateFixedBufferIndexerOverloads, false),
        ["generate-guid-member"] = (GenerateGuidMember, false),
        ["generate-helper-types"] = (GenerateHelperTypes, false),
        ["generate-macro-bindings"] = (GenerateMacroBindings, false),
        ["generate-marker-interfaces"] = (GenerateMarkerInterfaces, false),
        ["generate-native-alignment-attribute"] = (GenerateNativeAlignmentAttribute, false),
        ["generate-native-bitfield-attribute"] = (GenerateNativeBitfieldAttribute, false),
        ["generate-native-inheritance-attribute"] = (GenerateNativeInheritanceAttribute, false),
        ["generate-generic-pointer-wrapper"] = (GenerateGenericPointerWrapper, false),
        ["generate-objective-c-bindings"] = (GenerateObjectiveCBindings, false),
        ["generate-setslastsystemerror-attribute"] = (GenerateSetsLastSystemErrorAttribute, false),
        ["generate-template-bindings"] = (GenerateTemplateBindings, false),
        ["generate-unmanaged-constants"] = (GenerateUnmanagedConstants, false),
        ["generate-vtbl-index-attribute"] = (GenerateVtblIndexAttribute, false),

        // Inverted: the flag records an exclusion/opt-out, so `=false` opts the feature out.
        ["generate-anonymous-field-helpers"] = (ExcludeAnonymousFieldHelpers, true),
        ["generate-com-proxies"] = (ExcludeComProxies, true),
        ["generate-default-remappings"] = (NoDefaultRemappings, true),
        ["generate-empty-records"] = (ExcludeEmptyRecords, true),
        ["generate-enum-operators"] = (ExcludeEnumOperators, true),
        ["generate-enum-member-type-name"] = (StripEnumMemberTypeName, true),
        ["generate-fnptr-codegen"] = (ExcludeFnptrCodegen, true),
        ["generate-funcs-with-body"] = (ExcludeFunctionsWithBody, true),
        ["generate-nint-codegen"] = (ExcludeNIntCodegen, true),
        ["generate-using-statics-for-enums"] = (DontUseUsingStaticsForEnums, true),
        ["generate-using-statics-for-guid-members"] = (DontUseUsingStaticsForGuidMember, true),

        // Diagnostics; positive booleans that keep their `log-` prefix.
        ["log-exclusions"] = (LogExclusions, false),
        ["log-potential-typedef-remappings"] = (LogPotentialTypedefRemappings, false),
        ["log-visited-files"] = (LogVisitedFiles, false),
    };

    // Legacy `-c` spellings mapped to their canonical replacement. Still accepted, but emit a
    // deprecation warning and are hidden from the `-c help` listing.
    private static readonly Dictionary<string, string> s_deprecatedConfigSwitches = new(StringComparer.Ordinal)
    {
        ["compatible-codegen"] = "codegen=compatible",
        ["default-codegen"] = "codegen=default",
        ["latest-codegen"] = "codegen=latest",
        ["preview-codegen"] = "codegen=preview",
        ["single-file"] = "file=single",
        ["multi-file"] = "file=multi",
        ["windows-types"] = "types=windows",
        ["unix-types"] = "types=unix",
        ["explicit-vtbls"] = "vtbls=explicit",
        ["implicit-vtbls"] = "vtbls=implicit",
        ["trimmable-vtbls"] = "vtbls=trimmable",
        ["exclude-anonymous-field-helpers"] = "generate-anonymous-field-helpers=false",
        ["exclude-com-proxies"] = "generate-com-proxies=false",
        ["exclude-default-remappings"] = "generate-default-remappings=false",
        ["no-default-remappings"] = "generate-default-remappings=false",
        ["default-remappings"] = "generate-default-remappings=true",
        ["exclude-empty-records"] = "generate-empty-records=false",
        ["exclude-enum-operators"] = "generate-enum-operators=false",
        ["exclude-fnptr-codegen"] = "generate-fnptr-codegen=false",
        ["exclude-funcs-with-body"] = "generate-funcs-with-body=false",
        ["exclude-nint-codegen"] = "generate-nint-codegen=false",
        ["exclude-using-statics-for-enums"] = "generate-using-statics-for-enums=false",
        ["dont-use-using-statics-for-enums"] = "generate-using-statics-for-enums=false",
        ["exclude-using-statics-for-guid-members"] = "generate-using-statics-for-guid-members=false",
        ["dont-use-using-statics-for-guid-members"] = "generate-using-statics-for-guid-members=false",
        ["strip-enum-member-type-name"] = "generate-enum-member-type-name=false",
    };

    public static int Main(params string[] args)
    {
        s_parser.Parse(args);

        if (s_helpOption.IsPresent)
        {
            s_parser.WriteHelp(Console.Out, Name, Description, WildcardsTitle, Wildcards, MoreInfoTitle, MoreInfo);
            return 0;
        }

        if (s_versionOption.IsPresent)
        {
            Console.WriteLine($"{Description} version {Version}");
            Console.WriteLine($"  {clang.getClangVersion()}");
            Console.WriteLine($"  {clangsharp.getVersion()}");
            return 0;
        }

        return Run();
    }

    private static string GetVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        return (version is not null) ? $"{version.Major}.{version.Minor}.{version.Build}" : "";
    }

    public static int Run()
    {
        var errorList = new List<string>(s_parser.Errors);
        var warningList = new List<string>(s_parser.Warnings);

        var additionalArgs = s_additionalOption.GetValues();
        var configSwitches = s_configOption.GetValues();
        var generateSwitches = s_generateOption.GetValues();
        var logSwitches = s_logOption.GetValues();
        var defineMacros = s_defineMacros.GetValues();
        var excludedNames = s_excludedNames.GetValues();
        var files = s_files.GetValues();
        var fileDirectory = s_fileDirectory.SingleValue;
        var headerFile = s_headerFile.SingleValue;
        var includedNames = s_includedNames.GetValues();
        var includeDirectories = s_includeDirectories.GetValues();
        var language = s_language.SingleValue;
        var libraryPath = s_libraryPath.SingleValue;
        var methodClassName = s_methodClassName.SingleValue;
        var methodPrefixToStrip = s_methodPrefixToStrip.SingleValue;
        var typePrefixToStrip = s_typePrefixToStrip.SingleValue;
        var nativeTypeNamesToStrip = s_nativeTypeNamesToStrip.GetValues();
        var namespaceName = s_namespaceName.SingleValue;
        var outputLocation = s_outputLocation.SingleValue;
        var remappedNameValuePairs = s_remappedNameValuePairs.GetValues();
        var remappedTypeNameValuePairs = s_remappedTypeNameValuePairs.GetValues();
        var remappedFieldNameValuePairs = s_remappedFieldNameValuePairs.GetValues();
        var std = s_std.SingleValue;
        var testOutputLocation = s_testOutputLocation.SingleValue;
        var traversalNames = s_traversalNames.GetValues();
        var withConditional = s_withConditional.SingleValue;
        var withAccessSpecifierNameValuePairs = s_withAccessSpecifierNameValuePairs.GetValues();
        var withAttributeNameValuePairs = s_withAttributeNameValuePairs.GetValues();
        var withBaseNameValuePairs = s_withBaseNameValuePairs.GetValues();
        var withCallConvNameValuePairs = s_withCallConvNameValuePairs.GetValues();
        var withClassNameValuePairs = s_withClassNameValuePairs.GetValues();
        var withConstantFoldedValues = s_withConstantFoldedValues.GetValues();
        var withEnumMemberStripNameValuePairs = s_withEnumMemberStripNameValuePairs.GetValues();
        var withEqualityMembers = s_withEqualityMembers.GetValues();
        var withGuidNameValuePairs = s_withGuidNameValuePairs.GetValues();
        var withLengthNameValuePairs = s_withLengthNameValuePairs.GetValues();
        var withLibraryPathNameValuePairs = s_withLibraryPathNameValuePairs.GetValues();
        var withManualImports = s_withManualImports.GetValues();
        var withNamespaceNameValuePairs = s_withNamespaceNameValuePairs.GetValues();
        var withReadonlys = s_withReadonlys.GetValues();
        var withSetLastErrors = s_withSetLastErrors.GetValues();
        var withSuppressGCTransitions = s_withSuppressGCTransitions.GetValues();
        var withTransparentStructNameValuePairs = s_withTransparentStructNameValuePairs.GetValues();
        var withTypeNameValuePairs = s_withTypeNameValuePairs.GetValues();
        var withUsingNameValuePairs = s_withUsingNameValuePairs.GetValues();
        var withPackingNameValuePairs = s_withPackingNameValuePairs.GetValues();
        var withoutAccessSpecifiers = s_withoutAccessSpecifiers.GetValues();
        var withoutAttributes = s_withoutAttributes.GetValues();
        var withoutCallConvs = s_withoutCallConvs.GetValues();
        var withoutConstantFoldedValues = s_withoutConstantFoldedValues.GetValues();
        var withoutEnumMemberStrip = s_withoutEnumMemberStrip.GetValues();
        var withoutEqualityMembers = s_withoutEqualityMembers.GetValues();
        var withoutLibraryPaths = s_withoutLibraryPaths.GetValues();
        var withoutReadonlys = s_withoutReadonlys.GetValues();
        var withoutSetLastErrors = s_withoutSetLastErrors.GetValues();
        var withoutSuppressGCTransitions = s_withoutSuppressGCTransitions.GetValues();
        var withoutTypes = s_withoutTypes.GetValues();
        var withoutUsings = s_withoutUsings.GetValues();

        if (!Enum.TryParse<PInvokeGeneratorOutputMode>(s_outputMode.SingleValue, ignoreCase: true, out var outputMode))
        {
            errorList.Add($"Error: Unrecognized output mode: {s_outputMode.SingleValue}. Must be one of CSharp or Xml");
        }

        if (files.Length == 0)
        {
            errorList.Add("Error: No input C/C++ files provided. Use --file or -f");
        }

        if (string.IsNullOrWhiteSpace(namespaceName))
        {
            errorList.Add("Error: No namespace provided. Use --namespace or -n");
        }

        if (string.IsNullOrWhiteSpace(outputLocation))
        {
            errorList.Add("Error: No output file location provided. Use --output or -o");
        }

        ParseKeyValuePairs(remappedNameValuePairs, errorList, out Dictionary<string, string> remappedNames);
        ParseKeyValuePairs(remappedTypeNameValuePairs, errorList, out Dictionary<string, string> remappedTypeNames);
        ParseKeyValuePairs(remappedFieldNameValuePairs, errorList, out Dictionary<string, string> remappedFieldNames);
        ParseKeyValuePairs(withAccessSpecifierNameValuePairs, errorList, out Dictionary<string, AccessSpecifier> withAccessSpecifiers);
        ParseKeyValuePairs(withAttributeNameValuePairs, errorList, out Dictionary<string, IReadOnlyList<string>> withAttributes);
        ParseKeyValuePairs(withBaseNameValuePairs, errorList, out Dictionary<string, IReadOnlyList<string>> withBases);
        ParseKeyValuePairs(withCallConvNameValuePairs, errorList, out Dictionary<string, string> withCallConvs);
        ParseKeyValuePairs(withClassNameValuePairs, errorList, out Dictionary<string, string> withClasses);
        ParseKeyValuePairs(withEnumMemberStripNameValuePairs, errorList, out Dictionary<string, string> withEnumMemberStrip);
        ParseKeyValuePairs(withGuidNameValuePairs, errorList, out Dictionary<string, Guid> withGuids);
        ParseKeyValuePairs(withLengthNameValuePairs, errorList, out Dictionary<string, string> withLengths);
        ParseKeyValuePairs(withLibraryPathNameValuePairs, errorList, out Dictionary<string, string> withLibraryPaths);
        ParseKeyValuePairs(withNamespaceNameValuePairs, errorList, out Dictionary<string, string> withNamespaces);
        ParseKeyValuePairs(withTransparentStructNameValuePairs, errorList, out Dictionary<string, (string, PInvokeGeneratorTransparentStructKind)> withTransparentStructs);
        ParseKeyValuePairs(withTypeNameValuePairs, errorList, out Dictionary<string, string> withTypes);
        ParseKeyValuePairs(withUsingNameValuePairs, errorList, out Dictionary<string, IReadOnlyList<string>> withUsings);
        ParseKeyValuePairs(withPackingNameValuePairs, errorList, out Dictionary<string, string> withPackings);

        foreach (var key in withTransparentStructs.Keys)
        {
            remappedNames.Add(key, key);
        }

        var configOptions = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? PInvokeGeneratorConfigurationOptions.None : PInvokeGeneratorConfigurationOptions.GenerateUnixTypes;
        var printConfigHelp = false;

        // `--generate` and `--log` are the canonical homes for the feature and diagnostic switches;
        // the name is the `-c generate-*`/`log-*` token with its prefix stripped.
        foreach (var generateSwitchRaw in generateSwitches)
        {
            var (name, value, hasValue) = SplitConfigSwitch(generateSwitchRaw);
            configOptions = ApplyFullConfigSwitch($"generate-{name}", value, configOptions, testOutputLocation, errorList, warningList);
        }

        foreach (var logSwitchRaw in logSwitches)
        {
            var (name, value, hasValue) = SplitConfigSwitch(logSwitchRaw);
            configOptions = ApplyFullConfigSwitch($"log-{name}", value, configOptions, testOutputLocation, errorList, warningList);
        }

        // `-c` now only carries the four mode families (codegen/file/types/vtbls) plus `help`. The
        // old feature/diagnostic and legacy spellings are still accepted, but route to the dedicated
        // `--generate`/`--log` options with a deprecation warning.
        foreach (var configSwitchRaw in configSwitches)
        {
            var (configSwitch, configSwitchValue, hasValue) = SplitConfigSwitch(configSwitchRaw);

            switch (configSwitch)
            {
                case "?":
                case "h":
                case "help":
                {
                    printConfigHelp = true;
                    break;
                }

                case "codegen":
                case "file":
                case "types":
                case "vtbls":
                {
                    configOptions = ApplyFullConfigSwitch(configSwitch, configSwitchValue, configOptions, testOutputLocation, errorList, warningList);
                    break;
                }

                default:
                {
                    // Resolve any legacy spelling so we can classify the canonical target and point
                    // the user at the new option in a single warning.
                    var canonicalSwitch = configSwitch;
                    var canonicalValue = configSwitchValue;

                    if (s_deprecatedConfigSwitches.TryGetValue(configSwitch, out var canonical))
                    {
                        var canonicalSeparator = canonical.IndexOf('=', StringComparison.Ordinal);
                        canonicalSwitch = canonicalSeparator >= 0 ? canonical[..canonicalSeparator] : canonical;
                        canonicalValue = canonicalSeparator >= 0 ? canonical[(canonicalSeparator + 1)..] : "";
                        hasValue = canonicalSeparator >= 0;
                    }

                    if (canonicalSwitch.StartsWith("generate-", StringComparison.Ordinal))
                    {
                        var name = canonicalSwitch["generate-".Length..];
                        var suggestion = hasValue ? $"--generate {name}={canonicalValue}" : $"--generate {name}";
                        warningList.Add($"Warning: Config option '-c {configSwitch}' is deprecated and will be removed in a future release. Use '{suggestion}' instead.");
                        configOptions = ApplyFullConfigSwitch(canonicalSwitch, canonicalValue, configOptions, testOutputLocation, errorList, warningList);
                    }
                    else if (canonicalSwitch.StartsWith("log-", StringComparison.Ordinal))
                    {
                        var name = canonicalSwitch["log-".Length..];
                        var suggestion = hasValue ? $"--log {name}={canonicalValue}" : $"--log {name}";
                        warningList.Add($"Warning: Config option '-c {configSwitch}' is deprecated and will be removed in a future release. Use '{suggestion}' instead.");
                        configOptions = ApplyFullConfigSwitch(canonicalSwitch, canonicalValue, configOptions, testOutputLocation, errorList, warningList);
                    }
                    else
                    {
                        // A canonical/legacy mode family (or an unrecognized switch); ApplyConfigSwitch
                        // reports the legacy-spelling warning or the unrecognized-switch error.
                        configOptions = ApplyFullConfigSwitch(configSwitch, configSwitchValue, configOptions, testOutputLocation, errorList, warningList);
                    }
                    break;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(testOutputLocation) && (configOptions & PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit) == 0 && (configOptions & PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit) == 0)
        {
            errorList.Add("Error: No test format provided. Use --config generate-tests-nunit or --config generate-tests-xunit");
        }

        if (printConfigHelp)
        {
            CommandLineParser.WriteOptionHelp(Console.Out, s_configOption, s_configOptions);
            return -1;
        }

        foreach (var warning in warningList)
        {
            Console.Error.Write(warning);
            Console.Error.Write(Environment.NewLine);
        }

        if (errorList.Count != 0)
        {
            Console.Error.Write($"Error in args for '{files.FirstOrDefault()}'");
            Console.Error.Write(Environment.NewLine);

            foreach (var error in errorList)
            {
                Console.Error.Write(error);
                Console.Error.Write(Environment.NewLine);
            }
            Console.Error.Write(Environment.NewLine);

            s_parser.WriteHelp(Console.Out, Name, Description, WildcardsTitle, Wildcards, MoreInfoTitle, MoreInfo);
            return -1;
        }

        var clangCommandLineArgs = string.IsNullOrWhiteSpace(std)
                                 ? new string[] {
                                     $"--language={language}",               // Treat subsequent input files as having type <language>
                                     "-Wno-pragma-once-outside-header"       // We are processing files which may be header files
                                 } : [
                                     $"--language={language}",               // Treat subsequent input files as having type <language>
                                     $"--std={std}",                         // Language standard to compile for
                                     "-Wno-pragma-once-outside-header"       // We are processing files which may be header files
                                 ];

        clangCommandLineArgs = [.. clangCommandLineArgs, .. includeDirectories.Select(x => $"--include-directory={x}")];
        clangCommandLineArgs = [.. clangCommandLineArgs, .. defineMacros.Select(x => $"--define-macro={x}")];
        clangCommandLineArgs = [.. clangCommandLineArgs, .. additionalArgs];
        clangCommandLineArgs = AddResourceDirectory(clangCommandLineArgs, additionalArgs);

        var translationFlags = CXTranslationUnit_None;

        translationFlags |= CXTranslationUnit_IncludeAttributedTypes;               // Include attributed types in CXType
        translationFlags |= CXTranslationUnit_VisitImplicitAttributes;              // Implicit attributes should be visited

        PInvokeGeneratorConfiguration config;

        try
        {
            config = new PInvokeGeneratorConfiguration(language, std, namespaceName, outputLocation, headerFile, outputMode, configOptions) {
                DefaultClass = methodClassName,
                ExcludedNames = excludedNames,
                IncludedNames = includedNames,
                LibraryPath = libraryPath,
                MethodPrefixToStrip = methodPrefixToStrip,
                TypePrefixToStrip = typePrefixToStrip,
                NativeTypeNamesToStrip = nativeTypeNamesToStrip,
                RemappedNames = remappedNames,
                RemappedTypeNames = remappedTypeNames,
                RemappedFieldNames = remappedFieldNames,
                TraversalNames = traversalNames,
                TestOutputLocation = testOutputLocation,
                WithAccessSpecifiers = withAccessSpecifiers,
                WithAttributes = withAttributes,
                WithBases = withBases,
                WithCallConvs = withCallConvs,
                WithClasses = withClasses,
                WithConditional = withConditional,
                WithConstantFoldedValues = withConstantFoldedValues,
                WithEnumMemberStrip = withEnumMemberStrip,
                WithEqualityMembers = withEqualityMembers,
                WithGuids = withGuids,
                WithLengths = withLengths,
                WithLibraryPaths = withLibraryPaths,
                WithManualImports = withManualImports,
                WithNamespaces = withNamespaces,
                WithReadonlys = withReadonlys,
                WithSetLastErrors = withSetLastErrors,
                WithSuppressGCTransitions = withSuppressGCTransitions,
                WithTransparentStructs = withTransparentStructs,
                WithTypes = withTypes,
                WithUsings = withUsings,
                WithPackings = withPackings,
                WithoutAccessSpecifiers = withoutAccessSpecifiers,
                WithoutAttributes = withoutAttributes,
                WithoutCallConvs = withoutCallConvs,
                WithoutConstantFoldedValues = withoutConstantFoldedValues,
                WithoutEnumMemberStrip = withoutEnumMemberStrip,
                WithoutEqualityMembers = withoutEqualityMembers,
                WithoutLibraryPaths = withoutLibraryPaths,
                WithoutReadonlys = withoutReadonlys,
                WithoutSetLastErrors = withoutSetLastErrors,
                WithoutSuppressGCTransitions = withoutSuppressGCTransitions,
                WithoutTypes = withoutTypes,
                WithoutUsings = withoutUsings,
            };
        }
        catch (ArgumentException e)
        {
            Console.Error.Write($"Error: {e.Message}");
            Console.Error.Write(Environment.NewLine);
            return -1;
        }

        if (config.GenerateMacroBindings)
        {
            translationFlags |= CXTranslationUnit_DetailedPreprocessingRecord;
        }

        var exitCode = 0;

        using (var pinvokeGenerator = new PInvokeGenerator(config))
        {
            foreach (var file in files)
            {
                var filePath = Path.Combine(fileDirectory, file);

                var translationUnitError = CXTranslationUnit.TryParse(pinvokeGenerator.IndexHandle, filePath, clangCommandLineArgs, [], translationFlags, out var handle);
                var skipProcessing = false;

                if (translationUnitError != CXError_Success)
                {
                    Console.WriteLine($"Error: Parsing failed for '{filePath}' due to '{translationUnitError}'.");
                    skipProcessing = true;
                }
                else if (handle.NumDiagnostics != 0)
                {
                    Console.WriteLine($"Diagnostics for '{filePath}':");

                    for (uint i = 0; i < handle.NumDiagnostics; ++i)
                    {
                        using var diagnostic = handle.GetDiagnostic(i);

                        Console.Write("    ");
                        Console.WriteLine(diagnostic.Format(CXDiagnostic.DefaultDisplayOptions).ToString());

                        skipProcessing |= diagnostic.Severity == CXDiagnostic_Error;
                        skipProcessing |= diagnostic.Severity == CXDiagnostic_Fatal;
                    }
                }

                if (skipProcessing)
                {
                    Console.WriteLine($"Skipping '{filePath}' due to one or more errors listed above.");
                    Console.WriteLine("");

                    exitCode = -1;
                    continue;
                }

#pragma warning disable CA1031 // Do not catch general exception types

                try
                {
                    using var translationUnit = TranslationUnit.GetOrCreate(handle);
                    Debug.Assert(translationUnit is not null);

                    Console.WriteLine($"Processing '{filePath}'");
                    pinvokeGenerator.GenerateBindings(translationUnit, filePath, clangCommandLineArgs, translationFlags);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

#pragma warning restore CA1031 // Do not catch general exception types
            }

            if (pinvokeGenerator.Diagnostics.Count != 0)
            {
                Console.WriteLine($"Diagnostics for binding generation of {pinvokeGenerator.FilePath}:");

                foreach (var diagnostic in pinvokeGenerator.Diagnostics)
                {
                    Console.Write("    ");
                    Console.WriteLine(diagnostic.ToString());

                    if (diagnostic.Level == DiagnosticLevel.Warning)
                    {
                        if (exitCode >= 0)
                        {
                            exitCode++;
                        }
                    }
                    else if (diagnostic.Level == DiagnosticLevel.Error)
                    {
                        if (exitCode >= 0)
                        {
                            exitCode = -1;
                        }
                        else
                        {
                            exitCode--;
                        }
                    }
                }
            }
        }

        return exitCode;
    }

    // Splits a `name` or `name=value` switch token. HasValue distinguishes a bare switch from one
    // that carried an explicit value, which callers use when suggesting the replacement spelling.
    private static (string Name, string Value, bool HasValue) SplitConfigSwitch(string raw)
    {
        var separatorIndex = raw.IndexOf('=', StringComparison.Ordinal);
        return separatorIndex >= 0
            ? (raw[..separatorIndex], raw[(separatorIndex + 1)..], true)
            : (raw, "", false);
    }

    // Applies a `-c`/`--generate`/`--log` switch, handling the two test-generation switches that
    // need the test output location and delegating everything else to ApplyConfigSwitch.
    internal static PInvokeGeneratorConfigurationOptions ApplyFullConfigSwitch(string configSwitch, string configSwitchValue, PInvokeGeneratorConfigurationOptions configOptions, string? testOutputLocation, List<string> errorList, List<string> warningList)
    {
        switch (configSwitch)
        {
            case "generate-tests-nunit":
            {
                if (!TryParseConfigBool(configSwitch, configSwitchValue, errorList, out var enable))
                {
                    return configOptions;
                }

                if (!enable)
                {
                    return configOptions & ~GenerateTestsNUnit;
                }

                if (string.IsNullOrWhiteSpace(testOutputLocation))
                {
                    errorList.Add("Error: No test output file location provided. Use --test-output or -to");
                }

                if ((configOptions & GenerateTestsXUnit) != 0)
                {
                    errorList.Add("Cannot generate both NUnit and XUnit tests.");
                }

                return configOptions | GenerateTestsNUnit;
            }

            case "generate-tests-xunit":
            {
                if (!TryParseConfigBool(configSwitch, configSwitchValue, errorList, out var enable))
                {
                    return configOptions;
                }

                if (!enable)
                {
                    return configOptions & ~GenerateTestsXUnit;
                }

                if (string.IsNullOrWhiteSpace(testOutputLocation))
                {
                    errorList.Add("Error: No test output file location provided. Use --test-output or -to");
                }

                if ((configOptions & GenerateTestsNUnit) != 0)
                {
                    errorList.Add("Cannot generate both NUnit and XUnit tests.");
                }

                return configOptions | GenerateTestsXUnit;
            }

            default:
            {
                return ApplyConfigSwitch(configSwitch, configSwitchValue, configOptions, errorList, warningList);
            }
        }
    }

    // Applies a single non-test, non-help `-c` switch, resolving deprecated spellings to their
    // canonical form first. Returns the updated options; recognized-but-invalid input is reported
    // via errorList and leaves the options otherwise unchanged.
    internal static PInvokeGeneratorConfigurationOptions ApplyConfigSwitch(string configSwitch, string configSwitchValue, PInvokeGeneratorConfigurationOptions configOptions, List<string> errorList, List<string> warningList)
    {
        if (s_deprecatedConfigSwitches.TryGetValue(configSwitch, out var canonical))
        {
            warningList.Add($"Warning: Config option '{configSwitch}' is deprecated and will be removed in a future release. Use '{canonical}' instead.");

            var canonicalSeparator = canonical.IndexOf('=', StringComparison.Ordinal);
            configSwitch = canonicalSeparator >= 0 ? canonical[..canonicalSeparator] : canonical;
            configSwitchValue = canonicalSeparator >= 0 ? canonical[(canonicalSeparator + 1)..] : "";
        }

        switch (configSwitch)
        {
            case "codegen":
            {
                switch (configSwitchValue)
                {
                    case "compatible":
                    {
                        configOptions |= GenerateCompatibleCode;
                        configOptions &= ~(GenerateLatestCode | GeneratePreviewCode);
                        break;
                    }

                    case "" or "default":
                    {
                        configOptions &= ~(GenerateCompatibleCode | GenerateLatestCode | GeneratePreviewCode);
                        break;
                    }

                    case "latest":
                    {
                        configOptions |= GenerateLatestCode;
                        configOptions &= ~(GenerateCompatibleCode | GeneratePreviewCode);
                        break;
                    }

                    case "preview":
                    {
                        configOptions |= GeneratePreviewCode;
                        configOptions &= ~(GenerateCompatibleCode | GenerateLatestCode);
                        break;
                    }

                    default:
                    {
                        errorList.Add($"Error: Unrecognized codegen value: {configSwitchValue}. Expected 'compatible', 'default', 'latest', or 'preview'.");
                        break;
                    }
                }
                return configOptions;
            }

            case "file":
            {
                switch (configSwitchValue)
                {
                    case "" or "single":
                    {
                        configOptions &= ~GenerateMultipleFiles;
                        break;
                    }

                    case "multi":
                    {
                        configOptions |= GenerateMultipleFiles;
                        break;
                    }

                    default:
                    {
                        errorList.Add($"Error: Unrecognized file value: {configSwitchValue}. Expected 'single' or 'multi'.");
                        break;
                    }
                }
                return configOptions;
            }

            case "types":
            {
                switch (configSwitchValue)
                {
                    case "" or "windows":
                    {
                        configOptions &= ~GenerateUnixTypes;
                        break;
                    }

                    case "unix":
                    {
                        configOptions |= GenerateUnixTypes;
                        break;
                    }

                    default:
                    {
                        errorList.Add($"Error: Unrecognized types value: {configSwitchValue}. Expected 'windows' or 'unix'.");
                        break;
                    }
                }
                return configOptions;
            }

            case "vtbls":
            {
                switch (configSwitchValue)
                {
                    case "explicit":
                    {
                        configOptions |= GenerateExplicitVtbls;
                        configOptions &= ~GenerateTrimmableVtbls;
                        break;
                    }

                    case "" or "implicit":
                    {
                        configOptions &= ~(GenerateExplicitVtbls | GenerateTrimmableVtbls);
                        break;
                    }

                    case "trimmable":
                    {
                        configOptions |= GenerateTrimmableVtbls;
                        configOptions &= ~GenerateExplicitVtbls;
                        break;
                    }

                    default:
                    {
                        errorList.Add($"Error: Unrecognized vtbls value: {configSwitchValue}. Expected 'explicit', 'implicit', or 'trimmable'.");
                        break;
                    }
                }
                return configOptions;
            }

            case "generate-generated-code":
            {
                switch (configSwitchValue)
                {
                    case "" or "assembly":
                    {
                        configOptions &= ~(GenerateGeneratedCodeAttributeAsType | ExcludeGeneratedCodeAttribute);
                        break;
                    }

                    case "type":
                    {
                        configOptions |= GenerateGeneratedCodeAttributeAsType;
                        configOptions &= ~ExcludeGeneratedCodeAttribute;
                        break;
                    }

                    case "none":
                    {
                        configOptions |= ExcludeGeneratedCodeAttribute;
                        configOptions &= ~GenerateGeneratedCodeAttributeAsType;
                        break;
                    }

                    default:
                    {
                        errorList.Add($"Error: Unrecognized generate-generated-code value: {configSwitchValue}. Expected 'assembly', 'type', or 'none'.");
                        break;
                    }
                }
                return configOptions;
            }

            default:
            {
                if (s_booleanConfigSwitches.TryGetValue(configSwitch, out var boolSwitch))
                {
                    if (TryParseConfigBool(configSwitch, configSwitchValue, errorList, out var enable))
                    {
                        // `Inverted` flags record an opt-out, so enabling the feature clears them.
                        if (boolSwitch.Inverted ? !enable : enable)
                        {
                            configOptions |= boolSwitch.Flag;
                        }
                        else
                        {
                            configOptions &= ~boolSwitch.Flag;
                        }
                    }
                }
                else
                {
                    errorList.Add($"Error: Unrecognized config switch: {configSwitch}.");
                }
                return configOptions;
            }
        }
    }

    private static bool TryParseConfigBool(string configSwitch, string configSwitchValue, List<string> errorList, out bool result)
    {
        switch (configSwitchValue)
        {
            case "" or "true":
            {
                result = true;
                return true;
            }

            case "false":
            {
                result = false;
                return true;
            }

            default:
            {
                errorList.Add($"Error: Unrecognized value '{configSwitchValue}' for config switch '{configSwitch}'. Expected 'true' or 'false'.");
                result = false;
                return false;
            }
        }
    }


    private static void ParseKeyValuePairs(IEnumerable<string> keyValuePairs, List<string> errorList, out Dictionary<string, string> result)
    {
        result = [];

        foreach (var keyValuePair in keyValuePairs)
        {
            var parts = keyValuePair.Split('=', 2);

            if (parts.Length < 2)
            {
                errorList.Add($"Error: Invalid key/value pair argument: {keyValuePair}. Expected 'name=value'");
                continue;
            }

            var key = parts[0].TrimEnd();

            if (result.TryGetValue(key, out var value))
            {
                errorList.Add($"Error: A key with the given name already exists: {key}. Existing: {value}");
                continue;
            }

            result.Add(key, parts[1].TrimStart());
        }
    }

    private static void ParseKeyValuePairs(IEnumerable<string> keyValuePairs, List<string> errorList, out Dictionary<string, AccessSpecifier> result)
    {
        result = [];

        foreach (var keyValuePair in keyValuePairs)
        {
            var parts = keyValuePair.Split('=');

            if (parts.Length != 2)
            {
                errorList.Add($"Error: Invalid key/value pair argument: {keyValuePair}. Expected 'name=value'");
                continue;
            }

            var key = parts[0].TrimEnd();

            if (result.TryGetValue(key, out var value))
            {
                errorList.Add($"Error: A key with the given name already exists: {key}. Existing: {value}");
                continue;
            }

            result.Add(key, PInvokeGeneratorConfiguration.ConvertStringToAccessSpecifier(parts[1].TrimStart()));
        }
    }

    private static void ParseKeyValuePairs(IEnumerable<string> keyValuePairs, List<string> errorList, out Dictionary<string, Guid> result)
    {
        result = [];

        foreach (var keyValuePair in keyValuePairs)
        {
            var parts = keyValuePair.Split('=');

            if (parts.Length != 2)
            {
                errorList.Add($"Error: Invalid key/value pair argument: {keyValuePair}. Expected 'name=value'");
                continue;
            }

            var key = parts[0].TrimEnd();

            if (result.TryGetValue(key, out var value))
            {
                errorList.Add($"Error: A key with the given name already exists: {key}. Existing: {value}");
                continue;
            }

            if (!Guid.TryParse(parts[1].TrimStart(), out var guid))
            {
                errorList.Add($"Error: Failed to parse guid: {parts[1]}");
                continue;
            }

            result.Add(key, guid);
        }
    }

    private static void ParseKeyValuePairs(IEnumerable<string> keyValuePairs, List<string> errorList, out Dictionary<string, (string, PInvokeGeneratorTransparentStructKind)> result)
    {
        result = [];

        foreach (var keyValuePair in keyValuePairs)
        {
            var parts = keyValuePair.Split('=');

            if (parts.Length != 2)
            {
                errorList.Add($"Error: Invalid key/value pair argument: {keyValuePair}. Expected 'name=value' or 'name=value;kind'");
                continue;
            }

            var key = parts[0].TrimEnd();

            if (result.TryGetValue(key, out var value))
            {
                errorList.Add($"Error: A key with the given name already exists: {key}. Existing: {value}");
                continue;
            }

            parts = parts[1].Split(';');

            if (parts.Length == 1)
            {
                result.Add(key, (parts[0], PInvokeGeneratorTransparentStructKind.Unknown));
            }
            else if ((parts.Length == 2) && Enum.TryParse<PInvokeGeneratorTransparentStructKind>(parts[1], out var transparentStructKind))
            {
                result.Add(key, (parts[0], transparentStructKind));
            }
            else
            {
                errorList.Add($"Error: Invalid key/value pair argument: {keyValuePair}. Expected 'name=value' or 'name=value;kind'");
                continue;
            }
        }
    }

    private static void ParseKeyValuePairs(IEnumerable<string> keyValuePairs, List<string> errorList, out Dictionary<string, IReadOnlyList<string>> result)
    {
        result = [];

        foreach (var keyValuePair in keyValuePairs)
        {
            var parts = keyValuePair.Split('=');

            if (parts.Length != 2)
            {
                errorList.Add($"Error: Invalid key/value pair argument: {keyValuePair}. Expected 'name=value'");
                continue;
            }

            var key = parts[0].TrimEnd();

            if (!result.TryGetValue(key, out var value))
            {
                value = new List<string>();
                result.Add(key, value);
            }

            var list = (List<string>)value;
            list.Add(parts[1].TrimStart());
        }
    }
}
