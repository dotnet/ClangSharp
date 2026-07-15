// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression tests for https://github.com/dotnet/ClangSharp/issues/333 and
/// https://github.com/dotnet/ClangSharp/issues/461.
/// <c>--type-prefix-strip</c> strips a library-wide prefix from enum, struct, and union type names
/// (and their enum member names), at both the declaration and every reference, analogous to how
/// <c>--prefix-strip</c> works for methods.
/// </summary>
[Platform("win")]
public sealed class TypePrefixStripTest : StandaloneBaselineTest
{
    protected override string Area => "TypePrefixStrip";

    private const string InputContents = @"typedef struct abc_some_struct
{
    int value;
} abc_some_struct;

typedef enum abc_some_enum
{
    abc_some_enum_key1,
    abc_some_enum_key2,
} abc_some_enum;

typedef struct abc_2d_point
{
    int x;
    int y;
} abc_2d_point;

void abc_use(abc_some_struct* s, abc_some_enum e, abc_2d_point* p);
";

    // The prefix is stripped from the type declarations and every reference to them. Methods are left
    // untouched (they have their own `--prefix-strip`), and a strip that would leave a leading digit keeps
    // an underscore so the result stays a valid C# identifier (`abc_2d_point` -> `_2d_point`).
    [Test]
    public Task PrefixIsStrippedFromTypes()
        => ValidateGeneratedCSharpLatestWindowsBaselineAsync(InputContents, typePrefixToStrip: "abc_");

    // Combined with `strip-enum-member-type-name`, the members collapse all the way to `key1`/`key2`:
    // the library prefix is stripped first, then the (already stripped) enum type name.
    [Test]
    public Task PrefixAndMemberTypeNameStripped()
        => ValidateGeneratedCSharpLatestWindowsBaselineAsync(InputContents, PInvokeGeneratorConfigurationOptions.StripEnumMemberTypeName, typePrefixToStrip: "abc_");
}
