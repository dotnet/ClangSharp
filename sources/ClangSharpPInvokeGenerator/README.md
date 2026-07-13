# ClangSharpPInvokeGenerator

ClangSharpPInvokeGenerator is a .NET tool that generates strongly-typed C#/.NET P/Invoke bindings from C and C++ header files by parsing them with libclang. It is part of the [ClangSharp](https://github.com/dotnet/ClangSharp) project, which is self-hosted and uses this tool to auto-generate its own bindings.

## Installing

Install the generator as a global .NET tool:

```
dotnet tool install --global ClangSharpPInvokeGenerator
```

**NOTE:** The generator depends on the native `libClang` and `libClangSharp` libraries. When running as a .NET tool, you may need to manually copy the appropriate native DLLs from the [libClang](https://www.nuget.org/packages/libclang) and [libClangSharp](https://www.nuget.org/packages/libClangSharp) packages due to limitations in the .NET tool support. See [#46](https://github.com/dotnet/ClangSharp/issues/46) and [#118](https://github.com/dotnet/ClangSharp/issues/118) for details.

## Usage

At a minimum, the tool expects one or more input files (`-f`), an output namespace (`-n`), and an output location (`-o`):

```
ClangSharpPInvokeGenerator -f example.h -n Example.Namespace -o Example.cs
```

The simplest and recommended setup is to use a response file, which lets you specify and check in the command line arguments in a text file with one argument per line:

```
ClangSharpPInvokeGenerator @generate.rsp
```

See [GenerateClang.rsp](https://github.com/dotnet/ClangSharp/blob/main/sources/ClangSharpPInvokeGenerator/Properties/GenerateClang.rsp) for a real-world example. A typical response file also specifies explicit files to traverse, configuration options, name remappings, and other fixups.

You can see the full set of available switches by passing `--help`, and the available configuration options by passing `-c help`.

## More information

* Full documentation and the list of options: https://github.com/dotnet/ClangSharp#generating-bindings
* Source repository: https://github.com/dotnet/ClangSharp
* Nightly packages: https://pkgs.clangsharp.dev/index.json

## License

Copyright (c) .NET Foundation and Contributors. All Rights Reserved.
Licensed under the MIT License (MIT). See [LICENSE.md](https://github.com/dotnet/ClangSharp/blob/main/LICENSE.md) in the repository root for more information.
