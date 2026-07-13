// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression tests for https://github.com/dotnet/ClangSharp/issues/559.
/// The <c>--with-type</c> option is extended to accept a field-qualified key (<c>Struct.field=Type</c>)
/// so the emitted type of an individual struct field can be overridden. The original Clang type is
/// preserved as a <c>[NativeTypeName]</c> attribute and unrelated fields are left untouched.
/// </summary>
[Platform("win")]
public sealed class WithTypeFieldTest : PInvokeGeneratorTest
{
    [Test]
    public Task OverridesFieldType()
    {
        var inputContents = @"struct Something
{
    int type;
    int other;
};
";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct Something
    {
        [NativeTypeName(""int"")]
        public SomethingType type;

        public int other;
    }
}
";

        var withTypes = new Dictionary<string, string>
        {
            ["Something.type"] = "SomethingType",
        };

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, withTypes: withTypes);
    }
}
