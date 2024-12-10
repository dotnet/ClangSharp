# ClangSharp

ClangSharp provides Clang bindings written in C#. It is self-hosted and auto-generates itself by parsing the Clang C header files using ClangSharpPInvokeGenerator.

[![ci](https://github.com/dotnet/ClangSharp/actions/workflows/ci.yml/badge.svg?branch=main&event=push)](https://github.com/dotnet/ClangSharp/actions/workflows/ci.yml)

A nuget package for the project is provided here: https://www.nuget.org/packages/clangsharp.
A .NET tool for the P/Invoke generator project is provided here: https://www.nuget.org/packages/ClangSharpPInvokeGenerator

**NOTE:** If you are running as a dotnet tool, you may need to manually copy the appropriate DLLs from NuGet due to limitations in the dotnet tool support.

A convenience package which provides the native libClang library for several platforms is provided here: https://www.nuget.org/packages/libclang

A helper package which exposes many Clang APIs missing from libClang is provided here: https://www.nuget.org/packages/libClangSharp

**NOTE:** libclang and libClangSharp are meta-packages which point to the platform-specific runtime packages ([e.g.](https://www.nuget.org/packages/libClangSharp.runtime.win-x64/18.1.3); see others owned by [tannergooding](https://www.nuget.org/profiles/tannergooding)). Several manual steps may be required to use them, see discussion in [#46](https://github.com/dotnet/ClangSharp/issues/46) and [#118](https://github.com/dotnet/ClangSharp/issues/118).

Nightly packages are available via the NuGet Feed URL: https://pkgs.clangsharp.dev/index.json

Source browsing is available via: https://source.clangsharp.dev/

## Table of Contents

* [Code of Conduct](#code-of-conduct)
* [License](#license)
* [Features](#features)
* [Building Managed](#building-managed)
* [Building Native](#building-native)
* [Generating Bindings](#generating-bindings)
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

ClangSharp requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and can be built simply with `dotnet build -c Release`.

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
git clone --single-branch --branch llvmorg-18.1.3 https://github.com/llvm/llvm-project
cd llvm-project
mkdir artifacts/bin
cd artifacts/bin
cmake -DCMAKE_INSTALL_PREFIX=../install -DLLVM_ENABLE_PROJECTS=clang -G "Visual Studio 17 2022" -A x64 -Thost=x64 ../../llvm
```

You can then open `LLVM.sln` in Visual Studio, change the configuration to `Release` and build the `install` project.

Afterwards, you can then build `libClangSharp` where the process followed is roughly:
```cmd
git clone https://github.com/dotnet/clangsharp
cd clangsharp
mkdir artifacts/bin/native
cd artifacts/bin/native
cmake -DCMAKE_INSTALL_PREFIX=../install -DPATH_TO_LLVM=../../../../llvm-project/artifacts/install -G "Visual Studio 17 2022" -A x64 -Thost=x64 ../../..
```

You can then open `libClangSharp.sln` in Visual Studio, change the configuration to `Release` and build the `install` project.

#### Linux

The process done on Linux is roughly:
```bash
git clone --single-branch --branch llvmorg-18.1.3 https://github.com/llvm/llvm-project
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

### Generating Bindings

This program will take a given set of C or C++ header files and generate C# bindings from them. It is still a work-in-progress and not every declaration can have bindings generated today (contributions are welcome).

The simplest and recommended setup is to install the generator as a .NET tool and then use response files:
```
dotnet tool install --global ClangSharpPInvokeGenerator --version 18.1.0
ClangSharpPInvokeGenerator @generate.rsp
```

A response file allows you to specify and checkin the command line arguments in a text file, with one argument per line. For example: https://github.com/dotnet/ClangSharp/blob/main/sources/ClangSharpPInvokeGenerator/Properties/GenerateClang.rsp
At a minimum, the command line expects one or more input files (`-f`), an output namespace (`-n`), and an output location (`-o`). A typical response file may also specify explicit files to traverse, configuration options, name remappings, and other fixups.

The full set of available switches:
```
ClangSharpPInvokeGenerator
  ClangSharp P/Invoke Binding Generator

Usage:
  ClangSharpPInvokeGenerator [options]

Options:
  -a, --additional <additional>                                    An argument to pass to Clang when parsing the input files. []
  -c, --config <config>                                            A configuration option that controls how the bindings are generated. Specify 'help' to see the available options. []
  -D, --define-macro <define-macro>                                Define <macro> to <value> (or 1 if <value> omitted). []
  -e, --exclude <exclude>                                          A declaration name to exclude from binding generation. []
  -f, --file <file>                                                A file to parse and generate bindings for. []
  -F, --file-directory <file-directory>                            The base path for files to parse. []
  -hf, --headerFile <headerFile>                                   A file which contains the header to prefix every generated file with. []
  -i, --include <include>                                          A declaration name to include in binding generation. []
  -I, --include-directory <include-directory>                      Add directory to include search path. []
  -x, --language <c|c++>                                           Treat subsequent input files as having type <language>. [default: c++]
  -l, --libraryPath <libraryPath>                                  The string to use in the DllImport attribute used when generating bindings. []
  -m, --methodClassName <methodClassName>                          The name of the static class that will contain the generated method bindings. [default: Methods]
  -n, --namespace <namespace>                                      The namespace in which to place the generated bindings. []
  -om, --output-mode <CSharp|Xml>                                  The mode describing how the information collected from the headers are presented in the resultant bindings. [default: CSharp]
  -o, --output <output>                                            The output location to write the generated bindings to. []
  -p, --prefixStrip <prefixStrip>                                  The prefix to strip from the generated method bindings. []
  --nativeTypeNamesToStrip <nativeTypeNamesToStrip>                The contents to strip from the generated NativeTypeName attributes. []
  -r, --remap <remap>                                              A declaration name to be remapped to another name during binding generation. []
  -std <std>                                                       Language standard to compile for. []
  -to, --test-output <test-output>                                 The output location to write the generated tests to. []
  -t, --traverse <traverse>                                        A glob used to filter file names included either directly or indirectly by -f that will be traversed during binding generation. []
  -v, --version                                                    Prints the current version information for the tool and its native dependencies.
  -was, --with-access-specifier <with-access-specifier>            An access specifier to be used with the given qualified or remapped declaration name during binding generation. Supports wildcards. []
  -wa, --with-attribute <with-attribute>                           An attribute to be added to the given remapped declaration name during binding generation. Supports wildcards. []
  -wcc, --with-callconv <with-callconv>                            A calling convention to be used for the given declaration during binding generation. Supports wildcards. []
  -wc, --with-class <with-class>                                   A class to be used for the given remapped constant or function declaration name during binding generation. Supports wildcards. []
  -wg, --with-guid <with-guid>                                     A GUID to be used for the given declaration during binding generation. Supports wildcards. []
  -wl, --with-length <with-length>                                 A length to be used for the given declaration during binding generation. Supports wildcards. []
  -wlb, --with-librarypath <with-librarypath>                      A library path to be used for the given declaration during binding generation. Supports wildcards. []
  -wmi, --with-manual-import <with-manual-import>                  A remapped function name to be treated as a manual import during binding generation. Supports wildcards. []
  -wn, --with-namespace <with-namespace>                           A namespace to be used for the given remapped declaration name during binding generation. Supports wildcards. []
  -wp, --with-packing <with-packing>                               Overrides the StructLayoutAttribute.Pack property for the given type. Supports wildcards. []
  -wsle, --with-setlasterror <with-setlasterror>                   Add the SetLastError=true modifier or SetsSystemLastError attribute to a given DllImport or UnmanagedFunctionPointer. Supports wildcards. []
  -wsgct, --with-suppressgctransition <with-suppressgctransition>  Add the SuppressGCTransition calling convention to a given DllImport or UnmanagedFunctionPointer. Supports wildcards. []
  -wts, --with-transparent-struct <with-transparent-struct>        A remapped type name to be treated as a transparent wrapper during binding generation. Supports wildcards. []
  -wt, --with-type <with-type>                                     A type to be used for the given enum declaration during binding generation. Supports wildcards. []
  -wu, --with-using <with-using>                                   A using directive to be included for the given remapped declaration name during binding generation. Supports wildcards. []
  -?, -h, --help                                                   Show help and usage information

Wildcards:
You can use * as catch-all rule for remapping procedures. For example if you want make all of your generated code internal you can use --with-access-specifier *=Internal.
```

The available configuration options (visible with `-c help`) are:
```
--config, -c    A configuration option that controls how the bindings are generated. Specify 'help' to see the available options.

Options:
  ?, h, help                             Show help and usage information for -c, --config

  # Codegen Options

  compatible-codegen                     Bindings should be generated with .NET Standard 2.0 compatibility. Setting this disables preview code generation.
  default-codegen                        Bindings should be generated for the previous LTS version of .NET/C#. This is currently .NET 6/C# 10.
  latest-codegen                         Bindings should be generated for the current LTS/STS version of .NET/C#. This is currently .NET 8/C# 12.
  preview-codegen                        Bindings should be generated for the preview version of .NET/C#. This is currently .NET 9/C# 13.

  # File Options

  single-file                            Bindings should be generated to a single output file. This is the default.
  multi-file                             Bindings should be generated so there is approximately one type per file.

  # Type Options

  unix-types                             Bindings should be generated assuming Unix defaults. This is the default on Unix platforms.
  windows-types                          Bindings should be generated assuming Windows defaults. This is the default on Windows platforms.

  # Exclusion Options

  exclude-anonymous-field-helpers        The helper ref properties generated for fields in nested anonymous structs and unions should not be generated.
  exclude-com-proxies                    Types recognized as COM proxies should not have bindings generated. These are currently function declarations ending with _UserFree, _UserMarshal, _UserSize, _UserUnmarshal, _Proxy, or _Stub.
  exclude-default-remappings             Default remappings for well known types should not be added. This currently includes intptr_t, ptrdiff_t, size_t, and uintptr_t
  exclude-empty-records                  Bindings for records that contain no members should not be generated. These are commonly encountered for opaque handle like types such as HWND.
  exclude-enum-operators                 Bindings for operators over enum types should not be generated. These are largely unnecessary in C# as the operators are available by default.
  exclude-fnptr-codegen                  Generated bindings for latest or preview codegen should not use function pointers.
  exclude-funcs-with-body                Bindings for functions with bodies should not be generated.
  exclude-using-statics-for-enums        Enum usages should be fully qualified and should not include a corresponding 'using static EnumName;'

  # Vtbl Options

  explicit-vtbls                         VTBLs should have an explicit type generated with named fields per entry.
  implicit-vtbls                         VTBLs should be implicit to reduce metadata bloat. This is the current default
  trimmable-vtbls                        VTBLs should be defined but not used in helper methods to reduce metadata bloat when trimming.

  # Test Options

  generate-tests-nunit                   Basic tests validating size, blittability, and associated metadata should be generated for NUnit.
  generate-tests-xunit                   Basic tests validating size, blittability, and associated metadata should be generated for XUnit.

  # Generation Options

  generate-aggressive-inlining           [MethodImpl(MethodImplOptions.AggressiveInlining)] should be added to generated helper functions.
  generate-callconv-member-function      Instance function pointers should use [CallConvMemberFunction] where applicable.
  generate-cpp-attributes                [CppAttributeList("")] should be generated to document the encountered C++ attributes.
  generate-disable-runtime-marshalling   [assembly: DisableRuntimeMarshalling] should be generated.
  generate-doc-includes                  <include> xml documentation tags should be generated for declarations.
  generate-file-scoped-namespaces        Namespaces should be scoped to the file to reduce nesting.
  generate-guid-member                   Types with an associated GUID should have a corresponding member generated.
  generate-helper-types                  Code files should be generated for various helper attributes and declared transparent structs.
  generate-macro-bindings                Bindings for macro-definitions should be generated. This currently only works with value like macros and not function-like ones.
  generate-marker-interfaces             Bindings for marker interfaces representing native inheritance hierarchies should be generated.
  generate-native-bitfield-attribute     [NativeBitfield("", offset: #, length: #)] attribute should be generated to document the encountered bitfield layout.
  generate-native-inheritance-attribute  [NativeInheritance("")] attribute should be generated to document the encountered C++ base type.
  generate-generic-pointer-wrapper       Pointer<T> should be used for limited generic type support.
  generate-setslastsystemerror-attribute [SetsLastSystemError] attribute should be generated rather than using SetLastError = true.
  generate-template-bindings             Bindings for template-definitions should be generated. This is currently experimental.
  generate-unmanaged-constants           Unmanaged constants should be generated using static ref readonly properties. This is currently experimental.
  generate-vtbl-index-attribute          [VtblIndex(#)] attribute should be generated to document the underlying VTBL index for a helper method.

  # Stripping Options

  strip-enum-member-type-name            Strips the enum type name from the beginning of its member names.

  # Logging Options

  log-exclusions                         A list of excluded declaration types should be generated. This will also log if the exclusion was due to an exact or partial match.
  log-potential-typedef-remappings       A list of potential typedef remappings should be generated. This can help identify missing remappings.
  log-visited-files                      A list of the visited files should be generated. This can help identify traversal issues.
```

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
