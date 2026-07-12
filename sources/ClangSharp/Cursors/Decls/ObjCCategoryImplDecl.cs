// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ObjCCategoryImplDecl : ObjCImplDecl
{
    private ValueLazy<ObjCCategoryImplDecl, ObjCCategoryDecl> _categoryDecl;

    internal unsafe ObjCCategoryImplDecl(CXCursor handle) : base(handle, CXCursor_ObjCCategoryImplDecl, CX_DeclKind_ObjCCategoryImpl)
    {
        _categoryDecl = new ValueLazy<ObjCCategoryImplDecl, ObjCCategoryDecl>(&CategoryDeclFactory);
    }

    public ObjCCategoryDecl CategoryDecl => _categoryDecl.GetValue(this);

    private static unsafe ObjCCategoryDecl CategoryDeclFactory(ObjCCategoryImplDecl self) => self.TranslationUnit.GetOrCreate<ObjCCategoryDecl>(self.Handle.GetSubDecl(1));
}
