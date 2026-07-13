// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXDiagnosticSeverity;
using static ClangSharp.Interop.CXErrorCode;
using static ClangSharp.Interop.CXTranslationUnit_Flags;

namespace ClangSharp;

internal static partial class Program
{
    public static IEnumerable<HelpSectionDelegate> GetExtendedHelp(HelpContext context)
    {
        foreach (var sectionDelegate in HelpBuilder.Default.GetLayout())
        {
            yield return sectionDelegate;
        }

        yield return _ => {
            Console.WriteLine("Wildcards:");
            Console.WriteLine("You can use * as catch-all rule for remapping procedures. For example if you want make all of your generated code internal you can use --with-access-specifier *=Internal.");
        };
    }

    public static async Task<int> Main(params string[] args)
    {
        var parser = new CommandLineBuilder(s_rootCommand)
            .UseHelp(context => context.HelpBuilder.CustomizeLayout(GetExtendedHelp))
            .UseEnvironmentVariableDirective()
            .UseParseDirective()
            .UseSuggestDirective()
            .RegisterWithDotnetSuggest()
            .UseTypoCorrections()
            .UseParseErrorReporting()
            .UseExceptionHandler()
            .CancelOnProcessTermination()
            .Build();
        return await parser.InvokeAsync(args).ConfigureAwait(false);
    }

    public static void Run(InvocationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var additionalArgs = context.ParseResult.GetValueForOption(s_additionalOption) ?? [];
        var configSwitches = context.ParseResult.GetValueForOption(s_configOption) ?? [];
        var defineMacros = context.ParseResult.GetValueForOption(s_defineMacros) ?? [];
        var excludedNames = context.ParseResult.GetValueForOption(s_excludedNames) ?? [];
        var files = context.ParseResult.GetValueForOption(s_files) ?? [];
        var fileDirectory = context.ParseResult.GetValueForOption(s_fileDirectory) ?? "";
        var headerFile = context.ParseResult.GetValueForOption(s_headerFile) ?? "";
        var includedNames = context.ParseResult.GetValueForOption(s_includedNames) ?? [];
        var includeDirectories = context.ParseResult.GetValueForOption(s_includeDirectories) ?? [];
        var language = context.ParseResult.GetValueForOption(s_language) ?? "";
        var libraryPath = context.ParseResult.GetValueForOption(s_libraryPath) ?? "";
        var methodClassName = context.ParseResult.GetValueForOption(s_methodClassName) ?? "";
        var methodPrefixToStrip = context.ParseResult.GetValueForOption(s_methodPrefixToStrip) ?? "";
        var nativeTypeNamesToStrip = context.ParseResult.GetValueForOption(s_nativeTypeNamesToStrip) ?? [];
        var namespaceName = context.ParseResult.GetValueForOption(s_namespaceName) ?? "";
        var outputLocation = context.ParseResult.GetValueForOption(s_outputLocation) ?? "";
        var outputMode = context.ParseResult.GetValueForOption(s_outputMode);
        var remappedNameValuePairs = context.ParseResult.GetValueForOption(s_remappedNameValuePairs) ?? [];
        var remappedTypeNameValuePairs = context.ParseResult.GetValueForOption(s_remappedTypeNameValuePairs) ?? [];
        var remappedFieldNameValuePairs = context.ParseResult.GetValueForOption(s_remappedFieldNameValuePairs) ?? [];
        var std = context.ParseResult.GetValueForOption(s_std) ?? "";
        var testOutputLocation = context.ParseResult.GetValueForOption(s_testOutputLocation) ?? "";
        var traversalNames = context.ParseResult.GetValueForOption(s_traversalNames) ?? [];
        var withAccessSpecifierNameValuePairs = context.ParseResult.GetValueForOption(s_withAccessSpecifierNameValuePairs) ?? [];
        var withAttributeNameValuePairs = context.ParseResult.GetValueForOption(s_withAttributeNameValuePairs) ?? [];
        var withCallConvNameValuePairs = context.ParseResult.GetValueForOption(s_withCallConvNameValuePairs) ?? [];
        var withClassNameValuePairs = context.ParseResult.GetValueForOption(s_withClassNameValuePairs) ?? [];
        var withGuidNameValuePairs = context.ParseResult.GetValueForOption(s_withGuidNameValuePairs) ?? [];
        var withLengthNameValuePairs = context.ParseResult.GetValueForOption(s_withLengthNameValuePairs) ?? [];
        var withLibraryPathNameValuePairs = context.ParseResult.GetValueForOption(s_withLibraryPathNameValuePairs) ?? [];
        var withManualImports = context.ParseResult.GetValueForOption(s_withManualImports) ?? [];
        var withNamespaceNameValuePairs = context.ParseResult.GetValueForOption(s_withNamespaceNameValuePairs) ?? [];
        var withReadonlys = context.ParseResult.GetValueForOption(s_withReadonlys) ?? [];
        var withSetLastErrors = context.ParseResult.GetValueForOption(s_withSetLastErrors) ?? [];
        var withSuppressGCTransitions = context.ParseResult.GetValueForOption(s_withSuppressGCTransitions) ?? [];
        var withTransparentStructNameValuePairs = context.ParseResult.GetValueForOption(s_withTransparentStructNameValuePairs) ?? [];
        var withTypeNameValuePairs = context.ParseResult.GetValueForOption(s_withTypeNameValuePairs) ?? [];
        var withUsingNameValuePairs = context.ParseResult.GetValueForOption(s_withUsingNameValuePairs) ?? [];
        var withPackingNameValuePairs = context.ParseResult.GetValueForOption(s_withPackingNameValuePairs) ?? [];

        var versionResult = context.ParseResult.FindResultFor(s_versionOption);

        if (versionResult is not null)
        {
            context.Console.WriteLine($"{s_rootCommand.Description} version 21.1.8");
            context.Console.WriteLine($"  {clang.getClangVersion()}");
            context.Console.WriteLine($"  {clangsharp.getVersion()}");
            context.ExitCode = 0;
            return;
        }

        var errorList = new List<string>();

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

        foreach (var configSwitch in configSwitches)
        {
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

                case "generate-guid-member":
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateGuidMember;
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
            var helpBuilder = new CustomHelpBuilder(context.Console, context.LocalizationResources);

            helpBuilder.Write(s_configOption);
            helpBuilder.WriteLine();
            helpBuilder.Write(s_configOptions);

            context.ExitCode = -1;
            return;
        }

        if (errorList.Count != 0)
        {
            context.Console.Error.Write($"Error in args for '{files.FirstOrDefault()}'");
            context.Console.Error.Write(Environment.NewLine);

            foreach (var error in errorList)
            {
                context.Console.Error.Write(error);
                context.Console.Error.Write(Environment.NewLine);
            }
            context.Console.Error.Write(Environment.NewLine);

            using var textWriter = context.Console.Out.CreateTextWriter();
            var customHelpBuilder = new CustomHelpBuilder(context.Console, context.LocalizationResources);
            customHelpBuilder.Write(s_rootCommand, textWriter);

            context.ExitCode = -1;
            return;
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

        var translationFlags = CXTranslationUnit_None;

        translationFlags |= CXTranslationUnit_IncludeAttributedTypes;               // Include attributed types in CXType
        translationFlags |= CXTranslationUnit_VisitImplicitAttributes;              // Implicit attributes should be visited

        var config = new PInvokeGeneratorConfiguration(language, std, namespaceName, outputLocation, headerFile, outputMode, configOptions) {
            DefaultClass = methodClassName,
            ExcludedNames = excludedNames,
            IncludedNames = includedNames,
            LibraryPath = libraryPath,
            MethodPrefixToStrip = methodPrefixToStrip,
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
                    context.Console.WriteLine($"Error: Parsing failed for '{filePath}' due to '{translationUnitError}'.");
                    skipProcessing = true;
                }
                else if (handle.NumDiagnostics != 0)
                {
                    context.Console.WriteLine($"Diagnostics for '{filePath}':");

                    for (uint i = 0; i < handle.NumDiagnostics; ++i)
                    {
                        using var diagnostic = handle.GetDiagnostic(i);

                        context.Console.Write("    ");
                        context.Console.WriteLine(diagnostic.Format(CXDiagnostic.DefaultDisplayOptions).ToString());

                        skipProcessing |= diagnostic.Severity == CXDiagnostic_Error;
                        skipProcessing |= diagnostic.Severity == CXDiagnostic_Fatal;
                    }
                }

                if (skipProcessing)
                {
                    context.Console.WriteLine($"Skipping '{filePath}' due to one or more errors listed above.");
                    context.Console.WriteLine("");

                    exitCode = -1;
                    continue;
                }

#pragma warning disable CA1031 // Do not catch general exception types

                try
                {
                    using var translationUnit = TranslationUnit.GetOrCreate(handle);
                    Debug.Assert(translationUnit is not null);

                    context.Console.WriteLine($"Processing '{filePath}'");
                    pinvokeGenerator.GenerateBindings(translationUnit, filePath, clangCommandLineArgs, translationFlags);
                }
                catch (Exception e)
                {
                    context.Console.WriteLine(e.ToString());
                }

#pragma warning restore CA1031 // Do not catch general exception types
            }

            if (pinvokeGenerator.Diagnostics.Count != 0)
            {
                context.Console.WriteLine($"Diagnostics for binding generation of {pinvokeGenerator.FilePath}:");

                foreach (var diagnostic in pinvokeGenerator.Diagnostics)
                {
                    context.Console.Write("    ");
                    context.Console.WriteLine(diagnostic.ToString());

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

        context.ExitCode = exitCode;
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
