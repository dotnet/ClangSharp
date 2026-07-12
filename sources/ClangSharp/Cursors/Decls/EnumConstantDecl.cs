// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class EnumConstantDecl : ValueDecl, IMergeable<EnumConstantDecl>
{
    private ValueLazy<EnumConstantDecl, Expr?> _initExpr;

    internal unsafe EnumConstantDecl(CXCursor handle) : base(handle, CXCursor_EnumConstantDecl, CX_DeclKind_EnumConstant)
    {
        _initExpr = new ValueLazy<EnumConstantDecl, Expr?>(&InitExprFactory);
    }

    public new EnumConstantDecl CanonicalDecl => (EnumConstantDecl)base.CanonicalDecl;

    public Expr? InitExpr => _initExpr.GetValue(this);

    public long InitVal => Handle.EnumConstantDeclValue;

    public ulong UnsignedInitVal => Handle.EnumConstantDeclUnsignedValue;

    public bool IsNegative => Handle.IsNegative;

    public bool IsNonNegative => Handle.IsNonNegative;

    public bool IsSigned => Handle.IsSigned;

    public bool IsStrictlyPositive => Handle.IsStrictlyPositive;

    public bool IsUnsigned => Handle.IsUnsigned;

    private static unsafe Expr? InitExprFactory(EnumConstantDecl self) => !self.Handle.InitExpr.IsNull ? self.TranslationUnit.GetOrCreate<Expr>(self.Handle.InitExpr) : null;
}
