// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp;

internal static partial class Program
{
    private static readonly string[] s_additionalOptionAliases = ["--additional", "-a"];
    private static readonly string[] s_configOptionAliases = ["--config", "-c"];
    private static readonly string[] s_generateOptionAliases = ["--generate", "-g"];
    private static readonly string[] s_logOptionAliases = ["--log", "-lg"];
    private static readonly string[] s_defineMacroOptionAliases = ["--define-macro", "-D"];
    private static readonly string[] s_excludeOptionAliases = ["--exclude", "-e"];
    private static readonly string[] s_fileOptionAliases = ["--file", "-f"];
    private static readonly string[] s_fileDirectionOptionAliases = ["--file-directory", "-F"];
    private static readonly string[] s_headerOptionAliases = ["--header-file", "-hf"];
    private static readonly string[] s_includeOptionAliases = ["--include", "-i"];
    private static readonly string[] s_includeDirectoryOptionAliases = ["--include-directory", "-I"];
    private static readonly string[] s_languageOptionAliases = ["--language", "-x"];
    private static readonly string[] s_libraryOptionAliases = ["--library-path", "-l"];
    private static readonly string[] s_methodClassNameOptionAliases = ["--method-class-name", "-m"];
    private static readonly string[] s_namespaceOptionAliases = ["--namespace", "-n"];
    private static readonly string[] s_nativeTypeNamesStripOptionAliases = ["--native-type-names-to-strip"];
    private static readonly string[] s_outputModeOptionAliases = ["--output-mode", "-om"];
    private static readonly string[] s_outputOptionAliases = ["--output", "-o"];
    private static readonly string[] s_prefixStripOptionAliases = ["--prefix-strip", "-p"];
    private static readonly string[] s_typePrefixStripOptionAliases = ["--type-prefix-strip", "-tp"];
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
    private static readonly string[] s_withEqualityMembersOptionAliases = ["--with-equality-members", "-wem"];
    private static readonly string[] s_withGuidOptionAliases = ["--with-guid", "-wg"];
    private static readonly string[] s_withLengthOptionAliases = ["--with-length", "-wl"];
    private static readonly string[] s_withLibraryPathOptionAliases = ["--with-library-path", "-wlb"];
    private static readonly string[] s_withManualImportOptionAliases = ["--with-manual-import", "-wmi"];
    private static readonly string[] s_withNamespaceOptionAliases = ["--with-namespace", "-wn"];
    private static readonly string[] s_withPackingOptionAliases = ["--with-packing", "-wp"];
    private static readonly string[] s_withReadonlyOptionAliases = ["--with-readonly", "-wro"];
    private static readonly string[] s_withSetLastErrorOptionAliases = ["--with-setlasterror", "-wsle"];
    private static readonly string[] s_withSuppressGCTransitionOptionAliases = ["--with-suppressgctransition", "-wsgct"];
    private static readonly string[] s_withTransparentStructOptionAliases = ["--with-transparent-struct", "-wts"];
    private static readonly string[] s_withTypeOptionAliases = ["--with-type", "-wt"];
    private static readonly string[] s_withUsingOptionAliases = ["--with-using", "-wu"];
    private static readonly string[] s_withoutAccessSpecifierOptionAliases = ["--without-access-specifier"];
    private static readonly string[] s_withoutAttributeOptionAliases = ["--without-attribute"];
    private static readonly string[] s_withoutCallConvOptionAliases = ["--without-callconv"];
    private static readonly string[] s_withoutEnumMemberStripOptionAliases = ["--without-enum-member-strip"];
    private static readonly string[] s_withoutEqualityMembersOptionAliases = ["--without-equality-members"];
    private static readonly string[] s_withoutLibraryPathOptionAliases = ["--without-library-path"];
    private static readonly string[] s_withoutReadonlyOptionAliases = ["--without-readonly"];
    private static readonly string[] s_withoutSetLastErrorOptionAliases = ["--without-setlasterror"];
    private static readonly string[] s_withoutSuppressGCTransitionOptionAliases = ["--without-suppressgctransition"];
    private static readonly string[] s_withoutTypeOptionAliases = ["--without-type"];
    private static readonly string[] s_withoutUsingOptionAliases = ["--without-using"];
    private static readonly string[] s_helpOptionAliases = ["--help", "-?", "-h"];

    private static readonly CommandLineOption s_additionalOption = Multi(s_additionalOptionAliases, "An argument to pass to Clang when parsing the input files.");
    private static readonly CommandLineOption s_configOption = Multi(s_configOptionAliases, "A configuration option that controls how the bindings are generated. Specify 'help' to see the available options.");
    private static readonly CommandLineOption s_generateOption = Multi(s_generateOptionAliases, "A feature to generate, e.g. 'aggressive-inlining', 'tests-nunit', or 'generated-code=type'. Append '=false' to opt a feature back out. Specify '--config help' to see the available names.");
    private static readonly CommandLineOption s_logOption = Multi(s_logOptionAliases, "A diagnostic log to emit during generation: 'exclusions', 'potential-typedef-remappings', or 'visited-files'.");
    private static readonly CommandLineOption s_defineMacros = Multi(s_defineMacroOptionAliases, "Define <macro> to <value> (or 1 if <value> omitted).");
    private static readonly CommandLineOption s_excludedNames = Multi(s_excludeOptionAliases, "A declaration name to exclude from binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_files = Multi(s_fileOptionAliases, "A file to parse and generate bindings for.");
    private static readonly CommandLineOption s_fileDirectory = Single(s_fileDirectionOptionAliases, "The base path for files to parse.");
    private static readonly CommandLineOption s_headerFile = Single(s_headerOptionAliases, "A file which contains the header to prefix every generated file with.", deprecatedAliases: ["--headerFile"]);
    private static readonly CommandLineOption s_includedNames = Multi(s_includeOptionAliases, "A declaration name to include in binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_includeDirectories = Multi(s_includeDirectoryOptionAliases, "Add directory to include search path.");
    private static readonly CommandLineOption s_language = Single(s_languageOptionAliases, "Treat subsequent input files as having type <language>.", defaultValue: "c++", valueName: "c|c++|objective-c|objective-c++", allowedValues: ["c", "c++", "objective-c", "objective-c++"]);
    private static readonly CommandLineOption s_libraryPath = Single(s_libraryOptionAliases, "The string to use in the DllImport attribute used when generating bindings.", deprecatedAliases: ["--libraryPath"]);
    private static readonly CommandLineOption s_methodClassName = Single(s_methodClassNameOptionAliases, "The name of the static class that will contain the generated method bindings.", defaultValue: "Methods", deprecatedAliases: ["--methodClassName"]);
    private static readonly CommandLineOption s_namespaceName = Single(s_namespaceOptionAliases, "The namespace in which to place the generated bindings.");
    private static readonly CommandLineOption s_nativeTypeNamesToStrip = Multi(s_nativeTypeNamesStripOptionAliases, "The contents to strip from the generated NativeTypeName attributes.", deprecatedAliases: ["--nativeTypeNamesToStrip"]);
    private static readonly CommandLineOption s_outputMode = Single(s_outputModeOptionAliases, "The mode describing how the information collected from the headers are presented in the resultant bindings.", defaultValue: "CSharp", valueName: "CSharp|Xml");
    private static readonly CommandLineOption s_outputLocation = Single(s_outputOptionAliases, "The output location to write the generated bindings to.");
    private static readonly CommandLineOption s_methodPrefixToStrip = Single(s_prefixStripOptionAliases, "The prefix to strip from the generated method bindings.", deprecatedAliases: ["--prefixStrip"]);
    private static readonly CommandLineOption s_typePrefixToStrip = Single(s_typePrefixStripOptionAliases, "The prefix to strip from the generated enum, struct, and union type bindings (and their enum member names).", deprecatedAliases: ["--typePrefixStrip"]);
    private static readonly CommandLineOption s_remappedNameValuePairs = Multi(s_remapOptionAliases, "A declaration name to be remapped to another name during binding generation.");
    private static readonly CommandLineOption s_remappedTypeNameValuePairs = Multi(s_remapTypeOptionAliases, "A type (record or enum) declaration name to be remapped to another name during binding generation. Takes precedence over --remap and is useful when a type and field share a name.");
    private static readonly CommandLineOption s_remappedFieldNameValuePairs = Multi(s_remapFieldOptionAliases, "A field declaration name to be remapped to another name during binding generation. Takes precedence over --remap and is useful when a type and field share a name.");
    private static readonly CommandLineOption s_resourceDirectory = Single(s_resourceDirectoryOptionAliases, "The Clang resource directory containing the builtin headers (such as stddef.h). When omitted, an installed and version-matched Clang's resource directory is automatically detected.", valueName: "directory");
    private static readonly CommandLineOption s_resourceDirectoryDetectionDisabled = Flag(s_resourceDirectoryDetectionOptionAliases, "Disable the automatic detection of the Clang resource directory.");
    private static readonly CommandLineOption s_std = Single(s_stdOptionAliases, "Language standard to compile for.");
    private static readonly CommandLineOption s_testOutputLocation = Single(s_testOutputOptionAliases, "The output location to write the generated tests to.");
    private static readonly CommandLineOption s_traversalNames = Multi(s_traverseOptionAliases, "A file name included either directly or indirectly by -f that should be traversed during binding generation.");
    private static readonly CommandLineOption s_versionOption = Flag(s_versionOptionAliases, "Prints the current version information for the tool and its native dependencies.");
    private static readonly CommandLineOption s_withAccessSpecifierNameValuePairs = Multi(s_withAccessSpecifierOptionAliases, "An access specifier to be used with the given qualified or remapped declaration name during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_withAttributeNameValuePairs = Multi(s_withAttributeOptionAliases, "An attribute to be added to the given remapped declaration name during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_withCallConvNameValuePairs = Multi(s_withCallConvOptionAliases, "A calling convention to be used for the given declaration during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_withClassNameValuePairs = Multi(s_withClassOptionAliases, "A class to be used for the given remapped constant or function declaration name during binding generation. Supports a trailing `*` wildcard for prefix matching; an exact match takes precedence.");
    private static readonly CommandLineOption s_withConditional = Single(s_withConditionalOptionAliases, "A preprocessor symbol used to wrap single-file C# output in a leading '#if <symbol>' and trailing '#endif'. Useful when files can't be conditionally excluded at the project level (e.g. Unity).", valueName: "symbol");
    private static readonly CommandLineOption s_withEnumMemberStripNameValuePairs = Multi(s_withEnumMemberStripOptionAliases, "How to strip a prefix or suffix from the members of the given remapped enum name during binding generation. Mode is one of `none`, `common-prefix`, `common-suffix`, `type-name`, `prefix:<str>`, or `suffix:<str>`. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_withEqualityMembers = Multi(s_withEqualityMembersOptionAliases, "Generate IEquatable<T> with field-wise Equals, GetHashCode, and the == and != operators for the given struct. Opt-in and not valid for every native type; a named struct also opts in the nested and base structs it compares. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_withGuidNameValuePairs = Multi(s_withGuidOptionAliases, "A GUID to be used for the given declaration during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_withLengthNameValuePairs = Multi(s_withLengthOptionAliases, "A length to be used for the given declaration during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_withLibraryPathNameValuePairs = Multi(s_withLibraryPathOptionAliases, "A library path to be used for the given declaration during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.", deprecatedAliases: ["--with-librarypath"]);
    private static readonly CommandLineOption s_withManualImports = Multi(s_withManualImportOptionAliases, "A remapped function name to be treated as a manual import during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_withNamespaceNameValuePairs = Multi(s_withNamespaceOptionAliases, "A namespace to be used for the given remapped declaration name during binding generation. Supports a trailing `*` wildcard for prefix matching; an exact match takes precedence.");
    private static readonly CommandLineOption s_withPackingNameValuePairs = Multi(s_withPackingOptionAliases, "Overrides the StructLayoutAttribute.Pack property for the given type. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_withReadonlys = Multi(s_withReadonlyOptionAliases, "Add the readonly modifier to a given instance method. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_withSetLastErrors = Multi(s_withSetLastErrorOptionAliases, "Add the SetLastError=true modifier or SetsSystemLastError attribute to a given DllImport or UnmanagedFunctionPointer. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_withSuppressGCTransitions = Multi(s_withSuppressGCTransitionOptionAliases, "Add the SuppressGCTransition calling convention to a given DllImport or UnmanagedFunctionPointer. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_withTransparentStructNameValuePairs = Multi(s_withTransparentStructOptionAliases, "A remapped type name to be treated as a transparent wrapper during binding generation. Matched by exact name only.");
    private static readonly CommandLineOption s_withTypeNameValuePairs = Multi(s_withTypeOptionAliases, "A type to be used for the given enum declaration, macro constant, or struct field (using the qualified `Type.field`) during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_withUsingNameValuePairs = Multi(s_withUsingOptionAliases, "A using directive to be included for the given remapped declaration name during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence.");
    private static readonly CommandLineOption s_withoutAccessSpecifiers = Multi(s_withoutAccessSpecifierOptionAliases, "A declaration name to opt back out of a '--with-access-specifier *' catch-all.");
    private static readonly CommandLineOption s_withoutAttributes = Multi(s_withoutAttributeOptionAliases, "A declaration name to opt back out of a '--with-attribute *' catch-all.");
    private static readonly CommandLineOption s_withoutCallConvs = Multi(s_withoutCallConvOptionAliases, "A declaration name to opt back out of a '--with-callconv *' catch-all.");
    private static readonly CommandLineOption s_withoutEnumMemberStrip = Multi(s_withoutEnumMemberStripOptionAliases, "An enum name to opt back out of a '--with-enum-member-strip *' catch-all.");
    private static readonly CommandLineOption s_withoutEqualityMembers = Multi(s_withoutEqualityMembersOptionAliases, "A struct name to opt back out of a '--with-equality-members *' catch-all.");
    private static readonly CommandLineOption s_withoutLibraryPaths = Multi(s_withoutLibraryPathOptionAliases, "A declaration name to opt back out of a '--with-library-path *' catch-all.");
    private static readonly CommandLineOption s_withoutReadonlys = Multi(s_withoutReadonlyOptionAliases, "A method name to opt back out of a '--with-readonly *' catch-all.");
    private static readonly CommandLineOption s_withoutSetLastErrors = Multi(s_withoutSetLastErrorOptionAliases, "A declaration name to opt back out of a '--with-setlasterror *' catch-all.");
    private static readonly CommandLineOption s_withoutSuppressGCTransitions = Multi(s_withoutSuppressGCTransitionOptionAliases, "A declaration name to opt back out of a '--with-suppressgctransition *' catch-all.");
    private static readonly CommandLineOption s_withoutTypes = Multi(s_withoutTypeOptionAliases, "A declaration name to opt back out of a '--with-type *' catch-all.");
    private static readonly CommandLineOption s_withoutUsings = Multi(s_withoutUsingOptionAliases, "A declaration name to opt back out of a '--with-using *' catch-all.");
    private static readonly CommandLineOption s_helpOption = Flag(s_helpOptionAliases, "Show help and usage information");

    private static readonly CommandLineOption[] s_options =
    [
        s_additionalOption,
        s_configOption,
        s_generateOption,
        s_logOption,
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
        s_withEqualityMembers,
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
        s_withoutAccessSpecifiers,
        s_withoutAttributes,
        s_withoutCallConvs,
        s_withoutEnumMemberStrip,
        s_withoutEqualityMembers,
        s_withoutLibraryPaths,
        s_withoutReadonlys,
        s_withoutSetLastErrors,
        s_withoutSuppressGCTransitions,
        s_withoutTypes,
        s_withoutUsings,
        s_helpOption,
    ];

    private static readonly CommandLineParser s_parser = new(s_options);

    private static readonly HelpRow[] s_configOptions =
    [
        new HelpRow("?, h, help", "Show help and usage information for -c, --config, --generate, and --log"),

        new HelpRow("", ""),
        new HelpRow("# -c, --config now carries only the four mode families below (plus this help). The", ""),
        new HelpRow("# feature switches moved to --generate <name> and the diagnostics to --log <name>.", ""),
        new HelpRow("# Boolean --generate features accept an optional '=true'/'=false' ('=true' is implied", ""),
        new HelpRow("# when omitted), so a later response file can override an earlier one. Valued switches", ""),
        new HelpRow("# take the value shown as name=<value>.", ""),

        new HelpRow("", ""),
        new HelpRow("# Mode Families (-c <name>=<value>)", ""),
        new HelpRow("", ""),

        new HelpRow("codegen=<value>", "Which .NET/C# level to target: 'compatible' (.NET Standard 2.0), 'default' (current LTS; .NET 8/C# 12), 'latest' (current STS; .NET 10/C# 14), or 'preview'. Defaults to 'default'."),
        new HelpRow("file=<value>", "How output is split: 'single' (one output file; the default) or 'multi' (approximately one type per file)."),
        new HelpRow("types=<value>", "Which platform defaults to assume: 'windows' or 'unix'. Defaults to the host platform."),
        new HelpRow("vtbls=<value>", "How VTBLs are generated: 'explicit' (a named field per entry), 'implicit' (the default; reduces metadata bloat), or 'trimmable' (defined but unused in helpers to reduce bloat when trimming)."),

        new HelpRow("", ""),
        new HelpRow("# Test Generation (--generate <name>)", ""),
        new HelpRow("", ""),

        new HelpRow("tests-nunit", "Basic tests validating size, blittability, and associated metadata should be generated for NUnit."),
        new HelpRow("tests-xunit", "Basic tests validating size, blittability, and associated metadata should be generated for XUnit."),

        new HelpRow("", ""),
        new HelpRow("# Generation Features (--generate <name>[=false])", ""),
        new HelpRow("", ""),

        new HelpRow("aggressive-inlining", "[MethodImpl(MethodImplOptions.AggressiveInlining)] should be added to generated helper functions."),
        new HelpRow("anonymous-field-helpers", "The helper ref properties for fields in nested anonymous structs and unions should be generated. On by default; use =false to opt out."),
        new HelpRow("callconv-member-function", "Instance function pointers should use [CallConvMemberFunction] where applicable."),
        new HelpRow("com-proxies", "Types recognized as COM proxies should have bindings generated. These are currently function declarations ending with _UserFree, _UserMarshal, _UserSize, _UserUnmarshal, _Proxy, or _Stub. On by default; use =false to opt out."),
        new HelpRow("cpp-attributes", "[CppAttributeList(\"\")] should be generated to document the encountered C++ attributes."),
        new HelpRow("default-remappings", "Default remappings for well known types should be added. This currently includes intptr_t, ptrdiff_t, size_t, ssize_t, uintptr_t, and the exact-width stdint types (int8_t, int16_t, int32_t, int64_t, uint8_t, uint16_t, uint32_t, and uint64_t). When targeting Windows, the pointer-width Windows types (INT_PTR, LONG_PTR, SSIZE_T, DWORD_PTR, SIZE_T, UINT_PTR, and ULONG_PTR) and _GUID are also included. On by default; use =false to opt out."),
        new HelpRow("disable-runtime-marshalling", "[assembly: DisableRuntimeMarshalling] should be generated."),
        new HelpRow("doc-includes", "<include> xml documentation tags should be generated for declarations."),
        new HelpRow("empty-records", "Bindings for records that contain no members should be generated. These are commonly encountered for opaque handle like types such as HWND. On by default; use =false to opt out."),
        new HelpRow("enum-member-type-name", "The enum type name should be kept at the beginning of its member names. On by default; use =false to strip it."),
        new HelpRow("enum-operators", "Bindings for operators over enum types should be generated. These are largely unnecessary in C# as the operators are available by default. On by default; use =false to opt out."),
        new HelpRow("extern-variables", "Top-level extern/extern const global variables should be surfaced as settable pointer fields on a <Class>ManualImports struct for the consumer to resolve (like --with-manual-import). Opt-in; pointer and primitive types only."),
        new HelpRow("file-scoped-namespaces", "Namespaces should be scoped to the file to reduce nesting."),
        new HelpRow("fixed-buffer-indexer-overloads", "Fixed sized buffer helper types should generate additional uint, nint, and nuint indexer overloads."),
        new HelpRow("fnptr-codegen", "Generated bindings for latest or preview codegen should use function pointers. On by default; use =false to opt out."),
        new HelpRow("funcs-with-body", "Bindings for functions with bodies should be generated. On by default; use =false to opt out."),
        new HelpRow("generated-code=<mode>", "Controls the emission of the GeneratedCode attribute. 'assembly' (default) emits a single '[assembly: GeneratedCode]' when helper types are generated; 'type' instead annotates each generated top-level type; 'none' emits neither."),
        new HelpRow("generic-pointer-wrapper", "Pointer<T> should be used for limited generic type support."),
        new HelpRow("guid-member", "Types with an associated GUID should have a corresponding member generated."),
        new HelpRow("helper-types", "Code files should be generated for various helper attributes and declared transparent structs."),
        new HelpRow("macro-bindings", "Bindings for macro-definitions should be generated. This currently only works with value like macros and not function-like ones."),
        new HelpRow("marker-interfaces", "Bindings for marker interfaces representing native inheritance hierarchies should be generated."),
        new HelpRow("native-alignment-attribute", "[NativeAlignment(#)] attribute should be generated to document the requested over-alignment (__declspec(align) / DECLSPEC_ALIGN) that .NET cannot honor."),
        new HelpRow("native-bitfield-attribute", "[NativeBitfield(\"\", offset: #, length: #)] attribute should be generated to document the encountered bitfield layout."),
        new HelpRow("native-inheritance-attribute", "[NativeInheritance(\"\")] attribute should be generated to document the encountered C++ base type."),
        new HelpRow("nint-codegen", "Generated bindings should use nint/nuint where applicable. On by default; use =false to opt out."),
        new HelpRow("objective-c-bindings", "Bindings for Objective-C declarations (currently @protocol types) should be generated. This is experimental and requires the Objective-C runtime (libobjc) at runtime."),
        new HelpRow("setslastsystemerror-attribute", "[SetsLastSystemError] attribute should be generated rather than using SetLastError = true."),
        new HelpRow("template-bindings", "Bindings for template-definitions should be generated. This is currently experimental."),
        new HelpRow("unmanaged-constants", "Unmanaged constants should be generated using static ref readonly properties. This is currently experimental."),
        new HelpRow("using-statics-for-enums", "Enum usages should include a corresponding 'using static EnumName;' rather than being fully qualified. On by default; use =false to opt out."),
        new HelpRow("using-statics-for-guid-members", "GUID member usages should include a corresponding 'using static' rather than being fully qualified. On by default; use =false to opt out."),
        new HelpRow("vtbl-index-attribute", "[VtblIndex(#)] attribute should be generated to document the underlying VTBL index for a helper method."),

        new HelpRow("", ""),
        new HelpRow("# Diagnostic Logs (--log <name>)", ""),
        new HelpRow("", ""),

        new HelpRow("exclusions", "A list of excluded declaration types should be generated. This will also log if the exclusion was due to an exact or partial match."),
        new HelpRow("potential-typedef-remappings", "A list of potential typedef remappings should be generated. This can help identify missing remappings."),
        new HelpRow("visited-files", "A list of the visited files should be generated. This can help identify traversal issues."),

        new HelpRow("", ""),
        new HelpRow("# Legacy/Deprecated -c spellings (still accepted, but each emits a deprecation warning)", ""),
        new HelpRow("", ""),

        new HelpRow("-c compatible-codegen", "Use -c codegen=compatible."),
        new HelpRow("-c default-codegen", "Use -c codegen=default."),
        new HelpRow("-c latest-codegen", "Use -c codegen=latest."),
        new HelpRow("-c preview-codegen", "Use -c codegen=preview."),
        new HelpRow("-c single-file", "Use -c file=single."),
        new HelpRow("-c multi-file", "Use -c file=multi."),
        new HelpRow("-c windows-types", "Use -c types=windows."),
        new HelpRow("-c unix-types", "Use -c types=unix."),
        new HelpRow("-c explicit-vtbls", "Use -c vtbls=explicit."),
        new HelpRow("-c implicit-vtbls", "Use -c vtbls=implicit."),
        new HelpRow("-c trimmable-vtbls", "Use -c vtbls=trimmable."),
        new HelpRow("-c generate-*", "Use --generate * (e.g. -c generate-helper-types becomes --generate helper-types)."),
        new HelpRow("-c log-*", "Use --log * (e.g. -c log-visited-files becomes --log visited-files)."),
        new HelpRow("-c exclude-anonymous-field-helpers", "Use --generate anonymous-field-helpers=false."),
        new HelpRow("-c exclude-com-proxies", "Use --generate com-proxies=false."),
        new HelpRow("-c exclude-default-remappings", "Use --generate default-remappings=false."),
        new HelpRow("-c no-default-remappings", "Use --generate default-remappings=false."),
        new HelpRow("-c default-remappings", "Use --generate default-remappings (or =true)."),
        new HelpRow("-c exclude-empty-records", "Use --generate empty-records=false."),
        new HelpRow("-c exclude-enum-operators", "Use --generate enum-operators=false."),
        new HelpRow("-c exclude-fnptr-codegen", "Use --generate fnptr-codegen=false."),
        new HelpRow("-c exclude-funcs-with-body", "Use --generate funcs-with-body=false."),
        new HelpRow("-c exclude-nint-codegen", "Use --generate nint-codegen=false."),
        new HelpRow("-c exclude-using-statics-for-enums", "Use --generate using-statics-for-enums=false."),
        new HelpRow("-c dont-use-using-statics-for-enums", "Use --generate using-statics-for-enums=false."),
        new HelpRow("-c exclude-using-statics-for-guid-members", "Use --generate using-statics-for-guid-members=false."),
        new HelpRow("-c dont-use-using-statics-for-guid-members", "Use --generate using-statics-for-guid-members=false."),
        new HelpRow("-c strip-enum-member-type-name", "Use --generate enum-member-type-name=false."),
    ];

    private static CommandLineOption Multi(string[] aliases, string description, string[]? deprecatedAliases = null) => new(aliases, description, CommandLineOptionKind.MultipleValue, deprecatedAliases: deprecatedAliases);

    private static CommandLineOption Single(string[] aliases, string description, string defaultValue = "", string? valueName = null, string[]? allowedValues = null, string[]? deprecatedAliases = null) => new(aliases, description, CommandLineOptionKind.SingleValue, valueName, defaultValue, allowedValues, deprecatedAliases);

    private static CommandLineOption Flag(string[] aliases, string description, string[]? deprecatedAliases = null) => new(aliases, description, CommandLineOptionKind.Flag, deprecatedAliases: deprecatedAliases);
}
