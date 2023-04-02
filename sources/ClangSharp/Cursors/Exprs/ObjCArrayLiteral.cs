// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCArrayLiteral : Expr
{
    private readonly Lazy<ObjCMethodDecl> _arrayWithObjectsMethod;
    private readonly Lazy<IReadOnlyList<Expr>> _elements;

    internal ObjCArrayLiteral(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_ObjCArrayLiteral)
    {
        _arrayWithObjectsMethod = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.Referenced));
        _elements = new Lazy<IReadOnlyList<Expr>>(() => Children.Cast<Expr>().ToList());
    }

    public ObjCMethodDecl ArrayWithObjectsMethod => _arrayWithObjectsMethod.Value;

    public IReadOnlyList<Expr> Elements => _elements.Value;

    public uint NumElements => NumChildren;
}
