// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXPseudoDestructorExpr : Expr
{
    private ValueLazy<CXXPseudoDestructorExpr, Type> _destroyedType;

    internal unsafe CXXPseudoDestructorExpr(CXCursor handle) : base(handle, CXCursor_MemberRefExpr, CX_StmtClass_CXXPseudoDestructorExpr)
    {
        Debug.Assert(NumChildren is 1);
        _destroyedType = new ValueLazy<CXXPseudoDestructorExpr, Type>(&DestroyedTypeFactory);
    }

    public Expr Base => (Expr)Children[0];

    public Type DestroyedType => _destroyedType.GetValue(this);

    public bool IsArrow => Handle.IsArrow;

    private static unsafe Type DestroyedTypeFactory(CXXPseudoDestructorExpr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);
}
