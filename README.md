# ClangSharp

ClangSharp provides Clang bindings written in C#. It is self-hosted and auto-generates itself parsing the Clang C header files.

| Job | Debug Status | Release Status |
| --- | ------------ | -------------- |
| Windows x86 | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=windows_debug_x86)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=windows_release_x86)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) |
| Windows x64 | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=windows_debug_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=windows_release_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) |
| Ubuntu 16.04 x64 | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=ubuntu_debug_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=ubuntu_release_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) |
| MacOS x64 | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=macos_debug_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=macos_release_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) |

A nuget package for the project is provided here: https://www.nuget.org/packages/clangsharp. NOTE: This is currently out of date as compared to the current sources and we hope to have a new package published soon.

A convenience package which provides the native libclang library for several platforms is provided here: https://www.nuget.org/packages/libclang

## Building ClangSharp

ClangSharp provides several build scripts in the repository root. On Windows these scripts end with `.cmd` and expect arguments with a `-` prefix. On Unix these scripts end with `.sh` and expect arguments with a `--` prefix.

By default, each script performs only the action specified in its name (i.e. `restore` only restores, `build` only builds, `test` only tests, and `pack` only packs). You can specify additional actions to be run by passing that name as an argument to the script (e.g. `build.cmd -restore` will perform a package restore and build; `test.cmd -pack` will run tests and package artifacts).

Certain actions are dependent on a previous action having been run at least once. `build` depends on `restore`, `test` depends on `build`, and `pack` depends on `build`. This means the recommended first time action is `build -restore`.

You can reproduce what the CI environment does by running `./scripts/cibuild.cmd` on Windows or `./scripts.cibuild.sh` on Unix. This will download the required .NET SDK locally and use that to build the repo. It will also run through all available actions in the appropriate order.

You can see any additional options that are available by passing `-help` on Windows or `--help` on Unix to the available build scripts.

## Features

 * Auto-generated using Clang C headers files, and supports all functionality exposed by them ~ which means you can build tooling around C/C++
 * Exposes the raw unsafe API for performance
 * Exposes a slightly higher abstraction that is type safe (CXIndex and CXTranslationUnit are different types, despite being pointers internally)
 * Exposes an again slightly higher abstraction that tries to mirror the Clang C++ Type Hierarchy where possible
 * Nearly identical to the Clang C APIs, e.g. `clang_getDiagnosticSpelling` in C, vs. `clang.getDiagnosticSpelling` (notice the . in the C# API)

## ClangSharp P/Invoke Binding Generator

A great example of ClangSharp's use case is its self-hosting mechanism: [ClangSharp P/Invoke Binding Generator](sources/ClangSharpPInvokeGenerator).

This program will take a given set of C or C++ header files and generate C# bindings from them. It is still a work-in-progress and not every declaration can have bindings generated today (contributions are welcome).

## Microsoft Open Source Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
