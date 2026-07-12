// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_ConstructionKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXInheritedCtorInitExpr : Expr
{
    private ValueLazy<CXXInheritedCtorInitExpr, CXXConstructorDecl> _constructor;

    internal unsafe CXXInheritedCtorInitExpr(CXCursor handle) : base(handle, CXCursor_CallExpr, CX_StmtClass_CXXInheritedCtorInitExpr)
    {
        Debug.Assert(NumChildren is 0);
        _constructor = new ValueLazy<CXXInheritedCtorInitExpr, CXXConstructorDecl>(&ConstructorFactory);
    }

    public CX_ConstructionKind ConstructionKind => ConstructsVBase ? CX_CK_VirtualBase : CX_CK_NonVirtualBase;

    public CXXConstructorDecl Constructor => _constructor.GetValue(this);

    public bool ConstructsVBase => Handle.ConstructsVirtualBase;

    public bool InheritedFromVBase => Handle.InheritedFromVBase;

    private static unsafe CXXConstructorDecl ConstructorFactory(CXXInheritedCtorInitExpr self) => self.TranslationUnit.GetOrCreate<CXXConstructorDecl>(self.Handle.Referenced);
}
