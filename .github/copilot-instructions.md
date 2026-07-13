# ClangSharp — Copilot instructions

ClangSharp provides Clang bindings in C#. It is **self-hosted**: the hand-written generator parses the
Clang/libClang C headers and auto-generates most of the repo's own bindings. Read this before making
changes so you edit the right layer and validate correctly.

## Repository layout — what is hand-written vs generated

| Path | Kind | Edit here? |
| --- | --- | --- |
| `sources/ClangSharp.PInvokeGenerator/` | **Hand-written generator library** — the real logic. `PInvokeGenerator*.cs`, `CSharp/`, `XML/`, `Abstractions/`. | **Yes** — this is where behavior changes go. |
| `sources/ClangSharpPInvokeGenerator/` | Hand-written CLI wrapping the library (`Program.cs`, `.rsp` files). | Yes, for CLI/option changes. |
| `sources/libClangSharp/` | Native C++ helper (CMake) exposing APIs missing from libClang. | Yes, for native surface. Requires building LLVM/Clang. |
| `sources/ClangSharp/` | **Auto-generated** high-level bindings (mirrors the Clang C++ type hierarchy). | **No** — regenerate, don't hand-edit. |
| `sources/ClangSharp.Interop/` | **Auto-generated** raw interop (plus a few hand-written interop helpers). | **No** for generated files — regenerate. |
| `tests/` | NUnit tests. `PInvokeGenerator.UnitTests` = golden-file generator tests; `UnitTests` = interop/`InteropTests`. | Yes. |

If a change belongs in `sources/ClangSharp/` or `sources/ClangSharp.Interop/`, it almost always belongs
in the **generator** instead — change the generator, then regenerate.

## Build / test (do this to validate every change)

Requires the **.NET 10 SDK** (`global.json` pins `10.0.100-preview`, `allowPrerelease`). `dotnet` resolves it.

- Build: `dotnet build -c Release` (root, builds `ClangSharp.slnx`). ~20s incremental, clean.
- Test: `dotnet test -c Release --no-build` (~1 min; ~3760 tests: 3706 generator + interop on `net10.0`).
- `--no-build` requires a prior successful build; drop it if you changed code since the last build.

`TreatWarningsAsErrors=true`, `Nullable=enable`, `AnalysisLevel=latest-all`, and `EnforceCodeStyleInBuild=true`
are set repo-wide — **a style/analyzer/nullable violation fails the build**, so a green build is a real bar.
The baseline builds with **0 warnings**; keep it that way.

## Generator test model (read before touching generator output)

Tests are **golden-file** and cover a matrix. `tests/ClangSharp.PInvokeGenerator.UnitTests/Base/*` defines
abstract `*Impl` test methods; per-configuration folders implement them with **inline expected output**:

- Output modes × language levels × OS: `CSharp`/`Xml` × `Default`/`Latest`/`Preview`/`Compatible` × `Windows`/`Unix`.
- Concrete cases feed `inputContents` (C/C++) + `expectedOutputContents` to `ValidateGenerated...Async`.

Consequences when you change generator output:
- Update **every affected configuration variant**, not just one — the same case is duplicated across folders.
- Windows/Unix variants are `[Platform(...)]`-gated, so a single-OS run only exercises half the matrix.
  CI runs Windows + Linux + macOS (x64 and arm64); confirm both `Windows` and `Unix` expected outputs.
- Add a regression test in `Base/` (+ all variants) for any fix; that is the established pattern here.

## Regenerating the self-hosted bindings

After a generator change, regenerate `sources/ClangSharp[.Interop]` via the launch profiles in
`sources/ClangSharpPInvokeGenerator/Properties/launchSettings.json` (`GenerateClang`, `GenerateClangSharp`),
which run the CLI against `GenerateClang.rsp` / `GenerateClangSharp.rsp`. This needs `LLVMIncludePath` (the
Clang C headers) and the native `libClang`/`libClangSharp` — it will **not** run in a bare checkout without
them. If you can't regenerate in-session, say so and describe the expected diff rather than hand-editing
generated files.

## Conventions

- Follow `.editorconfig` and the surrounding file. 4-space indent, LF, UTF-8, final newline, trim trailing
  whitespace; 2-space for `csproj`/`props`/`targets`/`json`/`yml`/`sh`/`cmd`. Naming rules are enforced as
  **errors** (`_camelCase` private fields, `s_` static fields, `I`/`T` prefixes, PascalCase publics, etc.).
- File header on new `.cs` files: `// Copyright (c) .NET Foundation and Contributors. All Rights Reserved.
  Licensed under the MIT License (MIT). See License.md in the repository root for more information.` This
  matches the repo norm, `LICENSE.md`, and the `.editorconfig` `file_header_template`.
- Commit messages: imperative, sentence case, specific, **no trailing period** (e.g. `Fix incorrect constant
  folding of CompareScalar comparisons`). Start with a strong verb (`Add`/`Fix`/`Ensure`/`Expose`/`Cleanup`).
- Keep changes focused; defer unrelated cleanup to a follow-up and note it.
- Prefer reusing existing generator helpers over adding a parallel path; keep Windows/Unix and signed/unsigned
  branches consistent unless there's a stated reason to diverge.

## Gotchas

- Don't hand-edit `sources/ClangSharp/` or `sources/ClangSharp.Interop/` generated files — the change won't
  survive the next regeneration and belongs in the generator.
- Three generator files are large (`PInvokeGenerator.cs` ~266KB, `PInvokeGenerator.VisitDecl.cs` ~152KB,
  `PInvokeGenerator.VisitStmt.cs` ~110KB). Use `view_range`/grep; don't read them whole.
- Version/packaging lives in `Directory.Build.props` (`VersionPrefix`) and `Directory.Packages.props`
  (`libClang`/`libClangSharp` pins) — bump them together and match the LLVM release in the README.
