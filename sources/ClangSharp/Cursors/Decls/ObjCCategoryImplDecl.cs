// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ObjCCategoryImplDecl : ObjCImplDecl
{
    private readonly ValueLazy<ObjCCategoryDecl> _categoryDecl;

    internal ObjCCategoryImplDecl(CXCursor handle) : base(handle, CXCursor_ObjCCategoryImplDecl, CX_DeclKind_ObjCCategoryImpl)
    {
        _categoryDecl = new ValueLazy<ObjCCategoryDecl>(() => TranslationUnit.GetOrCreate<ObjCCategoryDecl>(Handle.GetSubDecl(1)));
    }

    public ObjCCategoryDecl CategoryDecl => _categoryDecl.Value;
}
