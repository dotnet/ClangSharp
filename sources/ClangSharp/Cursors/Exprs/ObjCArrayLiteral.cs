// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCArrayLiteral : Expr
{
    private ValueLazy<ObjCArrayLiteral, ObjCMethodDecl> _arrayWithObjectsMethod;
    private readonly LazyList<Expr, Stmt> _elements;

    internal unsafe ObjCArrayLiteral(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_ObjCArrayLiteral)
    {
        _arrayWithObjectsMethod = new ValueLazy<ObjCArrayLiteral, ObjCMethodDecl>(&ArrayWithObjectsMethodFactory);
        _elements = LazyList.Create<Expr, Stmt>(_children);
    }

    public ObjCMethodDecl ArrayWithObjectsMethod => _arrayWithObjectsMethod.GetValue(this);

    public IReadOnlyList<Expr> Elements => _elements;

    public uint NumElements => NumChildren;

    private static unsafe ObjCMethodDecl ArrayWithObjectsMethodFactory(ObjCArrayLiteral self) => self.TranslationUnit.GetOrCreate<ObjCMethodDecl>(self.Handle.Referenced);
}
