// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXPseudoDestructorExpr : Expr
{
    private readonly Lazy<Type> _destroyedType;

    internal CXXPseudoDestructorExpr(CXCursor handle) : base(handle, CXCursor_MemberRefExpr, CX_StmtClass_CXXPseudoDestructorExpr)
    {
        Debug.Assert(NumChildren is 1);
        _destroyedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
    }

    public Expr Base => (Expr)Children[0];

    public Type DestroyedType => _destroyedType.Value;

    public bool IsArrow => Handle.IsArrow;
}
