// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

[TestFixtureSource(nameof(Variants))]
public sealed class MemberPointerDeclarationTest : BaselineTest
{
    public MemberPointerDeclarationTest(BaselineVariant variant) : base(variant)
    {
    }

    protected override string Area => "MemberPointerDeclaration";

    // C++ pointers-to-members have no C# equivalent, so the generator degrades to an opaque `void*` and a
    // warning rather than hard-erroring the whole run (see dotnet/ClangSharp#511). Windows targets use the
    // MSVC ABI, which additionally attaches an MSInheritance attribute to the referenced class.
    private IReadOnlyList<Diagnostic> ExpectedDiagnostics(int column)
    {
        var memberPointer = new Diagnostic(DiagnosticLevel.Warning, "Unsupported type: 'CX_TypeClass_MemberPointer'. Emitting an opaque 'void*'; the size and layout may be incorrect for member function pointers.", $"Line 5, Column {column} in ClangUnsavedFile.h");

        if (Variant.Os == BaselineOs.Windows)
        {
            var msInheritance = new Diagnostic(DiagnosticLevel.Warning, "Unsupported attribute: 'MSInheritance'. Generated bindings may be incomplete.", "Line 1, Column 8 in ClangUnsavedFile.h");
            return [msInheritance, memberPointer];
        }

        return [memberPointer];
    }

    [Test]
    public Task DataMemberPointerTest()
    {
        var inputContents = @"struct MyClass;

struct MyStruct
{
    int MyClass::* field;
};
";

        return ValidateAsync(nameof(DataMemberPointerTest), inputContents, expectedDiagnostics: ExpectedDiagnostics(column: 20));
    }

    [Test]
    public Task FunctionMemberPointerTest()
    {
        var inputContents = @"struct MyClass;

struct MyStruct
{
    void (MyClass::* callback)(int);
};
";

        return ValidateAsync(nameof(FunctionMemberPointerTest), inputContents, expectedDiagnostics: ExpectedDiagnostics(column: 22));
    }
}
