// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp;

internal static partial class Program
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
    private static readonly string[] s_typePrefixStripOptionAliases = ["--typePrefixStrip", "-tp"];
    private static readonly string[] s_remapOptionAliases = ["--remap", "-r"];
    private static readonly string[] s_remapTypeOptionAliases = ["--remap-type", "-rt"];
    private static readonly string[] s_remapFieldOptionAliases = ["--remap-field", "-rf"];
    private static readonly string[] s_resourceDirectoryOptionAliases = ["--resource-directory", "-rd"];
    private static readonly string[] s_resourceDirectoryDetectionOptionAliases = ["--no-resource-directory-detection"];
    private static readonly string[] s_stdOptionAliases = ["--std", "-std"];
    private static readonly string[] s_testOutputOptionAliases = ["--test-output", "-to"];
    private static readonly string[] s_traverseOptionAliases = ["--traverse", "-t"];
    private static readonly string[] s_versionOptionAliases = ["--version", "-v"];
    private static readonly string[] s_withAccessSpecifierOptionAliases = ["--with-access-specifier", "-was"];
    private static readonly string[] s_withAttributeOptionAliases = ["--with-attribute", "-wa"];
    private static readonly string[] s_withCallConvOptionAliases = ["--with-callconv", "-wcc"];
    private static readonly string[] s_withClassOptionAliases = ["--with-class", "-wc"];
    private static readonly string[] s_withConditionalOptionAliases = ["--with-conditional", "-wcond"];
    private static readonly string[] s_withEnumMemberStripOptionAliases = ["--with-enum-member-strip", "-wems"];
    private static readonly string[] s_withGuidOptionAliases = ["--with-guid", "-wg"];
    private static readonly string[] s_withLengthOptionAliases = ["--with-length", "-wl"];
    private static readonly string[] s_withLibraryPathOptionAliases = ["--with-librarypath", "-wlb"];
    private static readonly string[] s_withManualImportOptionAliases = ["--with-manual-import", "-wmi"];
    private static readonly string[] s_withNamespaceOptionAliases = ["--with-namespace", "-wn"];
    private static readonly string[] s_withPackingOptionAliases = ["--with-packing", "-wp"];
    private static readonly string[] s_withReadonlyOptionAliases = ["--with-readonly", "-wro"];
    private static readonly string[] s_withSetLastErrorOptionAliases = ["--with-setlasterror", "-wsle"];
    private static readonly string[] s_withSuppressGCTransitionOptionAliases = ["--with-suppressgctransition", "-wsgct"];
    private static readonly string[] s_withTransparentStructOptionAliases = ["--with-transparent-struct", "-wts"];
    private static readonly string[] s_withTypeOptionAliases = ["--with-type", "-wt"];
    private static readonly string[] s_withUsingOptionAliases = ["--with-using", "-wu"];
    private static readonly string[] s_helpOptionAliases = ["--help", "-?", "-h"];

    private static readonly CommandLineOption s_additionalOption = Multi(s_additionalOptionAliases, "An argument to pass to Clang when parsing the input files.");
    private static readonly CommandLineOption s_configOption = Multi(s_configOptionAliases, "A configuration option that controls how the bindings are generated. Specify 'help' to see the available options.");
    private static readonly CommandLineOption s_defineMacros = Multi(s_defineMacroOptionAliases, "Define <macro> to <value> (or 1 if <value> omitted).");
    private static readonly CommandLineOption s_excludedNames = Multi(s_excludeOptionAliases, "A declaration name to exclude from binding generation.");
    private static readonly CommandLineOption s_files = Multi(s_fileOptionAliases, "A file to parse and generate bindings for.");
    private static readonly CommandLineOption s_fileDirectory = Single(s_fileDirectionOptionAliases, "The base path for files to parse.");
    private static readonly CommandLineOption s_headerFile = Single(s_headerOptionAliases, "A file which contains the header to prefix every generated file with.");
    private static readonly CommandLineOption s_includedNames = Multi(s_includeOptionAliases, "A declaration name to include in binding generation.");
    private static readonly CommandLineOption s_includeDirectories = Multi(s_includeDirectoryOptionAliases, "Add directory to include search path.");
    private static readonly CommandLineOption s_language = Single(s_languageOptionAliases, "Treat subsequent input files as having type <language>.", defaultValue: "c++", valueName: "c|c++", allowedValues: ["c", "c++"]);
    private static readonly CommandLineOption s_libraryPath = Single(s_libraryOptionAliases, "The string to use in the DllImport attribute used when generating bindings.");
    private static readonly CommandLineOption s_methodClassName = Single(s_methodClassNameOptionAliases, "The name of the static class that will contain the generated method bindings.", defaultValue: "Methods");
    private static readonly CommandLineOption s_namespaceName = Single(s_namespaceOptionAliases, "The namespace in which to place the generated bindings.");
    private static readonly CommandLineOption s_nativeTypeNamesToStrip = Multi(s_nativeTypeNamesStripOptionAliases, "The contents to strip from the generated NativeTypeName attributes.");
    private static readonly CommandLineOption s_outputMode = Single(s_outputModeOptionAliases, "The mode describing how the information collected from the headers are presented in the resultant bindings.", defaultValue: "CSharp", valueName: "CSharp|Xml");
    private static readonly CommandLineOption s_outputLocation = Single(s_outputOptionAliases, "The output location to write the generated bindings to.");
    private static readonly CommandLineOption s_methodPrefixToStrip = Single(s_prefixStripOptionAliases, "The prefix to strip from the generated method bindings.");
    private static readonly CommandLineOption s_typePrefixToStrip = Single(s_typePrefixStripOptionAliases, "The prefix to strip from the generated enum, struct, and union type bindings (and their enum member names).");
    private static readonly CommandLineOption s_remappedNameValuePairs = Multi(s_remapOptionAliases, "A declaration name to be remapped to another name during binding generation.");
    private static readonly CommandLineOption s_remappedTypeNameValuePairs = Multi(s_remapTypeOptionAliases, "A type (record or enum) declaration name to be remapped to another name during binding generation. Takes precedence over --remap and is useful when a type and field share a name.");
    private static readonly CommandLineOption s_remappedFieldNameValuePairs = Multi(s_remapFieldOptionAliases, "A field declaration name to be remapped to another name during binding generation. Takes precedence over --remap and is useful when a type and field share a name.");
    private static readonly CommandLineOption s_resourceDirectory = Single(s_resourceDirectoryOptionAliases, "The Clang resource directory containing the builtin headers (such as stddef.h). When omitted, an installed and version-matched Clang's resource directory is automatically detected.", valueName: "directory");
    private static readonly CommandLineOption s_resourceDirectoryDetectionDisabled = Flag(s_resourceDirectoryDetectionOptionAliases, "Disable the automatic detection of the Clang resource directory.");
    private static readonly CommandLineOption s_std = Single(s_stdOptionAliases, "Language standard to compile for.");
    private static readonly CommandLineOption s_testOutputLocation = Single(s_testOutputOptionAliases, "The output location to write the generated tests to.");
    private static readonly CommandLineOption s_traversalNames = Multi(s_traverseOptionAliases, "A file name included either directly or indirectly by -f that should be traversed during binding generation.");
    private static readonly CommandLineOption s_versionOption = Flag(s_versionOptionAliases, "Prints the current version information for the tool and its native dependencies.");
    private static readonly CommandLineOption s_withAccessSpecifierNameValuePairs = Multi(s_withAccessSpecifierOptionAliases, "An access specifier to be used with the given qualified or remapped declaration name during binding generation. Supports wildcards.");
    private static readonly CommandLineOption s_withAttributeNameValuePairs = Multi(s_withAttributeOptionAliases, "An attribute to be added to the given remapped declaration name during binding generation. Supports wildcards.");
    private static readonly CommandLineOption s_withCallConvNameValuePairs = Multi(s_withCallConvOptionAliases, "A calling convention to be used for the given declaration during binding generation. Supports wildcards.");
    private static readonly CommandLineOption s_withClassNameValuePairs = Multi(s_withClassOptionAliases, "A class to be used for the given remapped constant or function declaration name during binding generation. Supports wildcards.");
    private static readonly CommandLineOption s_withConditional = Single(s_withConditionalOptionAliases, "A preprocessor symbol used to wrap single-file C# output in a leading '#if <symbol>' and trailing '#endif'. Useful when files can't be conditionally excluded at the project level (e.g. Unity).", valueName: "symbol");
    private static readonly CommandLineOption s_withEnumMemberStripNameValuePairs = Multi(s_withEnumMemberStripOptionAliases, "How to strip a prefix or suffix from the members of the given remapped enum name during binding generation. Mode is one of `none`, `common-prefix`, `common-suffix`, `type-name`, `prefix:<str>`, or `suffix:<str>`. Supports wildcards.");
    private static readonly CommandLineOption s_withGuidNameValuePairs = Multi(s_withGuidOptionAliases, "A GUID to be used for the given declaration during binding generation. Supports wildcards.");
    private static readonly CommandLineOption s_withLengthNameValuePairs = Multi(s_withLengthOptionAliases, "A length to be used for the given declaration during binding generation. Supports wildcards.");
    private static readonly CommandLineOption s_withLibraryPathNameValuePairs = Multi(s_withLibraryPathOptionAliases, "A library path to be used for the given declaration during binding generation. Supports wildcards.");
    private static readonly CommandLineOption s_withManualImports = Multi(s_withManualImportOptionAliases, "A remapped function name to be treated as a manual import during binding generation. Supports wildcards.");
    private static readonly CommandLineOption s_withNamespaceNameValuePairs = Multi(s_withNamespaceOptionAliases, "A namespace to be used for the given remapped declaration name during binding generation. Supports wildcards.");
    private static readonly CommandLineOption s_withPackingNameValuePairs = Multi(s_withPackingOptionAliases, "Overrides the StructLayoutAttribute.Pack property for the given type. Supports wildcards.");
    private static readonly CommandLineOption s_withReadonlys = Multi(s_withReadonlyOptionAliases, "Add the readonly modifier to a given instance method. Supports wildcards.");
    private static readonly CommandLineOption s_withSetLastErrors = Multi(s_withSetLastErrorOptionAliases, "Add the SetLastError=true modifier or SetsSystemLastError attribute to a given DllImport or UnmanagedFunctionPointer. Supports wildcards.");
    private static readonly CommandLineOption s_withSuppressGCTransitions = Multi(s_withSuppressGCTransitionOptionAliases, "Add the SuppressGCTransition calling convention to a given DllImport or UnmanagedFunctionPointer. Supports wildcards.");
    private static readonly CommandLineOption s_withTransparentStructNameValuePairs = Multi(s_withTransparentStructOptionAliases, "A remapped type name to be treated as a transparent wrapper during binding generation. Supports wildcards.");
    private static readonly CommandLineOption s_withTypeNameValuePairs = Multi(s_withTypeOptionAliases, "A type to be used for the given enum declaration, macro constant, or struct field (using the qualified `Type.field`) during binding generation. Supports wildcards.");
    private static readonly CommandLineOption s_withUsingNameValuePairs = Multi(s_withUsingOptionAliases, "A using directive to be included for the given remapped declaration name during binding generation. Supports wildcards.");
    private static readonly CommandLineOption s_helpOption = Flag(s_helpOptionAliases, "Show help and usage information");

    private static readonly CommandLineOption[] s_options =
    [
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
        s_typePrefixToStrip,
        s_nativeTypeNamesToStrip,
        s_remappedNameValuePairs,
        s_remappedTypeNameValuePairs,
        s_remappedFieldNameValuePairs,
        s_resourceDirectory,
        s_resourceDirectoryDetectionDisabled,
        s_std,
        s_testOutputLocation,
        s_traversalNames,
        s_versionOption,
        s_withAccessSpecifierNameValuePairs,
        s_withAttributeNameValuePairs,
        s_withCallConvNameValuePairs,
        s_withClassNameValuePairs,
        s_withConditional,
        s_withEnumMemberStripNameValuePairs,
        s_withGuidNameValuePairs,
        s_withLengthNameValuePairs,
        s_withLibraryPathNameValuePairs,
        s_withManualImports,
        s_withNamespaceNameValuePairs,
        s_withPackingNameValuePairs,
        s_withReadonlys,
        s_withSetLastErrors,
        s_withSuppressGCTransitions,
        s_withTransparentStructNameValuePairs,
        s_withTypeNameValuePairs,
        s_withUsingNameValuePairs,
        s_helpOption,
    ];

    private static readonly CommandLineParser s_parser = new(s_options);

    private static readonly HelpRow[] s_configOptions =
    [
        new HelpRow("?, h, help", "Show help and usage information for -c, --config"),

        new HelpRow("", ""),
        new HelpRow("# Codegen Options", ""),
        new HelpRow("", ""),

        new HelpRow("compatible-codegen", "Bindings should be generated with .NET Standard 2.0 compatibility. Setting this disables preview code generation."),
        new HelpRow("default-codegen", "Bindings should be generated for the current LTS version of .NET/C#. This is currently .NET 8/C# 12."),
        new HelpRow("latest-codegen", "Bindings should be generated for the current STS version of .NET/C#. This is currently .NET 10/C# 14."),
        new HelpRow("preview-codegen", "Bindings should be generated for the preview version of .NET/C#. This is currently .NET 10/C# 14."),

        new HelpRow("", ""),
        new HelpRow("# File Options", ""),
        new HelpRow("", ""),

        new HelpRow("single-file", "Bindings should be generated to a single output file. This is the default."),
        new HelpRow("multi-file", "Bindings should be generated so there is approximately one type per file."),

        new HelpRow("", ""),
        new HelpRow("# Type Options", ""),
        new HelpRow("", ""),

        new HelpRow("unix-types", "Bindings should be generated assuming Unix defaults. This is the default on Unix platforms."),
        new HelpRow("windows-types", "Bindings should be generated assuming Windows defaults. This is the default on Windows platforms."),

        new HelpRow("", ""),
        new HelpRow("# Exclusion Options", ""),
        new HelpRow("", ""),

        new HelpRow("exclude-anonymous-field-helpers", "The helper ref properties generated for fields in nested anonymous structs and unions should not be generated."),
        new HelpRow("exclude-com-proxies", "Types recognized as COM proxies should not have bindings generated. These are currently function declarations ending with _UserFree, _UserMarshal, _UserSize, _UserUnmarshal, _Proxy, or _Stub."),
        new HelpRow("exclude-default-remappings", "Default remappings for well known types should not be added. This currently includes intptr_t, ptrdiff_t, size_t, ssize_t, uintptr_t, and the exact-width stdint types (int8_t, int16_t, int32_t, int64_t, uint8_t, uint16_t, uint32_t, and uint64_t). When targeting Windows, the pointer-width Windows types (INT_PTR, LONG_PTR, SSIZE_T, DWORD_PTR, SIZE_T, UINT_PTR, and ULONG_PTR) and _GUID are also included"),
        new HelpRow("exclude-empty-records", "Bindings for records that contain no members should not be generated. These are commonly encountered for opaque handle like types such as HWND."),
        new HelpRow("exclude-enum-operators", "Bindings for operators over enum types should not be generated. These are largely unnecessary in C# as the operators are available by default."),
        new HelpRow("exclude-fnptr-codegen", "Generated bindings for latest or preview codegen should not use function pointers."),
        new HelpRow("exclude-funcs-with-body", "Bindings for functions with bodies should not be generated."),
        new HelpRow("exclude-using-statics-for-enums", "Enum usages should be fully qualified and should not include a corresponding 'using static EnumName;'"),

        new HelpRow("", ""),
        new HelpRow("# Vtbl Options", ""),
        new HelpRow("", ""),

        new HelpRow("explicit-vtbls", "VTBLs should have an explicit type generated with named fields per entry."),
        new HelpRow("implicit-vtbls", "VTBLs should be implicit to reduce metadata bloat. This is the current default"),
        new HelpRow("trimmable-vtbls", "VTBLs should be defined but not used in helper methods to reduce metadata bloat when trimming."),

        new HelpRow("", ""),
        new HelpRow("# Test Options", ""),
        new HelpRow("", ""),

        new HelpRow("generate-tests-nunit", "Basic tests validating size, blittability, and associated metadata should be generated for NUnit."),
        new HelpRow("generate-tests-xunit", "Basic tests validating size, blittability, and associated metadata should be generated for XUnit."),

        new HelpRow("", ""),
        new HelpRow("# Generation Options", ""),
        new HelpRow("", ""),

        new HelpRow("generate-aggressive-inlining", "[MethodImpl(MethodImplOptions.AggressiveInlining)] should be added to generated helper functions."),
        new HelpRow("generate-callconv-member-function", "Instance function pointers should use [CallConvMemberFunction] where applicable."),
        new HelpRow("generate-cpp-attributes", "[CppAttributeList(\"\")] should be generated to document the encountered C++ attributes."),
        new HelpRow("generate-disable-runtime-marshalling", "[assembly: DisableRuntimeMarshalling] should be generated."),
        new HelpRow("generate-doc-includes", "<include> xml documentation tags should be generated for declarations."),
        new HelpRow("generate-file-scoped-namespaces", "Namespaces should be scoped to the file to reduce nesting."),
        new HelpRow("generate-fixed-buffer-indexer-overloads", "Fixed sized buffer helper types should generate additional uint, nint, and nuint indexer overloads."),
        new HelpRow("generate-generated-code=<mode>", "Controls the emission of the GeneratedCode attribute. 'assembly' (default) emits a single '[assembly: GeneratedCode]' when helper types are generated; 'type' instead annotates each generated top-level type; 'none' emits neither."),
        new HelpRow("generate-guid-member", "Types with an associated GUID should have a corresponding member generated."),
        new HelpRow("generate-helper-types", "Code files should be generated for various helper attributes and declared transparent structs."),
        new HelpRow("generate-macro-bindings", "Bindings for macro-definitions should be generated. This currently only works with value like macros and not function-like ones."),
        new HelpRow("generate-marker-interfaces", "Bindings for marker interfaces representing native inheritance hierarchies should be generated."),
        new HelpRow("generate-native-bitfield-attribute", "[NativeBitfield(\"\", offset: #, length: #)] attribute should be generated to document the encountered bitfield layout."),
        new HelpRow("generate-native-inheritance-attribute", "[NativeInheritance(\"\")] attribute should be generated to document the encountered C++ base type."),
        new HelpRow("generate-generic-pointer-wrapper", "Pointer<T> should be used for limited generic type support."),
        new HelpRow("generate-setslastsystemerror-attribute", "[SetsLastSystemError] attribute should be generated rather than using SetLastError = true."),
        new HelpRow("generate-template-bindings", "Bindings for template-definitions should be generated. This is currently experimental."),
        new HelpRow("generate-unmanaged-constants", "Unmanaged constants should be generated using static ref readonly properties. This is currently experimental."),
        new HelpRow("generate-vtbl-index-attribute", "[VtblIndex(#)] attribute should be generated to document the underlying VTBL index for a helper method."),

        new HelpRow("", ""),
        new HelpRow("# Stripping Options", ""),
        new HelpRow("", ""),

        new HelpRow("strip-enum-member-type-name", "Strips the enum type name from the beginning of its member names."),

        new HelpRow("", ""),
        new HelpRow("# Logging Options", ""),
        new HelpRow("", ""),

        new HelpRow("log-exclusions", "A list of excluded declaration types should be generated. This will also log if the exclusion was due to an exact or partial match."),
        new HelpRow("log-potential-typedef-remappings", "A list of potential typedef remappings should be generated. This can help identify missing remappings."),
        new HelpRow("log-visited-files", "A list of the visited files should be generated. This can help identify traversal issues."),
    ];

    private static CommandLineOption Multi(string[] aliases, string description) => new(aliases, description, CommandLineOptionKind.MultipleValue);

    private static CommandLineOption Single(string[] aliases, string description, string defaultValue = "", string? valueName = null, string[]? allowedValues = null) => new(aliases, description, CommandLineOptionKind.SingleValue, valueName, defaultValue, allowedValues);

    private static CommandLineOption Flag(string[] aliases, string description) => new(aliases, description, CommandLineOptionKind.Flag);
}
