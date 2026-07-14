// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Tests for https://github.com/dotnet/ClangSharp/issues/631.
/// The GeneratedCode attribute has three modes. In the default 'assembly' mode a single
/// <c>[assembly: GeneratedCode("ClangSharp", version)]</c> marker is emitted when helper types are
/// generated. In 'type' mode (<c>generate-generated-code=type</c>) each generated top-level type is
/// annotated with <c>[GeneratedCode("ClangSharp", version)]</c> instead, and no assembly marker is
/// emitted. In 'none' mode (<c>generate-generated-code=none</c>) neither is emitted. The version is the
/// ClangSharp assembly version.
/// </summary>
[Platform("win")]
public sealed class GeneratedCodeAttributeTest : StandaloneBaselineTest
{
    protected override string Area => "GeneratedCodeAttribute";

    private const string InputContents = @"enum MyEnum
{
    MyEnum_Value,
};

struct MyStruct
{
    int value;
};

typedef void (*MyCallback)(int value);

void MyFunction(MyCallback callback);
";

    [Test]
    public Task OmitsAssemblyAttributeWithoutHelperTypes()
        => ValidateGeneratedCSharpLatestWindowsBaselineAsync(InputContents);

    [Test]
    public Task EmitsAssemblyAttributeWithHelperTypes()
        => ValidateGeneratedCSharpLatestWindowsBaselineAsync(InputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateHelperTypes);

    [Test]
    public Task EmitsPerTypeAttributeInTypeMode()
        => ValidateGeneratedCSharpLatestWindowsBaselineAsync(InputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateGeneratedCodeAttributeAsType);

    [Test]
    public Task EmitsPerTypeAttributeOnDelegateInTypeMode()
        => ValidateGeneratedCSharpCompatibleWindowsBaselineAsync(InputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateGeneratedCodeAttributeAsType);

    [Test]
    public Task EmitsPerTypeInsteadOfAssemblyWithHelperTypes()
        => ValidateGeneratedCSharpLatestWindowsBaselineAsync(InputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateHelperTypes | PInvokeGeneratorConfigurationOptions.GenerateGeneratedCodeAttributeAsType);

    [Test]
    public Task OmitsAllAttributesInNoneModeWithHelperTypes()
        => ValidateGeneratedCSharpLatestWindowsBaselineAsync(InputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateHelperTypes | PInvokeGeneratorConfigurationOptions.ExcludeGeneratedCodeAttribute);
}
