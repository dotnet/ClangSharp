// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class UnresolvedMemberExpr : OverloadExpr
{
    private ValueLazy<UnresolvedMemberExpr, Type> _baseType;

    internal unsafe UnresolvedMemberExpr(CXCursor handle) : base(handle, CXCursor_MemberRefExpr, CX_StmtClass_UnresolvedMemberExpr)
    {
        Debug.Assert(NumChildren is 0 or 1);
        _baseType = new ValueLazy<UnresolvedMemberExpr, Type>(&BaseTypeFactory);
    }

    public Expr? Base => (Expr?)Children.SingleOrDefault();

    public Type BaseType => _baseType.GetValue(this);

    public bool IsImplicitAccess => NumChildren == 0;

    public bool IsArrow => Handle.IsArrow;

    public string MemberName => Handle.Name.CString;

    private static unsafe Type BaseTypeFactory(UnresolvedMemberExpr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);
}
