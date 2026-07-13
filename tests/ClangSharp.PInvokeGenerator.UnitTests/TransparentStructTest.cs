// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class TransparentStructTest : StandaloneBaselineTest
{
    protected override string Area => "TransparentStruct";

    [Test]
    public Task BooleanKindGeneratesValidHelper()
    {
        var inputContents = @"typedef unsigned char MyBoolean;";

        var withTransparentStructs = new Dictionary<string, (string, PInvokeGeneratorTransparentStructKind)> {
            ["MyBoolean"] = ("byte", PInvokeGeneratorTransparentStructKind.Boolean),
        };

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateHelperTypes, withTransparentStructs: withTransparentStructs);
    }
}
