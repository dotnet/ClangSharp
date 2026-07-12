// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCIvarRefExpr : Expr
{
    private ValueLazy<ObjCIvarRefExpr, ObjCIvarDecl> _decl;

    internal unsafe ObjCIvarRefExpr(CXCursor handle) : base(handle, CXCursor_MemberRefExpr, CX_StmtClass_ObjCIvarRefExpr)
    {
        Debug.Assert(NumChildren is 1);
        _decl = new ValueLazy<ObjCIvarRefExpr, ObjCIvarDecl>(&DeclFactory);
    }

    public Expr Base => (Expr)Children[0];

    public ObjCIvarDecl Decl => _decl.GetValue(this);

    public bool IsArrow => Handle.IsArrow;

    private static unsafe ObjCIvarDecl DeclFactory(ObjCIvarRefExpr self) => self.TranslationUnit.GetOrCreate<ObjCIvarDecl>(self.Handle.Referenced);
}
