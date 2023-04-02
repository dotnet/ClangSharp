// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_ConstructionKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXInheritedCtorInitExpr : Expr
{
    private readonly Lazy<CXXConstructorDecl> _constructor;

    internal CXXInheritedCtorInitExpr(CXCursor handle) : base(handle, CXCursor_CallExpr, CX_StmtClass_CXXInheritedCtorInitExpr)
    {
        Debug.Assert(NumChildren is 0);
        _constructor = new Lazy<CXXConstructorDecl>(() => TranslationUnit.GetOrCreate<CXXConstructorDecl>(Handle.Referenced));
    }

    public CX_ConstructionKind ConstructionKind => ConstructsVBase ? CX_CK_VirtualBase : CX_CK_NonVirtualBase;

    public CXXConstructorDecl Constructor => _constructor.Value;

    public bool ConstructsVBase => Handle.ConstructsVirtualBase;

    public bool InheritedFromVBase => Handle.InheritedFromVBase;
}
