// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>Provides validation that a pointer to an <c>__unaligned</c>-qualified record resolves to the record type
/// rather than leaking the qualifier and elaborated <c>struct</c> keyword (e.g. <c>__unaligned struct tagFoo*</c>),
/// which is not valid C#.</summary>
[Platform("win")]
public sealed class UnalignedTypeBindingTest : StandaloneBaselineTest
{
    protected override string Area => "UnalignedTypeBinding";

    [Test]
    public Task UnalignedRecordPointerParameterTest()
    {
        var inputContents = @"typedef struct tagMETARECORD { int rdSize; } METARECORD;
typedef struct tagMETARECORD __unaligned *LPMETARECORD;

void PlayMetaFileRecord(LPMETARECORD lpMR);";

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(inputContents);
    }
}
