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

namespace ClangSharp;

internal static partial class Program
{
    private const string Name = "ClangSharpPInvokeGenerator";
    private const string Description = "ClangSharp P/Invoke Binding Generator";

    // The clang release the tool targets, tracked via the assembly version (major.minor.build)
    // so it stays in sync with VersionPrefix and matches the clang/clangsharp lines below it.
    private static readonly string Version = GetVersion();

    private const string WildcardsTitle = "Wildcards:";
    private const string Wildcards = "You can use * as catch-all rule for remapping procedures. For example if you want make all of your generated code internal you can use --with-access-specifier *=Internal.";

    private const string MoreInfoTitle = "More information:";
    private const string MoreInfo = "See https://github.com/dotnet/ClangSharp/blob/main/docs/generating-bindings-best-practices.md for a guide on structuring a generation project and using these options.";

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

        var additionalArgs = s_additionalOption.GetValues();
        var configSwitches = s_configOption.GetValues();
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
        var withCallConvNameValuePairs = s_withCallConvNameValuePairs.GetValues();
        var withClassNameValuePairs = s_withClassNameValuePairs.GetValues();
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

        foreach (var configSwitchRaw in configSwitches)
        {
            var separatorIndex = configSwitchRaw.IndexOf('=', StringComparison.Ordinal);
            var configSwitch = separatorIndex >= 0 ? configSwitchRaw[..separatorIndex] : configSwitchRaw;
            var configSwitchValue = separatorIndex >= 0 ? configSwitchRaw[(separatorIndex + 1)..] : "";

            switch (configSwitch)
            {
                case "?":
                case "h":
                case "help":
                {
                    printConfigHelp = true;
                    break;
                }

                // Codegen Options

                case "compatible-codegen":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode;
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateLatestCode;
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GeneratePreviewCode;
                    break;
                }

                case "default-codegen":
                {
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode;
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateLatestCode;
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GeneratePreviewCode;
                    break;
                }

                case "latest-codegen":
                {
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode;
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateLatestCode;
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GeneratePreviewCode;
                    break;
                }

                case "preview-codegen":
                {
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode;
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateLatestCode;
                    configOptions |= PInvokeGeneratorConfigurationOptions.GeneratePreviewCode;
                    break;
                }

                // File Options

                case "single-file":
                {
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles;
                    break;
                }

                case "multi-file":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles;
                    break;
                }

                // Type Options

                case "unix-types":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateUnixTypes;
                    break;
                }

                case "windows-types":
                {
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateUnixTypes;
                    break;
                }

                // Exclusion Options

                case "exclude-anonymous-field-helpers":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.ExcludeAnonymousFieldHelpers;
                    break;
                }

                case "exclude-com-proxies":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.ExcludeComProxies;
                    break;
                }

                case "exclude-default-remappings":
                case "no-default-remappings":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.NoDefaultRemappings;
                    break;
                }

                case "exclude-empty-records":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.ExcludeEmptyRecords;
                    break;
                }

                case "exclude-enum-operators":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.ExcludeEnumOperators;
                    break;
                }

                case "exclude-fnptr-codegen":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.ExcludeFnptrCodegen;
                    break;
                }

                case "exclude-funcs-with-body":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.ExcludeFunctionsWithBody;
                    break;
                }

                case "exclude-nint-codegen":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.ExcludeNIntCodegen;
                    break;
                }

                case "exclude-using-statics-for-enums":
                case "dont-use-using-statics-for-enums":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.DontUseUsingStaticsForEnums;
                    break;
                }

                case "exclude-using-statics-for-guid-members":
                case "dont-use-using-statics-for-guid-members":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.DontUseUsingStaticsForGuidMember;
                    break;
                }

                // VTBL Options

                case "explicit-vtbls":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls;
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateTrimmableVtbls;
                    break;
                }

                case "implicit-vtbls":
                {
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls;
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateTrimmableVtbls;
                    break;
                }

                case "trimmable-vtbls":
                {
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls;
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateTrimmableVtbls;
                    break;
                }

                // Test Options

                case "generate-tests-nunit":
                {
                    if (string.IsNullOrWhiteSpace(testOutputLocation))
                    {
                        errorList.Add("Error: No test output file location provided. Use --test-output or -to");
                    }

                    if ((configOptions & PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit) != 0)
                    {
                        errorList.Add("Cannot generate both NUnit and XUnit tests.");
                    }
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit;
                    break;
                }

                case "generate-tests-xunit":
                {
                    if (string.IsNullOrWhiteSpace(testOutputLocation))
                    {
                        errorList.Add("Error: No test output file location provided. Use --test-output or -to");
                    }

                    if ((configOptions & PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit) != 0)
                    {
                        errorList.Add("Cannot generate both NUnit and XUnit tests.");
                    }
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit;
                    break;
                }

                // Generation Options

                case "generate-aggressive-inlining":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateAggressiveInlining;
                    break;
                }

                case "generate-callconv-member-function":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateCallConvMemberFunction;
                    break;
                }

                case "generate-cpp-attributes":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateCppAttributes;
                    break;
                }

                case "generate-disable-runtime-marshalling":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateDisableRuntimeMarshalling;
                    break;
                }

                case "generate-doc-includes":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateDocIncludes;
                    break;
                }

                case "generate-file-scoped-namespaces":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateFileScopedNamespaces;
                    break;
                }

                case "generate-fixed-buffer-indexer-overloads":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateFixedBufferIndexerOverloads;
                    break;
                }

                case "generate-generated-code":
                {
                    switch (configSwitchValue)
                    {
                        case "" or "assembly":
                        {
                            configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateGeneratedCodeAttributeAsType;
                            configOptions &= ~PInvokeGeneratorConfigurationOptions.ExcludeGeneratedCodeAttribute;
                            break;
                        }

                        case "type":
                        {
                            configOptions |= PInvokeGeneratorConfigurationOptions.GenerateGeneratedCodeAttributeAsType;
                            configOptions &= ~PInvokeGeneratorConfigurationOptions.ExcludeGeneratedCodeAttribute;
                            break;
                        }

                        case "none":
                        {
                            configOptions |= PInvokeGeneratorConfigurationOptions.ExcludeGeneratedCodeAttribute;
                            configOptions &= ~PInvokeGeneratorConfigurationOptions.GenerateGeneratedCodeAttributeAsType;
                            break;
                        }

                        default:
                        {
                            errorList.Add($"Error: Unrecognized generate-generated-code value: {configSwitchValue}. Expected 'assembly', 'type', or 'none'.");
                            break;
                        }
                    }
                    break;
                }

                case "generate-guid-member":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateGuidMember;
                    break;
                }

                case "generate-extern-variables":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateExternVariables;
                    break;
                }

                case "generate-helper-types":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateHelperTypes;
                    break;
                }

                case "generate-macro-bindings":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateMacroBindings;
                    break;
                }

                case "generate-marker-interfaces":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateMarkerInterfaces;
                    break;
                }

                case "generate-native-alignment-attribute":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateNativeAlignmentAttribute;
                    break;
                }

                case "generate-native-bitfield-attribute":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateNativeBitfieldAttribute;
                    break;
                }

                case "generate-native-inheritance-attribute":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateNativeInheritanceAttribute;
                    break;
                }

                case "generate-generic-pointer-wrapper":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateGenericPointerWrapper;
                    break;
                }

                case "generate-setslastsystemerror-attribute":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateSetsLastSystemErrorAttribute;
                    break;
                }

                case "generate-template-bindings":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateTemplateBindings;
                    break;
                }

                case "generate-unmanaged-constants":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateUnmanagedConstants;
                    break;
                }

                case "generate-vtbl-index-attribute":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateVtblIndexAttribute;
                    break;
                }

                // Logging Options

                case "log-exclusions":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.LogExclusions;
                    break;
                }

                case "log-potential-typedef-remappings":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.LogPotentialTypedefRemappings;
                    break;
                }

                case "log-visited-files":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.LogVisitedFiles;
                    break;
                }

                // Strip Options

                case "strip-enum-member-type-name":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.StripEnumMemberTypeName;
                    break;
                }

                // Legacy Options

                case "default-remappings":
                {
                    configOptions &= ~PInvokeGeneratorConfigurationOptions.NoDefaultRemappings;
                    break;
                }

                default:
                {
                    errorList.Add($"Error: Unrecognized config switch: {configSwitch}.");
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
                WithCallConvs = withCallConvs,
                WithClasses = withClasses,
                WithConditional = withConditional,
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
