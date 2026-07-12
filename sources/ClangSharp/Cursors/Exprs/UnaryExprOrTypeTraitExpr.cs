// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class UnaryExprOrTypeTraitExpr : Expr
{
    private ValueLazy<UnaryExprOrTypeTraitExpr, Expr> _argumentExpr;
    private ValueLazy<UnaryExprOrTypeTraitExpr, Type> _argumentType;

    internal unsafe UnaryExprOrTypeTraitExpr(CXCursor handle) : base(handle, CXCursor_UnaryExpr, CX_StmtClass_UnaryExprOrTypeTraitExpr)
    {
        _argumentExpr = new ValueLazy<UnaryExprOrTypeTraitExpr, Expr>(&ArgumentExprFactory);
        _argumentType = new ValueLazy<UnaryExprOrTypeTraitExpr, Type>(&ArgumentTypeFactory);
    }

    public Expr ArgumentExpr => _argumentExpr.GetValue(this);

    public Type ArgumentType => _argumentType.GetValue(this);

    public bool IsArgumentType => Handle.IsArgumentType;

    public CX_UnaryExprOrTypeTrait Kind => Handle.UnaryExprOrTypeTraitKind;

    public Type TypeOfArgument => IsArgumentType ? ArgumentType : ArgumentExpr.Type;

    private static unsafe Type ArgumentTypeFactory(UnaryExprOrTypeTraitExpr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ArgumentType);

    private static unsafe Expr ArgumentExprFactory(UnaryExprOrTypeTraitExpr self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.SubExpr);
}
