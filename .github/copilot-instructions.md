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

Tests are **golden-file** and cover a matrix of `CSharp`/`Xml` × `Default`/`Latest`/`Preview`/`Compatible`
× `Windows`/`Unix` (16 variants). Fixtures live in `tests/ClangSharp.PInvokeGenerator.UnitTests/Baseline/*.cs`
(e.g. `FunctionDeclarationBodyImportTest`), derive from `BaselineTest`, and are parameterized over every
variant via `[TestFixtureSource]`. A concrete case supplies its C/C++ `inputContents` and a case name once
by calling `ValidateAsync(nameof(TheTest), inputContents, ...)`; the **expected output is a checked-in
baseline file**, not inline text.

Baselines live under `Baseline/Baselines/{Area}/` and are resolved per variant via a most-specific-first
fallback chain (`BaselineHarness.CandidateNames`):
`{Case}.{Mode}.{Config}.{Os}.{ext}` → `{Case}.{Mode}.{Config}.{ext}` → `{Case}.{Mode}.{ext}`.

Consequences when you change generator output:
- Regenerate baselines with `UPDATE_BASELINES=1` (it also implies `RUN_ALL_VARIANTS=1`, so both OSes are
  written from one host via the pinned Unix triple). This writes the **most-specific** file for every
  variant — i.e. fully expanded — so you **must then unify** (see below).
- **Always unify identical baselines — do not check in duplicates.** Collapse output that is identical
  across the OS axis to the `{Case}.{Mode}.{Config}.{ext}` level, and collapse output identical across
  **all 16** variants to a single `{Case}.{Mode}.{ext}` per mode. Keep one file per `Config` when configs
  differ, even if two happen to match (matching `AccessUnionMemberTest` / `VirtualTest`); do **not** invent
  a global-plus-override mix — the repo doesn't use it. Verify a collapse with
  `RUN_ALL_VARIANTS=1 dotnet test ... --filter FullyQualifiedName~TheTest` so both OSes are checked against
  the collapsed files.
- Add a regression test (in the matching `Baseline/*Test.cs`) for any fix; that is the established pattern.

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
