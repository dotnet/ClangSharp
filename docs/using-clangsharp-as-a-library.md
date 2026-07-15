# Using ClangSharp as a library

ClangSharp is first and foremost a *bindings* library. It exposes Clang's C API (libClang) and a
higher-level surface that mirrors Clang's C++ AST to .NET. It is explicitly **not** a goal of this
repository to document how to use Clang itself — Clang provides its own documentation and that is
always the source of truth:

* [Clang documentation index](https://clang.llvm.org/docs/index.html)
* [libClang: C Interface to Clang](https://clang.llvm.org/docs/LibClang.html)
* [Introduction to the Clang AST](https://clang.llvm.org/docs/IntroductionToTheClangAST.html)

Working with ClangSharp is an **advanced** scenario. It assumes you are already comfortable with the
Clang APIs (which are documented in terms of C and C++), with translating those concepts into
equivalent C# (`IDisposable`, spans, pointers, etc.), and with consuming NuGet packages. If you can
follow the upstream Clang docs, the code translates over almost directly.

This document only covers the **non-obvious differences** between ClangSharp and the underlying Clang
APIs, so that you can follow that upstream documentation directly rather than re-learning it here.

## Two layers

ClangSharp ships two related but distinct surfaces:

* **`ClangSharp.Interop`** — the low-level, effectively 1:1 bindings over libClang's stable C API. The
  `clang_*` functions and `CX*` types documented in
  [LibClang](https://clang.llvm.org/docs/LibClang.html) live here.
* **`ClangSharp`** — a higher-level layer that mirrors the Clang C++ AST (`Cursor`, `Decl`, `Stmt`,
  `Type`, and their many subclasses). This is the surface described by
  [Introduction to the Clang AST](https://clang.llvm.org/docs/IntroductionToTheClangAST.html).

Because both layers deliberately mirror their upstream counterparts, the upstream documentation
remains valid — you are mostly applying the naming and lifetime conventions below.

## How libClang maps to `ClangSharp.Interop`

The libClang C functions are all prefixed with `clang_` and take the object they operate on as the
first parameter, for example:

```c
CXType clang_getCursorType(CXCursor C);
```

In ClangSharp these are exposed as static methods on the `clang` class with the `clang_` prefix
dropped and the leading character left as-is:

```csharp
CXType type = clang.getCursorType(cursor);
```

Where it reads more naturally, the common getters are *also* surfaced as instance members on the
`CX*` types, so the call above can equivalently be written:

```csharp
CXType type = cursor.Type;
```

The `CX*` handle types (`CXIndex`, `CXTranslationUnit`, `CXCursor`, `CXType`, ...) keep their upstream
names, so anything you read in the libClang docs has an obvious ClangSharp equivalent.

## How the Clang C++ AST maps to `ClangSharp`

The high-level layer mirrors the Clang C++ class hierarchy, so the concepts from
[Introduction to the Clang AST](https://clang.llvm.org/docs/IntroductionToTheClangAST.html) carry
over directly:

* `Cursor` is the base type; `Decl`, `Stmt`, `Expr`, and `Type` (and their subclasses such as
  `FunctionDecl`, `CallExpr`, `ReturnStmt`) derive from it and match the upstream names.
* You obtain the root from a `TranslationUnit` via `TranslationUnit.TranslationUnitDecl` and walk it
  from there.

Instances are cached: rather than constructing these types yourself, you get them through
`TranslationUnit.GetOrCreate(...)`, which returns the same managed object for a given underlying
handle. This means reference equality is meaningful and you should not `new` them up directly.

## Lifetime and disposal

Several Clang objects own native resources and must be released. In C++/libClang this is done with
explicit `clang_dispose*` calls; in ClangSharp the equivalent types implement `IDisposable`, so wrap
them in `using`:

* `CXIndex` — created with `CXIndex.Create()`, disposed via its `Dispose()`.
* `TranslationUnit` — obtained via `TranslationUnit.GetOrCreate(...)`; disposing it releases the
  underlying `CXTranslationUnit`.

As with any unsafe/interop code, the runtime cannot validate that handles are still alive — using a
cursor or type after its owning `TranslationUnit` has been disposed is undefined behavior, exactly as
it would be in C++.

## Package references

Consuming ClangSharp only requires the `ClangSharp` package itself — it brings the native `libClang`
and `libClangSharp` runtimes in transitively. You do, however, need to specify a `RuntimeIdentifier`
so the correct platform-specific runtime package is restored:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>

    <!-- Required so the platform-specific libClang/libClangSharp runtime package is restored -->
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  </PropertyGroup>

  <ItemGroup>
    <!-- The native libClang/libClangSharp runtimes come in transitively -->
    <PackageReference Include="ClangSharp" Version="21.1.8.4" />
  </ItemGroup>

</Project>
```

`libclang` and `libClangSharp` are meta-packages that point at platform-specific runtime packages
(for example `libClangSharp.runtime.win-x64`). Several manual steps may be required depending on your
setup; see the discussion in [#46](https://github.com/dotnet/ClangSharp/issues/46) and
[#118](https://github.com/dotnet/ClangSharp/issues/118).

## A minimal example

The following shows the conventions above in practice — creating an index, parsing a source file, and
walking the AST. Everything it does maps directly onto the libClang and Clang AST documentation linked
at the top of this page:

```csharp
using ClangSharp;
using ClangSharp.Interop;

using CXIndex index = CXIndex.Create();

ReadOnlySpan<string> args = ["--language=c"];
ReadOnlySpan<CXUnsavedFile> unsavedFiles = [];
CXTranslationUnit handle = CXTranslationUnit.CreateFromSourceFile(index, "main.c", args, unsavedFiles);

using TranslationUnit tu = TranslationUnit.GetOrCreate(handle);
PrintDecl(tu.TranslationUnitDecl, indent: "");

static void PrintDecl(Decl decl, string indent)
{
    Console.WriteLine($"{indent}{decl.DeclKindName} {decl.Spelling}");

    foreach (Decl child in decl.Decls)
    {
        PrintDecl(child, indent + "  ");
    }
}
```

For anything beyond this — what a given cursor, declaration, statement, or type *means* — refer to the
upstream Clang documentation. ClangSharp intentionally follows it closely.

## Using the generator instead

If your goal is to produce P/Invoke bindings for a C or C++ library rather than to inspect the AST
yourself, you likely want the `ClangSharpPInvokeGenerator` tool rather than the raw library. See
[Generating bindings: best practices](generating-bindings-best-practices.md) and the
[main README](../README.md#generating-bindings).
