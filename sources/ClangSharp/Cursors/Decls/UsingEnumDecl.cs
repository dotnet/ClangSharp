// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class UsingEnumDecl : BaseUsingDecl, IMergeable<UsingEnumDecl>
{
    private ValueLazy<UsingEnumDecl, EnumDecl> _enumDecl;

    // libClang surfaces a `using enum` declaration with `cursor.kind == CXCursor_EnumDecl`, even
    // though its `DeclKind` is `CX_DeclKind_UsingEnum`, so that is the expected cursor kind here.
    internal unsafe UsingEnumDecl(CXCursor handle) : base(handle, CXCursor_EnumDecl, CX_DeclKind_UsingEnum)
    {
        _enumDecl = new ValueLazy<UsingEnumDecl, EnumDecl>(&EnumDeclFactory);
    }

    public new UsingEnumDecl CanonicalDecl => (UsingEnumDecl)base.CanonicalDecl;

    public EnumDecl EnumDecl => _enumDecl.GetValue(this);

    // TODO: `Handle.Definition` does not resolve to the referenced `EnumDecl` for a `using enum`
    // (it surfaces as an empty `CXCursor_OverloadedDeclRef`); correctly resolving it requires a
    // native libClangSharp shim wrapping `UsingEnumDecl::getEnumDecl()`. See dotnet/clangsharp #633.
    private static unsafe EnumDecl EnumDeclFactory(UsingEnumDecl self) => self.TranslationUnit.GetOrCreate<EnumDecl>(self.Handle.Definition);
}
