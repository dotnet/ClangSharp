# ClangSharp

ClangSharp provides Clang bindings written in C#. It is self-hosted and auto-generates itself by parsing the Clang C header files using ClangSharpPInvokeGenerator.

[![ci](https://github.com/dotnet/ClangSharp/actions/workflows/ci.yml/badge.svg?branch=main&event=push)](https://github.com/dotnet/ClangSharp/actions/workflows/ci.yml)

A nuget package for the project is provided here: https://www.nuget.org/packages/clangsharp.
A .NET tool for the P/Invoke generator project is provided here: https://www.nuget.org/packages/ClangSharpPInvokeGenerator

**NOTE:** If you are running as a dotnet tool, you may need to manually copy the appropriate DLLs from NuGet due to limitations in the dotnet tool support.

A convenience package which provides the native libClang library for several platforms is provided here: https://www.nuget.org/packages/libclang

A helper package which exposes many Clang APIs missing from libClang is provided here: https://www.nuget.org/packages/libClangSharp

**NOTE:** libclang and libClangSharp are meta-packages which point to the platform-specific runtime packages ([e.g.](https://www.nuget.org/packages/libClangSharp.runtime.win-x64/22.1.8); see others owned by [tannergooding](https://www.nuget.org/profiles/tannergooding)). Several manual steps may be required to use them, see discussion in [#46](https://github.com/dotnet/ClangSharp/issues/46) and [#118](https://github.com/dotnet/ClangSharp/issues/118).

Source browsing is available via: https://source.clangsharp.dev/

## Table of Contents

* [Code of Conduct](#code-of-conduct)
* [License](#license)
* [Features](#features)
* [Building Managed](#building-managed)
* [Building Native](#building-native)
* [Regenerating native binaries](#regenerating-native-binaries)
* [Generating Bindings](#generating-bindings)
  * [Best practices](docs/generating-bindings-best-practices.md)
  * [XML binding format](docs/xml-binding-format.md)
* [Using as a Library](#using-as-a-library)
* [Using locally built versions](#using-locally-built-versions)
* [Spotlight](#spotlight)

### Code of Conduct

ClangSharp and everyone contributing (this includes issues, pull requests, the
wiki, etc) must abide by the .NET Foundation Code of Conduct:
https://dotnetfoundation.org/about/code-of-conduct.

Instances of abusive, harassing, or otherwise unacceptable behavior may be
reported by contacting the project team at conduct@dotnetfoundation.org.

### License

Copyright (c) .NET Foundation and Contributors. All Rights Reserved.
Licensed under the MIT License (MIT).
See [LICENSE.md](LICENSE.md) in the repository root for more information.

### Features

 * Auto-generated using Clang C headers files, and supports all functionality exposed by them ~ which means you can build tooling around C/C++
 * Exposes the raw unsafe API for performance
 * Exposes a slightly higher abstraction that is type safe (CXIndex and CXTranslationUnit are different types, despite being pointers internally)
 * Exposes an again slightly higher abstraction that tries to mirror the Clang C++ Type Hierarchy where possible
 * Nearly identical to the Clang C APIs, e.g. `clang_getDiagnosticSpelling` in C, vs. `clang.getDiagnosticSpelling` (notice the . in the C# API)

### Building Managed

ClangSharp requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and can be built simply with `dotnet build -c Release`.

You can reproduce what the CI environment does by running `./scripts/cibuild.cmd` on Windows or `./scripts.cibuild.sh` on Unix.
This will download the required .NET SDK locally and use that to build the repo; it will also run through all available actions in the appropriate order.

There are also several build scripts in the repository root. On Windows these scripts end with `.cmd` and expect arguments with a `-` prefix. On Unix these scripts end with `.sh` and expect arguments with a `--` prefix.
By default, each script performs only the action specified in its name (i.e. `restore` only restores, `build` only builds, `test` only tests, and `pack` only packs). You can specify additional actions to be run by passing that name as an argument to the script (e.g. `build.cmd -restore` will perform a package restore and build; `test.cmd -pack` will run tests and package artifacts).
Certain actions are dependent on a previous action having been run at least once. `build` depends on `restore`, `test` depends on `build`, and `pack` depends on `build`. This means the recommended first time action is `build -restore`.

You can see any additional options that are available by passing `-help` on Windows or `--help` on Unix to the available build scripts.

### Building Native

ClangSharp provides a helper library, `libClangSharp`, that exposes additional functionality that is not available in `libClang`.
Building this requires [CMake 3.13 or later](https://cmake.org/download/) as well as a version of MSVC or Clang that supports C++ 17.

To successfully build `libClangSharp` you must first build Clang (https://clang.llvm.org/get_started.html).

#### Windows

The process done on Windows is roughly:
```cmd
git clone --single-branch --branch llvmorg-22.1.8 https://github.com/llvm/llvm-project
cd llvm-project
mkdir artifacts/bin
cd artifacts/bin
cmake -DCMAKE_INSTALL_PREFIX=../install -DLLVM_ENABLE_PROJECTS=clang -G "Visual Studio 17 2022" -A x64 -Thost=x64 ../../llvm
```

You can then open `LLVM.slnx` in Visual Studio, change the configuration to `Release` and build the `INSTALL` project. You may need to build the `ALL_BUILD` first.

Afterwards, you can then build `libClangSharp` where the process followed is roughly:
```cmd
git clone https://github.com/dotnet/clangsharp
cd clangsharp
mkdir artifacts/bin/native
cd artifacts/bin/native
cmake -DCMAKE_INSTALL_PREFIX=../install -DPATH_TO_LLVM=absolute/path/to/repos/llvm-project/artifacts/install -G "Visual Studio 17 2022" -A x64 -Thost=x64 ../../..
```

You can then open `libClangSharp.slnx` in Visual Studio, change the configuration to `Release` and build the `INSTALL` project.

#### Linux

The process done on Linux is roughly:
```bash
git clone --single-branch --branch llvmorg-22.1.8 https://github.com/llvm/llvm-project
cd llvm-project
mkdir -p artifacts/bin
cd artifacts/bin
cmake -DCMAKE_BUILD_TYPE=Release -DCMAKE_INSTALL_PREFIX=../install -DLLVM_ENABLE_PROJECTS=clang ../../llvm
make install
```

If you want your Linux build to be portable, you may also consider specifying the following options:
* `-DLLVM_ENABLE_TERMINFO=OFF`
* `-DLLVM_ENABLE_ZLIB=OFF`
* `-DLLVM_ENABLE_ZSTD=OFF`
* `-DLLVM_STATIC_LINK_CXX_STDLIB=ON`

If you would prefer to use `Ninja`, then make sure to pass in `-G Ninja` and then invoke `ninja` rather than `make install`.

Afterwards, you can then build `libClangSharp` where the process followed is roughly:
```cmd
git clone https://github.com/dotnet/clangsharp
cd clangsharp
mkdir -p artifacts/bin/native
cd artifacts/bin/native
cmake -DCMAKE_BUILD_TYPE=Release -DCMAKE_INSTALL_PREFIX=../install -DPATH_TO_LLVM=../../../../llvm-project/artifacts/install ../../../
make install
```

### Regenerating native binaries

The native runtime packages are produced by CI (`.github/workflows/regenerate-native.yml`) rather than by hand:

* `libclang.runtime.*` (and the `libclang` meta-package) — the prebuilt `libclang` shared library, lifted directly from the matching official LLVM release.
* `libClangSharp.runtime.*` (and the `libClangSharp` meta-package) — the `libClangSharp` helper, compiled from `sources/libClangSharp` against that same LLVM release.

The tracked LLVM version is the `project(ClangSharp VERSION X.Y.Z)` value in the top-level `CMakeLists.txt`, which maps to the `llvmorg-X.Y.Z` release tag. For each runtime (`win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-arm64`) the download-and-stage step is handled by `scripts/build.ps1`/`scripts/build.sh`:

```
# lift the prebuilt libclang out of the matching LLVM release
./scripts/build.sh --regeneratenative --target libclang --rid linux-x64

# compile libClangSharp against that same LLVM release
./scripts/build.sh --regeneratenative --target libClangSharp --rid linux-x64
```

The change-detection and version-verification the workflow gates on live in the same scripts, so they can be reproduced locally: `./scripts/build.sh --detectchanges --base <ref>` prints which families a range of commits regenerates, and `./scripts/build.sh --verifypackages` checks the nuspec/`runtime.json` versions match the tracked LLVM version.

The staged binary is written to `artifacts/native/<rid>/`. The LLVM release distribution bundles both the prebuilt `libclang` and the `lib/cmake/{llvm,clang}` config, headers, and import libraries used as `PATH_TO_LLVM` when building `libClangSharp`, so no from-source LLVM build is required (the manual [Building Native](#building-native) steps remain the way to reproduce a build locally). Because lifting `libclang` is just unpacking prebuilt binaries, the workflow does all five runtimes on a single Windows runner (its bundled `bsdtar` handles `.tar.xz`); `libClangSharp` is compiled per-runtime on native runners.

The `win-arm64` runtime is compiled with the LLVM release's own `clang-cl` (via `-G Ninja`) rather than MSVC. The official LLVM binaries are clang-built, and on Arm64 clang and MSVC disagree on the record layout of over-aligned non-POD base classes (e.g. `clang::TemplateSpecializationType`), so an MSVC-built shim reads members of clang-built types at the wrong offset. The other runtimes are unaffected (`win-x64` layouts agree between the two, and the Unix runtimes already use clang).

The jobs run when:

* **libclang** — the tracked LLVM major/minor version changes, or the workflow is dispatched manually with the `libclang` input set.
* **libClangSharp** — the same LLVM version change, or anything under `sources/libClangSharp/` changes, or the workflow is dispatched manually with the `libclangsharp` input set.

Before anything is downloaded, the workflow verifies the `packages/**/*.nuspec` and `packages/**/runtime.json` versions match the tracked LLVM version (libclang exactly; libClangSharp as `<llvm-version>.<revision>`), so a version bump that forgot to update a package fails fast. Each job uploads the resulting `.nupkg` files as build artifacts and signs them; publishing them to NuGet.org is a manual step. To move to a new LLVM release, bump the version in `CMakeLists.txt` alongside the `<version>` in the affected `packages/**/*.nuspec` files (and the versions in `packages/**/runtime.json` and this README); pushing that change regenerates the packages.

### Generating Bindings

This program will take a given set of C or C++ header files and generate C# bindings from them. It is still a work-in-progress and not every declaration can have bindings generated today (contributions are welcome).

The simplest and recommended setup is to install the generator as a .NET tool and then use response files:
```
dotnet tool install --global ClangSharpPInvokeGenerator
ClangSharpPInvokeGenerator @generate.rsp
```

A response file allows you to specify and checkin the command line arguments in a text file, with one argument per line. For example: https://github.com/dotnet/ClangSharp/blob/main/sources/ClangSharpPInvokeGenerator/Properties/GenerateClang.rsp
At a minimum, the command line expects one or more input files (`-f`), an output namespace (`-n`), and an output location (`-o`). A typical response file may also specify explicit files to traverse, configuration options, name remappings, and other fixups.

For an opinionated walkthrough of how to structure a real generation project — response-file composition, the key options and when to use them, incremental regeneration, and common pitfalls — see [Generating bindings: best practices](docs/generating-bindings-best-practices.md).

The generator can also emit its bindings as XML rather than C# via `--output-mode Xml`; the shape of that output is described in [The XML binding format](docs/xml-binding-format.md).

The full set of available switches:
```
ClangSharpPInvokeGenerator
  ClangSharp P/Invoke Binding Generator

Usage:
  ClangSharpPInvokeGenerator [options]

Options:
  -a, --additional <additional>                                     An argument to pass to Clang when parsing the input files. []
  -c, --config <config>                                             A configuration option that controls how the bindings are generated. Specify 'help' to see the available options. []
  -g, --generate <generate>                                         A feature to generate, e.g. 'aggressive-inlining', 'tests-nunit', or 'generated-code=type'. Append '=false' to opt a feature back out. Specify '--config help' to see the available names. []
  -lg, --log <log>                                                  A diagnostic log to emit during generation: 'exclusions', 'potential-typedef-remappings', or 'visited-files'. []
  -D, --define-macro <define-macro>                                 Define <macro> to <value> (or 1 if <value> omitted). []
  -e, --exclude <exclude>                                           A declaration name to exclude from binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -f, --file <file>                                                 A file to parse and generate bindings for. []
  -F, --file-directory <file-directory>                             The base path for files to parse. []
  -hf, --header-file <header-file>                                  A file which contains the header to prefix every generated file with. []
  -i, --include <include>                                           A declaration name to include in binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -I, --include-directory <include-directory>                       Add directory to include search path. []
  -x, --language <c|c++|objective-c|objective-c++>                  Treat subsequent input files as having type <language>. [default: c++]
  -l, --library-path <library-path>                                 The string to use in the DllImport attribute used when generating bindings. []
  -m, --method-class-name <method-class-name>                       The name of the static class that will contain the generated method bindings. [default: Methods]
  -n, --namespace <namespace>                                       The namespace in which to place the generated bindings. []
  -om, --output-mode <CSharp|Xml>                                   The mode describing how the information collected from the headers are presented in the resultant bindings. [default: CSharp]
  -o, --output <output>                                             The output location to write the generated bindings to. []
  -p, --prefix-strip <prefix-strip>                                 The prefix to strip from the generated method bindings. []
  -tp, --type-prefix-strip <type-prefix-strip>                      The prefix to strip from the generated enum, struct, and union type bindings (and their enum member names). []
  --native-type-names-to-strip <native-type-names-to-strip>         The contents to strip from the generated NativeTypeName attributes. []
  -r, --remap <remap>                                               A declaration name to be remapped to another name during binding generation. []
  -rt, --remap-type <remap-type>                                    A type (record or enum) declaration name to be remapped to another name during binding generation. Takes precedence over --remap and is useful when a type and field share a name. []
  -rf, --remap-field <remap-field>                                  A field declaration name to be remapped to another name during binding generation. Takes precedence over --remap and is useful when a type and field share a name. []
  -rd, --resource-directory <directory>                             The Clang resource directory containing the builtin headers (such as stddef.h). When omitted, an installed and version-matched Clang's resource directory is automatically detected. []
  --no-resource-directory-detection                                 Disable the automatic detection of the Clang resource directory.
  -std, --std <std>                                                 Language standard to compile for. []
  -to, --test-output <test-output>                                  The output location to write the generated tests to. []
  -t, --traverse <traverse>                                         A file name included either directly or indirectly by -f that should be traversed during binding generation. []
  -v, --version                                                     Prints the current version information for the tool and its native dependencies.
  -was, --with-access-specifier <with-access-specifier>             An access specifier to be used with the given qualified or remapped declaration name during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wa, --with-attribute <with-attribute>                            An attribute to be added to the given remapped declaration name during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wcc, --with-callconv <with-callconv>                             A calling convention to be used for the given declaration during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wc, --with-class <with-class>                                    A class to be used for the given remapped constant or function declaration name during binding generation. Supports a trailing `*` wildcard for prefix matching; an exact match takes precedence. []
  -wcond, --with-conditional <symbol>                               A preprocessor symbol used to wrap single-file C# output in a leading '#if <symbol>' and trailing '#endif'. Useful when files can't be conditionally excluded at the project level (e.g. Unity). []
  -wcfv, --with-constant-folded-value <with-constant-folded-value>  Emit the clang-evaluated constant value for the given declaration instead of translating the written initializer expression. Useful when an initializer references companion declarations that aren't themselves generated. Applies to enum members (matched by the qualified `Enum.Member`, so `Enum.*` folds every member) and to macro or const value declarations. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wems, --with-enum-member-strip <with-enum-member-strip>          How to strip a prefix or suffix from the members of the given remapped enum name during binding generation. Mode is one of `none`, `common-prefix`, `common-suffix`, `type-name`, `prefix:<str>`, or `suffix:<str>`. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wem, --with-equality-members <with-equality-members>             Generate IEquatable<T> with field-wise Equals, GetHashCode, and the == and != operators for the given struct. Opt-in and not valid for every native type; a named struct also opts in the nested and base structs it compares. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wg, --with-guid <with-guid>                                      A GUID to be used for the given declaration during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wl, --with-length <with-length>                                  A length to be used for the given declaration during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wlb, --with-library-path <with-library-path>                     A library path to be used for the given declaration during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wmi, --with-manual-import <with-manual-import>                   A remapped function name to be treated as a manual import during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wn, --with-namespace <with-namespace>                            A namespace to be used for the given remapped declaration name during binding generation. Supports a trailing `*` wildcard for prefix matching; an exact match takes precedence. []
  -wp, --with-packing <with-packing>                                Overrides the StructLayoutAttribute.Pack property for the given type. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wro, --with-readonly <with-readonly>                             Add the readonly modifier to a given instance method. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wsle, --with-setlasterror <with-setlasterror>                    Add the SetLastError=true modifier or SetsSystemLastError attribute to a given DllImport or UnmanagedFunctionPointer. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wsgct, --with-suppressgctransition <with-suppressgctransition>   Add the SuppressGCTransition calling convention to a given DllImport or UnmanagedFunctionPointer. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wts, --with-transparent-struct <with-transparent-struct>         A remapped type name to be treated as a transparent wrapper during binding generation. Matched by exact name only. []
  -wt, --with-type <with-type>                                      A type to be used for the given enum declaration, macro constant, or struct field (using the qualified `Type.field`) during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wu, --with-using <with-using>                                    A using directive to be included for the given remapped declaration name during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  --without-access-specifier <without-access-specifier>             A declaration name to opt back out of a '--with-access-specifier *' catch-all. []
  --without-attribute <without-attribute>                           A declaration name to opt back out of a '--with-attribute *' catch-all. []
  --without-callconv <without-callconv>                             A declaration name to opt back out of a '--with-callconv *' catch-all. []
  --without-constant-folded-value <without-constant-folded-value>   A declaration name to opt back out of a '--with-constant-folded-value *' catch-all. []
  --without-enum-member-strip <without-enum-member-strip>           An enum name to opt back out of a '--with-enum-member-strip *' catch-all. []
  --without-equality-members <without-equality-members>             A struct name to opt back out of a '--with-equality-members *' catch-all. []
  --without-library-path <without-library-path>                     A declaration name to opt back out of a '--with-library-path *' catch-all. []
  --without-readonly <without-readonly>                             A method name to opt back out of a '--with-readonly *' catch-all. []
  --without-setlasterror <without-setlasterror>                     A declaration name to opt back out of a '--with-setlasterror *' catch-all. []
  --without-suppressgctransition <without-suppressgctransition>     A declaration name to opt back out of a '--with-suppressgctransition *' catch-all. []
  --without-type <without-type>                                     A declaration name to opt back out of a '--with-type *' catch-all. []
  --without-using <without-using>                                   A declaration name to opt back out of a '--with-using *' catch-all. []
  -?, -h, --help                                                    Show help and usage information

Wildcards:
Many name-matching options accept glob patterns using `*` (matches any run of characters, including qualification separators) and `?` (matches a single character); `::` and `.` are treated as equivalent separators and matching is case-sensitive. An exact match always wins over a glob, and among globs the most specific (the most literal characters) wins. Many `--with-*` options also accept a bare `*` as a catch-all that applies a rule to everything; for value options it is written `*=value` (e.g. --with-access-specifier *=Internal makes all generated code internal). Each such option has a paired `--without-<name>` option that opts a specific declaration (by exact name or glob) back out of its `*` catch-all (e.g. --with-access-specifier *=Internal --without-access-specifier Foo). To opt everything in and exclude piecemeal, use the include (-i) and exclude (-e) options together; they are already an opt-in/opt-out pair and likewise accept globs.

More information:
See https://github.com/dotnet/ClangSharp/blob/main/docs/generating-bindings-best-practices.md for a guide on structuring a generation project and using these options.
```

The available configuration options (visible with `-c help`) are:
```
--config, -c	A configuration option that controls how the bindings are generated. Specify 'help' to see the available options.

Options:
  ?, h, help                                  Show help and usage information for -c, --config, --generate, and --log

  # -c, --config now carries only the four mode families below (plus this help). The
  # feature switches moved to --generate <name> and the diagnostics to --log <name>.
  # Boolean --generate features accept an optional '=true'/'=false' ('=true' is implied
  # when omitted), so a later response file can override an earlier one. Valued switches
  # take the value shown as name=<value>.

  # Mode Families (-c <name>=<value>)

  codegen=<value>                             Which .NET/C# level to target: 'compatible' (.NET Standard 2.0), 'default' (current LTS; .NET 8/C# 12), 'latest' (current STS; .NET 10/C# 14), or 'preview'. Defaults to 'default'.
  file=<value>                                How output is split: 'single' (one output file; the default) or 'multi' (approximately one type per file).
  types=<value>                               Which platform defaults to assume: 'windows' or 'unix'. Defaults to the host platform.
  vtbls=<value>                               How VTBLs are generated: 'explicit' (a named field per entry), 'implicit' (the default; reduces metadata bloat), or 'trimmable' (defined but unused in helpers to reduce bloat when trimming).

  # Test Generation (--generate <name>)

  tests-nunit                                 Basic tests validating size, blittability, and associated metadata should be generated for NUnit.
  tests-xunit                                 Basic tests validating size, blittability, and associated metadata should be generated for XUnit.

  # Generation Features (--generate <name>[=false])

  aggressive-inlining                         [MethodImpl(MethodImplOptions.AggressiveInlining)] should be added to generated helper functions.
  anonymous-field-helpers                     The helper ref properties for fields in nested anonymous structs and unions should be generated. On by default; use =false to opt out.
  callconv-member-function                    Instance function pointers should use [CallConvMemberFunction] where applicable.
  com-proxies                                 Types recognized as COM proxies should have bindings generated. These are currently function declarations ending with _UserFree, _UserMarshal, _UserSize, _UserUnmarshal, _Proxy, or _Stub. On by default; use =false to opt out.
  cpp-attributes                              [CppAttributeList("")] should be generated to document the encountered C++ attributes.
  default-remappings                          Default remappings for well known types should be added. This currently includes intptr_t, ptrdiff_t, size_t, ssize_t, uintptr_t, and the exact-width stdint types (int8_t, int16_t, int32_t, int64_t, uint8_t, uint16_t, uint32_t, and uint64_t). When targeting Windows, the pointer-width Windows types (INT_PTR, LONG_PTR, SSIZE_T, DWORD_PTR, SIZE_T, UINT_PTR, and ULONG_PTR) and _GUID are also included. On by default; use =false to opt out.
  disable-runtime-marshalling                 [assembly: DisableRuntimeMarshalling] should be generated.
  doc-includes                                <include> xml documentation tags should be generated for declarations.
  empty-records                               Bindings for records that contain no members should be generated. These are commonly encountered for opaque handle like types such as HWND. On by default; use =false to opt out.
  enum-member-type-name                       The enum type name should be kept at the beginning of its member names. On by default; use =false to strip it.
  enum-operators                              Bindings for operators over enum types should be generated. These are largely unnecessary in C# as the operators are available by default. On by default; use =false to opt out.
  extern-variables                            Top-level extern/extern const global variables should be surfaced as settable pointer fields on a <Class>ManualImports struct for the consumer to resolve (like --with-manual-import). Opt-in; pointer and primitive types only.
  file-scoped-namespaces                      Namespaces should be scoped to the file to reduce nesting.
  fixed-buffer-indexer-overloads              Fixed sized buffer helper types should generate additional uint, nint, and nuint indexer overloads.
  fnptr-codegen                               Generated bindings for latest or preview codegen should use function pointers. On by default; use =false to opt out.
  funcs-with-body                             Bindings for functions with bodies should be generated. On by default; use =false to opt out.
  generated-code=<mode>                       Controls the emission of the GeneratedCode attribute. 'assembly' (default) emits a single '[assembly: GeneratedCode]' when helper types are generated; 'type' instead annotates each generated top-level type; 'none' emits neither.
  generic-pointer-wrapper                     Pointer<T> should be used for limited generic type support.
  guid-member                                 Types with an associated GUID should have a corresponding member generated.
  helper-types                                Code files should be generated for various helper attributes and declared transparent structs.
  macro-bindings                              Bindings for macro-definitions should be generated. This currently only works with value like macros and not function-like ones.
  marker-interfaces                           Bindings for marker interfaces representing native inheritance hierarchies should be generated.
  native-alignment-attribute                  [NativeAlignment(#)] attribute should be generated to document the requested over-alignment (__declspec(align) / DECLSPEC_ALIGN) that .NET cannot honor.
  native-bitfield-attribute                   [NativeBitfield("", offset: #, length: #)] attribute should be generated to document the encountered bitfield layout.
  native-inheritance-attribute                [NativeInheritance("")] attribute should be generated to document the encountered C++ base type.
  nint-codegen                                Generated bindings should use nint/nuint where applicable. On by default; use =false to opt out.
  objective-c-bindings                        Bindings for Objective-C declarations (currently @protocol types) should be generated. This is experimental and requires the Objective-C runtime (libobjc) at runtime.
  setslastsystemerror-attribute               [SetsLastSystemError] attribute should be generated rather than using SetLastError = true.
  template-bindings                           Bindings for template-definitions should be generated. This is currently experimental.
  unmanaged-constants                         Unmanaged constants should be generated using static ref readonly properties. This is currently experimental.
  using-statics-for-enums                     Enum usages should include a corresponding 'using static EnumName;' rather than being fully qualified. On by default; use =false to opt out.
  using-statics-for-guid-members              GUID member usages should include a corresponding 'using static' rather than being fully qualified. On by default; use =false to opt out.
  vtbl-index-attribute                        [VtblIndex(#)] attribute should be generated to document the underlying VTBL index for a helper method.

  # Diagnostic Logs (--log <name>)

  exclusions                                  A list of excluded declaration types should be generated. This will also log if the exclusion was due to an exact or partial match.
  potential-typedef-remappings                A list of potential typedef remappings should be generated. This can help identify missing remappings.
  visited-files                               A list of the visited files should be generated. This can help identify traversal issues.

  # Legacy/Deprecated -c spellings (still accepted, but each emits a deprecation warning)

  -c compatible-codegen                       Use -c codegen=compatible.
  -c default-codegen                          Use -c codegen=default.
  -c latest-codegen                           Use -c codegen=latest.
  -c preview-codegen                          Use -c codegen=preview.
  -c single-file                              Use -c file=single.
  -c multi-file                               Use -c file=multi.
  -c windows-types                            Use -c types=windows.
  -c unix-types                               Use -c types=unix.
  -c explicit-vtbls                           Use -c vtbls=explicit.
  -c implicit-vtbls                           Use -c vtbls=implicit.
  -c trimmable-vtbls                          Use -c vtbls=trimmable.
  -c generate-*                               Use --generate * (e.g. -c generate-helper-types becomes --generate helper-types).
  -c log-*                                    Use --log * (e.g. -c log-visited-files becomes --log visited-files).
  -c exclude-anonymous-field-helpers          Use --generate anonymous-field-helpers=false.
  -c exclude-com-proxies                      Use --generate com-proxies=false.
  -c exclude-default-remappings               Use --generate default-remappings=false.
  -c no-default-remappings                    Use --generate default-remappings=false.
  -c default-remappings                       Use --generate default-remappings (or =true).
  -c exclude-empty-records                    Use --generate empty-records=false.
  -c exclude-enum-operators                   Use --generate enum-operators=false.
  -c exclude-fnptr-codegen                    Use --generate fnptr-codegen=false.
  -c exclude-funcs-with-body                  Use --generate funcs-with-body=false.
  -c exclude-nint-codegen                     Use --generate nint-codegen=false.
  -c exclude-using-statics-for-enums          Use --generate using-statics-for-enums=false.
  -c dont-use-using-statics-for-enums         Use --generate using-statics-for-enums=false.
  -c exclude-using-statics-for-guid-members   Use --generate using-statics-for-guid-members=false.
  -c dont-use-using-statics-for-guid-members  Use --generate using-statics-for-guid-members=false.
  -c strip-enum-member-type-name              Use --generate enum-member-type-name=false.
```

### Using as a Library

In addition to generating bindings, ClangSharp can be consumed directly as a library to parse C/C++ and inspect the resulting AST. This is an advanced scenario that assumes familiarity with the Clang APIs, which remain the source of truth. For the ClangSharp-specific conventions — how the `clang_*` C functions and the Clang C++ AST map onto the `ClangSharp.Interop` and `ClangSharp` surfaces, lifetime/`IDisposable` handling, and the required package references — see [Using ClangSharp as a library](docs/using-clangsharp-as-a-library.md).

### Using locally built versions

After you build local version, you can use executable from build location.

```
artifacts/bin/sources/ClangSharpPInvokeGenerator/Debug/net6.0/ClangSharpPInvokeGenerator
```

If you are on Linux

```
LD_LIBRARY_PATH=$(pwd)/artifacts/bin/native/lib/
artifacts/bin/sources/ClangSharpPInvokeGenerator/Debug/net6.0/ClangSharpPInvokeGenerator
```

### Spotlight

The P/Invoke Generator is currently used by several projects:

* [dotnet/clangsharp](https://github.com/dotnet/clangsharp) - ClangSharp is self-hosting
* [dotnet/llvmsharp](https://github.com/dotnet/llvmsharp) - Bindings over libLLVM
* [microsoft/win32metadata](https://github.com/microsoft/win32metadata) - Bindings over the Windows SDK meant for downstream use by projects such as CsWin32, RsWin32, etc
* [terrafx/terrafx.interop.windows](https://github.com/terrafx/terrafx.interop.windows) - Bindings for D3D10, D3D11, D3D12, D2D1, DWrite, WIC, User32, and more in a single NuGet
* [terrafx/terrafx.interop.vulkan](https://github.com/terrafx/terrafx.interop.vulkan) - Bindings for Vulkan
* [terrafx/terrafx.interop.xlib](https://github.com/terrafx/terrafx.interop.xlib) - Bindings for Xlib
