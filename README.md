# ClangSharp

ClangSharp provides Clang bindings written in C#. It is self-hosted and auto-generates itself by parsing the Clang C header files using ClangSharpPInvokeGenerator.

![ci](https://github.com/dotnet/clangsharp/workflows/ci/badge.svg?branch=main&event=push)

A nuget package for the project is provided here: https://www.nuget.org/packages/clangsharp.
A .NET tool for the P/Invoke generator project is provided here: https://www.nuget.org/packages/ClangSharpPInvokeGenerator

A convenience package which provides the native libClang library for several platforms is provided here: https://www.nuget.org/packages/libclang

A helper package which exposes many Clang APIs missing from libClang is provided here: https://www.nuget.org/packages/libClangSharp

NOTE: libclang and libClangSharp are meta-packages which point to the platform-specific runtime packages ([e.g.](https://www.nuget.org/packages/libClangSharp.runtime.win-x64/15.0.0); see others owned by [tannergooding](https://www.nuget.org/profiles/tannergooding)). Several manual steps may be required to use them, see discussion in [#46](https://github.com/dotnet/ClangSharp/issues/46) and [#118](https://github.com/dotnet/ClangSharp/issues/118).

Nightly packages are available via the NuGet Feed URL: https://pkgs.clangsharp.dev/index.json

Source browsing is available via: https://source.clangsharp.dev/

## Table of Contents

* [Code of Conduct](#code-of-conduct)
* [License](#license)
* [Features](#features)
* [Building Managed](#building-managed)
* [Building Native](#building-native)
* [Generating Bindings](#generating-bindings)
* [Using locally built versions](#using-locally-build-versions)
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

ClangSharp requires the [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) and can be built simply with `dotnet build -c Release`.

You can reproduce what the CI environment does by running `./scripts/cibuild.cmd` on Windows or `./scripts.cibuild.sh` on Unix.
This will download the required .NET SDK locally and use that to build the repo; it will also run through all available actions in the appropriate order.

There are also several build scripts in the repository root. On Windows these scripts end with `.cmd` and expect arguments with a `-` prefix. On Unix these scripts end with `.sh` and expect arguments with a `--` prefix.
By default, each script performs only the action specified in its name (i.e. `restore` only restores, `build` only builds, `test` only tests, and `pack` only packs). You can specify additional actions to be run by passing that name as an argument to the script (e.g. `build.cmd -restore` will perform a package restore and build; `test.cmd -pack` will run tests and package artifacts).
Certain actions are dependent on a previous action having been run at least once. `build` depends on `restore`, `test` depends on `build`, and `pack` depends on `build`. This means the recommended first time action is `build -restore`.

You can see any additional options that are available by passing `-help` on Windows or `--help` on Unix to the available build scripts.

### Building Native

ClangSharp provides a helper library, `libClangSharp`, that exposes additional functionality that is not available in `libClang`.
Building this requires [CMake 3.13 or later](https://cmake.org/download/) as well as a version of MSVC or Clang that supports C++ 17.

To succesfully build `libClangSharp` you must first build Clang (https://clang.llvm.org/get_started.html). The process done on Windows is roughly:
```cmd
git clone --single-branch --branch llvmorg-15.0.0 https://github.com/llvm/llvm-project
cd llvm-project
mkdir artifacts/bin
cd artifacts/bin
cmake -DLLVM_ENABLE_PROJECTS=clang -DCMAKE_INSTALL_PREFIX=../install -G "Visual Studio 16 2019" -A x64 -Thost=x64 ../../llvm
```

You can then open `LLVM.sln` in Visual Studio, change the configuration to `Release` and build the `install` project.
Afterwards, you can then build `libClangSharp` where the process followed is roughly:
```cmd
git clone https://github.com/dotnet/clangsharp
cd clangsharp
mkdir artifacts/bin/native
cd artifacts/bin/native
cmake -DPATH_TO_LLVM=../../../../llvm-project/artifacts/install/ -G "Visual Studio 16 2019" -A x64 -Thost=x64 ../../..
```

You can then open `libClangSharp.sln` in Visual Studio, change the configuration to `Release` and build the `ALL_BUILD` project.

If you building on Linux
```
git clone https://github.com/dotnet/clangsharp
cd clangsharp
mkdir artifacts/bin/native
cd artifacts/bin/native
cmake -DPATH_TO_LLVM=/usr/lib/llvm/14/ ../../..
make
```

or if you prefer Ninja
```
git clone https://github.com/dotnet/clangsharp
cd clangsharp
mkdir artifacts/bin/native
cd artifacts/bin/native
cmake -DPATH_TO_LLVM=/usr/lib/llvm/14/ -G Ninja ../../..
ninja
```

### Generating Bindings

This program will take a given set of C or C++ header files and generate C# bindings from them. It is still a work-in-progress and not every declaration can have bindings generated today (contributions are welcome).

The simplest and recommended setup is to install the generator as a .NET tool and then use response files:
```
dotnet tool install --global ClangSharpPInvokeGenerator --version 15.0.0
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
  -a, --additional <additional>                              An argument to pass to Clang when parsing the input files. []
  -c, --config <config>                                      A configuration option that controls how the bindings are generated. Specify 'help' to see the available options. []
  -D, --define-macro <define-macro>                          Define <macro> to <value> (or 1 if <value> omitted). []
  -e, --exclude <exclude>                                    A declaration name to exclude from binding generation. []
  -f, --file <file>                                          A file to parse and generate bindings for. []
  -F, --file-directory <file-directory>                      The base path for files to parse. []
  -h, --headerFile <headerFile>                              A file which contains the header to prefix every generated file with. []
  -i, --include <include>                                    A declaration name to include in binding generation. []
  -I, --include-directory <include-directory>                Add directory to include search path. []
  -x, --language <language>                                  Treat subsequent input files as having type <language>. [default: c++]
  -l, --libraryPath <libraryPath>                            The string to use in the DllImport attribute used when generating bindings. []
  -m, --methodClassName <methodClassName>                    The name of the static class that will contain the generated method bindings. [default: Methods]
  -n, --namespace <namespace>                                The namespace in which to place the generated bindings. []
  -om, --output-mode <CSharp|Xml>                            The mode describing how the information collected from the headers are presented in the resultant bindings. [default: CSharp]
  -o, --output <output>                                      The output location to write the generated bindings to. []
  -p, --prefixStrip <prefixStrip>                            The prefix to strip from the generated method bindings. []
  -r, --remap <remap>                                        A declaration name to be remapped to another name during binding generation. []
  -std <std>                                                 Language standard to compile for. []
  -to, --test-output <test-output>                           The output location to write the generated tests to. []
  -t, --traverse <traverse>                                  A file name included either directly or indirectly by -f that should be traversed during binding generation. []
  -v, --version <version>                                    Prints the current version information for the tool and its native dependencies.
  -was, --with-access-specifier <with-access-specifier>      An access specifier to be used with the given qualified or remapped declaration name during binding generation. []
  -wa, --with-attribute <with-attribute>                     An attribute to be added to the given remapped declaration name during binding generation. []
  -wcc, --with-callconv <with-callconv>                      A calling convention to be used for the given declaration during binding generation. []
  -wc, --with-class <with-class>                             A class to be used for the given remapped constant or function declaration name during binding generation. []
  -wg, --with-guid <with-guid>                               A GUID to be used for the given declaration during binding generation. []
  -wlb, --with-librarypath <with-librarypath>                A library path to be used for the given declaration during binding generation. []
  -wmi, --with-manual-import <with-manual-import>            A remapped function name to be treated as a manual import during binding generation. []
  -wn, --with-namespace <with-namespace>                     A namespace to be used for the given remapped declaration name during binding generation. []
  -wsle, --with-setlasterror <with-setlasterror>             Add the SetLastError=true modifier to a given DllImport or UnmanagedFunctionPointer. []
  -wsgct, --with-suppressgctransition <with-suppressgctransition>  Add the SuppressGCTransition calling convention to a given DllImport or UnmanagedFunctionPointer. []
  -wts, --with-transparent-struct <with-transparent-struct>  A remapped type name to be treated as a transparent wrapper during binding generation. []
  -wt, --with-type <with-type>                               A type to be used for the given enum declaration during binding generation. []
  -wu, --with-using <with-using>                             A using directive to be included for the given remapped declaration name during binding generation. []
  -?, -h, --help                                             Show help and usage information

Wildcards:
You can use * as catch-all rule for remapping procedures. For example if you want make all of your generated code internal you can use --with-access-specifier *=Internal.
```

The available configuration options (visible with `-c help`) are:
```
--config, -c    A configuration option that controls how the bindings are generated. Specify 'help' to see the available options.

Options:
  ?, h, help                             Show help and usage information for -c, --config
  compatible-codegen                     Bindings should be generated with .NET Standard 2.0 compatibility. Setting this disables preview code generation.
  latest-codegen                         Bindings should be generated for the latest stable version of .NET/C#. This is currently .NET 6/C# 10.
  preview-codegen                        Bindings should be generated for the latest preview version of .NET/C#. This is currently .NET 7/C# 11.
  single-file                            Bindings should be generated to a single output file. This is the default.
  multi-file                             Bindings should be generated so there is approximately one type per file.
  unix-types                             Bindings should be generated assuming Unix defaults. This is the default on Unix platforms.
  windows-types                          Bindings should be generated assuming Windows defaults. This is the default on Windows platforms.
  exclude-anonymous-field-helpers        The helper ref properties generated for fields in nested anonymous structs and unions should not be generated.
  exclude-com-proxies                    Types recognized as COM proxies should not have bindings generated. Thes are currently function declarations ending with _UserFree, _UserMarshal, _UserSize, _UserUnmarshal, _Proxy, or _Stub.
  exclude-default-remappings             Default remappings for well known types should not be added. This currently includes intptr_t, ptrdiff_t, size_t, and uintptr_t
  exclude-empty-records                  Bindings for records that contain no members should not be generated. These are commonly encountered for opaque handle like types such as HWND.
  exclude-enum-operators                 Bindings for operators over enum types should not be generated. These are largely unnecessary in C# as the operators are available by default.
  exclude-fnptr-codegen                  Generated bindings for latest or preview codegen should not use function pointers.
  exclude-funcs-with-body                Bindings for functions with bodies should not be generated.
  exclude-using-statics-for-enums        Enum usages should be fully qualified and should not include a corresponding 'using static EnumName;'
  explicit-vtbls                         VTBLs should have an explicit type generated with named fields per entry.
  implicit-vtbls                         VTBLs should be implicit to reduce metadata bloat. This is the current default
  trimmable-vtbls                        VTBLs should be defined but not used in helper methods to reduce metadata bloat when trimming.
  generate-tests-nunit                   Basic tests validating size, blittability, and associated metadata should be generated for NUnit.
  generate-tests-xunit                   Basic tests validating size, blittability, and associated metadata should be generated for XUnit.
  generate-aggressive-inlining           [MethodImpl(MethodImplOptions.AggressiveInlining)] should be added to generated helper functions.
  generate-cpp-attributes                [CppAttributeList("")] should be generated to document the encountered C++ attributes.
  generate-doc-includes                  &lt;include&gt; xml documentation tags should be generated for declarations.
  generate-file-scoped-namespaces        Namespaces should be scoped to the file to reduce nesting.
  generate-guid-member                   Types with an associated GUID should have a corresponding member generated.
  generate-helper-types                  Code files should be generated for various helper attributes and declared transparent structs.
  generate-macro-bindings                Bindings for macro-definitions should be generated. This currently only works with value like macros and not function-like ones.
  generate-marker-interfaces             Bindings for marker interfaces representing native inheritance hierarchies should be generated.
  generate-native-inheritance-attribute  [NativeInheritance("")] attribute should be generated to document the encountered C++ base type.
  generate-setslastsystemerror-attribute [SetsLastSystemError] attribute should be generated rather than using SetLastError = true.
  generate-template-bindings             Bindings for template-definitions should be generated. This is currently experimental.
  generate-unmanaged-constants           Unmanaged constants should be generated using static ref readonly properties. This is currently experimental.
  generate-vtbl-index-attribute          [VtblIndex(#)] attribute should be generated to document the underlying VTBL index for a helper method.
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
