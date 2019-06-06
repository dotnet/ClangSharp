# ClangSharp

ClangSharp are strongly-typed safe Clang bindings written in C# for .NET and Mono, tested on Linux and Windows. ClangSharp is self-hosted and ClangSharp auto-generates itself parsing LLVM-C header files.

| Job | Debug Status | Release Status |
| --- | ------------ | -------------- |
| Windows x86 | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=windows_debug_x86)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=windows_release_x86)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) |
| Windows x64 | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=windows_debug_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=windows_release_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) |
| Ubuntu 16.04 x64 | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=ubuntu_1604_debug_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) | [![Build Status](https://dev.azure.com/ms/ClangSharp/_apis/build/status/microsoft.ClangSharp?branchName=master&jobName=ubuntu_1604_release_x64)](https://dev.azure.com/ms/ClangSharp/_build/latest?definitionId=155&branchName=master) |

If you're on Windows, consider using the [**ClangSharp 3.6 NuGet Package**](http://www.nuget.org/packages/ClangSharp/3.6.0) - built from Clang 3.6 Release.

## Building ClangSharp

```bash
dotnet msbuild /t:GenerateClangSharp ClangSharpPInvokeGenerator
```

## Features

 * Auto-generated using Clang C headers files, and supports all functionality exposed by them ~ which means you can build tooling around C/C++
 * Type safe (CXIndex and CXTranslationUnit are different types, despite being pointers internally)
 * Nearly identical to Clang C APIs, e.g. clang_getDiagnosticSpelling in C, vs. clang.getDiagnosticSpelling (notice the . in the C# API)

## ClangSharp PInvoke Generator

A great example of ClangSharp's use case is its self-hosting mechanism [Clang Sharp PInvoke Generator](https://github.com/mjsabby/ClangSharp/tree/master/ClangSharpPInvokeGenerator)

## Microsoft Open Source Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
