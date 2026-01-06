# Getting Started

A complete example using ClangSharp is [PInvokeGenerator](../../sources/ClangSharpPInvokeGenerator/), but it's a huge app with lots of options and a bit too much for a tutorial.

Therefore here describes how to get start to using ClangSharp as C/C++ syntactic analysis library.

## Project Overview

[ClangSharp repository](../../) contains several contents and they depend on each other for itself objective.

When used as a library, [sources/ClangSharp](../../sources/ClangSharp/) will be the main content.

<figure>
<img src="./img/project-overview.svg" width="480px" />
</figure>

## References Requirements in Your Project

To get started using ClangSharp, you need append several configurations in your `*.csproj`.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>

    <!-- 👇 Specify RuntimeIdentifier to determine what libClang/libClangSharp runtime version should be installed -->
    <RuntimeIdentifier>linux-x64</RuntimeIdentifier>
  </PropertyGroup>

  <ItemGroup>
    <!-- 👇 Includes ClangSharp -->
    <PackageReference Include="ClangSharp" Version="20.1.2.3" />

    <!-- 👇 Includes libClangSharp -->
    <PackageReference Include="libClangSharp" Version="20.1.2" />
  </ItemGroup>

</Project>
```

## Sample App to Show C language AST

To analyze C/C++ source code with ClangSharp, you need to follow these steps:

1. Creates `CXIndex` instance
2. Creates `CXTranslationUnit` instance with the target source file path
3. Creates `TranslationUnit` instance
4. Traces `Decl` instance from `TranslationUnit.TranslationUnitDecl`

Following source *Program.cs* is minimum sample to show declarations/statements in C source file.

```CSharp
using ClangSharp;
using ClangSharp.Interop;

string sourceFilePath = args[0];
CXIndex index = CXIndex.Create();
Span<string> commandLineArgs = ["--language=c",];
Span<CXUnsavedFile> unsavedFiles = [];
CXTranslationUnit cxtu = CXTranslationUnit.CreateFromSourceFile(index, sourceFilePath, commandLineArgs, unsavedFiles);
using TranslationUnit tu = TranslationUnit.GetOrCreate(cxtu);
Console.WriteLine($"target source file: {tu.TranslationUnitDecl}");
PrintDecl(tu.TranslationUnitDecl, "  ");


static void PrintDecl(Decl decl, string indent)
{
    Console.WriteLine($"{indent}[decl][{decl.Location}] {decl.DeclKindName} {decl.Spelling}");
    indent += "  ";
    foreach (var child in decl.Decls)
    {
        PrintDecl(child, indent);
    }
    if(decl.Body is { } body)
    {
        PrintStmt(body, indent);
    }
}

static void PrintStmt(Stmt stmt, string indent)
{
    Console.WriteLine($"{indent}[stmt][{stmt.Location}] {stmt.StmtClass} {stmt.Spelling}");
    indent += "  ";
    foreach (var child in stmt.Children)
    {
        PrintStmt(child, indent);
    }
}
```

Running this app will produce the following results.

- command

  ```bash
  dotnet run -- main.c
  ```

- input source file `./main.c`

  ```c
  #include <stdio.h>

  int main() {
      printf("Hello, World!\n");
      return 0;
  }
  ```

- console output

  ```plaintext
  target source file: ./main.c
    [decl][Line 0, Column 0 in ] TranslationUnit ./main.c
      [decl][Line 0, Column 0 in ] Typedef __int128_t
      [decl][Line 0, Column 0 in ] Typedef __uint128_t
      [decl][Line 0, Column 0 in ] Typedef __NSConstantString
      [decl][Line 0, Column 0 in ] Typedef __builtin_ms_va_list
      [decl][Line 0, Column 0 in ] Typedef __builtin_va_list
      [decl][Line 3, Column 5 in ./main.c] Function main
        [stmt][Line 3, Column 12 in ./main.c] CX_StmtClass_CompoundStmt
          [stmt][Line 4, Column 5 in ./main.c] CX_StmtClass_FirstCallExpr printf
            [stmt][Line 4, Column 5 in ./main.c] CX_StmtClass_FirstCastExpr printf
              [stmt][Line 4, Column 5 in ./main.c] CX_StmtClass_DeclRefExpr printf
            [stmt][Line 4, Column 12 in ./main.c] CX_StmtClass_FirstCastExpr
              [stmt][Line 4, Column 12 in ./main.c] CX_StmtClass_FirstCastExpr
                [stmt][Line 4, Column 12 in ./main.c] CX_StmtClass_StringLiteral "Hello,   World!\n"
          [stmt][Line 5, Column 5 in ./main.c] CX_StmtClass_ReturnStmt
            [stmt][Line 5, Column 12 in ./main.c] CX_StmtClass_IntegerLiteral
      [decl][Line 4, Column 5 in ./main.c] Function printf
  ```
