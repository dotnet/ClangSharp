# Generating bindings: best practices

This guide is an opinionated walkthrough of how to drive `ClangSharpPInvokeGenerator` well for a
real, maintainable binding project. It complements the option reference in the
[main README](../README.md#generating-bindings) and the output of `--help` and `--config help`.

If you have ever wondered *why* the generator emitted nothing, traversed half of the Windows SDK,
or produced a slightly different diff on every run, the problem is almost always in how the tool is
being invoked rather than in the tool itself. The recommendations below are drawn from
[terrafx/terrafx.interop.windows](https://github.com/terrafx/terrafx.interop.windows) — a large,
mature ClangSharp consumer maintained by the same maintainer — and from this repository's own
self-hosted [`GenerateClang.rsp`](../sources/ClangSharpPInvokeGenerator/Properties/GenerateClang.rsp).

## Contents

* [Structure a generation project](#structure-a-generation-project)
* [Use response files](#use-response-files)
* [Split shared settings from per-target settings](#split-shared-settings-from-per-target-settings)
* [Key options and when to use them](#key-options-and-when-to-use-them)
* [Wildcards](#wildcards)
* [Incremental regeneration](#incremental-regeneration)
* [Common pitfalls](#common-pitfalls)

## Structure a generation project

Treat generation as a first-class, checked-in part of your repository, not a one-off command:

* **One folder per target.** In terrafx, `generation/` contains a folder per header being wrapped
  (for example `generation/DirectX/d3d12/d3d12/`). Each folder holds three files:
  * `generate.rsp` — the response file for that target,
  * `header.txt` — the license/header text prefixed to every generated file (via `--header-file`),
  * an umbrella `*.h` that `#include`s exactly what the target should expose.
* **Shared configuration at the root.** Cross-cutting settings live once at `generation/`
  (`settings.rsp`, `remap.rsp`, and topical `remap-*.rsp`) and are pulled in by every target.
* **A thin driver script.** terrafx's `scripts/build.ps1` simply discovers every `generate.rsp`
  and regenerates each in parallel:

  ```powershell
  $generateRspFiles = Get-ChildItem -Path "$generationDir" -Recurse -Filter "generate.rsp"

  $generateRspFiles | ForEach-Object -Parallel {
      Push-Location -Path $_.DirectoryName
      & ClangSharpPInvokeGenerator "@generate.rsp"
      Pop-Location
  }
  ```

  The equivalent on Linux/macOS, using `find` to discover the targets and `xargs` to run them
  in parallel:

  ```bash
  find "$generationDir" -name generate.rsp -print0 |
      xargs -0 -P "$(nproc)" -I{} sh -c 'cd "$(dirname "$1")" && ClangSharpPInvokeGenerator "@generate.rsp"' _ {}
  ```

  The driver has no per-target knowledge — adding a new binding is just a new folder with a
  `generate.rsp`. Because each target is independent, they parallelize cleanly.

* **Check the generated code into source control** alongside the `generation/` inputs, so that
  regenerating produces a reviewable diff (see [Incremental regeneration](#incremental-regeneration)).

## Use response files

The recommended way to invoke the generator is a response file, passed with `@`:

```
ClangSharpPInvokeGenerator @generate.rsp
```

A response file lists command-line arguments one per line, so they can be checked in and reviewed.
Two properties make them scale:

* **`@`-includes compose.** A response file can pull in other response files by prefixing them with
  `@`. terrafx's per-target `generate.rsp` begins by including the shared config, then adds only
  what is unique to that target:

  ```
  @../../../settings.rsp
  @../../../remap.rsp
  --file
  d3d12-d3d12.h
  --namespace
  TerraFX.Interop.DirectX
  --output
  ../../../../sources/Interop/Windows/DirectX/d3d12/d3d12
  ...
  ```

  `remap.rsp` is itself nothing but `@`-includes of topical files, so a single concern
  (handles, namespaces, transparent structs, ...) is defined in exactly one place:

  ```
  @../../../remap-classes.rsp
  @../../../remap-handles.rsp
  @../../../remap-namespaces.rsp
  @../../../remap-transparent.rsp
  @../../../remap-types.rsp
  ...
  ```

* **Relative paths resolve from the invoking directory.** Because the driver changes into each
  target folder before invoking the tool, every relative path (`--file`, `--output`,
  `--include-directory`, and the `@`-includes) is written relative to that folder.

For a minimal single-file example, see this repository's own
[`GenerateClang.rsp`](../sources/ClangSharpPInvokeGenerator/Properties/GenerateClang.rsp).

## Split shared settings from per-target settings

Decide where each option belongs by asking whether it is a property of *the whole project* or of
*one target*.

**Shared (`settings.rsp`)** — the things that should be identical everywhere:

* `--additional` clang arguments, including the architecture (`-m64`) and warning suppression
  (`-Wno-comment`, `-Wno-deprecated-declarations`, `-Wno-ignored-attributes`, ...). Suppressing the
  noise up front keeps real diagnostics visible.
* The `--config` mode families plus the `--generate` / `--log` switches (see
  [config flags](#config-flags--c)).
* `--define-macro` values that shape what the headers expose (for example `UNICODE`, `INITGUID`).
* `--include-directory` search paths (your local headers plus the SDK).
* `--header-file header.txt` and project-wide conventions such as `--with-callconv *=Winapi`.

**Per-target (`generate.rsp`)** — the things unique to one header:

* `--file` (the umbrella header) and `--traverse` (which included files to emit).
* `--namespace`, `--method-class-name`, `--output`, `--test-output`.
* Target-scoped `--exclude`, `--with-attribute`, and `--with-library-path`.

Keeping remappings in shared topical files means a type like `HANDLE` or a namespace like `ABM` is
mapped once and applied consistently to every target that encounters it.

## Key options and when to use them

The switch reference in the [README](../README.md#generating-bindings), `--help`, and `--config help` are
authoritative. This section groups the options by intent and notes when to reach for each.

### Inputs and traversal

* **`-f, --file`** — the input header(s) to parse. Usually a single umbrella header per target.
* **`-t, --traverse`** — *which* included files should have bindings emitted. Without it, the
  generator will happily emit everything transitively reachable, including system headers. This is
  the single most important option for keeping output scoped.
* **`-I, --include-directory`** — the header search path. Order matters; put your own headers before
  the SDK so local overrides win.
* **`-D, --define-macro`** — define macros before parsing (for example `INITGUID`, `UNICODE`).
* **`-a, --additional`** — pass raw arguments to clang. Use for the target architecture and for
  `-Wno-*` warning suppression.
* **`-x, --language` / `-std, --std`** — force C vs C++ and the language standard when the headers
  need it.
* **`-rd, --resource-directory`** — the Clang resource directory holding the builtin headers
  (`stddef.h`, `stdarg.h`, the intrinsics, ...). You normally don't need this: on Unix the tool
  auto-detects an installed, version-matched Clang the same way the `clang` driver locates its own
  builtin headers, and only warns (never fails) when nothing is found. Auto-detection is skipped on
  Windows, where the MSVC/Windows SDK toolchain ships compatible copies of these headers so parsing
  works without one. Set it explicitly to pin a specific toolchain (honored on every platform), or
  pass `--no-resource-directory-detection` to opt out.

### Naming and remapping

* **`-r, --remap`** — rename a declaration during generation. The topical `remap-*.rsp` files show
  the pattern: opaque handle typedefs remapped to themselves to keep a distinct C# type, and
  pointer-sized integers remapped to `@nint`/`@nuint` (`SIZE_T=@nuint`, `INT_PTR=@nint`).
* **`-rt, --remap-type` / `-rf, --remap-field`** — take precedence over `--remap` and disambiguate
  the case where a type and a field share a name.
* **`-wn, --with-namespace`** — place a declaration (or a whole prefix family such as `ABM=`,
  `ACCESS=`) into a chosen namespace. terrafx funnels many prefixes into
  `TerraFX.Interop.Windows`.
* **`-wu, --with-using`, `-wc, --with-class`, `-m, --method-class-name`, `-p, --prefix-strip`** —
  control the `using` directives, the owning static class for free functions, and prefix stripping
  (for example stripping `clang_` in this repo's own bindings).

### Type shaping

* **`-wts, --with-transparent-struct`** — wrap a typedef in a strongly-typed transparent struct.
  The value is `underlying;kind`, and the *kind* controls the generated conveniences —
  `HANDLE=void*;HandleWin32`, `BOOL=int;Boolean`, `COLORREF=uint;TypedefHex`. This is how terrafx
  turns dozens of `void*` handle typedefs into distinct, safe C# types.
* **`-wt, --with-type`** — set the backing type for an enum declaration.
* **`-wa, --with-attribute`** — attach an attribute to a declaration. Two common uses: marking flag
  enums (`D3D12_RESOURCE_FLAGS=Flags`) and platform gating
  (`ID3D12Device4=SupportedOSPlatform("windows10.0.19043.0")`).
* **`-was, --with-access-specifier`, `-wp, --with-packing`, `-wl, --with-length`,
  `-wg, --with-guid`, `-wro, --with-readonly`** — fine-grained control over accessibility, struct
  packing, array lengths, GUIDs, and `readonly` methods.

### Interop and P/Invoke

* **`-l, --library-path` / `-wlb, --with-library-path`** — the DLL name for `DllImport`. Use
  `--with-library-path` to route specific functions to specific libraries
  (`D3D12CreateDevice=d3d12`).
* **`-wcc, --with-callconv`** — the calling convention, commonly `*=Winapi` project-wide.
* **`-wsle, --with-setlasterror`, `-wsgct, --with-suppressgctransition`,
  `-wmi, --with-manual-import`** — opt specific functions into `SetLastError`,
  `SuppressGCTransition` (for tiny, hot, non-blocking calls such as the D2D1 math helpers), or
  manual import.

### Exclusions

* **`-e, --exclude` / `-i, --include`** — drop or keep specific declarations. Use `--exclude` for
  macros and helpers that don't translate (for example the `D3D12_DECODE_*`/`D3D12_ENCODE_*` macro
  helpers, or `CINDEX_VERSION_*` in this repo). They are an opt-in/opt-out pair: `--exclude` removes
  a named declaration, `--include` keeps one that would otherwise be dropped.
* The `--generate` features handle *categories* of declarations (COM proxies, empty records, enum
  operators, ...); use `=false` to opt a category out.

### Config flags (`-c`)

`-c, --config` now carries only the four mode families below (plus `help`); the feature switches
moved to `--generate <name>` and the diagnostics to `--log <name>`. Run
`ClangSharpPInvokeGenerator --config help` for the full, authoritative list. `--generate` booleans
accept an optional `=true`/`=false` (with `=true` implied when omitted), so a later response file can
override an earlier one. The groups worth knowing:

* **Codegen level (`-c codegen=`)** — `compatible` (.NET Standard 2.0), `default` (current LTS),
  `latest`, `preview`. Pick the one that matches your target framework; the more modern levels enable
  function pointers and other niceties.
* **File layout (`-c file=`)** — `single` (default) vs `multi` (roughly one type per file, easier to
  review and diff for large projects).
* **Type assumptions (`-c types=`)** — `windows` vs `unix`.
* **Vtbl strategy (`-c vtbls=`)** — `explicit`, `implicit` (default), `trimmable`.
* **Tests (`--generate`)** — `--generate tests-nunit` / `--generate tests-xunit` emit
  size/blittability tests alongside the bindings.
* **Generation toggles (`--generate`)** — for example `--generate aggressive-inlining`,
  `--generate file-scoped-namespaces`, `--generate guid-member`, `--generate marker-interfaces`,
  `--generate native-inheritance-attribute`, `--generate vtbl-index-attribute`.

### Output and test modes

* **`-o, --output`** — where bindings are written. With `-c file=multi`, this is a directory.
* **`-to, --test-output`** — where the generated tests are written (pair with a
  `--generate tests-*` feature).
* **`-om, --output-mode`** — `CSharp` (default) or `Xml`.

## Wildcards

Most name-matching options accept **glob patterns** built from two wildcards:

* `*` — matches any run of characters, including the `::`/`.` qualification separators.
* `?` — matches a single character.

`::` and `.` are treated as equivalent separators and matching is case-sensitive, so `NS::Foo*` and
`NS.Foo*` behave identically. When several entries match one declaration, an **exact match always
wins over a glob**, and among globs the **most specific** (the one with the most literal, non-wildcard
characters) wins; ties fall back to the order the patterns were registered. A glob is tested against
the declaration's qualified, parameter-truncated, and remapped name forms — a match on any form
counts. For example:

```
# Make every type whose name ends in "Flags" internal, and give every "PFN" callback the Winapi convention
--with-access-specifier
*Flags=Internal
--with-callconv
PFN*=Winapi
```

Many valued `--with-*` options also accept a bare `*` as a catch-all, applying a rule to everything in
one line. This is written `*=value`, which is how project-wide conventions are applied, for example:

```
--with-callconv
*=Winapi
```

```
--with-access-specifier
*=Internal
```

Each such option has a paired `--without-<name>` option that opts a specific declaration back out of
its `*` catch-all (or out of a glob opt-in), matched by exact name or by glob, so opt-in and opt-out
are symmetric:

```
# Make everything internal, except keep Foo (and anything matching Bar*) at its default accessibility
--with-access-specifier
*=Internal
--without-access-specifier
Foo
--without-access-specifier
Bar*
```

To opt everything in and then exclude piecemeal (or vice versa) for whole declarations, use the
`--include` / `--exclude` pair, which is already an opt-in/opt-out pair and likewise accepts globs
(including a bare `*` to match everything).

A few options are narrower: `--with-class` and `--with-namespace` accept only a trailing `*` for
prefix matching, and `--with-transparent-struct` is matched by exact name only.

## Incremental regeneration

* **Commit the generated output.** With generated code in source control, regenerating turns every
  header/SDK bump into a reviewable diff instead of an opaque black box.
* **Regenerate per target, in parallel.** Because each `generate.rsp` is self-contained, the driver
  can regenerate every target concurrently; you only re-run the ones whose inputs changed.
* **Pin your inputs.** Keep the SDK/include versions in the shared `settings.rsp` (terrafx pins an
  explicit Windows Kits version) so output stays deterministic across machines and CI.
* **Diagnose with the `--log` switches** rather than guessing. These are the practical answer when
  the output isn't what you expected:
  * `--log exclusions` — lists every declaration that was excluded, and whether it matched exactly or
    partially. Use this when something you wanted is missing.
  * `--log potential-typedef-remappings` — surfaces typedefs that look like they *should* be remapped,
    which helps you find missing `--remap`/`--with-transparent-struct` entries.
  * `--log visited-files` — lists the files that were actually visited, which is the fastest way to
    catch a `--traverse`/`--include-directory` mistake.

## Common pitfalls

* **Forgetting `--traverse`.** Without it the generator emits everything reachable, including system
  headers. Scope every target with `--traverse` and confirm with `--log visited-files`.
* **`--include-directory` order.** If the wrong copy of a header is found first, you get subtly
  wrong output. Put local include directories before the SDK.
* **Skipping warning suppression.** Real headers produce many benign clang warnings; add the
  `-Wno-*` set to `--additional` so genuine parse errors aren't buried.
* **`--remap` vs `--remap-type`/`--remap-field` clashes.** When a type and field share a name, use
  the more specific option — it takes precedence and avoids remapping the wrong entity.
* **Transparent-struct kind strings.** The kind after the `;` (for example `HandleWin32`,
  `Boolean`, `TypedefHex`) determines the generated helpers; picking the wrong one changes the API
  surface. Model new handles on the existing `remap-transparent.rsp` entries.
* **Flag enums.** Native enums used as bit flags won't get `[Flags]` automatically; add
  `--with-attribute NAME=Flags`.
* **Platform-specific paths.** Absolute SDK include paths in `settings.rsp` are environment
  specific; keep them centralized so there is a single place to update them.
