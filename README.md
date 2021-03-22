# ClangSharp

ClangSharp provides Clang bindings written in C#. It is self-hosted and auto-generates itself by parsing the Clang C header files using ClangSharpPInvokeGenerator.

| Job | Debug Status | Release Status |
| --- | ------------ | -------------- |
| Windows x86 | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=windows_debug_x86)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=windows_release_x86)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) |
| Windows x64 | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=windows_debug_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=windows_release_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) |
| Ubuntu 18.04 x64 | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=ubuntu_debug_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=ubuntu_release_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) |
| MacOS x64 | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=macos_debug_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=macos_release_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) |

A nuget package for the project is provided here: https://www.nuget.org/packages/clangsharp.
A .NET tool for the P/Invoke generator project is provided here: https://www.nuget.org/packages/ClangSharpPInvokeGenerator

A convenience package which provides the native libClang library for several platforms is provided here: https://www.nuget.org/packages/libclang
A helper package which exposes many Clang APIs missing from libClang is provided here: https://www.nuget.org/packages/libClangSharp

NOTE: These may be out of date as compared to the latest sources. New versions are published as appropriate and a nightly feed is not currently available.

## Table of Contents

* [Microsoft Open Source Code of Conduct](#microsoft-open-source-code-of-conduct)
* [License](#license)
* [Features](#features)
* [Building Managed](#building-managed)
* [Building Native](#building-native)
* [Generating Bindings](#generating-bindings)
* [Spotlight](#spotlight)

## Microsoft Open Source Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

### License

Copyright (c) Microsoft and Contributors. All rights reserved.
Licensed under the University of Illinois/NCSA Open Source License.
See [LICENSE.txt](LICENSE.txt) in the project root for license information.

## Features

 * Auto-generated using Clang C headers files, and supports all functionality exposed by them ~ which means you can build tooling around C/C++
 * Exposes the raw unsafe API for performance
 * Exposes a slightly higher abstraction that is type safe (CXIndex and CXTranslationUnit are different types, despite being pointers internally)
 * Exposes an again slightly higher abstraction that tries to mirror the Clang C++ Type Hierarchy where possible
 * Nearly identical to the Clang C APIs, e.g. `clang_getDiagnosticSpelling` in C, vs. `clang.getDiagnosticSpelling` (notice the . in the C# API)

## Building Managed

ClangSharp requires the [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0) and can be built simply with `dotnet build -c Release`.

You can reproduce what the CI environment does by running `./scripts/cibuild.cmd` on Windows or `./scripts.cibuild.sh` on Unix.
This will download the required .NET SDK locally and use that to build the repo; it will also run through all available actions in the appropriate order.

There are also several build scripts in the repository root. On Windows these scripts end with `.cmd` and expect arguments with a `-` prefix. On Unix these scripts end with `.sh` and expect arguments with a `--` prefix.
By default, each script performs only the action specified in its name (i.e. `restore` only restores, `build` only builds, `test` only tests, and `pack` only packs). You can specify additional actions to be run by passing that name as an argument to the script (e.g. `build.cmd -restore` will perform a package restore and build; `test.cmd -pack` will run tests and package artifacts).
Certain actions are dependent on a previous action having been run at least once. `build` depends on `restore`, `test` depends on `build`, and `pack` depends on `build`. This means the recommended first time action is `build -restore`.

You can see any additional options that are available by passing `-help` on Windows or `--help` on Unix to the available build scripts.

## Building Native

ClangSharp provides a helper library, `libClangSharp`, that exposes additional functionality that is not available in `libClang`.
Building this requires [CMake 3.13 or later](https://cmake.org/download/) as well as a version of MSVC or Clang that supports C++ 17.

To succesfully build `libClangSharp` you must first build Clang (https://clang.llvm.org/get_started.html). The process done on Windows is roughly:
```cmd
git clone https://github.com/llvm/llvm-project
cd llvm-project
mkdir artifacts/bin
cd artifacts/bin
cmake -DLLVM_ENABLE_PROJECTS=clang -DCMAKE_INSTALL_PREFIX=../install -G "Visual Studio 16 2019" -A x64 -Thost=x64 ../../llvm
```

You can then open `LLVM.sln` in Visual Studio, change the configuration to `Release` and build the `install` project.
Afterwards, you can then build `libClangSharp` where the process followed is roughly:
```cmd
git clone https://github.com/microsoft/clangsharp
mkdir artifacts/bin/native
cd artifacts/bin/native
cmake -DPATH_TO_LLVM=../../../../llvm-project/artifacts/install/ -G "Visual Studio 16 2019" -A x64 -Thost=x64 ../../..
```

You can then open `libClangSharp.sln` in Visual Studio, change the configuration to `Release` and build the `ALL_BUILD` project.

## Generating Bindings

This program will take a given set of C or C++ header files and generate C# bindings from them. It is still a work-in-progress and not every declaration can have bindings generated today (contributions are welcome).

The simplest and recommended setup is to install the generator as a .NET tool and then use response files:
```
dotnet tool install --global ClangSharpPInvokeGenerator --version 11.0.0-beta2
ClangSharpPInvokeGenerator @generate.rsp
```

A response file allows you to specify and checkin the command line arguments in a text file, with one argument per line. For example: https://github.com/microsoft/ClangSharp/blob/master/sources/ClangSharpPInvokeGenerator/Properties/GenerateClang.rsp
At a minimum, the command line expects one or more input files (`-f`), an output namespace (`-n`), and an output location (`-o`). A typical response file may also specify explicit files to traverse, configuration options, name remappings, and other fixups.

The full set of available switches:
```
ClangSharpPInvokeGenerator:
  ClangSharp P/Invoke Binding Generator

Usage:
  ClangSharpPInvokeGenerator [options]

Options:
  -a, --additional <<arg>>                                An argument to pass to Clang when parsing the input files.
                                                          [default: System.String[]]
  -c, --config <<arg>>                                    Aconfiguration option that controls how the bindings are
                                                          generated. Specify 'help' to see the available options.
                                                          [default: System.String[]]
  -D, --define-macro <<macro>=<value>>                    Define <macro> to <value> (or 1 if <value> omitted).
                                                          [default: System.String[]]
  -e, --exclude <<name>>                                  Adeclaration name to exclude from binding generation.
                                                          [default: System.String[]]
  -f, --file <<file>>                                     Afile to parse and generate bindings for. [default:
                                                          System.String[]]
  -F, --file-directory <<directory>>                      The base path for files to parse. [default: ]
  -h, --headerFile <<file>>                               Afile which contains the header to prefix every generated
                                                          file with. [default: ]
  -I, --include-directory <<arg>>                         Add directory to include search path. [default:
                                                          System.String[]]
  -x, --language <<arg>>                                  Treat subsequent input files as having type <language>.
                                                          [default: c++]
  -l, --libraryPath <<dllName>>                           The string to use in the DllImport attribute used when
                                                          generating bindings. [default: ]
  -m, --methodClassName <<className>>                     The name of the static class that will contain the generated
                                                          method bindings. [default: Methods]
  -n, --namespace <<namespace>>                           The namespace in which to place the generated bindings.
                                                          [default: ]
  -o, --output <<file>>                                   The output location to write the generated bindings to.
                                                          [default: ]
  -p, --prefixStrip <<prefix>>                            The prefix to strip from the generated method bindings.
                                                          [default: ]
  -r, --remap <<name>=<value>>                            Adeclaration name to be remapped to another name during
                                                          binding generation. [default: System.String[]]
  -std <<arg>>                                            Language standard to compile for. [default: c++17]
  -to, --test-output <<file>>                             The output location to write the generated tests to.
                                                          [default: ]
  -t, --traverse <<name>>                                 Afile name included either directly or indirectly by -f that
                                                          should be traversed during binding generation. [default:
                                                          System.String[]]
  -wa, --with-attribute <<remapped-name>=<value>>         An attribute to be added to the given remapped declaration
                                                          name during binding generation. [default: System.String[]]
  -wcc, --with-callconv <<remapped-name>=<value>>         Acalling convention to be used for the given declaration
                                                          during binding generation. [default: System.String[]]
  -wlb, --with-librarypath <<remapped-name>=<value>>      Alibrary path to be used for the given declaration during
                                                          binding generation. [default: System.String[]]
  -wsle, --with-setlasterror <<remapped-name>=<value>>    Add the SetLastError=true modifier to a given DllImport or
                                                          UnmanagedFunctionPointer. [default: System.String[]]
  -wt, --with-type <<remapped-name>=<value>>              Atype to be used for the given enum declaration during
                                                          binding generation. [default: System.String[]]
  -wu, --with-using <<remapped-name>=<value>>             Ausing directive to be included for the given remapped
                                                          declaration name during binding generation. [default:
                                                          System.String[]]
  -om, --output-mode <CSharp|Xml>                         The mode describing how the information collected from the
                                                          headers are presented in the resultant bindings. [default:
                                                          CSharp]
  --version                                               Show version information
  -?, -h, --help                                          Show help and usage information
```

The available configuration options (visible with `-c help`) are:
```
-c, --config <<arg>>    A configuration option that controls how the bindings are generated. Specify 'help' to see the available options.

Options:
  ?, h, help                               Show help and usage information for -c, --config

  compatible-codegen                       Bindings should be generated with .NET Standard 2.0 compatibility. Setting
                                           this disables preview code generation.
  latest-codegen                           Bindings should be generated for the latest stable version of .NET/C#. This
                                           is currently .NET 5/C# 9.
  preview-codegen                          Bindings should be generated for the latest preview version of .NET/C#.
                                           This is currently .NET 6/C# 10.

  single-file                              Bindings should be generated to a single output file. This is the default.
  multi-file                               Bindings should be generated so there is approximately one type per file.

  unix-types                               Bindings should be generated assuming Unix defaults. This is the default on
                                           Unix platforms.
  windows-types                            Bindings should be generated assuming Windows defaults. This is the default
                                           on Windows platforms.

  exclude-anonymous-field-helpers          The helper ref properties generated for fields in nested anonymous structs
                                           and unions should not be generated.
  exclude-com-proxies                      Types recognized as COM proxies should not have bindings generated. Thes
                                           are currently function declarations ending with _UserFree, _UserMarshal,
                                           _UserSize, _UserUnmarshal, _Proxy, or _Stub.
  exclude-default-remappings               Default remappings for well known types should not be added. This currently
                                           includes intptr_t, ptrdiff_t, size_t, and uintptr_t
  exclude-empty-records                    Bindings for records that contain no members should not be generated. These
                                           are commonly encountered for opaque handle like types such as HWND.
  exclude-enum-operators                   Bindings for operators over enum types should not be generated. These are
                                           largely unnecessary in C# as the operators are available by default.
  exclude-fnptr-codegen                    Generated bindings for latest or preview codegen should not use function
                                           pointers.
  exclude-funcs-with-body                  Bindings for functions with bodies should not be generated.
  preview-codegen-nint                     Generated bindings for latest or preview codegen should not use nint or
                                           nuint.
  exclude-using-statics-for-enums          Enum usages should be fully qualified and should not include a
                                           corresponding 'using static EnumName;'

  explicit-vtbls                           VTBLs should have an explicit type generated with named fields per entry.
  implicit-vtbls                           VTBLs should be implicit to reduce metadata bloat. This is the current
                                           default

  generate-tests-nunit                     Basic tests validating size, blittability, and associated metadata should
                                           be generated for NUnit.
  generate-tests-xunit                     Basic tests validating size, blittability, and associated metadata should
                                           be generated for XUnit.

  generate-aggressive-inlining             [MethodImpl(MethodImplOptions.AggressiveInlining)] should be added to
                                           generated helper functions.
  generate-cpp-attributes                  [CppAttributeList("")] should be generated to document the encountered C++
                                           attributes.
  generate-macro-bindings                  Bindings for macro-definitions should be generated. This currently only
                                           works with value like macros and not function-like ones.
  generate-native-inheritance-attribute    [NativeInheritance("")] attribute should be generated to document the
                                           encountered C++ base type.
  generate-vtbl-index-attribute            [VtblIndex(#)] attribute should be generated to document the underlying
                                           VTBL index for a helper method.

  log-exclusions                           Alist of excluded declaration types should be generated. This will also log
                                           if the exclusion was due to an exact or partial match.
  log-potential-typedef-remappings         Alist of potential typedef remappings should be generated. This can help
                                           identify missing remappings.
  log-visited-files                        Alist of the visited files should be generated. This can help identify
                                           traversal issues.
```

## Spotlight

The P/Invoke Generator is currently used by several projects:

* [microsoft/clangsharp](https://github.com/microsoft/clangsharp) - ClangSharp is self-hosting
* [microsoft/llvmsharp](https://github.com/microsoft/llvmsharp) - Bindings over libLLVM
* [microsoft/win32metadata](https://github.com/microsoft/win32metadata) - Bindings over the Windows SDK meant for downstream use by projects such as CsWin32, RsWin32, etc
* [terrafx/terrafx.interop.windows](https://github.com/terrafx/terrafx.interop.windows) - Bindings for D3D10, D3D11, D3D12, D2D1, DWrite, WIC, User32, and more in a single NuGet
* [terrafx/terrafx.interop.vulkan](https://github.com/terrafx/terrafx.interop.vulkan) - Bindings for Vulkan
* [terrafx/terrafx.interop.xlib](https://github.com/terrafx/terrafx.interop.xlib) - Bindings for Xlib
