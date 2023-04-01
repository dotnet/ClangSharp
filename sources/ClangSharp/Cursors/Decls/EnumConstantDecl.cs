// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class EnumConstantDecl : ValueDecl, IMergeable<EnumConstantDecl>
{
    private readonly Lazy<Expr?> _initExpr;

    internal EnumConstantDecl(CXCursor handle) : base(handle, CXCursor_EnumConstantDecl, CX_DeclKind_EnumConstant)
    {
        _initExpr = new Lazy<Expr?>(() => !Handle.InitExpr.IsNull ? TranslationUnit.GetOrCreate<Expr>(Handle.InitExpr) : null);
    }

    public new EnumConstantDecl CanonicalDecl => (EnumConstantDecl)base.CanonicalDecl;

    public Expr? InitExpr => _initExpr.Value;

    public long InitVal => Handle.EnumConstantDeclValue;

    public ulong UnsignedInitVal => Handle.EnumConstantDeclUnsignedValue;

    public bool IsNegative => Handle.IsNegative;

    public bool IsNonNegative => Handle.IsNonNegative;

    public bool IsSigned => Handle.IsSigned;

    public bool IsStrictlyPositive => Handle.IsStrictlyPositive;

    public bool IsUnsigned => Handle.IsUnsigned;
}
