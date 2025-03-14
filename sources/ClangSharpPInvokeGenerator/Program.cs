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

public static class Program
{
    private static readonly string[] s_additionalOptionAliases = ["--additional", "-a"];
    private static readonly string[] s_configOptionAliases = ["--config", "-c"];
    private static readonly string[] s_defineMacroOptionAliases = ["--define-macro", "-D"];
    private static readonly string[] s_excludeOptionAliases = ["--exclude", "-e"];
    private static readonly string[] s_fileOptionAliases = ["--file", "-f"];
    private static readonly string[] s_fileDirectionOptionAliases = ["--file-directory", "-F"];
    private static readonly string[] s_headerOptionAliases = ["--headerFile", "-hf"];
    private static readonly string[] s_includeOptionAliases = ["--include", "-i"];
    private static readonly string[] s_includeDirectoryOptionAliases = ["--include-directory", "-I"];
    private static readonly string[] s_languageOptionAliases = ["--language", "-x"];
    private static readonly string[] s_libraryOptionAliases = ["--libraryPath", "-l"];
    private static readonly string[] s_methodClassNameOptionAliases = ["--methodClassName", "-m"];
    private static readonly string[] s_namespaceOptionAliases = ["--namespace", "-n"];
    private static readonly string[] s_nativeTypeNamesStripOptionAliases = ["--nativeTypeNamesToStrip"];
    private static readonly string[] s_outputModeOptionAliases = ["--output-mode", "-om"];
    private static readonly string[] s_outputOptionAliases = ["--output", "-o"];
    private static readonly string[] s_prefixStripOptionAliases = ["--prefixStrip", "-p"];
    private static readonly string[] s_remapOptionAliases = ["--remap", "-r"];
    private static readonly string[] s_stdOptionAliases = ["--std", "-std"];
    private static readonly string[] s_testOutputOptionAliases = ["--test-output", "-to"];
    private static readonly string[] s_traverseOptionAliases = ["--traverse", "-t"];
    private static readonly string[] s_versionOptionAliases = ["--version", "-v"];
    private static readonly string[] s_withAccessSpecifierOptionAliases = ["--with-access-specifier", "-was"];
    private static readonly string[] s_withAttributeOptionAliases = ["--with-attribute", "-wa"];
    private static readonly string[] s_withCallConvOptionAliases = ["--with-callconv", "-wcc"];
    private static readonly string[] s_withClassOptionAliases = ["--with-class", "-wc"];
    private static readonly string[] s_withGuidOptionAliases = ["--with-guid", "-wg"];
    private static readonly string[] s_withLengthOptionAliases = ["--with-length", "-wl"];
    private static readonly string[] s_withLibraryPathOptionAliases = ["--with-librarypath", "-wlb"];
    private static readonly string[] s_withManualImportOptionAliases = ["--with-manual-import", "-wmi"];
    private static readonly string[] s_withNamespaceOptionAliases = ["--with-namespace", "-wn"];
    private static readonly string[] s_withPackingOptionAliases = ["--with-packing", "-wp"];
    private static readonly string[] s_withSetLastErrorOptionAliases = ["--with-setlasterror", "-wsle"];
    private static readonly string[] s_withSuppressGCTransitionOptionAliases = ["--with-suppressgctransition", "-wsgct"];
    private static readonly string[] s_withTransparentStructOptionAliases = ["--with-transparent-struct", "-wts"];
    private static readonly string[] s_withTypeOptionAliases = ["--with-type", "-wt"];
    private static readonly string[] s_withUsingOptionAliases = ["--with-using", "-wu"];

    private static readonly Option<string[]> s_additionalOption = GetAdditionalOption();
    private static readonly Option<string[]> s_configOption = GetConfigOption();
    private static readonly Option<string[]> s_defineMacros = GetDefineMacroOption();
    private static readonly Option<string[]> s_excludedNames = GetExcludeOption();
    private static readonly Option<string[]> s_files = GetFileOption();
    private static readonly Option<string> s_fileDirectory = GetFileDirectoryOption();
    private static readonly Option<string> s_headerFile = GetHeaderOption();
    private static readonly Option<string[]> s_includedNames = GetIncludeOption();
    private static readonly Option<string[]> s_includeDirectories = GetIncludeDirectoryOption();
    private static readonly Option<string> s_language = GetLanguageOption();
    private static readonly Option<string> s_libraryPath = GetLibraryOption();
    private static readonly Option<string> s_methodClassName = GetMethodClassNameOption();
    private static readonly Option<string> s_methodPrefixToStrip = GetPrefixStripOption();
    private static readonly Option<string> s_namespaceName = GetNamespaceOption();
    private static readonly Option<string[]> s_nativeTypeNamesToStrip = GetNativeTypeNamesStripOption();
    private static readonly Option<string> s_outputLocation = GetOutputOption();
    private static readonly Option<PInvokeGeneratorOutputMode> s_outputMode = GetOutputModeOption();
    private static readonly Option<string[]> s_remappedNameValuePairs = GetRemapOption();
    private static readonly Option<string> s_std = GetStdOption();
    private static readonly Option<string> s_testOutputLocation = GetTestOutputOption();
    private static readonly Option<string[]> s_traversalNames = GetTraverseOption();
    private static readonly Option<bool> s_versionOption = GetVersionOption();
    private static readonly Option<string[]> s_withAccessSpecifierNameValuePairs = GetWithAccessSpecifierOption();
    private static readonly Option<string[]> s_withAttributeNameValuePairs = GetWithAttributeOption();
    private static readonly Option<string[]> s_withCallConvNameValuePairs = GetWithCallConvOption();
    private static readonly Option<string[]> s_withClassNameValuePairs = GetWithClassOption();
    private static readonly Option<string[]> s_withGuidNameValuePairs = GetWithGuidOption();
    private static readonly Option<string[]> s_withLengthNameValuePairs = GetWithLengthOption();
    private static readonly Option<string[]> s_withLibraryPathNameValuePairs = GetWithLibraryPathOption();
    private static readonly Option<string[]> s_withManualImports = GetWithManualImportOption();
    private static readonly Option<string[]> s_withNamespaceNameValuePairs = GetWithNamespaceOption();
    private static readonly Option<string[]> s_withPackingNameValuePairs = GetWithPackingOption();
    private static readonly Option<string[]> s_withSetLastErrors = GetWithSetLastErrorOption();
    private static readonly Option<string[]> s_withSuppressGCTransitions = GetWithSuppressGCTransitionOption();
    private static readonly Option<string[]> s_withTransparentStructNameValuePairs = GetWithTransparentStructOption();
    private static readonly Option<string[]> s_withTypeNameValuePairs = GetWithTypeOption();
    private static readonly Option<string[]> s_withUsingNameValuePairs = GetWithUsingOption();

    private static readonly RootCommand s_rootCommand = GetRootCommand();

    private static readonly TwoColumnHelpRow[] s_configOptions =
    [
        new TwoColumnHelpRow("?, h, help", "Show help and usage information for -c, --config"),

        new TwoColumnHelpRow("", ""),
        new TwoColumnHelpRow("# Codegen Options", ""),
        new TwoColumnHelpRow("", ""),

        new TwoColumnHelpRow("compatible-codegen", "Bindings should be generated with .NET Standard 2.0 compatibility. Setting this disables preview code generation."),
        new TwoColumnHelpRow("default-codegen", "Bindings should be generated for the current LTS version of .NET/C#. This is currently .NET 6/C# 10."),
        new TwoColumnHelpRow("latest-codegen", "Bindings should be generated for the current STS version of .NET/C#. This is currently .NET 7/C# 11."),
        new TwoColumnHelpRow("preview-codegen", "Bindings should be generated for the preview version of .NET/C#. This is currently .NET 8/C# 12."),

        new TwoColumnHelpRow("", ""),
        new TwoColumnHelpRow("# File Options", ""),
        new TwoColumnHelpRow("", ""),

        new TwoColumnHelpRow("single-file", "Bindings should be generated to a single output file. This is the default."),
        new TwoColumnHelpRow("multi-file", "Bindings should be generated so there is approximately one type per file."),

        new TwoColumnHelpRow("", ""),
        new TwoColumnHelpRow("# Type Options", ""),
        new TwoColumnHelpRow("", ""),

        new TwoColumnHelpRow("unix-types", "Bindings should be generated assuming Unix defaults. This is the default on Unix platforms."),
        new TwoColumnHelpRow("windows-types", "Bindings should be generated assuming Windows defaults. This is the default on Windows platforms."),

        new TwoColumnHelpRow("", ""),
        new TwoColumnHelpRow("# Exclusion Options", ""),
        new TwoColumnHelpRow("", ""),

        new TwoColumnHelpRow("exclude-anonymous-field-helpers", "The helper ref properties generated for fields in nested anonymous structs and unions should not be generated."),
        new TwoColumnHelpRow("exclude-com-proxies", "Types recognized as COM proxies should not have bindings generated. These are currently function declarations ending with _UserFree, _UserMarshal, _UserSize, _UserUnmarshal, _Proxy, or _Stub."),
        new TwoColumnHelpRow("exclude-default-remappings", "Default remappings for well known types should not be added. This currently includes intptr_t, ptrdiff_t, size_t, and uintptr_t"),
        new TwoColumnHelpRow("exclude-empty-records", "Bindings for records that contain no members should not be generated. These are commonly encountered for opaque handle like types such as HWND."),
        new TwoColumnHelpRow("exclude-enum-operators", "Bindings for operators over enum types should not be generated. These are largely unnecessary in C# as the operators are available by default."),
        new TwoColumnHelpRow("exclude-fnptr-codegen", "Generated bindings for latest or preview codegen should not use function pointers."),
        new TwoColumnHelpRow("exclude-funcs-with-body", "Bindings for functions with bodies should not be generated."),
        new TwoColumnHelpRow("exclude-using-statics-for-enums", "Enum usages should be fully qualified and should not include a corresponding 'using static EnumName;'"),

        new TwoColumnHelpRow("", ""),
        new TwoColumnHelpRow("# Vtbl Options", ""),
        new TwoColumnHelpRow("", ""),

        new TwoColumnHelpRow("explicit-vtbls", "VTBLs should have an explicit type generated with named fields per entry."),
        new TwoColumnHelpRow("implicit-vtbls", "VTBLs should be implicit to reduce metadata bloat. This is the current default"),
        new TwoColumnHelpRow("trimmable-vtbls", "VTBLs should be defined but not used in helper methods to reduce metadata bloat when trimming."),

        new TwoColumnHelpRow("", ""),
        new TwoColumnHelpRow("# Test Options", ""),
        new TwoColumnHelpRow("", ""),

        new TwoColumnHelpRow("generate-tests-nunit", "Basic tests validating size, blittability, and associated metadata should be generated for NUnit."),
        new TwoColumnHelpRow("generate-tests-xunit", "Basic tests validating size, blittability, and associated metadata should be generated for XUnit."),

        new TwoColumnHelpRow("", ""),
        new TwoColumnHelpRow("# Generation Options", ""),
        new TwoColumnHelpRow("", ""),

        new TwoColumnHelpRow("generate-aggressive-inlining", "[MethodImpl(MethodImplOptions.AggressiveInlining)] should be added to generated helper functions."),
        new TwoColumnHelpRow("generate-callconv-member-function", "Instance function pointers should use [CallConvMemberFunction] where applicable."),
        new TwoColumnHelpRow("generate-cpp-attributes", "[CppAttributeList(\"\")] should be generated to document the encountered C++ attributes."),
        new TwoColumnHelpRow("generate-disable-runtime-marshalling", "[assembly: DisableRuntimeMarshalling] should be generated."),
        new TwoColumnHelpRow("generate-doc-includes", "<include> xml documentation tags should be generated for declarations."),
        new TwoColumnHelpRow("generate-file-scoped-namespaces", "Namespaces should be scoped to the file to reduce nesting."),
        new TwoColumnHelpRow("generate-guid-member", "Types with an associated GUID should have a corresponding member generated."),
        new TwoColumnHelpRow("generate-helper-types", "Code files should be generated for various helper attributes and declared transparent structs."),
        new TwoColumnHelpRow("generate-macro-bindings", "Bindings for macro-definitions should be generated. This currently only works with value like macros and not function-like ones."),
        new TwoColumnHelpRow("generate-marker-interfaces", "Bindings for marker interfaces representing native inheritance hierarchies should be generated."),
        new TwoColumnHelpRow("generate-native-bitfield-attribute", "[NativeBitfield(\"\", offset: #, length: #)] attribute should be generated to document the encountered bitfield layout."),
        new TwoColumnHelpRow("generate-native-inheritance-attribute", "[NativeInheritance(\"\")] attribute should be generated to document the encountered C++ base type."),
        new TwoColumnHelpRow("generate-generic-pointer-wrapper", "Pointer<T> should be used for limited generic type support."),
        new TwoColumnHelpRow("generate-setslastsystemerror-attribute", "[SetsLastSystemError] attribute should be generated rather than using SetLastError = true."),
        new TwoColumnHelpRow("generate-template-bindings", "Bindings for template-definitions should be generated. This is currently experimental."),
        new TwoColumnHelpRow("generate-unmanaged-constants", "Unmanaged constants should be generated using static ref readonly properties. This is currently experimental."),
        new TwoColumnHelpRow("generate-vtbl-index-attribute", "[VtblIndex(#)] attribute should be generated to document the underlying VTBL index for a helper method."),

        new TwoColumnHelpRow("", ""),
        new TwoColumnHelpRow("# Stripping Options", ""),
        new TwoColumnHelpRow("", ""),

        new TwoColumnHelpRow("strip-enum-member-type-name", "Strips the enum type name from the beginning of its member names."),

        new TwoColumnHelpRow("", ""),
        new TwoColumnHelpRow("# Logging Options", ""),
        new TwoColumnHelpRow("", ""),

        new TwoColumnHelpRow("log-exclusions", "A list of excluded declaration types should be generated. This will also log if the exclusion was due to an exact or partial match."),
        new TwoColumnHelpRow("log-potential-typedef-remappings", "A list of potential typedef remappings should be generated. This can help identify missing remappings."),
        new TwoColumnHelpRow("log-visited-files", "A list of the visited files should be generated. This can help identify traversal issues."),
    ];

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
        var withSetLastErrors = context.ParseResult.GetValueForOption(s_withSetLastErrors) ?? [];
        var withSuppressGCTransitions = context.ParseResult.GetValueForOption(s_withSuppressGCTransitions) ?? [];
        var withTransparentStructNameValuePairs = context.ParseResult.GetValueForOption(s_withTransparentStructNameValuePairs) ?? [];
        var withTypeNameValuePairs = context.ParseResult.GetValueForOption(s_withTypeNameValuePairs) ?? [];
        var withUsingNameValuePairs = context.ParseResult.GetValueForOption(s_withUsingNameValuePairs) ?? [];
        var withPackingNameValuePairs = context.ParseResult.GetValueForOption(s_withPackingNameValuePairs) ?? [];

        var versionResult = context.ParseResult.FindResultFor(s_versionOption);

        if (versionResult is not null)
        {
            context.Console.WriteLine($"{s_rootCommand.Description} version 18.1.3");
            context.Console.WriteLine($"  {clang.getClangVersion()}");
            context.Console.WriteLine($"  {clangsharp.getVersion()}");
            context.ExitCode = -1;
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

                    if (configOptions.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit))
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

                    if (configOptions.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit))
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

        if (!string.IsNullOrWhiteSpace(testOutputLocation) && !configOptions.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit) && !configOptions.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit))
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

    private static Option<string[]> GetAdditionalOption()
    {
        return new Option<string[]>(
            aliases: s_additionalOptionAliases,
            description: "An argument to pass to Clang when parsing the input files.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetConfigOption()
    {
        return new Option<string[]>(
            aliases: s_configOptionAliases,
            description: "A configuration option that controls how the bindings are generated. Specify 'help' to see the available options.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetDefineMacroOption()
    {
        return new Option<string[]>(
            aliases: s_defineMacroOptionAliases,
            description: "Define <macro> to <value> (or 1 if <value> omitted).",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetExcludeOption()
    {
        return new Option<string[]>(
            aliases: s_excludeOptionAliases,
            description: "A declaration name to exclude from binding generation.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetFileOption()
    {
        return new Option<string[]>(
            aliases: s_fileOptionAliases,
            description: "A file to parse and generate bindings for.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string> GetFileDirectoryOption()
    {
        return new Option<string>(
            aliases: s_fileDirectionOptionAliases,
            description: "The base path for files to parse.",
            getDefaultValue: () => string.Empty
        );
    }

    private static Option<string> GetHeaderOption()
    {
        return new Option<string>(
            aliases: s_headerOptionAliases,
            description: "A file which contains the header to prefix every generated file with.",
            getDefaultValue: () => string.Empty
        );
    }

    private static Option<string[]> GetIncludeOption()
    {
        return new Option<string[]>(
            aliases: s_includeOptionAliases,
            description: "A declaration name to include in binding generation.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetIncludeDirectoryOption()
    {
        return new Option<string[]>(
            aliases: s_includeDirectoryOptionAliases,
            description: "Add directory to include search path.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string> GetLanguageOption()
    {
        return new Option<string>(
            aliases: s_languageOptionAliases,
            description: "Treat subsequent input files as having type <language>.",
            getDefaultValue: () => "c++"
        ).FromAmong("c", "c++");
    }

    private static Option<string> GetLibraryOption()
    {
        return new Option<string>(
            aliases: s_libraryOptionAliases,
            description: "The string to use in the DllImport attribute used when generating bindings.",
            getDefaultValue: () => string.Empty
        );
    }

    private static Option<string> GetMethodClassNameOption()
    {
        return new Option<string>(
            aliases: s_methodClassNameOptionAliases,
            description: "The name of the static class that will contain the generated method bindings.",
            getDefaultValue: () => "Methods"
        );
    }

    private static Option<string> GetNamespaceOption()
    {
        return new Option<string>(
            aliases: s_namespaceOptionAliases,
            description: "The namespace in which to place the generated bindings.",
            getDefaultValue: () => string.Empty
        );
    }

    private static Option<string[]> GetNativeTypeNamesStripOption()
    {
        return new Option<string[]>(
            aliases: s_nativeTypeNamesStripOptionAliases,
            description: "The contents to strip from the generated NativeTypeName attributes.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<PInvokeGeneratorOutputMode> GetOutputModeOption()
    {
        return new Option<PInvokeGeneratorOutputMode>(
            aliases: s_outputModeOptionAliases,
            description: "The mode describing how the information collected from the headers are presented in the resultant bindings.",
            getDefaultValue: () => PInvokeGeneratorOutputMode.CSharp
        );
    }

    private static Option<string> GetOutputOption()
    {
        return new Option<string>(
            aliases: s_outputOptionAliases,
            description: "The output location to write the generated bindings to.",
            getDefaultValue: () => string.Empty
        );
    }

    private static Option<string> GetPrefixStripOption()
    {
        return new Option<string>(
            aliases: s_prefixStripOptionAliases,
            description: "The prefix to strip from the generated method bindings.",
            getDefaultValue: () => string.Empty
        );
    }

    private static Option<string[]> GetRemapOption()
    {
        return new Option<string[]>(
            aliases: s_remapOptionAliases,
            description: "A declaration name to be remapped to another name during binding generation.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static RootCommand GetRootCommand()
    {
        var rootCommand = new RootCommand("ClangSharp P/Invoke Binding Generator")
        {
            s_additionalOption,
            s_configOption,
            s_defineMacros,
            s_excludedNames,
            s_files,
            s_fileDirectory,
            s_headerFile,
            s_includedNames,
            s_includeDirectories,
            s_language,
            s_libraryPath,
            s_methodClassName,
            s_namespaceName,
            s_outputMode,
            s_outputLocation,
            s_methodPrefixToStrip,
            s_nativeTypeNamesToStrip,
            s_remappedNameValuePairs,
            s_std,
            s_testOutputLocation,
            s_traversalNames,
            s_versionOption,
            s_withAccessSpecifierNameValuePairs,
            s_withAttributeNameValuePairs,
            s_withCallConvNameValuePairs,
            s_withClassNameValuePairs,
            s_withGuidNameValuePairs,
            s_withLengthNameValuePairs,
            s_withLibraryPathNameValuePairs,
            s_withManualImports,
            s_withNamespaceNameValuePairs,
            s_withPackingNameValuePairs,
            s_withSetLastErrors,
            s_withSuppressGCTransitions,
            s_withTransparentStructNameValuePairs,
            s_withTypeNameValuePairs,
            s_withUsingNameValuePairs
        };
        Handler.SetHandler(rootCommand, (Action<InvocationContext>)Run);
        return rootCommand;
    }

    private static Option<string> GetStdOption()
    {
        return new Option<string>(
            aliases: s_stdOptionAliases,
            description: "Language standard to compile for.",
            getDefaultValue: () => ""
        );
    }

    private static Option<string> GetTestOutputOption()
    {
        return new Option<string>(
            aliases: s_testOutputOptionAliases,
            description: "The output location to write the generated tests to.",
            getDefaultValue: () => string.Empty
        );
    }

    private static Option<bool> GetVersionOption()
    {
        return new Option<bool>(
            aliases: s_versionOptionAliases,
            description: "Prints the current version information for the tool and its native dependencies."
        ) {
            Arity = ArgumentArity.Zero
        };
    }

    private static Option<string[]> GetTraverseOption()
    {
        return new Option<string[]>(
            aliases: s_traverseOptionAliases,
            description: "A file name included either directly or indirectly by -f that should be traversed during binding generation.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithAccessSpecifierOption()
    {
        return new Option<string[]>(
            aliases: s_withAccessSpecifierOptionAliases,
            description: "An access specifier to be used with the given qualified or remapped declaration name during binding generation. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithAttributeOption()
    {
        return new Option<string[]>(
            aliases: s_withAttributeOptionAliases,
            description: "An attribute to be added to the given remapped declaration name during binding generation. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithCallConvOption()
    {
        return new Option<string[]>(
            aliases: s_withCallConvOptionAliases,
            description: "A calling convention to be used for the given declaration during binding generation. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithClassOption()
    {
        return new Option<string[]>(
            aliases: s_withClassOptionAliases,
            description: "A class to be used for the given remapped constant or function declaration name during binding generation. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithGuidOption()
    {
        return new Option<string[]>(
            aliases: s_withGuidOptionAliases,
            description: "A GUID to be used for the given declaration during binding generation. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithLengthOption()
    {
        return new Option<string[]>(
            aliases: s_withLengthOptionAliases,
            description: "A length to be used for the given declaration during binding generation. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithLibraryPathOption()
    {
        return new Option<string[]>(
            aliases: s_withLibraryPathOptionAliases,
            description: "A library path to be used for the given declaration during binding generation. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithManualImportOption()
    {
        return new Option<string[]>(
            aliases: s_withManualImportOptionAliases,
            description: "A remapped function name to be treated as a manual import during binding generation. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithNamespaceOption()
    {
        return new Option<string[]>(
            aliases: s_withNamespaceOptionAliases,
            description: "A namespace to be used for the given remapped declaration name during binding generation. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithSetLastErrorOption()
    {
        return new Option<string[]>(
            aliases: s_withSetLastErrorOptionAliases,
            description: "Add the SetLastError=true modifier or SetsSystemLastError attribute to a given DllImport or UnmanagedFunctionPointer. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithSuppressGCTransitionOption()
    {
        return new Option<string[]>(
            aliases: s_withSuppressGCTransitionOptionAliases,
            description: "Add the SuppressGCTransition calling convention to a given DllImport or UnmanagedFunctionPointer. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithTransparentStructOption()
    {
        return new Option<string[]>(
            aliases: s_withTransparentStructOptionAliases,
            description: "A remapped type name to be treated as a transparent wrapper during binding generation. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithTypeOption()
    {
        return new Option<string[]>(
            aliases: s_withTypeOptionAliases,
            description: "A type to be used for the given enum declaration during binding generation. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithUsingOption()
    {
        return new Option<string[]>(
            aliases: s_withUsingOptionAliases,
            description: "A using directive to be included for the given remapped declaration name during binding generation. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }

    private static Option<string[]> GetWithPackingOption()
    {
        return new Option<string[]>(
            aliases: s_withPackingOptionAliases,
            description: "Overrides the StructLayoutAttribute.Pack property for the given type. Supports wildcards.",
            getDefaultValue: Array.Empty<string>
        ) {
            AllowMultipleArgumentsPerToken = true
        };
    }
}
